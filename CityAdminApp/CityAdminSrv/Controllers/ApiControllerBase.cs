using Microsoft.AspNetCore.Mvc;

namespace CityAdminSrv.Controllers {
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

    }
}
