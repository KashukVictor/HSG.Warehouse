using HSG.Warehouse.Common.Models;
using HSG.Warehouse.Common.Models.Entity;

namespace HSG.Warehouse.Web.ViewModels
{
    public class ProductToInvoiceViewModel
    {
        public Invoice? Invoice { get; set; }
        public IEnumerable<ProductToInvoice?>? ProductToInvoices { get; set; }
        public IEnumerable<Category?>? Categories { get; set; }
        public CategoryMenu? CategoryMenu { get; set; }
        public int? ProductId { get; set; }
        public double? Amount { get; set; }
        public double? Price { get; set; }
        public double? Sum { get; set; }    
    }
}
