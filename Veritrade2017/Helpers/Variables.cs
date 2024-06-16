using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Dapper;
using Microsoft.Ajax.Utilities;
using Veritrade2017.Util;

namespace Veritrade2017.Helpers
{
    public class Variables
    {
        public enum ConfigManager
        {
            [Display(Name = "Content")] CONTENT,
            [Display(Name = "Minisite")] MINISITE,
            [Display(Name = "System")] SYSTEM,
            [Display(Name = "Admin")] ADMIN,
        }

        public static int CantRegMax { get; set; } = 200000;

        public static int CantRegMaxExcel { get; set; } = 40000;

        public static int CantRegMaxFreeTrial { get; set; } = 20;

        public static int NumTextTruncateDescriptionExcel { get; } = 155;
        public static int NumTrunResProd { get; } = 103;
        public static int NumTrunResSmall { get; } = 17;
        public static int NumTrunResSmallX2 { get; } = 20;
        public static int NumTrunTabProd { get; } = 52;
        public static int NumTrunTabMarcasAndAduana { get; } = 70;
        public static int NumTrunTabOthers { get; } = 26;

        public static bool ExisteFiltro(string IdTipoFiltro, string IdFiltro, string IdTipoOpe, string CodPais)
        {
            int res = 0;
            using (var db = new ConexProvider().Open)
            {

                res = db.ExecuteScalar<int>("select count(*) from dbo.VariableFiltros " +
                                            "Where IdTipoFiltro=@IdTipoFiltro And IdFiltro=@IdFiltro And IdTipoOpe=@IdTipoOpe " +
                                            "And CodPais=@CodPais", new { IdTipoFiltro, IdFiltro, IdTipoOpe, CodPais });
            }

            return res > 0;
        }
    }

    public class VarValues
    {
        public string IdGrupo { get; set; }
        public string IdVariable { get; set; }
        public string Descripcion { get; set; }
        public string Descripcion_Eng { get; set; }
        public string IdParent { get; set; }
        public string Valores { get; set; }
        public string iOrden { get; set; }
        public bool Estado { get; set; }
    }

    public class VarGeneral
    {
        #region SINGLETON
        private static VarGeneral singletonInstance = CreateSingleton();

        private VarGeneral()
        {
        }

        private static VarGeneral CreateSingleton()
        {
            if (singletonInstance == null)
            {
                singletonInstance = new VarGeneral();
            }

            return singletonInstance;
        }

        public static VarGeneral Instance
        {
            get { return singletonInstance; }
        }
        #endregion

        private List<VarValues> _Values;


        public  List<VarValues> Values
        {
            get
            {
                //if (_Values == null)
                //{
                    _Values = new List<VarValues>();
                    using (var db = new ConexProvider().Open)
                    {
                        db.Query<VarValues>("select * from dbo.VariableGeneral Where Estado=1 Order By iOrden")
                            .ForEach(i => _Values.Add(i));
                    }
                //}
                return _Values;
            }
        }

        public Dictionary<string, VarValues> ValuesDict
        {
            get
            {
                var dic = Values.ToDictionary(x => x.IdVariable, y => y);
                return dic;
            }
        } 

    }

    #region MisFiltros

    public class VarFiltros
    {
        public string TipoOpe { get; set; }
        public string CodPais { get; set; }

        public bool IsManifiesto => (new[] { "PEI", "USI", "PEE", "USE","ECI","ECE", "BRI", "BRE" }).Contains(CodPais);

        public VarFiltros(string TipoOpe, string CodPais)
        {
            this.TipoOpe = TipoOpe;
            this.CodPais = CodPais;
        }

        public VarFiltros()
        {
        }

    }
    public class FiltroMisBusquedas : VarFiltros
    {
        public bool FlagDescComercialB, ExistePartida, Importador, Proveedor, Exportador, ImportadorExp, PaisOrigen;

        public FiltroMisBusquedas(string TipoOpe, string CodPais)
            : base(TipoOpe, CodPais)
        {
            this.SetFiltroMisBusquedas();
        }

        public void SetFiltroMisBusquedas()
        {
            var IdTipoFil = Enums.VarId.MIS_BUS.GetDn();

            if (!IsManifiesto)
            {
                FlagDescComercialB = Variables.ExisteFiltro(IdTipoFil, Enums.VarId.DESC_COMERCIAL.GetDn(), TipoOpe, CodPais);
                ExistePartida = Variables.ExisteFiltro(IdTipoFil, Enums.VarId.PARTIDAS.GetDn(), TipoOpe, CodPais);
                Importador =
                    ImportadorExp = //Funciones.ExisteVariable(codPais, tipoOpe, "IdImportadorExp")
                        Variables.ExisteFiltro(IdTipoFil, Enums.VarId.IMPORTADOR.GetDn(), TipoOpe, CodPais);
                Proveedor = Exportador = Variables.ExisteFiltro(IdTipoFil, Enums.VarId.EXPORTADOR.GetDn(), TipoOpe, CodPais); //Funciones.ExisteVariable(codPais, tipoOpe, "IdProveedor") // Funciones.ExisteVariable(codPais, tipoOpe, "IdExportador") 
                PaisOrigen = Variables.ExisteFiltro(IdTipoFil, Enums.VarId.PAIS.GetDn(), TipoOpe, CodPais);
            }
            else
            {
                FlagDescComercialB = Variables.ExisteFiltro(IdTipoFil, Enums.VarId.DESC_COMERCIAL.GetDn(), TipoOpe, CodPais);
                ExistePartida = Variables.ExisteFiltro(IdTipoFil, Enums.VarId.PARTIDAS.GetDn(), TipoOpe, CodPais); 
                Importador = Variables.ExisteFiltro(IdTipoFil, Enums.VarId.IMPORTADOR.GetDn(), TipoOpe, CodPais); //(TipoOpe == "I");
                ImportadorExp = Variables.ExisteFiltro(IdTipoFil, Enums.VarId.IMPORTADOR.GetDn(), TipoOpe, CodPais); //(CodPais == "PEE");
                Proveedor = Variables.ExisteFiltro(IdTipoFil, Enums.VarId.EXPORTADOR.GetDn(), TipoOpe, CodPais);  //(TipoOpe == "I");
                Exportador = Variables.ExisteFiltro(IdTipoFil, Enums.VarId.EXPORTADOR.GetDn(), TipoOpe, CodPais); //(TipoOpe == "E");
                PaisOrigen = Variables.ExisteFiltro(IdTipoFil, Enums.VarId.PAIS.GetDn(), TipoOpe, CodPais); //(TipoOpe == "I");
            }
        }

    }

    public class TabMisBusquedas : VarFiltros
    {
        //private readonly string[] _paisesCondicionViaTransp = new[] { "BO", "MXD", "PY", "US", "UE" };
        //private readonly string[] _paisesCondicionAduana = new[] { "CN", "IN", "PY", "US", "UE" };
        private readonly string[] _paisesCondicionDua = new[] { "AR", "BR", "CO", "MX", "MXD", "UY" };

        public bool ExistePartida,
            ExisteImportador,
            ExisteNotificado,
            ExisteProveedor,
            ExistePaisOrigen,
            ExisteExportador,
            ExisteImportadorExp,
            ExistePaisDestino,
            ExisteViaTransp,
            ExisteAduana,
            ExisteDistrito,
            ExistePtoDescarga,
            ExistePtoEmbarque,
            ExistePtoDestino,
            ExisteManifiesto, ExisteDUA, ExisteDesComercial, ExisteDesAdicional, ExisteMarcasModelos, ExisteMarcaEC,
            ExisteInfoTabla;

        public TabMisBusquedas(string TipoOpe, string CodPais) 
            : base(TipoOpe, CodPais)
        {
            SetTabMisBusquedas();
        }

        public TabMisBusquedas()
        {
        }


        public void SetTabMisBusquedas()
        {
            ExisteInfoTabla = false;
            var IdTipoFil = Enums.VarId.TABS_MIS_BUS.GetDn();

            if (!IsManifiesto)
            {
                ExisteImportador = ExisteImportadorExp = Variables.ExisteFiltro(IdTipoFil, Enums.VarId.IMPORTADOR.GetDn(), TipoOpe, CodPais);
                ExisteExportador = ExisteProveedor = Variables.ExisteFiltro(IdTipoFil, Enums.VarId.EXPORTADOR.GetDn(), TipoOpe, CodPais);
                ExistePaisOrigen = ExistePaisDestino = Variables.ExisteFiltro(IdTipoFil, Enums.VarId.PAIS.GetDn(), TipoOpe, CodPais);
                //ExisteExportador = Funciones.ExisteVariable(CodPais, TipoOpe, "IdExportador");
                //ExisteImportadorExp = Funciones.ExisteVariable(CodPais, TipoOpe, "IdImportadorExp");
                //ExistePaisDestino = Funciones.ExisteVariable(CodPais, TipoOpe, "IdPaisDestino");
                ExisteViaTransp = Variables.ExisteFiltro(IdTipoFil, Enums.VarId.VIA.GetDn(), TipoOpe, CodPais);   //GetExisteViaTransp();
                ExisteAduana = Variables.ExisteFiltro(IdTipoFil, Enums.VarId.ADUANA.GetDn(), TipoOpe, CodPais); //GetExisteAduana();
                ExisteDUA = GetExisteDua(ExisteAduana);
                ExisteDistrito = Variables.ExisteFiltro(IdTipoFil, Enums.VarId.DISTRITO.GetDn(), TipoOpe, CodPais);  //GetExisteDistrito();
                ExisteDesComercial = Variables.ExisteFiltro(IdTipoFil, Enums.VarId.DESC_COMERCIAL.GetDn(), TipoOpe, CodPais); 
                                        //Funciones.FlagDesComercial(CodPais, TipoOpe);
                ExistePartida = Variables.ExisteFiltro(IdTipoFil, Enums.VarId.PARTIDAS.GetDn(), TipoOpe, CodPais);
                ExisteMarcaEC = Variables.ExisteFiltro(IdTipoFil, Enums.VarId.MARCAEC.GetDn(), TipoOpe, CodPais);
                //ExisteMarcasModelos  = Variables.ExisteFiltro(IdTipoFil, Enums.VarId.MARCAS_MODELOS.GetDn(), TipoOpe, CodPais);
            }
            else
            {
                ExisteDesComercial = Variables.ExisteFiltro(IdTipoFil, Enums.VarId.DESC_COMERCIAL.GetDn(), TipoOpe, CodPais); 
                ExisteDesAdicional = Variables.ExisteFiltro(IdTipoFil, Enums.VarId.DESC_ADICIONAL.GetDn(), TipoOpe, CodPais);  //(CodPais != "USE");
                ExisteImportador = Variables.ExisteFiltro(IdTipoFil, Enums.VarId.IMPORTADOR.GetDn(), TipoOpe, CodPais); //(TipoOpe == "I");
                ExisteNotificado = Variables.ExisteFiltro(IdTipoFil, Enums.VarId.NOTIFICADO.GetDn(), TipoOpe, CodPais); //(CodPais == "USI");
                ExisteProveedor = Variables.ExisteFiltro(IdTipoFil, Enums.VarId.EXPORTADOR.GetDn(), TipoOpe, CodPais); //(TipoOpe == "I");
                ExistePaisOrigen = Variables.ExisteFiltro(IdTipoFil, Enums.VarId.PAIS.GetDn(), TipoOpe, CodPais); //(TipoOpe == "I");
                ExisteExportador = Variables.ExisteFiltro(IdTipoFil, Enums.VarId.EXPORTADOR.GetDn(), TipoOpe, CodPais); //(TipoOpe == "E");
                ExisteImportadorExp = Variables.ExisteFiltro(IdTipoFil, Enums.VarId.IMPORTADOR.GetDn(), TipoOpe, CodPais); //(CodPais == "PEE");
                ExistePaisDestino = Variables.ExisteFiltro(IdTipoFil, Enums.VarId.PAIS.GetDn(), TipoOpe, CodPais); //(TipoOpe == "E");
                ExistePtoDescarga = Variables.ExisteFiltro(IdTipoFil, Enums.VarId.PTO_DESCARGA.GetDn(), TipoOpe, CodPais);   //(CodPais == "USI");
                ExistePtoEmbarque = Variables.ExisteFiltro(IdTipoFil, Enums.VarId.PTO_EMBARQUE.GetDn(), TipoOpe, CodPais); //(CodPais == "USE" || CodPais == "PEI");
                ExistePtoDestino = Variables.ExisteFiltro(IdTipoFil, Enums.VarId.PTO_DESTINO.GetDn(), TipoOpe, CodPais); //(CodPais == "PEE");
                ExisteManifiesto = Variables.ExisteFiltro(IdTipoFil, Enums.VarId.MANIFIESTO.GetDn(), TipoOpe, CodPais); //(CodPais == "PEI" || CodPais == "PEE");

                ExistePartida = Variables.ExisteFiltro(IdTipoFil, Enums.VarId.PARTIDAS.GetDn(), TipoOpe, CodPais);
                ExisteAduana = Variables.ExisteFiltro(IdTipoFil, Enums.VarId.ADUANA.GetDn(), TipoOpe, CodPais); //false;

                //ExisteMarcasModelos = Variables.ExisteFiltro(IdTipoFil, Enums.VarId.MARCAS_MODELOS.GetDn(), TipoOpe, CodPais);

            }
            ExisteMarcasModelos = GetExisteMarcasModelos(Variables.ExisteFiltro(IdTipoFil, Enums.VarId.MARCAS_MODELOS.GetDn(), TipoOpe, CodPais));
        }

        //private bool GetExisteViaTransp()
        //{
        //    return !_paisesCondicionViaTransp.Contains(CodPais);
        //}

        //private bool GetExisteAduana()
        //{
        //    return !_paisesCondicionAduana.Contains(CodPais);
        //}

        private bool GetExisteDua(bool existeAduana)
        {
            if (HttpContext.Current.Session["Plan"] == null)
                HttpContext.Current.Session["Plan"] = "";

            return existeAduana
                   && !_paisesCondicionDua.Contains(CodPais)
                   && HttpContext.Current.Session["Plan"].ToString() != "ESENCIAL";
        }

        private bool GetExisteMarcasModelos(bool existeMarcasModelos)
        {   
            return existeMarcasModelos
                   && new []{ "BUSINESS", "PREMIUM", "UNIVERSIDADES" }.Contains(HttpContext.Current.Session["Plan"].ToString());
        }

        //private bool GetExisteDistrito()
        //{
        //    return (CodPais == "CN" || CodPais == "US");
        //}
    }

    #endregion

}