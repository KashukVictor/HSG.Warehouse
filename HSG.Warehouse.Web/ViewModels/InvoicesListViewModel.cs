using HSG.Warehouse.Common.Models.Entity;

namespace HSG.Warehouse.Web.ViewModels
{
    public class InvoicesListViewModel
    {
        public IEnumerable<Invoice?>? Invoices { get; set; }
        public double? Sum { get; set; }

    }
}
