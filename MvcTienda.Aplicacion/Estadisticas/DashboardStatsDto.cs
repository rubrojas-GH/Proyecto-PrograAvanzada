namespace MvcTienda.Aplicacion.Dtos
{
    public class DashboardStatsDto
    {
        public int TotalUsuarios { get; set; }
        public int UsuariosActivos { get; set; }
        public int UsuariosInactivos { get; set; }

        public int TotalProductos { get; set; }
        public int ProductosBajoInventario { get; set; }

        public decimal VentasUltimaSemana { get; set; }
    }
}
