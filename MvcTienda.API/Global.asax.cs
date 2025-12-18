using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using MvcTienda.API.App_Start;

namespace MvcTienda.API
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            // MVC (aunque no lo usemos mucho)
            AreaRegistration.RegisterAllAreas();

            //  Web API
            GlobalConfiguration.Configure(WebApiConfig.Register);

            //  Autofac (DESPUÉS de Web API config)
            AutofacConfig.Register(GlobalConfiguration.Configuration);

            //  Filtros y rutas MVC
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
