using System.Web.Mvc;

namespace MvcTienda.Web.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class DashboardController : Controller
    {
        // SOLO retorna la vista
        public ActionResult Index()
        {
            return View();
        }
    }
}
