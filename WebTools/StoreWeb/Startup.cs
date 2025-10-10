using CityHubCore.Application.Jwt;
using CityHubCore.Application.Session;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StoreWeb.Application.Middleware;
using StoreWeb.Configuration;
using System.Text.Json;

namespace StoreWeb {
    public class Startup {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env) {
            _configuration = configuration;
            _env = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            //���ϥ�AddControllers�A�]react�ä��ϥ�razer��Views�C
            //Ref https://www.carlrippon.com/whats-new-in-asp-net-core-3-0-for-react-spas/
            //MoonFeng @ 2021/7/31
            //services.AddControllersWithViews();
            //services.AddControllers();
            services.AddControllers(options =>
            {
                //options.ModelBinderProviders.Insert(0, new DtoFormBinderProvider());
            })
                //�w�]��Jason��camelCase MoonFeng @ 2022/8/4
                .AddJsonOptions(options => {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                });

            // todo Add AntiForgeryKey for CSRF. 
            // https://stackoverflow.com/questions/53487586/validateantiforgerytoken-in-an-asp-net-core-react-spa-application
            // https://codedocu.com/Software/React/Code/Code-Asp-Core-with-React_colon_-Save-in-API-with-Authorize,-Validate-Antiforgery?2767
            // MoonFeng @ 2021/8/6

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration => {
                configuration.RootPath = "ClientApp/build";
            });

            // Add Services
            services.AddApplicationServices(_configuration);

            // AddSwaggerServices 
            if (_env.IsDevelopment() || _env.IsStaging()) {
                services.AddSwaggerServices(_configuration);
            }

            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = long.MaxValue;
            });

            services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = long.MaxValue; // if don't set default value is: 30 MB
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (_env.IsDevelopment() || _env.IsStaging()) {
                app.UseDeveloperExceptionPage();
                /*
                Swagger  
                ref : https://docs.microsoft.com/zh-tw/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-5.0&tabs=visual-studio
                 */
                app.UseSwagger();
                app.UseSwaggerUI(c => {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "StoreWeb v1");
                });
            } else {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //Todo: Check server health
            //REF: https://marcus116.blogspot.com/2019/05/how-to-setup-netcore-aspnet-core-health-check.html
            //     http://dog0416.blogspot.com/2020/12/aspnet-core-health-checks-ui.html
            //app.UseHealthChecks("/health");

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            // global error handler
            app.UseMiddleware<ErrorHandlerMiddleware>();

            // custom jwt auth middleware
            app.UseMiddleware<JwtMiddleware>();

            //// custom Session auth middleware ! Should be setting behind of JwtMiddleware
            app.UseMiddleware<SessionMiddleware>();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa => {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment()) {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
