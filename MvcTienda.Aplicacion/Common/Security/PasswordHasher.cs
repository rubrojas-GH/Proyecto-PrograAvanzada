using System;
using System.Security.Cryptography;

namespace MvcTienda.Aplicacion.Common.Security
{
    public static class PasswordHasher
    {
        // === Parámetros de Seguridad ===
        private const int SaltSize = 16; // 128 bits
        private const int KeySize = 20;  // 160 bits
        private const int Iterations = 10000; // Mínimo recomendado por NIST/OWASP

        // === Posiciones en la cadena de hash almacenada (Base64) ===
        private static readonly int HashIndex = SaltSize;
        private static readonly int FullHashSize = SaltSize + KeySize;

        // Función para cifrar (hashear) una contraseña usando PBKDF2
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException(nameof(password), "La contraseña no puede ser nula o vacía.");
            }

            // 1. Generar Salt aleatorio (seguro para cada usuario)
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] salt = new byte[SaltSize];
                rng.GetBytes(salt);

                // 2. Generar Hash lento (Key Stretching)
                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
                {
                    byte[] hash = pbkdf2.GetBytes(KeySize);

                    // 3. Combinar Salt y Hash para almacenar (Salt + Hash)
                    byte[] fullHash = new byte[FullHashSize];

                    // Copiar Salt al inicio
                    Buffer.BlockCopy(salt, 0, fullHash, 0, SaltSize);
                    // Copiar Hash después del Salt
                    Buffer.BlockCopy(hash, 0, fullHash, SaltSize, KeySize);

                    // Devolver la combinación codificada en Base64 para almacenarla como string
                    return Convert.ToBase64String(fullHash);
                }
            }
        }

        // Función para verificar una contraseña contra el hash almacenado
        public static bool VerifyPassword(string password, string storedHash)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedHash))
            {
                return false;
            }

            try
            {
                // 1. Decodificar la cadena Base64 (que contiene Salt + Hash)
                byte[] fullHashBytes = Convert.FromBase64String(storedHash);
                if (fullHashBytes.Length != FullHashSize) return false;

                // 2. Separar el Salt y el Hash Almacenado
                byte[] salt = new byte[SaltSize];
                byte[] storedKey = new byte[KeySize];

                Buffer.BlockCopy(fullHashBytes, 0, salt, 0, SaltSize); // Extraer el Salt
                Buffer.BlockCopy(fullHashBytes, HashIndex, storedKey, 0, KeySize); // Extraer el Hash

                // 3. Generar un nuevo Hash usando el mismo Salt y la contraseña de entrada
                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
                {
                    byte[] enteredKey = pbkdf2.GetBytes(KeySize);

                    // 4. Comparar el Hash recién generado con el Hash Almacenado
                    return ConstantTimeComparison(enteredKey, storedKey);
                }
            }
            catch
            {
                // Manejar errores de formato (ej. Base64 inválido)
                return false;
            }
        }

        // Método de comparación de tiempo constante para evitar ataques de temporización
        private static bool ConstantTimeComparison(byte[] a, byte[] b)
        {
            uint diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
            {
                diff |= (uint)(a[i] ^ b[i]);
            }
            return diff == 0;
        }
    }
}