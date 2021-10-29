using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using VendingMachine.Services.Application.Abstractions.Repositories;
using VendingMachine.Services.Domain;
using VendingMachine.Services.Infrastructure.EFDbContext;

namespace VendingMachine.Services.Infrastructure
{
    public class UserRepository : IUserRepository
    {
        #region << Fields >>

        private readonly VendingMachineContext _context;

        #endregion << Fields >>

        #region << Controllers >>

        public UserRepository(VendingMachineContext context)
        {
            _context = context;
        }

        #endregion << Controllers >>

        #region << Public Methods >>

        public async Task<User> CreateUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<bool> UserExitsAsync(string username)
        {
            return await _context.Users.AnyAsync(x => x.Username == username);
        }

        public async Task<User> GetUser(string username)
        {
            return await _context.Users.SingleOrDefaultAsync(x => x.Username == username);
        }

        #endregion << Public Methods >>
    }
}
