
namespace HSG.Warehouse.Common.Models
{
    public class ProductForSale
    {
        public Entity.Warehouse? Warehouse { get; set; }
        public int SaleId { get; set; }
        public int? SaleDetailId { get; set; }
        //public int ProductId { get; set; }
        //public int InvoiceDetailId { get; set; }
        public double? Amount { get; set; }
        public double? Price { get; set; }
    }
}
