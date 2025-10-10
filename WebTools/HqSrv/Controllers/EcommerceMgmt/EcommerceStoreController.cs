using CityHubCore.Infrastructure.API;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using POVWebDomain.Models.DB.POVWeb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using CityHubCore.Infrastructure.ServiceClient;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.SqlTypes;
using Microsoft.AspNetCore.Mvc.RazorPages;
// using static System.Net.Mime.MediaTypeNames;
using POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.EcommerceStore;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Http;
using HqSrv.Repository.EcommerceMgmt;
using Azure.Core;
using HqSrv.Repository.SettingMgmt;
using POVWebDomain.Models.API.StoreSrv.SettingMgmt.SystemSetting;
using POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods;
using POVWebDomain.Models.ExternalApi.Momo;
using POVWebDomain.Models.ExternalApi.Store91;

namespace HqSrv.Controllers.EcommerceStoreMgmt
{
    public class EcommerceStoreController : ApiControllerBase
    {
        private readonly ILogger<EcommerceStoreController> _logger;
        private readonly POVWebDbContext _POVWebDb;
        private readonly POVWebDbContextDapper _POVWebDbContextDapper;
        private readonly CityAdminSrvClient _CityAdminSrvClient;
        private readonly IConfiguration _Config;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly EcommerceStoreRepository _ecommerceStoreRepository;

        public EcommerceStoreController(
            ILogger<EcommerceStoreController> logger
            , POVWebDbContext pOVWebDbContext
            , POVWebDbContextDapper pOVWebDbContextDapper
            , CityAdminSrvClient cityAdminSrvClient
            , IConfiguration config
            , IWebHostEnvironment hostEnvironment
            , EcommerceStoreRepository ecommerceStoreRepository
            )
        {
            _logger = logger;
            _POVWebDb = pOVWebDbContext;
            _POVWebDbContextDapper = pOVWebDbContextDapper;
            _CityAdminSrvClient = cityAdminSrvClient;
            _Config = config;
            _hostEnvironment = hostEnvironment;
            _ecommerceStoreRepository = ecommerceStoreRepository;
        }

        [HttpGet("V1/GetEcommerceStoreData")]
        public async Task<ActionResult> GetEcommerceStoreData()
       {

            string selectStr1 =
                @$"
                    SELECT EStoreID, EStoreName, EStoreStyle, Active
                    FROM EC_Store
                    WHERE Active != 0
                ";

            string selectStr2 =
                @$"
                    SELECT StoreID, StoreTag, StoreNumber, ApiKey, HostAddress, StoreAlias, StoreName, ShippingWareHouse, EBackStage, CustomerServiceMail, CustomerServicePhone, EOfficialAddress
                           , SenderName, SenderPhone, ShippingAddress, ZipID01, DiscountActivity, HongLi, DiscountCoupon, ShippingCost, AddDetail, SevenCode, PrintImage, EStoreID, TaxCategory
                    FROM EcommerceStore E
                    WHERE E.StoreTag IN @StoreTag

                    SELECT EStoreID, EStoreName
                    FROM EC_Store
                ";
            
            try
            {
                var storeImage = await _POVWebDbContextDapper.Connection.QueryAsync(selectStr1, commandTimeout: 180);
                /*var EStoreData = storeImage.Select(x => new { x.EStoreImage, x.EStoreName }).ToList();
                foreach(var store in EStoreData)
                {
                    await SaveImage(store.EStoreImage, store.EStoreName);
                }*/
                
                var Tag = storeImage.Select(t => t.EStoreName).ToArray();
                
                var storeData = await _POVWebDbContextDapper.Connection.QueryAsync(selectStr2, new {StoreTag = Tag}, commandTimeout: 180);
                

                return Ok(FormatResultModel<dynamic>.Success(new { storeData }));
            }
            catch (Exception ex)
            {
                return Ok(FormatResultModel<dynamic>.Failure(new { MSG = ex.Message }));
            }
        }

        [HttpPost("V1/GetSingleData")]
        public async Task<ActionResult> GetSingleData(GetSingleDataRequest request)
        {
            string selectStr =
                @$"
                    SELECT StoreID, StoreTag, StoreNumber, ApiKey, HostAddress, StoreAlias, StoreName, ShippingWareHouse, EBackStage, CustomerServiceMail, CustomerServicePhone, EOfficialAddress
                           , SenderName, SenderPhone, ShippingAddress, ZipID01, DiscountActivity, HongLi, DiscountCoupon, ShippingCost, AddDetail, SevenCode, PrintImage, EStoreID, TaxCategory
                    FROM EcommerceStore E
                    WHERE E.StoreTag=@StoreTag
                          AND E.StoreID = @StoreID
                ";
            try
            {
                var result = await _POVWebDbContextDapper.Connection.QueryAsync(selectStr, request, commandTimeout: 180);

                return Ok(FormatResultModel<dynamic>.Success(new { result }));
            }
            catch (Exception ex)
            {
                return Ok(FormatResultModel<dynamic>.Failure(new { MSG = ex.Message }));
            }
        }

        [HttpPost("V1/AddEcommerceStoreData")]
        public async Task<ActionResult> AddEcommerceStoreData(AddEcommerceStoreDataRequest request)
        {
            if (request.CustomerServiceMail != "" && request.CustomerServiceMail != null)
            {
                if (request.CustomerServiceMail.Length > 0)
                {
                    string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                    if (!Regex.IsMatch(request.CustomerServiceMail, pattern))
                    {
                        return Ok(FormatResultModel<dynamic>.Failure(new { MSG = "請輸入正確的電子信箱" }));
                    }
                }
            }
            string addSQL = @$"
                Declare @O_MSG VARCHAR(MAX)=N''
                DECLARE @StoreID VARCHAR(10) = N'', @NewID VARCHAR(10)
                SET @NewID = (SELECT MAX(StoreID) FROM EcommerceStore WHERE StoreID LIKE @ShippingWarehouse + '%')
                IF ISNULL(@NewID, '') = ''
                BEGIN
                    SET @NewID = @ShippingWarehouse+'000'
                END
                SET @NewID= CONVERT(INT, SUBSTRING(@NewID, LEN(@ShippingWarehouse)+1, LEN(@NewID))) + 1
                SET @NewID = @ShippingWarehouse + REPLACE(STR(@NewID, 6), ' ', 0)
                SET @StoreID = @NewID

                IF  EXISTS(SELECT 1 FROM EcommerceStore WHERE StoreNumber=@StoreNumber) BEGIN
	                SET @O_MSG = '商店編號已存在!'
	                ;THROW 6636001, @O_MSG, 1;
                END 

                IF  EXISTS(SELECT 1 FROM EcommerceStore WHERE ApiKey=@ApiKey) BEGIN
	                SET @O_MSG = 'APIKey 已存在!'
	                ;THROW 6636001, @O_MSG, 1;
                END 

	            BEGIN TRY
	                INSERT INTO EcommerceStore
                           ([StoreID], [StoreTag], [StoreNumber], [ApiKey], [HostAddress], [StoreAlias], [StoreName]
                           , [ShippingWareHouse], [SellBranch], [EBackStage], [CustomerServiceMail], [CustomerServicePhone], [EOfficialAddress]
                           , [SenderName], [SenderPhone], [ShippingAddress], [ZipID01], [DiscountActivity], [HongLi]
                           , [DiscountCoupon], [ShippingCost], [AddDetail], [SevenCode], [PrintImage], [EStoreID], [TaxCategory]
                           , [ChangePerson], [ChangeDate], [DeliveryWay], [OldID], [isEcApiEnabled])
                     VALUES
			                (@StoreID, @StoreTag, @StoreNumber, @ApiKey, @HostAddress, @StoreAlias, @StoreName, @ShippingWareHouse, @SellBranch, @EBackStage
                            , @CustomerServiceMail, @CustomerServicePhone, @EOfficialAddress, @SenderName, @SenderPhone, @ShippingAddress
                            , @ZipID01, @DiscountActivity, @HongLi, @DiscountCoupon, @ShippingCost, @AddDetail, @SevenCode, @PrintImage, @EStoreID
                            , @TaxCategory, @ChangePerson, @ChangeDate, @DeliveryWay, @OldID, @IsEcApiEnabled)
	                END TRY
	            BEGIN CATCH
		            ;THROW
	            END CATCH

            ";
            using (var connection = _POVWebDbContextDapper.Connection)
            {
                connection.Open();
                using var transaction = connection.BeginTransaction();
                try
                {
                    var result = await connection.ExecuteScalarAsync<string>(
                            sql: addSQL,
                            request,
                            transaction: transaction,
                            commandTimeout: 180);
                    transaction.Commit();

                    return Ok(FormatResultModel<dynamic>.Success(new { result }));
                }
                catch (SqlException e)
                {
                    return Ok(FormatResultModel<dynamic>.Failure(new { MSG = e.Message }));
                }
            }
        }

        [HttpPost("V1/UpdEcommerceStore")]
        public async Task<ActionResult> UpdEcommerceStore(UpdEcommerceStoreDataRequest request)
        {
            if (request.CustomerServiceMail != "" && request.CustomerServiceMail != null)
            {
                if (request.CustomerServiceMail.Length > 0)
                {
                    string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                    if (!Regex.IsMatch(request.CustomerServiceMail, pattern))
                    {
                        return Ok(FormatResultModel<dynamic>.Failure(new { MSG = "請輸入正確的電子信箱" }));
                    }
                }
            }
            string updSQL = @$"
                    IF EXISTS(
                        SELECT 1 FROM EcommerceStore E
                        WHERE E.StoreNumber=@StoreNumber
                              AND E.StoreTag = @StoreTag
                    ) BEGIN                       
                        UPDATE EcommerceStore
                        SET ApiKey=@ApiKey, HostAddress=@HostAddress, StoreAlias=@StoreAlias, StoreName=@StoreName, ShippingWareHouse=@ShippingWareHouse, SellBranch=@SellBranch, EBackStage=@EBackStage
                            , CustomerServiceMail=@CustomerServiceMail, CustomerServicePhone=@CustomerServicePhone, EOfficialAddress=@EOfficialAddress, SenderName=@SenderName
                            , SenderPhone=@SenderPhone, ShippingAddress=@ShippingAddress, ZipID01=@ZipID01, DiscountActivity=@DiscountActivity, HongLi=@HongLi
                            , DiscountCoupon=@DiscountCoupon, ShippingCost=@ShippingCost, AddDetail=@AddDetail, SevenCode=@SevenCode, PrintImage=@PrintImage
                            , TaxCategory=@TaxCategory, ChangeDate=@ChangeDate, ChangePerson=@ChangePerson, DeliveryWay=@DeliveryWay, IsEcApiEnabled=@IsEcApiEnabled
                        WHERE StoreNumber=@OriginalStoreID
                              AND StoreTag=@StoreTag
                    END
                ";
            try
            {
                var result = await _POVWebDbContextDapper.Connection.ExecuteAsync(updSQL, request);

                return Ok(FormatResultModel<dynamic>.Success(new { result }));
            }
            catch (Exception ex)
            {
                return Ok(FormatResultModel<dynamic>.Failure(new { MSG = ex.Message }));
            }
        }

        [HttpPost("V1/DelEcommerceStoreData")]
        public async Task<ActionResult> DelEcommerceStoreData(DelEcommerceStoreDataRequest request)
        {

            string actionStr =
                @$" Declare @O_MSG VARCHAR(MAX)=N''
                    BEGIN TRY
		                DELETE FROM EcommerceStore 
                        WHERE StoreTag = @StoreTag
                              AND StoreNumber = @StoreNumber
                              AND StoreName = @StoreName
	                END TRY
	                BEGIN CATCH
		                SET @O_MSG = '刪除資料失敗。' + ERROR_MESSAGE() 
		                ;THROW 6636001, @O_MSG, 1;
	                END CATCH	
                ";
            try
            {
                var result = await _POVWebDbContextDapper.Connection.ExecuteAsync(actionStr, request);

                return Ok(FormatResultModel<dynamic>.Success(new { result }));
            }
            catch (Exception ex)
            {
                return Ok(FormatResultModel<dynamic>.Failure(new { MSG = ex.Message }));
            }
        }

        [HttpPost("V1/GetEcommerceStoreDetailByID")]
        public async Task<ActionResult> GetEcommerceStoreDetailByID(GetIDRequest request)
        {

            string actionStr =
                @$" Select * From EcommerceStore
                    Where StoreNumber = @storeID
                ";
            try
            {
                var result = await _POVWebDbContextDapper.Connection.QueryAsync(actionStr, request);

                return Ok(FormatResultModel<dynamic>.Success(new { result }));
            }
            catch (Exception ex)
            {
                return Ok(FormatResultModel<dynamic>.Failure(new { MSG = ex.Message }));
            }
        }

        [HttpGet("V1/GetEStoreImage")]
        public async Task<ActionResult> GetEStoreImage()
        {
            string imgSrc;
            if (_hostEnvironment.IsDevelopment())
            {
                imgSrc = string.Format("CASE WHEN EStoreSrc != '' THEN '{0}://{1}{2}' + EStoreSrc ELSE '' END AS EStoreSrc", Request.Scheme, Request.Host, Request.PathBase);
            }
            else
            {
                imgSrc = string.Format("CASE WHEN EStoreSrc != '' THEN '' + EStoreSrc ELSE '' END AS EStoreSrc");
            }

            var getStr =
                $@"
                    SELECT EStoreID, EStoreName, EStoreImage
                    FROM EC_Store
                    WHERE Active != 0
                ";
            var addPathStr =
                $@"
                    UPDATE EC_Store
	                    SET EStoreSrc = @EStoreSrc
                    WHERE EStoreID = @StoreID
                        AND EStoreName = @StoreName 
                ";
            var resultStr =
                $@"
                    SELECT EStoreID, EStoreName, {imgSrc}, EStoreStyle
                    FROM EC_Store
                    WHERE Active != 0
                ";
            try
            {
                var getData = await _POVWebDbContextDapper.Connection.QueryAsync(getStr, commandTimeout: 180);
                var image = getData.Select(x => new { x.EStoreID, x.EStoreImage, x.EStoreName }).ToList();
                foreach(var data in image)
                {
                    var imagePath = SaveImage(data.EStoreImage, data.EStoreName);

                    await _POVWebDbContextDapper.Connection.QueryAsync(addPathStr, new { StoreID = data.EStoreID, StoreName = data.EStoreName, EStoreSrc = imagePath.Result }, commandTimeout: 180);
                }
                var result = await _POVWebDbContextDapper.Connection.QueryAsync(resultStr, commandTimeout: 180);

                return Ok(FormatResultModel<dynamic>.Success(new { result }));
            }
            catch (Exception ex)
            {
                return Ok(FormatResultModel<dynamic>.Failure(new { MSG = ex.Message }));
            }
        }

        [HttpGet("V1/GetStoreOption")]
        public async Task<ActionResult> GetStoreOption()
        {
            string getStr = $@"
                Select EStoreID, EStoreName From EC_Store
            ";
            using (var connection = _POVWebDbContextDapper.Connection)
            {
                connection.Open();
                try
                {
                    var result = await connection.QueryAsync(getStr, commandTimeout: 180);

                    return Ok(FormatResultModel<dynamic>.Success(new { result }));
                }
                catch (SqlException e)
                {
                    return Ok(FormatResultModel<dynamic>.Failure(new { MSG = e.Message }));
                }
            }
        }

        [HttpGet("V1/GetEStoreTag")]
        public async Task<ActionResult> GetEStoreTag()
        {
            try
            {
                var result = await _ecommerceStoreRepository.GetEStoreTag();
                if (string.IsNullOrEmpty(result.Item2))
                {
                    return Ok(FormatResultModel<dynamic>.Success(result.Item1));
                }
                else
                {
                    return Ok(FormatResultModel<dynamic>.Failure(new { MSG = result.Item2 }));
                }
            }
            catch (Exception e)
            {
                return Ok(FormatResultModel<dynamic>.Failure(new { MSG = e.Message }));
            }
        }

        [HttpGet("V1/GetEStoreOptionsAll")]
        public async Task<ActionResult> GetEStoreOptionsAll()
        {
            try
            {
                var data = await _ecommerceStoreRepository.GetEStoreOptionsAll();

                // Result
                return Ok(FormatResultModel<dynamic>.Success(data));
            }
            catch (Exception ex)
            {
                return Ok(FormatResultModel<dynamic>.Success(new { ex.Message }));
            }

        }


        [NonAction]
        public async Task<string> SaveImage(string EStoreImage, string EStoreName)
        {
            if (EStoreImage != null)
            {
                try
                {
                    string directoryPath = Path.Combine(_hostEnvironment.ContentRootPath, "Images", "EStore");
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory("Images/EStore");
                    }

                    int commaIndex = EStoreImage.IndexOf(',');
                    if (commaIndex == -1)
                    {
                        Console.WriteLine("無效的 Data URI 格式。");
                    }

                    string base64Data = EStoreImage.Substring(commaIndex + 1);
                    byte[] bytes = Convert.FromBase64String(base64Data);
                    var imagePath = Path.Combine(_hostEnvironment.ContentRootPath, "Images/EStore", EStoreName);

                    if (!imagePath.ToLower().EndsWith(".png"))
                    {
                        imagePath += ".png";
                    }

                    Image image;
                    using (MemoryStream ms = new MemoryStream(bytes))
                    {
                        image = Image.FromStream(ms);
                        image.Save(imagePath, System.Drawing.Imaging.ImageFormat.Png);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return "/Images/EStore/" + EStoreName + ".png";
        }

        [HttpPost("V1/GetECStore")]
        public async Task<ActionResult> GetECStore(GetECStoreRequest request)
        {
            try
            {
                var result = await _ecommerceStoreRepository.GetECStore(request);
                if (string.IsNullOrEmpty(result.Item2))
                {
                    return Ok(FormatResultModel<dynamic>.Success(result.Item1));
                }
                else
                {
                    return Ok(FormatResultModel<dynamic>.Failure(new { MSG = result.Item2 }));
                }
            }
            catch (Exception e)
            {
                return Ok(FormatResultModel<dynamic>.Failure(new { MSG = e.Message }));
            }

        }

        [HttpPost("V1/UpdECStore")]
        public async Task<ActionResult> UpdECStore(UpdECStoreRequest request)
        {
            try
            {
                var result = await _ecommerceStoreRepository.UpdECStore(request);
                if (string.IsNullOrEmpty(result.Item2))
                {
                    return Ok(FormatResultModel<dynamic>.Success(result.Item1));
                }
                else
                {
                    return Ok(FormatResultModel<dynamic>.Failure(new { MSG = result.Item2 }));
                }
            }
            catch (Exception e)
            {
                return Ok(FormatResultModel<dynamic>.Failure(new { MSG = e.Message }));
            }

        }

    }
}
