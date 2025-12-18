using MvcTienda.Aplicacion.Dtos;

namespace MvcTienda.Aplicacion.Dashboard
{
    public interface IDashboardService
    {
        DashboardStatsDto ObtenerEstadisticas();
    }
}
