using HSG.Warehouse.Common.Models.Report;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HSG.Warehouse.Interfaces.Repository
{
    public interface IReportRepository
    {
        Task<IEnumerable<SaleStats?>?> GetSaleStat();
    }
}
