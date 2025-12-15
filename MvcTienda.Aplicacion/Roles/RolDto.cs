using System.ComponentModel.DataAnnotations;

namespace MvcTienda.Aplicacion.Roles
{
    public class RolDto
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string Nombre { get; set; }
    }
}
