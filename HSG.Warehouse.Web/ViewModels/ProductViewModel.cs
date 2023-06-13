
using HSG.Warehouse.Common.Models.Entity;

namespace HSG.Warehouse.Web.ViewModels
{
    public class ProductViewModel
    {
        public Product? Product { get; set; }
        public IEnumerable<Category?>? Categories { get; set; }
        public IEnumerable<Manufacturer?>? Manufacturers { get; set; }
        public IEnumerable<Unit?>? Units { get; set; }
        public bool IsAutoBarcode { get; set; }

    }
}
