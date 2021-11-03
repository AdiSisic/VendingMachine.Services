using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VendingMachine.Services.Api.Base;
using VendingMachine.Services.Application.Abstractions;
using VendingMachine.Services.Application.Abstractions.Repositories;

using AppModels = VendingMachine.Services.Application.Models;
using DomainModels = VendingMachine.Services.Domain;

namespace VendingMachine.Services.Application
{
    public class ProductService : IProductService
    {
        #region << Fields >>

        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;

        #endregion << Fields >>

        #region << Constructors >>

        public ProductService(IMapper mapper, IProductRepository productRepository)
        {
            _mapper = mapper;
            _productRepository = productRepository;
        }

        #endregion << Constructors >>

        #region << Public Methods >>

        public async Task<BaseResponse<AppModels.Product>> CreateProductAsync(AppModels.Product product)
        {
            BaseResponse<AppModels.Product> baseResponse = new();

            try
            {
                baseResponse = await CreateProductValidationAsync(product);
                if (String.IsNullOrWhiteSpace(baseResponse.Message))
                {
                    // Add new product
                    var newProduct = _mapper.Map<AppModels.Product, Domain.Product>(product);
                    var dbProduct = await _productRepository.CreateProductAsync(newProduct);

                    // Validate new product creation
                    if (dbProduct == null)
                    {
                        baseResponse.Message = "Product could not be created";
                    }
                    else
                    {
                        baseResponse.Data = _mapper.Map<DomainModels.Product, AppModels.Product>(dbProduct);
                        baseResponse.Success = true;
                    }
                }
            }
            catch(Exception)
            {
                baseResponse.Message = "Something went wrong. Please contact support for more details";
            }

            return baseResponse;
        }

        public async Task<BaseResponse<IEnumerable<AppModels.Product>>> GetSellerProductsAsync(int sellerId)
        {
            BaseResponse<IEnumerable<AppModels.Product>> baseResponse = new();

            if (sellerId <= 0)
            {
                baseResponse.Message = "Invalid Seller Id";
            }
            else
            {
                try
                {
                    var dbProducts = await _productRepository.GetSellerProductsAsync(sellerId);
                    baseResponse.Data = _mapper.Map<IEnumerable<DomainModels.Product>, IEnumerable<AppModels.Product>>(dbProducts);
                    baseResponse.Success = true;
                }
                catch (Exception)
                {
                    baseResponse.Message = "Something went wrong. Please contact support for more details";
                }
            }

            return baseResponse;
        }

        public async Task<BaseResponse<IEnumerable<AppModels.Product>>> GetProductsAsync()
        {
            BaseResponse<IEnumerable<AppModels.Product>> baseResponse = new();

            try
            {
                var dbProducts = await _productRepository.GetProductsAsync();
                baseResponse.Data = _mapper.Map<IEnumerable<DomainModels.Product>, IEnumerable<AppModels.Product>>(dbProducts);
                baseResponse.Success = true;
            }
            catch (Exception)
            {
                baseResponse.Message = "Something went wrong. Please contact support for more details";
            }

            return baseResponse;
        }

        public async Task<BaseResponse<bool>> DeleteProductAsync(int productId, int userId)
        {
            BaseResponse<bool> baseResponse = new();

            try
            {
                var dbProduct = await _productRepository.GetProductAsync(productId);

                if (dbProduct == null)
                {
                    baseResponse.Message = "Invalid Product Id";
                }
                else if(dbProduct.SellerId != userId)
                {
                    baseResponse.Message = "Unauthorized delete";
                }
                else
                {
                    await _productRepository.DeleteProductAsync(dbProduct);

                    baseResponse.Data = true;
                    baseResponse.Success = true;
                }

            }
            catch(Exception)
            {
                baseResponse.Message = "Something went wrong. Please contact support for more details";
            }

            return baseResponse;
        }

        public async Task<BaseResponse<bool>> UpdateProductAsync(AppModels.Product product)
        {
            BaseResponse<bool> baseResponse = new();

            try
            {
                var dbProduct = await _productRepository.GetProductAsync(product.Id);

                if(dbProduct == null)
                {
                    baseResponse.Message = "Invalid Product Id";
                }
                else if(dbProduct.SellerId != product.SellerId)
                {
                    baseResponse.Message = "Unauthorized update";
                }
                else
                {
                    _mapper.Map(product, dbProduct);
                    await _productRepository.UpdateProductAsync(dbProduct);

                    baseResponse.Data = true;
                    baseResponse.Success = true;
                }
            }
            catch(Exception)
            {
                baseResponse.Message = "Something went wrong. Please contact support for more details";
            }

            return baseResponse;
        }

        #endregion << Public Methods >>

        #region << Private Methods >>

        private async Task<BaseResponse<AppModels.Product>> CreateProductValidationAsync(AppModels.Product product)
        {
            BaseResponse<AppModels.Product> response = new();

            if (product == null)
            {
                response.Message = "Product has not been provided";
                return response;
            }

            if (String.IsNullOrWhiteSpace(product.Name))
            {
                response.Message = "Product Name has not been provided";
                return response;
            }

            if (product.SellerId <= 0)
            {
                response.Message = "Seller ID has not been provided";
                return response;
            }

            if (product.Cost <= 0)
            {
                response.Message = "Cost has not been provided";
                return response;
            }

            if (product.Amount <= 0)
            {
                response.Message = "Amount has not been provided";
                return response;
            }

            var productExists = await _productRepository.ProductAvailableForSellerAsync(product.Name, product.SellerId);
            if (productExists)
            {
                response.Message = "Seller already has Product with same name";
            }

            return response;
        }

        #endregion << Private Methods >>
    }
}
