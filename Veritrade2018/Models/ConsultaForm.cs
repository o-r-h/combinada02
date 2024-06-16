using System.ComponentModel.DataAnnotations;
using Veritrade2018.Helpers;
using Veritrade2018.Models.Admin;

namespace Veritrade2018.Models
{
    public class ConsultaForm
    {
        public bool Importador { get; set; }
        public bool Proveedor { get; set; }
        public bool PaisOrigen { get; set; }
        public bool Exportador { get; set; }
        public bool ImportadorExp { get; set; }
        public bool Filtros { get; set; }
        public string CodPais { get; set; }
        public string TipoOpe { get; set; }

        [Required(ErrorMessage = " ")]
        public string txtDesComercialB { get; set; }

        public bool FlagDescComercialB { get; set; }

        public string Cif { get; set; }

        public string CampoPeso { get; set; }

        public  bool ExisteDistrito { get; set; }

        public bool ExisteAduana { get; set; }

        public bool ExistePaisDestino { get; set; }

        public bool ExistePartida { get; set; }
        public bool ExisteDua { set; get; }

        public string Dua { get; set; }

        public bool IsOcultoMarcasModelos { get; set; }

        public bool ExisteViaTransp { get; set; }



        public bool HideTabExcel { get; set; }
        public bool FlagRegMax { get; set; }
        //public FlagVarVisibles FlagVarVisibles { get; set; }
        public TabMisBusquedas FlagVarVisibles { get; set; }

        public ConsultaForm()
        {
            Importador = true;
            Proveedor = true;
            PaisOrigen = true;
            Exportador = true;
            ImportadorExp = true;
            Filtros = false;
            CodPais = "PE";
            TipoOpe = "I";
            FlagDescComercialB = true;
            Cif = "";
            CampoPeso = "";
            ExisteDistrito = false;
            ExisteAduana = false;
            ExistePaisDestino = false;
            IsOcultoMarcasModelos = false;
            ExisteViaTransp = true;
            FlagVarVisibles = new TabMisBusquedas();  //FlagVarVisibles();
            FlagRegMax = false;
        }
    }
}