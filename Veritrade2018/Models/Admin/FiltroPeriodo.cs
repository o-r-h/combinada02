using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Veritrade2018.Models.Admin
{
    public class FiltroPeriodo
    {
        public DateTime FechaInfoIni { get; set; }
        public DateTime FechaInfoFin { get; set; }

        public bool EnabledAnioMesIni { get; set; }
        public bool EnabledAnioMesFin { get; set; }

        /// <summary>
        /// Atributos usados unicamente Mis Productos y Mis Compañizas
        /// </summary>
        public DateTime FechaAnioIni { get; set; }
        public DateTime FechaAnioFin { get; set; }

        public DateTime DefaultFechaInfoIni { get; set; }
        public DateTime DefaultFechaInfoFin { get; set; }

        public DateTime DefaultFechaAnioIni { get; set; }
        public DateTime DefaultFechaAnioFin { get; set; }

        public FiltroPeriodo()
        {
            EnabledAnioMesIni = true;
            EnabledAnioMesFin = true;
        }
    }
}