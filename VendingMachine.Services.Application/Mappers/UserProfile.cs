using AutoMapper;
using System;
using VendingMachine.Services.Api.Enums;

namespace VendingMachine.Services.Application.Mappers
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<Models.User, Domain.User>().ForMember(dest => dest.RoleId, opts => opts.MapFrom(src => (int)src.Role));

            CreateMap<Domain.User, Models.User>().ForMember(dest => dest.Role, opts => opts.MapFrom(src => Enum.GetName(typeof(RoleType), src.RoleId)));
        }
    }
}
