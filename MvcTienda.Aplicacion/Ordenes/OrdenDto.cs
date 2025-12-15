using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MvcTienda.Aplicacion.Ordenes
{
    public class OrdenDto
    {
        public int Id { get; set; }

        [Required]
        public int IdUsuario { get; set; }

        public string NombreUsuario { get; set; }

        public List<OrdenItemDto> Items { get; set; } = new List<OrdenItemDto>();

        public DateTime Fecha { get; set; }

        [Range(0, 9999999)]
        public decimal Total { get; set; }

    }
}
