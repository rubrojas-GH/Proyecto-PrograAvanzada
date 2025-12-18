using System.Web.Http;

namespace MvcTienda.API.Controllers
{
    [RoutePrefix("api/test")]
    public class TestController : ApiController
    {
        [HttpGet]
        [Route("ping")]
        public IHttpActionResult Ping()
        {
            return Ok(new { message = "API funcionando correctamente" });
        }
    }
}
