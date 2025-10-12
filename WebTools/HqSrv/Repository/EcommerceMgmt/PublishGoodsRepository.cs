using Dapper;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using POVWebDomain.Common;
using POVWebDomain.Models.API.StoreSrv.Common;
using POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.Common;
using POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods;
using POVWebDomain.Models.DB.POVWeb;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using IndexList = POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods.IndexList;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace HqSrv.Repository.EcommerceMgmt
{
    public class PublishGoodsRepository : IPublishGoodsRepository
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly POVWebDbContextDapper _context;
        private readonly IConfiguration _configuration;

        public PublishGoodsRepository(
            POVWebDbContextDapper context,
            IWebHostEnvironment hostEnvironment,
            IConfiguration configuration)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _configuration = configuration;
        }

        #region 查詢方法 (Result Pattern)

        public async Task<Result<object>> GetSubmitModeAsync(GetSubmitModeRequest request)
        {
            string searchSql = @"
                DECLARE @ParentID VARCHAR(20) = (SELECT TOP 1 ParentID FROM Goods WHERE GoodID=@GoodID)
                IF EXISTS(SELECT 1 FROM ESubmitGoodsReq WHERE ParentID=@ParentID)
                    SELECT 'edit' AS Mode, @ParentID AS ParentID
                ELSE 
                    SELECT 'add' AS Mode, @ParentID AS ParentID
            ";

            using (var connection = _context.Connection)
            {
                connection.Open();
                try
                {
                    var result = connection.QueryFirstOrDefault(searchSql, request);
                    return Result<object>.Success(new { result });
                }
                catch (SqlException e)
                {
                    return Result<object>.Failure(Error.Custom("DB_ERROR", e.Message));
                }
            }
        }

        public async Task<Result<object>> GetSubmitDefValAsync(GetSubmitDefValRequest request)
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
                        return Result<object>.Failure("沒有款式編號!");
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

                    return Result<object>.Success(new { defVal, submitHistory, missingItems, optionList });
                }
                catch (SqlException e)
                {
                    return Result<object>.Failure(Error.Custom("DB_ERROR", e.Message));
                }
            }
        }

        public async Task<Result<object>> GetEStoreCatOptionsAsync()
        {
            string searchSql = @"
                SELECT EStoreID, CategoryID, CategoryName
                FROM EstoreCat
                WHERE Status = 1
                ORDER BY EStoreID, CategoryID
            ";

            using (var connection = _context.Connection)
            {
                connection.Open();
                try
                {
                    List<GetEStoreCatResponse> response = (await connection.QueryAsync<GetEStoreCatResponse>(searchSql)).ToList();

                    var result = response.GroupBy(r => new { r.EStoreID })
                        .Select(r => new
                        {
                            r.Key.EStoreID,
                            Option = r.Select(opt => new
                            {
                                Value = opt.CategoryID,
                                Label = opt.CategoryName
                            })
                        });

                    return Result<object>.Success(new { result });
                }
                catch (SqlException e)
                {
                    return Result<object>.Failure(Error.Custom("DB_ERROR", e.Message));
                }
            }
        }

        #endregion

        #region 查詢方法 (保持原樣)

        public async Task<GetSubmitModeReponse> GetSubmitResByStoreAsync(string parentID, string platformID)
        {
            string searchSql = @"
                SELECT ResponseData, RequestParams 
                FROM ESubmitGoodsRes WHERE ParentID=@ParentID AND StoreID=@platformID
            ";

            var connection = _context.Connection;
            connection.Open();
            try
            {
                var resData = connection.QueryFirstOrDefault<GetSubmitModeReponse>(searchSql, new { parentID, platformID });
                return resData;
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task<GetLookupAndCommonValueResponse> GetLookupAndCommonValueAsync(string parentID, string storeID)
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
                LEFT JOIN EstoreValue E ON G.ParentID = E.ParentID
                WHERE G.ParentID = @ParentID
            ";

            var connection = _context.Connection;
            connection.Open();
            try
            {
                var resData = await connection.QueryFirstOrDefaultAsync<GetLookupAndCommonValueResponse>(
                    getLookupSql,
                    new { parentID, EstoreID = storeID });
                return resData;
            }
            catch (SqlException e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task<GetEcIndexReturn> MergeEcAttributesAsync(List<string> platforms, string categoryCode = null)
        {
            // 1. 收集所有平台屬性
            var allEcIndexes = new List<EcIndex>();

            // 2. 查詢對應關係
            var mappings = await GetAttributeMappingsAsync(platforms);

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
                Options = new List<EcIndex>(),
                IndexList = new List<IndexList>()
            };
        }

        #endregion

        #region 儲存方法 (Result Pattern)

        public async Task<Result<object>> SaveSubmitGoodsReqAsync(SubmitMainRequestAll request)
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

            var connection = _context.Connection;
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                int effectRows = await connection.ExecuteAsync(
                    saveReqSql,
                    new { request.ParentID, RequestParams = request.JsonData, request.ChangePerson },
                    transaction: transaction,
                    commandTimeout: 180);

                transaction.Commit();
                return Result<object>.Success(new { effectRows });
            }
            catch (SqlException e)
            {
                return Result<object>.Failure(Error.Custom("DB_ERROR", e.Message));
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task<Result<object>> SaveSubmitGoodsResAsync(
            SubmitMainRequestAll request,
            object requestDto,
            SubmitMainResponseAll response,
            StoreSetting store)
        {
            JObject objJson = JObject.Parse(request.JsonData);
            JObject obj = JObject.Parse(request.BasicInfo);
            obj["categoryId"] = store.CategoryId;
            obj["title"] = store.Title;
            obj["cost"] = store.Cost;
            obj["moreInfo"] = objJson["moreInfo"];

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

            var connection = _context.Connection;
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                int effectRows = await connection.ExecuteAsync(
                    saveResSql,
                    new
                    {
                        storeID = store.PlatformID,
                        request.ParentID,
                        ResponseData = JsonConvert.SerializeObject(response.Response),
                        request.ChangePerson,
                        RequestParams = JsonSerializer.Serialize(requestDto)
                    },
                    transaction: transaction,
                    commandTimeout: 180);

                transaction.Commit();
                return Result<object>.Success(new { effectRows });
            }
            catch (SqlException e)
            {
                return Result<object>.Failure(Error.Custom("DB_ERROR", e.Message));
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task<Result<object>> SavePictureAsync(IFormFile file, string baseName, string type, int? index = -1)
        {
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

                    return Result<object>.Success(new { path = relativePath });
                }
                catch (Exception ex)
                {
                    return Result<object>.Failure(Error.Custom("FILE_SAVE_ERROR", ex.Message));
                }
            }

            return Result<object>.Success(new { path = relativePath });
        }

        #endregion

        #region 處理方法 (保持原樣)

        public async Task HandleImageAsync(SubmitMainRequestAll request)
        {
            JObject obj = JObject.Parse(request.JsonData);

            for (int idx = 0; idx < request.MainImage?.Count; idx++)
            {
                IFormFile image = request.MainImage[idx];
                if (image.FileName == "blob") continue;

                var result = await SavePictureAsync(image, request.ParentID, "mainImage", idx);
                if (result.IsFailure) throw new Exception(result.Error.Message);

                if (idx == 0)
                {
                    var adResult = await SavePictureAsync(image, request.ParentID, "ad", idx);
                    if (adResult.IsFailure) throw new Exception(adResult.Error.Message);
                }

                string path = ((dynamic)result.Data).path;
                obj["mainImage"][idx]["path"] = path == "" ? image.FileName : path;
            }

            for (int idx = 0; idx < request.SkuImage?.Count; idx++)
            {
                IFormFile image = request.SkuImage[idx];
                if (image.FileName == "blob") continue;

                var result = await SavePictureAsync(image, request.ParentID, "skuImage", idx);
                if (result.IsFailure) throw new Exception(result.Error.Message);

                string path = ((dynamic)result.Data).path;
                obj["skuList"][idx]["image"]["path"] = path == "" ? image.FileName : path;
            }

            if (request.SizeImage != null)
            {
                IFormFile image = request.SizeImage;
                var result = await SavePictureAsync(image, request.ParentID, "sizeImage", 0);
                if (result.IsFailure) throw new Exception(result.Error.Message);

                string path = ((dynamic)result.Data).path;
                if (image.FileName != "blob")
                {
                    obj["sizeImage"]["path"] = path == "" ? image.FileName : path;
                }
            }

            MoreInfoResult moreInfoResult = await ProcessMoreInfoAsync((string)obj["moreInfo"], request.ParentID, request.Origin);
            obj["moreInfo"] = moreInfoResult.ProcessedHtml;
            request.JsonData = obj.ToString(Formatting.None);
        }

        public async Task<MoreInfoResult> ProcessMoreInfoAsync(string originalHtml, string baseName, string origin)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(originalHtml);

            var imgNodes = doc.DocumentNode.SelectNodes("//img") ?? Enumerable.Empty<HtmlNode>();

            int imgIndex = 0;
            var uploadedImages = new List<ImageUploadInfo>();

            foreach (var imgNode in imgNodes)
            {
                var src = imgNode.GetAttributeValue("src", "");

                if (ImageCompressorExtensions.IsBase64OrBlobImage(src))
                {
                    var uploadResult = await UploadImageFromSourceAsync(src, baseName, imgIndex);
                    imgNode.SetAttributeValue("src", $"{origin}/{uploadResult.NewSrc}");
                    uploadedImages.Add(uploadResult);
                    imgIndex++;
                }
                else if (ImageCompressorExtensions.IsExternalImage(src))
                {
                    var uploadResult = await UploadExternalImageAsync(src, baseName, imgIndex);
                    imgNode.SetAttributeValue("src", $"{origin}/{uploadResult.NewSrc}");
                    uploadedImages.Add(uploadResult);
                    imgIndex++;
                }
            }

            var processedHtml = doc.DocumentNode.OuterHtml;

            return new MoreInfoResult
            {
                ProcessedHtml = processedHtml,
                UploadedImages = uploadedImages
            };
        }

        #endregion

        #region 私有輔助方法

        private void EnsureDirectoryExists(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        private async Task<ImageUploadInfo> UploadImageFromSourceAsync(string src, string baseName, int index)
        {
            var formFile = ImageCompressorExtensions.ConvertBase64ToFormFile(src);
            var result = await SavePictureAsync(formFile, baseName, "moreInfo", index);

            if (result.IsFailure)
            {
                throw new Exception($"圖片上傳失敗: {result.Error.Message}");
            }

            return new ImageUploadInfo
            {
                OriginalSrc = src,
                NewSrc = (result.Data as dynamic)?.path
            };
        }

        private async Task<ImageUploadInfo> UploadExternalImageAsync(string imageUrl, string baseName, int index)
        {
            var formFile = await ImageCompressorExtensions.DownloadExternalImageAsFormFileAsync(imageUrl);
            var result = await SavePictureAsync(formFile, baseName, "moreInfo", index);

            if (result.IsFailure)
            {
                throw new Exception($"圖片上傳失敗: {result.Error.Message}");
            }

            return new ImageUploadInfo
            {
                OriginalSrc = imageUrl,
                NewSrc = (result.Data as dynamic)?.path
            };
        }

        private List<OptionItem> CreateOptionList(List<SkuItem_Goods> goodsList)
        {
            var optionList = new List<OptionItem>();

            // 處理尺寸選項
            var uniqueSizes = goodsList
                .Select(x => (x.SizeID, x.SizeName))
                .Distinct()
                .Where(x => !string.IsNullOrEmpty(x.SizeID))
                .Select(x => new Option<string>(x.SizeID, x.SizeName))
                .ToList();

            if (uniqueSizes.Any())
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

            // 處理顏色選項
            var uniqueColors = goodsList
                .Select(x => (x.ColorID, x.ColorName))
                .Distinct()
                .Where(x => !string.IsNullOrEmpty(x.ColorID))
                .Select(x => new Option<string>(x.ColorID, x.ColorName))
                .ToList();

            if (uniqueColors.Any())
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

            return optionList;
        }

        private List<SkuItem> CreateMissingItems(List<SkuItem> skuItems, List<SkuItem_Goods> goodsList)
        {
            // 保持原有實作
            var existingOuterIds = skuItems?.Select(s => s.OuterId).ToHashSet() ?? new HashSet<string>();
            var uniqueSizes = goodsList.Select(g => new { g.SizeID, g.SizeName }).Distinct().ToList();
            var uniqueColors = goodsList.Select(g => new { g.ColorID, g.ColorName }).Distinct().ToList();

            bool hasSize = skuItems?.Any(s => s.ColDetail1 != null) ?? false;
            bool hasColor = skuItems?.Any(s => s.ColDetail2 != null) ?? false;

            var missingItems = goodsList
                .Where(goods => !existingOuterIds.Contains(goods.GoodID))
                .Select(goods => new SkuItem
                {
                    OuterId = goods.GoodID,
                    Qty = 0,
                    OnceQty = 1,
                    SafetyStockQty = 0,
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
                        item.ColDetail1 = new Option<string>(targetGood.SizeID, targetGood.SizeName);
                    if (hasColor)
                        item.ColDetail2 = new Option<string>(targetGood.ColorID, targetGood.ColorName);
                }
                else
                {
                    if (uniqueSizes.Count > 0)
                        item.ColDetail1 = new Option<string>(targetGood.SizeID, targetGood.SizeName);
                    if (uniqueColors.Count > 0)
                        item.ColDetail2 = new Option<string>(targetGood.ColorID, targetGood.ColorName);
                }
            }

            return missingItems;
        }

        private async Task<List<EC_AttributeMapping>> GetAttributeMappingsAsync(List<string> platforms)
        {
            // 保持原有實作
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
    }
}
