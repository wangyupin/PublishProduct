using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CityHubCore.Infrastructure.ServiceClient {
    public class DeviceSrvClient : SrvClientBase {
        public DeviceSrvClient(HttpClient client, IConfiguration config)
            : base(client, config, "DeviceSrvClient") {
        }
    }
}
