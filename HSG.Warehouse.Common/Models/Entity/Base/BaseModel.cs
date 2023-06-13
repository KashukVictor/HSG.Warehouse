using System;
using System.ComponentModel.DataAnnotations;

namespace HSG.Warehouse.Common.Models.Entity.Base
{
    public abstract class BaseModel
    {
        [Key]
        public int Id { get; set; }

        public DateTime DateInsert { get; set; } = DateTime.Now;

        public bool IsDeleted { get; set; } = false;

        //Поле, яке не можна видаляти (Початковий набір даних, констант)
        public bool SystemField { get; set; } = false;
    }
}
