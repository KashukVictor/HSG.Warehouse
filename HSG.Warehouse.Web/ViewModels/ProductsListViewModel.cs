using HSG.Warehouse.Common.Models;
using HSG.Warehouse.Common.Models.Entity;

namespace HSG.Warehouse.Web.ViewModels
{
    public class ProductsListViewModel
    {
        public IEnumerable<Category?>? Categories { get; set; }
        public IEnumerable<Product?>? Products { get; set; }
        public CategoryMenu? CategoryMenu { get; set; }

    }
}
