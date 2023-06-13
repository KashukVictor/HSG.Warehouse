using HSG.Warehouse.Common.Models;
using HSG.Warehouse.Common.Models.Entity;
using HSG.Warehouse.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace HSG.Warehouse.Repository.Repository
{
    public class SalesRepository : ISalesRepository
    {
        private readonly AppDbContext _context;
        private readonly IWarehouseRepository _warehouseRepository;

        public SalesRepository(AppDbContext context, IWarehouseRepository warehouseRepository)
        {
            _context = context;
            _warehouseRepository = warehouseRepository;
        }
        public async Task<IEnumerable<Sale?>?> GetAllAsync()
        {
            return await _context.Sales
                .Include(c => c.Client)
                .Include(cur => cur.Currency)
                .Where(s => s.IsDeleted == false)
                .ToListAsync();
        }
        public Task<Sale?> GetByIdAsync(int saleId)
        {
            return _context.Sales
                .Include(c => c.Client)
                .Include(cur => cur.Currency)
                .Where(i => i.Id == saleId).FirstOrDefaultAsync();
        }
        public Task<double?> GetSumByIdAsync(int saleId)
        {
            return _context.SaleDetails
                .Where(i => i.SaleId == saleId)
                .SumAsync(s => s.Amount * s.Price);
        }
        public async Task<string?> CreateAsync(Sale sale)
        {
            string? result = null;
            if (sale.ClientId == 0) result += "Виберіть клієнта ";
            if (sale.CurrencyId == 0) result += "Виберіть валюту ";
            if (string.IsNullOrEmpty(sale.Number))
            {
                result += "Введіть номер ";
            }
            else
            {
                sale.Number = sale.Number.Trim();
            }
            sale.Description = sale.Description?.Trim();

            if (result != null)
            {
                return result;
            }

            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();

            return null;
        }
        public async Task<IEnumerable<SaleDetail?>?> GetAllProductsFromSaleAsync(int saleId)
        {
            var list = await _context.SaleDetails
                .Include(i => i.Product)
                .Include(m => m.Product.Manufacturer)
                .Include(u => u.Product.Unit)
                .Where(i => i.IsDeleted == false)
                .Where(i => i.SaleId == saleId || saleId == 0)
                .ToListAsync();
            return list;
        }
        public async Task<IEnumerable<ProductForSale?>?> GetProductsForSaleAsync(int categoryId, int saleId)
        {
            var sale = await GetByIdAsync(saleId);

            var list = from productsInWarehouse in await _warehouseRepository.GetProductsAsync(categoryId, sale.CurrencyId)
                       join productsInSale in await GetAllProductsFromSaleAsync(saleId)
                       on productsInWarehouse.InvoiceDetailId equals productsInSale.InvoiceDetailId into productsList
                       from result in productsList.DefaultIfEmpty()
                       select new ProductForSale
                       {
                           SaleDetailId = result?.Id,
                           Warehouse = productsInWarehouse,
                           Amount = result?.Amount,
                           Price = result?.Price,
                           SaleId = saleId
                       };

            return list;
        }
        public async Task<int> GetSatusAsync(int saleId)
        {
            // 0 - Інвойс є і на стадії заповнення
            // 1 - Інвойс зафіксований (зміна і доповнення вже не доступна)
            // 2 - Такого інвойсу нема

            var sale = await _context.Sales.Where(s => s.Id == saleId).FirstOrDefaultAsync();
            if (sale == null) return 2;

            return sale.Fixed ? 1 : 0;

        }
        public async Task<string?> AddProductAsync(SaleDetail saleDetail)
        {
            //Перевіряю чи є така накладна і чи вона не зафіксована
            if (saleDetail == null || await GetSatusAsync(saleDetail.SaleId) != 0) return "Нема даних!";
            if (saleDetail.Amount <= 0 || saleDetail.Price <= 0) return "Введіть значення більше нуля!";

            // Перевіряю, чи є вже такий товар у цьому інвойсі, якщо є, то апдейчу, якщо нема, то додаю
            var product = _context.SaleDetails.Where(s => s.SaleId == saleDetail.SaleId && s.InvoiceDetailId == saleDetail.InvoiceDetailId).FirstOrDefault();
            if (product == null)
            {
                _context.SaleDetails.Add(saleDetail);
            }
            else
            {
                product.Amount = saleDetail.Amount;
                product.Price = saleDetail.Price;
                _context.SaleDetails.Update(product);
            }

            await _context.SaveChangesAsync();
            return null;
        }
        public async Task<string?> DeleteProductAsync(int saleDetailId)
        {
            var saleDetail = await _context.SaleDetails
                .Include(s => s.Sale)
                .Where(s => s.Id == saleDetailId && s.Sale.Fixed == false)
                .FirstOrDefaultAsync();

            //Перевіряю чи є такий інвойс і чи він не зафіксований            
            if (saleDetail == null) return "Нема даних!";

            //var deleteProduct = await _context.SaleDetails.Where(s => s.SaleId == saleDetail.SaleId && s.Id == saleDetail.Id).FirstOrDefaultAsync();

            _context.SaleDetails.Remove(saleDetail);
            await _context.SaveChangesAsync();

            return null;
        }
        public async Task<string?> SaveAsync(int saleId)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    //Перевріюя чи є така накладна
                    var sale = await _context.Sales.Where(s => s.Id == saleId && s.Fixed == false).FirstOrDefaultAsync();
                    if (sale == null)
                    {
                        return "Немає даних!";
                    }

                    //Перелік товарів, який необхідо списати зі складу

                    var list = from productsInSale in await GetAllProductsFromSaleAsync(saleId)
                               join productsInWarehouse in await _warehouseRepository.GetProductsAsync(0, sale.CurrencyId)
                               on productsInSale.InvoiceDetailId equals productsInWarehouse.InvoiceDetailId into productsList
                               from result in productsList.DefaultIfEmpty()
                               select new Common.Models.Entity.Warehouse
                               {
                                   Id = result.Id,
                                   DateInsert = result.DateInsert,
                                   SystemField = result.SystemField,
                                   IsDeleted = result.IsDeleted,
                                   ProductId = result.ProductId,
                                   InvoiceDetailId = result.InvoiceDetailId,
                                   Amount = result.Amount - productsInSale.Amount,
                                   Price = result.Price
                               };

                    var count = list.Where(s => s.Amount < 0).Count();
                    if (count > 0)
                    {
                        return "На складі не вистачає продукції";
                    }

                    _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;

                    _context.Warehouse.UpdateRange(list);

                    //Видаляю всі товари з складу, що == 0 (яких не залишилось)
                    var delete = await _context.Warehouse.Where(c => c.Amount <= 0).ToListAsync();
                    _context.Warehouse.RemoveRange(delete);
                    sale.Fixed = true;
                    _context.Sales.Update(sale);

                    _context.SaveChanges();

                    // Товар, який залишився набраний у накладний, але який вже видалений зі складу.
                    var productsInWareshouse = await _warehouseRepository.GetProductsAsync(0, 0);
                    var productsInSaleDetail = await _context.SaleDetails
                        .Include(s => s.Sale)
                        .Where(s => s.Sale.Fixed == false)
                        .ToListAsync();

                    var deleteFromSaleDetail = (from productsInSale in productsInSaleDetail
                                                join productsInWarehouse in productsInWareshouse
                                                on productsInSale.InvoiceDetailId equals productsInWarehouse.InvoiceDetailId into productsList
                                                from result in productsList.DefaultIfEmpty()
                                                where result == null || result.Amount == 0
                                                select productsInSale).ToList();

                    _context.SaleDetails.RemoveRange(deleteFromSaleDetail);
                    _context.SaveChanges();


                    transaction.Commit();

                    _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return "Виникла помилка";
                }
            }

            return null;
        }
        public async Task<string?> DeleteAsync(int saleId)
        {
            //На даний час, перевіряю щоб накладна була не зафіксована
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                var sale = await _context.Sales.Where(i => i.Id == saleId && i.Fixed == false).FirstOrDefaultAsync();
                if (sale != null)
                {
                    try
                    {
                        var saleDetail = await _context.SaleDetails.Where(i => i.SaleId == saleId).ToListAsync();

                        _context.Sales.Remove(sale);
                        _context.SaleDetails.RemoveRange(saleDetail);

                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        return null;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return "Виникла помилка!";
                    }
                }

            }

            return "Нема даних для видалення!";
        }
        public async Task<string?> EditAsync(Sale sale)
        {
            string? result = null;
            if (sale.ClientId == 0) result += "Виберіть клієнта ";
            if (sale.CurrencyId == 0) result += "Виберіть валюту ";
            if (string.IsNullOrEmpty(sale.Number))
            {
                result += "Введіть номер ";
            }
            else
            {
                sale.Number = sale.Number.Trim();
            }
            sale.Description = sale.Description?.Trim();

            if (result != null)
            {
                return result;
            }

            _context.Sales.Update(sale);
            await _context.SaveChangesAsync();

            return null;
        }
    }
}
