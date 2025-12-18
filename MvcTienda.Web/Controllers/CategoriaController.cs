using MvcTienda.Aplicacion.Categorias;
using System.Linq;
using System.Web.Mvc;

namespace MvcTienda.Web.Controllers
{
    // Opcional: Aplicar un filtro de autorización si solo los administradores pueden acceder
    // [Authorize(Roles = "Administrador")] 
    public class CategoriaController : Controller
    {
        private readonly ICategoriaService _categoriaService;

        // Inyección de Dependencias (Autofac ya sabe cómo crear ICategoriaService)
        public CategoriaController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        // GET: Categoria
        public ActionResult Index()
        {
            var categorias = _categoriaService.GetAllCategorias()
                                    .OrderBy(c => c.IdCategoria) // Orden ascendente por ID
                                    .ToList();

            return View(categorias);
        }

        // GET: Categoria/Create
        public ActionResult Create()
        {
            // Pasamos un modelo vacío para evitar el error de Model nulo
            return View(new CategoriaDto());
        }

        // POST: Categoria/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CategoriaDto categoriaDto)
        {
            if (ModelState.IsValid)
            {
                if (_categoriaService.CreateCategoria(categoriaDto))
                {
                    TempData["MensajeExito"] = "Categoría creada exitosamente.";
                    return RedirectToAction("Index");
                }
                // Si falla (ej: nombre duplicado, manejado en el servicio)
                ModelState.AddModelError("", "Ya existe una categoría con ese nombre.");
            }
            return View(categoriaDto);
        }

        // GET: Categoria/Edit/5
        public ActionResult Edit(int id)
        {
            var categoria = _categoriaService.GetCategoriaById(id);
            if (categoria == null)
            {
                return HttpNotFound();
            }
            return View(categoria);
        }

        // POST: Categoria/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CategoriaDto categoriaDto)
        {
            if (ModelState.IsValid)
            {
                if (_categoriaService.UpdateCategoria(categoriaDto))
                {
                    TempData["MensajeExito"] = "Categoría actualizada exitosamente.";
                    return RedirectToAction("Index");
                }
                // Si falla (ej: nombre duplicado)
                ModelState.AddModelError("", "Ya existe otra categoría con ese nombre.");
            }
            return View(categoriaDto);
        }

        // GET: Categoria/Delete/5
        // Se recomienda usar el patrón POST/Redirect/GET para evitar borrados accidentales
        public ActionResult Delete(int id)
        {
            var categoria = _categoriaService.GetCategoriaById(id);
            if (categoria == null)
            {
                return HttpNotFound();
            }
            return View(categoria);
        }

        // POST: Categoria/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _categoriaService.DeleteCategoria(id);
            TempData["MensajeExito"] = "Categoría eliminada exitosamente.";
            return RedirectToAction("Index");

            // Nota: En un sistema real, se debería manejar una excepción si la categoría tiene productos asociados.
        }
    }
}