using CityAdminDomain.Services.User;
using CityHubCore.Application.Jwt;
using CityHubCore.Infrastructure.DB;
using CityHubCore.Infrastructure.ServiceClient;
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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using POVWebDomain.Models.DB.POVWeb;
using System;
using System.Collections.Generic;
using System.Configuration;
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
               //�w�]��Jason��camelCase MoonFeng @ 2022/8/4
               .AddJsonOptions(options => {
                   options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                   options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
               });


            // ============================================
            // 1. ��¦�]�I�t�m (��Ʈw�BHTTP Client ��)
            // ============================================

            // EF Core & Dapper �t�m (�O���{��)
            var POVWebDbConnectionString = _configuration.GetConnectionString("POVWebDb");
            services.AddDbContext<POVWebDbContext>(options => {
                options.UseSqlServer(POVWebDbConnectionString);
            });
            services.AddScoped<IApplicationDbContext>(provider => provider.GetService<POVWebDbContext>());
            services.AddScoped<POVWebDbContextDapper>();
            services.AddScoped(m => new POVWebDapper(POVWebDbConnectionString));

            // HTTP Client
            services.AddHttpClient();

            // ============================================
            // 2. Domain Services (�֤߷~���޿�)
            // ============================================

            services.AddDomainServices();

            // ============================================
            // 3. Infrastructure Services (��¦�]�I��@)
            // ============================================

            services.AddInfrastructureServices();

            // ============================================
            // 4. Application Services (�ΨҨ�վ�)
            // ============================================

            services.AddApplicationServices(_configuration);

            // ============================================
            // 5. �ǲ� Repository Services (�V��ݮe)
            // ============================================

            services.AddRepositoryServices(_configuration);

            // ============================================
            // 6. ��L�A�� (JWT, AutoMapper ���A�O���{��)
            // ============================================

            services.Configure<JwtConfig>(_configuration.GetSection("Jwt"));
            services.AddHttpClient<CityAdminSrvClient>();
            services.AddAutoMapper(typeof(Startup));
            services.AddScoped<IUserAuth, UserAuthCityAdminService>();
            services.AddScoped<IUserAuth, UserAuthPOVWebService>();

            // Cache
            services.AddMemoryCache();

            // �K�[ Coravel �A��
            services.AddScheduler();

            // AddTasksServices
            services.AddTasksServices(_configuration);


            // AddSwaggerServices
            if (_env.IsDevelopment() || _env.IsStaging())
            {
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
                    //�Y�n�b���ε{�����ڥؿ� (http://localhost:<port>/) �W���� Swagger UI�A�бN RoutePrefix �ݩʳ]���Ŧr��G
                    //c.RoutePrefix = string.Empty;
                });
            }

            // �b�o�̲K�[�۩w�q middleware
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
