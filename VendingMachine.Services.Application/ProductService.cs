using AutoMapper;
using System;
using System.Collections;
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
            BaseResponse<AppModels.Product> response = new();

            try
            {
                response = await CreateProductValidationAsync(product);
                if (String.IsNullOrWhiteSpace(response.Message))
                {
                    // Add new product
                    var newProduct = _mapper.Map<AppModels.Product, Domain.Product>(product);
                    var dbProduct = await _productRepository.CreateProductAsync(newProduct);

                    // Validate new product creation
                    if (dbProduct == null)
                    {
                        response.Message = "Product could not be created. for more help contact or support";
                    }
                    else
                    {
                        response.Data = _mapper.Map<DomainModels.Product, AppModels.Product>(dbProduct);
                        response.Success = true;
                    }
                }
            }
            catch(Exception)
            {

            }

            return response;
        }

        public async Task<BaseResponse<IEnumerable<AppModels.Product>>> GetSellerProductsAsync(int sellerId)
        {
            BaseResponse<IEnumerable<AppModels.Product>> response = new();

            if (sellerId <= 0)
            {
                response.Message = "Invalid Seller Id";
            }
            else
            {
                try
                {
                    var dbProducts = await _productRepository.GetSellerProductsAsync(sellerId);
                    response.Data = _mapper.Map<IEnumerable<DomainModels.Product>, IEnumerable<AppModels.Product>>(dbProducts);
                    response.Success = true;
                }
                catch (Exception)
                {
                    // TODO: CREATE LOG
                }
            }

            return response;
        }

        public async Task<BaseResponse<AppModels.Product>> GetProductAsync(int productId)
        {
            BaseResponse<AppModels.Product> response = new();

            try
            {
                var dbProduct = await _productRepository.GetProductAsync(productId);
                if(dbProduct == null)
                {
                    response.Message = "Invalid Product Id";
                }
                else
                {
                    response.Data = _mapper.Map<DomainModels.Product, AppModels.Product>(dbProduct);
                    response.Success = true;
                }
            }
            catch (Exception)
            {
                // TODO: CREATE LOG
            }

            return response;
        }

        public async Task<BaseResponse<bool>> DeleteProductAsync(int productId, int userId)
        {
            BaseResponse<bool> response = new();

            try
            {
                var dbProduct = await _productRepository.GetProductAsync(productId);

                if (dbProduct == null)
                {
                    response.Message = "Invalid Product Id";
                }
                else if(dbProduct.SellerId != userId)
                {
                    response.Message = "Unauthorized delete";
                }
                else
                {
                    await _productRepository.DeleteProductAsync(dbProduct);

                    response.Data = true;
                    response.Success = true;
                }

            }
            catch(Exception)
            {
                //TODO: CREATE LOG
            }

            return response;
        }

        public async Task<BaseResponse<bool>> UpdateProductAsync(AppModels.Product product)
        {
            BaseResponse<bool> response = new();

            try
            {
                var dbProduct = await _productRepository.GetProductAsync(product.Id);

                if(dbProduct == null)
                {
                    response.Message = "Invalid Product Id";
                }
                else if(dbProduct.SellerId != product.SellerId)
                {
                    response.Message = "Unauthorized update";
                }
                else
                {
                    _mapper.Map(product, dbProduct);
                    await _productRepository.UpdateProductAsync(dbProduct);

                    response.Data = true;
                    response.Success = true;
                }
            }
            catch(Exception)
            {

            }

            return response;
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
