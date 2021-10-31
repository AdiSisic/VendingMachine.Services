using AutoMapper;

namespace VendingMachine.Services.Application.Mappers
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<Models.User, Domain.User>()
                .ForMember(dest => dest.RoleId, opts => opts.MapFrom(src => (int)src.Role))
                .ReverseMap();
        }
    }
}
