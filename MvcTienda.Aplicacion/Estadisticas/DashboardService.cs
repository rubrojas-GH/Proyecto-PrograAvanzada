using MvcTienda.Aplicacion.Dtos;
using MvcTienda.Infraestructura.Data;
using System;
using System.Linq;

namespace MvcTienda.Aplicacion.Dashboard
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public DashboardStatsDto ObtenerEstadisticas()
        {
            var hoy = DateTime.Now;
            var hace7Dias = hoy.AddDays(-7);

            return new DashboardStatsDto
            {
                TotalUsuarios = _context.Usuarios.Count(),
                UsuariosActivos = _context.Usuarios.Count(u => u.estado),
                UsuariosInactivos = _context.Usuarios.Count(u => !u.estado),

                TotalProductos = _context.Productos.Count(),
                ProductosBajoInventario = _context.Productos.Count(p => p.stock < 5),

                VentasUltimaSemana = _context.Ordenes
                    .Where(o => o.fecha >= hace7Dias)
                    .Sum(o => (decimal?)o.total) ?? 0
            };
        }
    }
}
