using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VendingMachine.Services.Api.Base;
using VendingMachine.Services.Api.User.Request;
using VendingMachine.Services.Api.User.Requests;
using VendingMachine.Services.Api.User.Response;
using VendingMachine.Services.Application.Abstractions;
using VendingMachine.Services.Application.Models;

namespace VendingMachine.Services.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        #region << Fields >>

        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        #endregion << Fields >>

        #region << Constructor >>

        public UserController(IMapper mapper, ITokenService tokenService, IUserService userService)
        {
            _mapper = mapper;
            _tokenService = tokenService;
            _userService = userService;
        }

        #endregion << Constructor >>

        [HttpPost, Route("register", Name = "Register")]
        public async Task<BaseResponse<bool>> Register([FromBody] RegisterRequest request)
        {
            var user = _mapper.Map<RegisterRequest, User>(request);

            return await _userService.Register(user);
        }
        
        [HttpPost, Route("login", Name = "Login")]
        public async Task<BaseResponse<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            var loginResponse = await _userService.Login(request.Username, request.Password);
            BaseResponse<LoginResponse> response = new() { Success = loginResponse.Success, Message = loginResponse.Message };

            if (loginResponse.Success)
            {
                response.Data = _mapper.Map<User, LoginResponse>(loginResponse.Data);
                response.Data.Token = _tokenService.CreateToken(loginResponse.Data);
            }

            return response;
        }

        [Authorize]
        [HttpGet, Route("authorizeTest", Name = "authorizeTest")]
        public async Task<string> AuthorizedTest()
        {
            return "authorized";
        }
    }
}
