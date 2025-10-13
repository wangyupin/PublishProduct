using CityHubCore.Infrastructure.API;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using POVWebDomain.Models.DB.POVWeb;
using POVWebDomain.Models.API.StoreSrv.MainTableMgmt.Goods;
using System;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using CityHubCore.Infrastructure.ServiceClient;
using Microsoft.AspNetCore.Hosting;



namespace HqSrv.Controllers.MainTableMgmt
{
    public class GoodsController : ApiControllerBase
    {
        private readonly ILogger<HelloController> _logger;
        private readonly POVWebDbContext _POVWebDb;
        private readonly POVWebDbContextDapper _POVWebDbContextDapper;
        private readonly CityAdminSrvClient _CityAdminSrvClient;
        private readonly IConfiguration _Config;
        private readonly IWebHostEnvironment _hostEnvironment;
        

        public GoodsController(
            ILogger<HelloController> logger
            , POVWebDbContext pOVWebDbContext
            , POVWebDbContextDapper pOVWebDbContextDapper
            , CityAdminSrvClient cityAdminSrvClient
            , IConfiguration config
            , IWebHostEnvironment hostEnvironment
            )
        {
            _logger = logger;
            _POVWebDb = pOVWebDbContext;
            _POVWebDbContextDapper = pOVWebDbContextDapper;
            _CityAdminSrvClient = cityAdminSrvClient;
            _Config = config;
            _hostEnvironment = hostEnvironment;
        }

  

        [HttpPost("V1/GetGoodsHelpOffset")]
        public async Task<ActionResult> GetGoodsHelpOffset(GetGoodsHelpOffsetRequest request)
        {
            string advanceRequestStr = "";
            string searchTermRequestStr = "";
            string userOrderByStr = string.Format("ORDER BY {0} {1}", request.SortColumn, request.Sort);
            if(request.SortColumn == "GoodID")
            {
                userOrderByStr = $"order by styleRemark {request.Sort}, t4.DisplayOrder asc";
            }
            string offSetStr = request.Mode == String.Empty ?
                @$"OFFSET (@Page-1)*@PerPage ROWS 
                   FETCH NEXT @PerPage ROWS ONLY" : "";
            if (request.AdvanceRequest != null)
            {
                advanceRequestStr = request.AdvanceRequest.GenerateSqlString();
            }
            if (request.SearchTerm != null)
            {
                searchTermRequestStr = request.SearchTerm.GenerateSqlString();
            }
            string sql = @$"
                select GoodID, GoodName, t1.BrandName, t2.FactoryName, AdvicePrice, PicturePath, Sort01, Sort02, Sort03, Sort04, Sort05, ParentID, Currency,
                        IntoPrice, Cost, s_Ply, price, specialPrice, hongLiTimes, Season, Material1, P_StyleA, P_StyleB, P_StyleC, P_StyleD, P_StyleE, t3.Bar,
                        GoodNameUS, ParentID, CurrencyRate, TradePrice1, SizeTagName as SizeName, YearOfSeason, t0.OpenDate, SellDateST, Remark, GoodTags, t0.Display, Material,
				        IsStruct, IsExclCost
                from Goods t0
                left join Brand t1 on t1.BrandID = t0.Brand
                left join Factory t2 on t2.FactoryID = t0.Factory
                outer apply(
					SELECT STRING_AGG(Bar, ',') AS Bar
					from GoodsBar GB
					where GB.GoodsID = t0.GoodID
				) t3
                left join SizeTag t4 on t0.SizeName = t4.SizeTagID
                where 1=1
                and (goodID like '%' + @Q + '%'OR goodName like '%' + @Q + '%'OR factoryName like '%' + @Q + '%'OR brandName like '%' + @Q + '%'OR parentID like '%' + @Q + '%'OR advicePrice like '%' + @Q + '%'OR currency like '%' + @Q + '%'OR intoPrice like '%' + @Q + '%'OR cost like '%' + @Q + '%'OR s_Ply like '%' + @Q + '%'OR price like '%' + @Q + '%'OR specialPrice like '%' + @Q + '%'OR hongLiTimes like '%' + @Q + '%'OR season like '%' + @Q + '%'OR material1 like '%' + @Q + '%'OR p_StyleA like '%' + @Q + '%'OR p_StyleB like '%' + @Q + '%'OR p_StyleC like '%' + @Q + '%'OR p_StyleD like '%' + @Q + '%'OR p_StyleE like '%' + @Q + '%'OR sort01 like '%' + @Q + '%'OR sort02 like '%' + @Q + '%'OR sort03 like '%' + @Q + '%'OR sort04 like '%' + @Q + '%'OR sort05 like '%' + @Q + '%'OR goodNameUS like '%' + @Q + '%'OR parentID like '%' + @Q + '%'OR currencyRate like '%' + @Q + '%'OR tradePrice1 like '%' + @Q + '%'OR sizeName like '%' + @Q + '%'OR yearOfSeason like '%' + @Q + '%'OR t0.openDate like '%' + @Q + '%'OR sellDateST like '%' + @Q + '%'OR remark like '%' + @Q + '%'OR t0.display like '%' + @Q + '%'OR material like '%' + @Q + '%'OR isStruct like '%' + @Q + '%'OR isExclCost like '%' + @Q + '%'OR Bar like '%' + @Q + '%')
                {advanceRequestStr}
                {userOrderByStr}
                {offSetStr}
                ";
            string sqlNum = @$"
                select Count(*)
                from Goods t0
                left join Brand t1 on t1.BrandID = t0.Brand
                left join Factory t2 on t2.FactoryID = t0.Factory
                left join GoodsBar t3 on t3.GoodsID = t0.GoodID
                where 1=1
                and (goodID like '%' + @Q + '%'OR goodName like '%' + @Q + '%'OR factoryName like '%' + @Q + '%'OR brandName like '%' + @Q + '%'OR parentID like '%' + @Q + '%'OR advicePrice like '%' + @Q + '%'OR currency like '%' + @Q + '%'OR intoPrice like '%' + @Q + '%'OR cost like '%' + @Q + '%'OR s_Ply like '%' + @Q + '%'OR price like '%' + @Q + '%'OR specialPrice like '%' + @Q + '%'OR hongLiTimes like '%' + @Q + '%'OR season like '%' + @Q + '%'OR material1 like '%' + @Q + '%'OR p_StyleA like '%' + @Q + '%'OR p_StyleB like '%' + @Q + '%'OR p_StyleC like '%' + @Q + '%'OR p_StyleD like '%' + @Q + '%'OR p_StyleE like '%' + @Q + '%'OR sort01 like '%' + @Q + '%'OR sort02 like '%' + @Q + '%'OR sort03 like '%' + @Q + '%'OR sort04 like '%' + @Q + '%'OR sort05 like '%' + @Q + '%'OR goodNameUS like '%' + @Q + '%'OR parentID like '%' + @Q + '%'OR currencyRate like '%' + @Q + '%'OR tradePrice1 like '%' + @Q + '%'OR sizeName like '%' + @Q + '%'OR yearOfSeason like '%' + @Q + '%'OR t0.openDate like '%' + @Q + '%'OR sellDateST like '%' + @Q + '%'OR remark like '%' + @Q + '%'OR t0.display like '%' + @Q + '%'OR material like '%' + @Q + '%'OR isStruct like '%' + @Q + '%'OR isExclCost like '%' + @Q + '%'OR Bar like '%' + @Q + '%')
                {advanceRequestStr}
                ";

            using (var connection = _POVWebDbContextDapper.Connection)
            {
                connection.Open();
                try
                {
                    var resultNum = await connection.ExecuteScalarAsync<int>(sqlNum, request, commandTimeout: 180);
                    var result = await connection.QueryAsync(sql, request, commandTimeout: 180);
                    //int num = resultNum % request.Offset == 0 ? resultNum / request.Offset : (resultNum / request.Offset) + 1;
                    return Ok(FormatResultModel<dynamic>.Success(new { result, resultNum }));
                }
                catch (Exception ex)
                {
                    return Ok(FormatResultModel<dynamic>.Failure(new { Msg = ex.Message }));
                }
            }
        }

   
    }
}
