using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Veritrade2018.Models.Admin
{
    public class GridRow
    {
        public string Id { set; get; } 
        public string Descripcion { set; get; }
        public string TotalReg { set; get; }

        public string CiFoFobTot { set; get; }

        public string CiFoFobPor { set; get; }

        public string TotalKg { get; set; }
        public string Precio { get; set; }

        public bool IsVisibleSentinel { get; set; }

        public bool IsVisibleLupa { get; set; }
        public bool IsVisibleLupaPartida { get; set; }

        public bool IsPlanPermiteSentinel { get; set; }

        public bool IsEnabledTotalReg { set; get; }
        
        public string Ruc { get; set; }
        public string LnkGoogle { get; set; }
        public string LnkGoogle2 { get; set; }

        

        public string Dua { get; set; }
        public GridRow()
        {
            IsEnabledTotalReg = true;
            CiFoFobTot = string.Empty;
            
        }

        public string Marca { get; set; }
        public string Fecha { get; set; }
        public string PartidaAduanera { get; set; }
        public string IdImportador2 { get; set; }
        public string Importador { get; set; }
        public string Exportador { get; set; }
        public string Cantidad { get; set; }
        public string UndMedida { get; set; }
        public string FobUnit { set; get; }
        public string CifUnit { set; get; }
        public string CifUnitImp { set; get; }
        public string PaisOrigen { set; get; }
        public Int64 Nro { set; get; }
        public string Distrito { set; get; }      
        public string ImportadorOrExportador { get; set; }
        public string Notificado { get; set; }
        public string ExportadorOrImportador { get; set; }
        public string PaisEmbarqueOrPaisDest { get; set; }
        public string Puerto { get; set; }
        public string PesoBruto { get; set; }
        public string CantidadOrBultos { get; set; }
        public string DesAdicional { get; set; }
        public string CodPaisComplementario { get; set; }
        public bool mostrarInformaColombia { get; set; }
    }
}