using AutoMapper;

using VendingMachine.Services.Api.User.Request;
using VendingMachine.Services.Api.User.Response;
using VendingMachine.Services.Application.Models;

namespace VendingMachine.Services.Mapper
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<CreateMemberRequest, User>();
            CreateMap<User, LoginMemberResponse>();
        }
    }
}
