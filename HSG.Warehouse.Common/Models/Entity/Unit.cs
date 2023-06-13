using HSG.Warehouse.Common.Models.Entity.Base;
using System.ComponentModel.DataAnnotations;

namespace HSG.Warehouse.Common.Models.Entity
{
    public class Unit : BaseModel
    {
        [Required]
        public string? Name { get; set; }
    }
}
