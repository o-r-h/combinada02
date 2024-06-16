using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Veritrade2018.Models.Admin
{
    public class VerRegistroTableRow
    {
        public int Numero { get; set; }
        public string Fecha { set; get; }
        public string Nandina { set; get; }
        public string Importador { set; get; }
        public string Exportador { set; get; }
        public string ExportadorProveedor { set; get; }

        public string CampoPeso { set; get; }

        public string Cantidad { get; set; }

        public string Unidad { get; set; }
        public string FobOFasUnit { get; set; }
        public string CifOFobUnit { get; set; }
        public string CifImptoUnit { get; set; }

        public string Dua { get; set; }

        public string PaisOrigenODestino { get; set; }

        public string DesComercial { get; set; }

        public string Distrito { get; set; }
        public string ImportadorExp { get; set; }
        public string Notificado { get; set; }
        public string PaisEmbarque { get; set; }
        public string PtoDestino { get; set; }
        public string Pto { get; set; }
        public string PesoBruto { get; set; }
        public string QtyOrButos { get; set; }
        public string DesAdicional { get; set; }
    }
}