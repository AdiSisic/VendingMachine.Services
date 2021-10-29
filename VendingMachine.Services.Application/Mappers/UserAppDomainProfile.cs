using AutoMapper;

namespace VendingMachine.Services.Application.Mappers
{
    public class UserAppDomainProfile : Profile
    {
        public UserAppDomainProfile()
        {
            CreateMap<Models.User, Domain.User>()
                .ForMember(dest => dest.RoleId, opts => opts.MapFrom(src => (int)src.Role))
                .ReverseMap();
        }
    }
}
