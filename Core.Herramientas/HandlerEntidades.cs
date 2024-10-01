using Core.Encriptador;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Herramientas
{
    public static class HandlerEntidades
    {
        static string urlApi = System.Configuration.ConfigurationManager.AppSettings["UrlApi"].ToString();

        public static List<T> ConsultarLista<T>(string controlador, string metodo, List<object> parametros) where T : class
        {
            List<T> data = new List<T>();
            InvocarApi invocar = new InvocarApi(urlApi);
            string datosApi = invocar.LlamarMetodo(controlador, metodo, parametros.Count > 0 ? parametros.ToArray() : null);
            data = JsonConvert.DeserializeObject<List<T>>(datosApi);
            return data;
        }

        public static T Consultar<T>(string controlador, string metodo, List<object> parametros)
        {
            RespuestaWeb rw = new RespuestaWeb();
            InvocarApi invocar = new InvocarApi(urlApi);

            string datosApi = invocar.LlamarMetodo(controlador, metodo, parametros.Count > 0 ? parametros.ToArray() : null);
            var d = JsonConvert.DeserializeObject<T>(datosApi);
            return d;
        }
    }
}
