using System;
using ProyectoPrograAvanzada.Repositories.Interfaces;
using ProyectoPrograAvanzada.Models;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;

namespace ProyectoPrograAvanzada.Repositories
{
    public class ReseñaRepository : IReseñaRepository
    {
        private readonly ApplicationDbContext _db;

        public ReseñaRepository()
        {
            _db = new ApplicationDbContext();
        }

        // --- Consultas ---

        public Reseña GetReseñaById(int id)
        {
            return _db.Reseñas
                       .Include(r => r.Usuario) // Incluye el nombre del usuario
                       .Include(r => r.Producto) // Incluye el nombre del producto
                       .FirstOrDefault(r => r.idReseña == id);
        }

        public IEnumerable<Reseña> GetReseñasPendientes()
        {
            // Lógica RF4: Estado 'Pendiente'
            return _db.Reseñas.Include(r => r.Usuario)
                              .Include(r => r.Producto)
                              .Where(r => r.estado == "Pendiente")
                              .ToList();
        }

        public IEnumerable<Reseña> GetReseñasAprobadasByProducto(int idProducto)
        {
            // Lógica RF4: Solo mostrar reseñas 'Aprobadas'
            return _db.Reseñas.Include(r => r.Usuario)
                              .Where(r => r.idProducto == idProducto && r.estado == "Aprobada")
                              .OrderByDescending(r => r.fecha)
                              .ToList();
        }

        // --- Escritura ---

        public void AddReseña(Reseña reseña)
        {
            // Lógica RF4: Toda reseña nueva inicia en estado 'Pendiente'
            reseña.fecha = DateTime.Now;
            reseña.estado = "Pendiente";
            _db.Reseñas.Add(reseña);
            _db.SaveChanges();
        }

        public void UpdateReseña(Reseña reseña)
        {
            // Usado principalmente para cambiar el campo 'estado'
            _db.Entry(reseña).State = EntityState.Modified;
            _db.SaveChanges();
        }

        public void DeleteReseña(int id)
        {
            var reseña = _db.Reseñas.Find(id);
            if (reseña != null)
            {
                _db.Reseñas.Remove(reseña);
                _db.SaveChanges();
            }
        }
    }
}