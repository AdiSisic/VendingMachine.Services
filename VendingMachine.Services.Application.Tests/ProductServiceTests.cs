using AutoMapper;
using Moq;
using System.Collections.Generic;
using System.Linq;
using VendingMachine.Services.Application.Abstractions.Repositories;
using VendingMachine.Services.Application.Models;
using Xunit;

namespace VendingMachine.Services.Application.Tests
{
    public class ProductServiceTests
    {
        private Mock<IMapper> _mapper;
        private Mock<IProductRepository> _productRepository;

        private readonly ProductService _service;

        public ProductServiceTests()
        {
            SetupMocks();
            _service = new ProductService(_mapper.Object, _productRepository.Object);
        }

        [Fact]
        public async void CreateProduct_ShouldSucceed()
        {
            Product product = new Product() { SellerId = 1, Amount = 2, Cost = 3, Name = "Fanta" };

            var result = await _service.CreateProductAsync(product);

            Assert.True(result.Success);
        }

        [Fact]
        public async void GetSellerProducts_ShouldSucceed()
        {
            var result = await _service.GetSellerProductsAsync(1);

            Assert.True(result.Success);
            Assert.True(result.Data.Count() == 2);
        }

        [Fact]
        public async void GetProducts_ShouldSucceed()
        {
            var result = await _service.GetProductsAsync();

            Assert.True(result.Success);
            Assert.True(result.Data.Count() == 4);
        }

        [Fact]
        public async void DeleteProduct_ShouldSucceed()
        {
            var result = await _service.DeleteProductAsync(1, 1);
            Assert.True(result.Success);
        }

        [Fact]
        public async void UpdateProduct_ShouldSucceed()
        {
            var result = await _service.UpdateProductAsync(new Product() { Id = 1, Amount = 2, Cost = 3, Name = "Fanta", SellerId = 1 });

            Assert.True(result.Success);
        }

        #region << Setup >>

        private void SetupMocks()
        {
            _mapper = SetupMapperMock();
            _productRepository = SetupProductRepositoryMock();
        }

        private Mock<IMapper> SetupMapperMock()
        {
            Mock<IMapper> mapper = new();

            mapper.Setup(x => x.Map<Product, Domain.Product>(It.IsAny<Product>()))
                  .Returns((Product x) => new Domain.Product
                  {
                      Id = x.Id,
                      Amount = x.Amount,
                      Cost = x.Cost,
                      Name = x.Name,
                      SellerId = x.SellerId
                  });

            mapper.Setup(x => x.Map<Domain.Product, Product>(It.IsAny<Domain.Product>()))
                .Returns((Domain.Product x) => new Product
                {
                    Id = x.Id,
                    Amount = x.Amount,
                    Cost = x.Cost,
                    Name = x.Name,
                    SellerId = x.SellerId
                });

            mapper.Setup(x => x.Map<IEnumerable<Domain.Product>, IEnumerable<Product>>(It.IsAny<IEnumerable<Domain.Product>>()))
                .Returns((IEnumerable<Domain.Product> x) => x.Select(y => new Product
                {
                    Amount = y.Amount,
                    Cost = y.Cost,
                    Id = y.Id,
                    Name = y.Name,
                    SellerId = y.SellerId
                }).ToList());

            return mapper;
        }

        private Mock<IProductRepository> SetupProductRepositoryMock()
        {
            Mock<IProductRepository> productRepository = new();

            productRepository.Setup(x => x.GetProductAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => new Domain.Product
                {
                    Id =  id,
                    Amount = 1,
                    Cost = 2,
                    Name = "Fanta",
                    SellerId = 1
                });

            productRepository.Setup(x => x.CreateProductAsync(It.IsAny<Domain.Product>()))
                .ReturnsAsync((Domain.Product x) => new Domain.Product
                {
                    Id = 1,
                    Amount = x.Amount,
                    Cost = x.Cost,
                    Name = x.Name,
                    SellerId = x.SellerId
                });

            productRepository.Setup(x => x.GetSellerProductsAsync(It.IsAny<int>())).ReturnsAsync(new List<Domain.Product>
            {
                new Domain.Product() { Amount = 1, Cost = 2, Id = 1, Name = "Fanta", SellerId = 1 },
                new Domain.Product() { Amount = 2, Cost = 3, Id = 2, Name = "Pepsi", SellerId = 1 },
            });

            productRepository.Setup(x => x.GetProductsAsync()).ReturnsAsync(new List<Domain.Product>
            {
                new Domain.Product() { Amount = 1, Cost = 2, Id = 1, Name = "Fanta", SellerId = 1 },
                new Domain.Product() { Amount = 2, Cost = 3, Id = 2, Name = "Pepsi", SellerId = 1 },
                new Domain.Product() { Amount = 1, Cost = 2, Id = 1, Name = "Fanta", SellerId = 2 },
                new Domain.Product() { Amount = 2, Cost = 3, Id = 2, Name = "Pepsi", SellerId = 2 }
            });

            return productRepository;
        }

        #endregion << Setup >>
    }
}
