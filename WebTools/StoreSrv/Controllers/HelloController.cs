using CityHubCore.Infrastructure.API;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using POVWebDomain.Models.DB.POVWeb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using CityHubCore.Infrastructure.ServiceClient;
using Microsoft.Extensions.Configuration;

namespace StoreSrv.Controllers {
    public class GetVersionRequest {
        public string Token { get; set; }
    }

    public class GetHelloRequest : GetVersionRequest {
        public int id { get; set; }
    }

    public class AddHelloAllRequest : GetVersionRequest {

    }

    public class AddHelloRequest : GetVersionRequest {
        public string memo { get; set; }
    }

    public class AddHelloListRequest : GetVersionRequest {
        public List<string> memoList { get; set; }
    }

    public class HelloController : ApiControllerBase {
        private readonly ILogger<HelloController> _logger;
        private readonly POVWebDbContext _POVWebDb;
        private readonly POVWebDbContextDapper _POVWebDbContextDapper;
        private readonly HqSrvClient _HqSrvClient;
        private readonly IConfiguration _Config;

        public HelloController(
            ILogger<HelloController> logger
            , POVWebDbContext pOVWebDbContext
            , POVWebDbContextDapper pOVWebDbContextDapper
            , HqSrvClient hqSrvClient
            , IConfiguration config
            ) {
            _logger = logger;
            _POVWebDb = pOVWebDbContext;
            _POVWebDbContextDapper = pOVWebDbContextDapper;
            _HqSrvClient = hqSrvClient;
            _Config = config;
        }

        private object GetSrvClientSetting(string SessionName) {
            return new {
                Uri = _Config[$"{SessionName}:Uri"],
                UserAgent = _Config[$"{SessionName}:UserAgent"],
                Timeout = _Config[$"{SessionName}:Timeout"]
            };
        }

        private object GetLocalHostInfo() {
            return new {
                Product = "StoreSrv",
                Version = "V0.0.0.1",
                Date = "2021/12/13",
                ServerDT = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff}",
                HqSrv = GetSrvClientSetting("HqSrv"),
                DeviceSrv = GetSrvClientSetting("DeviceSrv")
            };
        }

        private bool IsTokenBad(string token ) {
            // Check parameters
            token ??= "";

            // Initial variables
            var nowDTM = DateTime.UtcNow;
            var nowToken = $"{nowDTM:MMddmm}";
            var lastToken = $"{nowDTM.AddMinutes(-1):MMddmm}";

            // Biz
            var result = token.Equals(nowToken) || token.Equals(lastToken);

            // Result
            return !result;
        }

        [HttpPost("V1/GetVersion")]
        public ActionResult GetVersion(GetVersionRequest request) {
            // Check parameters
            if (IsTokenBad(request.Token))
                return BadRequest(FormatResultModel<dynamic>.Failure(
                    new {
                        MSG = "Bad Token",
                        ServerDT = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff}"
                    }));

            // Initial variables

            // Biz

            // Result
            return Ok(FormatResultModel<dynamic>.Success(GetLocalHostInfo()));
        }

        [HttpPost("V1/GetAllVersion")]
        public ActionResult GetAllVersion(GetVersionRequest request) {
            // Check parameters
            if (IsTokenBad(request.Token))
                return BadRequest(FormatResultModel<dynamic>.Failure(
                    new {
                        MSG = "Bad Token",
                        ServerDT = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff}"
                    }));

            // Initial variables

            // Biz
            var result = _HqSrvClient.HttpPost<ResultModel<object>>("api/Hello/V1/GetVersion",
                new GetVersionRequest() { Token = $"{DateTime.UtcNow:MMddmm}" }
                );

            if (result is null || result.Succeeded is false || result.Data is null) {
                return Ok(FormatResultModel<dynamic>.Failure(
                    new {
                        MSG = "HqSrvClient not exist",
                        ServerDT = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff}"
                    }));
            }

            // Result
            return Ok(FormatResultModel<dynamic>.Success(
                new {
                    storeSrv = GetLocalHostInfo(),
                    hqSrv = result
                }));
        }

        [HttpPost("V1/GetAllVersionAsync")]
        public async Task<ActionResult> GetAllVersionAsync(GetVersionRequest request) {
            // Check parameters
            if (IsTokenBad(request.Token))
                return BadRequest(FormatResultModel<dynamic>.Failure(
                    new {
                        MSG = "Bad Token",
                        ServerDT = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff}"
                    }));

            // Initial variables

            // Biz
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<object>>("api/Hello/V1/GetAllVersionAsync", 
                new GetVersionRequest() { Token = $"{DateTime.UtcNow:MMddmm}" }
                );

            if (result is null || result.Succeeded is false || result.Data is null) {
                    return Ok(FormatResultModel<dynamic>.Failure(
                        new {
                            MSG = "HqSrvClient not exist",
                            ServerDT = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff}"
                        }));
            } 

            // Result
            return Ok(FormatResultModel<dynamic>.Success(
                new {
                    storeSrv = GetLocalHostInfo(),
                    hqSrv = result
                }));
        }

        #region EF Core and Dapper Sample

        [HttpPost("V1/GetHelloByEFCore")]
        public ActionResult GetHelloByEFCore(GetHelloRequest request) {
            // Check parameters
            if (IsTokenBad(request.Token))
                return BadRequest(FormatResultModel<dynamic>.Failure(
                    new {
                        MSG = "Bad Token",
                        ServerDT = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff}"
                    }));

            // Initial variables

            // Biz
            var query = from tmpHello in _POVWebDb.tmpHello
                        where tmpHello.id == request.id
                        select tmpHello;
            // Result
            return query.FirstOrDefault() != null
                ? Ok(FormatResultModel<dynamic>.Success(
                    query
                    ))
                : Ok(FormatResultModel<dynamic>.Failure(
                    new {
                        MSG = $"id->{request.id} not found"
                    }));
        }

        [HttpPost("V1/GetHelloByDapper")]
        public ActionResult GetHelloByDapper(GetHelloRequest request) {
            // Check parameters
            if (IsTokenBad(request.Token))
                return BadRequest(FormatResultModel<dynamic>.Failure(
                    new {
                        MSG = "Bad Token",
                        ServerDT = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff}"
                    }));

            // Initial variables
            string seelctStr =
                @"SELECT * 
                     FROM PovWeb.tmpHello as M
                    WHERE M.id = @tmpHelloId";
            // Biz
            List<tmpHello> tmpHelloList = _POVWebDbContextDapper.Connection.Query<tmpHello>(seelctStr,
                new {
                    tmpHelloId = request.id
                })
                .ToList();

            // Result
            return tmpHelloList.Count() > 0
                ? Ok(FormatResultModel<dynamic>.Success(
                    tmpHelloList
                    ))
                : Ok(FormatResultModel<dynamic>.Failure(
                    new {
                        MSG = $"id->{request.id} not found"
                    }));
        }

        [HttpPost("V1/GetHelloAll")]
        public ActionResult GetHelloAllByDapper(AddHelloAllRequest request) {
            // Check parameters
            if (IsTokenBad(request.Token))
                return BadRequest(FormatResultModel<dynamic>.Failure(
                    new {
                        MSG = "Bad Token",
                        ServerDT = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff}"
                    }));

            // Initial variables
            string seelctStr =
                @$"SELECT * 
                     FROM PovWeb.tmpHello as M
                 ORDER BY M.id DESC";
            // Biz
            List<tmpHello> tmpHelloList = _POVWebDbContextDapper.Connection.Query<tmpHello>(seelctStr)
                .ToList();

            // Result
            return Ok(FormatResultModel<dynamic>.Success(
                    new {
                        totalCount = $"{tmpHelloList.Count()}",
                        tmpHelloList
                    }));
        }

        [HttpPost("V1/AddHello")]
        public ActionResult AddHelloByDapper(AddHelloRequest request) {
            // Check parameters
            if (IsTokenBad(request.Token))
                return BadRequest(FormatResultModel<dynamic>.Failure(
                    new {
                        MSG = "Bad Token",
                        ServerDT = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff}"
                    }));

            // Initial variables
            string insertStr =
                @"INSERT INTO PovWeb.tmpHello (memo)
                   OUTPUT Inserted.id 
                   VALUES (@memo)";

            // Biz
            var result = _POVWebDbContextDapper.Connection.ExecuteScalar(insertStr,
               new {
                   // memo = request.memo //參數與變數名相同時，可省略。
                   request.memo
               });

            // Result
            return (result != null)
                ? Ok(FormatResultModel<dynamic>.Success(
                    new {
                        MSG = $"Insert OK. id->{Convert.ToInt32(result)}"
                    }))
                : Ok(FormatResultModel<dynamic>.Failure(
                    new {
                        MSG = $"Insert NG"
                    }));
        }

        [HttpPost("V1/AddHelloList")]
        public ActionResult AddHelloListByDapper(AddHelloListRequest request) {
            // Check parameters
            if (IsTokenBad(request.Token))
                return BadRequest(FormatResultModel<dynamic>.Failure(
                    new {
                        MSG = "Bad Token",
                        ServerDT = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff}"
                    }));

            if (request.memoList == null || request.memoList.Count() < 1) {
                return Ok(FormatResultModel<dynamic>.Failure(
                    new {
                        MSG = "memoList not exist",
                        ServerDT = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff}"
                    }));
            }

            // Initial variables
            string insertStr =
                @$"INSERT INTO PovWeb.tmpHello (memo)
                   OUTPUT Inserted.id 
                   VALUES (@memo)";

            int keepCount = 36;
            string housekeepingSql =
                @$"DELETE 
                  FROM PovWeb.tmpHello
                  WHERE id IN(SELECT id
                                FROM PovWeb.tmpHello AS M
                               ORDER BY M.createDTM DESC
                              OFFSET {keepCount} ROWS)";

            // Biz
            int effectRows = 0;
            using (var connection = _POVWebDbContextDapper.Connection) {
                connection.Open();
                using (var transaction = connection.BeginTransaction()) {
                    // Insert List
                    foreach (var memo in request.memoList) {
                        effectRows += connection.Execute(insertStr,
                            new {
                                memo
                            },
                            transaction: transaction);
                    }

                    // Housekeeping
                    connection.Execute(housekeepingSql, transaction: transaction);

                    transaction.Commit();
                }
            }

            // Result
            return Ok(FormatResultModel<dynamic>.Success(
                    new {
                        MSG = $"effectRows = {effectRows}"
                    }));
        }

        #endregion
    }
}
