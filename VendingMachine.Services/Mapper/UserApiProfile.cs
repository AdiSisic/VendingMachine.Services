using AutoMapper;

using VendingMachine.Services.Api.User.Request;
using VendingMachine.Services.Application.Models;

namespace VendingMachine.Services.Mapper
{
    public class UserApiProfile : Profile
    {
        public UserApiProfile()
        {
            CreateMap<CreateUserRequest, User>();
        }
    }
}
