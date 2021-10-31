using AutoMapper;
using VendingMachine.Services.Api.Product;
using VendingMachine.Services.Api.Product.Request;
using VendingMachine.Services.Api.Product.Response;
using VendingMachine.Services.Application.Models;

namespace VendingMachine.Services.Mapper
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<CreateProductRequest, Product>();
            CreateMap<CreateProductResponse, Product>();
            CreateMap<GetProductResponse, Product>();
        }
    }
}
