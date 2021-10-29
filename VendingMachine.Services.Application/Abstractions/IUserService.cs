using System.Threading.Tasks;
using VendingMachine.Services.Api.Base;
using VendingMachine.Services.Application.Models;

namespace VendingMachine.Services.Application.Abstractions
{
    public interface IUserService
    {
        /// <summary>
        /// Register new User
        /// </summary>
        /// <param name="user">User data</param>
        /// <returns>User</returns>
        Task<BaseResponse<bool>> Register(User user);

        /// <summary>
        /// Login existing user
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns></returns>
        Task<BaseResponse<User>> Login(string username, string password);
    }
}
