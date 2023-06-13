using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSG.Warehouse.Interfaces
{
    public interface IPasswordValidator
    {
        bool VerifyPassword(string hashPassword, string password);
        string GetHashString(string password);
    }
}
