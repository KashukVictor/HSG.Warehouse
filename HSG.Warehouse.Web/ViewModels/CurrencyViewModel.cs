using HSG.Warehouse.Common.Models.Entity;

namespace HSG.Warehouse.Web.ViewModels
{
    public class CurrencyViewModel
    {
        public IEnumerable<Currency?>? Currency { get; set; }
        public IEnumerable<Currency?>? CurrencyNBU { get; set; }

    }
}
