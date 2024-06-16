using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Veritrade2018.Models
{
    public class ListProductByPaises
    {
        public int IdPaisAduana { get; set; }
        public String PaisAduana { get; set; }
        public String Importaciones { get; set; }
        public String Exportaciones { get; set; }
        public int Importadores { get; set; }
        public int Exportadores { get; set; }
    }
}