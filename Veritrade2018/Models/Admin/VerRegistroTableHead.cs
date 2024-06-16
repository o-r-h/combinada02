using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Veritrade2018.Models.Admin
{
    public class VerRegistroTableHead
    {
        public string Numero { get; set; }

        public string Fecha { set; get; }
        public string Nandina { set; get; }
        public string Importador { set; get; }
        public string Notificado { set; get; }
        public string PaisEmbarque { set; get; }
        public string PuertoDescarga { set; get; }
        public string PuertoEmbarque { set; get; }
        public string Bultos { set; get; }
        public string Exportador { set; get; }

        public string CampoPeso { set; get; }

        public string Cantidad { get; set; }

        public string Unidad { get; set; }
        public string  FobOFasUnit { get; set; }
        public string CifOFobUnit { get; set; }
        public string CifImptoUnit { get; set; }

        public string Dua { get; set; }

        public string PaisOrigenODestino { get; set; }

        public string DesComercial { get; set; }

        public string DesAdicional { get; set; }

        public string Distrito { get; set; }

        public bool IsVisibleImportador { get; set; }
        public bool IsVisibleExportador { get; set; }
        public bool IsVisibleFobOFasUnit { get; set; }
        public bool IsVisibleCifOFobUnit { get; set; }
        public bool IsVisibleCifImptoUnit { get; set; }
        public bool IsVisibleDua { get; set; }
        public bool IsVisiblePaisOrigenODestino { get; set; }
        public bool IsVisibleDesComercial { get; set; }
        public bool IsVisibleDistrito { get; set; }

        public bool IsVisibleCampoPeso { get; set; }

        public VerRegistroTableHead()
        {
            IsVisibleImportador = true;
            IsVisibleExportador = true;
            IsVisibleFobOFasUnit = true;
            IsVisibleCifOFobUnit = true;
            IsVisibleCifImptoUnit = true;
            IsVisibleDua = true;
            IsVisiblePaisOrigenODestino = true;
            IsVisibleDesComercial = true;
            IsVisibleDistrito = true;
            IsVisibleCampoPeso = true;
        }
    }
}