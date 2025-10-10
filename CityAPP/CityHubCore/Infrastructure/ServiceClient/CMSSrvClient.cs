using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CityHubCore.Infrastructure.ServiceClient
{
    public class CMSSrvClient : SrvClientBase
    {
        public CMSSrvClient(HttpClient client, IConfiguration config)
            : base(client, config, "CMSSrv")
        {
        }
    }
}