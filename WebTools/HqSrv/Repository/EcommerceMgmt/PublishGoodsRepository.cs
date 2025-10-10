using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting;
using POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods;
using POVWebDomain.Models.DB.POVWeb;
using System.IO;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Drawing.Imaging;
using System.Drawing;
using System.Data;
using Newtonsoft.Json;
using System.Net;
using System.Linq;
using POVWebDomain.Models.API.StoreSrv.Common;
using System.Dynamic;
using Newtonsoft.Json.Linq;
using HtmlAgilityPack;
using POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.Common;
using POVWebDomain.Models.ExternalApi.Momo;
using POVWebDomain.Models.ExternalApi;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using IndexList = POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods.IndexList;
using JsonSerializer = System.Text.Json.JsonSerializer;
using static OfficeOpenXml.ExcelErrorValue;
using System.Security.Cryptography.Xml;
using POVWebDomain.Models.ExternalApi.ShopeeSCM;

namespace HqSrv.Repository.EcommerceMgmt
{
    public class PublishGoodsRepository
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly POVWebDbContextDapper _context;
        private readonly IConfiguration _configuration;

        public PublishGoodsRepository(
            POVWebDbContextDapper context, 
            IWebHostEnvironment hostEnvironment,
            IConfiguration configuration
        )
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _configuration = configuration;
        }

        public static async Task<string> GenerateFileHashAsync(IFormFile file)
        {
            using (var sha256 = SHA256.Create())
            {
                using (var stream = file.OpenReadStream())
                {
                    var hashBytes = await sha256.ComputeHashAsync(stream);

                    var hashStringBuilder = new StringBuilder();
                    foreach (var b in hashBytes)
                    {
                        hashStringBuilder.Append(b.ToString("x2"));
                    }

                    return hashStringBuilder.ToString();
                }
            }
        }

        private void EnsureDirectoryExists(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        public async Task<(object, string)> SaveSubmitGoodsReq(SubmitMainRequestAll request)
        {
            
            string saveReqSql = @"
                MERGE INTO ESubmitGoodsReq AS target
                USING (VALUES (@ParentID, @RequestParams, GETDATE(), @ChangePerson)) AS source (ParentID, RequestParams, ChangeTime, ChangePerson)
                ON target.ParentID = source.ParentID
                WHEN MATCHED THEN
                    UPDATE SET RequestParams = source.RequestParams, ChangeTime = source.ChangeTime, ChangePerson = source.ChangePerson
                WHEN NOT MATCHED THEN
                    INSERT (ParentID, RequestParams, ChangeTime, ChangePerson)
                    VALUES (source.ParentID, source.RequestParams, source.ChangeTime, source.ChangePerson);
            ";

            string message = string.Empty;
            int effectRows = 0;

            var connection = _context.Connection;
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                effectRows = await connection.ExecuteAsync(saveReqSql, new { request.ParentID, RequestParams = request.JsonData, request.ChangePerson }, transaction: transaction, commandTimeout: 180);

                transaction.Commit();

                return (new { effectRows }, message);
            }
            catch (SqlException e)
            {
                message = e.Message;
                return (null, message);
            }
            finally
            {
                connection.Close();
            }
        }


        public async Task<(object, string)> SaveSubmitGoodsRes(SubmitMainRequestAll request, object requestDto, SubmitMainResponseAll response, StoreSetting store)
        {
            JObject objJson = JObject.Parse(request.JsonData);
            JObject obj = JObject.Parse(request.BasicInfo);
            obj["categoryId"] = store.CategoryId;
            obj["title"] = store.Title;
            obj["cost"] = store.Cost;
            obj["moreInfo"] = objJson["moreInfo"];
            //string requestParams = obj.ToString(Formatting.None);

            string saveResSql = @"
                MERGE INTO ESubmitGoodsRes AS target
                USING (VALUES (@StoreID, @ParentID, @ResponseData, GETDATE(), @ChangePerson, @RequestParams)) AS source (StoreID, ParentID, ResponseData, ChangeTime, ChangePerson, RequestParams)
                ON target.StoreID = source.StoreID AND target.ParentID = source.ParentID
                WHEN MATCHED THEN
                    UPDATE SET ResponseData = source.ResponseData, ChangeTime = source.ChangeTime, ChangePerson = source.ChangePerson,
                               RequestParams = source.RequestParams
                WHEN NOT MATCHED THEN
                    INSERT (StoreID, ParentID, ResponseData, ChangeTime, ChangePerson, RequestParams)
                    VALUES (source.StoreID, source.ParentID, source.ResponseData, source.ChangeTime, source.ChangePerson, source.RequestParams);
            ";

            string message = string.Empty;
            int effectRows = 0;
            var connection = _context.Connection;
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                effectRows = await connection.ExecuteAsync(saveResSql, new { storeID = store.PlatformID, request.ParentID, ResponseData = JsonConvert.SerializeObject(response.Response), request.ChangePerson, RequestParams = JsonSerializer.Serialize(requestDto) }, transaction: transaction, commandTimeout: 180);

                transaction.Commit();

                return (new { effectRows }, message);
            }
            catch (SqlException e)
            {
                message = e.Message;
                return (null, message);
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task<(object, string)> GetSubmitMode(GetSubmitModeRequest request)
        {

            string searchSql = @"
                DECLARE @ParentID VARCHAR(20) = (SELECT TOP 1 ParentID FROM Goods WHERE GoodID=@GoodID)
                IF EXISTS(SELECT 1 FROM ESubmitGoodsReq WHERE ParentID=@ParentID)
                    SELECT 'edit' AS Mode, @ParentID AS ParentID
                ELSE 
                    SELECT 'add' AS Mode, @ParentID AS ParentID
            ";

            string message = string.Empty;
            using (var connection = _context.Connection)
            {
                connection.Open();
                try
                {
                    var result = connection.QueryFirstOrDefault(searchSql, request);
                    return (new { result }, message);
                }
                catch (SqlException e)
                {
                    message = e.Message;
                    return (null, message);
                }
            }
        }

        public async Task<GetSubmitModeReponse> GetSubmitResByStore(string parentID, string platformID)
        {

            string searchSql = @"
                SELECT ResponseData, RequestParams 
                FROM ESubmitGoodsRes WHERE ParentID=@ParentID AND StoreID=@platformID
            ";

            string message = string.Empty;
            var connection = _context.Connection;
            connection.Open();
            try
            {
                var resData = connection.QueryFirstOrDefault<GetSubmitModeReponse>(searchSql, new { parentID, platformID });
                return resData;
            }
            catch (SqlException e)
            {
                message = e.Message;
                throw new Exception(message);
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task<(object, string)> GetSubmitDefVal(GetSubmitDefValRequest request)
        {

            string searchSql = @"
                SELECT RequestParams FROM ESubmitGoodsReq WHERE ParentID=@ParentID

                SELECT G.GoodID, G.GoodName, G.AdvicePrice, 
                       ISNULL(MIN(CASE pjtpsort when '2' THEN ROUND(G.AdvicePrice*A.pjtdiscount/100,0) ELSE pjtprice END),G.SpecialPrice) AS Price, 
                       G.Cost, G.Sort05 AS ColorID, S.Sort05Name AS ColorName, G.SizeName AS SizeID, ST.SizeTagName AS SizeName
                FROM Goods G
                LEFT JOIN Sort05 S ON G.Sort05=S.Sort05ID
                LEFT JOIN SizeTag ST ON G.SizeName=ST.SizeTagID
                LEFT JOIN anewpjtdetail AD ON G.GoodID = AD.goodid
                LEFT JOIN anewpjt A ON A.pjtid = AD.pjtid
                LEFT JOIN anewpjtrange AR ON A.pjtid = AR.pjtid
                LEFT JOIN Client C ON C.Remark02='4' AND C.ClientID BETWEEN AR.sstore AND AR.estore
                WHERE G.ParentID = @ParentID
                  AND (AR.sdate IS NULL OR AR.sdate <= CONVERT(VARCHAR, GETDATE(), 112))
                  AND (AR.edate IS NULL OR AR.edate >= CONVERT(VARCHAR, GETDATE(), 112))
                GROUP BY G.GoodID, G.GoodName, G.AdvicePrice, G.Cost, G.Sort05, S.Sort05Name, G.SizeName, ST.SizeTagName, G.SpecialPrice
                
            ";

            string message = string.Empty;
            using (var connection = _context.Connection)
            {
                connection.Open();
                try
                {
                    List<SkuItem> skuItems = new List<SkuItem>();
                    var result = await connection.QueryMultipleAsync(searchSql, request, commandTimeout: 180);
                    var submitHistory = result.IsConsumed ? null : result.Read().ToList().FirstOrDefault();
                    var goodsList = result.IsConsumed ? null : result.Read<SkuItem_Goods>().ToList();


                    if (submitHistory != null)
                    {
                        IDictionary<string, object> submitHistoryDict = (IDictionary<string, object>)submitHistory;
                        if (submitHistoryDict.ContainsKey("RequestParams"))
                        {
                            if (submitHistoryDict["RequestParams"] != null)
                            {
                                submitHistory = submitHistoryDict["RequestParams"].ToString();
                                skuItems = JsonConvert.DeserializeObject<SkuWrapper>(submitHistory)?.SkuList;
                            }
                            else submitHistory = "{}";
                        }
                    }

                    if (goodsList.Count == 0)
                    {
                        message = "沒有款式編號!";
                        return (null, message);
                    }

                    object defVal = new
                    {
                        Title = goodsList[0].GoodName,
                        SuggestPrice = goodsList[0].AdvicePrice,
                        Price = goodsList[0].Price,
                        Cost = goodsList[0].Cost,
                        OuterId = goodsList[0].GoodID
                    };
                    
                    var optionList = CreateOptionList(goodsList);
                    var missingItems = CreateMissingItems(skuItems, goodsList);
                    return (new { defVal, submitHistory, missingItems, optionList }, message);
                }
                catch (SqlException e)
                {
                    message = e.Message;
                    return (null, message);
                }
            }
        }

        public async Task<(object, string)> GetEStoreCatOptions()
        {

            string searchSql = @"
                SELECT EStoreID, CategoryID, CategoryName
                FROM EstoreCat
                WHERE Status = 1
                ORDER BY EStoreID, CategoryID
            ";

            string message = string.Empty;
            using (var connection = _context.Connection)
            {
                connection.Open();
                try
                {
                    List<GetEStoreCatResponse> response = (await connection.QueryAsync<GetEStoreCatResponse>(searchSql)).ToList();

                    var result =  response.GroupBy(r => new { r.EStoreID })
                        .Select(r => new
                        {
                            r.Key.EStoreID,
                            Option = r.Select(opt => new
                            {
                                Value = opt.CategoryID,
                                Label = opt.CategoryName
                            })
                        });

                    return (new { result }, message);
                }
                catch (SqlException e)
                {
                    message = e.Message;
                    return (null, message);
                }
            }
        }

        public async Task<GetLookupAndCommonValueResponse> GetLookupAndCommonValue(string parentID, string storeID)
        {

            string getLookupSql = @"
                SELECT Top 1 
                    CM.MappingID AS CountryID, BM.MappingID AS BrandID, 
                    E.EstoreID, E.IsRestricted_91, E.SoldOutActionType_91, E.Status_91, E.IsShowPurchaseList_91, E.IsShowSoldQty_91, E.IsShowStockQty_91, 
                    E.GoodsType_Momo, E.IsECWarehouse_Momo, E.HasAs_Momo, E.IsCommission_Momo, E.IsAcceptTravelCard_Momo, E.OutplaceSeq_Momo, E.OutplaceSeqRtn_Momo, E.IsIncludeInstall_Momo,
                    E.LiveStreamYn_Momo, 
                    E.ContentRating_Yahoo, E.ProductWarrantlyPeriod_Yahoo, E.ProductWarrantlyScope_Yahoo, E.ProductWarrantlyHandler_Yahoo, E.ProductWarrantlyDescription_Yahoo,
                    E.IsInstallRequired_Yahoo, E.IsLargeVolumnProductGift_Yahoo, E.IsNeedRecycle_Yahoo, E.IsOutrightPurchase_Yahoo,
                    E.Condition_Shopee, E.DescriptionType_Shopee
                FROM Goods G
                LEFT JOIN Country C ON G.P_StyleE = C.CountryID
                LEFT JOIN Country_Mapping CM ON C.CountryID = CM.CountryID AND CM.EStoreID = @EstoreID
                LEFT JOIN Brand B ON G.Brand = B.BrandID
                LEFT JOIN Brand_Mapping BM ON B.BrandID = BM.BrandID AND BM.EStoreID = @EstoreID
                LEFT JOIN EC_Store E ON E.EStoreID = @EStoreID
                WHERE ParentID = @ParentID
            ";

            string message = string.Empty;
            var connection = _context.Connection;
            connection.Open();
            try
            {
                var resData = connection.QueryFirstOrDefault<GetLookupAndCommonValueResponse>(getLookupSql, new { parentID, EStoreID = storeID });
                return resData;
            }
            catch (SqlException e)
            {
                message = e.Message;
                throw new Exception(message);
            }
            finally
            {
                connection.Close();
            }
        }
        public List<OptionItem> CreateOptionList(List<SkuItem_Goods> goodsList)
        {
            var optionList = new List<OptionItem>();

            // 處理顏色選項
            var uniqueColors = goodsList
                .Select(x => (x.ColorID, x.ColorName))
                .Distinct()
                .Where(x => !string.IsNullOrEmpty(x.ColorID))
                .Select(x => new Option<string>(x.ColorID, x.ColorName))
                .ToList();

            if (uniqueColors.Count() > 0)
            {
                optionList.Add(new OptionItem
                {
                    Name = "顏色",
                    Options = new OptionValue
                    {
                        InputValue = "",
                        Value = uniqueColors
                    }
                });
            }

            // 處理尺寸選項
            var uniqueSizes = goodsList
                .Select(x => (x.SizeID, x.SizeName))
                .Distinct()
                .Where(x => !string.IsNullOrEmpty(x.SizeID))
                .Select(x => new Option<string>(x.SizeID, x.SizeName))
                .ToList();

            if (uniqueSizes.Count() > 0)
            {
                optionList.Add(new OptionItem
                {
                    Name = "尺寸",
                    Options = new OptionValue
                    {
                        InputValue = "",
                        Value = uniqueSizes
                    }
                });
            }

            return optionList;
        }

        public List<SkuItem> CreateMissingItems(List<SkuItem> skuItems, List<SkuItem_Goods> goodsList)
        {
            var missingItems = new List<SkuItem>();


            bool hasSize = skuItems.Any(t => !string.IsNullOrEmpty(t.ColDetail1.Value));
            bool hasColor = skuItems.Any(t => !string.IsNullOrEmpty(t.ColDetail2.Value));

            var uniqueColors = goodsList
               .Select(x => (x.ColorID, x.ColorName))
               .Distinct()
               .Where(x => !string.IsNullOrEmpty(x.ColorID))
               .ToList();

            var uniqueSizes = goodsList
                .Select(x => (x.SizeID, x.SizeName))
                .Distinct()
                .Where(x => !string.IsNullOrEmpty(x.SizeID))
                .ToList();

            missingItems = goodsList
                        .Where(goods => !skuItems.Any(sku => sku.OriginalOuterId == goods.GoodID))
                        .Select(goods => new SkuItem
                        {
                            Qty = 0,
                            OnceQty = 50,
                            OuterId = goods.GoodID,
                            SafetyStockQty = 0,
                            OriginalOuterId = null,
                            OriginalQty = 0,
                            SuggestPrice = goods.AdvicePrice,
                            Price = goods.Price,
                            Cost = goods.Cost

                        })
                        .ToList();

            foreach (var item in missingItems)
            {
                var targetGood = goodsList.First(g => g.GoodID == item.OuterId);
                if (skuItems.Count > 0)
                {
                    if (hasSize)
                    {
                        item.ColDetail1 = new Option<string>(targetGood.SizeID, targetGood.SizeName);
                    }

                    if (hasColor)
                    {
                        item.ColDetail2 = new Option<string>(targetGood.ColorID, targetGood.ColorName);
                    }
                }
                else
                {
                    if (uniqueSizes.Count>0)
                    {
                        item.ColDetail1 = new Option<string>(targetGood.SizeID, targetGood.SizeName);
                    }

                    if ( uniqueColors.Count>0)
                    {
                        item.ColDetail2 = new Option<string>(targetGood.ColorID, targetGood.ColorName);
                    }
                }
                
            }


            return missingItems;
        }

        public async Task HandleImage(SubmitMainRequestAll request)
        {
            JObject obj = JObject.Parse(request.JsonData);

            for (int idx = 0; idx < request.MainImage?.Count; idx++)
            {
                IFormFile image = request.MainImage[idx];
                if (image.FileName == "blob") continue;
                var result = await SavePicture(image, request.ParentID, "mainImage", idx);
                if(idx == 0) await SavePicture(image, request.ParentID, "ad", idx);
                string path = ((dynamic)result.Item1).path;
                obj["mainImage"][idx]["path"] = path == "" ? image.FileName : path;
            }

            for (int idx = 0; idx < request.SkuImage?.Count; idx++)
            {
                IFormFile image = request.SkuImage[idx];
                var result = await SavePicture(image, request.ParentID, "skuImage", idx);
                string path = ((dynamic)result.Item1).path;
                if (image.FileName == "blob") continue;
                obj["skuList"][idx]["image"]["path"] = path == "" ? image.FileName : path;
            }

            if (request.SizeImage != null)
            {
                IFormFile image = request.SizeImage;
                var result = await SavePicture(image, request.ParentID, "sizeImage", 0);
                string path = ((dynamic)result.Item1).path;
                if (image.FileName != "blob")
                {
                    obj["sizeImage"]["path"] = path == "" ? image.FileName : path;
                }
               
            }
           

            MoreInfoResult moreInfoResult = await ProcessMoreInfoAsync((string)obj["moreInfo"], request.ParentID, request.Origin);

            obj["moreInfo"] = moreInfoResult.ProcessedHtml;

            request.JsonData = obj.ToString(Formatting.None);

        }
        public async Task<(object, string)> SavePicture(IFormFile file, string baseName, string type, int? index = -1)
        {

            string message = string.Empty;

            string relativePath = "";

            if (file != null && file.Length > 0)
            {
                try
                {
                    string fileHash = "";
                    string folder = $"Images/PublishGoods/{baseName}";
                    string directory = Path.Combine(_hostEnvironment.ContentRootPath, folder);

                    switch (type)
                    {
                        case "mainImage":
                            fileHash = $"{baseName}_B{index + 1}.jpg";
                            break;
                        case "skuImage":
                            fileHash = $"{baseName}_01_00{index + 1}_B.jpg";
                            break;
                        case "moreInfo":
                            fileHash = $"{baseName}_m_1_{index + 1}.jpg";
                            break;
                        case "ad":
                            fileHash = $"{baseName}_O.jpg";
                            break;
                        case "sizeImage":
                            fileHash = $"{baseName}_Size.jpg";
                            break;
                        default: break;
                    }

                    relativePath = $"{folder}/{fileHash}";

                    EnsureDirectoryExists(directory);
                    string filePath = Path.Combine(_hostEnvironment.ContentRootPath, relativePath);

                    using (var stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        stream.Position = 0;
                        using (var image = System.Drawing.Image.FromStream(stream))
                        {
                            image.Save(filePath, ImageFormat.Jpeg);
                        }
                    }
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                    return ("", message);
                }
            }
            return (new { path = relativePath }, message);
        }


        public async Task<MoreInfoResult> ProcessMoreInfoAsync(string originalHtml, string baseName, string origin)
        {
            // 1. 解析HTML
            var doc = new HtmlDocument();
            doc.LoadHtml(originalHtml);

            // 2. 找出所有圖片節點
            var imgNodes = doc.DocumentNode.SelectNodes("//img") ?? Enumerable.Empty<HtmlNode>();

            // 3. 處理圖片
            int imgIndex = 0;
            var uploadedImages = new List<ImageUploadInfo>();
            foreach (var imgNode in imgNodes)
            {
                var src = imgNode.GetAttributeValue("src", "");

                // 如果是Base64或Blob圖片
                if (ImageCompressorExtensions.IsBase64OrBlobImage(src))
                {
                    // 上傳圖片到圖片站台
                    var uploadResult = await UploadImageFromSourceAsync(src, baseName, imgIndex);

                    // 更新HTML中的圖片地址
                    imgNode.SetAttributeValue("src", $"{origin}/{uploadResult.NewSrc}");

                    uploadedImages.Add(uploadResult);
                    imgIndex++;
                }
                else if (ImageCompressorExtensions.IsExternalImage(src))
                {
                    // 下載並上傳外部圖片
                    var uploadResult = await UploadExternalImageAsync(src, baseName, imgIndex);

                    // 更新HTML中的圖片地址
                    imgNode.SetAttributeValue("src", $"{origin}/{uploadResult.NewSrc}");

                    uploadedImages.Add(uploadResult);
                    imgIndex++;
                }
            }

            // 4. 生成處理後的HTML
            var processedHtml = doc.DocumentNode.OuterHtml;

            return new MoreInfoResult
            {
                ProcessedHtml = processedHtml,
                UploadedImages = uploadedImages
            };
        }

        #region 建構平台屬性
        /// <summary>
        /// 智能合併多平台屬性
        /// </summary>
        public async Task<GetEcIndexReturn> MergeEcAttributes(List<string> platforms, string categoryCode = null)
        {
            // 1. 收集所有平台屬性
            var allEcIndexes = new List<EcIndex>();

            // 2. 查詢對應關係
            var mappings = await GetAttributeMappings(platforms);

            // 3. 分類屬性：系統標準 vs 平台專屬
            var standardAttributeGroups = new Dictionary<int, List<AttributeMappingInfo>>();
            var platformSpecificAttributes = new List<EcIndex>();

            foreach (var ecIndex in allEcIndexes)
            {
                var mapping = mappings.FirstOrDefault(m =>
                    m.PlatformFieldName.Equals(ecIndex.Label, StringComparison.OrdinalIgnoreCase) &&
                    m.PlatformType == ecIndex.PlatformSource);

                if (mapping != null)
                {
                    // 系統標準屬性
                    if (!standardAttributeGroups.ContainsKey(mapping.AttributeId))
                        standardAttributeGroups[mapping.AttributeId] = new List<AttributeMappingInfo>();

                    standardAttributeGroups[mapping.AttributeId].Add(new AttributeMappingInfo
                    {
                        EcIndex = ecIndex,
                        Mapping = mapping
                    });
                }
                else
                {
                    // 平台專屬屬性
                    ecIndex.IsStandardAttribute = false;
                    ecIndex.PlatformSource = ecIndex.PlatformSource;
                    platformSpecificAttributes.Add(ecIndex);
                }
            }

            // 4. 合併系統標準屬性
            var mergedStandardAttributes = await MergeStandardAttributes(standardAttributeGroups);

            // 5. 組合最終結果
            var finalOptions = mergedStandardAttributes.Concat(platformSpecificAttributes).ToList();

            // 6. 產生空的 IndexList
            var indexList = GenerateEmptyIndexList(finalOptions);

            return new GetEcIndexReturn
            {
                Options = finalOptions,
                IndexList = indexList
            };
        }

        /// <summary>
        /// 查詢屬性對應關係
        /// </summary>
        private async Task<List<EC_AttributeMapping>> GetAttributeMappings(List<string> platforms)
        {
            string connectionString = _configuration["ConnectionStrings:POVWebDb"];
            using var connection = new SqlConnection(connectionString);

            var sql = @"
            SELECT am.*, a.Name, a.SystemKey, a.Options 
            FROM EC_AttributeMapping am
            INNER JOIN EC_Attribute a ON am.AttributeId = a.Id
            WHERE am.PlatformType IN @platforms";

            return (await connection.QueryAsync<EC_AttributeMapping>(sql, new { platforms })).ToList();
        }

        /// <summary>
        /// 合併系統標準屬性
        /// </summary>
        private async Task<List<EcIndex>> MergeStandardAttributes(
            Dictionary<int, List<AttributeMappingInfo>> standardAttributeGroups)
        {
            if (!standardAttributeGroups.Any())
                return new List<EcIndex>();

            // 取得標準屬性定義
            var attributeIds = standardAttributeGroups.Keys;
            var standardAttributes = await GetStandardAttributeDefinitions(attributeIds);

            var mergedAttributes = new List<EcIndex>();

            foreach (var group in standardAttributeGroups)
            {
                var attributeId = group.Key;
                var mappingInfos = group.Value;
                var standardAttribute = standardAttributes.First(a => a.Id == attributeId);

                // 套用最嚴格規則
                var finalIsRequired = mappingInfos.Any(info => info.Mapping.IsRequired);
                var finalIsMultipleSelect = mappingInfos.All(info => info.Mapping.Type == "multi-select");

                // 建立合併後的屬性
                var mergedAttribute = new EcIndex
                {
                    Label = standardAttribute.Name,
                    Value = standardAttribute.SystemKey,
                    IsMultipleSelect = finalIsMultipleSelect,
                    IsRequired = finalIsRequired,
                    IsStandardAttribute = true,
                    PlatformSource = "standard",
                    Options = ParseStandardOptions(standardAttribute.Options)
                };

                mergedAttributes.Add(mergedAttribute);
            }

            return mergedAttributes;
        }

        /// <summary>
        /// 取得標準屬性定義
        /// </summary>
        private async Task<List<EC_Attribute>> GetStandardAttributeDefinitions(IEnumerable<int> attributeIds)
        {
            string connectionString = _configuration["ConnectionStrings:POVWebDb"];
            using var connection = new SqlConnection(connectionString);

            var sql = "SELECT * FROM EC_Attribute WHERE Id IN @attributeIds";

            return (await connection.QueryAsync<EC_Attribute>(sql, new { attributeIds })).ToList();
        }

        /// <summary>
        /// 解析標準選項
        /// </summary>
        private List<Option<string>> ParseStandardOptions(string optionsJson)
        {
            if (string.IsNullOrEmpty(optionsJson))
                return new List<Option<string>>();

            var optionValues = System.Text.Json.JsonSerializer.Deserialize<List<string>>(optionsJson);

            return optionValues.Select(value => new Option<string>
            {
                Label = value,
                Value = value
            }).ToList();
        }

        /// <summary>
        /// 產生空的 IndexList
        /// </summary>
        private List<POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods.IndexList> GenerateEmptyIndexList(
            List<EcIndex> options)
        {
            return options.Select(option => new POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods.IndexList
            {
                // 根據你的 IndexList 結構設定空值
                // 例如：
                KeyNo = option.Value,
                Key = option.Label,
                PlatformSource = option.PlatformSource,
                Value = new List<Option<string>>()
            }).ToList();
        }


        #endregion

        #region 還原平台屬性

        /// <summary>
        /// 轉換前端填寫資料為各平台格式
        /// </summary>
        public async Task<Dictionary<string, PlatformConvertResult>> ConvertToPlatformAttributes(
            List<IndexList> indexList, List<string> targetPlatforms)
        {
            var result = new Dictionary<string, PlatformConvertResult>();

            // 初始化各平台結果
            foreach (var platform in targetPlatforms)
            {
                result[platform] = new PlatformConvertResult();
            }

            // 查詢所有需要的 ValueMappings
            var standardAttributes = indexList.Where(x => x.PlatformSource == "standard").ToList();
            var valueMappings = await GetValueMappings(standardAttributes, targetPlatforms);

            // 處理每個屬性
            foreach (var item in indexList)
            {
                if (item.PlatformSource == "standard")
                {
                    // 系統標準屬性：嘗試轉換給所有目標平台
                    ProcessStandardAttribute(item, targetPlatforms, valueMappings, result);
                }
                else
                {
                    // 平台專屬屬性：只給對應平台當標準屬性，其他平台當自定義
                    ProcessPlatformSpecificAttribute(item, targetPlatforms, result);
                }
            }

            return result;
        }

        /// <summary>
        /// 處理系統標準屬性轉換
        /// </summary>
        private void ProcessStandardAttribute(
            IndexList item,
            List<string> targetPlatforms,
            List<ValueMappingInfo> valueMappings,
            Dictionary<string, PlatformConvertResult> result)
        {
            foreach (var platform in targetPlatforms)
            {
                var mapping = valueMappings.FirstOrDefault(vm =>
                    vm.SystemKey.Equals(item.KeyNo, StringComparison.OrdinalIgnoreCase) &&
                    vm.PlatformType.Equals(platform, StringComparison.OrdinalIgnoreCase));

                if (mapping != null)
                {
                    // 找到對應：加到標準屬性
                    var convertedValues = ConvertValues(item.Value, mapping.ValueMappings, platform);

                    result[platform].StandardAttributes.Add(new IndexList
                    {
                        Key = mapping.PlatformFieldId,
                        Value = convertedValues.Select(t => new Option<string> { Value = t, Label = t }).ToList()
                    });
                }
                else
                {
                    // 找不到對應：加到自定義屬性
                    Console.WriteLine($"標準屬性 {item.Key} 在 {platform} 找不到對應，歸到自定義屬性");

                    var customValue = ConvertToCustomAttributeString(item.Value);
                    result[platform].CustomAttributes.Add(new IndexList
                    {
                        Key = item.Key,
                        Value = new List<Option<string>> { new Option<string> { Value = customValue, Label = customValue } }
                    });
                }
            }
        }

        /// <summary>
        /// 處理平台專屬屬性
        /// </summary>
        private void ProcessPlatformSpecificAttribute(
            IndexList item,
            List<string> targetPlatforms,
            Dictionary<string, PlatformConvertResult> result)
        {
            // 只處理對應的平台，其他平台忽略
            if (targetPlatforms.Contains(item.PlatformSource, StringComparer.OrdinalIgnoreCase))
            {
                result[item.PlatformSource].StandardAttributes.Add(new IndexList
                {
                    Key = item.Key,
                    Value = item.Value ?? new List<Option<string>>()
                });

                Console.WriteLine($"平台專屬屬性 {item.Key} 加到 {item.PlatformSource} 的標準屬性");
            }
            else
            {
                Console.WriteLine($"平台專屬屬性 {item.Key} ({item.PlatformSource}) 不在目標平台清單中，忽略");
            }
        }

        /// <summary>
        /// 將複雜的 Option 值轉換為簡單字串（用於自定義屬性，多值用逗號區隔）
        /// </summary>
        private string ConvertToCustomAttributeString(List<Option<string>> originalValues)
        {
            if (originalValues == null || !originalValues.Any())
                return string.Empty;

            var stringValues = originalValues
                .Select(option => option.Value ?? option.Label ?? "")
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .ToList();

            return string.Join(",", stringValues);
        }



        /// <summary>
        /// 轉換屬性值
        /// </summary>
        private List<string> ConvertValues(List<Option<string>> originalValues, string valueMappingsJson, string platform)
        {
            if (string.IsNullOrEmpty(valueMappingsJson) || !originalValues?.Any() == true)
                return originalValues?.Select(o => o.Value ?? o.Label).ToList() ?? new List<string>();

            try
            {
                var mappings = JsonSerializer.Deserialize<Dictionary<string, string>>(valueMappingsJson);
                var convertedValues = new List<string>();

                foreach (var option in originalValues)
                {
                    // 使用 Option 的 Value 或 Text 進行轉換
                    var valueToConvert = option.Value ?? option.Label;

                    if (mappings.TryGetValue(valueToConvert, out var mappedValue))
                    {
                        // 處理多選對應 (如 "通用" → "男,女")
                        if (mappedValue.Contains(","))
                        {
                            convertedValues.AddRange(mappedValue.Split(',').Select(v => v.Trim()));
                        }
                        else
                        {
                            convertedValues.Add(mappedValue);
                        }
                    }
                    else
                    {
                        // 找不到對應時使用原值
                        Console.WriteLine($"找不到值 '{valueToConvert}' 在 {platform} 的對應，使用原值");
                        convertedValues.Add(valueToConvert);
                    }
                }

                return convertedValues;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ValueMappings 轉換錯誤: {ex.Message}, JSON: {valueMappingsJson}");
                return originalValues?.Select(o => o.Value ?? o.Label).ToList() ?? new List<string>();
            }
        }

        /// <summary>
        /// 查詢 ValueMappings 資料
        /// </summary>
        private async Task<List<ValueMappingInfo>> GetValueMappings(
            List<IndexList> standardAttributes,
            List<string> targetPlatforms)
        {
            if (!standardAttributes.Any() || !targetPlatforms.Any())
                return new List<ValueMappingInfo>();

            string connectionString = _configuration["ConnectionStrings:POVWebDb"];
            using var connection = new SqlConnection(connectionString);

            var sql = @"
            SELECT 
                a.SystemKey,
                m.PlatformType,
                m.PlatformFieldId,
                m.Type,
                m.IsRequired,
                m.ValueMappings
            FROM EC_AttributeMapping m
            INNER JOIN EC_Attribute a ON m.AttributeId = a.Id
            WHERE a.Name IN @attributeNames 
            AND m.PlatformType IN @platforms";

            var attributeNames = standardAttributes.Select(x => x.Key).Distinct().ToList();

            return (await connection.QueryAsync<ValueMappingInfo>(sql, new
            {
                attributeNames,
                platforms = targetPlatforms
            })).ToList();
        }

        #endregion

        #region 還原平台裡顏色和尺寸的屬性和代號
        public async Task<ConvertedSkuResponse> ConvertSkuForPlatformAsync(
        string parentID,
        List<SkuItem> skuList,
        string platformType)
        {
            try
            {
                // 1. 取得商品大類
                var sort01 = await GetSort01ByParentIDAsync(parentID);
                if (string.IsNullOrEmpty(sort01))
                {
                    throw new Exception($"找不到 ParentID: {parentID} 對應的商品分類");
                }

                // 2. 取得屬性名稱對應
                var attributeMapping = await GetAttributeMappingAsync(sort01, platformType);
                if (attributeMapping == null)
                {
                    throw new Exception($"找不到大類: {sort01} 在平台: {platformType} 的屬性對應設定");
                }

                // 3. 轉換 SKU 清單
                var convertedSkuList = await ConvertSkuListAsync(skuList, platformType);

                // 4. 回傳結果
                return new ConvertedSkuResponse
                {
                    SkuList = convertedSkuList,
                    AttributeInfo = new SkuAttributeInfo
                    {
                        ColDetail1AttributeName = attributeMapping.ColDetail1AttributeName,    // ColDetail1 是尺寸
                        ColDetail2AttributeName = attributeMapping.ColDetail2AttributeName    // ColDetail2 是顏色
                    }
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"SKU轉換失敗: {ex.Message}", ex);
            }
        }

        private async Task<string> GetSort01ByParentIDAsync(string parentID)
        {
            var sql = @"
            SELECT TOP 1 Sort01 
            FROM Goods 
            WHERE ParentID = @ParentID";

            var connection = _context.Connection;
            return await connection.QueryFirstOrDefaultAsync<string>(sql, new { ParentID = parentID });
        }

        private async Task<SkuAttributeInfo> GetAttributeMappingAsync(string sort01, string platformType)
        {
            var sql = @"
            SELECT ColorAttributeKey as ColDetail2AttributeName, SizeAttributeKey as ColDetail1AttributeName
            FROM EC_CategoryAttributeMapping 
            WHERE Sort01ID = @Sort01 AND PlatformType = @PlatformType";

            var connection = _context.Connection;
            var result = await connection.QueryFirstOrDefaultAsync<SkuAttributeInfo>(sql,
                new { Sort01 = sort01, PlatformType = platformType });

            // 如果沒找到，就用預設值
            if (result == null)
            {
                result = new SkuAttributeInfo
                {
                    ColDetail1AttributeName = "尺寸",  // ColDetail1 預設是尺寸
                    ColDetail2AttributeName = "顏色"   // ColDetail2 預設是顏色
                };
            }

            return result;
        }

        private async Task<List<SkuItem>> ConvertSkuListAsync(List<SkuItem> skuList, string platformType)
        {
            // 取得所有需要轉換的 ColorID 和 SizeID
            var colorIDs = skuList.Where(x => x.ColDetail2?.Value != null)
                                  .Select(x => x.ColDetail2.Value)
                                  .Distinct()
                                  .ToList();

            var sizeIDs = skuList.Where(x => x.ColDetail1?.Value != null)
                                 .Select(x => x.ColDetail1.Value)
                                 .Distinct()
                                 .ToList();

            // 批量查詢轉換對照
            var colorMappings = await GetColorMappingsAsync(colorIDs, platformType);
            var sizeMappings = await GetSizeMappingsAsync(sizeIDs, platformType);

            // 轉換每個 SkuItem
            var convertedList = new List<SkuItem>();

            foreach (var sku in skuList)
            {
                var convertedSku = CloneSku(sku);

                // 轉換 ColDetail1 (Size)
                if (convertedSku.ColDetail1?.Value != null)
                {
                    var sizeMapping = sizeMappings.FirstOrDefault(x => x.Key == convertedSku.ColDetail1.Value);
                    if (sizeMapping.Value != null)
                    {
                        convertedSku.ColDetail1.Value = sizeMapping.Value;
                    }
                }

                // 轉換 ColDetail2 (Color)
                if (convertedSku.ColDetail2?.Value != null)
                {
                    var colorMapping = colorMappings.FirstOrDefault(x => x.Key == convertedSku.ColDetail2.Value);
                    if (colorMapping.Value != null)
                    {
                        convertedSku.ColDetail2.Value = colorMapping.Value;
                    }
                }

                convertedList.Add(convertedSku);
            }

            return convertedList;
        }

        private async Task<Dictionary<string, string>> GetColorMappingsAsync(List<string> colorIDs, string platformType)
        {
            if (!colorIDs.Any()) return new Dictionary<string, string>();

            var sql = @"
            SELECT Sort05ID as ColorID, MappingID
            FROM Sort05_Mapping 
            WHERE Sort05ID IN @ColorIDs AND EStoreID = @PlatformType";

            var connection = _context.Connection;
            var results = await connection.QueryAsync(sql,
                new { ColorIDs = colorIDs, PlatformType = platformType });

            return results.ToDictionary(
                x => (string)x.ColorID,
                x => (string)x.MappingID
            );
        }

        private async Task<Dictionary<string, string>> GetSizeMappingsAsync(List<string> sizeIDs, string platformType)
        {
            if (!sizeIDs.Any()) return new Dictionary<string, string>();

            var sql = @"
            SELECT SizeTagID as SizeID, MappingID
            FROM SizeTag_Mapping 
            WHERE SizeTagID IN @SizeIDs AND EStoreID = @PlatformType";

            var connection = _context.Connection;
            var results = await connection.QueryAsync(sql,
                new { SizeIDs = sizeIDs, PlatformType = platformType });

            return results.ToDictionary(
                x => (string)x.SizeID,
                x => (string)x.MappingID
            );
        }

        private SkuItem CloneSku(SkuItem original)
        {
            return new SkuItem
            {
                Path = original.Path,
                ColDetail1 = original.ColDetail1 != null ?
                    new Option<string>(original.ColDetail1.Value, original.ColDetail1.Label) : null,
                ColDetail2 = original.ColDetail2 != null ?
                    new Option<string>(original.ColDetail2.Value, original.ColDetail2.Label) : null,
                Qty = original.Qty,
                OnceQty = original.OnceQty,
                OuterId = original.OuterId,
                SafetyStockQty = original.SafetyStockQty,
                OriginalOuterId = original.OriginalOuterId,
                OriginalQty = original.OriginalQty,
                SuggestPrice = original.SuggestPrice,
                Price = original.Price,
                Cost = original.Cost
            };
        }
        #endregion

        private async Task<ImageUploadInfo> UploadImageFromSourceAsync(string src, string baseName, int index)
        {
            // 將Base64轉換為IFormFile
            var formFile = ImageCompressorExtensions.ConvertBase64ToFormFile(src);

            // 使用你現有的SavePicture方法
            var (result, message) = await SavePicture(formFile, baseName, "moreInfo", index);

            if (!string.IsNullOrEmpty(message))
            {
                throw new Exception($"圖片上傳失敗: {message}");
            }

            return new ImageUploadInfo
            {
                OriginalSrc = src,
                NewSrc = (result as dynamic)?.path
            };
        }

        private async Task<ImageUploadInfo> UploadExternalImageAsync(string imageUrl, string baseName, int index)
        {
            // 下載外部圖片並轉換為IFormFile
            var formFile = await ImageCompressorExtensions.DownloadExternalImageAsFormFileAsync(imageUrl);

            // 使用你現有的SavePicture方法
            var (result, message) = await SavePicture(formFile, baseName, "moreInfo", index);

            if (!string.IsNullOrEmpty(message))
            {
                throw new Exception($"圖片上傳失敗: {message}");
            }

            return new ImageUploadInfo
            {
                OriginalSrc = imageUrl,
                NewSrc = (result as dynamic)?.path
            };
        }

        
    }
}
