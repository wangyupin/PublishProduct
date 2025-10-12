using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using POVWebDomain.Models.ExternalApi.Store91;
using HqSrv.Application.Services.ExternalApiServices.Store91;
using POVWebDomain.Common;

namespace HqSrv.Application.Services.EcommerceServices
{
    public class Ecommerce91Service : IEcommerceService
    {
        private readonly Store91ExternalApiService __91Api;

        public Ecommerce91Service(Store91ExternalApiService _91api)
        {
            __91Api = _91api;
        }

        public async Task<Result<object>> SubmitGoodsAddAsync(object requestDto, string platformID)
        {
            try
            {
                __91Api.Configure(platformID);
                SubmitGoodsRequest request = (SubmitGoodsRequest)requestDto;

                var result = await __91Api.SubmitMain(request.MainRequest);
                if (result.IsFailure)
                    return Result<object>.Failure(result.Error);

                SubmitMainResponse skuResult = ((dynamic)result.Data).Data;

                // 上傳主圖片
                for (int idx = 0; idx < request.MainImage?.Count; idx++)
                {
                    IFormFile file = request.MainImage[idx];
                    if (file != null && file.Length > 0)
                    {
                        UpdateMainImageRequest image = new UpdateMainImageRequest()
                        {
                            Image = file,
                            Index = idx,
                            Id = skuResult.Id
                        };

                        var imageResult = await __91Api.UpdateMainImage(image);
                        if (imageResult.IsFailure)
                            return Result<object>.Failure(imageResult.Error);
                    }
                }

                // 上傳 SKU 圖片
                for (int idx = 0; idx < request.SkuImage?.Count; idx++)
                {
                    IFormFile file = request.SkuImage[idx];
                    if (file != null && file.Length > 0)
                    {
                        UpdateSKUImageRequest image = new UpdateSKUImageRequest()
                        {
                            Image = file,
                            Index = 0,
                            Id = skuResult.SkuList[idx].SkuId
                        };

                        var imageResult = await __91Api.UpdateSKUImage(image);
                        if (imageResult.IsFailure)
                            return Result<object>.Failure(imageResult.Error);
                    }
                }

                // 更新 SKU
                if (request.UpdateSaleProductSkuRequest != null)
                {
                    request.UpdateSaleProductSkuRequest.Id = skuResult.Id;
                    for (int idx = 0; idx < request.UpdateSaleProductSkuRequest.SkuList.Count; idx++)
                    {
                        request.UpdateSaleProductSkuRequest.SkuList[idx].Id = skuResult.SkuList[idx].SkuId;
                    }

                    var skuUpdateResult = await __91Api.UpdateSaleProductSku(request.UpdateSaleProductSkuRequest);
                    if (skuUpdateResult.IsFailure)
                        return Result<object>.Failure(skuUpdateResult.Error);
                }

                // 更新規格表
                if (request.SpecChartRequest.SalePageSpecChartId != null)
                {
                    request.SpecChartRequest.SalePageId = skuResult.Id;
                    var specResult = await __91Api.UpdateSpecChartId(request.SpecChartRequest);
                    if (specResult.IsFailure)
                        return Result<object>.Failure(specResult.Error);
                }

                // 操作品牌
                if (request.OperateBrandRequest.BrandId != null)
                {
                    request.OperateBrandRequest.SalePageIds = new List<long> { skuResult.Id };
                    var brandResult = await __91Api.OperateBrand(request.OperateBrandRequest);
                    if (brandResult.IsFailure)
                        return Result<object>.Failure(brandResult.Error);
                }

                return Result<object>.Success(skuResult);
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
                __91Api.Configure(platformID);
                SubmitGoodsEditRequest request = (SubmitGoodsEditRequest)requestDto;

                var result = await __91Api.UpdateMainDetail(request.MainRequest);
                if (result.IsFailure)
                    return Result<object>.Failure(result.Error);

                // 更新規格表
                if (request.SpecChartRequest != null)
                {
                    var specResult = await __91Api.UpdateSpecChartId(request.SpecChartRequest);
                    if (specResult.IsFailure)
                        return Result<object>.Failure(specResult.Error);
                }

                // 更新標題
                if (request.UpdateTitleRequest != null)
                {
                    var titleResult = await __91Api.UpdateTitle(request.UpdateTitleRequest);
                    if (titleResult.IsFailure)
                        return Result<object>.Failure(titleResult.Error);
                }

                // 更新 SKU
                if (request.UpdateSaleProductSkuRequest != null)
                {
                    var skuResult = await __91Api.UpdateSaleProductSku(request.UpdateSaleProductSkuRequest);
                    if (skuResult.IsFailure)
                        return Result<object>.Failure(skuResult.Error);
                }

                if (request.CreateSaleProductSkuRequest != null)
                {
                    var createSkuResult = await __91Api.CreateSaleProductSku(request.CreateSaleProductSkuRequest);
                    if (createSkuResult.IsFailure)
                        return Result<object>.Failure(createSkuResult.Error);

                    CreateSaleProductSkuResponse skuResult = ((dynamic)createSkuResult.Data).Data;
                    for (int idx = 0; idx < request.CreateSaleProductSkuRequest.SkuList.Count; idx++)
                    {
                        SkuList_All sku = request.CreateSaleProductSkuRequest.SkuList[idx];
                        UpdateSKUImageRequest imageReq = request.SkuImage.Find(t => t.Id == null);
                        imageReq.Id = skuResult.SkuIds[idx];
                        request.SkuList.SkuList.Add(new SkuListRes { OuterId = sku.OuterId, SkuId = (int)imageReq.Id });
                    }
                }

                if (request.MainImage != null)
                {
                    foreach (UpdateMainImageRequest req in request.MainImage)
                    {
                        result = await __91Api.UpdateMainImage(req);
                        if (result.IsFailure)
                            return Result<object>.Failure(result.Error);
                    }
                }

                if (request.SkuImage != null)
                {
                    foreach (UpdateSKUImageRequest req in request.SkuImage)
                    {
                        result = await __91Api.UpdateSKUImage(req);
                        if (result.IsFailure)
                            return Result<object>.Failure(result.Error);
                    }
                }
              
                return Result<object>.Success(result.Data);
            }
            catch (Exception ex)
            {
                return Result<object>.Failure(Error.Custom("SUBMIT_GOODS_EDIT_ERROR", ex.Message));
            }
        }
    }
}