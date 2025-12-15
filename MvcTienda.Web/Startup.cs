using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MvcTienda.Web.Startup))]
namespace MvcTienda.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
