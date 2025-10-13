using Azure.Core;
using HqSrv.Infrastructure.ExternalServices;
using HqSrv.Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using POVWebDomain.Common;
using POVWebDomain.Models.ExternalApi.OfficialWebsite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HqSrv.Application.Services.EcommerceServices
{
    public class OfficialWebsiteService : IEcommerceService
    {
        private readonly OfficialWebsiteExternalApiService _websiteApi;

        public OfficialWebsiteService(OfficialWebsiteExternalApiService websiteApi)
        {
            _websiteApi = websiteApi;
        }

        public async Task<Result<object>> SubmitGoodsAddAsync(object requestDto, string platformID)
        {
            try
            {
                _websiteApi.Configure(platformID);
                SubmitGoodsRequest request = (SubmitGoodsRequest)requestDto;

                // 1. 先新增產品
                var result = await _websiteApi.AddProduct(request.MainRequest);
                if (result.IsFailure)
                    return Result<object>.Failure(result.Error);

                var addResult = result.Data;
                int productID = addResult.Data.ProductID;
                int storeNumber = request.MainRequest.StoreNumber;

                // 2. 上傳主圖 - 簡化邏輯
                if (request.MainImage != null && request.MainImage.Count > 0)
                {
                    var updateMainImageRequest = new UpdateProductImageRequest
                    {
                        StoreNumber = storeNumber,
                        ProductID = productID,
                        Data = new List<ProductImageData>()
                    };

                    for (int idx = 0; idx < request.MainImage.Count; idx++)
                    {
                        IFormFile file = request.MainImage[idx];

                        // 只要不是 blob 就上傳 (前面已經處理過了)
                        if (file != null && file.FileName != "blob")
                        {
                            updateMainImageRequest.Data.Add(new ProductImageData
                            {
                                DisplayOrder = idx + 1,
                                ImageFile = file
                            });
                        }
                    }

                    if (updateMainImageRequest.Data.Count > 0)
                    {
                        var mainImageResult = await _websiteApi.UpdateProductImage(updateMainImageRequest);
                        if (mainImageResult.IsFailure)
                            return Result<object>.Failure(mainImageResult.Error);
                    }
                }

                // 3. 上傳 SKU 圖片
                if (request.SkuImage != null && request.SkuImage.Count > 0 && addResult.Data.ProductOpton?.Count > 0)
                {
                    var updateSkuImageRequest = new UpdateProductOptionImageRequest
                    {
                        StoreNumber = storeNumber,
                        Data = new List<ProductOptionImageData>()
                    };

                    for (int idx = 0; idx < request.SkuImage.Count && idx < addResult.Data.ProductOpton.Count; idx++)
                    {
                        IFormFile file = request.SkuImage[idx];

                        if (file != null && file.FileName != "blob")
                        {
                            updateSkuImageRequest.Data.Add(new ProductOptionImageData
                            {
                                SkuID = addResult.Data.ProductOpton[idx].SkuID,
                                DisplayOrder = 1,
                                ImageFile = file
                            });
                        }
                    }

                    if (updateSkuImageRequest.Data.Count > 0)
                    {
                        var skuImageResult = await _websiteApi.UpdateProductOptionImage(updateSkuImageRequest);
                        if (skuImageResult.IsFailure)
                            return Result<object>.Failure(skuImageResult.Error);
                    }
                }

                return Result<object>.Success(addResult);
            }
            catch (Exception ex)
            {
                return Result<object>.Failure(Error.Custom("SUBMIT_GOODS_ADD_ERROR", ex.Message));
            }
        }

        public async Task<Result<object>> SubmitGoodsEditAsync(object requestDto, string platformID)
        {
            try
            {
                _websiteApi.Configure(platformID);
                SubmitGoodsEditRequest request = (SubmitGoodsEditRequest)requestDto;

                var result = await _websiteApi.UpdateProduct(request.MainRequest);
                if (result.IsFailure)
                    return Result<object>.Failure(result.Error);

                // 更新商品選項
                if (request.UpdateProductOptionRequest != null)
                {
                    var optionResult = await _websiteApi.UpdateProductOption(request.UpdateProductOptionRequest);
                    if (optionResult.IsFailure)
                        return Result<object>.Failure(optionResult.Error);
                }

                return Result<object>.Success(result.Data);
            }
            catch (Exception ex)
            {
                return Result<object>.Failure(Error.Custom("SUBMIT_GOODS_EDIT_ERROR", ex.Message));
            }
        }

        public async Task<Result<object>> DeleteGoodsAsync(int storeNumber, int productID, string platformID)
        {
            try
            {
                _websiteApi.Configure(platformID);

                var request = new RemoveProductRequest
                {
                    StoreNumber = storeNumber,
                    ProductID = productID
                };

                var result = await _websiteApi.RemoveProduct(request);
                if (result.IsFailure)
                    return Result<object>.Failure(result.Error);

                return Result<object>.Success(result.Data);
            }
            catch (Exception ex)
            {
                return Result<object>.Failure(Error.Custom("DELETE_GOODS_ERROR", ex.Message));
            }
        }
    }
}