using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VendingMachine.Services.Application.Abstractions.Repositories;
using VendingMachine.Services.Domain;
using VendingMachine.Services.Infrastructure.EFDbContext;

namespace VendingMachine.Services.Infrastructure
{
    public class ProductRepository : IProductRepository
    {
        #region << Fields >>

        private readonly VendingMachineContext _context;

        #endregion << Fields >>

        #region << Constructors >>

        public ProductRepository(VendingMachineContext context)
        {
            _context = context;
        }

        #endregion << Constructors >>

        #region << Public Methods >>

        public async Task<Product> CreateProductAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return product;
        }

        public async Task<bool> ProductAvailableForSellerAsync(string productName, int sellerId)
        {
            return await _context.Products.AnyAsync(x => x.Name == productName && x.SellerId == sellerId);
        }

        public async Task<Product> GetProductAsync(int productId)
        {
            return await _context.Products.FirstOrDefaultAsync(x => x.Id == productId);
        }

        public async Task<IEnumerable<Product>> GetSellerProductsAsync(int sellerId)
        {
            return await _context.Products.Where(x => x.SellerId == sellerId).ToListAsync();
        }

        public async Task<IEnumerable<Product>>GetProductsAsync()
        {
            return await _context.Products.Where(x => x.Amount > 0).ToListAsync();
        }

        public async Task DeleteProductAsync(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        #endregion << Public Methods >>
    }
}
