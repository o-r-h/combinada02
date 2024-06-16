using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Veritrade2018.Helpers;
using Dapper;

namespace Veritrade2018.Models.Admin
{
    public class MiBusqueda : ConsultaForm
    {
        public bool ForzarLinkExcel { set; get; }

        public bool MisPartidas { set; get; }
        public bool MisImportadores { set; get; }
        public bool MisExportadores { set; get; }
        public bool MisProveedores { set; get; }
        public bool MisImportadoresExp { set; get; }
        public bool EsFreeTrial { set; get; }

        public string RangoInfoEnLinea { set; get; }
        public string RangoInfoEnLineaFree { set; get; }
        public FiltroPeriodo Periodos { set; get; }

        public string CodPais2 { set; get; }

        public bool IsManifiesto { get; set; }

        public string FiltroUtilizado { get; set; }

        public MiBusqueda()
        {
            MisPartidas = true;
            MisImportadores = true;
            MisExportadores = true;
            MisProveedores = true;
            MisImportadoresExp = true;
            EsFreeTrial = false;
            RangoInfoEnLinea = "";
            RangoInfoEnLineaFree = "";
            CodPais2 = "1LAT";
            FiltroUtilizado = "";
        }


        public static DataTable BuscaPartidas(string nandina, string codPais, string codPais2,  string idioma)
        {
            string sql = "";
            nandina = nandina.Replace("'","''");
            string partida = "Partida";

            if (idioma == "en")
                partida = "Partida_en";

            string codPaisT = codPais;

            if (codPais == "PEB" || codPais == "PEP")
            {
                codPaisT = "PE";
            }
            else if (codPais2 == "4UE")
            {
                codPaisT = "UE";
            }

            sql = "select count(*) as Cant from Partida_" + codPaisT + " ";
            sql += "where Nandina like '" + nandina + "%' ";

            int cant = 0;

            DataTable dtRespuesta;
            try
            {
                DataTable dt = Conexion.SqlDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    cant = Convert.ToInt32((dt.Rows[0])["Cant"]);
                }

                string subCapitulo = "SubCapitulo";
                string HTS6 = "HTS6";
                string lTodos = "TODOS";

                if (idioma == "en" || codPais == "BR" || codPais == "CN" || codPais == "IN" || codPais == "US")
                {
                    subCapitulo = "SubCapituloEN";
                    HTS6 = "HTS6_EN";
                    lTodos = "ALL";
                }

                sql = "";
                if (cant > 0)
                {
                    sql += "select CodSubCapitulo as Nandina2, '" + nandina + "' as Nandina, replace(convert(char(13), '" + nandina + "'), ' ', '_') + ' [" + lTodos + " 4] ' + substring(" + subCapitulo + ", 1, 80) as Partida ";
                    sql += "from SubCapitulo where CodSubCapitulo = '" + nandina + "' union ";
                    sql += "select CodHTS6 as Nandina2, CodHTS6 as Nandina, replace(convert(char(13), CodHTS6), ' ', '_') + ' [" + lTodos + " 6] ' + substring(" + HTS6 + ", 1, 80) as Partida ";
                    sql += "from HTS6 H, Partida_" + codPaisT + " P where H.CodHTS6 = substring(P.Nandina, 1, 6) and CodHTS6 like '" + nandina + "%' union ";
                }
                sql += "select Nandina as Nandina2, Nandina, replace(convert(char(13), Nandina), ' ', '_') + ' ' + substring(" + partida + ", 1, 80) as Partida ";
                sql += "from Partida_" + codPaisT + " ";
                sql += "where Nandina like '" + nandina + "%' or " + partida + " like '%" + nandina + "%' ";
                sql += "order by Nandina";

                dtRespuesta = Conexion.SqlDataTable(sql);

            }
            catch (Exception e)
            {
                dtRespuesta = null;
            }

            return dtRespuesta;
        }

        public static string StripQuotes(string text, char quote, string unescape)
        {
            string with = quote.ToString();
            if (quote != '\0')
            {
                // check if text contains quote character at all
                if (text.Length >= 2 && text.StartsWith(with) && text.EndsWith(with))
                {
                    text = text.Trim(quote);
                }
            }
            if (!string.IsNullOrEmpty(unescape))
            {
                text = text.Replace(unescape, with);
            }
            return text;
        }

        public static List<Empresas> BuscaEmpresas(string empresa, string codPais)
        {
            string sql;

            string[] paisesCondicion = {"PE", "PEB", "PEP"};

            string tabla = (paisesCondicion.Contains(codPais)) ? "Empresa_PE_con_datos" : "Empresa_" + codPais;

            string ruc = (paisesCondicion.Contains(codPais) || codPais == "CL" || codPais == "EC") ? "RUC" : "'' as RUC";

            string condRuc = (paisesCondicion.Contains(codPais) || codPais == "CL" || codPais == "EC") ? "or RUC like @empresa+'%'" : "";

            sql = "select * from (select top 20 1 as Orden, IdEmpresa, Empresa, " + ruc + " from " + tabla + " ";
            sql += "where Empresa like @empresa+'%' " + condRuc + " order by Empresa) T union ";

            sql += "select * from (select top 20 2 as Orden, IdEmpresa, Empresa, " + ruc + " from " + tabla + " ";
            sql += "where 1 = 1 ";

            foreach (string Emp in empresa.Split(' '))
            {
                string Emp2 = Emp.Replace("'", "''");
                sql += "and Empresa like '%" + Emp2 + "%' ";
            }

            sql += "and IdEmpresa not in (select IdEmpresa from " + tabla + " where Empresa like @empresa+'%') ";
            sql += "order by Empresa) T2 order by 1, 3 ";

            

            /*DataTable dtRespuesta;
            try
            {
                dtRespuesta = Conexion.SqlDataTable(sql);
            }
            catch (Exception e)
            {
                dtRespuesta = null;
            }*/

            var v = new List<Empresas>();

            using (var db = new ConexProvider().Open)
            {
                v = db.Query<Empresas>(sql, new { empresa = empresa }).ToList();
                
            }

            return v;
        }
    }

    //public class FlagVarVisibles
    //{
    //    private readonly string[] _paisesCondicionViaTransp = new[] { "BO", "MXD", "PY", "US", "UE" };
    //    private readonly string[] _paisesCondicionAduana = new[] { "CN", "IN", "PY", "US", "UE" };
    //    private readonly string[] _paisesCondicionDua = new[] { "AR", "BR", "CO", "MX", "MXD", "UY" };

    //    public bool ExisteImportador, ExisteNotificado, ExisteProveedor, ExistePaisOrigen;
    //    public bool ExisteExportador, ExisteImportadorExp, ExistePaisDestino;
    //    public bool ExistePtoDescarga, ExistePtoEmbarque, ExistePtoDestino;
    //    public bool ExisteManifiesto, ExisteDesComercial, ExisteDesAdicional;
    //    public bool ExistePartida, ExisteAduana;
    //    public bool ExisteViaTransp, ExisteDUA, ExisteDistrito;
    //    public bool IsManifiesto = false;
    //    public string CodPais = string.Empty;

    //    public FlagVarVisibles()
    //    {

    //    }
    //    public FlagVarVisibles(string CodPais, string TipoOpe, bool IsManifi = false)
    //    {
    //        this.CodPais = CodPais;
    //        this.IsManifiesto = IsManifi;
    //        if (!IsManifi)
    //        {
    //            ExisteImportador = Funciones.ExisteVariable(CodPais, TipoOpe, "IdImportador");
    //            ExisteProveedor = Funciones.ExisteVariable(CodPais, TipoOpe, "IdProveedor");
    //            ExistePaisOrigen = Funciones.ExisteVariable(CodPais, TipoOpe, "IdPaisOrigen");
    //            ExisteExportador = Funciones.ExisteVariable(CodPais, TipoOpe, "IdExportador");
    //            ExisteImportadorExp = Funciones.ExisteVariable(CodPais, TipoOpe, "IdImportadorExp");
    //            ExistePaisDestino = Funciones.ExisteVariable(CodPais, TipoOpe, "IdPaisDestino");
    //            ExisteViaTransp = GetExisteViaTransp();
    //            ExisteAduana = GetExisteAduana();
    //            ExisteDUA = GetExisteDua(ExisteAduana);
    //            ExisteDistrito = GetExisteDistrito();
    //            ExisteDesComercial = Funciones.FlagDesComercial(CodPais, TipoOpe);
    //            ExistePartida = true;
    //        }
    //        else
    //        {
    //            ExisteDesComercial = true;
    //            ExisteDesAdicional = (CodPais != "USE");
    //            ExisteImportador = (TipoOpe == "I");
    //            ExisteNotificado = (CodPais == "USI");
    //            ExisteProveedor = (TipoOpe == "I");
    //            ExistePaisOrigen = (TipoOpe == "I");
    //            ExisteExportador = (TipoOpe == "E");
    //            ExisteImportadorExp = (CodPais == "PEE");
    //            ExistePaisDestino = (TipoOpe == "E");
    //            ExistePtoDescarga = (CodPais == "USI");
    //            ExistePtoEmbarque = (CodPais == "USE" || CodPais == "PEI");
    //            ExistePtoDestino = (CodPais == "PEE");
    //            ExisteManifiesto = (CodPais == "PEI" || CodPais == "PEE");

    //            ExistePartida = false;
    //            ExisteAduana = false;

    //        }
    //    }

    //    private bool GetExisteViaTransp()
    //    {
    //        return !_paisesCondicionViaTransp.Contains(CodPais);
    //    }

    //    private bool GetExisteAduana()
    //    {
    //        return !_paisesCondicionAduana.Contains(CodPais);
    //    }

    //    private bool GetExisteDua(bool existeAduana)
    //    {
    //        return existeAduana
    //               && !_paisesCondicionDua.Contains(CodPais)
    //               && HttpContext.Current.Session["Plan"].ToString() != "ESENCIAL";
    //    }

    //    private bool GetExisteDistrito()
    //    {
    //        return (CodPais == "CN" || CodPais == "US");
    //    }
    //}
    
}