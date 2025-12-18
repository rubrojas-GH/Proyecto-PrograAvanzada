using MvcTienda.Aplicacion.Dashboard;
using System.Web.Http;

namespace MvcTienda.API.Controllers
{
    [RoutePrefix("api/dashboard")]
    public class DashboardController : ApiController
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        // GET: api/dashboard
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetEstadisticas()
        {
            var stats = _dashboardService.ObtenerEstadisticas();
            return Ok(stats);
        }
    }
}
