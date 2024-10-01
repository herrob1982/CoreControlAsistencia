using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace Core.Encriptador
{
    public static class Cifrador
    {
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


        private static TripleDESCryptoServiceProvider proveedorDes = new TripleDESCryptoServiceProvider();
        private static UTF8Encoding codificacion = new UTF8Encoding();

        /// <summary>
        /// Convierte texto legible a texto codificado
        /// </summary>
        /// <param name="valor">cadena de caracteres a codificar</param>
        /// <returns>texto codificado</returns>
        public static string Encriptar(string valor)
        {
            byte[] entrada = codificacion.GetBytes(valor);
            proveedorDes.Mode = CipherMode.ECB;
            byte[] salida = Transformar(entrada, proveedorDes.CreateEncryptor(key3DES, iv3DES));
            return Convert.ToBase64String(salida);
        }

        public static string Encriptar(byte[] valor)
        {
            proveedorDes.Mode = CipherMode.ECB;
            byte[] salida = Transformar(valor, proveedorDes.CreateEncryptor(key3DES, iv3DES));
            return Convert.ToBase64String(salida);
        }

        /// <summary>
        /// Convierte texto codificado a texto legible
        /// </summary>
        /// <param name="valor">cadena de caracteres a descodificar</param>
        /// <returns>texto legible</returns>
        public static string Desencriptar(string valor)
        {
            byte[] entrada = Convert.FromBase64String(valor);
            proveedorDes.Mode = CipherMode.ECB;
            byte[] salida = Transformar(entrada, proveedorDes.CreateDecryptor(key3DES, iv3DES));
            return codificacion.GetString(salida);
        }

        public static byte[] DesencriptarToBytes(string valor)
        {
            byte[] entrada = Convert.FromBase64String(valor);
            proveedorDes.Mode = CipherMode.ECB;
            byte[] salida = Transformar(entrada, proveedorDes.CreateDecryptor(key3DES, iv3DES));
            return salida;
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

        /// <summary>
        /// Codifica a BCD los datos entregados
        /// </summary>
        /// <param name="datos"></param>
        /// <param name="rellenadoIzquierda"></param>
        /// <param name="desplazamiento">desplazamiento para inicio de codificación en el buffer</param>
        /// <param name="buffer"></param>
        public static void ToBcdEncode(string datos, bool rellenadoIzquierda, int desplazamiento, byte[] buffer)
        {
            int length = (datos.Length + 1) >> 1;
            // Initialize result bytes.
            for (int i = desplazamiento + length - 1; i >= desplazamiento; i--)
            {
                buffer[i] = 0;
            }

            int start = (((datos.Length & 1) == 1) && rellenadoIzquierda) ? 1 : 0;
            for (int i = start; i < (start + datos.Length); i++)
            {
                if (datos[i - start] < 0x40)
                {
                    buffer[desplazamiento + (i >> 1)] |= (byte)(((datos[i - start]) - 0x30) <<
                        ((i & 1) == 1 ? 0 : 4));
                }
                else
                {
                    buffer[desplazamiento + (i >> 1)] |= (byte)(((datos[i - start]) - 0x37) <<
                        ((i & 1) == 1 ? 0 : 4));
                }
            }
        }

        /// <summary>
        /// Encripta una caneda numérica 
        /// </summary>
        /// <param name="cadenaNumericaPlano">texto numérico plano</param>
        /// <param name="longitudSalt">longitud de bytes extras</param>
        /// <returns>cadena cifrada</returns>
        public static string EncriptarCadenaNumericaConSalt(string cadenaNumericaPlano, int longitudSalt)
        {
            byte[] random = new Byte[longitudSalt];
            byte[] claveBuffMascara = new byte[((cadenaNumericaPlano.Length + 1) >> 1) + random.Length];
            ToBcdEncode(cadenaNumericaPlano, true, random.Length, claveBuffMascara);

            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetNonZeroBytes(random);

            if ((cadenaNumericaPlano.Length & 1) == 1) //tiene relleno
                random[random.Length - 1] |= 0x01;
            else
                random[random.Length - 1] = (byte)(random[random.Length - 1] << 1);

            Array.Copy(random, claveBuffMascara, random.Length);
            byte[] bytesCifrados = Transformar(claveBuffMascara, proveedorDes.CreateEncryptor(key3DES, iv3DES));

            return Convert.ToBase64String(bytesCifrados);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="cadenaNumericaCifrado"></param>
        /// <param name="longitudSalt"></param>
        /// <returns></returns>
        public static string DesencriptarCadenaNumericaConSalt(string cadenaNumericaCifrado, int longitudSalt)
        {
            byte[] bytesCifrados = Convert.FromBase64String(cadenaNumericaCifrado);
            byte[] bytes = Transformar(bytesCifrados, proveedorDes.CreateDecryptor(key3DES, iv3DES));

            bool esRelleno = (byte)(bytes[longitudSalt - 1] << 7) == 0x80;

            string repuesta = ToBcdDecode((bytes.Length - longitudSalt) * 2 - (esRelleno ? 1 : 0), longitudSalt, esRelleno, bytes);
            return repuesta;
        }

        /// <summary>
        /// Decodifica una entrada BCD
        /// </summary>
        /// <param name="longitud">longitud a transaformar en el buffer</param>
        /// <param name="offset">desplazamiento</param>
        /// <param name="rellenadoIzquierda"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static string ToBcdDecode(int longitud, int offset, bool rellenadoIzquierda, byte[] buffer)
        {
            char[] result = new char[longitud];
            int start = (((longitud & 1) == 1) && rellenadoIzquierda) ? 1 : 0;

            for (int i = start; i < longitud + start; i++)
            {
                int shift = ((i & 1) == 1 ? 0 : 4);
                int c = ((buffer[offset + (i >> 1)] >> shift) & 0x0F);
                if (c < 10)
                {
                    c += 0x30;
                }
                else
                {
                    c += 0x37;
                }
                result[i - start] = (char)c;
            }
            return new string(result);
        }
    }
}
