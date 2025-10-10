using CityHubCore.Application.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;

namespace StoreSrv.Configuration {
    public static class ConfigureSwaggerServices {
        public static IServiceCollection AddSwaggerServices(this IServiceCollection services, IConfiguration config) {

            var jwtConfig = services.BuildServiceProvider().GetRequiredService<IOptions<JwtConfig>>().Value;

            if (jwtConfig.HeaderName is null) throw new KeyNotFoundException("ConfigureSwaggerServices.cs > Jwt.HeaderName not found in appsettings.json");

            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "StoreSrv", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme() {
                    Name = jwtConfig.HeaderName,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "請輸入Access Token",
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                        {
                          new OpenApiSecurityScheme {
                                Reference = new OpenApiReference {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                },
                                Scheme = "oauth2",
                                Name = "Bearer",
                                In = ParameterLocation.Header
                            },
                            new List<string>()
                        }
                    });

                //Swagger different classes in different namespaces with same name
                //https://stackoverflow.com/questions/56475384/swagger-different-classes-in-different-namespaces-with-same-name-dont-work
                c.CustomSchemaIds(x => x.FullName);
            });

            return services;
        }
    }
}