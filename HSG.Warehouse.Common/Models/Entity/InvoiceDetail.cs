using HSG.Warehouse.Common.Models.Entity.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HSG.Warehouse.Common.Models.Entity
{
    public class InvoiceDetail : BaseModel
    {
        [ForeignKey("Invoice")]
        public int InvoiceId { get; set; }
        public Invoice? Invoice { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        [Required]
        public double Amount { get; set; }

        [Required]
        public double Price { get; set; }
    }
}
