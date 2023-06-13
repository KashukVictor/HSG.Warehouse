using HSG.Warehouse.Common.Models.Entity.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HSG.Warehouse.Common.Models.Entity
{
    public class Category : BaseModel
    {
        [Required]
        public string? Name { get; set; }

        [ForeignKey("Category")]
        public int? ParentId { get; set; }
        public Category? Parent { get; set; }

    }
}
