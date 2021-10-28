using System.Threading.Tasks;
using VendingMachine.Services.Domain;

namespace VendingMachine.Services.Application.Abstractions.Repositories
{
    public interface IUserRepository
    {
        Task<User> CreateUser(User user);
    }
}
