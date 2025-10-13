using System;
using System.Collections.Generic;

namespace POVWebDomain.Models.ExternalApi.OfficialWebsite
{
    // ============================================
    // 新增相關
    // ============================================

    public class SubmitGoodsRequest
    {
        public AddProductRequest MainRequest { get; set; }
        public AddProductOptionRequest ProductOptionRequest { get; set; }
    }

    public class AddProductRequest
    {
        public int StoreNumber { get; set; }
        public int CategoryID { get; set; }
        public string ProductName { get; set; }
        public bool IsClosed { get; set; }
        public DateTime? SellingStartDT { get; set; }
        public DateTime? SellingEndDT { get; set; }
        public int SellingEndSetting { get; set; }
        public bool IsProductOption { get; set; }
        public string GoodID { get; set; }
        public decimal SuggestPrice { get; set; }
        public decimal Price { get; set; }
        public decimal Cost { get; set; }
        public int SafetyInventoryQty { get; set; }
        public int InventoryQty { get; set; }
        public int LimitedQty { get; set; }
        public int DeliveryDateType { get; set; }
        public DateTime? SpecifiedDeliveryDate { get; set; }
        public int? WaitingDays { get; set; }
        public bool IsEnbaleDeliveryDate { get; set; }
        public int? PrepareDays { get; set; }
        public int? AvailableDeliveryDays { get; set; }
        public DateTime? AvailableDeliveryStartDate { get; set; }
        public DateTime? AvailableDeliveryEndDate { get; set; }
        public int TemperatureTypeID { get; set; }
        public float Length { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float Weight { get; set; }
        public string WebPageTitle { get; set; }
        public string WebPageDesc { get; set; }
        public string WebPageKeywords { get; set; }
        public int IsShowProduct { get; set; }
        public int IsShowSold { get; set; }
        public int IsShowInventory { get; set; }
        public int IsRestricted { get; set; }
        public int StockoutShow { get; set; }
        public int IsReturnable { get; set; }
        public string ProductFeatures { get; set; }
        public string ProductDetail { get; set; }
        public int[] Brand { get; set; }
        public int PictureCount { get; set; }
        public string[] PayType { get; set; }
        public int[] ShippingType { get; set; }
        public List<ProductSpecification> ProductSpecification { get; set; }
        public bool IsProductOption1 { get; set; }
        public bool IsProductOption2 { get; set; }
        public ProductOptionType ProductOption1 { get; set; }
        public ProductOptionType ProductOption2 { get; set; }
        public List<ProductOption> ProductOptionList { get; set; }
    }

    public class ProductSpecification
    {
        public string Title { get; set; }
        public string Value { get; set; }
    }

    public class ProductOptionType
    {
        public int Type { get; set; }
        public string Name { get; set; }
        public string[] Data { get; set; }
    }

    public class AddProductResponse
    {
        public string Error { get; set; }
        public long ProductID { get; set; }
        public List<SkuResult> SkuList { get; set; }
    }

    public class AddProductOptionRequest
    {
        public long ProductID { get; set; }
        public List<ProductOption> ProductOptionList { get; set; }
    }

    public class AddProductOptionResponse
    {
        public string Error { get; set; }
        public List<SkuResult> SkuList { get; set; }
    }

    // ============================================
    // 修改相關
    // ============================================

    public class SubmitGoodsEditRequest
    {
        public UpdateProductRequest MainRequest { get; set; }
        public UpdateProductOptionRequest UpdateProductOptionRequest { get; set; }
    }

    public class UpdateProductRequest
    {
        public int StoreNumber { get; set; }
        public long ProductID { get; set; }
        public int CategoryID { get; set; }
        public string ProductName { get; set; }
        public bool IsClosed { get; set; }
        public DateTime? SellingStartDT { get; set; }
        public DateTime? SellingEndDT { get; set; }
        public int SellingEndSetting { get; set; }
        public bool IsProductOption { get; set; }
        public string GoodID { get; set; }
        public decimal SuggestPrice { get; set; }
        public decimal Price { get; set; }
        public decimal Cost { get; set; }
        public int SafetyInventoryQty { get; set; }
        public int InventoryQty { get; set; }
        public int LimitedQty { get; set; }
        public int DeliveryDateType { get; set; }
        public DateTime? SpecifiedDeliveryDate { get; set; }
        public int? WaitingDays { get; set; }
        public bool IsEnbaleDeliveryDate { get; set; }
        public int? PrepareDays { get; set; }
        public int? AvailableDeliveryDays { get; set; }
        public DateTime? AvailableDeliveryStartDate { get; set; }
        public DateTime? AvailableDeliveryEndDate { get; set; }
        public int TemperatureTypeID { get; set; }
        public float Length { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float Weight { get; set; }
        public string WebPageTitle { get; set; }
        public string WebPageDesc { get; set; }
        public string WebPageKeywords { get; set; }
        public int IsShowProduct { get; set; }
        public int IsShowSold { get; set; }
        public int IsShowInventory { get; set; }
        public int IsRestricted { get; set; }
        public int StockoutShow { get; set; }
        public int IsReturnable { get; set; }
        public string ProductFeatures { get; set; }
        public string ProductDetail { get; set; }
        public int[] Brand { get; set; }
        public int PictureCount { get; set; }
        public string[] PayType { get; set; }
        public int[] ShippingType { get; set; }
        public List<ProductSpecification> ProductSpecification { get; set; }
        public bool IsProductOption1 { get; set; }
        public bool IsProductOption2 { get; set; }
        public ProductOptionType ProductOption1 { get; set; }
        public ProductOptionType ProductOption2 { get; set; }
        public string ProductDescription { get; set; }
    }

    public class UpdateProductResponse
    {
        public string Error { get; set; }
    }

    public class UpdateProductOptionRequest
    {
        public int StoreNumber { get; set; }
        public long ProductID { get; set; }
        public List<ProductOptionUpdate> ProductOptionList { get; set; }
    }

    public class UpdateProductOptionResponse
    {
        public string Error { get; set; }
        public List<SkuResult> SkuList { get; set; }
    }

    // ============================================
    // 共用類別
    // ============================================

    public class ProductOption
    {
        public long? SkuID { get; set; }
        public string SkuName { get; set; }
        public string GoodID { get; set; }
        public decimal SuggestPrice { get; set; }
        public decimal Price { get; set; }
        public decimal Cost { get; set; }
        public int SafetyInventoryQty { get; set; }
        public int InventoryQty { get; set; }
        public int LimitedQty { get; set; }
    }

    public class ProductOptionUpdate : ProductOption
    {
        public long? SkuID { get; set; }
    }

    public class SkuResult
    {
        public long SkuID { get; set; }
        public string GoodID { get; set; }
    }
}