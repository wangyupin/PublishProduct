using CityHubCore.Infrastructure.API;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using POVWebDomain.Models.DB.POVWeb;
using POVWebDomain.Models.API.StoreSrv.DashBoard.Announcement;
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
using static System.Net.Mime.MediaTypeNames;
// using HqSrv.Repository.MainTableMgmt;

namespace HqSrv.Controllers.DashBoard
{
    public class AnnouncementController : ApiControllerBase
    {
        private readonly ILogger<AnnouncementController> _logger;
        private readonly POVWebDbContext _POVWebDb;
        private readonly POVWebDbContextDapper _POVWebDbContextDapper;
        private readonly CityAdminSrvClient _CityAdminSrvClient;
        private readonly IConfiguration _Config;
        //private readonly ClientRepository _ClientRepository;

        public AnnouncementController(
            ILogger<AnnouncementController> logger
            , POVWebDbContext pOVWebDbContext
            , POVWebDbContextDapper pOVWebDbContextDapper
            , CityAdminSrvClient cityAdminSrvClient
            , IConfiguration config
            //ClientRepository clientRepository
            )
        {
            _logger = logger;
            _POVWebDb = pOVWebDbContext;
            _POVWebDbContextDapper = pOVWebDbContextDapper;
            _CityAdminSrvClient = cityAdminSrvClient;
            _Config = config;
        }

        [HttpGet("V1/GetAnnouncementData")]
        public async Task<ActionResult> GetClientHelp()
        {

            string selectStr =
            @$"SELECT AnnouncementID, PostingTime, Subject, Views FROM Announcement ORDER BY postingTime DESC";

            try
            {
                var AnnouncementList = await _POVWebDbContextDapper.Connection.QueryAsync(selectStr, commandTimeout: 180);

                return Ok(FormatResultModel<dynamic>.Success(new { AnnouncementList }));
            }
            catch (Exception ex)
            {
                return Ok(FormatResultModel<dynamic>.Failure(new { MSG = ex.Message }));
            }
        }

        [HttpPost("V1/GetAnnouncementAllData")]
        public async Task<ActionResult> GetAnnouncementAllData(GetAnnouncementDataRequest request)
        {
            string advanceRequestStr = "";
            string queryOrder = string.Format("ORDER BY CASE WHEN CheckTop = 'Y' THEN 1 ELSE 0 END DESC");
            string offSetStr = request.Mode == String.Empty ?
                @$"OFFSET (@Page-1)*@PerPage ROWS 
                   FETCH NEXT @PerPage ROWS ONLY" :
                "";

            if (request.AdvanceRequest != null)
            {
                advanceRequestStr = request.AdvanceRequest.GenerateSqlString();
            }
            // t0.[Card91ID]
            string sql = @$"
                SELECT *
                FROM Announcement t0
                WHERE (AnnouncementID LIKE '%'+LOWER(@Q)+'%'
	                OR LOWER(Subject) LIKE '%'+LOWER(@Q)+'%'
                    OR LOWER(PostingTime) LIKE '%'+LOWER(@Q)+'%'
	                OR LOWER(PostingUnit) LIKE '%'+LOWER(@Q)+'%'
	                OR LOWER(PostingPerson) LIKE '%'+LOWER(@Q)+'%'
	                OR LOWER(PostingContent) LIKE '%'+LOWER(@Q)+'%'
                    OR LOWER(CheckTop) LIKE '%'+LOWER(@Q)+'%'
	                OR LOWER(Views) LIKE '%'+LOWER(@Q)+'%')
                {advanceRequestStr}
                {queryOrder}
                {offSetStr}
                ";
            string sqlNum = @$"
                SELECT Count(*)
                FROM Announcement t0
                WHERE (AnnouncementID LIKE '%'+LOWER(@Q)+'%'
	                OR LOWER(Subject) LIKE '%'+LOWER(@Q)+'%'
                    OR LOWER(PostingTime) LIKE '%'+LOWER(@Q)+'%'
	                OR LOWER(PostingUnit) LIKE '%'+LOWER(@Q)+'%'
	                OR LOWER(PostingPerson) LIKE '%'+LOWER(@Q)+'%'
	                OR LOWER(PostingContent) LIKE '%'+LOWER(@Q)+'%'
                    OR LOWER(CheckTop) LIKE '%'+LOWER(@Q)+'%'
	                OR LOWER(Views) LIKE '%'+LOWER(@Q)+'%')
                {advanceRequestStr}
                ";

            using (var connection = _POVWebDbContextDapper.Connection)
            {
                connection.Open();
                try
                {
                    var resultNum = await connection.ExecuteScalarAsync<int>(sqlNum, request, commandTimeout: 180);
                    var result = await connection.QueryAsync(sql, request, commandTimeout: 180);
                    return Ok(FormatResultModel<dynamic>.Success(new { result, resultNum }));
                }
                catch (Exception ex)
                {
                    return Ok(FormatResultModel<dynamic>.Failure(new { Msg = ex.Message }));
                }
            }
        }

        [HttpPost("V1/AddAnnouncementData")]
        public async Task<ActionResult> AddAnnouncementData(AddAnnouncementDataRequest request)
        {
            string addSQL = @$"
                DECLARE @AnnouncementID VARCHAR(20)

	            SET @AnnouncementID = (CONVERT(VARCHAR, GETDATE(), 120) + RIGHT(CONVERT(VARCHAR, GETDATE(), 114), 7))

	            BEGIN TRY
	            INSERT INTO [dbo].[Announcement]
                       ([AnnouncementID], [Subject], [PostingTime], [PostingUnit], [PostingPerson], [PostingContent]
                       , [CheckTop], [Views])
                 VALUES
			            (@AnnouncementID, @Subject, @PostingTime, @PostingUnit, @PostingPerson, @PostingContent, @CheckTop, @Views)
	            END TRY
	            BEGIN CATCH
		            ;THROW
	            END CATCH	

	            SELECT @AnnouncementID
            ";

            string AnnouncementID = "";
            using (var connection = _POVWebDbContextDapper.Connection)
            {
                connection.Open();
                using var transaction = connection.BeginTransaction();
                try
                {
                    AnnouncementID = await connection.ExecuteScalarAsync<string>(
                            sql: addSQL,
                            request,
                            transaction: transaction,
                            commandTimeout: 180);
                    transaction.Commit();
                }
                catch (SqlException e)
                {
                    return Ok(FormatResultModel<dynamic>.Failure(new { MSG = e.Message }));
                }
            }
            return Ok(FormatResultModel<dynamic>.Success(new { AnnouncementID }));
        }

        [HttpPost("V1/UpdAnnouncement")]
        public async Task<ActionResult> UpdAnnouncement(UpdAnnouncementDataRequest request)
        {
            string updSQL = @$"
                    IF EXISTS(SELECT 1 FROM Announcement WHERE AnnouncementID=@AnnouncementID) BEGIN                       
                        UPDATE Announcement
                        SET Subject=@Subject, PostingContent=@PostingContent,
                            CheckTop=@CheckTop
                        WHERE AnnouncementID=@AnnouncementID
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

        [HttpPost("V1/DelAnnouncementData")]
        public async Task<ActionResult> DelAnnouncementData(DelAnnouncementDataRequest request)
        {

            string actionStr =
                @$" Declare @O_MSG VARCHAR(MAX)=N''
                    BEGIN TRY
		                DELETE FROM Announcement WHERE AnnouncementID = @AnnouncementID
	                END TRY
	                BEGIN CATCH
		                SET @O_MSG = '刪除資料失敗。' + ERROR_MESSAGE() 
		                ;THROW 6636001, @O_MSG, 1;
	                END CATCH	
                ";
            try
            {
                var result = await _POVWebDbContextDapper.Connection.ExecuteAsync(actionStr, request.DelList);

                return Ok(FormatResultModel<dynamic>.Success(new { result }));
            }
            catch (Exception ex)
            {
                return Ok(FormatResultModel<dynamic>.Failure(new { MSG = ex.Message }));
            }
        }

        [HttpPost("V1/GetViewAnnouncementData")]
        public async Task<ActionResult> GetViewAnnouncementData(GetViewAnnouncementDataRequest request)
        {
            string getStr = @$"
                    SELECT AnnouncementID, PostingTime, Subject, Views , PostingContent, PostingPerson, PostingUnit
                    FROM Announcement
                    Where AnnouncementID = @AnnouncementID
                ";
            try
            {
                var result = await _POVWebDbContextDapper.Connection.QueryAsync(getStr, request);

                return Ok(FormatResultModel<dynamic>.Success(new { result }));
            }
            catch (Exception ex)
            {
                return Ok(FormatResultModel<dynamic>.Failure(new { MSG = ex.Message }));
            }
        }

        [HttpPost("V1/UpdViews")]
        public async Task<ActionResult> UpdViews(UpdViewsRequest request)
        {
            string updSQL = @$"
                    IF EXISTS(SELECT 1 FROM Announcement WHERE AnnouncementID=@AnnouncementID) BEGIN                       
                        UPDATE Announcement
                        SET AnnouncementID=@AnnouncementID, Views=@Views
                        WHERE AnnouncementID=@AnnouncementID
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

        [HttpPost("V1/AnnouncementContentEdit")]
        public async Task<ActionResult> AnnouncementContentEdit(AnnouncementContentEditRequest request)
        {
            string getStr = @$"
                    Select * From Announcement
                    Where AnnouncementID = @AnnouncementID
                ";
            try
            {
                var result = await _POVWebDbContextDapper.Connection.QueryAsync(getStr, request);

                return Ok(FormatResultModel<dynamic>.Success(new { result }));
            }
            catch (Exception ex)
            {
                return Ok(FormatResultModel<dynamic>.Failure(new { MSG = ex.Message }));
            }
        }
    }
}
