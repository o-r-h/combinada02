using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Veritrade2017.Models
{
    public class ListProductByPaises
    {
        public int Año { set; get; }
        public int IdPaisAduana { get; set; }
        public string PaisAduana { get; set; }
        public string PaisAduanaEN { get; set; } // Ruben 202404
        public string AbrevPais { get; set; }
        public decimal Importaciones { get; set; }
        public decimal Exportaciones { get; set; }
        public int Importadores { get; set; }
        public int Exportadores { get; set; }
        public decimal Porcentaje { get; set; }
        public decimal ImportsExports { get; set; }
        public String Empresa { set; get; }
        public string Regimen { set; get; }
        public int RegistrosI { set; get; }
        public int RegistrosE { set; get; }
        public decimal CifOrPrecProm { set; get; }
    }
}