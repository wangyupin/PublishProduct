using Microsoft.AspNetCore.Http;
using POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using POVWebDomain.Models.API.StoreSrv.Common;
using POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods;
using Newtonsoft.Json;

namespace POVWebDomain.Models.ExternalApi.Store91
{
    public class GetCategoryRequest
    {
        public long SourceCategoryId { get; set; }
    }
    public class GetCategoryResponse
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryLevel { get; set; }
    }

    public class GetShopCategoryRequest
    {
        public long ShopId { get; set; }
    }
    public class GetShopCategoryResponse
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryLevel { get; set; }
        public string CategoryStatus { get; set; }
    }
    public class GetPaymentRequest
    {
    }
    public class GetPaymentResponse
    {
        public string PayType { get; set; }
        public string PayTypeDesc { get; set; }
    }

    public class GetPaymentReturn
    {
        public List<CheckboxOption<string>> Options { get; set; }
        public List<PayTypes> PayTypes { get; set; }
    }

    public class GetShippingRequest
    {
        public long Id { get; set; }
    }
    public class GetShippingResponse
    {
        public long Id { get; set; }
        public string TypeName { get; set; }
        public string OrderDeliverType { get; set; }
        public string TemperatureTypeDef { get; set; }
        public string ShippingAreaName { get; set; }
        public string FeeTypeDef { get; set; }
        public bool IsEnableBookingPickupDate { get; set; }
        public string MargeTypeName { get; set; }
        public bool IsDefault { get; set; }
    }


    public class PointsPayPairsReq
    {
        public decimal PairsPrice { get; set; }
        public decimal PairsPoints { get; set; }
        public string OuterPromotionCode { get; set; }
    }

    public class SkuListReq
    {
        public string Name { get; set; }
        public int Qty { get; set; }
        public int OnceQty { get; set; }
        public string OuterId { get; set; }
        public int SafetyStockQty { get; set; }
    }
    public class SubmitMainRequest
    {
        public long ShopId { get; set; }
        public int CategoryId { get; set; }
        public List<int> MirrorCategoryIdList { get; set; }
        public int ShopCategoryId { get; set; }
        public List<long> MirrorShopCategoryIdList { get; set; }
        public string Title { get; set; }
        public DateTime? SellingStartDateTime { get; set; }
        public DateTime? SellingEndDateTime { get; set; }
        public string ApplyType { get; set; }
        public DateTime ExpectShippingDate { get; set; }
        public int ShippingPrepareDay { get; set; }
        public List<long> ShippingTypes { get; set; }
        public List<string> PayTypes { get; set; }
        public decimal SuggestPrice { get; set; }
        public decimal Price { get; set; }
        public decimal Cost { get; set; }
        public string ProductHighlight { get; set; } = string.Empty;
        public string ProductDescription { get; set; } = string.Empty;
        public string MoreInfo { get; set; }
        public string Brand { get; set; }
        public string Type { get; set; } = string.Empty;
        public object Specifications { get; set; }
        public bool HasSku { get; set; }
        public int? OnceQty { get; set; }
        public int? Qty { get; set; }
        public string OuterId { get; set; }
        public List<SkuListReq> SkuList { get; set; }
        public string TemperatureTypeDef { get; set; }
        public int Length { get; set; }
        public int WIdth { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public string Status { get; set; }
        public bool IsShowStockQty { get; set; }
        public string TaxTypeDef { get; set; } = "Taxable";
        public bool IsReturnable { get; set; } = true;
        public bool IsEnableBookingPickupDate { get; set; }
        public int? PrepareDays { get; set; }
        public int? AvailablePickupDays { get; set; }
        public DateTime? AvailablePickupStartDateTime { get; set; }
        public DateTime? AvailablePickupEndDateTime { get; set; }
        public string SEOTitle { get; set; } = null;
        public string SEOKeywords { get; set; } = null;
        public string SEODescription { get; set; } = null;
        public int SafetyStockQty { get; set; }
        public bool IsShowPurchaseList { get; set; }
        public bool IsShowSoldQty { get; set; }
        public bool IsDesignatedReturnGoodsType { get; set; } = false;
        public List<string> ReturnGoodsType { get; set; } = null;
        public string SoldOutActionType { get; set; }
        public bool IsRestricted { get; set; }
        public int SalesModeTypeDef { get; set; }
        public List<PointsPayPairsReq> PointsPayPairs { get; set; }
    }

    public class SkuListRes
    {
        public int SkuId { get; set; }
        public string OuterId { get; set; }
    }
    public class SubmitMainResponse
    {
        public int Id { get; set; }
        public List<SkuListRes> SkuList { get; set; }
    }

    public class SkuList_All
    {
        public string Name { get; set; }
        public int Sort { get; set; }
        public int Qty { get; set; }
        public int OnceQty { get; set; }
        public string OuterId { get; set; }
        public bool IsShow { get; set; }
        public decimal SuggestPrice { get; set; }
        public decimal Price { get; set; }
        public decimal Cost { get; set; }
        public int SafetyStockQty { get; set; }
    }
    public class CreateSaleProductSkuRequest {
        public int Id { get; set; }
        public bool IsSkuDifferentPrice { get; set; }
        public List<SkuList_All> SkuList { get; set; }

    }

    public class CreateSaleProductSkuResponse
    {
        public int Id { get; set; }
        public List<long> SkuIds { get; set; }

    }

    public class UpdateMainImageRequest
    {
        public long Id { get; set; }
        public int Index { get; set; }

        private IFormFile _image;
        public IFormFile Image
        {
            get { return _image; }
            set
            {
                if (value != null)
                {
                    _image = ImageCompressor.CompressAndResizeImage(value, 300 * 1024, 500, 500);
                }
                else
                {
                    _image = null;
                }
            }
        }
    }

    public class UpdateSKUImageRequest
    {
        public long? Id { get; set; }
        public int Index { get; set; }
        private IFormFile _image;
        public IFormFile Image
        {
            get { return _image; }
            set
            {
                if (value != null)
                {
                    _image = ImageCompressor.CompressAndResizeImage(value, 200 * 1024, 400, 400);
                }
                else
                {
                    _image = null;
                }
            }
        }
    }

    public class StringContentWithoutContentType : HttpContent
    {
        private readonly StringContent _stringContent;

        public StringContentWithoutContentType(string content)
        {
            _stringContent = new StringContent(content);
            Headers.Clear();
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            return _stringContent.CopyToAsync(stream, context);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = _stringContent.Headers.ContentLength.GetValueOrDefault();
            return true;
        }
    }

    public class SearchItem
    {
        public long ShopId { get; set; }
        public long? SalePageSpecChartId { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
    }

    public class SalePageSpecChartGetListRequest
    {
        public SearchItem SearchItem { get; set; }
        public int Take { get; set; }
        public int Skip { get; set; }

    }

    public class SalePageSpecChartGetListResponse
    {
        public List<SearchItem> List { get; set; }
        public int PageCount { get; set; }
        public int TotalCount { get; set; }

    }

    public class UpdateSpecChartIdRequest
    {
        public long ShopId { get; set; }
        public long SalePageId { get; set; }
        public long? SalePageSpecChartId { get; set; }
    }


    public class SubmitGoodsRequest
    {
        public SubmitMainRequest MainRequest { get; set; }
        public UpdateSpecChartIdRequest SpecChartRequest { get; set; }
        public UpdateSaleProductSkuRequest UpdateSaleProductSkuRequest { get; set; }
        public List<IFormFile> MainImage { get; set; }
        public List<IFormFile> SkuImage { get; set; }
        public OperateBrandRequest OperateBrandRequest { get; set; }
    }

    public class UpdateMainDetailRequest
    {
        public long Id { get; set; }
        public long ShopId { get; set; }
        public int CategoryId { get; set; }
        public int ShopCategoryId { get; set; }
        public DateTime? SellingStartDateTime { get; set; }
        public DateTime? SellingEndDateTime { get; set; }
        public List<long> ShippingTypes { get; set; }
        public List<string> PayTypes { get; set; }
        public string Brand { get; set; }
        public string Type { get; set; }
        public object Specifications { get; set; }
        public string ProductHighlight { get; set; }
        public string ProductDescription { get; set; }
        public string MoreInfo { get; set; }
        public string SEOTitle { get; set; }
        public string SEOKeywords { get; set; }
        public string SEODescription { get; set; }
        public string TemperatureTypeDef { get; set; }
        public int Length { get; set; }
        public int WIdth { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public string Status { get; set; }
        public bool IsShowStockQty { get; set; }
        public string TaxTypeDef { get; set; }
        public bool IsReturnable { get; set; }
        public bool IsEnableBookingPickupDate { get; set; }
        public int? PrepareDays { get; set; }
        public int? AvailablePickupDays { get; set; }
        public DateTime? AvailablePickupStartDateTime { get; set; }
        public DateTime? AvailablePickupEndDateTime { get; set; }
        public string ApplyType { get; set; }
        public DateTime? ExpectShippingDate { get; set; }
        public string ShippingPrepareDay { get; set; }
        public bool IsShowPurchaseList { get; set; }
        public bool IsShowSoldQty { get; set; }
        public bool IsDesignatedReturnGoodsType { get; set; }
        public List<string> ReturnGoodsType { get; set; }
        public string SoldOutActionType { get; set; }
        public bool IsRestricted { get; set; }
        public int SalesModeTypeDef { get; set; }
        public List<PointsPayPairsReq> PointsPayPairs { get; set; }
    }

    public class UpdateTitleRequest
    {
        public long Id { get; set; }
        public string Title { get; set; }
    }

    public class UpdatePriceRequest
    {
        public long Id { get; set; }
        public long SkuId { get; set; }
        public decimal SuggestPrice { get; set; }
        public decimal Price { get; set; }
        public decimal Cost { get; set; }
    }

    public class UpdateSkuDetailRequest
    {
        public long SalePageId { get; set; }
        public long SkuId { get; set; }
        public string OuterId { get; set; }
        public bool IsShow { get; set; }
        public int OnceQty { get; set; }
        public int SafetyStockQty { get; set; }
    }

    public class UpdateSellingQtyRequest
    {
        public long Id { get; set; }
        public long SkuId { get; set; }
        public string OuterId { get; set; }
        public int SellingQty { get; set; }
    }

    public class UpdSku
    {
        public long Id { get; set; }
        public int? ChangeQty { get; set; }
        public int Sort { get; set; }
        public int OnceQty { get; set; }
        public string OuterId { get; set; }
        public bool IsShow { get; set; }
        public decimal? SuggestPrice { get; set; }
        public decimal? Price { get; set; }
        public decimal? Cost { get; set; }
        public int? SafetyStockQty { get; set; }
    }
    public class UpdateSaleProductSkuRequest
    {
        public int Id { get; set; }
        public bool IsSkuDifferentPrice { get; set; }
        public List<UpdSku> SkuList { get; set; }
    }

    public class UpdateSaleProductSkuResponse
    {
        public int Id { get; set; }
        public List<long> SkuIds { get; set; }
    }


    public class SubmitGoodsEditRequest
    {
        public UpdateMainDetailRequest MainRequest { get; set; }
        public UpdateSpecChartIdRequest SpecChartRequest { get; set; }
        public UpdateTitleRequest UpdateTitleRequest { get; set; }
        public UpdateSaleProductSkuRequest UpdateSaleProductSkuRequest { get; set; }
        public List<UpdateMainImageRequest> MainImage { get; set; }
        public List<UpdateSKUImageRequest> SkuImage { get; set; }
        public CreateSaleProductSkuRequest CreateSaleProductSkuRequest { get; set; }
        public SubmitMainResponse SkuList { get; set; }
    }

    public class SaleModeTypeReturn
    {
        public List<CheckboxOption<int>> Options { get; set; }
        public List<SaleModeTypes> SalesModeTypeDef { get; set; }
    }

    public class SellingDateTimeReturn
    {
        public List<Option<int>> Options { get; set; }
        public Option<int> SellingDateTime { get; set; }
    }

    public class OperateBrandRequest
    {
        public long ShopId { get; set; }
        public string BrandId { get; set; }
        public List<long> SalePageIds { get; set; }
        public string ModifyType { get; set; }
    }
}
