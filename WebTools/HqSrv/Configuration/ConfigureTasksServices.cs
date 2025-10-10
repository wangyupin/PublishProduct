using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using AutoMapper.Internal;


namespace HqSrv.Configuration
{
    public static class ConfigureTasksServices
    {
        public static IServiceCollection AddTasksServices(this IServiceCollection services, IConfiguration config)
        {


            return services;
        }
    }
}
