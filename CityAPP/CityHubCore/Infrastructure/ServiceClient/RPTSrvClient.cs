using Microsoft.Extensions.Configuration;
using System.Net.Http;

namespace CityHubCore.Infrastructure.ServiceClient {
    public class RPTSrvClient : SrvClientBase {
        public RPTSrvClient(HttpClient client, IConfiguration config)
            : base(client, config, "RPTSrv") {
        }
    }
}
