using LocalPortService.BizService.CardMachine;
using LocalPortService.BizService.PDF;
using LocalPortService.BizService.PrinterInformation;
using LocalPortService.BizService.ProgramLauch;
using LocalPortService.BizService.TransferMgmt;
using LocalPortService.BizServices.SaleMgmt;
using LocalPortService.Core.ModelValid;
using LocalPortService.Core.ProgramUpdate;
using LocalPortService.Model.API;
using LocalPortService.Model.CardMachine;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.WindowsServices;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using Velopack;

//#if RELEASE
await VeloPackService.UpdateMyApp();
VelopackApp.Build().Run();
//#endif
var webApOpts = new WebApplicationOptions
{
    ContentRootPath = WindowsServiceHelpers.IsWindowsService() ?
        AppContext.BaseDirectory : default,
    Args = args
};
var builder = WebApplication.CreateBuilder(webApOpts);
builder.Host.UseWindowsService();
builder.WebHost.ConfigureKestrel((options) =>
{
    options.ListenLocalhost(8787);
});

var config = builder.Configuration; // 取得 IConfiguration
// 取得檔案儲存位置
//var fileStorePath = config.GetValue<string>("FileStorePath");
//Directory.CreateDirectory(fileStorePath);
// 取得允許的副檔名
//var allowExts = config.GetSection("AllowExts").Get<string[]>();

// 加入 CORS 服務
const string OriginsFromSetting = "OriginsFromAppSettingsJson";
builder.Services.AddCors(options => {
    options.AddPolicy(
        name: OriginsFromSetting,
        builder => {
            builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
        }
    );
});

// 註冊服務
builder.Services.AddTransient<CardPaymentService>();
builder.Services.AddTransient<TransferServices>();
builder.Services.AddTransient<SaleServices>();
builder.Services.AddTransient<PDFPrintService>();
builder.Services.AddTransient<PrinterService>();
builder.Services.AddTransient<LaunchExistingFileService>();
builder.Services.Configure<JsonOptions>((options) =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
});

//timeOut
builder.Services.AddRequestTimeouts(options => {
    options.AddPolicy("MyPolicy", TimeSpan.FromMicroseconds(100));
});
var app = builder.Build();

//In apps that explicitly call UseRouting, UseRequestTimeouts must be called after UseRouting.
//When an app is running in debug mode, the timeout middleware doesn't trigger. This behavior is the same as for Kestrel timeouts. To test timeouts, run the app without the debugger attached.
app.UseRequestTimeouts();

// 啟用 CORS Middleware
app.UseCors();

app.MapPost("/getMacAddress", (IConfiguration configuration, LoginMacAddressModel req) =>
{

    bool IsFront = configuration.GetValue<bool>("IsFront");
    if (!(IsFront && !req.CityAdminIsFront))
    {
        if (req.CityAdminIsFront)
        {
            var macAddresses = NetworkInterface.GetAllNetworkInterfaces()
                                                      .Where(nic => (nic.OperationalStatus == OperationalStatus.Up || nic.OperationalStatus == OperationalStatus.Down) && nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                                                      .Select(nic => nic.GetPhysicalAddress().ToString())
                                                      .Where(mac => !string.IsNullOrEmpty(mac))
                                                      .FirstOrDefault();
            var responseData = new
            {
                Address = macAddresses,
                Status = "Success"
            };

            return Results.Ok(responseData);
        }
        else
        {
            return Results.Ok();
        }
    }
    else
    {
        return Results.BadRequest(new {msg = "禁止登入"});
    }
}).RequireCors(OriginsFromSetting);
app.MapPost("/transferPrinter", async (TransferServices transferService, TransferPrinterJsonObj req) =>
{
    await transferService.ExecThermalPrinter((req.Head, req.Body));
    return Results.Ok("列印成功");

}).RequireCors(OriginsFromSetting);

app.MapPost("/salePrinter", async (SaleServices saleService, SalePrinterJsonObj req) =>
{
    await saleService.ExecThermalPrinter((req.Head, req.Body));
    return Results.Ok("列印成功");

}).RequireCors(OriginsFromSetting);

app.MapPost("/salePrinterLittle", async (SaleServices saleService, SalePrinterJsonObj req) =>
{
    await saleService.ExecThermalPrinterLittle((req.Head, req.Body));
    return Results.Ok("列印成功");

}).RequireCors(OriginsFromSetting);

app.MapPost("/CardMachineCheckOut", async (HttpContext context, CardPaymentService service, CreditCardPayment req) =>
{
    try
    {
        (bool isValid, var errors) = MinimalApiModelValidator.Valid(req);
        if (!isValid) return Results.BadRequest(new { msg = errors });
        var stopwatch = Stopwatch.StartNew();
        var result = await service.ProcessPayment(req);
        stopwatch.Stop();
        if (stopwatch.Elapsed > TimeSpan.FromSeconds(100))
        {
            throw new TimeoutException();
        }
        return Results.Ok(result);
    }
    catch (TimeoutException)
    {
        return Results.Json(new { Error = "TimeOut!" }, statusCode: StatusCodes.Status504GatewayTimeout);
    }
}).RequireCors(OriginsFromSetting).WithRequestTimeout("MyPolicy");

app.MapPost("/CardMachineRefund", async (HttpContext context, CardPaymentService service, CreditCardRefund req) =>
{
    try
    {
        (bool isValid, var errors) = MinimalApiModelValidator.Valid(req);
        if (!isValid) return Results.BadRequest(new { msg = errors });
        var stopwatch = Stopwatch.StartNew();
        var result = await service.ProcessRefund(req);
        stopwatch.Stop();
        if (stopwatch.Elapsed > TimeSpan.FromSeconds(100))
        {
            throw new TimeoutException();
        }
        return Results.Ok(result);
    }
    catch (TimeoutException)
    {
        return Results.Json(new { Error = "TimeOut!" }, statusCode: StatusCodes.Status504GatewayTimeout);
    }

}).RequireCors(OriginsFromSetting).WithRequestTimeout("MyPolicy");

app.MapPost("/testInvoicePrinter", async (SaleServices saleService, SalesInvoice req) =>
{
    (bool isValid, var errors) = MinimalApiModelValidator.SaleValid(req);
    if (!isValid) return Results.BadRequest(new { msg = errors });
    bool result = saleService.PrintInvoice(req);
    if (result) return Results.Ok(new { msg = "列印成功" });
    else return Results.BadRequest(new { msg = "列印失敗" });

}).RequireCors(OriginsFromSetting);

app.MapPost("/testInvoiceDetailPrinter", async (SaleServices saleService, SalesInvoiceAndDetail req) =>
{
    (bool isValid, var errors) = MinimalApiModelValidator.SaleValid(req);
    if (!isValid) return Results.BadRequest(new { msg = errors });
    bool result = saleService.PrintInvoiceAndDetail(req);
    if (result) return Results.Ok(new { msg = "列印成功" });
    else return Results.BadRequest(new { msg = "列印失敗" });

}).RequireCors(OriginsFromSetting);

// 銷售時有統編就用這個 ****
app.MapPost("/testSaleInvoiceDetailPrinter", async (SaleServices saleService, SalesInvoiceAndDetail req) =>
{
    (bool isValid, var errors) = MinimalApiModelValidator.SaleValid(req);
    if (!isValid) return Results.BadRequest(new { msg = errors });
    bool result = saleService.PrintSaleInvoiceAndDetail(req);
    if (result) return Results.Ok(new { msg = "列印成功" });
    else return Results.BadRequest(new { msg = "列印失敗" });

}).RequireCors(OriginsFromSetting);

app.MapPost("/discountOrderPrinter", async (SaleServices saleService, DiscountOrderPrint req) =>
{
    (bool isValid, var errors) = MinimalApiModelValidator.SaleValid(req);
    if (!isValid) return Results.BadRequest(new { msg = errors });
    bool result = saleService.PrintDiscountOrder(req);
    if (result) return Results.Ok(new { msg = "列印成功" });
    else return Results.BadRequest(new { msg = "列印失敗" });

}).RequireCors(OriginsFromSetting);

app.MapPost("/PDFPrinter", async (PDFPrintService PDFPrintService, PDFExample example) =>
{
    (bool isValid, var errors) = MinimalApiModelValidator.Valid(example);
    if (!isValid) return Results.BadRequest(new { msg = errors });
    bool result = await PDFPrintService.Print((example));
    if (result) return Results.Ok(new { msg = "列印成功" });
    else return Results.BadRequest(new { msg = "列印失敗" });

}).RequireCors(OriginsFromSetting);

app.MapPost("/PDFPrinterPivotSizeNo", async (PDFPrintService PDFPrintService, PDFExample example) =>
{
    (bool isValid, var errors) = MinimalApiModelValidator.Valid(example);
    if (!isValid) return Results.BadRequest(new { msg = errors });
    bool result = await PDFPrintService.PrintPivot((example));
    if (result) return Results.Ok(new { msg = "列印成功" });
    else return Results.BadRequest(new { msg = "列印失敗" });

}).RequireCors(OriginsFromSetting);

app.MapPost("/PrinterService", async (PrinterService PrinterService, PrinterExample PrinterName) =>
{
    (bool isValid, var errors) = MinimalApiModelValidator.Valid(PrinterName);
    if (!isValid) return Results.BadRequest(new { msg = errors });
    string result = PrinterService.GetPrinter((PrinterName));
    if (result != null && result != "") return Results.Ok(new { result });
    else return Results.BadRequest(new { result });
}).RequireCors(OriginsFromSetting);

app.MapPost("/OpenProgram", async (LaunchExistingFileService launchExistingFileService, OpenProgramRequest req) =>
{
    (bool isValid, var errors) = MinimalApiModelValidator.Valid(req);
    if (!isValid) return Results.BadRequest(new { msg = errors });
    bool result = launchExistingFileService.LauchProgram(req);
    if (result) return Results.Ok(new { msg = "啟動成功" });
    return Results.BadRequest(new { msg = "啟動失敗" });

}).RequireCors(OriginsFromSetting);

app.MapPost("/PrintECPickingList", async (PDFPrintService PDFPrintService, ECPickingListMultiPDFModel example) =>
{
    (bool isValid, var errors) = MinimalApiModelValidator.Valid(example);
    if (!isValid) return Results.BadRequest(new { msg = errors });
    bool result = await PDFPrintService.PrintECPickingListMulti((example));
    if (result) return Results.Ok(new { msg = "列印成功" });
    else return Results.BadRequest(new { msg = "列印失敗" });

}).RequireCors(OriginsFromSetting);

app.MapPost("/PrintECMasterPickingList", async (PDFPrintService PDFPrintService, ECMasterPickingListPDFModel example) =>
{
    (bool isValid, var errors) = MinimalApiModelValidator.Valid(example);
    if (!isValid) return Results.BadRequest(new { msg = errors });
    bool result = await PDFPrintService.PrintECMasterPickingList((example));
    if (result) return Results.Ok(new { msg = "列印成功" });
    else return Results.BadRequest(new { msg = "列印失敗" });

}).RequireCors(OriginsFromSetting);

app.MapFallback(async (HttpContext context) =>
{
    return Results.NotFound("404 - Not Found");
});
app.Run();
