using System.Threading.Tasks;
using VendingMachine.Services.Domain;

namespace VendingMachine.Services.Application.Abstractions.Repositories
{
    public interface IProductRepository
    {
        /// <summary>
        /// Create new Product
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns></returns>
        Task<Product> CreateProductAsync(Product product);

        /// <summary>
        /// Check if seller already has product with provided name
        /// </summary>
        /// <param name="productName">Product Name</param>
        /// <param name="sellerId">Seller Id</param>
        /// <returns></returns>
        Task<bool> ProductAvailableForSellerAsync(string productName, int sellerId);

        /// <summary>
        /// Get Product by Product Id
        /// </summary>
        /// <param name="productId">Product Id</param>
        /// <returns></returns>
        Task<Product> GetProductAsync(int productId);

        /// <summary>
        /// Delete provided product
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns></returns>
        Task DeleteProductAsync(Product product);
    }
}
