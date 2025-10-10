using CityHubCore.Infrastructure.API;
using Microsoft.AspNetCore.Http;
using POVWebDomain.Models.API.StoreSrv.Common;
using POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods;
using POVWebDomain.Models.ExternalApi;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static POVWebDomain.Models.ExternalApi.ShopeeSCM.InitTierVariationRequest;

namespace POVWebDomain.Models.ExternalApi.ShopeeSCM
{
    public class ShopeeApiKeyEntity
    {
        public int StoreID { get; set; }
        public int ApiKey { get; set; }
        public string Accesstoken { get; set; }
        public string Refreshtoken { get; set; }
        public string Scope { get; set; }
        public long KeyValue { get; set; }
        public string AccessCode { get; set; }
    }
    public class ShopeeRefreshToken
    {
        public int Shop_id { get; set; }
        public string Refresh_token { get; set; }
        public int Partner_id { get; set; }
    }
    public class ShopeeRefreshTokenResponse
    {
        public int Partner_id { get; set; }
        public string Refresh_token { get; set; }
        public string Access_token { get; set; }
        public int Expire_in { get; set; }
        public string Error { get; set; }
        public string Message { get; set; }
        public int Shop_id { get; set; }

    }

    public class ShopeeApiKey
    {
        public int Partner_id { get; set; }
        public string Access_token { get; set; }
        public int Shop_id { get; set; }

        public string Partner_key { get; set; }
    }
    public class ShopeeResponseBase
    {
        public string Request_id { get; set; }
        public string Error { get; set; }
        public string Message { get; set; }
        public string Warning { get; set; }
        public string ReqString { get; set; }
        public string RepString { get; set; }
    }
    public class ShippingParameterResponse : ShopeeResponseBase
    {
        public ShippingParameter Response { get; set; }
    }

    public class ShippingParameter
    {
        //訂單的物流方式
        public ShippingParameter_InfoNeeded Info_needed { get; set; }
        //投遞模式訂單的物流信息 ***branch_id***sender_real_name***tracking_no***slug
        public ShippingParameter_Dropoff Dropoff { get; set; }
        public ShippingParameter_Pickup Pickup { get; set; }
    }

    public class ShippingParameter_Dropoff
    {
        public List<BranchList> Branch_list { get; set; }
        public List<SlugList> Slug_list { get; set; }
    }
    //可用的投遞分支店信息列表
    public class BranchList
    {
        public long Branch_id { get; set; }
        public string Region { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Zipcode { get; set; }
        public string District { get; set; }
        public string Town { get; set; }
        //可用的 TW 3PL 投遞合作夥伴列表

    }
    public class SlugList
    {
        public string Slug { get; set; }
        public string Slug_name { get; set; }
    }

    public class ShippingParameter_InfoNeeded
    {
        //可以包含“branch_id”、“sender_real_name”或“tracking_no”。
        //如果它包含'branch_id'，請選擇一個來初始化。
        //如果它包含 'sender_real_name' 或 'tracking_no'，應在 Init API 中手動輸入這些值。
        //如果為空值，開發者仍應在 Init API 中包含“dropoff”字段。
        //可以包含“slug”。如果包含“slug”，則返回選定的 3PL 合作夥伴，僅用於 TW C2C 賣家用於投遞包裹。
        public List<string> Dropoff { get; set; }
        //可以包含“address_id”和“pickup_time_id”。選擇一個address_id及其對應的pickup_time_id給Init。
        //如果它為空值，開發者仍應在 Init API 中包含“pickup”字段。
        public List<string> Pickup { get; set; }
        //可能包含“tracking_no”。如果它包含“tracking_no”，則應在 Init API 中手動輸入這些值。
        //如果它為空值，開發者仍應在 Init API 中包含“non-integrated”字段。
        public List<string> Non_integrated { get; set; }

    }

    public class ShippingParameter_Pickup
    {
        public List<ShippingParameter_Pickup_Address> Address_list { get; set; }
    }
    public class ShippingParameter_Pickup_Address
    {
        public long Address_id { get; set; }
        public string Region { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Town { get; set; }
        public string Address { get; set; }
        public string Zipcode { get; set; }
        public List<string> Address_flag { get; set; }
        public List<ShippingParameter_Pickup_Address_TimeSlotList> Time_slot_list { get; set; }
    }

    public class ShippingParameter_Pickup_Address_TimeSlotList
    {
        public int Date { get; set; }
        public string Time_text { get; set; }
        public string Pickup_time_id { get; set; }
    }

    public class ShipOrder
    {
        public string Order_sn { get; set; }
        public ShipOrder_Dropoff Dropoff { get; set; }
        public ShipOrder_Pickup Pickup { get; set; }
        public ShipOrder_NonIntegrated Non_integrated { get; set; }
    }
    public class ShipOrder_Dropoff
    {
        public long Branch_id { get; set; }
        public string Sender_real_name { get; set; }
        public string Tracking_number { get; set; }
        public string Slug { get; set; }
    }
    public class ShipOrder_Pickup
    {
        public long? Address_id { get; set; }
        public string Pickup_time_id { get; set; }
        public string Tracking_number { get; set; }
    }
    public class ShipOrder_NonIntegrated
    {
        public string Tracking_number { get; set; }
    }

    public class GetTrackingNumberResponse : ShopeeResponseBase
    {
        public Get_Tracking_Number Response { get; set; }

    }

    public class Get_Tracking_Number
    {
        public string Tracking_number { get; set; }
        public string Plp_number { get; set; }
        public string First_mile_tracking_number { get; set; }
        public string Last_mile_tracking_number { get; set; }
        public string Hint { get; set; }
    }

    #region 商品上架

    public class Get_categoryRequest
    {
        public string Language { get; set; }
    }

    public class Get_categoryApiResponse : ShopeeResponseBase
    {
        public Get_categoryResponse Response { get; set; }
    }

    public class Get_categoryResponse
    {
        public List<Category_list> Category_list { get; set; }
    }

    public class Category_list
    {
        public int Category_id { get; set; }
        public int Parent_category_id { get; set; }
        public string Original_category_name { get; set; }
        public string Display_category_name { get; set; }
        public bool Has_children { get; set; }
    }

    public class GetCategoryRecommendRequest
    {
        public string Item_name { get; set; }
        public string Product_cover_image { get; set; }
    }

    public class GetCategoryRecommendApiResponse : ShopeeResponseBase
    {
        public GetCategoryRecommendResponse Response { get; set; }
    }

    public class GetCategoryRecommendResponse
    {
        public List<int> Category_id { get; set; }
    }

    //GetGetAttributeTree
    public class GetAttributeTreeRequest
    {
        public List<int> Category_id_list { get; set; }
        public string Language { get; set; }
    }

    public class GetAttributeTreeResponse
    {
        public List<List> List { get; set; }
    }

    public class List
    {
        public List<Attribute_tree> Attribute_tree { get; set; }
        public int Category_id { get; set; }
        public string Warning { get; set; }
    }

    public class Attribute_tree
    {
        public int Attribute_id { get; set; }
        public bool Mandatory { get; set; }
        public string Name { get; set; }
        public List<Attribute_value_list> Attribute_value_list { get; set; }
        public Attribute_info Attribute_info { get; set; }
        public List<Multi_lang> Multi_lang { get; set; }
    }

    public class Attribute_value_list
    {
        public int Value_id { get; set; }
        public string Name { get; set; }
        public string Value_unit { get; set; }
        public List<object> Child_attribute_list { get; set; }
        public List<Multi_lang> Multi_lang { get; set; }
    }

    public class Multi_lang
    {
        public string Language { get; set; }
        public string Value { get; set; }
    }

    public class Attribute_info
    {
        public int Input_type { get; set; }
        public int Input_validation_type { get; set; }
        public int Format_type { get; set; }
        public int Date_format_type { get; set; }
        public List<string> Attribute_unit_list { get; set; }
        public int Max_value_count { get; set; }
        public string Introduction { get; set; }
        public bool Is_oem { get; set; }
        public bool Support_search_value { get; set; }

    }

    public class GetAttributeTreeApiResponse : ShopeeResponseBase
    {
        public GetAttributeTreeResponse Response { get; set; }
    }

    #region 物流
    public class GetChannelListRequest {}
    public class GetChannelListResponse
    {
        public List<LogisticsChannelList> Logistics_channel_list { get; set; }
    }


    public class LogisticsChannelList
    {
        public int Logistics_channel_id { get; set; }
        public string Logistics_channel_name { get; set; }
        public bool Cod_enabled { get; set; }
        public bool Enabled { get; set; }
        public string Fee_type { get; set; }
        public List<SizeList> Size_list { get; set; }
        public WeightLimit Weight_limit { get; set; }
        public ItemMaxDimension Item_max_dimension { get; set; }
        public VolumeLimit Volume_limit { get; set; }
        public string Logistics_description { get; set; }
        public bool Force_enable { get; set; }
        public int Mask_channel_id { get; set; }
        public bool Block_seller_cover_shipping_fee { get; set; }
        public bool Support_cross_border { get; set; }
        public bool? Seller_logistic_has_configuration { get; set; }
        public LogisticsCapability Logistics_capability { get; set; }
        public bool Support_pre_order { get; set; }

    }
    public class SizeList
    {
        public string Size_id { get; set; }
        public string Name { get; set; }
        public float Default_price { get; set; }
    }

    public class WeightLimit
    {
        public float Item_max_weight { get; set; }
        public float Item_min_weight { get; set; }
    }

    public class ItemMaxDimension
    {
        public float Height { get; set; }
        public float Width { get; set; }
        public float Length { get; set; }
        public string Unit { get; set; }
        public float Dimension_sum { get; set; }
    }

    public class VolumeLimit
    {
        public float Item_max_volume { get; set; }
        public float Item_min_volume { get; set; }
    }

    public class LogisticsCapability
    {
        public bool Seller_logistics { get; set; }
        public bool Direct_delivery { get; set; }
        public bool Spx_instant { get; set; }
    }

    public class GetChannelListApiResponse : ShopeeResponseBase
    {
        public GetChannelListResponse Response { get; set; }
    }

    public class GetBrandListRequest
    {
        public int Offset { get; set; }
        public int Page_size { get; set; }
        public int Category_id { get; set; }
        public int Status { get; set; }
        public string Language { get; set; }
    }
    public class GetBrandListResponse
    {
        public List<BrandList> Brand_list { get; set; }
        public bool Has_next_page { get; set; }
        public int Next_offset { get; set; }
        public bool Is_mandatory { get; set; }
        public string Input_type { get; set; }
    }

    public class BrandList
    {
        public string Original_brand_name { get; set; }
        public int Brand_id { get; set; }
        public string Display_brand_name { get; set; }
    }

    public class GetBrandListApiResponse: ShopeeResponseBase
    {
        public GetBrandListResponse Response { get; set; }
    }

    #endregion

    public class PublishGoodsRequest
    {
        public AddItem AddItem { get; set; }
        public InitTierVariationRequest.InitTierVariation Sku { get; set; }
    }

    #region add item
    public class AddItem
    {
        public float Original_price { get; set; }
        public string Description { get; set; }
        public float Weight { get; set; }
        public string Item_name { get; set; }
        public string Item_status { get; set; }
        public Dimension Dimension { get; set; }
        public List<LogisticInfo> Logistic_info { get; set; }
        public List<AttributeList> Attribute_list { get; set; }
        public int Category_id { get; set; }
        public Image Image { get; set; }
        public PreOrder Pre_order { get; set; }
        public string Item_sku { get; set; }
        public string Condition { get; set; }
        public List<Wholesale> Wholesale { get; set; }
        public List<string> Video_upload_id { get; set; }
        public Brand Brand { get; set; }
        public int Item_dangerous { get; set; }
        public TaxInfo Tax_info { get; set; }
        public ComplaintPolicy Complaint_policy { get; set; }
        public DescriptionInfo Description_info { get; set; }
        public string Description_type { get; set; }
        public List<SellerStock> Seller_stock { get; set; }
        public string Gtin_code { get; set; }
        public string Ds_cat_rcmd_id { get; set; }
        public PromotionImages Promotion_images { get; set; }
        public CompatibilityInfo Compatibility_info { get; set; }
        public DateTime? Scheduled_publish_time { get; set; }
        public int? Authorised_brand_id { get; set; }
        public SizeChartInfo Size_chart_info { get; set; }
        public CertificationInfo Certification_info { get; set; }

    }

    public class Dimension
    {
        public int Package_height { get; set; }
        public int Package_length { get; set; }
        public int Package_width { get; set; }
    }

    public class LogisticInfo
    {
        public int Size_id { get; set; }
        public float Shipping_fee { get; set; }
        public bool Enabled { get; set; }
        public int Logistic_id { get; set; }
        public bool Is_free { get; set; }
    }

    public class AttributeList
    {
        public int Attribute_id { get; set; }
        public List<AttributeValueList> Attribute_value_list { get; set; }
    }

    public class AttributeValueList
    {
        public int Value_id { get; set; }
        public string Original_value_name { get; set; }
        public string Value_unit { get; set; }
    }

    public class Image
    {
        public List<string> Image_id_list { get; set; } = new List<string>();
        public string Image_ratio { get; set; }
    }

    public class PreOrder
    {
        public bool Is_pre_order { get; set; }
        public int Days_to_ship { get; set; }
    }

    public class Wholesale
    {
        public int Min_count { get; set; }
        public int Max_count { get; set; }
        public float Unit_price { get; set; }
    }

    public class Brand
    {
        public int Brand_id { get; set; }
        public string Original_brand_name { get; set; }
    }

    public class TaxInfo
    {
        public string Ncm { get; set; }
        public string Same_state_cfop { get; set; }
        public string Different_state_cfop { get; set; }
        public string Csosn { get; set; }
        public string Origin { get; set; }
        public string Cest { get; set; }
        public string Measure_unit { get; set; }
        public string Hs_code { get; set; }
        public string Tax_code { get; set; }
        public int Tax_type { get; set; }
        public string Pis { get; set; }
        public string Cofins { get; set; }
        public string Icms_cst { get; set; }
        public string Pis_cofins_cst { get; set; }
        public string Federal_state_taxes { get; set; }
        public string Operation_type { get; set; }
        public string Ex_tipi { get; set; }
        public string Fci_num { get; set; }
        public string Recopi_num { get; set; }
        public string Additional_info { get; set; }
        public GroupItemInfo Group_item_info { get; set; }
        public string Export_cfop { get; set; }

    }

    public class GroupItemInfo
    {
        public string Group_qtd { get; set; }
        public string Group_unit { get; set; }
        public string Group_unit_value { get; set; }
        public string Original_group_price { get; set; }
        public string Group_gtin_sscc { get; set; }
        public string Group_grai_gtin_sscc { get; set; }
    }

    public class ComplaintPolicy
    {
        public string Warranty_time { get; set; }
        public string Exclude_entrepreneur_warranty { get; set; }
        public int Complaint_address_id { get; set; }
        public string Additional_information { get; set; }
    }

    public class DescriptionInfo
    {
        public ExtendedDescription Extended_description { get; set; }
    }

    public class ExtendedDescription
    {
        public List<FieldList> Field_list { get; set; }
    }

    public class FieldList
    {
        public string Field_type { get; set; }
        public string Text { get; set; }
        public ImageInfo Image_info { get; set; }
    }

    public class ImageInfo
    {
        public string Image_id { get; set; }
        public List<ImageUrlList> Image_url_list { get; set; }
    }

    public class ImageUrlList
    {
        public string Image_url_region { get; set; }
        public string Image_url { get; set; }
    }

    public class SellerStock
    {
        public string Location_id { get; set; }
        public int Stock { get; set; }
    }

    public class PromotionImages
    {
        public List<string> Image_id_list { get; set; }
    }
    public class CompatibilityInfo
    {
        List<VehicleInfoList> Vehicle_info_list { get; set; }
    }
    public class VehicleInfoList
    {
        public int Brand_id { get; set; }
        public int Model_id { get; set; }
        public int Year_id { get; set; }
        public string Version_id { get; set; }
    }
    public class SizeChartInfo
    {
        public string Size_chart { get; set; }
        public int Size_chart_id { get; set; }
    }

    public class CertificationInfo
    {
        public List<CertificationList> Ｃertification_list { get; set; }
    }

    public class CertificationList
    {
        public int Ｃertification_type { get; set; }
        public string Ｃertification_no { get; set; }
        public int Ｐermit_id { get; set; }
        public int Expiry_date { get; set; }
        public List<CertificationProofs> Certification_proofs { get; set; }

    }

    public class CertificationProofs
    {
        public string File_name { get; set; }
        public int Image_id { get; set; }
        public float Ratio { get; set; }
    }
    #endregion

    #region add item response
    public class AddItemResponse
    {
        public string Description { get; set; }
        public float Weight { get; set; }
        public PreOrder Pre_order { get; set; }
        public string Item_name { get; set; }
        public Images Images { get; set; }
        public string Item_status { get; set; }
        public PriceInfo Price_info { get; set; }
        public List<LogisticInfo> Logistic_info { get; set; }
        public int Item_id { get; set; }
        public List<AttributeList> Attribute { get; set; }
        public int Category_id { get; set; }
        public Dimension Dimension { get; set; }
        public string Condition { get; set; }
        public List<VideoInfo> Video_info { get; set; }
        public List<Wholesale> Wholesale { get; set; }
        public Brand Brand { get; set; }
        public int Item_dangerous { get; set; }
        public DescriptionInfo Description_info { get; set; }
        public string Description_type { get; set; }
        public ComplaintPolicy Complaint_policy { get; set; }
        public List<SellerStock> Seller_stock { get; set; }

    }

    public class Images
    {
        public List<string> Image_id_list { get; set; }
        public List<string> Image_url_list { get; set; }
    }

    public class PriceInfo
    {
        public float current_price { get; set; }
        public float original_price { get; set; }
    }

    public class VideoInfo
    {
        public string video_url { get; set; }
        public string thumbnail_url { get; set; }
        public int duration { get; set; }
    }

    public class AddItemApiResponse: ShopeeResponseBase
    {
        public AddItemResponse Response { get; set; }
    }


    #endregion

    #region add tier
    public class InitTierVariationRequest { 
        public class InitTierVariation
        {
            public Int64 Item_id { get; set; }
            public List<TierVariation> Tier_variation { get; set; }
            public List<Model> Model { get; set; }
            public List<StandardiseTierVariation> Standardise_tier_variation { get; set; }

        }
        public class TierVariation
        {
            public string Name { get; set; }
            public List<OptionList> Option_list { get; set; }

        }
        public class OptionList
        {
            public string Option { get; set; }
            public AddTierImage Image { get; set; }
        }
        public class AddTierImage
        {
            public string Image_id { get; set; }
        }

        public class Model
        {
            public Int32[] Tier_index { get; set; }
            public float Original_price { get; set; }
            public string Model_sku { get; set; }
            public List<SellerStock> Seller_stock { get; set; }
            public string Gtin_code { get; set; }
            public float? Weight { get; set; }
            public Dimension Dimension { get; set; }
            public PreOrder Pre_order { get; set; }
        }

        public class StandardiseTierVariation
        {
            public Int32 Variation_id { get; set; }
            public string Variation_name { get; set; }
            public Int32 Variation_group_id { get; set; }
            public List<VariationOptionList> Variation_option_list { get; set; }
        }

        public class VariationOptionList
        {
            public Int32 Variation_option_id { get; set; }
            public string Variation_option_name { get; set; }
            public string Image_id { get; set; }
        }
    }
    public class InitTierVariationResponse
    {
        public class InitTierVariation
        {
            public Int64 Item_id { get; set; }
            public List<TierVariation> Tier_variation { get; set; }
            public List<Model> Model { get; set; }

        }
        public class TierVariation
        {
            public string Name { get; set; }
            public List<OptionList> Option_list { get; set; }

        }
        public class OptionList
        {
            public string Option { get; set; }
            public AddTierImage Image { get; set; }
        }
        public class AddTierImage
        {
            public string Image_url { get; set; }
        }
        public class Model
        {
            public Int32[] Tier_index { get; set; }
            public Int64 Model_id { get; set; }
            public string Model_sku { get; set; }
            public List<PriceInfo> Price_info { get; set; }

            public List<SellerStock> Seller_stock { get; set; }
            public float Weight { get; set; }
            public Dimension Dimension { get; set; }
        }
        public class PriceInfo
        {
            public float Original_price { get; set; }
        }
    }

    public class InitTierVariationApiResponse : ShopeeResponseBase
    {
        public InitTierVariationResponse.InitTierVariation Response { get; set; }
    }
    #endregion


    #region upload image
    public class UploadImageRequest
    {
        public IFormFile Image { get; set; }
        public string Scene { get; set; } = string.Empty;
        public string Ratio { get; set; }
    }
    public class UploadImageResponse
    {
        public ImageInfo Image_info { get; set; }
        public List<ImageInfoList> Image_info_list { get; set; }
    }

    public class ImageInfoList
    {
        public int Id { get; set; }
        public string Error { get; set; }
        public string Message { get; set; }
        public ImageInfo Image_info { get; set; }
    }

    public class UploadImageApiResponse: ShopeeResponseBase
    {
        public UploadImageResponse Response { get; set; }
    }

    public class ShopeeBatchUploadResult
    {
        public List<ShopeeImageUploadResult> Results { get; set; } = new List<ShopeeImageUploadResult>();
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
        public List<string> FailedImages { get; set; } = new List<string>();
    }

    public class ShopeeImageUploadResult
    {
        public string LocalPath { get; set; }
        public ImageInfo ImageInfo { get; set; }
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class ShopeeHtmlProcessResult
    {
        public DescriptionInfo DescriptionInfo { get; set; }
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
        public List<string> FailedImages { get; set; } = new List<string>();
    }
    #endregion

    #endregion
}
