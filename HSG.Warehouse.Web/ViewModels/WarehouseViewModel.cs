using HSG.Warehouse.Common.Models;
using HSG.Warehouse.Common.Models.Entity;

namespace HSG.Warehouse.Web.ViewModels
{
    public class WarehouseViewModel
    {
        public Sale? Sale { get; set; }
        public IEnumerable<Common.Models.Entity.Warehouse?>? Warehouse { get; set; }
        public IEnumerable<ProductForSale?>? ProductToSales { get; set; }// Для додавання товару у накладній відпуску
        public Currency? Currency { get; set; }
        public Unit? Unit { get; set; }
        public IEnumerable<Category?>? Categories { get; set; }
        public CategoryMenu? CategoryMenu { get; set; }
        public IEnumerable<SaleDetail?>? SaleDetails { get; set; }
        public double? Sum { get; set; }

    }
}
