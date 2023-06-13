using HSG.Warehouse.Common.Models.Entity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSG.Warehouse.Common.Models.Entity
{
    public class LoginLog : BaseModel
    {
        public string? IP { get; set; }
        public string? Login { get; set; }
        public bool IsSuccess { get; set; }
    }
}
