using Microsoft.Extensions.Configuration;
using POVWebDomain.Models.API.StoreSrv.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods;

namespace POVWebDomain.Models.ExternalApi
{
    public class YahooApiKey
    {
        public string Token { get; set; }
        public string KeyValue { get; set; }
        public string KeyIV { get; set; }
        public string SaltKey { get; set; }
        public string KeyVersion { get; set; }
    }
    public class StoreDeliveryConfirmOrderRequest
    {
        public string OrderCode { get; set; }
        public int PackageNo { get; set; }
    }
    public class SuccessfulOrder
    {
        public int PackageNo { get; set; }
        public string ShipmentNo { get; set; }
        public string BarCode { get; set; }
        public DateTime ShipDate { get; set; }
        public List<string> OrderCodes { get; set; }

        //全家新增欄位
        public string FirstBarcode { get; set; }
        public string RoutingBarcode { get; set; }
        public string PickupEShopBarcode { get; set; }
        public string PickupCodBarcode { get; set; }
        public string PickupLogisticCode { get; set; }
        public string LogisticCode { get; set; }
        public string LogisticCheckSum { get; set; }
        public string QrCode { get; set; }
        public string StoreEquimentId { get; set; }
        public string StoreRegion { get; set; }
        public string StoreRoute { get; set; }
        public string StoreRouteTrim { get; set; }
        public string ReturnPeriod { get; set; }
        public string ReturnType { get; set; }
        public string MobilePhone { get; set; }
        public string ConvenienceStoreId { get; set; }
        public string ConvenienceStoreName { get; set; }
        public string OrderInformation { get; set; }
        public string LogisticOrderNumber { get; set; }
        public string DistributionCenter { get; set; }
        public string CustomerInformation { get; set; }
        public string Remark { get; set; }
        public string OfficialName { get; set; }
        public string ChildShopcode { get; set; }
    }

    public class FailedOrder
    {
        public int PackageNo { get; set; }
        public string FailedCode { get; set; }
        public string FailedMessage { get; set; }
        public List<string> OrderCodes { get; set; }
    }
    public class StoreDeliveryConfirmOrderResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public int SuccessfulCount { get; set; }
        public int FailedCount { get; set; }
        public List<SuccessfulOrder> SuccessfulOrders { get; set; }
        public List<FailedOrder> FailedOrders { get; set; }
    }


    public class ThirdDeliveryConfirmOrderRequest
    {
        public string OrderCode { get; set; }
        public int PackageNo { get; set; }
        public int DeliveryType { get; set; }
    }

    public class ThirdSuccessfulOrder
    {
        public int PackageNo { get; set; }
        public string ShipmentNo { get; set; }
        public string BarCode { get; set; }
        public List<string> OrderCodes { get; set; }
        public DateTime ShippingDate { get; set; }

        public int PackageSerialNo { get; set; }
        public string Code2 { get; set; }
        public string Code3 { get; set; }
        public string Code4 { get; set; }
        public string Code5 { get; set; }
        public string QRCode { get; set; }

        public string MDCode1 { get; set; }
        public string MDCode2 { get; set; }
        public string MDCode3 { get; set; }
        public int TransporterId { get; set; }
        public string LocationOfficeId { get; set; }

        public string LocationOfficeName { get; set; }

    }


    public class ThirdDeliveryConfirmOrderResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public int SuccessfulCount { get; set; }
        public int FailedCount { get; set; }
        public List<ThirdSuccessfulOrder> SuccessfulOrders { get; set; }
        public List<FailedOrder> FailedOrders { get; set; }
    }

    public class HomeDeliveryConfirmOrderRequest
    {
        public string OrderCode { get; set; }
        public int ShipperId { get; set; }
        public string ShipCode { get; set; }
    }

    #region 上架用的model
 
    public class CategoriesRequest
    {
        public string CategoryId { get; set; }
        public bool IsGift { get; set; }
        public string Fields { get; set; }
    }

    public class CategoriesResponse
    {
        public List<Category> Categories { get; set; }
        public List<Category> Parents { get; set; }
        public List<Category> Children { get; set; }
    }
    public class Category
    {
        public string CategoryId { get; set; }
        public string Name { get; set; }
        public string ParentId { get; set; }
        public List<string> ChildrenIdList { get; set; }
        public bool IsVideoGame { get; set; }
        public bool IsSexToy { get; set; }
        public List<string> DeliveryTypes { get; set; }
        public List<string> Functions { get; set; }
        public bool IsGift { get; set; }
        public ContactWindow CategoryManager { get; set; }
        public ContactWindow MerchandiseManager { get; set; }
        public ContactWindow ProductManager { get; set; }
        public bool Visible { get; set; }

    }
    public class ContactWindow
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public Location Location { get; set; }

    }

    public class Location
    {
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Address { get; set; }
    }

    public class Proposals
    {
        public string Applicant { get; set; } //申請人
        public string SubStationId { get; set; } //提案子站ID
        public string Type { get; set; } //提案單類型
        public string Note { get; set; } //備註
        public string ReviewStatus { get; set; } //審核結果
        public ProposalListing Listing { get; set; } //賣場資訊
        public ProposalProduct Product { get; set; } //商品資訊

    }

    public class Proposal
    {
        public int Id { get; set; }
        public int SeqNo { get; set; }
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string Creator { get; set; }
        public string ContactWindow { get; set; }
        public string Type { get; set; }
        public string Applicant { get; set; }
        public string SubStationId { get; set; }
        public string SubStationName { get; set; }
        public string ExpiredTs { get; set; }
        public ProposalProduct Product { get; set; }
        public ProposalListing Listing { get; set; }
        public string CreatedTs { get; set; }
        public string AppliedTs { get; set; }
        public string Reviewer { get; set; }
        public string ReviewStatus { get; set; }
        public string ReviewedTs { get; set; }
        public List<string> DeclineReason { get; set; }
        public string DeclineDetail { get; set; }
        public string ExecuteStatus { get; set; }
        public string ExecutedTs { get; set; }
        public string ExecuteResult { get; set; }
        public int ModifiedTimes { get; set; }
        public string Modifier { get; set; }
        public string ModifiedTs { get; set; }
        public string Note { get; set; }
        public string Suggestion { get; set; }
        public ParentProposal Parent { get; set; }
        public BatchProposalInfo Batch { get; set; }
    }

    public class ProposalListing
    {
        public int Id { get; set; } //賣場 ID
        public string ZoneId { get; set; } //賣場目前的分類區 ID
        public string ZoneName { get; set; } //賣場目前的分類區名
        public string SubStationId { get; set; } //賣場目前的分類子站 ID
        public string SubStationName { get; set; } //賣場目前的分類子站名
        public string CatId { get; set; } //賣場目前的分類父類
        public string CatName { get; set; } //賣場目前的分類父類名
        public string CatItemId { get; set; } //賣場目前的分類子類 ID
        public string CatItemName { get; set; } //賣場目前的分類子類名
        public string SeoUrl { get; set; } //賣場網址
        public string FeatureTitle { get; set; } //特色標題
        public string DeliveryType { get; set; } //交貨期限
        public string PreOrderExpectedShipDate { get; set; } //預購型商品預定出貨日
        public int? CustomizedOrderShipDateAfterPlaced { get; set; } //客製化商品完成訂單後出貨天數
        public string OnShelvedTs { get; set; } //開始時間
        public string OffShelvedTs { get; set; } //結束時間
        public string Price { get; set; } //購物中心售價
        public int? PurchaseQtyLimit { get; set; } //限購數量
        public int? CvsPurchaseQtyLimit { get; set; } //超商取貨限購數量
        public string Copy { get; set; } //商品詳情 (文案)，HTML content
        public string SwCode { get; set; } //隱藏賣場 SW code
        public List<ProposalModel> Models { get; set; } //屬性商品型號
        public List<ProductVideo> Videos { get; set; } //賣場影片
        public List<ProductImage> Images { get; set; } //賣場圖片
        public List<ProposalModel> AdditionalPurchases { get; set; } //加價購內容
        public List<ProposalModel> Complimentaries { get; set; } //買就送贈品內容
        public List<ProposalModel> SelectComplimentaries { get; set; } //任選贈品內容
        public bool ShareMediaBetweenModels { get; set; } //是否共用同一組商品圖/影片
        public string AttributeDisplayMode { get; set; } //商品規格顯示方式
        public string StruDataAttrClusterId { get; set; }
        public List<Attribute> Attributes { get; set; } //商品規格表
        public bool SyncProductImages { get; set; } //是否同時修改無屬性賣場之商品圖片
        public bool IsThresholdFreebie { get; set; } //是否為滿額贈贈品賣場
        public ProductWarranty Warranty { get; set; } //商品保證
        public string PartNo { get; set; } //賣場主件供應商商品料號
        public string ProductName { get; set; } //賣場主件商品名稱
        public List<string> ShortDescription { get; set; } //賣場商品特色
        public int OrigLayer { get; set; } //提案更新前的賣場屬性
        public string BrandId { get; set; } //品牌 ID
        public string BrandName { get; set; } //品牌名稱

    }

    public class ProposalModel
    {
        public int Sku { get; set; } //型號 ID
        public Attribute Spec { get; set; } //第 1 層屬性的 spec
        public List<ProposalItem> Items { get; set; } //屬性內容
        public List<ProductVideo> Videos { get; set; } //商品影片
        public List<ProductImage> Images { get; set; } //商品圖
        public string DisplayName { get; set; } //賣場顯示名稱
        public bool IsVisible { get; set; } //屬性是否顯示於前台賣場
        public string Name { get; set; } //屬性商品名稱
        public string PartNo { get; set; } //屬性商品料號
        public string Barcode { get; set; } //實際國際條碼
        public int Stock { get; set; } //備貨數量
        public List<Attribute> Attributes { get; set; } //商品規格表
        public string Price { get; set; } //商品售價
        public string ShortTitle { get; set; } //商品短標題

    }

    public class Attribute
    {
        public string Name { get; set; } //屬性名稱
        public List<string> Values { get; set; } //屬性選項
        public string SelectedValue { get; set; } //不知道是三小
        public Constraints Constraints { get; set; } //屬性限制
        public bool Required { get; set; } //是否必填
        public bool Searchable { get; set; } //是否可用於搜尋頁之限縮條件

    }

    public class Constraints
    {
        public string Type { get; set; }
        public int Minimum { get; set; }
        public int Maximum { get; set; }
    }

    public class ProposalItem
    {
        public int Id { get; set; } //Item ID
        public int Sku { get; set; } //Item ID
        public int Stock { get; set; } //備貨數量
        public string PartNo { get; set; } //屬性商品供應商商品料號
        public string Barcode { get; set; } //實際國際條碼
        public string WarehouseBarcode { get; set; } //進倉用國際條碼
        public Attribute Spec { get; set; } //第 2 層屬性的 spec
        public string DisplayName { get; set; } //賣場顯示名稱
        public string WarehouseProductName { get; set; } //進倉用商品名稱
        public bool IsVisible { get; set; } //屬性是否顯示於前台賣場
        public string Name { get; set; } //屬性商品名稱
        public List<ProductImage> Images { get; set; } //商品圖
        public List<Attribute> Attributes { get; set; } //商品規格表
        public string ShortTitle { get; set; } //商品短標題

    }

    public class ProductImage
    {
        public string Url { get; set; } //圖檔 URL
        public int? Width { get; set; }
        public int? Height { get; set; }
        public int Order { get; set; }
    }

    public class ProductVideo
    {
        public string Url { get; set; } //影片 URL
        public int Duration { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public ProductImage Thumbnail { get; set; } //預覽圖
        public int Order { get; set; }
    }

    public class ProductWarranty
    {
        public string Period { get; set; } //保固期限
        public string Scope { get; set; } //保固範圍
        public string Handler { get; set; } //保固來源
        public string ProductStatus { get; set; } //商品狀態資訊
        public string Description { get; set; } //保固說明

    }

    public class ProposalProduct
    {
        public int Id { get; set; }
        public string Name { get; set; } //商品名稱
        public List<string> ShortDescription { get; set; } //賣場簡短說明
        public bool ShareMediaBetweenModels { get; set; } //是否共用同一組商品圖/影片
        public List<ProposalModel> Models { get; set; } //商品型號(屬性)
        public string Brand { get; set; } //ESD 專用品牌
        public string BrandId { get; set; } //品牌 ID
        public string BrandName { get; set; } //品牌名稱
        public string Model { get; set; } //商品型號
        public ShipType ShipType { get; set; } //配送方式
        public string ContentRating { get; set; } //內容級別
        public string ZoneId { get; set; } //商品目前的分類區 ID
        public string ZoneName { get; set; } //商品目前的分類區名
        public string SubStationId { get; set; } //商品目前的分類子站 ID
        public string SubStationName { get; set; } //商品目前的分類子站名
        public string CatId { get; set; } //商品目前的分類父類 ID
        public string CatName { get; set; } //商品目前的分類父類名
        public string CatItemId { get; set; } //商品目前的分類子類 ID
        public string CatItemName { get; set; } //商品目前的分類子類名
        public string Msrp { get; set; } //廠商建議價
        public string Cost { get; set; } //成本(含稅+運費)
        public int? SafeStockQty { get; set; } //安全庫存量
        public int Length { get; set; } //包裝完成後的商品長度
        public int Width { get; set; } //包裝完成後的商品寬度
        public int Height { get; set; } //包裝完成後的商品高度
        public int Weight { get; set; } //包裝完成後的商品重量
        public bool IsInstallRequired { get; set; } //是否需要安裝
        public bool IsLargeVolumnProductGift { get; set; } //是否為大型商品附屬贈品
        public bool IsNeedRecycle { get; set; } //是否屬於廢四機
        public int? PreserveDays { get; set; } //商品保存期限
        public bool IsOutrightPurchase { get; set; } //是否為買斷商品
        public bool IsExpiringItem { get; set; } //是否為即期品
        public int MinPackingCount { get; set; } //最小包裝數
        public string Copy { get; set; } //商品詳情 (文案)
        public ProductWarranty Warranty { get; set; } //商品保證
        public List<ProposalProductSpec> Specs { get; set; } //商品屬性
        public string AttributeDisplayMode { get; set; } //商品規格顯示方式
        public string StruDataAttrClusterId { get; set; } //結構化資料屬性集 ID
        public string StruDataAttrClusterName { get; set; } //結構化資料屬性集名稱
        public List<Attribute> Attributes { get; set; } //商品規格表
        public List<string> GameContents { get; set; } //遊戲情節內容
        public string PartNo { get; set; } //主件商品供應商商品料號
        public bool Taxable { get; set; } //商品是否應稅
        public List<VoucherAttribute> EVoucherAttributes { get; set; } //電子票券資訊
        public string EVoucherType { get; set; } //電子票券類型
    }

    public class ShipType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? Model { get; set; }
    }

    public class ProposalProductSpec
    {
        public int Level { get; set; } //屬性層級
        public string Name { get; set; } //屬性名稱
    }

    public class VoucherAttribute
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } //屬性說明
        public string MaxLength { get; set; } //最大長度
        public string DataType { get; set; } //資料型別
        public bool IsRequired { get; set; } //是否必要
        public string Value { get; set; } //電子票券屬性值

    }

    public class ProposalsResponse 
    {
        public int Id { get; set; } //提案編號
        public string Applicant { get; set; } //提案人
        public string ContactWindow { get; set; } //提案對象
        public string CreatedTs { get; set; } //建檔時間
        public string ExecuteStatus { get; set; } //執行狀態
        public string ExpiredTs { get; set; } //提案有效時間
        public int ModifiedTimes { get; set; } //更新次數
        public string ModifiedTs { get; set; } //更新時間
        public string Modifier { get; set; } //更新者
        public string Note { get; set; } //備註
        public string ReviewStatus { get; set; } //審核狀態
        public string SubStationId { get; set; } //提案當下的提案站別 ID
        public string SubStationName { get; set; } //提案當下的提案站別名稱
        public string SupplierId { get; set; } //供應商編號
        public string SupplierName { get; set; } //供應商名稱
        public string Type { get; set; } //提案單類型
        public ProposalListing Listing { get; set; } //賣場資訊
        public ProposalProduct Product { get; set; } //商品資訊
    }

    public class ProductWarrantlyHandlerReturn
    {
        public List<Option<string>> Options { get; set; }
        public Option<string> ProductWarrantlyHandler { get; set; }
    }

    public class ContentRatingReturn
    {
        public List<Option<string>> Options { get; set; }
        public Option<string> ContentRating { get; set; }
    }

    public class ShipTypeReturn
    {
        public List<Option<int>> Options { get; set; }
        public Option<int> ShipType { get; set; }
    }

    public class ProductStatusReturn
    {
        public List<Option<string>> Options { get; set; }
        public Option<string> ProductStatus { get; set; }
    }

    public class StruDataAttrClustersRequest
    {
        public string Id { get; set; }
        public string ProposalType { get; set; }
        public string CategoryId { get; set; }
    }

    public class StruDataAttrClustersResponse
    {
        public List<StruDataAttrCluster> StruDataAttrClusters { get; set; }
    }

    public class StruDataAttrCluster
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string CategoryId { get; set; }
        public string ProposalType { get; set; }
        public List<Attribute> Attributes { get; set; }
    }

    public class ParentProposal
    {
        public int Id { get; set; }
        public string Type { get; set; }
    }

    public class BatchProposalInfo
    {
        public int Id { get; set; }
        public string Type { get; set; }
    }

    public class UploadContext
    {
        public Dictionary<string, string> Headers { get; set; }
        public Credential S3Credentials { get; set; }
        public string Wssid { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsExpired(int validMinutes = 50)
        {
            return DateTime.Now - CreatedAt > TimeSpan.FromMinutes(validMinutes);
        }
    }

    public class BatchUploadResult
    {
        public List<ImageUploadResult> Results { get; set; } = new List<ImageUploadResult>();
        public int SuccessCount => Results.Count(r => r.IsSuccess);
        public int FailedCount => Results.Count(r => !r.IsSuccess);
        public List<string> FailedImages => Results.Where(r => !r.IsSuccess).Select(r => r.ImagePath).ToList();
    }

    public class ImageUploadResult
    {
        public string ImagePath { get; set; }
        public string YahooUrl { get; set; }
        public bool IsSuccess => !string.IsNullOrEmpty(YahooUrl);
        public string ErrorMessage { get; set; }
        public string ETag { get; set; }
    }

    public class ImageProcessInfo
    {
        public int Index { get; set; }
        public string FileName { get; set; }
        public string LocalPath { get; set; }
        public ImageType ImageType { get; set; }
    }

    #endregion
}
