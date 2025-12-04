using System;
using ProyectoPrograAvanzada.Services.Interfaces;
using ProyectoPrograAvanzada.Repositories.Interfaces;
using ProyectoPrograAvanzada.Models;
using ProyectoPrograAvanzada.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoPrograAvanzada.Services
{
    public class UsuarioService : IUsuarioService
    {
        // Dependencia del Repositorio (Inyección de Dependencias)
        private readonly IUsuarioRepository _usuarioRepository;

        // Constructor para inyectar la dependencia del Repositorio
        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            // Inicialización de la dependencia.
            // En un proyecto real, esto se manejaría con un Contenedor IoC.
            _usuarioRepository = usuarioRepository;
        }

        public Usuario GetUsuarioByEmail(string email)
        {
            // El servicio llama al Repositorio para buscar en la base de datos
            return _usuarioRepository.GetUsuarioByEmail(email);
        }

        // Implementación del Registro de Usuario (RF1)
        public bool RegisterUser(Usuario model, string password)
        {
            // 1. Verificar si el email ya existe
            if (_usuarioRepository.GetUsuarioByEmail(model.email) != null)
            {
                return false; // El usuario ya existe
            }

            // 2. Cifrar la contraseña (RNF2)
            string hashedPassword = PasswordHasher.HashPassword(password);
            model.contrasena = hashedPassword;

            // 3. Asignar el rol por defecto (Asociado) (RF1)
            // Esto asume que el rol 'Asociado' existe en la DB.
            var rolAsociado = _usuarioRepository.GetRolByName("Asociado");

            if (rolAsociado == null)
            {
                // Manejar error si el rol no existe
                throw new InvalidOperationException("El rol 'Asociado' no fue encontrado en la base de datos.");
            }

            model.idRol = rolAsociado.idRol;

            // 4. Asignar propiedades iniciales
            model.estado = true;
            model.ultimaConexion = DateTime.Now;

            // 5. Guardar en la DB a través del Repositorio
            _usuarioRepository.AddUsuario(model);

            return true;
        }

        // Implementación del Inicio de Sesión (RF1 y RNF2)
        public Usuario LoginUser(string email, string password)
        {
            // 1. Cifrar la contraseña de entrada para comparar con la almacenada (RNF2)
            string hashedPassword = PasswordHasher.HashPassword(password);

            // 2. Intentar autenticar a través del Repositorio
            return _usuarioRepository.Authenticate(email, hashedPassword);
        }

        public Usuario GetUserById(int id)
        {
            return _usuarioRepository.GetUsuarioById(id);
        }

        public IEnumerable<Usuario> GetAllUsers()
        {
            return _usuarioRepository.GetAllUsuarios();
        }

        public void UpdateUser(Usuario usuario)
        {
            // NOTA: Aquí puedes añadir lógica de negocio, como asegurar que un Administrador 
            // no pueda eliminarse a sí mismo o cambiar su propio rol si es el único.
            _usuarioRepository.UpdateUsuario(usuario);
        }

        public void DeleteUser(int id)
        {
            _usuarioRepository.DeleteUsuario(id);
        }

        public IEnumerable<Rol> GetAllRoles()
        {
             return _usuarioRepository.GetAllRoles();

        }

        public bool RegisterNewAsociado(Usuario model)
        {
            // 1. Verificar si el email ya existe
            if (_usuarioRepository.GetUsuarioByEmail(model.email) != null)
            {
                return false; // El usuario ya existe
            }

            // 2. Cifrar la contraseña (RNF2)
            // CRÍTICO: El modelo 'Usuario' que viene de la vista ya debería tener la contraseña
            // ¡Ojo! El modelo que viene del controlador solo tiene la contraseña sin cifrar
            // si tu vista solo envía el texto plano. Asumiremos que el controlador te envía el texto plano.
            string plainPassword = model.contrasena; // Guardamos el texto plano
            model.contrasena = PasswordHasher.HashPassword(plainPassword); // Ciframos y asignamos

            // 3. Asignar el rol por defecto (Asociado) (RF1)
            var rolAsociado = _usuarioRepository.GetRolByName("Asociado");

            if (rolAsociado == null)
            {
                throw new InvalidOperationException("El rol 'Asociado' no fue encontrado en la base de datos.");
            }

            model.idRol = rolAsociado.idRol;

            // 4. Asignar propiedades iniciales
            model.estado = true;
            model.ultimaConexion = DateTime.Now;

            // 5. Guardar en la DB a través del Repositorio
            _usuarioRepository.AddUsuario(model);

            return true;
        }

        //ESTE METODO COMENTADO SERIA EL METODO PARA REGISTRAR UN ADMIN, ES EL MISMO QUE EL DE ASOCIADOS PERO HAY QUE IMPLEMENTARLO
        //SE CREO UN USUARIO ADMIN PARA PROBAR LAS FUNCIONALIDADES DE ADMIN
        //if (_usuarioRepository.GetUsuarioByEmail(model.email) != null)
            //{
              //  return false; // El usuario ya existe
            //}

            // 2. Cifrar la contraseña (RNF2)
            // CRÍTICO: El modelo 'Usuario' que viene de la vista ya debería tener la contraseña
            // ¡Ojo! El modelo que viene del controlador solo tiene la contraseña sin cifrar
            // si tu vista solo envía el texto plano. Asumiremos que el controlador te envía el texto plano.
            //tring plainPassword = model.contrasena; // Guardamos el texto plano
            //model.contrasena = PasswordHasher.HashPassword(plainPassword); // Ciframos y asignamos

            // 3. Asignar el rol por defecto (Asociado) (RF1)
            //var rolAsociado = _usuarioRepository.GetRolByName("Asociado");
            //var rolAdministrador = _usuarioRepository.GetRolByName("Administrador");

            //if (rolAdministrador == null)
            //{
                //throw new InvalidOperationException("El rol 'Administrador' no fue encontrado en la base de datos.");
            //}

            //model.idRol = rolAdministrador.idRol;

            // 4. Asignar propiedades iniciales
            //model.estado = true;
            //model.ultimaConexion = DateTime.Now;

            // 5. Guardar en la DB a través del Repositorio
            //_usuarioRepository.AddUsuario(model);

            //return true;
        //}

        // Implementación de ValidateCredentials (Soluciona el error de firma)
        // Se encarga de verificar la contraseña y retornar el usuario para la sesión.
        public Usuario ValidateCredentials(string email, string password)
        {
            // 1. Buscar el usuario por email
            var user = _usuarioRepository.GetUsuarioByEmail(email);

            if (user == null)
            {
                return null; // Usuario no encontrado
            }

            // 2. Verificar la contraseña cifrada (RNF2)
            if (PasswordHasher.VerifyPassword(password, user.contrasena))
            {
                // Actualizar la última conexión
                user.ultimaConexion = DateTime.Now;
                _usuarioRepository.UpdateUsuario(user);
                return user; // Credenciales válidas
            }

            return null; // Contraseña incorrecta
        }
    }
}