using Microsoft.Extensions.Configuration;
using System.Net.Http;

namespace CityHubCore.Infrastructure.ServiceClient {
    public class StoreSrvClient : SrvClientBase {
        public StoreSrvClient(HttpClient client, IConfiguration config)
            : base(client, config, "StoreSrv") {
        }
    }
}
