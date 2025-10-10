using Coravel;
using HqSrv.Application.Middleware;
using HqSrv.Application.Services;
using HqSrv.Configuration;
using HqSrv.Factories.Ecommerce;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace HqSrv {
    public class Startup {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env) {

            _configuration = configuration;
            _env = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {

            services.AddControllers(options =>
            {
                //options.ModelBinderProviders.Insert(0, new DtoFormBinderProvider());
            })
                //預設為Jason為camelCase MoonFeng @ 2022/8/4
                .AddJsonOptions(options => {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                });
                

            // Add Services
            services.AddApplicationServices(_configuration);
            // Repo Services
            services.AddReopServices(_configuration);
            // IService
            services.AddBizServices(_configuration);
            // OuterApi Srevices
            services.AddExternalApiServices(_configuration);

            services.AddScoped<IEcommerceFactoryManager, EcommerceFactoryManager>();

            // Cache
            services.AddMemoryCache();

            // 添加 Coravel 服務
            services.AddScheduler();

            // AddTasksServices
            services.AddTasksServices(_configuration);


            // AddSwaggerServices
            if (_env.IsDevelopment() || _env.IsStaging()) {
                services.AddSwaggerServices(_configuration);
            }

            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = 2147483648;
            });

            services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = 2147483648; // if don't set default value is: 30 MB
            });

            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 2147483648;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment() || _env.IsStaging()) {
                app.UseDeveloperExceptionPage();
                /*
                Swagger  
                ref : https://docs.microsoft.com/zh-tw/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-5.0&tabs=visual-studio
                 */
                app.UseSwagger();
                app.UseSwaggerUI(c => {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "HqSrv v1");
                    //若要在應用程式的根目錄 (http://localhost:<port>/) 上提供 Swagger UI，請將 RoutePrefix 屬性設為空字串：
                    //c.RoutePrefix = string.Empty;
                });
            }

            // 在這裡添加自定義 middleware
            app.UseMiddleware<LoggingMiddleware>();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(env.ContentRootPath, "BackendImages")),
                RequestPath = "/BackendImages"
            });
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}
