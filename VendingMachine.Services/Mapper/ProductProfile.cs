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
            CreateMap<ManipulateProductRequest, Product>();
            CreateMap<Product, CreateProductResponse>();
            CreateMap<GetProductResponse, Product>();
            CreateMap<Product, GetProductResponse>();
        }
    }
}
