
using HSG.Warehouse.Common.Models.Entity;

namespace HSG.Warehouse.Web.ViewModels
{
    public class SaleViewModel
    {
        public Sale? Sale { get; set; }
        public IEnumerable<Client?>? Clients { get; set; }
        public IEnumerable<Currency?>? Currencys { get; set; }

    }
}
