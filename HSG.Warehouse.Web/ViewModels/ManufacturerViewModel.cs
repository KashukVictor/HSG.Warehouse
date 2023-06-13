using HSG.Warehouse.Common.Models.Entity;

namespace HSG.Warehouse.Web.ViewModels
{
    public class ManufacturerViewModel
    {
        public Manufacturer? Manufacturer { get; set; }
        public IEnumerable<Manufacturer?>? Manufacturers { get; set; }

    }
}
