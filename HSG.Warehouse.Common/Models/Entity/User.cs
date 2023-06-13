using HSG.Warehouse.Common.Models.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSG.Warehouse.Common.Models.Entity
{
    public class User : BaseModel
    {
        public string? Login { get; set; }
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
