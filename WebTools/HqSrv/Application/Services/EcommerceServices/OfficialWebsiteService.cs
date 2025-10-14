using Azure.Core;
using HqSrv.Domain.Entities;
using HqSrv.Infrastructure.ExternalServices;
using HqSrv.Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using POVWebDomain.Common;
using POVWebDomain.Models.ExternalApi.OfficialWebsite;
using System;
using System.Collections.Generic;
using System.Linq;
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

                var addResult = result.Data.Data;
                int productID = addResult.ProductID;
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
                if (request.SkuImage != null && request.SkuImage.Count > 0 && addResult.ProductOpton?.Count > 0)
                {
                    var updateSkuImageRequest = new UpdateProductOptionImageRequest
                    {
                        StoreNumber = storeNumber,
                        ProductID = productID,
                        Data = new List<ProductOptionImageData>()
                    };

                    for (int idx = 0; idx < request.SkuImage.Count && idx < addResult.ProductOpton.Count; idx++)
                    {
                        IFormFile file = request.SkuImage[idx];

                        if (file != null && file.FileName != "blob")
                        {
                            updateSkuImageRequest.Data.Add(new ProductOptionImageData
                            {
                                SkuID = addResult.ProductOpton[idx].SkuID,
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

                // 1. 更新商品主資料
                var result = await _websiteApi.UpdateProduct(request.MainRequest);
                if (result.IsFailure)
                    return Result<object>.Failure(result.Error);

                var productId = request.MainRequest.ProductID;

                //// 2. 更新商品選項（只有在有選項時才呼叫）
                //if (request.UpdateOptionsRequest != null)
                //{
                //    var updateOptionResult = await _websiteApi.UpdateProductOption(request.UpdateOptionsRequest);
                //    if (updateOptionResult.IsFailure)
                //        return Result<object>.Failure(updateOptionResult.Error);
                //}

                // 4. 上傳主圖片
                if (request.MainImage?.Any() == true)
                {
                    var imageData = request.MainImage.Select((file, index) => new ProductImageData
                    {
                        DisplayOrder = index + 1,
                        ImageFile = file
                    }).ToList();

                    var imageRequest = new UpdateProductImageRequest
                    {
                        StoreNumber = request.MainRequest.StoreNumber,
                        ProductID = productId,
                        Data = imageData
                    };

                    var imageResult = await _websiteApi.UpdateProductImage(imageRequest);
                    if (imageResult.IsFailure)
                        return Result<object>.Failure(imageResult.Error);
                }

                //// 5. 上傳 SKU 圖片
                if (request.UpdateOptionsRequest != null && request.SkuImage?.Any() == true)
                {
                    var skuImageData = new List<ProductOptionImageData>();

                    for (int i = 0; i < request.SkuImage.Count; i++)
                    {
                        var file = request.SkuImage[i];
                        if (file != null && file.Length > 0)
                        {
                            // 取得對應的 SkuID
                            var correspondingSkuOption = request.UpdateOptionsRequest.ProductOptionList.ElementAtOrDefault(i);
                            if (correspondingSkuOption != null && correspondingSkuOption.SkuID.HasValue)
                            {
                                skuImageData.Add(new ProductOptionImageData
                                {
                                    SkuID = correspondingSkuOption.SkuID.Value,
                                    DisplayOrder = 1, // 每個 SKU 的第一張圖
                                    ImageFile = file
                                });
                            }
                        }
                    }

                    if (skuImageData.Any())
                    {
                        var skuImageRequest = new UpdateProductOptionImageRequest
                        {
                            StoreNumber = request.MainRequest.StoreNumber,
                            ProductID = productId,
                            Data = skuImageData
                        };

                        var skuImageResult = await _websiteApi.UpdateProductOptionImage(skuImageRequest);
                        if (skuImageResult.IsFailure)
                            return Result<object>.Failure(skuImageResult.Error);
                    }
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