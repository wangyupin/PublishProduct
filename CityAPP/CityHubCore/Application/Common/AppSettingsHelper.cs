using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityHubCore.Application.Exceptions {
    public class AppSettingsHelper {
        public static IConfiguration Configuration { get; set; }
        /// <summary>
        /// Sample
        /// string sqlString= AppSettingsHelper.Configuration.GetConnectionString("TestConnection");
        /// string sqlString1 = AppSettingsHelper.Configuration["Logging:LogLevel:Default"];
        /// </summary>
        static AppSettingsHelper() {
            //ReloadOnChange = true 當appsettings.json被修改時重新加載            
            Configuration = new ConfigurationBuilder()
            .Add(new JsonConfigurationSource { Path = "appsettings.json", ReloadOnChange = true })
            .Build();
        }

    }
}
