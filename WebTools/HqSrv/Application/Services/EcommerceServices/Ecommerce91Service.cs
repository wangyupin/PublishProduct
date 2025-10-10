using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System;
using POVWebDomain.Models.ExternalApi.Store91;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using HqSrv.Application.Services.ExternalApiServices.Store91;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace HqSrv.Application.Services.EcommerceServices
{
   
    public class Ecommerce91Service: IEcommerceService
    {
        private readonly Store91ExternalApiService __91Api;
        public Ecommerce91Service(Store91ExternalApiService _91api)
        {
            __91Api = _91api;
        }

        public async Task<(object, string)> SubmitGoodsAdd(object requestDto, string platformID)
        {
            __91Api.Configure(platformID);
            SubmitGoodsRequest request = (SubmitGoodsRequest)requestDto;

            var result = await __91Api.SubmitMain(request.MainRequest);

            if (!string.IsNullOrEmpty(result.Item2)) throw new Exception(result.Item2);

            SubmitMainResponse skuResult = ((dynamic)result.Item1).Data;

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

                    result = await __91Api.UpdateMainImage(image);
                    if (!string.IsNullOrEmpty(result.Item2)) return (skuResult, result.Item2);
                }
            }

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

                    result = await __91Api.UpdateSKUImage(image);
                    if (!string.IsNullOrEmpty(result.Item2)) return (skuResult, result.Item2);
                }
            }

            if (request.UpdateSaleProductSkuRequest != null)
            {
                request.UpdateSaleProductSkuRequest.Id = skuResult.Id;
                for (int idx = 0; idx < request.UpdateSaleProductSkuRequest.SkuList.Count; idx++)
                {
                    request.UpdateSaleProductSkuRequest.SkuList[idx].Id = skuResult.SkuList[idx].SkuId;
                }

                result = await __91Api.UpdateSaleProductSku(request.UpdateSaleProductSkuRequest);
                if (!string.IsNullOrEmpty(result.Item2)) return (skuResult, result.Item2);
            }


            if (request.SpecChartRequest.SalePageSpecChartId != null)
            {
                request.SpecChartRequest.SalePageId = skuResult.Id;
                result = await __91Api.UpdateSpecChartId(request.SpecChartRequest);
                if (!string.IsNullOrEmpty(result.Item2)) return (skuResult, result.Item2);
            }

            if (request.OperateBrandRequest.BrandId != null)
            {
                request.OperateBrandRequest.SalePageIds = new List<long> { skuResult.Id };
                result = await __91Api.OperateBrand(request.OperateBrandRequest);
                if (!string.IsNullOrEmpty(result.Item2)) return (skuResult, result.Item2);
            }

            return (skuResult, "");
        }

        public async Task<(object, string)> SubmitGoodsEdit(object requestDto, string platformID)
        {
            __91Api.Configure(platformID);
            SubmitGoodsEditRequest request = (SubmitGoodsEditRequest)requestDto;

            var result = await __91Api.UpdateMainDetail(request.MainRequest);

            if (!string.IsNullOrEmpty(result.Item2)) throw new Exception(result.Item2);

            if (request.SpecChartRequest != null)
            {
                result = await __91Api.UpdateSpecChartId(request.SpecChartRequest);
                if (!string.IsNullOrEmpty(result.Item2)) throw new Exception(result.Item2);
            }

            if (request.UpdateTitleRequest != null)
            {
                result = await __91Api.UpdateTitle(request.UpdateTitleRequest);
                if (!string.IsNullOrEmpty(result.Item2)) throw new Exception(result.Item2);
            }

            if (request.UpdateSaleProductSkuRequest != null)
            {
                result = await __91Api.UpdateSaleProductSku(request.UpdateSaleProductSkuRequest);
                if (!string.IsNullOrEmpty(result.Item2)) throw new Exception(result.Item2);
            }

            if (request.CreateSaleProductSkuRequest != null)
            {
                result = await __91Api.CreateSaleProductSku(request.CreateSaleProductSkuRequest);
                if (!string.IsNullOrEmpty(result.Item2)) throw new Exception(result.Item2);

                CreateSaleProductSkuResponse skuResult = ((dynamic)result.Item1).Data;
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
                    if (!string.IsNullOrEmpty(result.Item2)) throw new Exception(result.Item2);
                }
            }

            if (request.SkuImage != null)
            {
                foreach (UpdateSKUImageRequest req in request.SkuImage)
                {
                    result = await __91Api.UpdateSKUImage(req);
                    if (!string.IsNullOrEmpty(result.Item2)) throw new Exception(result.Item2);
                }
            }

            return (request.SkuList, "");
        }
    }
}
