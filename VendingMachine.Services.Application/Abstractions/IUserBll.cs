using System.Threading.Tasks;
using VendingMachine.Services.Application.Models;

namespace VendingMachine.Services.Application.Abstractions
{
    public interface IUserBll
    {
        /// <summary>
        /// Create new User
        /// </summary>
        /// <param name="user">User data</param>
        /// <returns>Success flag</returns>
        Task<bool> CreateUser(User user);
    }
}
