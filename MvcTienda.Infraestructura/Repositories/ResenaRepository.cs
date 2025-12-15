using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using MvcTienda.Domain.Entities;
using MvcTienda.Domain.Repositories;
using MvcTienda.Infraestructura.Data;

namespace MvcTienda.Infraestructura.Repositories
{
    public class ResenaRepository : IResenaRepository
    {
        private readonly ApplicationDbContext _db;

        // Constantes para evitar strings mágicos
        private const string ESTADO_PENDIENTE = "Pendiente";
        private const string ESTADO_APROBADA = "Aprobada";

        // El DbContext se inyecta para reducir acoplamiento
        public ResenaRepository(ApplicationDbContext context)
        {
            _db = context;
        }

        // --- Consultas ---

        public Resena GetResenaById(int id)
        {
            return _db.Resenas
                       .Include(r => r.Usuario)   // Incluye el usuario que escribió la reseña
                       .Include(r => r.Producto)  // Incluye el producto reseñado
                       .FirstOrDefault(r => r.idResena == id);
        }

        public IEnumerable<Resena> GetResenasPendientes()
        {
            // Regla funcional (RF): mostrar solo reseñas en estado 'Pendiente' al administrador
            return _db.Resenas
                      .Include(r => r.Usuario)
                      .Include(r => r.Producto)
                      .Where(r => r.estado == ESTADO_PENDIENTE)
                      .ToList();
        }

        public IEnumerable<Resena> GetResenasAprobadasByProducto(int idProducto)
        {
            // Regla funcional (RF): solo mostrar reseñas 'Aprobadas' al público
            return _db.Resenas
                      .Include(r => r.Usuario)
                      .Where(r => r.idProducto == idProducto && r.estado == ESTADO_APROBADA)
                      .OrderByDescending(r => r.fecha)
                      .ToList();
        }

        // --- Escritura ---

        public void AddResena(Resena resena)
        {
            // Regla funcional (RF): toda reseña nueva inicia en estado 'Pendiente'
            resena.fecha = DateTime.Now;
            resena.estado = ESTADO_PENDIENTE;

            _db.Resenas.Add(resena);
            _db.SaveChanges();
        }

        public void UpdateResena(Resena resena)
        {
            // Usado principalmente para cambiar el campo 'estado'
            var existingResena = _db.Resenas.Find(resena.idResena);
            if (existingResena != null)
            {
                existingResena.estado = resena.estado;
                _db.SaveChanges();
            }
        }

        public void DeleteResena(int id)
        {
            var resena = _db.Resenas.Find(id);
            if (resena != null)
            {
                _db.Resenas.Remove(resena);
                _db.SaveChanges();
            }
        }
    }
}
