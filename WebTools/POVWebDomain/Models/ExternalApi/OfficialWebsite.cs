using Microsoft.AspNetCore.Http;
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
        public List<IFormFile> MainImage { get; set; }
        public List<IFormFile> SkuImage { get; set; }

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
        public bool IsShowProduct { get; set; }
        public bool IsShowSold { get; set; }
        public bool IsShowInventory { get; set; }
        public bool IsRestricted { get; set; }
        public int StockoutShow { get; set; }
        public bool IsReturnable { get; set; }
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
        public int ProductID { get; set; }
        public List<SkuResult> ProductOpton { get; set; }
    }

    public class AddProductOptionRequest
    {
        public int ProductID { get; set; }
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
        public List<IFormFile> MainImage { get; set; }
        public List<IFormFile> SkuImage { get; set; }
        public AddProductOptionRequest NewOptionsRequest { get; set; } 
        public UpdateProductOptionRequest UpdateOptionsRequest { get; set; }
    }

    public class UpdateProductRequest
    {
        public int StoreNumber { get; set; }
        public int ProductID { get; set; }
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
        public bool IsShowProduct { get; set; }
        public bool IsShowSold { get; set; }
        public bool IsShowInventory { get; set; }
        public bool IsRestricted { get; set; }
        public int StockoutShow { get; set; }
        public bool IsReturnable { get; set; }
        public string ProductFeatures { get; set; }
        public string ProductDetail { get; set; }
        public int[] Brand { get; set; }
        public int PictureCount { get; set; }
        public string[] PayType { get; set; }
        public int[] ShippingType { get; set; }
        public List<ProductSpecification> ProductSpecification { get; set; }
    }

    public class UpdateProductResponse
    {
        public string Error { get; set; }
    }

    public class UpdateProductOptionRequest
    {
        public int StoreNumber { get; set; }
        public long ProductID { get; set; }
        public List<ProductOption> ProductOptionList { get; set; }
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


    public class SkuResult
    {
        public long SkuID { get; set; }
        public string GoodID { get; set; }
    }

    // 主圖上傳 Request (Update_ProductImage)
    public class UpdateProductImageRequest
    {
        public int StoreNumber { get; set; }
        public int ProductID { get; set; }
        public List<ProductImageData> Data { get; set; } = new List<ProductImageData>();
    }

    public class ProductImageData
    {
        public int DisplayOrder { get; set; }
        public IFormFile ImageFile { get; set; }
    }

    // 選項圖片上傳 Request (Update_ProductOption_Image)
    public class UpdateProductOptionImageRequest
    {
        public int StoreNumber { get; set; }
        public int ProductID { get; set; }
        public List<ProductOptionImageData> Data { get; set; } = new List<ProductOptionImageData>();
    }

    public class ProductOptionImageData
    {
        public long SkuID { get; set; }
        public int DisplayOrder { get; set; }
        public IFormFile ImageFile { get; set; }
    }

    public class RemoveProductRequest
    {
        public int StoreNumber { get; set; }
        public int ProductID { get; set; }
    }

    public class RemoveProductResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Data { get; set; }
    }
}