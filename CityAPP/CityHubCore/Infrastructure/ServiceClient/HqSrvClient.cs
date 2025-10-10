using Microsoft.Extensions.Configuration;
using System.Net.Http;

namespace CityHubCore.Infrastructure.ServiceClient {
    public class HqSrvClient : SrvClientBase {
        public HqSrvClient(HttpClient client, IConfiguration config)
            : base(client, config, "HqSrv") {
        }
    }
}
