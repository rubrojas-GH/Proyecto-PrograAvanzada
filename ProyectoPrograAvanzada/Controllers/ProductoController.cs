using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ProyectoPrograAvanzada.Models;
using ProyectoPrograAvanzada.Services.Interfaces;
using ProyectoPrograAvanzada.Repositories;
using ProyectoPrograAvanzada.Services;
using ProyectoPrograAvanzada.Repositories.Interfaces;
using System.Web;

namespace ProyectoPrograAvanzada.Controllers
{
    public class ProductoController : Controller
    {
        private readonly IProductoService _productoService;
        private readonly IProductoRepository _productoRepository;

        public ProductoController()
        {
            // Inicialización Manual de dependencias para el Servicio de Productos
            IProductoRepository repository = new ProductoRepository(); //  crear esta clase
            _productoService = new ProductoService(repository); //  crear esta clase
        }

        //
        // GET: /Producto
        // Muestra el catálogo de productos disponibles al público (RF2)
        [AllowAnonymous] // Permite el acceso a usuarios no logueados
        public ActionResult Index()
        {
            // Muestra solo productos activos
            var productos = _productoService.GetCatalogoProductosActivos();
            return View(productos);
        }

        //
        // GET: /Producto/Details/5
        // Muestra la ficha de un producto con sus detalles, imágenes y reseñas (RF2, RF4)
        [AllowAnonymous]
        public ActionResult Details(int id)
        {
            var producto = _productoService.GetProductoConDetalles(id);
            if (producto == null || producto.estadoProducto == false)
            {
                return HttpNotFound();
            }
            return View(producto);
        }

        // --- Operaciones CRUD para Administradores (RF2) ---

        // El resto de los métodos CRUD requieren ser Administrador
        [Authorize(Roles = "Administrador")]
        //
        // GET: /Producto/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Producto/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        // Se añade manejo de error de ArgumentException (stock negativo)
        public ActionResult Create(Producto producto, List<string> urlsImagenes)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _productoService.CreateProducto(producto, urlsImagenes);
                    TempData["SuccessMessage"] = $"El producto '{producto.nombreProducto}' ha sido creado exitosamente.";
                    return RedirectToAction("AdminIndex");
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Ocurrió un error desconocido al crear el producto.");
                }
            }
            return View(producto);
        }

        // GET: /Producto/AdminIndex
        // Muestra la lista COMPLETA de productos (activos e inactivos)
        [Authorize(Roles = "Administrador")]
        public ActionResult AdminIndex()
        {
            var productos = _productoRepository.GetAllProductos(); 
            return View(productos); 
        }

        // GET: /Producto/Edit/5
        [Authorize(Roles = "Administrador")]
        public ActionResult Edit(int id)
        {
            var producto = _productoService.GetProductoConDetalles(id);
            if (producto == null)
            {
                return HttpNotFound();
            }
            return View(producto);
        }

        // POST: /Producto/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult Edit(Producto producto)
        {
            if (ModelState.IsValid)
            {
                // El servicio maneja la lógica de negocio (ej. stock cero => inactivo)
                _productoService.UpdateProducto(producto);
                TempData["SuccessMessage"] = $"El producto '{producto.nombreProducto}' ha sido actualizado.";
                return RedirectToAction("AdminIndex");
            }
            return View(producto);
        }

        // POST: /Producto/Delete/5
        // Nota: Por convención de MVC, es mejor usar POST para la eliminación
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult Delete(int id)
        {
            _productoService.DeleteProducto(id);
            TempData["SuccessMessage"] = "Producto eliminado exitosamente del catálogo.";
            return RedirectToAction("AdminIndex");
        }
    }
}