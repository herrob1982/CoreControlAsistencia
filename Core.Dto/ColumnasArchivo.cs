using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dto
{
    public class ColumnasArchivo
    {
        public string Nombre { get; set; }
        public string Titulo { get; set; }
        public string TipoDato { get; set; }
        public string Formato { get; set; }
        public string Alineacion { get; set; }
        public float? Ancho { get; set; }
    }
}
