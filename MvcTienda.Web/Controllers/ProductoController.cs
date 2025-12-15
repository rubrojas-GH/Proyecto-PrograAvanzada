using MvcTienda.Aplicacion.Productos;
using MvcTienda.Aplicacion.Categorias;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;

namespace MvcTienda.Web.Controllers
{
    public class ProductoController : Controller
    {
        private readonly IProductoService _productoService;
        private readonly ICategoriaService _categoriaService;

        // Inyección de dependencias (Autofac inyectará ambos servicios)
        public ProductoController(IProductoService productoService, ICategoriaService categoriaService) // 🟢 Recibe ICategoriaService
        {
            _productoService = productoService;
            _categoriaService = categoriaService;
        }

        // 🟢 Método auxiliar para cargar categorías en un SelectList
        private void CargarCategoriasEnViewBag()
        {
            // Obtener todas las categorías
            var categorias = _categoriaService.GetAllCategorias();

            // Convertir la lista de CategoríaDto a SelectListItem (necesario para el DropDownList en la vista)
            ViewBag.CategoriaList = new SelectList(
                categorias.Select(c => new SelectListItem
                {
                    Value = c.IdCategoria.ToString(), // El ID es el valor que se guarda
                    Text = c.NombreCategoria         // El Nombre es lo que ve el usuario
                }),
                "Value",
                "Text"
            );
        }

        // =========================
        // CATÁLOGO PÚBLICO
        // =========================

        // GET: /Producto
        // Muestra el catálogo de productos disponibles al público (RF2)
        [AllowAnonymous] // Permite el acceso a usuarios no logueados
        public ActionResult Index()
        {
            // Muestra solo productos activos
            var productos = _productoService.GetCatalogoProductosActivos();
            return View(productos);
        }

        // GET: /Producto/Details/5
        // Muestra la ficha de un producto con sus detalles, imágenes y resenas (RF2, RF4)
        [AllowAnonymous]
        public ActionResult Details(int id)
        {
            var producto = _productoService.GetProductoConDetalles(id);
            if (producto == null)
            {
                return HttpNotFound();
            }
            return View(producto);
        }

        // =========================
        // CRUD ADMINISTRADOR
        // =========================
        // --- Operaciones CRUD para Administradores (RF2) ---

        // GET: /Producto/AdminIndex
        // Muestra la lista COMPLETA de productos (activos e inactivos)
        [Authorize(Roles = "Administrador")]
        public ActionResult AdminIndex()
        {
            var productos = _productoService.GetAllProductosAdmin();
            return View(productos);
        }

        // El resto de los métodos CRUD requieren ser Administrador
        // GET: /Producto/Create
        [Authorize(Roles = "Administrador")]
        public ActionResult Create()
        {
            CargarCategoriasEnViewBag();
            return View();
        }

        //
        // POST: /Producto/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult Create(ProductoDto productoDto, List<string> urlsImagenes)
        {
            if (!ModelState.IsValid)
            {
                CargarCategoriasEnViewBag(); // Recargar DropDownList en caso de error
                return View(productoDto);
            }

            try
            {
                _productoService.CreateProducto(productoDto, urlsImagenes);
                TempData["SuccessMessage"] = "Producto creado correctamente.";
                return RedirectToAction(nameof(AdminIndex));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                CargarCategoriasEnViewBag(); // Recargar DropDownList en caso de error
                return View(productoDto);
            }
        }


        // GET: /Producto/Edit/5
        [Authorize(Roles = "Administrador")]
        public ActionResult Edit(int id)
        {
            var producto = _productoService.GetProductoConDetalles(id);

            if (producto == null)
                return HttpNotFound();

            CargarCategoriasEnViewBag();
            ((SelectList)ViewBag.CategoriaList).Where(s => s.Value == producto.IdCategoria.ToString()).First().Selected = true;

            return View(producto);
        }

        // POST: /Producto/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult Edit(ProductoDto productoDto)
        {
            if (!ModelState.IsValid)
            {
                CargarCategoriasEnViewBag(); // Recargar DropDownList en caso de error
                return View(productoDto);
            }

            _productoService.UpdateProducto(productoDto);
            TempData["SuccessMessage"] = "Producto actualizado correctamente.";
            return RedirectToAction(nameof(AdminIndex));
        }

        // POST: /Producto/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult Delete(int id)
        {
            _productoService.DeleteProducto(id);
            TempData["SuccessMessage"] = "Producto eliminado correctamente.";
            return RedirectToAction(nameof(AdminIndex));
        }
    }
}