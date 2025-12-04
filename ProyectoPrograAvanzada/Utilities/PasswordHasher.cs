using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace ProyectoPrograAvanzada.Utilities
{
    public static class PasswordHasher
    {
        // NOTA: Para proyectos modernos, BCrypt.Net o Argon2 son más seguros.
        // Usamos SHA256 como un ejemplo básico y compatible con .NET Framework.

        // Función para cifrar (hashear) una contraseña
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return null;
            }

            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Convertir la entrada de string a un array de bytes
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convertir el array de bytes a una cadena hexadecimal
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // Función para verificar una contraseña contra un hash
        public static bool VerifyPassword(string password, string storedHash)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedHash))
            {
                return false;
            }

            string hashOfInput = HashPassword(password);

            // Comparación de cadenas para verificar
            return StringComparer.OrdinalIgnoreCase.Compare(hashOfInput, storedHash) == 0;
        }
    }
}