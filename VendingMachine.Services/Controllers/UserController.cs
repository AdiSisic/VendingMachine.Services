using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VendingMachine.Services.Api.Base;
using VendingMachine.Services.Api.User.Request;
using VendingMachine.Services.Application.Abstractions;
using AppModels = VendingMachine.Services.Application.Models;

namespace VendingMachine.Services.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        #region << Fields >>

        private readonly IMapper _mapper;
        private readonly IUserBll _userBll;

        #endregion << Fields >>

        #region << Constructor >>

        public UserController(IMapper mapper, IUserBll userBll)
        {
            _mapper = mapper;
            _userBll = userBll;
        }

        #endregion << Constructor >>

        [HttpPost, Route("", Name = "CreateUser")]
        public async Task<BaseResponse<bool>> CreateUser([FromBody] CreateUserRequest request)
        {
            var user = _mapper.Map<CreateUserRequest, AppModels.User>(request);
            
            BaseResponse<bool> baseResponse = new();
            baseResponse.Data = await _userBll.CreateUser(user);
            baseResponse.Success = baseResponse.Data;

            return baseResponse;
        }
    }
}
