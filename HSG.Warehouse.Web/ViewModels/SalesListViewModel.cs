
using HSG.Warehouse.Common.Models.Entity;

namespace HSG.Warehouse.Web.ViewModels
{
    public class SalesListViewModel
    {
        public IEnumerable<Sale?>? Sales { get; set; }
        public double? Sum { get; set; }

    }
}
