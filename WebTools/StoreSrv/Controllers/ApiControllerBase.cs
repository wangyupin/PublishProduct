using Microsoft.AspNetCore.Mvc;

namespace StoreSrv.Controllers {
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public abstract class ApiControllerBase : ControllerBase {

        //private ISender _mediator;
        //protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetService<ISender>();
        //protected readonly ECSrvClient _ecSrvClient;

        //protected ApiControllerBase(ECSrvClient ecSrvClient) {
        //    _ecSrvClient = ecSrvClient;
        //}

        public string RemoteIpAddress {
            get {
                if (Request.Headers.ContainsKey("X-Forwarded-For"))
                    return Request.Headers["X-Forwarded-For"];
                else
                    return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            }
        }

        public string RemoteUserAgent {
            get {
                if (Request.Headers.ContainsKey("User-Agent"))
                    return Request.Headers["User-Agent"];
                else
                    return null;
            }
        }

    }
}
