using AutoMapper;
using Moq;
using System;
using System.Security.Cryptography;
using System.Text;
using VendingMachine.Services.Api.Enums;
using VendingMachine.Services.Application.Abstractions.Repositories;
using VendingMachine.Services.Application.Models;
using Xunit;

namespace VendingMachine.Services.Application.Tests
{
    public class MemberServiceTest
    {
        private Mock<IMapper> _mapper;
        private Mock<IUserRepository> _userRepository;

        private readonly MemberService _service;

        public MemberServiceTest()
        {
            SetupMocks();
            _service = new MemberService(_mapper.Object, _userRepository.Object);
        }

        [Fact]
        public async void Register_ShouldSucceed()
        {
            User user = new User() { Deposit = 0, Id = 1, Password = "Test123", Username = "LoveCoffe", Role = Api.Enums.RoleType.Buyer };

            var result = await _service.RegisterAsync(user);
            Assert.True(result.Success);
        }

        [Fact]
        public async void UpdateUser_ShouldSucceed()
        {
            User user = new User() { Deposit = 0, Id = 1, Password = "Test123", Username = "LoveCoffe", Role = Api.Enums.RoleType.Buyer };

            _userRepository.Setup(x => x.GetUserAsync(1, false))
           .ReturnsAsync((int userId, bool tracking) => new Domain.User
           {
               Id = userId,
               Deposit = 20,
               Password = "Password123",
               Username = "LoveCoffe",
               RoleId = 0,
           });

            var result = await _service.UpdateUserAsync(user);
            Assert.True(result.Success);
        }

        [Fact]
        public async void Login_ShouldSucceed()
        {
            _userRepository.Setup(x => x.GetUserAsync(It.IsAny<string>()))
            .ReturnsAsync((string username) => new Domain.User
            {
                Id = 1,
                Deposit = 20,
                Password = HashString("Password123"),
                Username = username,
                RoleId = 0,
            });

            var result = await _service.LoginAsync("LoveCoffe", "Password123");

            Assert.True(result.Success);
            Assert.True(result.Data.Id == 1);
        }

        [Fact]
        public async void DeleteUser_ShouldSucceed()
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

            var result = await _service.DeleteUserAsync(1);
            Assert.True(result.Success);
        }

        #region << Setup >>

        private void SetupMocks()
        {
            _mapper = SetupMapperMock();
            _userRepository = new Mock<IUserRepository>();
        }

        private Mock<IMapper> SetupMapperMock()
        {
            Mock<IMapper> mapper = new();

            mapper.Setup(x => x.Map<User, Domain.User>(It.IsAny<User>()))
            .Returns((User x) => new Domain.User
            {
                Deposit = x.Deposit,
                Id = x.Id,
                Password = x.Password,
                RoleId = (int)x.Role,
                Username = x.Username
            });

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

        private string HashString(string text)
        {
            return BitConverter.ToString(new SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes(text)))
                               .Replace("-", string.Empty);
        }

        #endregion << Setup >>
    }
}
