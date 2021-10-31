using System.Threading.Tasks;
using VendingMachine.Services.Api.Base;
using VendingMachine.Services.Application.Models;

namespace VendingMachine.Services.Application.Abstractions
{
    public interface IMemberService
    {
        /// <summary>
        /// Register new User
        /// </summary>
        /// <param name="user">User data</param>
        /// <returns>User</returns>
        Task<BaseResponse<bool>> RegisterAsync(User user);

        /// <summary>
        /// Login existing user
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns></returns>
        Task<BaseResponse<User>> LoginAsync(string username, string password);

        /// <summary>
        /// Delete existing User
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns></returns>
        Task<BaseResponse<bool>> DeleteUserAsync(int userId);
    }
}
