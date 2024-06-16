using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Veritrade2017.Models
{
    public class TelefonoPais
    {
        public int TelefonoId { get; set; }
        public int CodTelefono { get; set; }
        public Int64 Telefono { get; set; }
        public string CodPais { get; set; }
        public string CodBandera { get; set; }
        public string IconoBandera { get; set; }
        public string NombrePais { get; set; }
    }
}