using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Veritrade2018.Models
{
    public class Correo
    {
        public string Titulo { set; get; }
        public string CodUsuario { set; get; }
        public string Nombres { set; get; }
        public string Empresa { set; get; }
        public string Telefono { set; get; }
        public string Email1 { set; get; }

        public string Mensaje { set; get; }
        public string DireccionIP { set; get; }
        public string Ubicacion { set; get; }

        public Correo()
        {
            Titulo = "";
            CodUsuario = "";
            Nombres = "";
            Empresa = "";
            Telefono = "";
            Email1 = "";
            Mensaje = "";
            DireccionIP = "";
            Ubicacion = "";
        }
    }
}