using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Configuration;
using POVWebDomain.Models.API.StoreSrv.SettingMgmt.SystemSetting;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration.Json;
using System.Linq;
using Microsoft.Data.SqlClient;
using System.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;
using POVWebDomain.Models.DB.POVWeb;
using Azure.Core;

namespace HqSrv.Repository.SettingMgmt
{
    public class SystemSettingsRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;
        private readonly IWebHostEnvironment _environment;
        private const string CACHE_KEY = "SystemSettings";
        private readonly POVWebDbContextDapper _context;

        public SystemSettingsRepository(IConfiguration configuration, IMemoryCache cache, IWebHostEnvironment environment, POVWebDbContextDapper context)
        {
            _configuration = configuration;
            _cache = cache;
            _environment = environment;
            _context = context;
        }

        public async Task<SystemSettings> GetSettingsAsync(GetSettingsRequest request)
        {
            //if (_cache.TryGetValue(CACHE_KEY, out SystemSettings cachedSettings))
            //{
            //    return cachedSettings;
            //}

            string SQL = @"
                SELECT s.EnableForeignCurrency, s.DecimalPlaces, s.DualScreenDisplay, s.SaleMemberLoadOption, s.LocalStoreOnly, s.AllowPriceEdit, s.StockCheck
                FROM SystemSettings s

                SELECT CostPermission FROM Users WHERE UserID = @UserID
            ";
            string connectionString = _configuration["ConnectionStrings:POVWebDb"];
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                try
                {
                    var result = await connection.QueryMultipleAsync(
                        sql: SQL,
                        request,
                        commandTimeout: 180);

                    var settings = result.IsConsumed ? null : result.Read<SystemSettings>().ToList().FirstOrDefault();

                    var costPermission = result.IsConsumed ? false : result.Read<bool>().ToList().FirstOrDefault();

                    settings.CostPermission = costPermission;

                    var cacheOptions = new MemoryCacheEntryOptions()
                          .SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
                                _cache.Set(CACHE_KEY, settings, cacheOptions);


                    return settings;
                }
                catch (SqlException e)
                {
                    return new SystemSettings();
                }
            }
        }

        public async Task<(object, string)> GetSingleData()
        {
            string selectStr = @$"
               SELECT Top 1
                     ISNULL(s.DualScreenDisplay, 'image') as DualScreenDisplay, s.SaleMemberLoadOption, s.PointsPerDollar, s.MinimumSpend, s.MaxRedemptionPercentage, s.RefundRestoreUsedPoints, s.CountPackagingMaterial, 
                     s.EnableForeignCurrency, s.LocalStoreOnly
                FROM 
                    SystemSettings s
            ";

            string message = string.Empty;
            using (var connection = _context.Connection)
            {
                connection.Open();
                try
                {
                    var result = await connection.QueryFirstOrDefaultAsync(
                        selectStr,
                        commandTimeout: 180);

                    return (result, message);
                }
                catch (SqlException e)
                {
                    message = e.Message;
                    return (null, message);
                }
            }
        }

        public async Task<(int, string)> UpdateSettingsAsync(SystemSettingsEditable settings)
        {
            string updateSQL = @"
                    DECLARE @AdminAuth BIT = [POVWeb].[udfHadAuthority] ('系統設定-旌泓', @UserID, 'update')
                    DECLARE @Auth BIT = [POVWeb].[udfHadAuthority] ('系統設定', @UserID, 'update')

                    IF @Auth = 1
                    BEGIN
                        IF EXISTS (SELECT 1 FROM SystemSettings)
                        BEGIN
                            UPDATE SystemSettings
                            SET 
                                DualScreenDisplay = @DualScreenDisplay,
                                SaleMemberLoadOption = @SaleMemberLoadOption,
                                PointsPerDollar = @PointsPerDollar,
                                MinimumSpend = @MinimumSpend,
                                MaxRedemptionPercentage = @MaxRedemptionPercentage,
                                RefundRestoreUsedPoints = @RefundRestoreUsedPoints,
                                ChangeDate = CONVERT(varchar(8), GETDATE(), 112),
                                CountPackagingMaterial = CASE @AdminAuth WHEN 1 THEN @CountPackagingMaterial ELSE CountPackagingMaterial END,
                                EnableForeignCurrency = CASE @AdminAuth WHEN 1 THEN @EnableForeignCurrency ELSE EnableForeignCurrency END,
                                LocalStoreOnly = CASE @AdminAuth WHEN 1 THEN @LocalStoreOnly ELSE LocalStoreOnly END
                        END
                        ELSE
                        BEGIN
                            INSERT INTO SystemSettings (
                                PointsPerDollar, MinimumSpend, MaxRedemptionPercentage, CreateDate, ChangeDate, RefundRestoreUsedPoints, RefundDeductEarnedPoints,
                                CountGeneralProduct, CountGiftCertificate, CountPoints, CountStoredValue, CountPackagingMaterial, CountDiscountCoupon,
                                EnableForeignCurrency, DecimalPlaces, DualScreenDisplay, SaleMemberLoadOption, 
                                LocalStoreOnly
                            ) VALUES (
                                @PointsPerDollar, @MinimumSpend, @MaxRedemptionPercentage, CONVERT(varchar(8), GETDATE(), 112), CONVERT(varchar(8), GETDATE(), 112), @RefundRestoreUsedPoints, 1,
                                1, 0, 0, 0, CASE @AdminAuth WHEN 1 THEN @CountPackagingMaterial ELSE 1 END, 0,
                                CASE @AdminAuth WHEN 1 THEN @EnableForeignCurrency ELSE 0 END, 2, @DualScreenDisplay, @SaleMemberLoadOption,
                                CASE @AdminAuth WHEN 1 THEN @LocalStoreOnly ELSE 0 END
                            )
                        END
                    END
 
                ";

            string message = string.Empty;
            int effectRows = 0;
            using (var connection = _context.Connection)
            {
                connection.Open();
                using var transaction = connection.BeginTransaction();
                try
                {
                    effectRows = await connection.ExecuteAsync(
                        sql: updateSQL,
                        param: settings,
                        transaction: transaction,
                        commandTimeout: 180,
                        commandType: CommandType.Text);

                    transaction.Commit();
                }
                catch (SqlException e)
                {
                    message = e.Message;
                }
            }
            return (effectRows, message);
          
        }
    }

}