using HSG.Warehouse.Common.Models.Entity;

namespace HSG.Warehouse.Web.ViewModels
{
    public class CategoryViewModel
    {
        public IEnumerable<Category?>? Categories { get; set; }
        public Category? Category { get; set; }
    }
}
