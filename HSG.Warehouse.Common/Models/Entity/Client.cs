using HSG.Warehouse.Common.Models.Entity.Base;
using System.ComponentModel.DataAnnotations;

namespace HSG.Warehouse.Common.Models.Entity
{
    public class Client : BaseModel
    {
        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }

        public string? MiddleName { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }
    }
}
