using Autofac;
using Autofac.Integration.Mvc;
using MvcTienda.Aplicacion.Categorias;
using MvcTienda.Aplicacion.Dashboard;
using MvcTienda.Aplicacion.Ordenes;
using MvcTienda.Aplicacion.Productos;
using MvcTienda.Aplicacion.Resenas;
using MvcTienda.Aplicacion.Roles;
// Capa de Aplicación (Servicios)
using MvcTienda.Aplicacion.Usuarios;
// Capa de Dominio (Interfaces)
using MvcTienda.Domain.Repositories;
// Capa de Infraestructura (Data Access)
using MvcTienda.Infraestructura.Data;
using MvcTienda.Infraestructura.Repositories;
using System.Web.Mvc;

namespace MvcTienda.Web
{
    public static class ContainerConfig
    {
        public static void ConfigureContainer()
        {
            var builder = new ContainerBuilder();

            // 1. Registrar Controladores de MVC
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            // 2. Registrar el DbContext (Unit of Work)
            builder.RegisterType<ApplicationDbContext>().AsSelf().InstancePerRequest();


            // 3. Registro de Repositorios (Interfaces y sus Implementaciones)
            builder.RegisterType<UsuarioRepository>().As<IUsuarioRepository>().InstancePerRequest();
            builder.RegisterType<OrdenRepository>().As<IOrdenRepository>().InstancePerRequest();
            builder.RegisterType<RolRepository>().As<IRolRepository>().InstancePerRequest();

            // Repositorios Actualizados/Nuevos
            builder.RegisterType<ProductoRepository>().As<IProductoRepository>().InstancePerRequest();
            builder.RegisterType<ResenaRepository>().As<IResenaRepository>().InstancePerRequest();
            builder.RegisterType<CategoriaRepository>().As<ICategoriaRepository>().InstancePerRequest();

            // 4. Registro de Servicios de Aplicación (Interfaces y sus Implementaciones)
            builder.RegisterType<UsuarioService>().As<IUsuarioService>().InstancePerRequest();
            builder.RegisterType<OrdenService>().As<IOrdenService>().InstancePerRequest();
            builder.RegisterType<RolService>().As<IRolService>().InstancePerRequest();

            // Servicios Actualizados/Nuevos
            builder.RegisterType<ResenaService>().As<IResenaService>().InstancePerRequest();

            builder.RegisterType<ProductoService>().As<IProductoService>().InstancePerRequest();
            builder.RegisterType<DashboardService>().As<IDashboardService>().InstancePerRequest();
            builder.RegisterType<CategoriaService>().As<ICategoriaService>().InstancePerRequest();

            // 5. Configurar el Dependency Resolver
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}