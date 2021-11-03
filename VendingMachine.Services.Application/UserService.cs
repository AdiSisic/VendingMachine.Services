using AutoMapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VendingMachine.Services.Api.Base;
using VendingMachine.Services.Api.Enums;
using VendingMachine.Services.Api.User.Response;
using VendingMachine.Services.Application.Abstractions;
using VendingMachine.Services.Application.Abstractions.Repositories;

using AppModels = VendingMachine.Services.Application.Models;
using DomainModels = VendingMachine.Services.Domain;

namespace VendingMachine.Services.Application
{
    public class UserService : IUserService
    {
        #region << Fields >>

        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;

        #endregion << Fields >>

        #region << Constructor >>

        public UserService(IMapper mapper, IConfiguration configuration, IUserRepository userRepository, IProductRepository productRepository)
        {
            _mapper = mapper;
            _configuration = configuration;
            _userRepository = userRepository;
            _productRepository = productRepository;
        }

        #endregion << Constructor >>

        #region << Public Methods >>

        public async Task<BaseResponse<AppModels.User>> GetUserAsync(int userId)
        {
            BaseResponse<AppModels.User> response = new();

            try
            {
                var dbUser = await _userRepository.GetUserAsync(userId);
                if (dbUser == null)
                {
                    response.Message = "Invalid User Id";
                }
                else
                {
                    response.Data = _mapper.Map<DomainModels.User, AppModels.User>(dbUser);
                    response.Success = true;
                }
            }
            catch (Exception)
            {
                //TODO: CREATE LOG
            }

            return response;
        }

        public async Task<BaseResponse<bool>> DepositAsync(int coin, int userId)
        {
            BaseResponse<bool> baseResponse = new();

            try
            {
                var validCoins = _configuration.GetValue<string>("ValidCoins").Split(",").Select(Int32.Parse).ToList();
                if (!validCoins.Contains(coin))
                {
                    baseResponse.Message = "Invalid coin detected";
                    return baseResponse;
                }

                var dbUser = await _userRepository.GetUserAsync(userId);
                if (dbUser == null)
                {
                    baseResponse.Message = "Invalid User Id";
                    return baseResponse;
                }

                if(dbUser.RoleId != (int)RoleType.Buyer)
                {
                    baseResponse.Message = "Only buyers can deposit money";
                }

                dbUser.Deposit += coin;
                await _userRepository.UpdateUserAsync(dbUser);
                baseResponse.Data = true;
                baseResponse.Success = true;
            }
            catch (Exception)
            {
                baseResponse.Message = "Something went wrong. Please contact support for more details";
            }

            return baseResponse;
        }

        public async Task<BaseResponse<int>> GetDepositAsync(int userId)
        {
            BaseResponse<int> baseResponse = new();

            try
            {
                var dbUser = await _userRepository.GetUserAsync(userId);
                if (dbUser == null)
                {
                    baseResponse.Message = "Invalid User Id";
                    return baseResponse;
                }

                baseResponse.Data = dbUser.Deposit;
                baseResponse.Success = true;
            }
            catch (Exception)
            {
                baseResponse.Message = "Something went wrong. Please contact support for more details";
            }

            return baseResponse;
        }

        public async Task<BaseResponse<BuyProductsResponse>> PurchaseAsync(int userId, int productId, int count)
        {
            BaseResponse<BuyProductsResponse> baseResponse = new();

            try
            {
                // Check if product valid
                var dbProduct = await _productRepository.GetProductAsync(productId);
                if (dbProduct == null)
                {
                    baseResponse.Message = "Invalid product";
                    return baseResponse;
                }

                // Check if user valid
                var dbUser = await _userRepository.GetUserAsync(userId);
                if (dbUser == null)
                {
                    baseResponse.Message = "Invalid user";
                    return baseResponse;
                }

                if (dbUser.RoleId != (int)RoleType.Buyer)
                {
                    baseResponse.Message = "Only buyers can deposit money";
                    return baseResponse;
                }

                if (dbProduct.Amount < count)
                {
                    baseResponse.Message = $"Only {dbProduct.Amount} products are available";
                    return baseResponse;
                }

                // Regardless of UI, we need to make sure that user has enough money to make purchase
                if (dbUser.Deposit < (dbProduct.Cost * count))
                {
                    baseResponse.Message = "Deposit to low to buy a product";
                    return baseResponse;
                }

                dbProduct.Amount -= count;
                dbUser.Deposit -= dbProduct.Cost * count;

                await _productRepository.UpdateProductAsync(dbProduct);
                await _userRepository.UpdateUserAsync(dbUser);

                baseResponse.Data = new BuyProductsResponse();
                baseResponse.Data.Amount = count;
                baseResponse.Data.MoneyLeft = dbUser.Deposit;
                baseResponse.Data.ProductId = productId;
                baseResponse.Data.Spent = dbProduct.Cost * count;
                baseResponse.Data.Change = CalculateChange(dbUser.Deposit);
                baseResponse.Success = true;
            }
            catch (Exception)
            {
                baseResponse.Message = "Something went wrong. Please contact support for more details";
            }

            return baseResponse;
        }

        public async Task<BaseResponse<bool>> ResetDeposit(int userId)
        {
            BaseResponse<bool> baseResponse = new();

            try
            {
                var dbUser = await _userRepository.GetUserAsync(userId);
                if (dbUser == null)
                {
                    baseResponse.Message = "Invalid user";
                    return baseResponse;
                }

                dbUser.Deposit = 0;
                await _userRepository.UpdateUserAsync(dbUser);

                baseResponse.Data = true;
                baseResponse.Success = true;
            }
            catch (Exception)
            {
                baseResponse.Message = "Something went wrong. Please contact support for more details";
            }

            return baseResponse;
        }

        #endregion << Public Methods >>

        #region << Private Methods >>

        private List<Change> CalculateChange(int userDeposit)
        {
            List<Change> change = new();
            List<int> validCoins = _configuration.GetValue<string>("ValidCoins").Split(",").Select(Int32.Parse).OrderByDescending(x => x).ToList();

            foreach (int coin in validCoins)
            {
                int amount = userDeposit / coin;
                if (amount > 0)
                {
                    change.Add(new Change() { Coin = coin, Amount = amount });
                }

                userDeposit -= amount * coin;
            }

            return change;
        }

        #endregion << Private Methods >>
    }
}
