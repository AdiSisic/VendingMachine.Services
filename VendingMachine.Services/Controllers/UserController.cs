using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Security.Claims;
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
        private readonly IConfiguration _configuration;

        #endregion << Fields >>

        #region << Constructor >>

        public UserController(IMapper mapper, IConfiguration configuration, IUserService userService)
        {
            _mapper = mapper;
            _configuration = configuration;
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
        
        /// <summary>
        /// Login for Existing User
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
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

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, loginResponse.Data.Username),
                    new Claim(ClaimTypes.Role, loginResponse.Data.Role.ToString()),
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IssuedUtc = DateTime.UtcNow,
                    ExpiresUtc = request.KeepLogged ? DateTime.UtcNow.AddDays(15) : DateTime.UtcNow.AddSeconds(_configuration.GetValue<int>("Token:ExpirationInSeconds")),
                    AllowRefresh = _configuration.GetValue<bool>("Token:Refresh"),
                    IsPersistent = request.KeepLogged
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
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
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Ok();
        }

        [Authorize]
        [HttpGet, Route("authorizeTest", Name = "authorizeTest")]
        public async Task<string> AuthorizedTest()
        {
            return "authorized";
        }
    }
}
