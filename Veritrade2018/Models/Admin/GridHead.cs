using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Veritrade2018.Helpers;

namespace Veritrade2018.Models.Admin
{
    public class GridHead
    {
        public string Descripcion { set; get; }
        public string TotalReg { set; get; }

        public string CiFoFobTot { set; get; }

        public string CiFoFobPor { set; get; }

        public string TotalKg { set; get; }

        public string Precio { set; get; }

        public bool IsVisibleTotalKg { set; get; }
        public bool IsVisblePrecio { set; get; }

        public string OrdenTotalKg { set; get; }

        public string OrdenCiFoFobTot { get; set; }
        public string TitleColumnTotalReg { get; set; }

        public bool IsVisibleFiltroCboDescripcion { get; set; }

        public bool IsVisibleColumnCheck { get; set; }
        public string Duas { get; set; }

        public bool IsVisibleDuas { get; set; }

        public GridHead()
        {
            CiFoFobTot = "%";
            IsVisibleFiltroCboDescripcion = true;
            IsVisibleColumnCheck = true;
            TitleColumnTotalReg = Resources.Resources.Grid_Column_SeeRecords;
            IsVisibleDuas = true;
        }

        public bool visible { get; set; }
        public string label { get; set; }
        public object value { get; set; }
        public string label2 { get; set; }
        public string value2 { get; set; }
        public string className { get; set; }
        public string dataField { get; set; }
        public bool hasVal2 { get; set; } = false;

        public GridHead(string label, object value = null, bool visible = true, string className = "", string dataField="", string label2 = "", string value2 = "", bool hasVal2 = false)
        {
            this.label = label;
            this.value = value;
            this.visible = visible;
            this.className = className;
            this.dataField = dataField;
            this.label2 = label2; ;
            this.value2 = value2;
            this.hasVal2 = hasVal2;
        }


        public bool IsVisibleImportador { get; set; }
        public bool IsVisibleExportador { get; set; }
        public bool IsVisibleFobUnit { get; set; }
        public bool IsVisibleCifUnit { get; set; }
        public bool IsVisibleCifImptos { get; set; }
        public bool IsVisibleDua { get; set; }
        public bool IsVisiblePaisOrigen { get; set; }
        public bool IsVisibleDesCom { get; set; }
        public bool IsVisibleKgNeto { get; set; }
        public bool IsVisibleDistrito { get; set; }
        public bool IsVisibleMarcaEC { get; set; }
        public bool mostrarInforColombia { get; set; }

        public void SetVisiblesColumns(string tipoOpe, string codPais, string cif, bool ExisteImportador, bool ExisteExportador, bool ExisteProveedor, bool ExisteImportadorExp, bool ExisteDua,
            bool ExistePaisOrigen, bool ExistePaisDestino, bool ExisteDesComercial, string campoPeso , string plan,bool ExisteMarcaEC)
        {
            IsVisibleImportador = (tipoOpe == "I" ? ExisteImportador : ExisteExportador);
            IsVisibleExportador = (tipoOpe == "I" ? ExisteProveedor : (ExisteImportadorExp && plan != "ESENCIAL"));
            IsVisibleFobUnit = FuncionesBusiness.IsVisibleFobUnit(codPais, tipoOpe);//tipoOpe != "I" || (cif != "FOB" && codPais != "CN" && codPais != "IN");
            IsVisibleCifUnit = (tipoOpe == "I");
            IsVisibleCifImptos = (tipoOpe == "I" && (codPais == "PE"));
            IsVisibleDua = ExisteDua;
            IsVisiblePaisOrigen = (tipoOpe == "I" ? ExistePaisOrigen : ExistePaisDestino);
            IsVisibleDesCom = ExisteDesComercial;
            IsVisibleDistrito = (codPais == "US");
            IsVisibleKgNeto = (campoPeso != "");
            IsVisibleMarcaEC = ExisteMarcaEC;
            mostrarInforColombia = (tipoOpe == "I" && codPais == "CO");
            //mostrarInforColombia = false;
        }
    }
}