using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Reflection;
using System.Text;
using System.Web.Hosting;
using System.Web.Mvc;
using Dapper;
using PagedList;
using Veritrade2018.App_Start;
using Veritrade2018.Helpers;
using Veritrade2018.Models;
using Veritrade2018.Models.Admin;
using Veritrade2018.Util;
using System.Web.SessionState;
using Microsoft.Ajax.Utilities;
using resx = Resources.Resources;
using System.Threading;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using System.IO.Compression;
using System.Net;
using System.Web.Http.Cors;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using System.Net.Http.Headers;

using RestSharp;
using System.Xml;

namespace Veritrade2018.Controllers.Admin
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public  class MisBusquedasController : BaseController
    {
        private readonly VeritradeAdmin.Seguridad _ws = new VeritradeAdmin.Seguridad();
        private string urlWeb = Properties.Settings.Default.UrlWeb;

        private string IdUsuario, TipoUsuarioT, CodPais, TipoOpe;
        private bool useRand = false;

        private bool ExistePaisDestino;
        private bool ExisteViaTransp, ExisteAduana, ExisteDUA, ExisteDistrito;
        private bool EsFreeTrial, OcultarVideo, BtnMostrarVideo, MostrarAviso;


        private const string PageID = "_Page";

        int CantRegMax = 200000, CantRegMaxExcel = 40000, CantRegMaxFreeTrial = 20, LimiteFiltros =10;

        private int ResumenTabsGridPageSize = 5;

        private int TabGridPageSize = 10;
        private int VerRegistrosPageSize = 10;
        private int MisFavoritosPageSize = 20;

        private readonly string[] _paisesCondicionViaTransp = new[] { "BO", "MXD", "PY", "US", "UE" };
        private readonly string[] _paisesCondicionAduana = new[] { "CN", "IN", "PY", "US", "UE","CA","GT","HN","SV" };
        private readonly string[] _paisesCondicionDua = new[] { "AR", "BR", "CO", "MX", "MXD", "UY", "RD" };
        private readonly string[] _paisesConInfoComplementaria = new[] { "BO", "BR", "CA", "CN", "CR", "GT", "HN", "IN", "MX", "RD", "SV", "UE", "US" };
        private readonly string[] _CodManifiestosModificado = new[] { "PEI", "PEE", "ECI", "ECE", "USI", "USE","BRI","BRE" };

        private string _CodManifiesto = "";

        public static bool ExisteImportador, ExisteProveedor, ExistePaisOrigen;
        public static bool ExisteExportador, ExisteImportadorExp, ExisteDesComercial;
        public static bool opcionFreeTrial;
        // GET: MisBusquedas

        public   ActionResult Index(string culture, string compra, string ruta)
        {   
            Extensiones.SetCookie("IsDemo", "0", TimeSpan.FromDays(-1));
            /* para prueba*/
            string url = Request.Url.AbsoluteUri;
            int pos = url.IndexOf("?");
            if (!string.IsNullOrEmpty(ruta))
            {
                url = url.Substring(0, pos) + ruta;
            }

            url = url.Replace("&amp;", "&");

            Dictionary<string, string> queryValues = Extensiones.GetQueryValues(url);
            if (pos != -1)
            {
                Session["url"] = url.Substring(pos, url.Length - pos);
            }
            Session["queryValues"] = queryValues;

            Console.WriteLine("DIC: >>>" + queryValues.Count );

            /* Session["queryValues"] = queryValues;
             Session["url"] = url.Substring(pos,url.Length-pos);*/

            bool opcion = (queryValues.ContainsKey("acc"));

            opcionFreeTrial = (queryValues.ContainsKey("op")) && queryValues["op"] == "ft";

            if (queryValues.ContainsKey("op") && queryValues["op"] == "et")
            {
                Session["IngresoComoFreeTrial"] = false;
            }

            if (Session["IngresoComoFreeTrial"] != null && (bool)Session["IngresoComoFreeTrial"])
            {
                opcionFreeTrial = true;
                //Session["TipoUsuario"] = "Free Trial";
            }

            

            Session["opcionFreeTrial"] = opcionFreeTrial;

            if (opcionFreeTrial) {
                Session["IngresoComoFreeTrial"] = true;
            }
                

            int obj = Session.Timeout;
            /* 
             */


            MostrarAviso = false;
            OcultarVideo = true;
            Session["BotonUtilizado"] = false;
            if (Session["IdUsuario"] == null)
                return RedirectToAction("Logout", "Common");

            //if (Session["IdUsuario"] == null)
            //{
            //    if (inicio == null)
            //        inicio = "";
            //    if (inicio != "")
            //        Validacion(txtCodUsuario, txtPassword, inicio);
            //    else
            //        return RedirectToAction("Index", "Home");

            //    if (Session["IdUsuario"] == null)
            //    {
            //        Response.BufferOutput = true;
            //        return RedirectToAction("Index", "Home");
            //    }
            //}
            OcultarVideo = FuncionesBusiness.OcultarVideo(Session["IdUsuario"].ToString());
            FuncionesBusiness.ActualizaContTemp(Session["IdUsuario"].ToString(), 4);
            

            if (string.IsNullOrEmpty(compra))
            {
                MostrarAviso = (FuncionesBusiness.ContTemp(Session["IdUsuario"].ToString()) < 4);

                string Origen = Funciones.ObtieneOrigen(Session["IdUsuario"].ToString());
                string[] auxArrayOrigen = { "ADEX", "ESAN", "ULIMA", "UPC", "UPN", "PUCP" };

                if (Array.Exists(auxArrayOrigen, x => x == Origen))
                    MostrarAviso = false;
            }



            IdUsuario = Session["IdUsuario"].ToString();
            //CambiaIdioma(); // usar razor para trabajar esta lógica

            //TipoUsuarioT = Session["TipoUsuario"].ToString();
            //if (TipoUsuarioT == "Free Trial" && Session["ShowVideoBusquedas"] == null)


            //{
            //    Session["ShowVideoBusquedas"] = "show";
            //}
            //else if (TipoUsuarioT == "Free Trial" && Session["ShowVideoBusquedas"] != null)

            //{
            //    OcultarVideo = true;
            //}

            if (!OcultarVideo)
            {
                Extensiones.SetCookie("OcultarVideo" + "_id"+ IdUsuario,
                    (Extensiones.ConvertToInt(Extensiones.GetCookie("OcultarVideo" + "_id" + IdUsuario)) + 1).ToString(), TimeSpan.FromDays(1));
                OcultarVideo = Extensiones.ConvertToInt(Extensiones.GetCookie("OcultarVideo" + "_id" + IdUsuario)) > 3;
            }

            //para habilitar visilibilidad de img excel
            bool ForzarLinkExcel = FuncionesBusiness.ForzarLinkExcel(IdUsuario);
            ViewData["ForzarLinkExcel"] = ForzarLinkExcel;
            string Idioma = culture;

            //var listaPais2 = getListaPaisesInicial(Idioma);


            string CodPaisIP = "";


            VeritradeAdmin.Seguridad ws = new VeritradeAdmin.Seguridad();
#if DEBUG

            string DireccionIP = Properties.Settings.Default.IP_Debug;
#else
            string DireccionIP = Request.ServerVariables["REMOTE_ADDR"];

#endif

            string Ubicacion = _ws.BuscaUbicacionIP2(DireccionIP, ref CodPaisIP);


            string valuePasises2 = "";
            IEnumerable<SelectListItem> listaPaises;

            if (opcion)
            {
                Session["CodPais"] = queryValues["p"];
                CodPais = Session["CodPais"].ToString();
            }
            else
            {
                if (Session["CodPais"] != null)
                {
                    CodPais = Session["CodPais"].ToString();

                    if ((Session["Plan"].ToString() == "ESENCIAL" /*|| Session["Plan"].ToString() == "PERU UNO"*/) && !(bool)(Session["opcionFreeTrial"] ?? false))
                    {
                        CodPais = "PE";
                        Session["CodPais2"] = "1LAT";
                    }
                }
                else

                {
                    string[] auxArrayCodPaisIP = { "AR", "BO", "BR", "CL", "CO", "CR", "EC", "MX", "PA", "PY", "PE", "UY" };

                    if (Funciones.ValidaPais(Session["IdUsuario"].ToString(), CodPaisIP)
                        && !Funciones.FlagCarga(CodPaisIP)
                        && Array.Exists(auxArrayCodPaisIP, x => x == CodPaisIP))
                    {

                        CodPais = CodPaisIP;
                    }
                    else
                    {
                        CodPais = Funciones.ObtieneCodPaisAcceso(Session["IdUsuario"].ToString());
                        string[] auxArrayCodPais = { "PEI", "PEE", "USI", "USE" };

                        if (Array.Exists(auxArrayCodPais, x => x == CodPais))
                        {
                            Session["CodPais"] = CodPais.Substring(0, 2) + "_";

                            Session["TipoOpe"] = CodPais.Substring(2, 1);

                            //Response.Redirect("MisBusquedasUS.aspx");
                            RedirectToAction("Index", "MisBusquedas");
                        }
                    }
                }
            }

            
            if(Session["Plan"].ToString() == "PERU UNO")
            {
                if(Session["CodPais"].ToString() == "PEP" && CodPaisIP == "PE")
                {
                    CodPais = CodPaisIP;
                    Session["CodPais"] = CodPais;
                }
            }


            string CodPaisFlag = CodPais;
            string CodPaisAux = CodPais;
            TipoOpe = Session["TipoOpe"]?.ToString();

            if (opcion)
            {
                Session["TipoOpe"] = TipoOpe = queryValues["to"];
            }

            if (_CodManifiestosModificado.Contains(CodPaisFlag))
            {
                ValidaCodPaisManif(ref CodPaisFlag, TipoOpe);
            }

            if (!_CodManifiestosModificado.Contains(CodPais))
            {
                ValidaCodPaisManif(ref CodPais, TipoOpe);
            }

            //valuePasises2 = "1LAT"; //la primera vez carga por defecto el primer elemento de cboPaises2

            if (Session["CodPais2"] != null && !opcion)
            {
                valuePasises2 = Session["CodPais2"].ToString();
            }
            else
            {
                if(CodPaisFlag == "MXD" || CodPaisFlag == "MX")
                {
                    valuePasises2 = new ListaPaises().BuscarCodPais2(CodPaisFlag);//"7NOR";
                }
                else
                {
                    valuePasises2 = new ListaPaises().BuscarCodPais2(CodPaisFlag);
                }
                
                valuePasises2 = (valuePasises2 == "") ? "1LAT" : valuePasises2;
            }

            

            Session["CodPais2"] = valuePasises2;

            var listaAuxPaises = new ListaPaises().GetPaisesAdmin(culture, valuePasises2);
            var objSelectListItem = listaAuxPaises.Where(x => x.Value == CodPaisFlag).FirstOrDefault();
            if (objSelectListItem  == null)
            {
                CodPais = listaAuxPaises.First().Value;
            }

            //listaPaises = getListaPaises(Idioma, valuePasises2); -- no usado xq se llama directo desde la view
            //enviar el CodPais para la seleccion por defecto

            //desabilitar cboPais2 y cboPais usar la variable flagEnabledCboPais
            bool flagEnabledCboPais2AndPais = true;


            bool flagEnabledrdbImp = true;

            bool flagCheckedrdbImp = true;

            bool flagEnabledrdbExp = true;

            bool flagCheckedrdbExp = false;


            if (CodPais == "PEB")
            {
                flagCheckedrdbImp = true;


                flagEnabledrdbExp = false;



                flagCheckedrdbExp = false;

            }
            else
            {
                flagEnabledrdbExp = true;
            }

            if(!(bool)(Session["opcionFreeTrial"] ?? false))
            {
                if (Session["Plan"].ToString() == "PERU IMEX" || Session["Plan"].ToString() == "ECUADOR IMEX")
                {
                    flagEnabledCboPais2AndPais = false;
                }

                else if (/*Session["Plan"].ToString() == "PERU UNO" ||*/
                         Session["Plan"].ToString() == "ECUADOR UNO")

                {
                    flagEnabledCboPais2AndPais = false;

                    //flagEnabledrdbImp = flagCheckedrdbImp = (Session["TipoOpe"].ToString() == "I");

                    //flagEnabledrdbExp = flagCheckedrdbExp = (Session["TipoOpe"].ToString() == "E");
                    CodPais = ""; TipoOpe = "";
                    Funciones.BuscaDatosPlanEspecial(IdUsuario, ref CodPais, ref TipoOpe);
                    CodPais = CodPais.Substring(0, 2);
                    Session["CodPais"] = CodPais;
                    Session["TipoOpe"] = TipoOpe;

                    if (TipoOpe == "T")
                    {
                        flagEnabledrdbImp = flagEnabledrdbExp = flagCheckedrdbImp = true;
                        flagCheckedrdbExp = false;
                        Session["TipoOpe"] = "I";
                    }
                    else
                    {
                        flagEnabledrdbImp = flagCheckedrdbImp = (Session["TipoOpe"].ToString() == "I");

                        flagEnabledrdbExp = flagCheckedrdbExp = (Session["TipoOpe"].ToString() == "E");
                    }
                }
                else if (Session["Plan"].ToString() == "ESENCIAL" && !(bool)(Session["opcionFreeTrial"] ?? false))
                {
                    flagEnabledCboPais2AndPais = false;
                }
            }

            

            bool FlagCambioPais2 = Session["CodPais2"] != null && Session["CodPais2"].ToString() != valuePasises2;
            bool FlagCambioPais = Session["CodPais"] != null && Session["CodPais"].ToString() != CodPais;

            //bool FlagCambioTipope = Session["TipoOpe"] != null &&
            //                        ((Session["TipoOpe"].ToString() == "I" && flagCheckedrdbExp) ||
            //                         (Session["TipoOpe"].ToString() == "E" && flagCheckedrdbImp));

            //string cboPaisText = "";

            //bool rdbImpChecked = true;

            //bool rdbExpChecked = false;

            //if (FlagCambioPais2 || FlagCambioPais || FlagCambioTipope)
            //{               
            //    if (FlagCambioPais2)
            //    {
            //        //IdUsuario = Session["IdUsuario"].ToString();
            //        //CargaPaises();
            //        listaPaises = getListaPaises(Idioma, valuePasises2);
            //        //ValidaPais2();

            //        var oValidaPais2 = ValidaPais2(IdUsuario, valuePasises2, cboPaisText, Idioma, CodPais, rdbImpChecked);
            //    }
            //    else if (FlagCambioPais)
            //    {
            //        //IdUsuario = Session["IdUsuario"].ToString();
            //        var oValidaPais = ValidaPais(CodPais, cboPaisText, valuePasises2, IdUsuario, Idioma, rdbImpChecked);
            //    }

            //    else if (FlagCambioTipope)
            //    {
            //        //ValidaTipoOpe();
            //        var oValidaTipoOpe = ValidaTipoOpe(rdbImpChecked, rdbExpChecked, CodPais, Idioma, valuePasises2);
            //    }
            //}
            if (opcion)
            {
                Session["TipoOpe"] = queryValues["to"];

                flagCheckedrdbImp = Session["TipoOpe"].ToString() == "I";
                flagCheckedrdbExp = Session["TipoOpe"].ToString() == "E";

            }
            else
            {
                if (CodPaisAux.Contains("_"))
                {
                    Session["TipoOpe"] = CodPais.Substring(2, 1);
                }
                else
                {

                    if(Session["TipoOpe"] == null)
                    {
                        if (flagCheckedrdbImp)
                            Session["TipoOpe"] = "I";

                        else
                            Session["TipoOpe"] = "E";
                    }
                    else
                    {
                        flagCheckedrdbImp = Session["TipoOpe"].ToString() == "I";
                        flagCheckedrdbExp = Session["TipoOpe"].ToString() == "E";
                    }

                    
                }
            }


          

            //CodPais = Session["CodPais"].ToString();

            if (valuePasises2 == "4UE")
                CodPais = "UE";

            TipoOpe = Session["TipoOpe"].ToString();

            if (TipoOpe == "T")
            {
                Session["TipoOpe"] = "I";
                flagCheckedrdbImp = true;
                TipoOpe = "I";
            }
                

            //Session["CodPais2"] = valuePasises2;
            //Session["CodPais"] = CodPais;


            MiBusqueda objMisBusqueda = new MiBusqueda
            {
                Importador = Funciones.ExisteVariable(CodPais, TipoOpe, "IdImportador"),

                Proveedor = Funciones.ExisteVariable(CodPais, TipoOpe, "IdProveedor"),

                PaisOrigen = Funciones.ExisteVariable(CodPais, TipoOpe, "IdPaisOrigen"),

                Exportador = Funciones.ExisteVariable(CodPais, TipoOpe, "IdExportador"),


                ImportadorExp = Funciones.ExisteVariable(CodPais, TipoOpe, "IdImportadorExp"),



                TipoOpe = TipoOpe,

                CodPais = CodPais,


                CodPais2 = valuePasises2
            };


            ExistePaisDestino = Funciones.ExisteVariable(CodPais, TipoOpe, "IdPaisDestino");



            ExisteViaTransp = (CodPais != "BO" && CodPais != "MXD" && CodPais != "PY"

                               && CodPais != "US" && CodPais != "UE");

            ExisteAduana = GetExisteAduana(CodPais);

            ExisteDUA = GetExisteDua(ExisteAduana, CodPais);



            if (Session["Plan"].ToString() == "ESENCIAL" && !(bool)(Session["opcionFreeTrial"] ?? false))
            {
                objMisBusqueda.ImportadorExp = false;
                
            }

            ExisteDistrito = (CodPais == "CN" || CodPais == "US");


            objMisBusqueda.FlagDescComercialB = Funciones.FlagDesComercial(CodPais, TipoOpe);




            objMisBusqueda.EsFreeTrial = (Session["TipoUsuario"].ToString() == "Free Trial") || (bool)(Session["opcionFreeTrial"] ?? false);

            

            //if (!objMisBusqueda.EsFreeTrial)

            objMisBusqueda.RangoInfoEnLinea = FuncionesBusiness.InfoEnLinea(CodPais, TipoOpe, Idioma, objMisBusqueda.EsFreeTrial);

            if (opcionFreeTrial)
            {
                objMisBusqueda.RangoInfoEnLineaFree = FuncionesBusiness.InfoEnLinea(CodPais, TipoOpe, Idioma, false);
            }

            int LimiteVisitas = 0, Visitas = 0;

            Funciones.ValidaVisitasMes(IdUsuario, ref LimiteVisitas, ref Visitas);

            BtnMostrarVideo = !EsFreeTrial || (EsFreeTrial && Visitas > 1);



            //if (!IsPostBack || FlagCambioPais2 || FlagCambioPais || FlagCambioTipope)
            //{
            //    Restablecer();
            //}

            // para la primera vez, es decir cuando no es postback

            //objMisBusqueda.Periodos = CargaPeriodos(CodPais, TipoOpe, Idioma);

           


            objMisBusqueda.Periodos = CargaPeriodos(CodPais, TipoOpe);


            var restablecer = Restablecer(CodPais, valuePasises2, TipoOpe, Idioma);

            string OrigenAux = Funciones.ObtieneOrigen(IdUsuario);

            if (Session["Plan"].ToString() == "UNIVERSIDADES" && OrigenAux != "DELOITTE" && OrigenAux != "ITP")


            {
                objMisBusqueda.MisPartidas = false;

                objMisBusqueda.MisImportadores = false;

                objMisBusqueda.MisExportadores = false;

                objMisBusqueda.MisProveedores = false;

                objMisBusqueda.MisImportadoresExp = false;


                //lnkFavPartidas.Visible = lnkFavImportadores.Visible = lnkFavExportadores.Visible =
                //    lnkFavProveedores.Visible = lnkFavImportadoresExp.Visible = false;
                //lnkAgregarAFavPartidas.Visible = lnkAgregarAFavPartidas2.Visible = false;
                //lnkAgregarAFavImportadores.Visible = lnkAgregarAFavImportadores2.Visible = false;
                //lnkAgregarAFavProveedores.Visible = lnkAgregarAFavProveedores2.Visible = false;
            }

            List<object> nuevoFiltro = new List<object>();

            if (opcion)
            {
                //string textOption, valueOption;
                if (queryValues["acc"] == "ACP")
                {
                    nuevoFiltro.Add(AgregaPalabrasFiltros("[" + queryValues["idFav"] + "]", "3"+ queryValues["to"] + "_", queryValues["p"], queryValues["to"],
                        "",
                        queryValues["idFav"]));

                    if (queryValues.ContainsKey("idFav2"))
                    {
                        /*string nandina = FuncionesBusiness.BuscaNandina(queryValues["idFav2"], queryValues["p"]);
                        nuevoFiltro.Add(AgregaPalabrasFiltros(nandina, "2P_", queryValues["p"], queryValues["to"], GetCurrentIdioma()));*/
                        BuscarFavoritos(IdUsuario, queryValues["acc"], queryValues["p"], queryValues["idFav"], queryValues["to"], nuevoFiltro);
                    }
                    else
                    {
                        BuscarFavoritos(IdUsuario, queryValues["acc"], queryValues["p"], queryValues["idFav"], queryValues["to"],nuevoFiltro);
                    }
                }
                else if(queryValues["acc"] == "APC")
                {
                    string nandina = FuncionesBusiness.BuscaNandina(queryValues["idFav"], queryValues["p"]);
                    nuevoFiltro.Add(AgregaPalabrasFiltros(nandina, "2P_", queryValues["p"], queryValues["to"], GetCurrentIdioma()));

                    if (queryValues.ContainsKey("idFav2"))
                    {
                        /*nuevoFiltro.Add(AgregaPalabrasFiltros("[" + queryValues["idFav2"] + "]", "3" + queryValues["to"] + "_", queryValues["p"], queryValues["to"],
                            "",
                            queryValues["idFav2"]));*/
                        BuscarFavoritos(IdUsuario, queryValues["acc"], queryValues["p"], queryValues["idFav"], queryValues["to"], nuevoFiltro);
                    }
                    else
                    {
                        BuscarFavoritos(IdUsuario, queryValues["acc"], queryValues["p"], queryValues["idFav"], queryValues["to"], nuevoFiltro);
                    }
                }
                else if (queryValues["acc"] == "AMP")
                {
                    string nandina = FuncionesBusiness.BuscaNandina(queryValues["idFav"], queryValues["p"]);
                    nuevoFiltro.Add(AgregaPalabrasFiltros(nandina, "2P_", queryValues["p"], queryValues["to"], GetCurrentIdioma()));
                }
                else if(queryValues["acc"] == "AMC"){
                    nuevoFiltro.Add(AgregaPalabrasFiltros("[" + queryValues["idFav"] + "]", "3" + queryValues["to"] + "_", queryValues["p"], queryValues["to"],
                        "",
                        queryValues["idFav"]));
                }

                DateTime fechaDefault = Convert.ToDateTime(queryValues["fecha"]);

                objMisBusqueda.Periodos.DefaultFechaInfoFin = Convert.ToDateTime(fechaDefault.Year + "-" + fechaDefault.Month + "-01 00:00:00");

            }

           
            ViewData["InDesarrollo"] = Properties.Settings.Default.TableVarGeneral_InDev;

            ValidaCodPaisManif(ref CodPais, TipoOpe);

            Session["culture"] = culture;
            Session["CodPais2"] = valuePasises2;
            Session["CodPais"] = CodPais;
            
            //ViewData["CodPais"] = Session["CodPais"].ToString();

            ViewData["CodPais"] = CodPais;
            ViewData["CodPais2"] = valuePasises2; //Se usa en las vistas parciales incialmente
            ViewData["listFilter"] = nuevoFiltro;
            

            ViewData["MostrarAviso"] = MostrarAviso;
            ViewData["OcultarVideo"] = OcultarVideo;

            ViewData["flagEnabledCboPais2AndPais"] = flagEnabledCboPais2AndPais;
            ViewData["flagCheckedrdbImp"] = flagCheckedrdbImp ;
            ViewData["flagCheckedrdbExp"] = flagCheckedrdbExp;
            ViewData["flagEnabledrdbImp"] = flagEnabledrdbImp;
            ViewData["flagEnabledrdbExp"] = flagEnabledrdbExp;
            ViewData["objMiBusqueda"] = objMisBusqueda;
            ViewData["filtrosTabs"] = GetFiltersNames();
            ViewData["IngresoComoFreeTrial"] = Session["IngresoComoFreeTrial"] != null && (bool)Session["IngresoComoFreeTrial"];

            ViewData["CantidadAlertas"] = "";

            string plan = Session["Plan"].ToString();
            ViewBag.nombreUsuario = Funciones.BuscaUsuario(IdUsuario);
            ViewBag.tipoUsuario = (string)Session["TipoUsuario"]??"";
            //ViewBag.tipoUsuario2 = (string)Session["TipoUsuario"] ?? "";
            ViewBag.plan = plan;
            Popups popups = new Popups();
            int idOrigen = Funciones.ObtieneIdOrigenUsuario(IdUsuario);
            if (idOrigen == 53 || idOrigen == 75 || idOrigen == 98 || idOrigen == 102 || idOrigen == 106 ||
                idOrigen == 107 || idOrigen == 111 || idOrigen == 114)
            {
                popups = popups.GetPopupUniversidad();
                if (popups != null) {
                    switch(idOrigen) {
                        case 53:
                            popups.Texto = popups.Texto.Replace("@UNIVERSIDAD", "Universidad de Lima");
                            break;
                        case 75:
                            popups.Texto = popups.Texto.Replace("@UNIVERSIDAD", "ESAN");
                            break;
                        case 98:
                            popups.Texto = popups.Texto.Replace("@UNIVERSIDAD", "ADEX");
                            break;
                        case 102:
                            popups.Texto = popups.Texto.Replace("@UNIVERSIDAD", "Universidad Privada del Norte");
                            break;
                        case 106:
                            popups.Texto = popups.Texto.Replace("@UNIVERSIDAD", "UPC");
                            break;
                        case 107:
                            popups.Texto = popups.Texto.Replace("@UNIVERSIDAD", "UPT");
                            break;
                        case 111:
                            popups.Texto = popups.Texto.Replace("@UNIVERSIDAD", "Universidad Continenal");
                            break;
                        case 114:
                            popups.Texto = popups.Texto.Replace("@UNIVERSIDAD", "CENTRUM");
                            break;
                        default:
                            popups.Texto = popups.Texto.Replace("@UNIVERSIDAD", OrigenAux);
                            break;
                    }
                }
            }
            else
            {
                popups = popups.GetPopup(culture);
                if (popups != null)
                {
                    if (Extensiones.GetCookie("OcultarModalId").ToString() != popups.Id.ToString())
                    {
                        Extensiones.SetCookie("OcultarModalId", popups.Id.ToString(), TimeSpan.FromDays(1));
                        Extensiones.SetCookie("OcultarModal" + "_id" + IdUsuario, "0", TimeSpan.FromDays(1));
                    }

                    Extensiones.SetCookie("OcultarModal" + "_id" + IdUsuario,
                        (Extensiones.ConvertToInt(Extensiones.GetCookie("OcultarModal" + "_id" + IdUsuario)) + 1).ToString(), TimeSpan.FromDays(1));
                    //OcultarVideo = Extensiones.ConvertToInt(Extensiones.GetCookie("OcultarModal" + "_id" + IdUsuario)) > 3;

                    /*if (Session["PopupView"] == null)
                    {
                        Session["PopupView"] = 1;
                    }
                    else
                    {
                        Session["PopupView"] = (int)Session["PopupView"] + 1;
                    }

                    if ((int)Session["PopupView"] > 3)
                    {
                        popups = null;
                    }*/

                    if (Extensiones.ConvertToInt(Extensiones.GetCookie("OcultarModal" + "_id" + IdUsuario)) > 4)
                    {
                        popups = null;
                    }
                    else
                    {
                        Extensiones.SetCookie("OcultarModalId", "", TimeSpan.FromDays(1));
                    }
                }
            

            }
            
            
            


            ViewData["popups"] = popups;
            return View();
        }

        private void GuardarLogInicioErrorSession(string metodo)
        {
            try
            {

                if (Session["sqlFiltro"] == null)
                {
                    /*bool tipoServidor = Properties.Settings.Default.TableVarGeneral_InDev;
                    string path = ConfigurationManager.AppSettings["directorio_logs"];
                    Logs oLog = new Logs(path);
                    try
                    {
                        oLog.Add("IdUsuario => " + Session["idUsuario"].ToString(), "-N-" +Session["idUsuario"].ToString());
                        oLog.Add("Servidor => " + (tipoServidor ? "Desarrollo" : "Produccion"), "-N-" + Session["idUsuario"].ToString());
                        oLog.Add("Controlador => MisBusquedas", "-N-" + Session["idUsuario"].ToString());
                        oLog.Add("Metodo => " + metodo, "-N-" + Session["idUsuario"].ToString());
                        oLog.Add("La Session sqlFiltro es nula => ", "-N-" + Session["idUsuario"].ToString());
                    }
                    catch (Exception ex)
                    {
                        oLog.Add("Excepcion => " + ex.ToString(), "-N-" + Session["idUsuario"].ToString());
                    }*/
                }
                else if (Session["sqlFiltro"].ToString().Contains("'%'"))
                {
                    /*bool tipoServidor = Properties.Settings.Default.TableVarGeneral_InDev;
                    string path = ConfigurationManager.AppSettings["directorio_logs"];
                    Logs oLog = new Logs(path);
                    try
                    {
                        oLog.Add("IdUsuario => " + Session["idUsuario"].ToString(), "-N-" + Session["idUsuario"].ToString());
                        oLog.Add("Servidor => " + (tipoServidor ? "Desarrollo" : "Produccion"), "-N-" + Session["idUsuario"].ToString());
                        oLog.Add("Controlador => MisBusquedas", "-N-" + Session["idUsuario"].ToString());
                        oLog.Add("Consulta Session => " + Session["sqlFiltro"].ToString(), "-N-" + Session["idUsuario"].ToString());
                        oLog.Add("Metodo => " + metodo, "-N-" + Session["idUsuario"].ToString());
                        oLog.Add("La Session sqlFiltro contine una busqueda incorrecta => ", "-N-" + Session["idUsuario"].ToString());
                    }
                    catch (Exception ex)
                    {
                        oLog.Add("Excepcion => " + ex.ToString(), "-N-" + Session["idUsuario"].ToString());
                    }*/
                }

            }
            catch(Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }

        }

        private void BuscarFavoritos(string idUsuario, string IdTipoAlerta, string codPais, string IdFavorito, string TipoOpe, List<object> nuevoFiltro)
        {
            List<string> listaFavoritos = FuncionesBusiness.SearchMyFavoriteAlertPorIdPadre(idUsuario, IdTipoAlerta, codPais, IdFavorito, TipoOpe);

           

            string tipoFil = "";
            string tipo = "";


            foreach (var item in listaFavoritos)
            {
                
                if(IdTipoAlerta == "APC")
                {
                    tipo = "3" + TipoOpe + "_";
                    tipoFil = "[" + item + "]";
                    nuevoFiltro.Add(AgregaPalabrasFiltros(tipoFil, tipo, codPais, TipoOpe,
                            "",
                            item));
                }
                else
                {
                    tipo = "2P_";
                     tipoFil = FuncionesBusiness.BuscaNandina(item, codPais);
                    nuevoFiltro.Add(AgregaPalabrasFiltros(tipoFil, "2P_", codPais, TipoOpe, GetCurrentIdioma()));
                }

            }

        }

        void ValidExistData(string codPais, string tipoOpe)
        {
            ExisteDesComercial = Funciones.FlagDesComercial(codPais, tipoOpe);
            ExisteImportador = Funciones.ExisteVariable(codPais, tipoOpe, "IdImportador");
            ExisteProveedor = Funciones.ExisteVariable(codPais, tipoOpe, "IdProveedor");
            ExistePaisOrigen = Funciones.ExisteVariable(codPais, tipoOpe, "IdPaisOrigen");
            ExistePaisDestino = Funciones.ExisteVariable(codPais, tipoOpe, "IdPaisDestino");
            ExisteExportador = Funciones.ExisteVariable(codPais, tipoOpe, "IdExportador");
            ExisteImportadorExp = Funciones.ExisteVariable(codPais, tipoOpe, "IdImportadorExp");
            if (Session["Plan"].ToString() == "ESENCIAL" && !(bool)(Session["opcionFreeTrial"] ?? false))
            {
                ExisteImportadorExp = false;
            }
        }

        [HttpPost]
        public ActionResult Index(string inicio, string txtCodUsuario, string txtPassword,
            string compra, string ruta)
        {
            Extensiones.SetCookie("Mb", "1", TimeSpan.FromDays(360));
            Extensiones.SetCookie("IsDemo", "0", TimeSpan.FromDays(-1));
            //Console.WriteLine("MB >>> " + Session["IdUsuario"]);


            var cultura = RouteData.Values["culture"] as string;  //cultura
            if (!string.IsNullOrEmpty(ruta))
            {
                ruta = ruta.Replace("&amp;", "&");
            }
            
            if (Session["IdUsuario"] == null)
            {
                if (string.IsNullOrEmpty(inicio) ||
                    string.IsNullOrEmpty(txtCodUsuario) ||
                    string.IsNullOrEmpty(txtPassword))
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    //Si la validación es correcta se asigna los valores de sesión como por ejemplo el IdUsuario
                    Validacion(txtCodUsuario, txtPassword, inicio);
                }

                if (Session["IdUsuario"] == null)
                {
                    Response.BufferOutput = true;
                    if (Session["Error"] != null)
                    {
                        return RedirectToAction("Mostrar", "Error", new { cultura  });
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                //else
                //{
                //    return RedirectToAction("Index", "MisBusquedas", new { compra });
                //}
                else
                {
                    Extensiones.SetCookie("XT", Session["IdUsuario"].ToString(), TimeSpan.FromDays(360));
                    if (!string.IsNullOrEmpty(ruta  ))
                    {
                        string CodPaisIP = "";


                        VeritradeAdmin.Seguridad ws = new VeritradeAdmin.Seguridad();
#if DEBUG

                        string DireccionIP = Properties.Settings.Default.IP_Debug;
#else
            string DireccionIP = Request.ServerVariables["REMOTE_ADDR"];

#endif

                        _ws.BuscaUbicacionIP2(DireccionIP, ref CodPaisIP);

                        if (Session["CodPais"] == null)
                        {
                            string[] auxArrayCodPaisIP = { "AR", "BO", "BR", "CL", "CO", "CR", "EC", "MX", "PA", "PY", "PE", "UY" };

                            if (Funciones.ValidaPais(Session["IdUsuario"].ToString(), CodPaisIP)
                                && !Funciones.FlagCarga(CodPaisIP)
                                && Array.Exists(auxArrayCodPaisIP, x => x == CodPaisIP))
                            {

                                CodPais = CodPaisIP;
                            }
                            else
                            {
                                CodPais = Funciones.ObtieneCodPaisAcceso(Session["IdUsuario"].ToString());
                            }
                            Session["CodPais"] = CodPais;
                        }
                        var idPlan = Funciones.ObtieneIdPlan(Session["IdUsuario"].ToString());

                        if (ruta.ToLower().Contains("acc") && (ruta.ToLower().Contains("acp") || ruta.ToLower().Contains("apc")))
                            return RedirectToAction("Index", "MisBusquedas", new { compra, ruta });
                        else if (ruta.ToLower().Contains("acc") && ruta.ToLower().Contains("amp"))
                        {
                            if(idPlan == "79")
                                return RedirectToAction("Index", "MisBusquedas", new { compra, ruta });
                            else
                                return RedirectToAction("Index", "MisProductos", new { compra, ruta });
                        }                            
                        else if (ruta.ToLower().Contains("acc") && ruta.ToLower().Contains("amc"))
                        {
                            //return RedirectToAction("Index", "MisCompanias", new { compra, ruta });
                            if (idPlan == "79")
                                return RedirectToAction("Index", "MisBusquedas", new { compra, ruta });
                            else
                                return RedirectToAction("Index", "MisCompanias", new { compra, ruta });
                        }                            
                        else if (ruta.ToLower().Contains("acc") && ruta.ToLower().Contains("ss"))
                            return RedirectToAction("Index", "MisAlertasFavoritos", new { compra, ruta });
                        else
                            return RedirectToAction("Index", "MisBusquedas", new { compra });
                    }
                    else
                    {
                        return RedirectToAction("Index", "MisBusquedas", new { compra });
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", "MisBusquedas", new { compra });
            }
        }

        public ActionResult Index2(string inicio, string txtCodUsuario2, string txtPassword2,
            string compra, string ruta)
        {
            var cultura = RouteData.Values["culture"] as string;  //cultura
            if (Session["IdUsuario"] == null)
            {
                if (string.IsNullOrEmpty(inicio) ||
                    string.IsNullOrEmpty(txtCodUsuario2) ||
                    string.IsNullOrEmpty(txtPassword2))
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    //Si la validación es correcta se asigna los valores de sesión como por ejemplo el IdUsuario
                    Validacion(txtCodUsuario2, txtPassword2, inicio);
                }

                if (Session["IdUsuario"] == null)
                {
                    Response.BufferOutput = true;
                    if (Session["Error"] != null)
                    {
                        return RedirectToAction("Mostrar", "Error", new { cultura });
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    return RedirectToAction("Index", "MisBusquedas", new { compra, ruta });
                }
            }
            else
            {
                return RedirectToAction("Index", "MisBusquedas", new { compra });
            }
        }

        private ActionResult Validacion(string txtCodUsuario, string txtPassword, string inicio)
        {
            
            string IdUsuario = "", IdAplicacion = "", CodSeguridad = "";
            int CantUsuariosUnicos = 0;

            string CodUsuario = txtCodUsuario.ToUpper();

#if DEBUG
            string DireccionIP = Properties.Settings.Default.IP_Debug;

#else
            string DireccionIP = Request.ServerVariables["REMOTE_ADDR"];
#endif
            if (Funciones.BuscaCodEstado(CodUsuario) == "I")
            {
                Session["Error"] = "INACTIVO";
                //return RedirectToAction("Logout", "Common");
                return RedirectToAction("Index", "Error");
            }

            bool Valida = false;
//#if DEBUG
            Valida = Funciones.Valida2(CodUsuario, txtPassword, ref IdUsuario, ref IdAplicacion, ref CodSeguridad,
                ref CantUsuariosUnicos);
//#else
//            Valida = _ws. /*Funciones.*/Valida2(CodUsuario, txtPassword, ref IdUsuario, ref IdAplicacion, ref CodSeguridad,
//                ref CantUsuariosUnicos);
//#endif

            if (!Valida)
            {
                Session["Error"] = "INCORRECTO";
                //return RedirectToAction("Logout", "Common");
                return RedirectToAction("Index", "Error");
            }

            string Origen = Funciones.ObtieneOrigen(IdUsuario);


            if (Valida && (CodUsuario == "UPC" || Origen == "ULIMA" || Origen == "ESAN" || Origen == "ADEX" ||
                           Origen == "UPN") &&

                inicio != "referid0")
            {
                Session["Error"] = "INCORRECTO";

                //return RedirectToAction("Logout", "Common");
                return RedirectToAction("Index", "Error");
            }


            if (CodSeguridad == "IP" && DireccionIP != Properties.Settings.Default.IP_Veritrade &&
                !_ws.ValidaIPPais(IdUsuario, DireccionIP))

            {
                Session["Error"] = "OTRO_PAIS";
                //return RedirectToAction("Logout", "Common");
                return RedirectToAction("Index", "Error");
            }

            //if (CodSeguridad != "Off")
            {
                if (Funciones.SessionUnica(CodUsuario) && Funciones.ExisteUsuarioEnLinea(IdUsuario))
                {
                    Session["Error"] = "SESION_UNICA";
                    Session["CodUsuarioTemp"] = CodUsuario;
                    //return RedirectToAction("Logout", "Common");
                    return RedirectToAction("Index", "Error");
                }
            }
            

            


            int LimiteVisitas = 0, Visitas = 0;

            if (!Funciones.ValidaVisitasMes(IdUsuario, ref LimiteVisitas, ref Visitas))
            {

                Session["Error"] = "LIMITE_VISITAS";
                //return RedirectToAction("Logout", "Common");
                return RedirectToAction("Index", "Error");
            }

            FuncionesBusiness.GrabaHistorial(IdUsuario, DireccionIP, Request.ServerVariables["HTTP_USER_AGENT"], "");


            Session["IdUsuario"] = IdUsuario;
            

            var onlineUser = new OnlineUsers
            {
                IdSesion = Session.SessionID,
                IdUsuario = Session["IdUsuario"].ToString()

            };

            ActiveSessions.Sessions.AddOrUpdate(Session.SessionID, onlineUser, (k, v) => v = onlineUser);

            Session["TipoUsuario"] = Funciones.BuscaTipoUsuario(IdUsuario);
            Session["Plan"] = Funciones.ObtienePlan(IdUsuario).ToUpper();
            Session["Origen"] = Funciones.ObtieneOrigen(IdUsuario).ToUpper();

            // Ruben 202209
            if (Session["Origen"].ToString() == "ULIMA" || Session["Origen"].ToString() == "UPC" || Session["Origen"].ToString() == "ESAN" || Session["Origen"].ToString() == "ADEX" ||
                Session["Origen"].ToString() == "UPN" || Session["Origen"].ToString() == "UPT" || Session["Origen"].ToString() == "UNIV" || Session["Origen"].ToString() == "PUCP" || 
                Session["Origen"].ToString() == "CERTUS" || Session["Origen"].ToString() == "SISE" || Session["Origen"].ToString() == "UCV") 
                    Session["Plan"] = "UNIVERSIDADES";
            else if (Session["Plan"].ToString() == "ESENCIAL" || Session["Plan"].ToString() == "PERU UNO" || Session["Plan"].ToString() == "ECUADOR UNO" || 
                Session["Plan"].ToString() == "PERU IMEX" || Session["Plan"].ToString() == "ECUADOR IMEX")
            {
                string CodPais = "", TipoOpe = "";
                Funciones.BuscaDatosPlanEspecial(IdUsuario, ref CodPais, ref TipoOpe);
                Session["CodPais"] = CodPais;
                Session["TipoOpe"] = TipoOpe;
            }

            return null;
        }

        private object ValidacionCambioCodPais(string codPais)
        {
            bool flagCheckedrdbImp = true;
            bool flagEnabledrdbExp = true;
            bool flagCheckedrdbExp = false;
            if (codPais == "PEB")
            {
                flagCheckedrdbImp = true;
                flagEnabledrdbExp = false;
                flagCheckedrdbExp = false;
            }
            else
            {
                flagEnabledrdbExp = true;
            }

            return new
            {
                flagCheckedrdbImp,
                flagEnabledrdbExp,
                flagCheckedrdbExp
            };
        }

#region Método Auxiliares

        private void GetTablaAndDua(string tipoOpe, string codPais, ref string tabla, ref string dua)
        {
            if (tipoOpe == "I")
            {
                tabla = "Importacion_" + codPais;
                if (codPais == "PE" || codPais == "PEB" || codPais == "PEP")
                    dua = "NroCorre";
                else
                {
                    dua = codPais.ToUpper() == "EC" ? "Refrendo" : "DUA";
                }
            }
            else
            {
                tabla = "Exportacion_" + codPais;

                if (codPais == "PE" || codPais == "PEP")
                    dua = "NroOrden";
                else
                {
                    dua = codPais == "EC" ? "Refrendo" : "DUA";
                }
            }
        }

        string GetCIFTot(string codPais, string tipoOpe)
        {
            if (!IsManifiesto(codPais))
            {
                return Funciones.Incoterm(codPais, tipoOpe) + "Tot";
            }
            else { return "PesoBruto"; };
        }

        private void GetCifTot_PesoNeto_CantReg_ValueCifTot_Unidad_ValuePesoNeto(string codPais, string tipoOpe,
            string tabla, ref string cifTot, ref string pesoNeto,
            ref int cantReg, ref string valueCifTot, ref string unidad,
            ref decimal valuePesoNeto)
        {
            bool isManif = IsManifiesto(codPais);

            cifTot = GetCIFTot(codPais, tipoOpe);   //Funciones.Incoterm(codPais, tipoOpe) + "Tot";
            if (!isManif) pesoNeto = Funciones.CampoPeso(codPais, tipoOpe);

            DataRow drTotales =
                FuncionesBusiness.CalculaTotales(Session["SqlFiltro"].ToString(), cifTot, codPais, pesoNeto, tabla, isManif:isManif);

            cantReg = drTotales.GetValue<int>("CantReg");

            if (cantReg > 0)
            {
                valueCifTot = drTotales.GetValue<decimal>(cifTot).ToString();
                if (!isManif)
                {
                    int numRegistros = 0;

                    unidad = FuncionesBusiness.Unidades(Session["SqlFiltro"].ToString(), tabla, ref numRegistros);

                    if (numRegistros != 1 && pesoNeto != "")
                    {
                        unidad = "kg";
                    }

                    if (unidad != "kg")
                    {
                        valuePesoNeto = Convert.ToDecimal(drTotales["Cantidad"]);
                    }
                    else if (pesoNeto != "")
                    {
                        valuePesoNeto = Convert.ToDecimal(drTotales[pesoNeto]);
                    }
                }
                else
                {
                    if (pesoNeto != "")
                    {
                        valuePesoNeto = Convert.ToDecimal(drTotales[pesoNeto]);
                    }
                }
            }
        }

        private string GetFiltersNames()
        {
            var namesFiltros = Enum.GetNames(typeof(Enums.Filtro));
            return string.Join(",", namesFiltros);
        }

        private bool GetExisteViaTransp(string codPais)
        {
            return !_paisesCondicionViaTransp.Contains(codPais);
        }

        private bool GetExisteAduana(string codPais)
        {
            return !_paisesCondicionAduana.Contains(codPais);
        }

        private bool GetExisteDua(bool existeAduana, string codPais)
        {
            return existeAduana
                   && !_paisesCondicionDua.Contains(codPais)
                   && Session["Plan"].ToString() != "ESENCIAL";
        }

        private bool GetExisteDistrito(string codPais)
        {
            return (codPais == "CN" || codPais == "US");
        }

        private string GetRangoFechas(string fechaIni, string fechaFin, string idioma)
        {
            string rango;
            System.Globalization.CultureInfo culture;

            if (idioma == "es")
                culture = new System.Globalization.CultureInfo("es-PE");

            else
                culture = new System.Globalization.CultureInfo("en-US");

            var fechaIniT = fechaIni.Substring(0, 4) + "-" + fechaIni.Substring(4, 2) + "-01";
            fechaIniT = Convert.ToDateTime(fechaIniT).ToString("MMM-yyyy", culture).ToUpper();

            var fechaFinT = fechaFin.Substring(0, 4) + "-" + fechaFin.Substring(4, 2) + "-01";
            fechaFinT = Convert.ToDateTime(fechaFinT).ToString("MMM-yyyy", culture).ToUpper();

            if (idioma == "es")
                rango = "DE " + fechaIniT + " A " + fechaFinT;
            else
                rango = "FROM " + fechaIniT + " TO " + fechaFinT;

            return rango;
        }

        private string GetDescriptionRecordsFound(int cantReg, CultureInfo cultureInfo, string idioma)
        {
            if (cantReg > 0)
            {
                if (idioma == "en")
                {
                    return cantReg.ToString("n0", cultureInfo) + " " + Resources.Resources.RecordsFound_Text_Part2 +
                           " " + Resources.Resources.RecordsFound_Text_Part1;
                }
                return Resources.Resources.RecordsFound_Text_Part1 + " " + cantReg.ToString("n0", cultureInfo) +
                       " " + Resources.Resources.RecordsFound_Text_Part2;
            }
            else
            {
                return Resources.AdminResources.NoRecordsFound_Text;
            }
        }

        private string GetCurrentIdioma()
        {
            if (Session["Idioma"] != null)
            {
                return Session["Idioma"].ToString();
            }
            else
            {
                return "es";
            }
        }

        bool IsManifiesto(string codPais)
        {
            //AGREGANDO BRASIL MANIFIESTOS Y ECUADOR MANIFIESTOS
            return (new[] {"PEI", "USI", "PEE", "USE", "ECI", "ECE", "BRI", "BRE" }).Contains(codPais);
        }
        

        private MiBusqueda GetMiBusqueda(string codPais, string codPais2, string tipoOpe)
        {
            var obj = new MiBusqueda() { CodPais  = codPais};
            obj.IsManifiesto = this.IsManifiesto(codPais);
            //obj.Importador = !obj.IsManifiesto ? Funciones.ExisteVariable(codPais, tipoOpe, "IdImportador") : (tipoOpe == "I");
            //obj.Proveedor = !obj.IsManifiesto ?  Funciones.ExisteVariable(codPais, tipoOpe, "IdProveedor") : (tipoOpe == "I");
            //obj.PaisOrigen = Funciones.ExisteVariable(codPais, tipoOpe, "IdPaisOrigen");
            //obj.Exportador = !obj.IsManifiesto ? Funciones.ExisteVariable(codPais, tipoOpe, "IdExportador") : (tipoOpe == "E");
            //obj.ImportadorExp = !obj.IsManifiesto ? Funciones.ExisteVariable(codPais, tipoOpe, "IdImportadorExp") : (codPais == "PEE") ;
            //obj.FlagDescComercialB = !obj.IsManifiesto ? Funciones.FlagDesComercial(codPais, tipoOpe) : true;
            obj.TipoOpe = tipoOpe;
            obj.CodPais2 = codPais2;
            //obj.ExistePartida = false;

            return obj;
        }

        private void ValidaCodPais2(string codPais2, ref string codPais)
        {
            if (codPais2 == "4UE")
                codPais = "UE";
        }

#endregion

        private List<SelectListItem> getPaisesDatatableToListItem(DataTable dt)
        {
            var lista = new List<SelectListItem>();
            if(dt!= null)
                foreach (DataRow dr in dt.Rows)

                {
                    lista.Add(new SelectListItem { Text = dr["Pais"].ToString(), Value = dr["IdPais"].ToString() });
                }

            return lista;
        }

        private object ValidaPais2(string IdUsuario, string valueCboPais2, string cboPaisText,
            string Idioma, string valueCboPais, bool rdbImpChecked)
        {
            string CodPaisT, PaisT, CodPais2T;

            //value to default cboPais
            string cboPaisValue, Mensaje, cboPais2Value;
            bool flagMostrarMensaje = false;
            IEnumerable<SelectListItem> listaPaises;
            object oValidaPais2 = null;

            CodPais2T = valueCboPais2;
            CodPaisT = FuncionesBusiness.ObtieneCodPaisAcceso(IdUsuario, ref CodPais2T);
            if (CodPaisT != "")
                cboPaisValue = CodPaisT;
            else
            {
                PaisT = cboPaisText;
                if (valueCboPais2 == "4UE")
                    PaisT = "UE - " + PaisT;
                Mensaje = "El país seleccionado (" + PaisT + ") no está incluído en el plan contratado.";
                if (Idioma == "en")
                    Mensaje = "Selected country: " + PaisT + " is not included in your plan.";
                flagMostrarMensaje = true;
                CodPais2T = "";
                CodPaisT = FuncionesBusiness.ObtieneCodPaisAcceso(IdUsuario, ref CodPais2T);
                cboPais2Value = CodPais2T;

                listaPaises = new ListaPaises().GetPaisesAdmin(Idioma, cboPais2Value);

                cboPaisValue = CodPaisT;

                return oValidaPais2 = new
                {
                    flagMostrarMensaje,
                    Mensaje,
                    cboPaisValue,

                    cboPais2Value,
                    listaPaises
                };
            }


            if (valueCboPais == "US_" || valueCboPais == "PE_")

            {
                Session["CodPais2"] = valueCboPais2;
                Session["CodPais"] = valueCboPais;
                if (rdbImpChecked)
                    Session["TipoOpe"] = "I";
                else
                    Session["TipoOpe"] = "E";
                //Response.Redirect("MisBusquedasUS.aspx");
                RedirectToAction("Index", "Home");
            }

            return oValidaPais2 = new
            {
                cboPaisValue
            };
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult TipoOpeChange(string tipoOpe, string codPais, string codPais2)
        {
            ValidaCodPais2(codPais2, ref codPais);
            ValidaCodPaisManif(ref codPais, tipoOpe);
            IEnumerable<SelectListItem> listaPaises = new List<SelectListItem>();
            ListaPaises objListaPaises = new ListaPaises();

            string idioma = GetCurrentIdioma();

            if (tipoOpe.Equals("I"))
            {

                listaPaises = objListaPaises.GetPaisesAdmin(idioma, codPais2, true);
            }
            else
            {
                if (codPais == "PEB")
                {
                    listaPaises = objListaPaises.GetPaisesAdmin(idioma, codPais2, false);

                    codPais = "PE";
                }
            }

            MiBusqueda objMisBusqueda = GetMiBusqueda(codPais, codPais2, tipoOpe);

            if (Session["Plan"].ToString() == "ESENCIAL" && tipoOpe == "E" && !(bool)(Session["opcionFreeTrial"] ?? false)) 
            {
                objMisBusqueda.ImportadorExp = false;
            }

            bool tipoUsuarioEsFreeTrial = (Session["TipoUsuario"].ToString() == "Free Trial") || (bool)(Session["opcionFreeTrial"] ?? false);


            string rangoInfoEnLinea = "";
            //if (!tipoUsuarioEsFreeTrial)
                rangoInfoEnLinea = FuncionesBusiness.InfoEnLinea(codPais, tipoOpe, idioma, tipoUsuarioEsFreeTrial);

            Restablecer();

            Session["CodPais"] = codPais;

            if(tipoOpe != "")
                Session["TipoOpe"] = tipoOpe;

            bool tipoServidor = Properties.Settings.Default.TableVarGeneral_InDev;
            bool visibleInfoBO = false;
            if (tipoServidor)
                visibleInfoBO = ((Session["CodPais"].ToString() == "BO" && Session["TipoOpe"].ToString() == "I") && (Session["TipoUsuario"].ToString() == "Cliente" || Session["TipoUsuario"].ToString() == "Gratis"));
            else
                visibleInfoBO = ((Session["CodPais"].ToString() == "BO") && (Session["TipoUsuario"].ToString() == "Cliente" || Session["TipoUsuario"].ToString() == "Gratis"));
            return Json(new
            {
                listaPaises,
                //paisesOrigenOdestino = getPaisesDatatableToListItem(FuncionesBusiness.CargaPaises(codPais, codPais2, idioma)),
                vistaFiltros = RenderViewToString(this.ControllerContext, "FormFilterFields", objMisBusqueda),
                objPeriodos = CargaPeriodos(codPais, tipoOpe),
                tipoUsuarioEsFreeTrial,
                rangoInfoEnLinea,
                flagVisibleMisFiltros = (Session["Plan"].ToString() != "UNIVERSIDADES"),
                flagVisibleHlkArchivoAndHlkArchivo2 = false,
                visibleInfoBO
            });
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult CboPais2Change(string codPais2, string textoPais2, string textoPais, string tipoOpe,
            string idioma)
        {
            IEnumerable<SelectListItem> listaPaises = new ListaPaises().GetPaisesAdmin(idioma, codPais2);

            string idUsuario = Session["IdUsuario"].ToString();
            string codPaisAselecconar = "";
            object objMensaje = null;
            bool redirectUS = false;

            string CodPaisT, PaisT, CodPais2T, CodPais2AuxT;


            CodPais2T = codPais2;
            CodPaisT = listaPaises.FirstOrDefault().Value;

            if (codPais2 == "4UE")
            {
                textoPais = textoPais2;
                CodPaisT = "UE";
            }

            if (!(bool)(Session["opcionFreeTrial"] ?? false))
            {
                CodPaisT = FuncionesBusiness.ObtieneCodPaisAcceso(idUsuario, ref CodPais2T);                
            }

            codPaisAselecconar = CodPaisT;

            textoPais = listaPaises.FirstOrDefault().Text??"";

            

            if (CodPaisT != "")
            {
                PaisT = textoPais;
                
                if (Funciones.FlagCarga(CodPaisT))
                {
                    string mensaje = "Estamos actualizando la información de " + PaisT +
                                     ". Por favor consulte mas tarde.";
                    if (GetCurrentIdioma() == "en")
                        mensaje = "We are updating data for " + PaisT + ". Please return later.";


                    objMensaje = new
                    {
                        titulo = "Veritrade",
                        mensaje,
                        flagContactenos = true
                    };
                    CodPaisT = FuncionesBusiness.ObtieneCodPaisAcceso(idUsuario, ref CodPais2T,true);
                    if (CodPaisT == "")
                    {
                        CodPaisT = FuncionesBusiness.ObtieneCodPaisAccesoFlag(idUsuario, ref CodPais2T);
                        listaPaises = new ListaPaises().GetPaisesAdmin(idioma, CodPais2T);
                        codPais2 = CodPais2T;
                    }

                    codPaisAselecconar = CodPaisT;
                }
               
                
                if (listaPaises.Where(x => x.Value == CodPaisT).ToList().Count <= 0)
                {
                    codPaisAselecconar = "0";
                }

                

                Session["CodPais2"] = codPais2;
                Session["CodPais"] = CodPaisT;


                if (CodPaisT == "US_" || CodPaisT == "PE_")
                {
                    Session["CodPais2"] = codPais2;
                    Session["CodPais"] = CodPaisT;
                    if (tipoOpe.Equals("I"))

                        Session["TipoOpe"] = "I";
                    else
                        Session["TipoOpe"] = "E";
                    redirectUS = true;
                }
            }
            else

            {
                PaisT = textoPais;
                if (codPais2 == "4UE")
                    PaisT = "UE - " + PaisT;
                string mensaje = "El país seleccionado (" + PaisT + ") no está incluído en el plan contratado.";

                if (idioma == "en")
                    mensaje = "Selected country: " + PaisT + " is not included in your plan.";

                objMensaje = new
                {
                    titulo = "Veritrade",
                    mensaje,

                    flagContactenos = true
                };

                CodPaisT = Session["CodPais"].ToString();
                CodPais2T = new ListaPaises().BuscarCodPais2(CodPaisT);
                CodPais2T = (CodPais2T == "") ? "1LAT" : CodPais2T;
                listaPaises = new ListaPaises().GetPaisesAdmin(idioma, CodPais2T);
                codPaisAselecconar = CodPaisT;
                if (listaPaises.Where(x => x.Value == CodPaisT).ToList().Count <= 0)
                {
                    codPaisAselecconar = "0";
                }

                

                Session["CodPais2"] = CodPais2T;

                Session["CodPais"] = CodPaisT;
            }

            bool tipoUsuarioEsFreeTrial = (Session["TipoUsuario"].ToString() == "Free Trial") || (bool)(Session["opcionFreeTrial"] ?? false);

            string rangoInfoEnLinea = "";
            string rangoInfoEnLineaFree = "";

            string viewFormFilterFields = "";
            FiltroPeriodo objFiltroPeriodos = null;

            if (!redirectUS)
            {
                MiBusqueda objMisBusqueda = GetMiBusqueda(CodPaisT, CodPais2T, tipoOpe);
                viewFormFilterFields = RenderViewToString(this.ControllerContext, "FormFilterFields", objMisBusqueda);
                objFiltroPeriodos = CargaPeriodos(CodPaisT, tipoOpe);

                //if (!tipoUsuarioEsFreeTrial)
                    rangoInfoEnLinea = FuncionesBusiness.InfoEnLinea(CodPaisT, tipoOpe, idioma, tipoUsuarioEsFreeTrial);
                if ((bool)(Session["opcionFreeTrial"] ?? false))
                {
                    rangoInfoEnLineaFree = FuncionesBusiness.InfoEnLinea(CodPais, TipoOpe, idioma, false);
                }
            }

            bool visibleInfoCL = (Session["CodPais"].ToString() == "CL" && (Session["TipoUsuario"].ToString() == "Cliente" || Session["TipoUsuario"].ToString() == "Gratis"));
            //bool visibleInfoBO = ((Session["CodPais"].ToString() == "BO" && Session["TipoOpe"].ToString() == "I") && (Session["TipoUsuario"].ToString() == "Cliente" || Session["TipoUsuario"].ToString() == "Gratis"));
            bool tipoServidor = Properties.Settings.Default.TableVarGeneral_InDev;
            bool visibleInfoBO = false;
            if (tipoServidor)
                visibleInfoBO = ((Session["CodPais"].ToString() == "BO" && Session["TipoOpe"].ToString() == "I") && (Session["TipoUsuario"].ToString() == "Cliente" || Session["TipoUsuario"].ToString() == "Gratis"));
            else
                visibleInfoBO = ((Session["CodPais"].ToString() == "BO") && (Session["TipoUsuario"].ToString() == "Cliente" || Session["TipoUsuario"].ToString() == "Gratis"));

            return Json(new
            {
                listaPaises,
                codPaisSeleccionar = codPaisAselecconar,
                codPais2Seleccionar = Session["CodPais2"].ToString(),
                objMensaje,
                vistaFiltros = viewFormFilterFields,
                flagVisibleMisFiltros = (Session["Plan"].ToString() != "UNIVERSIDADES"),

                objPeriodos = objFiltroPeriodos,
                tipoUsuarioEsFreeTrial,
                rangoInfoEnLinea,
                rangoInfoEnLineaFree,
                redirectUS,
                visibleInfoCL,
                visibleInfoBO
            });
        }

        private void ValidaCodPaisManif(ref string codPais, string tipoOpe)
        {
            if (codPais.Contains("_"))
            {
                codPais = codPais.Replace("_", tipoOpe);
            }
            else if (_CodManifiestosModificado.Contains(codPais))
            {
                //_CodManifiesto = CodPais;
                codPais = codPais.Substring(0, 2) + "_";
            }
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult CboPaisChange(string codPais, string codPaisText, string codPais2,
            string tipoOpe, string idioma)
        {
            bool redirectUS = false;
            string codPais2T;
            bool modificarCombo = false;
            var codPaisT = codPais;
            var paisT = codPaisText;

            if (codPais2 == "4UE")
            {
                codPaisT = "UE";
                paisT = "UE - " + paisT;
            }

            ValidaCodPaisManif(ref codPaisT, tipoOpe);
            string idUsuario = Session["IdUsuario"].ToString();
            object objMensaje = null;


            if (!Funciones.ValidaPais(idUsuario, codPaisT) && !(bool)(Session["opcionFreeTrial"] ?? false))
            {
                string mensaje = "El país seleccionado (" + paisT + ") no está incluído en el plan contratado.";
                if (idioma == "en")
                    mensaje = "Selected country: " + paisT + " is not included in your plan.";

                objMensaje = new
                {
                    titulo = "Veritrade",
                    mensaje,
                    flagContactenos = true
                };

                codPais2T = codPais2;
                codPaisT = FuncionesBusiness.ObtieneCodPaisAcceso(idUsuario, ref codPais2T);
                Session["CodPais"] = codPaisT;
                modificarCombo = true;
            }
            else
            {
                if (Funciones.FlagCarga(codPaisT))
                {
                    string mensaje = "Estamos actualizando la información de " + paisT +
                                     ". Por favor consulte mas tarde.";
                    if (idioma == "en")
                        mensaje = "We are updating data for " + paisT + ". Please return later.";


                    objMensaje = new
                    {
                        titulo = "Veritrade",
                        mensaje,
                        flagContactenos = true
                    };

                    codPais2T = codPais2;

                    codPaisT = FuncionesBusiness.ObtieneCodPaisAcceso(idUsuario, ref codPais2T, true);
                    Session["CodPais"] = codPaisT;

                    modificarCombo = true;
                }
                //else
                //{
                //    if (codPais == "US_" || codPais == "PE_")
                //    {
                //        Session["CodPais2"] = codPais2;
                //        Session["CodPais"] = codPais;
                //        if (tipoOpe.Equals("I"))
                //            Session["TipoOpe"] = "I";
                //        else
                //            Session["TipoOpe"] = "E";
                //        redirectUS = true;
                //    }
                //}
            }

            bool tipoUsuarioEsFreeTrial = (Session["TipoUsuario"].ToString() == "Free Trial") || (bool)(Session["opcionFreeTrial"] ?? false);

            string rangoInfoEnLinea = "";
            string rangoInfoEnLineaFree = "";

            string viewFormFilterFields = "";
            FiltroPeriodo objFiltroPeriodos = null;

            if (!redirectUS)
            {
                MiBusqueda objMisBusqueda = GetMiBusqueda(codPaisT, codPais2, tipoOpe);

                viewFormFilterFields = RenderViewToString(this.ControllerContext, "FormFilterFields", objMisBusqueda);

                objFiltroPeriodos = CargaPeriodos(codPaisT, tipoOpe);


                //if (!tipoUsuarioEsFreeTrial)
                    rangoInfoEnLinea = FuncionesBusiness.InfoEnLinea(codPaisT, tipoOpe, idioma, tipoUsuarioEsFreeTrial);
                if ((bool)(Session["opcionFreeTrial"] ?? false))
                {
                    rangoInfoEnLineaFree = FuncionesBusiness.InfoEnLinea(codPaisT, tipoOpe, idioma, false);
                }
            }


            Restablecer();
            Session["CodPais"] = codPaisT;
            bool visibleInfoCL = (Session["CodPais"].ToString() == "CL" && (Session["TipoUsuario"].ToString() == "Cliente" || Session["TipoUsuario"].ToString() == "Gratis"));
            //bool visibleInfoBO = ((Session["CodPais"].ToString() == "BO" && Session["TipoOpe"].ToString() == "I") && (Session["TipoUsuario"].ToString() == "Cliente" || Session["TipoUsuario"].ToString() == "Gratis"));

            bool tipoServidor = Properties.Settings.Default.TableVarGeneral_InDev;
            bool visibleInfoBO = false;
            if (tipoServidor)
                visibleInfoBO = ((Session["CodPais"].ToString() == "BO" && Session["TipoOpe"].ToString() == "I") && (Session["TipoUsuario"].ToString() == "Cliente" || Session["TipoUsuario"].ToString() == "Gratis"));
            else
                visibleInfoBO = ((Session["CodPais"].ToString() == "BO") && (Session["TipoUsuario"].ToString() == "Cliente" || Session["TipoUsuario"].ToString() == "Gratis"));

            return Json(new
            {
                objMensaje,

                vistaFiltros = viewFormFilterFields,
                flagVisibleMisFiltros = (Session["Plan"].ToString() != "UNIVERSIDADES"),

                //paisesOrigenOdestino = getPaisesDatatableToListItem(FuncionesBusiness.CargaPaises(codPaisT, codPais2, idioma)),

                objPeriodos = objFiltroPeriodos,
                tipoUsuarioEsFreeTrial,
                rangoInfoEnLinea,
                rangoInfoEnLineaFree,
                redirectUS,
                codPaisT,
                modificarCombo,
                visibleInfoCL,
                visibleInfoBO
            });
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult CboPaisBChange(string tipoOpe, string textoCboPaisB, string valorCboPaisB,
            int indexCboPaisB)
        {
            var nuevosFiltros = new List<object>();
            string codPais = Session["codPais"].ToString();
            if (codPais.Contains("_"))
            {
                ValidaCodPaisManif(ref codPais, tipoOpe);
            }
            bool isManifiesto = IsManifiesto(codPais);
            Session["codPaisB"] = false;
            if (indexCboPaisB > 0)
            {
                Session["codPaisB"] = true;
                if (tipoOpe == "I")
                {
                    nuevosFiltros.Add(new

                    {
                        text =
                            "[" + (isManifiesto ? Resources.AdminResources.EmbarqueCountry_FormField_Label :Resources.AdminResources.OriginCountry_FormField_Label) + "] " + textoCboPaisB + " ",
                        value = "5O_|" + valorCboPaisB
                    });
                }
                else
                {
                    nuevosFiltros.Add(new
                    {
                        text = "[" + Resources.AdminResources.DestinationCountry_FormField_Label + "] " +
                               textoCboPaisB + " ",
                        value = "5D_|" + valorCboPaisB
                    });
                }
            }

            Restablecer();

            return Json(new
            {
                flagVisibleMisFiltros = (Session["Plan"].ToString() != "UNIVERSIDADES"),

                nuevosFiltros
            });
        }

        public object AgregaPalabrasFiltros(string palabra, string opcion, string codPais = "",
            string tipoOpe = "", string idioma = "", string selectedValue = "0")
        {
            string codPais2 = Session["CodPais2"].ToString();
            ValidaCodPais2(codPais2, ref codPais);
            ValidaCodPaisManif(ref codPais, tipoOpe);
            string PalAux = "";
            var valueOption = "";
            var textOption = "";

            switch (opcion)

            {
                case "1DE":
                    int cantPalabrasY = 0;
                    if (!string.IsNullOrWhiteSpace(Session["hdfCantPalabrasY"]?.ToString()))
                        cantPalabrasY = Convert.ToInt32(Session["hdfCantPalabrasY"].ToString());

                    Session["hdfPalabrasY"] = (Session["hdfPalabrasY"] + " " + palabra).Trim();
                    cantPalabrasY += 1;


                    valueOption = opcion + "|" + palabra;
                    textOption = "[" + Resources.AdminResources.ComercialDesc_FilterText + "] " + palabra;

                    Session["hdfCantPalabrasY"] = cantPalabrasY.ToString();
                    
                    break;

                case "2P_":

                    PalAux = palabra;

                    PalAux = PalAux.Replace("[TODOS]", "");
                    PalAux = PalAux.Replace("[Todos]", "");
                    PalAux = PalAux.Replace("[ALL]", "");
                    PalAux = PalAux.Replace("[All]", "");

                    if (Session["hdfNandinaB"] == null)
                        Session["hdfNandinaB"] = PalAux.Trim();
                    else
                        Session["hdfNandinaB"] = Session["hdfNandinaB"] + "|" + PalAux.Trim();

                    var idPartida = Funciones.BuscaIdPartida(palabra, codPais);
                    var subCapitulo = Funciones.BuscaSubCapitulo(palabra);

                    var hts6 = Funciones.BuscaHts6(palabra);

                    var todos = "TODOS";
                    if (idioma == "en" || codPais == "BR" || codPais == "CN" || codPais == "IN" || codPais == "US")
                    {
                        subCapitulo = Funciones.BuscaSubCapitulo(palabra, "EN");
                        hts6 = Funciones.BuscaHts6(palabra, "EN");
                        todos = "ALL";
                    }

                    if (idPartida != "")

                    {
                        string partidaFav = FuncionesBusiness.ObtienePartidaFav(Session["TipoUsuario"].ToString(),
                            codPais, tipoOpe, idPartida);

                        if (partidaFav == "")
                        {
                            partidaFav = FuncionesBusiness.BuscaPartida(idPartida, codPais, idioma);
                        }

                        textOption = "[" + Resources.AdminResources.Nandina2_FilterText + "] " + partidaFav;
                    }
                    else if (subCapitulo != "")
                    {
                        textOption = "[" + Resources.AdminResources.Nandina2_FilterText + "] " + palabra + " [" +
                                     todos + " 4] " + subCapitulo;
                    }

                    else if (hts6 != "")
                    {
                        textOption = "[" + Resources.AdminResources.Nandina2_FilterText + "] " + palabra + " [" +
                                     todos + " 6] " + hts6;
                    }
                    else
                    {
                        textOption = "[" + Resources.AdminResources.Nandina2_FilterText + "] " + palabra + " [" +
                                     todos + "]";
                    }

                    valueOption = opcion + '|' + palabra;
                    
                    break;


                case "3I_":

                    PalAux = palabra;

                    PalAux = PalAux.Replace("[TODOS]","");
                    PalAux = PalAux.Replace("[Todos]", "");
                    PalAux = PalAux.Replace("[ALL]", "");
                    PalAux = PalAux.Replace("[All]", "");

                    if (Session["hdfImportadorB"] == null)
                        
                        Session["hdfImportadorB"] = PalAux.Trim();
                    else
                        Session["hdfImportadorB"] = Session["hdfImportadorB"] + "|" + PalAux.Trim();


                    if (selectedValue != "0")
                    {
                        textOption = "[" + Resources.Resources.Search_Form_Item05 + "] " +
                                     Funciones.BuscaEmpresa(selectedValue, codPais);
                    }
                    else
                    {
                        textOption =
                            "[" + Resources.Resources.Search_Form_Item05 + "] " +
                            palabra; // + " " + Resources.Resources.Option_Filter_All;
                    }

                    valueOption = opcion + '|' + palabra;
                    
                    break;
                case "3E_":
                    PalAux = palabra;

                    PalAux = PalAux.Replace("[TODOS]", "");
                    PalAux = PalAux.Replace("[Todos]", "");
                    PalAux = PalAux.Replace("[ALL]", "");
                    PalAux = PalAux.Replace("[All]", "");
                    if (Session["hdfExportadorB"] == null)
                        Session["hdfExportadorB"] = PalAux.Trim();
                    else
                        Session["hdfExportadorB"] = Session["hdfExportadorB"] + "|" + PalAux.Trim();

                    /*if(codPais == "BRI" || codPais == "BRE")
                    {
                        textOption = "[" + Resources.Resources.Search_Form_Item06 + "] " + palabra;
                    }
                    else
                    {*/
                        if (selectedValue != "0")
                        {
                            textOption = "[" + Resources.Resources.Search_Form_Item06 + "] " + Funciones.BuscaEmpresa(selectedValue, codPais);
                        }
                        else
                        {
                            textOption = "[" + Resources.Resources.Search_Form_Item06 + "] " + palabra; //+ " " +
                                                                                                        //Resources.Resources.Option_Filter_All;
                        }
                    //}                   
                     
                    valueOption = opcion + '|' + palabra;
                    
                    break;
                case "4P_":
                    PalAux = palabra;

                    PalAux = PalAux.Replace("[TODOS]", "");
                    PalAux = PalAux.Replace("[Todos]", "");
                    PalAux = PalAux.Replace("[ALL]", "");
                    PalAux = PalAux.Replace("[All]", "");
                    if (Session["hdfProveedorB"] == null)
                        Session["hdfProveedorB"] = PalAux.Trim();
                    else
                        Session["hdfProveedorB"] = Session["hdfProveedorB"] + "|" + PalAux.Trim();


                    textOption =
                        (codPais != "CL" 
                            ? "[" + Resources.Resources.Search_Form_Item06 + "] "
                            : "[" + Resources.Resources.Search_Form_BrandField + "] ") +
                        palabra; //+ " " + Resources.Resources.Option_Filter_All
                    valueOption = opcion + '|' + palabra;
                    
                    break;


                case "4I_":
                    PalAux = palabra;

                    PalAux = PalAux.Replace("[TODOS]", "");
                    PalAux = PalAux.Replace("[Todos]", "");
                    PalAux = PalAux.Replace("[ALL]", "");
                    PalAux = PalAux.Replace("[All]", "");
                    if (Session["hdfImportadorExpB"] == null)
                        Session["hdfImportadorExpB"] = PalAux.Trim();
                    else
                        Session["hdfImportadorExpB"] = Session["hdfImportadorExpB"] + "|" + PalAux.Trim();

                    textOption = "[" + Resources.Resources.Search_Form_Item05 + "] " + palabra; //+ " " +
                                 //Resources.Resources.Option_Filter_All;
                    valueOption = opcion + '|' + palabra;
                    
                    break;
            }

            Session["isVisibleInfoComplementario"] = Session["hdfPalabrasY"] == null && (Session["hdfNandinaB"] != null || Session["PartidasB"] != null) && Session["hdfImportadorB"] == null &&
                                                        Session["hdfExportadorB"] == null && Session["hdfProveedorB"] == null && Session["hdfImportadorExpB"] == null &&
                                                        Session["MarcasB"] == null && Session["ModelosB"] == null && Session["ImportadoresB"] == null && Session["ExportadoresB"] == null &&
                                                        Session["ProveedoresB"] == null && Session["ImportadoresExpB"] == null && Session["PaisesOrigenB"] == null && 
                                                        Session["PaisesDestinoB"] == null && Session["PaisesEmbarqueB"] == null && Session["PtosDescargaB"] == null &&
                                                        Session["MarcaECB"] == null && Session["ViasTranspB"] == null && Session["AduanaDUAsB"] == null && Session["AduanasB"] == null &&
                                                        Session["DistritosB"] == null && Session["PtosEmbarqueB"] == null && Session["PtosDestinoB"] == null && !(bool)(Session["codPaisB"] ?? false) && Extensiones.GetCookie("IsDemo") != "1";

            ActualizarSessionListaFitros(valueOption, textOption);


            return new

            {
                text = textOption,
                value = valueOption
            };
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult AgregarDesComercialClick(string txtDesComercialB, int numFiltrosExistentes, string idioma)
        {
            var listaPalabras = new List<object>();
            string textoFiltro = EliminaStopWords(txtDesComercialB);
            object objMensaje = null;
            bool maximoLimiteFiltros = false;
            bool validarBotones  = Convert.ToBoolean(Session["BotonUtilizado"]);
            int numFiltrosActuales = numFiltrosExistentes;
            if (textoFiltro == "")
            {
                string auxMensaje = txtDesComercialB.Trim().Replace("  ", " ").ToUpper();

                objMensaje = new
                {
                    titulo = "Buscar",
                    mensaje = "Las palabras ingresadas se eliminarán de su búsqueda: <b>" + auxMensaje +
                              "</b>.<br>Por favor ingrese otras palabras",
                    flagContactenos = false
                };
            }
            else
            {
                if (numFiltrosExistentes == LimiteFiltros)
                {
                    maximoLimiteFiltros = true;
                }
                else
                {
                    string[] Palabras = textoFiltro.Split(' ');
                    foreach (string Palabra in Palabras)
                    {
                        //if (numFiltrosExistentes == LimiteFiltros)
                        //{
                        //    maximoLimiteFiltros = true;
                        //    break;
                        //}
                        listaPalabras.Add(AgregaPalabrasFiltros(Palabra, "1DE"));


                        numFiltrosExistentes += 1;

                        if (numFiltrosExistentes == LimiteFiltros)
                            maximoLimiteFiltros = true;
                    }
                }
            }

            if (numFiltrosActuales == LimiteFiltros)
            {
                string mensaje = "Se ha llegado al maximo número de filtros: <b>" + LimiteFiltros +
                              "</b>.<br>Por favor elimine alguna palabra de la busqueda actual";
                if (idioma == "en")
                    mensaje = "You’ve reached the maximum number of filters allowed. Limit:" + LimiteFiltros +
                              "</b>.<br>Please delete a word from the current search";

                objMensaje = new
                {
                    titulo = idioma == "en" ? "Search" : "Buscar",
                    mensaje,
                    flagContactenos = false
                };
            }

            return Json(new
            {
                objMensaje,
                listaPalabras,
                maximoLimiteFiltros,
                validarBotones
            });
        }

        private List<object> GeneraJsonAutocompletePartidas(DataTable dataTablePartidas)
        {
            List<object> lista = new List<object>();
            if (dataTablePartidas.Rows.Count > 0)
            {
                foreach (DataRow dr in dataTablePartidas.Rows)
                {
                    lista.Add(new
                    {
                        id = dr["Nandina2"].ToString().Trim(),
                        value0 = dr["Nandina"].ToString(),
                        value = dr["Partida"].ToString().Replace("\"", "")
                    });
                }
            }
            else
            {
                lista.Add(new
                {
                    id = 0,
                    value = "-"
                });
            }


            return lista;
        }

        private List<object> GeneraJsonAutocompleteEmpresas(List<Empresas> dtEmpresas, string empresa)

        {
            List<object> lista = new List<object>();
            if (dtEmpresas.Count > 0)
            {
                lista.Add(new
                {
                    id = 0,
                    value = empresa + " " + Resources.Resources.Option_Filter_All,
                    texto = empresa
                });

                DataRow dr;
                /*for (int i = 0; i < dtEmpresas.Count && i < 20; i++)
                {
                    dr = dtEmpresas[0];
                    if (empresa != "")
                    {
                        lista.Add(new
                        {
                            id = dr["IdEmpresa"].ToString(),
                            value = dr["Empresa"].ToString().Replace("\"", " ") + " " + dr["RUC"].ToString()
                        });
                    }
                    else
                    {
                        lista.Add(new
                        {
                            id = dr["IdEmpresa"].ToString(),
                            value = dr["RUC"].ToString() + " " + dr["Empresa"].ToString()
                        });
                    }
                }*/
                int i = 0; 
                foreach(var item in dtEmpresas)
                {
                    if( i == 20) break;
                    if (empresa != "")
                    {
                        lista.Add(new
                        {
                            id = item.IdEmpresa,
                            value = item.Empresa.Replace("\"", " ") + " " + item.Ruc
                        });
                    }
                    else
                    {
                        lista.Add(new
                        {
                            id = item.IdEmpresa,
                            value = item.Ruc + " " + item.Empresa
                        });
                    }

                }
            }
            else
            {
                lista.Add(new
                {
                    id = 0,
                    value = "-"
                });
            }

            return lista;
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult BuscarPartidas(string nandinaB, string codPais, string codPais2)
        {
            //Si el código país es de manifiesto se reemplaza el carácter '_' por 
            //'I' o 'E' dependiendo si es de tipo importaciones o exportaciones
            string tipoOperacion = Session["TipoOpe"].ToString();
            if (codPais[codPais.Length - 1] == '_')
            {
                StringBuilder sb = new StringBuilder(codPais);
                sb[codPais.Length - 1] = tipoOperacion[0];
                codPais = sb.ToString();
            }

            DataTable dtpPartidas = MiBusqueda.BuscaPartidas(nandinaB.ToUpper(), codPais, codPais2, GetCurrentIdioma());
            var json =  Json(GeneraJsonAutocompletePartidas(dtpPartidas));
            //return json;
            return new JsonResult()
            {
                ContentEncoding = Encoding.Default,
                ContentType = "application/json",
                Data = json,
                //JsonRequestBehavior = requestBehavior,
                MaxJsonLength = int.MaxValue
            };

        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult BuscarImportadorB(string importadorB, string codPais, string tipoOpe)
        {
            ValidaCodPaisManif(ref codPais, tipoOpe);

            string auxImportadorB = importadorB.ToUpper();

            List<object> listaEmpresas = new List<object>();

            string[] paisesCondicion = { "PEI", "PEE", "USI", "USE" };


            if (!paisesCondicion.Contains(codPais))
            {
                List<Empresas> dtEmpresas = MiBusqueda.BuscaEmpresas(auxImportadorB, codPais);

                listaEmpresas = GeneraJsonAutocompleteEmpresas(dtEmpresas, auxImportadorB);
            }
            else
            {
                listaEmpresas.Add(new
                {
                    id = 0,
                    value = auxImportadorB + " " + Resources.Resources.Option_Filter_All,
                });
            }

            return Json(listaEmpresas);
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult BuscarProveedorB(string proveedorB)

        {
            List<object> listaProveedor = new List<object>
            {
                new

                {
                    id = 0,
                    value = proveedorB.ToUpper() + " " + Resources.Resources.Option_Filter_All
                }

            };

            return Json(listaProveedor);
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult AgregarPartida(string idSeleccionado, string codPais, string tipoOpe,
          int numFiltrosExistentes)
        {

            if (idSeleccionado == "")
                return Json(new

                {
                    mensaje = "No hay un elemento seleccionado"
                });

            List<object> nuevoFiltro = new List<object>();
            bool maximoLimiteFiltros = false;

            if (numFiltrosExistentes != LimiteFiltros)

            {
                nuevoFiltro.Add(AgregaPalabrasFiltros(idSeleccionado, "2P_", codPais, tipoOpe, GetCurrentIdioma()));


                if (numFiltrosExistentes + 1 == LimiteFiltros)
                    maximoLimiteFiltros = true;
            }

            return Json(new

            {
                mensaje = "",
                nuevoFiltro,
                maximoLimiteFiltros
            });
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult AgregaImportador(string idSeleccionado, string textoSeleccionado, string codPais,
            int numFiltrosExistentes)
        {
            if (idSeleccionado == "")
                return Json(new

                {
                    mensaje = "No hay un elemento seleccionado"
                });

            List<object> nuevoFiltro = new List<object>();
            bool maximoLimiteFiltros = false;

            string tipoOperacion = Session["TipoOpe"].ToString();

            if (numFiltrosExistentes != LimiteFiltros)
            {
                if(codPais == "BR_" || codPais == "EC_")
                {
                    nuevoFiltro.Add(AgregaPalabrasFiltros(textoSeleccionado.ToString(), "3I_"));
                }
                else
                {
                    if (idSeleccionado != "0")
                    {
                        nuevoFiltro.Add(AgregaPalabrasFiltros("[" + idSeleccionado + "]", "3I_", codPais, tipoOperacion, "",
                            idSeleccionado));
                    }
                    else
                    {

                        nuevoFiltro.Add(AgregaPalabrasFiltros(textoSeleccionado.ToString(), "3I_"));
                    }
                }
                


                if (numFiltrosExistentes + 1 == LimiteFiltros)
                    maximoLimiteFiltros = true;
            }

            return Json(new
            {
                mensaje = "",
                nuevoFiltro,
                maximoLimiteFiltros
            });
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult AgregaProveedor(string idSeleccionado, string textoSeleccionado, string codPais,
            int numFiltrosExistentes)
        {
            if (idSeleccionado == "")
                return Json(new
                {
                    mensaje = "No hay un elemento seleccionado"
                });

            List<object> nuevoFiltro = new List<object>();
            bool maximoLimiteFiltros = false;

            if (numFiltrosExistentes != LimiteFiltros)
            {
                nuevoFiltro.Add(AgregaPalabrasFiltros(textoSeleccionado.ToUpper(), "4P_", codPais));
                if (numFiltrosExistentes + 1 == LimiteFiltros)
                    maximoLimiteFiltros = true;
            }

            return Json(new
            {
                mensaje = "",
                nuevoFiltro,
                maximoLimiteFiltros
            });
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult AgregaExportador(string idSeleccionado, string textoSeleccionado, string codPais,
            int numFiltrosExistentes)
        {
            if (idSeleccionado == "")
                return Json(new
                {
                    mensaje = "No hay un elemento seleccionado"
                });

            List<object> nuevoFiltro = new List<object>();
            bool maximoLimiteFiltros = false;

            string tipoOpe = Session["TipoOpe"].ToString();

            if (numFiltrosExistentes != LimiteFiltros)
            {
                if(codPais == "BR_" || codPais == "EC_")
                {
                    nuevoFiltro.Add(AgregaPalabrasFiltros(textoSeleccionado.ToUpper().Trim(), "3E_", codPais, tipoOpe, "",
                        "0"));
                }
                else
                {
                    if (idSeleccionado != "0")
                    {
                        nuevoFiltro.Add(AgregaPalabrasFiltros("[" + idSeleccionado + "]", "3E_", codPais, tipoOpe, "",
                            idSeleccionado));
                    }
                    else
                    {
                        nuevoFiltro.Add(AgregaPalabrasFiltros(textoSeleccionado.ToUpper().Trim(), "3E_", codPais, "", "",
                            idSeleccionado));
                    }
                }
                
                if (numFiltrosExistentes + 1 == LimiteFiltros)
                    maximoLimiteFiltros = true;
            }

            return Json(new
            {
                mensaje = "",
                nuevoFiltro,
                maximoLimiteFiltros
            });
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult AgregaImportadorExp(string idSeleccionado, string textoSeleccionado, string codPais, int numFiltrosExistentes)
        {
            if (idSeleccionado == "")
                return Json(new

                {
                    mensaje = "No hay un elemento seleccionado"
                });

            List<object> nuevoFiltro = new List<object>();
            bool maximoLimiteFiltros = false;

            if (numFiltrosExistentes != LimiteFiltros)
            {
                nuevoFiltro.Add(AgregaPalabrasFiltros(textoSeleccionado.ToUpper().Trim(), "4I_"));
                if (numFiltrosExistentes + 1 == LimiteFiltros)

                    maximoLimiteFiltros = true;
            }


            return Json(new
            {
                mensaje = "",
                nuevoFiltro,

                maximoLimiteFiltros
            });
        }

        private bool ValidarFiltros(string[] filtrosSeleccionados, string codPais, string tipoOpe, bool IsManif, int nroBeforeDelete)
        {
            bool bValida = false;
            bool vtValida = false;
            bool isCountry = false;

            if (Session["lstFiltros"] != null)
            {
                var listOption = ((List<OptionSelect>)Session["lstFiltros"]).ToList();

                for (int i = 0; i < listOption.Count; i++)
                {
                    if (filtrosSeleccionados.Contains(listOption[i].value))
                    {
                        listOption.RemoveAt(i);
                        i--;
                    }
                }
                
                for (int i = 0; i < filtrosSeleccionados.Length; i++)
                {
                    isCountry = filtrosSeleccionados[i].Substring(0, 2).Equals("5O");
                    if (isCountry) break;
                }
                if (!IsManif)
                {
                    foreach (OptionSelect item in listOption)
                    {
                        if (item.text.Contains("[" + Resources.AdminResources.FilterText_ViaTransp + "]")
                            || item.text.Contains("[" + Resources.AdminResources.Customs_FilterText + "]") ||
                            item.text.Contains("[" + Resources.AdminResources.CustomsDUA_FilterText + "]"))
                        {
                            vtValida = true;
                        }

                        if (item.text.Contains("[" + Resources.AdminResources.ComercialDesc_FilterText + "]") ||
                            item.text.Contains("[" + Resources.AdminResources.Nandina2_FilterText + "]") ||
                            item.text.Contains("[" + Resources.Resources.Search_Form_Item05 + "]") ||
                            item.text.Contains("[" + Resources.Resources.Search_Form_Item06 + "]") ||
                            item.text.Contains("[" + Resources.AdminResources.OriginCountry_FormField_Label + "]") ||
                            item.text.Contains("[" + Resources.AdminResources.DestinationCountry_FormField_Label + "]") ||
                            //item.text.Contains("[" + Resources.AdminResources.FilterText_ViaTransp + "]") ||//via de transporte
                            //item.text.Contains("[" + Resources.AdminResources.Customs_FilterText + "]") || // valida aduana
                            //item.text.Contains("[" + Resources.AdminResources.CustomsDUA_FilterText + "]") || // valida aduana 2
                            (codPais == "CL" && tipoOpe == "I" &&
                             item.text.Contains("[" + Resources.Resources.Search_Form_BrandField + "]")))
                        {
                            bValida = true;
                            break;
                        }
                    }
                    if (nroBeforeDelete != listOption.Count)
                    {
                        isCountry = true;
                        bValida = true;
                    }
                    if (!vtValida && !isCountry)
                    {
                        bValida = true;
                    }
                }
                else
                {
                    foreach (OptionSelect item in listOption)
                    {
                        if (item.text.Contains("[" + Resources.AdminResources.Manifest_FilterText + "]"))
                        {
                            vtValida = true;
                        }

                        if (item.text.Contains("[" + Resources.AdminResources.ComercialDesc_FilterText + "]") ||
                            item.text.Contains("[" + Resources.Resources.Search_Form_Item05 + "]") ||
                            item.text.Contains("[" + Resources.Resources.Search_Form_Item06 + "]") ||
                            item.text.Contains("[" + Resources.AdminResources.EmbarqueCountry_FormField_Label + "]") ||
                            item.text.Contains("[" + Resources.AdminResources.DestinationCountry_FormField_Label + "]"))
                            
                        {
                            bValida = true;
                            break;
                        }
                    }

                    if (nroBeforeDelete != listOption.Count)
                    {
                        isCountry = true;
                        bValida = true;
                    }
                    if (!vtValida && !isCountry)
                    {
                        bValida = true;
                    }
                }

                

                if (listOption.Count == 0)
                    bValida = true;
            }
            else
            {
                bValida = true;
            }


            return bValida;
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult EliminarFiltrosClick(string[] filtrosSeleccionados, int nroTotalFiltros,
            int nroFiltrosEliminar,
            string codPais, string codPais2, string tipoOpe,
            string anioMesIni, string anioMesFin, int indexCboPaisB,
            string codPaisB, bool filtrarDatos, string idioma)
        {
            
            string auxCodPais = codPais;
            ValidaCodPais2(codPais2, ref codPais);
            ValidaCodPaisManif(ref codPais, tipoOpe);
            var IsManif = IsManifiesto(codPais);

            object objMensaje = null;
            object objRestablecer = null;
            if (!ValidarFiltros(filtrosSeleccionados, codPais, tipoOpe, IsManif, nroTotalFiltros - nroFiltrosEliminar))
            {
                string mensaje = "Se necesita al menos un filtro para País, Desc. Comercial, Importador y/o Exportador";
                if (idioma == "en")
                {
                    mensaje = "At least one filter for Country, Comercial Desc, Importer and/or Exporter is needed";
                }

                objMensaje = new
                {
                    titulo = Resources.AdminResources.Label_MyFilters,
                    mensaje,
                    flagContactenos = false
                };

                return Json(new { objMensaje, cantReg = 1 });
            }

            string palabraItem;
            bool flagCboPaisB = false;
            foreach (var item in filtrosSeleccionados)
            {
                string ID = item.Substring(3, item.Length - 3);
                switch (item.Substring(0, 3))
                {
                    case "1DE":
                        //string Palabra = item.Text.Replace("[" + lDesComercial2 + "] ", "");

                        //hdfPalabrasY.Value = (hdfPalabrasY.Value).Replace(Palabra, "").Replace("  ", " ").Trim();

                        //se obtiene solo el codigo usado para eliminar del value del filtro
                        palabraItem = item.Substring(4);

                        string[] palabras = Session["hdfPalabrasY"].ToString().Split(new Char[] { ' ' });

                        int numIndex = Array.IndexOf(palabras, palabraItem);

                        palabras = palabras.Where((val, idx) => idx != numIndex).ToArray();

                        if (palabras.Length > 0)
                        {
                            Session["hdfPalabrasY"] = string.Join(" ", palabras);
                        }
                        else
                        {
                            Session["hdfPalabrasY"] = null;
                        }

                        break;
                    case "2P_":
                        ID = item.Substring(4, item.Length - 4);
                        var sq = Session["hdfNandinaB"];

                        var x = (" " + Session["hdfNandinaB"].ToString() + " ")
                            .Replace("|", " | ")
                            .Replace(" " + ID + " ", "")
                            .Replace("  ", " ")
                            .Replace(" | ", "|")
                            .Replace(" || ", "|")
                            .Replace(" |", "")
                            .Replace("| ", "").Trim();

                        /*Session["hdfNandinaB"] = (" " + Session["hdfNandinaB"].ToString() + " ")
                            .Replace(" " + ID + " ", "")
                            .Replace("  ", " ").Trim();*/
                        Session["hdfNandinaB"] = x;
                        sq = Session["hdfNandinaB"];
                        break;
                    case "2PA":
                        //if (!palabraItem.Contains("[G]"))
                        if (!item.Substring(4).Contains("Page"))
                        {
                            Session["PartidasB"] = Funciones.EliminaSeleccion((ArrayList)Session["PartidasB"], ID);

                            //if (Session["PartidasB"] != null)
                            //    hdfCantPartidasB.Value =
                            //        ((ArrayList)Session["PartidasB"]).Count.ToString();
                            //else hdfCantPartidasB.Value = "";
                        }
                        else
                        {
                            Session["hdfIdGrupoB"] = null;
                        }

                        break;
                    case "2MA":
                        Session["MarcasB"] = Funciones.EliminaSeleccion((ArrayList)Session["MarcasB"], ID);
                        break;
                    case "2MO":
                        Session["ModelosB"] = Funciones.EliminaSeleccion((ArrayList)Session["ModelosB"], ID);
                        break;
                    case "3I_":
                        //ID = item.Substring(4, item.Length - 4);

                        //string[] importadoresB = Session["hdfImportadorB"].ToString().Split(new char[] { '|' });

                        //int numIndexImpoB = Array.IndexOf(importadoresB, ID);

                        //importadoresB = importadoresB.Where((val, idx) => idx != numIndexImpoB).ToArray();


                        //if ((importadoresB.Length > 0) && (!ID.Contains("[TODOS]")))
                        //{
                        //    Session["hdfImportadorB"] = string.Join("|", importadoresB);
                        //}
                        //else
                        //{
                        //    Session["hdfImportadorB"] = null;
                        //}

                        ID = item.Substring(4, item.Length - 4);
                        ID = ID.Replace("[TODOS]", "")
                                .Replace("[ALL]", "")
                                .Replace("[all]", "")
                                .Replace("[todos]", "").Trim();

                        var imp = (" " + Session["hdfImportadorB"].ToString() + " ")
                            .Replace("|", " | ")
                            .Replace(" " + ID + " ", "")
                            .Replace("  ", " ")
                            .Replace(" | ", "|")
                            .Replace(" || ", "|")
                            .Replace(" |", "")
                            .Replace("| ", "").Trim();
                        Session["hdfImportadorB"] = imp;

                        break;
                    case "3IM":
                        if (!item.Substring(4).Contains("Page"))
                        {
                            Session["ImportadoresB"] =
                                Funciones.EliminaSeleccion((ArrayList)Session["ImportadoresB"], ID);

                            //if (Session["ImportadoresB"] != null)
                            //    hdfCantImportadoresB.Value =
                            //        ((ArrayList)Session["ImportadoresB"]).Count.ToString();
                            //else hdfCantImportadoresB.Value = "";
                        }
                        else
                        {
                            Session["hdfIdGrupoB"] = null;

                        }

                        break;
                    case "3E_":
                        ID = item.Substring(4, item.Length - 4);
                        string[] auxExportadoresB = Session["hdfExportadorB"].ToString().Split(new char[] { '|' });

                        int numIndexExpB = Array.IndexOf(auxExportadoresB, ID);

                        auxExportadoresB = auxExportadoresB.Where((val, idx) => idx != numIndexExpB).ToArray();

                        if (auxExportadoresB.Length > 0)
                        {
                            Session["hdfExportadorB"] = string.Join("|", auxExportadoresB);
                        }
                        else
                        {
                            Session["hdfExportadorB"] = null;
                        }

                        break;
                    case "3EX":
                        if (!item.Substring(4).Contains("Page"))
                        {
                            Session["ExportadoresB"] =
                                Funciones.EliminaSeleccion((ArrayList)Session["ExportadoresB"], ID);
                            //if (Session["ExportadoresB"] != null)
                            //    hdfCantExportadoresB.Value =
                            //        ((ArrayList)Session["ExportadoresB"]).Count.ToString();
                            //else hdfCantExportadoresB.Value = "";
                        }
                        else
                            Session["hdfIdGrupoB"] = null;

                        break;
                    case "4P_":
                        ID = item.Substring(4, item.Length - 4);
                        ID = ID.Replace("[TODOS]", "")
                                .Replace("[ALL]", "")
                                .Replace("[all]", "")
                                .Replace("[todos]", "").Trim();
                                
                        var prov = (" " + Session["hdfProveedorB"].ToString() + " ")
                            .Replace("|", " | ")
                            .Replace(" " + ID + " ", "")
                            .Replace("  ", " ")
                            .Replace(" | ", "|")
                            .Replace(" || ", "|")
                            .Replace(" |", "")
                            .Replace("| ", "").Trim();
                        Session["hdfProveedorB"] = prov;
                        /*Session["hdfProveedorB"] = (" " + Session["hdfProveedorB"].ToString() + " ")
                            .Replace(" " + ID + " ", "")
                            .Replace("  ", " ").Trim();*/
                        break;
                    case "4PR":
                        palabraItem = item.Substring(4);
                        if (!item.Substring(4).Contains("Page"))
                        {
                            Session["ProveedoresB"] =
                                Funciones.EliminaSeleccion((ArrayList)Session["ProveedoresB"], ID);

                            //if (Session["ProveedoresB"] != null)
                            //    hdfCantProveedoresB.Value =
                            //        ((ArrayList)Session["ProveedoresB"]).Count.ToString();
                            //else hdfCantProveedoresB.Value = "";
                        }
                        else
                            Session["hdfIdGrupoB"] = null;

                        break;
                    case "4I_":
                        ID = item.Substring(4, item.Length - 4);

                        var ImpExp = (" " + Session["hdfImportadorExpB"].ToString() + " ")
                                .Replace("|", " | ")
                                .Replace(" " + ID + " ", "")
                                .Replace("  ", " ")
                                .Replace(" | ", "|")
                                .Replace(" || ", "|")
                                .Replace(" |", "")
                                .Replace("| ", "").Trim();

                        /*Session["hdfImportadorExpB"] = (" " + Session["hdfImportadorExpB"].ToString() + " ")
                            .Replace(" " + ID + " ", "").Replace("  ", " ").Trim();*/
                        break;
                    case "4IE":
                        if (!item.Substring(4).Contains("Page"))
                        {
                            Session["ImportadoresExpB"] =
                                Funciones.EliminaSeleccion((ArrayList)Session["ImportadoresExpB"], ID);
                            //if (Session["ImportadoresExpB"] != null)
                            //    hdfCantImportadoresExpB.Value =
                            //        ((ArrayList)Session["ImportadoresExpB"]).Count.ToString();
                            //else hdfCantImportadoresExpB.Value = "";
                        }
                        else
                            Session["hdfIdGrupoB"] = null;

                        break;
                    case "5O_":
                        //cboPaisB.SelectedIndex = 0;
                        flagCboPaisB = true;
                        break;
                    case "5D_":
                        //cboPaisB.SelectedIndex = 0;
                        flagCboPaisB = true;
                        break;

                    case "5PO":
                        Session["PaisesOrigenB"] =
                            Funciones.EliminaSeleccion((ArrayList)Session["PaisesOrigenB"], ID);
                        break;
                    case "5PD":
                        Session["PaisesDestinoB"] =
                            Funciones.EliminaSeleccion((ArrayList)Session["PaisesDestinoB"], ID);
                        break;
                    case "5PE":
                        Session["PaisesEmbarqueB"] =
                            Funciones.EliminaSeleccion((ArrayList)Session["PaisesEmbarqueB"], ID);
                        break;
                    case "6MA":
                        Session["MarcaECB"] =
                            Funciones.EliminaSeleccion((ArrayList)Session["MarcaECB"], ID);
                        break;
                    case "6VT":
                        Session["ViasTranspB"] =
                            Funciones.EliminaSeleccion((ArrayList)Session["ViasTranspB"], ID);
                        break;
                    case "7AD":
                        Session["AduanaDUAsB"] =
                            Funciones.EliminaSeleccion((ArrayList)Session["AduanaDUAsB"], ID);
                        break;
                    case "7AA":
                        Session["AduanasB"] =
                            Funciones.EliminaSeleccion((ArrayList)Session["AduanasB"], ID);
                        break;

                    case "8DI":

                        Session["DistritosB"] =
                            Funciones.EliminaSeleccion((ArrayList)Session["DistritosB"], ID);

                        break;
                    case "3NO":
                        Session["NotificadosB"] =
                            Funciones.EliminaSeleccion((ArrayList)Session["NotificadosB"], ID);
                        break;
                    case "6PD":
                        Session["PtosDescargaB"] =
                            Funciones.EliminaSeleccion((ArrayList)Session["PtosDescargaB"], ID);
                        break;
                    case "6PE":
                        Session["PtosEmbarqueB"] =
                            Funciones.EliminaSeleccion((ArrayList)Session["PtosEmbarqueB"], ID);
                        break;
                    case "6DE":
                        Session["PtosDestinoB"] =
                            Funciones.EliminaSeleccion((ArrayList)Session["PtosDestinoB"], ID);
                        break;
                    case "9MA":
                        Session["ManifiestosB"] =
                            Funciones.EliminaSeleccion((ArrayList)Session["ManifiestosB"], ID);
                        break;
                }
            }

            if (flagCboPaisB)
            {
                Session["codPaisB"] = false;
            }

            Session["isVisibleInfoComplementario"] = Session["hdfPalabrasY"] == null && (Session["hdfNandinaB"] != null || Session["PartidasB"] != null) && Session["hdfImportadorB"] == null &&
                                                        Session["hdfExportadorB"] == null && Session["hdfProveedorB"] == null && Session["hdfImportadorExpB"] == null &&
                                                        Session["MarcasB"] == null && Session["ModelosB"] == null && Session["ImportadoresB"] == null && Session["ExportadoresB"] == null &&
                                                        Session["ProveedoresB"] == null && Session["ImportadoresExpB"] == null && Session["PaisesOrigenB"] == null && Session["PaisesDestinoB"] == null && 
                                                        Session["PaisesEmbarqueB"] == null && Session["PtosDescargaB"] == null &&
                                                        Session["MarcaECB"] == null && Session["ViasTranspB"] == null && Session["AduanaDUAsB"] == null && Session["AduanasB"] == null &&
                                                        Session["DistritosB"] == null && Session["PtosEmbarqueB"] == null && Session["PtosDestinoB"] == null && !(bool)(Session["codPaisB"]??false) && Extensiones.GetCookie("IsDemo") != "1";
            List<object> listGridData = null;

            int cantReg = 0;
            bool hideTabExcel = false;

            var specificCulture = GetSpecificCulture();
            if (nroTotalFiltros - nroFiltrosEliminar > 0)
            {
                if (Session["lstFiltros"] != null)
                {
                    var listOption = Session["lstFiltros"] as List<OptionSelect>;
                    for (int i = 0; i < listOption.Count; i++)
                    {
                        if (filtrosSeleccionados.Contains(listOption[i].value))
                        {
                            listOption.RemoveAt(i);
                            i--;
                        }                            
                    }
                    Session["lstFiltros"] = listOption;
                }

                if (filtrarDatos)
                {
                    var tabla = "";
                    var dua = "";
                    GetTablaAndDua(tipoOpe, codPais, ref tabla, ref dua);

                    if (flagCboPaisB)
                    {
                        indexCboPaisB = 0;
                    }

                    Session["SqlFiltro"] = GeneraSqlFiltro(codPais, codPais2, tipoOpe, anioMesIni, anioMesFin,
                        indexCboPaisB, codPaisB, dua, auxCodPais);

                    Session["SqlInfoComplementario"] = GeneraSqlFiltro(codPais, codPais2, tipoOpe, anioMesIni, anioMesFin,
                        indexCboPaisB, codPaisB, dua, auxCodPais,isInfoTabla:true);

                    if (useRand)
                    {
                        Session["useRand"] = true;
                    }
                    else
                    {
                        Session["useRand"] = false;
                    }
             
                    //string testac = Session["SqlInfoComplementario"].ToString();
                    //Extensiones.SetCookie("sl", Session["sqlFiltro"].ToString() + " $ " + Session["CodPais"].ToString() + " $ " + Session["TipoOpe"].ToString(), TimeSpan.FromDays(360));
                    Extensiones.SetCookie("sl", "", TimeSpan.FromDays(360));
                    string cifTot = "";
                    string pesoNeto = "";
                    string valueCifTot = "";

                    decimal valuePesoNeto = 0;
                    string unidad = "";
                    GetCifTot_PesoNeto_CantReg_ValueCifTot_Unidad_ValuePesoNeto(codPais, tipoOpe, tabla, ref cifTot,
                        ref pesoNeto,
                        ref cantReg, ref valueCifTot, ref unidad, ref valuePesoNeto);

                    bool FlagPalabras = (!string.IsNullOrWhiteSpace(Session["hdfPalabrasY"]?.ToString()));
                    //hideTabExcel = cantReg > CantRegMax && !FlagPalabras;

                    

                    hideTabExcel = Funciones.VisualizarExcel();
                    if (cantReg > 0)
                    {
                        GuardarLogInicioErrorSession("EliminarFiltrosClick()");
                        listGridData = Busca(codPais, tipoOpe, cifTot, pesoNeto, idioma, dua, tabla, indexCboPaisB, valueCifTot,
                        valuePesoNeto, unidad,
                        cantReg, specificCulture, hideTabExcel,anioMesIni:anioMesIni,anioMesFin:anioMesFin,codPaisB:codPaisB,auxCodPais:auxCodPais, useRand: useRand);
                    }

                    else
                    {
                        string mensaje = "La búsqueda con los últimos filtros NO encontró resultados";
                        if (idioma == "en")

                            mensaje = "Search with last filters found no results";


                        objMensaje = new

                        {
                            titulo = Resources.Resources.Search_Button,
                            mensaje,
                            flagContactenos = false
                        };
                    }               
                    
                }
            }
            else
            {
                objRestablecer = Restablecer(codPais, codPais2, tipoOpe, idioma);
            }


            var json =  new
            {
                objMensaje,
                flagCboPaisB,
                objRestablecer,
                totalRecordsFound = GetDescriptionRecordsFound(cantReg, specificCulture, idioma),
                gridData = listGridData,
                hideTabExcel = hideTabExcel,
                FlagRegMax = cantReg > CantRegMax,
                cantReg
            };

            return new JsonResult()
            {
                ContentEncoding = Encoding.Default,
                ContentType = "application/json",
                Data = json,
                //JsonRequestBehavior = requestBehavior,
                MaxJsonLength = int.MaxValue
            };
        }

        private object ObtenerObjetoMuestraMensaje(string titulo, string mensaje, string flagContactenos)
        {
            return new
            {
                titulo,


                mensaje,
                flagContactenos
            };

        }

        private object Restablecer(string cboPaisValue, string cboPais2Value, string tipoOpe, string idioma)
        {
            //ocultar los tabs desde el cliente

            //trabajar esta lógica con Razor
            //string Ayuda = "Ingrese palabras en Español";
            //if (Idioma == "en")
            //    Ayuda = "Enter words in Spanish";




            string codPaisT1 = cboPaisValue;

            if (cboPaisValue == "PEB" || cboPaisValue == "PEP")
                codPaisT1 = "PE";

            else if (cboPais2Value == "4UE")
                codPaisT1 = "UE";

            if (!_CodManifiestosModificado.Contains(codPaisT1))
            {
                ValidaCodPaisManif(ref codPaisT1, tipoOpe);
            }
            

            List<SelectListItem> lista =
                getPaisesDatatableToListItem(FuncionesBusiness.CargaPaises(codPaisT1, cboPais2Value, idioma));


            FiltroPeriodo objPeriodos = CargaPeriodos(codPaisT1, tipoOpe);


            //clear formFields



            bool flagVisibleMisFiltros = Session["Plan"].ToString() != "UNIVERSIDADES";

            //revisar si es necesario visible paneles a false
            //bool flagVisibleHlkArchivoAndHlkArchivo2 = false;

            //Revisar codigo faltante y utilización de Razor

            Session["hdfPalabrasY"] = null;
            Session["hdfUltPalabrasY"] = null;
            Session["hdfCantPalabrasY"] = null;


            Session["hdfNandinaB"] = null;
            Session["hdfImportadorB"] = null;
            Session["hdfProveedorB"] = null;

            Session["hdfExportadorB"] = null;
            Session["hdfImportadorExpB"] = null;


            //hdfCantPartidasB.Value = "";
            //hdfCantImportadoresB.Value = hdfCantExportadoresB.Value = "";
            //hdfCantProveedoresB.Value = hdfCantImportadoresExpB.Value = "";
            //hdfCantReg.Value = hdfCIFTot.Value = hdfPesoNeto.Value = "";


            Session["hdfTipoFavoritoB"] = null;
            Session["hdfIdGrupoB"] = null;

            Session["hdfVariable"] = null;
            Session["hdfValor"] = null;

            Session["hdfDesComercialBB"] = null;

            Session["hdfIdAutocompletado"] = null;

            Session["isVisibleInfoComplementario"] = null;

            Session["PaisesComplementarioOrigenDestino"] = null;

            string idUsuarioT = "";
            string tipoUsuarioT = "";
            string planT = "";
            string codPais2T = "";
            string codPaisT = "";
            string tipoOpeT = "";
            string culture = "";
            int cantPopup = 0;
            bool isVisibleInfoComplementario = false;
            List<string> PaisComplementariosAux = new List<string>();


            if (Session["isVisibleInfoComplementario"] != null)
            {
                isVisibleInfoComplementario = (bool)Session["isVisibleInfoComplementario"];
            }
            if (Session["PopupView"] != null)
            {
                cantPopup = (int)Session["PopupView"];
            }
            if (Session["IdUsuario"] != null)
            {
                idUsuarioT = Session["IdUsuario"].ToString();
            }
            if (Session["TipoUsuario"] != null)
            {
                tipoUsuarioT = Session["TipoUsuario"].ToString();
            }

            if (Session["Plan"] != null)
            {
                planT = Session["Plan"].ToString();
            }

            if (Session["CodPais2"] != null)
            {
                codPais2T = Session["CodPais2"].ToString();
            }

            if (Session["CodPais"] != null)
            {
                codPaisT = Session["CodPais"].ToString();
            }

            if (Session["TipoOpe"] != null)
            {
                tipoOpeT = Session["TipoOpe"].ToString();
            }

            if (Session["culture"] != null)
            {
                culture = Session["culture"].ToString();
            }

            string university = "";
            if (Session["university"] != null)
            {
                university = Session["University"].ToString();
            }

            bool opcionFreeT = false;
            if(Session["opcionFreeTrial"] != null)
            {
                opcionFreeT = (bool)Session["opcionFreeTrial"];
            }

            bool ingresoComoFreeTrial = false;
            if (Session["IngresoComoFreeTrial"] != null)
            {
                ingresoComoFreeTrial = (bool)Session["IngresoComoFreeTrial"];
            }

            bool IsDemo = false;
            if (Session["IsDemo"] != null)
            {
                IsDemo = (bool)Session["IsDemo"];
            }

            if (Session["PaisesComplementarioOrigenDestino"] != null)
            {
                PaisComplementariosAux = (List<string>)Session["PaisesComplementarioOrigenDestino"];
            }

            Session.RemoveAll();

            Session["PopupView"] = cantPopup;
            Session["Idioma"] = idioma;
            Session["IdUsuario"] = idUsuarioT;
            Session["TipoUsuario"] = tipoUsuarioT;
            Session["Plan"] = planT;
            Session["CodPais2"] = codPais2T;
            Session["CodPais"] = codPaisT;
            Session["TipoOpe"] = tipoOpeT;
            Session["University"] = university;
            Session["culture"] = culture;
            Session["opcionFreeTrial"] = opcionFreeT;
            Session["IngresoComoFreeTrial"] = ingresoComoFreeTrial;
            Session["IsDemo"] = IsDemo;
            Session["isVisibleInfoComplementario"] = isVisibleInfoComplementario;
            Session["PaisesComplementarioOrigenDestino"] = PaisComplementariosAux;
            return new
            {
                listOriginOrDestinationCountry = lista,
                objPeriodos,
                flagVisibleMisFiltros,
                flagVisibleHlkArchivoAndHlkArchivo2 = false
            };
        }

        private void Restablecer()
        {
            Session["hdfPalabrasY"] = null;
            Session["hdfUltPalabrasY"] = null;

            Session["hdfCantPalabrasY"] = null;
            Session["hdfNandinaB"] = null;
            Session["hdfImportadorB"] = null;
            Session["hdfProveedorB"] = null;
            Session["hdfExportadorB"] = null;
            Session["hdfImportadorExpB"] = null;

            //hdfCantPartidasB.Value = "";
            //hdfCantImportadoresB.Value = hdfCantExportadoresB.Value = "";
            //hdfCantProveedoresB.Value = hdfCantImportadoresExpB.Value = "";
            //hdfCantReg.Value = hdfCIFTot.Value = hdfPesoNeto.Value = "";

            Session["hdfTipoFavoritoB"] = null;
            Session["hdfIdGrupoB"] = null;


            Session["hdfVariable"] = null;
            Session["hdfValor"] = null;
            Session["hdfDesComercialBB"] = null;

            Session["hdfIdAutocompletado"] = null;

            Session["isVisibleInfoComplementario"] = null;

            string idUsuarioT = "";
            string tipoUsuarioT = "";
            string planT = "";
            string codPais2T = "";

            string codPaisT = "";
            string tipoOpeT = "";
            string culture = "";
            int cantPopup = 0;
            bool isVisibleInfoComplementario = false;
            List<string> PaisComplementariosAux = new List<string>();
            bool codPaisB = false;

            if(Session["codPaisB"] != null)
            {
                codPaisB = (bool)Session["codPaisB"];
            }

            if (Session["isVisibleInfoComplementario"] != null)
            {
                isVisibleInfoComplementario = (bool)Session["isVisibleInfoComplementario"];
            }

            if (Session["isVisibleInfoComplementario"] != null)
            {
                cantPopup = (int)Session["isVisibleInfoComplementario"];
            }

            if (Session["PopupView"] != null)
            {
                cantPopup = (int)Session["PopupView"];
            }

            if (Session["IdUsuario"] != null)
            {
                idUsuarioT = Session["IdUsuario"].ToString();
            }

            if (Session["TipoUsuario"] != null)

            {
                tipoUsuarioT = Session["TipoUsuario"].ToString();
            }

            if (Session["Plan"] != null)
            {
                planT = Session["Plan"].ToString();
            }

            if (Session["CodPais2"] != null)
            {
                codPais2T = Session["CodPais2"].ToString();
            }

            if (Session["CodPais"] != null)

            {
                codPaisT = Session["CodPais"].ToString();
            }

            if (Session["TipoOpe"] != null)
            {

                tipoOpeT = Session["TipoOpe"].ToString();
            }


            if (Session["culture"] != null)
            {
                culture = Session["culture"].ToString();
            }

            bool opcionFreeT = false;
            if (Session["opcionFreeTrial"] != null)
            {
                opcionFreeT = (bool)Session["opcionFreeTrial"];
            }

            bool ingresoComoFreeTrial = false;
            if (Session["IngresoComoFreeTrial"] != null)
            {
                ingresoComoFreeTrial = (bool)Session["IngresoComoFreeTrial"];
            }

            bool IsDemo = false;
            if (Session["IsDemo"] != null)
            {
                IsDemo = (bool)Session["IsDemo"];
            }

            if (Session["PaisesComplementarioOrigenDestino"] != null)
            {
                PaisComplementariosAux = (List<string>)Session["PaisesComplementarioOrigenDestino"];
            }

            Session.RemoveAll();
            Session["codPaisB"] = codPaisB;
            Session["PopupView"] = cantPopup;
            Session["Idioma"] = culture;
            Session["IdUsuario"] = idUsuarioT;
            Session["TipoUsuario"] = tipoUsuarioT;
            Session["Plan"] = planT;
            Session["CodPais2"] = codPais2T;
            Session["CodPais"] = codPaisT;
            Session["TipoOpe"] = tipoOpeT;
            Session["opcionFreeTrial"] = opcionFreeT;
            Session["IngresoComoFreeTrial"] = ingresoComoFreeTrial;
            Session["culture"] = culture;
            Session["IsDemo"] = IsDemo;
            Session["PaisesComplementarioOrigenDestino"] = PaisComplementariosAux;
        }
        
        public FiltroPeriodo CargaPeriodos(string codPais, string tipoOpe)
        {
            if (tipoOpe == "T" || tipoOpe == "_") tipoOpe = "I";
            codPais = codPais.Replace("_T", "").Replace("_", "");

            string anioIni = "", mesIni = "", anioFin = "", mesFin = "";
            Funciones.Rango(codPais, tipoOpe, ref anioIni, ref mesIni, ref anioFin, ref mesFin);

            FiltroPeriodo filtroPeriodo = new FiltroPeriodo();
            filtroPeriodo.FechaInfoIni = Convert.ToDateTime(anioIni + "-" + mesIni + "-01 00:00:00");
            filtroPeriodo.FechaInfoFin = Convert.ToDateTime(anioFin + "-" + mesFin + "-01 00:00:00");
            

            if (Session["TipoUsuario"].ToString() == "Free Trial" || (bool)(Session["opcionFreeTrial"] ?? false))
            {
                //filtroPeriodo.FechaInfoFin.AddMonths(Properties.Settings.Default.FreeTrial_Periodo_Atraz); //-6
                //DateTime auxFechaInfoFin = filtroPeriodo.FechaInfoFin;
                //filtroPeriodo.FechaInfoIni = auxFechaInfoFin.AddMonths(Properties.Settings.Default.FreeTrial_Periodo_Count); //-2

                filtroPeriodo.FechaInfoFin =
                    filtroPeriodo.FechaInfoFin.AddMonths(Properties.Settings.Default.FreeTrial_Periodo_Atraz);
                if(filtroPeriodo.FechaInfoFin.AddMonths(Properties.Settings.Default.FreeTrial_Periodo_Count) >= filtroPeriodo.FechaInfoIni)
                {
                    filtroPeriodo.FechaInfoIni = filtroPeriodo.FechaInfoFin.AddMonths(Properties.Settings.Default.FreeTrial_Periodo_Count); //-2
                }
                

            }
            filtroPeriodo.DefaultFechaInfoFin = filtroPeriodo.FechaInfoFin;
            filtroPeriodo.EnabledAnioMesIni = (codPais != "PEB");
            filtroPeriodo.EnabledAnioMesFin = (codPais != "PEB");

            return filtroPeriodo;
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult EnviarMensajeContactenos(string Mensaje)
        {
            EnviarSolicitud(Mensaje);

            return Json(new { Respuesta = "OK" });
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult ActualizaOcultarVideo()
        {
            string sIdUsuario = Session["IdUsuario"].ToString();

            FuncionesBusiness.ActualizaOcultarVideo(sIdUsuario);

            return Json(new { Respuesta = "OK" });
        }

        private void EnviarSolicitud(string Mensaje)
        {
            VeritradeServicios.ServiciosCorreo wsc = new VeritradeServicios.ServiciosCorreo();
            try
            {
#if DEBUG
                string DireccionIP = Properties.Settings.Default.IP_Debug;
#else
                string DireccionIP = Request.ServerVariables["REMOTE_ADDR"];
#endif

                string CodPais = "";
                string Ubicacion = _ws.BuscaUbicacionIP2(DireccionIP, ref CodPais);

                string sIdUsuario = Session["IdUsuario"].ToString();

                string IdSolicitud =
                    FuncionesBusiness.CreaSolicitud("CONTACTENOS2", sIdUsuario, Mensaje, DireccionIP, Ubicacion);

                string EmailEnvio1 = "", EmailEnvioNombre1 = "", EmailEnvioPassword1 = "";
                Funciones.BuscaDatosCorreoEnvio("4", ref EmailEnvio1, ref EmailEnvioNombre1, ref EmailEnvioPassword1);


                string FromName, FromEmail, ToName, ToEmail, Subject, URL;

                FromName = EmailEnvioNombre1;
                FromEmail = EmailEnvio1;
                ToName = "VERITRADE";
                ToEmail = "info@veritrade-ltd.com";
                

                Subject = "Solicitud de Contacto o Soporte - Usuario";
                URL = Properties.Settings.Default.FrontEnd + "/Correo/Index?Opcion=SOLICITUD&IdSolicitud=" +
                      IdSolicitud;


                wsc.EnviaEmail3(FromName, FromEmail, ToName, ToEmail, null, null, null, null, Subject, URL, EmailEnvio1,
                    EmailEnvioPassword1);
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult CargarFavoritos(string tipoFavorito, string codPais2, string codPais,
            string tipoOpe, string idioma, int pagina = 1,
            bool esBusqueda = false)
        {
            Session["TipoFavorito"] = tipoFavorito;

            string auxCodPais = codPais;
            ValidaCodPais2(codPais2, ref codPais);
            ValidaCodPaisManif(ref codPais, tipoOpe);

            string favoritoB = "";
            if (Session["FavoritoF"] != null && esBusqueda)
                favoritoB = Session["FavoritoF"].ToString().Replace("[G] ", "");


            string codPaisT = codPais;

            if (codPais == "PEB" || codPais == "PEP")
            {
                codPaisT = "PE";
            }
            else
            {
                if (codPais2 == "4UE")
                    codPaisT = "UE" + auxCodPais;
            }

            DataTable dtRowsFavourites = FuncionesBusiness.BuscaFavoritos(Session["IdUsuario"].ToString(), codPaisT,
                tipoOpe, Session["TipoFavorito"].ToString(), favoritoB, false, idioma);

            //DataTable dtComboFavoritos = FuncionesBusiness.BuscaFavoritos(Session["IdUsuario"].ToString(), codPaisT,
            //    tipoOpe, Session["TipoFavorito"].ToString(), "", false, idioma);

            var listFavouritesRows = dtRowsFavourites.AsEnumerable().Select(m => new MiFavorito()
            {
                IdFavorito = m.Field<int>("IdFavorito"),
                Favorito = m.Field<string>("Favorito")
            }).ToList().ToPagedList(pagina, MisFavoritosPageSize);

            List<MiFavorito> listFavouritesOptions = dtRowsFavourites.AsEnumerable().Select(m => new MiFavorito()
            {
                IdFavorito = m.Field<int>("IdFavorito"),
                Favorito = m.Field<string>("Favorito")
            }).ToList();

            string stringPagingFavourites = "";
            if (dtRowsFavourites.Rows.Count > MisFavoritosPageSize)
            {
                stringPagingFavourites =
                    RenderViewToString(this.ControllerContext, "FavouritesPaging", listFavouritesRows);
            }

            return Json(new
            {
                htmlRowsFavourites = RenderViewToString(this.ControllerContext, "FavouritesRows", listFavouritesRows),
                listFavouritesOptions,
                htmlPagingFavourites = stringPagingFavourites
            });
        }
        [SessionExpireFilter]
        [HttpPost]
        public JsonResult cboFavoritosF_SelectedIndexChanged(string cboFavoritosFDataText,
            string cboFavoritosFDataValue, string idioma,
            string tipoOpe, int numFiltrosExistentes)
        {
            var tipo = "";
            var nombre = "";
            var nuevoFiltro = true;
            var desabilitarControles = false;
            var auxValue = "";

            var newItems = new List<object>();

            bool maximoLimiteFiltros = false;
            object objMensaje = null;
            if (numFiltrosExistentes >= LimiteFiltros)
            {
                string mensaje = "Se ha llegado al maximo número de filtros: <b>" + LimiteFiltros +
                              "</b>.<br>Por favor elimine alguna palabra de la busqueda actual";
                if (idioma == "en")
                    mensaje = "You’ve reached the maximum number of filters allowed. Limit:" + LimiteFiltros +
                              "</b>.<br>Please delete a word from the current search";
                maximoLimiteFiltros = true;
                objMensaje = new
                {
                    titulo = idioma == "en"? "Search" : "Buscar",
                    mensaje,
                    flagContactenos = false
                };
            }
            else
            {
                if (cboFavoritosFDataText.Length < 3 || cboFavoritosFDataText.Substring(0, 3) != "[G]")
                {
                    Session["hdfFiltroSel"] = Session["TipoFavorito"].ToString();
                    Session["hdfIDSel"] = cboFavoritosFDataValue;
                    nombre = AgregaFiltro(idioma, tipoOpe, ref tipo);
                    if (nombre == "")
                        nuevoFiltro = false;


                    auxValue = tipo + cboFavoritosFDataValue;
                    newItems.Add(new

                    {
                        text = nombre,
                        value = auxValue
                    });
                }
                else
                {
                    nombre = AgregaGrupoAFiltros(cboFavoritosFDataValue, ref tipo);
                    desabilitarControles = true;

                    auxValue = tipo + PageID;
                    newItems.Add(new

                    {
                        text = nombre,
                        value = auxValue
                    });
                }


                if (nuevoFiltro)
                {
                    ActualizarSessionListaFitros(auxValue, nombre);

                    numFiltrosExistentes++;
                    if (numFiltrosExistentes == LimiteFiltros)
                    {
                        maximoLimiteFiltros = true;
                    }
                }

                return Json(new

                {
                    nuevoFiltro,
                    newItems,
                    desabilitarControles,
                    maximoLimiteFiltros,
                    objMensaje
                });
            }
            return Json(new
            {
                objMensaje,
                maximoLimiteFiltros
            });

        }
        private string AgregaFiltro(string idioma, string tipoOpe, ref string pOutTipo)
        {
            string filtro = Session["hdfFiltroSel"].ToString();
            string id = Session["hdfIDSel"].ToString();
            ArrayList IDsSeleccionados = null;

            switch (filtro)
            {
                case "Partida":
                    IDsSeleccionados = (ArrayList)Session["PartidasB"];
                    break;
                case "Marca":
                    IDsSeleccionados = (ArrayList)Session["MarcasB"];
                    break;
                case "Modelo":
                    IDsSeleccionados = (ArrayList)Session["ModelosB"];
                    break;
                case "Importador":
                    IDsSeleccionados = (ArrayList)Session["ImportadoresB"];
                    break;
                case "Notificado":
                    IDsSeleccionados = (ArrayList)Session["NotificadosB"];
                    break;
                case "Exportador":
                    IDsSeleccionados = (ArrayList)Session["ExportadoresB"];
                    break;
                case "Proveedor":
                    IDsSeleccionados = (ArrayList)Session["ProveedoresB"];
                    break;
                case "ImportadorExp":
                    IDsSeleccionados = (ArrayList)Session["ImportadoresExpB"];
                    break;
                case "PaisOrigen":
                    IDsSeleccionados = (ArrayList)Session["PaisesOrigenB"];
                    break;
                case "PaisDestino":
                    IDsSeleccionados = (ArrayList)Session["PaisesDestinoB"];
                    break;
                case "PaisEmbarque":
                    IDsSeleccionados = (ArrayList)Session["PaisesEmbarqueB"];
                    break;
                case "ViaTransp":
                    IDsSeleccionados = (ArrayList)Session["ViasTranspB"];
                    break;
                case "AduanaDUA":
                    IDsSeleccionados = (ArrayList)Session["AduanaDUAsB"];
                    break;
                case "Aduana":
                    IDsSeleccionados = (ArrayList)Session["AduanasB"];
                    break;
                case "DUA":
                    IDsSeleccionados = (ArrayList)Session["DUAsB"];
                    break;
                case "Distritos":
                    IDsSeleccionados = (ArrayList)Session["DistritosB"];
                    break;
                case "PtoDescarga":
                    IDsSeleccionados = (ArrayList)Session["PtosDescargaB"];
                    break;
                case "PtoEmbarque":
                    IDsSeleccionados = (ArrayList)Session["PtosEmbarqueB"];
                    break;
                case "PtoDestino":
                    IDsSeleccionados = (ArrayList)Session["PtosDestinoB"];
                    break;
                case "Manifiesto":
                    IDsSeleccionados = (ArrayList)Session["ManifiestosB"];
                    break;
            }

            string tipo = "", nombre = "";

            if (IDsSeleccionados == null)
                IDsSeleccionados = new ArrayList();

            if (!IDsSeleccionados.Contains(id))
                IDsSeleccionados.Add(id);
            else
                return nombre;

            CodPais = Session["CodPais"].ToString();


            switch (filtro)
            {
                case "Partida":

                    Session["PartidasB"] = IDsSeleccionados;
                    //hdfCantPartidasB.Value = IDsSeleccionados.Count.ToString();
                    tipo = "2PA";
                    string partidaFav =
                        FuncionesBusiness.ObtienePartidaFav(Session["IdUsuario"].ToString(), CodPais, tipoOpe, id);

                    if (partidaFav == "")

                        partidaFav = FuncionesBusiness.BuscaPartida(id, CodPais, idioma);


                    nombre = "[" + Resources.Resources.Search_Form_Item04 + "] " + partidaFav;

                    break;
                case "Marca":
                    Session["MarcasB"] = IDsSeleccionados;
                    tipo = "2MA";
                    nombre = "[" + Resources.Resources.Search_Form_BrandField + "] " +
                             Funciones.BuscaMarca(id, CodPais);
                    break;
                case "Modelo":
                    Session["ModelosB"] = IDsSeleccionados;
                    tipo = "2MO";

                    nombre = "[" + Resources.AdminResources.Model_FormField_Label + "] " +
                             Funciones.BuscaModelo(id, CodPais);
                    break;
                case "Importador":

                    Session["ImportadoresB"] = IDsSeleccionados;
                    //hdfCantImportadoresB.Value = IDsSeleccionados.Count.ToString();
                    tipo = "3IM";
                    nombre = "[" + Resources.Resources.Search_Form_Item05 + "] " + Funciones.BuscaEmpresa(id, CodPais);
                    break;
                case "Notificado":
                    Session["NotificadosB"] = IDsSeleccionados;
                    tipo = "3NO";
                    nombre = "["+ Resources.Resources.Demo_Notif_Tab_Fil + "] " + Funciones.BuscaNotificado(id, CodPais);
                    break;

                case "Exportador":
                    Session["ExportadoresB"] = IDsSeleccionados;
                    //hdfCantExportadoresB.Value = IDsSeleccionados.Count.ToString();
                    tipo = "3EX";
                    nombre = "[" + Resources.Resources.Search_Form_Item06 + "] " + Funciones.BuscaEmpresa(id, CodPais);

                    break;
                case "Proveedor":

                    Session["ProveedoresB"] = IDsSeleccionados;
                    //hdfCantProveedoresB.Value = IDsSeleccionados.Count.ToString();
                    tipo = "4PR";

                    nombre = "[" + Resources.Resources.Search_Form_Item06 + "] " +
                             Funciones.BuscaProveedor(id, CodPais);
                    break;

                case "ImportadorExp":
                    Session["ImportadoresExpB"] = IDsSeleccionados;
                    //hdfCantImportadoresExpB.Value = IDsSeleccionados.Count.ToString();

                    tipo = "4IE";

                    nombre = "[" + @Resources.Resources.Search_Form_Item05 + "] " +
                             Funciones.BuscaImportadorExp(id, CodPais);
                    break;
                case "PaisEmbarque":
                    Session["PaisesEmbarqueB"] = IDsSeleccionados;
                    tipo = "5PE";
                    nombre = "["+ Resources.Resources.Ult_Paises_Embarque + "] " + Funciones.BuscaPais(id, CodPais);
                    break;

                case "PaisOrigen":
                    Session["PaisesOrigenB"] = IDsSeleccionados;
                    tipo = "5PO";
                    nombre = "[" + Resources.AdminResources.OriginCountry_FormField_Label + "] " +
                             Funciones.BuscaPais(id, CodPais);
                    break;
                case "PaisDestino":

                    Session["PaisesDestinoB"] = IDsSeleccionados;
                    tipo = "5PD";
                    nombre = "[" + Resources.AdminResources.DestinationCountry_FormField_Label + "] " +
                             Funciones.BuscaPais(id, CodPais);
                    break;
                case "ViaTransp":
                    Session["ViasTranspB"] = IDsSeleccionados;
                    tipo = "6VT";
                    nombre = "[" + Resources.AdminResources.FilterText_ViaTransp + "] " +
                             Funciones.BuscaVia(id, CodPais);
                    break;
                case "AduanaDUA":
                    Session["AduanaDUAsB"] = IDsSeleccionados;
                    tipo = "7AD";
                    nombre = "[" + Resources.AdminResources.FilterText_Aduana + " DUA] " +
                             Funciones.BuscaAduana(id.Split('-')[0], CodPais) + " - " +
                             id.Split('-')[1];
                    break;
                case "Aduana":
                    Session["AduanasB"] = IDsSeleccionados;
                    tipo = "7AA";
                    nombre = "[" + Resources.AdminResources.FilterText_Aduana + "] " +
                             Funciones.BuscaAduana(id, CodPais);
                    break;
                case "DUA":
                    Session["DUAsB"] = IDsSeleccionados;

                    tipo = "7DU";
                    nombre = "[DUA] " + id;
                    break;
                case "Distrito":
                    Session["DistritosB"] = IDsSeleccionados;
                    tipo = "8DI";
                    nombre = "[" + Resources.AdminResources.FilterText_District + "] " +
                             Funciones.BuscaDistrito(id, CodPais);
                    break;
                case "PtoDescarga":
                    Session["PtosDescargaB"] = IDsSeleccionados;
                    tipo = "6PD";
                    nombre = "[" + Resources.AdminResources.UnloadingPort_FilterText + "] " + Funciones.BuscaPuerto(id, CodPais);
                    break;
                case "PtoEmbarque":
                    Session["PtosEmbarqueB"] = IDsSeleccionados;
                    tipo = "6PE";
                    nombre = "[" + Resources.AdminResources.LastShipmentPort_FilterText + "] " + Funciones.BuscaPuerto(id, CodPais);
                    break;
                case "PtoDestino":
                    Session["PtosDestinoB"] = IDsSeleccionados;
                    tipo = "6DE";
                    nombre = "[" + Resources.AdminResources.DestinationPort_FilterText + "] " + Funciones.BuscaPuerto(id, CodPais);
                    break;
                case "Manifiesto":
                    Session["ManifiestosB"] = IDsSeleccionados;
                    tipo = "9MA";
                    nombre = "[" + Resources.AdminResources.Manifest_FilterText + "] " + id;
                    break;
            }

            Session["hdfFiltroSel"] = null;
            Session["hdfIDSel"] = null;


            pOutTipo = tipo;
            return nombre;
        }

        private string AgregaGrupoAFiltros(string idGrupo, ref string pOutTipo)
        {
            string tipoFavorito = Session["TipoFavorito"].ToString();
            string tipo = "";
            string nombre = "[G] " + Funciones.BuscaGrupo(idGrupo).Replace("(GRUPO) ", "");

            switch (tipoFavorito)
            {
                case "Partida":
                    //hdfCantPartidasB.Value = "1";
                    tipo = "2PA";

                    nombre = "[" + Resources.Resources.Search_Form_Item04 + "] [G] " +
                             Funciones.BuscaGrupo(idGrupo).Replace("(GROUP) ", "");

                    break;
                case "Importador":

                    //hdfCantImportadoresB.Value = "1";

                    tipo = "3IM";

                    nombre = "[" + Resources.Resources.Search_Form_Item05 + "] [G] " +
                             Funciones.BuscaGrupo(idGrupo).Replace("(GROUP) ", "");
                    break;
                case "Exportador":
                    //hdfCantExportadoresB.Value = "1";
                    tipo = "3EX";
                    nombre = "[" + Resources.Resources.Search_Form_Item06 + "] [G] " +
                             Funciones.BuscaGrupo(idGrupo).Replace("(GROUP) ", "");
                    break;

                case "Proveedor":
                    //hdfCantProveedoresB.Value = "1";
                    tipo = "4PR";
                    nombre = "[" + Resources.Resources.Search_Form_Item06 + "] [G] " +
                             Funciones.BuscaGrupo(idGrupo).Replace("(GROUP) ", "");

                    break;
                case "ImportadorExp":

                    //hdfCantImportadoresExpB.Value = "1";

                    tipo = "4IE";
                    nombre = "[" + Resources.Resources.Search_Form_Item05 + "] [G] " +
                             Funciones.BuscaGrupo(idGrupo).Replace("(GROUP) ", "");
                    break;
            }


            Session["hdfTipoFavoritoB"] = tipoFavorito;
            Session["hdfIdGrupoB"] = idGrupo;


            pOutTipo = tipo;
            return nombre;
        }
        [SessionExpireFilter]
        [HttpPost]
        public JsonResult AgregarGrpAFiltros_Click(string idGrupo)

        {
            string tipo = "";
            string nombre = "";
            var newItems = new List<object>();

            nombre = AgregaGrupoAFiltros(idGrupo, ref tipo);

            newItems.Add(new
            {
                text = nombre,
                value = tipo + PageID
            });

            Session.Remove("TipoFavorito");
            Session.Remove("FavoritoF");
            return Json(new
            {
                nuevoFiltro = true,
                newItems,
                desabilitarControles = true
            });
        }
        [SessionExpireFilter]
        [HttpPost]
        public JsonResult AgregarFavAFiltros_Click(string favoritosSeleccionados, string codPais2, string codPais,
            string tipoOpe, string idioma, int numFiltrosExistentes)
        {
            string[] listaSeleccionados = favoritosSeleccionados.Split(new char[] { ',' });
            object objMensaje = null;
            if (numFiltrosExistentes + listaSeleccionados.Length > LimiteFiltros)
            {
                string mensaje = "Se ha llegado al maximo número de filtros: <b>" + LimiteFiltros +
                              "</b>.<br>Por favor elimine alguna palabra de la busqueda actual";
                if (idioma == "en")
                    mensaje = "You’ve reached the maximum number of filters allowed. Limit:" + LimiteFiltros +
                              "</b>.<br>Please delete a word from the current search";

                objMensaje = new
                {
                    titulo = idioma == "en" ? "Search" :"Buscar",
                    mensaje,
                    flagContactenos = false
                };
            }
            else
            {

                string auxCodPais = codPais;
                ValidaCodPais2(codPais2, ref codPais);
                ValidaCodPaisManif(ref codPais, tipoOpe);


                string tipoFavorito = Session["TipoFavorito"].ToString();
                var json = new List<object>();


                string idUsuario = Session["IdUsuario"].ToString();
                switch (tipoFavorito)
                {
                    case "Partida":
                        //gdvFavoritos.ID = "gdvPartidas";
                        Session["PartidasB"] = FuncionesBusiness.GuardaSeleccionados(codPais, tipoOpe, "Partida",
                            listaSeleccionados, (ArrayList) Session["PartidasB"], ref json, numFiltrosExistentes,
                            idUsuario, LimiteFiltros,
                            idioma, auxCodPais);
                        //Session["hdfCantPartidasB"] = ((ArrayList)Session["PartidasB"]).Count.ToString();
                        break;
                    case "Importador":
                        //gdvFavoritos.ID = "gdvImportadores";
                        Session["ImportadoresB"] = FuncionesBusiness.GuardaSeleccionados(codPais, tipoOpe, "Importador",
                            listaSeleccionados,
                            (ArrayList) Session["ImportadoresB"], ref json, numFiltrosExistentes, idUsuario,
                            LimiteFiltros,
                            idioma);
                        //hdfCantImportadoresB.Value = ((ArrayList)Session["ImportadoresB"]).Count.ToString();
                        break;
                    case "Exportador":

                        //gdvFavoritos.ID = "gdvImportadores";
                        Session["ExportadoresB"] = FuncionesBusiness.GuardaSeleccionados(codPais, tipoOpe, "Exportador",
                            listaSeleccionados,
                            (ArrayList) Session["ExportadoresB"], ref json, numFiltrosExistentes, idUsuario,
                            LimiteFiltros,
                            idioma);
                        //hdfCantExportadoresB.Value = ((ArrayList)Session["ExportadoresB"]).Count.ToString();

                        break;
                    case "Proveedor":

                        //gdvFavoritos.ID = "gdvProveedores";

                        Session["ProveedoresB"] = FuncionesBusiness.GuardaSeleccionados(codPais, tipoOpe, "Proveedor",
                            listaSeleccionados,
                            (ArrayList) Session["ProveedoresB"], ref json, numFiltrosExistentes, idUsuario,
                            LimiteFiltros,
                            idioma);
                        //hdfCantProveedoresB.Value = ((ArrayList)Session["ProveedoresB"]).Count.ToString();

                        break;
                    case "ImportadorExp":

                        //gdvFavoritos.ID = "gdvProveedores";
                        Session["ImportadoresExpB"] = FuncionesBusiness.GuardaSeleccionados(codPais, tipoOpe,
                            "ImportadorExp", listaSeleccionados,
                            (ArrayList) Session["ImportadoresExpB"], ref json, numFiltrosExistentes, idUsuario,

                            LimiteFiltros, idioma);

                        //hdfCantImportadoresExpB.Value = ((ArrayList)Session["ImportadoresExpB"]).Count.ToString();

                        break;

                }

                Session.Remove("TipoFavorito");

                Session.Remove("FavoritoF");

                return Json(new

                {
                    nuevosFiltros = json.Count > 0,
                    maximoLimiteFiltros = ((json.Count + numFiltrosExistentes) == LimiteFiltros),
                    objMensaje,
                    listNuevosFiltros = json

                });
            }
            return Json(new
            {
                objMensaje
            });
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult FavoritoFautocomplete(string codPais, string codPais2, string tipoOpe,
            string favoritoB, string idioma)
        {
            string codPaisT = codPais;

            if (codPais == "PEB" || codPais == "PEP")
            {
                codPaisT = "PE";
            }
            else

            {
                if (codPais2 == "4UE")

                    codPaisT = "UE" + codPais;
            }

            DataTable dt = FuncionesBusiness.BuscaFavoritos(Session["IdUsuario"].ToString(), codPaisT, tipoOpe,
                Session["TipoFavorito"].ToString(), favoritoB, false, idioma);

            List<MiFavorito> listaFavoritos = new List<MiFavorito>();
            if (dt.Rows.Count > 0)
            {
                listaFavoritos = dt.AsEnumerable().Select(m => new MiFavorito()
                {
                    IdFavorito = m.Field<int>("IdFavorito"),
                    Favorito = m.Field<string>("Favorito")
                }).ToList();
            }
            else
            {
                listaFavoritos.Add(new MiFavorito()
                {
                    IdFavorito = 0,
                    Favorito = "-"
                });
            }

            return Json(new
            {

                listaFavoritos
            });
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult AgregaFavoritoAutocompleteSelect(string idAutocompletado, string textFavoritoF, string tipoOpe,
            int numFiltrosExistentes, string idioma)
        {

            if (idAutocompletado == "")
            {
                return Json(new
                {
                    existeData = false
                });
            }

            var tipo = "";
            var nombre = "";
            var nuevoFiltro = true;
            var auxValue = "";

            var desabilitarControles = false;
            bool maximoLimiteFiltros = false;


            var nuevosFiltros = new List<object>();

            if (textFavoritoF.Length < 3 || textFavoritoF.Substring(0, 3) != "[G]")
            {
                Session["hdfFiltroSel"] = Session["TipoFavorito"].ToString();
                Session["hdfIDSel"] = idAutocompletado;


                nombre = AgregaFiltro(idioma, tipoOpe, ref tipo);
                if (nombre == "")
                    nuevoFiltro = false;

                auxValue = tipo + idAutocompletado;
                nuevosFiltros.Add(new
                {
                    text = nombre,
                    value = auxValue
                });
            }

            else
            {
                nombre = AgregaGrupoAFiltros(idAutocompletado, ref tipo);

                desabilitarControles = true;

                auxValue = tipo + PageID;
                nuevosFiltros.Add(new
                {
                    text = nombre,
                    value = auxValue
                });
            }

            if (nuevoFiltro)
            {
                ActualizarSessionListaFitros(auxValue, nombre);

                numFiltrosExistentes++;
                if (numFiltrosExistentes == LimiteFiltros)
                {
                    maximoLimiteFiltros = true;
                }
            }

            Session.Remove("TipoFavorito");
            Session.Remove("FavoritoF");



            return Json(new
            {
                existeData = true,
                nuevoFiltro,

                nuevosFiltros,
                desabilitarControles,
                maximoLimiteFiltros
            });
        }
        [SessionExpireFilter]
        [HttpPost]
        public JsonResult BuscarFclick(string txtFavoritoF, string codPais2, string codPais,
           string tipoOpe, string idioma)
        {
            Session["FavoritoF"] = txtFavoritoF;
            return CargarFavoritos(Session["TipoFavorito"].ToString(), codPais2, codPais, tipoOpe, idioma, 1, true);
        }
        [SessionExpireFilter]
        [HttpPost]
        public JsonResult RestablecerFclick(string codPais2, string codPais, string tipoOpe, string idioma)
        {
            Session["FavoritoF"] = "";
            return CargarFavoritos(Session["TipoFavorito"].ToString(), codPais2, codPais, tipoOpe, idioma, 1, true);
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult Restablecer_Click(string cboPaisValue, string cboPais2Value, string tipoOpe, string idioma)
        {
            //return Restablecer(cboPaisValue, cboPais2Value, tipoOpe, idioma);

            return Json(
                Restablecer(cboPaisValue, cboPais2Value, tipoOpe, idioma)
            );
        }


        /// <summary>
        /// Elimina las palabras no aceptadas por los filtros
        /// </summary>
        /// <param name="desComercialB"></param>
        /// <returns></returns>
        private string EliminaStopWords(string desComercialB)

        {
            var textoFin = "";
            var textoIni = desComercialB.Trim().Replace("  ", " ").ToUpper();
            if (textoIni != "")
            {
                var cantPalabras = 0;
                var palabras = textoIni.Split(' ');
                textoFin = "";
                foreach (var palabra in palabras)
                    if (!Consulta.EsStopWord(palabra))

                    {
                        cantPalabras += 1;
                        textoFin += palabra + " ";
                    }


                if (cantPalabras == 0)
                {
                    return "";
                }
            }


            textoFin = textoFin.Trim();
            return textoFin;
        }

        [SessionExpireFilter]
        [HttpPost]
        public ActionResult GetTabsView(string tipoOpe, string codPais2, string codPais, bool hideTabExcel = false , bool FlagRegMax = false)
        {
            GuardarLogInicioErrorSession("GetTabsView()");
            ValidaCodPais2(codPais2, ref codPais);
            ValidaCodPaisManif(ref codPais, tipoOpe);

            ConsultaForm model = new ConsultaForm();
            model.TipoOpe = tipoOpe;

            model.CodPais = codPais;
            model.IsOcultoMarcasModelos = (Session["Plan"].ToString() != "BUSINESS" &&
                                           Session["Plan"].ToString() != "PREMIUM" &&
                                           Session["PLAN"].ToString() != "UNIVERSIDADES");
            model.Importador = Funciones.ExisteVariable(codPais, tipoOpe, "IdImportador");
            model.Proveedor = Funciones.ExisteVariable(codPais, tipoOpe, "IdProveedor");
            model.Exportador = Funciones.ExisteVariable(codPais, tipoOpe, "IdExportador");
            model.ImportadorExp = Funciones.ExisteVariable(codPais, tipoOpe, "IdImportadorExp");
            model.PaisOrigen = Funciones.ExisteVariable(codPais, tipoOpe, "IdPaisOrigen");
            model.ExistePaisDestino = Funciones.ExisteVariable(codPais, tipoOpe, "IdPaisDestino");
            model.ExisteViaTransp = GetExisteViaTransp(codPais);
            model.ExisteAduana = GetExisteAduana(codPais);
            model.ExisteDua = GetExisteDua(model.ExisteAduana, codPais);
            model.ExisteDistrito = GetExisteDistrito(codPais);

            model.HideTabExcel = hideTabExcel;
            model.FlagRegMax = FlagRegMax;
            model.FlagVarVisibles = new TabMisBusquedas(tipoOpe, codPais);   //new FlagVarVisibles(codPais, tipoOpe, IsManifiesto(codPais));
            model.FlagVarVisibles.ExisteInfoComplementario = model.FlagVarVisibles.ExisteInfoComplementario && (bool)(Session["isVisibleInfoComplementario"]??false);
            /*if (Session["Plan"].ToString() == "ESENCIAL" )
            {
                model.ImportadorExp = model.FlagVarVisibles.ExisteImportadorExp = false;
                
            }*/
            //model.IsMostrarDetalleExcel = false;
            return PartialView("TabsView", model);
        }

        private void ActualizarSessionListaFitros(string pValue, string pText)
        {
            List<OptionSelect> listFilters;

            if (Session["lstFiltros"] != null)
                listFilters = Session["lstFiltros"] as List<OptionSelect>;

            else
                listFilters = new List<OptionSelect>();

            if (listFilters != null)
                listFilters.Add(new OptionSelect
                {
                    value = pValue,
                    text = pText
                });


            if (listFilters != null && listFilters.Count > 0)
                Session["lstFiltros"] = listFilters;
            else
                Session.Remove("lstFiltros");
        }

        private void ActualizarSessionListaFitros(List<OptionSelect> nuevosFiltros)
        {
            List<OptionSelect> listFilters;

            if (Session["lstFiltros"] != null)
                listFilters = Session["lstFiltros"] as List<OptionSelect>;
            else

                listFilters = new List<OptionSelect>();

            if (listFilters != null)
            {
                foreach (var item in nuevosFiltros)
                {

                    listFilters.Add(new OptionSelect

                    {
                        value = item.value,
                        text = item.text
                    });

                }
            }

            if (listFilters != null && listFilters.Count > 0)
                Session["lstFiltros"] = listFilters;

            else
                Session.Remove("lstFiltros");
        }


#region "Métodos de Busqueda"

        [HttpPost]
        public JsonResult BuscarClick(string codPais, string codPais2,
            string tipoOpe, string anioMesIni, string anioMesFin,
            int indexCboPaisB, string codPaisB, string idioma, string codPaisText)
        {
            String codPaisT = codPais2;
            if (codPais2 == "4UE")
            {
                codPaisT = "UE";
            }

            if (!(bool)(Session["opcionFreeTrial"] ?? false) && !Funciones.ValidaPaisBusqueda(Session["idUsuario"].ToString(), codPaisT) && !Funciones.ValidaPaisBusqueda(Session["idUsuario"].ToString(), codPais))
            {
                string mensaje =
                    "Su búsqueda infringe las normas del sitio.";
                if (idioma == "en")
                    mensaje = "Your search is not permitted.";
                object objMensaje = new
                {
                    titulo = Resources.Resources.Search_Button,
                    mensaje,
                    flagContactenos = false
                };

                object jsonErr = (new
                {
                    objMensaje,
                    totalRecordsFound = "0"
                });

                return new JsonResult()
                {
                    ContentEncoding = Encoding.Default,
                    ContentType = "application/json",
                    Data = jsonErr,
                    MaxJsonLength = int.MaxValue
                };
            }

            bool tipoUsuarioEsFreeTrial = (Session["TipoUsuario"] != null && Session["TipoUsuario"].ToString() == "Free Trial") || (bool)(Session["opcionFreeTrial"] ?? false);
            if (tipoUsuarioEsFreeTrial) {
                if (codPaisT != "UE")
                    codPaisT = codPais;
                ValidaCodPaisManif(ref codPaisT, tipoOpe);
                FiltroPeriodo filtroPeriodo = CargaPeriodos(codPaisT, tipoOpe);

                DateTime fecIni = DateTime.ParseExact(String.Concat(anioMesIni, "01"), "yyyyMMdd", null);
                DateTime fecFin = DateTime.ParseExact(String.Concat(anioMesFin, "01"), "yyyyMMdd", null);

                if (fecIni < filtroPeriodo.FechaInfoIni)
                {
                    anioMesIni = filtroPeriodo.FechaInfoIni.ToString("yyyyMM");
                }

                if (fecFin > filtroPeriodo.FechaInfoFin)
                {
                    anioMesFin = filtroPeriodo.FechaInfoFin.ToString("yyyyMM");
                }

            }
            var json = BuscaInicio(true, codPais, codPais2, tipoOpe, anioMesIni, anioMesFin, indexCboPaisB, codPaisB,
                idioma, codPaisText);

            //return json;
            return new JsonResult()
            {
                ContentEncoding = Encoding.Default,
                ContentType = "application/json",
                Data = json,
                //JsonRequestBehavior = requestBehavior,
                MaxJsonLength = int.MaxValue
            };
        }

        private object BuscaInicio(bool flagBusca, string codPais, string codPais2,
            string tipoOpe, string anioMesIni, string anioMesFin,
            int indexCboPaisB, string codPaisB, string idioma , string codPaisText)
        {
            var specificCulture = GetSpecificCulture();

            var flagVisibleArchivo = false;
            var flagVisibleArchivo2 = false;

            string auxCodPais = codPais;
            ValidaCodPais2(codPais2, ref codPais);
            ValidaCodPaisManif(ref codPais, tipoOpe);
            bool IsManifiesto = this.IsManifiesto(codPais);
            string idUsuario = Session["IdUsuario"].ToString();
            var codPaisT = codPais;
            var paisT = codPaisText;
            object objMensaje = null;

            if (codPais2 == "4UE")
            {
                codPaisT = "UE";
                paisT = "UE - " + paisT;
            }


            //if (!Funciones.ValidaPais(idUsuario, codPais))
            //{
            //    string mensaje = "El país seleccionado (" + paisT + ") no está incluído en el plan contratado.";
            //    if (idioma == "en")
            //        mensaje = "Selected country: " + paisT + " is not included in your plan.";

            //    objMensaje = new
            //    {
            //        titulo = "Veritrade",
            //        mensaje,
            //        flagContactenos = true
            //    };

            //    return Json(new
            //    {
            //        objMensaje

            //    });
            //}
            //else
            //{
            //    if (Funciones.FlagCarga(codPaisT))
            //    {
            //        string mensaje = "Estamos actualizando la información de " + paisT +
            //                         ". Por favor consulte mas tarde.";
            //        if (idioma == "en")
            //            mensaje = "We are updating data for " + paisT + ". Please return later.";


            //        objMensaje = new
            //        {
            //            titulo = "Veritrade",
            //            mensaje,
            //            flagContactenos = true
            //        };
            //        return Json(new
            //        {
            //            objMensaje

            //        });
            //    }objTabData.FlagRegMax = cantReg > CantRegMax;
            //}

            bool FlagPalabras = (!string.IsNullOrWhiteSpace(Session["hdfPalabrasY"]?.ToString()));

            var tabla = "";
            var dua = "";
            GetTablaAndDua(tipoOpe, codPais, ref tabla, ref dua);

            Session["SqlFiltro"] = GeneraSqlFiltro(codPais, codPais2, tipoOpe, anioMesIni, anioMesFin, indexCboPaisB,
                codPaisB, dua, auxCodPais);

            //Extensiones.SetCookie("sl", Session["sqlFiltro"].ToString() + " $ " + Session["CodPais"].ToString() + " $ " + Session["TipoOpe"].ToString(), TimeSpan.FromDays(360));

            Extensiones.SetCookie("sl", "", TimeSpan.FromDays(360));
            GuardarLogInicioErrorSession("BuscaInicio()");
            /*Session["SqlTablas"] = GeneraSqlFiltroTablas(tipoOpe);
            Session["SqlTablasTabs"] = GeneraSqlFiltroTablas(tipoOpe == "I" ? "E" : "I");
            Session["SqlFiltroTablas"] = GenerarTablas((List<string>)Session["SqlTablas"]);
            Session["SqlFiltroTablasTabs"] = GenerarTablas((List<string>)Session["SqlTablasTabs"]);*/

            string lblResultado = "";


            if (useRand)
            {
                Session["useRand"] = true;
            }
            else
            {
                Session["useRand"] = false;
            }

            string sql = Session["SqlFiltro"].ToString();

            /*string path = ConfigurationManager.AppSettings["directorio_logs"];
            Logs oLog = new Logs(path);
            oLog.Add("IdUsuario => " + Session["idUsuario"].ToString(), Session["idUsuario"].ToString());
            oLog.Add("Consulta Generada => " + sql, Session["idUsuario"].ToString());*/

            if (Session["UltSqlFiltro"] != null &&
                Session["UltSqlFiltro"].ToString() == Session["SqlFiltro"].ToString())
            {
                string mensaje =
                    "Ya realizó esta búsqueda.<br>Le sugerimos que cambie el rango de fechas y/o modifique sus filtros de búsqueda";
                if (idioma == "en")
                    mensaje =
                        "You already did this search.<br>We suggest to change dates range and/or modify search filters";


                objMensaje = new
                {
                    titulo = Resources.Resources.Search_Button,
                    mensaje,
                    flagContactenos = false
                };

                return (new
                {
                    objMensaje

                });
            }

            else
            {
                Session["UltSqlFiltro"] = Session["SqlFiltro"].ToString();
            }

            int cantReg = 0;
            string cifTot = "";

            string pesoNeto = "";
            string valueCifTot = "";
            decimal valuePesoNeto = 0;
            string unidad = "";
            
            GetCifTot_PesoNeto_CantReg_ValueCifTot_Unidad_ValuePesoNeto(codPais, tipoOpe, tabla, ref cifTot,
                    ref pesoNeto, ref cantReg, ref valueCifTot, ref unidad, ref valuePesoNeto);

            bool hideTabExcel = (cantReg > CantRegMax && !FlagPalabras);

            List<object> listGridData = null;
                   
            if (cantReg == 0)
            {
                if (Session["UltSqlFiltro"] == null ||
                    Session["UltSqlFiltro"].ToString() != Session["SqlFiltro"].ToString())
                {
                    Session["UltSqlFiltro"] = Session["SqlFiltro"].ToString();

                    string mensaje =
                        "Su búsqueda no encontró resultados.<br>Le sugerimos que cambie el rango de fechas y/o modifique sus filtros de búsqueda";

                    if (idioma == "en")
                        mensaje =
                            "Your search found no results.<br>We suggest to change dates range and/or modify search filters";

                    objMensaje = new
                    {
                        titulo = Resources.Resources.Search_Button,
                        mensaje,
                        flagContactenos = false
                    };
                }
                else
                {
                    string mensaje = "La búsqueda con los últimos filtros NO encontró resultados";
                    if (idioma == "en")

                        mensaje = "Search with last filters found no results";


                    objMensaje = new

                    {
                        titulo = Resources.Resources.Search_Button,
                        mensaje,
                        flagContactenos = false
                    };

                    //foreach (ListItem item in lstEliminar.Items)
                    //    lstFiltros.Items.Remove(item);
                    //lstEliminar.Items.Clear();
                }

                return (new
                {
                    objMensaje,
                    totalRecordsFound = Resources.AdminResources.NoRecordsFound_Text
                });
            }
            else if (cantReg > CantRegMax && FlagPalabras)
            {
                string mensaje = "Su búsqueda incluye Descripción Comercial y supera el límite de " +
                                 CantRegMax.ToString("n0") + " registros.<br>";
                mensaje += "Reduzca el rango de fechas y/o modifique sus filtros de búsqueda";

                objMensaje = new
                {
                    titulo = Resources.Resources.Search_Button,
                    mensaje,
                    flagContactenos = false
                };
            }
            else if (hideTabExcel)
            {
                string mensaje = "Su búsqueda supera el límite de " + CantRegMax.ToString("n0") +
                                 " registros y no puede ser descargada en Excel. ";
                mensaje +=
                    "Si desea ver todas las pestañas habilitadas y/ó descargar en Excel, reduzca el rango de fechas y/o modifique sus filtros de búsqueda";

                if (idioma == "en")
                {
                    mensaje = "Your search exceeds " + CantRegMax.ToString("n0") +
                              " records limit and it can not be download to Excel. ";
                    mensaje +=
                        "If you want to see all tabs enabled and/or download to Excel, reduce the dates range and/or modify your filters search";
                }

                objMensaje = new
                {
                    titulo = Resources.Resources.Search_Button,
                    mensaje,
                    flagContactenos = false,
                };

                //Session["SqlFiltro"] = GeneraSqlFiltro(codPais, codPais2, tipoOpe, anioMesIni, anioMesFin,
                //    indexCboPaisB, codPaisB, dua);

                //drTotales = FuncionesBusiness.CalculaTotales(Session["SqlFiltro"].ToString(), cifTot, codPais, pesoNeto,
                //    tabla);



                //cantReg = Convert.ToInt32(drTotales["CantReg"]);

                //Session["hdfCantReg"] = drTotales["CantReg"].ToString();


                //Session["hdfCIFTot"] = drTotales[cifTot].ToString();


                FuncionesBusiness.RegistraConsumo(Session["IdUsuario"].ToString(), codPais, tipoOpe, "Mis Busquedas",
                    Session["SqlFiltro"].ToString());


                if (idioma == "es")
                {
                    lblResultado = "Se encontraron " + cantReg.ToString("n0") + " registros";
                }
                else

                {
                    lblResultado = cantReg.ToString("n0") + " records were found";
                }

                hideTabExcel = Funciones.VisualizarExcel();
                //string anioMesIni = "", string anioMesFin = "", string codPaisB = ""
                //falta completar
                listGridData = Busca(codPais, tipoOpe, cifTot, pesoNeto, idioma, dua, tabla, indexCboPaisB, valueCifTot,
                    valuePesoNeto, unidad, cantReg, specificCulture, hideTabExcel, anioMesIni: anioMesIni, anioMesFin: anioMesFin, codPaisB: codPaisB);
            }
            else

            {
                listGridData = Busca(codPais, tipoOpe, cifTot, pesoNeto, idioma, dua, tabla, indexCboPaisB, valueCifTot,
                    valuePesoNeto, unidad, cantReg, specificCulture, false, anioMesIni: anioMesIni, anioMesFin: anioMesFin, codPaisB: codPaisB, auxCodPais: auxCodPais, useRand: useRand);
            }

            
            if (listGridData != null)
            {
                Session["BotonUtilizado"] = true;
            }

            return (new
            {
                objMensaje,
                totalRecordsFound = GetDescriptionRecordsFound(cantReg, specificCulture,idioma), //cantReg.ToString("n0", specificCulture),
                gridData = listGridData,
                hideTabExcel = hideTabExcel,
                FlagRegMax = cantReg > CantRegMax
            });
        }

        private List<string> GenerarSeleccionTablas(string tipoOpe)
        {
            List<string> tablaResumen = (List<string>)Session["SqlTablas"];
            List<string> tablaTabs =  (List<string>)Session["SqlTablasTabs"];
            List<string> tablaSeleccionadas = new List<string>();
            
            foreach (var item in tablaResumen)
            {
                string fil = item;
                string codPais = item.Substring(12);
                bool isManifiesto = codPais.Length == 3 && (codPais.Substring(2) == "I" || codPais.Substring(2) == "E");

                if (isManifiesto)
                {
                    if(tipoOpe == "I")
                        fil = fil.Substring(0,14) + "E";
                    else
                        fil = fil.Substring(14) + "I";
                }

                if (fil.Contains("Importacion_"))
                    fil = fil.Replace("Importacion_", "Exportacion_");
                else if (fil.Contains("Exportacion_"))
                    fil = fil.Replace("Exportacion_", "Importacion_");

                if (tablaTabs.Contains(fil))
                {
                    tablaSeleccionadas.Add(item);
                }

            }


            return tablaSeleccionadas;
        }

        private string GenerarTablas(List<string> tablas)
        {

            //List<string> tablas = (List<string>)Session["SqlTablas"];
            List<object> listas = new List<object>();
            string sqlFiltro = Session["SqlFiltro"].ToString();


            /*int posicion = sqlFiltro.IndexOf("FechaNum");

            string parte = sqlFiltro.Substring(posicion, 49);

            sqlFiltro = sqlFiltro.Replace(parte, "");*/

            string sqlGeneral = "";
            int num = 1;
            foreach (var item in tablas)
            {

                var sqlFiltroAux = sqlFiltro;
                string tabla = item;
                string codPais = item.Substring(12);
                string tipoOpe = item.Substring(0, 1);
                bool isManif = IsManifiesto(codPais);
                var cifTot = GetCIFTot(codPais, tipoOpe);
                var pesoNeto = "";
                if (!isManif) pesoNeto = Funciones.CampoPeso(codPais, tipoOpe);

                string cifTot1 = cifTot;

                if (codPais == "BR" || codPais == "IN")
                    cifTot1 = "convert(decimal(19,2), " + cifTot + ")";

                var pesoNeto1 = pesoNeto;
                var pesoNeto2 = pesoNeto;

                if (pesoNeto1 == "")
                {
                    pesoNeto1 = "0";
                    pesoNeto2 = "PesoNeto";
                }

                var tabla1 = tabla;

                var sql = string.Empty;

                string auxCodPais = codPais;
                if (auxCodPais.Length == 3 && (auxCodPais.Substring(2) == "I" || auxCodPais.Substring(2) == "E"))
                {
                    auxCodPais = codPais.Substring(0, 2) + "_";
                }

                //DataTable DescripcionPais = FuncionesBusiness.BuscarPais("'"+auxCodPais+"'", GetCurrentIdioma());
                string DescripcionPais = "Union Europea";
                if (auxCodPais != "UE")
                    DescripcionPais = FuncionesBusiness.BuscarPais("'" + auxCodPais + "'", GetCurrentIdioma()).Rows[0]["Descripcion"].ToString();


                if (sqlFiltroAux.Contains("Partida_"))
                {
                    int pos = sqlFiltroAux.IndexOf("Partida_");
                    string fil = sqlFiltroAux.Substring(pos, 10);
                    sqlFiltroAux = sqlFiltroAux.Replace(fil, "Partida_" + codPais);

                }

                if (sqlFiltroAux.Contains("Importador_"))
                {
                    int pos = sqlFiltroAux.IndexOf("Importador_");
                    string fil = sqlFiltroAux.Substring(pos, 10);
                    sqlFiltroAux = sqlFiltroAux.Replace(fil, "Importador_" + codPais);

                }

                if (!isManif)
                {
                    sql = "select '" + codPais + "' as IdPais ,'"+ DescripcionPais + "' as Pais,count(*) as CantReg, sum(isnull(" + cifTot1 + ",0)) as ciftot ,'" + cifTot + "' as tipoTop, sum (isnull(" + pesoNeto1 +
                              ",0)) as pesoNeto, sum(isnull(Cantidad,0)) as Cantidad ";
                    sql += "from " + tabla1 + " T where 1 = 1 ";
                    sql += sqlFiltroAux;
                }
                else
                {
                    sql = "select '" + codPais + "' as IdPais ,'" + DescripcionPais + "' as Pais,count(*) as CantReg, sum(isnull(" + cifTot + ",0)) / 1000 as ciftot ,'" + cifTot + "' as tipoTop , sum(isnull(" + cifTot + ",0)) / 1000 as pesoNeto, 0 Cantidad ";
                    sql += "from " + tabla + " T where 1 = 1 ";
                    sql += sqlFiltroAux;
                }

                sqlGeneral += " union " + sql;
                num++;
                //if(tipoOpe == "E")
                //{
                //    if(sql.Contains("IdPaisOrigen"))
                //}

                //DataRow dr;
                //try
                //{
                //    var dt = Conexion.SqlDataTable(sql);
                //    dr = dt.Rows[0];
                //}
                //catch (Exception e)
                //{
                //    dr = null;
                //}
                //listas.Add(dr);


            }
            if(sqlGeneral.Length > 7)
            {
                sqlGeneral = sqlGeneral.Substring(7);
            }
            

            int page = 1;
            int maximumRows = ResumenTabsGridPageSize;

            int startRowIndex = (page - 1) * maximumRows;

            //sqlGeneral = $@"select * from
            //                ({sqlGeneral}) T where T.CantReg > 0 and Nro between " + (startRowIndex + 1).ToString() + " and " +
            //                    (startRowIndex + maximumRows).ToString();

            sqlGeneral = $@"select *,  ROW_NUMBER() OVER(ORDER BY T.ciftot desc) AS Nro from
                            ({sqlGeneral}) T where T.CantReg > 0 ";

            //DataRow dr;
            //try
            //{
            //    var dt = Conexion.SqlDataTable(sqlGeneral);
            //    dr = dt.Rows[0];
            //}
            //catch (Exception e)
            //{
            //    dr = null;
            //}
            //listas.Add(dr);

            return sqlGeneral;
        }

        private List<object> Busca(string codPais, string tipoOpe, string cifTot, string pesoNeto,
            string idioma, string dua, string tabla,
            int indexCboPaisB, string valueCifTot, decimal valuePesoNeto,
            string unidad, int totalRegistros, CultureInfo specificCulture, bool hideTabExcel,
            string anioMesIni = "", string anioMesFin = "", string codPaisB = "",string auxCodPais = "", bool useRand = false)
        {
            //string sessionSqlFiltro = Session["SqlFiltro"].ToString();

            var flags = new TabMisBusquedas(tipoOpe, codPais);   //new FlagVarVisibles(codPais, tipoOpe, IsManifiesto(codPais));

            /*if (Session["Plan"].ToString() == "ESENCIAL" && tipoOpe == "E")
            {
                flags.ExisteImportadorExp = false;
            }*/

            FuncionesBusiness.RegistraConsumo(Session["IdUsuario"].ToString(), codPais, tipoOpe, "Mis Busquedas",
                Session["SqlFiltro"].ToString());


            var json = new List<object>();


            string totalRegistrosFormateado = totalRegistros.ToString("n0", specificCulture);

            

            string cifTotFormateado = Convert.ToDecimal(valueCifTot).ToString("n" + (!flags.IsManifiesto ? "0" : "1"), specificCulture);
            string pesoNetoFormateado = valuePesoNeto.ToString("n0", specificCulture);



            


            if (flags.ExistePartida)
                json.Add(GetDataObjectByFilter(Enums.Filtro.Partida.ToString(), codPais, tipoOpe, cifTot, pesoNeto, dua, tabla,
                    valueCifTot, indexCboPaisB, totalRegistrosFormateado, cifTotFormateado, pesoNetoFormateado,
                    specificCulture, idioma, unidad, false, hideTabExcel: hideTabExcel, useRand: useRand));

            if (/*codPais == "PEB" && !flags.IsManifiesto*/ flags.ExisteMarcasModelos  )
            {
                json.Add(GetDataObjectByFilter(Enums.Filtro.Marca.ToString(), codPais, tipoOpe, cifTot, pesoNeto, dua, tabla,
                    valueCifTot, indexCboPaisB, totalRegistrosFormateado, cifTotFormateado, pesoNetoFormateado,
                    specificCulture, idioma, unidad, false, hideTabExcel: hideTabExcel, useRand: useRand));

                json.Add(GetDataObjectByFilter(Enums.Filtro.Modelo.ToString(), codPais, tipoOpe, cifTot, pesoNeto, dua, tabla,
                    valueCifTot, indexCboPaisB, totalRegistrosFormateado, cifTotFormateado, pesoNetoFormateado,
                    specificCulture, idioma, unidad, false,  hideTabExcel: hideTabExcel, useRand: useRand));
            }

            if (tipoOpe == Enums.TipoOpe.I.ToString())
            {
                if (flags.ExisteImportador) {
                    json.Add(GetDataObjectByFilter(Enums.Filtro.Importador.ToString(), codPais, tipoOpe, cifTot, pesoNeto, dua, tabla,
                        valueCifTot, indexCboPaisB, totalRegistrosFormateado, cifTotFormateado, pesoNetoFormateado,
                        specificCulture, idioma, unidad, hideTabExcel: hideTabExcel, useRand: useRand));
                }

                if (flags.ExisteNotificado)
                {
                    json.Add(GetDataObjectByFilter(Enums.Filtro.Notificado.ToString(), codPais, tipoOpe, cifTot, pesoNeto, dua, tabla,
                        valueCifTot, indexCboPaisB, totalRegistrosFormateado, cifTotFormateado, pesoNetoFormateado,
                        specificCulture, idioma, unidad, hideTabExcel: hideTabExcel, useRand: useRand));
                }

                if (flags.ExisteProveedor)
                {
                    json.Add(GetDataObjectByFilter(Enums.Filtro.Proveedor.ToString(), codPais, tipoOpe, cifTot, pesoNeto, dua, tabla, valueCifTot,
                      indexCboPaisB, totalRegistrosFormateado, cifTotFormateado, pesoNetoFormateado, specificCulture,
                      idioma, unidad, hideTabExcel: hideTabExcel, useRand: useRand));
                }

                if (flags.ExistePaisOrigen) 
                {
                    json.Add(GetDataObjectByFilter(!flags.IsManifiesto ? Enums.Filtro.PaisOrigen.ToString() : Enums.Filtro.PaisEmbarque.ToString(), codPais, tipoOpe, cifTot, pesoNeto, dua, tabla,
                       valueCifTot, indexCboPaisB, totalRegistrosFormateado, cifTotFormateado, pesoNetoFormateado,
                       specificCulture, idioma, unidad, hideTabExcel: hideTabExcel, anioMesIni : anioMesIni, anioMesFin : anioMesFin, codPaisB : codPaisB,auxCodPais : auxCodPais, useRand: useRand));
                }
            }
            else
            {
                if (flags.ExisteExportador) {
                    json.Add(GetDataObjectByFilter(Enums.Filtro.Exportador.ToString(), codPais, tipoOpe, cifTot, pesoNeto, dua, tabla,
                    valueCifTot, indexCboPaisB, totalRegistrosFormateado, cifTotFormateado, pesoNetoFormateado,
                    specificCulture, idioma, unidad, hideTabExcel: hideTabExcel, useRand: useRand));
                }

                if (flags.ExisteImportadorExp && Session["Plan"].ToString() != "ESENCIAL")
                {
                    json.Add(GetDataObjectByFilter(Enums.Filtro.ImportadorExp.ToString(), codPais, tipoOpe, cifTot, pesoNeto, dua, tabla,
                    valueCifTot, indexCboPaisB, totalRegistrosFormateado, cifTotFormateado, pesoNetoFormateado,
                    specificCulture, idioma, unidad, hideTabExcel: hideTabExcel, useRand: useRand));
                }

                if (flags.ExistePaisDestino)
                {
                    json.Add(GetDataObjectByFilter(Enums.Filtro.PaisDestino.ToString(), codPais, tipoOpe, cifTot, pesoNeto, dua, tabla,
                    valueCifTot, indexCboPaisB, totalRegistrosFormateado, cifTotFormateado, pesoNetoFormateado,
                    specificCulture, idioma, unidad, hideTabExcel: hideTabExcel, anioMesIni: anioMesIni, anioMesFin: anioMesFin, codPaisB: codPaisB, auxCodPais: auxCodPais, useRand: useRand));
                }   
            }

            //ExisteMarca
            if (flags.ExisteMarcaEC)
            {
                json.Add(GetDataObjectByFilter(Enums.Filtro.MarcaEC.ToString(), codPais, tipoOpe, cifTot, pesoNeto, dua, tabla,
                    valueCifTot, indexCboPaisB, totalRegistrosFormateado, cifTotFormateado, pesoNetoFormateado,
                    specificCulture, idioma, unidad,
                    false, hideTabExcel: hideTabExcel, useRand: useRand));

            }

            //ExisteViaTransp
            if (flags.ExisteViaTransp)
            {
                json.Add(GetDataObjectByFilter(Enums.Filtro.ViaTransp.ToString(), codPais, tipoOpe, cifTot, pesoNeto, dua, tabla,
                    valueCifTot, indexCboPaisB, totalRegistrosFormateado, cifTotFormateado, pesoNetoFormateado,
                    specificCulture, idioma, unidad,
                    false, hideTabExcel: hideTabExcel, useRand: useRand));

            }
            if (flags.ExisteAduana)
            {
                if (flags.ExisteDUA)
                {
                    json.Add(GetDataObjectAduanaDua(Enums.Filtro.AduanaDUA.ToString(), codPais, cifTot, pesoNeto, dua, tabla, valueCifTot, indexCboPaisB, totalRegistrosFormateado, cifTotFormateado, specificCulture, idioma, hideTabExcel: hideTabExcel, useRand: useRand));
                }
                else
                {
                    json.Add(GetDataObjectAduanaDua(Enums.Filtro.Aduana.ToString(), codPais, cifTot, pesoNeto, dua, tabla, valueCifTot, indexCboPaisB, totalRegistrosFormateado, cifTotFormateado, specificCulture, idioma, hideTabExcel: hideTabExcel, useRand: useRand));
                }
            }

            if (flags.ExisteDistrito)
            {
                json.Add(GetDataObjectByFilter(Enums.Filtro.Distrito.ToString(), codPais, tipoOpe, cifTot, pesoNeto, dua, tabla,
                    valueCifTot, indexCboPaisB, totalRegistrosFormateado, cifTotFormateado, pesoNetoFormateado,
                    specificCulture, idioma, unidad,
                    false, hideTabExcel: hideTabExcel, useRand: useRand));

            }

            if (flags.ExistePtoDescarga || flags.ExistePtoEmbarque || flags.ExistePtoDestino)
            {
                json.Add(GetDataObjectByFilter(
                        flags.ExistePtoDescarga ? Enums.Filtro.PtoDescarga.ToString():
                        flags.ExistePtoEmbarque ? Enums.Filtro.PtoEmbarque.ToString() :
                        Enums.Filtro.PtoDestino.ToString(), 
                        codPais, tipoOpe, cifTot, pesoNeto, dua, tabla,
                    valueCifTot, indexCboPaisB, totalRegistrosFormateado, cifTotFormateado, pesoNetoFormateado,
                    specificCulture, idioma, unidad,
                    false, hideTabExcel: hideTabExcel, useRand: useRand));

            }

            if (flags.ExisteManifiesto)
                json.Add(GetDataObjectByFilter(Enums.Filtro.Manifiesto.ToString(), codPais, tipoOpe, cifTot, pesoNeto, dua, tabla,
                    valueCifTot, indexCboPaisB, totalRegistrosFormateado, cifTotFormateado, pesoNetoFormateado,
                    specificCulture, idioma, unidad, false, hideTabExcel: hideTabExcel, useRand: useRand));

            /*if (flags.ExistePaisOrigen && tipoOpe == "I")
            {
                int cantReg = 0;
                GetTablaAndDua("E", codPais, ref tabla, ref dua);
                GetCifTot_PesoNeto_CantReg_ValueCifTot_Unidad_ValuePesoNeto(codPais, tipoOpe, tabla, ref cifTot,
                ref pesoNeto, ref cantReg, ref valueCifTot, ref unidad, ref valuePesoNeto);

                json.Add(GetDataObjectByFilter(!flags.IsManifiesto ? Enums.Filtro.PaisOrigen.ToString() : Enums.Filtro.PaisEmbarque.ToString(), codPais, "E", cifTot, pesoNeto, dua, tabla,
                   valueCifTot, indexCboPaisB, totalRegistrosFormateado, cifTotFormateado, pesoNetoFormateado,
                   specificCulture, idioma, unidad, hideTabExcel: hideTabExcel));
            }

            if (flags.ExistePaisDestino && tipoOpe == "E")
            {
                int cantReg = 0;
                GetTablaAndDua("I", codPais, ref tabla, ref dua);
                GetCifTot_PesoNeto_CantReg_ValueCifTot_Unidad_ValuePesoNeto(codPais, tipoOpe, tabla, ref cifTot,
                ref pesoNeto, ref cantReg, ref valueCifTot, ref unidad, ref valuePesoNeto);

                json.Add(GetDataObjectByFilter(!flags.IsManifiesto ? Enums.Filtro.PaisOrigen.ToString() : Enums.Filtro.PaisEmbarque.ToString(), codPais, "I", cifTot, pesoNeto, dua, tabla,
                   valueCifTot, indexCboPaisB, totalRegistrosFormateado, cifTotFormateado, pesoNetoFormateado,
                   specificCulture, idioma, unidad, hideTabExcel: hideTabExcel));
            }*/
            if (flags.ExisteInfoTabla) {
                Session["SqlTablas"] = GeneraSqlFiltroTablas(tipoOpe);
                Session["SqlTablasTabs"] = GeneraSqlFiltroTablas(tipoOpe == "I" ? "E" : "I");

                Session["tableSeleccionadas"] = GenerarSeleccionTablas(tipoOpe);

                Session["SqlFiltroTablas"] = GenerarTablas((List<string>)Session["SqlTablas"]);
                Session["SqlFiltroTablasTabs"] = GenerarTablas((List<string>)Session["SqlTablasTabs"]);
                json.Add(GetDataObjectByFilter(!flags.IsManifiesto ? Enums.Filtro.InfoTabla.ToString() : Enums.Filtro.PaisEmbarque.ToString(), codPais, tipoOpe, cifTot, pesoNeto, dua, tabla,
                       valueCifTot, indexCboPaisB, totalRegistrosFormateado, cifTotFormateado, pesoNetoFormateado,
                       specificCulture, idioma, unidad, hideTabExcel: hideTabExcel, useRand: useRand));
            }
                
            return json;
        }

        private object GetDataObjectByFilter(string filtro, string codPais, string tipoOpe,
            string cifTot, string pesoNeto, string dua,
            string tabla, string valueCifTot, int indexCboPaisB,
            string totalRegistrosFormateado, string cifTotFormateado, string pesoNetoFormateado,
            CultureInfo specificCulture, string idioma, string unidad,
            bool validaExisteFiltro = true, bool hideTabExcel = false , string anioMesIni = "", 
            string anioMesFin = "", string codPaisB = "",string auxCodPais = "", bool useRand = false)
        {
            

            var isManif = IsManifiesto(codPais);
            object obj = new
            {
                tabDataName = filtro,
                tabDataNumRows = 0
            };

            
            if (validaExisteFiltro && !isManif  && filtro != Enums.Filtro.InfoTabla.ToString())
            {
                //if (Funciones.ExisteVariable(codPais, tipoOpe, "Id" + filtro))
                {
                    var cuenta = CuentaAgrupado(filtro, tabla, dua);
                    if (cuenta > 0)
                    {
                        var listDataTable = CargaFiltro(filtro, -1, codPais, cifTot, pesoNeto, idioma, dua, tabla,
                            indexCboPaisB, Enums.TipoFiltro.Todos, hideTabExcel: hideTabExcel, useRand: useRand);

                        List<string> PaisComplementariosAux = new List<string>();
                        if((filtro == "PaisDestino" || filtro == "PaisOrigen") && listDataTable.Count > 2)
                        {
                            DataTable dt = listDataTable[2];
                            foreach (DataRow drRow in dt.Rows)
                            {
                                PaisComplementariosAux.Add(drRow[filtro].ToString().ToUpper());//cantReg = Convert.ToInt64(drRow["CantReg"]);
                            }
                        }
                        Session["PaisesComplementarioOrigenDestino"] = PaisComplementariosAux;

                        TabData objData = GetTabDataByFilter(filtro, cifTot, cuenta, pesoNeto, unidad, codPais);
                        objData.CodPais = codPais;
                        
                        objData.IsVisibleInfoComplementario = objData.IsVisibleInfoComplementario && _paisesConInfoComplementaria.Contains(codPais);
                        objData.TotalRegistros = totalRegistrosFormateado;
                        objData.CiFoFobTotal = cifTotFormateado;
                        objData.PesoNeto = pesoNetoFormateado;
                        objData.ListRows =
                            GetDataTableToListGridRow(listDataTable[0], filtro, cifTot, valueCifTot, codPais,
                                specificCulture, anioMesIni: anioMesIni, anioMesFin: anioMesFin, indexCboPaisB: indexCboPaisB, codPaisB : codPaisB, auxCodPais : auxCodPais);

                        objData.ListRowsTab = GetDataTableToListGridRowTab(listDataTable[1], filtro, cifTot,
                            valueCifTot, codPais, specificCulture, pesoNeto, unidad, anioMesIni: anioMesIni, anioMesFin: anioMesFin, indexCboPaisB: indexCboPaisB, codPaisB: codPaisB, auxCodPais: auxCodPais);
                        objData.HideTabExcel = hideTabExcel;
                        if (Session["dt" + filtro + "2"] != null && Convert.ToBoolean(Session["dt" + filtro + "2"]))

                        {
                            objData.GridHead.IsVisibleFiltroCboDescripcion = true;
                            objData.ListRowsCbo = GetDataTableToListCbo(listDataTable[2], filtro);
                        }
                        else
                        {
                            objData.GridHead.IsVisibleFiltroCboDescripcion = false;
                        }

                        obj = new
                        {
                            tabDataName = filtro,
                            tabDataNumRows = cuenta,
                            resumenTabDataList =
                                RenderViewToString(this.ControllerContext, "GridViews/ResumenGridView", objData),

                            resumenTotalPages = objData.TotalPaginasResumen,
                            tabDataList = RenderViewToString(this.ControllerContext, "GridViews/TabGridView", objData),
                            tabTotalPages = objData.TotalPaginasTab,
                            pieTitle = objData.TituloTab,
                            tabPieData = GetJsonDataPie(filtro, listDataTable[1], cifTot, valueCifTot, specificCulture)
                        };
                    }
                }
            }
            else if(filtro == Enums.Filtro.InfoTabla.ToString())
            {
                //filtro = "PaisOrigen"; // remove here
                var cuenta = CuentaAgrupadoInfoComplementario();

                if (cuenta > 0)
                {
                    cifTot = "ciftot";
                    pesoNeto = "pesoneto";
                    string totalCifTotResumen = FuncionesBusiness.TotalInfoComplementario(Session["sqlFiltroTablas"].ToString());
                    string totalCifTotTabs = FuncionesBusiness.TotalInfoComplementario(Session["sqlFiltroTablasTabs"].ToString());

                    totalCifTotResumen = totalCifTotResumen == "" ? "0" : totalCifTotResumen;
                    totalCifTotTabs = totalCifTotTabs == "" ? "0" : totalCifTotTabs;

                    var listDataTable = CargaFiltro(filtro, -1, codPais, cifTot, pesoNeto, idioma, dua, tabla,
                        indexCboPaisB, Enums.TipoFiltro.Todos, hideTabExcel: hideTabExcel);


                    TabData objData = GetTabDataByFilter(filtro, cifTot, cuenta, pesoNeto, unidad, codPais);
                    objData.TotalRegistros = totalRegistrosFormateado;
                    objData.CiFoFobTotal = cifTotFormateado;

                    objData.CodPais = codPais;
                    

                    objData.HideTabExcel = hideTabExcel;

                    objData.PesoNeto = pesoNetoFormateado;
                    objData.ListRows =
                        GetDataTableToListGridRow(listDataTable[0], filtro, cifTot, totalCifTotResumen, codPais,
                            specificCulture);
                    objData.ListRowsTab = GetDataTableToListGridRowTab(listDataTable[1], filtro, "ciftot",
                        totalCifTotTabs, codPais, specificCulture, "pesoneto", unidad);
                    objData.HideTabExcel = hideTabExcel;
                    //if (Session["dt" + filtro + "2"] != null && Convert.ToBoolean(Session["dt" + filtro + "2"]))
                    //{
                        objData.GridHead.IsVisibleFiltroCboDescripcion = false;
                        objData.ListRowsCbo = GetDataTableToListCbo(listDataTable[2], filtro);
                    //}
                    //else
                    //{
                    //    objData.GridHead.IsVisibleFiltroCboDescripcion = false;
                    //}
                    objData.IsVisibleCheck = false;
                    obj = new
                    {
                        tabDataName = filtro,
                        tabDataNumRows = cuenta,
                        resumenTabDataList =
                            RenderViewToString(this.ControllerContext, "GridViews/ResumenGridView", objData),
                        resumenTotalPages = objData.TotalPaginasResumen,
                        tabDataList = RenderViewToString(this.ControllerContext, "GridViews/TabGridView", objData),
                        tabTotalPages = objData.TotalPaginasTab,
                        pieTitle = objData.TituloTab,
                        tabPieData = GetJsonDataPie((filtro == "Partida" ? "Nandina" : filtro), listDataTable[1], cifTot, totalCifTotTabs, specificCulture)
                    };
                }
                else
                {
                    Session["ExisteDataInfoTabla"] = false;
                }
            }

            else
            {
                var cuenta = CuentaAgrupado(filtro, tabla, dua);
                if (cuenta > 0)
                {

                    var listDataTable = CargaFiltro(filtro, -1, codPais, cifTot, pesoNeto, idioma, dua, tabla,
                        indexCboPaisB, Enums.TipoFiltro.Todos, hideTabExcel: hideTabExcel, useRand: useRand);
                    TabData objData = GetTabDataByFilter(filtro, cifTot, cuenta, pesoNeto, unidad, codPais);
                    objData.IsVisibleInfoComplementario = objData.IsVisibleInfoComplementario && _paisesConInfoComplementaria.Contains(codPais);
                    objData.TotalRegistros = totalRegistrosFormateado;
                    objData.CiFoFobTotal = cifTotFormateado;

                    objData.CodPais = codPais;
                   

                    objData.HideTabExcel = hideTabExcel;

                    if (filtro == Enums.Filtro.Marca.ToString() || filtro == Enums.Filtro.Modelo.ToString())
                    {
                        objData.ListRowsTab = GetDataTableToListGridRowTab(listDataTable[0], filtro, cifTot,
                            valueCifTot, codPais, specificCulture, pesoNeto, unidad);
                        objData.GridHead.IsVisibleFiltroCboDescripcion = false;
                        obj = new
                        {
                            tabDataName = filtro,
                            tabDataNumRows = cuenta,
                            resumenTabDataList = "",
                            resumenTotalPages = 0,
                            tabDataList = RenderViewToString(this.ControllerContext, "GridViews/TabGridView", objData),
                            tabTotalPages = objData.TotalPaginasTab,
                            pieTitle = objData.TituloTab,
                            tabPieData = (filtro == Enums.Filtro.Marca.ToString() ? GetJsonDataPie(filtro, listDataTable[0], cifTot, valueCifTot, specificCulture) : null)
                        };

                    }
                    else
                    {
                        objData.PesoNeto = pesoNetoFormateado;
                        objData.ListRows =
                            GetDataTableToListGridRow(listDataTable[0], filtro, cifTot, valueCifTot, codPais,
                                specificCulture, anioMesIni: anioMesIni, anioMesFin: anioMesFin, indexCboPaisB: indexCboPaisB, codPaisB: codPaisB);
                        objData.ListRowsTab = GetDataTableToListGridRowTab(listDataTable[1], filtro, cifTot,
                            valueCifTot, codPais, specificCulture, pesoNeto, unidad);
                        objData.HideTabExcel = hideTabExcel;
                        if (Session["dt" + filtro + "2"] != null && Convert.ToBoolean(Session["dt" + filtro + "2"]))
                        {
                            objData.GridHead.IsVisibleFiltroCboDescripcion = true;
                            objData.ListRowsCbo = GetDataTableToListCbo(listDataTable[2], filtro);
                        }
                        else
                        {
                            objData.GridHead.IsVisibleFiltroCboDescripcion = false;
                        }

                        obj = new
                        {
                            tabDataName = filtro,
                            tabDataNumRows = cuenta,
                            resumenTabDataList =
                                RenderViewToString(this.ControllerContext, "GridViews/ResumenGridView", objData),
                            resumenTotalPages = objData.TotalPaginasResumen,
                            tabDataList = RenderViewToString(this.ControllerContext, "GridViews/TabGridView", objData),
                            tabTotalPages = objData.TotalPaginasTab,
                            pieTitle = objData.TituloTab,
                            tabPieData = GetJsonDataPie((filtro == "Partida" ? "Nandina" : filtro), listDataTable[1], cifTot, valueCifTot, specificCulture)
                        };
                    }
                }
            }
            return obj;
        }

        private object GetDataObjectAduanaDua(string filtro, string codPais, string cifTot,
            string pesoNeto, string dua, string tabla,
            string valueCifTot, int indexCboPaisB, string totalRegistrosFormateado,
            string cifTotFormateado, CultureInfo specificCulture, string idioma, bool hideTabExcel = false, bool useRand = false)
        {
            object obj = new
            {
                tabDataName = filtro,
                tabDataNumRows = 0
            };
            var cuenta = CuentaAgrupado(filtro, tabla, dua);
            if (cuenta > 0)
            {
                var listDataTable = CargaFiltro(filtro, -1, codPais, cifTot, pesoNeto, idioma, dua, tabla, indexCboPaisB, Enums.TipoFiltro.Todos, hideTabExcel: hideTabExcel, useRand: useRand);
                TabData objData = GetTabDataByFilter(filtro, cifTot, cuenta, "", "");
                objData.TotalRegistros = totalRegistrosFormateado;
                objData.CiFoFobTotal = cifTotFormateado;

                objData.ListRowsTab = GetListGridRowAduanaDua(listDataTable[0], filtro, cifTot, specificCulture);

                if (Session["dt" + Enums.Filtro.Aduana.ToString() + "2"] != null
                    && Convert.ToBoolean(Session["dt" + Enums.Filtro.Aduana.ToString() + "2"]))
                {
                    objData.GridHead.IsVisibleFiltroCboDescripcion = true;
                    objData.ListRowsCbo = GetDataTableToListCbo((filtro == Enums.Filtro.Aduana.ToString() ? listDataTable[1] : listDataTable[2]), Enums.Filtro.Aduana.ToString());
                }
                else
                {
                    objData.GridHead.IsVisibleFiltroCboDescripcion = false;
                }
                objData.HideTabExcel = hideTabExcel;
                obj = new
                {
                    tabDataName = filtro,
                    tabDataNumRows = cuenta,
                    resumenTabDataList = "",
                    resumenTotalPages = 0,
                    tabDataList = RenderViewToString(this.ControllerContext, "GridViews/AduanaDuaGridView", objData),
                    tabTotalPages = objData.TotalPaginasTab,
                    pieTitle = Resources.Resources.Demo_Aduanas_Tab,
                    tabPieData = GetJsonDataPie(Enums.Filtro.Aduana.ToString(), (filtro == Enums.Filtro.Aduana.ToString() ? listDataTable[0] : listDataTable[1]), cifTot, valueCifTot, specificCulture)
                };

            }
            return obj;
        }

        private List<DataTable> CargaFiltro(string filtro, int pagina, string codPais,
            string cifTot, string pesoNeto, string idioma,
            string dua, string tabla, int indexCboPaisB,
            Enums.TipoFiltro tipoFiltro, string orden = "", string addExtraFiltro = "", bool hideTabExcel = false, bool useRand = false)
        {
            if(filtro != Enums.Filtro.InfoTabla.ToString())
            {
                string sqlFiltro = "";
                string sqlFiltroO = "";

                if (pagina == -1)
                {
                    if (filtro == "AduanaDUA")
                    {
                        sqlFiltro = GeneraSqlAgrupado("Aduana", codPais, cifTot, pesoNeto, idioma, dua, tabla, orden);
                        Session["sqlAduana"] = sqlFiltro;

                        sqlFiltroO = GeneraSqlAgrupadoOrdenado("Aduana", codPais, idioma, tabla);
                        Session["sqlAduanaO"] = sqlFiltroO;

                        //DataTable dt = null;

                        //if (indexCboPaisB == 0)
                        //{
                        //    dt = Lista2("Aduana");
                        //}
                        //Session["dtAduana2"] = dt;

                        if (indexCboPaisB == 0)
                        {
                            Session["dtAduana2"] = true;
                        }
                        else
                        {
                            Session["dtAduana2"] = false;
                        }
                    }

                    if (filtro == "Modelo")
                    {
                        sqlFiltro = GeneraSqlAgrupado("Marca", codPais, cifTot, pesoNeto, idioma, dua, tabla, orden);
                        Session["sqlMarca"] = sqlFiltro;
                    }

                    sqlFiltro = GeneraSqlAgrupado(filtro, codPais, cifTot, pesoNeto, idioma, dua, tabla, orden);
                    Session["sql" + filtro] = sqlFiltro;

                    if (filtro != "AduanaDUA")
                    {
                        if (filtro == "Marca" || filtro == "Modelo")
                        {
                            Session["dt" + filtro + "2"] = false;
                        }
                        else
                        {
                            if (!IsManifiesto(codPais))
                                sqlFiltroO = GeneraSqlAgrupadoOrdenado(filtro, codPais, idioma, tabla);
                            else
                                sqlFiltroO = GeneraSqlAgrupadoOrdenadoManif(filtro, codPais, idioma, tabla);

                            Session["sql" + filtro + "O"] = sqlFiltroO;

                            //DataTable dt2 = null;

                            //if (indexCboPaisB == 0)
                            //    dt2 = Lista2(filtro);

                            //Session["dt" + filtro + "2"] = dt2;

                            if (indexCboPaisB == 0)
                            {
                                Session["dt" + filtro + "2"] = true;
                            }
                            else
                            {
                                Session["dt" + filtro + "2"] = false;
                            }
                        }
                    }
                }
                else
                {
                    sqlFiltro = Session["sql" + filtro].ToString();
                }

                if (!string.IsNullOrEmpty(addExtraFiltro)) sqlFiltro += addExtraFiltro;
                return GetDataTables(filtro, sqlFiltro, pagina, tipoFiltro, hideTabExcel: hideTabExcel, useRand: useRand);
            }
            else
            {
                /*string sqlFiltro = Session["sqlFiltroTablas"].ToString();
                string sqlFiltroTabs = Session["sqlFiltroTablasTabs"].ToString();

                string sqlFiltroAux = sqlFiltro;
                string sqlFiltroTabsAux = sqlFiltroTabs;

                if (tipoFiltro == Enums.TipoFiltro.Resumen && orden != "")
                {
                    sqlFiltroAux = sqlFiltro.Replace("T.ciftot desc", "T."+ orden);
                }else if(tipoFiltro == Enums.TipoFiltro.Tab && orden != "")
                {
                    sqlFiltroTabsAux = sqlFiltroTabs.Replace("T.ciftot desc", "T." + orden);
                }*/

                List<DataTable> lista = new List<DataTable>();

                string sqlFiltro = GeneraSqlAgrupado(filtro, codPais, cifTot, pesoNeto, idioma, dua, tabla, orden);
                Session["sql" + filtro] = sqlFiltro;

                DataTable dt;
                try
                {
                    dt = Conexion.SqlDataTable(sqlFiltro);
                }
                catch (Exception ex)
                {
                    
                    
                    
                    ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                    dt = null;
                }

                lista.Add(dt);


                /*lista.Add(FuncionesBusiness.Lista(sqlFiltroAux, (pagina == -1 ? 1 : pagina),
                                    ResumenTabsGridPageSize));
                lista.Add(FuncionesBusiness.Lista(sqlFiltroTabsAux, (pagina == -1 ? 1 : pagina), TabGridPageSize));
                lista.Add(GenerarDatatableOrdenado(sqlFiltroTabsAux, idioma));*/

                return lista;
            }
        }

        private string CargaFiltroInfoTabla(string filtro, int pagina, string codPais,
            string cifTot, string pesoNeto, string idioma,
            string dua, string tabla, int indexCboPaisB,
            Enums.TipoFiltro tipoFiltro, string orden = "", string addExtraFiltro = "", bool hideTabExcel = false)

        {            
            {                

                string sqlFiltro = GeneraSqlAgrupado(filtro, codPais, cifTot, pesoNeto, idioma, dua, tabla, orden);
                Session["sql" + filtro] = sqlFiltro;
               
                return sqlFiltro;
            }
        }


        private DataTable GenerarDatatableOrdenado(string sqlFiltro, string idioma)
        {
            string IdVariable = "";
            string codPais = "";
            try
            {
                DataTable dt = Conexion.SqlDataTable(sqlFiltro);

                foreach (DataRow dataRow in dt.Rows)
                {
                    codPais = "'"+dataRow["IdPais"].ToString()+"'";

                    if(codPais.Length == 3 && (codPais.Substring(2) == "E" || codPais.Substring(2) == "E"))
                    {
                        codPais = codPais.Substring(0, 2) + "_";
                    }

                    IdVariable +=codPais + ",";
                }
                IdVariable += "''";

            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }

            //return dt;

            return FuncionesBusiness.BuscarPais(IdVariable,idioma);
        }

        private List<DataTable> GetDataTables(string filtro, string sqlFiltro, int pagina,
            Enums.TipoFiltro tipoFiltro, bool hideTabExcel = false, bool useRand = false)
        {
            List<DataTable> lista = new List<DataTable>();
            switch (filtro)
            {

                case "Partida":
                case "Importador":
                case "Proveedor":
                case "Exportador":
                case "ImportadorExp":
                case "ViaTransp":
                case "PaisOrigen":
                case "PaisDestino":
                case "DetalleExcel":
                case "Distrito":
                case "PaisEmbarque":
                case "PtoDestino":
                case "PtoEmbarque":
                case "PtoDescarga":
                case "Manifiesto":
                case "Notificado":
                case "MarcaEC":
                    switch (tipoFiltro)
                    {
                        case Enums.TipoFiltro.Todos:
                            DataTable data = FuncionesBusiness.Lista(sqlFiltro, (pagina == -1 ? 1 : pagina), TabGridPageSize, false, useRand);
                            if (!hideTabExcel)
                                lista.Add(data.Rows.Cast<System.Data.DataRow>().Take(ResumenTabsGridPageSize).CopyToDataTable());
                            else
                                lista.Add(new DataTable());
                            lista.Add(data);
                            if (Session["dt" + filtro + "2"] != null && Convert.ToBoolean(Session["dt" + filtro + "2"]))
                                lista.Add(FuncionesBusiness.Lista2(Session["sql" + filtro + "O"].ToString()));
                            break;
                        case Enums.TipoFiltro.Resumen:
                            lista.Add(FuncionesBusiness.Lista(sqlFiltro, (pagina == -1 ? 1 : pagina), ResumenTabsGridPageSize, false, useRand));
                            break;
                        case Enums.TipoFiltro.Tab:
                            lista.Add(FuncionesBusiness.Lista(sqlFiltro, (pagina == -1 ? 1 : pagina), TabGridPageSize, filtro == "DetalleExcel", useRand));
                            break;
                    }
                    break;
                case "Marca":
                case "Modelo":
                    lista.Add(FuncionesBusiness.Lista(sqlFiltro, (pagina == -1 ? 1 : pagina), TabGridPageSize, false, useRand));
                    break;
                case "AduanaDUA":
                    if (tipoFiltro == Enums.TipoFiltro.Todos)
                    {
                        lista.Add(FuncionesBusiness.Lista(sqlFiltro, (pagina == -1 ? 1 : pagina), TabGridPageSize, false, useRand));
                        lista.Add(FuncionesBusiness.Lista(Session["sql" + Enums.Filtro.Aduana.ToString()].ToString(), 1, TabGridPageSize, false, useRand));
                        if (Session["dt" + Enums.Filtro.Aduana.ToString() + "2"] != null
                            && Convert.ToBoolean(Session["dt" + Enums.Filtro.Aduana.ToString() + "2"]))
                        {
                            lista.Add(FuncionesBusiness.Lista2(Session["sqlAduanaO"].ToString()));
                        }
                    }
                    else
                    {
                        lista.Add(FuncionesBusiness.Lista(sqlFiltro, (pagina == -1 ? 1 : pagina), TabGridPageSize, false, useRand));
                    }
                    break;
                case "Aduana":
                    if (tipoFiltro == Enums.TipoFiltro.Todos)
                    {
                        lista.Add(FuncionesBusiness.Lista(sqlFiltro, (pagina == -1 ? 1 : pagina), TabGridPageSize, false, useRand));
                        if (Session["dt" + filtro + "2"] != null && Convert.ToBoolean(Session["dt" + filtro + "2"]))
                            lista.Add(FuncionesBusiness.Lista2(Session["sql" + filtro + "O"].ToString()));
                    }
                    else
                    {
                        lista.Add(FuncionesBusiness.Lista(sqlFiltro, (pagina == -1 ? 1 : pagina), TabGridPageSize, false, useRand));
                    }
                    break;
            }
            return lista;
        }

        private TabData GetTabDataByFilter(string filtro, string cifTot, int totalRegistrosFiltro,
            string pesoNeto, string unidad, string codPais = "")
        {
            var IsManif = IsManifiesto(codPais);
            TabData objData = new TabData();
            objData.Filtro = filtro;
            objData.AddToFiltersAndSearchButton = Resources.Resources.Add_Filter_Search_Button;

            objData.TotalPaginasResumen =
                (int)Math.Ceiling(Convert.ToDecimal(totalRegistrosFiltro) / ResumenTabsGridPageSize);
            objData.TotalPaginasTab = (int)Math.Ceiling(Convert.ToDecimal(totalRegistrosFiltro) / TabGridPageSize);

            //var lenCifTot = cifTot.Length;
            objData.GridHead.CiFoFobPor = "%";

            if (unidad != "")
            {
                objData.GridHead.TotalKg = "Total " + unidad;
                objData.GridHead.Precio = "US$ / " + unidad;
                if (unidad != "kg")
                    objData.GridHead.OrdenTotalKg = "Cantidad desc";
                else
                    objData.GridHead.OrdenTotalKg = pesoNeto + " desc";

                objData.GridHead.IsVisblePrecio = filtro != "Marca" && filtro != "Modelo" && unidad != "";
                objData.GridHead.IsVisibleTotalKg = filtro != "Marca" && filtro != "Modelo" && unidad != "";
            }
            else
            {
                objData.GridHead.IsVisblePrecio = false;
                objData.GridHead.IsVisibleTotalKg = false;
            }

            objData.GridHead.TotalReg = Resources.Resources.Grid_Column_TotalRecords;
            objData.GridHead.TitleColumnTotalReg = Resources.Resources.Grid_Column_SeeRecords;
            if (!IsManif)
                objData.GridHead.CiFoFobTot = "Total US$ " + cifTot.Substring(0, 3);
            else
            {
                objData.GridHead.CiFoFobTot = cifTot + " Tn";
                if (GetCurrentIdioma() == "en")
                {
                    objData.GridHead.CiFoFobTot = "Gross Weight" + " Tn";
                }
            }
                

            objData.GridHead.OrdenCiFoFobTot = cifTot;
            switch (filtro)
            {
                case "Partida":
                    objData.GridHead.Descripcion = Resources.Resources.Demo_Products_Tab;
                    objData.AddMyFavouritesButton = Resources.AdminResources.Add_ToMyProducts_Button;
                    objData.TituloTab = Resources.Resources.Demo_Products_Tab;
                    break;
                case "Importador":
                case "ImportadorExp":
                    objData.GridHead.mostrarInforColombia = codPais == "CO";
                    //objData.GridHead.mostrarInforColombia = false;
                    objData.GridHead.Descripcion = !IsManif ?Resources.Resources.Demo_Importers_Tab: Resources.Resources.Demo_Importers_Tab_Manif;
                    objData.AddMyFavouritesButton = Resources.AdminResources.Add_ToMyImporters_Button;
                    objData.TituloTab = objData.GridHead.Descripcion; //Resources.Resources.Demo_Importers_Tab;
                    break;
                case "Manifiesto":
                case "Notificado":
                    objData.GridHead.Descripcion = filtro=="Notificado" ?  Resources.Resources.Demo_Notif_Tab :Resources.Resources.Demo_Manifiesto_Tab ;
                    objData.TituloTab = objData.GridHead.Descripcion; //Resources.Resources.Demo_Importers_Tab;
                    break;
                case "Proveedor":
                    if (codPais != "CL" )
                    {
                        objData.GridHead.Descripcion = !IsManif ? Resources.Resources.Demo_Exporters_Tab : Resources.Resources.Demo_Exporters_Tab_Manif;
                        objData.AddMyFavouritesButton = Resources.AdminResources.Add_ToMyExporters_Button;
                        objData.TituloTab = objData.GridHead.Descripcion; //Resources.Resources.Demo_Exporters_Tab;
                    }
                    else
                    {
                        objData.GridHead.Descripcion = Resources.Resources.Demo_Brands_Tab;
                        objData.AddMyFavouritesButton = Resources.AdminResources.Add_ToMyBrands_Button;
                        objData.TituloTab = Resources.Resources.Demo_Brands_Tab;
                    }
                    break;
                case "Exportador":
                    objData.GridHead.Descripcion = !IsManif ? Resources.Resources.Demo_Exporters_Tab: Resources.Resources.Demo_Exporters_Tab_Manif;
                    objData.AddMyFavouritesButton = Resources.AdminResources.Add_ToMyExporters_Button;
                    objData.TituloTab = objData.GridHead.Descripcion; //Resources.Resources.Demo_Exporters_Tab;
                    break;
                case "PaisEmbarque":
                case "PaisOrigen":
                    objData.GridHead.Descripcion = !IsManif ? Resources.Resources.Demo_OriginCountries_Tab: Resources.Resources.Ult_Paises_Embarque;
                    objData.TituloTab = objData.GridHead.Descripcion; //Resources.Resources.Demo_OriginCountries_Tab;
                    break;
                case "PaisDestino":
                    objData.GridHead.Descripcion = Resources.Resources.Demo_DestinationCountries_Tab;
                    objData.TituloTab = Resources.Resources.Demo_DestinationCountries_Tab;
                    break;
                case "ViaTransp":
                    objData.GridHead.Descripcion = Resources.Resources.Demo_Vias_Tab;
                    objData.TituloTab = Resources.Resources.Demo_Vias_Tab;
                    break;
                case "Marca":
                    objData.GridHead.Descripcion = Resources.Resources.Demo_Brands_Tab;
                    objData.GridHead.CiFoFobTot = "Total US$ FOB";
                    objData.GridHead.OrdenCiFoFobTot = "FOBTot";
                    break;
                case "MarcaEC":
                    objData.GridHead.Descripcion = objData.TituloTab = Resources.Resources.Demo_Brands_Tab;
                    objData.GridHead.CiFoFobTot = "Total US$ FOB";
                    objData.GridHead.OrdenCiFoFobTot = "FOBTot";
                    break;
                case "Modelo":
                    objData.GridHead.Descripcion = Resources.Resources.Demo_Models_Tab;
                    objData.GridHead.CiFoFobTot = "Total US$ FOB";
                    objData.GridHead.OrdenCiFoFobTot = "FOBTot";
                    break;
                case "AduanaDUA":
                    objData.GridHead.Descripcion = Resources.Resources.Grid_ColumnHeader_Customs;
                    objData.GridHead.Duas = Resources.Resources.Grid_ColumnHeader_Duas;
                    objData.TituloTab = Resources.Resources.Demo_CustomsDuas_Tab;
                    break;
                case "Aduana":
                    objData.GridHead.Descripcion = Resources.Resources.Grid_ColumnHeader_Customs;
                    objData.TituloTab = Resources.Resources.Grid_ColumnHeader_Customs;
                    objData.GridHead.IsVisibleDuas = false;
                    break;
                case "Distrito":
                    objData.GridHead.Descripcion = Resources.Resources.Demo_Districts_Tab;
                    objData.TituloTab = Resources.Resources.Demo_Districts_Tab;
                    break;
                case "PtoDestino":
                case "PtoEmbarque":
                case "PtoDescarga":
                    objData.GridHead.Descripcion = filtro == "PtoDestino" ?  Resources.Resources.Demo_PtoDestino_Tab:
                                                   filtro == "PtoEmbarque" ? Resources.Resources.Demo_PtoEmbarque_Tab:
                                                          Resources.Resources.Demo_PtoDescarga_Tab;
                    objData.TituloTab = objData.GridHead.Descripcion; //Resources.Resources.Demo_Vias_Tab;
                    break;
                case "InfoTabla":
                    objData.GridHead.Descripcion = "XXX";
                    objData.TituloTab = "XXX";
                    break;
            }

            return objData;
        }

        private int CuentaAgrupado(string filtro, string tabla, string dua, string addExtraFiltro = "")

        {
            string sql = "";
            if (filtro == "Partida")
                sql = "select count(*) as Cant from (select distinct IdPartida ";
            else if (filtro == "Marca" || filtro == "MarcaEC")
                sql = "select count(*) as Cant from (select distinct IdMarca ";
            else if (filtro == "Modelo")
                sql = "select count(*) as Cant from (select distinct IdModelo ";

            else if (filtro == "Importador")
                sql = "select count(*) as Cant from (select distinct IdImportador ";
            else if (filtro == "Notificado") //
                sql = "select count(*) as Cant from (select distinct IdNotificado ";
            else if (filtro == "Proveedor")
                sql = "select count(*) as Cant from (select distinct IdProveedor ";
            else if (filtro == "Exportador")
                sql = "select count(*) as Cant from (select distinct IdExportador ";
            else if (filtro == "ImportadorExp")
                sql = "select count(*) as Cant from (select distinct IdImportadorExp ";
            else if (filtro == "PaisEmbarque")
                sql = "select count(*) as Cant from (select distinct IdPaisEmbarque ";
            else if (filtro == "PaisOrigen")
                sql = "select count(*) as Cant from (select distinct IdPaisOrigen ";
            else if (filtro == "PtoEmbarque" || filtro == "PtoDestino" || filtro == "PtoDescarga")
                sql = "select count(*) as Cant from (select distinct Id" + filtro + " ";
            else if (filtro == "Manifiesto")
                sql = "select count(*) as Cant from (select distinct Manifiesto ";

            else if (filtro == "PaisDestino")
                sql = "select count(*) as Cant from (select distinct IdPaisDestino ";
            else if (filtro == "ViaTransp")
                sql = "select count(*) as Cant from (select distinct IdViaTransp ";
            else if (filtro == "AduanaDUA")
                sql = "select count(*) as Cant from (select distinct IdAduana, " + dua + " ";
            else if (filtro == "Aduana")
                sql = "select count(*) as Cant from (select distinct IdAduana ";
            else if (filtro == "Distrito")
                sql = "select count(*) as Cant from (select distinct IdDistrito ";
            else if (filtro == "DetalleExcel")
                sql = "select count(*) as Cant from (select 1 A ";

            string tabla1 = tabla;
            if (filtro == "Marca" || filtro == "Modelo") tabla1 = "Importacion_PEB";

            sql += "from " + tabla1 + " T where 1 = 1 ";
            sql += Session["SqlFiltro"].ToString();

            if (!string.IsNullOrEmpty(addExtraFiltro)) sql += addExtraFiltro;

            sql += ") T ";




            return FuncionesBusiness.CuentaRegistros(sql);
        }

        private int CuentaAgrupadoInfoComplementario()
        {

            string sql = "SELECT COUNT(*) AS CANT FROM ("+Session["SqlFiltroTablas"].ToString();
            sql += ") TA";

            return FuncionesBusiness.CuentaRegistros(sql);
        }

        DataTable Lista2(string filtro)
        {
            DataTable dt = FuncionesBusiness.Lista2(Session["sql" + filtro + "O"].ToString());

            DataRow dr = dt.NewRow();

            if (filtro == "Partida")
            {
                dr[0] = 0;
                dr[1] = "";
                dr[2] = Resources.Resources.Option_Filter_All.ToUpper();
            }
            else
            {
                dr[0] = 0;
                dr[1] = Resources.Resources.Option_Filter_All.ToUpper();
            }

            dt.Rows.InsertAt(dr, 0);

            return dt;
        }

        string GeneraSqlAgrupado(string filtro, string codPais, string cifTot,
            string pesoNeto, string idioma, string dua,
            string tabla, string orden = "",bool isInfoTabla = false)
        {
            bool isManif = IsManifiesto(codPais);
            string sql = "";
            bool isDesarrollo = true;/*Properties.Settings.Default.TableVarGeneral_InDev; */
            if (!string.IsNullOrWhiteSpace(Session["hdfPalabrasY"]?.ToString()) && !isManif )
            {
                return GeneraSqlAgrupadoDesComercial(filtro, codPais, cifTot, pesoNeto,
                    idioma, dua, tabla,
                    orden);
            }


            if (orden == "")
            {
                orden = cifTot + " desc";

                if (filtro == "Marca" || filtro == "Modelo")
                    orden = "FOBTot desc";
            }

            string cifTot1 = cifTot;
            if (codPais == "BR" || codPais == "IN")
                cifTot1 = "convert(decimal(19,2), " + cifTot + ")";

            string pesoNeto1 = pesoNeto;
            string pesoNeto2 = pesoNeto;

            if (pesoNeto1 == "")
            {
                pesoNeto1 = "0";
                pesoNeto2 = "PesoNeto";
            }

            if (filtro == "Partida")

            {
                string partida = "Partida";
                if (idioma == "en")
                    partida = "Partida_en";

                sql = "select P.IdPartida, P.Nandina, P.Nandina + ' ' + P." + partida + " as Partida, CantReg, " +
                      cifTot + ", " + pesoNeto2 + ", Cantidad, ROW_NUMBER() over (order by " + orden + ") as Nro ";
                sql += "from Partida_" + codPais + " P, (select IdPartida, count(*) as CantReg, sum(" + cifTot1 +
                       ") as " + cifTot + ", sum(" + pesoNeto1 + ") as " + pesoNeto2 + ", sum(Cantidad) as Cantidad ";
            }
            else if (filtro == "Marca")
            {
                sql = "select T.IdMarca, Marca, CantReg, FOBTot, PesoNeto, Cantidad, ROW_NUMBER() over (order by " +
                      orden + ") as Nro ";
                sql +=
                    "from Marca_PEB M, (select IdMarca, count(*) as CantReg, sum(FOBTot) as FOBTot, sum(0) as PesoNeto, sum(Cantidad) as Cantidad ";
            }
            else if (filtro == "InfoTabla")
            {
                string pais = FuncionesBusiness.BuscarPaisNombre(codPais, idioma);
                sql = "select '" + codPais + "' as CodPais,'" + pais.ToUpper() + "' as Pais , count(*) as cantidad,sum(" + cifTot + ") as ciftot, SUM(" + pesoNeto1 + ") AS pesoNeto ";
            }
            else if (filtro == "MarcaEC")
            {
                sql = "select M.idMarca, Marca, CantReg, " + cifTot + ", " + pesoNeto2 +
                      ", Cantidad, ROW_NUMBER() over (order by " + orden + ") as Nro ";
                sql += "from Marca_" + codPais + " M, (select idMarca, count(*) as CantReg, sum(" + cifTot1 +
                       ") as " + cifTot + ", sum(" + pesoNeto1 + ") as " + pesoNeto2 + ", sum(Cantidad) as Cantidad ";
            }
            else if (filtro == "Modelo")

            {
                sql =
                    "select T.IdModelo, Marca + ' - ' + Modelo as Modelo, CantReg, FOBTot, PesoNeto, Cantidad, ROW_NUMBER() over (order by " +
                    orden + ") as Nro ";
                sql +=
                    "from Modelo_PEB M, (select IdModelo, count(*) as CantReg, sum(FOBTot) as FOBTot, sum(0) as PesoNeto, sum(Cantidad) as Cantidad ";
            }
            else if (filtro == "Importador")
            {
                if (!isManif)
                {
                    //sql = "select IdImportador, Empresa as Importador, CantReg, " + cifTot + ", " + pesoNeto2 +
                    //      ", Cantidad, ROW_NUMBER() over (order by " + orden + ") as Nro ";

                    sql = "select IdImportador, Empresa as Importador, CantReg, " + cifTot + ", " + pesoNeto2 +
                          ", Cantidad, ROW_NUMBER() over (order by " + (orden == filtro ? "Empresa" : orden) + ") as Nro ";

                    sql += "from Empresa_" + codPais + " E, (select IdImportador, count(*) as CantReg, sum(" + cifTot1 +
                           ") as " + cifTot + ", sum(" + pesoNeto1 + ") as " + pesoNeto2 + ", sum(Cantidad) as Cantidad ";
                }
                else
                {
                    sql = "select IdImportador, Empresa as Importador, CantReg, " + cifTot +
                          ", ROW_NUMBER() over (order by " + orden + ") as Nro ";
                    sql += "from Empresa_" + codPais + " E, (select IdImportador, count(*) as CantReg, sum(" + cifTot +
                           ") / 1000 as " + cifTot + " ";
                }
            }
            else if (filtro == "Proveedor")
            {
                if (!isManif)
                {
                    sql = "select T.IdProveedor, Proveedor, CantReg, " + cifTot + ", " + pesoNeto2 +
                          ", Cantidad, ROW_NUMBER() over (order by " + orden + ") as Nro ";

                    sql += "from Proveedor_" + codPais + " P, (select IdProveedor, count(*) as CantReg, sum(" + cifTot1 +
                           ") as " + cifTot + ", sum(" + pesoNeto1 + ") as " + pesoNeto2 + ", sum(Cantidad) as Cantidad ";
                }
                else
                {
                    sql = "select T.IdProveedor, Proveedor, CantReg, " + cifTot + ", ROW_NUMBER() over (order by " + orden +
                          ") as Nro ";
                    sql += "from Proveedor_" + codPais + " P, (select IdProveedor, count(*) as CantReg, sum(" + cifTot +
                           ") / 1000 as " + cifTot + " ";
                }
            }
            else if (filtro == "Exportador")
            {
                if (!isManif)
                {
                    sql = "select IdExportador, Empresa as Exportador, CantReg, " + cifTot + ", " + pesoNeto2 +
                          ", Cantidad, ROW_NUMBER() over (order by " + orden + ") as Nro ";
                    sql += "from Empresa_" + codPais + " E, (select IdExportador, count(*) as CantReg, sum(" + cifTot1 +
                           ") as " + cifTot + ", sum(" + pesoNeto1 + ") as " + pesoNeto2 + ", sum(Cantidad) as Cantidad ";
                }
                else
                {
                    sql = "select IdExportador, Empresa as Exportador, CantReg, " + cifTot +
                          ", ROW_NUMBER() over (order by " + orden + ") as Nro ";
                    sql += "from Empresa_" + codPais + " E, (select IdExportador, count(*) as CantReg, sum(" + cifTot +
                           ") / 1000 as " + cifTot + " ";
                }
            }
            else if (filtro == "ImportadorExp")
            {
                if (!isManif)
                {
                    sql = "select T.IdImportadorExp, ImportadorExp, CantReg, " + cifTot + ", " + pesoNeto2 +
                          ", Cantidad, ROW_NUMBER() over (order by " + orden + ") as Nro ";
                    sql += "from ImportadorExp_" + codPais + " I, (select IdImportadorExp, count(*) as CantReg, sum(" +
                           cifTot1 + ") as " + cifTot + ", sum(" + pesoNeto1 + ") as " + pesoNeto2 +
                           ", sum(Cantidad) as Cantidad ";
                }
                else
                {
                    sql = "select T.IdImportadorExp, ImportadorExp, CantReg, " + cifTot + ", ROW_NUMBER() over (order by " +
                          orden + ") as Nro ";
                    sql += "from ImportadorExp_" + codPais + " I, (select IdImportadorExp, count(*) as CantReg, sum(" +
                           cifTot + ") / 1000 as " + cifTot + " ";
                }
            }
            else if (filtro == "PaisOrigen")
            {
                if (orden == "PaisOrigen")
                {
                    orden = "Pais";
                }

                if (isDesarrollo)
                {
                    sql = "select IdPaisOrigen, Pais as PaisOrigen, CantReg, " + cifTot + ", " + pesoNeto2 +
                     ", Cantidad,Flag, ROW_NUMBER() over (order by " + orden + ") as Nro ";
                    sql += "from Pais_" + codPais + " P, (select IdPaisOrigen, count(*) as CantReg, sum(" + cifTot1 +
                           ") as " + cifTot + ", sum(" + pesoNeto1 + ") as " + pesoNeto2 + ", sum(Cantidad) as Cantidad, ISNULL(PC.COD_PAIS_COMPLEMENTARIO,'') as Flag ";
                }
                else
                {
                    sql = "select IdPaisOrigen, Pais as PaisOrigen, CantReg, " + cifTot + ", " + pesoNeto2 +
                     ", Cantidad,Flag, ROW_NUMBER() over (order by " + orden + ") as Nro ";
                    sql += "from Pais_" + codPais + " P, (select IdPaisOrigen, count(*) as CantReg, sum(" + cifTot1 +
                           ") as " + cifTot + ", sum(" + pesoNeto1 + ") as " + pesoNeto2 + ", sum(Cantidad) as Cantidad, '' as Flag ";
                }
               
            }
            else if (filtro == "PaisEmbarque")
            {
                sql = "select IdPaisEmbarque, Pais as PaisEmbarque, CantReg, " + cifTot +
                      ", ROW_NUMBER() over (order by " + orden + ") as Nro ";
                sql += "from Pais_" + codPais + " P, (select IdPaisEmbarque, count(*) as CantReg, sum(" + cifTot +
                       ") / 1000 as " + cifTot + " ";
            }
            else if (filtro == "PaisDestino")
            {
                if (orden == "PaisDestino")
                {
                    orden = "Pais";
                }
                if (!isManif)
                {
                    if (isDesarrollo)
                    {

                        sql = "select IdPaisDestino, Pais as PaisDestino, CantReg, " + cifTot + ", " + pesoNeto2 +
                          ", Cantidad,Flag, ROW_NUMBER() over (order by " + orden + ") as Nro ";
                        sql += "from Pais_" + codPais + " P, (select IdPaisDestino, count(*) as CantReg, sum(" + cifTot1 +
                               ") as " + cifTot + ", sum(" + pesoNeto1 + ") as " + pesoNeto2 + ", sum(Cantidad) as Cantidad, ISNULL(PC.COD_PAIS_COMPLEMENTARIO,'') as Flag ";
                    }
                    else
                    {

                        sql = "select IdPaisDestino, Pais as PaisDestino, CantReg, " + cifTot + ", " + pesoNeto2 +
                          ", Cantidad,Flag, ROW_NUMBER() over (order by " + orden + ") as Nro ";
                        sql += "from Pais_" + codPais + " P, (select IdPaisDestino, count(*) as CantReg, sum(" + cifTot1 +
                               ") as " + cifTot + ", sum(" + pesoNeto1 + ") as " + pesoNeto2 + ", sum(Cantidad) as Cantidad, '' as Flag ";
                    }
                        
                }
                else
                {
                    sql = "select IdPaisDestino, Pais as PaisDestino, CantReg, " + cifTot +
                          ", ROW_NUMBER() over (order by " + orden + ") as Nro ";
                    sql += "from Pais_" + codPais + " P, (select IdPaisDestino, count(*) as CantReg, sum(" + cifTot +
                           ") / 1000 as " + cifTot + " ";
                }
                
            }
            else if (filtro == "ViaTransp")
            {
                sql = "select T.IdViaTransp, ViaTransp, CantReg, " + cifTot + ", " + pesoNeto2 +
                      ", Cantidad, ROW_NUMBER() over (order by " + orden + ") as Nro ";
                sql += "from ViaTransp_" + codPais + " V, (select IdViaTransp, count(*) as CantReg, sum(" + cifTot1 +
                       ") as " + cifTot + ", sum(" + pesoNeto1 + ") as " + pesoNeto2 + ", sum(Cantidad) as Cantidad ";
            }
            else if (filtro == "AduanaDUA")
            {
                sql =
                    "select T.IdAduana, Aduana, DUA, convert(varchar(3), T.IdAduana) + '-' + convert(varchar(20), DUA) as IdAduanaDUA,  Aduana + ' ' + convert(varchar(20), DUA) as AduanaDUA, CantReg, " +
                    cifTot + ", " + pesoNeto2 + ", Cantidad, ROW_NUMBER() over (order by " + orden + ") as Nro ";
                sql += "from Aduana_" + codPais + " A, (select IdAduana, " + dua + " as DUA, ";
                sql += "count(*) as CantReg, sum(" + cifTot1 + ") as " + cifTot + ", sum(" + pesoNeto1 + ") as " +
                       pesoNeto2 + ", sum(Cantidad) as Cantidad ";
            }
            else if (filtro == "Aduana")
            {
                sql = "select T.IdAduana, Aduana, CantReg, " + cifTot + ", " + pesoNeto2 +
                      ", Cantidad, ROW_NUMBER() over (order by " + orden + ") as Nro ";
                sql += "from Aduana_" + codPais + " A, (select IdAduana, count(*) as CantReg, sum(" + cifTot1 +
                       ") as " + cifTot + ", sum(" + pesoNeto1 + ") as " + pesoNeto2 + ", sum(Cantidad) as Cantidad ";
            }
            else if (filtro == "Distrito")
            {
                sql = "select T.IdDistrito, Distrito, CantReg, " + cifTot + ", " + pesoNeto2 +
                      ", Cantidad, ROW_NUMBER() over (order by " + orden + ") as Nro ";
                sql += "from Distrito_" + codPais + " D, (select IdDistrito, count(*) as CantReg, sum(" + cifTot1 +
                       ") as " + cifTot + ", sum(" + pesoNeto1 + ") as " + pesoNeto2 + ", sum(Cantidad) as Cantidad ";
            }
            else if (filtro == "DetalleExcel")
            {
                orden = " FechaNum Desc ";
                sql = "select *, ROW_NUMBER() over (order by " + orden + ") as Nro ";
            }
            else if (filtro == "PtoEmbarque" || filtro == "PtoDestino" || filtro == "PtoDescarga")
            {
                sql = "select T.Id" + filtro + ", Puerto as " + filtro + ", CantReg, " + cifTot +
                      ", ROW_NUMBER() over (order by " + orden + ") as Nro ";
                sql += "from Puerto_" + codPais + " P, (select Id" + filtro + ", count(*) as CantReg, sum(" + cifTot +
                       ") / 1000 as " + cifTot + " ";
            }
            else if (filtro == "Manifiesto")
            {
                sql = "select Manifiesto, CantReg, " + cifTot + ", ROW_NUMBER() over (order by " + orden + ") as Nro ";
                sql += "from (select Manifiesto, count(*) as CantReg, sum(" + cifTot + ") / 1000 as " + cifTot + " ";
            }
            else if (filtro == "Notificado")
            {
                sql = "select T.IdNotificado, Notificado, CantReg, " + cifTot + ", ROW_NUMBER() over (order by " +
                      orden + ") as Nro ";
                sql += "from Notificado_" + codPais + " N, (select IdNotificado, count(*) as CantReg, sum(" + cifTot +
                       ") / 1000 as " + cifTot + " ";
            }


            string tabla1 = tabla;
            if (filtro == "Marca" || filtro == "Modelo")
                tabla1 = "Importacion_PEB";

            sql += "from " + tabla1 + " T where 1 = 1 ";

            if (filtro == "PaisOrigen" && isDesarrollo)
            {
                sql = sql.Replace("T where", "T left join paises_complementarios PC on T.IdPaisOrigen = PC.ID_PAIS_COMPLEMENTARIO and PC.ID_PAIS_ORIGEN = '" + codPais + "' where");
                    
            }else if (filtro == "PaisDestino" && isDesarrollo)
            {
                sql = sql.Replace("T where", "T left join paises_complementarios PC on T.IdPaisDestino = PC.ID_PAIS_COMPLEMENTARIO and PC.ID_PAIS_ORIGEN = '" + codPais + "' where");
            }

            if (isInfoTabla)
            {
                sql += Session["SqlInfoComplementario" + codPais].ToString();
            }
            else
            {
                sql += Session["SqlFiltro"].ToString();
            }
            

            if (filtro == "Partida")
                sql += "group by IdPartida) T where P.IdPartida = T.IdPartida ";
            else if (filtro == "Marca" || filtro == "MarcaEC")
                sql += "group by IdMarca) T where M.IdMarca = T.IdMarca ";

            else if (filtro == "Modelo")
                sql += "group by IdModelo) T where M.IdModelo = T.IdModelo ";
            else if (filtro == "Importador")
                sql += "group by IdImportador) T where E.IdEmpresa = T.IdImportador ";
            else if (filtro == "Notificado")
                sql += "group by IdNotificado) T where N.IdNotificado = T.IdNotificado ";
            else if (filtro == "Proveedor")
                sql += "group by IdProveedor) T where P.IdProveedor = T.IdProveedor ";
            else if (filtro == "Exportador")
                sql += "group by IdExportador) T where E.IdEmpresa = T.IdExportador ";
            else if (filtro == "ImportadorExp")
                sql += "group by IdImportadorExp) T where I.IdImportadorExp = T.IdImportadorExp ";
            else if (filtro == "PaisOrigen")
            {
                if (isDesarrollo)
                {
                    sql += "group by IdPaisOrigen, PC.COD_PAIS_COMPLEMENTARIO) T where P.IdPais = T.IdPaisOrigen ";
                }
                else
                {
                    sql += "group by IdPaisOrigen) T where P.IdPais = T.IdPaisOrigen ";
                }
            }                
            else if (filtro == "PaisEmbarque")
                sql += "group by IdPaisEmbarque) T where P.IdPais = T.IdPaisEmbarque ";
            else if (filtro == "PaisDestino")
            {
                if (isDesarrollo)
                {
                    sql += "group by IdPaisDestino, PC.COD_PAIS_COMPLEMENTARIO) T where P.IdPais = T.IdPaisDestino ";
                }
                else
                {

                    sql += "group by IdPaisDestino) T where P.IdPais = T.IdPaisDestino ";
                }
            }
            else if (filtro == "ViaTransp")
                sql += "group by IdViaTransp) T where V.IdViaTransp = T.IdViaTransp ";
            else if (filtro == "AduanaDUA")
                sql += "group by IdAduana, " + dua + ") T where A.IdAduana = T.IdAduana ";
            else if (filtro == "Aduana")
                sql += "group by IdAduana) T where A.IdAduana = T.IdAduana ";
            else if (filtro == "Distrito")
                sql += "group by IdDistrito) T where D.IdDistrito = T.IdDistrito ";
            else if (filtro == "PtoEmbarque" || filtro == "PtoDestino" || filtro == "PtoDescarga")
                sql += "group by Id" + filtro + ") T where P.IdPto = T.Id" + filtro;
            else if (filtro == "Manifiesto")
                sql += "group by Manifiesto) T ";

            return sql;
        }

        string GeneraSqlAgrupadoDesComercial(string filtro, string codPais, string cifTot,
            string pesoNeto, string idioma, string dua,
            string tabla, string orden = "")

        {
            string sql = "";
            bool isDesarrollo = true;/*Properties.Settings.Default.TableVarGeneral_InDev;*/
            if (orden == "")
            {
                orden = cifTot + " desc";

                if (filtro == "Marca" || filtro == "Modelo")
                    orden = "FOBTot desc";
            }

            string cifTot1 = cifTot;

            if (codPais == "BR" || codPais == "IN")

                cifTot1 = "convert(decimal(19,2), " + cifTot + ")";


            string pesoNeto1 = pesoNeto;
            string pesoNeto2 = pesoNeto;

            if (pesoNeto1 == "")
            {
                pesoNeto1 = "0";
                pesoNeto2 = "PesoNeto";
            }


            if (filtro == "Partida")
            {
                string partida = "Partida";


                if (idioma == "en")
                    partida = "Partida_en";

                sql = "select T.IdPartida, P.Nandina, P.Nandina + ' ' + P." + partida + " as Partida, CantReg, " +
                      cifTot + ", " + pesoNeto2 + ", Cantidad, ROW_NUMBER() over (order by " + orden + ") as Nro ";
                sql += "from Partida_" + codPais + " P, (select IdPartida, count(*) as CantReg, sum(" + cifTot1 +
                       ") as " + cifTot + ", sum(" + pesoNeto1 + ") as " + pesoNeto2 + ", sum(Cantidad) as Cantidad ";
            }
            else if (filtro == "Marca")
            {
                // Ruben 2017-06-25
                sql = "select IdMarca, Marca, CantReg, FOBTot, PesoNeto, Cantidad, ROW_NUMBER() over (order by " +
                      orden + ") as Nro ";
                sql +=
                    "from (select IdMarca, Marca, count(*) as CantReg, sum(FOBTot) as FOBTot, sum(0) as PesoNeto, sum(Cantidad) as Cantidad ";
            }
            else if (filtro == "InfoTabla")
            {
                // Ruben 2017-06-25
                string pais = FuncionesBusiness.BuscarPaisNombre(codPais,idioma);
                sql = "select '"+codPais+"' as CodPais,'"+pais+ "' as Pais , count(*) as cantidad,sum(" + cifTot + ") as ciftot, SUM("+pesoNeto1+") AS pesoNeto ";
            }

            else if (filtro == "Modelo")
            {
                // Ruben 2017-06-25
                sql = "select IdModelo, Modelo, CantReg, FOBTot, PesoNeto, Cantidad, ROW_NUMBER() over (order by " +
                      orden + ") as Nro ";

                sql +=
                    "from (select IdModelo, Marca + ' - ' + Modelo as Modelo, count(*) as CantReg, sum(FOBTot) as FOBTot, sum(0) as PesoNeto, sum(Cantidad) as Cantidad ";
            }
            else if (filtro == "Importador")
            {
                sql = "select IdImportador, Importador, CantReg, " + cifTot + ", " + pesoNeto2 +
                      ", Cantidad, ROW_NUMBER() over (order by " + orden + ") as Nro ";

                sql += "from (select IdImportador, Importador, count(*) as CantReg, sum(" + cifTot1 + ") as " + cifTot +
                       ", sum(" + pesoNeto1 + ") as " + pesoNeto2 + ", sum(Cantidad) as Cantidad ";
            }
            else if (filtro == "Proveedor")
            {
                sql = "select IdProveedor, Proveedor, CantReg, " + cifTot + ", " + pesoNeto2 +
                      ", Cantidad, ROW_NUMBER() over (order by " + orden + ") as Nro ";

                sql += "from (select IdProveedor, Proveedor, count(*) as CantReg, sum(" + cifTot1 + ") as " + cifTot +
                       ", sum(" + pesoNeto1 + ") as " + pesoNeto2 + ", sum(Cantidad) as Cantidad ";
            }
            else if (filtro == "Exportador")

            {
                sql = "select IdExportador, Exportador, CantReg, " + cifTot + ", " + pesoNeto2 +
                      ", Cantidad, ROW_NUMBER() over (order by " + orden + ") as Nro ";

                sql += "from (select IdExportador, Exportador, count(*) as CantReg, sum(" + cifTot1 + ") as " + cifTot +
                       ", sum(" + pesoNeto1 + ") as " + pesoNeto2 + ", sum(Cantidad) as Cantidad ";
            }
            else if (filtro == "ImportadorExp")
            {
                sql = "select IdImportadorExp, ImportadorExp, CantReg, " + cifTot + ", " + pesoNeto2 +
                      ", Cantidad, ROW_NUMBER() over (order by " + orden + ") as Nro ";

                sql += "from (select IdImportadorExp, ImportadorExp, count(*) as CantReg, sum(" + cifTot1 + ") as " +
                       cifTot + ", sum(" + pesoNeto1 + ") as " + pesoNeto2 + ", sum(Cantidad) as Cantidad ";
            }
            else if (filtro == "PaisOrigen")
            {
                if (isDesarrollo)
                {
                    sql = "select IdPaisOrigen, PaisOrigen, CantReg, " + cifTot + ", " + pesoNeto2 +
                      ", Cantidad,Flag, ROW_NUMBER() over (order by " + orden + ") as Nro ";
                    sql += "from (select IdPaisOrigen, PaisOrigen, count(*) as CantReg, sum(" + cifTot1 + ") as " + cifTot +
                           ", sum(" + pesoNeto1 + ") as " + pesoNeto2 + ", sum(Cantidad) as Cantidad, ISNULL(PC.COD_PAIS_COMPLEMENTARIO,'') as Flag ";
                }
                else
                {
                    sql = "select IdPaisOrigen, PaisOrigen, CantReg, " + cifTot + ", " + pesoNeto2 +
                      ", Cantidad,Flag, ROW_NUMBER() over (order by " + orden + ") as Nro ";
                    sql += "from (select IdPaisOrigen, PaisOrigen, count(*) as CantReg, sum(" + cifTot1 + ") as " + cifTot +
                           ", sum(" + pesoNeto1 + ") as " + pesoNeto2 + ", sum(Cantidad) as Cantidad, '' as Flag ";
                }
                
            }
            else if (filtro == "PaisDestino")
            {
                if (isDesarrollo)
                {
                    sql = "select IdPaisDestino, PaisDestino, CantReg, " + cifTot + ", " + pesoNeto2 +
                      ", Cantidad,Flag, ROW_NUMBER() over (order by " + orden + ") as Nro ";
                    sql += "from (select IdPaisDestino, PaisDestino, count(*) as CantReg, sum(" + cifTot1 + ") as " +
                           cifTot + ", sum(" + pesoNeto1 + ") as " + pesoNeto2 + ", sum(Cantidad) as Cantidad,ISNULL(PC.COD_PAIS_COMPLEMENTARIO, '') AS Flag ";
                }
                else
                {
                    sql = "select IdPaisDestino, PaisDestino, CantReg, " + cifTot + ", " + pesoNeto2 +
                      ", Cantidad,Flag, ROW_NUMBER() over (order by " + orden + ") as Nro ";
                    sql += "from (select IdPaisDestino, PaisDestino, count(*) as CantReg, sum(" + cifTot1 + ") as " +
                           cifTot + ", sum(" + pesoNeto1 + ") as " + pesoNeto2 + ", sum(Cantidad) as Cantidad, '' AS Flag ";
                }
                    
            }
            else if (filtro == "MarcaEC")
            {
                sql = "select IdMarca, Marca, CantReg, " + cifTot + ", " + pesoNeto2 +
                      ", Cantidad, ROW_NUMBER() over (order by " + orden + ") as Nro ";
                sql += "from (select IdMarca, Marca, count(*) as CantReg, sum(" + cifTot1 + ") as " + cifTot +
                       ", sum(" + pesoNeto1 + ") as " + pesoNeto2 + ", sum(Cantidad) as Cantidad ";
            }
            else if (filtro == "ViaTransp")
            {
                sql = "select IdViaTransp, ViaTransp, CantReg, " + cifTot + ", " + pesoNeto2 +
                      ", Cantidad, ROW_NUMBER() over (order by " + orden + ") as Nro ";
                sql += "from (select IdViaTransp, ViaTransp, count(*) as CantReg, sum(" + cifTot1 + ") as " + cifTot +
                       ", sum(" + pesoNeto1 + ") as " + pesoNeto2 + ", sum(Cantidad) as Cantidad ";
            }
            else if (filtro == "AduanaDUA")

            {
                sql =
                    "select IdAduana, Aduana, DUA, convert(varchar(3), T.IdAduana) + '-' + convert(varchar(20), DUA) as IdAduanaDUA,  Aduana + ' ' + convert(varchar(20), DUA) as AduanaDUA, CantReg, " +
                    cifTot + ", " + pesoNeto2 + ", Cantidad, ROW_NUMBER() over (order by " + orden + ") as Nro ";

                sql += "from (select IdAduana, Aduana, " + dua + " as DUA, ";
                sql += "count(*) as CantReg, sum(" + cifTot1 + ") as " + cifTot + ", sum(" + pesoNeto1 + ") as " +
                       pesoNeto2 + ", sum(Cantidad) as Cantidad ";
            }
            else if (filtro == "Aduana")
            {
                sql = "select IdAduana, Aduana, CantReg, " + cifTot + ", " + pesoNeto2 +
                      ", Cantidad, ROW_NUMBER() over (order by " + orden + ") as Nro ";
                sql += "from (select IdAduana, Aduana, count(*) as CantReg, sum(" + cifTot1 + ") as " + cifTot +
                       ", sum(" + pesoNeto1 + ") as " + pesoNeto2 + ", sum(Cantidad) as Cantidad ";
            }
            else if (filtro == "Distrito")
            {
                sql = "select IdDistrito, Distrito, CantReg, " + cifTot + ", " + pesoNeto2 +
                      ", Cantidad, ROW_NUMBER() over (order by " + orden + ") as Nro ";
                sql += "from (select IdDistrito, Distrito, count(*) as CantReg, sum(" + cifTot1 + ") as " + cifTot +
                       ", sum(" + pesoNeto1 + ") as " + pesoNeto2 + ", sum(Cantidad) as Cantidad ";
            }
            else if (filtro == "DetalleExcel")
            {
                sql = "select *, ROW_NUMBER() over (order by " + orden + ") as Nro ";
            }


            string tabla1 = tabla;

            if (filtro == "Marca" || filtro == "Modelo")
                tabla1 = "Importacion_PEB";

            sql += "from " + tabla1 + " T where 1 = 1 ";

            if( filtro == "PaisOrigen" && isDesarrollo)
            {
                sql = sql.Replace("T where", "T left join paises_complementarios PC on T.IdPaisOrigen = PC.ID_PAIS_COMPLEMENTARIO and PC.ID_PAIS_ORIGEN = '"+ codPais + "' where");
            }else if (filtro == "PaisDestino" && isDesarrollo)
            {
                sql = sql.Replace("T where", "T left join paises_complementarios PC on T.IdPaisDestino = PC.ID_PAIS_COMPLEMENTARIO and PC.ID_PAIS_ORIGEN = '" + codPais + "' where");
            }

            sql += Session["SqlFiltro"].ToString();

            if (filtro == "Partida")
                sql += "group by IdPartida) T where P.IdPartida = T.IdPartida ";
            else if (filtro == "Marca" || filtro == "MarcaEC")
                sql += "group by IdMarca, Marca) T ";
            else if (filtro == "Modelo")
                sql += "group by IdModelo, Marca, Modelo) T ";
            else if (filtro == "Importador")
                sql += "group by IdImportador, Importador) T ";
            else if (filtro == "Proveedor")

                sql += "group by IdProveedor, Proveedor) T ";
            else if (filtro == "Exportador")

                sql += "group by IdExportador, Exportador) T ";
            else if (filtro == "ImportadorExp")
                sql += "group by IdImportadorExp, ImportadorExp) T ";
            else if (filtro == "PaisOrigen")
            {
                if (isDesarrollo)
                {
                    sql += "group by IdPaisOrigen, PaisOrigen, PC.COD_PAIS_COMPLEMENTARIO) T ";
                }
                else
                {
                    sql += "group by IdPaisOrigen, PaisOrigen) T ";
                }
            }                
            else if (filtro == "PaisDestino")
            {
                if (isDesarrollo)
                {
                    sql += "group by IdPaisDestino, PaisDestino, PC.COD_PAIS_COMPLEMENTARIO) T ";
                }
                else
                {
                    sql += "group by IdPaisDestino, PaisDestino) T ";
                }
            }                
            else if (filtro == "ViaTransp")

                sql += "group by IdViaTransp, ViaTransp) T ";
            else if (filtro == "AduanaDUA")
                sql += "group by IdAduana, Aduana, " + dua + ") T ";
            else if (filtro == "Aduana")
                sql += "group by IdAduana, Aduana) T ";
            else if (filtro == "Distrito")
                sql += "group by IdDistrito, Distrito) T ";

            return sql;
        }

        private bool ValidarIngreso(string palabra)
        {
            bool bandera = true;
            

            if (palabra.Length < 3)
            {
                bandera = false;
            }
            else if (palabra.Contains("0") || palabra.Contains("1") || palabra.Contains("2") || palabra.Contains("3") || palabra.Contains("4") || palabra.Contains("5") ||
                palabra.Contains("6") || palabra.Contains("7") || palabra.Contains("8") || palabra.Contains("9")
                )
            {
                bandera = false;
            }

            return bandera;
        }

        private string GeneraSqlFiltro(string codPais, string codPais2, string tipoOpe,
            string anioMesIni, string anioMesFin, int indexCboPaisB,
            string codPaisB, string dua, string auxCodPais = "", bool isInfoTabla = false, bool tabInfo = false)
        {
            bool isManif = IsManifiesto(codPais);
            string sql = "";
            string filtroAux = "";
            List<string> listFiltroTablas = new List<string>();
            List<object> listFiltroValores= new List<object>();

            if (codPais2 == "4UE")
            {
                if (tipoOpe == "I")
                    sql += "and IdPaisImp = " + auxCodPais + " ";
                else
                {
                    sql += "and IdPaisExp = " + auxCodPais + " ";
                }
            }

            sql += "and FechaNum >= " + anioMesIni + "00 and FechaNum <= " + anioMesFin + "99 ";
            Session["FechaIni"] = anioMesIni + "00";
            Session["FechaFin"] = anioMesFin + "99";


            if (indexCboPaisB > 0)
            {
                if (tipoOpe == "I") { 
                    sql += "and "+ (!isManif? "IdPaisOrigen": "IdPaisEmbarque")  +" = " + codPaisB + " ";
                    listFiltroTablas.Add(!isManif ? "IdPaisOrigen" : "IdPaisEmbarque");
                }
                else

                {
                    sql += "and IdPaisDestino = " + codPaisB + " ";
                    listFiltroTablas.Add("IdPaisDestino");
                }
            }

            if (!string.IsNullOrWhiteSpace(Session["hdfPalabrasY"]?.ToString()))
            {
                filtroAux = "";
                sql += "and contains(Descomercial, '";
                listFiltroTablas.Add("Descomercial");
                //Se usa la instrucción rand en el sql final cuando se filtra Descomercial sin otro elemento posterior
                useRand = true;
                string[] palabrasY = Session["hdfPalabrasY"].ToString().Split(' ');
                bool inicio = true;
                foreach (string palabra in palabrasY)
                {
                    bool band = ValidarIngreso(palabra);
                    if (inicio)
                    {
                        if(band)
                            sql += "\"" + palabra + "*\" ";
                        else
                            sql += "\"" + palabra + "\" ";
                        inicio = false;
                    }
                    else
                    {
                        if(band)
                            sql += "and \"" + palabra + "*\" ";
                        else
                            sql += "and \"" + palabra + "\" ";
                    }
                        
                }
                sql += "') ";
            }
            
            if (!String.IsNullOrEmpty(Session["hdfNandinaB"]?.ToString()))
            {
                useRand = false;

                sql += "and (";
                string[] nandinas = Session["hdfNandinaB"].ToString().Split('|');
                bool contiene = false;
                listFiltroTablas.Add("IdPartida");
                foreach (string nandina in nandinas)
                {
                    string nandinaAux = nandina;
                    if (tabInfo && nandinaAux.Length > 6)
                    {
                        nandinaAux = nandinaAux.Substring(0, 6);
                    }
                    string buscarNandina = Funciones.BuscaIdTables("IdPartida","Partida_"+codPais, nandinaAux + "%","nandina"); 
                    if(nandinaAux != "")
                    {
                        bool existeNandina = (Funciones.BuscaIdPartida(nandinaAux, codPais) != "");
                        if (existeNandina) 
                            sql += "IdPartida = " + Funciones.BuscaIdPartida(nandinaAux, codPais) + " or ";
                        else
                        {
                            if (isInfoTabla)
                            {
                                sql += "IdPartida in (select IdPartida from Partida_" + codPais + " where Nandina like '" +
                                       nandinaAux + "%') or ";
                            }
                            else{
                                sql += "IdPartida in (" + buscarNandina + ") or ";
                            }
                            //sql += "IdPartida in (select IdPartida from Partida_" + codPais + " where Nandina like '" +
                            //       nandina + "%') or ";

                            //sql += "IdPartida in ("+buscarNandina+") or ";
                        }

                            
                        contiene = true;
                    }                    
                }

                if(contiene)
                    sql = sql.Substring(0, sql.Length - 3) + ") ";
                else
                    sql = sql.Substring(0, sql.Length - 5);

            }

            if (!String.IsNullOrEmpty(Session["hdfImportadorB"]?.ToString()))
            {
                listFiltroTablas.Add("IdImportador");
                string word = "";
                if (!isManif)
                {
                    useRand = false;
                    sql += "and (";
                    string[] importadores = Session["hdfImportadorB"].ToString().Split('|');
                    foreach (string importador in importadores)
                    {
                        word = (importador.ToUpper().Replace("[TODOS]", "")).Trim();
                        word = (word.ToUpper().Replace("[Todos]", "")).Trim();
                        word = (word.ToUpper().Replace("[ALL]", "")).Trim();
                        word = (word.ToUpper().Replace("[All]", "")).Trim();
                        if (word.Substring(0, 1) == "[")
                            sql += "IdImportador = " + word.Replace("[", "").Replace("]", "") + " or ";
                        else
                        {
                            //if (codPais == "PE")
                            //    sql += "IdImportador in (select IdEmpresa from Empresa_PE_con_datos where 1 = 1 ";
                            //else
                            //    sql += "IdImportador in (select IdEmpresa from Empresa_" + codPais + " where 1 = 1 ";

                            sql += " (1 = 1 ";
                            string[] palabras = word.Split(' ');
                            string tabla1 = "";

                            if (codPais == "PE")
                                tabla1 = "Empresa_PE_con_datos";
                            else
                                tabla1 = "Empresa_" + codPais;
                            string filtro = "1 = 1";
                            foreach (string palabra in palabras)
                            {

                                filtro += $" and Empresa like '%{palabra}%'";

                                
                            }

                            string buscarImportador = Funciones.BuscaCampoPorFiltro("IdEmpresa", tabla1, filtro);
                            sql += " and IdImportador in ( " + buscarImportador + ") ";
                            //sql += "and Empresa like '%" + palabra + "%' ";


                            sql += ") or ";
                        }
                    }

                    sql = sql.Substring(0, sql.Length - 3) + ") ";
                }
                else
                {
                    useRand = false;

                    if (codPais == "PE")
                        sql += "and IdImportador in (select IdEmpresa from Empresa_PE_con_datos where 1 = 1 ";
                    else
                        sql += "and IdImportador in (select IdEmpresa from Empresa_" + codPais + " where 1 = 1 ";

                    string[] Palabras = Session["hdfImportadorB"].ToString().Split(' ');
                    foreach (string Palabra in Palabras)
                    {
                        sql += "and Empresa like '%" + Palabra + "%' ";
                    }
                        
                    sql += ") ";
                }
            }

            if (!String.IsNullOrEmpty(Session["hdfExportadorB"]?.ToString()))
            {
                listFiltroTablas.Add("IdExportador");
                string word = "";
                if (!isManif)
                {
                    useRand = false;
                    sql += "and (";
                    string[] exportadores = Session["hdfExportadorB"].ToString().Split('|');

                    foreach (string exportador in exportadores)
                    {
                        word = (exportador.ToUpper().Replace("[TODOS]", "")).Trim();
                        word = (word.ToUpper().Replace("[Todos]", "")).Trim();
                        word = (word.ToUpper().Replace("[ALL]", "")).Trim();
                        word = (word.ToUpper().Replace("[All]", "")).Trim();
                        if (word.Substring(0, 1) == "[")
                            sql += "IdExportador = " + word.Replace("[", "").Replace("]", "") + " or ";
                        else
                        {
                            //if (codPais == "PE")
                            //    sql += "IdExportador in (select IdEmpresa from Empresa_PE_con_datos where 1 = 1 ";
                            //else
                            //    sql += "IdExportador in (select IdEmpresa from Empresa_" + codPais + " where 1 = 1 ";
                            sql += " (1 = 1 ";
                            string[] palabras = word.Split(' ');


                            string tabla1 = "";

                            if (codPais == "PE")
                                tabla1 = "Empresa_PE_con_datos";
                            else
                                tabla1 = "Empresa_" + codPais;
                            string filtro = "1 = 1";
                            foreach (string palabra in palabras)
                            {

                                filtro += $" and Empresa like '%{palabra}%'";


                            }

                            string buscarExportador = Funciones.BuscaCampoPorFiltro("IdEmpresa", tabla1, filtro);
                            sql += " and IdExportador in ( " + buscarExportador + ") ";

                            sql += ") or ";
                        }
                    }

                    sql = sql.Substring(0, sql.Length - 3) + ") ";
                }
                else
                {
                    useRand = false;

                    if (codPais == "PE")
                        sql += "and IdExportador in (select IdEmpresa from Empresa_PE_con_datos where 1 = 1 ";
                    else
                        sql += "and IdExportador in (select IdEmpresa from Empresa_" + codPais + " where 1 = 1 ";

                    string[] Palabras = Session["hdfExportadorB"].ToString().Split(' ');
                    foreach (string Palabra in Palabras)
                        sql += "and Empresa like '%" + Palabra + "%' ";
                    sql += ") ";
                }
            }

            if (!String.IsNullOrEmpty(Session["hdfProveedorB"]?.ToString()))
            {

                listFiltroTablas.Add("IdProveedoor");
                string word = "";
                if (!isManif)
                {                    

                    string[] palabras = Session["hdfProveedorB"].ToString().Split('|');

                    sql += "and (";
                    //sql += "and IdProveedor in (select IdProveedor from Proveedor_" + codPais + " where ";

                    foreach (string palabra in palabras) {                         
                        word = (palabra.ToUpper().Replace("[TODOS]", "")).Trim();
                        word = (word.ToUpper().Replace("[Todos]", "")).Trim();
                        word = (word.ToUpper().Replace("[ALL]", "")).Trim();
                        word = (word.ToUpper().Replace("[All]", "")).Trim();
                        string buscarProveedor = Funciones.BuscaIdTables("IdProveedor", "Proveedor_" + codPais, "%"+word+"%", "Proveedor");

                        sql += "IdProveedor in ("+buscarProveedor+") or ";
                        //sql += "Proveedor like '%" + word + "%' or ";
                    }
                    sql = sql.Substring(0, sql.Length - 3) + ")";
                    useRand = false;
                }
                else
                {
                    sql += "and IdProveedor in (select IdProveedor from Proveedor_" + codPais + " PR where 1 = 1 ";
                    string[] Palabras = Session["hdfProveedorB"].ToString().Split(' ');
                    foreach (string Palabra in Palabras) {
                        word = (Palabra.ToUpper().Replace("[TODOS]", "")).Trim();
                        word = (word.ToUpper().Replace("[Todos]", "")).Trim();
                        word = (Palabra.ToUpper().Replace("[ALL]", "")).Trim();
                        word = (word.ToUpper().Replace("[All]", "")).Trim();
                        sql += "and Proveedor like '%" + word + "%' ";
                    }
                    sql += ") ";
                    useRand = false;
                }
                
            }

            if (!String.IsNullOrEmpty(Session["hdfImportadorExpB"]?.ToString()))
            {

                listFiltroTablas.Add("IdImportadorExp");
                if (!isManif)
                {
                    useRand = false;
                    sql += "and IdImportadorExp in (select IdImportadorExp from ImportadorExp_" + codPais + " where ";

                    string[] palabras = Session["hdfImportadorExpB"].ToString().Split('|');

                    foreach (string palabra in palabras)
                    {
                        var palabraAux = (palabra.Replace("[TODOS]", "")).Trim();
                        palabraAux = (palabraAux.ToUpper().Replace("[Todos]", "")).Trim();
                        palabraAux = (palabraAux.Replace("[ALL]", "")).Trim();
                        palabraAux = (palabraAux.ToUpper().Replace("[All]", "")).Trim();
                        sql += "ImportadorExp like '%" + palabraAux + "%' or ";
                    }
                    sql = sql.Substring(0, sql.Length - 3) + ")";
                }
                else
                {
                    useRand = false;
                    sql += "and IdImportadorExp in (select IdImportadorExp from ImportadorExp_" + codPais +
                           " IE where 1 = 1 ";
                    string[] Palabras = Session["hdfImportadorExpB"].ToString().Split(' ');
                    foreach (string Palabra in Palabras)
                    {
                        var palabraAux = (Palabra.Replace("[TODOS]", "")).Trim();
                        palabraAux = (palabraAux.ToUpper().Replace("[Todos]", "")).Trim();
                        palabraAux = (palabraAux.Replace("[ALL]", "")).Trim();
                        palabraAux = (palabraAux.ToUpper().Replace("[All]", "")).Trim();
                        sql += "and IE.ImportadorExp like '%" + palabraAux + "%' ";
                    }
                    sql += ") ";
                }
            }

            if (!isManif)
            {
                if (Session["PartidasB"] != null)
                {
                    sql += "and IdPartida in " + Funciones.ListaItems((ArrayList)Session["PartidasB"]) + " ";
                    listFiltroTablas.Add("IdPartida");
                    useRand = false;
                }
                if (Session["MarcasB"] != null) { 
                    sql += "and IdMarca in " + Funciones.ListaItems((ArrayList)Session["MarcasB"]) + " ";
                    listFiltroTablas.Add("IdMarca");
                    useRand = false;
                }
                if (Session["ModelosB"] != null) { 
                    sql += "and IdModelo in " + Funciones.ListaItems((ArrayList)Session["ModelosB"]) + " ";
                    listFiltroTablas.Add("IdModelo");
                    useRand = false;
                }
                if (Session["ImportadoresB"] != null) { 
                    sql += "and IdImportador in " + Funciones.ListaItems((ArrayList)Session["ImportadoresB"]) + " ";
                    listFiltroTablas.Add("IdImportador");
                    useRand = false;
                }
                if (Session["ExportadoresB"] != null) { 
                    sql += "and IdExportador in " + Funciones.ListaItems((ArrayList)Session["ExportadoresB"]) + " ";
                    listFiltroTablas.Add("IdExportador");
                    useRand = false;
                }
                if (Session["ProveedoresB"] != null) { 
                    sql += "and IdProveedor in " + Funciones.ListaItems((ArrayList)Session["ProveedoresB"]) + " ";
                    listFiltroTablas.Add("IdProveedor");
                    useRand = false;
                }
                if (Session["ImportadoresExpB"] != null) { 
                    sql += "and IdImportadorExp in " + Funciones.ListaItems((ArrayList)Session["ImportadoresExpB"]) + " ";
                    listFiltroTablas.Add("IdImportadorExp");
                    useRand = false;
                }
                if (Session["PaisesOrigenB"] != null) { 
                    sql += "and IdPaisOrigen in " + Funciones.ListaItems((ArrayList)Session["PaisesOrigenB"]) + " ";
                    listFiltroTablas.Add("IdPaisOrigen");
                    useRand = false;
                }
                if (Session["PaisesDestinoB"] != null) { 
                    sql += "and IdPaisDestino in " + Funciones.ListaItems((ArrayList)Session["PaisesDestinoB"]) + " ";
                    listFiltroTablas.Add("IdPaisDestino");
                    useRand = false;
                }
                //if (Session["PaisesEmbarqueB"] != null)
                //{
                //    sql += "and IdPaisEmbarque in " + Funciones.ListaItems((ArrayList)Session["PaisesEmbarqueB"]) + " ";
                //    listFiltroTablas.Add("IdPaisEmbarque");
                //    useRand = false;
                //}
                if (Session["MarcaECB"] != null) { 
                    sql += "and IdMarca in " + Funciones.ListaItems((ArrayList)Session["MarcaECB"]) + " ";
                    listFiltroTablas.Add("IdMarca");
                    useRand = false;
                }
                if (Session["ViasTranspB"] != null) { 
                    sql += "and IdViaTransp in " + Funciones.ListaItems((ArrayList)Session["ViasTranspB"]) + " ";
                    listFiltroTablas.Add("IdViaTransp");
                    useRand = false;
                }
                if (Session["AduanaDUAsB"] != null) { 
                    sql += "and convert(varchar(3), IdAduana) + '-' + convert(varchar(20), " + dua + ") in " +
                           Funciones.ListaItemsS((ArrayList)Session["AduanaDUAsB"]) + " ";
                    listFiltroTablas.Add("IdAduana");
                    useRand = false;
                }
                if (Session["AduanasB"] != null) { 
                    sql += "and IdAduana in " + Funciones.ListaItems((ArrayList)Session["AduanasB"]) + " ";
                    listFiltroTablas.Add("IdAduana");
                    useRand = false;
                }
                //if (Session["DUAsB"] != null)
                //    sql += "and " + DUA + " like '" + Functions.ListaItems((ArrayList)Session["DUAsB"]).Replace("(", "").Replace(")", "") + "%' ";
                if (Session["DistritosB"] != null) { 
                    sql += "and IdDistrito in " + Funciones.ListaItems((ArrayList)Session["DistritosB"]) + " ";
                    listFiltroTablas.Add("IdDistrito");
                    useRand = false;
                }
            }
            else
            {
                if (Session["ImportadoresB"] != null) { 
                    sql += "and IdImportador in " + Funciones.ListaItems((ArrayList)Session["ImportadoresB"]) + " ";
                    listFiltroTablas.Add("IdImportador");
                    useRand = false;
                }
                if (Session["NotificadosB"] != null) {
                    sql += "and IdNotificado in " + Funciones.ListaItems((ArrayList)Session["NotificadosB"]) + " ";
                    listFiltroTablas.Add("IdNotificado");
                    useRand = false;
                }
                if (Session["ExportadoresB"] != null) {
                    sql += "and IdExportador in " + Funciones.ListaItems((ArrayList)Session["ExportadoresB"]) + " ";
                    listFiltroTablas.Add("IdExportador");
                    useRand = false;
                }
                if (Session["ProveedoresB"] != null) {
                    sql += "and IdProveedor in " + Funciones.ListaItems((ArrayList)Session["ProveedoresB"]) + " ";
                    listFiltroTablas.Add("IdProveedor");
                    useRand = false;
                }
                if (Session["ImportadoresExpB"] != null) {
                    sql += "and IdImportadorExp in " + Funciones.ListaItems((ArrayList)Session["ImportadoresExpB"]) + " ";
                    listFiltroTablas.Add("IdImportadorExp");
                    useRand = false;
                }
                if (Session["PaisesOrigenB"] != null) {
                    sql += "and IdPaisOrigen in " + Funciones.ListaItems((ArrayList)Session["PaisesOrigenB"]) + " ";
                    listFiltroTablas.Add("IdPaisOrigen");
                    useRand = false;
                }
                if (Session["PaisesEmbarqueB"] != null) {
                    sql += "and IdPaisEmbarque in " + Funciones.ListaItems((ArrayList)Session["PaisesEmbarqueB"]) + " ";
                    listFiltroTablas.Add("IdPaisEmbarque");
                    useRand = false;
                }
                if (Session["PaisesDestinoB"] != null) {
                    sql += "and IdPaisDestino in " + Funciones.ListaItems((ArrayList)Session["PaisesDestinoB"]) + " ";
                    listFiltroTablas.Add("IdPaisDestino");
                    useRand = false;
                }
                if (Session["PtosDescargaB"] != null) {
                    sql += "and IdPtoDescarga in " + Funciones.ListaItems((ArrayList)Session["PtosDescargaB"]) + " ";
                    listFiltroTablas.Add("IdPtoDescarga");
                    useRand = false;
                }
                if (Session["PtosEmbarqueB"] != null) {
                    sql += "and IdPtoEmbarque in " + Funciones.ListaItems((ArrayList)Session["PtosEmbarqueB"]) + " ";
                    listFiltroTablas.Add("IdPtoEmbarque");
                    useRand = false;
                }
                if (Session["PtosDestinoB"] != null) {
                    sql += "and IdPtoDestino in " + Funciones.ListaItems((ArrayList)Session["PtosDestinoB"]) + " ";
                    listFiltroTablas.Add("IdPtoDestino");
                    useRand = false;
                }
                if (Session["ManifiestosB"] != null) {
                    sql += "and Manifiesto in " + Funciones.ListaItemsS((ArrayList)Session["ManifiestosB"]) + " ";
                    listFiltroTablas.Add("Manifiesto");
                    useRand = false;
                }
            }
            

            if (Session["hdfIdGrupoB"] != null) { 
                sql += "and Id" + Session["hdfTipoFavoritoB"].ToString() +
                       " in (select IdFavorito from FavoritoGrupo where IdGrupo = " + Session["hdfIdGrupoB"] + ") ";
                listFiltroTablas.Add("Id" + Session["hdfTipoFavoritoB"].ToString());
                useRand = false;
            }
            Session["FiltroTablas"] = listFiltroTablas;

            return sql;
        }

       

        private List<string> GeneraSqlFiltroTablas(string tipOpe)
        {
            List<string> tablas = new List<string>();
            List<string> filtros = (List<string>)Session["FiltroTablas"];
            string codPais = Session["CodPais"].ToString();
            //string tipOpe = Session["TipoOpe"].ToString();

            string filtroRegimen = "";
            if (tipOpe == "I")
            {
                filtroRegimen = @"Importacion_";
            }
            else
            {
                filtroRegimen = @"Exportacion_";
            }

            string sql1 = "SELECT TABLE_NAME, count(TABLE_NAME) as NUMROWS FROM INFORMATION_SCHEMA.COLUMNS ";
            string sql2 = "WHERE (";
            string sql3 = $@"and (TABLE_NAME like '{filtroRegimen}%')
                            and SUBSTRING(table_name,13,6) in (
	                            select
	                            case when SUBSTRING(IdVariable,3,1) = '_' then SUBSTRING(IdVariable,0,3) + 'I'
		                            else IdVariable 
	                            end as IdVariable
	                            from VariableGeneral where IdGrupo = 'SRE' and idVariable != '{codPais}'
	                            union
	                            select SUBSTRING(IdVariable,0,3) + 'E' as IdVariable
	                            from VariableGeneral where IdGrupo = 'SRE' and SUBSTRING(IdVariable,3,1) = '_' and idVariable != '{codPais}'
	                            group by IdVariable
	                            union
	                            select 'UE' as IdVariable
                            ) group by TABLE_NAME having count(TABLE_NAME) = { filtros.Count } order by TABLE_NAME";            
            int cont = 1;
            foreach (var filtro in filtros)
            {
                string fil = filtro;
                if (filtro == "IdPaisOrigen" && tipOpe == "E")
                    fil = "IdPaisDestino";
                else if (filtro == "IdPaisDestino" && tipOpe == "I")
                    fil = "IdPaisOrigen";
                else if (filtro == "IdImportador" && tipOpe == "E")
                    fil = "IdExportador";
                else if (filtro == "IdExportador" && tipOpe == "I")
                    fil = "IdImportador";
                else if (filtro == "IdProveedoor" && tipOpe == "E")
                    fil = "IdImportadorExp";
                else if (filtro == "IdImportadorExp" && tipOpe == "I")
                    fil = "IdProveedoor";
                sql2 += cont == filtros.Count ? " COLUMN_NAME LIKE '" + fil + "') " : " COLUMN_NAME LIKE '" + fil + "' OR";
                cont++;
                
            }
            DataTable dt;
            string sqlTabla = sql1 + sql2 + sql3;

            try
            {
                dt = Conexion.SqlDataTable(sqlTabla);
                foreach (DataRow dtRow in dt.Rows)
                {
                    tablas.Add(dtRow["TABLE_NAME"].ToString());
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                dt = null;
            }

            return tablas;
        }
        string GeneraSqlAgrupadoOrdenadoManif(string filtro, string codPais, string idioma, string tabla)
        {
            string sql = "";

            if (filtro == "Importador")
            {
                sql = "select IdImportador, Empresa as Importador ";
                sql += "from Empresa_" + codPais + " E, (select IdImportador ";
            }
            else if (filtro == "Notificado")
            {
                sql = "select T.IdNotificado, Notificado  ";
                sql += "from Notificado_" + codPais + " N, (select IdNotificado ";
            }
            else if (filtro == "Proveedor")
            {
                sql = "select T.IdProveedor, Proveedor  ";
                sql += "from Proveedor_" + codPais + " P, (select IdProveedor ";
            }
            else if (filtro == "Exportador")
            {
                sql = "select IdExportador, Empresa as Exportador ";
                sql += "from Empresa_" + codPais + " E, (select IdExportador ";
            }
            else if (filtro == "ImportadorExp")
            {
                sql = "select T.IdImportadorExp, ImportadorExp ";
                sql += "from ImportadorExp_" + codPais + " I, (select IdImportadorExp ";
            }
            else if (filtro == "PaisEmbarque")
            {
                sql = "select IdPaisEmbarque, Pais as PaisEmbarque ";
                sql += "from Pais_" + codPais + " P, (select IdPaisEmbarque ";
            }
            else if (filtro == "PaisDestino")
            {
                sql = "select IdPaisDestino, Pais as PaisDestino ";
                sql += "from Pais_" + codPais + " P, (select IdPaisDestino ";
            }
            else if (filtro == "PtoEmbarque" || filtro == "PtoDestino" || filtro == "PtoDescarga")
            {
                sql = "select T.Id" + filtro + ", Puerto as " + filtro + " ";
                sql += "from Puerto_" + codPais + " P, (select Id" + filtro + " ";
            }
            else if (filtro == "Manifiesto")
            {
                sql = "select distinct Manifiesto ";
            }

            sql += "from " + tabla + " T where 1 = 1 ";
            sql += Session["SqlFiltro"].ToString();

            if (filtro == "Importador")
                sql += "group by IdImportador) T where E.IdEmpresa = T.IdImportador ";
            else if (filtro == "Notificado")
                sql += "group by IdNotificado) T where N.IdNotificado = T.IdNotificado ";
            else if (filtro == "Proveedor")
                sql += "group by IdProveedor) T where P.IdProveedor = T.IdProveedor ";
            else if (filtro == "Exportador")
                sql += "group by IdExportador) T where E.IdEmpresa = T.IdExportador ";
            else if (filtro == "ImportadorExp")
                sql += "group by IdImportadorExp) T where I.IdImportadorExp = T.IdImportadorExp ";
            else if (filtro == "PaisEmbarque")
                sql += "group by IdPaisEmbarque) T where P.IdPais = T.IdPaisEmbarque ";
            else if (filtro == "PaisDestino")
                sql += "group by IdPaisDestino) T where P.IdPais = T.IdPaisDestino ";
            else if (filtro == "PtoEmbarque" || filtro == "PtoDestino" || filtro == "PtoDescarga")
                sql += "group by Id" + filtro + ") T where P.IdPto = T.Id" + filtro + " ";
            else if (filtro == "Manifiesto")
                sql += " ";

            if (filtro != "Manifiesto") sql += "order by 2";
            else sql += "order by 1";


            return sql;
        }

        string GeneraSqlAgrupadoOrdenado(string filtro, string codPais, string idioma, string tabla)
        {
            string sql = "";

            if (filtro == "Partida")
            {
                string partida = "Partida";
                if (idioma == "en")
                    partida = "Partida_en";


                sql = "select IdPartida, Nandina, Nandina + ' ' + " + partida + " as Partida from Partida_" + codPais +
                      " where IdPartida in (select distinct IdPartida ";
            }

            else if (filtro == "Marca")

                sql = "select IdMarca, Marca from Marca_PEB where IdMarca in (select distinct IdMarca ";
            else if (filtro == "MarcaEC")

                sql = "select IdMarca, Marca from Marca_"+codPais+" where IdMarca in (select distinct IdMarca ";
            else if (filtro == "Modelo")
                sql =
                    "select IdModelo, Marca + ' - ' + Modelo as Modelo from Modelo_PEB where IdModelo in (select distinct IdModelo ";
            else if (filtro == "Importador")
                sql = "select IdEmpresa as IdImportador, Empresa as Importador from Empresa_" + codPais +
                      " where IdEmpresa in (select distinct IdImportador ";
            else if (filtro == "Proveedor")
                sql = "select IdProveedor, Proveedor from Proveedor_" + codPais +
                      " where IdProveedor in (select distinct IdProveedor ";

            else if (filtro == "Exportador")
                sql = "select IdEmpresa as IdExportador, Empresa as Exportador from Empresa_" + codPais +
                      " where IdEmpresa in (select distinct IdExportador ";
            else if (filtro == "ImportadorExp")
                sql = "select IdImportadorExp, ImportadorExp from ImportadorExp_" + codPais +
                      " where IdImportadorExp in (select distinct IdImportadorExp ";
            else if (filtro == "PaisOrigen")
                sql = "select IdPais as IdPaisOrigen, Pais as PaisOrigen from Pais_" + codPais +
                      " where IdPais in (select distinct IdPaisOrigen ";
            else if (filtro == "PaisDestino")
                sql = "select IdPais as IdPaisDestino, Pais as PaisDestino from Pais_" + codPais +
                      " where IdPais in (select distinct IdPaisDestino ";
            else if (filtro == "ViaTransp")
                sql = "select IdViaTransp, ViaTransp from ViaTransp_" + codPais +
                      " where IdViaTransp in (select distinct IdViaTransp ";
            else if (filtro == "Aduana")

                sql = "select IdAduana, Aduana from Aduana_" + codPais +
                      " where IdAduana in (select distinct IdAduana ";

            else if (filtro == "Distrito")
                sql = "select IdDistrito, Distrito from Distrito_" + codPais +
                      " where IdDistrito in (select IdDistrito ";

            string tabla1 = tabla;
            if (filtro == "Marca" || filtro == "Modelo")
                tabla1 = "Importacion_PEB";

            sql += "from " + tabla1 + " T where 1 = 1 ";
            sql += Session["SqlFiltro"].ToString();


            sql += ") order by 2";

            return sql;
        }

        private List<GridRow> GetDataTableToListGridRow(DataTable dt, string filtro, string cifTot,
            string valueCifTot, string codPais, CultureInfo specificCulture , string anioMesIni = "", 
            string anioMesFin = "", int indexCboPaisB = 0, string codPaisB = "", string auxCodPais = "")
        {
            try
            {
                var lista = new List<GridRow>();
                if (dt == null || dt.Rows.Count == 0) goto ReturnMe;
                decimal floatValueCifTot = Convert.ToDecimal(valueCifTot);
                decimal valRecordCitTot;
                var idPlan = Funciones.ObtieneIdPlan(Session["IdUsuario"].ToString());
                Int64 cantReg = 0;
                if ((filtro == Enums.Filtro.Importador.ToString() || filtro == Enums.Filtro.Exportador.ToString())
                    && (codPais == "PE" || codPais == "PEB" || codPais == "CO"))
                {
                    string[] planesNoSentinel = { "BUSINESS", "PREMIUM", "UNIVERSIDADES" };

                    if (planesNoSentinel.Contains(Session["Plan"].ToString()))
                    {
                        Int64 id = 0;
                        foreach (DataRow drRow in dt.Rows)
                        {
                            id = drRow.GetValue<Int64>("Id" + filtro);
                            valRecordCitTot = drRow.GetValue<decimal>(cifTot);
                            cantReg = Convert.ToInt64(drRow["CantReg"]);
                            GridRow row = new GridRow()
                            {
                                Id = id.ToString(),
                                Descripcion = drRow[filtro].ToString(),
                                TotalReg = cantReg.ToString("n0", specificCulture),
                                CiFoFobTot = valRecordCitTot.ToString("n0", specificCulture),
                                CiFoFobPor =
                                    (valRecordCitTot / floatValueCifTot * 100).ToString("n2", specificCulture) + "%",
                                Ruc = FuncionesBusiness.BuscaRUC(id.ToString(), "PE"),
                                IsEnabledTotalReg = cantReg <= CantRegMax
                            };
                            if(codPais == "PE" || codPais == "PEB")
                            {
                                row.IsPlanPermiteSentinel = FuncionesBusiness.IsVisibleSentinal(idPlan);//true,
                                row.IsVisibleSentinel = false; // Ruben 202311
                                //row.IsVisibleSentinel = true;//FuncionesBusiness.IsVisibleSentinal(idPlan)
                            } else if(codPais == "CO")
                            {
                                row.IsPlanPermiteSentinel = FuncionesBusiness.IsVisibleSentinal2023(idPlan); // Ruben 202311
                                row.mostrarInformaColombia = true;
                            }
                            lista.Add(row);
                        }
                    }
                    else
                    {
                        foreach (DataRow drRow in dt.Rows)
                        {
                            valRecordCitTot = drRow.GetValue<decimal>(cifTot);
                            cantReg = Convert.ToInt64(drRow["CantReg"]);
                            GridRow row = new GridRow()
                            {
                                Id = drRow.GetValue<string>("Id" + filtro),
                                Descripcion = drRow[filtro].ToString(),
                                TotalReg = cantReg.ToString("n0", specificCulture),
                                CiFoFobTot = valRecordCitTot.ToString("n0", specificCulture),
                                CiFoFobPor =
                                    (valRecordCitTot / floatValueCifTot * 100).ToString("n2", specificCulture) + "%",
                                IsEnabledTotalReg = cantReg <= CantRegMax
                            };
                            if (codPais == "PE" || codPais == "PEB")
                            {
                                row.IsPlanPermiteSentinel = FuncionesBusiness.IsVisibleSentinal(idPlan);//true,
                                row.IsVisibleSentinel = false; // Ruben 202311
                                //row.IsVisibleSentinel = true;//FuncionesBusiness.IsVisibleSentinal(idPlan)
                            }
                            else if (codPais == "CO")
                            {
                                row.IsPlanPermiteSentinel = FuncionesBusiness.IsVisibleSentinal2023(idPlan); // Ruben 202311
                                row.mostrarInformaColombia = true;
                            }
                            lista.Add(row);
                        }
                    }
                }
                else if (filtro == Enums.Filtro.InfoTabla.ToString())
                {

                    if (dt != null)
                    {
                        //if (filtro == "MarcaEC") filtro = "Marca";
                        foreach (DataRow drRow in dt.Rows)
                        {
                            var IsManif = IsManifiesto(drRow.GetValue<string>("IdPais"));
                            cantReg = drRow.GetValue<int>("CantReg");
                            valRecordCitTot = drRow.GetValue<decimal>(cifTot);
                            /*string id = drRow.GetValue<string>(drRow.GetValue<string>("IdPais"));
                            string a = valRecordCitTot.ToString("n" + (!IsManif ? "0" : "1"), specificCulture);
                            string b = (valRecordCitTot / floatValueCifTot * 100).ToString("n" + (!IsManif ? "2" : "1"),
                                           specificCulture) + "%";*/
                            lista.Add(new GridRow()
                            {
                                Id = drRow.GetValue<string>("IdPais"),
                                Descripcion = drRow.GetValue<string>("Pais"),
                                TotalReg = cantReg.ToString("n0", specificCulture),
                                CiFoFobTot = valRecordCitTot.ToString("n" + (!IsManif ? "0" : "1"), specificCulture),
                                CiFoFobPor = (valRecordCitTot / floatValueCifTot * 100).ToString("n" + (!IsManif ? "2" : "1"), specificCulture) + "%",
                                IsEnabledTotalReg = false,
                                IsVisibleSentinel = false//FuncionesBusiness.IsVisibleSentinal(idPlan)//false
                            });
                        }
                    }
                }
                else
                {
                    var IsManif = IsManifiesto(codPais);
                    if (dt != null)
                    {
                        if (filtro == "MarcaEC") filtro = "Marca";
                        foreach (DataRow drRow in dt.Rows)
                        {
                            cantReg = drRow.GetValue<int>("CantReg");
                            valRecordCitTot = drRow.GetValue<decimal>(cifTot);
                            string id = drRow.GetValue<string>((filtro != "Manifiesto" ? "Id" : "") + filtro);
                            string a = valRecordCitTot.ToString("n" + (!IsManif ? "0" : "1"), specificCulture);
                            string b = (floatValueCifTot > 0 ? (valRecordCitTot / floatValueCifTot * 100) : 0).ToString("n" + (!IsManif ? "2" : "1"),
                                           specificCulture) + "%";

                            string codPaisFlag = (filtro == "PaisOrigen" || filtro == "PaisDestino") ? drRow.GetValue<string>("Flag") : "";

                            bool tipoServidor = Properties.Settings.Default.TableVarGeneral_InDev;
                            if (!string.IsNullOrEmpty(codPaisFlag) /*&& tipoServidor*/)
                            {
                                string idioma = GetCurrentIdioma();
                                string codPaisx = codPais;
                                
                                string nombrePais = FuncionesBusiness.BuscarPaisNombre(codPais, idioma);

                                if (codPaisx == "UE")
                                {
                                    nombrePais = FuncionesBusiness.BuscarPaisUE(auxCodPais, idioma);
                                    codPaisx = auxCodPais;
                                }

                                if (nombrePais == "USA")
                                {
                                    nombrePais = "ESTADOS UNIDOS";
                                }
                                string codPais2 = new ListaPaises().BuscarCodPais2(codPaisFlag);
                                if (codPais == "UE")
                                {
                                    codPais2 = "4UE1";
                                }
                                string tipoOpe = Session["TipoOpe"].ToString() == "I" ? "E" : "I";
                                string tabla = "", dua = "";
                                
                                GetTablaAndDua(tipoOpe, codPaisFlag, ref tabla, ref dua);

                                string sqlAux =  GeneraSqlFiltro(codPaisFlag, codPais2, tipoOpe, anioMesIni, anioMesFin, indexCboPaisB, codPaisB, dua, codPaisx, tabInfo: true);
                                int idPais = FuncionesBusiness.BuscarIdPaisNombre(nombrePais, codPaisFlag);
                                if (nombrePais == "Alemania" && codPaisFlag == "EC")
                                {
                                    idPais = 53;
                                }
                                if (nombrePais == "ESTADOS UNIDOS" && codPaisFlag == "EC")
                                {
                                    idPais = 158;
                                }
                                // Ruben 202310
                                else if (nombrePais == "ESTADOS UNIDOS" && codPaisFlag == "MXD")
                                {
                                    idPais = 321;
                                }

                                if (idPais != 0)
                                {
                                    Session["SqlInfoComplementario" + codPaisFlag] = sqlAux + (tipoOpe == "I" ? " and idPaisOrigen = " : " and idPaisDestino = ") + idPais + " ";
                                }
                                else
                                {
                                    Session["SqlInfoComplementario" + codPaisFlag] = sqlAux;
                                }

                                string cifTotAux = Funciones.Incoterm(codPaisFlag, tipoOpe) + "Tot";
                                string pesoNeto = Funciones.CampoPeso(codPaisFlag, tipoOpe);
                                string sql = GeneraSqlAgrupado(Enums.Filtro.InfoTabla.ToString(), codPaisFlag, cifTotAux, pesoNeto, idioma, dua, tabla, "", isInfoTabla: true);

                                DataTable dt1;
                                try
                                {
                                    dt1 = Conexion.SqlDataTable(sql);
                                }
                                catch (Exception ex)
                                {                                                                                                            
                                    ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                                    dt1 = null;
                                }

                                if(dt1 == null || (int)dt1.Rows[0][2] == 0)
                                {
                                    codPaisFlag = "";
                                }

                            }

                            lista.Add(new GridRow()
                            {
                                Id = drRow.GetValue<string>((filtro != "Manifiesto" ? "Id" : "") + filtro),
                                Descripcion = drRow.GetValue<string>(filtro),
                                TotalReg = cantReg.ToString("n0", specificCulture),
                                CiFoFobTot = valRecordCitTot.ToString("n" + (!IsManif ? "0" : "1"), specificCulture),
                                CiFoFobPor = (floatValueCifTot > 0 ? (valRecordCitTot / floatValueCifTot * 100) : 0).ToString("n" + (!IsManif ? "2" : "1"), specificCulture) + "%",
                                IsEnabledTotalReg = cantReg <= CantRegMax,
                                IsVisibleSentinel = false,//FuncionesBusiness.IsVisibleSentinal(idPlan)//false
                                CodPaisComplementario = codPaisFlag//(filtro == "PaisOrigen" || filtro == "PaisDestino") ? drRow.GetValue<string>("Flag") : ""
                            });
                        }
                    }
                }
                ReturnMe:
                return lista;
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                throw;
            }
            
        }

        private List<GridRow> GetDataTableToListGridRowTab(DataTable dt, string filtro, string cifTot,
            string valueCifTot, string codPais, CultureInfo specificCulture,
            string pesoNeto, string unidad, string anioMesIni = "",
            string anioMesFin = "", int indexCboPaisB = 0, string codPaisB = "", string auxCodPais = "")
        {
            var lista = new List<GridRow>();
            var idPlan = Funciones.ObtieneIdPlan(Session["IdUsuario"].ToString());
            decimal floatValueCifTot = Convert.ToDecimal(valueCifTot);
            bool existeTotalKgyPrecio = false;
            string cantidadField = "";
            if (unidad != "")
            {
                existeTotalKgyPrecio = true;
                if (unidad != "kg")
                    cantidadField = "Cantidad";
                else
                {
                    cantidadField = pesoNeto;
                }
            }

            decimal cantidad = 0;
            decimal valRecordCitTot;

            Int64 cantReg = 0;
            if ((filtro == "Importador" || filtro == "Exportador")
                && (codPais == "PE" || codPais == "PEB" || codPais == "CO") && filtro != Enums.Filtro.InfoTabla.ToString())
            {
                string[] planesNoSentinel = { "BUSINESS", "PREMIUM", "UNIVERSIDADES" };

                var planPermiteSentinel = planesNoSentinel.Contains(Session["Plan"].ToString());

                if (planPermiteSentinel)
                {
                    Int64 id = 0;
                    if (existeTotalKgyPrecio)
                    {
                        foreach (DataRow drRow in dt.Rows)
                        {
                            id = drRow.GetValue<Int64>("Id" + filtro);
                            cantReg = Convert.ToInt64(drRow["CantReg"]);
                            valRecordCitTot = drRow.GetValue<decimal>(cifTot);
                            cantidad = Convert.ToDecimal(drRow[cantidadField]);
                            GridRow row = new GridRow()
                            {
                                Id = id.ToString(),
                                Descripcion = drRow[filtro].ToString(),
                                TotalReg = cantReg.ToString("n0", specificCulture),
                                CiFoFobTot = valRecordCitTot.ToString("n0", specificCulture),
                                CiFoFobPor =
                                    (valRecordCitTot / floatValueCifTot * 100).ToString("n2", specificCulture) + "%",
                                Ruc = FuncionesBusiness.BuscaRUC(id.ToString(), "PE"),
                                IsEnabledTotalReg = cantReg <= CantRegMax,
                                IsVisibleSentinel = false, // Ruben 202311
                                //IsVisibleSentinel = true,//FuncionesBusiness.IsVisibleSentinal(idPlan),//true,
                                IsPlanPermiteSentinel = FuncionesBusiness.IsVisibleSentinal(idPlan),//true,
                                TotalKg = cantidad.ToString("n0", specificCulture),
                                Precio = cantidad > 0 ? (valRecordCitTot / cantidad).ToString("n3", specificCulture) : "0"
                            };
                            if (codPais == "CO")
                            {
                                row.IsVisibleSentinel = false;
                                row.IsPlanPermiteSentinel = FuncionesBusiness.IsVisibleSentinal2023(idPlan); // Ruben 202311
                                row.mostrarInformaColombia = true;
                            } 
                            lista.Add(row);

                        }
                    }
                    else
                    {
                        foreach (DataRow drRow in dt.Rows)
                        {
                            id = drRow.GetValue<Int64>("Id" + filtro);
                            cantReg = Convert.ToInt64(drRow["CantReg"]);
                            valRecordCitTot = drRow.GetValue<decimal>(cifTot);
                            GridRow row = new GridRow()
                            {
                                Id = id.ToString(),
                                Descripcion = drRow[filtro].ToString(),
                                TotalReg = cantReg.ToString("n0", specificCulture),
                                CiFoFobTot = valRecordCitTot.ToString("n0", specificCulture),
                                CiFoFobPor =
                                    (valRecordCitTot / floatValueCifTot * 100).ToString("n2", specificCulture) + "%",
                                Ruc = FuncionesBusiness.BuscaRUC(id.ToString(), "PE"),
                                IsEnabledTotalReg = cantReg <= CantRegMax,
                                IsVisibleSentinel = false, // Ruben 202311
                                //IsVisibleSentinel = true,//FuncionesBusiness.IsVisibleSentinal(idPlan),//true,
                                IsPlanPermiteSentinel = FuncionesBusiness.IsVisibleSentinal(idPlan)//true
                            };
                            if (codPais == "CO")
                            {
                                row.IsVisibleSentinel = false;
                                row.IsPlanPermiteSentinel = FuncionesBusiness.IsVisibleSentinal2023(idPlan); // Ruben 202311
                                row.mostrarInformaColombia = true;
                            }
                            lista.Add(row);
                        }
                    }
                }
                else
                {
                    if (existeTotalKgyPrecio)
                    {
                        foreach (DataRow drRow in dt.Rows)
                        {
                            cantReg = Convert.ToInt64(drRow["CantReg"]);
                            cantidad = Convert.ToDecimal(drRow[cantidadField]);
                            valRecordCitTot = drRow.GetValue<decimal>(cifTot);
                            lista.Add(new GridRow()
                            {
                                Id = drRow.GetValue<string>("Id" + filtro),
                                Descripcion = drRow[filtro].ToString(),
                                TotalReg = cantReg.ToString("n0", specificCulture),
                                CiFoFobTot = valRecordCitTot.ToString("n0", specificCulture),
                                CiFoFobPor =
                                    (valRecordCitTot / floatValueCifTot * 100).ToString("n2", specificCulture) + "%",
                                IsEnabledTotalReg = cantReg <= CantRegMax,
                                IsVisibleSentinel = false, // Ruben 202311
                                //IsVisibleSentinel = true,//FuncionesBusiness.IsVisibleSentinal(idPlan),//true,
                                IsPlanPermiteSentinel = FuncionesBusiness.IsVisibleSentinal(idPlan),//false,
                                TotalKg = cantidad.ToString("n0", specificCulture),
                                Precio = cantidad > 0 ? (valRecordCitTot / cantidad).ToString("n3", specificCulture) : "0"
                            });
                        }
                    }
                    else
                    {
                        foreach (DataRow drRow in dt.Rows)
                        {
                            cantReg = Convert.ToInt64(drRow["CantReg"]);
                            valRecordCitTot = drRow.GetValue<decimal>(cifTot);
                            lista.Add(new GridRow()
                            {
                                Id = drRow.GetValue<string>("Id" + filtro),
                                Descripcion = drRow[filtro].ToString(),
                                TotalReg = cantReg.ToString("n0", specificCulture),
                                CiFoFobTot = valRecordCitTot.ToString("n0", specificCulture),
                                CiFoFobPor =
                                    (valRecordCitTot / floatValueCifTot * 100).ToString("n2", specificCulture) + "%",
                                IsEnabledTotalReg = cantReg <= CantRegMax,
                                IsVisibleSentinel = false, // Ruben 202311
                                //IsVisibleSentinel = true,//FuncionesBusiness.IsVisibleSentinal(idPlan),//true,
                                IsPlanPermiteSentinel = FuncionesBusiness.IsVisibleSentinal(idPlan)//false
                            });
                        }
                    }
                }
            }
            else if (filtro == Enums.Filtro.InfoTabla.ToString())
            {
                if(dt != null)
                foreach (DataRow drRow in dt.Rows)
                {
                    cantReg = Convert.ToInt64(drRow["cantidad"]);
                    cantidad = drRow.GetValue<decimal>("pesoneto");
                    valRecordCitTot = drRow.GetValue<decimal>(cifTot);
                    lista.Add(new GridRow()
                    {
                        Id = drRow.GetValue<string>("CodPais"),
                        Descripcion = drRow["Pais"].ToString(),
                        TotalReg = cantReg.ToString("n0", specificCulture),
                        CiFoFobTot = valRecordCitTot.ToString("n0", specificCulture),
                        CiFoFobPor =
                            (valRecordCitTot / floatValueCifTot * 100).ToString("n2", specificCulture) + "%",
                        IsEnabledTotalReg = cantReg <= CantRegMax,
                        IsVisibleSentinel = false,//FuncionesBusiness.IsVisibleSentinal(idPlan),//false,
                        TotalKg = cantidad.ToString("n0", specificCulture),
                        Precio = cantidad > 0 ? (valRecordCitTot / cantidad).ToString("n3", specificCulture) : "0"
                    });
                }
            }
            else
            {
                if (filtro == "MarcaEC") filtro = "Marca";
                if (existeTotalKgyPrecio)
                {
                    foreach (DataRow drRow in dt.Rows)
                    {
                        string codPaisFlag = (filtro == "PaisOrigen" || filtro == "PaisDestino") ? drRow.GetValue<string>("Flag") : "";

                        bool tipoServidor = Properties.Settings.Default.TableVarGeneral_InDev;
                        if (!string.IsNullOrEmpty(codPaisFlag) /*&& tipoServidor*/)
                        {
                            string codPaisx = codPais;
                            
                            string idioma = GetCurrentIdioma();
                            string nombrePais = FuncionesBusiness.BuscarPaisNombre(codPais, idioma);
                            if (codPaisx == "UE")
                            {
                                nombrePais = FuncionesBusiness.BuscarPaisUE(auxCodPais, idioma);
                                codPaisx = auxCodPais;
                            }
                            if (nombrePais == "USA")
                            {
                                nombrePais = "ESTADOS UNIDOS";
                            }
                            string codPais2 = new ListaPaises().BuscarCodPais2(codPaisFlag);
                            if (codPais == "UE")
                            {
                                codPais2 = "4UE1";
                            }
                            string tipoOpe = Session["TipoOpe"].ToString() == "I" ? "E" : "I";
                            string tabla = "", dua = "";

                            GetTablaAndDua(tipoOpe, codPaisFlag, ref tabla, ref dua);

                            string sqlAux = GeneraSqlFiltro(codPaisFlag, codPais2, tipoOpe, anioMesIni, anioMesFin, indexCboPaisB, codPaisB, dua, codPaisx, tabInfo: true);
                            int idPais = FuncionesBusiness.BuscarIdPaisNombre(nombrePais, codPaisFlag);
                            if (nombrePais == "Alemania" && codPais == "EC")
                            {
                                idPais = 53;
                            }
                            if (nombrePais == "ESTADOS UNIDOS" && codPaisFlag == "EC")
                            {
                                idPais = 158;
                            }
                            // Ruben 202310
                            else if (nombrePais == "ESTADOS UNIDOS" && codPaisFlag == "MXD")
                            {
                                idPais = 321;
                            }

                            if (idPais != 0)
                            {
                                Session["SqlInfoComplementario" + codPaisFlag] = sqlAux + (tipoOpe == "I" ? " and idPaisOrigen = " : " and idPaisDestino = ") + idPais + " ";
                            }
                            else
                            {
                                Session["SqlInfoComplementario" + codPaisFlag] = sqlAux;
                            }

                            string cifTotAux = Funciones.Incoterm(codPaisFlag, tipoOpe) + "Tot";
                            string pesoNeto1 = Funciones.CampoPeso(codPaisFlag, tipoOpe);
                            string sql = GeneraSqlAgrupado(Enums.Filtro.InfoTabla.ToString(), codPaisFlag, cifTotAux, pesoNeto1, idioma, dua, tabla, "", isInfoTabla: true);

                            DataTable dt1;
                            try
                            {
                                dt1 = Conexion.SqlDataTable(sql);
                            }
                            catch (Exception ex)
                            {                                                                                                
                                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                                dt1 = null;
                            }

                            if (dt1 == null || (int)dt1.Rows[0][2] == 0)
                            {
                                codPaisFlag = "";
                            }

                        }

                        cantReg = Convert.ToInt64(drRow["CantReg"]);
                        cantidad = drRow.GetValue<decimal>( cantidadField);
                        valRecordCitTot = drRow.GetValue<decimal>(cifTot);
                        lista.Add(new GridRow()
                        {
                            Id = drRow.GetValue<string>("Id" + filtro),
                            Descripcion = drRow[filtro].ToString(),
                            TotalReg = cantReg.ToString("n0", specificCulture),
                            CiFoFobTot = valRecordCitTot.ToString("n0", specificCulture),
                            CiFoFobPor =
                                (valRecordCitTot / floatValueCifTot * 100).ToString("n2", specificCulture) + "%",
                            IsEnabledTotalReg = cantReg <= CantRegMax,
                            IsVisibleSentinel = false,//FuncionesBusiness.IsVisibleSentinal(idPlan),//false,
                            TotalKg = cantidad.ToString("n0", specificCulture),
                            Precio = cantidad > 0 ? (valRecordCitTot / cantidad).ToString("n3", specificCulture) : "0",
                            CodPaisComplementario = codPaisFlag//(filtro == "PaisOrigen" || filtro == "PaisDestino") ? drRow.GetValue<string>("Flag") : ""
                        });
                    }
                }
                else
                {
                    


                    var IsManif = IsManifiesto(codPais);
                    foreach (DataRow drRow in dt.Rows)
                    {
                        string codPaisFlag = (filtro == "PaisOrigen" || filtro == "PaisDestino") ? drRow.GetValue<string>("Flag") : "";

                        bool tipoServidor = Properties.Settings.Default.TableVarGeneral_InDev;
                        if (!string.IsNullOrEmpty(codPaisFlag) /*&& tipoServidor*/)
                        {
                            string idioma = GetCurrentIdioma();
                            string nombrePais = FuncionesBusiness.BuscarPaisNombre(codPais, idioma);
                            string codPaisx = codPais;
                            if (codPaisx == "UE")
                            {
                                nombrePais = FuncionesBusiness.BuscarPaisUE(auxCodPais, idioma);
                                codPaisx = auxCodPais;
                            }
                            if (nombrePais == "USA")
                            {
                                nombrePais = "ESTADOS UNIDOS";
                            }
                            string codPais2 = new ListaPaises().BuscarCodPais2(codPaisFlag);
                            if (codPais == "UE")
                            {
                                codPais2 = "4UE1";
                            }
                            string tipoOpe = Session["TipoOpe"].ToString() == "I" ? "E" : "I";
                            string tabla = "", dua = "";

                            GetTablaAndDua(tipoOpe, codPaisFlag, ref tabla, ref dua);

                            string sqlAux = GeneraSqlFiltro(codPaisFlag, codPais2, tipoOpe, anioMesIni, anioMesFin, indexCboPaisB, codPaisB, dua, codPaisx, tabInfo: true);
                            int idPais = FuncionesBusiness.BuscarIdPaisNombre(nombrePais, codPaisFlag);
                            if (nombrePais == "Alemania" && codPaisFlag == "EC")
                            {
                                idPais = 53;
                            }
                            if (nombrePais == "ESTADOS UNIDOS" && codPaisFlag == "EC")
                            {
                                idPais = 158;
                            }
                            // Ruben 202310
                            else if (nombrePais == "ESTADOS UNIDOS" && codPaisFlag == "MXD")
                            {
                                idPais = 321;
                            }

                            if (idPais != 0)
                            {
                                Session["SqlInfoComplementario" + codPaisFlag] = sqlAux + (tipoOpe == "I" ? " and idPaisOrigen = " : " and idPaisDestino = ") + idPais + " ";
                            }
                            else
                            {
                                Session["SqlInfoComplementario" + codPaisFlag] = sqlAux;
                            }

                            string cifTotAux = Funciones.Incoterm(codPaisFlag, tipoOpe) + "Tot";
                            string pesoNeto1 = Funciones.CampoPeso(codPaisFlag, tipoOpe);
                            string sql = GeneraSqlAgrupado(Enums.Filtro.InfoTabla.ToString(), codPaisFlag, cifTotAux, pesoNeto1, idioma, dua, tabla, "", isInfoTabla: true);

                            DataTable dt1;
                            try
                            {
                                dt1 = Conexion.SqlDataTable(sql);
                            }
                            catch (Exception ex)
                            {                                                                                                
                                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                                dt1 = null;
                            }

                            if (dt1 == null || (int)dt1.Rows[0][2] == 0)
                            {
                                codPaisFlag = "";
                            }

                        }

                        cantReg = Convert.ToInt64(drRow["CantReg"]);
                        valRecordCitTot = drRow.GetValue<decimal>(cifTot);
                        lista.Add(new GridRow()
                        {
                            Id = drRow.GetValue<string>((filtro!="Manifiesto" ? "Id" : "") + filtro),
                            Descripcion = drRow[filtro].ToString(),
                            TotalReg = cantReg.ToString("n0", specificCulture),
                            CiFoFobTot = valRecordCitTot.ToString("n" + (!IsManif ? "0" : "1"), specificCulture),
                            CiFoFobPor = (floatValueCifTot > 0 ? (valRecordCitTot / floatValueCifTot * 100) : 0).ToString("n" + (!IsManif ? "2" : "1"), specificCulture) + "%",

                            IsEnabledTotalReg = cantReg <= CantRegMax,
                            IsVisibleSentinel = false,//FuncionesBusiness.IsVisibleSentinal(idPlan)//false
                            CodPaisComplementario = codPaisFlag//(filtro == "PaisOrigen" || filtro == "PaisDestino") ? drRow.GetValue<string>("Flag") : ""
                        });
                    }
                }
            }
            return lista;
        }

        private List<GridRow> GetDataTableToListCbo(DataTable dt, string filtro)
        {
            var lista = new List<GridRow>();
            if (filtro == "MarcaEC") filtro = "Marca";
            if (filtro == Enums.Filtro.InfoTabla.ToString())
            {
                if (dt != null)
                    foreach (DataRow dtRow in dt.Rows)
                    {
                        lista.Add(new GridRow()
                        {
                            Id = dtRow.GetValue<string>("IdVariable"),
                            Descripcion = dtRow["Descripcion"].ToString()
                        });
                    }
            }
            else {
                if (dt != null)
                    foreach (DataRow dtRow in dt.Rows)
                    {
                        lista.Add(new GridRow()
                        {
                            Id = dtRow.GetValue<string>((filtro != "Manifiesto" ? "Id" : "") + filtro),
                            Descripcion = dtRow[filtro].ToString()
                        });
                    }
            }
                    

            return lista;
        }

        private List<object> GetJsonDataPie(string filtro, DataTable dt, string cifTot,
            string valueCifTot, CultureInfo specificCulture)
        {
            var json = new List<object>();
            var numSectoresCirculares = 5;
            float sumCif = 0;
            string auxIdSlice = filtro == "Nandina" ? Enums.Filtro.Partida.ToString() : filtro;
            if (dt != null && dt.Rows.Count > 0)
            {
                if (filtro == "MarcaEC") auxIdSlice=filtro = "Marca";
                float dValueCifTot = Convert.ToSingle(valueCifTot);
                string name = "";
                if (dt.Rows.Count <= numSectoresCirculares)
                {
                    if (filtro == Enums.Filtro.InfoTabla.ToString())
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            name = dr["Pais"].ToString().Trim();
                            if (dr["Pais"].ToString().Trim().Length > 35)
                            {
                                name = dr["Pais"].ToString().Trim().Substring(0, 35) + "...";
                            }


                            var s = Math.Round(Convert.ToDouble(dr.GetValue<Single>(cifTot) / dValueCifTot * 100), 1);
                            json.Add(new
                            {
                                name = name,
                                y = Math.Round(Convert.ToDouble(dr.GetValue<Single>(cifTot) / dValueCifTot * 100), 1),
                                id = dr.GetValue<string>("IdPais"),
                                custom = dr["Pais"].ToString(),
                                cifTot = dr.GetValue<Single>(cifTot)
                            });
                        }
                    }
                    else
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            name = dr[filtro].ToString().Trim();
                            if (dr[filtro].ToString().Trim().Length > 35)
                            {
                                name = dr[filtro].ToString().Trim().Substring(0, 35) + "...";
                            }
                            var s = dValueCifTot > 0 ? Math.Round(Convert.ToDouble(dr.GetValue<Single>(cifTot) / dValueCifTot * 100), 1) : 0;
                            json.Add(new
                            {
                                name = name,
                                y = dValueCifTot > 0 ?  Math.Round(Convert.ToDouble(dr.GetValue<Single>(cifTot) / dValueCifTot * 100), 1) : 0,
                                id = dr.GetValue<string>((filtro != "Manifiesto" ? "Id" : "") + auxIdSlice),
                                custom = dr[filtro].ToString(),
                                cifTot = dr.GetValue<Single>(cifTot)
                            });
                        }
                    }
                        
                }
                else
                {
                    
                    var dtAux = dt.AsEnumerable().Take(numSectoresCirculares);
                    double sumPercentage = 0;
                    // var filtroAux = filtro;
                    
                    if (filtro == Enums.Filtro.InfoTabla.ToString())
                    {
                       
                        foreach (DataRow dr in dtAux)
                        {
                            var percentage = Math.Round(Convert.ToDouble(dr.GetValue<Single>(cifTot) / dValueCifTot * 100), 1);
                            sumPercentage += percentage;
                            sumCif += dr.GetValue<Single>(cifTot);
                            if (dr["Pais"].ToString().Trim().Length > 30)
                            {
                                name = dr["Pais"].ToString().Trim().Substring(0, 30) + "...";
                            }
                            else
                            {
                                name = dr["Pais"].ToString().Trim();
                            }

                            json.Add(new
                            {
                                name = name,
                                y = percentage,
                                id = dr.GetValue<string>("IdPais"),
                                custom = dr["Pais"].ToString(),
                                cifTot = dr.GetValue<Single>(cifTot)
                            });
                        }
                    }
                    else
                    {
                        foreach (DataRow dr in dtAux)
                        {
                            
                            var percentage = dValueCifTot > 0 ? Math.Round(Convert.ToDouble(dr.GetValue<Single>(cifTot) / dValueCifTot * 100), 1) : 0;
                            sumPercentage += percentage;

                            if (dr[filtro].ToString().Trim().Length > 30)
                            {
                                name = dr[filtro].ToString().Trim().Substring(0, 30) + "...";
                            }
                            else
                            {
                                name = dr[filtro].ToString().Trim();
                            }

                            json.Add(new
                            {
                                name = name,
                                y = percentage,
                                id = dr.GetValue<string>((filtro != "Manifiesto" ? "Id" : "") + auxIdSlice),
                                custom = dr[filtro].ToString(),
                                cifTot = dr.GetValue<Single>(cifTot)
                            });
                        }
                    }

                    

                    json.Add(new
                    {
                        name = "[" + Resources.Resources.Others_Text.ToUpper() + "]",
                        y = 100 - sumPercentage,
                        id = "",
                        custom = "[" + Resources.Resources.Others_Text.ToUpper() + "]",
                        cifTot = dValueCifTot - sumCif
                    });
                }
            }
            return json;
        }


        private Chart GetJsonDataColumn(string filtro, DataTable dt, string cifTot,
            string valueCifTot, CultureInfo specificCulture)
        {
            List<string> categories = new List<string>();
            List<ChartColumnPP> lista = new List<ChartColumnPP>();
            var json = new List<object>();
            var numSectoresCirculares = 5;
            double sumCif = 0;
            if (dt != null && dt.Rows.Count > 0)
            {
                double dValueCifTot = Convert.ToDouble(valueCifTot);
                string name = "";
                if (dt.Rows.Count <= numSectoresCirculares)
                {
                    if (filtro == Enums.Filtro.InfoTabla.ToString())
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            /*name = dr["Pais"].ToString().Trim();
                            if (dr["Pais"].ToString().Trim().Length > 35)
                            {
                                name = dr["Pais"].ToString().Trim().Substring(0, 35) + "...";
                            }

                            
                            var s = Math.Round(Convert.ToDouble(dr.GetValue<Single>(cifTot) / dValueCifTot * 100), 1);
                            json.Add(new
                            {
                                name = name,
                                y = Math.Round(Convert.ToDouble(dr.GetValue<Single>(cifTot) / dValueCifTot * 100), 1),
                                id = dr.GetValue<string>("IdPais"),
                                custom = dr["Pais"].ToString(),
                                cifTot = dr.GetValue<Single>(cifTot)
                            });*/
                            //lista.Add(Math.Round(dr.GetValue<decimal>(cifTot), 3));
                            lista.Add(new ChartColumnPP()
                            {
                                y = Math.Round(dr.GetValue<decimal>(cifTot), 3),
                                id = dr.GetValue<string>("CodPais")
                            });
                            categories.Add(dr["Pais"].ToString());
                            
                        }
                    }

                }
                else
                {
                    var dtAux = dt.AsEnumerable().Take(numSectoresCirculares);
                    double sumPercentage = 0;
                    // var filtroAux = filtro;

                    if (filtro == Enums.Filtro.InfoTabla.ToString())
                    {
                        foreach (DataRow dr in dtAux)
                        {
                            sumCif += dr.GetValue<Double>(cifTot);
                           
                            /*var percentage = Math.Round(Convert.ToDouble(dr.GetValue<Single>(cifTot) / dValueCifTot * 100), 1);
                            sumPercentage += percentage;
                            sumCif += dr.GetValue<Single>(cifTot);
                            if (dr["Pais"].ToString().Trim().Length > 30)
                            {
                                name = dr["Pais"].ToString().Trim().Substring(0, 30) + "...";
                            }
                            else
                            {
                                name = dr["Pais"].ToString().Trim();
                            }

                            json.Add(new
                            {
                                name = name,
                                y = percentage,
                                id = dr.GetValue<string>("IdPais"),
                                custom = dr["Pais"].ToString(),
                                cifTot = dr.GetValue<Single>(cifTot)
                            });*/
                            lista.Add(new ChartColumnPP (){
                               y= Math.Round(dr.GetValue<decimal>(cifTot), 3),
                               id = dr.GetValue<string>("CodPais")
                            });
                            categories.Add(dr["Pais"].ToString());
                        }
                    }
                    categories.Add("[" + Resources.Resources.Others_Text.ToUpper() + "]");
                    lista.Add(new ChartColumnPP()
                    {
                        y = Math.Round((decimal)(dValueCifTot - sumCif), 2),
                        id = ""
                    });
                    //lista.Add(Math.Round((decimal)(dValueCifTot - sumCif), 2));
                    
                    /*json.Add(new
                    {
                        name = "[" + Resources.Resources.Others_Text.ToUpper() + "]",
                        y = 100 - sumPercentage,
                        id = "",
                        custom = "[" + Resources.Resources.Others_Text.ToUpper() + "]",
                        cifTot = dValueCifTot - sumCif
                    });*/

                }
            }
            var objChart = new Chart { Categories = categories };
            objChart.Column.Add(new ChartColumn
            {
                data = lista
            });
            return objChart;
        }

        private List<GridRow> GetListGridRowMarcasModelos(DataTable dt, string filtro, decimal valueCifTot,
            CultureInfo specificCulture)
        {
            var lista = new List<GridRow>();
            Int64 cantReg = 0;
            decimal valRecordCitTot;
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    cantReg = Convert.ToInt64(dr["CantReg"]);
                    valRecordCitTot = Convert.ToDecimal(dr["FOBTot"]);
                    lista.Add(new GridRow()
                    {
                        Id = dr["Id" + filtro].ToString(),
                        Descripcion = dr[filtro].ToString(),
                        TotalReg = cantReg.ToString("n0", specificCulture),
                        CiFoFobTot = valRecordCitTot.ToString("n0", specificCulture),
                        CiFoFobPor =
                            (valRecordCitTot / valueCifTot * 100).ToString("n2", specificCulture) + "%",
                        IsEnabledTotalReg = cantReg <= CantRegMax,
                        IsVisibleSentinel = false//FuncionesBusiness.IsVisibleSentinal(Funciones.ObtieneIdPlan(Session["IdUsuario"].ToString()))//false
                    });
                }
            } 
            
            return lista;
        }



        private List<GridRow> GetListGridRowInfoComplementario(DataTable dt, string filtro, decimal valueCifTot,
            CultureInfo specificCulture)
        {
            List<GridRow> Grid = new List<GridRow>();
            Int64 cantReg = 0;
            decimal valRecordCitTot;
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    cantReg = Convert.ToInt64(dr["Cantidad"]);
                    valRecordCitTot = Convert.ToDecimal(dr["ciftot"]);
                    Grid.Add( new GridRow()
                    {
                        Id = dr["CodPais"].ToString(),
                        Descripcion = dr["Pais"].ToString(),
                        TotalReg = cantReg.ToString("n0", specificCulture),
                        CiFoFobTot = valRecordCitTot.ToString("n0", specificCulture),
                        CiFoFobPor =
                            (valRecordCitTot / valueCifTot * 100).ToString("n2", specificCulture) + "%",
                        IsEnabledTotalReg = cantReg <= CantRegMax,
                        IsVisibleSentinel = false//FuncionesBusiness.IsVisibleSentinal(Funciones.ObtieneIdPlan(Session["IdUsuario"].ToString()))//false
                    });
                }
            }

            return Grid;
        }

        private List<GridRow> GetListGridRowDetalleExcel(DataTable dt, string filtro,
            CultureInfo specificCulture, string DUA, string codPais, string txtPais, string tipoOpe, string idioma, string cif, string campoPeso,
            /*FlagVarVisibles */ TabMisBusquedas  vFlag)
        {
            var lista = new List<GridRow>();
            Int64 i = 0;
            string auxCif = cif.Replace("Tot", "");
            if (IsManifiesto(codPais))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    var obj = new GridRow();
                    obj.Id = dr[0].ToString(); //id
                    obj.Nro = Convert.ToInt64(dr["Nro"]);
                    obj.Fecha = Convert.ToDateTime(dr["Fechanum_date"])
                        .ToString(idioma == "es" ? "dd/MM/yyyy" : "MM/dd/yyyy");                    
                    obj.ImportadorOrExportador = dr.GetValue<string>(tipoOpe == "I" && vFlag.ExisteImportador ? "Importador" : vFlag.ExisteExportador ?  "Exportador" : "__");                    
                    obj.Notificado = dr.GetValue<string>("Notificado");
                    obj.ExportadorOrImportador = dr.GetValue<string>(tipoOpe == "I" && vFlag.ExisteProveedor ? "Proveedor" : vFlag.ExisteImportadorExp ?   "ImportadorExp" : "__" );
                    obj.PaisEmbarqueOrPaisDest = dr.GetValue<string>(tipoOpe == "I" ? "PaisEmbarque" :   "PaisDestino" );
                    obj.Puerto = dr.GetValue<string>(vFlag.ExistePtoDescarga ? "PtoDescarga" :
                        vFlag.ExistePtoEmbarque ? "PtoEmbarque" : "PtoDestino");
                    obj.PesoBruto = dr.GetValue<decimal>("PesoBruto").ToString("n0", specificCulture);
                    obj.CantidadOrBultos = dr
                        .GetValue<decimal>(vFlag.CodPais == "USI" || vFlag.CodPais == "USE" ? "Cantidad" : "Bultos")
                        .ToString("n" + (vFlag.CodPais == "USI" || vFlag.CodPais == "USE" ? "2" : "0"),
                            specificCulture);
                    obj.UndMedida = dr.GetValue<string>("Unidad");
                    obj.Descripcion = dr.GetValue<string>("DesComercial");
                    obj.DesAdicional = dr.GetValue<string>("MarcasContenedor");

                    obj.LnkGoogle = "http://www.google.com/search?q=\"" +
                                    obj.ImportadorOrExportador?.Trim().Replace("&", "") +
                                    "\" "+ (
                                        (codPais == "BRI" || codPais == "BRE") ?"BRASIL":
                                         (codPais == "ECI" || codPais == "ECE") ? "ECUADOR" :
                                            "\"USA\"");
                    obj.LnkGoogle2 = "http://www.google.com/search?q=\"" +  obj.ExportadorOrImportador?.Trim()?.Replace("&", "") +
                                     "\" \"" + obj.PaisEmbarqueOrPaisDest?.Trim()?.ToUpper() + "\"";
                    lista.Add(obj);
                }

                goto ReturnMe;
            }

            try
            {
                foreach (DataRow dr in dt.Rows)
                {
                    var sIdImportador = dr.GetValue<string>(tipoOpe == "I" ? "IdImportador" : "IdExportador");
                    var sImportador = dr.GetValue<string>(tipoOpe == "I" ? "Importador" : "Exportador");
                    var sProveedor = dr.GetValue<string>(tipoOpe == "I" ? "Proveedor" : "ImportadorExp");
                    var sPaisOrigen = dr.GetValue<string>(tipoOpe == "I" ? "PaisOrigen" : "PaisDestino");

                    var obj = new GridRow();
                    obj.Id = dr[0].ToString(); //id
                    obj.Nro = Convert.ToInt64(dr["Nro"]);
                    obj.Fecha = Convert.ToDateTime(dr["Fechanum_date"]).ToString(idioma == "es" ? "dd/MM/yyyy" : "MM/dd/yyyy");
                    obj.PartidaAduanera = dr.GetValue<string>("Nandina");
                    obj.IdImportador2 = sIdImportador;
                    obj.Importador = sImportador;
                    obj.Exportador = sProveedor;
                    obj.TotalKg = dr.GetValue<decimal>(campoPeso).ToString("n2", specificCulture);
                    obj.Cantidad = dr.GetValue<decimal>("Cantidad").ToString("n2", specificCulture);
                    obj.UndMedida = dr.GetValue<string>("Unidad");
                    obj.FobUnit = dr.GetValue<decimal>(codPais != "US" ? "FOBUnit" : "FASUnit").ToString("n2", specificCulture);
                    obj.CifUnit = dr.GetValue<decimal>(auxCif + "Unit").ToString("n2", specificCulture);
                    obj.CifUnitImp = dr.GetValue<decimal>("CIFImptoUnit").ToString("n2", specificCulture);
                    obj.Dua = dr.GetValue<string>(DUA);
                    obj.PaisOrigen = sPaisOrigen;
                    obj.Descripcion = dr.GetValue<string>("DesComercial");
                    //IsEnabledTotalReg = cantReg <= CantRegMax,

                    obj.IsVisibleSentinel = false; // Ruben 202311
                    //obj.IsVisibleSentinel = (codPais == "PE" || codPais == "PEB");//FuncionesBusiness.IsVisibleSentinal(Funciones.ObtieneIdPlan(Session["IdUsuario"].ToString())) ;//(codPais == "PE" || codPais == "PEB");
                    
                    // Ruben 202311
                    if (codPais == "PE" || codPais == "PEB")
                        obj.IsPlanPermiteSentinel = FuncionesBusiness.IsVisibleSentinal(Funciones.ObtieneIdPlan(Session["IdUsuario"].ToString()));
                    else
                        obj.IsPlanPermiteSentinel = FuncionesBusiness.IsVisibleSentinal2023(Funciones.ObtieneIdPlan(Session["IdUsuario"].ToString()));
                    //obj.IsPlanPermiteSentinel = FuncionesBusiness.IsVisibleSentinal(Funciones.ObtieneIdPlan(Session["IdUsuario"].ToString())); //(new String[] { "BUSINESS", "PREMIUM", "UNIVERSIDADES" }.Contains(Session["Plan"].ToString()));

                    obj.Ruc = dr.GetValue<string>("RUC");
                    string nomPais = GetValidarPais(codPais);
                    obj.LnkGoogle = "http://www.google.com/search?q=\"" + sImportador?.Trim().Replace("&", "") +
                                                "\" \"" +
                                                (codPais != "PEB" ? (nomPais.Equals("") ? txtPais : nomPais).Trim().ToUpper() : "PERÚ") + "\"";
                    obj.LnkGoogle2 = "http://www.google.com/search?q=\"" + sProveedor?.Trim().Replace("&", "") +
                                                "\" \"" + sPaisOrigen?.Trim().ToUpper() + "\"";


                    obj.IsVisibleLupa = (codPais != "US" && codPais != "CN" && codPais != "UE");
                    obj.IsVisibleLupaPartida = (codPais == "PE");

                    obj.Distrito = dr.GetValue<string>("Distrito");
                    if(vFlag.ExisteMarcaEC)
                        obj.Marca = dr.GetValue<string>("Marca");
                    lista.Add(obj);
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                throw new Exception(ex.InnerException.Message);
            }
ReturnMe:
            return lista;
        }

        private string GetValidarPais(string codPais)
        {
            switch (codPais)
            {
                case "MXD":
                    return "MÉXICO";
                case "PE_":
                case "PEP":
                    return "PERU";
                case "US_":
                    return "USA";

            }
            return "";
        }

        private List<GridRow> GetListGridRowAduanaDua(DataTable dt, string filtro, string cifTot, CultureInfo specificCulture)
        {
            var lista = new List<GridRow>();
            Int64 cantReg = 0;
            if (filtro == Enums.Filtro.AduanaDUA.ToString())
            {
                foreach (DataRow dr in dt.Rows)
                {
                    cantReg = Convert.ToInt64(dr["CantReg"]);
                    lista.Add(new GridRow()
                    {
                        Id = dr["IdAduana"].ToString() + "-" + dr["DUA"].ToString(),
                        Descripcion = dr["Aduana"].ToString(),
                        Dua = dr["DUA"].ToString(),
                        TotalReg = cantReg.ToString("n0", specificCulture),
                        CiFoFobTot = dr.GetValue<decimal>(cifTot).ToString("n0", specificCulture),
                        IsEnabledTotalReg = cantReg <= CantRegMax
                    });
                }
            }
            else
            {
                foreach (DataRow dr in dt.Rows)
                {
                    cantReg = Convert.ToInt64(dr["CantReg"]);
                    lista.Add(new GridRow()
                    {
                        Id = dr["IdAduana"].ToString(),
                        Descripcion = dr["Aduana"].ToString(),
                        Dua = "",
                        TotalReg = cantReg.ToString("n0", specificCulture),
                        CiFoFobTot = dr.GetValue<decimal>(cifTot).ToString("n0", specificCulture),
                        IsEnabledTotalReg = cantReg <= CantRegMax
                    });
                }
            }
            return lista;
        }
       

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult CboFiltroSelectedIndexChanged(string filtro, string valorFiltro, string tipoOpe,
            string codPais, string codPais2, int indexCboPaisB,
            string codPaisB, string anioMesIni, string anioMesFin,
            string idioma)
        {
            string auxCodPais = codPais;
            ValidaCodPais2(codPais2, ref codPais);
            ValidaCodPaisManif(ref codPais, tipoOpe);

            var tipo = "";

            var nombre = "";
            var nuevoFiltro = true;

            var nuevosFiltros = new List<object>();

            if (filtro == Enums.Filtro.AduanaDUA.ToString())
                filtro = Enums.Filtro.Aduana.ToString();

            Session["hdfFiltroSel"] = filtro;
            Session["hdfIDSel"] = valorFiltro;
            nombre = AgregaFiltro(idioma, tipoOpe, ref tipo);
            if (nombre == "")
                nuevoFiltro = false;

            var specificCulture = GetSpecificCulture();

            List<object> listGridData = null;
            int cantReg = 0;

            bool FlagPalabras = (!string.IsNullOrWhiteSpace(Session["hdfPalabrasY"]?.ToString()));
            //bool hideTabExcel = cantReg > CantRegMax && !FlagPalabras;
            bool hideTabExcel = Funciones.VisualizarExcel();
            if (nuevoFiltro)
            {
                var auxValue = tipo + valorFiltro;
                nuevosFiltros.Add(new
                {
                    text = nombre,
                    value = auxValue
                });

                ActualizarSessionListaFitros(auxValue, nombre);

                var tabla = "";
                var dua = "";
                GetTablaAndDua(tipoOpe, codPais, ref tabla, ref dua);


                Session["SqlFiltro"] = GeneraSqlFiltro(codPais, codPais2, tipoOpe, anioMesIni, anioMesFin,
                    indexCboPaisB, codPaisB, dua, auxCodPais);
                //Extensiones.SetCookie("sl", Session["sqlFiltro"].ToString() + " $ " + Session["CodPais"].ToString() + " $ " + Session["TipoOpe"].ToString(), TimeSpan.FromDays(360));
                Extensiones.SetCookie("sl", "", TimeSpan.FromDays(360));
                string cifTot = "";
                string pesoNeto = "";
                string valueCifTot = "";
                decimal valuePesoNeto = 0;
                string unidad = "";
                GetCifTot_PesoNeto_CantReg_ValueCifTot_Unidad_ValuePesoNeto(codPais, tipoOpe, tabla, ref cifTot,
                    ref pesoNeto,
                    ref cantReg, ref valueCifTot, ref unidad, ref valuePesoNeto);

                
                listGridData = Busca(codPais, tipoOpe, cifTot, pesoNeto, idioma, dua, tabla, indexCboPaisB, valueCifTot,
                    valuePesoNeto, unidad,
                    cantReg, specificCulture, hideTabExcel);
            }

            



            return Json(new
            {
                nuevoFiltro,
                nuevosFiltros,
                totalRecordsFound = GetDescriptionRecordsFound(cantReg, specificCulture,idioma),
                gridData = listGridData,
                hideTabExcel = hideTabExcel,
                FlagRegMax = cantReg > CantRegMax
        });
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult VerDetalleExcelById(string tipoOpe, string codPais, string id)
        {
            ValidaCodPaisManif(ref codPais, tipoOpe);
            var isManif = IsManifiesto(codPais);
            string idioma = GetCurrentIdioma();
            String sql = "";
            String Tabla = (tipoOpe == "I" ? "Importacion" : "Exportacion") + "_" + codPais;
            if (!isManif)
            {
                if (tipoOpe == "I")
                {
                    sql = "select IdImportacion as Id, T.*, null as Exportador, null as ImportadorExp ";
                    sql += "from V_" + Tabla + " T where IdImportacion = @id";
                }
                else
                {
                    sql = "select IdExportacion as Id, T.*, null as Importador, null as Proveedor ";
                    sql += "from V_" + Tabla + " T where IdExportacion = @id";
                }
            }
            else
            {
                if (tipoOpe == "I")
                    sql = "select * from " + Tabla + " where IdImportacion = @id " ;
                else
                    sql = "select * from " + Tabla + " where IdExportacion = @id " ;
            }

            using (var db = new ConexProvider().Open)
            {
                string[] thead1, thead2;
                dynamic[] tbody1, tbody2;
                var resume = new List<GridHead>();
                //var result = db.Query(sql, new { id = id }).ToList();
                dynamic v = db.Query(sql, new { id = id }).FirstOrDefault();

                //if (result.Count > 0)
                if (v != null)
                {
                    var vFlag = new TabMisBusquedas(tipoOpe, codPais);   //new FlagVarVisibles(codPais, tipoOpe, isManif);
                    ValidExistData(codPais, tipoOpe);
                    //var v = (from x in result
                    //         select x).First();

                    var isEs = idioma == "es";
                    var cultureInfo = new System.Globalization.CultureInfo("en-US");
                    if (!isManif)
                    {
                        resume.Add(new GridHead(isEs ? "Partida Aduanera" : "HTS Code", v.Nandina));
                        resume.Add(new GridHead((isEs ? "Descripción Partida Aduanera" : "HTS Code Description"), (isEs ? v.Partida : v.Partida_en)));
                        resume.Add(new GridHead(isEs ? "Aduana" : "Customs", v.Aduana));
                        if(v.FechaNum_Date != null)
                            resume.Add(new GridHead(isEs ? "Fecha " + (tipoOpe == "I" ? "Importación " : "Exportación") + " (dd/mm/yyyy)" :
                                "Date " + (tipoOpe == "I" ? "Import" : "Export") + " (mm/dd/yyyy)",
                            isEs ? v.FechaNum_Date?.ToString("dd/MM/yyyy") :
                                v.FechaNum_Date?.ToString("MM/dd/yyyy")));
                        else
                        {
                            resume.Add(new GridHead(isEs ? "Fecha " + (tipoOpe == "I" ? "Importación " : "Exportación") + " (dd/mm/yyyy)" :
                                    "Date " + (tipoOpe == "I" ? "Import" : "Export") + " (mm/dd/yyyy)",
                                isEs ? v.fechanum_date?.ToString("dd/MM/yyyy") :
                                    v.fechanum_date?.ToString("MM/dd/yyyy")));
                        }
                        resume.Add(new GridHead(isEs ? "Fecha Llegada (dd/mm/yyyy)" : "Arrival date (mm/dd/yyyy)",
                            isEs ? v.FechaLLegada_Date?.ToString("dd/MM/yyyy") :
                                v.FechaLLegada_Date?.ToString("MM/dd/yyyy"),
                            (codPais == "PE" || codPais == "PEB") && tipoOpe == "I"));
                        resume.Add(new GridHead(isEs ? "Descripción Comercial" : "Commercial description", v.DesComercial, vFlag.ExisteDesComercial));
                        if (tipoOpe == "I")
                        {
                            resume.Add(new GridHead(isEs ? "Importador" : "Importer", v.Importador, vFlag.ExisteImportador));
                            resume.Add(new GridHead((codPais != "CL") ? (isEs ? "Exportador" : "Exporter") : (isEs ? "Marca" : "Brand"), v.Proveedor, vFlag.ExisteProveedor));
                        }
                        else
                        {
                            resume.Add(new GridHead(isEs ? "Importador" : "Importer", v.ImportadorExp, vFlag.ExisteImportador));
                            resume.Add(new GridHead((codPais != "CL") ? (isEs ? "Exportador" : "Exporter") : (isEs ? "Marca" : "Brand"), v.Exportador, vFlag.ExisteProveedor));
                        }                        
                        
                        //resume.Add(new GridHead(isEs ? "Exportador" : "Exporter", v.ImportadorExp, vFlag.ExisteExportador));
                        //resume.Add(new GridHead(isEs ? "Importador" : "Importer", v.ImportadorExp, vFlag.ExisteImportadorExp));
                        
                        if (tipoOpe == "I")
                        {
                            if (codPais != "PEB")
                            {
                                thead1 = new[]
                                {
                                        (isEs?"Peso Bruto kg":"Gross kg"), (isEs?"Peso Neto kg":"Net kg"), (isEs?"Cantidad":"Quantity"), (isEs?"Unid.":"Unit"), (isEs?"País Origen":"Origin Country"), (isEs?"País Compra":"Adquisition Country"),
                                        (isEs?"Puerto Embarque":"Loading Port"), (isEs?"Vía":"Via"), (isEs?"Transporte":"Transport"), (isEs?"Agente Aduanas":"Customs Agent")
                                };
                                thead2 = new[]
                                {
                                        (isEs?"FOB US$ Total":"FOB US$ Total"), (isEs?"FOB US$ Unit.":"FOB US$ Unit"), (isEs?"Flete US$ Total":"Freight US$ Total"), (isEs?"Flete US$ Unit.":"Freight US$ Unit"), (isEs?"Seguro US$ Total":"Insurance US$ Total"),
                                        (isEs?"Seguro US$ Unit.":"Insurance US$ Unit"), (isEs?"CIF US$ Total":"CIF US$ Total"), (isEs?"CIF US$ Unit.":"CIF US$ Unit"), (isEs?"CIF + Impuestos (IGV no inc.) US$ Unit.":"CIF + Taxes US$ Unit")
                                };

                                tbody1 = new[]
                                {
                                    v?.PesoBruto?.ToString("n2", cultureInfo), v?.PesoNeto?.ToString("n2", cultureInfo), v?.Cantidad?.ToString("n2", cultureInfo),
                                    v.Unidad, v.PaisOrigen, v.PaisProced, v.PtoEmbarque, v.ViaTransp, v.Transporte, v.Agente
                                };

                                
                                var n4 = v?.FleteTot;
                               
                                if( n4!= null)
                                {
                                    if (char.IsNumber(n4 + "", 0))
                                    {
                                        n4 = v?.FleteTot?.ToString("n2", cultureInfo);
                                    }
                                    else
                                    {
                                        n4 = null;
                                    }
                                }
                                //tbody2 = new[]
                                //{
                                //    v?.FOBTot?.ToString("n2", cultureInfo), v?.FOBUnit?.ToString("n3", cultureInfo), v?.FleteTot?.ToString("n2", cultureInfo),
                                //    v?.FleteUnit?.ToString("n3", cultureInfo), v?.SeguroTot?.ToString("n2", cultureInfo),v?.SeguroUnit?.ToString("n3", cultureInfo),v?.CIFTot?.ToString("n2", cultureInfo)
                                //    ,v?.CIFUnit?.ToString("n3", cultureInfo),v?.CIFImptoUnit?.ToString("n3", cultureInfo)
                                //};

                                tbody2 = new[]
                                {
                                    v?.FOBTot?.ToString("n2", cultureInfo), v?.FOBUnit?.ToString("n3", cultureInfo), n4,
                                    v?.FleteUnit?.ToString("n3", cultureInfo), v?.SeguroTot?.ToString("n2", cultureInfo),v?.SeguroUnit?.ToString("n3", cultureInfo),v?.CIFTot?.ToString("n2", cultureInfo)
                                    ,v?.CIFUnit?.ToString("n3", cultureInfo),v?.CIFImptoUnit?.ToString("n3", cultureInfo)
                                };
                            }
                            else
                            {
                                thead1 = new[]
                                {
                                        "Cantidad", "Unid.", "FOB US$ Total", "FOB US$ Unit.", "País Origen", "País Adquisición",
                                        "Puerto Carga", "Vía"
                                };
                                thead2 = new[]
                                {
                                        "Nombre Comercial", "Característica", "Marca", "Modelo"
                                };

                                tbody1 = new[]
                                {
                                    v?.Cantidad?.ToString("n2",cultureInfo),
                                    v.Unidad, v?.FOBTot?.ToString("n2",cultureInfo), v?.FOBUnit?.ToString("n3",cultureInfo), v.PaisOrigen, v.PaisProced, v.PtoEmbarque, v.ViaTransp
                                };

                                tbody2 = new[]
                                {
                                    v?.NomComercial, v?.Caracteristica, v?.Marca,
                                    v?.Modelo
                                };

                            }
                        }
                        else
                        {
                            thead1 = new[]
                            {
                                    "Peso Bruto kg", "Peso Neto kg", "Cantidad", "Unid.", "FOB US$ Total", "FOB US$ Unit"
                            };
                            thead2 = new[]
                            {
                                    "País Destino", "Puerto Destino", "Puerto Carga", "Vía", "Transporte",
                                    "Agente Aduanas"
                            };

                            tbody1 = new[]
                            {
                                v?.PesoBruto?.ToString("n2",cultureInfo), v?.PesoNeto?.ToString("n2",cultureInfo), v?.Cantidad?.ToString("n2",cultureInfo),
                                v?.Unidad,
                                v?.FOBTot?.ToString("n2",cultureInfo), v?.FOBUnit?.ToString("n3",cultureInfo)
                            };

                            tbody2 = new[]
                            {
                                v?.PaisDestino, v?.PtoDestino, v?.PtoEmbarque, v?.ViaTransp, v?.Transporte
                                , v?.Agente
                            };
                        }
                    }
                    else
                    {
                        if (codPais == "USI")
                        {
                            resume.Add(new GridHead(isEs ? "Fecha Importación (dd/mm/yyyy)" : "Import date (mm/dd/yyyy)", isEs ? v.fechanum_date?.ToString("dd/MM/yyyy") :
                                v.fechanum_date?.ToString("MM/dd/yyyy")));
                            resume.Add(new GridHead(isEs ? "Descripción Comercial" : "Commercial description",
                                v.DesComercial));
                            resume.Add(new GridHead(isEs ? "Descripción Adicional" : "Additional description",
                                v.MarcasContenedor));
                            resume.Add(new GridHead(isEs ? "Importador / Consignatario" : "Importer / Consignee",
                                v.Importador, label2: v.TrueBuyer == "YES" ?  (isEs ? "[Comprador real]" : "[True buyer]") : "", value2: v.ImportadorDireccion, hasVal2: true));
                            resume.Add(new GridHead(isEs ? "Tramitador / Notificado" : "Processor / Notified",
                                v.Notificado, label2: "&nbsp;", value2: v.NotificadoDireccion, hasVal2: true));
                            resume.Add(new GridHead(isEs ? "Exportador / Embarcador" : "Exporter / Shipper",
                                v.Proveedor, label2: v.TrueBuyer == "YES" ? (isEs ?  "Proveedor real" : "True supplier") : "", value2: v.ProveedorDireccion, hasVal2: true));

                            thead1 = new[]
                            {
                                (isEs?"Peso Bruto kg":"Gross kg"), (isEs?"Cantidad":"Quantity"), (isEs?"Unid.":"Unit"), (isEs?"País Embarque":"Shipment Country"), (isEs?"Ciudad Embarque":"Shippment City"),
                                (isEs?"Puerto Embarque":"Shipment Port"), (isEs?"Puerto Descarga":"Unloading Port") };
                            thead2 = null;

                            tbody1 = new[]
                            {
                                v?.PesoBruto?.ToString("n0",cultureInfo), v?.Cantidad?.ToString("n2",cultureInfo),
                                v.Unidad, v.PaisEmbarque, v.CiudadEmbarque, v.PtoEmbarque, v.PtoDescarga
                            };

                            tbody2 = null;

                        }
                        else if (codPais == "USE") {
                            resume.Add(new GridHead(isEs ? "Fecha Importación (dd/mm/yyyy)" : "Import date (mm/dd/yyyy)", isEs ? v.fechanum_date?.ToString("dd/MM/yyyy") :
                                v.fechanum_date?.ToString("MM/dd/yyyy")));
                            resume.Add(new GridHead(isEs ? "Descripción Comercial" : "Commercial description",
                                v.DesComercial));
                            resume.Add(new GridHead(isEs ? "Exportador / Embarcador" : "Exporter / Shipper",
                                v.Exportador, label2: isEs ? "Proveedor real" : "True supplier", value2: v.ExportadorDireccion, hasVal2: true));

                            thead1 = new[]
                            {
                                (isEs?"Peso Bruto kg":"Gross kg"), (isEs?"Cantidad":"Quantity"), (isEs?"Unid.":"Unit"), (isEs?"País Destino":"Destination Country"), (isEs?"Puerto Descarga":"Unloading Port"), (isEs?"Puerto Embarque":"Shipment Port"), (isEs?"Naviera":"Shipping company"), (isEs?"Nave":"Ship") };
                            thead2 = null;

                            tbody1 = new[]
                            {
                                v?.PesoBruto?.ToString("n0",cultureInfo), v?.Cantidad?.ToString("n2",cultureInfo),
                                v.Unidad, v.PaisDestino, v.PtoDescarga, v.PtoEmbarque, v.Transporte, v.Nave
                            };

                            tbody2 = null;

                        }
                        else if (new[] {"PEI", "BRI", "ECI" }.Contains(codPais))
                        {
                            resume.Add(new GridHead(isEs ? "Fecha Importación (dd/mm/yyyy)" : "Import date (mm/dd/yyyy)", isEs ? 
                                v.fechanum_date?.ToString("dd/MM/yyyy") :
                                v.fechanum_date?.ToString("MM/dd/yyyy")));
                            resume.Add(new GridHead(isEs ? "Descripción Comercial" : "Commercial description",
                                v.DesComercial));
                            resume.Add(new GridHead(isEs ? "Descripción Adicional" : "Additional description",
                                v.MarcasContenedor));
                            resume.Add(new GridHead(isEs ? "Importador / Consignatario" : "Importer / Consignee",
                                v.Importador));
                            resume.Add(new GridHead(isEs ? "Exportador / Embarcador" : "Exporter / Shipper",
                                v.Proveedor));

                            thead1 = new[]
                            {
                                (isEs?"Peso Bruto kg":"Gross kg"), (isEs?"Bultos":"Packages"), (isEs?"País Embarque":"Shipment Country"), (isEs?"Puerto Embarque":"Shipment Port"),  (isEs?"Nave":"Ship"),  (isEs?"Manifiesto":"Manifests") };
                            thead2 = null;

                            tbody1 = new[]
                            {
                                v?.PesoBruto?.ToString("n0",cultureInfo), v?.Bultos?.ToString("n0",cultureInfo),
                                v.PaisEmbarque, v.PtoEmbarque, v.Nave, v.Manifiesto
                            };

                            tbody2 = null;

                        }
                        else // PEE
                        {
                            resume.Add(new GridHead(isEs ? "Fecha Importación (dd/mm/yyyy)" : "Import date (mm/dd/yyyy)", isEs ? v.fechanum_date?.ToString("dd/MM/yyyy") :
                                v.fechanum_date?.ToString("MM/dd/yyyy")));
                            resume.Add(new GridHead(isEs ? "Descripción Comercial" : "Commercial description",
                                v.DesComercial));
                            resume.Add(new GridHead(isEs ? "Descripción Adicional" : "Additional description",
                                v.MarcasContenedor));
                            resume.Add(new GridHead(isEs ? "Exportador / Embarcador" : "Exporter / Shipper",
                                v.Exportador));
                            resume.Add(new GridHead(isEs ? "Importador / Consignatario" : "Importer / Consignee",
                                v.ImportadorExp));

                            thead1 = new[]
                            {
                                (isEs?"Peso Bruto kg":"Gross kg"), (isEs?"Bultos":"Packages"), (isEs?"País Destino":"Destination Country"), (isEs?"Puerto Destino":"Destination Port"),  (isEs?"Nave":"Ship"),  (isEs?"Manifiesto":"Manifests") };
                            thead2 = null;

                            tbody1 = new[]
                            {
                                v?.PesoBruto?.ToString("n0",cultureInfo), v?.Bultos?.ToString("n0",cultureInfo),
                                v.PaisDestino, v.PtoDestino, v.Nave, v.Manifiesto
                            };

                            tbody2 = null;
                        }
                    }

                    var modal_title = isEs
                        ? "Detalles de la " + (tipoOpe == "I" ? "Importación" : "Exportación")
                        : (tipoOpe == "I" ? "Import" : "Export") + " details";

                    var data = new { resume, thead1, thead2, tbody1, tbody2, modal_title }.ToExpando();
                    //return PartialView("Modals/Modal_VerDetalleId", new { resume, thead1, thead2, tbody1, tbody2, modal_title   }.ToExpando());
                    return Json(new
                    {
                        result = RenderViewToString(this.ControllerContext, "Modals/Modal_VerDetalleId", data)
                    });
                }
            }

            return null;
        }

        //[SessionExpireFilter]
        [HttpGet]
        public ActionResult Testing()
        {
            var result = new object();
            using (var db = new ConexProvider().Open)
            {
                result = db.QueryFirst("select Aduana = 'Test'");
            }




            List<string> mylist = new List<string>(new string[] { "element1", "element2", "element3" });

            mylist.ForEach(x =>
            {
                x += " ss";
            });

            var xy = (from w in mylist
                      select new { a = w, b = "hh" });



            return PartialView("Test", new { exito = "Test 1", result, mylist, xy }.ToExpando());
        }

        private List<GridHead> _GetTitleTableDetExcel(string tipoOpe, string codPais, string campoPeso, string cif,  /*FlagVarVisibles*/ TabMisBusquedas   vFlag, bool FlagFormatoB = false, 
            bool existeDua = false)
        {
            string idioma = GetCurrentIdioma();
            var isEs = (idioma == "es");
            var isManif = IsManifiesto(codPais);
            string auxCif = cif.Replace("Tot", "");
            string plan = Session["Plan"].ToString();
            if (isManif)
            {
                return new List<GridHead>()
                {
                    new GridHead("No."),
                    new GridHead(resx.Date_Text),
                    new GridHead(tipoOpe == "I" ? resx.Search_Form_Item05_I : resx.Search_Form_Item06_I),
                    new GridHead(resx.Demo_Notif_Tab_2, visible: vFlag.ExisteNotificado),
                    new GridHead(tipoOpe == "I" ? resx.Search_Form_Item06_I : resx.Search_Form_Item05_I, visible: (tipoOpe == "I" ? vFlag.ExisteProveedor : (vFlag.ExisteImportadorExp && plan != "ESENCIAL") ) ),
                    new GridHead(tipoOpe == "I" ? resx.Pais_Embarque : resx.Demo_DestinationCountry),
                    new GridHead(vFlag.ExistePtoDescarga ? resx.Demo_PtoDescarga : vFlag.ExistePtoEmbarque ? resx.Demo_PtoEmbarque : resx.Demo_PtoDestino   ),
                    new GridHead(resx.PesoBruto_Text + " Kg"),
                    new GridHead(codPais == "USI" || codPais == "USE" ? resx.Quantity_Text : resx.Demo_Bultos ),
                    new GridHead(resx.Unit_Text, visible:  codPais == "USI" || codPais == "USE" ),
                    new GridHead(resx.Home_Search_Cbo_Item02 + "<div class=\"wh-control\"><input type=\"text\" id=\"txtDesComExtra\"  />"+
                                 "<a href=\"javascript:void(0)\" class=\"lnkSearch\"><img id=\"imgDesComExtra\" src=\""+Url.Content("/Content/Images/bot_lupal_off.png")+"\" " +
                                 "title=\""+ (isEs?"Buscar":"Search") +"\" width=\"20\" height=\"20\" /></a>"+
                                 "<a href=\"javascript:void(0)\" class=\"lnkReset\"><img src=\""+ Url.Content("/Content/Images/bot_recarga_off.png")+"\" " +
                                 "title=\""+ (isEs?"Restablecer":"Clear")  +"\" width=\"20\" height=\"20\" /></a>"+
                                 "<div><label id=\"lblResultDesc\"></label></div></div>", className: "th-des", visible: vFlag.ExisteDesComercial),
                    new GridHead(resx.Demo_DescAdicional, visible: vFlag.ExisteDesAdicional ),

                };
            }

            return new List<GridHead>()
                {
                    new GridHead("No."),
                    new GridHead(isEs?"Fecha": "Date"),
                    new GridHead(isEs?"Partida Aduanera":"HTS Code"),
                    new GridHead(tipoOpe == "I" ? (isEs?"Importador": "Importer") : (isEs?"Exportador": "Exporter"), visible: (tipoOpe == "I" ? vFlag.ExisteImportador : vFlag.ExisteExportador)),
                    new GridHead(tipoOpe == "I" ? (codPais != "CL" ? (isEs?"Exportador":"Exporter") : (isEs?"Marca":"Brand") ) : (isEs?"Importador":"Importer"), visible: (tipoOpe == "I" ? vFlag.ExisteProveedor : (vFlag.ExisteImportadorExp && plan != "ESENCIAL") )),
                    new GridHead(isEs?"Marca":"Brand", visible: (vFlag.ExisteMarcaEC) ),
                    new GridHead(campoPeso == "PesoNeto" ? (isEs? "Kg Neto": "Net kg"): (isEs?"Kg Bruto": "Gross kg"), visible: (campoPeso != "") ),
                    new GridHead(isEs?"Cantidad":"Quantity"),
                    new GridHead(isEs?"Unid.":"Unit"),
                    new GridHead(codPais != "US" ? "US$ FOB Unit." : "US$ FAS Unit.", visible: FuncionesBusiness.IsVisibleFobUnit(codPais,tipoOpe)/*((auxCif != "FOB" && codPais != "CN" && codPais != "IN") || tipoOpe != "I"   )*/ ),
                    new GridHead("US$ "+ auxCif +" Unit.", visible:(tipoOpe=="I") ),
                    new GridHead(isEs?"US$ CIF Unit. + Imptos.": "US$ CIF Unit. + Taxes.", visible:(tipoOpe =="I"? (!FlagFormatoB && codPais =="PE" )  : false ) ),
                    new GridHead("DUA <div class=\"wh-control\"><input type=\"text\" id=\"txtDuaExtra\" />"+
                                 "<a href=\"javascript:void(0)\" class=\"lnkSearch\"><img id=\"imgDuaExtra\" src=\""+ Url.Content("/Content/Images/bot_lupal_off.png") + "\" " +
                                 "title=\""+ (isEs?"Buscar":"Search") +"\" width=\"20\" height=\"20\" /></a>"+
                                 "<a href=\"javascript:void(0)\" class=\"lnkReset\"><img src=\""+ Url.Content("/Content/Images/bot_recarga_off.png")+"\" " +
                                 "title=\""+ (isEs?"Restablecer":"Clear") +"\" width=\"20\" height=\"20\" /></a> "+
                                 "<div><label id=\"lblResultDua\"></label></div></div>", className: "th-dua", visible: vFlag.ExisteDUA),
                    new GridHead(tipoOpe == "I" ? (isEs? "País Origen": "Origin Country") :  (isEs? "País Destino": "Destination Country"),
                                visible: (tipoOpe =="I" ? vFlag.ExistePaisOrigen : vFlag.ExistePaisDestino ) ),
                    new GridHead((isEs?"Descripción Comercial":"Commercial description")+ "<div class=\"wh-control\"><input type=\"text\" id=\"txtDesComExtra\"  />"+
                                 "<a href=\"javascript:void(0)\" class=\"lnkSearch\"><img id=\"imgDesComExtra\" src=\""+Url.Content("/Content/Images/bot_lupal_off.png")+"\" " +
                                 "title=\""+ (isEs?"Buscar":"Search") +"\" width=\"20\" height=\"20\" /></a>"+
                                 "<a href=\"javascript:void(0)\" class=\"lnkReset\"><img src=\""+ Url.Content("/Content/Images/bot_recarga_off.png")+"\" " +
                                 "title=\""+ (isEs?"Restablecer":"Clear")  +"\" width=\"20\" height=\"20\" /></a>"+
                                 "<div><label id=\"lblResultDesc\"></label></div></div>", className: "th-des", visible: vFlag.ExisteDesComercial),
                    new GridHead("Unlanding District", visible: (codPais=="US") ),
                    
                };
        }

        private string AddExtraFiltroDetalleExcel(string txtDua, string txtDesCom, string duaField)
        {
            string sql = string.Empty;
            if (txtDua.Trim().Length > 0)
            {
                sql += " and " + duaField + " like '%" + txtDua + "%'";
            }
            if (txtDesCom.Trim().Length > 0)
            {
                sql += " and DesComercial like '%" + txtDesCom + "%'";
            }
            return sql;
        }

        private SelectList GetDropDownExcel(string codPais, string codPais2, string tipoOpe, string idioma,  bool isVisible=false)
        {
            if (!isVisible) return null;

            var idUsuario = Session["IdUsuario"].ToString();
            string codPaisT = codPais;
            if (codPais == "PEP")
                codPaisT = "PE";
            else if (codPais2 == "4UE")
                codPaisT = "UE" + codPais;

            var idDescargaDefault = FuncionesBusiness.BuscaDescargaDefault(idUsuario, codPaisT, tipoOpe);
            var lstDescargas =
                FuncionesBusiness.CargaDescargas(idUsuario, codPais, codPais2, tipoOpe, idioma).DataTableToList<DownloadExcel>();
            return new SelectList(lstDescargas, "IdDescargaCab", "Descarga", idDescargaDefault);
        }

       
        [SessionExpireFilter]
        [HttpPost]
        public JsonResult VerInfoTabla(string tipoOpe, string codPais, int indexCboPaisB,string codPais2, string codPaisB, string anioMesIni,
            string anioMesFin)
        {


            return Json(new { });
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult VerDetalleExcel(string tipoOpe, string codPais, int indexCboPaisB, string txtPais, string txtDua, string txtDesCom, string codPais2)
        {      
            GuardarLogInicioErrorSession("VerDetalleExcel()");

            TabGridPageSize = 20;
            string codPaisAux = codPais;
            ValidaCodPais2(codPais2, ref codPais);
            ValidaCodPaisManif(ref codPais, tipoOpe);
            ValidExistData(codPais, tipoOpe);
            var isManif = IsManifiesto(codPais);
            var vFlag = new TabMisBusquedas(tipoOpe, codPais);   //new FlagVarVisibles(codPais, tipoOpe, isManif);

            /*if (Session["Plan"].ToString() == "ESENCIAL" && tipoOpe == "E")
            {
                vFlag.ExisteImportadorExp = false;
            }*/

            List<object> lista = null;
            lista = new List<object>();
            string idioma = GetCurrentIdioma();
            CultureInfo specificCulture = GetSpecificCulture();

            var tabla = "";
            var dua = "";
            GetTablaAndDua(tipoOpe, codPais, ref tabla, ref dua);

            string cif = GetCIFTot(codPais, tipoOpe);  //Funciones.Incoterm(codPais, tipoOpe);
            string cifTot = cif;//+ "Tot";
            string pesoNeto = Funciones.CampoPeso(codPais, tipoOpe);

            //DataRow drTotales =
            //    FuncionesBusiness.CalculaTotales(Session["SqlFiltro"].ToString(), cifTot, codPais, pesoNeto, tabla, false);
            //int cantReg = Convert.ToInt32(drTotales["CantReg"]);
            //decimal valueCifTot = Convert.ToDecimal(drTotales[cifTot]);

            string filtroMarca = "DetalleExcel"; //Enums.Filtro.Marca.ToString();
            string _addEx = AddExtraFiltroDetalleExcel(txtDua, txtDesCom, dua);
            int totalRegistosMarca = CuentaAgrupado(filtroMarca, tabla, dua, _addEx);
            //bool existeDua = GetExisteDua(GetExisteAduana(codPais), codPais);

            TabData objTabData = new TabData();
            //objTabData.TotalRegistros = cantReg.ToString("n0", specificCulture);
            //objTabData.CiFoFobTotal = Convert.ToDecimal(valueCifTot).ToString("n0", specificCulture);
            objTabData.Lang = idioma;
            objTabData.HeadTitles = this._GetTitleTableDetExcel(tipoOpe, codPais, pesoNeto, cif, vFlag: vFlag);
            objTabData.GridHead.SetVisiblesColumns(tipoOpe, codPais, cif, vFlag.ExisteImportador, vFlag.ExisteExportador, vFlag.ExisteProveedor, vFlag.ExisteImportadorExp, vFlag.ExisteDUA
                , vFlag.ExistePaisOrigen, vFlag.ExistePaisDestino, vFlag.ExisteDesComercial, pesoNeto, Session["Plan"].ToString(),vFlag.ExisteMarcaEC);

            //objTabData.IsVisbleOpcionDescarga = (Session["Plan"].ToString() != "UNIVERSIDADES");

            //objTabData.DropDownDescarga = GetDropDownExcel(codPais, codPais2, tipoOpe, idioma,
            //    (Session["Plan"].ToString() != "UNIVERSIDADES"));

            if(codPais2 != "4UE")
            {
                codPaisAux = codPais;
            }

            objTabData.DropDownDescarga = GetDropDownExcel(codPaisAux, codPais2, tipoOpe, idioma,
                    true);

            //ViewBag.ActionStatusId = new SelectList(repository.GetAll<ActionStatus>(), "ActionStatusId", "Name", myAction.ActionStatusId);
            objTabData.GridHead.CiFoFobPor = "%";
            objTabData.GridHead.IsVisblePrecio = false;
            objTabData.GridHead.IsVisibleTotalKg = false;
            objTabData.GridHead.IsVisibleFiltroCboDescripcion = false;
            objTabData.GridHead.TotalReg = Resources.Resources.Grid_Column_TotalRecords;
            objTabData.GridHead.TitleColumnTotalReg = Resources.Resources.Grid_Column_SeeRecords;
            //objTabData.GridHead.CiFoFobTot = "Total US$ FOB";
            //objTabData.GridHead.OrdenCiFoFobTot = "FOBTot";
            objTabData.GridHead.IsVisibleColumnCheck = false;
            objTabData.IsVisibleButtons = false;

            objTabData.Filtro = filtroMarca;
            objTabData.TotalPaginasTab = (int)Math.Ceiling(Convert.ToDecimal(totalRegistosMarca) / TabGridPageSize);
            objTabData.GridHead.Descripcion = Resources.Resources.Demo_Brands_Tab;
            objTabData.TituloTab = Resources.Resources.MySearchs_BrandsModels_Tabs;

            useRand = (bool)Session["useRand"];

            var listDataTable = CargaFiltro(filtroMarca, -1, codPais, cifTot, pesoNeto, idioma, dua, tabla,
                indexCboPaisB, Enums.TipoFiltro.Tab, "FechaNum", addExtraFiltro: _addEx, useRand: useRand);

            //objTabData.ListRowsTab = GetListGridRowMarcasModelos(listDataTable[0], filtroMarca, valueCifTot, specificCulture);
            objTabData.ListRowsTab = GetListGridRowDetalleExcel(listDataTable[0], filtroMarca, specificCulture, dua, codPais, txtPais, tipoOpe, idioma, cif, pesoNeto, vFlag);

            //objTabData.FlagRegMax = cantReg > CantRegMax;

            if(objTabData.ListRowsTab.Count > 0)
            {
                if (codPais == "CO")
                {
                    var idPlan = Funciones.ObtieneIdPlan(Session["IdUsuario"].ToString());
                    bool isPlanPermiteSentinel = FuncionesBusiness.IsVisibleSentinal2023(idPlan); // Ruben 202311

                    foreach (var item in objTabData.ListRowsTab)
                    {
                        item.mostrarInformaColombia = true;
                        item.IsPlanPermiteSentinel = isPlanPermiteSentinel;
                    }
                }
                if (objTabData.ListRowsTab[0].Descripcion == null)
                {
                    objTabData.GridHead.IsVisibleDesCom = false;

                    foreach (var item in objTabData.HeadTitles)
                    {
                        if(item.label.Contains( "Descripción Comercial") || item.label.Contains("Commercial description"))
                            item.visible = false;
                    }

                }   
            }

            objTabData.FlagVarVisibles = vFlag;

            object objMensaje = null;
            if (totalRegistosMarca > 0)
            {
                lista.Add(new
                {
                    tabDataName = filtroMarca,
                    tabDataNumRows = totalRegistosMarca,
                    tabDataList = RenderViewToString(this.ControllerContext, "GridViews/DetalleExcelGridView" + (isManif ? "_Manif" : "") , objTabData),
                    tabTotalPages = objTabData.TotalPaginasTab,
                    pieTitle = Resources.Resources.Demo_Brands_Tab,
                    //tabPieData = GetJsonDataPieMarcas(filtroMarca, listDataTable[0], valueCifTot, specificCulture)
                    lblRecords = string.Format("{0} {1}", totalRegistosMarca, (idioma == "es" ? " registro(s)" : " record(s)")),
                });
            }
            else
            {
                objMensaje = new
                {
                    titulo = Resources.Resources.Search_Button,
                    mensaje = Resources.Resources.NoResultsWereFound_Text,
                    flagContactenos = false
                };
            }



            return Json(new
            {
                gridData = lista,
                objMensaje
            });
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult ChangePageTableDetalleExcel(string filtro, int pagina, string codPais,
            string tipoOpe, string idTabla, string txtPais, string txtDua, string txtDesCom, string codPais2)
        {

            GuardarLogInicioErrorSession("ChangePageTableDetalleExcel()");

            TabGridPageSize = 20;
            ValidaCodPais2(codPais2, ref codPais);
            ValidaCodPaisManif(ref codPais, tipoOpe);
            ValidExistData(codPais, tipoOpe);

            var isManif = IsManifiesto(codPais);
            var vFlag = new TabMisBusquedas(tipoOpe, codPais); //new FlagVarVisibles(codPais, tipoOpe, isManif);


            var tabla = "";
            var dua = "";
            GetTablaAndDua(tipoOpe, codPais, ref tabla, ref dua);

            string idioma = GetCurrentIdioma();


            string cif = GetCIFTot(codPais, tipoOpe);//Funciones.Incoterm(codPais, tipoOpe);
            string cifTot = cif + "Tot";
            string pesoNeto = Funciones.CampoPeso(codPais, tipoOpe);

            //DataRow drTotales =
            //    FuncionesBusiness.CalculaTotales(Session["SqlFiltro"].ToString(), cifTot, codPais, pesoNeto, tabla, true);
            //decimal valueCifTot = Convert.ToDecimal(drTotales[cifTot]);


            string _addEx = AddExtraFiltroDetalleExcel(txtDua, txtDesCom, dua);

            List<DataTable> listaAux = CargaFiltro(filtro, pagina, "", "", "", "", dua, "", 0, Enums.TipoFiltro.Tab, addExtraFiltro: _addEx);
            TabData objTabData = new TabData();
            objTabData.Lang = idioma;
            objTabData.Filtro = filtro;
            objTabData.FlagVarVisibles = vFlag;

            bool existeDua = GetExisteDua(GetExisteAduana(codPais), codPais);
            objTabData.GridHead.SetVisiblesColumns(tipoOpe, codPais, cif, vFlag.ExisteImportador, vFlag.ExisteExportador, vFlag.ExisteProveedor, vFlag.ExisteImportadorExp, vFlag.ExisteDUA
                , vFlag.ExistePaisOrigen, vFlag.ExistePaisDestino, vFlag.ExisteDesComercial, pesoNeto, Session["Plan"].ToString(),vFlag.ExisteMarcaEC);
            objTabData.GridHead.IsVisblePrecio = false;
            objTabData.GridHead.IsVisibleTotalKg = false;
            objTabData.GridHead.IsVisibleColumnCheck = false;

            CultureInfo specificCulture = GetSpecificCulture();

            objTabData.ListRowsTab = GetListGridRowDetalleExcel(listaAux[0], filtro, specificCulture, dua, codPais, txtPais, tipoOpe, idioma, cif, pesoNeto, vFlag);

            if (objTabData.ListRowsTab.Count > 0)
            {
                if (codPais == "CO")
                {
                    var idPlan = Funciones.ObtieneIdPlan(Session["IdUsuario"].ToString());
                    bool isPlanPermiteSentinel = FuncionesBusiness.IsVisibleSentinal(idPlan); // Ruben 202311

                    foreach (var item in objTabData.ListRowsTab)
                    {
                        item.mostrarInformaColombia = true;
                        item.IsPlanPermiteSentinel = isPlanPermiteSentinel;
                    }
                }
            }

            return Json(new
            {
                filasNuevaPagina = RenderViewToString(this.ControllerContext, "GridViews/DetalleExcelGridRowView" + (isManif?"_Manif" : ""), objTabData)
            });
        }

        public DataTable GenerarListaInfoTabla(string sql)
        {
            string sqlFiltro = $@" SELECT * , ROW_NUMBER() over (order by ciftot) as Nro 
                                    FROM ( "+ sql + " ) X";
            DataTable dt;
            try
            {


                dt = Conexion.SqlDataTable(sql);
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                dt = null;
            }

            return dt;
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult VerInfoComplementario(string tipoOpe, string codPais, string codPais2Aux, int indexCboPaisB,string codPaisB,string anioMesIni,string anioMesFin, bool hideTabExcel = false, bool FlagRegMax = false)
        {

            GuardarLogInicioErrorSession("VerInfoComplementario()");

            List<object> lista = null;
        
            if (Session["dtMarca"] == null)
            {
                lista = new List<object>();
                string idioma = GetCurrentIdioma();
                CultureInfo specificCulture = GetSpecificCulture();

                var tabla = "";
                var dua = "";
                List<string> paisesInfoComplementario = (List<string>)Session["PaisesComplementarioOrigenDestino"];

                string codPaisAux = codPais;
                
                if(codPais2Aux == "4UE")
                {
                    codPaisAux = "UE";
                }

                List<String> countryInfo1 = FuncionesBusiness.SearchCountryInfoComplementario(codPaisAux);
                List<String> countryInfo = new List<String>();
                
                foreach (var pais in countryInfo1)
                {
                    string paisAux = FuncionesBusiness.BuscarPaisNombre(pais, idioma);
                    if(pais == "MXD")
                    {
                        paisAux = "México";
                    }

                    if (paisesInfoComplementario.Contains(paisAux.ToUpper()) || 
                        paisesInfoComplementario.Contains(paisAux.ToUpper().Replace('Á','A').Replace('É', 'E').Replace('Í', 'I').Replace('Ó', 'O').Replace('Ú', 'U')))
                    {
                        countryInfo.Add(pais);
                    }

                }

                string cifTot = "";
                string pesoNeto = "";
                decimal valueCifTot = 0;
                int cantReg = 0;
                int totalData = 0;
                TabData objTabData = new TabData();
                string filtroMarca = Enums.Filtro.InfoTabla.ToString();
                DataTable listDataTable = null;
                string sqlFiltro = "";
                tipoOpe = tipoOpe == "I" ? "E" : "I";
                string codPais2 = new ListaPaises().BuscarCodPais2(codPaisAux);
                
                if (codPais2Aux == "4UE")
                {
                    codPais2 = codPais2Aux;
                }
                string nombrePais = FuncionesBusiness.BuscarPaisNombre(codPaisAux, idioma);

                if(codPais2Aux == "4UE")
                {
                    nombrePais = FuncionesBusiness.BuscarPaisUE(codPais, idioma);
                }

                if(nombrePais == "USA")
                {
                    nombrePais = "ESTADOS UNIDOS";
                }
                
                foreach (var pais in countryInfo)
                {
                    codPais = pais;

                    codPais2 = new ListaPaises().BuscarCodPais2(codPais);
                    if(codPais == "UE")
                    {
                        codPais2 = "4UE";
                    }

                    string sqlAux =
                     GeneraSqlFiltro(codPais, codPais2, tipoOpe, anioMesIni, anioMesFin, indexCboPaisB,
                codPaisB, dua, codPais,tabInfo : true);
                    int idPais = FuncionesBusiness.BuscarIdPaisNombre(nombrePais, codPais);

                    if(nombrePais == "Alemania" && codPais == "EC")
                    {
                        idPais = 53;
                    }
                    

                    GetTablaAndDua(tipoOpe, codPais, ref tabla, ref dua);
                    string cifTotAux = Funciones.Incoterm(codPais, tipoOpe) + "Tot";
                    pesoNeto = Funciones.CampoPeso(codPais, tipoOpe);
                    if (nombrePais == "ESTADOS UNIDOS" && codPais == "EC")
                    {
                        idPais = 158;
                    }
                    // Ruben 202310
                    else if (nombrePais == "ESTADOS UNIDOS" && codPais == "MXD")
                    {
                        idPais = 321;
                    }

                    if (idPais != 0)
                    {
                        Session["SqlInfoComplementario" + codPais] = sqlAux + (tipoOpe == "I" ? " and idPaisOrigen = " : " and idPaisDestino = ") + idPais + " ";
                    }
                    else
                    {
                        Session["SqlInfoComplementario" + codPais] = sqlAux;
                    }

                    //Session["SqlInfoComplementario" + codPais] = sqlAux;
                    
                    string sql = GeneraSqlAgrupado(filtroMarca, codPais, cifTotAux, pesoNeto, idioma, dua, tabla, "", isInfoTabla: true);

                    DataTable dt;
                    try
                    {
                        dt = Conexion.SqlDataTable(sql);
                    }
                    catch (Exception ex)
                    {
                        
                        
                        
                        ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                        dt = null;
                    }

                    if (dt != null && (int)dt.Rows[0][2] > 0) {
                        sqlFiltro += sql;
                        sqlFiltro += " UNION ";
                    }
                    

                }

                if(sqlFiltro != "")
                {
                    
                    sqlFiltro = sqlFiltro.Substring(0, sqlFiltro.Length - 6);
                    Session["sqlTabInfoComplementaria"] = sqlFiltro;
                    cifTot = "ciftot";

                    DataRow drTotales = FuncionesBusiness.CalculaTotalesInfo(sqlFiltro);
                    cantReg = Convert.ToInt32(drTotales["CantReg"]);
                    totalData = Convert.ToInt32(drTotales["TotalData"]);
                    valueCifTot = drTotales.GetValue<decimal>(cifTot);

                    listDataTable = FuncionesBusiness.ListaInfoTabla(sqlFiltro, 1, TabGridPageSize, false);//GenerarListaInfoTabla(sqlFiltro);


                    var listAux = GetListGridRowInfoComplementario(listDataTable, filtroMarca, valueCifTot, specificCulture);

                    int totalRegistosMarca = cantReg;//listAux.Count;//CuentaAgrupado(filtroMarca, tabla, dua);

                    objTabData.TotalRegistros = totalData.ToString("n0", specificCulture);
                    objTabData.CiFoFobTotal = Convert.ToDecimal(valueCifTot).ToString("n0", specificCulture);
                    objTabData.GridHead.CiFoFobPor = "%";
                    objTabData.GridHead.IsVisblePrecio = false;
                    objTabData.GridHead.IsVisibleTotalKg = false;
                    objTabData.GridHead.IsVisibleFiltroCboDescripcion = false;
                    objTabData.GridHead.TotalReg = Resources.Resources.Grid_Column_TotalRecords;
                    objTabData.GridHead.TitleColumnTotalReg = Resources.Resources.Grid_Column_SeeRecords;
                    objTabData.GridHead.CiFoFobTot = "Total US$ FOB";
                    objTabData.GridHead.OrdenCiFoFobTot = "FOBTot";
                    objTabData.GridHead.IsVisibleColumnCheck = false;
                    objTabData.IsVisibleButtons = false;
                    objTabData.HideTabExcel = hideTabExcel;

                    objTabData.Filtro = filtroMarca;
                    objTabData.TotalPaginasTab = (int)Math.Ceiling(Convert.ToDecimal(totalRegistosMarca) / 20);//(int)Math.Ceiling(Convert.ToDecimal(totalRegistosMarca) / TabGridPageSize);
                    objTabData.GridHead.Descripcion = tipoOpe == "E" ?
                                                        Resources.AdminResources.AduanaOriginInfoComplementaria_text:
                                                        Resources.AdminResources.AduanaDestinationInfoComplementaria_text;//"InfoTabla";//Resources.Resources.Demo_Brands_Tab;
                    objTabData.TituloTab = "Información Complementaria";//Resources.Resources.MySearchs_BrandsModels_Tabs;

                    objTabData.ListRowsTab = listAux;//GetListGridRowInfoComplementario(listDataTable, filtroMarca, valueCifTot, specificCulture);

                    objTabData.FlagRegMax = FlagRegMax;//cantReg > CantRegMax;

                    Chart chartMyChart = GetJsonDataColumn(filtroMarca, listDataTable, "CifTot", valueCifTot.ToString(), specificCulture);

                    decimal mayor = 0;

                    foreach (var listax in chartMyChart.Column[0].data)
                    {
                        if (listax.y > mayor)
                            mayor = listax.y;
                    }

                    string numero = Convert.ToInt64(mayor).ToString();
                    string cadena = "";
                    for (int i = 0; i < numero.Length - 1; i++)
                        cadena += "0";

                    chartMyChart.TitleContainer = objTabData.TituloTab;
                    chartMyChart.Title = chartMyChart.TitleContainer;
                    chartMyChart.TickInterval = Convert.ToInt64("1" + cadena);

                    //var x = GetJsonDataPie(filtroMarca, listDataTable, "CifTot", valueCifTot.ToString(), specificCulture);
                    lista.Add(new
                    {
                        tabDataName = filtroMarca,
                        tabDataNumRows = totalRegistosMarca,
                        tabDataList = RenderViewToString(this.ControllerContext, "GridViews/TabGridView", objTabData),
                        tabTotalPages = objTabData.TotalPaginasTab,
                        pieTitle = tipoOpe == "E" ?
                                                        Resources.AdminResources.AduanaOriginInfoComplementaria_text :
                                                        Resources.AdminResources.AduanaDestinationInfoComplementaria_text,//"InfoTabla",
                        //tabPieData = GetJsonDataPieMarcas(filtroMarca, listDataTable[0], valueCifTot, specificCulture)
                    tabPieData = GetJsonDataPie(filtroMarca, listDataTable, "CifTot", valueCifTot.ToString(), specificCulture),
                        chartMyChart
                    });
                }

                
            }

            return Json(new
            {
                gridData = lista
            });
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult VerMarcas(string tipoOpe, string codPais, int indexCboPaisB, bool hideTabExcel = false,bool FlagRegMax = false)
        {
            GuardarLogInicioErrorSession("VerMarcas()");

            List<object> lista = null;
            if (Session["dtMarca"] == null)
            {
                lista = new List<object>();
                string idioma = GetCurrentIdioma();
                CultureInfo specificCulture = GetSpecificCulture();

                var tabla = "";
                var dua = "";
                GetTablaAndDua(tipoOpe, codPais, ref tabla, ref dua);

                string cifTot = Funciones.Incoterm(codPais, tipoOpe) + "Tot";
                string pesoNeto = Funciones.CampoPeso(codPais, tipoOpe);

                DataRow drTotales =
                    FuncionesBusiness.CalculaTotales(Session["SqlFiltro"].ToString(), cifTot, codPais, pesoNeto, tabla, true);
                int cantReg = Convert.ToInt32(drTotales["CantReg"]);
                decimal valueCifTot = drTotales.GetValue<decimal>(cifTot);

                string filtroMarca = Enums.Filtro.Marca.ToString();
                int totalRegistosMarca = CuentaAgrupado(filtroMarca, tabla, dua);

               
                TabData objTabData = new TabData();
                objTabData.TotalRegistros = cantReg.ToString("n0", specificCulture);
                objTabData.CiFoFobTotal = Convert.ToDecimal(valueCifTot).ToString("n0", specificCulture);
                objTabData.GridHead.CiFoFobPor = "%";
                objTabData.GridHead.IsVisblePrecio = false;
                objTabData.GridHead.IsVisibleTotalKg = false;
                objTabData.GridHead.IsVisibleFiltroCboDescripcion = false;
                objTabData.GridHead.TotalReg = Resources.Resources.Grid_Column_TotalRecords;
                objTabData.GridHead.TitleColumnTotalReg = Resources.Resources.Grid_Column_SeeRecords;
                objTabData.GridHead.CiFoFobTot = "Total US$ FOB";
                objTabData.GridHead.OrdenCiFoFobTot = "FOBTot";
                objTabData.GridHead.IsVisibleColumnCheck = false;
                objTabData.IsVisibleButtons = false;
                objTabData.HideTabExcel = hideTabExcel;

                objTabData.Filtro = filtroMarca;
                objTabData.TotalPaginasTab = (int)Math.Ceiling(Convert.ToDecimal(totalRegistosMarca) / TabGridPageSize);
                objTabData.GridHead.Descripcion = Resources.Resources.Demo_Brands_Tab;
                objTabData.TituloTab = Resources.Resources.MySearchs_BrandsModels_Tabs;

                var listDataTable = CargaFiltro(filtroMarca, -1, codPais, cifTot, pesoNeto, idioma, dua, tabla,
                    indexCboPaisB, Enums.TipoFiltro.Tab);

                objTabData.ListRowsTab = GetListGridRowMarcasModelos(listDataTable[0], filtroMarca, valueCifTot, specificCulture);

                objTabData.FlagRegMax = FlagRegMax;//cantReg > CantRegMax;

                lista.Add(new
                {
                    tabDataName = filtroMarca,
                    tabDataNumRows = totalRegistosMarca,
                    tabDataList = RenderViewToString(this.ControllerContext, "GridViews/TabGridView", objTabData),
                    tabTotalPages = objTabData.TotalPaginasTab,
                    pieTitle = Resources.Resources.Demo_Brands_Tab,
                    //tabPieData = GetJsonDataPieMarcas(filtroMarca, listDataTable[0], valueCifTot, specificCulture)
                    tabPieData = GetJsonDataPie(filtroMarca, listDataTable[0], "FOBTot", valueCifTot.ToString(), specificCulture)
                });

                string filtroModelo = Enums.Filtro.Modelo.ToString();
                int totalRegistosModelo = CuentaAgrupado(filtroModelo, tabla, dua);

                objTabData.Filtro = filtroModelo;
                objTabData.TotalPaginasTab = (int)Math.Ceiling(Convert.ToDecimal(totalRegistosModelo) / TabGridPageSize);
                objTabData.GridHead.Descripcion = Resources.Resources.Demo_Models_Tab;

                var listDataTableModelo = CargaFiltro(filtroModelo, -1, codPais, cifTot, pesoNeto, idioma, dua, tabla,
                    indexCboPaisB, Enums.TipoFiltro.Tab);
                objTabData.ListRowsTab = GetListGridRowMarcasModelos(listDataTableModelo[0], filtroModelo, valueCifTot, specificCulture);
                objTabData.IsVisbleOpcionDescarga = false;
                lista.Add(new
                {
                    tabDataName = filtroModelo,
                    tabDataNumRows = totalRegistosModelo,
                    tabDataList = RenderViewToString(this.ControllerContext, "GridViews/TabGridView", objTabData),
                    tabTotalPages = objTabData.TotalPaginasTab,
                    pieTitle = "",
                    tabPieData = ""
                });
            }

            return Json(new
            {
                gridData = lista
            });
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult ChangePageTableMarcaModelo(string filtro, int pagina, string codPais,
            string tipoOpe, string idTabla)
        {
            GuardarLogInicioErrorSession("ChangePageTableMarcaModelo()");
            var tabla = "";
            var dua = "";
            GetTablaAndDua(tipoOpe, codPais, ref tabla, ref dua);

            string cifTot = Funciones.Incoterm(codPais, tipoOpe) + "Tot";
            string pesoNeto = Funciones.CampoPeso(codPais, tipoOpe);

            DataRow drTotales =
                FuncionesBusiness.CalculaTotales(Session["SqlFiltro"].ToString(), cifTot, codPais, pesoNeto, tabla, true);
            decimal valueCifTot = drTotales.GetValue<decimal>(cifTot);

            List<DataTable> listaAux = CargaFiltro(filtro, pagina, "", "", "", "", "", "", 0, Enums.TipoFiltro.Tab);
            TabData objTabData = new TabData();
            objTabData.Filtro = filtro;
            objTabData.GridHead.IsVisblePrecio = false;
            objTabData.GridHead.IsVisibleTotalKg = false;
            objTabData.GridHead.IsVisibleColumnCheck = false;
            objTabData.ListRowsTab = GetListGridRowMarcasModelos(listaAux[0], filtro, valueCifTot, GetSpecificCulture());
            objTabData.CodPais = codPais;

            return Json(new
            {
                filasNuevaPagina = RenderViewToString(this.ControllerContext, "GridViews/TabGridRowView", objTabData)
            });
        }

#endregion
        [SessionExpireFilter]
        [HttpPost]
        public JsonResult PageIndexChangingTable(string filtro, Enums.TipoFiltro tipoFiltro, string codPais2,
            int pagina, string codPais, string tipoOpe,
            string idTabla, string idsSeleccionados, string idsPagina, string anioMesIni,string anioMesFin, int indexCboPaisB,string codPaisB)
        {
            GuardarLogInicioErrorSession("PageIndexChangingTable()");
            string auxCodPais = codPais;
            ValidaCodPais2(codPais2, ref codPais);
            ValidaCodPaisManif(ref codPais, tipoOpe );

            var isManif = IsManifiesto(codPais);

            string[] arrayIdsSeleccionados = idsSeleccionados.Split(new char[] { ',' });
            string[] arrayIdsPagina = idsPagina.Split(new char[] { ',' });

            ArrayList listaIdsSeleccionados = null;
            if (Session["GdvSeleccionado"] != null && Session["GdvSeleccionado"].ToString() == idTabla)
                listaIdsSeleccionados = (ArrayList)Session["IDsSeleccionados"];

            Session["GdvSeleccionado"] = idTabla;

            //var existeDua = GetExisteDua(GetExisteAduana(codPais), codPais);

            //string idGdv = idTabla;
            //if (idTabla == Enums.TipoFiltro.Tab.ToString()+Enums.Filtro.AduanaDUA.ToString() && !existeDua)
            //{
            //    //idGdv = "Aduanas";
            //    filtro = Enums.Filtro.Aduana.ToString();
            //}

            Session["IDsSeleccionados"] = FuncionesBusiness.GuardaSeleccionados(filtro, listaIdsSeleccionados,
                arrayIdsSeleccionados, arrayIdsPagina);

            //if (idTabla == "Aduanas" && !existeDua)
            //    idGdv = "AduanaDUAs";

            var tabla = "";
            var dua = "";
            GetTablaAndDua(tipoOpe, codPais, ref tabla, ref dua);

            int cantReg = 0;
            string cifTot = "";
            string pesoNeto = "";
            string valueCifTot = "";
            decimal valuePesoNeto = 0;
            string unidad = "";
            GetCifTot_PesoNeto_CantReg_ValueCifTot_Unidad_ValuePesoNeto(codPais, tipoOpe, tabla, ref cifTot,
                ref pesoNeto, ref cantReg, ref valueCifTot, ref unidad, ref valuePesoNeto);

            List<DataTable> listaAux = CargaFiltro(filtro, pagina, "", "", "", "", "", "", 0, tipoFiltro);

            TabData objTabData = new TabData();
            objTabData.IsVisibleInfoComplementario = objTabData.IsVisibleInfoComplementario && _paisesConInfoComplementaria.Contains(codPais);
            objTabData.Filtro = filtro;
            objTabData.IdsSeleccionados = (ArrayList)Session["IDsSeleccionados"];
            objTabData.CodPais = codPais;
            string filasNuevaPagina = "";
            if (tipoFiltro == Enums.TipoFiltro.Resumen)
            {
                if(idTabla == "ResumenInfoTabla")
                {
                    valueCifTot = FuncionesBusiness.TotalInfoComplementario(Session["sqlFiltroTablas"].ToString());
                    cifTot = "ciftot";
                    objTabData.IsVisibleCheck = false;
                }

                objTabData.ListRows =
                    GetDataTableToListGridRow(listaAux[0], filtro, cifTot, valueCifTot, codPais, GetSpecificCulture(),
                    anioMesIni:anioMesIni,anioMesFin: anioMesFin,indexCboPaisB:indexCboPaisB,codPaisB:codPaisB,auxCodPais:auxCodPais);
                filasNuevaPagina = RenderViewToString(this.ControllerContext, "GridViews/GridRowView", objTabData);
            }
            else
            {
                if (filtro == Enums.Filtro.AduanaDUA.ToString() || filtro == Enums.Filtro.Aduana.ToString())
                {
                    if (filtro == Enums.Filtro.Aduana.ToString())
                        objTabData.GridHead.IsVisibleDuas = false;
                    objTabData.ListRowsTab = GetListGridRowAduanaDua(listaAux[0], filtro, cifTot, GetSpecificCulture());
                    filasNuevaPagina =
                        RenderViewToString(this.ControllerContext, "GridViews/AduanaDuaGridRowView", objTabData);
                }
                else
                {
                    objTabData.ListRowsTab = GetDataTableToListGridRowTab(listaAux[0], filtro, cifTot, valueCifTot, codPais,
                        GetSpecificCulture(), pesoNeto, unidad, 
                        anioMesIni: anioMesIni, anioMesFin: anioMesFin, indexCboPaisB: indexCboPaisB, codPaisB: codPaisB,auxCodPais:auxCodPais);
                    objTabData.GridHead.IsVisblePrecio = filtro != "Marca" && filtro != "Modelo" && unidad != "";
                    objTabData.GridHead.IsVisibleTotalKg = filtro != "Marca" && filtro != "Modelo" && unidad != "";
                    filasNuevaPagina = RenderViewToString(this.ControllerContext, "GridViews/TabGridRowView", objTabData);
                }
            }

            return Json(new
            {
                filasNuevaPagina
            });
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult AgregarFiltrosClick(string filtro, string idTabla, string codPais,
            string tipoOpe, string codPais2, string anioMesIni,
            string anioMesFin, int indexCboPaisB, string codPaisB,
            string idsSeleccionados, string idsPagina, int numFiltrosExistentes, string idioma)
        {
            bool maximoLimiteFiltros = false;

            object objMensaje = null;

            

            if (numFiltrosExistentes >= LimiteFiltros)
            {
                string mensaje = "Se ha llegado al maximo número de filtros: <b>" + LimiteFiltros +
                              "</b>.<br>Por favor elimine alguna palabra de la busqueda actual";
                if (idioma == "en")
                    mensaje = "You’ve reached the maximum number of filters allowed. Limit:" + LimiteFiltros +
                              "</b>.<br>Please delete a word from the current search";

                objMensaje = new
                {
                    titulo = idioma == "en" ? "Search" :"Buscar",
                    mensaje,
                    flagContactenos = false
                };

                maximoLimiteFiltros = true;
                return Json(new
                {
                    maximoLimiteFiltros,
                    objMensaje
                });
            }
            string[] arrayIdsSeleccionados = idsSeleccionados.Split(new char[] { ',' });
            string[] arrayIdsPagina = idsPagina.Split(new char[] { ',' });

            if (numFiltrosExistentes + arrayIdsSeleccionados.Length > LimiteFiltros)
            {
                string mensaje = "Se ha llegado al maximo número de filtros: <b>" + LimiteFiltros +
                              "</b>.<br>Por favor elimine alguna palabra de la busqueda actual";
                if (idioma == "en")
                    mensaje = "You’ve reached the maximum number of filters allowed. Limit:" + LimiteFiltros +
                              "</b>.<br>Please delete a word from the current search";

                objMensaje = new
                {
                    titulo = idioma == "en" ? "Search" : "Buscar",
                    mensaje,
                    flagContactenos = false
                };

                maximoLimiteFiltros = true;
                return Json(new
                {
                    maximoLimiteFiltros,
                    objMensaje
                });
            }


            string auxCodPais = codPais;
            ValidaCodPais2(codPais2, ref codPais);
            ValidaCodPaisManif(ref codPais, tipoOpe);

            var isManif = IsManifiesto(codPais);

            ArrayList listaIdsSeleccionados = null;
            if (Session["GdvSeleccionado"] != null && Session["GdvSeleccionado"].ToString() == idTabla)
                listaIdsSeleccionados = (ArrayList)Session["IDsSeleccionados"];

            bool existeDua = GetExisteDua(GetExisteAduana(codPais), codPais);

            //string idGdv = idTabla;
            //if (idTabla == "AduanaDUAs" && !existeDua)
            //{
            //    idGdv = "Aduanas";
            //}

            

            bool existenFiltros = false;

            listaIdsSeleccionados = FuncionesBusiness.GuardaSeleccionadosAbuscar(filtro, listaIdsSeleccionados,
                arrayIdsSeleccionados, arrayIdsPagina, (ArrayList)Session[GetIdSessionByFiltro(filtro)], ref existenFiltros);

            //if (idTabla == "Aduanas" && !existeDua)
            //    idGdv = "AduanaDUAs";

            bool hideTabExcel = false;
            List<OptionSelect> nuevosFiltros = null;
            List<object> listGridData = null;
            int cantReg = 0;
            var specificCulture = GetSpecificCulture();
           
            if (listaIdsSeleccionados.Count == 0)
            {
                if (!existenFiltros)
                {
                    objMensaje = new
                    {
                        titulo = Resources.AdminResources.Label_MyFilters,
                        mensaje = Resources.AdminResources.Message_Valida_AddFilterSearchButton,
                        flagContactenos = false
                    };
                }
                else
                {
                    objMensaje = new
                    {
                        existenFiltros
                    };
                }

            }
            else
            {
                Session.Remove("GdvSeleccionado");
                Session.Remove("IDsSeleccionados");

                if (filtro == Enums.Filtro.AduanaDUA.ToString() && !existeDua)
                    filtro = Enums.Filtro.Aduana.ToString();

                ArrayList auxIdsSeleccionados = new ArrayList();
                foreach (var item in listaIdsSeleccionados)
                {
                    auxIdsSeleccionados.Add(item);

                    numFiltrosExistentes++;
                    if (numFiltrosExistentes == LimiteFiltros)
                        break;
                }

                nuevosFiltros = FuncionesBusiness.AgregaFiltros(filtro, codPais, auxIdsSeleccionados);


                switch (filtro)
                {
                    case "Partida":
                        Session["PartidasB"] =
                            FuncionesBusiness.GuardaSeleccionados(auxIdsSeleccionados,
                                (ArrayList)Session["PartidasB"]);
                        //hdfCantPartidasB.Value = ((ArrayList)Session["PartidasB"]).Count.ToString();
                        break;
                    case "Marca":
                        Session["MarcasB"] =
                            FuncionesBusiness.GuardaSeleccionados(auxIdsSeleccionados,
                                (ArrayList)Session["MarcasB"]);
                        break;
                    case "PtoDescarga":
                        Session["PtosDescargaB"] =
                            FuncionesBusiness.GuardaSeleccionados(auxIdsSeleccionados,
                                (ArrayList)Session["PtosDescargaB"]);
                        break;
                    case "Modelo":
                        Session["ModelosB"] =
                            FuncionesBusiness.GuardaSeleccionados(auxIdsSeleccionados,
                                (ArrayList)Session["ModelosB"]);
                        break;
                    case "Importador":
                        Session["ImportadoresB"] =
                            FuncionesBusiness.GuardaSeleccionados(auxIdsSeleccionados,
                                (ArrayList)Session["ImportadoresB"]);
                        //hdfCantImportadoresB.Value = ((ArrayList)Session["ImportadoresB"]).Count.ToString();

                        break;
                    case "Exportador":
                        Session["ExportadoresB"] =
                            FuncionesBusiness.GuardaSeleccionados(auxIdsSeleccionados,
                                (ArrayList)Session["ExportadoresB"]);
                        //hdfCantExportadoresB.Value = ((ArrayList)Session["ExportadoresB"]).Count.ToString();
                        break;
                    case "Proveedor":
                        Session["ProveedoresB"] =
                            FuncionesBusiness.GuardaSeleccionados(auxIdsSeleccionados,
                                (ArrayList)Session["ProveedoresB"]);
                        //hdfCantProveedoresB.Value = ((ArrayList)Session["ProveedoresB"]).Count.ToString();
                        break;
                    case "ImportadorExp":
                        Session["ImportadoresExpB"] =
                            FuncionesBusiness.GuardaSeleccionados(auxIdsSeleccionados,
                                (ArrayList)Session["ImportadoresExpB"]);
                        //hdfCantImportadoresExpB.Value = ((ArrayList)Session["ImportadoresExpB"]).Count.ToString();
                        break;
                    case "PaisOrigen":
                        Session["PaisesOrigenB"] =
                            FuncionesBusiness.GuardaSeleccionados(auxIdsSeleccionados,
                                (ArrayList)Session["PaisesOrigenB"]);
                        break;
                    case "PaisDestino":
                        Session["PaisesDestinoB"] =
                            FuncionesBusiness.GuardaSeleccionados(auxIdsSeleccionados,
                                (ArrayList)Session["PaisesDestinoB"]);
                        break;
                    case "PaisEmbarque":
                        Session["PaisesEmbarqueB"] =
                            FuncionesBusiness.GuardaSeleccionados(auxIdsSeleccionados,
                                (ArrayList)Session["PaisesEmbarqueB"]);
                        break;
                    case "MarcaEC":
                        Session["MarcaECB"] =
                            FuncionesBusiness.GuardaSeleccionados(auxIdsSeleccionados,
                                (ArrayList)Session["MarcaECB"]);
                        break;
                    case "ViaTransp":
                        Session["ViasTranspB"] =
                            FuncionesBusiness.GuardaSeleccionados(auxIdsSeleccionados,
                                (ArrayList)Session["ViasTranspB"]);
                        break;
                    case "AduanaDUA":
                        Session["AduanaDUAsB"] =
                            FuncionesBusiness.GuardaSeleccionados(auxIdsSeleccionados,
                                (ArrayList)Session["AduanaDUAsB"]);
                        break;
                    case "Aduana":
                        Session["AduanasB"] =
                            FuncionesBusiness.GuardaSeleccionados(auxIdsSeleccionados,
                                (ArrayList)Session["AduanasB"]);
                        break;
                    case "Distrito":
                        Session["DistritosB"] =
                            FuncionesBusiness.GuardaSeleccionados(auxIdsSeleccionados,
                                (ArrayList)Session["DistritosB"]);
                        break;
                    case "PtoEmbarque":
                        Session["PtosEmbarqueB"] = FuncionesBusiness.GuardaSeleccionados(auxIdsSeleccionados,
                                (ArrayList)Session["PtosEmbarqueB"]);
                        break;
                    case "PtoDestino":
                        Session["PtosDestinoB"] = FuncionesBusiness.GuardaSeleccionados(auxIdsSeleccionados,
                                (ArrayList)Session["PtosDestinoB"]);
                        break;
                }
        
                Session["isVisibleInfoComplementario"] = Session["hdfPalabrasY"] == null && (Session["hdfNandinaB"] != null || Session["PartidasB"] != null) && Session["hdfImportadorB"] == null &&
                                                        Session["hdfExportadorB"] == null && Session["hdfProveedorB"] == null && Session["hdfImportadorExpB"] == null &&
                                                        Session["MarcasB"] == null && Session["ModelosB"] == null && Session["ImportadoresB"] == null && Session["ExportadoresB"] == null &&
                                                        Session["ProveedoresB"] == null && Session["ImportadoresExpB"] == null && Session["PaisesOrigenB"] == null && Session["PaisesDestinoB"] == null && 
                                                        Session["PaisesEmbarqueB"] == null && Session["PtosDescargaB"] == null &&
                                                        Session["MarcaECB"] == null && Session["ViasTranspB"] == null && Session["AduanaDUAsB"] == null && Session["AduanasB"] == null &&
                                                        Session["DistritosB"] == null && Session["PtosEmbarqueB"] == null && Session["PtosDestinoB"] == null && !(bool)(Session["codPaisB"] ?? false) && Extensiones.GetCookie("IsDemo") != "1";
                ActualizarSessionListaFitros(nuevosFiltros);

                var tabla = "";
                var dua = "";
                GetTablaAndDua(tipoOpe, codPais, ref tabla, ref dua);


                Session["SqlFiltro"] = GeneraSqlFiltro(codPais, codPais2, tipoOpe, anioMesIni, anioMesFin,
                    indexCboPaisB, codPaisB, dua, auxCodPais);
                //Extensiones.SetCookie("sl", Session["sqlFiltro"].ToString() + " $ " + Session["CodPais"].ToString() + " $ " + Session["TipoOpe"].ToString(), TimeSpan.FromDays(360));
                Extensiones.SetCookie("sl", "", TimeSpan.FromDays(360));
                string cifTot = "";
                string pesoNeto = "";
                string valueCifTot = "";

                decimal valuePesoNeto = 0;
                string unidad = "";
                GetCifTot_PesoNeto_CantReg_ValueCifTot_Unidad_ValuePesoNeto(codPais, tipoOpe, tabla, ref cifTot,
                    ref pesoNeto, ref cantReg, ref valueCifTot, ref unidad, ref valuePesoNeto);

                bool FlagPalabras = (!string.IsNullOrWhiteSpace(Session["hdfPalabrasY"]?.ToString()));
                //hideTabExcel = cantReg > CantRegMax && !FlagPalabras;
                hideTabExcel = Funciones.VisualizarExcel();

                listGridData = Busca(codPais, tipoOpe, cifTot, pesoNeto, idioma, dua, tabla, indexCboPaisB, valueCifTot,
                    valuePesoNeto, unidad,
                    cantReg, specificCulture, hideTabExcel);
            }

            return Json(new
            {
                maximoLimiteFiltros,
                existenFiltros,
                objMensaje,
                nuevosFiltros,
                totalRecordsFound = GetDescriptionRecordsFound(cantReg, specificCulture,idioma),
                gridData = listGridData,
                hideTabExcel = hideTabExcel,
                FlagRegMax = cantReg > CantRegMax
            });
        }

        private string GetIdSessionByFiltro(string filtro)
        {
            string idSession = "";
            switch (filtro)
            {
                case "Partida":
                    idSession = "PartidasB";
                    break;
                case "Marca":
                    idSession = "MarcasB";
                    break;
                case "Modelo":
                    idSession = "ModelosB";
                    break;
                case "Importador":
                    idSession = "ImportadoresB";
                    break;
                case "Exportador":
                    idSession = "ExportadoresB";
                    break;
                case "Proveedor":
                    idSession = "ProveedoresB";
                    break;
                case "ImportadorExp":
                    idSession = "ImportadoresExpB";
                    break;
                case "PaisOrigen":
                    idSession = "PaisesOrigenB";
                    break;
                case "PaisDestino":
                    idSession = "PaisesDestinoB";
                    break;
                case "PtoDescarga":
                    idSession = "PtosDescargaB";
                    break;
                case "ViaTransp":
                    idSession = "ViasTranspB";
                    break;
                case "MarcaEC":
                    idSession = "MarcaECB";
                    break;
                case "AduanaDUA":
                    idSession = "AduanaDUAsB";
                    break;
                case "Aduana":
                    idSession = "AduanasB";
                    break;
                case "Distrito":
                    idSession = "DistritosB";
                    break;
                case "Notificado":
                    idSession = "NotificadosB";
                    break;
                case "PaisEmbarque":
                    idSession = "PaisesEmbarqueB";
                    break;
                case "PtoEmbarque":
                    idSession = "PtosEmbarqueB";
                    break;
                case "PtoDestino":
                    idSession = "PtosDestinoB";
                    break;
                case "Manifiesto":
                    idSession = "ManifiestosB";
                    break;
            }
            return idSession;
        }

#region Agregar a favoritos registros seleccionados

        /// <summary>
        /// Obtener datos seleccionados a agregar a favoritos y lista grupos existentes
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="idTabla"></param>
        /// <param name="codPais">País Seleccionado</param>
        /// <param name="codPais2"></param>
        /// <param name="tipoOpe"></param>
        /// <param name="idsSeleccionados"></param>
        /// <param name="idsPagina"></param>
        /// <param name="idioma"></param>
        /// <returns></returns>
        [SessionExpireFilter]
        [HttpPost]
        public JsonResult AgregarAFavoritosClick(string filtro, string idTabla, string codPais,
            string codPais2, string tipoOpe, string idsSeleccionados,
            string idsPagina, string idioma)
        {
            ValidaCodPais2(codPais2, ref codPais);
            ValidaCodPaisManif(ref codPais, tipoOpe);
            bool flagCheckAlertas = true;
            var isManif = IsManifiesto(codPais);
            if ((idTabla == "ResumenProveedor" && tipoOpe =="I") || (idTabla == "ResumenImportadorExp" && tipoOpe == "E") || isManif || codPais == "PEP")
            {
                flagCheckAlertas = false;
            }
            

            string[] arrayIdsSeleccionados = idsSeleccionados.Split(',');
            string[] arrayIdsPagina = idsPagina.Split(',');


            ArrayList listaIdsSeleccionados = null;
            if (Session["GdvSeleccionado"] != null && Session["GdvSeleccionado"].ToString() == idTabla)
                listaIdsSeleccionados = (ArrayList)Session["IDsSeleccionados"];

            Session["GdvSeleccionado"] = idTabla;

            bool existeDua = GetExisteDua(GetExisteAduana(codPais), codPais);

            string idGdv = idTabla;
            if (idTabla == "AduanaDUAs" && !existeDua)
            {
                idGdv = "Aduanas";
            }

            listaIdsSeleccionados = FuncionesBusiness.GuardaSeleccionados(filtro, listaIdsSeleccionados,
                arrayIdsSeleccionados, arrayIdsPagina);

            //Session["IDsSeleccionados"] = FuncionesBusiness.GuardaSeleccionados(filtro, listaIdsSeleccionados, arrayIdsSeleccionados, arrayIdsPagina);

            if (idTabla == "Aduanas" && !existeDua)
                idGdv = "AduanaDUAs";

            object objMensaje = null;
            string tituloFavoritos = "";

            string lblAgregarAFavorito = "";
            List<OptionSelect> listaGrupos = null;

            string favoritosAagregar = "";

            if (listaIdsSeleccionados.Count == 0)
            {
                objMensaje = new
                {
                    titulo = Resources.AdminResources.Favorites_Word,
                    mensaje = Resources.AdminResources.Message_Valida_AddFilterSearchButton,
                    flagContactenos = false
                };
            }
            else
            {
                Session["TipoFavorito"] = filtro;
                Session["IdTablaSelected"] = idTabla;

                AgregarFavoritoTable objAgregarFavoritoTable =
                    GetAgregarFavoritoTable(filtro, codPais, idioma, listaIdsSeleccionados);
                favoritosAagregar = RenderViewToString(this.ControllerContext, "_TablaAgregarFavoritos",
                    objAgregarFavoritoTable);

                switch (filtro)
                {
                    case "Partida":
                        tituloFavoritos = Resources.Resources.Demo_Products_Tab;
                        lblAgregarAFavorito = Resources.AdminResources.Add_ToMyProducts_Button;
                        break;
                    case "Importador":
                    case "ImportadorExp":
                        tituloFavoritos = Resources.Resources.Demo_Importers_Tab;
                        lblAgregarAFavorito = Resources.AdminResources.Add_ToMyImporters_Button;
                        break;

                    case "Exportador":
                    case "Proveedor":
                        tituloFavoritos = codPais.Equals("CL") ? Resources.Resources.Demo_Brands_Tab : Resources.Resources.Demo_Exporters_Tab;
                        lblAgregarAFavorito = codPais.Equals("CL") ? Resources.AdminResources.Add_ToMyBrands_Button : Resources.AdminResources.Add_ToMyExporters_Button;
                        break;
                }

                string codPaisT = codPais;
                if (codPais == "PEB" || codPais == "PEP")
                    codPaisT = "PE";
                else if (codPais2 == "4UE")
                    codPaisT = "UE" + codPais;


                DataTable dt = FuncionesBusiness.LlenaGrupos(false, Session["IdUsuario"].ToString(), codPaisT, tipoOpe,
                    filtro, idioma);
                listaGrupos = dt.AsEnumerable().Select(m => new OptionSelect()
                {
                    value = Convert.ToString(m.Field<Int32>("IdGrupo")),
                    text = m.Field<string>("Grupo")
                }).ToList();
            }

            return Json(new
            {
                objMensaje,
                tituloFavoritos,
                lblAgregarAFavorito,
                favoritosAagregar,
                listaGrupos,
                flagCheckAlertas
            });
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult AgregarAFavClick(bool checkedAgregarAFavorito, bool checkedCrearGrupo,
            bool checkedAgregarAGrupo, bool checkedAgregarAlerta,
            string textNuevoGrupo, string codPais, string codPais2,
            string tipoOpe, string valueCboGrupos, string idsSeleccionados,
            string textosPartidaFav, string idioma)
        {
            string auxCodPais = codPais;
            ValidaCodPais2(codPais2, ref codPais);
            ValidaCodPaisManif(ref codPais, tipoOpe);

            string tipoFavorito = Session["TipoFavorito"].ToString();
            string mensaje = "";

            ArrayList IDsSeleccionados = (ArrayList)Session["IDsSeleccionados"];

                var tipoFav = tipoFavorito != "ImportadorExp" ? tipoFavorito.Substring(0, 2) : "IE";

            string idUsuario = Session["IdUsuario"].ToString();

            object objMensaje = null;

            bool flagSave = false;
            if (checkedCrearGrupo &&
                FuncionesBusiness.ExisteGrupo(textNuevoGrupo, idUsuario,
                    ((codPais != "PEB" && codPais != "PEP") ? codPais : "PE"), tipoOpe, tipoFav))
            {
                mensaje = "Ya existe el grupo: " + textNuevoGrupo;
                if (idioma == "en")
                    mensaje = "Group: " + textNuevoGrupo + " already exists";
            }
            else
            {
                ArrayList IDsSeleccionados1 = new ArrayList();
                string[] arrayIdsSeleccionados = idsSeleccionados.Split(',');

                foreach (var t in arrayIdsSeleccionados)
                {
                    IDsSeleccionados1.Add(t);
                }

                bool flagOk1 = true, flagOk2 = true, flagOkA = true;
                string Mensaje1 = "", Mensaje2 = "", Mensaje3 = "";
                var limiteFavUnicos = 0;
                var cantFavUnicos = 0;
                var cantFavSel = 0;
                var idPlan = Funciones.ObtieneIdPlan(idUsuario);
                if (checkedAgregarAFavorito)
                {
                    //idPlan = Funciones.ObtieneIdPlan(idUsuario);
                    limiteFavUnicos = Funciones.ObtieneLimite(idPlan, "LimiteFavUnicos");
                    cantFavUnicos = FuncionesBusiness.CantFavUnicos(idUsuario, codPais, tipoOpe, tipoFavorito);
                    //IDsSeleccionados = (ArrayList)Session["IDsSeleccionados"];                    

                    foreach (var item in IDsSeleccionados1)
                    {
                        if (!FuncionesBusiness.ExisteFavUnico(idUsuario, codPais, tipoOpe, tipoFavorito,
                            item.ToString()))
                            cantFavSel += 1;
                    }

                    if (cantFavUnicos + cantFavSel > limiteFavUnicos)
                    {
                        mensaje = Resources.AdminResources.MessageLimitPlanFavorites;
                    }

                    if (mensaje == "")

                    {
                        string codPaisT = codPais;
                        if (codPais == "PEB" || codPais == "PEP")
                            codPaisT = "PE";
                        else if (codPais2 == "4UE")
                            codPaisT = "UE" + auxCodPais;

                        flagOk1 = FuncionesBusiness.AgregaFavoritos(idUsuario, codPaisT, tipoOpe, tipoFavorito,
                            IDsSeleccionados1, idioma, ref Mensaje1);

                        if (textosPartidaFav != "")

                        {
                            string[] arrayTextosPartidaFav = textosPartidaFav.Split(',');
                            if (arrayTextosPartidaFav.Length > 0)
                            {
                                foreach (string item in arrayTextosPartidaFav)
                                {
                                    var indexSeparador = item.IndexOf("|", StringComparison.Ordinal);
                                    var txtPartida = item.Substring(indexSeparador + 1);

                                    FuncionesBusiness.ActualizaPartidaFavorita(idUsuario, codPaisT, tipoOpe,
                                        item.Substring(0, (item.Length - txtPartida.Length - 1)), txtPartida);
                                }
                            }
                        }
                    }

                    if (mensaje == "" && checkedAgregarAlerta)

                    {
                        var idPlanA = Funciones.ObtieneIdPlan(idUsuario);
                        string tipo = tipoFavorito == "Partida" ? "AMP" : "AMC";

                        var limitAlert = FuncionesBusiness.GetLimitAlert(idPlan,tipo);

                        string codPaisT = codPais;
                        string TipoFavorito = tipoFavorito == "Partida" ? "Producto" : "Compañia";
                        string tipofiltro = TipoFavorito == "Producto" ? Enums.VarId.ALERT_MP.GetDn() :Enums.VarId.ALERT_MC.GetDn();

                        string[] arrayIdsSeleccionadosA = idsSeleccionados.Split(',');

                        for (int i = 0; i < arrayIdsSeleccionadosA.Length; i++)
                        {
                            //FuncionesBusiness.GuardarAlertFavorito(tipofiltro, idUsuario, codPaisT,
                            //    arrayIdsSeleccionadosA[i], "null", tipoOpe);
                            FuncionesBusiness.GrabarMisBusquedasAlerta(limitAlert, arrayIdsSeleccionadosA.Length,tipofiltro,idUsuario,
                                codPaisT,arrayIdsSeleccionadosA[i], tipoOpe, ref Mensaje3, ref flagSave);
                        }

                    }
                }
                else
                {
                    if (mensaje == "" && checkedAgregarAlerta  )

                    {
                        bool existeFavorito = true;
                        
                        var idPlanA = Funciones.ObtieneIdPlan(idUsuario);
                        string tipo = tipoFavorito == "Partida" ? "AMP" : "AMC";

                        var limitAlert = FuncionesBusiness.GetLimitAlert(idPlan, tipo);

                        string codPaisT = codPais;
                        string TipoFavorito = tipoFavorito == "Partida" ? "Producto" : "Compañia";
                        string tipofiltro = TipoFavorito == "Producto" ? Enums.VarId.ALERT_MP.GetDn() : Enums.VarId.ALERT_MC.GetDn();

                        string[] arrayIdsSeleccionadosA = idsSeleccionados.Split(',');

                        for (int i = 0; i < arrayIdsSeleccionadosA.Length; i++)
                        {
                            //FuncionesBusiness.GuardarAlertFavorito(tipofiltro, idUsuario, codPaisT,
                            //    arrayIdsSeleccionadosA[i], "null", tipoOpe);
                            if (tipoFavorito == "Partida")
                            {
                                existeFavorito = FuncionesBusiness.ExisteFavorito(tipoFavorito, idUsuario, codPais, tipoOpe, arrayIdsSeleccionadosA[i]);
                            }
                            else
                            {
                                existeFavorito = FuncionesBusiness.ExisteFavUnico(idUsuario,codPaisT,tipoOpe,tipoFavorito, arrayIdsSeleccionadosA[i]);
                            }
                            if (existeFavorito)
                            {
                                FuncionesBusiness.GrabarMisBusquedasAlerta(limitAlert, arrayIdsSeleccionadosA.Length, tipofiltro, idUsuario,
                                codPaisT, arrayIdsSeleccionadosA[i], tipoOpe, ref Mensaje3,ref flagSave);
                            }
                            else if(tipoFavorito == "Partida")
                            {
                                Mensaje3 = Resources.AdminResources.MesExistsAlertPr_Text;//"Debe agregar a mis productos para poder registrar la alerta.";
                            }
                            else
                            {
                                if (tipoOpe == "I")
                                    Mensaje3 = Resources.AdminResources.MesExistsAlertIm_Text;//"Debe agregar a mis importadores para poder registrar la alerta";
                                else
                                    Mensaje3 = Resources.AdminResources.MesExistsAlertEx_Text; //"Debe agregar a mis exportadores para poder registrar la alerta";
                            }
                            
                        }

                    }
                }

                if(Mensaje3 != "" && !flagSave)
                {
                    mensaje = Mensaje3;
                }

                if (mensaje == "")
                {
                    if (checkedCrearGrupo || checkedAgregarAGrupo)
                    {
                        limiteFavUnicos = Funciones.ObtieneLimite(idPlan, "LimiteFavUnicos");
                        cantFavUnicos = FuncionesBusiness.CantFavUnicos(idUsuario, codPais, tipoOpe, tipoFavorito);

                        if (cantFavUnicos + cantFavSel >= limiteFavUnicos)
                        {
                            mensaje = Resources.AdminResources.MessageLimitPlanFavorites;
                        }
                        else
                        {
                            string codPaisT = codPais;
                            if (codPais == "PEB" || codPais == "PEP")
                                codPaisT = "PE";
                            else if (codPais2 == "4UE")
                                codPaisT = "UE" + auxCodPais;

                            flagOk2 = FuncionesBusiness.ActualizaGrupo(idUsuario, codPaisT, tipoOpe, tipoFavorito,
                                checkedCrearGrupo, textNuevoGrupo, valueCboGrupos, IDsSeleccionados1, idioma,
                                ref Mensaje2);
                        }
                    }


                    Session.Remove("IDsSeleccionados");
                    Session.Remove("GdvSeleccionado");

                    if (mensaje == "")
                    {
                        string mensaje2 = "";

                        if(Mensaje2 == "")
                        {
                            mensaje2 = Mensaje1 + "<br>" + Mensaje3;
                        }
                        else
                        {
                            mensaje2 = Mensaje1 + "<br>" + Mensaje2 + "<br>" + Mensaje3;
                        }

                        objMensaje = new
                        {
                            titulo = Resources.AdminResources.Favorites_Word,
                            mensaje = mensaje2,
                            flagContactenos = !flagOk1 || !flagOk2
                        };
                    }
                }
            }

            return Json(new
            {
                mensaje,
                objMensaje,
                idTabla = Session["IdTablaSelected"]?.ToString() ?? ""
            });
        }

        private AgregarFavoritoTable GetAgregarFavoritoTable(string filtro, string codPais, string idioma,
           ArrayList listaIdsSeleccionados)
        {
            List<MiFavorito> listaFavoritos = new List<MiFavorito>();
            string nandina = "", favorito = "";
            foreach (string id in listaIdsSeleccionados)
            {
                switch (filtro)
                {
                    case "Partida":
                        nandina = FuncionesBusiness.BuscaNandina(id, codPais);
                        favorito = FuncionesBusiness.BuscaPartida2(id, codPais, idioma);
                        break;
                    case "Importador":
                    case "Exportador":
                        favorito = Funciones.BuscaEmpresa(id, codPais);
                        break;
                    case "Proveedor":
                        favorito = Funciones.BuscaProveedor(id, codPais);
                        break;
                    case "ImportadorExp":
                        favorito = Funciones.BuscaImportadorExp(id, codPais);
                        break;
                }

                listaFavoritos.Add(new MiFavorito()
                {
                    IdFavorito = Convert.ToInt32(id),
                    Favorito = favorito,
                    Nandina = nandina
                });
            }

            AgregarFavoritoTable objAgregarFavoritoTable = new AgregarFavoritoTable();
            objAgregarFavoritoTable.ListaFavoritos = listaFavoritos;
            objAgregarFavoritoTable.ObjTableHead = new AgregarFavoritoTableHead()
            {
                Nandina = Resources.Resources.Nandina_FormField_Label,
                Favorito = Resources.AdminResources.Favorite_Text,
                NombreFavorito = Resources.AdminResources.FavoriteName_Text,
                IsVisibleNandina = (filtro == "Partida"),
                IsVisbleNombreFavorito = (filtro == "Partida")
            };

            return objAgregarFavoritoTable;
        }
#endregion

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult OrdenarTabla(string filtro, Enums.TipoFiltro tipoFiltro, string orden,
            string codPais, string tipoOpe, int indexCboPaisB,string anioMesIni,string anioMesFin,string codPaisB)
        {
            string auxCodPais = codPais;
            GuardarLogInicioErrorSession("OrdenarTabla()");
            string auxCodPais2 = Session["CodPais2"].ToString(); // al cambiar cboPais se actualiza esta variable de sesión
            ValidaCodPais2(auxCodPais2, ref codPais);
            ValidaCodPaisManif(ref codPais, tipoOpe);
            bool IsManifiesto = this.IsManifiesto(codPais);


            string idioma = GetCurrentIdioma();
            var tabla = "";
            var dua = "";
            GetTablaAndDua(tipoOpe, codPais, ref tabla, ref dua);

            int cantReg = 0;
            string cifTot = "";
            string pesoNeto = "";
            string valueCifTot = "";

            decimal valuePesoNeto = 0;
            string unidad = "";

            decimal valueCifTotMarcaModelo = 0;

            if (filtro == Enums.Filtro.Marca.ToString() || filtro == Enums.Filtro.Modelo.ToString())
            {
                cifTot = Funciones.Incoterm(codPais, tipoOpe) + "Tot";
                pesoNeto = Funciones.CampoPeso(codPais, tipoOpe);

                DataRow drTotales =
                    FuncionesBusiness.CalculaTotales(Session["SqlFiltro"].ToString(), cifTot, codPais, pesoNeto, tabla, true, isManif: IsManifiesto);
                valueCifTotMarcaModelo = drTotales.GetValue<decimal>(cifTot);
            }else if (filtro == "InfoTabla")
            {
                string sql = Session["sqlTabInfoComplementaria"].ToString();
            }
            else
            {
                GetCifTot_PesoNeto_CantReg_ValueCifTot_Unidad_ValuePesoNeto(codPais, tipoOpe, tabla, ref cifTot,
                    ref pesoNeto, ref cantReg, ref valueCifTot, ref unidad, ref valuePesoNeto);
            }
            var listaAux = new List<DataTable>();
            if (filtro == Enums.Filtro.InfoTabla.ToString())
            {
                cifTot = "ciftot";
                pesoNeto = "pesoneto";
                listaAux.Add(FuncionesBusiness.ListaInfoTabla(Session["sqlTabInfoComplementaria"].ToString(), 1, TabGridPageSize, false,order: orden));
            }
            else
            {
                listaAux = CargaFiltro(filtro, -1, codPais, cifTot, pesoNeto, idioma, dua, tabla,
                indexCboPaisB, tipoFiltro, orden);
            }

            


             

            bool ordenarTabla = false;
            string filasOrdenas = "";
            if (listaAux != null && listaAux[0] != null && listaAux[0].Rows.Count > 1)
            {
                ordenarTabla = true;
                TabData objTabData = new TabData();
                objTabData.Filtro = filtro;
                objTabData.CodPais = codPais;
                if (filtro == Enums.Filtro.InfoTabla.ToString())
                {
                    objTabData.IsVisibleCheck = false;
                }
                objTabData.IsVisibleInfoComplementario = objTabData.IsVisibleInfoComplementario && _paisesConInfoComplementaria.Contains(codPais);
                if (tipoFiltro == Enums.TipoFiltro.Resumen)
                {
                    if (filtro == Enums.Filtro.InfoTabla.ToString())
                    {
                        valueCifTot = FuncionesBusiness.TotalInfoComplementario(Session["sqlFiltroTablas"].ToString());
                    }
                    objTabData.ListRows =
                        GetDataTableToListGridRow(listaAux[0], filtro, cifTot, valueCifTot, codPais, GetSpecificCulture(),
                        anioMesIni:anioMesIni,anioMesFin:anioMesFin,indexCboPaisB:indexCboPaisB,codPaisB:codPaisB);
                    filasOrdenas = RenderViewToString(this.ControllerContext, "GridViews/GridRowView", objTabData);
                }
                else
                {
                   
                    if (filtro == Enums.Filtro.Marca.ToString() || filtro == Enums.Filtro.Modelo.ToString())
                    {
                        objTabData.GridHead.IsVisblePrecio = false;
                        objTabData.GridHead.IsVisibleTotalKg = false;
                        objTabData.GridHead.IsVisibleColumnCheck = false;
                        objTabData.ListRowsTab = GetListGridRowMarcasModelos(listaAux[0], filtro, valueCifTotMarcaModelo, GetSpecificCulture());
                        filasOrdenas = RenderViewToString(this.ControllerContext, "GridViews/TabGridRowView", objTabData);
                    }
                    else
                    {
                        if (filtro == Enums.Filtro.AduanaDUA.ToString() || filtro == Enums.Filtro.Aduana.ToString())
                        {
                            if (filtro == Enums.Filtro.Aduana.ToString())
                                objTabData.GridHead.IsVisibleDuas = false;

                            objTabData.ListRowsTab =
                                GetListGridRowAduanaDua(listaAux[0], filtro, cifTot, GetSpecificCulture());
                            filasOrdenas = RenderViewToString(this.ControllerContext, "GridViews/AduanaDuaGridRowView", objTabData);
                        }
                        else
                        {
                            if (filtro == Enums.Filtro.InfoTabla.ToString())
                            {
                                valueCifTot = FuncionesBusiness.TotalInfoComplementario(Session["sqlTabInfoComplementaria"].ToString());
                                objTabData.ListRowsTab = GetDataTableToListGridRowTab(listaAux[0], filtro, cifTot, valueCifTot, codPais, GetSpecificCulture(), pesoNeto, unidad);
                            }
                            else
                            {
                                objTabData.ListRowsTab = GetDataTableToListGridRowTab(listaAux[0], filtro, cifTot, valueCifTot, codPais,
                                    GetSpecificCulture(), pesoNeto, unidad,
                                    anioMesIni: anioMesIni, anioMesFin: anioMesFin, indexCboPaisB: indexCboPaisB, codPaisB: codPaisB, auxCodPais: auxCodPais);
                            }
                            
                            objTabData.GridHead.IsVisblePrecio = unidad != "";
                            objTabData.GridHead.IsVisibleTotalKg = unidad != "";
                            filasOrdenas = RenderViewToString(this.ControllerContext, "GridViews/TabGridRowView", objTabData);
                        }
                    }
                }
            }

            return Json(new
            {
                ordenarTabla,
                filasOrdenas
            });
        }

#region VerRegistrosEnModal
        [SessionExpireFilter]
        [HttpPost]
        public JsonResult VerRegistros(string filtro, string idregistro, string tipoOpe,
            string codPais, string codPais2, string anioMesIni,
            string anioMesFin, int indexCboPaisB, string codPaisB,string filtros, string codPais3,
            int numFiltrosExistentes)

        {
            var excelHabilitado = true;
            GuardarLogInicioErrorSession("VerRegistros()");
            Session["filtro"] = filtro;
            if(filtro == "InfoTabla" || filtro == "InfoComplementario")
            {
                tipoOpe = tipoOpe == "I" ? "E" : "I";
            }
            bool maximoLimiteFiltros = false;
            if (numFiltrosExistentes == LimiteFiltros)
            {
                maximoLimiteFiltros = true;
            }

            if(filtro == "InfoComplementario" || filtro == "InfoTabla")
            {
                if (int.TryParse(codPais, out int n))
                {
                    string codPaisConsulta = Funciones.BuscaCodPais(codPais, idregistro);
                    if (codPaisConsulta != null || codPaisConsulta.Trim() != "")
                    {
                        excelHabilitado = Funciones.ValidaPais(Session["IdUsuario"].ToString(), codPaisConsulta);
                    }
                }
                
                codPais = idregistro.ToUpper();
                Session["CodPaisComplementario"] = codPais;
                codPais2 = new ListaPaises().BuscarCodPais2(codPais);
            }

            string auxCodPais = codPais;
            if(filtro != "InfoComplementario" && filtro != "InfoTabla")
            {
                ValidaCodPais2(codPais2, ref codPais);
                
            }
            ValidaCodPaisManif(ref codPais, tipoOpe);
            //hlkArchivo.Visible = hlkArchivo2.Visible = false;
            string idioma = GetCurrentIdioma();
            var isManif = IsManifiesto(codPais);
            
            //if (filtro == Enums.Filtro.AduanaDUA.ToString()
            //    && !GetExisteDua(GetExisteAduana(codPais), codPais))
            //{
            //    filtro = Enums.Filtro.Aduana.ToString();
            //}

            if (filtro == Enums.Filtro.AduanaDUA.ToString() && !idregistro.Contains("-"))
            {
                filtro = Enums.Filtro.Aduana.ToString();
            }

            bool lnkAgregarFiltroSelVisible = (filtro != "Marca" && filtro != "Modelo" && filtro != "InfoComplementario" && filtro != "InfoTabla");
            bool cboDescargas2Visible = (filtro != "Marca" && filtro != "Modelo");

            string[] filtroCondicion = { "Partida", "Importador", "Exportador", "Proveedor", "ImportadorExp" };

            bool lnkAgregarSelAFavoritosVisible = filtroCondicion.Contains(filtro);

            var tabla = "";
            var dua = "";
            GetTablaAndDua(tipoOpe, codPais, ref tabla, ref dua);

            if (filtro != "Manifiesto" &&  filtro != Enums.Filtro.AduanaDUA.ToString())
            {
                Session["hdfVariable"] = "Id" + (filtro == "MarcaEC" ? "Marca" : filtro);
                Session["hdfValor"] = idregistro.ToUpper();
            }
            else if (filtro == "Manifiesto")
            {
                Session["hdfVariable"] = "Manifiesto";
                Session["hdfValor"] = "'" + idregistro.ToUpper() + "'";
            }
            else
            {
                Session["hdfVariable"] = "convert(varchar(3), IdAduana) + '-' + convert(varchar(20), " + dua + ")";
                Session["hdfValor"] = "'" + idregistro.ToUpper() + "'";
            }

            Session["hdfDesComercialBB2"] = null;
            Session["hdfDUAB2"] = null;

            Session["hdfFiltroSel"] = (filtro == "MarcaEC" ? "Marca" : filtro);
            Session["hdfIDSel"] = idregistro.ToUpper();

            string lblFiltroSel = "";
            string lnkAgregarSelAFavoritos = "";


            string hdfValor = Session["hdfValor"].ToString();
            string textoInfoComplementario = "";
            switch (filtro)
            {
                case "Partida":
                    string partida = FuncionesBusiness.BuscaPartida(hdfValor, codPais, idioma);
                    lblFiltroSel = Resources.AdminResources.Nandina2_FilterText + " : " +
                                   ((partida.Length > 80) ? partida.Substring(0, 80) : partida);
                    lnkAgregarSelAFavoritos = Resources.AdminResources.Add_ToMyProducts_Button;
                    break;
                case "Notificado":
                    lblFiltroSel = Resources.Resources.Demo_Notif_Tab + ": " + Funciones.BuscaNotificado(hdfValor, codPais);
                    break;
                case "Marca":
                    lblFiltroSel = Resources.Resources.Search_Form_BrandField + " : " + Funciones.BuscaMarca(hdfValor);//Funciones.BuscaMarca(hdfValor, codPais);
                    break;
                case "Modelo":
                    lblFiltroSel = Resources.AdminResources.Model_FormField_Label + " : " + Funciones.BuscaModelo(hdfValor);// Funciones.BuscaModelo(hdfValor, codPais);
                    break;
                case "Importador":
                    lblFiltroSel = Resources.Resources.Search_Form_Item05 + " : " +
                                   Funciones.BuscaEmpresa(hdfValor, codPais);
                    lnkAgregarSelAFavoritos = Resources.AdminResources.Add_ToMyImporters_Button;
                    break;
                case "Exportador":
                    lblFiltroSel = Resources.Resources.Search_Form_Item06 + " : " +
                                   Funciones.BuscaEmpresa(hdfValor, codPais);
                    lnkAgregarSelAFavoritos = Resources.AdminResources.Add_ToMyExporters_Button;
                    break;
                case "Proveedor":
                    lblFiltroSel = Resources.Resources.Search_Form_Item06 + " : " +
                                   Funciones.BuscaProveedor(hdfValor, codPais);
                    lnkAgregarSelAFavoritos = Resources.AdminResources.Add_ToMyExporters_Button;
                    break;
                case "ImportadorExp":
                    lblFiltroSel = Resources.Resources.Search_Form_Item05 + " : " +
                                   Funciones.BuscaImportadorExp(hdfValor, codPais);
                    lnkAgregarSelAFavoritos = Resources.AdminResources.Add_ToMyImporters_Button;
                    break;
                case "PaisOrigen":
                    lblFiltroSel = Resources.AdminResources.OriginCountry_FormField_Label + " : " +
                                   Funciones.BuscaPais(hdfValor, codPais);
                    
                    break;
                case "PaisDestino":
                    lblFiltroSel = Resources.AdminResources.DestinationCountry_FormField_Label + " : " +
                                   Funciones.BuscaPais(hdfValor, codPais);
                    break;
                case "MarcaEC":
                    lblFiltroSel = Resources.Resources.Search_Form_BrandField + " : " +
                                   Funciones.BuscaMarcaEC(hdfValor, codPais);
                    break;
                case "ViaTransp":
                    lblFiltroSel = Resources.AdminResources.FilterText_ViaTransp + " : " +
                                   Funciones.BuscaVia(hdfValor, codPais);
                    break;
                case "AduanaDUA":
                    lblFiltroSel = Resources.AdminResources.FilterText_Aduana + " - DUA : " +
                                   Funciones.BuscaAduana(idregistro.Split('-')[0], codPais) + " - " +
                                   idregistro.Split('-')[1];
                    break;
                case "Aduana":
                    lblFiltroSel = Resources.AdminResources.FilterText_Aduana + " : " +
                                   Funciones.BuscaAduana(hdfValor, codPais);
                    break;
                case "Distrito":
                    lblFiltroSel = Resources.AdminResources.FilterText_District + " : " +
                                   Funciones.BuscaDistrito(hdfValor, codPais);
                    break;
                case "PaisEmbarque":
                    lblFiltroSel = Resources.Resources.Pais_Embarque + ": " + Funciones.BuscaPais(hdfValor, codPais);
                    break;
                case "PtoDescarga":
                    lblFiltroSel = Resources.Resources.Demo_PtoDescarga_Tab + ": " + Funciones.BuscaPuerto(hdfValor, codPais);
                    break;
                case "PtoEmbarque":
                    lblFiltroSel = Resources.Resources.Demo_PtoEmbarque_Tab + ": " + Funciones.BuscaPuerto(hdfValor, codPais);
                    break;
                case "PtoDestino":
                    lblFiltroSel = Resources.Resources.Demo_PtoDestino_Tab + ": " + Funciones.BuscaPuerto(hdfValor, codPais);
                    break;
                case "Manifiesto":
                    lblFiltroSel = Resources.Resources.Demo_Manifiesto_Tab + ": " + hdfValor.Replace("'", "");
                    break;
                case "InfoComplementario":
                    
                case "InfoTabla":
                    
                    string pais = FuncionesBusiness.BuscarPaisNombre(codPais, idioma);
                    lblFiltroSel = "<u>" + Resources.AdminResources.LabelComplementaryInformation_Text + ": "+ pais + " - " + 
                                    (tipoOpe == "I" ? Resources.AdminResources.Imports_Text : Resources.AdminResources.Exports_Text) + "</u>";
                    lblFiltroSel += "<br>"+filtros;
                    lblFiltroSel += "<br>" + Resources.AdminResources.Period_Text + ": " + GetRangoFechas(anioMesIni, anioMesFin, idioma) + "<br>";
                    textoInfoComplementario = Resources.AdminResources.InfoTabla_Modal1 + " <b>" + pais + "</b>" + Resources.AdminResources.InfoTabla_Modal2;
                    break;
            }

            var cifTot = GetCIFTot(codPais, tipoOpe); //Funciones.Incoterm(codPais, tipoOpe) + "Tot";
            var pesoNeto = Funciones.CampoPeso(codPais, tipoOpe); //
            if(filtro == "InfoTabla")
            {
                string sql = Session["SqlInfoComplementario" + codPais].ToString();

                Session["SqlFiltroR2"] = sql;
            }
            else
            {
                string sqlAux = GeneraSqlFiltroR2(tipoOpe, codPais, codPais2, anioMesIni, anioMesFin,
                indexCboPaisB, codPaisB, dua, auxCodPais, filtro: filtro);

                if(filtro == "InfoComplementario")
                {
                    string codPaisAux = Session["CodPais"].ToString();

                    string nombrePais = FuncionesBusiness.BuscarPaisNombre(codPaisAux, idioma);

                    if(codPaisAux == "UE")
                    {
                        nombrePais = FuncionesBusiness.BuscarPaisUE(codPais3, idioma);
                    }

                    if (nombrePais == "USA")
                    {
                        nombrePais = "ESTADOS UNIDOS";
                    }

                    int idPais = FuncionesBusiness.BuscarIdPaisNombre(nombrePais, codPais);
                    if (nombrePais == "Alemania" && codPais == "EC")
                    {
                        idPais = 53;
                    }
                    if (nombrePais == "ESTADOS UNIDOS" && codPais == "EC")
                    {
                        idPais = 158;
                    }
                    // Ruben 202310
                    else if (nombrePais == "ESTADOS UNIDOS" && codPais == "MXD")
                    {
                        idPais = 321;
                    }

                    if (idPais != 0)
                    {
                        Session["SqlFiltroR2"] = sqlAux + (tipoOpe == "I" ? " and idPaisOrigen = " : " and idPaisDestino = ") + idPais + " ";
                    }
                    else
                    {
                        Session["SqlFiltroR2"] = sqlAux;
                    }
                }
                else
                {
                    Session["SqlFiltroR2"] = sqlAux;
                }           

            }
             /*Session["SqlFiltroR2"] = GeneraSqlFiltroR2(tipoOpe, codPais, codPais2, anioMesIni, anioMesFin,
                indexCboPaisB, codPaisB, dua, auxCodPais, filtro:filtro);*/
            //Session["hdfVariable"] = null;
            DataRow drTotales =
                FuncionesBusiness.CalculaTotales(Session["SqlFiltroR2"].ToString(), cifTot, codPais, pesoNeto, tabla, isManif: isManif, filtro:filtro);
            object objMensaje = null;
            string lblRecordsFound = "";
            if(drTotales != null)
            {
                bool FlagPalabras = (!string.IsNullOrWhiteSpace(Session["hdfPalabrasY"]?.ToString()));
                bool hideTabExcel = (Convert.ToInt32(drTotales["CantReg"]) > CantRegMax && !FlagPalabras);

                if (hideTabExcel && filtro != "InfoTabla" && filtro != "InfoComplementario")
                {
                    string mensaje = "Su búsqueda supera el límite de " + CantRegMax.ToString("n0") +
                                 " registros y no puede ser descargada en Excel. ";
                    mensaje +=
                        "Si desea ver todas las pestañas habilitadas y/ó descargar en Excel, reduzca el rango de fechas y/o modifique sus filtros de búsqueda";

                    if (idioma == "en")
                    {
                        mensaje = "Your search exceeds " + CantRegMax.ToString("n0") +
                                  " records limit and it can not be download to Excel. ";
                        mensaje +=
                            "If you want to see all tabs enabled and/or download to Excel, reduce the dates range and/or modify your filters search";
                    }

                    objMensaje = new
                    {
                        titulo = Resources.Resources.Search_Button,
                        mensaje,
                        flagContactenos = false,
                    };
                    return Json(new
                    {
                        objMensaje
                    });
                }

                if (idioma == "es")
                {
                    lblRecordsFound = "Se encontraron " + Convert.ToInt32(drTotales["CantReg"]).ToString("n0") +
                                        " registros";
                }
                else
                {
                    lblRecordsFound = Convert.ToInt32(drTotales["CantReg"]).ToString("n0") + " records were found";
                }
            }
            

            string sqlFinal = "";

            

            
            string idDescargaDefault = "";
            EnumerableRowCollection<OptionSelect> optionsDescargas = null;
            string tablaVerRegistro = "";
            int totalPages = 0;

            MiBusqueda objMiBusqueda = new MiBusqueda
            {
                FlagVarVisibles = new TabMisBusquedas(tipoOpe, codPais),    //new FlagVarVisibles(codPais, tipoOpe, isManif),
                Importador = Funciones.ExisteVariable(codPais, tipoOpe, "IdImportador"),
                Proveedor = Funciones.ExisteVariable(codPais, tipoOpe, "IdProveedor"),
                PaisOrigen = Funciones.ExisteVariable(codPais, tipoOpe, "IdPaisOrigen"),
                Exportador = Funciones.ExisteVariable(codPais, tipoOpe, "IdExportador"),
                ImportadorExp = Funciones.ExisteVariable(codPais, tipoOpe, "IdImportadorExp"),
                ExistePaisDestino = Funciones.ExisteVariable(codPais, tipoOpe, "IdPaisDestino"),
                CampoPeso = Funciones.CampoPeso(codPais, tipoOpe),
                FlagDescComercialB = Funciones.FlagDesComercial(codPais, tipoOpe),
                Cif = GetCIFTot(codPais, tipoOpe), //Funciones.Incoterm(codPais, tipoOpe),
                TipoOpe = tipoOpe,
                CodPais = codPais,
                CodPais2 = codPais2
            };

            bool existeAduana = GetExisteAduana(codPais);
            bool existeDua = GetExisteDua(existeAduana, codPais);

            objMiBusqueda.ExisteAduana = existeAduana;
            objMiBusqueda.ExisteDua = existeDua;
            objMiBusqueda.Dua = dua;

            string sqlCampos = "";

            if (filtro == "InfoTabla" || filtro == "InfoComplementario")
            {
                //objMiBusqueda.ExisteDua = objMiBusqueda.ExisteDua && objMiBusqueda.CodPais.ToUpper() != "EC";
                if (!objMiBusqueda.FlagVarVisibles.IsManifiesto)
                {
                    var isVisibleFobOFasUnit = (objMiBusqueda.Cif != "FOB"
                                                && objMiBusqueda.CodPais != "CN"
                                                && objMiBusqueda.CodPais != "MXD"
                                                && objMiBusqueda.CodPais != "IN");
                    var dataFieldFobOFasUnit = objMiBusqueda.CodPais != "US" ? "FOBUnit" : "FASUnit";

                    var cif = objMiBusqueda.Cif.ToLower().Replace("tot", "");
                    var dataFieldCifOFobUnit = cif + "Unit";
                    var flagFormatoB = ((Session["SqlFiltroR2"].ToString().Contains("IdMarca") && filtro != "MarcaEC") || Session["SqlFiltroR2"].ToString().Contains("IdModelo"));

                    var isVisibleCifImptoUnit = (objMiBusqueda.CodPais == "PE");

                    if (idioma == "es")
                    {
                        isVisibleCifImptoUnit = (!flagFormatoB && isVisibleCifImptoUnit);
                    }

                    var isVisibleDistrito = objMiBusqueda.CodPais == "US";

                    sqlCampos += "Fechanum_date,Nandina,";

                    if (objMiBusqueda.TipoOpe == "I")
                    {
                        if(objMiBusqueda.Importador)
                            sqlCampos += "Importador,";

                        if (objMiBusqueda.Proveedor)
                            sqlCampos += "Proveedor,";                        

                        if(isVisibleFobOFasUnit)
                            sqlCampos += $"{dataFieldFobOFasUnit},";

                        if(isVisibleCifImptoUnit)
                            sqlCampos += "CIFImptoUnit,";

                        if (objMiBusqueda.PaisOrigen)
                            sqlCampos += "PaisOrigen,";

                        sqlCampos += $"{dataFieldCifOFobUnit},";
                    }
                    else
                    {
                        if (objMiBusqueda.Exportador)
                            sqlCampos += "Exportador,";

                        if (objMiBusqueda.ImportadorExp)
                            sqlCampos += "ImportadorExp,";

                        if (objMiBusqueda.ExistePaisDestino)
                            sqlCampos += "PaisDestino,";

                        sqlCampos += $"{dataFieldFobOFasUnit},";
                    }

                    if (objMiBusqueda.CampoPeso != "")
                        sqlCampos += $"{objMiBusqueda.CampoPeso},";

                    if (objMiBusqueda.ExisteDua)
                        sqlCampos += $"{objMiBusqueda.Dua},";

                    if (objMiBusqueda.FlagDescComercialB)
                        sqlCampos += "DesComercial,";

                    if (isVisibleDistrito)
                        sqlCampos += "Distrito,";

                    sqlCampos += "Cantidad,Unidad";

                }
                else
                {
                    var hasQty = objMiBusqueda.FlagVarVisibles.CodPais == "USI" ||
                                objMiBusqueda.FlagVarVisibles.CodPais == "USE";

                    var Cantidad = hasQty ? "Cantidad" : "Bultos";

                    var pto = objMiBusqueda.FlagVarVisibles.ExistePtoDescarga ? "PtoDescarga" :
                            objMiBusqueda.FlagVarVisibles.ExistePtoEmbarque ? "PtoEmbarque" : "PtoDestino";

                    sqlCampos += $@"Fechanum_date,Importador,Notificado,Proveedor,PaisEmbarque,Exportador,ImportadorExp,PaisDestino,{pto},PesoBruto,{Cantidad},
                                    DesComercial,MarcasContenedor";                   

                }

                sqlCampos += ", ROW_NUMBER() over (order by Fechanum_date desc) as Nro ";

            }

            DataTable dtRegistros = FuncionesBusiness.BuscarRegistros(codPais2, tipoOpe, tabla,
                Session["SqlFiltroR2"].ToString(),
                Session["hdfIdGrupoB"]?.ToString(), ref sqlFinal, sqlCampos:sqlCampos , pagina:1);

            int cantidadRegistros = dtRegistros.Rows.Count;

            if(sqlCampos != "")
            {               
                cantidadRegistros = Convert.ToInt32(drTotales["CantReg"]);
            }

            if (cantidadRegistros > 0)
            {                
                objMiBusqueda.FiltroUtilizado = filtro;               

                VerRegistroTable objVerRegistroTable = new VerRegistroTable
                {
                    TotalPaginas = (int)Math.Ceiling(Convert.ToDecimal(cantidadRegistros) / VerRegistrosPageSize),
                    TipoOpe = tipoOpe,
                    VerRegistroTableRows = GetListaVerRegistroTableRow(
                        dtRegistros.AsEnumerable().Take(VerRegistrosPageSize).CopyToDataTable(), objMiBusqueda,
                        GetSpecificCulture(), idioma, ((sqlFinal.Contains("IdMarca") && filtro != "MarcaEC") || sqlFinal.Contains("IdModelo"))),
                    VerRegistroTableHead = GetVerRegistroTableHead(objMiBusqueda,
                        ((sqlFinal.Contains("IdMarca") && filtro != "MarcaEC") || sqlFinal.Contains("IdModelo"))),
                    MiBusqueda =  objMiBusqueda
                };

                totalPages = objVerRegistroTable.TotalPaginas;
                string idUsuario = Session["IdUsuario"].ToString();


                if (cboDescargas2Visible)
                {
                    DataTable dtDescargas =
                        FuncionesBusiness.CargaDescargas(idUsuario, codPais, codPais2, tipoOpe, idioma);

                    optionsDescargas = dtDescargas.AsEnumerable().Select(m => new OptionSelect()
                    {
                        value = (m.Field<Int32>("IdDescargaCab")).ToString(),
                        text = m.Field<string>("Descarga")
                    });

                }

                if (Session["PLAN"].ToString() == "UNIVERSIDADES")
                {
                    lnkAgregarSelAFavoritosVisible = false;
                }
                else
                {
                    string codPaisT = codPais;
                    if (codPais == "PEP")
                        codPaisT = "PE";
                    else if (codPais2 == "4UE")
                        codPaisT = "UE" + codPais;

                    idDescargaDefault = FuncionesBusiness.BuscaDescargaDefault(idUsuario, codPaisT, tipoOpe);
                }

                tablaVerRegistro = RenderViewToString(this.ControllerContext, "GridViews/VerRegistroGridView" + (isManif ? "_Manif": ""),
                    objVerRegistroTable);
            }
            else
            {
                objMensaje = new
                {
                    titulo = Resources.Resources.Search_Button,
                    mensaje = Resources.Resources.NoResultsWereFound_Text,
                    flagContactenos = false
                };
            }

            return Json(new
            {
                maximoLimiteFiltros,
                objMensaje,
                lblFiltroSel,
                lblRecordsFound,
                lnkAgregarFiltroSelVisible,
                cboDescargas2Visible,
                optionsDescargas,
                lnkAgregarSelAFavoritosVisible,
                lnkAgregarSelAFavoritos,
                idDescargaDefault,
                tablaVerRegistro,
                totalPages,
                textoInfoComplementario,
                excelHabilitado
            });
        }

        private List<VerRegistroTableRow> GetListaVerRegistroTableRow(DataTable dt, MiBusqueda objMiBusqueda,
            CultureInfo specificCulture,
            string idioma, bool flagFormatoB = false , int pagina = 0)
        {
            List<VerRegistroTableRow> lista = new List<VerRegistroTableRow>();
            int numero = 0;

            if((objMiBusqueda.FiltroUtilizado == "InfoTabla" || objMiBusqueda.FiltroUtilizado == "InfoComplementario") && pagina > 0)
            {
                numero = (pagina - 1) * 10;
            }

            if (!objMiBusqueda.FlagVarVisibles.IsManifiesto)
            {
                var isVisibleCampoPeso = !flagFormatoB && objMiBusqueda.CampoPeso != "";
                var dataFieldFobOFasUnit = objMiBusqueda.CodPais != "US" ? "FOBUnit" : "FASUnit";

                var isVisibleDistrito = objMiBusqueda.CodPais == "US";

                

                if (objMiBusqueda.TipoOpe == "I")
                {
                    var isVisibleFobOFasUnit = (objMiBusqueda.Cif != "FOB"
                                                && objMiBusqueda.CodPais != "CN"
                                                && objMiBusqueda.CodPais != "MXD"
                                                && objMiBusqueda.CodPais != "IN");

                    var cif = objMiBusqueda.Cif.ToLower().Replace("tot", "");
                    //var dataFieldCifOFobUnit = objMiBusqueda.Cif + "Unit";
                    var dataFieldCifOFobUnit = cif + "Unit";

                    var isVisibleCifImptoUnit = (objMiBusqueda.CodPais == "PE");
                    if (idioma == "es")
                    {
                        isVisibleCifImptoUnit = (!flagFormatoB && isVisibleCifImptoUnit);
                    }

                    foreach (DataRow dr in dt.Rows)
                    {   
                        numero++;
                        lista.Add(new VerRegistroTableRow()
                        {
                            Numero = numero,
                            Fecha = dr.GetValue<DateTime>("Fechanum_date").ToString("dd/MM/yyyy", specificCulture),
                            Nandina = dr.GetValue<string>("Nandina"),
                            Importador = objMiBusqueda.Importador ? dr.GetValue<string>("Importador") : "",
                            Exportador = objMiBusqueda.Proveedor ? dr.GetValue<string>("Proveedor") : "",
                            CampoPeso = isVisibleCampoPeso ? (dr.GetValue<string>(objMiBusqueda.CampoPeso) != "" ? dr.GetValue<decimal>(objMiBusqueda.CampoPeso).ToString("n2", specificCulture) : "") : "",
                            Cantidad = dr.GetValue<string>("Cantidad") != "" ? dr.GetValue<decimal>("Cantidad").ToString("n2", specificCulture) : "",
                            Unidad = dr.GetValue<string>("Unidad"),
                            FobOFasUnit = isVisibleFobOFasUnit ? (dr.GetValue<string>(dataFieldFobOFasUnit) != "" ? dr.GetValue<decimal>(dataFieldFobOFasUnit).ToString("n2", specificCulture) : "") : "",
                            CifOFobUnit = (dr.GetValue<string>(dataFieldCifOFobUnit) != "" ? dr.GetValue<decimal>(dataFieldCifOFobUnit).ToString("n2", specificCulture) : ""),
                            CifImptoUnit = isVisibleCifImptoUnit ? (dr.GetValue<string>("CIFImptoUnit") != "" ? dr.GetValue<decimal>("CIFImptoUnit").ToString("n2", specificCulture) : "") : "",
                            Dua = objMiBusqueda.ExisteDua ? dr.GetValue<string>(objMiBusqueda.Dua) : "",
                            PaisOrigenODestino = objMiBusqueda.PaisOrigen ? dr.GetValue<string>("PaisOrigen") : "",
                            DesComercial = objMiBusqueda.FlagDescComercialB ? dr.GetValue<string>("DesComercial") : "",
                            Distrito = isVisibleDistrito ? dr.GetValue<string>("Distrito") : ""
                        });
                    }
                }
                else
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        numero++;
                        lista.Add(new VerRegistroTableRow()
                        {
                            Numero = numero,
                            Fecha = dr.GetValue<DateTime>("Fechanum_date").ToString("dd/MM/yyyy", specificCulture),
                            Nandina = dr.GetValue<string>("Nandina"),
                            Exportador = objMiBusqueda.Exportador ? dr.GetValue<string>("Exportador") : "",
                            Importador = objMiBusqueda.ImportadorExp ? dr.GetValue<string>("ImportadorExp") : "",
                            CampoPeso = isVisibleCampoPeso ? (dr.GetValue<string>(objMiBusqueda.CampoPeso) != "" ? dr.GetValue<decimal>(objMiBusqueda.CampoPeso).ToString("n2", specificCulture) : "") : "",
                            Cantidad = dr["Cantidad"].ToString() != "" ? Convert.ToDecimal(dr["Cantidad"]).ToString("n2", specificCulture) : "",
                            Unidad = dr["Unidad"].ToString(),
                            FobOFasUnit = dr[dataFieldFobOFasUnit].ToString() != "" ? Convert.ToDecimal(dr[dataFieldFobOFasUnit]).ToString("n2", specificCulture) : "",
                            Dua = objMiBusqueda.ExisteDua ? dr[objMiBusqueda.Dua].ToString() : "",
                            PaisOrigenODestino = objMiBusqueda.ExistePaisDestino ? dr["PaisDestino"].ToString() : "",
                            DesComercial = objMiBusqueda.FlagDescComercialB ? dr["DesComercial"].ToString() : "",
                            Distrito = isVisibleDistrito ? dr["Distrito"].ToString() : ""
                        });
                    }
                }
            }
            else
            {
                var hasQty = objMiBusqueda.FlagVarVisibles.CodPais == "USI" ||
                                objMiBusqueda.FlagVarVisibles.CodPais == "USE";
                foreach (DataRow dr in dt.Rows)
                {
                    numero++;
                    lista.Add(new VerRegistroTableRow()
                    {
                        Numero = numero,
                        Fecha = Convert.ToDateTime(dr["Fechanum_date"]).ToString("dd/MM/yyyy", specificCulture),
                        Importador = dr.GetValue<string>("Importador"),
                        Notificado = dr.GetValue<string>("Notificado"),
                        ExportadorProveedor = dr.GetValue<string>("Proveedor"),
                        PaisEmbarque = dr.GetValue<string>("PaisEmbarque"),
                        Exportador = dr.GetValue<string>("Exportador"),
                        ImportadorExp = dr.GetValue<string>("ImportadorExp"),
                        PaisOrigenODestino = dr.GetValue<string>("PaisDestino"),
                        Pto =  dr.GetValue<string>(objMiBusqueda.FlagVarVisibles.ExistePtoDescarga ? "PtoDescarga" : 
                            objMiBusqueda.FlagVarVisibles.ExistePtoEmbarque ? "PtoEmbarque" : "PtoDestino"),
                        PesoBruto = dr.GetValue<decimal>("PesoBruto").ToString("n0", specificCulture),
                        QtyOrButos = dr.GetValue<decimal>(hasQty ? "Cantidad" : "Bultos").ToString("n" + (hasQty ? "2" : "0"), specificCulture),
                        DesComercial = dr.GetValue<string>("DesComercial"),
                        DesAdicional = dr.GetValue<string>("MarcasContenedor"),
                    });
                }
            }
            return lista;
        }

        private VerRegistroTableHead GetVerRegistroTableHead(MiBusqueda objMiBusqueda, bool flagFormatoB = false)
        {
            var isManif = objMiBusqueda.FlagVarVisibles. IsManifiesto;

            VerRegistroTableHead objVerRegistroTableHead = new VerRegistroTableHead();

            objVerRegistroTableHead.Numero = "No.";

            objVerRegistroTableHead.Nandina = Resources.Resources.Nandina_FormField_Label;
            objVerRegistroTableHead.Fecha = Resources.Resources.Date_Text;

            objVerRegistroTableHead.Importador = !isManif? Resources.Resources.Search_Form_Item05: Resources.Resources.Demo_Importers_Tab_Manif;
            objVerRegistroTableHead.Notificado = Resources.Resources.Demo_Notif_Tab_Fil;
            
            objVerRegistroTableHead.IsVisibleCampoPeso = !flagFormatoB && objMiBusqueda.CampoPeso != "";
            objVerRegistroTableHead.CampoPeso = objMiBusqueda.CampoPeso == "PesoNeto"
                ? Resources.Resources.NetKg_Text
                : ( !isManif ?Resources.Resources.GrossKg_Text: Resources.Resources.PesoBruto_Text );

            objVerRegistroTableHead.Cantidad = Resources.Resources.Quantity_Text;
            objVerRegistroTableHead.Unidad = Resources.Resources.Unit_Text;

            objVerRegistroTableHead.FobOFasUnit = objMiBusqueda.CodPais != "US"
                ? Resources.Resources.USFOBUnit_Text
                : Resources.Resources.USFASUnit_Text;

            objVerRegistroTableHead.IsVisibleDua = objMiBusqueda.ExisteDua;
            objVerRegistroTableHead.Dua = "DUA";

            objVerRegistroTableHead.IsVisibleDesComercial = objMiBusqueda.FlagDescComercialB;
            objVerRegistroTableHead.DesComercial = Resources.AdminResources.ComercialDesc_FilterText;

            objVerRegistroTableHead.IsVisibleDistrito = (objMiBusqueda.CodPais == "US");
            objVerRegistroTableHead.Distrito = Resources.Resources.UnlandingDistrict_Text;

            objVerRegistroTableHead.PaisEmbarque = Resources.Resources.Pais_Embarque;
            objVerRegistroTableHead.PuertoDescarga = Resources.Resources.Demo_PtoDescarga_Tab;
            objVerRegistroTableHead.PuertoEmbarque = Resources.Resources.Demo_PtoEmbarque;
            objVerRegistroTableHead.Bultos = Resources.Resources.Demo_Bultos;
            objVerRegistroTableHead.DesAdicional = Resources.Resources.Demo_DescAdicional;

            if (objMiBusqueda.TipoOpe == "I")
            {
                objVerRegistroTableHead.IsVisibleImportador = objMiBusqueda.Importador;
                objVerRegistroTableHead.Exportador = (objMiBusqueda.CodPais != "CL" )
                    ? (!isManif?Resources.Resources.Search_Form_Item06: Resources.Resources.Demo_Exporters_Tab_Manif)
                    : Resources.Resources.Search_Form_BrandField;
                objVerRegistroTableHead.IsVisibleExportador = objMiBusqueda.Proveedor;


                objVerRegistroTableHead.IsVisibleFobOFasUnit =
                    (objMiBusqueda.Cif != "FOB" && objMiBusqueda.CodPais != "CN" && objMiBusqueda.CodPais != "MXD" &&
                     objMiBusqueda.CodPais != "IN");

                var cif = objMiBusqueda.Cif.ToLower().Replace("tot", "").ToUpper();
                objVerRegistroTableHead.CifOFobUnit = "US$ " + cif + " Unit.";

                objVerRegistroTableHead.CifImptoUnit = Resources.Resources.USCIFUnitTaxes_Text;

                objVerRegistroTableHead.IsVisibleCifImptoUnit = !flagFormatoB && objMiBusqueda.CodPais == "PE";

                objVerRegistroTableHead.PaisOrigenODestino = Resources.AdminResources.OriginCountry_FormField_Label;
                objVerRegistroTableHead.IsVisiblePaisOrigenODestino = objMiBusqueda.PaisOrigen;
            }
            else
            {
                objVerRegistroTableHead.Exportador = Resources.Resources.Search_Form_Item06;
                objVerRegistroTableHead.IsVisibleExportador = objMiBusqueda.Exportador;
                objVerRegistroTableHead.IsVisibleImportador = objMiBusqueda.ImportadorExp;

                objVerRegistroTableHead.IsVisibleFobOFasUnit = true;

                objVerRegistroTableHead.IsVisibleCifOFobUnit = false;
                objVerRegistroTableHead.IsVisibleCifImptoUnit = false;

                objVerRegistroTableHead.PaisOrigenODestino =
                    Resources.AdminResources.DestinationCountry_FormField_Label;
                objVerRegistroTableHead.IsVisiblePaisOrigenODestino = objMiBusqueda.ExistePaisDestino;
            }

            /*if(objMiBusqueda.FiltroUtilizado == "InfoComplementario")
            {
                if(objMiBusqueda.TipoOpe == "I")
                {
                    objVerRegistroTableHead.Importador = Resources.Resources.Search_Form_Item06;                    
                    objVerRegistroTableHead.PaisOrigenODestino = Resources.AdminResources.DestinationCountry_FormField_Label;
                }
                else
                {
                    objVerRegistroTableHead.Exportador = !isManif ? Resources.Resources.Search_Form_Item05 : Resources.Resources.Demo_Importers_Tab_Manif;
                    objVerRegistroTableHead.PaisOrigenODestino = Resources.AdminResources.OriginCountry_FormField_Label;
                }
                
            }*/

            return objVerRegistroTableHead;
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult VerRegistrosPageIndexChanging(int pagina, string codPais, string codPais2,
            string tipoOpe)
        {
            GuardarLogInicioErrorSession("VerRegistrosPageIndexChanging()");
            if (Session["filtro"].ToString() == "InfoTabla" || Session["filtro"].ToString() == "InfoComplementario")
            {
                tipoOpe = tipoOpe == "I" ? "E" : "I";
                codPais = Session["CodPaisComplementario"].ToString().ToUpper();
                codPais2 = new ListaPaises().BuscarCodPais2(codPais);
            }
            if(Session["filtro"].ToString() != "InfoComplementario" && Session["filtro"].ToString() != "InfoTabla")
            {
                ValidaCodPais2(codPais2, ref codPais);
                ValidaCodPaisManif(ref codPais, tipoOpe);
            }
            
            string idioma = GetCurrentIdioma();

            var isManif = IsManifiesto(codPais);

            var tabla = "";
            var dua = "";
            GetTablaAndDua(tipoOpe, codPais, ref tabla, ref dua);

            string sqlFinal = "";

            string sqlCampos = "";

            MiBusqueda objMiBusqueda = new MiBusqueda
            {
                FlagVarVisibles = new TabMisBusquedas(tipoOpe, codPais),    //new FlagVarVisibles(codPais, tipoOpe, isManif),
                Importador = Funciones.ExisteVariable(codPais, tipoOpe, "IdImportador"),
                Proveedor = Funciones.ExisteVariable(codPais, tipoOpe, "IdProveedor"),
                PaisOrigen = Funciones.ExisteVariable(codPais, tipoOpe, "IdPaisOrigen"),
                Exportador = Funciones.ExisteVariable(codPais, tipoOpe, "IdExportador"),
                ImportadorExp = Funciones.ExisteVariable(codPais, tipoOpe, "IdImportadorExp"),
                ExistePaisDestino = Funciones.ExisteVariable(codPais, tipoOpe, "IdPaisDestino"),
                CampoPeso = Funciones.CampoPeso(codPais, tipoOpe),
                FlagDescComercialB = Funciones.FlagDesComercial(codPais, tipoOpe),
                Cif = GetCIFTot(codPais, tipoOpe), //Funciones.Incoterm(codPais, tipoOpe),
                TipoOpe = tipoOpe,
                CodPais = codPais,
                CodPais2 = codPais2
            };

            bool existeAduana = GetExisteAduana(codPais);
            bool existeDua = GetExisteDua(existeAduana, codPais);

            objMiBusqueda.ExisteAduana = existeAduana;
            objMiBusqueda.ExisteDua = existeDua;
            objMiBusqueda.Dua = dua;
            objMiBusqueda.FiltroUtilizado = Session["filtro"].ToString();
            if (Session["filtro"].ToString() == "InfoTabla" || Session["filtro"].ToString() == "InfoComplementario")
            {
                //objMiBusqueda.ExisteDua = objMiBusqueda.ExisteDua && objMiBusqueda.CodPais.ToUpper() != "EC";
                if (!objMiBusqueda.FlagVarVisibles.IsManifiesto)
                {
                    var isVisibleFobOFasUnit = (objMiBusqueda.Cif != "FOB"
                                                && objMiBusqueda.CodPais != "CN"
                                                && objMiBusqueda.CodPais != "MXD"
                                                && objMiBusqueda.CodPais != "IN");
                    var dataFieldFobOFasUnit = objMiBusqueda.CodPais != "US" ? "FOBUnit" : "FASUnit";

                    var cif = objMiBusqueda.Cif.ToLower().Replace("tot", "");
                    var dataFieldCifOFobUnit = cif + "Unit";
                    var flagFormatoB = ((Session["SqlFiltroR2"].ToString().Contains("IdMarca") && Session["filtro"].ToString() != "MarcaEC") || Session["SqlFiltroR2"].ToString().Contains("IdModelo"));

                    var isVisibleCifImptoUnit = (objMiBusqueda.CodPais == "PE");

                    if (idioma == "es")
                    {
                        isVisibleCifImptoUnit = (!flagFormatoB && isVisibleCifImptoUnit);
                    }

                    var isVisibleDistrito = objMiBusqueda.CodPais == "US";

                    sqlCampos += "Fechanum_date,Nandina,";

                    if (objMiBusqueda.TipoOpe == "I")
                    {
                        if (objMiBusqueda.Importador)
                            sqlCampos += "Importador,";

                        if (objMiBusqueda.Proveedor)
                            sqlCampos += "Proveedor,";

                        if (isVisibleFobOFasUnit)
                            sqlCampos += $"{dataFieldFobOFasUnit},";

                        if (isVisibleCifImptoUnit)
                            sqlCampos += "CIFImptoUnit,";

                        if (objMiBusqueda.PaisOrigen)
                            sqlCampos += "PaisOrigen,";

                        sqlCampos += $"{dataFieldCifOFobUnit},";
                    }
                    else
                    {
                        if (objMiBusqueda.Exportador)
                            sqlCampos += "Exportador,";

                        if (objMiBusqueda.ImportadorExp)
                            sqlCampos += "ImportadorExp,";

                        if (objMiBusqueda.ExistePaisDestino)
                            sqlCampos += "PaisDestino,";

                        sqlCampos += $"{dataFieldFobOFasUnit},";
                    }

                    if (objMiBusqueda.CampoPeso != "")
                        sqlCampos += $"{objMiBusqueda.CampoPeso},";

                    if (objMiBusqueda.ExisteDua)
                        sqlCampos += $"{objMiBusqueda.Dua},";

                    if (objMiBusqueda.FlagDescComercialB)
                        sqlCampos += "DesComercial,";

                    if (isVisibleDistrito)
                        sqlCampos += "Distrito,";

                    sqlCampos += "Cantidad,Unidad";

                }
                else
                {
                    var hasQty = objMiBusqueda.FlagVarVisibles.CodPais == "USI" ||
                                objMiBusqueda.FlagVarVisibles.CodPais == "USE";

                    var Cantidad = hasQty ? "Cantidad" : "Bultos";

                    var pto = objMiBusqueda.FlagVarVisibles.ExistePtoDescarga ? "PtoDescarga" :
                            objMiBusqueda.FlagVarVisibles.ExistePtoEmbarque ? "PtoEmbarque" : "PtoDestino";

                    sqlCampos += $@"Fechanum_date,Importador,Notificado,Proveedor,PaisEmbarque,Exportador,ImportadorExp,PaisDestino,{pto},PesoBruto,{Cantidad},
                                    DesComercial,MarcasContenedor";

                }
                sqlCampos += ", ROW_NUMBER() over (order by Fechanum_date desc) as Nro ";
            }

            DataTable dtRegistros = FuncionesBusiness.BuscarRegistros(codPais2, tipoOpe, tabla,
                Session["SqlFiltroR2"].ToString(),
                Session["hdfIdGrupoB"]?.ToString(), ref sqlFinal, sqlCampos: sqlCampos , pagina : pagina);

            

            /*if (Session["Plan"].ToString() == "ESENCIAL" && !isManif)
            {
                objMiBusqueda.ImportadorExp = false;
            }*/

            

            List<VerRegistroTableRow> listaVerRegistroTableRows = GetListaVerRegistroTableRow(dtRegistros,
                objMiBusqueda, GetSpecificCulture(), idioma,
                ((sqlFinal.Contains("IdMarca") && !objMiBusqueda.FlagVarVisibles.ExisteMarcaEC) || sqlFinal.Contains("IdModelo")), pagina:pagina);

            VerRegistroTable objVerRegistroTable = new VerRegistroTable
            {
                TipoOpe = tipoOpe,
                VerRegistroTableHead = GetVerRegistroTableHead(objMiBusqueda,
                    ((sqlFinal.Contains("IdMarca") && !objMiBusqueda.FlagVarVisibles.ExisteMarcaEC) || sqlFinal.Contains("IdModelo"))),
                PagedListTableRows = (sqlCampos == "" ? listaVerRegistroTableRows.ToPagedList(pagina, VerRegistrosPageSize) : listaVerRegistroTableRows.ToPagedList(1, VerRegistrosPageSize)),
                MiBusqueda = objMiBusqueda
            };

            return Json(new
            {
                filasVerRegistro =
                    RenderViewToString(this.ControllerContext, "GridViews/VerRegistroRows" + (isManif ? "_Manif" : ""), objVerRegistroTable)
            });
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult BuscarPorDuaVerRegistros(string txtDuaB, string tipoOpe, string codPais,
            string codPais2, string anioMesIni, string anioMesFin,
            int indexCboPaisB, string codPaisB, string codPaisRep)
        {
            GuardarLogInicioErrorSession("BuscarPorDuaVerRegistros()");
            string auxCodPais = codPais;
            string filtro = Session["filtro"].ToString();
            string idioma = Session["Idioma"].ToString();
            if (filtro != "InfoComplementario" && filtro != "InfoTabla")
            {
                ValidaCodPais2(codPais2, ref codPais);
            }
            if(filtro == "InfoComplementario" || filtro == "InfoTabla")
            {
                if(Session["tipoOpe"].ToString() == tipoOpe)
                {
                    tipoOpe = tipoOpe == "I" ? "E" : "I";
                }
                /*if(codPais2 == "4UE")
                {
                    string nombrePais = FuncionesBusiness.BuscarPaisUE(codPaisRep, idioma);
                    if (nombrePais == "USA")
                    {
                        nombrePais = "ESTADOS UNIDOS";
                    }

                    int idPais = FuncionesBusiness.BuscarIdPaisNombre(nombrePais, codPais);
                    if (nombrePais == "Alemania" && codPais == "EC")
                    {
                        idPais = 53;
                    }
                    if (nombrePais == "ESTADOS UNIDOS" && codPais == "EC")
                    {
                        idPais = 158;
                    }
                    indexCboPaisB = 1;
                    codPaisB = idPais.ToString();
                }*/


                codPais2 = new ListaPaises().BuscarCodPais2(codPais);
            }
            
            ValidaCodPaisManif(ref codPais, tipoOpe);
            var isManif = IsManifiesto(codPais);

            Session["hdfDUAB2"] = txtDuaB.Trim();
            Session["hdfDesComercialBB2"] = null;

            var tabla = "";
            var dua = "";
            GetTablaAndDua(tipoOpe, codPais, ref tabla, ref dua);

            string sqlAux = GeneraSqlFiltroR2(tipoOpe, codPais, codPais2, anioMesIni, anioMesFin,
                indexCboPaisB, codPaisB, dua, auxCodPais, filtro : filtro);

            string sqlFinal = "";

            if (Session["filtro"].ToString() == "InfoComplementario" || Session["filtro"].ToString() == "InfoTabla")
            {
                string codPaisAux = Session["CodPais"].ToString();
                string nombrePais = FuncionesBusiness.BuscarPaisNombre(codPaisAux, idioma);
                if (codPaisAux == "UE")
                {
                    codPaisAux = codPaisRep;
                    nombrePais = FuncionesBusiness.BuscarPaisUE(codPaisAux, idioma);

                }
                codPais2 = new ListaPaises().BuscarCodPais2(codPais);



                if (nombrePais == "USA")
                {
                    nombrePais = "ESTADOS UNIDOS";
                }

                int idPais = FuncionesBusiness.BuscarIdPaisNombre(nombrePais, codPais);
                if (nombrePais == "Alemania" && codPais == "EC")
                {
                    idPais = 53;
                }
                if (nombrePais == "ESTADOS UNIDOS" && codPais == "EC")
                {
                    idPais = 158;
                }
                // Ruben 202310
                else if (nombrePais == "ESTADOS UNIDOS" && codPais == "MXD")
                {
                    idPais = 321;
                }

                if (idPais != 0)
                {
                    Session["SqlFiltroR2"] = sqlAux + (tipoOpe == "I" ? " and idPaisOrigen = " : " and idPaisDestino = ") + idPais + " ";
                }
                else
                {
                    Session["SqlFiltroR2"] = sqlAux;
                }
            }
            else
            {
                Session["SqlFiltroR2"] = sqlAux;
            }


            DataTable dtRegistros = FuncionesBusiness.BuscarRegistros(codPais2, tipoOpe, tabla,
                Session["SqlFiltroR2"].ToString(),
                Session["hdfIdGrupoB"]?.ToString(), ref sqlFinal, filtro: filtro);

            object objMensaje = null;
            var tablaVerRegistro = "";
            var totalPages = 0;
            var resultadoDuaVerRegistro = "";
            var registrosEncontrados = dtRegistros.Rows.Count;

            if (registrosEncontrados > 0)
            {
                MiBusqueda objMiBusqueda = new MiBusqueda
                {
                    FlagVarVisibles = new TabMisBusquedas(tipoOpe, codPais),   //new FlagVarVisibles(codPais, tipoOpe, isManif),
                    Importador = Funciones.ExisteVariable(codPais, tipoOpe, "IdImportador"),
                    Proveedor = Funciones.ExisteVariable(codPais, tipoOpe, "IdProveedor"),
                    PaisOrigen = Funciones.ExisteVariable(codPais, tipoOpe, "IdPaisOrigen"),
                    Exportador = Funciones.ExisteVariable(codPais, tipoOpe, "IdExportador"),
                    ImportadorExp = Funciones.ExisteVariable(codPais, tipoOpe, "IdImportadorExp"),
                    ExistePaisDestino = Funciones.ExisteVariable(codPais, tipoOpe, "IdPaisDestino"),
                    CampoPeso = Funciones.CampoPeso(codPais, tipoOpe),
                    FlagDescComercialB = Funciones.FlagDesComercial(codPais, tipoOpe),
                    Cif = GetCIFTot(codPais, tipoOpe), //Funciones.Incoterm(codPais, tipoOpe),
                    TipoOpe = tipoOpe,
                    CodPais = codPais,
                    CodPais2 = codPais2
                };

                bool existeAduana = GetExisteAduana(codPais);
                bool existeDua = GetExisteDua(existeAduana, codPais);

                /*if (Session["Plan"].ToString() == "ESENCIAL" && !isManif)
                {
                    objMiBusqueda.ImportadorExp = false;
                }*/

                objMiBusqueda.ExisteAduana = existeAduana;
                objMiBusqueda.ExisteDua = existeDua;
                objMiBusqueda.Dua = dua;

                
                List<VerRegistroTableRow> listaVerRegistroTableRows = GetListaVerRegistroTableRow(dtRegistros,
                    objMiBusqueda, GetSpecificCulture(), idioma,
                    (sqlFinal.Contains("IdMarca") || sqlFinal.Contains("IdModelo")));

                VerRegistroTable objVerRegistroTable = new VerRegistroTable
                {
                    TotalPaginas = (int)Math.Ceiling(Convert.ToDecimal(registrosEncontrados) / VerRegistrosPageSize),
                    TipoOpe = tipoOpe,
                    VerRegistroTableHead = GetVerRegistroTableHead(objMiBusqueda,
                        (sqlFinal.Contains("IdMarca") || sqlFinal.Contains("IdModelo"))),
                    PagedListTableRows = listaVerRegistroTableRows.ToPagedList(1, VerRegistrosPageSize),
                    MiBusqueda = objMiBusqueda
                };

                tablaVerRegistro = RenderViewToString(this.ControllerContext, "GridViews/VerRegistroRows"+ (isManif ? "_Manif": ""), objVerRegistroTable);
                totalPages = objVerRegistroTable.TotalPaginas;
                if (Session["hdfDUAB2"].ToString() != "")
                {
                    resultadoDuaVerRegistro = registrosEncontrados.ToString() + " " + Resources.AdminResources.Records_Text;
                }

            }
            else
            {
                objMensaje = new
                {
                    titulo = Resources.Resources.Search_Button,
                    mensaje = Resources.Resources.NoResultsWereFound_Text,
                    flagContactenos = false
                };
            }

            return Json(new
            {
                objMensaje,
                tablaVerRegistro,
                totalPages,
                resultadoDuaVerRegistro
            });
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult BuscarPorDesComercialVerRegistro(string txtDesComercialBB, string tipoOpe, string codPais,
            string codPais2, string anioMesIni, string anioMesFin,
            int indexCboPaisB, string codPaisB,string codPaisRep)
        {
            GuardarLogInicioErrorSession("BuscarPorDesComercialVerRegistro()");
            if (Session["filtro"].ToString() == "InfoTabla" || Session["filtro"].ToString() == "InfoComplementario")
            {
                codPais = Session["CodPaisComplementario"].ToString().ToUpper();
                tipoOpe = tipoOpe == "I" ? "E" : "I";
                codPais2 = new ListaPaises().BuscarCodPais2(codPais);
            }
            string auxCodPais = codPais;
            if(Session["filtro"].ToString() != "InfoComplementario" && Session["filtro"].ToString() != "InfoTabla")
            {
                ValidaCodPais2(codPais2, ref codPais);
                ValidaCodPaisManif(ref codPais, tipoOpe);
            }
            

            var isManif = IsManifiesto(codPais);

            Session["hdfDesComercialBB2"] = txtDesComercialBB.Trim();
            Session["hdfDUAB2"] = null;

            var tabla = "";
            var dua = "";
            string filtro = Session["filtro"].ToString();
            GetTablaAndDua(tipoOpe, codPais, ref tabla, ref dua);
            string idioma = Session["Idioma"].ToString();
            string sqlAux = GeneraSqlFiltroR2(tipoOpe, codPais, codPais2, anioMesIni, anioMesFin,
                indexCboPaisB, codPaisB, dua, auxCodPais, filtro : filtro);

            if (Session["filtro"].ToString() == "InfoComplementario" || Session["filtro"].ToString() == "InfoTabla")
            {
                string codPaisAux = Session["CodPais"].ToString();
                string nombrePais = FuncionesBusiness.BuscarPaisNombre(codPaisAux, idioma);
                if (codPaisAux == "UE")
                {
                    codPaisAux = codPaisRep;
                    nombrePais = FuncionesBusiness.BuscarPaisUE(codPaisAux, idioma);
                    
                }
                codPais2 = new ListaPaises().BuscarCodPais2(codPais);



                if (nombrePais == "USA")
                {
                    nombrePais = "ESTADOS UNIDOS";
                }

                int idPais = FuncionesBusiness.BuscarIdPaisNombre(nombrePais, codPais);
                if (nombrePais == "Alemania" && codPais == "EC")
                {
                    idPais = 53;
                }
                if (nombrePais == "ESTADOS UNIDOS" && codPais == "EC")
                {
                    idPais = 158;
                }
                // Ruben 202310
                else if (nombrePais == "ESTADOS UNIDOS" && codPais == "MXD")
                {
                    idPais = 321;
                }

                if (idPais != 0)
                {
                    Session["SqlFiltroR2"] = sqlAux + (tipoOpe == "I" ? " and idPaisOrigen = " : " and idPaisDestino = ") + idPais + " ";
                }
                else
                {
                    Session["SqlFiltroR2"] = sqlAux;
                }
            }
            else
            {
                Session["SqlFiltroR2"] = sqlAux;
            }

            string sqlFinal = "";
            //string idioma = Session["Idioma"].ToString();
            MiBusqueda objMiBusqueda = new MiBusqueda
            {
                FlagVarVisibles = new TabMisBusquedas(tipoOpe, codPais), //new FlagVarVisibles(codPais, tipoOpe, isManif),
                Importador = Funciones.ExisteVariable(codPais, tipoOpe, "IdImportador"),
                Proveedor = Funciones.ExisteVariable(codPais, tipoOpe, "IdProveedor"),
                PaisOrigen = Funciones.ExisteVariable(codPais, tipoOpe, "IdPaisOrigen"),
                Exportador = Funciones.ExisteVariable(codPais, tipoOpe, "IdExportador"),
                ImportadorExp = Funciones.ExisteVariable(codPais, tipoOpe, "IdImportadorExp"),
                ExistePaisDestino = Funciones.ExisteVariable(codPais, tipoOpe, "IdPaisDestino"),
                CampoPeso = Funciones.CampoPeso(codPais, tipoOpe),
                FlagDescComercialB = Funciones.FlagDesComercial(codPais, tipoOpe),
                Cif = GetCIFTot(codPais, tipoOpe), //Funciones.Incoterm(codPais, tipoOpe),
                TipoOpe = tipoOpe,
                CodPais = codPais,
                CodPais2 = codPais2
            };

            bool existeAduana = GetExisteAduana(codPais);
            bool existeDua = GetExisteDua(existeAduana, codPais);

            /* if (Session["Plan"].ToString() == "ESENCIAL" && !isManif )
             {
                 objMiBusqueda.ImportadorExp = false;
             }*/

            objMiBusqueda.ExisteAduana = existeAduana;
            objMiBusqueda.ExisteDua = existeDua;
            objMiBusqueda.Dua = dua;

            string sqlCampos = "";

            if (filtro == "InfoTabla" || filtro == "InfoComplementario")
            {
                //objMiBusqueda.ExisteDua = objMiBusqueda.ExisteDua && objMiBusqueda.CodPais.ToUpper() != "EC";
                if (!objMiBusqueda.FlagVarVisibles.IsManifiesto)
                {
                    var isVisibleFobOFasUnit = (objMiBusqueda.Cif != "FOB"
                                                && objMiBusqueda.CodPais != "CN"
                                                && objMiBusqueda.CodPais != "MXD"
                                                && objMiBusqueda.CodPais != "IN");
                    var dataFieldFobOFasUnit = objMiBusqueda.CodPais != "US" ? "FOBUnit" : "FASUnit";

                    var cif = objMiBusqueda.Cif.ToLower().Replace("tot", "");
                    var dataFieldCifOFobUnit = cif + "Unit";
                    var flagFormatoB = ((Session["SqlFiltroR2"].ToString().Contains("IdMarca") && filtro != "MarcaEC") || Session["SqlFiltroR2"].ToString().Contains("IdModelo"));

                    var isVisibleCifImptoUnit = (objMiBusqueda.CodPais == "PE");

                    if (idioma == "es")
                    {
                        isVisibleCifImptoUnit = (!flagFormatoB && isVisibleCifImptoUnit);
                    }

                    var isVisibleDistrito = objMiBusqueda.CodPais == "US";

                    sqlCampos += "Fechanum_date,Nandina,";

                    if (objMiBusqueda.TipoOpe == "I")
                    {
                        if (objMiBusqueda.Importador)
                            sqlCampos += "Importador,";

                        if (objMiBusqueda.Proveedor)
                            sqlCampos += "Proveedor,";

                        if (isVisibleFobOFasUnit)
                            sqlCampos += $"{dataFieldFobOFasUnit},";

                        if (isVisibleCifImptoUnit)
                            sqlCampos += "CIFImptoUnit,";

                        if (objMiBusqueda.PaisOrigen)
                            sqlCampos += "PaisOrigen,";

                        sqlCampos += $"{dataFieldCifOFobUnit},";
                    }
                    else
                    {
                        if (objMiBusqueda.Exportador)
                            sqlCampos += "Exportador,";

                        if (objMiBusqueda.ImportadorExp)
                            sqlCampos += "ImportadorExp,";

                        if (objMiBusqueda.ExistePaisDestino)
                            sqlCampos += "PaisDestino,";

                        sqlCampos += $"{dataFieldFobOFasUnit},";
                    }

                    if (objMiBusqueda.CampoPeso != "")
                        sqlCampos += $"{objMiBusqueda.CampoPeso},";

                    if (objMiBusqueda.ExisteDua)
                        sqlCampos += $"{objMiBusqueda.Dua},";

                    if (objMiBusqueda.FlagDescComercialB)
                        sqlCampos += "DesComercial,";

                    if (isVisibleDistrito)
                        sqlCampos += "Distrito,";

                    sqlCampos += "Cantidad,Unidad";

                }
                else
                {
                    var hasQty = objMiBusqueda.FlagVarVisibles.CodPais == "USI" ||
                                objMiBusqueda.FlagVarVisibles.CodPais == "USE";

                    var Cantidad = hasQty ? "Cantidad" : "Bultos";

                    var pto = objMiBusqueda.FlagVarVisibles.ExistePtoDescarga ? "PtoDescarga" :
                            objMiBusqueda.FlagVarVisibles.ExistePtoEmbarque ? "PtoEmbarque" : "PtoDestino";

                    sqlCampos += $@"Fechanum_date,Importador,Notificado,Proveedor,PaisEmbarque,Exportador,ImportadorExp,PaisDestino,{pto},PesoBruto,{Cantidad},
                                    DesComercial,MarcasContenedor";

                }

                sqlCampos += ", ROW_NUMBER() over (order by Fechanum_date desc) as Nro ";

            }

            DataTable dtRegistros = FuncionesBusiness.BuscarRegistros(codPais2, tipoOpe, tabla,
                Session["SqlFiltroR2"].ToString(),
                Session["hdfIdGrupoB"]?.ToString(), ref sqlFinal, sqlCampos: sqlCampos, pagina: 1);

            object objMensaje = null;
            var tablaVerRegistro = "";
            var totalPages = 0;
            var resultadoDesComercialVerRegistro = "";
            var registrosEncontrados = dtRegistros.Rows.Count;

            if(sqlCampos != "")
            {

                registrosEncontrados = FuncionesBusiness.CantidadRegistrosPorConsulta(Session["SqlFiltroR2"].ToString(), tabla);
            }

            if (registrosEncontrados > 0)
            {
                

                

                List<VerRegistroTableRow> listaVerRegistroTableRows = GetListaVerRegistroTableRow(dtRegistros,
                    objMiBusqueda, GetSpecificCulture(), idioma,
                    ((sqlFinal.Contains("IdMarca") && !objMiBusqueda.FlagVarVisibles.ExisteMarcaEC) || sqlFinal.Contains("IdModelo")));

                VerRegistroTable objVerRegistroTable = new VerRegistroTable
                {
                    TotalPaginas = (int)Math.Ceiling(Convert.ToDecimal(registrosEncontrados) / VerRegistrosPageSize),
                    TipoOpe = tipoOpe,
                    VerRegistroTableHead = GetVerRegistroTableHead(objMiBusqueda,
                        ((sqlFinal.Contains("IdMarca") && !objMiBusqueda.FlagVarVisibles.ExisteMarcaEC) || sqlFinal.Contains("IdModelo"))),
                    PagedListTableRows = listaVerRegistroTableRows.ToPagedList(1, VerRegistrosPageSize),
                    MiBusqueda = objMiBusqueda
                };

                tablaVerRegistro = RenderViewToString(this.ControllerContext, "GridViews/VerRegistroRows" + (isManif ? "_Manif" : ""), objVerRegistroTable);
                totalPages = objVerRegistroTable.TotalPaginas;
                if (Session["hdfDesComercialBB2"].ToString() != "")
                {
                    resultadoDesComercialVerRegistro = registrosEncontrados.ToString() + " " + Resources.AdminResources.Records_Text;
                }

            }
            else
            {
                objMensaje = new
                {
                    titulo = Resources.Resources.Search_Button,
                    mensaje = Resources.Resources.NoResultsWereFound_Text,
                    flagContactenos = false
                };
            }

            return Json(new
            {
                objMensaje,
                tablaVerRegistro,
                totalPages,
                resultadoDesComercialVerRegistro
            });
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult AgregarFiltroSeleccionado(string codPais, string codPais2, string tipoOpe,
            string anioMesIni, string anioMesFin, int indexCboPaisB,
            string codPaisB)
        {
            string auxCodPais = codPais;
            ValidaCodPais2(codPais2, ref codPais);
            ValidaCodPaisManif(ref codPais, tipoOpe);

            var tipo = "";
            var nombre = "";
            var nuevoFiltro = true;

            var nuevosFiltros = new List<object>();
            List<object> listGridData = null;

            Session["CodPais"] = codPais;
            string idioma = Session["Idioma"].ToString();

            string id = Session["hdfIDSel"].ToString();

            nombre = AgregaFiltro(idioma, tipoOpe, ref tipo);

            string cifTot = "";
            string pesoNeto = "";
            string valueCifTot = "";

            decimal valuePesoNeto = 0;
            string unidad = "";
            var tabla = "";
            var dua = "";
            int cantReg = 0;
            var specificCulture = GetSpecificCulture();
            //GetTablaAndDua(tipoOpe, codPais, ref tabla, ref dua);
            //GetCifTot_PesoNeto_CantReg_ValueCifTot_Unidad_ValuePesoNeto(codPais, tipoOpe, tabla, ref cifTot,
            //    ref pesoNeto,
            //    ref cantReg, ref valueCifTot, ref unidad, ref valuePesoNeto);

            bool FlagPalabras = (!string.IsNullOrWhiteSpace(Session["hdfPalabrasY"]?.ToString()));
            //bool hideTabExcel = cantReg > CantRegMax && !FlagPalabras;
            bool hideTabExcel = Funciones.VisualizarExcel();
            //listGridData = Busca(codPais, tipoOpe, cifTot, pesoNeto, idioma, dua, tabla, indexCboPaisB, valueCifTot,
            //       valuePesoNeto, unidad,
            //       cantReg, specificCulture, hideTabExcel);

            if (nombre == "")
                nuevoFiltro = false;

            
            
            if (nuevoFiltro)
            {
                var auxValue = tipo + id;

                nuevosFiltros.Add(new
                {
                    text = nombre,
                    value = auxValue
                });

                ActualizarSessionListaFitros(auxValue, nombre);

              
                GetTablaAndDua(tipoOpe, codPais, ref tabla, ref dua);

                Session["SqlFiltro"] = GeneraSqlFiltro(codPais, codPais2, tipoOpe, anioMesIni, anioMesFin,
                    indexCboPaisB, codPaisB, dua, auxCodPais);
                //Extensiones.SetCookie("sl", Session["sqlFiltro"].ToString() + " $ " + Session["CodPais"].ToString() + " $ " + Session["TipoOpe"].ToString(), TimeSpan.FromDays(360));
                Extensiones.SetCookie("sl", "", TimeSpan.FromDays(360));
                GuardarLogInicioErrorSession("AgregarFiltroSeleccionado()");
                GetCifTot_PesoNeto_CantReg_ValueCifTot_Unidad_ValuePesoNeto(codPais, tipoOpe, tabla, ref cifTot,
                    ref pesoNeto,
                    ref cantReg, ref valueCifTot, ref unidad, ref valuePesoNeto);

                listGridData = Busca(codPais, tipoOpe, cifTot, pesoNeto, idioma, dua, tabla, indexCboPaisB, valueCifTot,
                    valuePesoNeto, unidad,
                    cantReg, specificCulture, hideTabExcel);


            }

            return Json(new
            {
                nuevoFiltro,
                nuevosFiltros,
                totalRecordsFound = GetDescriptionRecordsFound(cantReg, specificCulture,idioma),
                gridData = listGridData,
                hideTabExcel = hideTabExcel,
                FlagRegMax = cantReg > CantRegMax
            });
        }
        [SessionExpireFilter]
        [HttpPost]
        public JsonResult AgregarFiltrosSeleccionadoAFavoritos(string codPais, string codPais2, string tipoOpe)
        {
            string auxCodPais = codPais;
            ValidaCodPais2(codPais2, ref codPais);
            ValidaCodPaisManif(ref codPais, tipoOpe);
            string idioma = Session["Idioma"].ToString();

            string filtro = Session["hdfFiltroSel"].ToString();
            string id = Session["hdfIDSel"].ToString();

            Session["TipoFavorito"] = filtro;
            ArrayList listaIdsSeleccionados = new ArrayList { id };
            //Session["IDsSeleccionados"] = IDsSeleccionados;

            Session["IdTablaSelected"] = "";
            string tituloFavoritos = "";
            string lblAgregarAFavorito = "";
            List<OptionSelect> listaGrupos = null;

            AgregarFavoritoTable objAgregarFavoritoTable =
                GetAgregarFavoritoTable(filtro, codPais, idioma, listaIdsSeleccionados);
            string favoritosAagregar = RenderViewToString(this.ControllerContext, "_TablaAgregarFavoritos",
                objAgregarFavoritoTable);

            switch (filtro)
            {
                case "Partida":
                    tituloFavoritos = Resources.Resources.Demo_Products_Tab;
                    lblAgregarAFavorito = Resources.AdminResources.Add_ToMyProducts_Button;
                    break;
                case "Importador":
                case "ImportadorExp":
                    tituloFavoritos = Resources.Resources.Demo_Importers_Tab;
                    lblAgregarAFavorito = Resources.AdminResources.Add_ToMyImporters_Button;
                    break;

                case "Exportador":
                case "Proveedor":
                    tituloFavoritos = Resources.Resources.Demo_Exporters_Tab;
                    lblAgregarAFavorito = Resources.AdminResources.Add_ToMyExporters_Button;
                    break;
            }

            string codPaisT = codPais;
            if (codPais == "PEB" || codPais == "PEP")
                codPaisT = "PE";
            else if (codPais2 == "4UE")
                codPaisT = "UE" + auxCodPais;


            DataTable dt = FuncionesBusiness.LlenaGrupos(false, Session["IdUsuario"].ToString(), codPaisT, tipoOpe,
                filtro, idioma);
            listaGrupos = dt.AsEnumerable().Select(m => new OptionSelect()
            {
                value = Convert.ToString(m.Field<Int32>("IdGrupo")),
                text = m.Field<string>("Grupo")
            }).ToList();

            return Json(new
            {
                tituloFavoritos,
                lblAgregarAFavorito,
                favoritosAagregar,
                listaGrupos
            });
        }
#endregion


        private string GeneraSqlFiltroR2(string tipoOpe, string codPais, string codPais2,
            string anioMesIni, string anioMesFin, int indexCboPaisB,
            string codPaisB, string dua, string auxCodPais = "",string filtro = "")
        {
            string sql = "";/*GeneraSqlFiltroR(tipoOpe, codPais, codPais2, anioMesIni, anioMesFin, indexCboPaisB, codPaisB,
                dua, auxCodPais, filtro : filtro);*/
            if(filtro == "InfoTabla")
            {
                if(codPais == "UE")
                {

                }
                sql = Session["SqlInfoComplementario" + codPais].ToString();
            }else if(filtro == "InfoComplementario")
            {
                sql = GeneraSqlFiltro(codPais, codPais2, tipoOpe, anioMesIni, anioMesFin, indexCboPaisB,
                codPaisB, dua, auxCodPais, tabInfo: true);
            }
            else
            {
                sql = GeneraSqlFiltroR(tipoOpe, codPais, codPais2, anioMesIni, anioMesFin, indexCboPaisB, codPaisB,
                dua, auxCodPais, filtro: filtro);
            }
            //sql = sql.Substring(0, sql.Length - 1);

            if (Session["hdfVariable"] != null && filtro != "InfoComplementario" && filtro != "InfoTabla")
                sql += "and " + Session["hdfVariable"].ToString() + " = " + Session["hdfValor"].ToString() + " ";
            if (Session["hdfDUAB2"] != null)
                sql += "and " + dua + " like '%" + Session["hdfDUAB2"].ToString() + "%' ";
            if (Session["hdfDesComercialBB2"] != null)
                sql += "and DesComercial like '%" + Session["hdfDesComercialBB2"].ToString() + "%' ";

            if (!string.IsNullOrWhiteSpace(Session["hdfPalabrasY"]?.ToString()))
                sql += ") ";

            return sql;
        }

        string GeneraSqlFiltroR(string tipoOpe, string codPais, string codPais2,
            string anioMesIni, string anioMesFin, int indexCboPaisB,
            string codPaisB, string dua, string auxCodPais = "", string filtro = "")
        {
            string sql = "";
            var isManif = IsManifiesto(codPais);

            if (!string.IsNullOrWhiteSpace(Session["hdfPalabrasY"]?.ToString()))
            {
                sql += "and contains(Descomercial, '";
                string[] palabrasY = Session["hdfPalabrasY"].ToString().Split(' ');
                bool inicio = true;
                foreach (string palabra in palabrasY)
                {
                    bool band = ValidarIngreso(palabra);
                    if (inicio)
                    {
                        if(band)
                            sql += "\"" + palabra + "*\" ";
                        else
                            sql += "\"" + palabra + "\" ";
                        inicio = false;
                    }
                    else
                    {
                        if(band)
                            sql += "and \"" + palabra + "*\" ";
                        else
                            sql += "and \"" + palabra + "\" ";
                        
                    }
                        
                }

                sql += "') ";



                if (tipoOpe == "I")
                    sql += "and IdImportacion in (select IdImportacion from Importacion_" + ((codPais == "PE" && (filtro == "Modelo" || filtro == "Marca")) ? codPais+"B":codPais) + " where 1 = 1 ";
                else
                    sql += "and IdExportacion in (select IdExportacion from Exportacion_" + ((codPais == "PE" && (filtro == "Modelo" || filtro == "Marca")) ? codPais + "B" : codPais) + " where 1 = 1 ";
            }

            if (codPais2 == "4UE")
            {
                if (tipoOpe == "I")
                    sql += "and IdPaisImp = " + auxCodPais + " ";
                else
                    sql += "and IdPaisExp = " + auxCodPais + " ";
            }

            sql += "and FechaNum >= " + anioMesIni + "00 and FechaNum <= " + anioMesFin + "99 ";
            Session["FechaIni"] = anioMesIni + "00";
            Session["FechaFin"] = anioMesFin + "99";

            if (indexCboPaisB > 0)
            {
                if (tipoOpe == "I")
                    sql += "and " + (!isManif ? "IdPaisOrigen" : "IdPaisEmbarque") + " = " + codPaisB + " ";
                else
                    sql += "and IdPaisDestino = " + codPaisB + " ";
            }

            if (!String.IsNullOrEmpty(Session["hdfNandinaB"]?.ToString()))
            {
                sql += "and (";
                string[] nandinas = Session["hdfNandinaB"].ToString().Split('|');
                foreach (string nandina in nandinas)
                {
                    var existeNandina = (Funciones.BuscaIdPartida(nandina, codPais) != "");
                    if (existeNandina)
                        sql += "IdPartida in (" + Funciones.BuscaIdPartida(nandina, codPais) + ") or ";
                    //sql += "IdPartida = " + Functions.BuscaIdPartida(Nandina, codPais) + " or ";
                    else
                        sql += "IdPartida in (select IdPartida from Partida_" + codPais + " where Nandina like '" +
                               nandina + "%') or ";
                }

                sql = sql.Substring(0, sql.Length - 3) + ") ";
            }

            if (!String.IsNullOrEmpty(Session["hdfImportadorB"]?.ToString()))
            {
                if (!isManif)
                {
                    sql += "and (";
                    string[] importadores = Session["hdfImportadorB"].ToString().Split('|');
                    foreach (string importador in importadores)
                    {
                        if (importador.Substring(0, 1) == "[")
                            sql += "IdImportador = " + importador.Replace("[", "").Replace("]", "") + " or ";
                        else
                        {
                            if (codPais == "PE")
                                sql += "IdImportador in (select IdEmpresa from Empresa_PE_con_datos where 1 = 1 ";
                            else
                                sql += "IdImportador in (select IdEmpresa from Empresa_" + codPais + " where 1 = 1 ";

                            string[] palabras = importador.Split(' ');
                            foreach (string palabra in palabras)
                                sql += "and Empresa like '%" + palabra + "%' ";
                            sql += ") or ";
                        }
                    }

                    sql = sql.Substring(0, sql.Length - 3) + ") ";
                }
                else
                {

                    if (codPais == "PE")
                        sql += "and IdExportador in (select IdEmpresa from Empresa_PE_con_datos where 1 = 1 ";
                    else
                        sql += tipoOpe == "I" ? "and IdImportador in (select IdEmpresa from Empresa_" + codPais + " where 1 = 1 " :
                                               "and IdExportador in (select IdEmpresa from Empresa_" + codPais + " where 1 = 1 ";

                    string[] Palabras = Session["hdfImportadorB"].ToString().Split(' ');
                    foreach (string Palabra in Palabras)
                        sql += "and Empresa like '%" + Palabra + "%' ";
                    sql += ") ";
                }
            }

            if (!String.IsNullOrEmpty(Session["hdfExportadorB"]?.ToString()))
            {
                if (!isManif)
                {
                    sql += "and (";
                    string[] exportadores = Session["hdfExportadorB"].ToString().Split('|');
                    foreach (string exportador in exportadores)
                    {
                        if (exportador.Substring(0, 1) == "[")
                            sql += "IdExportador = " + exportador.Replace("[", "").Replace("]", "") + " or ";
                        else
                        {
                            if (codPais == "PE")
                                sql += "IdExportador in (select IdEmpresa from Empresa_PE_con_datos where 1 = 1 ";
                            else
                                sql += "IdExportador in (select IdEmpresa from Empresa_" + codPais + " where 1 = 1 ";

                            string[] palabras = exportador.Split(' ');
                            foreach (string palabra in palabras)
                                sql += "and Empresa like '%" + palabra + "%' ";
                            sql += ") or ";
                        }
                    }

                    sql = sql.Substring(0, sql.Length - 3) + ") ";
                }
                else
                {
                    if (codPais == "PE")
                        sql += "and IdExportador in (select IdEmpresa from Empresa_PE_con_datos where 1 = 1 ";
                    else
                        sql += "and IdExportador in (select IdEmpresa from Empresa_" + codPais + " where 1 = 1 ";

                    string[] Palabras = Session["hdfExportadorB"].ToString().Split(' ');
                    foreach (string Palabra in Palabras)
                        sql += "and Empresa like '%" + Palabra + "%' ";
                    sql += ") ";
                }
            }

            if (!String.IsNullOrEmpty(Session["hdfProveedorB"]?.ToString()))
            {
                if (!isManif)
                {
                    sql += "and IdProveedor in (select IdProveedor from Proveedor_" + codPais + " where ";
                    string[] palabras = Session["hdfProveedorB"].ToString().Split('|');
                    foreach (string palabra in palabras)
                    {
                        var palabraAux = (palabra.Replace("[TODOS]", "")).Trim();
                        palabraAux = (palabraAux.Replace("[ALL]", "")).Trim();
                        sql += "Proveedor like '%" + palabraAux + "%' or ";
                    }
                    sql = sql.Substring(0, sql.Length - 3) + ")";
                }
                else
                {
                    sql += "and IdProveedor in (select IdProveedor from Proveedor_" + codPais + " PR where 1 = 1 ";
                    string[] Palabras = Session["hdfProveedorB"].ToString().Split(' ');
                    foreach (string Palabra in Palabras)
                    {
                        var palabraAux = (Palabra.Replace("[TODOS]", "")).Trim();
                        palabraAux = (palabraAux.Replace("[ALL]", "")).Trim();
                        sql += "and Proveedor like '%" + palabraAux + "%' ";
                    }
                    sql += ") ";
                }
            }

            if (!String.IsNullOrEmpty(Session["hdfImportadorExpB"]?.ToString()))
            {
                if (!isManif)
                {
                    sql += "and IdImportadorExp in (select IdImportadorExp from ImportadorExp_" + codPais + " where ";

                    string[] palabras = Session["hdfImportadorExpB"].ToString().Split('|');

                    foreach (string palabra in palabras)
                    {
                        var palabraAux = (palabra.Replace("[TODOS]", "")).Trim();
                        palabraAux = (palabraAux.Replace("[ALL]", "")).Trim();
                        sql += "ImportadorExp like '%" + palabraAux + "%' or ";
                    }
                    sql = sql.Substring(0, sql.Length - 3) + ")";
                }
                else
                {
                    sql += "and IdImportadorExp in (select IdImportadorExp from ImportadorExp_" + codPais +
                           " IE where 1 = 1 ";
                    string[] Palabras = Session["hdfImportadorExpB"].ToString().Split(' ');
                    foreach (string Palabra in Palabras)
                    {
                        var palabraAux = (Palabra.Replace("[TODOS]", "")).Trim();
                        palabraAux = (palabraAux.Replace("[ALL]", "")).Trim();
                        sql += "and IE.ImportadorExp like '%" + palabraAux + "%' ";
                    }
                    sql += ") ";
                }
            }

            if (!isManif)
            {
                if (!String.IsNullOrEmpty(Session["PartidasB"]?.ToString()))
                    sql += "and IdPartida in " + Funciones.ListaItems((ArrayList)Session["PartidasB"]) + " ";
                if (!String.IsNullOrEmpty(Session["MarcasB"]?.ToString()))
                    sql += "and IdMarca in " + Funciones.ListaItems((ArrayList)Session["MarcasB"]) + " ";
                if (!String.IsNullOrEmpty(Session["ModelosB"]?.ToString()))
                    sql += "and IdModelo in " + Funciones.ListaItems((ArrayList)Session["ModelosB"]) + " ";
                if (!String.IsNullOrEmpty(Session["ImportadoresB"]?.ToString()))
                    sql += "and IdImportador in " + Funciones.ListaItems((ArrayList)Session["ImportadoresB"]) + " ";
                if (!String.IsNullOrEmpty(Session["ExportadoresB"]?.ToString()))
                    sql += "and IdExportador in " + Funciones.ListaItems((ArrayList)Session["ExportadoresB"]) + " ";
                if (!String.IsNullOrEmpty(Session["ProveedoresB"]?.ToString()))
                    sql += "and IdProveedor in " + Funciones.ListaItems((ArrayList)Session["ProveedoresB"]) + " ";
                if (!String.IsNullOrEmpty(Session["ImportadoresExpB"]?.ToString()))
                    sql += "and IdImportadorExp in " + Funciones.ListaItems((ArrayList)Session["ImportadoresExpB"]) + " ";
                if (!String.IsNullOrEmpty(Session["PaisesOrigenB"]?.ToString()))
                    sql += "and IdPaisOrigen in " + Funciones.ListaItems((ArrayList)Session["PaisesOrigenB"]) + " ";
                if (!String.IsNullOrEmpty(Session["PaisesDestinoB"]?.ToString()))
                    sql += "and IdPaisDestino in " + Funciones.ListaItems((ArrayList)Session["PaisesDestinoB"]) + " ";
                if (!String.IsNullOrEmpty(Session["PtosDescargaB"]?.ToString()))
                    sql += "and PtoDescarga in " + Funciones.ListaItems((ArrayList)Session["PtosDescargaB"]) + " ";
                if (!String.IsNullOrEmpty(Session["PaisesEmbarqueB"]?.ToString()))
                    sql += "and IdPaisEmbarque in " + Funciones.ListaItems((ArrayList)Session["PaisesEmbarqueB"]) + " ";
                if (!String.IsNullOrEmpty(Session["ViasTranspB"]?.ToString()))
                    sql += "and IdViaTransp in " + Funciones.ListaItems((ArrayList)Session["ViasTranspB"]) + " ";
                if (!String.IsNullOrEmpty(Session["AduanaDUAsB"]?.ToString()))
                    sql += "and convert(varchar(3), IdAduana) + '-' + convert(varchar(20), " + dua + ") in " +
                           Funciones.ListaItemsS((ArrayList)Session["AduanaDUAsB"]) + " ";
                if (!String.IsNullOrEmpty(Session["AduanasB"]?.ToString()))
                    sql += "and IdAduana in " + Funciones.ListaItems((ArrayList)Session["AduanasB"]) + " ";
                //if (Session["DUAsB"] != null)
                //    sql += "and " + DUA + " like '" + Functions.ListaItems((ArrayList)Session["DUAsB"]).Replace("(", "").Replace(")", "") + "%' ";
                if (!String.IsNullOrEmpty(Session["DistritosB"]?.ToString()))
                    sql += "and IdDistrito in " + Funciones.ListaItems((ArrayList)Session["DistritosB"]) + " ";
            }
            else
            {
                if (!String.IsNullOrEmpty(Session["ImportadoresB"]?.ToString()))
                    sql += "and IdImportador in " + Funciones.ListaItems((ArrayList)Session["ImportadoresB"]) + " ";
                if (!String.IsNullOrEmpty(Session["NotificadosB"]?.ToString()))
                    sql += "and IdNotificado in " + Funciones.ListaItems((ArrayList)Session["NotificadosB"]) + " ";
                if (!String.IsNullOrEmpty(Session["ExportadoresB"]?.ToString()))
                    sql += "and IdExportador in " + Funciones.ListaItems((ArrayList)Session["ExportadoresB"]) + " ";
                if (!String.IsNullOrEmpty(Session["ProveedoresB"]?.ToString()))
                    sql += "and IdProveedor in " + Funciones.ListaItems((ArrayList)Session["ProveedoresB"]) + " ";
                if (!String.IsNullOrEmpty(Session["ImportadoresExpB"]?.ToString()))
                    sql += "and IdImportadorExp in " + Funciones.ListaItems((ArrayList)Session["ImportadoresExpB"]) + " ";
                if (!String.IsNullOrEmpty(Session["PaisesOrigenB"]?.ToString()))
                    sql += "and IdPaisOrigen in " + Funciones.ListaItems((ArrayList)Session["PaisesOrigenB"]) + " ";
                if (!String.IsNullOrEmpty(Session["PaisesEmbarqueB"]?.ToString()))
                    sql += "and IdPaisEmbarque in " + Funciones.ListaItems((ArrayList)Session["PaisesEmbarqueB"]) + " ";
                if (!String.IsNullOrEmpty(Session["PtosDescargaB"]?.ToString()))
                    sql += "and PtoDescarga in " + Funciones.ListaItems((ArrayList)Session["PtosDescargaB"]) + " ";
                if (!String.IsNullOrEmpty(Session["PaisesDestinoB"]?.ToString()))
                    sql += "and IdPaisDestino in " + Funciones.ListaItems((ArrayList)Session["PaisesDestinoB"]) + " ";
                if (!String.IsNullOrEmpty(Session["PtosDescargaB"]?.ToString()))
                    sql += "and IdPtoDescarga in " + Funciones.ListaItems((ArrayList)Session["PtosDescargaB"]) + " ";
                if (!String.IsNullOrEmpty(Session["PtosEmbarqueB"]?.ToString()))
                    sql += "and IdPtoEmbarque in " + Funciones.ListaItems((ArrayList)Session["PtosEmbarqueB"]) + " ";
                if (!String.IsNullOrEmpty(Session["PtosDestinoB"]?.ToString()))
                    sql += "and IdPtoDestino in " + Funciones.ListaItems((ArrayList)Session["PtosDestinoB"]) + " ";
                if (!String.IsNullOrEmpty(Session["ManifiestosB"]?.ToString()))
                    sql += "and Manifiesto in " + Funciones.ListaItemsS((ArrayList)Session["ManifiestosB"]) + " ";
            }

            

            if (!String.IsNullOrEmpty(Session["hdfIdGrupoB"]?.ToString()))
                sql += "and Id" + Session["hdfTipoFavoritoB"].ToString() +
                       " in (select IdFavorito from FavoritoGrupo where IdGrupo = " +
                       Session["hdfIdGrupoB"].ToString() + ") ";

            return sql;
        }

        [HttpPost]
        public JsonResult VerInformaColombia(string ruc, string codPais, string tipoOpe, string fechaInicial,
            string fechaFin)
        {
            InformaColombia informa = null;
            try
            {
                informa = InformaColombia.obtenerInforme(Convert.ToInt32(ruc));

                string logs = ruc + " | " + informa.RazonSocial;
                Funciones.GrabaLog(Session["IdUsuario"].ToString(), codPais, tipoOpe, fechaInicial, fechaFin, "InformaColombia", logs);
            } catch (Exception e)
            {
                ExceptionUtility.LogException(e, "MisBusquedasController.VerInformaColombia");
            }

            return Json(new
            {
                informaColombia = RenderViewToString(this.ControllerContext, "Modals/_InformaColombia", informa)
            });
        }

        [HttpPost]
        public JsonResult ObtenerImportador(string codPais, int idImportacion)
        {
            int idImportador = 0;
            try
            {
                DataRow dr = FuncionesBusiness.ObtenerImportador(codPais, idImportacion);
                idImportador = (int)dr[0];
            }
            catch (Exception e)
            {
                ExceptionUtility.LogException(e, "MisBusquedasController.VerInformaColombia");
            }

            return Json(new
            {
                idImportador
            });
        }

        [HttpPost]
        //Obtiene html de sitio web externo consultando a las fuentes de un iframe por medio de una cookie
        public JsonResult VerArancelesPartida(string cod_partida)
        {
            string urlConsultaPartida = "http://www.aduanet.gob.pe/itarancel/arancelS01Alias?accion=buscarPartida&esframe=1&cod_partida=" + cod_partida;
            string urlDetallePartidaArancel = "http://www.aduanet.gob.pe/itarancel/JSPDetallePartidaArancel.jsp";

            //Consulta al sitio externo para obtener cookie
            HttpWebRequest wrConsultaPartida =(HttpWebRequest)WebRequest.Create(urlConsultaPartida);
            HttpWebResponse TheResponse = (HttpWebResponse)wrConsultaPartida.GetResponse();
            var cookieHeader = TheResponse.Headers[HttpResponseHeader.SetCookie];

            //Obtiene html de un iframe del sitio web
            WebClient rqDetallePartidaArancel = new WebClient();
            rqDetallePartidaArancel.Headers["Cookie"] = cookieHeader;
            string detallePartidaArancel = rqDetallePartidaArancel.DownloadString(urlDetallePartidaArancel);

            detallePartidaArancel = detallePartidaArancel.Replace("\n", String.Empty);
            detallePartidaArancel = detallePartidaArancel.Replace("\r", String.Empty);
            detallePartidaArancel = detallePartidaArancel.Trim();
            
            string obtenerDiv = @"<\s*div[^>]*>(.*?)<\s*/div\s*>";
            string obtenerFont = @"<\s*font[^>]*>(.*?)<\s*/font\s*>";
            string hrefLowerPattern = @"href\s*=\s*(?:[""'](?<1>[^""']*)[""']|(?<1>\S+))";
            string hrefUpperPattern = @"HREF\s*=\s*(?:[""'](?<1>[^""']*)[""']|(?<1>\S+))";

            Match match = Regex.Match(detallePartidaArancel, obtenerFont);
            string fontDetallePartidaArancel = match.Groups[0].Value;
            fontDetallePartidaArancel = fontDetallePartidaArancel.Replace("TIPO DE PRODUCTO:", String.Empty).Replace("<BR>", String.Empty);
            //style = "color:white";
            match = Regex.Match(detallePartidaArancel, obtenerDiv);
            string divDetallePartidaArancel = match.Groups[0].Value;

            Regex hrefRemove = new Regex(hrefLowerPattern);
            divDetallePartidaArancel = hrefRemove.Replace(divDetallePartidaArancel, string.Empty);
            hrefRemove = new Regex(hrefUpperPattern);
            divDetallePartidaArancel = hrefRemove.Replace(divDetallePartidaArancel, string.Empty);
            divDetallePartidaArancel = divDetallePartidaArancel.Replace("<a >", string.Empty).Replace("</a>", String.Empty).Replace("<b>Detalle</b>", "Detalle");//.Replace("AADDCC", "30a7e5");
            divDetallePartidaArancel = divDetallePartidaArancel.Replace("BGCOLOR='AADDCC'", "BGCOLOR='30a7e5' style='color:white;'");
            return Json(new
            {
                fontDetallePartidaArancel,
                divDetallePartidaArancel
            });
        }

        // Ruben 202301
        SentinelPeru.CNSDTConRapWS BuscaDatosSentinel2(string Usuario, string Password, string Servicio, string TipoDoc, string NroDoc)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var client = new RestClient("https://www2.sentinelperu.com/ws/asentinelws02.aspx");
            client.Timeout = -1;

            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/xml");

            var body = @"<?xml version=""1.0"" encoding=""utf-8""?>" + "\n" +
            @"<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/""> " + "\n" +
            @"    <soap:Body>" + "\n" +
            @"        <sentinelws02.Execute xmlns=""AFPrivado"">" + "\n" +
            @"            <Usuario>" + Usuario + "</Usuario>" + "\n" +
            @"            <Password>" + Password + "</Password>" + "\n" +
            @"            <Servicio>" + Servicio + "</Servicio>" + "\n" +
            @"            <Tipodoc>" + TipoDoc + "</Tipodoc>" + "\n" +
            @"            <Nrodoc>" + NroDoc + "</Nrodoc>" + "\n" +
            @"        </sentinelws02.Execute>" + "\n" +
            @"    </soap:Body>" + "\n" +
            @"</soap:Envelope>";

            request.AddParameter("application/xml", body, ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            string XML = response.Content;
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(XML);

            SentinelPeru.CNSDTConRapWS cNSDTConRapWS = new SentinelPeru.CNSDTConRapWS();

            cNSDTConRapWS.Documento = xmldoc.GetElementsByTagName("Documento")[0].InnerText;
            cNSDTConRapWS.RazonSocial = xmldoc.GetElementsByTagName("RazonSocial")[0].InnerText;
            cNSDTConRapWS.FechaProceso = Convert.ToDateTime(xmldoc.GetElementsByTagName("FechaProceso")[0].InnerText);
            cNSDTConRapWS.Semaforos = xmldoc.GetElementsByTagName("Semaforos")[0].InnerText;
            cNSDTConRapWS.Score = xmldoc.GetElementsByTagName("Score")[0].InnerText;

            cNSDTConRapWS.NroBancos = xmldoc.GetElementsByTagName("NroBancos")[0].InnerText;
            cNSDTConRapWS.DeudaTotal = xmldoc.GetElementsByTagName("DeudaTotal")[0].InnerText;
            cNSDTConRapWS.VencidoBanco = xmldoc.GetElementsByTagName("VencidoBanco")[0].InnerText;
            cNSDTConRapWS.Calificativo = xmldoc.GetElementsByTagName("Calificativo")[0].InnerText;
            cNSDTConRapWS.Veces24m = xmldoc.GetElementsByTagName("Veces24m")[0].InnerText;

            cNSDTConRapWS.ScorePromedio = Convert.ToDouble(xmldoc.GetElementsByTagName("ScorePromedio")[0].InnerText);
            cNSDTConRapWS.SemaActual = xmldoc.GetElementsByTagName("SemaActual")[0].InnerText;
            cNSDTConRapWS.SemaPrevio = xmldoc.GetElementsByTagName("SemaPrevio")[0].InnerText;
            cNSDTConRapWS.SemaPeorMejor = xmldoc.GetElementsByTagName("SemaPeorMejor")[0].InnerText;
            cNSDTConRapWS.Documento2 = xmldoc.GetElementsByTagName("Documento2")[0].InnerText;

            cNSDTConRapWS.EstDomic = xmldoc.GetElementsByTagName("EstDomic")[0].InnerText;
            cNSDTConRapWS.CondDomic = xmldoc.GetElementsByTagName("CondDomic")[0].InnerText;
            cNSDTConRapWS.DeudaTributaria = xmldoc.GetElementsByTagName("DeudaTributaria")[0].InnerText;
            cNSDTConRapWS.DeudaLaboral = xmldoc.GetElementsByTagName("DeudaLaboral")[0].InnerText;
            cNSDTConRapWS.DeudaImpaga = xmldoc.GetElementsByTagName("DeudaImpaga")[0].InnerText;

            cNSDTConRapWS.DeudaProtestos = xmldoc.GetElementsByTagName("DeudaProtestos")[0].InnerText;
            cNSDTConRapWS.DeudaSBS = xmldoc.GetElementsByTagName("DeudaSBS")[0].InnerText;
            cNSDTConRapWS.TarCtas = xmldoc.GetElementsByTagName("TarCtas")[0].InnerText;
            cNSDTConRapWS.RepNeg = xmldoc.GetElementsByTagName("RepNeg")[0].InnerText;
            cNSDTConRapWS.TipoActv = xmldoc.GetElementsByTagName("TipoActv")[0].InnerText;

            cNSDTConRapWS.FechIniActv = xmldoc.GetElementsByTagName("FechIniActv")[0].InnerText;
            cNSDTConRapWS.CodigoWS = xmldoc.GetElementsByTagName("CodigoWS")[0].InnerText;

            return cNSDTConRapWS;
        }

        // Ruben 202301
        [HttpPost]
        public JsonResult VerSentinel(string nroDoc, string codPais, string tipoOpe, string fechaInicial,
            string fechaFin)
        {

           /* object objMensaje = null;

            if (!FuncionesBusiness.IsVisibleSentinal(Funciones.ObtieneIdPlan(Session["IdUsuario"].ToString())))
            {
                objMensaje = new
                {
                    titulo = "Sentinel",
                    mensaje = Resources.AdminResources.SentinelValidation_Text,
                    flagContactenos = true
                };
            }*/
            
            string Usuario = "07789552";
            string Password = Resources.Resources.SentinelPassword;
            int Servicio = 130773;

            string TipoDoc = "R";

            CultureInfo cultureInfo = new System.Globalization.CultureInfo("es-PE");
            SentinelPeru.CNSDTConRapWS resultado = null;
            try
            {
                //System.Net.ServicePointManager.SecurityProtocol = (System.Net.SecurityProtocolType)(0xc00);
                //SentinelPeru.SentinelWS02 ws = new SentinelPeru.SentinelWS02();

                string Logs2 = Usuario + "|" + Password + "|" + Servicio + "|" + TipoDoc + "|" + nroDoc;
                Funciones.GrabaLog(Session["IdUsuario"].ToString(), codPais, tipoOpe, fechaInicial, fechaFin, "Sentinel", Logs2);

                resultado = BuscaDatosSentinel2(Usuario, Password, Servicio.ToString(), TipoDoc, nroDoc);
                //resultado = ws.Execute(Usuario, Password, Servicio, TipoDoc, nroDoc);
            }
            catch(Exception ex)
            {                                                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                throw new Exception("error sentinel: " + ex.Message + " , RUC : " + TipoDoc + " - " + nroDoc);
            }

            InfoSentinel objInfoSentinel = new InfoSentinel();

            try
            {               
                if (resultado.CodigoWS == "0")
                {
                    objInfoSentinel.RazonSocial = resultado.RazonSocial;
                    objInfoSentinel.Documento = resultado.Documento;
                    objInfoSentinel.FechaInicioActvidades = resultado.FechIniActv;
                    objInfoSentinel.TipoActividad = resultado.TipoActv;

                    objInfoSentinel.InfoSentinelTabla1.FechaProceso =
                        Convert.ToDateTime(resultado.FechaProceso).ToString("dd/MM/yyyy");
                    objInfoSentinel.InfoSentinelTabla1.Score =
                        Convert.ToDecimal(resultado.Score, cultureInfo).ToString("n3", cultureInfo);
                    objInfoSentinel.InfoSentinelTabla1.DeudaTotal =
                        "S/ " + Convert.ToDecimal(resultado.DeudaTotal, cultureInfo).ToString("n0", cultureInfo);
                    objInfoSentinel.InfoSentinelTabla1.SemaforoSemanaActual = (resultado.SemaActual != ""
                        ? FuncionesBusiness.BuscaDescripcionSentinel("7SEM", resultado.SemaActual)
                        : "gris");
                    objInfoSentinel.InfoSentinelTabla1.SemaforoSemanaPrevio = (resultado.SemaPrevio != ""
                        ? FuncionesBusiness.BuscaDescripcionSentinel("7SEM", resultado.SemaPrevio)
                        : "gris");

                    objInfoSentinel.InfoSentinelTabla1.DeudaTributaria =
                        "S/ " + Convert.ToDecimal(resultado.DeudaTributaria, cultureInfo).ToString("n0", cultureInfo);
                    objInfoSentinel.InfoSentinelTabla1.DeudaLaboral =
                        "S/ " + Convert.ToDecimal(resultado.DeudaLaboral, cultureInfo).ToString("n0", cultureInfo);
                    objInfoSentinel.InfoSentinelTabla1.DeudaImpaga =
                        "S/ " + Convert.ToDecimal(resultado.DeudaImpaga, cultureInfo).ToString("n0", cultureInfo);
                    objInfoSentinel.InfoSentinelTabla1.DeudaProtestos =
                        "S/ " + Convert.ToDecimal(resultado.DeudaProtestos, cultureInfo).ToString("n0", cultureInfo);
                    objInfoSentinel.InfoSentinelTabla1.ScorePromedio = resultado.ScorePromedio.ToString("n3", cultureInfo);

                    string semaforos1 = resultado.Semaforos;
                    semaforos1 = semaforos1.Replace("A", "ambar ");
                    semaforos1 = semaforos1.Replace("V", "verde ");
                    semaforos1 = semaforos1.Replace("G", "gris ");
                    semaforos1 = semaforos1.Replace("R", "rojo ");
                    string[] semaforos = semaforos1.Split(' ');
                    objInfoSentinel.Semaforos = semaforos;

                    objInfoSentinel.InfoSentinelTabla2.NroEntidades = resultado.NroBancos;
                    objInfoSentinel.InfoSentinelTabla2.CalificacionSBSMicrof =
                        (resultado.NroBancos != "0" ? resultado.Calificativo : "-");
                    objInfoSentinel.InfoSentinelTabla2.DeudaSBSMicrof =
                        "S/ " + Convert.ToDecimal(resultado.DeudaSBS, cultureInfo).ToString("n0", cultureInfo);
                    objInfoSentinel.InfoSentinelTabla2.DeudaSBSMicrofVencida =
                        "S/ " + Convert.ToDecimal(resultado.VencidoBanco, cultureInfo).ToString("n0", cultureInfo);
                    objInfoSentinel.InfoSentinelTabla2.TotalTarjetasCtasAnuladasCerradas = resultado.TarCtas;
                    objInfoSentinel.InfoSentinelTabla2.Veces24m = resultado.Veces24m;
                    objInfoSentinel.InfoSentinelTabla2.EstadoDomicilioFiscal =
                        FuncionesBusiness.BuscaDescripcionSentinel("2EDM", resultado.EstDomic);
                    objInfoSentinel.InfoSentinelTabla2.CondicionDomicioFiscal =
                        FuncionesBusiness.BuscaDescripcionSentinel("3CDM", Convert.ToInt32(resultado.CondDomic).ToString());
                    objInfoSentinel.InfoSentinelTabla2.TotalReportesNegativos = resultado.RepNeg;
                }

                string logs;
                if (resultado.CodigoWS == "0")
                    logs = "0|" + nroDoc + "|" + resultado.RazonSocial;
                else
                    logs = "1|" + nroDoc;

                Funciones.GrabaLog(Session["IdUsuario"].ToString(), codPais, tipoOpe, fechaInicial, fechaFin, "Sentinel", logs);

                Session["hdfIdLog"] = FuncionesBusiness.ObtieneMaxIdLog(IdUsuario, "Sentinel");
                Session["hdfLogs"] = logs;
            }
            catch (Exception ex)
            {
                Funciones.GrabaLog(Session["IdUsuario"].ToString(), codPais, tipoOpe, fechaInicial, fechaFin, "Sentinel", ex.Message);
            }

            return Json(new { infoSentinel = RenderViewToString(this.ControllerContext, "Modals/_InfoSentinel", objInfoSentinel) });
        }

        [HttpPost]
        public JsonResult IrASentinel()
        {
            var logs = Session["hdfLogs"].ToString();
            logs = "2|" + logs.Substring(2);

            FuncionesBusiness.ActualizaLog(Session["hdfIdLog"].ToString(), logs);
            return Json(new
            {
                respuesta = "ok"
            });
        }

#region RespotesExcel
        void CalculaFormulasExcel(OfficeOpenXml.ExcelWorksheet ws, DataTable dt)
        {
            int i = 7;
            int t = i + dt.Rows.Count;

            ws.Cells["A" + t.ToString()].Value = "Total";
            ws.Cells["B" + t.ToString()].Formula = "=SUM(B7:B" + (t - 1).ToString() + ")";
            ws.Cells["C" + t.ToString()].Formula = "=SUM(C7:C" + (t - 1).ToString() + ")";
            ws.Cells["D" + t.ToString()].Formula = "=SUM(D7:D" + (t - 1).ToString() + ")";
            foreach (DataRow dr in dt.Rows)
            {
                ws.Cells["D" + i.ToString()].FormulaR1C1 = "=R[0]C[-1]/R" + t.ToString() + "C[-1]";
                i += 1;
            }
            ws.Cells["A" + t.ToString() + ":D" + t.ToString()].Style.Font.Color
                .SetColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));
            ws.Cells["A" + t.ToString() + ":D" + t.ToString()].Style.Font.Bold = true;
            ws.Cells["A" + t.ToString() + ":D" + t.ToString()].Style.Fill.PatternType =
                OfficeOpenXml.Style.ExcelFillStyle.Solid;
            ws.Cells["A" + t.ToString() + ":D" + t.ToString()].Style.Fill.BackgroundColor
                .SetColor(System.Drawing.Color.FromArgb(48, 167, 229));
        }

        string  GetExcelResumenManif(string Opcion, string TipoOpe, string CodPais,
            string textPais, string codPais2, string fechaIni,
            string fechaFin, string Filtros, string Idioma, DateTime FechaArchivo)
        {
            var vFlag = new TabMisBusquedas(TipoOpe, CodPais);  //new FlagVarVisibles(CodPais, TipoOpe, true);
            string NombreArchivo;
            //DateTime FechaArchivo;
            DataTable dtImportador = null,
                dtNotificado = null,
                dtProveedor = null,
                dtPaisEmbarque = null,
                dtPtoDescarga = null,
                dtManifiesto = null;

            if (TipoOpe == "I")
            {
                dtImportador = FuncionesBusiness.GeneraDt(Session["sqlImportador"]?.ToString());
                dtProveedor = FuncionesBusiness.GeneraDt(Session["sqlProveedor"]?.ToString());
                dtPaisEmbarque = FuncionesBusiness.GeneraDt(Session["sqlPaisEmbarque"]?.ToString());

                dtImportador.Columns.Remove("IdImportador");
                dtProveedor.Columns.Remove("IdProveedor");
                //if (!(vFlag.IsManifiesto&& CodPais=="BRI"))
                //{
                    dtPaisEmbarque.Columns.Remove("IdPaisEmbarque");
                //}
                
            }
            else
            {
                dtImportador = FuncionesBusiness.GeneraDt(Session["sqlExportador"].ToString());
                if (vFlag.ExisteImportadorExp)
                    dtProveedor = FuncionesBusiness.GeneraDt(Session["sqlImportadorExp"].ToString());
                dtPaisEmbarque = FuncionesBusiness.GeneraDt(Session["sqlPaisDestino"].ToString());

                dtImportador.Columns.Remove("IdExportador");
                if (vFlag.ExisteImportadorExp) dtProveedor.Columns.Remove("IdImportadorExp");
                dtPaisEmbarque.Columns.Remove("IdPaisDestino");
            }

            if (vFlag.ExisteNotificado)
            {
                dtNotificado = FuncionesBusiness.GeneraDt(Session["sqlNotificado"].ToString());
                dtNotificado.Columns.Remove("IdNotificado");
            }

            if (vFlag.ExistePtoDescarga)
            {
                dtPtoDescarga = FuncionesBusiness.GeneraDt(Session["sqlPtoDescarga"].ToString());
                dtPtoDescarga.Columns.Remove("IdPtoDescarga");
            }
            else if (vFlag.ExistePtoEmbarque)
            {
                dtPtoDescarga = FuncionesBusiness.GeneraDt(Session["sqlPtoEmbarque"].ToString());
                dtPtoDescarga.Columns.Remove("IdPtoEmbarque");
            }
            else if (vFlag.ExistePtoDestino)
            {
                dtPtoDescarga = FuncionesBusiness.GeneraDt(Session["sqlPtoDestino"].ToString());
                dtPtoDescarga.Columns.Remove("IdPtoDestino");
            }

            if (vFlag.ExisteManifiesto)
                dtManifiesto = FuncionesBusiness.GeneraDt(Session["sqlManifiesto"].ToString());

            string templateName = HostingEnvironment.MapPath("~/ExcelTemplate/VeritradeResumenTemplateUS.xlsx");
            using (OfficeOpenXml.ExcelPackage package =
                new OfficeOpenXml.ExcelPackage(new System.IO.FileInfo(templateName)))
            {
                if (Opcion != "Todo")
                {
                    if ((TipoOpe == "I" && Opcion != "Importador") || TipoOpe == "E")
                        package.Workbook.Worksheets["Importadores - Consignatarios"].Hidden =
                            OfficeOpenXml.eWorkSheetHidden.VeryHidden;
                    if ((TipoOpe == "E" && Opcion != "Exportador"))
                        package.Workbook.Worksheets["Exportadores - Embarcadores"].Hidden =
                            OfficeOpenXml.eWorkSheetHidden.VeryHidden;
                    if (Opcion != "Notificado")
                        package.Workbook.Worksheets["Tramitadores - Notificados"].Hidden =
                            OfficeOpenXml.eWorkSheetHidden.VeryHidden;
                    if (TipoOpe == "I" && Opcion != "Proveedor")
                        package.Workbook.Worksheets["Exportadores - Embarcadores"].Hidden =
                            OfficeOpenXml.eWorkSheetHidden.VeryHidden;
                    if ((TipoOpe == "E" && Opcion != "ImportadorExp") || TipoOpe == "I")
                        package.Workbook.Worksheets["Importadores - Consignatarios_"].Hidden =
                            OfficeOpenXml.eWorkSheetHidden.VeryHidden;
                    if (Opcion != "PaisEmbarque" && Opcion != "PaisDestino")
                        package.Workbook.Worksheets["Países Embarque"].Hidden =
                            OfficeOpenXml.eWorkSheetHidden.VeryHidden;
                    if (Opcion != "PtoDescarga" && Opcion != "PtoEmbarque" && Opcion != "PtoDestino")
                        package.Workbook.Worksheets["Puertos Descarga"].Hidden =
                            OfficeOpenXml.eWorkSheetHidden.VeryHidden;
                    if (Opcion != "Manifiesto")
                        package.Workbook.Worksheets["Manifiestos"].Hidden = OfficeOpenXml.eWorkSheetHidden.VeryHidden;
                }
                else
                {
                    if ((!vFlag.ExisteImportador && TipoOpe == "I") || TipoOpe == "E")
                        package.Workbook.Worksheets["Importadores - Consignatarios"].Hidden =
                            OfficeOpenXml.eWorkSheetHidden.VeryHidden;
                    if (!vFlag.ExisteExportador && !vFlag.ExisteProveedor)
                        package.Workbook.Worksheets["Exportadores - Embarcadores"].Hidden =
                            OfficeOpenXml.eWorkSheetHidden.VeryHidden;
                    if (!vFlag.ExisteImportadorExp || TipoOpe == "I")
                        package.Workbook.Worksheets["Importadores - Consignatarios_"].Hidden =
                            OfficeOpenXml.eWorkSheetHidden.VeryHidden;
                    if (!vFlag.ExisteNotificado)
                        package.Workbook.Worksheets["Tramitadores - Notificados"].Hidden =
                            OfficeOpenXml.eWorkSheetHidden.VeryHidden;
                    if (!vFlag.ExisteManifiesto)
                        package.Workbook.Worksheets["Manifiestos"].Hidden = OfficeOpenXml.eWorkSheetHidden.VeryHidden;
                }

                string PaisTipoOpe = FuncionesBusiness.Pais(CodPais, Idioma).ToUpper();
                //string Filtros = FuncionesBusiness.FiltrosEnExcel(lstFiltros);
                //string Rango = lPeriodo + ": " + FuncionesBusiness.RangoFechas(Session["FechaIni"].ToString(),
                //                   Session["FechaFin"].ToString(), Idioma);
                string Rango = Resources.AdminResources.Period_Text + ": " + GetRangoFechas(fechaIni, fechaFin, Idioma);

                OfficeOpenXml.ExcelWorksheet ws;
                if (TipoOpe == "I")
                {
                    ws = package.Workbook.Worksheets["Importadores - Consignatarios"];
                    ws.Name = Resources.Resources.Demo_Importers_Tab_Manif;
                    ws.Cells["B1"].Value = PaisTipoOpe;
                    ws.Cells["B2"].Value = Filtros;
                    ws.Cells["B3"].Value = Rango;
                    ws.Cells["A6"].Value = Resources.Resources.Search_Form_Item05;
                    ws.Cells["B6"].Value = Resources.Resources.Grid_Column_TotalRecords;
                    ws.Cells["C6"].Value = "Total " + Resources.Resources.PesoBruto_Text + " Tn";
                    ws.Cells["A7"].LoadFromDataTable(dtImportador, false);
                    CalculaFormulasExcel(ws, dtImportador);
                    ws = package.Workbook.Worksheets["Exportadores - Embarcadores"];
                    ws.Name = Resources.Resources.Search_Form_Item06_I_2;
                    ws.Cells["B1"].Value = PaisTipoOpe;
                    ws.Cells["B2"].Value = Filtros;
                    ws.Cells["B3"].Value = Rango;
                    ws.Cells["A6"].Value = Resources.Resources.Search_Form_Item06_I;
                    ws.Cells["B6"].Value = Resources.Resources.Grid_Column_TotalRecords;
                    ws.Cells["C6"].Value = "Total " + Resources.Resources.PesoBruto_Text + " Tn";
                    ws.Cells["A7"].LoadFromDataTable(dtProveedor, false);
                    CalculaFormulasExcel(ws, dtProveedor);
                    ws = package.Workbook.Worksheets["Países Embarque"];
                    ws.Name = Resources.Resources.Ult_Paises_Embarque;
                    ws.Cells["B1"].Value = PaisTipoOpe;
                    ws.Cells["B2"].Value = Filtros;
                    ws.Cells["B3"].Value = Rango;
                    ws.Cells["A6"].Value = Resources.Resources.Pais_Embarque;
                    ws.Cells["B6"].Value = Resources.Resources.Grid_Column_TotalRecords;
                    ws.Cells["C6"].Value = "Total " + Resources.Resources.PesoBruto_Text + " Tn";
                    ws.Cells["A7"].LoadFromDataTable(dtPaisEmbarque, false);
                    CalculaFormulasExcel(ws, dtPaisEmbarque);
                }
                else
                {
                    ws = package.Workbook.Worksheets["Exportadores - Embarcadores"];
                    ws.Name = Resources.Resources.Search_Form_Item06_I_2;
                    ws.Cells["B1"].Value = PaisTipoOpe;
                    ws.Cells["B2"].Value = Filtros;
                    ws.Cells["B3"].Value = Rango;
                    ws.Cells["A6"].Value = Resources.Resources.Search_Form_Item06_I;
                    ws.Cells["B6"].Value = Resources.Resources.Grid_Column_TotalRecords;
                    ws.Cells["C6"].Value = "Total " + Resources.Resources.PesoBruto_Text + " Tn";
                    ws.Cells["A7"].LoadFromDataTable(dtImportador, false);
                    CalculaFormulasExcel(ws, dtImportador);
                    if (vFlag. ExisteImportadorExp)
                    {
                        ws = package.Workbook.Worksheets["Importadores - Consignatarios_"];
                        ws.Name = Resources.Resources.Demo_Importers_Tab_Manif;
                        ws.Cells["B1"].Value = PaisTipoOpe;
                        ws.Cells["B2"].Value = Filtros;
                        ws.Cells["B3"].Value = Rango;
                        ws.Cells["A6"].Value = Resources.Resources.Search_Form_Item05_I;
                        ws.Cells["B6"].Value = Resources.Resources.Grid_Column_TotalRecords;
                        ws.Cells["C6"].Value = "Total " + Resources.Resources.PesoBruto_Text + " Tn";
                        ws.Cells["A7"].LoadFromDataTable(dtProveedor, false);
                        CalculaFormulasExcel(ws, dtProveedor);
                    }
                    ws = package.Workbook.Worksheets["Países Embarque"];
                    ws.Name = Resources.Resources.Demo_DestinationCountries_Tab;
                    ws.Cells["B1"].Value = PaisTipoOpe;
                    ws.Cells["B2"].Value = Filtros;
                    ws.Cells["B3"].Value = Rango;
                    ws.Cells["A6"].Value = Resources.Resources.Demo_DestinationCountries_Tab;
                    ws.Cells["B6"].Value = Resources.Resources.Grid_Column_TotalRecords;
                    ws.Cells["C6"].Value = "Total " + Resources.Resources.PesoBruto_Text + " Tn";
                    ws.Cells["A7"].LoadFromDataTable(dtPaisEmbarque, false);
                    CalculaFormulasExcel(ws, dtPaisEmbarque);
                }
                if (vFlag.ExisteNotificado)
                {
                    ws = package.Workbook.Worksheets["Tramitadores - Notificados"];
                    ws.Name = Resources.Resources.Demo_Notif_Tab;
                    ws.Cells["B1"].Value = PaisTipoOpe;
                    ws.Cells["B2"].Value = Filtros;
                    ws.Cells["B3"].Value = Rango;
                    ws.Cells["A6"].Value = Resources.Resources.Demo_Notif_Tab_2;
                    ws.Cells["B6"].Value = Resources.Resources.Grid_Column_TotalRecords;
                    ws.Cells["C6"].Value = "Total " + Resources.Resources.PesoBruto_Text + " Tn";
                    ws.Cells["A7"].LoadFromDataTable(dtNotificado, false);
                    CalculaFormulasExcel(ws, dtNotificado);
                }
                if (vFlag.ExistePtoDescarga)
                {
                    ws = package.Workbook.Worksheets["Puertos Descarga"];
                    ws.Name = Resources.Resources.Demo_PtoDescarga_Tab;
                    ws.Cells["B1"].Value = PaisTipoOpe;
                    ws.Cells["B2"].Value = Filtros;
                    ws.Cells["B3"].Value = Rango;
                    ws.Cells["A6"].Value = Resources.Resources.Demo_PtoDescarga_Tab;
                    ws.Cells["B6"].Value = Resources.Resources.Grid_Column_TotalRecords;
                    ws.Cells["C6"].Value = "Total " + Resources.Resources.PesoBruto_Text + " Tn";
                    ws.Cells["A7"].LoadFromDataTable(dtPtoDescarga, false);
                    CalculaFormulasExcel(ws, dtPtoDescarga);
                }
                else if (vFlag.ExistePtoEmbarque)
                {
                    ws = package.Workbook.Worksheets["Puertos Descarga"];
                    ws.Name = Resources.Resources.Demo_PtoEmbarque_Tab;
                    ws.Cells["B1"].Value = PaisTipoOpe;
                    ws.Cells["B2"].Value = Filtros;
                    ws.Cells["B3"].Value = Rango;
                    ws.Cells["A6"].Value = Resources.Resources.Demo_PtoEmbarque;
                    ws.Cells["B6"].Value = Resources.Resources.Grid_Column_TotalRecords;
                    ws.Cells["C6"].Value = "Total " + Resources.Resources.PesoBruto_Text + " Tn";
                    ws.Cells["A7"].LoadFromDataTable(dtPtoDescarga, false);
                    CalculaFormulasExcel(ws, dtPtoDescarga);
                }
                else if (vFlag.ExistePtoDestino)
                {
                    ws = package.Workbook.Worksheets["Puertos Descarga"];
                    ws.Name = Resources.Resources.Demo_PtoDestino_Tab;
                    ws.Cells["B1"].Value = PaisTipoOpe;
                    ws.Cells["B2"].Value = Filtros;
                    ws.Cells["B3"].Value = Rango;
                    ws.Cells["A6"].Value = Resources.Resources.Demo_PtoDestino;
                    ws.Cells["B6"].Value = Resources.Resources.Grid_Column_TotalRecords;
                    ws.Cells["C6"].Value = "Total " + Resources.Resources.PesoBruto_Text + " Tn";
                    ws.Cells["A7"].LoadFromDataTable(dtPtoDescarga, false);
                    CalculaFormulasExcel(ws, dtPtoDescarga);
                }
                if (vFlag. ExisteManifiesto)
                {
                    ws = package.Workbook.Worksheets["Manifiestos"];
                    ws.Name = Resources.Resources.Demo_Manifiesto_Tab;
                    ws.Cells["B1"].Value = PaisTipoOpe;
                    ws.Cells["B2"].Value = Filtros;
                    ws.Cells["B3"].Value = Rango;
                    ws.Cells["A6"].Value = Resources.Resources.Demo_Manifiesto;
                    ws.Cells["B6"].Value = Resources.Resources.Grid_Column_TotalRecords;
                    ws.Cells["C6"].Value = "Total " + Resources.Resources.PesoBruto_Text + " Tn";
                    ws.Cells["A7"].LoadFromDataTable(dtManifiesto, false);
                    CalculaFormulasExcel(ws, dtManifiesto);
                }

                string idUsuario = Session["IdUsuario"].ToString();
                string CodUsuario = FuncionesBusiness.BuscaCodUsuario(idUsuario).ToUpper();

                FechaArchivo = DateTime.Now;
                NombreArchivo = "Veritrade_" + Resources.Resources.Demo_Summay_Tab + "_" + CodUsuario + "_M_" +
                                CodPais.Substring(0, 2) + "_" + TipoOpe + "_" +
                                FechaArchivo.ToString("yyyyMMddHHmmss") + ".xlsx";

                package.SaveAs(
                    new System.IO.FileInfo(ConfigurationManager.AppSettings["directorio_descarga"] + NombreArchivo));
            }

            return NombreArchivo;
        }


        [SessionExpireFilter]
        [HttpPost]
        public JsonResult GetExcelResumen(string opcion, string tipoOpe, string codPais,
            string textPais, string codPais2, string fechaIni,
            string fechaFin, string filtros, bool FlagDirecto = true)
        {
            GuardarLogInicioErrorSession("GetExcelResumen()");

            Object objMensajeError = null;

            if (string.IsNullOrEmpty(filtros) || Session["SqlFiltro"].ToString().Length <50)
            {
                objMensajeError = new
                {
                    titulo = "Error",
                    mensaje = "No se Seleccionaron filtro para descargar",
                    flagContactenos = false
                };

                return Json(new { objMensajeError });

            }

            

            ValidaCodPais2(codPais2, ref codPais);
            ValidaCodPaisManif(ref codPais, tipoOpe);

            var isManif = IsManifiesto(codPais);
            string NombreArchivo;
            DateTime FechaArchivo = DateTime.Now;
            string idioma = Session["Idioma"].ToString();

            if (isManif)
            {
                NombreArchivo = GetExcelResumenManif(opcion, tipoOpe, codPais, textPais, codPais2, fechaIni,
                    fechaFin, filtros, idioma, FechaArchivo);
                goto ReturnMe;
            }


            string cif = GetCIFTot(codPais, tipoOpe);//Funciones.Incoterm(codPais, tipoOpe);
            string cifTot = cif + "Tot";
            string pesoNeto = Funciones.CampoPeso(codPais, tipoOpe);
            
            

            var tabla = "";
            var dua = "";
            GetTablaAndDua(tipoOpe, codPais, ref tabla, ref dua);
            int numRegistros = 0;
            string unidad = FuncionesBusiness.Unidades(Session["SqlFiltro"].ToString(), tabla, ref numRegistros);
            if (numRegistros != 1 && pesoNeto != "")
            {
                unidad = "kg";
            }

            

            string PesoNeto1 = pesoNeto;
            if (PesoNeto1 == "")
                PesoNeto1 = "PesoNeto";

            DataTable dtPartida,
                dtMarca,
                dtModelo,
                dtImportador,
                dtProveedor,
                dtPaisOrigen,
                dtViaTransp,
                dtAduanaDUA,
                dtMarcaEC,
                dtDistrito,
                dtInfo;
            dtPartida = dtMarca = dtModelo = dtMarcaEC =
                dtImportador = dtProveedor = dtPaisOrigen = dtViaTransp = dtAduanaDUA = dtDistrito = dtInfo = null;

            var vFlag = new TabMisBusquedas(tipoOpe, codPais);

            var existeImportador = vFlag.ExisteImportador;  // Funciones.ExisteVariable(codPais, tipoOpe, "IdImportador");
            var existeProveedor = vFlag.ExisteProveedor;  //Funciones.ExisteVariable(codPais, tipoOpe, "IdProveedor");
            var existeExportador = vFlag.ExisteExportador;//Funciones.ExisteVariable(codPais, tipoOpe, "IdExportador");
            var existeImportadorExp = vFlag.ExisteImportadorExp;//Funciones.ExisteVariable(codPais, tipoOpe, "IdImportadorExp");
            var existePaisOrigen = vFlag.ExistePaisOrigen;//Funciones.ExisteVariable(codPais, tipoOpe, "IdPaisOrigen");
            var existePaisDestino = vFlag.ExistePaisDestino;// Funciones.ExisteVariable(codPais, tipoOpe, "IdPaisDestino");
            var existeMarcaEC = vFlag.ExisteMarcaEC;
            var EsFreeTrial = (Session["TipoUsuario"].ToString() == "Free Trial") || (bool)(Session["opcionFreeTrial"] ?? false);

            if (opcion == "Todo" || opcion == Enums.Filtro.Partida.ToString())
            {
                string sqlPartida= Session["sqlPartida"].ToString();
                if (EsFreeTrial)
                {
                    sqlPartida = sqlPartida.Substring(0, 7) + " TOP 20 " + sqlPartida.Substring(7);
                }
                dtPartida = FuncionesBusiness.GeneraDt(sqlPartida);
                dtPartida.Columns.Remove("IdPartida");
                dtPartida.Columns.Remove("Nandina");
                if (unidad == "kg")
                    dtPartida.Columns.Remove("Cantidad");
                else if (unidad != "")
                    dtPartida.Columns.Remove(PesoNeto1);
            }

            if(opcion != "InfoTabla")
            {
                if (tipoOpe == Enums.TipoOpe.I.ToString())
                {
                    if (codPais == "PEB" || codPais == "PE")
                    {
                        if (opcion == "Todo" || opcion == Enums.Filtro.Marca.ToString() || opcion == Enums.Filtro.Modelo.ToString())
                        {
                            // Ruben 2017-06-25
                            string sqlMarca = GeneraSqlAgrupado(Enums.Filtro.Marca.ToString(), codPais, cifTot, pesoNeto, idioma, dua, tabla);
                            if (EsFreeTrial)
                            {
                                sqlMarca = sqlMarca.Substring(0, 7) + " TOP 20 " + sqlMarca.Substring(7);

                            }
                            dtMarca = FuncionesBusiness.GeneraDt(sqlMarca);
                            dtMarca.Columns.Remove("IdMarca");
                            dtMarca.Columns.Remove("PesoNeto");
                            dtMarca.Columns.Remove("Cantidad");
                            dtMarca.Columns.Remove("Nro");
                            /*if (hdfUnidad.Value == "kg")
                                dtMarca.Columns.Remove("Cantidad");
                            else if (hdfUnidad.Value != "")
                                dtMarca.Columns.Remove(PesoNeto1);*/
                        }
                        if (opcion == "Todo" || opcion == Enums.Filtro.Modelo.ToString())
                        {
                            // Ruben 2017-06-25
                            string sqlModelo = GeneraSqlAgrupado(Enums.Filtro.Modelo.ToString(), codPais, cifTot, pesoNeto, idioma, dua, tabla);
                            if (EsFreeTrial)
                            {
                                sqlModelo = sqlModelo.Substring(0, 7) + " TOP 20 " + sqlModelo.Substring(7);

                            }
                            dtModelo = FuncionesBusiness.GeneraDt(sqlModelo);
                            dtModelo.Columns.Remove("IdModelo");
                            dtModelo.Columns.Remove("PesoNeto");
                            dtModelo.Columns.Remove("Cantidad");
                            dtModelo.Columns.Remove("Nro");
                            /*if (hdfUnidad.Value == "kg")
                                dtModelo.Columns.Remove("Cantidad");
                            else if (hdfUnidad.Value != "")
                                dtModelo.Columns.Remove(PesoNeto1);*/
                        }
                    }

                    if (existeImportador && (opcion == "Todo" || opcion == Enums.Filtro.Importador.ToString()))
                    {
                        string sqlImportador = Session["sqlImportador"].ToString();
                        if (EsFreeTrial)
                        {
                            sqlImportador = sqlImportador.Substring(0, 7) + " TOP 20 " + sqlImportador.Substring(7);
                        }
                        dtImportador = FuncionesBusiness.GeneraDt(sqlImportador);
                        dtImportador.Columns.Remove("IdImportador");
                        if (unidad == "kg")
                            dtImportador.Columns.Remove("Cantidad");
                        else if (unidad != "")
                            dtImportador.Columns.Remove(PesoNeto1);
                    }

                    if (existeProveedor && (opcion == "Todo" || opcion == Enums.Filtro.Proveedor.ToString()))
                    {
                        string sqlProveedor = Session["sqlProveedor"].ToString();
                        if (EsFreeTrial)
                        {
                            sqlProveedor = sqlProveedor.Substring(0, 7) + " TOP 20 " + sqlProveedor.Substring(7);
                        }
                        dtProveedor = FuncionesBusiness.GeneraDt(sqlProveedor);
                        dtProveedor.Columns.Remove("IdProveedor");
                        if (unidad == "kg")
                            dtProveedor.Columns.Remove("Cantidad");
                        else if (unidad != "")
                            dtProveedor.Columns.Remove(PesoNeto1);
                    }

                    if (existePaisOrigen && (opcion == "Todo" || opcion == Enums.Filtro.PaisOrigen.ToString()))
                    {
                        string sqlPaisOrigen = Session["sqlPaisOrigen"].ToString();
                        if (EsFreeTrial)
                        {
                            sqlPaisOrigen = sqlPaisOrigen.Substring(0, 7) + " TOP 20 " + sqlPaisOrigen.Substring(7);
                        }
                        dtPaisOrigen = FuncionesBusiness.GeneraDt(sqlPaisOrigen);
                        dtPaisOrigen.Columns.Remove("IdPaisOrigen");
                        if (unidad == "kg")
                            dtPaisOrigen.Columns.Remove("Cantidad");
                        else if (unidad != "")
                            dtPaisOrigen.Columns.Remove(PesoNeto1);
                    }
                }
                else
                {
                    if (existeExportador && (opcion == "Todo" || opcion == "Exportador"))
                    {
                        string sqlExportador = Session["sqlExportador"].ToString();
                        if (EsFreeTrial)
                        {
                            sqlExportador = sqlExportador.Substring(0, 7) + " TOP 20 " + sqlExportador.Substring(7);
                        }
                        dtImportador = FuncionesBusiness.GeneraDt(sqlExportador);
                        dtImportador.Columns.Remove("IdExportador");
                        if (unidad == "kg")
                            dtImportador.Columns.Remove("Cantidad");
                        else if (unidad != "")
                            dtImportador.Columns.Remove(PesoNeto1);
                    }
                    if (existeImportadorExp && Session["Plan"].ToString() != "ESENCIAL" && (opcion == "Todo" || opcion == "ImportadorExp"))
                    {
                        string sqlImportadorExp = Session["sqlImportadorExp"].ToString();
                        if (EsFreeTrial)
                        {
                            sqlImportadorExp = sqlImportadorExp.Substring(0, 7) + " TOP 20 " + sqlImportadorExp.Substring(7);
                        }
                        dtProveedor = FuncionesBusiness.GeneraDt(sqlImportadorExp);
                        dtProveedor.Columns.Remove("IdImportadorExp");
                        if (unidad == "kg")
                            dtProveedor.Columns.Remove("Cantidad");
                        else if (unidad != "")
                            dtProveedor.Columns.Remove(PesoNeto1);
                    }
                    if (existePaisDestino && (opcion == "Todo" || opcion == "PaisDestino"))
                    {
                        string sqlPaisDestino = Session["sqlPaisDestino"].ToString();
                        if (EsFreeTrial)
                        {
                            sqlPaisDestino = sqlPaisDestino.Substring(0, 7) + " TOP 20 " + sqlPaisDestino.Substring(7);
                        }
                        dtPaisOrigen = FuncionesBusiness.GeneraDt(sqlPaisDestino);
                        dtPaisOrigen.Columns.Remove("IdPaisDestino");
                        if (unidad == "kg")
                            dtPaisOrigen.Columns.Remove("Cantidad");
                        else if (unidad != "")
                            dtPaisOrigen.Columns.Remove(PesoNeto1);
                    }
                }
            }
            else
            {
                string sqlInfo = "SELECT * FROM (" + Session["sqlTabInfoComplementaria"].ToString() + ") as T";
                if (EsFreeTrial)
                {
                    sqlInfo = sqlInfo.Substring(0, 7) + " TOP 20 " + sqlInfo.Substring(7);
                }
                dtInfo = FuncionesBusiness.GeneraDt(sqlInfo);
                dtInfo.Columns.Remove("CodPais");
               
            }

            

            var existeViaTransp = vFlag.ExisteViaTransp;//GetExisteViaTransp(codPais);
            bool existeAduana = vFlag.ExisteAduana;//GetExisteAduana(codPais);
            bool existeDua = vFlag.ExisteDUA;//GetExisteDua(existeAduana, codPais);
            bool existeDistrito = vFlag.ExisteDistrito;//GetExisteDistrito(codPais);

            if (existeMarcaEC && (opcion == "Todo" || opcion == "MarcaEC"))
            {
                string sqlMarcaEC = Session["sqlMarcaEC"].ToString();
                if (EsFreeTrial)
                {
                    sqlMarcaEC = sqlMarcaEC.Substring(0, 7) + " TOP 20 " + sqlMarcaEC.Substring(7);
                }
                dtMarcaEC = FuncionesBusiness.GeneraDt(sqlMarcaEC);
                dtMarcaEC.Columns.Remove("IdMarca");
                if (unidad == "kg")
                    dtMarcaEC.Columns.Remove("Cantidad");
                else if (unidad != "")
                    dtMarcaEC.Columns.Remove(PesoNeto1);
            }

            if (existeViaTransp && (opcion == "Todo" || opcion == "ViaTransp"))
            {
                string sqlViaTransp = Session["sqlViaTransp"].ToString();
                if (EsFreeTrial)
                {
                    sqlViaTransp = sqlViaTransp.Substring(0, 7) + " TOP 20 " + sqlViaTransp.Substring(7);
                }
                dtViaTransp = FuncionesBusiness.GeneraDt(sqlViaTransp);
                dtViaTransp.Columns.Remove("IdViaTransp");
                if (unidad == "kg")
                    dtViaTransp.Columns.Remove("Cantidad");
                else if (unidad != "")
                    dtViaTransp.Columns.Remove(PesoNeto1);
            }

            if (existeAduana)
            {
                if (existeDua && (opcion == "Todo" || opcion == Enums.Filtro.AduanaDUA.ToString()))
                {
                    string sqlAduanaDUA = Session["sqlAduanaDUA"].ToString();
                    if (EsFreeTrial)
                    {
                        sqlAduanaDUA = sqlAduanaDUA.Substring(0, 7) + " TOP 20 " + sqlAduanaDUA.Substring(7);
                    }
                    dtAduanaDUA = FuncionesBusiness.GeneraDt(sqlAduanaDUA);
                    dtAduanaDUA.Columns.Remove("IdAduana");
                    dtAduanaDUA.Columns.Remove("IdAduanaDUA");
                    dtAduanaDUA.Columns.Remove("AduanaDUA");
                    dtAduanaDUA.Columns.Remove(PesoNeto1);
                    dtAduanaDUA.Columns.Remove("Cantidad");
                    dtAduanaDUA.Columns.Remove("Nro");
                }
                else if ((opcion == "Todo" || opcion == Enums.Filtro.Aduana.ToString()))
                {
                    string sqlAduana = Session["sqlAduana"].ToString();
                    if (EsFreeTrial)
                    {
                        sqlAduana = sqlAduana.Substring(0, 7) + " TOP 20 " + sqlAduana.Substring(7);
                    }
                    dtAduanaDUA = FuncionesBusiness.GeneraDt(sqlAduana);
                    dtAduanaDUA.Columns.Remove("IdAduana");
                    dtAduanaDUA.Columns.Remove(PesoNeto1);
                    dtAduanaDUA.Columns.Remove("Cantidad");
                }
            }
            if ( (opcion == "Todo" || opcion == Enums.Filtro.Distrito.ToString()) && existeDistrito)//GetExisteDistrito(codPais) )
            {
                string sqlDistrito = Session["sqlDistrito"].ToString();
                if (EsFreeTrial)
                {
                    sqlDistrito = sqlDistrito.Substring(0, 7) + " TOP 20 " + sqlDistrito.Substring(7);
                }
                dtDistrito = FuncionesBusiness.GeneraDt(sqlDistrito);
                dtDistrito.Columns.Remove("IdDistrito");
                if (unidad == "kg")
                    dtDistrito.Columns.Remove("Cantidad");
                else if (unidad != "")
                    dtDistrito.Columns.Remove(PesoNeto1);
            }

            string templateName = HostingEnvironment.MapPath("~/ExcelTemplate/VeritradeResumenTemplate.xlsx");
            using (OfficeOpenXml.ExcelPackage package = new OfficeOpenXml.ExcelPackage(new System.IO.FileInfo(templateName)))
            {
                if (opcion == "Todo")
                {
                    if (codPais != "PEB" && (codPais != "PE" || tipoOpe != "I"))
                        package.Workbook.Worksheets["Marcas"].Hidden = OfficeOpenXml.eWorkSheetHidden.VeryHidden;
                    if (codPais != "PEB" && (codPais != "PE" || tipoOpe != "I"))
                        package.Workbook.Worksheets["Modelos"].Hidden = OfficeOpenXml.eWorkSheetHidden.VeryHidden;
                    if ((tipoOpe == "I" && !existeImportador) || (tipoOpe == "E" && !existeExportador))
                        package.Workbook.Worksheets["Importadores1"].Hidden = OfficeOpenXml.eWorkSheetHidden.VeryHidden;
                    if ((tipoOpe == "I" && !existeProveedor) || (tipoOpe == "E" && (!existeImportadorExp || Session["Plan"].ToString() == "ESENCIAL")))
                        package.Workbook.Worksheets["Exportadores1"].Hidden = OfficeOpenXml.eWorkSheetHidden.VeryHidden;
                    if ((tipoOpe == "I" && !existePaisOrigen) || (tipoOpe == "E" && !existePaisDestino))
                        package.Workbook.Worksheets["Países Origen"].Hidden = OfficeOpenXml.eWorkSheetHidden.VeryHidden;
                    if (!existeViaTransp)
                        package.Workbook.Worksheets["Vías Transporte"].Hidden =
                            OfficeOpenXml.eWorkSheetHidden.VeryHidden;
                    if (!existeAduana)
                        package.Workbook.Worksheets["Aduanas - DUAs"].Hidden =
                            OfficeOpenXml.eWorkSheetHidden.VeryHidden;
                    if (!existeDistrito)
                        package.Workbook.Worksheets["Distritos"].Hidden = OfficeOpenXml.eWorkSheetHidden.VeryHidden;
                    if (!existeMarcaEC)
                        package.Workbook.Worksheets["Marcas_"].Hidden = OfficeOpenXml.eWorkSheetHidden.VeryHidden;

                }
                else
                {
                    if (opcion != "Partida")
                        package.Workbook.Worksheets["Productos"].Hidden = OfficeOpenXml.eWorkSheetHidden.VeryHidden;
                    if ((codPais != "PEB" && (codPais != "PE" || tipoOpe != "I")) ||
                        (opcion != "Marca" && opcion != "Modelo"))
                        package.Workbook.Worksheets["Marcas"].Hidden = OfficeOpenXml.eWorkSheetHidden.VeryHidden;
                    if ((codPais != "PEB" && (codPais != "PE" || tipoOpe != "I")) || opcion != "Modelo")
                        package.Workbook.Worksheets["Modelos"].Hidden = OfficeOpenXml.eWorkSheetHidden.VeryHidden;

                    if ((tipoOpe == "I" && !existeImportador) ||
                        (tipoOpe == "E" && !existeExportador) ||
                        (opcion != "Importador" && opcion != "Exportador"))
                    {
                        package.Workbook.Worksheets["Importadores1"].Hidden = OfficeOpenXml.eWorkSheetHidden.VeryHidden;
                    }

                    if ((tipoOpe == "I" && !existeProveedor) ||
                        (tipoOpe == "E" && !existeImportadorExp) ||
                        (opcion != "Proveedor" && opcion != "ImportadorExp"))
                    {
                        package.Workbook.Worksheets["Exportadores1"].Hidden = OfficeOpenXml.eWorkSheetHidden.VeryHidden;
                    }

                    if ((tipoOpe == "I" && !existePaisOrigen) || (tipoOpe == "E" && !existePaisDestino) || (opcion != "PaisOrigen" && opcion != "PaisDestino"))
                        package.Workbook.Worksheets["Países Origen"].Hidden = OfficeOpenXml.eWorkSheetHidden.VeryHidden;
                    if (!existeViaTransp || opcion != "ViaTransp")
                        package.Workbook.Worksheets["Vías Transporte"].Hidden =
                            OfficeOpenXml.eWorkSheetHidden.VeryHidden;
                    if (!existeAduana || (opcion != Enums.Filtro.AduanaDUA.ToString() && opcion != Enums.Filtro.Aduana.ToString()))
                        package.Workbook.Worksheets["Aduanas - DUAs"].Hidden =
                            OfficeOpenXml.eWorkSheetHidden.VeryHidden;
                    if (!existeDistrito || opcion != "Distrito")
                        package.Workbook.Worksheets["Distritos"].Hidden = OfficeOpenXml.eWorkSheetHidden.VeryHidden;
                    if (!existeMarcaEC || opcion != "MarcaEC")
                        package.Workbook.Worksheets["Marcas_"].Hidden = OfficeOpenXml.eWorkSheetHidden.VeryHidden;                    
                }
                if (opcion != "InfoTabla")
                    package.Workbook.Worksheets["InfoComplementaria"].Hidden = OfficeOpenXml.eWorkSheetHidden.VeryHidden;
                string PaisTipoOpe;
                if (codPais2 != "4UE")
                    PaisTipoOpe = FuncionesBusiness.Pais(codPais, idioma).ToUpper() + " - " +
                                  (tipoOpe == "I" ? Resources.AdminResources.Imports_Text.ToUpper() : Resources.AdminResources.Exports_Text.ToUpper());
                else
                    PaisTipoOpe = textPais.ToUpper() + " - " +
                                  (tipoOpe == "I" ? Resources.AdminResources.Imports_Text.ToUpper() : Resources.AdminResources.Exports_Text.ToUpper());

                //string Filtros = FuncionesBusiness.FiltrosEnExcel(lstFiltros);
                string rango = Resources.AdminResources.Period_Text + ": " + GetRangoFechas(fechaIni, fechaFin, idioma);

                OfficeOpenXml.ExcelWorksheet ws;

                if (opcion == "Todo" || opcion == "Partida")
                {
                    ws = package.Workbook.Worksheets["Productos"];
                    ws.Name = Resources.Resources.Demo_Products_Tab;
                    if (EsFreeTrial)
                    {
                        ws.Cells["B5"].Value = Resources.Resources.Trial_Excel_Text;
                        ws.Cells["B5"].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                    }
                    ws.Cells["B1"].Value = PaisTipoOpe;
                    ws.Cells["B2"].Value = filtros;
                    ws.Cells["B3"].Value = rango;
                    ws.Cells["A6"].Value = Resources.AdminResources.HTS_CodeDescription_Text;
                    ws.Cells["B6"].Value = Resources.AdminResources.Total_records_Text;
                    ws.Cells["E6"].Value = "Total " + unidad;
                    ws.Cells["F6"].Value = "US$ / " + unidad;
                    ws.Cells["A7"].LoadFromDataTable(dtPartida, false);
                    CalculaFormulasExcel(ws, dtPartida, cif);
                    if (unidad == "")
                    {
                        ws.DeleteColumn(5);
                        ws.DeleteColumn(5);
                    }
                }

                if (codPais == "PEB" || (codPais == "PE" && tipoOpe == "I"))
                {
                    if (opcion == "Todo" || opcion == "Marca" || opcion == "Modelo")
                    {
                        ws = package.Workbook.Worksheets["Marcas"];
                        ws.Name = Resources.Resources.Demo_Brands_Tab;
                        if (EsFreeTrial)
                        {
                            ws.Cells["B5"].Value = Resources.Resources.Trial_Excel_Text;
                            ws.Cells["B5"].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                        }
                        ws.Cells["B1"].Value = PaisTipoOpe;
                        ws.Cells["B2"].Value = filtros;
                        ws.Cells["B3"].Value = rango;
                        ws.Cells["A6"].Value = Resources.Resources.Search_Form_BrandField;
                        ws.Cells["B6"].Value = Resources.AdminResources.Total_records_Text;
                        //ws.Cells["E6"].Value = "Total " + hdfUnidad.Value;
                        //ws.Cells["F6"].Value = "US$ / " + hdfUnidad.Value;
                        ws.Cells["A7"].LoadFromDataTable(dtMarca, false);
                        CalculaFormulasExcelMarca(ws, dtMarca, cif);
                        /*if (hdfUnidad.Value == "")
                        {
                            ws.DeleteColumn(5);
                            ws.DeleteColumn(5);
                        }*/
                        ws.DeleteColumn(5);
                        ws.DeleteColumn(5);
                        //ws.Cells["E6"].AutoFilter = false;
                        
                    }

                    if (opcion == "Todo" || opcion == "Modelo")
                    {
                        ws = package.Workbook.Worksheets["Modelos"];
                        ws.Name = Resources.Resources.Demo_Models_Tab;
                        if (EsFreeTrial)
                        {
                            ws.Cells["B5"].Value = Resources.Resources.Trial_Excel_Text;
                            ws.Cells["B5"].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                        }
                        ws.Cells["B1"].Value = PaisTipoOpe;
                        ws.Cells["B2"].Value = filtros;
                        ws.Cells["B3"].Value = rango;
                        ws.Cells["A6"].Value = Resources.AdminResources.Model_FormField_Label;
                        ws.Cells["B6"].Value = Resources.AdminResources.Total_records_Text;
                        //ws.Cells["E6"].Value = "Total " + hdfUnidad.Value;
                        //ws.Cells["F6"].Value = "US$ / " + hdfUnidad.Value;
                        ws.Cells["A7"].LoadFromDataTable(dtModelo, false);
                        CalculaFormulasExcelMarca(ws, dtModelo, cif);
                        /*if (hdfUnidad.Value == "")
                        {
                            ws.DeleteColumn(5);
                            ws.DeleteColumn(5);
                        }*/
                        ws.DeleteColumn(5);
                        ws.DeleteColumn(5);
                    }
                }
                if ((existeImportador || existeExportador) && (opcion == "Todo" || opcion == "Importador" || opcion == "Exportador")
                ) //(ExisteImportador || ExisteExportador)
                {
                    ws = package.Workbook.Worksheets["Importadores1"];
                    ws.Name = (tipoOpe == "I" ? Resources.Resources.Demo_Importers_Tab : Resources.Resources.Demo_Exporters_Tab);
                    if (EsFreeTrial)
                    {
                        ws.Cells["B5"].Value = Resources.Resources.Trial_Excel_Text;
                        ws.Cells["B5"].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                    }
                    ws.Cells["B1"].Value = PaisTipoOpe;
                    ws.Cells["B2"].Value = filtros;
                    ws.Cells["B3"].Value = rango;
                    ws.Cells["A6"].Value = (tipoOpe == "I" ? Resources.Resources.Search_Form_Item05 : Resources.Resources.Search_Form_Item06);
                    ws.Cells["B6"].Value = Resources.AdminResources.Total_records_Text;
                    ws.Cells["E6"].Value = "Total " + unidad;
                    ws.Cells["F6"].Value = "US$ / " + unidad;
                    ws.Cells["A7"].LoadFromDataTable(dtImportador, false);
                    CalculaFormulasExcel(ws, dtImportador, cif);
                    if (unidad == "")
                    {
                        ws.DeleteColumn(5);
                        ws.DeleteColumn(5);
                    }
                }
                if (((tipoOpe == "I" && existeProveedor) || (tipoOpe == "E" && existeImportadorExp && Session["Plan"].ToString() != "ESENCIAL")) && (opcion == "Todo" || opcion == "Proveedor" || opcion == "ImportadorExp")
                ) //(ExisteProveedor || ExisteImportadorExp)
                {
                    if (codPais == "CL")
                    {
                        ws = package.Workbook.Worksheets["Marcas_"];
                        ws.Name = "marcx";
                    }                        

                    ws = package.Workbook.Worksheets["Exportadores1"];
                    
                    ws.Name = (tipoOpe == "I" ? (codPais != "CL"  ? Resources.Resources.Demo_Exporters_Tab : Resources.Resources.Demo_Brands_Tab + "_") : Resources.Resources.Demo_Importers_Tab);
                    if (EsFreeTrial)
                    {
                        ws.Cells["B5"].Value = Resources.Resources.Trial_Excel_Text;
                        ws.Cells["B5"].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                    }
                    ws.Cells["B1"].Value = PaisTipoOpe;
                    ws.Cells["B2"].Value = filtros;
                    ws.Cells["B3"].Value = rango;
                    ws.Cells["A6"].Value = (tipoOpe == "I" ? (codPais != "CL"  ? Resources.Resources.Search_Form_Item06 : Resources.Resources.Search_Form_BrandField) : Resources.Resources.Search_Form_Item05);
                    ws.Cells["B6"].Value = Resources.AdminResources.Total_records_Text;
                    ws.Cells["E6"].Value = "Total " + unidad;
                    ws.Cells["F6"].Value = "US$ / " + unidad;
                    ws.Cells["A7"].LoadFromDataTable(dtProveedor, false);
                    CalculaFormulasExcel(ws, dtProveedor, cif);
                    if (unidad == "")
                    {
                        ws.DeleteColumn(5);
                        ws.DeleteColumn(5);
                    }
                }
                if ((existePaisOrigen || existePaisDestino) && (opcion == "Todo" || opcion == "PaisOrigen" || opcion == "PaisDestino")
                ) //(ExistePaisOrigen || ExistePaisDestino)
                {
                    ws = package.Workbook.Worksheets["Países Origen"];
                    ws.Name = (tipoOpe == "I" ? Resources.Resources.Demo_OriginCountries_Tab : Resources.Resources.Demo_DestinationCountries_Tab);
                    if (EsFreeTrial)
                    {
                        ws.Cells["B5"].Value = Resources.Resources.Trial_Excel_Text;
                        ws.Cells["B5"].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                    }
                    ws.Cells["B1"].Value = PaisTipoOpe;
                    ws.Cells["B2"].Value = filtros;
                    ws.Cells["B3"].Value = rango;
                    ws.Cells["A6"].Value = (tipoOpe == "I" ? Resources.AdminResources.OriginCountry_FormField_Label : Resources.AdminResources.DestinationCountry_FormField_Label);
                    ws.Cells["B6"].Value = Resources.AdminResources.Total_records_Text;
                    ws.Cells["E6"].Value = "Total " + unidad;
                    ws.Cells["F6"].Value = "US$ / " + unidad;
                    ws.Cells["A7"].LoadFromDataTable(dtPaisOrigen, false);
                    CalculaFormulasExcel(ws, dtPaisOrigen, cif);
                    if (unidad == "")
                    {
                        ws.DeleteColumn(5);
                        ws.DeleteColumn(5);
                    }
                }
                if (existeMarcaEC && (opcion == "Todo" || opcion == "MarcaEC")) //(ExisteViaTransp)
                {
                    ws = package.Workbook.Worksheets["Marcas_"];
                    ws.Name = Resources.Resources.Demo_Brands_Tab+"_";
                    if (EsFreeTrial)
                    {
                        ws.Cells["B5"].Value = Resources.Resources.Trial_Excel_Text;
                        ws.Cells["B5"].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                    }
                    ws.Cells["B1"].Value = PaisTipoOpe;
                    ws.Cells["B2"].Value = filtros;
                    ws.Cells["B3"].Value = rango;
                    ws.Cells["A6"].Value = Resources.Resources.Search_Form_BrandField;
                    ws.Cells["B6"].Value = Resources.AdminResources.Total_records_Text;
                    ws.Cells["E6"].Value = "Total " + unidad;
                    ws.Cells["F6"].Value = "US$ / " + unidad;
                    ws.Cells["A7"].LoadFromDataTable(dtMarcaEC, false);
                    CalculaFormulasExcel(ws, dtMarcaEC, cif);
                    if (unidad == "")
                    {
                        ws.DeleteColumn(5);
                        ws.DeleteColumn(5);
                    }
                }
                if (existeViaTransp && (opcion == "Todo" || opcion == "ViaTransp")) //(ExisteViaTransp)
                {
                    ws = package.Workbook.Worksheets["Vías Transporte"];
                    ws.Name = Resources.Resources.Demo_Vias_Tab;
                    if (EsFreeTrial)
                    {
                        ws.Cells["B5"].Value = Resources.Resources.Trial_Excel_Text;
                        ws.Cells["B5"].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                    }
                    ws.Cells["B1"].Value = PaisTipoOpe;
                    ws.Cells["B2"].Value = filtros;
                    ws.Cells["B3"].Value = rango;
                    ws.Cells["A6"].Value = Resources.AdminResources.FilterText_ViaTransp;
                    ws.Cells["B6"].Value = Resources.AdminResources.Total_records_Text;
                    ws.Cells["E6"].Value = "Total " + unidad;
                    ws.Cells["F6"].Value = "US$ / " + unidad;
                    ws.Cells["A7"].LoadFromDataTable(dtViaTransp, false);
                    CalculaFormulasExcel(ws, dtViaTransp, cif);
                    if (unidad == "")
                    {
                        ws.DeleteColumn(5);
                        ws.DeleteColumn(5);
                    }
                }

                if (existeAduana &&
                    (opcion == "Todo" || opcion == Enums.Filtro.AduanaDUA.ToString() || opcion == Enums.Filtro.Aduana.ToString())) //(ExisteAduana)
                {
                    ws = package.Workbook.Worksheets["Aduanas - DUAs"];
                    ws.Name = Resources.Resources.Demo_Aduanas_Tab + " - DUAs";
                    if (!existeDua)
                    {
                        ws.Name = Resources.Resources.Demo_Aduanas_Tab;
                        ws.DeleteColumn(2);
                    }
                    if (EsFreeTrial)
                    {
                        ws.Cells["B5"].Value = Resources.Resources.Trial_Excel_Text;
                        ws.Cells["B5"].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                    }
                    ws.Cells["B1"].Value = PaisTipoOpe;
                    ws.Cells["B2"].Value = filtros;
                    ws.Cells["B3"].Value = rango;
                    ws.Cells["A6"].Value = Resources.AdminResources.FilterText_Aduana;
                    ws.Cells["C6"].Value = Resources.AdminResources.Total_records_Text;
                    ws.Cells["A7"].LoadFromDataTable(dtAduanaDUA, false);

                    ws.Cells["D6"].Value = "% ";

                    if(!existeDua && codPais != "MX" && codPais != "VE")
                    {
                        ws.Cells["E6"].Value = "Total " + unidad;
                        ws.Cells["F6"].Value = "US$ / " + unidad;
                    }

                    if (existeDua)
                        CalculaFormulasExcel3(ws, dtAduanaDUA, cif);
                    else
                        CalculaFormulasExcel(ws, dtAduanaDUA, cif, codPais == "MX" || codPais == "VE");
                }
                if (existeDistrito && (opcion == "Todo" || opcion == Enums.Filtro.Distrito.ToString())) //(ExisteDistrito)
                {
                    ws = package.Workbook.Worksheets["Distritos"];
                    ws.Name = Resources.Resources.Demo_Districts_Tab;
                    ws.Cells["B1"].Value = PaisTipoOpe;
                    ws.Cells["B2"].Value = filtros;
                    ws.Cells["B3"].Value = rango;
                    if (EsFreeTrial)
                    {
                        ws.Cells["B5"].Value = Resources.Resources.Trial_Excel_Text;
                        ws.Cells["B5"].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                    }
                    ws.Cells["A6"].Value = Resources.AdminResources.FilterText_District;
                    ws.Cells["B6"].Value = Resources.AdminResources.Total_records_Text;
                    ws.Cells["E6"].Value = "Total " + unidad;
                    ws.Cells["F6"].Value = "US$ / " + unidad;
                    ws.Cells["A7"].LoadFromDataTable(dtDistrito, false);
                    CalculaFormulasExcel(ws, dtDistrito, cif);
                    if (unidad == "")
                    {
                        ws.DeleteColumn(5);
                        ws.DeleteColumn(5);
                    }
                }

                if (opcion == "InfoTabla")
                {
                    ws = package.Workbook.Worksheets["InfoComplementaria"];
                    ws.Name = "InfoComplementaria";
                    if (EsFreeTrial)
                    {
                        ws.Cells["B5"].Value = Resources.Resources.Trial_Excel_Text;
                        ws.Cells["B5"].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                    }
                    ws.Cells["B1"].Value = PaisTipoOpe;
                    ws.Cells["B2"].Value = filtros;
                    ws.Cells["B3"].Value = rango;
                    ws.Cells["A6"].Value = "Paises";
                    ws.Cells["B6"].Value = Resources.AdminResources.Total_records_Text;
                    /*ws.Cells["E6"].Value = "Total US$ CIF";
                    ws.Cells["F6"].Value = "Total kg";*/
                    ws.Cells["A7"].LoadFromDataTable(dtInfo, false);
                    CalculaFormulasExcelInfoComplementaria(ws, dtInfo, cif);
                }
                
                NombreArchivo = "Veritrade_" + Resources.Resources.Demo_Summay_Tab + "_" + FuncionesBusiness.BuscaCodUsuario(Session["IdUsuario"].ToString()) + "_" + codPais +
                                "_" + tipoOpe + "_" + FechaArchivo.ToString("yyyyMMddHHmmss") + ".xlsx";
                package.SaveAs(
                    new System.IO.FileInfo(ConfigurationManager.AppSettings["directorio_descarga"] + NombreArchivo));
            }
ReturnMe:
            if (FlagDirecto)
            {
                return Json(new { isFileDownload = true, fileName = NombreArchivo });
            }
            else
            {

                Object objMensaje = new
                {
                    titulo = Resources.AdminResources.DownloadToExcel_Text,
                    mensaje = Resources.Resources.DownloadToExcel_Text,
                    flagContactenos = false,
                    objMensajeError
                };

                return Json(new
                {
                    isFileDownload = false,
                    infoArchivoDescarga = new
                    {
                        infoFile = NombreArchivo + " [generado en " + FechaArchivo.ToString("HH:mm:ss tt") + "]",
                        navigateUrl = Funciones.ruta_descarga + NombreArchivo,
                        visible = true
                    },
                    objMensaje,
                    fileName = NombreArchivo,
                    objMensajeError
                });
            }
        }

        [HttpGet]
        public ActionResult DownloadExcelResumen(string file)
        {
            string fullPath = ConfigurationManager.AppSettings["directorio_descarga"] + file;
            return File(fullPath, "application/vnd.ms-excel", file);
        }

        private void CalculaFormulasExcel(OfficeOpenXml.ExcelWorksheet ws, DataTable dt, string cif , bool valida = false)
        {
            int i = 7;
            int t = i + dt.Rows.Count;

            ws.Cells["C6"].Value = "Total US$ " + cif;

            ws.Cells["A" + t.ToString()].Value = "Total";
            ws.Cells["B" + t.ToString()].Formula = "=SUM(B7:B" + (t - 1).ToString() + ")";
            ws.Cells["C" + t.ToString()].Formula = "=SUM(C7:C" + (t - 1).ToString() + ")";
            ws.Cells["D" + t.ToString()].Formula = "=SUM(D7:D" + (t - 1).ToString() + ")";
            if (!valida)
            {
                ws.Cells["E" + t.ToString()].Formula = "=SUM(E7:E" + (t - 1).ToString() + ")";
                ws.Cells["F" + t.ToString()].FormulaR1C1 = "=R[0]C[-3] / R[0]C[-1]";
            }
            
            foreach (DataRow dr in dt.Rows)
            {
                if (!valida)
                {
                    if (Convert.ToDecimal(ws.Cells["D" + i.ToString()].Value) > 0)
                        ws.Cells["F" + i.ToString()].Value = Convert.ToDecimal(ws.Cells["C" + i.ToString()].Value) /
                                                             Convert.ToDecimal(ws.Cells["D" + i.ToString()].Value);

                    ws.Cells["E" + i.ToString()].Value = ws.Cells["D" + i.ToString()].Value;
                }
                

                ws.Cells["D" + i.ToString()].FormulaR1C1 = "=R[0]C[-1] / R" + t.ToString() + "C[-1]";
                //ws.Cells["F" + i.ToString()].FormulaR1C1 = "=R[0]C[-3] / R[0]C[-1]";
                i += 1;
            }
            if (!valida)
            {
                ws.Cells["A" + t.ToString() + ":F" + t.ToString()].Style.Font.Color
                .SetColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));
                ws.Cells["A" + t.ToString() + ":F" + t.ToString()].Style.Font.Bold = true;
                ws.Cells["A" + t.ToString() + ":F" + t.ToString()].Style.Fill.PatternType =
                    OfficeOpenXml.Style.ExcelFillStyle.Solid;
                ws.Cells["A" + t.ToString() + ":F" + t.ToString()].Style.Fill.BackgroundColor
                    .SetColor(System.Drawing.Color.FromArgb(48, 167, 229));
            }
            else
            {
                ws.Cells["A" + t.ToString() + ":D" + t.ToString()].Style.Font.Color
                .SetColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));
                ws.Cells["A" + t.ToString() + ":D" + t.ToString()].Style.Font.Bold = true;
                ws.Cells["A" + t.ToString() + ":D" + t.ToString()].Style.Fill.PatternType =
                    OfficeOpenXml.Style.ExcelFillStyle.Solid;
                ws.Cells["A" + t.ToString() + ":D" + t.ToString()].Style.Fill.BackgroundColor
                    .SetColor(System.Drawing.Color.FromArgb(48, 167, 229));
            }
            
        }

        private void CalculaFormulasExcelInfoComplementaria(OfficeOpenXml.ExcelWorksheet ws, DataTable dt, string cif, bool valida = false)
        {
            int i = 7;
            int t = i + dt.Rows.Count;

            //ws.Cells["C6"].Value = "Total US$ " + cif;

            ws.Cells["A" + t.ToString()].Value = "Total";
            ws.Cells["B" + t.ToString()].Formula = "=SUM(B7:B" + (t - 1).ToString() + ")";
            ws.Cells["C" + t.ToString()].Formula = "=SUM(C7:C" + (t - 1).ToString() + ")";
            ws.Cells["D" + t.ToString()].Formula = "=SUM(D7:D" + (t - 1).ToString() + ")";

            ws.Cells["A" + t.ToString() + ":D" + t.ToString()].Style.Font.Color
                .SetColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));
            ws.Cells["A" + t.ToString() + ":D" + t.ToString()].Style.Font.Bold = true;
            ws.Cells["A" + t.ToString() + ":D" + t.ToString()].Style.Fill.PatternType =
                OfficeOpenXml.Style.ExcelFillStyle.Solid;
            ws.Cells["A" + t.ToString() + ":D" + t.ToString()].Style.Fill.BackgroundColor
                .SetColor(System.Drawing.Color.FromArgb(48, 167, 229));
            /*if (!valida)
            {
                ws.Cells["E" + t.ToString()].Formula = "=SUM(E7:E" + (t - 1).ToString() + ")";
                ws.Cells["F" + t.ToString()].FormulaR1C1 = "=R[0]C[-3] / R[0]C[-1]";
            }

            foreach (DataRow dr in dt.Rows)
            {
                if (!valida)
                {
                    if (Convert.ToDecimal(ws.Cells["D" + i.ToString()].Value) > 0)
                        ws.Cells["F" + i.ToString()].Value = Convert.ToDecimal(ws.Cells["C" + i.ToString()].Value) /
                                                             Convert.ToDecimal(ws.Cells["D" + i.ToString()].Value);

                    ws.Cells["E" + i.ToString()].Value = ws.Cells["D" + i.ToString()].Value;
                }


                ws.Cells["D" + i.ToString()].FormulaR1C1 = "=R[0]C[-1] / R" + t.ToString() + "C[-1]";
                //ws.Cells["F" + i.ToString()].FormulaR1C1 = "=R[0]C[-3] / R[0]C[-1]";
                i += 1;
            }
            if (!valida)
            {
                ws.Cells["A" + t.ToString() + ":F" + t.ToString()].Style.Font.Color
                .SetColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));
                ws.Cells["A" + t.ToString() + ":F" + t.ToString()].Style.Font.Bold = true;
                ws.Cells["A" + t.ToString() + ":F" + t.ToString()].Style.Fill.PatternType =
                    OfficeOpenXml.Style.ExcelFillStyle.Solid;
                ws.Cells["A" + t.ToString() + ":F" + t.ToString()].Style.Fill.BackgroundColor
                    .SetColor(System.Drawing.Color.FromArgb(48, 167, 229));
            }
            else
            {
                ws.Cells["A" + t.ToString() + ":D" + t.ToString()].Style.Font.Color
                .SetColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));
                ws.Cells["A" + t.ToString() + ":D" + t.ToString()].Style.Font.Bold = true;
                ws.Cells["A" + t.ToString() + ":D" + t.ToString()].Style.Fill.PatternType =
                    OfficeOpenXml.Style.ExcelFillStyle.Solid;
                ws.Cells["A" + t.ToString() + ":D" + t.ToString()].Style.Fill.BackgroundColor
                    .SetColor(System.Drawing.Color.FromArgb(48, 167, 229));
            }*/

        }

        private void CalculaFormulasExcelMarca(OfficeOpenXml.ExcelWorksheet ws, DataTable dt, string cif)
        {
            int i = 7;
            int t = i + dt.Rows.Count;

            ws.Cells["C6"].Value = "Total US$ " + cif;

            ws.Cells["A" + t.ToString()].Value = "Total";
            ws.Cells["B" + t.ToString()].Formula = "=SUM(B7:B" + (t - 1).ToString() + ")";
            ws.Cells["C" + t.ToString()].Formula = "=SUM(C7:C" + (t - 1).ToString() + ")";
            ws.Cells["D" + t.ToString()].Formula = "=SUM(D7:D" + (t - 1).ToString() + ")";
            //ws.Cells["E" + t.ToString()].Formula = "=SUM(E7:E" + (t - 1).ToString() + ")";
            //ws.Cells["F" + t.ToString()].FormulaR1C1 = "=R[0]C[-3] / R[0]C[-1]";
            foreach (DataRow dr in dt.Rows)
            {
                //if (Convert.ToDecimal(ws.Cells["D" + i.ToString()].Value) > 0)
                //    ws.Cells["F" + i.ToString()].Value = Convert.ToDecimal(ws.Cells["C" + i.ToString()].Value) / Convert.ToDecimal(ws.Cells["D" + i.ToString()].Value);

                //ws.Cells["E" + i.ToString()].Value = ws.Cells["D" + i.ToString()].Value;
                ws.Cells["D" + i.ToString()].FormulaR1C1 = "=R[0]C[-1] / R" + t.ToString() + "C[-1]";
                //ws.Cells["F" + i.ToString()].FormulaR1C1 = "=R[0]C[-3] / R[0]C[-1]";
                i += 1;
            }
            ws.Cells["A" + t.ToString() + ":D" + t.ToString()].Style.Font.Color
                .SetColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));
            ws.Cells["A" + t.ToString() + ":D" + t.ToString()].Style.Font.Bold = true;
            ws.Cells["A" + t.ToString() + ":D" + t.ToString()].Style.Fill.PatternType =
                OfficeOpenXml.Style.ExcelFillStyle.Solid;
            ws.Cells["A" + t.ToString() + ":D" + t.ToString()].Style.Fill.BackgroundColor
                .SetColor(System.Drawing.Color.FromArgb(48, 167, 229));
        }

        private void CalculaFormulasExcel3(OfficeOpenXml.ExcelWorksheet ws, DataTable dt, string cif)
        {
            int i = 7;
            int t = i + dt.Rows.Count;

            ws.Cells["D6"].Value = "Total US$ " + cif;

            ws.Cells["A" + t.ToString()].Value = "Total";
            ws.Cells["C" + t.ToString()].Formula = "=SUM(C7:C" + (t - 1).ToString() + ")";
            ws.Cells["D" + t.ToString()].Formula = "=SUM(D7:D" + (t - 1).ToString() + ")";
            ws.Cells["A" + t.ToString() + ":D" + t.ToString()].Style.Font.Color
                .SetColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));
            ws.Cells["A" + t.ToString() + ":D" + t.ToString()].Style.Font.Bold = true;
            ws.Cells["A" + t.ToString() + ":D" + t.ToString()].Style.Fill.PatternType =
                OfficeOpenXml.Style.ExcelFillStyle.Solid;
            ws.Cells["A" + t.ToString() + ":D" + t.ToString()].Style.Fill.BackgroundColor
                .SetColor(System.Drawing.Color.FromArgb(48, 167, 229));
        }

        private bool ValidarConsulta(string sql)
        {
            /*bool correct = true;

            if (sql.Contains("'%'") && sql.Contains("or"))
            {
                sql = sql.Replace("or", "and");
            }else if (sql.Contains("'%'"))
            {
                correct = false;
            }
            return correct;*/

            return sql.Contains("'%'");

        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult GetExcelVerRegistro(string tipoOpe, string codPais, string codPais2,
            string anioMesIni, string anioMesFin, int indexCboPaisB,
            string codPaisB, string codCboDescargas, string filtros, bool FlagDirecto = true, bool FlagDescarga = false, string txtDua = "" , string txtDesCom = "")
        {
            GuardarLogInicioErrorSession("GetExcelVerRegistro()");
            var filtro = (Session["filtro"] ?? "").ToString();
            var filx = "";

            if (filtro == "InfoComplementario" || filtro == "InfoTabla")
            {
                filx = filtro;
            }
            if (filtro == "InfoComplementario" || filtro == "InfoTabla")
            {
                codPais = (Session["CodPaisComplementario"] ?? codPais).ToString();
                codPais2 = new ListaPaises().BuscarCodPais2(codPais);
                tipoOpe = tipoOpe == "I" ? "E" : "I";
            }

            string auxCodPais = codPais;
            if (filtro != "InfoComplementario" && filtro != "InfoTabla")
            {
                ValidaCodPais2(codPais2, ref codPais);

            }
            ValidaCodPaisManif(ref codPais, tipoOpe);
            var isManif = IsManifiesto(codPais);
            int limiteDescargas = 0, descargas = 0;
            object objMensaje = null;
            string idioma = Session["Idioma"].ToString();
            if (!FuncionesBusiness.ValidaDescargasMes(Session["IdUsuario"].ToString(), codPais, tipoOpe, ref limiteDescargas, ref descargas))
            {
                string mensaje = "Esta descarga no se puede efectuar.<br>Se ha llegado al máximo contratado de " +
                                limiteDescargas.ToString("n0") + " descargas mensuales.";
                if (idioma == "en")
                {
                    mensaje = "Could not download to Excel.<br>It reached to Monthly downloads number limit: " +
                              limiteDescargas.ToString("n0");
                }

                return Json(new
                {
                    objMensaje = new
                    {
                        titulo = Resources.AdminResources.DownloadToExcel_Text,
                        mensaje,
                        flagContactenos = true
                    }
                });
            }
            else
            {
                var tabla = "";
                var dua = "";
                GetTablaAndDua(tipoOpe, codPais, ref tabla, ref dua);

                string sqlFiltro = "";
                if (FlagDescarga) {
                    sqlFiltro = GeneraSqlFiltroR2(tipoOpe, codPais, codPais2, anioMesIni, anioMesFin, indexCboPaisB, codPaisB, dua, auxCodPais, filtro: filx);

                    if (filtro == "InfoComplementario")
                    {
                        string codPaisAux = Session["CodPais"].ToString();

                        string nombrePais = FuncionesBusiness.BuscarPaisNombre(codPaisAux, idioma);

                        if (nombrePais == "USA")
                        {
                            nombrePais = "ESTADOS UNIDOS";
                        }

                        int idPais = FuncionesBusiness.BuscarIdPaisNombre(nombrePais, codPais);
                        if (nombrePais == "Alemania" && codPais == "EC")
                        {
                            idPais = 53;
                        }
                        if (nombrePais == "ESTADOS UNIDOS" && codPais == "EC")
                        {
                            idPais = 158;
                        }
                        // Ruben 202310
                        else if (nombrePais == "ESTADOS UNIDOS" && codPais == "MXD")
                        {
                            idPais = 321;
                        }

                        if (idPais != 0)
                        {
                            sqlFiltro = sqlFiltro + (tipoOpe == "I" ? " and idPaisOrigen = " : " and idPaisDestino = ") + idPais + " ";
                        }
                    }

                    if(sqlFiltro.Length < 55 || ValidarConsulta(sqlFiltro))
                    {
                        /*bool tipoServidor = Properties.Settings.Default.TableVarGeneral_InDev;
                        string path = ConfigurationManager.AppSettings["directorio_logs"];
                        Logs oLog = new Logs(path);
                        try
                        {
                            
                            oLog.Add("IdUsuario => " + Session["idUsuario"].ToString(), Session["idUsuario"].ToString());
                            oLog.Add("Servidor => " + (tipoServidor ? "Desarrollo" : "Produccion"), Session["idUsuario"].ToString());
                            oLog.Add("Controlador => MisBusquedas", Session["idUsuario"].ToString());
                            oLog.Add("Metodo Generado => GetExcelVerRegistro()", Session["idUsuario"].ToString());
                            oLog.Add("FlagDescarga => True", Session["idUsuario"].ToString());
                            oLog.Add("Consulta Generada => " + sqlFiltro, Session["idUsuario"].ToString());
                            oLog.Add("Consulta Session => " + Session["sqlFiltro"].ToString(), Session["idUsuario"].ToString());
                        }
                        catch(Exception ex)
                        {
                            oLog.Add("Excepcion => " + ex.ToString(), Session["idUsuario"].ToString());
                        }*/
                        
                        
                       
                        string mensaje = "Esta descarga no puede ser realizada porque no se ha seleccionado ningun filtro";

                        return Json(new
                        {
                            objMensaje = new
                            {
                                titulo = Resources.AdminResources.DownloadToExcel_Text,
                                mensaje,
                                flagContactenos = true
                            }
                        });
                    }

                }  
                else
                {
                    string _addEx = AddExtraFiltroDetalleExcel(txtDua, txtDesCom, dua);

                    sqlFiltro = GeneraSqlFiltroR(tipoOpe, codPais, codPais2, anioMesIni, anioMesFin, indexCboPaisB, codPaisB,
                dua, auxCodPais, filtro: "");
                    if (!string.IsNullOrWhiteSpace(Session["hdfPalabrasY"]?.ToString()))
                        sqlFiltro += ") ";
                    sqlFiltro += _addEx;

                    if(sqlFiltro.Length < 55 || ValidarConsulta(sqlFiltro))
                    {
                        /*bool tipoServidor = Properties.Settings.Default.TableVarGeneral_InDev;

                        string path = ConfigurationManager.AppSettings["directorio_logs"];
                        Logs oLog = new Logs(path);

                        try
                        {                            
                            oLog.Add("IdUsuario => " + Session["idUsuario"].ToString(), Session["idUsuario"].ToString());
                            oLog.Add("Servidor => " + (tipoServidor ? "Desarrollo" : "Produccion"), Session["idUsuario"].ToString());
                            oLog.Add("Controlador => MisBusquedas", Session["idUsuario"].ToString());
                            oLog.Add("Metodo Generado => GetExcelVerRegistro()", Session["idUsuario"].ToString());
                            oLog.Add("FlagDescarga => False", Session["idUsuario"].ToString());
                            oLog.Add("Consulta Generada => " + sqlFiltro, Session["idUsuario"].ToString());
                            oLog.Add("Consulta Session => " + Session["sqlFiltro"].ToString(), Session["idUsuario"].ToString());
                        }
                        catch(Exception ex)
                        {
                            oLog.Add("Excepcion => " + ex.ToString(), Session["idUsuario"].ToString());
                        }*/

                                               
                        

                        string mensaje = "Esta descarga no puede ser realizada porque no se ha seleccionado ningun filtro";

                        return Json(new
                        {
                            objMensaje = new
                            {
                                titulo = Resources.AdminResources.DownloadToExcel_Text,
                                mensaje,
                                flagContactenos = true
                            }
                        });
                    }
                }

                var flag = new TabMisBusquedas(tipoOpe, codPais);

                bool FlagFormatoB = ((sqlFiltro.Contains("IdMarca") && !flag.ExisteMarcaEC) || sqlFiltro.Contains("IdModelo"));

                if (FlagFormatoB)
                {
                    sqlFiltro = sqlFiltro.Replace("_PE", "_PEB");
                    sqlFiltro = sqlFiltro.Replace("_PEBB", "_PEB");
                }

                string cifTot = GetCIFTot(codPais, tipoOpe);  //Funciones.Incoterm(codPais, tipoOpe) + "Tot";
                string pesoNeto = Funciones.CampoPeso(codPais, tipoOpe);

                DataRow drTotales =
                    FuncionesBusiness.CalculaTotales(sqlFiltro, cifTot, codPais, pesoNeto, tabla, FlagFormatoB,  isManif: isManif);

                int cantReg = Convert.ToInt32(drTotales["CantReg"]);

                if (cantReg == 0)
                {
                    sqlFiltro = GeneraSqlFiltroR2Excel(tipoOpe, codPais, codPais2, anioMesIni, anioMesFin, indexCboPaisB, codPaisB,
                        dua,filtro: filx);

                    if(sqlFiltro.Length < 55 || ValidarConsulta(sqlFiltro))
                    {
                        /*bool tipoServidor = Properties.Settings.Default.TableVarGeneral_InDev;

                        string path = ConfigurationManager.AppSettings["directorio_logs"];
                        Logs oLog = new Logs(path);

                        try
                        {
                            oLog.Add("IdUsuario => " + Session["idUsuario"].ToString(), Session["idUsuario"].ToString());
                            oLog.Add("Servidor => " + (tipoServidor ? "Desarrollo" : "Produccion"), Session["idUsuario"].ToString());
                            oLog.Add("Controlador => MisBusquedas", Session["idUsuario"].ToString());
                            oLog.Add("Metodo Generado => GetExcelVerRegistro()", Session["idUsuario"].ToString());
                            oLog.Add("cantReg => 0", Session["idUsuario"].ToString());
                            oLog.Add("Consulta Generada => " + sqlFiltro, Session["idUsuario"].ToString());
                            oLog.Add("Consulta Session => " + Session["sqlFiltro"].ToString(), Session["idUsuario"].ToString());
                        }
                        catch(Exception ex)
                        {
                            oLog.Add("Excepcion => " + ex.ToString(), Session["idUsuario"].ToString());
                        } */                      
                        
                        

                        string mensaje = "Esta descarga no puede ser realizada porque no se ha seleccionado ningun filtro";

                        return Json(new
                        {
                            objMensaje = new
                            {
                                titulo = Resources.AdminResources.DownloadToExcel_Text,
                                mensaje,
                                flagContactenos = true
                            }
                        });
                    }

                    drTotales = FuncionesBusiness.CalculaTotales(sqlFiltro, cifTot, codPais, pesoNeto, tabla, FlagFormatoB, isManif: isManif);
                    cantReg = Convert.ToInt32(drTotales["CantReg"]);
                }

                object objMensajeFlag = null;
                if (!FlagDirecto)
                    objMensajeFlag = new
                    {
                        titulo = Resources.AdminResources.DownloadToExcel_Text,
                        mensaje = Resources.Resources.DownloadToExcel_Text,
                        flagContactenos = false
                    };
                return Json(new
                {
                    objMensaje,
                    infoArchivo = GeneraExcelRegistros(sqlFiltro, cantReg, tipoOpe, auxCodPais, codPais2, codCboDescargas, anioMesIni, anioMesFin, filtros, false, true, FlagFormatoB),
                    objMensajeFlag
                });
            }
        }

        private string GeneraSqlFiltroR2Excel(string tipoOpe, string codPais, string codPais2,
            string anioMesIni, string anioMesFin, int indexCboPaisB, string codPaisB,
            string dua, string filtro = "")
        {
            string sql = GeneraSqlFiltroR(tipoOpe, codPais, codPais2, anioMesIni, anioMesFin, indexCboPaisB, codPaisB,
                dua);
            //sql = sql.Substring(0, sql.Length - 1);

            if (Session["hdfVariable"] != null && filtro != "InfoComplementario" && filtro != "InfoTabla")
                sql += "and " + Session["hdfVariable"].ToString() + " = " + Session["hdfValor"].ToString() + " ";

            if (!string.IsNullOrWhiteSpace(Session["hdfPalabrasY"]?.ToString()))
                sql += ") ";

            return sql;
        }

        private object GeneraExcelRegistros(string SqlFiltro, int CantReg, string tipoOpe,
            string codPais, string codPais2, string codCboDescargas, string anioMesIni,
            string anioMesFin, string filtros, bool FlagDetalle = true, bool FlagDirecto = true,
            bool FlagFormatoB = false)
        {
            string idioma = Session["Idioma"].ToString();

            ValidaCodPaisManif(ref codPais, tipoOpe);
            var isManif = IsManifiesto(codPais);

            string CodPais1 = codPais;
            if (FlagFormatoB)
                CodPais1 = "PEB";

            string idUsuario = Session["IdUsuario"].ToString();
            string CodUsuario = FuncionesBusiness.BuscaCodUsuario(idUsuario).ToUpper();
            bool flagExcel = !FuncionesBusiness.ForzarZip(CodUsuario);
            string CamposIniciales, CamposExpressImpo, CamposExpressExpo;
            string Titulos, Campos;

            CamposExpressImpo =
                "('Nandina', 'Partida', 'Aduana', 'FechaNum', 'RUC', 'Importador', 'Proveedor', 'PesoNeto', 'Cantidad', 'Unidad', 'FOBTot', 'CIFTot', 'FOBUnit', 'CIFUnit', 'PaisOrigen', 'DesComercial') ";
            CamposExpressExpo =
                "('Nandina', 'Partida', 'Aduana', 'FechaNum', 'RUC', 'Exportador', 'PesoNeto', 'Cantidad', 'Unidad', 'FOBTot', 'FOBUnit', 'PaisDestino', 'DesComercial') ";



            if (Session["Plan"].ToString() != "ESENCIAL" || isManif)
                CamposIniciales = "";
            else if (tipoOpe == "I")
                CamposIniciales = CamposExpressImpo;
            else
                CamposIniciales = CamposExpressExpo;

            string IdDescargaCab;
            if (FlagDetalle)
            {
                IdDescargaCab = codCboDescargas;
                if (FlagFormatoB)
                    IdDescargaCab = "0";
                Titulos = FuncionesBusiness.ListaTitulosDescarga(IdDescargaCab, CamposIniciales,
                    (CodPais1 != "PEP" ? (codPais2.Contains("UE")? "UE" : CodPais1) : "PE"), tipoOpe, idioma);
                Campos = FuncionesBusiness.ListaCamposDescarga(IdDescargaCab, CamposIniciales,
                    (CodPais1 != "PEP" ? (codPais2.Contains("UE") ? "UE" : CodPais1) : "PE"), tipoOpe, idioma);
            }
            else
            {
                IdDescargaCab = codCboDescargas;
                if (FlagFormatoB)
                    IdDescargaCab = "0";
                Titulos = FuncionesBusiness.ListaTitulosDescarga(IdDescargaCab, CamposIniciales,
                    (CodPais1 != "PEP" ? (codPais2.Contains("UE") ? "UE" : CodPais1) : "PE"), tipoOpe, idioma);
                Campos = FuncionesBusiness.ListaCamposDescarga(IdDescargaCab, CamposIniciales,
                    (CodPais1 != "PEP" ? (codPais2.Contains("UE") ? "UE" : CodPais1) : "PE"), tipoOpe, idioma);
            }

            DateTime FechaArchivo = DateTime.Now;
            Session["UltHora"] = FechaArchivo;
            string nombreArchivo = "Veritrade_" + CodUsuario + "_" + (isManif ? "M_" : "") + (codPais2.Contains("UE") ? "UE" : "")+CodPais1 + "_" + tipoOpe + "_" +
                                   FechaArchivo.ToString("yyyyMMddHHmmss");

            var EsFreeTrial = (Session["TipoUsuario"].ToString() == "Free Trial") || (bool)(Session["opcionFreeTrial"] ?? false);

            FuncionesBusiness.GeneraArchivoBusqueda(Convert.ToInt32(idUsuario), EsFreeTrial, (codPais2.Contains("UE") ? "UE" : CodPais1),
                tipoOpe, Titulos, Campos, SqlFiltro.Replace("'", "''").Replace("\"", "\"\""), CantReg, nombreArchivo, ((CantReg <= CantRegMaxExcel && flagExcel) ? 'S' : 'N'));

            Funciones.GrabaLog(idUsuario, (codPais2.Contains("UE") ? "UE" :CodPais1), tipoOpe, anioMesIni, anioMesFin, "DownloadExcel", "");

            string extensionArchivo = ".xlsx";
            if (CantReg <= CantRegMaxExcel && flagExcel)
            {
                //NombreArchivo = "Veritrade_JANAQ_PE_I_20180418182351";

                nombreArchivo += ".xlsx";

                using (OfficeOpenXml.ExcelPackage package = new OfficeOpenXml.ExcelPackage(
                    new System.IO.FileInfo(ConfigurationManager.AppSettings["directorio_descarga"] + nombreArchivo)))
                {
                    var sheet = package.Workbook.Worksheets["Veritrade"];

                    if (codPais2 != "4UE")
                        sheet.Cells["B1"].Value = FuncionesBusiness.Pais(CodPais1, idioma).ToUpper() + (isManif ? "" :
                                                  " - " + (tipoOpe == "I" ? Resources.AdminResources.Imports_Text.ToUpper() : Resources.AdminResources.Exports_Text.ToUpper()));
                    else
                        sheet.Cells["B1"].Value = FuncionesBusiness.BuscarPaisUE(codPais, idioma) + " - " +
                                                  (tipoOpe == "I" ? Resources.AdminResources.Imports_Text.ToUpper() : Resources.AdminResources.Exports_Text.ToUpper());

                    sheet.Cells["B2"].Value = filtros;
                    sheet.Cells["B3"].Value = Resources.AdminResources.Period_Text + ": " + GetRangoFechas(anioMesIni, anioMesFin, idioma);

                    sheet.Cells["B4"].Value = Resources.AdminResources.RecordsFirsLetterUpperCase_Text + ": " + CantReg.ToString("n0");
                    if (EsFreeTrial && CantReg > CantRegMaxFreeTrial)
                        sheet.Cells["B5"].Value = (idioma == "es") ? "Muestra: 20 registros" : "Sample: 20 records";

                    package.Save();
                }
            }
            else
            {
                nombreArchivo += ".zip";
                extensionArchivo = ".zip";
            }

            /*
            string extensionArchivo = ".xlsx";

            //Se agrega encabezado a archivo excel generado por sp
            using (OfficeOpenXml.ExcelPackage package = new OfficeOpenXml.ExcelPackage(
                new System.IO.FileInfo(ConfigurationManager.AppSettings["directorio_descarga"] + nombreArchivo + extensionArchivo)))
            {
                var sheet = package.Workbook.Worksheets["Veritrade"];

                if (codPais2 != "4UE")
                    sheet.Cells["B1"].Value = FuncionesBusiness.Pais(CodPais1, idioma).ToUpper() + (isManif ? "" :
                                                " - " + (tipoOpe == "I" ? Resources.AdminResources.Imports_Text.ToUpper() : Resources.AdminResources.Exports_Text.ToUpper()));
                else
                    sheet.Cells["B1"].Value = FuncionesBusiness.BuscarPaisUE(codPais, idioma) + " - " +
                                                (tipoOpe == "I" ? Resources.AdminResources.Imports_Text.ToUpper() : Resources.AdminResources.Exports_Text.ToUpper());

                sheet.Cells["B2"].Value = filtros;
                sheet.Cells["B3"].Value = Resources.AdminResources.Period_Text + ": " + GetRangoFechas(anioMesIni, anioMesFin, idioma);

                sheet.Cells["B4"].Value = Resources.AdminResources.RecordsFirsLetterUpperCase_Text + ": " + CantReg.ToString("n0");
                if (EsFreeTrial && CantReg > CantRegMaxFreeTrial)
                    sheet.Cells["B5"].Value = (idioma == "es") ? "Muestra: 20 registros" : "Sample: 20 records";

                package.Save();
            }
            if (CantReg <= CantRegMaxExcel && flagExcel)
            {
                nombreArchivo += ".xlsx";
                extensionArchivo = ".xlsx";
            }
            else
            {
                //Se comprime xlsx en zip
                string newFile = ConfigurationManager.AppSettings["directorio_descarga"] + nombreArchivo + extensionArchivo;
                using (ZipArchive archive = ZipFile.Open(ConfigurationManager.AppSettings["directorio_descarga"] + nombreArchivo + ".zip", ZipArchiveMode.Update))
                {
                    archive.CreateEntryFromFile(newFile, nombreArchivo + extensionArchivo);
                }

                nombreArchivo += ".zip";
                extensionArchivo = ".zip";
            }
            */
            if (FlagDirecto)
            {
                Session["UltArchivo"] = nombreArchivo;
                Session["UltExtensionArchivo"] = extensionArchivo;

                return new
                {
                    flagDirecto = FlagDirecto,
                    fileName = nombreArchivo,
                    extensionArchivo
                };
            }
            else
            {
                return new
                {
                    flagDirecto = FlagDirecto,
                    extensionArchivo,
                    infoArchivoDescarga = new
                    {
                        infoFile = nombreArchivo + " [generado en " + FechaArchivo.ToString("HH:mm:ss tt") + "]",
                        navigateUrl = Funciones.ruta_descarga + nombreArchivo,
                        visible = true
                    }
                };
            }
        }

        [HttpGet]
        public ActionResult DownloadFile(string file, string typeFile)
        {
            string fullPath = ConfigurationManager.AppSettings["directorio_descarga"] + file;
            if (typeFile == ".zip")
            {
                return File(fullPath, "application/zip", file);
            }
            else
            {
                return File(fullPath, "application/vnd.ms-excel", file);
            }
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult GetExcelDetalleB(string tipoOpe, string codPais, string codPais2,
            string anioMesIni, string anioMesFin, int indexCboPaisB,
            string codPaisB, string filtros, bool FlagDirecto = true)
        {
            GuardarLogInicioErrorSession("GetExcelDetalleB()");
            ValidaCodPais2(codPais2, ref codPais);
            ValidaCodPaisManif(ref codPais, tipoOpe);
            var isManif = IsManifiesto(codPais);
            object objMensaje = null;
            if (Session["UltHora"] != null)
            {
                int Segundos = DateTime.Now.Subtract(Convert.ToDateTime(Session["UltHora"])).Seconds;
                if (Segundos < 10)
                {
                    return Json(new
                    {
                        objMensaje,
                        infoArchivo = new
                        {
                            flagDirecto = true,
                            fileName = Session["UltArchivo"].ToString(),
                            extensionArchivo = Session["UltExtensionArchivo"].ToString()
                        }
                    });
                }
            }

            int limiteDescargas = 0, descargas = 0;

            string idioma = Session["Idioma"].ToString();
            if (!FuncionesBusiness.ValidaDescargasMes(Session["IdUsuario"].ToString(), codPais, tipoOpe, ref limiteDescargas, ref descargas))
            {
                string mensaje = "Esta descarga no se puede efectuar.<br>Se ha llegado al máximo contratado de " +
                                 limiteDescargas.ToString("n0") + " descargas mensuales.";
                if (idioma == "en")
                {
                    mensaje = "Could not download to Excel.<br>It reached to Monthly downloads number limit: " +
                              limiteDescargas.ToString("n0");
                }

                return Json( new
                {
                    objMensaje = new
                    {
                        titulo = Resources.AdminResources.DownloadToExcel_Text,
                        mensaje,
                        flagContactenos = true
                    }
                });
            }

            var tabla = "";
            var dua = "";
            GetTablaAndDua(tipoOpe, codPais, ref tabla, ref dua);

            string sqlFiltro = GeneraSqlFiltroR1(tipoOpe, codPais, codPais2, anioMesIni, anioMesFin, indexCboPaisB, codPaisB,
                dua).Replace("Importacion_PE", "Importacion_PEB");

            if (sqlFiltro.Length < 55 || ValidarConsulta(sqlFiltro))
            {
                /*bool tipoServidor = Properties.Settings.Default.TableVarGeneral_InDev;
                string path = ConfigurationManager.AppSettings["directorio_logs"];
                
                Logs oLog = new Logs(path);

                try
                {
                    oLog.Add("IdUsuario => " + Session["idUsuario"].ToString(), Session["idUsuario"].ToString());
                    oLog.Add("Servidor => " + (tipoServidor ? "Desarrollo" : "Produccion"), Session["idUsuario"].ToString());
                    oLog.Add("Controlador => MisBusquedas", Session["idUsuario"].ToString());
                    oLog.Add("Metodo Generado => GetExcelDetalleB()", Session["idUsuario"].ToString());
                    oLog.Add("Consulta Generada => " + sqlFiltro, Session["idUsuario"].ToString());
                    oLog.Add("Consulta Session => " + Session["sqlFiltro"].ToString(), Session["idUsuario"].ToString());
                }
                catch (Exception ex)
                {
                    oLog.Add("Excepcion => " + ex.ToString(), Session["idUsuario"].ToString());
                }*/

                
                
                
                

                string mensaje = "Esta descarga no puede ser realizada porque no se ha seleccionado ningun filtro";

                return Json(new
                {
                    objMensaje = new
                    {
                        titulo = Resources.AdminResources.DownloadToExcel_Text,
                        mensaje,
                        flagContactenos = true
                    }
                });
            }

            string cifTot = GetCIFTot(codPais, tipoOpe);//Funciones.Incoterm(codPais, tipoOpe) + "Tot";
            string pesoNeto = Funciones.CampoPeso(codPais, tipoOpe);
            DataRow drTotales =
                FuncionesBusiness.CalculaTotales(sqlFiltro, cifTot, codPais, pesoNeto, tabla, true, isManif: isManif);
            int cantReg = Convert.ToInt32(drTotales["CantReg"]);
            if (cantReg == 0)
            {
                sqlFiltro = GeneraSqlFiltroR1Excel(tipoOpe, codPais, codPais2, anioMesIni, anioMesFin, indexCboPaisB, codPaisB,
                    dua).Replace("Importacion_PE", "Importacion_PEB");

                if (sqlFiltro.Length < 55 || ValidarConsulta(sqlFiltro))
                {
                    /*bool tipoServidor = Properties.Settings.Default.TableVarGeneral_InDev;

                    string path = ConfigurationManager.AppSettings["directorio_logs"];
                    Logs oLog = new Logs(path);

                    try
                    {

                        oLog.Add("IdUsuario => " + Session["idUsuario"].ToString(), Session["idUsuario"].ToString());
                        oLog.Add("Servidor => " + (tipoServidor ? "Desarrollo" : "Produccion"), Session["idUsuario"].ToString());
                        oLog.Add("Controlador => MisBusquedas", Session["idUsuario"].ToString());
                        oLog.Add("Metodo Generado => GetExcelDetalleB()", Session["idUsuario"].ToString());
                        oLog.Add("cantReg => 0", Session["idUsuario"].ToString());
                        oLog.Add("Consulta Generada => " + sqlFiltro, Session["idUsuario"].ToString());
                        oLog.Add("Consulta Session => " + Session["sqlFiltro"].ToString(), Session["idUsuario"].ToString());

                    }
                    catch(Exception ex)
                    {
                        oLog.Add("Excepcion => " + ex.ToString(), Session["idUsuario"].ToString());
                    }*/

                    
                    
                    

                    string mensaje = "Esta descarga no puede ser realizada porque no se ha seleccionado ningun filtro";

                    return Json(new
                    {
                        objMensaje = new
                        {
                            titulo = Resources.AdminResources.DownloadToExcel_Text,
                            mensaje,
                            flagContactenos = true
                        }
                    });
                }

                drTotales =
                    FuncionesBusiness.CalculaTotales(sqlFiltro, cifTot, codPais, pesoNeto, tabla, true, isManif:isManif);
                cantReg = Convert.ToInt32(drTotales["CantReg"]);
            }

            var infoArchivo = GeneraExcelRegistros(sqlFiltro, cantReg, tipoOpe, codPais, codPais2, "", anioMesIni,
                anioMesFin, filtros, true, true, true);
            Object objMensajeFlag = null;

            if (!FlagDirecto)
            {
                objMensajeFlag = new
                {
                    titulo = Resources.AdminResources.DownloadToExcel_Text,
                    mensaje = Resources.Resources.DownloadToExcel_Text,
                    flagContactenos = false
                };
            }

            return Json(new
            {
                objMensaje,
                infoArchivo,
                objMensajeFlag
            });
        }

        private string GeneraSqlFiltroR1(string tipoOpe, string codPais, string codPais2,
            string anioMesIni, string anioMesFin, int indexCboPaisB,
            string codPaisB, string dua)
        {
            string sql = GeneraSqlFiltroR(tipoOpe, codPais, codPais2, anioMesIni, anioMesFin, indexCboPaisB, codPaisB,
                dua);
            //sql = sql.Substring(0, sql.Length - 1);

            if (Session["hdfDUAB"] != null)
                sql += "and " + dua + " like '%" + Session["hdfDUAB"].ToString() + "%' ";
            if (Session["hdfDesComercialBB"] != null)
                sql += "and DesComercial like '%" + Session["hdfDesComercialBB"].ToString() + "%' ";

            if (!string.IsNullOrWhiteSpace(Session["hdfPalabrasY"]?.ToString()))
                sql += ") ";

            return sql;
        }

        private string GeneraSqlFiltroR1Excel(string tipoOpe, string codPais, string codPais2,
            string anioMesIni, string anioMesFin, int indexCboPaisB,
            string codPaisB, string dua)
        {
            string sql = GeneraSqlFiltroR(tipoOpe, codPais, codPais2, anioMesIni, anioMesFin, indexCboPaisB, codPaisB,
                dua);

            if (!string.IsNullOrWhiteSpace(Session["hdfPalabrasY"]?.ToString()))
                sql += ") ";

            return sql;
        }
#endregion

        public static string RenderViewToString(ControllerContext context, string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = context.RouteData.GetRequiredString("action");

            var viewData = new ViewDataDictionary(model);

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(context, viewName);
                var viewContext = new ViewContext(context, viewResult.View, viewData, new TempDataDictionary(), sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        private CultureInfo GetSpecificCulture()
        {
            if (Session["culture"] == null )
            {
                string cultureName = RouteData.Values["culture"] as string;

                // Attempt to read the culture cookie from Request
                if (cultureName == null)
                    cultureName = Request.UserLanguages != null && Request.UserLanguages.Length > 0 ? Request.UserLanguages[0] : null; // obtain it from HTTP header AcceptLanguages

                // Validate culture name
                cultureName = CultureHelper.GetImplementedCulture(cultureName); // This is safe


                // Modify current thread's cultures            
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
                Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

                Session["culture"] = cultureName;
            }


            return Session["culture"].ToString().Equals("es")
                ? CultureInfo.CreateSpecificCulture("es-pe")
                : CultureInfo.CreateSpecificCulture("en-us");
        }

        [HttpPost]
        public ActionResult SetCulture(string culture)
        {
            culture = CultureHelper.GetImplementedCulture(culture);
            RouteData.Values["culture"] = culture; // set culture 

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult SearchCulture(string cultur)
        {
            //var listaFiltro = Session["lstFiltros"] as List<OptionSelect>;
            //var sql = Session["UltSqlFiltro"] ?? "";
            var search = new object();
            if (string.IsNullOrEmpty(cultur))
            {
                search = Session["sqlFiltro"] ?? "";
            }
            else
            {
                search = Session[cultur] ?? "No hay información existente";
            }
            
            return Json(new
            {
                search
            });
        }

        /// <summary>
        /// Obtiene las notas y detalles correspondientes a un código país y el idioma seleccionado
        /// </summary>
        /// <param name="codigoPais"></param>
        /// <param name="lenguaje"></param>
        /// <returns></returns>
        public JObject ObtenerNotaYDetallesPais(string codigoPais, string lenguaje)
        {

            DataTable dt = FuncionesBusiness.ObtenerNotaYDetallesPais(codigoPais, lenguaje);

            int idMensajePais = (from DataRow dr in dt.Rows
                                    select (int)dr["IdMensajePais"]).FirstOrDefault();

            string notas = (from DataRow dr in dt.Rows
                            select (string)dr["Nota"]).FirstOrDefault();

            string detalles = (from DataRow dr in dt.Rows
                            select (string)dr["Detalle"]).FirstOrDefault();

            JObject datosPais = JObject.FromObject(new
            {
                datosPais = new
                {
                    idMensajePais,
                    notas,
                    detalles
                }
            });

            return datosPais;
        }
    }
}