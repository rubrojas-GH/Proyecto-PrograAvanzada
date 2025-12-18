using Autofac;
using Autofac.Integration.WebApi;
using MvcTienda.Aplicacion.Categorias;
using MvcTienda.Aplicacion.Dashboard;
using MvcTienda.Aplicacion.Ordenes;
using MvcTienda.Aplicacion.Productos;
using MvcTienda.Aplicacion.Resenas;
using MvcTienda.Aplicacion.Roles;
using MvcTienda.Aplicacion.Usuarios;
using MvcTienda.Domain.Repositories;
using MvcTienda.Infraestructura.Data;
using MvcTienda.Infraestructura.Repositories;
using System.Reflection;
using System.Web.Http;

namespace MvcTienda.API.App_Start
{
    public static class AutofacConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var builder = new ContainerBuilder();

            // Registrar controladores Web API
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            // DbContext
            builder.RegisterType<ApplicationDbContext>()
                   .AsSelf()
                   .InstancePerRequest();

            // Registrar tus servicios (Capa Aplicación)
            builder.RegisterType<UsuarioService>().As<IUsuarioService>().InstancePerRequest();
            builder.RegisterType<ProductoService>().As<IProductoService>().InstancePerRequest();
            builder.RegisterType<CategoriaService>().As<ICategoriaService>().InstancePerRequest();
            builder.RegisterType<OrdenService>().As<IOrdenService>().InstancePerRequest();
            builder.RegisterType<ResenaService>().As<IResenaService>().InstancePerRequest();
            builder.RegisterType<RolService>().As<IRolService>().InstancePerRequest();

            // Registrar Repositorios y Contexto (Capa Infraestructura)
            builder.RegisterType<UsuarioRepository>().As<IUsuarioRepository>().InstancePerRequest();
            builder.RegisterType<ProductoRepository>().As<IProductoRepository>().InstancePerRequest();
            builder.RegisterType<CategoriaRepository>().As<ICategoriaRepository>().InstancePerRequest();
            builder.RegisterType<OrdenRepository>().As<IOrdenRepository>().InstancePerRequest();
            builder.RegisterType<ResenaRepository>().As<IResenaRepository>().InstancePerRequest();
            builder.RegisterType<RolRepository>().As<IRolRepository>().InstancePerRequest();

            // Servicios de aplicación
            builder.RegisterType<DashboardService>()
                   .As<IDashboardService>()
                   .InstancePerRequest();

            // 🔹 Más adelante registraremos:
            // IProductoService, IResenaService, etc.

            var container = builder.Build();

            config.DependencyResolver =
                new AutofacWebApiDependencyResolver(container);
        }
    }
}
