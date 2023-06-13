using HSG.Warehouse.Common.Models;
using HSG.Warehouse.Common.Models.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HSG.Warehouse.Interfaces.Repository
{
    public interface ISalesRepository
    {
        Task<IEnumerable<Sale?>?> GetAllAsync();
        Task<string?> CreateAsync(Sale sale);
        Task<string?> EditAsync(Sale sale);
        Task<string?> DeleteAsync(int saleId);
        Task<IEnumerable<ProductForSale?>?> GetProductsForSaleAsync(int categoryId, int saleId);
        Task<IEnumerable<SaleDetail?>?> GetAllProductsFromSaleAsync(int saleId);
        Task<string?> AddProductAsync(SaleDetail saleDetail);
        Task<int> GetSatusAsync(int saleId);
        Task<double?> GetSumByIdAsync(int saleId);
        Task<string?> DeleteProductAsync(int saleDetailId);
        Task<string?> SaveAsync(int saleId);
        Task<Sale?> GetByIdAsync(int saleId);

    }
}
