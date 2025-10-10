using AutoMapper.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using Serilog.Sinks.MSSqlServer;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Collections.ObjectModel;
using System.Data;
using static Serilog.Sinks.MSSqlServer.ColumnOptions;
using Coravel;

namespace HqSrv {
    public class Program {
        public static void Main(string[] args) {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // �t�m Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Warning()
                .WriteTo.MSSqlServer(
                    connectionString: BuildConnectionString(),
                    sinkOptions: new MSSqlServerSinkOptions
                    {
                        TableName = "ApplicationLogs",
                        AutoCreateSqlTable = true // �p�G�S���o�Ӫ�A���|�۰ʳЫ�
                    },
                    columnOptions: GetSqlColumnOptions()

                )
                .CreateLogger();

            try
            {
                var host = CreateHostBuilder(args).Build();

                // �t�m Coravel �Ƶ{��
                host.Services.UseScheduler(scheduler =>
                {
                    
                });

                // �B�����ε{��
                host.Run();
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.UseStartup<Startup>();

                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders(); // �M�����ت���Ӵ��Ѫ�
                    logging.AddSerilog(); // �ϥ� Serilog �@����Ӵ��Ѫ�
                });

        private static string BuildConnectionString()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .Build();

            // Ū���s���r��
            return builder.GetConnectionString("POVWebDb");
        }

        private static ColumnOptions GetSqlColumnOptions()
        {
            var colOptions = new ColumnOptions();

            colOptions.Store.Remove(StandardColumn.Properties);
            colOptions.Store.Remove(StandardColumn.MessageTemplate);
            //colOptions.Store.Remove(StandardColumn.Message);
            //colOptions.Store.Remove(StandardColumn.Exception);
            //colOptions.Store.Remove(StandardColumn.TimeStamp);
            //colOptions.Store.Remove(StandardColumn.Level);
            colOptions.AdditionalColumns = new Collection<SqlColumn>
            {
                new SqlColumn(){ColumnName = "RequestPath", DataType = System.Data.SqlDbType.NVarChar},
                new SqlColumn(){ColumnName = "RequestMethod", DataType = System.Data.SqlDbType.Int},
                new SqlColumn(){ColumnName = "ChangePerson", DataType = System.Data.SqlDbType.VarChar},
                new SqlColumn(){ColumnName = "ItemID", DataType = System.Data.SqlDbType.VarChar},
                new SqlColumn(){ColumnName = "LogTopic", DataType = System.Data.SqlDbType.VarChar},
            };
            return colOptions;
        }
    }
}
