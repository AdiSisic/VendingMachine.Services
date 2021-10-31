using AutoMapper;
using System;
using System.Threading.Tasks;
using VendingMachine.Services.Api.Base;
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
        private readonly IUserRepository _userRepository;

        #endregion << Fields >>

        #region << Constructor >>

        public UserService(IMapper mapper, IUserRepository userRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
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

        #endregion << Public Methods >>
    }
}
