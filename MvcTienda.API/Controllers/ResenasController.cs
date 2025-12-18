using MvcTienda.Aplicacion.Resenas;
using System;
using System.Web.Http;

namespace MvcTienda.API.Controllers
{
    [RoutePrefix("api/resenas")]
    public class ResenasController : ApiController
    {
        private readonly IResenaService _resenaService;

        public ResenasController(IResenaService resenaService)
        {
            _resenaService = resenaService;
        }

        // PUT: api/resenas/approve/5
        [HttpPut]
        [Route("approve/{id}")]
        public IHttpActionResult Approve(int id)
        {
            try
            {
                _resenaService.AprobarResena(id);
                return Ok(new { message = "Reseña aprobada correctamente" });
            }
            catch (Exception ex)
            {
                // Si el servicio falla, devolvemos el error para que el alert del JS lo capture
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/resenas/reject/5
        // Nota: Usamos DELETE porque el servicio actualmente elimina el registro al rechazar
        [HttpDelete]
        [Route("reject/{id}")]
        public IHttpActionResult Reject(int id)
        {
            try
            {
                _resenaService.RechazarResena(id);
                return Ok(new { message = "Reseña eliminada correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}