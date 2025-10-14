using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using POVWebDomain.Models.API.StoreSrv.Common;
using POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.Common;
using POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.EcommerceStore;
using POVWebDomain.Models.ExternalApi.Momo;
using POVWebDomain.Models.ExternalApi.OfficialWebsite;
using POVWebDomain.Models.ExternalApi.Store91;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods
{
    public class SubmitMainRequestAll
    {
        public string ParentID { get; set; }
        public string BasicInfo { get; set; }
        public List<IFormFile> MainImage { get; set; }
        public List<IFormFile> SkuImage { get; set; }
        public IFormFile SizeImage { get; set; }
        public string ChangePerson { get; set; }
        public List<string> Store { get; set; }
        public string StoreSettings { get; set; }
        public string Origin { get; set; }
    }

    public class StoreSetting
    {
        public string PlatformID { get; set; }
        public string EStoreID { get; set; }
        public int? CategoryId { get; set; }
        public string Title { get; set; }
        public decimal Cost { get; set; }
        public bool Publish { get; set; }
        public bool NeedDelete { get; set; }
    }

    public class SubmitMainRequest
    {
        public string ParentID { get; set; }
        public long ShopId { get; set; }
        public int CategoryId { get; set; }
        public int? ShopCategoryId { get; set; }
        public string Title { get; set; }
        public DateTime? SellingStartDateTime { get; set; }
        public DateTime? SellingEndDateTime { get; set; }
        public string ApplyType { get; set; }
        public DateTime ExpectShippingDate { get; set; }
        public int ShippingPrepareDay { get; set; }
        public List<long> ShipType_91app { get; set; }
        public List<string> PayTypes { get; set; }
        public decimal SuggestPrice { get; set; }
        public decimal Price { get; set; }
        public decimal Cost { get; set; }
        public string ProductHighlight { get; set; } = string.Empty;
        public string ProductDescription { get; set; } = string.Empty;
        public string MoreInfo { get; set; }
        public string Type { get; set; } = string.Empty;
        public List<Specifications> Specifications { get; set; }
        public bool HasSku { get; set; }
        public int? OnceQty { get; set; }
        public int? Qty { get; set; }
        public string OuterId { get; set; }
        public List<SkuItem> SkuList { get; set; }
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
        public long? SalePageSpecChartId { get; set; }

        // Momo
        public string SupGoodsName_serial { get; set; }
        public string GoodsType { get; set; }
        public string SaleUnit { get; set; }
        public List<IndexList> IndexList { get; set; }
        public string ClothDataType { get; set; }
        public string ClothDataUnit { get; set; }

        [JsonPropertyName("clothDataSizeIndex")]
        public List<Dictionary<string, string>> ClothDataSizeIndex { get; set; }
        [JsonPropertyName("clothDataTryIndex")]
        public List<Dictionary<string, string>> ClothDataTryIndex { get; set; }
        public int ExpDays { get; set; }

        // Yahoo
        public int ShipType_yahoo { get; set; }
        public bool IsExpiringItem { get; set; }
        public string ProductStatus { get; set; }

        // Momo
        public List<ShippingTypes> ShipType_shopee { get; set; }

        //Official
        public int? CategoryOfficialId { get; set; }
    }

    public class SavePictureRequest
    {
        public IFormFile Image { get; set; }
        public string BaseName { get; set; }
        public string Type { get; set; }
        public int? Index { get; set; }
    }

    public class SubmitMainResponseAll
    {
        public object Response { get; set; }
    }

    public class GetSubmitModeRequest
    {
        public string GoodID { get; set; }
    }

    public class GetSubmitDefValRequest
    {
        public string ParentID { get; set; }
    }

    public class GetSubmitModeReponse
    {
        public string ResponseData { get; set; }
        public string RequestParams { get; set; }
    }

    public class GetEStoreCatResponse
    {
        public string EStoreID { get; set; }
        public string CategoryID { get; set; }
        public string CategoryName { get; set; }
    }

    public class SkuItem
    {
        public string Path { get; set; }
        public Option<string> ColDetail1 { get; set; }
        public Option<string> ColDetail2 { get; set; }
        public int Qty { get; set; }
        public int OnceQty { get; set; }
        public string OuterId { get; set; }
        public int SafetyStockQty { get; set; }
        public string OriginalOuterId { get; set; }
        public int OriginalQty { get; set; }
        public decimal SuggestPrice { get; set; }
        public decimal Price { get; set; }
        public decimal Cost { get; set; }
    }


    public class SkuWrapper
    {
        public List<SkuItem> SkuList { get; set; }
    }


    public class ImageInfo
    {
        public string Path { get; set; }
    }


    public class SkuItem_Goods
    {
        public string GoodID { get; set; }
        public string GoodName { get; set; }
        public decimal AdvicePrice { get; set; }
        public decimal Price { get; set; }
        public decimal Cost { get; set; }
        public string ColorID { get; set; }
        public string ColorName { get; set; }
        public string SizeID { get; set; }
        public string SizeName { get; set; }
    }

    public class OptionValue
    {
        public string InputValue { get; set; }
        public List<Option<string>> Value { get; set; }
    }

    public class OptionItem
    {
        public string Name { get; set; }
        public OptionValue Options { get; set; }
    }

    public class GetOptionAllRequest
    {
        public int StoreNumber { get; set; }
    }

    // default value
    public class Specifications
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class PayTypes
    {
        public string ID { get; set; }
        public bool Checked { get; set; } = false;
    }

    public class ShippingTypes
    {
        public long ID { get; set; }
        public bool Checked { get; set; } = false;
        public bool IsFree { get; set; } = false;
    }

    public class GetShippingReturn
    {
        public List<CheckboxOption<long>> Options { get; set; }
        public List<ShippingTypes> ShippingTypes { get; set; }
    }

    public class SaleModeTypes
    {
        public int ID { get; set; }
        public bool Checked { get; set; } = false;
    }

    public class IndexList
    {
        public string KeyNo { get; set; }
        public string Key { get; set; }
        public string PlatformSource { get; set; }
        public List<Option<string>> Value { get; set; }
    }

    public static class IndexListExtensions
    {
        // 91App
        public static Dictionary<string, string> ConvertToDictionary(this List<IndexList> indexList)
        {
            var dict = new Dictionary<string, string>();
            foreach (var index in indexList)
            {
                string combinedValue = string.Join("\n", index.Value.Select(v => v.Label));

                dict[index.Key] = combinedValue;
            }

            return dict;
        }

        //Momo
        public static List<POVWebDomain.Models.ExternalApi.Momo.IndexList> ConvertToIndexList(this List<IndexList> indexList)
        {
            var indexListMomo = new List<POVWebDomain.Models.ExternalApi.Momo.IndexList>();

            foreach (var index in indexList)
            {
                string chosenItemNo = string.Join("\t", index.Value.Select(v => v.Value));

                indexListMomo.Add(new POVWebDomain.Models.ExternalApi.Momo.IndexList
                {
                    IndexNo = index.KeyNo,
                    ChosenItemNo = chosenItemNo
                });
            }

            return indexListMomo;
        }

        //Yahoo
        public static List<POVWebDomain.Models.ExternalApi.Attribute> ConvertToAttribute(this List<IndexList> indexList)
        {
            var attributes = new List<POVWebDomain.Models.ExternalApi.Attribute>();
            foreach (var index in indexList)
            {
                attributes.Add(new POVWebDomain.Models.ExternalApi.Attribute
                {
                    Name = index.Key,
                    Values = index.Value.Select(t=>t.Value).ToList()
                });
            }

            return attributes;
        }

        //Shopee
        public static List<POVWebDomain.Models.ExternalApi.ShopeeSCM.AttributeList> ConvertToShopeeAttribute(this List<IndexList> indexList)
        {
            var attributes = new List<POVWebDomain.Models.ExternalApi.ShopeeSCM.AttributeList>();
            foreach (var index in indexList)
            {
                attributes.Add(new POVWebDomain.Models.ExternalApi.ShopeeSCM.AttributeList
                {
                    Attribute_id = Convert.ToInt32(index.Key),
                    Attribute_value_list = index.Value.Select(t=> new ExternalApi.ShopeeSCM.AttributeValueList
                    {
                        Value_id = Convert.ToInt32(t.Value)
                    }).ToList()
                });
            }

            return attributes;
        }
    }

    public static class SpecificationsExtensions
    {
        // 91App
        public static Dictionary<string, string> ConvertToDictionary(this List<Specifications> specsList, Dictionary<string, string> indexList)
        {

            foreach (var specs in specsList)
            {
                if (string.IsNullOrEmpty(specs.Key) || string.IsNullOrEmpty(specs.Value))
                    continue;

                indexList[specs.Key] = specs.Value;
            }

            return indexList;
        }

        // Momo

        public static string ConvertSpecificationsToString(this List<Specifications> specsList)
        {
            if (specsList == null || specsList.Count == 0)
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder();

            foreach (var spec in specsList)
            {
                if (string.IsNullOrEmpty(spec.Key) || string.IsNullOrEmpty(spec.Value))
                    continue;
                sb.AppendLine($"{spec.Key}: {spec.Value}");
            }

            return sb.ToString().TrimEnd();
        }

        // Yahoo
        public static List<POVWebDomain.Models.ExternalApi.Attribute> ConvertToAttribute(this List<Specifications> specsList, List<POVWebDomain.Models.ExternalApi.Attribute> indexList)
        {
            foreach (var specs in specsList)
            {
                if (string.IsNullOrEmpty(specs.Key) || string.IsNullOrEmpty(specs.Value))
                    continue;

                indexList.Add(new POVWebDomain.Models.ExternalApi.Attribute
                {
                    Name = specs.Key,
                    Values = new List<string> { specs.Value }
                });
            }

            return indexList;
        }

        // 官網
        public static List<ProductSpecification> ConvertToOfficial(this List<Specifications> specsList, List<ProductSpecification> indexList)
        {
            foreach (var specs in specsList)
            {
                if (string.IsNullOrEmpty(specs.Key) || string.IsNullOrEmpty(specs.Value))
                    continue;

                indexList.Add(new ProductSpecification
                {
                    Title = specs.Key,
                    Value = specs.Value
                });
            }

            return indexList;
        }

    }

    public static class SkuListExtensions
    {
        // 91App
        public static string ConbineColDetail(this SkuItem sku)
        {
            string name = "";
            if (!string.IsNullOrEmpty(sku.ColDetail1.Label) && !string.IsNullOrEmpty(sku.ColDetail2.Label)) name = $"尺寸:{sku.ColDetail1.Label};顏色:{sku.ColDetail2.Label}";
            else if (!string.IsNullOrEmpty(sku.ColDetail1.Label)) name = $"尺寸:{sku.ColDetail1.Label}";
            else if (!string.IsNullOrEmpty(sku.ColDetail2.Label)) name = $"顏色:{sku.ColDetail2.Label}";

            return name;
        }

        // Official
        public static string ConbineColDetail_Official(this SkuItem sku)
        {
            string name = "";
            if (!string.IsNullOrEmpty(sku.ColDetail1.Label) && !string.IsNullOrEmpty(sku.ColDetail2.Label)) name = $"尺寸:{sku.ColDetail1.Label}*顏色:{sku.ColDetail2.Label}";
            else if (!string.IsNullOrEmpty(sku.ColDetail1.Label)) name = $"尺寸:{sku.ColDetail1.Label}";
            else if (!string.IsNullOrEmpty(sku.ColDetail2.Label)) name = $"顏色:{sku.ColDetail2.Label}";

            return name;
        }
    }

    public class MoreInfoResult
    {
        public string ProcessedHtml { get; set; }
        public List<ImageUploadInfo> UploadedImages { get; set; }
    }

    public class ImageUploadInfo
    {
        public string OriginalSrc { get; set; }
        public string NewSrc { get; set; }
    }

    public class GetLookupAndCommonValueResponse:ECStore
    {
        public string CountryID { get; set; }
        public string BrandID { get; set; }
    }

    public enum ImageType
    {
        Main,
        Sku,
        Size
    }

    #region  fromform的request
    public class StringContentWithoutContentType : StringContent
    {
        public StringContentWithoutContentType(string content) : base(content)
        {
            Headers.ContentType = null;
        }
    }
    #endregion

    #region 平台屬性
    public class EcIndex : Option<string>
    {
        public List<Option<string>> Options { get; set; }
        public bool IsMultipleSelect { get; set; }
        public bool IsRequired { get; set; } 
        public bool IsStandardAttribute { get; set; }
        public string PlatformSource { get; set; }
    }
    public class GetEcIndexReturn
    {
        public List<EcIndex> Options { get; set; }
        public List<POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods.IndexList> IndexList { get; set; }
    }

    public class AttributeMappingInfo
    {
        public EcIndex EcIndex { get; set; }
        public EC_AttributeMapping Mapping { get; set; }
    }

    public class EC_Attribute
    {
        public int Id { get; set; }
        public string Name { get; set; }           // '適用性別'
        public string SystemKey { get; set; }     // 'gender'
        public string Options { get; set; }       // JSON: ["男性","女性","通用"]
        public DateTime CreateTime { get; set; }
    }
    public class EC_AttributeMapping
    {
        public int Id { get; set; }
        public int AttributeId { get; set; }
        public string PlatformType { get; set; }        // 'momo', 'yahoo'
        public string PlatformFieldName { get; set; }   // '對象與族群'
        public string Type { get; set; }                // 'select', 'multi-select', 'text'
        public bool IsRequired { get; set; }
        public string ValueMappings { get; set; }       // JSON: {"男性":"男性","女性":"女性"}
        public DateTime CreateTime { get; set; }

        // 導航屬性 (如果需要的話)
        public EC_Attribute Attribute { get; set; }
    }

    /// <summary>
    /// ValueMapping 查詢結果
    /// </summary>
    public class ValueMappingInfo
    {
        public string SystemKey { get; set; }
        public string PlatformType { get; set; }
        public string PlatformFieldId { get; set; }
        public string ValueMappings { get; set; }
    }

    /// <summary>
    /// 平台屬性結果
    /// </summary>
    public class PlatformAttribute
    {
        public string FieldName { get; set; }
        public List<string> Values { get; set; }
    }


    public class ConvertedSkuResponse
    {
        public List<SkuItem> SkuList { get; set; }
        public SkuAttributeInfo AttributeInfo { get; set; }
    }

    public class SkuAttributeInfo
    {
        public string ColDetail1AttributeName { get; set; }  // 第一層屬性名稱（Size欄位對應的）
        public string ColDetail2AttributeName { get; set; }  // 第二層屬性名稱（Color欄位對應的）
    }


    public class PlatformConvertResult
    {
        public List<IndexList> StandardAttributes { get; set; } = new List<IndexList>();
        public List<IndexList> CustomAttributes { get; set; } = new List<IndexList>();
    }
    #endregion

    #region html圖片處理
    public class HtmlImageProcessResult
    {
        public string ProcessedHtml { get; set; }
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
        public List<string> FailedImages { get; set; } = new List<string>();
    }
    #endregion

    public class GetOptionAllResponse
    {
        public dynamic Category_Official { get; set; }
        public dynamic ShipType_91app { get; set; }
        public dynamic Payment { get; set; }
        public dynamic SpecChart { get; set; }
        public dynamic ShopCategory { get; set; }
        public dynamic SalesModeType { get; set; }
        public dynamic SellingDateTime { get; set; }
        public dynamic EcIndex { get; set; }
    }
}
