using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VendingMachine.Services.Api.Base;
using VendingMachine.Services.Api.User.Response;
using VendingMachine.Services.Application.Abstractions;
using VendingMachine.Services.Application.Models;
using VendingMachine.Services.Attributes;

namespace VendingMachine.Services.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        #region << Fields >>

        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        #endregion << Fields >>

        #region << Constructor >>

        public UserController(IMapper mapper, IConfiguration configuration, IUserService userService)
        {
            _mapper = mapper;
            _userService = userService;
            _configuration = configuration;
        }

        #endregion << Constructor >>

        [HttpGet, Route("{userId}", Name = "GetUser"), JwtAuthorization]
        public async Task<BaseResponse<GetUserResponse>> GetUser(int userId)
        {
            if(userId <= 0)
            {
                return new BaseResponse<GetUserResponse>() { Message = "Bad Request" };
            }

            var serviceResponse = await _userService.GetUserAsync(userId);
            BaseResponse<GetUserResponse> response = new() { Success = serviceResponse.Success, Message = serviceResponse.Message };

            if (response.Success)
            {
                response.Data = _mapper.Map<User, GetUserResponse>(serviceResponse.Data);
            }

            return response;
        }

        [HttpPost, Route("deposit/{coin}", Name = "Deposit"), JwtAuthorization]
        public async Task<BaseResponse<bool>> Deposit(int coin)
        {
            var validCoins = _configuration.GetValue<string>("ValidCoins").Split(",").Select(Int32.Parse).ToList();
            if (!validCoins.Contains(coin))
            {
                return new BaseResponse<bool>() { Message = "Invalid coin detected" };
            }

            int userId = GetUserIdFromClaim();

            return await _userService.DepositAsync(coin, userId);
        }

        /// <summary>
        /// Get deposit for logged user
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("getDeposit", Name = "GetDeposit"), JwtAuthorization]
        public async Task<BaseResponse<int>> GetDeposit()
        {
            int userId = GetUserIdFromClaim();

            return await _userService.GetDepositAsync(userId);
        }

        /// <summary>
        /// Purchase product
        /// </summary>
        /// <param name="productId">ProductId</param>
        /// <returns></returns>
        [HttpGet, Route("purchase/{productId}")]
        public async Task<BaseResponse<bool>> Purchase([FromRoute]int productId)
        {
            if(productId <= 0)
            {
                return new BaseResponse<bool>() { Message = "Bad Request" };
            }

            int userId = GetUserIdFromClaim();

            return await _userService.PurchaseAsync(userId, productId);
        }


        #region << Private Methods >>

        private int GetUserIdFromClaim()
        {
            var claimUserId = User.Claims.Where(x => x.Type == "UserId").Select(x => x.Value).FirstOrDefault();
            Int32.TryParse(claimUserId, out int userId);

            return userId;
        }

        #endregion << Private Methods >>
    }
}
