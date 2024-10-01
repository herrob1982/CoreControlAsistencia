using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Core.Herramientas
{
    public class InvocarApi
    {
        public string URL = "";
        public InvocarApi(string url)
        {
            URL = url;
        }

        public string LlamarMetodo(string controlador, string metodo, object[] parametros = null, Enumerados.MetodoApi metodoApi = Enumerados.MetodoApi.GET)
        {
            try
            {
                string dataObjects = "";
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(URL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = new HttpResponseMessage();
                switch (metodoApi)
                {
                    case Enumerados.MetodoApi.GET:
                        if (parametros == null)
                            response = client.GetAsync(string.Format("api/{0}/{1}", controlador, metodo)).Result;
                        else
                        {
                            string url = string.Format("api/{0}/{1}", controlador, metodo);
                            parametros.ToList().ForEach(x =>
                            {
                                url += "/" + x.ToString();
                            });
                            response = client.GetAsync(url).Result;
                        }
                        break;
                    case Enumerados.MetodoApi.POST:
                        HttpContent content = new StringContent(parametros[0].ToString(), UTF8Encoding.UTF8, "application/json");
                        response = client.PostAsync(string.Format("api/{0}/{1}", controlador, metodo), content).Result;
                        break;
                    default:
                        break;
                }
                if (response.IsSuccessStatusCode)
                    dataObjects = response.Content.ReadAsStringAsync().Result;
                else
                {
                    RespuestaWeb respuesta = new RespuestaWeb();
                    respuesta.MensajeError = "No se encuentra el API";
                    respuesta.CodigoMensajeError = "NOTFOUND";
                    respuesta.CodigoTransaccion = metodo;
                    dataObjects = JsonConvert.SerializeObject(respuesta);
                }

                client.Dispose();

                return dataObjects;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }

    internal class RespuestaWeb
    {
        public string MensajeError { get; set; }
        public string CodigoMensajeError { get; set; }
        public string CodigoTransaccion { get; set; }
    }
}
