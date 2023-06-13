using HSG.Warehouse.Common.Models;
using HSG.Warehouse.Common.Models.Entity;
using HSG.Warehouse.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HSG.Warehouse.Repository.Repository
{
    public class InvoicesRepository : IInvoicesRepository
    {
        private readonly AppDbContext _context;
        private readonly IOptionsRepository _optionsRepository;

        public InvoicesRepository(AppDbContext context, IOptionsRepository optionsRepository)
        {
            _context = context;
            _optionsRepository = optionsRepository;
        }
        public async Task<IEnumerable<InvoiceDetail?>?> GetAllProductsFromInvoiceAsync(int invoiceId)
        {
            return await _context.InvoiceDetails
                .Include(i => i.Product)
                .Include(m => m.Product.Manufacturer)
                .Include(u => u.Product.Unit)
                .Where(i => i.IsDeleted == false)
                .Where(i => i.InvoiceId == invoiceId || invoiceId == 0)
                .ToListAsync();
        }
        public async Task<IEnumerable<ProductToInvoice?>?> GetProductToInvoices(int categoryId, int invoiceId)
        {
            var list = from products in await _optionsRepository.GetProductsFromCategoryAsync(categoryId)
                       orderby products.Category.Name, products.Manufacturer.Name descending, products.Name
                       join productsInInvoice in await GetAllProductsFromInvoiceAsync(invoiceId)
                       on products.Id equals productsInInvoice.ProductId into productsList
                       from result in productsList.DefaultIfEmpty()
                       select new ProductToInvoice
                       {
                           Product = products,
                           Amount = result?.Amount,
                           Price = result?.Price,
                           InvoiceId = invoiceId
                       };

            return list;
        }
        public async Task<IEnumerable<Invoice?>?> GetAllAsync()
        {
            return await _context.Invoices
                .Include(c => c.Client)
                .Include(cur => cur.Currency)
                .Where(i => i.IsDeleted == false)
                .ToListAsync();
        }
        public async Task<string?> CreateAsync(Invoice invoice)
        {
            string? result = null;
            if (invoice.ClientId == 0) result += "Виберіть клієнта ";
            if (invoice.CurrencyId == 0) result += "Виберіть валюту ";
            if (string.IsNullOrEmpty(invoice.Number))
            {
                result += "Введіть номер ";
            }
            else
            {
                invoice.Number = invoice.Number.Trim();
            }
            invoice.Description = invoice.Description?.Trim();

            if (result != null)
            {
                return result;
            }

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return null;
        }
        public async Task<string?> EditAsync(Invoice invoice)
        {
            string? result = null;
            if (invoice.ClientId == 0) result += "Виберіть клієнта ";
            if (invoice.CurrencyId == 0) result += "Виберіть валюту ";
            if (string.IsNullOrEmpty(invoice.Number))
            {
                result += "Введіть номер ";
            }
            else
            {
                invoice.Number = invoice.Number.Trim();
            }
            invoice.Description = invoice.Description?.Trim();

            if (result != null)
            {
                return result;
            }

            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync();

            return null;
        }
        public async Task<string?> DeleteAsync(int invoiceId)
        {
            //Щоб видалити накладну, треба щоб вона не була зафіксована            
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                var invoice = await _context.Invoices.Where(i => i.Id == invoiceId && i.Fixed == false).FirstOrDefaultAsync();
                if (invoice != null)
                {
                    try
                    {
                        var invoiceDetail = await _context.InvoiceDetails.Where(i => i.InvoiceId == invoiceId).ToListAsync();

                        _context.Invoices.Remove(invoice);
                        _context.InvoiceDetails.RemoveRange(invoiceDetail);

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
        public async Task<int> GetSatusAsync(int invoiceId)
        {
            // 0 - Інвойс є і на стадії заповнення
            // 1 - Інвойс зафіксований (зміна і доповнення вже не доступна)
            // 2 - Такого інвойсу нема

            var invoice = await _context.Invoices.Where(i => i.Id == invoiceId).FirstOrDefaultAsync();
            if (invoice == null) return 2;

            return invoice.Fixed ? 1 : 0;
        }
        public async Task<string?> AddProductAsync(InvoiceDetail invoiceDetail)
        {
            //Перевіряю чи є такий інвойс і чи він не зафіксований
            if (invoiceDetail == null || await GetSatusAsync(invoiceDetail.InvoiceId) != 0) return "Нема даних!";
            if (invoiceDetail.Amount <= 0 || invoiceDetail.Price <= 0) return "Введіть значення більше нуля!";

            // Перевіряю, чи є вже такий товар у цьому інвойсі, якщо є, то апдейчу, якщо нема, то додаю
            var product = _context.InvoiceDetails.Where(i => i.InvoiceId == invoiceDetail.InvoiceId && i.ProductId == invoiceDetail.ProductId).FirstOrDefault();
            if (product == null)
            {
                _context.InvoiceDetails.Add(invoiceDetail);
            }
            else
            {
                product.Amount = invoiceDetail.Amount;
                product.Price = invoiceDetail.Price;
                _context.InvoiceDetails.Update(product);
            }

            await _context.SaveChangesAsync();
            return null;
        }
        public async Task<string?> DeleteProductAsync(InvoiceDetail invoiceDetail)
        {
            //Перевіряю чи є такий інвойс і чи він не зафіксований            
            if (invoiceDetail == null || await GetSatusAsync(invoiceDetail.InvoiceId) != 0) return "Нема даних!";

            var deleteProduct = await _context.InvoiceDetails.Where(i => i.InvoiceId == invoiceDetail.InvoiceId && i.ProductId == invoiceDetail.ProductId).FirstOrDefaultAsync();
            if (deleteProduct != null)
            {
                _context.InvoiceDetails.Remove(deleteProduct);
                await _context.SaveChangesAsync();
                return null;
            }

            return "Нема даних!";
        }
        public Task<Invoice?> GetByIdAsync(int invoiceId)
        {
            return _context.Invoices
                .Include(c => c.Client)
                .Include(cur => cur.Currency)
                .Where(i => i.Id == invoiceId).FirstOrDefaultAsync();
        }
        public Task<double> GetSumByIdAsync(int invoiceId)
        {
            return _context.InvoiceDetails
                .Where(i => i.InvoiceId == invoiceId)
                .SumAsync(s => s.Amount * s.Price);
        }
        public async Task<string?> SaveAsync(int invoiceId)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    //Перевріюя чи є такий інвойс
                    var invoice = await _context.Invoices.Where(i => i.Id == invoiceId && i.Fixed == false).FirstOrDefaultAsync();
                    if (invoice == null)
                    {
                        return "Немає даних!";
                    }

                    //Перелік товарів, який необхідо додати на склад
                    var list = from productsInInvoice in await GetAllProductsFromInvoiceAsync(invoiceId)
                               select new Common.Models.Entity.Warehouse
                               {
                                   Amount = productsInInvoice.Amount,
                                   Price = productsInInvoice.Price,
                                   ProductId = productsInInvoice.ProductId,
                                   InvoiceDetailId = productsInInvoice.Id
                               };

                    _context.Warehouse.AddRange(list);
                    invoice.Fixed = true;
                    _context.Invoices.Update(invoice);
                    _context.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return "Виникла помилка";
                }
            }

            return null;
        }
    }
}
