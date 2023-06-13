using HSG.Warehouse.Common.Models.Entity;
using HSG.Warehouse.Interfaces.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HSG.Warehouse.Repository.Repository
{
    public class SeedDataRepository : ISeedData
    {
        private readonly AppDbContext _appDbContext;

        public SeedDataRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<string?> Seed()
        {
            try
            {
                //Одиниці виміру
                if (!_appDbContext.Units.Any())
                {
                    await _appDbContext.Units.AddRangeAsync(new List<Unit>()
                {
                   new Unit(){Name = "шт", SystemField = true},
                 new Unit(){Name = "м", SystemField = true}
                });
                }

                //Категорії
                if (!_appDbContext.Categories.Any())
                {
                    await _appDbContext.Categories.AddRangeAsync(new List<Category>()
                    {
                    new Category(){Name = "Головна", SystemField = false},
                    //new Category(){Name = "Головна 2", SystemField = true}
                });
                }

                //Валюти
                if (!_appDbContext.Currency.Any())
                {
                    await _appDbContext.Currency.AddRangeAsync(new List<Currency>()
                {
                    new Currency() { Name = "Гривня", Code = 980, ShortName = "UAH", SystemField = true},
                });
                }
                await _appDbContext.SaveChangesAsync();
                return null;
            }
            catch
            {
                return "Помилка!";
            }

        }
    }
}
