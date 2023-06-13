﻿using HSG.Warehouse.Common.Models.Entity.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HSG.Warehouse.Common.Models.Entity
{
    public class Warehouse : BaseModel
    {
        public double? Amount { get; set; }
        [Required]
        public double? Price { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        [ForeignKey("InvoiceDetail")]
        public int InvoiceDetailId { get; set; }
        public InvoiceDetail? InvoiceDetail { get; set; }
    }
}
