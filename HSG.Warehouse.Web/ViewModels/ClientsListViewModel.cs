using HSG.Warehouse.Common.Models.Entity;

namespace HSG.Warehouse.Web.ViewModels
{
    public class ClientsListViewModel
    {
        public Client? Client { get; set; }
        public IEnumerable<Client?>? Clients { get; set; }
    }
}
