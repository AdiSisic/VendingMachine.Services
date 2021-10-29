using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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

        /// <summary>
        /// Register new User
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("register", Name = "Register")]
        public async Task<BaseResponse<bool>> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return new BaseResponse<bool>() { Message = "Bad Request" };
            }

            var user = _mapper.Map<RegisterRequest, User>(request);
            return await _userService.Register(user);
        }

        [HttpPost, Route("login", Name = "Login")]
        public async Task<BaseResponse<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return new BaseResponse<LoginResponse>() { Message = "Bad Request" };
            }

            var loginResponse = await _userService.Login(request.Username, request.Password);
            BaseResponse<LoginResponse> response = new() { Success = loginResponse.Success, Message = loginResponse.Message };

            // If user found
            if (loginResponse.Success)
            {
                response.Data = _mapper.Map<User, LoginResponse>(loginResponse.Data);
                response.Data.Token = _tokenService.CreateToken(loginResponse.Data);
            }

            return response;
        }

        /// <summary>
        /// Logout for existing user
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost, Route("logout", Name = "Logout")]
        public async Task<IActionResult> Logout()
        {
            var user = HttpContext.User;
            //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Ok();
        }

        [Authorize]
        [HttpGet, Route("authorizeTest", Name = "authorizeTest")]
        public async Task<string> AuthorizedTest()
        {
            var user = HttpContext.User;
            return "authorized";
        }
    }
}
