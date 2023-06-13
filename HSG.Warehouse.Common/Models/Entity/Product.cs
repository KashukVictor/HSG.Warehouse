using HSG.Warehouse.Common.Models.Entity.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HSG.Warehouse.Common.Models.Entity
{
    public class Product : BaseModel
    {
        [Required(ErrorMessage = "Введіть клієнта")]
        public string? Name { get; set; }

        public string? Description { get; set; }

        public uint MinimunStock { get; set; } = 0;

        public string? Barcode { get; set; }
        
        public double? Price { get; set; } = 0;

        public byte[]? Image { get; set; }
        
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        
        [ForeignKey("Manufacturer")]
        public int ManufacturerId { get; set; }
        public Manufacturer? Manufacturer { get; set; }

        [ForeignKey("Unit")]
        public int UnitId { get; set; }
        public Unit? Unit { get; set; }
            
    }
}
