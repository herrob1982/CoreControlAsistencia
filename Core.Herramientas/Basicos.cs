using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelLibrary;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Data.OleDb;
using System.Configuration;
using System.IO;
using Core.Dto;

namespace Core.Herramientas
{
    public static class Basicos
    {
        private static string[] palabras = {" I "," E ", " U ", " O ", " A ", " DE ", " CON ", " Y ",
                                            " POR ", " DEL ", " LA ", " LOS ", " EL ", " PARA ",
                                            " SIN ", " NO "," LO ", " SI ", " ES ", " EL ", " TRAS ",
                                            " EN ", " ENTRE ", " HACIA ", " CABE ", " SO "
                                            };

        public static string GenerarClave()
        {
            string caracteres = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,0,1,2,3,4,5,6,7,8,9,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z";
            Random randon = new Random();
            int indiceCaracter = 0;
            string claveGenerada = "#";
            int longitudCaracteres = caracteres.Split(',').Length;
            for (int i = 0; i < 8; i++)
            {
                indiceCaracter = randon.Next(longitudCaracteres);
                claveGenerada = claveGenerada + caracteres.Split(',')[indiceCaracter];
            }
            return claveGenerada;
        }

        public static string GenerarCodigoNumerico(int cantidadDigitos)
        {
            string codigo = "";
            Random rnd = new Random();
            for (int i = 0; i < cantidadDigitos; i++)
            {
                int num = rnd.Next(0, 10);
                codigo += num.ToString();
            }

            return codigo;
        }

        public static string GenerarUserName(string primerNombre, string primerApellido)
        {
            string userName = "";
            userName = userName + primerApellido;
            userName = userName + primerNombre.Substring(0, 2);
            return userName;
        }

        public static string GenerarCodigo(string nombre)
        {
            string codigo = "";

            for (int x = 0; x < palabras.Count(); x++)
            {
                nombre = nombre.Replace(palabras[x], " ");
            }

            if (nombre != "" && nombre.Length > 3)
                codigo = nombre.Substring(0, 3);
            else
                codigo = nombre;

            if (nombre.Length > 3)
            {
                for (int x = 0; x < nombre.Length; x++)
                {
                    if (nombre.Substring(x, 1) == " ")
                    {
                        if (((nombre.Length) - x > 3))
                            codigo = codigo + nombre.Substring(x + 1, 3);
                        else
                            codigo = codigo + nombre.Substring(x + 1, (nombre.Length) - x - 1);
                    }
                }
            }

            string codigoFinal = RemoverSignosAcentos(codigo);

            return codigoFinal;
        }

        private const string consignos = "áàäéèëíìïóòöúùuñÁÀÄÉÈËÍÌÏÓÒÖÚÙÜÑçÇ";
        private const string sinsignos = "aaaeeeiiiooouuunAAAEEEIIIOOOUUUNcC";

        public static string RemoverSignosAcentos(String texto)
        {
            StringBuilder textoSinAcentos = new StringBuilder(texto.Length);
            int indexConAcento;
            foreach (char caracter in texto)
            {
                indexConAcento = consignos.IndexOf(caracter);
                if (indexConAcento > -1)
                    textoSinAcentos.Append(sinsignos.Substring(indexConAcento, 1));
                else
                    textoSinAcentos.Append(caracter);
            }
            return textoSinAcentos.ToString();
        }

        public static DataSet ImportarArchivoExcel(string rutaArchivo, string hojaExcel)
        {
            //declaramos las variables         
            OleDbConnection conexion = null;
            DataSet dataSet = null;
            OleDbDataAdapter dataAdapter = null;
            string consultaHojaExcel = "Select * from [" + hojaExcel + "$]";

            //esta cadena es para archivos excel 2007 y 2010
            string cadenaConexionArchivoExcel = "provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + rutaArchivo + "';Extended Properties=Excel 12.0;";

            //para archivos de 97-2003 usar la siguiente cadena
            //string cadenaConexionArchivoExcel = "provider=Microsoft.Jet.OLEDB.4.0;Data Source='" + archivo + "';Extended Properties=Excel 8.0;";

            //Validamos que el usuario ingrese el nombre de la hoja del archivo de excel a leer
            try
            {
                //Si el usuario escribio el nombre de la hoja se procedera con la busqueda
                conexion = new OleDbConnection(cadenaConexionArchivoExcel);//creamos la conexion con la hoja de excel
                conexion.Open(); //abrimos la conexion
                dataAdapter = new OleDbDataAdapter(consultaHojaExcel, conexion); //traemos los datos de la hoja y las guardamos en un dataSdapter
                dataSet = new DataSet(); // creamos la instancia del objeto DataSet
                dataAdapter.Fill(dataSet, hojaExcel);//llenamos el dataset
                conexion.Close();//cerramos la conexion
                return dataSet;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void ExportarArchivoExcel(string nombreArchivo, DataSet dtsData)
        {
            DataSetHelper.CreateWorkbook(nombreArchivo, dtsData);
        }

        public static void ExportarArchivoExcel(Stream archivo, DataSet dtsData)
        {
            DataSetHelper.CreateWorkbook(archivo, dtsData);
        }

        public static string CrearArchivoPDF<T>(string titulo, List<ColumnasArchivo> columnas, List<T> datos, List<Summary> summaries = null) where T : class
        {
            string carpetaArchivos = ConfigurationManager.AppSettings["CarpetaArchivos"].ToString();
            string ruta = AppDomain.CurrentDomain.BaseDirectory;
            string nombreArchivo = DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_" + Guid.NewGuid() + ".pdf";
            string rutaCompleta = string.Format(@"{0}\{1}\{2}", ruta, carpetaArchivos, nombreArchivo);

            Document doc = new Document(PageSize.LETTER);
            // Indicamos donde vamos a guardar el documento
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(rutaCompleta, FileMode.Create));

            // Le colocamos el título y el autor
            // **Nota: Esto no será visible en el documento
            doc.AddTitle(titulo);
            doc.AddCreator("Alex Erazo");

            // Abrimos el archivo
            doc.Open();

            iTextSharp.text.Font _headerFont = new iTextSharp.text.Font(10, 10, iTextSharp.text.Font.BOLD, iTextSharp.text.Color.WHITE);
            iTextSharp.text.Font _standardFont = new iTextSharp.text.Font(10, 8, iTextSharp.text.Font.NORMAL, iTextSharp.text.Color.BLACK);
            // Escribimos el encabezamiento en el documento
            doc.Add(new Paragraph(titulo));
            doc.Add(Chunk.NEWLINE);

            // Creamos una tabla que contendrá el nombre, apellido y país
            // de nuestros visitante.
            PdfPTable tblPrueba = new PdfPTable(columnas.Count);
            //tblPrueba.WidthPercentage = 100;
            float[] anchos = new float[columnas.Count];

            int i = 0;
            columnas.ForEach(x =>
            {
                PdfPCell columna = new PdfPCell(new Phrase(x.Titulo, _headerFont));
                //columna.BorderWidth = 1;
                //columna.BorderWidthBottom = 0.75f;
                columna.BackgroundColor = iTextSharp.text.Color.BLACK;
                columna.PaddingBottom = 5;
                columna.PaddingLeft = 5;
                columna.PaddingRight = 5;
                columna.PaddingTop = 5;
                switch (x.Alineacion)
                {

                    case "right":
                        columna.HorizontalAlignment = Element.ALIGN_RIGHT;
                        break;
                    case "center":
                        columna.HorizontalAlignment = Element.ALIGN_CENTER;
                        break;
                    case "left":
                    default:
                        columna.HorizontalAlignment = Element.ALIGN_LEFT;
                        break;
                }

                if (x.Ancho.HasValue)
                {
                    anchos[i] = x.Ancho.Value;
                }
                else
                {
                    anchos[i] = 100f;
                }

                // Añadimos las celdas a la tabla
                tblPrueba.AddCell(columna);
                i++;
            });

            tblPrueba.SetWidths(anchos);

            datos.ForEach(y =>
            {
                columnas.ForEach(x =>
                {
                    var propOrigen = y.GetType().GetProperty(x.Nombre);
                    if (propOrigen != null)
                    {
                        string valor = propOrigen.GetValue(y).ToString();
                        switch (x.TipoDato)
                        {
                            case "money":
                                valor = decimal.Parse(valor).ToString("c");
                                break;
                            case "date":
                                string formato = string.IsNullOrEmpty(x.Formato) ? "yyyy-MM-dd" : x.Formato;
                                valor = DateTime.Parse(valor).ToString(formato);
                                break;
                            default:
                                break;
                        }

                        PdfPCell data = new PdfPCell(new Phrase(valor, _standardFont));
                        switch (x.Alineacion)
                        {

                            case "right":
                                data.HorizontalAlignment = Element.ALIGN_RIGHT;
                                break;
                            case "center":
                                data.HorizontalAlignment = Element.ALIGN_CENTER;
                                break;
                            case "left":
                            default:
                                data.HorizontalAlignment = Element.ALIGN_LEFT;
                                break;
                        }

                        // Añadimos las celdas a la tabla
                        tblPrueba.AddCell(data);
                    }
                });

            });

            if (summaries != null && summaries.Count > 0)
            {
                columnas.ForEach(x =>
                {
                    var existeColumna = summaries.FirstOrDefault(y => y.Nombre == x.Nombre);
                    string valor = "";
                    if (existeColumna == null)
                    {

                        PdfPCell data = new PdfPCell(new Phrase("", _standardFont));
                        data.BackgroundColor = iTextSharp.text.Color.LIGHT_GRAY;
                        tblPrueba.AddCell(data);
                    }
                    else
                    {
                        switch (existeColumna.TipoSummary)
                        {
                            case "sum":
                                switch (existeColumna.TipoDato)
                                {
                                    case "money":
                                        valor = datos.Sum(d => decimal.Parse(d.GetType().GetProperty(x.Nombre).GetValue(d).ToString())).ToString("c");
                                        break;
                                    case "number":
                                        valor = datos.Sum(d => int.Parse(d.GetType().GetProperty(x.Nombre).GetValue(d).ToString())).ToString();
                                        break;
                                }
                                break;
                            case "count":
                                valor = datos.Count().ToString();
                                break;
                        }

                        if (!string.IsNullOrEmpty(existeColumna.DisplayFormat))
                        {
                            valor = existeColumna.DisplayFormat.Replace("{0}", valor);
                        }
                        PdfPCell data = new PdfPCell(new Phrase(valor, _standardFont));
                        data.BackgroundColor = iTextSharp.text.Color.LIGHT_GRAY;
                        switch (existeColumna.Alineacion)
                        {

                            case "right":
                                data.HorizontalAlignment = Element.ALIGN_RIGHT;
                                break;
                            case "center":
                                data.HorizontalAlignment = Element.ALIGN_CENTER;
                                break;
                            case "left":
                            default:
                                data.HorizontalAlignment = Element.ALIGN_LEFT;
                                break;
                        }
                        tblPrueba.AddCell(data);
                    }
                });
            }

            doc.Add(tblPrueba);

            doc.Close();
            writer.Close();

            return rutaCompleta;
        }

        public static T Convert<T>(this object obj) where T : class
        {
            return (T)obj;
        }

        public static T TransformarObjeto<S, T>(this S objetoOrigen, T objetoDestino)
        {
            var todasPropiedades = typeof(T).GetProperties();

            foreach (var propDestino in todasPropiedades)
            {
                //se busca si la propiedad existe en el origen y es del mismo tipo
                var propOrigen = objetoOrigen.GetType().GetProperty(propDestino.Name);

                //no existe propiedad origen. se verifica siguiente propiedad
                if (propOrigen == null)
                {
                    continue;
                }
                else
                {
                    //si las propiedades son del mismo tipo se asigna.
                    if (propOrigen.PropertyType == propDestino.PropertyType)
                    {
                        propDestino.SetValue(objetoDestino, objetoOrigen.ObtenerValorPropiedad(propDestino.Name));
                    }
                    else
                    {
                        //las propiedades son de distinto tipo => se procura asignar (cast simple. por ejemplo: short a int)
                        try
                        {
                            propDestino.SetValue(objetoDestino, objetoOrigen.ObtenerValorPropiedad(propDestino.Name));
                        }
                        catch
                        {
                            //no se asigna. se omite
                        }
                    }
                }
            }

            return objetoDestino;
        }

        public static object ObtenerValorPropiedad(this object objeto, string nombrePropiedad)
        {
            if (objeto == null)
                //no existe el objeto
                return null;

            var prop = objeto.GetType().GetProperty(nombrePropiedad);

            if (prop == null)
                //no existe la propiedad
                return null;

            var valor = prop.GetValue(objeto);
            if (valor == null)
                //no existe valor de la propiedad
                return null;

            return valor;
        }

        public static T TransformarObjeto<S, T>(this S objetoOrigen)
        {
            if (objetoOrigen != null)
            {
                T objetoDestino = Activator.CreateInstance<T>();
                return objetoOrigen.TransformarObjeto<S, T>(objetoDestino);
            }
            return default(T);
        }

        public static List<T> TransformarObjeto<S, T>(this IEnumerable<S> listaObjetoOrigen)
        {
            return listaObjetoOrigen.Select(p => p.TransformarObjeto<S, T>()).ToList();
        }
    }
}
