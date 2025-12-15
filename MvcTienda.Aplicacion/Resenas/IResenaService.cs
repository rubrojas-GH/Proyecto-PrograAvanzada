using System.Collections.Generic;

namespace MvcTienda.Aplicacion.Resenas
{
    public interface IResenaService
    {
        // Métodos para Asociados (Escribir y Consultar)
        // Crear una nueva reseña (pendiente de aprobación)
        void CreateResena(ResenaDto dto, int idUsuario);
        IEnumerable<ResenaDto> GetResenasAprobadasByProducto(int idProducto);

        // Métodos para Administradores (Aprobación - RF4)
        IEnumerable<ResenaDto> GetResenasPendientes();
        void AprobarResena(int idResena);
        void RechazarResena(int idResena);
    }
}
