using HSG.Warehouse.Common.Models.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HSG.Warehouse.Interfaces.Repository
{
    public interface IOptionsRepository
    {
        #region ОДИНИЦІ ВИМІРУ
        Task<IEnumerable<Unit?>?> GetAllUnitsAsync();
        Task<Unit?> GetUnitByIdAsync(int unitId);
        Task<Unit?> GetUnitByNameAsync(string name);
        Task<string?> AddUnitAsync(Unit? unit);
        Task<string?> EditUnitAsync(Unit? unit);
        Task<string?> DeleteUnitAsync(int unitId);
        #endregion
        #region ВАЛЮТИ
        Task<IEnumerable<Currency?>?> GetAllCurrencyAsync();
        Task<string?> DeleteCurrencyAsync(int currencyId);
        Task<string?> AddCurrencyAsync(Currency? currency);
        #endregion
        #region ВИРОБНИКИ / БРЕНДИ
        Task<IEnumerable<Manufacturer?>?> GetAllManufacturerAsync();
        Task<Manufacturer?> GetManufacturerByIdAsync(int currencyId);
        Task<Manufacturer?> GetManufacturerByNameAsync(string name);
        Task<string?> AddManufacturerAsync(Manufacturer? manufacturer);
        Task<string?> DeleteManufacturerAsync(int currencyId);
        Task<string?> EditManufacturerAsync(Manufacturer? manufacturer);

        #endregion
        #region КАТЕГОРІЇ
        Task<IEnumerable<Category?>?> GetAllCategoriesAsync();
        Task<string?> AddCategoryAsync(Category? category);
        Task<Category?> GetCategoryByIdAsync(int categoryId);
        Task<string?> DeleteCategoryAsync(int categoryId);
        Task<string?> EditCategoryAsync(Category? category);
        #endregion
        #region ПРОДУКЦІЯ
        string GenerateBarcode();
        Task<IEnumerable<Product?>?> GetProductsFromCategoryAsync(int categoryId);
        Task<string?> AddProductAsync(Product? product, bool isAutoBarcode);
        Task<Product?> GetProductByIdAsync(int productId);
        Task<string?> DeleteProductAsync(int productId);
        Task<string?> EditProductAsync(Product? product);
        #endregion
        #region КЛІЄНТИ
        Task<IEnumerable<Client?>?> GetAllClientsAsync();
        Task<Client?> GetClientByIdAsync(int clientId);
        Task<string?> AddClientAsync(Client? client);
        Task<string?> DeleteClientAsync(int clientId);
        Task<string?> EditClientAsync(Client? client);
        #endregion
    }
}
