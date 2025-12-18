using System.Web.Http;
using System.Web.Http.Cors;
using Newtonsoft.Json;

namespace MvcTienda.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // =========================
            // HABILITAR CORS (Web API clásico)
            // =========================
            var cors = new System.Web.Http.Cors.EnableCorsAttribute(
                origins: "*",
                headers: "*",
                methods: "*"
            );
            config.EnableCors(cors);

            // =========================
            // RUTAS WEB API
            // =========================
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // =========================
            // FORZAR JSON
            // =========================
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            var json = config.Formatters.JsonFormatter;
            json.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            json.SerializerSettings.ContractResolver =
                new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();

            // =========================
            // INICIALIZAR
            // =========================
            config.EnsureInitialized();
        }
    }
}
