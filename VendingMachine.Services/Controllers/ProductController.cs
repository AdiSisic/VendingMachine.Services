using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VendingMachine.Services.Api.Base;
using VendingMachine.Services.Api.Product;
using VendingMachine.Services.Api.Product.Request;
using VendingMachine.Services.Api.Product.Response;
using VendingMachine.Services.Application.Abstractions;
using VendingMachine.Services.Application.Models;
using VendingMachine.Services.Attributes;

namespace VendingMachine.Services.Controllers
{
    [Route("api/[controller]")]
    [ApiController, YwtAuthorization]
    public class ProductController : ControllerBase
    {
        #region << Fields >>

        private readonly IMapper _mapper;
        private readonly IProductService _productService;

        #endregion << Fields >>

        #region << Constructor >>

        public ProductController(IMapper mapper, IProductService productService)
        {
            _mapper = mapper;
            _productService = productService;
        }

        #endregion << Constructor >>

        /// <summary>
        /// Create new product
        /// </summary>
        /// <param name="request">Create Product request</param>
        /// <returns></returns>
        [HttpPost, Route("createProduct", Name = "CreateProduct")]
        public async Task<BaseResponse<CreateProductResponse>> CreateProduct([FromBody] ManipulateProductRequest request)
        {
            if (!ModelState.IsValid)
            {
                return new BaseResponse<CreateProductResponse>() { Message = "Bad Request" };
            }

            var claimUserId = User.Claims.Where(x => x.Type == "UserId").Select(x => x.Value).FirstOrDefault();
            Int32.TryParse(claimUserId, out int userId);

            var product = _mapper.Map<ManipulateProductRequest, Product>(request);
            product.SellerId = userId;

            var serivceResponse = await _productService.CreateProductAsync(product);
            BaseResponse<CreateProductResponse> response = new() { Success = serivceResponse.Success, Message = serivceResponse.Message };

            // If product created
            if(response.Success)
            {
                response.Data = _mapper.Map<Product, CreateProductResponse>(product);
            }

            return response;
        }

        /// <summary>
        /// This method returns all seller products
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("getSellerProducts", Name = "GetSellerProducts")]
        public async Task<BaseResponse<IEnumerable<GetProductResponse>>> GetSellerProducts()
        {
            var claimSellerId = User.Claims.Where(x => x.Type == "UserId").Select(x => x.Value).FirstOrDefault();
            Int32.TryParse(claimSellerId, out int sellerId);

            if (sellerId <= 0)
            {
                return new BaseResponse<IEnumerable<GetProductResponse>> { Message = "Bad Request" };
            }

            var serviceResponse = await _productService.GetSellerProductsAsync(sellerId);
            BaseResponse<IEnumerable<GetProductResponse>> response = new() { Success = serviceResponse.Success, Message = serviceResponse.Message };

            if (response.Success)
            {
                response.Data = _mapper.Map<IEnumerable<Product>, IEnumerable<GetProductResponse>>(serviceResponse.Data);
            }

            return response;
        }

        /// <summary>
        /// Get Product by Product ID
        /// </summary>
        /// <param name="productId">Product ID</param>
        /// <returns></returns>
        [HttpGet, Route("{productId}", Name = "GetProduct"), AllowAnonymous]
        public async Task<BaseResponse<GetProductResponse>> GetProduct([FromRoute] int productId)
        {
            if(productId <= 0)
            {
                return new BaseResponse<GetProductResponse>() { Message = "Bad Request" };
            }

            var serviceResponse = await _productService.GetProductAsync(productId);
            BaseResponse<GetProductResponse> response = new() { Success = serviceResponse.Success, Message = serviceResponse.Message };

            if (response.Success)
            {
                response.Data = _mapper.Map<Product, GetProductResponse>(serviceResponse.Data);
            }

            return response;
        }

        /// <summary>
        /// Delete Product
        /// </summary>
        /// <param name="productId">Product Id</param>
        /// <returns></returns>
        [HttpDelete, Route("deleteProduct/{productId}", Name = "DeleteProduct")]
        public async Task<BaseResponse<bool>> DeleteProduct([FromRoute] int productId)
        {
            if(productId <= 0)
            {
                return new BaseResponse<bool>() { Message = "Bad Request" };
            }

            var claimUserId = User.Claims.Where(x => x.Type == "UserId").Select(x => x.Value).FirstOrDefault();
            Int32.TryParse(claimUserId, out int userId);

            return await _productService.DeleteProductAsync(productId, userId);
        }

        /// <summary>
        /// Update existing product
        /// </summary>
        /// <param name="productId">Product Id</param>
        /// <param name="request">Request parameters</param>
        /// <returns></returns>
        [HttpPut, Route("updateProduct/{productId}", Name = "UpdateProduct")]
        public async Task<BaseResponse<bool>> UpdateProduct([FromRoute] int productId, [FromBody] ManipulateProductRequest request)
        {
            if(productId <= 0 || !ModelState.IsValid && request.Amount >= 0)
            {
                return new BaseResponse<bool>() { Message = "Bad Request" };
            }

            var claimUserId = User.Claims.Where(x => x.Type == "UserId").Select(x => x.Value).FirstOrDefault();
            Int32.TryParse(claimUserId, out int userId);

            var product = _mapper.Map<ManipulateProductRequest, Product>(request);
            product.Id = productId;
            product.SellerId = userId;

            return await _productService.UpdateProductAsync(product);
        }
    }
}
