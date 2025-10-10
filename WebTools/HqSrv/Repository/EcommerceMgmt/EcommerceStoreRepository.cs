using Dapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods;
using POVWebDomain.Models.DB.POVWeb;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.EcommerceStore;

namespace HqSrv.Repository.EcommerceMgmt
{
    public class EcommerceStoreRepository
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly POVWebDbContextDapper _context;
        public EcommerceStoreRepository(POVWebDbContextDapper context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<(object, string)> GetEStoreTag()
        {
            string sql = @"
                SELECT StoreID, ES.EStoreID, ES.EStoreName + ' ' + CAST(ROW_NUMBER()OVER(Partition By StoreTag Order By StoreID ASC) AS VARCHAR)+'店' AS StoreName, EStoreStyle, OldID AS PlatformID
                FROM EcommerceStore EC
                LEFT JOIN EC_Store ES ON EC.EStoreID = ES.EStoreID
                WHERE ES.EStoreID='0001'
                ORDER BY EC.EStoreID
            ";

            string message = string.Empty;

            using (var connection = _context.Connection)
            {
                connection.Open();
                try
                {
                    var result = await connection.QueryAsync(sql, commandTimeout: 180);


                    return (new { result }, message);
                }
                catch (SqlException e)
                {
                    message = e.Message;
                    return (null, message);
                }
            }
        }

        public async Task<IEnumerable<object>> GetEStoreOptionsAll()
        {

            string query = @"Select EStoreID as Value, EStoreName as Label
                             from EC_Store
                             Order By EStoreID
            ";
            var result = await _context.Connection.QueryAsync(query);

            return result;
        }

        public async Task<(object, string)> GetECStore(GetECStoreRequest request)
        {
            string selectStr = @$"
               SELECT Top 1
                     s.EstoreID, s.IsRestricted_91, s.SoldOutActionType_91, s.Status_91, s.IsShowPurchaseList_91, s.IsShowSoldQty_91, s.IsShowStockQty_91,
                     s.GoodsType_Momo, s.IsECWarehouse_Momo, s.HasAs_Momo, s.IsCommission_Momo, s.IsAcceptTravelCard_Momo, s.OutplaceSeq_Momo, s.OutplaceSeqRtn_Momo, s.IsIncludeInstall_Momo,
                     s.LiveStreamYn_Momo,
                     s.ContentRating_Yahoo, s.ProductWarrantlyPeriod_Yahoo, s.ProductWarrantlyScope_Yahoo, s.ProductWarrantlyHandler_Yahoo, s.ProductWarrantlyDescription_Yahoo,
                     s.IsInstallRequired_Yahoo, s.IsLargeVolumnProductGift_Yahoo, s.IsNeedRecycle_Yahoo, s.IsOutrightPurchase_Yahoo, s.Condition_Shopee, s.DescriptionType_Shopee
                FROM EC_Store s
                WHERE s.EstoreID = @EstoreID
            ";

            string message = string.Empty;
            using (var connection = _context.Connection)
            {
                connection.Open();
                try
                {
                    var result = await connection.QueryFirstOrDefaultAsync(
                        selectStr,
                        request,
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

        public async Task<(int, string)> UpdECStore(UpdECStoreRequest request)
        {
            string updateSQL = @"
                    UPDATE EC_Store
                        SET 
                            IsRestricted_91 = @IsRestricted_91,
                            SoldOutActionType_91 = @SoldOutActionType_91,
                            Status_91 = @Status_91,
                            IsShowPurchaseList_91 = @IsShowPurchaseList_91,
                            IsShowSoldQty_91 = @IsShowSoldQty_91,
                            IsShowStockQty_91 = @IsShowStockQty_91,
                            GoodsType_Momo = @GoodsType_Momo,
                            IsECWarehouse_Momo = @IsECWarehouse_Momo,
                            HasAs_Momo = @HasAs_Momo,
                            IsCommission_Momo = @IsCommission_Momo,
                            IsAcceptTravelCard_Momo = @IsAcceptTravelCard_Momo,
                            OutplaceSeq_Momo = @OutplaceSeq_Momo,
                            OutplaceSeqRtn_Momo = @OutplaceSeqRtn_Momo,
                            IsIncludeInstall_Momo = @IsIncludeInstall_Momo,
                            LiveStreamYn_Momo = @LiveStreamYn_Momo,
                            ContentRating_Yahoo = @ContentRating_Yahoo,
                            ProductWarrantlyPeriod_Yahoo= @ProductWarrantlyPeriod_Yahoo,
                            ProductWarrantlyScope_Yahoo = @ProductWarrantlyScope_Yahoo,
                            ProductWarrantlyHandler_Yahoo = @ProductWarrantlyHandler_Yahoo,
                            ProductWarrantlyDescription_Yahoo = @ProductWarrantlyDescription_Yahoo,
                            IsInstallRequired_Yahoo = @IsInstallRequired_Yahoo, 
                            IsLargeVolumnProductGift_Yahoo = @IsLargeVolumnProductGift_Yahoo,
                            IsNeedRecycle_Yahoo = @IsNeedRecycle_Yahoo, 
                            IsOutrightPurchase_Yahoo = @IsOutrightPurchase_Yahoo,
                            Condition_Shopee = @Condition_Shopee,
                            DescriptionType_Shopee = @DescriptionType_Shopee
                        WHERE EstoreID = @EstoreID
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
                        param: request,
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
