using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Veritrade2017.Models
{
    public class UsuarioMixPanel
    {
        public int IdUsuario { get; set; }
        public string CodUsuario { get; set; }
        public string codPais { get; set; }
        public string Nombres { get; set; }
        public string Email1 { get; set; }
        public string Email2 { get; set; }
        public string Ciudad { get; set; }
        public string IdTipo { get; set; }
        public string Tipo { get; set; }
        public string DireccionIP { get; set; }
        public string Cliente { get; set; }
        public string Plan { get; set; }
        public string Sector { get; set; }
    }
}