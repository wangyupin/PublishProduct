using Microsoft.AspNetCore.Http;
using POVWebDomain.Models.ExternalApi.Store91;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using POVWebDomain.Models.API.StoreSrv.Common;
using POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods;

namespace POVWebDomain.Models.ExternalApi.Momo
{
    

    public class GetWebBrandRequest
    {
        public MomoLoginInfo LoginInfo { get; set; }
        public string WebBrandNo { get; set; }
        public string BrandChi { get; set; }
        public string BrandEng { get; set; }
    }
    public class GetWebBrandResponse
    {
        public string QC_YN { get; set; }
        public string BRAND_ENG { get; set; }
        public string BRAND_CHI { get; set; }
        public string WEB_BRAND_NO { get; set; }
    }

    public class GetOriginRequest
    {
        public MomoLoginInfo LoginInfo { get; set; }
        public string OriginCode { get; set; }
        public string OriginName { get; set; }
    }
    public class GetOriginResponse
    {
        public string ORIGIN_CODE { get; set; }
        public string ORIGIN_NAME { get; set; }
    }

    public class GetOutplaceRequest
    {
        public MomoLoginInfo LoginInfo { get; set; }
        public string OutplaceSeq { get; set; }
        public string Address { get; set; }
        public string Flag { get; set; }
    }
    public class GetOutplaceResponse
    {
        public string ADDR_FLAG { get; set; }
        public string OUTPLACE_SEQ { get; set; }
        public string OUTPLACE_POST { get; set; }
        public string DEFAULT_YN { get; set; }
        public string FULL_ADDR { get; set; }
    }

    public class GetGoodsSpecRequest
    {
        public MomoLoginInfo LoginInfo { get; set; }
        public string ColSeq { get; set; }
        public string ColName { get; set; }
    }
    public class GetGoodsSpecResponse
    {
        public string COL_SEQ { get; set; }
        public string COL_NAME { get; set; }
    }

    public class GetEcIndexRequest
    {
        public MomoLoginInfo LoginInfo { get; set; }
        public string EcCategoryCode { get; set; }
    }

    public class GetEcIndexResponse
    {
        public string ITEM_CONTENT { get; set; }
        public string CHECK_YN { get; set; }
        public string INDEX_ITEM_NO { get; set; }
        public string INDEX_NAME { get; set; }
        public string INDEX_NO { get; set; }

    }

    public class QueryGoodsListSendInfo
    {
        public string EntpGoodsNoQ { get; set; }
        public string DoFlag { get; set; }
        public string GoodsName { get; set; }
        public string FrDate { get; set; }
        public string ToDate { get; set; }
        public string GoodsCode { get; set; }
    }

    public class QueryGoodsListRequest
    {
        public string DoAction { get; set; }
        public MomoLoginInfo LoginInfo { get; set; }
        public QueryGoodsListSendInfo SendInfo { get; set; }
    }
    
    public class QueryGoodsListDataList
    {
        public string DoFlag { get; set; }
        public string GoodsCode { get; set; }
        public string SupGoodsCode { get; set; }
        public string SupGoodsName { get; set; }
    }
    public class QueryGoodsListResponse
    {
        public List<QueryGoodsListDataList> DataList { get; set; }
    }

    public class IndexList
    {
        public string IndexNo { get; set; }
        public string ChosenItemNo { get; set; }
    }

    public class BranchSnList
    {
        public string BranchSn { get; set; }
    }

    public class SingleItemList
    {
        public string ColDetail1 { get; set; } = string.Empty;
        public string ColDetail2 { get; set; } = string.Empty;
        public string SpecImg1 { get; set; }
        public string SpecImg2 { get; set; }
        public string EntpGoodsNo { get; set; }
        public string InternationalNo { get; set; }
        public string PrepareQty { get; set; }
        public string EcFirstQty { get; set; }
        public string EcMinQty { get; set; }
        public string EcLeadTime { get; set; }
        public string SpecifiedDate { get; set; }
        public string LastSaleDate { get; set; }
        public string BranchNameSingle { get; set; }
        public List<BranchSnList> BranchSnList { get; set; }
    }

    public class SizeIndexList
    {
        public string SizeIndex { get; set; }
    }

    public class TryIndexList
    {
        public string TryIndex { get; set; }
    }


    public class ClothData
    {
        public string _type { get; set; }
        public string _unit { get; set; }
        public string SizeItemCounts { get; set; }
        public List<SizeIndexList> SizeIndexList { get; set; }
        public string TryItemCounts { get; set; }
        public List<TryIndexList> TryIndexList { get; set; }
    }

    public class MobileDetailInfo
    {
        public List<string> YoutubeUrl { get; set; }
        public string Content { get; set; }
    }

    public class SendInfoList
    {
        public string BatchSupNo { get; set; }
        public string SupGoodsCode { get; set; }
        public string SupGoodsName_brand { get; set; }
        public string SupGoodsName_salePoint { get; set; }
        public string SupGoodsName_serial { get; set; }
        public string IsPrompt { get; set; }
        public string IsGift { get; set; }
        public string MdId { get; set; }
        public string MainEcCategoryCode { get; set; }
        public string WebBrandNo { get; set; }
        public string BuyPrice { get; set; }
        public string BuyCost { get; set; }
        public string SalePrice { get; set; }
        public string CustPrice { get; set; }
        public string GoodsType { get; set; }
        public string TemperatureType { get; set; }
        public string OriginCode { get; set; }
        public string Width { get; set; }
        public string Length { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public string HasAs { get; set; }
        public string AsDays { get; set; }
        public string AsNote { get; set; }
        public string IsECWarehouse { get; set; }
        public string SaleUnit { get; set; }
        public string IsPointReachDate { get; set; }
        public string IsCommission { get; set; }
        public string IsAcceptTravelCard { get; set; }
        public string IsIncludeInstall { get; set; }
        public string RecycleItem { get; set; }
        public string Comments { get; set; }
        public string ExpDays { get; set; }
        public string EtkPromoSdate { get; set; }
        public string EtkPromoEdate { get; set; }
        public string EtkOfflineDate { get; set; }
        public string TrustBankCode { get; set; }
        public string OutplaceSeq { get; set; }
        public string OutplaceSeqRtn { get; set; }
        public string GoodsSpec { get; set; }
        public string SaleNotice { get; set; }
        public string SupGoodsPcert { get; set; }
        public string Accessories { get; set; }
        public string GiftDesc { get; set; }
        public string Headline { get; set; }
        public string Content { get; set; }
        public string DetailInfo { get; set; }
        public string EcEntpReturnSeq { get; set; }
        public string InspectionEffectiveDate { get; set; }
        public string InspectionExpiredDate { get; set; }
        public string Main_achievement { get; set; }
        public string Youtube_url { get; set; }
        public string Agreed_delivery_yn { get; set; }
        public string Tax_yn { get; set; }
        public string Disc_mach_yn { get; set; }
        public string Gov_subsidize_yn { get; set; }
        public string ColSeq1 { get; set; }
        public string ColSeq2 { get; set; }
        public string DisplaySpecImg { get; set; }
        public string MomoMountYn { get; set; }
        public string MomoMountType { get; set; }
        public string MomoMountSpec { get; set; }
        public string BsmiExpired { get; set; }
        public string SaveWaterExpired { get; set; }
        public string FreightType { get; set; }
        public string FreightSize { get; set; }
        public string LargeMachineModel { get; set; }
        public string LiveStreamYn { get; set; }
        public List<IndexList> IndexList { get; set; }
        public List<SingleItemList> SingleItemList { get; set; }
        public ClothData ClothData { get; set; }
        public MobileDetailInfo MobileDetailInfo { get; set; }

    }

    public class TempReportGoodsRequest
    {
        public string DoAction { get; set; }
        public MomoLoginInfo LoginInfo { get; set; }
        public List<SendInfoList> SendInfoList { get; set; }
        public string ZipFileData { get; set; }

    }

    public class TempReportGoodsResultInfo
    {
        public int TotalCnt { get; set; }
        public int SuccessCnt { get; set; }
        public int FailCnt { get; set; }
        public List<string> FailList { get; set; }
    }

    public class TempReportGoodsResponse
    {
        public TempReportGoodsResultInfo ResultInfo { get; set; }
    }
    public class SubmitGoodsRequest
    {
        public TempReportGoodsRequest MainRequest { get; set; }
       
    }

    public class GoodsTypeReturn
    {
        public List<Option<string>> Options { get; set; }
        public Option<string> GoodsType { get; set; }
    }

    public class SaleUnitReturn
    {
        public List<Option<string>> Options { get; set; }
        public Option<string> SaleUnit { get; set; }
    }

    public class IndexItem
    {
        public string Value { get; set; }
        public string[] Items { get; set; }

        public IndexItem(string value, string[] items)
        {
            Value = value;
            Items = items;
        }
    }
    public class ClothDataReturn
    {
        public List<Option<string>> TypeOptions { get; set; }
        public List<Option<string>> UnitOptions { get; set; }
        public List<IndexItem> SizeIndex { get; set; }
        public List<IndexItem> TryIndex { get; set; }
    }
}
