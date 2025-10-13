using System;
using System.Threading.Tasks;
using POVWebDomain.Common;
using POVWebDomain.Models.ExternalApi.OfficialWebsite;
using HqSrv.Infrastructure.ExternalServices;

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

                var result = await _websiteApi.AddProduct(request.MainRequest);
                if (result.IsFailure)
                    return Result<object>.Failure(result.Error);

                var addResult = result.Data;

               

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
    }
}