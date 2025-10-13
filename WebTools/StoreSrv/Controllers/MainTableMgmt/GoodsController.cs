using CityHubCore.Infrastructure.API;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using POVWebDomain.Models.DB.POVWeb;
using POVWebDomain.Models.API.StoreSrv.MainTableMgmt.Goods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using CityHubCore.Infrastructure.ServiceClient;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Hosting;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using POVWebDomain.Models.API.StoreSrv.Common;

namespace StoreSrv.Controllers.MainTableMgmt
{
    public class GoodsController : ApiControllerBase
    {
        private readonly ILogger<HelloController> _logger;
        private readonly POVWebDbContext _POVWebDb;
        private readonly POVWebDbContextDapper _POVWebDbContextDapper;
        private readonly HqSrvClient _HqSrvClient;
        private readonly IConfiguration _Config;

        public GoodsController(
            ILogger<HelloController> logger
            , POVWebDbContext pOVWebDbContext
            , POVWebDbContextDapper pOVWebDbContextDapper
            , HqSrvClient hqSrvClient
            , IConfiguration config
            )
        {
            _logger = logger;
            _POVWebDb = pOVWebDbContext;
            _POVWebDbContextDapper = pOVWebDbContextDapper;
            _HqSrvClient = hqSrvClient;
            _Config = config;
        }

        [HttpPost("V1/GetGoodsHelpOffset")]
        public async Task<ActionResult> GetGoodsHelpOffset(GetGoodsHelpOffsetRequest request)
        {

            // Biz
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<object>>("api/Goods/V1/GetGoodsHelpOffset", request);

            if (result is null || result.Succeeded is false || result.Data is null)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

    }
}
