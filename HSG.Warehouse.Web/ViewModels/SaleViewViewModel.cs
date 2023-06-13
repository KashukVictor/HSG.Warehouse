
using HSG.Warehouse.Common.Models.Entity;

namespace HSG.Warehouse.Web.ViewModels
{
    public class SaleViewViewModel
    {
        public Sale? Sale { get; set; }
        public IEnumerable<SaleDetail?>? SaleDetail { get; set; }
        public double? Sum { get; set; }

    }
}
