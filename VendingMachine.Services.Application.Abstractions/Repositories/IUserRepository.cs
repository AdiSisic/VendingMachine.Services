using System.Threading.Tasks;
using VendingMachine.Services.Domain;

namespace VendingMachine.Services.Application.Abstractions.Repositories
{
    public interface IUserRepository
    {
        /// <summary>
        /// Create new user
        /// </summary>
        /// <param name="user"><User/param>
        /// <returns></returns>
        Task<User> CreateUserAsync(User user);

        /// <summary>
        /// Check if user with specified username exits
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns></returns>
        Task<bool> UserExitsAsync(string username);

        /// <summary>
        /// Get user by username
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns></returns>
        Task<User> GetUser(string username);
    }
}
