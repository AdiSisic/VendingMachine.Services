using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VendingMachine.Services.Api.Base;
using VendingMachine.Services.Api.User.Response;
using VendingMachine.Services.Application.Abstractions;
using VendingMachine.Services.Application.Models;
using VendingMachine.Services.Attributes;

namespace VendingMachine.Services.Controllers
{
    [Route("api/[controller]")]
    [ApiController, YwtAuthorization]
    public class UserController : ControllerBase
    {
        #region << Fields >>

        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        #endregion << Fields >>

        #region << Constructor >>

        public UserController(IMapper mapper, IUserService userService)
        {
            _mapper = mapper;
            _userService = userService;
        }

        #endregion << Constructor >>

        [HttpGet, Route("{userId}", Name = "GetUser")]
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


        //[HttpGet, Route("GetSellerUser", Name = "GetSellerUser")]
        //public string GetSellerUser()
        //{
        //    return "Seller";
        //}

        //[HttpGet, Route("GetBuyerUser", Name = "GetBuyerUser")]
        //public string GetBuyerUser()
        //{
        //    return "Buyer";
        //}
    }
}
