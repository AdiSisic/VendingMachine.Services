using System.Threading.Tasks;
using VendingMachine.Services.Api.Base;
using VendingMachine.Services.Api.User.Response;
using VendingMachine.Services.Application.Models;

namespace VendingMachine.Services.Application.Abstractions
{
    public interface IUserService
    {
        /// <summary>
        /// Get User
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns></returns>
        Task<BaseResponse<User>> GetUserAsync(int userId);

        /// <summary>
        /// Deposit coins
        /// </summary>
        /// <param name="coin">Coins</param>
        /// <param name="userId">User ID</param>
        /// <returns></returns>
        Task<BaseResponse<bool>> DepositAsync(int coin, int userId);

        /// <summary>
        /// Reset user deposit
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns></returns>
        Task<BaseResponse<bool>> ResetDeposit(int userId);

        /// <summary>
        /// Get current deposit for user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns></returns>
        Task<BaseResponse<int>> GetDepositAsync(int userId);

        /// <summary>
        /// Purchase product
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="productId">Product ID</param>
        /// <param name="count">Number of items to buy</param>
        /// <returns></returns>
        Task<BaseResponse<BuyProductsResponse>> PurchaseAsync(int userId, int productId, int count);
    }
}
