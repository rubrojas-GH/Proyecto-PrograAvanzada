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
        public ProductoController(IProductoService productoService, ICategoriaService categoriaService) // Recibe ICategoriaService
        {
            _productoService = productoService;
            _categoriaService = categoriaService;
        }

        // 🟢 Método auxiliar para cargar categorías en un SelectList (usado en Create/Edit)
        private void CargarCategoriasEnViewBag()
        {
            var categorias = _categoriaService.GetAllCategorias();
            // "IdCategoria" es el DataValueField y "NombreCategoria" es el DataTextField
            ViewBag.CategoriaList = new SelectList(categorias, "IdCategoria", "NombreCategoria");
        }

        // =========================
        // CATÁLOGO PÚBLICO
        // =========================

        // GET: /Producto
        // Muestra el catálogo de productos disponibles al público (RF2)
        [AllowAnonymous]
        public ActionResult Index(int? categoriaId)
        {
            // 1. Cargar las categorías para el menú de navegación (la barra lateral)
            ViewBag.Categorias = _categoriaService.GetAllCategorias();

            // 2. Guardar el ID actual para que la vista sepa cuál resaltar
            ViewBag.CategoriaActualId = categoriaId;

            // 3. Llamar al servicio. 
            // Si categoriaId es null, el servicio traerá TODOS los activos.
            // Si tiene un valor (ej. 5), el servicio filtrará por esa categoría.
            var productos = _productoService.GetCatalogoProductosActivos(categoriaId);

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

            // En lugar de llamar a CargarCategoriasEnViewBag(), hacemos el SelectList aquí
            // para poder pasarle el 'IdCategoria' como valor seleccionado directamente.
            var categorias = _categoriaService.GetAllCategorias();
            ViewBag.CategoriaList = new SelectList(categorias, "IdCategoria", "NombreCategoria", producto.IdCategoria);

            return View(producto);
        }

        // POST: /Producto/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult Edit(ProductoDto productoDto)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

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