using Core.Encriptador;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Core.Herramientas
{
    public static class ManejadorCorreos
    {
        static string UrlApiMailgun = ConfigurationManager.AppSettings["ApiMailgun"] != null ? ConfigurationManager.AppSettings["ApiMailgun"].ToString() : "";
        static string apiKeyMailgun = ConfigurationManager.AppSettings["keyMailgun"] != null ? ConfigurationManager.AppSettings["keyMailgun"].ToString() : "";
        static string domainMailgun = ConfigurationManager.AppSettings["domainMailgun"] != null ? ConfigurationManager.AppSettings["domainMailgun"].ToString() : "";
        static string from = ConfigurationManager.AppSettings["AdminUser"] != null ? CifradorSelf.DecryptKey(ConfigurationManager.AppSettings["AdminUser"].ToString()) : "";

        static string hostSMTP = ConfigurationManager.AppSettings["SMTPName"] != null ? CifradorSelf.DecryptKey(ConfigurationManager.AppSettings["SMTPName"].ToString()) : "";
        static string portSMTP = ConfigurationManager.AppSettings["SMTPPort"] != null ? CifradorSelf.DecryptKey(ConfigurationManager.AppSettings["SMTPPort"].ToString()) : "";
        static string userSMTP = ConfigurationManager.AppSettings["AdminUser"] != null ? CifradorSelf.DecryptKey(ConfigurationManager.AppSettings["AdminUser"].ToString()) : "";
        static string keySMTP = ConfigurationManager.AppSettings["keySMTP"] != null ? CifradorSelf.DecryptKey(ConfigurationManager.AppSettings["AdminPassword"].ToString()) : "";

        public static IRestResponse EnviarCorreoElectronico(string[] to, string subject, string text)
        {

            RestClient client = new RestClient();
            client.BaseUrl = new Uri(UrlApiMailgun);
            client.Authenticator = new HttpBasicAuthenticator("api", apiKeyMailgun);
            RestRequest request = new RestRequest();
            request.AddParameter("domain", domainMailgun, ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", from);
            foreach (string item in to)
            {
                request.AddParameter("to", item);
            }
            request.AddParameter("subject", subject);
            request.AddParameter("text", text);
            request.Method = Method.POST;
            return client.Execute(request);
        }

        public static IRestResponse EnviarCorreoElectronico(string[] to, string subject, string text, string html)
        {

            RestClient client = new RestClient();
            client.BaseUrl = new Uri(UrlApiMailgun);
            client.Authenticator = new HttpBasicAuthenticator("api", apiKeyMailgun);
            RestRequest request = new RestRequest();
            request.AddParameter("domain", domainMailgun, ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", from);
            foreach (string item in to)
            {
                request.AddParameter("to", item);
            }
            request.AddParameter("subject", subject);
            request.AddParameter("text", text);
            request.AddParameter("html", html);
            request.Method = Method.POST;
            return client.Execute(request);
        }


        public static IRestResponse EnviarCorreoElectronico(string[] to, string subject, string text, string html, string[] cc)
        {

            RestClient client = new RestClient();
            client.BaseUrl = new Uri(UrlApiMailgun);
            client.Authenticator = new HttpBasicAuthenticator("api", apiKeyMailgun);
            RestRequest request = new RestRequest();
            request.AddParameter("domain", domainMailgun, ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", from);
            foreach (string item in to)
            {
                request.AddParameter("to", item);
            }

            foreach (string item in cc)
            {
                request.AddParameter("cc", item);
            }
            request.AddParameter("subject", subject);
            request.AddParameter("text", text);
            request.AddParameter("html", html);
            request.Method = Method.POST;
            return client.Execute(request);
        }

        public static IRestResponse EnviarCorreoElectronico(string[] to, string subject, string text, string html, string[] cc, string[] bcc)
        {

            RestClient client = new RestClient();
            client.BaseUrl = new Uri(UrlApiMailgun);
            client.Authenticator = new HttpBasicAuthenticator("api", apiKeyMailgun);
            RestRequest request = new RestRequest();
            request.AddParameter("domain", domainMailgun, ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", from);
            foreach (string item in to)
            {
                request.AddParameter("to", item);
            }

            foreach (string item in cc)
            {
                request.AddParameter("cc", item);
            }

            foreach (string item in bcc)
            {
                request.AddParameter("bcc", item);
            }
            request.AddParameter("subject", subject);
            request.AddParameter("text", text);
            request.AddParameter("html", html);
            request.Method = Method.POST;
            return client.Execute(request);
        }

        public static IRestResponse EnviarCorreoElectronico(string[] to, string subject, string text, string html, string[] cc, string[] bcc, string[] files)
        {

            RestClient client = new RestClient();
            client.BaseUrl = new Uri(UrlApiMailgun);
            client.Authenticator = new HttpBasicAuthenticator("api", apiKeyMailgun);
            RestRequest request = new RestRequest();
            request.AddParameter("domain", domainMailgun, ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", from);
            foreach (string item in to)
            {
                request.AddParameter("to", item);
            }

            foreach (string item in cc)
            {
                request.AddParameter("cc", item);
            }

            foreach (string item in bcc)
            {
                request.AddParameter("bcc", item);
            }
            request.AddParameter("subject", subject);
            request.AddParameter("text", text);
            request.AddParameter("html", html);
            foreach (string item in files)
            {
                request.AddFile("attachment", Path.Combine("files", item));
            }
            request.Method = Method.POST;
            return client.Execute(request);
        }

        public static string CargarTemplateCorreo(List<string> datos, string nombreTemplate)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            string message = File.ReadAllText($"{path}\\{ConfigurationManager.AppSettings["EmailTemplates"]}\\{nombreTemplate}.html");
            for (int i = 0; i < datos.Count; i++)
                message = message.Replace("{" + i + "}", datos[i]);

            return message;
        }

        #region STMP
        public static void EnviarCorreoSMTP(string[] to, string subject, string html)
        {

            MailMessage objeto_mail = new MailMessage();
            SmtpClient client = new SmtpClient();
            client.Port = int.Parse(portSMTP);
            client.Host = hostSMTP;
            client.Timeout = 10000;
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(userSMTP, keySMTP);
            objeto_mail.From = new MailAddress(from);
            foreach (var item in to)
            {
                objeto_mail.To.Add(new MailAddress(item));
            }
            objeto_mail.Subject = subject;
            ContentType mimeType = new ContentType("text/html");
            AlternateView alternate = AlternateView.CreateAlternateViewFromString(html, mimeType);
            objeto_mail.AlternateViews.Add(alternate);

            //MemoryStream streamBitmap = new MemoryStream(firma);
            //var imageToInline = new LinkedResource(streamBitmap, MediaTypeNames.Image.Jpeg);
            //imageToInline.ContentId = "MyImage";
            //alternate.LinkedResources.Add(imageToInline);

            client.Send(objeto_mail);
        }

        public static void EnviarCorreoSMTP(string[] to, string subject, string html, string[] archivosAdjuntos)
        {

            MailMessage objeto_mail = new MailMessage();
            SmtpClient client = new SmtpClient();
            client.Port = int.Parse(portSMTP);
            client.Host = hostSMTP;
            client.Timeout = 10000;
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(userSMTP, keySMTP);
            objeto_mail.From = new MailAddress(from);
            foreach (var item in to)
            {
                objeto_mail.To.Add(new MailAddress(item));
            }
            objeto_mail.Subject = subject;
            ContentType mimeType = new ContentType("text/html");
            AlternateView alternate = AlternateView.CreateAlternateViewFromString(html, mimeType);
            objeto_mail.AlternateViews.Add(alternate);

            foreach (string archivo in archivosAdjuntos)
            {
                objeto_mail.Attachments.Add(new Attachment(archivo));
            }

            client.Send(objeto_mail);
        }

        public static void EnviarCorreoSMTP(string[] to, string subject, string html, List<Stream> archivosAdjuntos, string nombresArchivo)
        {

            MailMessage objeto_mail = new MailMessage();
            SmtpClient client = new SmtpClient();
            client.Port = int.Parse(portSMTP);
            client.Host = hostSMTP;
            client.Timeout = 10000;
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(userSMTP, keySMTP);
            objeto_mail.From = new MailAddress(from);
            foreach (var item in to)
            {
                objeto_mail.To.Add(new MailAddress(item));
            }
            objeto_mail.Subject = subject;
            ContentType mimeType = new ContentType("text/html");
            AlternateView alternate = AlternateView.CreateAlternateViewFromString(html, mimeType);
            objeto_mail.AlternateViews.Add(alternate);

            int idxArch = 0;
            foreach (Stream archivo in archivosAdjuntos)
            {
                objeto_mail.Attachments.Add(new Attachment(archivo, nombresArchivo.Split(',')[idxArch], MediaTypeNames.Text.Plain));
                idxArch++;
            }

            //MemoryStream streamBitmap = new MemoryStream(firma);
            //var imageToInline = new LinkedResource(streamBitmap, MediaTypeNames.Image.Jpeg);
            //imageToInline.ContentId = "MyImage";
            //alternate.LinkedResources.Add(imageToInline);

            client.Send(objeto_mail);
        }
        #endregion

    }
}
