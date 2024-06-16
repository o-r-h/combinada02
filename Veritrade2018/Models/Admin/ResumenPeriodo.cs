using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Veritrade2018.Models.Admin
{
    public class ResumenPeriodo
    {
        public string DescripcionTotal { get; set; }
        public string CifTot { get; set; }

        public string DescripcionCantidad { get; set; }
        public string DescripcionPrecioUnitario { get; set; }
        public string Cantidad { get; set; }
        public string UnidadesCantidad { get; set; }

        public string CifUnit { get; set; }

        public string UnidadCifUnit { get; set; }

        public ResumenPeriodo()
        {
            Cantidad = "-";
            CifUnit = "-";
            UnidadesCantidad = "-";
            UnidadCifUnit = "-";
        }
    }
}