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
using VendingMachine.Services.Attributes;

namespace VendingMachine.Services.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        #region << Fields >>

        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly IMemberService _authenticationService;

        #endregion << Fields >>

        #region << Constructor >>

        public MemberController(IMapper mapper, ITokenService tokenService, IMemberService authenticationService)
        {
            _mapper = mapper;
            _tokenService = tokenService;
            _authenticationService = authenticationService;
        }

        #endregion << Constructor >>

        /// <summary>
        /// Login existing member
        /// </summary>
        /// <param name="request">LoginMember Request</param>
        /// <returns></returns>
        [HttpPost, Route("loginMember", Name = "LoginMember")]
        public async Task<BaseResponse<LoginMemberResponse>> LoginMember([FromBody] LoginMemberRequest request)
        {
            if (!ModelState.IsValid)
            {
                return new BaseResponse<LoginMemberResponse>() { Message = "Bad Request" };
            }

            var serviceResponse = await _authenticationService.LoginAsync(request.Username, request.Password);
            BaseResponse<LoginMemberResponse> response = new() { Success = serviceResponse.Success, Message = serviceResponse.Message };

            // If user found
            if (response.Success)
            {
                response.Data = _mapper.Map<User, LoginMemberResponse>(serviceResponse.Data);
                response.Data.Token = _tokenService.CreateToken(serviceResponse.Data);
            }

            return response;
        }

        /// <summary>
        /// Create new Member
        /// </summary>
        /// <param name="request">NewMember Request</param>
        /// <returns></returns>
        [HttpPost, Route("createMember", Name = "CreateMember")]
        public async Task<BaseResponse<bool>> CreateMember([FromBody] ManipulateMemberRequest request)
        {
            if (!ModelState.IsValid)
            {
                return new BaseResponse<bool>() { Message = "Bad Request" };
            }

            var user = _mapper.Map<ManipulateMemberRequest, User>(request);
            return await _authenticationService.RegisterAsync(user);
        }

        /// <summary>
        /// Delete existing member by existing Member ID (User ID)
        /// </summary>
        /// <param name="memberId">User Id</param>
        /// <returns></returns>
        [HttpDelete, Route("{memberId}", Name = "DeleteMember"), JwtAuthorization]
        public async Task<BaseResponse<bool>> DeleteMember([FromRoute] int memberId)
        {
            if(memberId <= 0)
            {
                return new BaseResponse<bool>() { Message = "Bad Request" };
            }

            return await _authenticationService.DeleteUserAsync(memberId);
        }

        /// <summary>
        /// Update user
        /// </summary>
        /// <param name="request">Request data</param>
        /// <param name="userId">User ID</param>
        /// <returns></returns>
        [HttpPut, Route("updateMember/{userId}", Name = "UpdateMember"), JwtAuthorization]
        public async Task<BaseResponse<bool>> UpdateMember([FromBody] ManipulateMemberRequest request, [FromRoute] int userId)
        {
            if (!ModelState.IsValid)
            {
                return new BaseResponse<bool>() { Message = "Bad Request" };
            }

            var user = _mapper.Map<ManipulateMemberRequest, User>(request);
            user.Id = userId;

            return await _authenticationService.UpdateUserAsync(user);
        }
    }
}
