using HSG.Warehouse.Common.Models.Entity;

namespace HSG.Warehouse.Web.ViewModels
{
    public class InvoiceViewViewModel
    {
        public Invoice? Invoice { get; set; }
        public IEnumerable<InvoiceDetail?>? InvoiceDetail { get; set; }
        public double? Sum { get; set; }

    }
}
