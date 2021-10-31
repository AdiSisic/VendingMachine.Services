using System.Threading.Tasks;
using VendingMachine.Services.Api.Base;
using VendingMachine.Services.Application.Models;

namespace VendingMachine.Services.Application.Abstractions
{
    public interface IProductService
    {
        /// <summary>
        /// Create new product
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns></returns>
        Task<BaseResponse<Product>> CreateProductAsync(Product product);

        /// <summary>
        /// Get product by Product Id
        /// </summary>
        /// <param name="productId">Product ID</param>
        /// <returns></returns>
        Task<BaseResponse<Product>> GetProductAsync(int productId);

        /// <summary>
        /// Delete product by Product Id
        /// </summary>
        /// <param name="productId">Product Id</param>
        /// <param name="userId">User Id</param>
        /// <returns></returns>
        Task<BaseResponse<bool>> DeleteProductAsync(int productId, int userId);
    }
}
