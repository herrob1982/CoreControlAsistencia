using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Core.Encriptador
{
    public static class Cifrador3DES
    {
        private static TripleDESCryptoServiceProvider proveedorDes = new TripleDESCryptoServiceProvider();
        private static UTF8Encoding codificacion = new UTF8Encoding();

        // LLaves para la encripción de 3DES.
        // OJO no cambiar NUNCA
        /// <summary>
        /// Vector de claves
        /// </summary>
        public static byte[] key3DES = { 245, 87, 124, 4, 123, 198, 122, 12, 71, 15, 134, 220, 59, 62, 131, 187, 76, 243, 65, 156, 191, 171, 114, 189 };
        /// <summary>
        /// Segundo vector de claves
        /// </summary>
        public static byte[] iv3DES = { 62, 81, 92, 156, 178, 142, 221, 199 };

        /// <summary>
        /// Convierte texto legible a texto codificado
        /// </summary>
        /// <param name="valor">cadena de caracteres a codificar</param>
        /// <param name="clave">clave de encriptación</param>
        /// <param name="vector">vector de encriptación</param>
        /// <returns>texto codificado</returns>
        public static string Encriptar(string valor)
        {
            byte[] entrada = codificacion.GetBytes(valor);
            byte[] salida = Transformar(entrada, proveedorDes.CreateEncryptor(key3DES, iv3DES));
            return Convert.ToBase64String(salida);
        }

        /// <summary>
        /// Convierte texto codificado a texto legible
        /// </summary>
        /// <param name="valor">cadena de caracteres a descodificar</param>
        /// <param name="clave">clave de encriptacion</param>
        /// <param name="vector">vector de encriptacion</param>
        /// <returns>texto legible</returns>
        public static string Desencriptar(string valor)
        {
            byte[] entrada = Convert.FromBase64String(valor);
            byte[] salida = Transformar(entrada, proveedorDes.CreateDecryptor(key3DES, iv3DES));
            return codificacion.GetString(salida);
        }

        /// <summary>
        /// Tranforma la información recuperada en información legible
        /// </summary>
        /// <param name="entrada">arreglo de bytes a transformar</param>
        /// <param name="transformacion">objeto transformador</param>
        /// <returns>arreglo de bytes transformado</returns>
        private static byte[] Transformar(byte[] entrada, ICryptoTransform transformacion)
        {
            // Create the necessary streams
            MemoryStream memoria = new MemoryStream();
            CryptoStream flujo = new CryptoStream(memoria, transformacion, CryptoStreamMode.Write);
            // Transform the bytes as requesed
            flujo.Write(entrada, 0, entrada.Length);
            flujo.FlushFinalBlock();
            // Read the memory stream and convert it back into byte array
            memoria.Position = 0;
            byte[] resultado = new byte[memoria.Length];
            memoria.Read(resultado, 0, resultado.Length);
            memoria.Close();
            flujo.Close();
            return resultado;
        }
    }
}
