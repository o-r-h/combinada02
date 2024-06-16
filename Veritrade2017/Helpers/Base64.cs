using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Veritrade2017.Helpers
{
    /// <summary>
    /// JANAQ 280520
    /// Clase que tiene las funciones que permiten codificar y decodificar una cadena
    /// </summary>
    public class Base64
    {
        /// <summary>
        /// Esta funcion permite codificar una cadena
        /// </summary>
        /// <param name="plainText">Parametro a codificar</param>
        /// <returns></returns>
        public static string encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        /// Esta funcion permite decodificar un dato codificado en base 64
        /// </summary>
        /// <param name="base64EncodedData">Parmetro a decodificar</param>
        /// <returns></returns>
        public static string decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

    }
}