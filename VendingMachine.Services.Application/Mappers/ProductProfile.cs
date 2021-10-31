using AutoMapper;

using AppModels = VendingMachine.Services.Application.Models;
using DomainModels = VendingMachine.Services.Domain;

namespace VendingMachine.Services.Application.Mappers
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<AppModels.Product, DomainModels.Product>().ReverseMap();
        }
    }
}
