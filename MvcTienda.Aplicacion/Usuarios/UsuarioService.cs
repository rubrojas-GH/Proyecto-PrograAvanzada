using MvcTienda.Aplicacion.Common.Security;
using MvcTienda.Domain.Entities;
using MvcTienda.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;


namespace MvcTienda.Aplicacion.Usuarios
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        // ============================
        // REGISTRO
        // ============================
        public bool RegisterNewAsociado(UsuarioCreateDto dto)
        {
            if (_usuarioRepository.GetUsuarioByEmail(dto.Email) != null)
                return false;

            var rolAsociado = _usuarioRepository.GetRolByName("Asociado");
            if (rolAsociado == null)
                throw new InvalidOperationException("Rol 'Asociado' no existe.");

            var usuario = new Usuario
            {
                nombre = dto.Nombre,
                email = dto.Email,
                contrasena = PasswordHasher.HashPassword(dto.Password),
                idRol = rolAsociado.idRol,
                estado = true,
                ultimaConexion = DateTime.Now
            };

            _usuarioRepository.AddUsuario(usuario);
            return true;
        }

        // ============================
        // LOGIN
        // ============================
        public UsuarioDto ValidateCredentials(string email, string password)
        {
            var usuario = _usuarioRepository.GetUsuarioByEmail(email);
            if (usuario == null) return null;

            if (!PasswordHasher.VerifyPassword(password, usuario.contrasena))
                return null;

            usuario.ultimaConexion = DateTime.Now;
            _usuarioRepository.UpdateUsuario(usuario);

            return MapToDto(usuario);
        }

        // ============================
        // CONSULTAS
        // ============================
        public UsuarioDto GetUserById(int id)
        {
            var usuario = _usuarioRepository.GetUsuarioById(id);
            return usuario == null ? null : MapToDto(usuario);
        }

        public UsuarioDto GetUsuarioByEmail(string email)
        {
            var usuario = _usuarioRepository.GetUsuarioByEmail(email);
            return usuario == null ? null : MapToDto(usuario);
        }

        public IEnumerable<UsuarioDto> GetAllUsers()
        {
            return _usuarioRepository
                .GetAllUsuarios()
                .Select(MapToDto)
                .ToList();
        }

        // ============================
        // ADMIN
        // ============================
        public void UpdateUser(UsuarioDto dto)
        {
            var usuario = _usuarioRepository.GetUsuarioById(dto.Id);
            if (usuario == null)
                throw new InvalidOperationException("Usuario no encontrado.");

            var rolEntity = _usuarioRepository.GetRolByName(dto.Rol);
            if (rolEntity == null)
                throw new InvalidOperationException("Rol inválido.");

            usuario.idRol = rolEntity.idRol;
            usuario.estado = dto.Estado;

            _usuarioRepository.UpdateUsuario(usuario);
        }

        public void DeleteUser(int id)
        {
            _usuarioRepository.DeleteUsuario(id);
        }

        public IEnumerable<string> GetAllRoles()
        {
            return _usuarioRepository
                .GetAllRoles()
                .Select(r => r.nombreRol)
                .ToList();
        }

        // ============================
        // MAPPER PRIVADO
        // ============================
        private UsuarioDto MapToDto(Usuario usuario)
        {
            return new UsuarioDto
            {
                Id = usuario.idUsuario,
                Nombre = usuario.nombre,
                Email = usuario.email,
                Rol = usuario.Rol?.nombreRol,
                IdRol = usuario.idRol,
                Estado = usuario.estado,
                UltimaConexion = usuario.ultimaConexion,
                ContrasenaActual = usuario.contrasena
            };
        }
    }
}
