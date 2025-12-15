using MvcTienda.Domain.Entities;
using MvcTienda.Domain.Repositories;
using MvcTienda.Infraestructura.Data;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace MvcTienda.Infraestructura.Repositories
{
    public class CategoriaRepository : ICategoriaRepository
    {
        // El ApplicationDbContext es inyectado y gestionado por Autofac (Unit of Work)
        private readonly ApplicationDbContext _context;

        // Inyección de Dependencias
        public CategoriaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Categoria GetById(int id)
        {
            return _context.Categorias.Find(id);
        }

        public IEnumerable<Categoria> GetAll()
        {
            // Ordenar por nombre para listados amigables
            return _context.Categorias.OrderBy(c => c.nombreCategoria).ToList();
        }

        public void Add(Categoria categoria)
        {
            _context.Categorias.Add(categoria);
            _context.SaveChanges(); // Guardamos inmediatamente (depende de la estrategia de Unit of Work)
        }

        public void Update(Categoria categoria)
        {
            _context.Entry(categoria).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var categoria = GetById(id);
            if (categoria != null)
            {
                _context.Categorias.Remove(categoria);
                _context.SaveChanges();
            }
        }

        public bool Exists(string nombre)
        {
            // Usamos StringComparison.OrdinalIgnoreCase para una búsqueda sin distinción entre mayúsculas y minúsculas
            return _context.Categorias.Any(c => c.nombreCategoria.Equals(nombre, System.StringComparison.OrdinalIgnoreCase));
        }
    }
}