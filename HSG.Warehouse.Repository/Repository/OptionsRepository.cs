using HSG.Warehouse.Common.Models.Entity;
using HSG.Warehouse.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HSG.Warehouse.Repository.Repository
{
    public class OptionsRepository : IOptionsRepository
    {
        private readonly AppDbContext _context;

        public OptionsRepository(AppDbContext context)
        {
            _context = context;
        }
        #region ВИРОБНИКИ / БРЕНДИ
        public async Task<IEnumerable<Manufacturer?>?> GetAllManufacturerAsync()
        {
            return await _context.Manufacturers.Where(m => m.IsDeleted == false).ToListAsync();
        }
        public Task<Manufacturer?> GetManufacturerByIdAsync(int manufacturerId)
        {
            return _context.Manufacturers.Where(m => m.IsDeleted == false && m.Id == manufacturerId).FirstOrDefaultAsync();
        }
        public Task<Manufacturer?> GetManufacturerByNameAsync(string name)
        {
            return _context.Manufacturers.Where(m => m.IsDeleted == false && m.Name == name).FirstOrDefaultAsync();
        }
        public async Task<string?> AddManufacturerAsync(Manufacturer? manufacturer)
        {
            if (string.IsNullOrWhiteSpace(manufacturer?.Name)) return "Введіть значення!";

            manufacturer.Name = manufacturer.Name.Trim();

            //Шукаю, чи є вже такий виробник
            var manufactererUpdate = _context.Manufacturers.Where(m => m.Name == manufacturer.Name).FirstOrDefault();
            if (manufactererUpdate != null)
            {
                //Якщо він видалений, то роблю його видимим
                if (!manufactererUpdate.IsDeleted)
                {
                    return "Такі дані вже є в базі!";
                }
                else
                {
                    manufactererUpdate.IsDeleted = false;
                    _context.Manufacturers.Update(manufactererUpdate);
                    await _context.SaveChangesAsync();
                    return null;
                }
            }

            _context.Manufacturers.Add(manufacturer);
            await _context.SaveChangesAsync();

            return null;
        }
        public async Task<string?> DeleteManufacturerAsync(int manufacturerId)
        {
            Manufacturer? manufacturer = await GetManufacturerByIdAsync(manufacturerId);

            if (manufacturer == null)
            {
                return "Нема даних для видалення!";
            }
            if (manufacturer.SystemField)
            {
                return "Змінювати ці дані заборонено!";

            }
            manufacturer.IsDeleted = false;
            _context.Manufacturers.Update(manufacturer);
            await _context.SaveChangesAsync();

            return null;
        }
        public async Task<string?> EditManufacturerAsync(Manufacturer? manufacturer)
        {
            if (string.IsNullOrWhiteSpace(manufacturer?.Name)) return "Введіть значення!";
            manufacturer.Name = manufacturer.Name.Trim();
            if (manufacturer.SystemField)
            {
                return "Змінювати ці дані заборонено!";

            }
            manufacturer.Name = manufacturer.Name.Trim();

            // Перевріяю, чи є вже такі дані у базі
            var manufacturerUpdate = _context.Manufacturers.Where(m => m.Name == manufacturer.Name).FirstOrDefault();

            if (manufacturerUpdate != null)
            {
                if (!manufacturerUpdate.IsDeleted)
                {
                    return "Такі дані вже є у базі!";
                }
                else
                {
                    //Якщо такий виробний є але видалений, тоді роблю його "видимим" а поточний "приховую"
                    manufacturerUpdate.IsDeleted = false;
                    manufacturer.IsDeleted = true;
                    _context.Manufacturers.Update(manufacturer);
                    _context.Manufacturers.Update(manufacturerUpdate);
                    await _context.SaveChangesAsync();
                    return null;
                }
            }

            _context.Manufacturers.Update(manufacturer);
            await _context.SaveChangesAsync();

            return null;
        }
        #endregion
        #region ОДИНИЦІ ВИМІРУ
        public async Task<string?> AddUnitAsync(Unit? unit)
        {
            if (string.IsNullOrWhiteSpace(unit.Name)) return "Введіть значення!";

            unit.Name = unit.Name.Trim();

            //Перевіряю чи є вже така одиниця виміру і чи вони не "видалена"
            if (_context.Units.Where(u => u.Name.ToLower() == unit.Name.ToLower() && u.IsDeleted == false).Count() == 0)
            {
                // Перевіряю чи не "видалена вона"
                Unit? unitToUpdate = _context.Units.Where(u => u.Name.ToLower() == unit.Name.ToLower() && u.IsDeleted == true).FirstOrDefault();
                if (unitToUpdate != null)
                {
                    unitToUpdate.IsDeleted = false;
                    _context.Units.Update(unitToUpdate);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    _context.Units.Add(unit);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                //Така одиниця є і вона видима, нічого не роблю.
                return "Така одиниця виміру вже є!";
            }

            return null;
        }
        public Task<Unit?> GetUnitByIdAsync(int unitId)
        {
            return _context.Units.Where(u => u.Id == unitId && u.IsDeleted == false).FirstOrDefaultAsync();
        }
        public Task<Unit?> GetUnitByNameAsync(string name)
        {
            return _context.Units.Where(u => u.Name == name && u.IsDeleted == false).FirstOrDefaultAsync();
        }
        public async Task<string?> DeleteUnitAsync(int unitId)
        {
            //Перевіряю, чи є такі дані
            Unit? unit = await GetUnitByIdAsync(unitId);

            if (unit != null)
            {
                //Системне поле змінювати не можна
                if (unit.SystemField)
                {
                    return "Змінювати ці дані заборонено!";
                }

                unit.IsDeleted = true;
                _context.Units.Update(unit);
                await _context.SaveChangesAsync();

                return null;
            }
            return "Нема даних для видалення!";
        }
        public async Task<IEnumerable<Unit?>?> GetAllUnitsAsync()
        {
            return await _context.Units
                .Where(u => u.IsDeleted == false)
                .OrderBy(u => u.Name)
                .ToListAsync();
        }
        public async Task<string?> EditUnitAsync(Unit? unit)
        {
            if (string.IsNullOrWhiteSpace(unit.Name)) return "Введіть значення!";
            unit.Name = unit.Name.Trim();
            //Перевіряю чи є така ІД
            Unit? updateUnit = await GetUnitByIdAsync(unit.Id);
            if (updateUnit != null)
            {
                //Системне поле змінювати не можна
                if (updateUnit.SystemField)
                {
                    return "Змінювати ці дані заборонено!";
                }

                // Шукаю таку назву, може вона вже є у базі
                Unit? findeUnitName = await _context.Units.Where(u => u.Name == unit.Name).FirstOrDefaultAsync();
                if (findeUnitName != null)
                {
                    // Якщо такі дані вже є, то перевіряю чи не "видалена" ця одиниця виміру
                    if (findeUnitName.IsDeleted == false)
                    {
                        return "Така одиниця виміру вже є";
                    }
                    else
                    {
                        //Роблю цю одиницю виміру знов "видимою" а поточню "ховаю"
                        findeUnitName.IsDeleted = false;
                        _context.Units.Update(findeUnitName);
                        updateUnit.IsDeleted = true;
                        _context.Units.Update(updateUnit);
                        await _context.SaveChangesAsync();

                        return null;
                    }
                }
                else
                {
                    updateUnit.Name = unit.Name;
                    _context.Units.Update(unit);
                    await _context.SaveChangesAsync();
                    return null;
                }
            }

            return "Нема даних для редагування!";
        }
        #endregion
        #region ВАЛЮТИ
        public async Task<IEnumerable<Currency?>?> GetAllCurrencyAsync()
        {
            return await _context.Currency.Where(c => c.IsDeleted == false).ToListAsync();
        }
        public async Task<string?> DeleteCurrencyAsync(int currencyId)
        {
            //Перевіряю, чи є такі дані
            var currency = _context.Currency.Where(c => c.Id == currencyId).FirstOrDefault();

            if (currency == null)
            {
                return "Відсутні дані для видалення";
            }

            //Якщо це системне поле, видаляти не можна
            if (currency.SystemField == true)
            {
                return "Змінювати ці дані заборонено!";
            }
            currency.IsDeleted = true;
            _context.Currency.Update(currency);
            await _context.SaveChangesAsync();

            return null;
        }
        public async Task<string?> AddCurrencyAsync(Currency? currency)
        {
            if (currency == null)
            {
                return "Виберіть валюту";
            }
            //перевіряю, чи є вже така валюта
            var addCurrency = _context.Currency.Where(c => c.Code == currency.Code).FirstOrDefault();
            //Якщо така валюта є і вона видалена, то роблюю її видимою
            if (addCurrency != null)
            {
                if (addCurrency.IsDeleted == true)
                {
                    addCurrency.IsDeleted = false;
                    _context.Currency.Update(addCurrency);
                    await _context.SaveChangesAsync();

                    return null;
                }
                else
                {
                    return "Така валюта вже є у базі!";
                }
            }

            _context.Currency.Add(currency);
            await _context.SaveChangesAsync();

            return null;
        }
        #endregion
        #region КАТЕГОРІЇ
        public async Task<IEnumerable<Category?>?> GetAllCategoriesAsync()
        {
            return await _context.Categories
                .Where(c => c.IsDeleted == false)
                .OrderBy(c => c.Name).ToListAsync();
        }
        public Task<Category?> GetCategoryByIdAsync(int categoryId)
        {
            return _context.Categories.Where(c => c.Id == categoryId && c.IsDeleted == false).FirstOrDefaultAsync();
        }
        public async Task<string?> DeleteCategoryAsync(int categoryId)
        {
            //Перевіряю, чи є такі дані
            Category? category = await GetCategoryByIdAsync(categoryId);
            if (category != null)
            {
                //Системне поле змінювати не можна
                if (category.SystemField)
                {
                    return "Змінювати ці дані заборонено!";
                }

                //Якщо у цій категорії є товар, то її видаляти не можна
                var productsInCategory = await _context.Warehouse
                    .Include(p => p.Product)
                    .Include(c => c.Product.Category)
                    .Where(c => c.Product.Category.Id == categoryId).CountAsync();
                if (productsInCategory > 0)
                {
                    return "У цій категорії є товар! Видалення не можливе!";
                }
                category.IsDeleted = true;
                _context.Categories.Update(category);
                await _context.SaveChangesAsync();

                return null;
            }
            return "Нема даних для видалення";
        }
        public async Task<string?> EditCategoryAsync(Category? category)
        {
            if (string.IsNullOrWhiteSpace(category?.Name)) return "Введіть значення!";
            category.Name = category.Name.Trim();
            //Перевіряю чи є така ІД
            var updateCategory = await GetCategoryByIdAsync(category.Id);

            if (updateCategory != null)
            {
                //Системне поле змінювати не можна
                if (updateCategory.SystemField)
                {
                    return "Змінювати ці дані заборонено!";
                }

                updateCategory.Name = category.Name;
                _context.Categories.Update(category);
                await _context.SaveChangesAsync();
                return null;
            }
            return "Нема даних для редагування!";
        }
        public async Task<string?> AddCategoryAsync(Category? category)
        {
            if (string.IsNullOrWhiteSpace(category?.Name)) return "Введіть значення";
            category.Name = category.Name.Trim();

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return null;
        }
        #endregion
        #region ПРОДУКЦІЯ
        public string GenerateBarcode()
        {
            // EAN13
            Random rnd = new Random();
            string bareCode = "200";
            int sum1 = 0;
            int sum2 = 0;
            int crc;

            bareCode += rnd.Next(100000000, 999999999).ToString();
            for (int i = 0; i < 12; i++)
            {
                sum1 += Convert.ToInt32(bareCode[i].ToString());
                sum2 += Convert.ToInt32(bareCode[++i].ToString());
            }
            crc = (sum1 * 3 + sum2) % 10;
            if (crc != 0)
                crc = 10 - crc;
            bareCode += crc.ToString();

            return bareCode;
        }
        public async Task<IEnumerable<Product?>?> GetProductsFromCategoryAsync(int categoryId)
        {
            return await _context.Products
                .Include(u => u.Unit)
                .Include(c => c.Category)
                .Include(m => m.Manufacturer)
                .Where(p => p.IsDeleted == false)
                .Where(p => p.CategoryId == categoryId || categoryId == 0)
                .OrderBy(o => o.Category.Name)
                .ThenBy(o => o.Manufacturer.Name)
                .ThenBy(o => o.Name)
                .ToListAsync();
        }
        public async Task<string?> AddProductAsync(Product? product, bool isAutoBarcode)
        {
            string? result = null;
            if (product?.CategoryId == 0) result += "Виберіть категорію ";
            if (product?.ManufacturerId == 0) result += "Виберіть виробника ";
            if (product?.UnitId == 0) result += "Виберіть одиниці виміру ";
            if (product?.Price <= 0) result += "Введіть ціну ";
            if (string.IsNullOrEmpty(product?.Name))
            {
                result += "Введіть назву ";
            }
            else
            {
                product.Name = product.Name.Trim();
            }

            if (result != null)
            {
                return result;
            }
            //Генерую новий штрих код
            if (isAutoBarcode)
            {
                string bareCode = GenerateBarcode();
                while (_context.Products.Where(p => p.Barcode == bareCode).Count() > 0)
                {
                    bareCode = GenerateBarcode();
                }
                product.Barcode = bareCode;
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return null;
        }
        public async Task<string?> DeleteProductAsync(int productId)
        {
            //Перевіряю чи є дані для видалення
            var product = _context.Products.Where(p => p.Id == productId && p.IsDeleted == false).FirstOrDefault();
            if (product == null)
            {
                return "Нема даних для видалення!";
            }
            if (product.SystemField)
            {
                return "Змінювати ці дані заборонено!";
            }
            product.IsDeleted = true;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return null;
        }
        public Task<Product?> GetProductByIdAsync(int productId)
        {
            return _context.Products.Where(p => p.Id == productId && p.IsDeleted == false).FirstOrDefaultAsync();
        }
        public async Task<string?> EditProductAsync(Product? product)
        {
            string? result = null;
            if (product?.CategoryId == 0) result += "Виберіть категорію ";
            if (product?.ManufacturerId == 0) result += "Виберіть виробника ";
            if (product?.UnitId == 0) result += "Виберіть одиниці виміру ";
            if (_context.Products.Where(p => p.Barcode == product.Barcode && p.Id != product.Id).Count() > 0)
            {
                result += "Такий штрих код вже є ";
            }

            if (string.IsNullOrEmpty(product?.Name))
            {
                result += "Введіть назву ";
            }
            else
            {
                product.Name = product.Name.Trim();
            }

            if (result != null)
            {
                return result;
            }

            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return null;
        }
        #endregion
        #region КЛІЄНТИ
        public async Task<IEnumerable<Client?>?> GetAllClientsAsync()
        {
            var list = await _context.Clients
                .Where(c => c.IsDeleted == false)
                .OrderBy(o => o.LastName)
                .ThenBy(o => o.FirstName)
                .ToListAsync();

            return list;
        }
        public Task<Client?> GetClientByIdAsync(int clientId)
        {
            return _context.Clients.Where(c => c.Id == clientId && c.IsDeleted == false).FirstOrDefaultAsync();
        }
        public async Task<string?> AddClientAsync(Client? client)
        {
            string? errors = null;
            if (string.IsNullOrEmpty(client?.FirstName))
            {
                errors += "Введіть і'мя ";
            }
            if (string.IsNullOrEmpty(client?.LastName))
            {
                errors += "Введіть прізвище ";
            }

            if (errors != null)
            {
                return errors;
            }

            client.FirstName = client.FirstName?.Trim();
            client.LastName = client.LastName?.Trim();
            client.MiddleName = client.MiddleName?.Trim();
            client.Phone = client.Phone?.Trim();
            client.Address = client.Address?.Trim();

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return errors;
        }
        public async Task<string?> EditClientAsync(Client client)
        {
            string? errors = null;
            if (string.IsNullOrEmpty(client.FirstName))
            {
                errors += "Введіть і'мя ";
            }
            if (string.IsNullOrEmpty(client.LastName))
            {
                errors += "Введіть прізвище ";
            }

            if (errors != null)
            {
                return errors;
            }

            client.FirstName = client.FirstName?.Trim();
            client.LastName = client.LastName?.Trim();
            client.MiddleName = client.MiddleName?.Trim();
            client.Phone = client.Phone?.Trim();
            client.Address = client.Address?.Trim();

            _context.Clients.Update(client);
            await _context.SaveChangesAsync();

            return errors;

        }
        public async Task<string?> DeleteClientAsync(int clientId)
        {
            var client = await GetClientByIdAsync(clientId);
            if (client == null)
            {
                return "Нема даних!";
            }
            if (client.SystemField)
            {
                return "Змінувати ці дані заборонено!";
            }

            client.IsDeleted = true;
            _context.Clients.Update(client);
            await _context.SaveChangesAsync();

            return null;
        }
        #endregion
    }
}
