using System.Threading.Tasks;
using VendingMachine.Services.Application.Abstractions.Repositories;
using VendingMachine.Services.Domain;
using VendingMachine.Services.Infrastructure.EFDbContext;

namespace VendingMachine.Services.Infrastructure
{
    public class UserRepository : IUserRepository
    {
        public async Task<User> CreateUser(User user)
        {
            using (VendingMachineContext context = new())
            {
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();
            }

            return user;
        }
    }
}
