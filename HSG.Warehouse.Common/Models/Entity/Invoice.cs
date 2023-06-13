using HSG.Warehouse.Common.Models.Entity.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HSG.Warehouse.Common.Models.Entity
{
    public class Invoice : BaseModel
    {
        [Required]
        public string? Number { get; set; }

        [Required]
        public DateTime Date { get; set; } = DateTime.Now;

        public string? Description { get; set; }

        [Required]
        [ForeignKey("Currency")]
        public int CurrencyId { get; set; }
        public Currency? Currency { get; set; }
        public double CurrencyRate { get; set; } = 1;


        [Required]
        [ForeignKey("Client")]
        public int ClientId { get; set; }
        public Client? Client { get; set; }

        [Required]
        public bool Fixed { get; set; }
    }
}
