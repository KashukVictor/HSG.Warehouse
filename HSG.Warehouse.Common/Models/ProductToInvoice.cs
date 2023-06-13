using HSG.Warehouse.Common.Models.Entity;

namespace HSG.Warehouse.Common.Models
{
    public class ProductToInvoice
    {
        public Product? Product { get; set; }
        public int InvoiceId { get; set; }
        public int ProductId { get; set; }
        public double? Amount { get; set; }
        public double? Price { get; set; }
    }
}
