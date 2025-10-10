using CityHubCore.Infrastructure.API;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using POVWebDomain.Models.DB.POVWeb;
using POVWebDomain.Models.API.StoreSrv.DashBoard;
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
using HqSrv.Repository.DashBoardMgmt;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace HqSrv.Controllers.DashBoard
{
    public class DashboardController : ApiControllerBase
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly POVWebDbContext _POVWebDb;
        private readonly POVWebDbContextDapper _POVWebDbContextDapper;
        private readonly CityAdminSrvClient _CityAdminSrvClient;
        private readonly IConfiguration _Config;
        private readonly DashboardRepository _dashBoardRepository;


        public DashboardController(
            ILogger<DashboardController> logger
            , POVWebDbContext pOVWebDbContext
            , POVWebDbContextDapper pOVWebDbContextDapper
            , CityAdminSrvClient cityAdminSrvClient
            , IConfiguration config
            , DashboardRepository dashBoardRepository
            )
        {
            _logger = logger;
            _POVWebDb = pOVWebDbContext;
            _POVWebDbContextDapper = pOVWebDbContextDapper;
            _CityAdminSrvClient = cityAdminSrvClient;
            _Config = config;
            _dashBoardRepository = dashBoardRepository;
        }

        [HttpPost("V1/GetPeersSaleEstimateData")]
        public async Task<ActionResult> GetPeersSaleEstimateData(GetPeersSaleEstimateDataRequest request)
        {

            string sql = @$"
                SELECT P.ClientID, C.ClientName, Year
                       , MonthAmount1, MonthAmount2, MonthAmount3, MonthAmount4, MonthAmount5, MonthAmount6, MonthAmount7, MonthAmount8, MonthAmount9, MonthAmount10, MonthAmount11, MonthAmount12
                FROM PeersSaleSet P
                LEFT JOIN Client C ON C.ClientID = P.ClientID
                WHERE P.ClientID = @ClientID
                    AND Year=@Year
                ";

            using (var connection = _POVWebDbContextDapper.Connection)
            {
                connection.Open();
                try
                {
                    var result = await connection.QueryAsync(sql, request, commandTimeout: 180);
                    return Ok(FormatResultModel<dynamic>.Success(new { result }));
                }
                catch (Exception ex)
                {
                    return Ok(FormatResultModel<dynamic>.Failure(new { Msg = ex.Message }));
                }
            }
        }

        [HttpPost("V1/CheckInvoiceData")]
        public async Task<ActionResult> CheckInvoiceData(CheckInvoiceRequest request)
        {
            try
            {
                var result = await _dashBoardRepository.CheckInvoiceData(request);
                return Ok(FormatResultModel<dynamic>.Success(result.Item1));
            }
            catch (SqlException e)
            {
                return Ok(FormatResultModel<dynamic>.Failure(new { MSG = e.Message }));
            }
        }

        [HttpPost("V1/GetInvoiceData")]
        public async Task<ActionResult> GetInvoiceData(GetInvoiceRequest request)
        {
            string message = string.Empty;
            try
            {
                var result = await _dashBoardRepository.GetInvoiceData(request);
                if (result.Item1)
                {
                    return Ok(FormatResultModel<dynamic>.Success(result.Item1));
                }
                else
                {
                    message = result.Item2;
                    return Ok(FormatResultModel<dynamic>.Failure(message));
                }
            }
            catch (SqlException e)
            {
                return Ok(FormatResultModel<dynamic>.Failure(new { MSG = e.Message }));
            }
        }

        [HttpPost("V1/GetCompanyTemplateData")]
        public async Task<ActionResult> GetCompanyTemplateData(GetCompanyTemplateDataRequest request)
        {
            string getStr = @$"
                    Select * From Template
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