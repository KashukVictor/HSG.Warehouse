using HSG.Warehouse.Common.Models.Entity;
using System.Collections.Generic;

namespace HSG.Warehouse.Common.Models
{
    public class CategoryMenu
    {
        public IEnumerable<Category?>? Categories { get; set; }
        public int SelectedId { get; set; }
        public string? QueryString { get; set; }
    }
}
