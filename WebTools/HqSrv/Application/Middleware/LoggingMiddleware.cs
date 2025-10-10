using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using Serilog;
using Serilog.Events;
using System.IO;

namespace HqSrv.Application.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {

            // 獲取 API 路徑
            var path = context.Request.Path.Value;
            var pathArr = path.Split('/');
            var pathArrLength = pathArr.Length - 1;
            var route = pathArr[pathArrLength];

            // 1. 獲取原始 Request Body 流
            var originalRequestBody = context.Request.Body;

            // 2. 使用 MemoryStream 暫存 Request Body
            using var memoryStream = new MemoryStream();
            await context.Request.Body.CopyToAsync(memoryStream);

            // 3. 將流位置重置為起始位置，方便讀取
            memoryStream.Seek(0, SeekOrigin.Begin);

            // 4. 讀取 Request Body 作為字符串（根據您的需求進行操作）
            var requestBodyText = await new StreamReader(memoryStream).ReadToEndAsync();

            // 記錄 Request Body（可用於日誌）
            // 判斷路徑是否以指定前綴開頭
            if (!string.IsNullOrEmpty(route) &&
                (route.StartsWith("Add", StringComparison.OrdinalIgnoreCase) ||
                 route.StartsWith("Upd", StringComparison.OrdinalIgnoreCase) ||
                 route.StartsWith("Del", StringComparison.OrdinalIgnoreCase)) && route != "UpdHeartbeat")
            {
                // 記錄日志
                //Log.ForContext("RequestPath", path).Warning(path);
            }

            // 5. 將流重置回起始位置，讓後續管道可以再次讀取
            memoryStream.Seek(0, SeekOrigin.Begin);
            context.Request.Body = memoryStream;

            // 6. 調用下一個中間件
            await _next(context);

            // 7. 最後，將原始流還原到請求中
            context.Request.Body = originalRequestBody;

            

        }
    }
}
