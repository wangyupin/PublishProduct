using CityAdminDomain.Models.DB.CityAdmin;
using CityAdminDomain.Models.Enum;
using CityHubCore.Application.Jwt;
using CityHubCore.Application.Totp;
using CityHubCore.Infrastructure.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CityAdminSrv.Configuration {
    public static class ConfigureApplicationServices {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration) {
            #region 請先看使用說明，再開始修改。 MoonFeng @ 2021/9/10 00:11
            /*
                ASP.NET DI 容器提供三種選項：
                Singleton 整個 Process 只建立一個 Instance，任何時候都共用它。
                Scoped 在網頁 Request 處理過程(指接到瀏覽器請求到回傳結果前的執行期間)共用一個 Instance。
                Transient 每次要求元件時就建立一個新的，永不共用。
                REF: https://blog.darkthread.net/blog/aspnet-core-di-notes/

                How to Read configuration
                REF: https://medium.com/@dozieogbo/a-better-way-to-inject-appsettings-in-asp-net-core-96be36ffa22b

                MoonFeng @ 2021/8/4
            */

            /*
                configure strongly typed settings object
                IOptions<T> same for the lifetime of the application
                IOptionsSnapshot<T> same for request lifetime.
                IOptionsMonitor<T> real time. be careful, it will a heavy cost for access physical storage.
                REF https://medium.com/@kmar.ayush/eli5-ioptions-vs-ioptionssnaphot-vs-ioptionsmonitor-fab1d7e26a75
                MoonFeng @ 2021/8/13 15:23
            */

            /* 
                AddDbContext VS AddDbContextPool
                REF : https://stackoverflow.com/questions/48443567/adddbcontext-or-adddbcontextpool
                https://docs.microsoft.com/en-us/ef/core/what-is-new/ef-core-2.0/#dbcontext-pooling
                MoonFeng @ 2021/9/8 15:38
            */
            #endregion 

            //Setup CityAdminDb
            //EF Core
            var CityAdminConnectionString = configuration.GetConnectionString(ConnectionStringName.CityAdminDb.ToString());
            services.AddDbContext<CityAdminDbContext>(options => {
                //TODO: 連線字串資安
                options.UseSqlServer(CityAdminConnectionString);
            });
            //Dapper 
            services.AddScoped<IApplicationDbContext>(provider => provider.GetService<CityAdminDbContext>());
            services.AddScoped<CityAdminDbContextDapper>();
            services.AddScoped(m => new CityAdminDapper(CityAdminConnectionString));

            // Configure JWT Service. MoonFeng @ 2021/8/21 17:20
            // user for swagger
            services.Configure<JwtConfig>(configuration.GetSection("Jwt"));

            // Configure TOPT Helper. MoonFeng @ 2021/10/12 17:19
            services.Configure<TotpConfig>(configuration.GetSection("Totp"));
            services.AddScoped<TotpHelper>();

            // 註冊 Mapper
            services.AddAutoMapper(typeof(Startup));
            // configure DI for application services sample
            //services.AddScoped<IOrderRepository, OrderRepository>();
            //services.AddSingleton<IUriComposer>(new UriComposer(configuration.Get<CatalogSettings>()));
            //services.AddScoped(typeof(IAppLogger<>), typeof(LoggerAdapter<>));
            //services.AddTransient<IEmailSender, EmailSender>();

            return services;
        }
    }
}
