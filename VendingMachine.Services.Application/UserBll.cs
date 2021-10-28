using AutoMapper;

using System.Threading.Tasks;
using VendingMachine.Services.Application.Abstractions;
using VendingMachine.Services.Application.Abstractions.Repositories;

using AppModels = VendingMachine.Services.Application.Models;
using DomainModels = VendingMachine.Services.Domain;

namespace VendingMachine.Services.Application
{
    public class UserBll : IUserBll
    {
        #region << Fields >>

        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        #endregion << Fields >>

        #region << Constructor >>

        public UserBll(IMapper mapper, IUserRepository userRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }

        #endregion << Constructor >>

        #region << Public Methods >>

        public async Task<bool> CreateUser(AppModels.User user)
        {
            var newUser = _mapper.Map<AppModels.User, DomainModels.User>(user);
            var domainUser = await _userRepository.CreateUser(newUser);

            return domainUser?.Id > 0;
        }

        #endregion << Public Methods >>
    }
}
