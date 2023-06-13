using System.Collections.Generic;
using System.Threading.Tasks;

namespace HSG.Warehouse.Interfaces.Repository
{
    public interface IWarehouseRepository
    {
        Task<IEnumerable<Common.Models.Entity.Warehouse?>?> GetProductsAsync(int categoryId, int currencId);

    }
}
