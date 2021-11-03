using AutoMapper;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VendingMachine.Services.Api.Enums;
using VendingMachine.Services.Application.Abstractions.Repositories;
using VendingMachine.Services.Application.Models;
using Xunit;

namespace VendingMachine.Services.Application.Tests
{
    public class UserServiceTest
    {
        private Mock<IMapper> _mapper;
        private Mock<IProductRepository> _productRepository;
        private Mock<IUserRepository> _userRepository;
        private IConfiguration _configuration;

        private readonly UserService _userService;

        public UserServiceTest()
        {
            SetupMocks();
            _userService = new UserService(_mapper.Object, _configuration, _userRepository.Object, _productRepository.Object);
        }

        [Fact]
        public async void GetUser_Buyer_ShouldSucceed()
        {
            _userRepository.Setup(x => x.GetUserAsync(1, true))
                .ReturnsAsync((int userId, bool tracking) => new Domain.User
                {
                    Id = userId,
                    Deposit = 20,
                    Password = "Password123",
                    Username = "LoveCoffe",
                    RoleId = 1,
                });

            var result = await _userService.GetUserAsync(1);

            Assert.True(result.Success);
            Assert.True(result.Data.Id == 1);
        }

        [Fact]
        public async void Deposit_ShouldSucceed()
        {
            _userRepository.Setup(x => x.GetUserAsync(1, true))
               .ReturnsAsync((int userId, bool tracking) => new Domain.User
               {
                   Id = userId,
                   Deposit = 20,
                   Password = "Password123",
                   Username = "LoveCoffe",
                   RoleId = 0,
               });

            var result = await _userService.DepositAsync(10, 1);

            Assert.True(result.Success);
        }

        [Fact]
        public async void GetDeposit_ShouldSucceed()
        {
            _userRepository.Setup(x => x.GetUserAsync(1, true))
               .ReturnsAsync((int userId, bool tracking) => new Domain.User
               {
                   Id = userId,
                   Deposit = 20,
                   Password = "Password123",
                   Username = "LoveCoffe",
                   RoleId = 0,
               });

            var result = await _userService.GetDepositAsync(1);

            Assert.True(result.Success);
            Assert.True(result.Data == 20);
        }

        [Fact]
        public async void Purchase_ShouldSucceed()
        {
            _productRepository.Setup(x => x.GetProductAsync(It.IsAny<int>()))
               .ReturnsAsync((int id) => new Domain.Product
               {
                   Id = id,
                   Amount = 1,
                   Cost = 2,
                   Name = "Fanta",
                   SellerId = 1
               });

            _userRepository.Setup(x => x.GetUserAsync(1, true))
              .ReturnsAsync((int userId, bool tracking) => new Domain.User
              {
                  Id = userId,
                  Deposit = 20,
                  Password = "Password123",
                  Username = "LoveCoffe",
                  RoleId = 0,
              });

            var result = await _userService.PurchaseAsync(1, 1, 1);

            Assert.True(result.Success);
            Assert.True(result.Data.Amount == 1);
            Assert.True(result.Data.Spent == 2);
            Assert.True(result.Data.MoneyLeft == 18);
        }

        [Fact]
        public async void ResetDeposit_ShouldSucceed()
        {
            _userRepository.Setup(x => x.GetUserAsync(1, true))
             .ReturnsAsync((int userId, bool tracking) => new Domain.User
             {
                 Id = userId,
                 Deposit = 20,
                 Password = "Password123",
                 Username = "LoveCoffe",
                 RoleId = 0,
             });

            var result = await _userService.ResetDeposit(1);
            Assert.True(result.Success);
        }

        #region << Setup >>

        private void SetupMocks()
        {
            _configuration = SetupConfiguration();
            _userRepository = SetupUserRepositoryMock();
            _mapper = SetupMapperMock();
            _productRepository = SetupProductRepositoryMock();
        }

        private IConfiguration SetupConfiguration()
        {
            var inMemorySettings = new Dictionary<string, string>() { { "ValidCoins", "5,10,20,50,100" } };
            return new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
        }

        private Mock<IUserRepository> SetupUserRepositoryMock()
        {
            Mock<IUserRepository> userRepository = new();

            userRepository.Setup(x => x.GetUserAsync(1, true))
                .ReturnsAsync((int userId, bool tracking) => new Domain.User
                {
                    Id = userId,
                    Deposit = 20,
                    Password = "Password123",
                    Username = "LoveCoffe",
                    RoleId = 1,
                });

            return userRepository;
        }

        private Mock<IMapper> SetupMapperMock()
        {
            Mock<IMapper> mapper = new();

            mapper.Setup(x => x.Map<Domain.User, User>(It.IsAny<Domain.User>()))
            .Returns((Domain.User x) => new User
            {
                Deposit = x.Deposit,
                Id = x.Id,
                Password = x.Password,
                Role = (RoleType)x.RoleId,
                Username = x.Username
            });

            return mapper;
        }

        private Mock<IProductRepository> SetupProductRepositoryMock()
        {
            return new Mock<IProductRepository>();
        }

        #endregion << Setup >>
    }
}
