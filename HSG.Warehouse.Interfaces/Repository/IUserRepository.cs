using HSG.Warehouse.Common.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSG.Warehouse.Interfaces.Repository
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int userId);
        Task<User?> GetByLonigAsync(string? login);
        Task<string?> CreateAsync(User? user);
        void LoginLog(string login, string IP, bool isSuccess);
    }
}
