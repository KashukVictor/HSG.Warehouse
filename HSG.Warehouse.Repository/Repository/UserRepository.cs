using HSG.Warehouse.Common.Models.Entity;
using HSG.Warehouse.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Diagnostics;

namespace HSG.Warehouse.Repository.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }
        public Task<User?> GetByIdAsync(int userId)
        {
            return _context.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
        }
        public async Task<string?> CreateAsync(User? user)
        {
            var userExist = await GetByLonigAsync(user.Login);
            if (userExist != null)
            {
                return "Такий логін вже є";
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return null;
        }
        public Task<User?> GetByLonigAsync(string? login)
        {
            return _context.Users.Where(u => u.Login == login).FirstOrDefaultAsync();
        }
        public void LoginLog(string login, string IP, bool isSuccess)
        {
            var loginLog = new LoginLog()
            {
                IP = IP,
                Login = login,
                IsSuccess = isSuccess
            };
            _context.LoginLogs.Add(loginLog);
            _context.SaveChanges();
        }
    }
}
