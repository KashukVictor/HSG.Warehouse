
using HSG.Warehouse.Common.Models.Entity;

namespace HSG.Warehouse.Web.ViewModels
{
    public class UnitViewModel
    {
        public IEnumerable<Unit?>? Units { get; set; }
        public Unit? Unit { get; set; }
    }
}
