using PagedList;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using Veritrade2018.App_Start;
using Veritrade2018.Helpers;
using Veritrade2018.Models;
using Veritrade2018.Models.Admin;
using Veritrade2018.Util;

namespace Veritrade2018.Controllers
{
    public class FreemiumController : Controller
    {
        // GET: Freemium
        private int ResumenTabsGridPageSize = 5;
        private int TabGridPageSize = 10;
        private int VerRegistrosPageSize = 10;
        private string _codPais, IdUsuario;
        //private int MisFavoritosPageSize = 20;
        //private string IdUsuario, TipoUsuarioT, CodPais, TipoOpe;
        int CantRegMax = 200000, LimiteFiltros = 10;
        //private int CantRegMaxExcel = 40000, CantRegMaxFreeTrial = 20;
        private readonly string[] _paisesCondicionViaTransp = new[] { "BO", "MXD", "PY", "US", "UE" };
        private readonly string[] _paisesCondicionAduana = new[] { "CN", "IN", "PY", "US", "UE", "RD" };
        private readonly string[] _paisesCondicionDua = new[] { "AR", "BR", "CO", "MX", "MXD", "UY" };
        private static  Dictionary<string, List<SelectListItem>> listaDictionary = new Dictionary<string, List<SelectListItem>>();
        private static bool existePaisFiltro = true;
        public ActionResult Index(string culture, string cod)
        {   
            if(listaDictionary.Count == 0)
                  cargarDiccionario();
            Session.RemoveAll();
            if (string.IsNullOrEmpty(cod) ||  !validarCodigo(cod))
            {
                return RedirectToAction("Index", "Home");
            }

            cod = cod.ToUpper();

#if DEBUG
            string DireccionIP = Properties.Settings.Default.IP_Debug;
#else
            string DireccionIP = Request.ServerVariables["REMOTE_ADDR"];

#endif
            string Navegador = Request.ServerVariables["HTTP_USER_AGENT"];

            if(cod.ToLower() == "asean" || cod.ToLower() == "camchal")
            {
                Session["IdUsuarioFree"] = "126911";
            }
            // Ruben 202303
            else if (cod.ToLower() == "ahkarg")
            {
                Session["IdUsuarioFree"] = "126911";
            }

            if (Session["IdUsuarioFree"] == null)
            {
                Session["IdUsuarioFree"] = Funciones.BuscarIdUsuario("FREE_" + cod);

                if(Session["IdUsuarioFree"].ToString() == "")
                    return RedirectToAction("Index", "Home");

                //string DireccionIP = Request.ServerVariables["REMOTE_ADDR"];
                FuncionesBusiness.GrabaHistorial(Session["IdUsuarioFree"].ToString(), DireccionIP, Navegador, "");
            }

            Session["TipoUsuario"] = Funciones.BuscaTipoUsuario(Session["IdUsuarioFree"].ToString());
            Session["Plan"] = Funciones.ObtienePlan(Session["IdUsuarioFree"].ToString()).ToUpper();
            Session["Origen"] = Funciones.ObtieneOrigen(Session["IdUsuarioFree"].ToString()).ToUpper();

            if(cod.ToLower() == "asean" || cod.ToLower() == "camchal")
            {
                Session["CodPais"] = "CL";
            }
            // Ruben 202303
            else if (cod.ToLower() == "ahkarg")
            {
                Session["CodPais"] = "AR";
            }
            else
            {
                Session["CodPais"] = "PE";
            }
            
            Session["TipoOpe"] = "I";


            VeritradeAdmin.Seguridad ws = new VeritradeAdmin.Seguridad();
            string CodPaisIP = "",CodPais ="";
            string Ubicacion = ws.BuscaUbicacionIP2(DireccionIP, ref CodPaisIP);

           // if (Session["CodPais"] != null)
                CodPais = Session["CodPais"].ToString();
            //else if (Funciones.ValidaPais(Session["IdUsuarioFree"].ToString(), CodPaisIP) && !Funciones.FlagCarga(CodPaisIP) && (CodPaisIP == "AR" || CodPaisIP == "BO" || CodPaisIP == "BR" || CodPaisIP == "CL" || CodPaisIP == "CO" || CodPaisIP == "CR" || CodPaisIP == "EC" || CodPaisIP == "MX" || CodPaisIP == "PA" || CodPaisIP == "PY" || CodPaisIP == "PE" || CodPaisIP == "UY"))
            //    CodPais = CodPaisIP;
            //else
            //{
            //    CodPais = Funciones.ObtieneCodPaisAcceso(Session["IdUsuarioFree"].ToString());
            //    if (CodPais == "PEI" || CodPais == "PEE" || CodPais == "USI" || CodPais == "USE")
            //    {
            //        Session["CodPais"] = CodPais.Substring(0, 2) + "_";
            //        Session["TipoOpe"] = CodPais.Substring(2, 1);
            //        Response.Redirect("MisBusquedasUS.aspx");
            //    }
            //}
            existePaisFiltro = true;
            var listaPalabras = new List<object>();
            string Campaña = "";
            switch (cod)
            {
                case "SNI": Campaña = "25100";
                    break;
                case "AMCHAM":
                case "ASEAN":
                    Campaña = "23100";
                    break;
                case "CCIPERU": Campaña = "22100";
                    break;
                case "AHKPERU":                
                    Campaña = "24100";
                    break;
                case "CAMCHAL":
                    Campaña = "31100";
                    break;
                default:
                    Campaña = "26100";
                    break;
            }
            /*if (cod == "SNI") Campaña = "25100";
            else if (cod == "AMCHAM")
            {
                Campaña = "23100";
                //listaPalabras.Add(AgregaPalabrasFiltros("UNITED STATES", "1PAI", CodPais));
            }
            else if (cod == "CCIPERU")
            {
                Campaña = "22100";
                //listaPalabras.Add(AgregaPalabrasFiltros("ITALY", "1PAI", CodPais));
            }
            else if (cod == "AHKPERU")
            {
                Campaña = "24100";
                //listaPalabras.Add(AgregaPalabrasFiltros("GERMANY", "1PAI", CodPais));
            }
            else if (cod == "COCEP")
            {
                Campaña = "26100";
                //listaPalabras.Add(AgregaPalabrasFiltros("SPAIN", "1PAI", CodPais));
            }*/
            Session["c"] = Campaña;
            ViewData["cargarPaises"] = CargarPaises(cod);

            int disabledComboPais = 0;
            Session["culture"] = culture;
            if (Session["Plan"].ToString() == "PERU IMEX" ||
                Session["Plan"].ToString() == "ECUADOR IMEX")
                disabledComboPais = 1;
            else if (Session["Plan"].ToString() == "ESENCIAL" || Session["Plan"].ToString() == "PERU UNO" ||
                     Session["Plan"].ToString() == "ECUADOR UNO")
                disabledComboPais = 2;

           // bool ExisteImportador, ExisteProveedor = false, ExistePaisOrigen , ExisteDistrito, ExisteDesComercial = false, ExisteDesComercial2 = true;
           // bool ExisteExportador, ExisteImportadorExp = false, ExistePaisDestino, ExisteViaTransp, ExisteAduana = false, ExisteDUA = false;
            string TipoOpe = Session["TipoOpe"].ToString();
            //ExisteImportador = Funciones.ExisteVariable(CodPais, TipoOpe, "IdImportador");
            //ExistePaisOrigen = Funciones.ExisteVariable(CodPais, TipoOpe, "IdPaisOrigen");
            //ExisteExportador = Funciones.ExisteVariable(CodPais, TipoOpe, "IdExportador");
            //ExistePaisDestino = Funciones.ExisteVariable(CodPais, TipoOpe, "IdPaisDestino");
            //ExisteViaTransp = (CodPais != "BO" && CodPais != "PY" && CodPais != "US");

            //if (Session["Plan"].ToString() == "ESENCIAL")
            //{
            //    ExisteDUA = false;
            //    ExisteImportadorExp = false;
            //}
            //ExisteDistrito = (CodPais == "CN" || CodPais == "US");

            string añoInfoIni = "", mesInfoIni = "", añoInfoFin = "", mesInfoFin = "";

            Funciones.Rango(CodPais, TipoOpe, ref añoInfoIni, ref mesInfoIni, ref añoInfoFin, ref mesInfoFin);            
            Session["codigoOperacion"] = cod;
            ViewData["cargarPeriodos"] = Session["listaTipoFiltro"] = CargarPeriodos(CodPais, TipoOpe,culture , "T");
            ViewData["listaPalabras"] = listaPalabras;
            ViewData["listaPaises2"] = FuncionesBusiness.CargarPaises(CodPais,culture);
            ViewData["culture"] = culture;
            ViewData["disabledComboPais"] = disabledComboPais;
            ViewData["filtrosTabs"] = GetFiltersNames();
            ViewData["CodCampaña"] = Campaña;
            ViewData["TipoOpe"] = Session["TipoOpe"].ToString();
            ViewData["codigoPais"] = Session["CodPais"].ToString();
            ViewData["codigoOperacion"] = cod;

            ConsultaForm consultaForm = new ConsultaForm
            {
                Importador = true,
                Proveedor = false,
                PaisOrigen = true,
                Exportador = false,
                ImportadorExp = false,
                CodPais = Session["CodPais"].ToString(),
                TipoOpe = Session["TipoOpe"].ToString(),
                FlagDescComercialB = false
            };
            ViewData["MaxDataPeriod"] = Session["MaxDataPeriod"].ToString();
            ViewData["consultaForm"] = consultaForm;
            ViewData["serviciosMenu"] = new Servicios().GetServicios("es");
            return View();
        }

        public void cargarDiccionario()
        {
            if(listaDictionary.Count == 0)
            {
                listaDictionary.Add
            ("es",
                new List<SelectListItem> {
                    new SelectListItem { Value = "PA", Text = @"Partida" },
                    new SelectListItem { Value = "IM", Text = @"Importador" },
                    new SelectListItem { Value = "EX", Text = @"Exportador" },
                    new SelectListItem { Value = "PO", Text = @"País Origen" },
                    new SelectListItem { Value = "PD", Text = @"País Destino" }
                }
            );
                listaDictionary.Add
                ("en",
                    new List<SelectListItem> {
                    new SelectListItem { Value = "PA", Text = @"HTS Code" },
                    new SelectListItem { Value = "IM", Text = @"Importer" },
                    new SelectListItem { Value = "EX", Text = @"Exporter" },
                    new SelectListItem { Value = "PO", Text = @"Origin Country" },
                    new SelectListItem { Value = "PD", Text = @"Destination Country" }
                    }
                );
            }
                   
        }

        [HttpPost]
        public JsonResult rdbtnTipoOpeChange(string selectedTipoOpe, string codPais, string idiomaCulture)
        {
            
            Session["CodPais"] = codPais;
            Session["TipoOpe"] = selectedTipoOpe;
            Restablecer();
            var consultaForm = GetConsultaForm(codPais, selectedTipoOpe);

            return Json(new
            {
                htmlFormSection = RenderViewToString(ControllerContext, "Consulta/_FormSection", consultaForm),

            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult rdbtnTipoFiltroChange(string tipoBusqueda, string tipoOperacion, string idiomaCulture)
        {
            var listado = CargarPeriodos(Session["CodPais"].ToString(), tipoBusqueda, idiomaCulture, tipoOperacion);
            if(listado != null)
                Session["listaTipoFiltro"] = listado;
            var mayor = listado.LastOrDefault()?.Value;
            Session["MaxDataPeriod"] = mayor;
            return Json(new
            {
                lista = listado,
                datoSeleccionado = mayor

            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult cboDesdeChange(int inicio , string CodPais ,string TipoOpe, string culture, string TipoPeriodo)
        {
            var listado = (IEnumerable<SelectListItem>)Session["listaTipoFiltro"] ?? CargarPeriodos(CodPais, TipoOpe, culture, TipoPeriodo);
            var lista = listado.Where(x => Convert.ToInt32(x.Value) >= inicio).ToList();
           

            return Json(new
            {
                lista = lista,
                datoSeleccionado = Session["MaxDataPeriod"].ToString()

            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult BuscarPartida(string nandina, string codPais, string idioma)
        {
           // var json = new List<object>();
            var json = GeneraJsonBuscaPartidas(nandina, codPais, idioma);

            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AgregaPartida(string filtro, string seleccionado, string codPais)
        {
            if (seleccionado == "") return Json("No hay un elemento seleccionado", JsonRequestBehavior.AllowGet);

            var newItem = new List<object>();
            var id = seleccionado;

            newItem.Add(AgregaPalabrasFiltros(id, "2P_", codPais));

            return Json(newItem, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult BuscarEmpresas(string importadorB, string codPais)
        {
            var json = GeneraJsonBuscaEmpresas(importadorB, codPais);
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        public JsonResult AgregaImportador(string filtro, string seleccionado, string codPais)
        {
            var id = seleccionado;
            if (id == "") return Json("No hay un elemento seleccionado", JsonRequestBehavior.AllowGet);

            var newItem = new List<object>();

            newItem.Add(id != "0"
                ? AgregaPalabrasFiltros("[" + id + "]", "3I_", codPais, id)
                : AgregaPalabrasFiltros(filtro.ToUpper().Trim(), "3I_", codPais));

            return Json(newItem, JsonRequestBehavior.AllowGet);
        }
        public JsonResult AgregaExportador(string filtro, string seleccionado, string CodPais)
        {
            var id = seleccionado;
            if (id == "") return Json("No hay un elemento seleccionado", JsonRequestBehavior.AllowGet);

            var newItem = new List<object>();

            newItem.Add(id != "0"
                ? AgregaPalabrasFiltros("[" + id + "]", "3E_", CodPais, id)
                : AgregaPalabrasFiltros(filtro.ToUpper().Trim(), "3E_", CodPais));

            return Json(newItem, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteFilters(string[] valueOptionsSelected)
        {
            if (!ValidarFiltros(valueOptionsSelected))
            {
                return Json(new { estado = true, mensaje = "Los filtros País, Vías y/o Aduanas tienen  que estar acompañados de algún otro filtro" }, JsonRequestBehavior.AllowGet);
            }
            //string palabraItem;
            foreach (var item in valueOptionsSelected)
            {
                string ID = item.Substring(3, item.Length - 3);
                switch (item.Substring(0, 3))
                {
                    case "1PA":
                        existePaisFiltro = false;
                        break;
                    case "2P_":
                        ID = item.Substring(4, item.Length - 4);
                        Session["hdfNandinaB"] = (" " + Session["hdfNandinaB"] + " ").Replace(" " + ID + " ", "")
                                        .Replace("  ", " ").Trim();
                        break;
                    case "2PA":
                        if (!item.Substring(4).Contains("Page"))
                        {
                            Session["PartidasB"] = Funciones.EliminaSeleccion((ArrayList)Session["PartidasB"], ID);
                        }
                        else
                        {
                            Session["hdfIdGrupoB"] = null;
                        }

                        break;
                    case "3I_":
                        ID = item.Substring(4, item.Length - 4);
                        string[] importadoresB = Session["hdfImportadorB"].ToString().Split(new char[] { '|' });
                        int numIndexImpoB = Array.IndexOf(importadoresB, ID);
                        importadoresB = importadoresB.Where((val, idx) => idx != numIndexImpoB).ToArray();
                        Session["hdfImportadorB"] = string.Join("|", importadoresB);
                        break;
                    case "3IM":
                        if (!item.Substring(4).Contains("Page"))
                        {
                            Session["ImportadoresB"] =
                                Funciones.EliminaSeleccion((ArrayList)Session["ImportadoresB"], ID);
                        }
                        else
                        {
                            Session["hdfIdGrupoB"] = null;

                        }
                        break;
                    case "3E_":
                        ID = item.Substring(4, item.Length - 4);
                        Session["hdfExportadorB"] = (" " + Session["hdfExportadorB"] + " ")
                            .Replace(" " + ID + " ", "").Replace("  ", " ").Trim();
                        break;
                    case "3EX":
                        if (!item.Substring(4).Contains("Page"))
                        {
                            Session["ExportadoresB"] =
                                Funciones.EliminaSeleccion((ArrayList)Session["ExportadoresB"], ID);
                        }
                        else
                            Session["hdfIdGrupoB"] = null;
                        break;
                    case "4P_":
                        ID = item.Substring(4, item.Length - 4);
                        Session["hdfProveedorB"] = (" " + Session["hdfProveedorB"] + " ")
                            .Replace(" " + ID + " ", "")
                            .Replace("  ", " ").Trim();
                        break;
                    case "4PR":
                        //palabraItem = item.Substring(4);
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
                        Session["hdfImportadorExpB"] = (" " + Session["hdfImportadorExpB"] + " ")
                            .Replace(" " + ID + " ", "").Replace("  ", " ").Trim();
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
                    case "5PO":
                        Session["PaisesOrigenB"] =
                            Funciones.EliminaSeleccion((ArrayList)Session["PaisesOrigenB"], ID);
                        break;
                    case "5PD":
                        Session["PaisesDestinoB"] =
                            Funciones.EliminaSeleccion((ArrayList)Session["PaisesDestinoB"], ID);
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
                    case "5PE":
                        Session["PaisesEmbarqueB"] =
                            Funciones.EliminaSeleccion((ArrayList)Session["PaisesEmbarqueB"], ID);
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
            var lista = Session["lstFiltros"] as List<OptionSelect>;
            if (lista==null)
                Restablecer();
            else if(lista.Count == 0)
                Restablecer();
            return Json(new { estado = false, mensaje = "",existe = existePaisFiltro }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FiltersContent()
        {
            return PartialView("Consulta/_FormFilters");
        }

        [HttpPost]
        public JsonResult PalabraInicio(string tipoOperacion , string codOperacion)
        {

            var listaPalabras = new List<object>();
            string CodPais = Session["CodPais"].ToString();
            
            
            
            if (codOperacion == "AMCHAM")
            {
                listaPalabras.Add(AgregaPalabrasFiltros("UNITED STATES", "1PA"+tipoOperacion, CodPais));
            }
            else if (codOperacion == "CCIPERU")
            {
                listaPalabras.Add(AgregaPalabrasFiltros("ITALY", "1PA" + tipoOperacion, CodPais));
            }
            else if (codOperacion == "AHKPERU" || codOperacion == "CAMCHAL")
            {
                listaPalabras.Add(AgregaPalabrasFiltros("GERMANY", "1PA" + tipoOperacion, CodPais));
            }
            // Ruben 202303
            else if (codOperacion == "AHKARG")
            {
                listaPalabras.Add(AgregaPalabrasFiltros("GERMANY", "1PA" + tipoOperacion, CodPais));
            }
            else if (codOperacion == "COCEP")
            {
                listaPalabras.Add(AgregaPalabrasFiltros("SPAIN", "1PA"+tipoOperacion, CodPais));
            }
            else if (codOperacion == "ASEAN")
            {
                listaPalabras.Add(AgregaPalabrasFiltros("UNITED STATES", "1PA" + tipoOperacion, CodPais));
            }

            return Json(new
            {
                response = true,
                listaPalabras

            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ComboChangePais(string tipoOperacion, string codOperacion, string pais, string codigoPais)
        {
            Session["lstFiltros"] = null;
            var listaPalabras = new List<object>();
            //string CodPais = Session["CodPais"].ToString();

            listaPalabras.Add(AgregaPalabrasFiltros(pais , "1PA" + tipoOperacion, Session["CodPais"].ToString()));
            existePaisFiltro = true;
            
            return Json(new
            {
                response = true,
                listaPalabras

            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult BuscarClick(string codPais, int codPais2,
            string tipoOpe, string tipoBusqueda, string desde, string hasta, string idioma)
        {
            var specificCulture = GetSpecificCulture();

            bool FlagPalabras = (!string.IsNullOrWhiteSpace(Session["hdfPalabrasY"]?.ToString()));

            var tabla = "";
            var dua = "";
            GetTablaAndDua(tipoOpe, codPais, ref tabla, ref dua);

            Session["SqlFiltro"] = GeneraSqlFiltro(codPais, tipoOpe, tipoBusqueda, desde, hasta, dua, codPais2.ToString());
            string sqlAntes = Session["UltSqlFiltro"] as string??"";
            /*if(Session["UltSqlFiltro"] != null)
            {
                 sqlAntes = Session["UltSqlFiltro"].ToString();
            }*/
                
            string sql = Session["SqlFiltro"].ToString();

            string lblResultado = "";
            object objMensaje = null;

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

                return Json(new
                {
                    objMensaje

                });
            }

            else
            {
                Session["UltSqlFiltro"] = Session["SqlFiltro"].ToString();
            }
            string cifTot = GetCIFTot(codPais, tipoOpe,searchBD:false);
            string pesoNeto = Funciones.CampoPeso(codPais, tipoOpe);
            DataRow drTotales =
                FuncionesBusiness.CalculaTotalesFreemium(Session["SqlFiltro"].ToString(), cifTot, codPais, pesoNeto, tabla);

            int cantReg = drTotales.GetValue<int>("CantReg");
            bool hideTabExcel = (cantReg > CantRegMax && !FlagPalabras);
            string valueCifTot = "", unidad ="";
            decimal valuePesoNeto =0;
            List<object> listGridData = null;
            if (cantReg > 0)
            {
                valueCifTot = drTotales.GetValue<decimal>(cifTot).ToString();
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

            if (cantReg == 0)
            {
                if (Session["UltSqlFiltro"] == null ||
                    Session["UltSqlFiltro"].ToString() != Session["SqlFiltro"].ToString())
                {
                    Session["UltSqlFiltro"] = Session["SqlFiltro"].ToString();

                    string mensaje =
                        "Su búsqueda no encontró resultados.Le sugerimos que cambie el rango de fechas y/o modifique sus filtros de búsqueda";

                    if (idioma == "en")
                        mensaje =
                            "Your search found no results.We suggest to change dates range and/or modify search filters";

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

                return Json(new
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
                //string mensaje = "Su búsqueda supera el límite de " + CantRegMax.ToString("n0") +
                //                 " registros y no puede ser descargada en Excel. ";
                //mensaje +=
                //    "Si desea ver todas las pestañas habilitadas y/ó descargar en Excel, reduzca el rango de fechas y/o modifique sus filtros de búsqueda";

                //if (idioma == "en")
                //{
                //    mensaje = "Your search exceeds " + CantRegMax.ToString("n0") +
                //              " records limit and it can not be download to Excel. ";
                //    mensaje +=
                //        "If you want to see all tabs enabled and/or download to Excel, reduce the dates range and/or modify your filters search";
                //}

                //objMensaje = new
                //{
                //    titulo = Resources.Resources.Search_Button,
                //    mensaje,
                //    flagContactenos = false,
                //};

                //Session["SqlFiltro"] = GeneraSqlFiltro(codPais, codPais2, tipoOpe, anioMesIni, anioMesFin,
                //    indexCboPaisB, codPaisB, dua);

                //drTotales = FuncionesBusiness.CalculaTotalesFreemium(Session["SqlFiltro"].ToString(), cifTot, codPais, pesoNeto,
                //    tabla);



                //cantReg = Convert.ToInt32(drTotales["CantReg"]);

                //Session["hdfCantReg"] = drTotales["CantReg"].ToString();


                //Session["hdfCIFTot"] = drTotales[cifTot].ToString();

                


                FuncionesBusiness.RegistraConsumo(Session["IdUsuarioFree"].ToString(), codPais, tipoOpe, "Mis Busquedas",
                    Session["SqlFiltro"].ToString());

                lblResultado = idioma == "es" ? $"Se encontraron {cantReg:n0} registros" : $"{cantReg:n0} records were found";

                /*if (idioma == "es")
                {
                    lblResultado = "Se encontraron " + cantReg.ToString("n0") + " registros";
                }
                else

                {
                    lblResultado = cantReg.ToString("n0") + " records were found";
                }*/


                //falta completar
                //listGridData = Busca(codPais, tipoOpe, cifTot, pesoNeto, idioma, dua, tabla, codPais2, valueCifTot,
                //    valuePesoNeto, unidad, cantReg, specificCulture, hideTabExcel);
                listGridData = Busca(codPais, tipoOpe, cifTot, pesoNeto, idioma, dua, tabla, codPais2, valueCifTot,
                    valuePesoNeto, unidad, cantReg, specificCulture, false);
            }
            else

            {
                listGridData = Busca(codPais, tipoOpe, cifTot, pesoNeto, idioma, dua, tabla, codPais2, valueCifTot,
                    valuePesoNeto, unidad, cantReg, specificCulture, false);
            }

            if (listGridData != null)
            {
                Session["BotonUtilizado"] = true;
            }

            return Json(new
            {
                objMensaje,
                totalRecordsFound = GetDescriptionRecordsFound(cantReg, specificCulture, idioma), //cantReg.ToString("n0", specificCulture),
                gridData = listGridData,
                hideTabExcel,
            });

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

        private List<object> Busca(string codPais, string tipoOpe, string cifTot, string pesoNeto,
            string idioma, string dua, string tabla,
            int indexCboPaisB, string valueCifTot, decimal valuePesoNeto,
            string unidad, int totalRegistros, CultureInfo specificCulture, bool hideTabExcel)
        {
            //string sessionSqlFiltro = Session["SqlFiltro"].ToString();

            //var flags = new TabMisBusquedas()
            //{
            //    ExistePartida = true,
            //    ExisteMarcasModelos = false,
            //    ExisteImportador = tipoOpe == Enums.TipoOpe.I.ToString(),
            //    ExisteNotificado = false,
            //    ExisteProveedor = false,
            //    ExistePaisOrigen = tipoOpe == Enums.TipoOpe.I.ToString(),
            //    ExisteExportador = tipoOpe == Enums.TipoOpe.E.ToString(),
            //    ExisteImportadorExp = false,
            //    ExistePaisDestino = tipoOpe == Enums.TipoOpe.E.ToString(),
            //    ExisteViaTransp = true,
            //    ExisteAduana = false,
            //    ExisteDistrito = false,
            //    ExistePtoDescarga = false,
            //    ExistePtoEmbarque = false,
            //    ExistePtoDestino = false,
            //    ExisteManifiesto =false
            //};   
            var flags = new TabMisBusquedas(tipoOpe, codPais);

            

            flags.ExisteAduana = false;
            flags.ExisteDUA = false;
            flags.ExisteDesComercial = false;
            flags.ExisteImportadorExp = false;
            flags.ExisteMarcasModelos = false;
            flags.ExisteProveedor = false;
            

            FuncionesBusiness.RegistraConsumo(Session["IdUsuarioFree"].ToString(), codPais, tipoOpe, "Freemium",
                Session["SqlFiltro"].ToString());


            var json = new List<object>();


            string totalRegistrosFormateado = totalRegistros.ToString("n0", specificCulture);



            string cifTotFormateado = Convert.ToDecimal(valueCifTot).ToString("n" + (!flags.IsManifiesto ? "0" : "1"), specificCulture);
            string pesoNetoFormateado = valuePesoNeto.ToString("n0", specificCulture);


            if (flags.ExistePartida)
                json.Add(GetDataObjectByFilter(Enums.Filtro.Partida.ToString(), codPais, tipoOpe, cifTot, pesoNeto, dua, tabla,
                    valueCifTot, indexCboPaisB, totalRegistrosFormateado, cifTotFormateado, pesoNetoFormateado,
                    specificCulture, idioma, unidad, false, hideTabExcel: hideTabExcel));

            if (/*codPais == "PEB" && !flags.IsManifiesto*/ flags.ExisteMarcasModelos)
            {
                json.Add(GetDataObjectByFilter(Enums.Filtro.Marca.ToString(), codPais, tipoOpe, cifTot, pesoNeto, dua, tabla,
                    valueCifTot, indexCboPaisB, totalRegistrosFormateado, cifTotFormateado, pesoNetoFormateado,
                    specificCulture, idioma, unidad, false, hideTabExcel: hideTabExcel));

                json.Add(GetDataObjectByFilter(Enums.Filtro.Modelo.ToString(), codPais, tipoOpe, cifTot, pesoNeto, dua, tabla,
                    valueCifTot, indexCboPaisB, totalRegistrosFormateado, cifTotFormateado, pesoNetoFormateado,
                    specificCulture, idioma, unidad, false, hideTabExcel: hideTabExcel));
            }

            if (tipoOpe == Enums.TipoOpe.I.ToString())
            {
                if (flags.ExisteImportador)
                {
                    json.Add(GetDataObjectByFilter(Enums.Filtro.Importador.ToString(), codPais, tipoOpe, cifTot, pesoNeto, dua, tabla,
                        valueCifTot, indexCboPaisB, totalRegistrosFormateado, cifTotFormateado, pesoNetoFormateado,
                        specificCulture, idioma, unidad, hideTabExcel: hideTabExcel));
                }

                if (flags.ExisteNotificado)
                {
                    json.Add(GetDataObjectByFilter(Enums.Filtro.Notificado.ToString(), codPais, tipoOpe, cifTot, pesoNeto, dua, tabla,
                        valueCifTot, indexCboPaisB, totalRegistrosFormateado, cifTotFormateado, pesoNetoFormateado,
                        specificCulture, idioma, unidad, hideTabExcel: hideTabExcel));
                }

                if (flags.ExisteProveedor)
                {
                    json.Add(GetDataObjectByFilter(Enums.Filtro.Proveedor.ToString(), codPais, tipoOpe, cifTot, pesoNeto, dua, tabla, valueCifTot,
                      indexCboPaisB, totalRegistrosFormateado, cifTotFormateado, pesoNetoFormateado, specificCulture,
                      idioma, unidad, hideTabExcel: hideTabExcel));
                }

                if (flags.ExistePaisOrigen)
                {
                    json.Add(GetDataObjectByFilter(!flags.IsManifiesto ? Enums.Filtro.PaisOrigen.ToString() : Enums.Filtro.PaisEmbarque.ToString(), codPais, tipoOpe, cifTot, pesoNeto, dua, tabla,
                       valueCifTot, indexCboPaisB, totalRegistrosFormateado, cifTotFormateado, pesoNetoFormateado,
                       specificCulture, idioma, unidad, hideTabExcel: hideTabExcel));
                }
            }
            else
            {
                if (flags.ExisteExportador)
                {
                    json.Add(GetDataObjectByFilter(Enums.Filtro.Exportador.ToString(), codPais, tipoOpe, cifTot, pesoNeto, dua, tabla,
                    valueCifTot, indexCboPaisB, totalRegistrosFormateado, cifTotFormateado, pesoNetoFormateado,
                    specificCulture, idioma, unidad, hideTabExcel: hideTabExcel));
                }

                if (flags.ExisteImportadorExp)
                {
                    json.Add(GetDataObjectByFilter(Enums.Filtro.ImportadorExp.ToString(), codPais, tipoOpe, cifTot, pesoNeto, dua, tabla,
                    valueCifTot, indexCboPaisB, totalRegistrosFormateado, cifTotFormateado, pesoNetoFormateado,
                    specificCulture, idioma, unidad, hideTabExcel: hideTabExcel));
                }

                if (flags.ExistePaisDestino)
                {
                    json.Add(GetDataObjectByFilter(Enums.Filtro.PaisDestino.ToString(), codPais, tipoOpe, cifTot, pesoNeto, dua, tabla,
                    valueCifTot, indexCboPaisB, totalRegistrosFormateado, cifTotFormateado, pesoNetoFormateado,
                    specificCulture, idioma, unidad, hideTabExcel: hideTabExcel));
                }
            }

            //ExisteViaTransp
            if (flags.ExisteViaTransp)
            {
                json.Add(GetDataObjectByFilter(Enums.Filtro.ViaTransp.ToString(), codPais, tipoOpe, cifTot, pesoNeto, dua, tabla,
                    valueCifTot, indexCboPaisB, totalRegistrosFormateado, cifTotFormateado, pesoNetoFormateado,
                    specificCulture, idioma, unidad,
                    false, hideTabExcel: hideTabExcel));

            }
            if (flags.ExisteAduana)
            {
                if (flags.ExisteDUA)
                {
                    json.Add(GetDataObjectAduanaDua(Enums.Filtro.AduanaDUA.ToString(), codPais, cifTot, pesoNeto, dua, tabla, valueCifTot, indexCboPaisB, totalRegistrosFormateado, cifTotFormateado, specificCulture, idioma, hideTabExcel: hideTabExcel));
                }
                else
                {
                    json.Add(GetDataObjectAduanaDua(Enums.Filtro.Aduana.ToString(), codPais, cifTot, pesoNeto, dua, tabla, valueCifTot, indexCboPaisB, totalRegistrosFormateado, cifTotFormateado, specificCulture, idioma, hideTabExcel: hideTabExcel));
                }
            }

            if (flags.ExisteDistrito)
            {
                json.Add(GetDataObjectByFilter(Enums.Filtro.Distrito.ToString(), codPais, tipoOpe, cifTot, pesoNeto, dua, tabla,
                    valueCifTot, indexCboPaisB, totalRegistrosFormateado, cifTotFormateado, pesoNetoFormateado,
                    specificCulture, idioma, unidad,
                    false, hideTabExcel: hideTabExcel));

            }

            if (flags.ExistePtoDescarga || flags.ExistePtoEmbarque || flags.ExistePtoDestino)
            {
                json.Add(GetDataObjectByFilter(
                        flags.ExistePtoDescarga ? Enums.Filtro.PtoDescarga.ToString() :
                        flags.ExistePtoEmbarque ? Enums.Filtro.PtoEmbarque.ToString() :
                        Enums.Filtro.PtoDestino.ToString(),
                        codPais, tipoOpe, cifTot, pesoNeto, dua, tabla,
                    valueCifTot, indexCboPaisB, totalRegistrosFormateado, cifTotFormateado, pesoNetoFormateado,
                    specificCulture, idioma, unidad,
                    false, hideTabExcel: hideTabExcel));

            }

            if (flags.ExisteManifiesto)
                json.Add(GetDataObjectByFilter(Enums.Filtro.Manifiesto.ToString(), codPais, tipoOpe, cifTot, pesoNeto, dua, tabla,
                    valueCifTot, indexCboPaisB, totalRegistrosFormateado, cifTotFormateado, pesoNetoFormateado,
                    specificCulture, idioma, unidad, false, hideTabExcel: hideTabExcel));

            return json;
        }

        private object GetDataObjectByFilter(string filtro, string codPais, string tipoOpe,
            string cifTot, string pesoNeto, string dua,
            string tabla, string valueCifTot, int indexCboPaisB,
            string totalRegistrosFormateado, string cifTotFormateado, string pesoNetoFormateado,
            CultureInfo specificCulture, string idioma, string unidad,
            bool validaExisteFiltro = true, bool hideTabExcel = false)
        {

            var isManif = IsManifiesto(codPais);
            object obj = new
            {
                tabDataName = filtro,
                tabDataNumRows = 0
            };

            if (validaExisteFiltro && !isManif)
            {
                if (Funciones.ExisteVariable(codPais, tipoOpe, "Id" + filtro))
                {
                    var cuenta = CuentaAgrupado(filtro, tabla, dua);
                    if (cuenta > 0)
                    {
                        var listDataTable = CargaFiltro(filtro, -1, codPais, cifTot, pesoNeto, idioma, dua, tabla,
                            indexCboPaisB, Enums.TipoFiltro.Todos, hideTabExcel: hideTabExcel);

                        TabData objData = GetTabDataByFilter(filtro, cifTot, cuenta, pesoNeto, unidad, codPais);
                        objData.CodPais = codPais;
                        objData.TotalRegistros = totalRegistrosFormateado;
                        objData.CiFoFobTotal = cifTotFormateado;
                        objData.PesoNeto = pesoNetoFormateado;
                        objData.ListRows =
                            GetDataTableToListGridRow(listDataTable[0], filtro, cifTot, valueCifTot, codPais,
                                specificCulture);

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
                                RenderViewToString(ControllerContext, "GridViews/ResumenGridView", objData),

                            resumenTotalPages = objData.TotalPaginasResumen,
                            tabDataList = RenderViewToString(ControllerContext, "GridViews/TabGridView", objData),
                            tabTotalPages = objData.TotalPaginasTab,
                            pieTitle = objData.TituloTab,
                            tabPieData = GetJsonDataPie(filtro, listDataTable[1], cifTot, valueCifTot, specificCulture)
                        };
                    }
                }
            }
            else
            {
                var cuenta = CuentaAgrupado(filtro, tabla, dua);
                if (cuenta > 0)
                {

                    var listDataTable = CargaFiltro(filtro, -1, codPais, cifTot, pesoNeto, idioma, dua, tabla,
                        indexCboPaisB, Enums.TipoFiltro.Todos, hideTabExcel: hideTabExcel);
                    TabData objData = GetTabDataByFilter(filtro, cifTot, cuenta, pesoNeto, unidad, codPais);
                    objData.CodPais = codPais;
                    objData.TotalRegistros = totalRegistrosFormateado;
                    objData.CiFoFobTotal = cifTotFormateado;
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
                            tabDataList = RenderViewToString(ControllerContext, "GridViews/TabGridView", objData),
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
                                specificCulture);
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
                                RenderViewToString(ControllerContext, "GridViews/ResumenGridView", objData),
                            resumenTotalPages = objData.TotalPaginasResumen,
                            tabDataList = RenderViewToString(ControllerContext, "GridViews/TabGridView", objData),
                            tabTotalPages = objData.TotalPaginasTab,
                            pieTitle = objData.TituloTab,
                            tabPieData = GetJsonDataPie((filtro == "Partida" ? "Nandina" : filtro), listDataTable[1], cifTot, valueCifTot, specificCulture)
                        };
                    }
                }
            }
            return obj;
        }

        private List<GridRow> GetListGridRowAduanaDua(DataTable dt, string filtro, string cifTot, CultureInfo specificCulture)
        {
            var lista = new List<GridRow>();
            Int64 cantReg;
            if (filtro == Enums.Filtro.AduanaDUA.ToString())
            {
                foreach (DataRow dr in dt.Rows)
                {
                    cantReg = Convert.ToInt64(dr["CantReg"]);
                    lista.Add(new GridRow()
                    {
                        Id = dr["IdAduana"]+ "-" + dr["DUA"],
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

        private object GetDataObjectAduanaDua(string filtro, string codPais, string cifTot,
            string pesoNeto, string dua, string tabla,
            string valueCifTot, int indexCboPaisB, string totalRegistrosFormateado,
            string cifTotFormateado, CultureInfo specificCulture, string idioma, bool hideTabExcel = false)
        {
            object obj = new
            {
                tabDataName = filtro,
                tabDataNumRows = 0
            };
            var cuenta = CuentaAgrupado(filtro, tabla, dua);
            if (cuenta > 0)
            {
                var listDataTable = CargaFiltro(filtro, -1, codPais, cifTot, pesoNeto, idioma, dua, tabla, indexCboPaisB, Enums.TipoFiltro.Todos, hideTabExcel: hideTabExcel);
                TabData objData = GetTabDataByFilter(filtro, cifTot, cuenta, "", "");
                objData.CodPais = codPais;
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
                    tabDataList = RenderViewToString(ControllerContext, "GridViews/AduanaDuaGridView", objData),
                    tabTotalPages = objData.TotalPaginasTab,
                    pieTitle = Resources.Resources.Demo_Aduanas_Tab,
                    tabPieData = GetJsonDataPie(Enums.Filtro.Aduana.ToString(), (filtro == Enums.Filtro.Aduana.ToString() ? listDataTable[0] : listDataTable[1]), cifTot, valueCifTot, specificCulture)
                };

            }
            return obj;
        }

        private List<GridRow> GetDataTableToListCbo(DataTable dt, string filtro)
        {
            var lista = new List<GridRow>();
            foreach (DataRow dtRow in dt.Rows)

            {
                lista.Add(new GridRow()
                {
                    Id = dtRow.GetValue<string>((filtro != "Manifiesto" ? "Id" : "") + filtro),
                    Descripcion = dtRow[filtro].ToString()
                });
            }

            return lista;
        }

        private List<object> GetJsonDataPie(string filtro, DataTable dt, string cifTot,
            string valueCifTot, CultureInfo specificCulture)
        {
            var json = new List<object>();
            var numSectoresCirculares = 5;
            string auxIdSlice = filtro == "Nandina" ? Enums.Filtro.Partida.ToString() : filtro;

            if (dt!= null && dt.Rows.Count > 0)
            {
                float dValueCifTot = Convert.ToSingle(valueCifTot);

                if (dt.Rows.Count <= numSectoresCirculares)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        json.Add(new
                        {
                            name = dr[filtro].ToString(),
                            y = Math.Round(Convert.ToDouble(dr.GetValue<Single>(cifTot) / dValueCifTot * 100), 1),
                            id = dr.GetValue<string>((filtro != "Manifiesto" ? "Id" : "") + auxIdSlice)
                        });
                    }
                }
                else
                {
                    var dtAux = dt.AsEnumerable().Take(numSectoresCirculares);
                    double sumPercentage = 0;
                    foreach (DataRow dr in dtAux)
                    {
                        var percentage = Math.Round(Convert.ToDouble(dr.GetValue<Single>(cifTot) / dValueCifTot * 100), 1);
                        sumPercentage += percentage;
                        json.Add(new
                        {
                            name = dr[filtro].ToString(),
                            y = percentage,
                            id = dr.GetValue<string>((filtro != "Manifiesto" ? "Id" : "") + auxIdSlice)
                        });
                    }
                    json.Add(new
                    {
                        name = "[" + Resources.Resources.Others_Text.ToUpper() + "]",
                        y = 100 - sumPercentage,
                        id = ""
                    });
                }
            }
            return json;
        }

        private List<GridRow> GetDataTableToListGridRow(DataTable dt, string filtro, string cifTot,
            string valueCifTot, string codPais, CultureInfo specificCulture)
        {
            var lista = new List<GridRow>();
            if (dt == null || dt.Rows.Count == 0) goto ReturnMe;
            decimal floatValueCifTot = Convert.ToDecimal(valueCifTot);
            decimal valRecordCitTot;

            Int64 cantReg;
            if ((filtro == Enums.Filtro.Importador.ToString() || filtro == Enums.Filtro.Exportador.ToString())
                && (codPais == "PE" || codPais == "PEB"))
            {
                string[] planesNoSentinel = { "BUSINESS", "PREMIUM", "UNIVERSIDADES" };

                if (planesNoSentinel.Contains(Session["Plan"].ToString()))
                {
                    //Int64 id;
                    if (dt != null)
                    {
                        foreach (DataRow drRow in dt.Rows)
                        {
                            Int64 id = drRow.GetValue<Int64>("Id" + filtro);
                            valRecordCitTot = drRow.GetValue<decimal>(cifTot);
                            cantReg = Convert.ToInt64(drRow["CantReg"]);
                            lista.Add(new GridRow()
                            {
                                Id = id.ToString(),
                                Descripcion = drRow[filtro].ToString(),
                                TotalReg = cantReg.ToString("n0", specificCulture),
                                CiFoFobTot = valRecordCitTot.ToString("n0", specificCulture),
                                CiFoFobPor =
                                    (valRecordCitTot / floatValueCifTot * 100).ToString("n2", specificCulture) + "%",
                                Ruc = FuncionesBusiness.BuscaRUC(id.ToString(), "PE"),
                                IsEnabledTotalReg = cantReg <= CantRegMax,
                                IsPlanPermiteSentinel = true,
                                IsVisibleSentinel = false
                            });
                        }
                    }
                        
                }
                else
                {
                    if (dt != null)
                    {
                        foreach (DataRow drRow in dt.Rows)
                        {
                            valRecordCitTot = drRow.GetValue<decimal>(cifTot);
                            cantReg = Convert.ToInt64(drRow["CantReg"]);
                            lista.Add(new GridRow()
                            {
                                Id = drRow.GetValue<string>("Id" + filtro),
                                Descripcion = drRow[filtro].ToString(),
                                TotalReg = cantReg.ToString("n0", specificCulture),
                                CiFoFobTot = valRecordCitTot.ToString("n0", specificCulture),
                                CiFoFobPor =
                                    (valRecordCitTot / floatValueCifTot * 100).ToString("n2", specificCulture) + "%",
                                IsEnabledTotalReg = cantReg <= CantRegMax,
                                IsVisibleSentinel = false,
                                IsPlanPermiteSentinel = false
                            });
                        }
                    }
                }
            }
            else
            {
                var IsManif = IsManifiesto(codPais);
                if (dt != null)
                {
                    foreach (DataRow drRow in dt.Rows)
                    {
                        cantReg = drRow.GetValue<int>("CantReg");
                        valRecordCitTot = drRow.GetValue<decimal>(cifTot);
                        lista.Add(new GridRow()
                        {
                            Id = drRow.GetValue<string>((filtro != "Manifiesto" ? "Id" : "") + filtro),
                            Descripcion = drRow.GetValue<string>(filtro),
                            TotalReg = cantReg.ToString("n0", specificCulture),
                            CiFoFobTot = valRecordCitTot.ToString("n" + (!IsManif ? "0" : "1"), specificCulture),
                            CiFoFobPor = (valRecordCitTot / floatValueCifTot * 100).ToString("n" + (!IsManif ? "2" : "1"), specificCulture) + "%",
                            IsEnabledTotalReg = cantReg <= CantRegMax,
                            IsVisibleSentinel = false
                        });
                    }
                }
            }
            ReturnMe:
            return lista;
        }

        private List<GridRow> GetDataTableToListGridRowTab(DataTable dt, string filtro, string cifTot,
            string valueCifTot, string codPais, CultureInfo specificCulture,
            string pesoNeto, string unidad)
        {
            var lista = new List<GridRow>();

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

            decimal cantidad ;
            decimal valRecordCitTot;

            Int64 cantReg ;
            if ((filtro == "Importador" || filtro == "Exportador")
                && (codPais == "PE" || codPais == "PEB"))
            {
                string[] planesNoSentinel = { "BUSINESS", "PREMIUM", "UNIVERSIDADES" };

                var planPermiteSentinel = planesNoSentinel.Contains(Session["Plan"].ToString());

                if (planPermiteSentinel)
                {
                    Int64 id ;
                    if (existeTotalKgyPrecio)
                    {
                        if(dt != null)
                            foreach (DataRow drRow in dt.Rows)
                            {
                                id = drRow.GetValue<Int64>("Id" + filtro);
                                cantReg = Convert.ToInt64(drRow["CantReg"]);
                                valRecordCitTot = drRow.GetValue<decimal>(cifTot);
                                cantidad = Convert.ToDecimal(drRow[cantidadField]);
                                lista.Add(new GridRow()
                                {
                                    Id = id.ToString(),
                                    Descripcion = drRow[filtro].ToString(),
                                    TotalReg = cantReg.ToString("n0", specificCulture),
                                    CiFoFobTot = valRecordCitTot.ToString("n0", specificCulture),
                                    CiFoFobPor =
                                        (valRecordCitTot / floatValueCifTot * 100).ToString("n2", specificCulture) + "%",
                                    Ruc = FuncionesBusiness.BuscaRUC(id.ToString(), "PE"),
                                    IsEnabledTotalReg = cantReg <= CantRegMax,
                                    IsVisibleSentinel = false,
                                    IsPlanPermiteSentinel = true,
                                    TotalKg = cantidad.ToString("n0", specificCulture),
                                    Precio = cantidad > 0 ? (valRecordCitTot / cantidad).ToString("n3", specificCulture) : "0"
                                });
                            }
                    }
                    else
                    {
                        if (dt != null)
                            foreach (DataRow drRow in dt.Rows)
                            {
                                id = drRow.GetValue<Int64>("Id" + filtro);
                                cantReg = Convert.ToInt64(drRow["CantReg"]);
                                valRecordCitTot = drRow.GetValue<decimal>(cifTot);
                                lista.Add(new GridRow()
                                {
                                    Id = id.ToString(),
                                    Descripcion = drRow[filtro].ToString(),
                                    TotalReg = cantReg.ToString("n0", specificCulture),
                                    CiFoFobTot = valRecordCitTot.ToString("n0", specificCulture),
                                    CiFoFobPor =
                                        (valRecordCitTot / floatValueCifTot * 100).ToString("n2", specificCulture) + "%",
                                    Ruc = FuncionesBusiness.BuscaRUC(id.ToString(), "PE"),
                                    IsEnabledTotalReg = cantReg <= CantRegMax,
                                    IsVisibleSentinel = false,
                                    IsPlanPermiteSentinel = true
                                });
                            }
                    }
                }
                else
                {
                    if (existeTotalKgyPrecio)
                    {
                        if (dt != null)
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
                                    IsVisibleSentinel = false,
                                    IsPlanPermiteSentinel = false,
                                    TotalKg = cantidad.ToString("n0", specificCulture),
                                    Precio = cantidad > 0 ? (valRecordCitTot / cantidad).ToString("n3", specificCulture) : "0"
                                });
                            }
                    }
                    else
                    {
                        if (dt != null)
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
                                    IsVisibleSentinel = false,
                                    IsPlanPermiteSentinel = false
                                });
                            }
                    }
                }
            }
            else if (filtro == "DetalleExcel")
            {

            }
            else
            {
                if (existeTotalKgyPrecio)
                {
                    if (dt != null)
                        foreach (DataRow drRow in dt.Rows)
                        {
                            cantReg = Convert.ToInt64(drRow["CantReg"]);
                            cantidad = drRow.GetValue<decimal>(cantidadField);
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
                                IsVisibleSentinel = false,
                                TotalKg = cantidad.ToString("n0", specificCulture),
                                Precio = cantidad > 0 ? (valRecordCitTot / cantidad).ToString("n3", specificCulture) : "0"
                            });
                        }
                }
                else
                {
                    var IsManif = IsManifiesto(codPais);
                    if (dt != null)
                        foreach (DataRow drRow in dt.Rows)
                        {
                            cantReg = Convert.ToInt64(drRow["CantReg"]);
                            valRecordCitTot = drRow.GetValue<decimal>(cifTot);
                            lista.Add(new GridRow()
                            {
                                Id = drRow.GetValue<string>((filtro != "Manifiesto" ? "Id" : "") + filtro),
                                Descripcion = drRow[filtro].ToString(),
                                TotalReg = cantReg.ToString("n0", specificCulture),
                                CiFoFobTot = valRecordCitTot.ToString("n" + (!IsManif ? "0" : "1"), specificCulture),
                                CiFoFobPor = (valRecordCitTot / floatValueCifTot * 100).ToString("n" + (!IsManif ? "2" : "1"), specificCulture) + "%",

                                IsEnabledTotalReg = cantReg <= CantRegMax,
                                IsVisibleSentinel = false
                            });
                        }
                }
            }
            return lista;
        }

        private TabData GetTabDataByFilter(string filtro, string cifTot, int totalRegistrosFiltro,
            string pesoNeto, string unidad, string codPais = "")
        {
            var IsManif = IsManifiesto(codPais);
            TabData objData = new TabData
            {
                Filtro = filtro,
                CodPais = codPais,
                AddToFiltersAndSearchButton = Resources.Resources.Add_Filter_Search_Button,
                TotalPaginasResumen =
                    (int) Math.Ceiling(Convert.ToDecimal(totalRegistrosFiltro) / ResumenTabsGridPageSize),
                TotalPaginasTab = (int) Math.Ceiling(Convert.ToDecimal(totalRegistrosFiltro) / TabGridPageSize),
                GridHead = {CiFoFobPor = "%"}
            };


            //var lenCifTot = cifTot.Length;

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
                objData.GridHead.CiFoFobTot = cifTot + " Tn";

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
                    objData.GridHead.Descripcion = !IsManif ? Resources.Resources.Demo_Importers_Tab : Resources.Resources.Demo_Importers_Tab_Manif;
                    objData.AddMyFavouritesButton = Resources.AdminResources.Add_ToMyImporters_Button;
                    objData.TituloTab = objData.GridHead.Descripcion; //Resources.Resources.Demo_Importers_Tab;
                    break;
                case "Manifiesto":
                case "Notificado":
                    objData.GridHead.Descripcion = filtro == "Notificado" ? Resources.Resources.Demo_Notif_Tab : Resources.Resources.Demo_Manifiesto_Tab;
                    objData.TituloTab = objData.GridHead.Descripcion; //Resources.Resources.Demo_Importers_Tab;
                    break;
                case "Proveedor":
                    if (codPais != "CL")
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
                    objData.GridHead.Descripcion = !IsManif ? Resources.Resources.Demo_Exporters_Tab : Resources.Resources.Demo_Exporters_Tab_Manif;
                    objData.AddMyFavouritesButton = Resources.AdminResources.Add_ToMyExporters_Button;
                    objData.TituloTab = objData.GridHead.Descripcion; //Resources.Resources.Demo_Exporters_Tab;
                    break;
                case "PaisEmbarque":
                case "PaisOrigen":
                    objData.GridHead.Descripcion = !IsManif ? Resources.Resources.Demo_OriginCountries_Tab : Resources.Resources.Ult_Paises_Embarque;
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
                    objData.GridHead.Descripcion = filtro == "PtoDestino" ? Resources.Resources.Demo_PtoDestino_Tab :
                                                   filtro == "PtoEmbarque" ? Resources.Resources.Demo_PtoEmbarque_Tab :
                                                          Resources.Resources.Demo_PtoDescarga_Tab;
                    objData.TituloTab = objData.GridHead.Descripcion; //Resources.Resources.Demo_Vias_Tab;
                    break;
            }

            return objData;
        }

        private List<DataTable> GetDataTables(string filtro, string sqlFiltro, int pagina,
           Enums.TipoFiltro tipoFiltro, bool hideTabExcel = false)
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
                    switch (tipoFiltro)
                    {
                        case Enums.TipoFiltro.Todos:
                            if (!hideTabExcel)
                                lista.Add(FuncionesBusiness.Lista(sqlFiltro, (pagina == -1 ? 1 : pagina),
                                    ResumenTabsGridPageSize));
                            else
                                lista.Add(new DataTable());

                            lista.Add(FuncionesBusiness.Lista(sqlFiltro, (pagina == -1 ? 1 : pagina), TabGridPageSize));
                            if (Session["dt" + filtro + "2"] != null && Convert.ToBoolean(Session["dt" + filtro + "2"]))
                                lista.Add(FuncionesBusiness.Lista2(Session["sql" + filtro + "O"].ToString()));
                            break;
                        case Enums.TipoFiltro.Resumen:
                            lista.Add(FuncionesBusiness.Lista(sqlFiltro, (pagina == -1 ? 1 : pagina),
                                ResumenTabsGridPageSize));
                            break;
                        case Enums.TipoFiltro.Tab:
                            lista.Add(FuncionesBusiness.Lista(sqlFiltro, (pagina == -1 ? 1 : pagina), TabGridPageSize));
                            break;
                    }
                    break;
                case "Marca":
                case "Modelo":
                    lista.Add(FuncionesBusiness.Lista(sqlFiltro, (pagina == -1 ? 1 : pagina), TabGridPageSize));
                    break;
                case "AduanaDUA":
                    if (tipoFiltro == Enums.TipoFiltro.Todos)
                    {
                        lista.Add(FuncionesBusiness.Lista(sqlFiltro, (pagina == -1 ? 1 : pagina), TabGridPageSize));
                        lista.Add(FuncionesBusiness.Lista(Session["sql" + Enums.Filtro.Aduana.ToString()].ToString(), 1, TabGridPageSize));
                        if (Session["dt" + Enums.Filtro.Aduana.ToString() + "2"] != null
                            && Convert.ToBoolean(Session["dt" + Enums.Filtro.Aduana.ToString() + "2"]))
                        {
                            lista.Add(FuncionesBusiness.Lista2(Session["sqlAduanaO"].ToString()));
                        }
                    }
                    else
                    {
                        lista.Add(FuncionesBusiness.Lista(sqlFiltro, (pagina == -1 ? 1 : pagina), TabGridPageSize));
                    }
                    break;
                case "Aduana":
                    if (tipoFiltro == Enums.TipoFiltro.Todos)
                    {
                        lista.Add(FuncionesBusiness.Lista(sqlFiltro, (pagina == -1 ? 1 : pagina), TabGridPageSize));
                        if (Session["dt" + filtro + "2"] != null && Convert.ToBoolean(Session["dt" + filtro + "2"]))
                            lista.Add(FuncionesBusiness.Lista2(Session["sql" + filtro + "O"].ToString()));
                    }
                    else
                    {
                        lista.Add(FuncionesBusiness.Lista(sqlFiltro, (pagina == -1 ? 1 : pagina), TabGridPageSize));
                    }
                    break;
            }
            return lista;
        }

        private List<DataTable> CargaFiltro(string filtro, int pagina, string codPais,
            string cifTot, string pesoNeto, string idioma,
            string dua, string tabla, int indexCboPaisB,
            Enums.TipoFiltro tipoFiltro, string orden = "", string addExtraFiltro = "", bool hideTabExcel = false)

        {
            string sqlFiltro ;
            

            if (pagina == -1)
            {
                string sqlFiltroO;
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
            return GetDataTables(filtro, sqlFiltro, pagina, tipoFiltro, hideTabExcel: hideTabExcel);
        }

        string GeneraSqlAgrupado(string filtro, string codPais, string cifTot,
            string pesoNeto, string idioma, string dua,
            string tabla, string orden = "")
        {
            bool isManif = IsManifiesto(codPais);
            string sql = "";

            if (!string.IsNullOrWhiteSpace(Session["hdfPalabrasY"]?.ToString()) && !isManif)
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
                sql += "from Partida_" + codPais + " P, (select IdPartida, SUM(Registros) AS CantReg, sum(" + cifTot1 +
                       ") as " + cifTot + ", sum(" + pesoNeto1 + ") as " + pesoNeto2 + ", sum(Cantidad) as Cantidad ";
            }
            else if (filtro == "Marca")
            {
                sql = "select T.IdMarca, Marca, CantReg, FOBTot, PesoNeto, Cantidad, ROW_NUMBER() over (order by " +
                      orden + ") as Nro ";
                sql +=
                    "from Marca_PEB M, (select IdMarca, SUM(Registros) AS CantReg, sum(FOBTot) as FOBTot, sum(0) as PesoNeto, sum(Cantidad) as Cantidad ";
            }
            else if (filtro == "Modelo")

            {
                sql =
                    "select T.IdModelo, Marca + ' - ' + Modelo as Modelo, CantReg, FOBTot, PesoNeto, Cantidad, ROW_NUMBER() over (order by " +
                    orden + ") as Nro ";
                sql +=
                    "from Modelo_PEB M, (select IdModelo, SUM(Registros) AS CantReg, sum(FOBTot) as FOBTot, sum(0) as PesoNeto, sum(Cantidad) as Cantidad ";
            }
            else if (filtro == "Importador")
            {
                if (!isManif)
                {
                    //sql = "select IdImportador, Empresa as Importador, CantReg, " + cifTot + ", " + pesoNeto2 +
                    //      ", Cantidad, ROW_NUMBER() over (order by " + orden + ") as Nro ";

                    sql = "select IdImportador, Empresa as Importador, CantReg, " + cifTot + ", " + pesoNeto2 +
                          ", Cantidad, ROW_NUMBER() over (order by " + (orden == filtro ? "Empresa" : orden) + ") as Nro ";

                    sql += "from Empresa_" + codPais + " E, (select IdImportador, SUM(Registros) AS CantReg, sum(" + cifTot1 +
                           ") as " + cifTot + ", sum(" + pesoNeto1 + ") as " + pesoNeto2 + ", sum(Cantidad) as Cantidad ";
                }
                else
                {
                    sql = "select IdImportador, Empresa as Importador, CantReg, " + cifTot +
                          ", ROW_NUMBER() over (order by " + orden + ") as Nro ";
                    sql += "from Empresa_" + codPais + " E, (select IdImportador, SUM(Registros) AS CantReg, sum(" + cifTot +
                           ") / 1000 as " + cifTot + " ";
                }
            }
            else if (filtro == "Proveedor")
            {
                if (!isManif)
                {
                    sql = "select T.IdProveedor, Proveedor, CantReg, " + cifTot + ", " + pesoNeto2 +
                          ", Cantidad, ROW_NUMBER() over (order by " + orden + ") as Nro ";

                    sql += "from Proveedor_" + codPais + " P, (select IdProveedor, SUM(Registros) AS CantReg, sum(" + cifTot1 +
                           ") as " + cifTot + ", sum(" + pesoNeto1 + ") as " + pesoNeto2 + ", sum(Cantidad) as Cantidad ";
                }
                else
                {
                    sql = "select T.IdProveedor, Proveedor, CantReg, " + cifTot + ", ROW_NUMBER() over (order by " + orden +
                          ") as Nro ";
                    sql += "from Proveedor_" + codPais + " P, (select IdProveedor, SUM(Registros) AS CantReg, sum(" + cifTot +
                           ") / 1000 as " + cifTot + " ";
                }
            }
            else if (filtro == "Exportador")
            {
                if (!isManif)
                {
                    sql = "select IdExportador, Empresa as Exportador, CantReg, " + cifTot + ", " + pesoNeto2 +
                          ", Cantidad, ROW_NUMBER() over (order by " + orden + ") as Nro ";
                    sql += "from Empresa_" + codPais + " E, (select IdExportador, SUM(Registros) AS CantReg, sum(" + cifTot1 +
                           ") as " + cifTot + ", sum(" + pesoNeto1 + ") as " + pesoNeto2 + ", sum(Cantidad) as Cantidad ";
                }
                else
                {
                    sql = "select IdExportador, Empresa as Exportador, CantReg, " + cifTot +
                          ", ROW_NUMBER() over (order by " + orden + ") as Nro ";
                    sql += "from Empresa_" + codPais + " E, (select IdExportador, SUM(Registros) AS CantReg, sum(" + cifTot +
                           ") / 1000 as " + cifTot + " ";
                }
            }
            else if (filtro == "ImportadorExp")
            {
                if (!isManif)
                {
                    sql = "select T.IdImportadorExp, ImportadorExp, CantReg, " + cifTot + ", " + pesoNeto2 +
                          ", Cantidad, ROW_NUMBER() over (order by " + orden + ") as Nro ";
                    sql += "from ImportadorExp_" + codPais + " I, (select IdImportadorExp, SUM(Registros) AS CantReg, sum(" +
                           cifTot1 + ") as " + cifTot + ", sum(" + pesoNeto1 + ") as " + pesoNeto2 +
                           ", sum(Cantidad) as Cantidad ";
                }
                else
                {
                    sql = "select T.IdImportadorExp, ImportadorExp, CantReg, " + cifTot + ", ROW_NUMBER() over (order by " +
                          orden + ") as Nro ";
                    sql += "from ImportadorExp_" + codPais + " I, (select IdImportadorExp, SUM(Registros) AS CantReg, sum(" +
                           cifTot + ") / 1000 as " + cifTot + " ";
                }
            }
            else if (filtro == "PaisOrigen")
            {
                sql = "select IdPaisOrigen, Pais as PaisOrigen, CantReg, " + cifTot + ", " + pesoNeto2 +
                      ", Cantidad, ROW_NUMBER() over (order by " + orden + ") as Nro ";
                sql += "from Pais_" + codPais + " P, (select IdPaisOrigen, SUM(Registros) AS CantReg, sum(" + cifTot1 +
                       ") as " + cifTot + ", sum(" + pesoNeto1 + ") as " + pesoNeto2 + ", sum(Cantidad) as Cantidad ";
            }
            else if (filtro == "PaisEmbarque")
            {
                sql = "select IdPaisEmbarque, Pais as PaisEmbarque, CantReg, " + cifTot +
                      ", ROW_NUMBER() over (order by " + orden + ") as Nro ";
                sql += "from Pais_" + codPais + " P, (select IdPaisEmbarque, SUM(Registros) AS CantReg, sum(" + cifTot +
                       ") / 1000 as " + cifTot + " ";
            }
            else if (filtro == "PaisDestino")
            {
                if (!isManif)
                {
                    sql = "select IdPaisDestino, Pais as PaisDestino, CantReg, " + cifTot + ", " + pesoNeto2 +
                          ", Cantidad, ROW_NUMBER() over (order by " + orden + ") as Nro ";
                    sql += "from Pais_" + codPais + " P, (select IdPaisDestino, SUM(Registros) AS CantReg, sum(" + cifTot1 +
                           ") as " + cifTot + ", sum(" + pesoNeto1 + ") as " + pesoNeto2 + ", sum(Cantidad) as Cantidad ";
                }
                else
                {
                    sql = "select IdPaisDestino, Pais as PaisDestino, CantReg, " + cifTot +
                          ", ROW_NUMBER() over (order by " + orden + ") as Nro ";
                    sql += "from Pais_" + codPais + " P, (select IdPaisDestino, SUM(Registros) AS CantReg, sum(" + cifTot +
                           ") / 1000 as " + cifTot + " ";
                }

            }
            else if (filtro == "ViaTransp")
            {
                sql = "select T.IdViaTransp, ViaTransp, CantReg, " + cifTot + ", " + pesoNeto2 +
                      ", Cantidad, ROW_NUMBER() over (order by " + orden + ") as Nro ";
                sql += "from ViaTransp_" + codPais + " V, (select IdViaTransp, SUM(Registros) AS CantReg, sum(" + cifTot1 +
                       ") as " + cifTot + ", sum(" + pesoNeto1 + ") as " + pesoNeto2 + ", sum(Cantidad) as Cantidad ";
            }
            else if (filtro == "AduanaDUA")
            {
                sql =
                    "select T.IdAduana, Aduana, DUA, convert(varchar(3), T.IdAduana) + '-' + convert(varchar(20), DUA) as IdAduanaDUA,  Aduana + ' ' + convert(varchar(20), DUA) as AduanaDUA, CantReg, " +
                    cifTot + ", " + pesoNeto2 + ", Cantidad, ROW_NUMBER() over (order by " + orden + ") as Nro ";
                sql += "from Aduana_" + codPais + " A, (select IdAduana, " + dua + " as DUA, ";
                sql += "SUM(Registros) AS CantReg, sum(" + cifTot1 + ") as " + cifTot + ", sum(" + pesoNeto1 + ") as " +
                       pesoNeto2 + ", sum(Cantidad) as Cantidad ";
            }
            else if (filtro == "Aduana")
            {
                sql = "select T.IdAduana, Aduana, CantReg, " + cifTot + ", " + pesoNeto2 +
                      ", Cantidad, ROW_NUMBER() over (order by " + orden + ") as Nro ";
                sql += "from Aduana_" + codPais + " A, (select IdAduana, SUM(Registros) AS CantReg, sum(" + cifTot1 +
                       ") as " + cifTot + ", sum(" + pesoNeto1 + ") as " + pesoNeto2 + ", sum(Cantidad) as Cantidad ";
            }
            else if (filtro == "Distrito")
            {
                sql = "select T.IdDistrito, Distrito, CantReg, " + cifTot + ", " + pesoNeto2 +
                      ", Cantidad, ROW_NUMBER() over (order by " + orden + ") as Nro ";
                sql += "from Distrito_" + codPais + " D, (select IdDistrito, SUM(Registros) AS CantReg, sum(" + cifTot1 +
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
                sql += "from Puerto_" + codPais + " P, (select Id" + filtro + ", SUM(Registros) AS CantReg, sum(" + cifTot +
                       ") / 1000 as " + cifTot + " ";
            }
            else if (filtro == "Manifiesto")
            {
                sql = "select Manifiesto, CantReg, " + cifTot + ", ROW_NUMBER() over (order by " + orden + ") as Nro ";
                sql += "from (select Manifiesto, SUM(Registros) AS CantReg, sum(" + cifTot + ") / 1000 as " + cifTot + " ";
            }
            else if (filtro == "Notificado")
            {
                sql = "select T.IdNotificado, Notificado, CantReg, " + cifTot + ", ROW_NUMBER() over (order by " +
                      orden + ") as Nro ";
                sql += "from Notificado_" + codPais + " N, (select IdNotificado, SUM(Registros) AS CantReg, sum(" + cifTot +
                       ") / 1000 as " + cifTot + " ";
            }


            string tabla1 = tabla;
            if (filtro == "Marca" || filtro == "Modelo")
                tabla1 = "Importacion_PEB";

            sql += "from " + tabla1 + " T where 1 = 1 ";
            sql += Session["SqlFiltro"].ToString();

            if (filtro == "Partida")
                sql += "group by IdPartida) T where P.IdPartida = T.IdPartida ";
            else if (filtro == "Marca")
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
                sql += "group by IdPaisOrigen) T where P.IdPais = T.IdPaisOrigen ";
            else if (filtro == "PaisEmbarque")
                sql += "group by IdPaisEmbarque) T where P.IdPais = T.IdPaisEmbarque ";
            else if (filtro == "PaisDestino")
                sql += "group by IdPaisDestino) T where P.IdPais = T.IdPaisDestino ";
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

        bool IsManifiesto(string codPais)
        {
            return (new[] { "PEI", "USI", "PEE", "USE" }).Contains(codPais);
        }

        string GeneraSqlAgrupadoDesComercial(string filtro, string codPais, string cifTot,
            string pesoNeto, string idioma, string dua,
            string tabla, string orden = "")

        {
            string sql = "";

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
                sql += "from Partida_" + codPais + " P, (select IdPartida, SUM(Registros) AS CantReg, sum(" + cifTot1 +
                       ") as " + cifTot + ", sum(" + pesoNeto1 + ") as " + pesoNeto2 + ", sum(Cantidad) as Cantidad ";
            }
            else if (filtro == "Marca")
            {
                // Ruben 2017-06-25
                sql = "select IdMarca, Marca, CantReg, FOBTot, PesoNeto, Cantidad, ROW_NUMBER() over (order by " +
                      orden + ") as Nro ";
                sql +=
                    "from (select IdMarca, Marca, SUM(Registros) AS CantReg, sum(FOBTot) as FOBTot, sum(0) as PesoNeto, sum(Cantidad) as Cantidad ";
            }

            else if (filtro == "Modelo")
            {
                // Ruben 2017-06-25
                sql = "select IdModelo, Modelo, CantReg, FOBTot, PesoNeto, Cantidad, ROW_NUMBER() over (order by " +
                      orden + ") as Nro ";

                sql +=
                    "from (select IdModelo, Marca + ' - ' + Modelo as Modelo, SUM(Registros) AS CantReg, sum(FOBTot) as FOBTot, sum(0) as PesoNeto, sum(Cantidad) as Cantidad ";
            }
            else if (filtro == "Importador")
            {
                sql = "select IdImportador, Importador, CantReg, " + cifTot + ", " + pesoNeto2 +
                      ", Cantidad, ROW_NUMBER() over (order by " + orden + ") as Nro ";

                sql += "from (select IdImportador, Importador, SUM(Registros) AS CantReg, sum(" + cifTot1 + ") as " + cifTot +
                       ", sum(" + pesoNeto1 + ") as " + pesoNeto2 + ", sum(Cantidad) as Cantidad ";
            }
            else if (filtro == "Proveedor")
            {
                sql = "select IdProveedor, Proveedor, CantReg, " + cifTot + ", " + pesoNeto2 +
                      ", Cantidad, ROW_NUMBER() over (order by " + orden + ") as Nro ";

                sql += "from (select IdProveedor, Proveedor, SUM(Registros) AS CantReg, sum(" + cifTot1 + ") as " + cifTot +
                       ", sum(" + pesoNeto1 + ") as " + pesoNeto2 + ", sum(Cantidad) as Cantidad ";
            }
            else if (filtro == "Exportador")

            {
                sql = "select IdExportador, Exportador, CantReg, " + cifTot + ", " + pesoNeto2 +
                      ", Cantidad, ROW_NUMBER() over (order by " + orden + ") as Nro ";

                sql += "from (select IdExportador, Exportador, SUM(Registros) AS CantReg, sum(" + cifTot1 + ") as " + cifTot +
                       ", sum(" + pesoNeto1 + ") as " + pesoNeto2 + ", sum(Cantidad) as Cantidad ";
            }
            else if (filtro == "ImportadorExp")
            {
                sql = "select IdImportadorExp, ImportadorExp, CantReg, " + cifTot + ", " + pesoNeto2 +
                      ", Cantidad, ROW_NUMBER() over (order by " + orden + ") as Nro ";

                sql += "from (select IdImportadorExp, ImportadorExp, SUM(Registros) AS CantReg, sum(" + cifTot1 + ") as " +
                       cifTot + ", sum(" + pesoNeto1 + ") as " + pesoNeto2 + ", sum(Cantidad) as Cantidad ";
            }
            else if (filtro == "PaisOrigen")
            {
                sql = "select IdPaisOrigen, PaisOrigen, CantReg, " + cifTot + ", " + pesoNeto2 +
                      ", Cantidad, ROW_NUMBER() over (order by " + orden + ") as Nro ";
                sql += "from (select IdPaisOrigen, PaisOrigen, SUM(Registros) AS CantReg, sum(" + cifTot1 + ") as " + cifTot +
                       ", sum(" + pesoNeto1 + ") as " + pesoNeto2 + ", sum(Cantidad) as Cantidad ";
            }
            else if (filtro == "PaisDestino")

            {
                sql = "select IdPaisDestino, PaisDestino, CantReg, " + cifTot + ", " + pesoNeto2 +
                      ", Cantidad, ROW_NUMBER() over (order by " + orden + ") as Nro ";
                sql += "from (select IdPaisDestino, PaisDestino, SUM(Registros) AS CantReg, sum(" + cifTot1 + ") as " +
                       cifTot + ", sum(" + pesoNeto1 + ") as " + pesoNeto2 + ", sum(Cantidad) as Cantidad ";
            }
            else if (filtro == "ViaTransp")
            {
                sql = "select IdViaTransp, ViaTransp, CantReg, " + cifTot + ", " + pesoNeto2 +
                      ", Cantidad, ROW_NUMBER() over (order by " + orden + ") as Nro ";
                sql += "from (select IdViaTransp, ViaTransp, SUM(Registros) AS CantReg, sum(" + cifTot1 + ") as " + cifTot +
                       ", sum(" + pesoNeto1 + ") as " + pesoNeto2 + ", sum(Cantidad) as Cantidad ";
            }
            else if (filtro == "AduanaDUA")

            {
                sql =
                    "select IdAduana, Aduana, DUA, convert(varchar(3), T.IdAduana) + '-' + convert(varchar(20), DUA) as IdAduanaDUA,  Aduana + ' ' + convert(varchar(20), DUA) as AduanaDUA, CantReg, " +
                    cifTot + ", " + pesoNeto2 + ", Cantidad, ROW_NUMBER() over (order by " + orden + ") as Nro ";

                sql += "from (select IdAduana, Aduana, " + dua + " as DUA, ";
                sql += "SUM(Registros) AS CantReg, sum(" + cifTot1 + ") as " + cifTot + ", sum(" + pesoNeto1 + ") as " +
                       pesoNeto2 + ", sum(Cantidad) as Cantidad ";
            }
            else if (filtro == "Aduana")
            {
                sql = "select IdAduana, Aduana, CantReg, " + cifTot + ", " + pesoNeto2 +
                      ", Cantidad, ROW_NUMBER() over (order by " + orden + ") as Nro ";
                sql += "from (select IdAduana, Aduana, SUM(Registros) AS CantReg, sum(" + cifTot1 + ") as " + cifTot +
                       ", sum(" + pesoNeto1 + ") as " + pesoNeto2 + ", sum(Cantidad) as Cantidad ";
            }
            else if (filtro == "Distrito")
            {
                sql = "select IdDistrito, Distrito, CantReg, " + cifTot + ", " + pesoNeto2 +
                      ", Cantidad, ROW_NUMBER() over (order by " + orden + ") as Nro ";
                sql += "from (select IdDistrito, Distrito, SUM(Registros) AS CantReg, sum(" + cifTot1 + ") as " + cifTot +
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

            sql += Session["SqlFiltro"].ToString();

            if (filtro == "Partida")
                sql += "group by IdPartida) T where P.IdPartida = T.IdPartida ";
            else if (filtro == "Marca")
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

                sql += "group by IdPaisOrigen, PaisOrigen) T ";
            else if (filtro == "PaisDestino")

                sql += "group by IdPaisDestino, PaisDestino) T ";
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


        private int CuentaAgrupado(string filtro, string tabla, string dua, string addExtraFiltro = "")

        {
            string sql = "";
            if (filtro == "Partida")
                sql = "select count(*) as Cant from (select distinct IdPartida ";
            else if (filtro == "Marca")
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

        string GetCIFTot(string codPais, string tipoOpe , bool searchBD = true)
        {
            if(searchBD)
            {
                return Funciones.Incoterm(codPais, tipoOpe) + "Tot";
            }
            else
            {
                if (tipoOpe == "I") return "ciftot";
                else return "fobtot";
            }
            
            //return Funciones.Incoterm(codPais, tipoOpe) + "Tot";
            
        }
        

        private void GetTablaAndDua(string tipoOpe, string codPais, ref string tabla, ref string dua,bool isVerRegistros = false)
        {
            if (tipoOpe == "I")
            {

                tabla = (isVerRegistros? "Importacion_" : "ImportacionR_") + codPais;
                if (codPais == "PE" || codPais == "PEB" || codPais == "PEP")
                    dua = "NroCorre";
                else
                {
                    dua = codPais == "EC" ? "Refrendo" : "DUA";
                }
            }
            else
            {
                tabla = (isVerRegistros ? "Exportacion_" : "ExportacionR_") + codPais;

                if (codPais == "PE" || codPais == "PEP")
                    dua = "NroOrden";
                else
                {
                    dua = codPais == "EC" ? "Refrendo" : "DUA";
                }
            }
        }

        [HttpPost]
        public JsonResult Restablecer()
        {
            string restablece = null;
            Session["hdfImportadorB"] = restablece;
            Session["hdfExportadorB"] = restablece;
            Session["hdfNandinaB"] = restablece;
            string maxPeriodo = "";
            string TipoOpeT = "", CodPaisT = "";
            string plan = "" , idUsuario = "";
            string codOpe = "";
            IEnumerable<SelectListItem> listado = new List<SelectListItem>() ;
            if (Session["TipoOpe"] != null)
                TipoOpeT = Session["TipoOpe"].ToString();
            if (Session["CodPais"] != null)
                CodPaisT = Session["CodPais"].ToString();
            if (Session["listaTipoFiltro"] != null)
                listado = (IEnumerable<SelectListItem>)Session["listaTipoFiltro"];
            if (Session["Plan"] != null)
                plan = Session["Plan"].ToString();
            if (Session["IdUsuarioFree"] != null)
                idUsuario = Session["IdUsuarioFree"].ToString();
            if (Session["MaxDataPeriod"] != null)
                maxPeriodo = Session["MaxDataPeriod"].ToString();
            if (Session["codigoOperacion"] != null)
                codOpe = Session["codigoOperacion"].ToString();
            string c = "";
            if (Session["c"] != null) c = Session["c"].ToString();
            string cultureAux = Session["culture"] != null ? Session["culture"].ToString() : "";

            Session.RemoveAll();
            Session["c"] = c;
            Session["TipoOpe"] = TipoOpeT;
            Session["CodPais"] = CodPaisT;
            Session["culture"] = cultureAux;
            Session["listaTipoFiltro"] = listado;
            Session["Plan"] = plan;
            Session["IdUsuarioFree"] = idUsuario;
            Session["MaxDataPeriod"] = maxPeriodo;
            Session["codigoOperacion"] = codOpe;
            return Json(new { estado = true }, JsonRequestBehavior.AllowGet);
        }

        private bool ValidarFiltros(string[] valuesOption)
        {
            bool bValida = false;
            List<OptionSelect> listOption = new List<OptionSelect>();
            if (Session["lstFiltros"] != null)
            {
                if (Session["lstFiltros"] is List<OptionSelect> listOptionAux)
                    foreach (var t in listOptionAux)
                    {
                        if (!valuesOption.Contains(t.value))
                        {
                            listOption.Add(t);
                        }
                    }


                string c = Session["culture"].ToString();

               List<SelectListItem> lista = listaDictionary[Session["culture"].ToString()];
               
               foreach (OptionSelect item in listOption)
                {
                    //if (item.text.Contains("[Partida]") ||
                    //item.text.Contains("[Importador]") || item.text.Contains("[Exportador]"))
                    if(item.text.Contains(lista.Find(x=>x.Value == "PA").Text)  ||
                       item.text.Contains(lista.Find(x => x.Value == "IM").Text) ||
                       item.text.Contains(lista.Find(x => x.Value == "EX").Text) ||
                       item.text.Contains(lista.Find(x => x.Value == "PO").Text) ||
                       item.text.Contains(lista.Find(x => x.Value == "PD").Text))
                    {
                        bValida = true;
                        break;
                    }
                }
                if (listOption.Count == 0)
                    bValida = true;
            }
            else
            {
                bValida = true;
            }

            if (bValida)
                Session["lstFiltros"] = listOption;

            return bValida;
        }
        public List<object> GeneraJsonBuscaPartidas(string nandina, string codPais,string culture)
        {
            var json = new List<object>();

            var dt = Consulta.BuscaPartidasFreemium(nandina, codPais, culture);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    json.Add(new
                    {
                        id = dr["Nandina2"].ToString().Trim(),
                        value0 = dr["Nandina"].ToString(),
                        value = dr["Partida"].ToString().Replace("\"", "")
                    });
                }
            }
            else
            {
                json.Add(new { id = 0, value = "-" });
            }

            return json;
        }
        public List<object> GeneraJsonBuscaEmpresas(string empresa, string codPais)
        {
            var json = new List<object>();

            var dt = Consulta.BuscaEmpresas(empresa, codPais);
            if (dt.Rows.Count > 0)
            {
                json.Add(new { id = 0, value = empresa + " [TODAS]", texto = empresa });
                foreach (DataRow dr in dt.Rows)
                {
                    if (empresa != "")
                    {
                        json.Add(new
                        {
                            id = dr["IdEmpresa"].ToString().Trim(),
                            value = dr["Empresa"].ToString().Replace("\"", " ") + " " + dr["RUC"]
                        });
                    }
                    else
                    {
                        json.Add(new
                        {
                            id = dr["IdEmpresa"].ToString().Trim(),
                            value = dr["RUC"] + " " + dr["Empresa"]
                        });
                    }
                }
            }
            else
            {
                json.Add(new { id = 0, value = "-" });
            }

            return json;
        }
        public object AgregaPalabrasFiltros(string palabras, string opcion, string codPais, string selected = "0")
        {
            
            string valueOption = "";
            string textOption = "";
            //var codPais = Session["CodPais"].ToString();

            if (opcion == "1PAI")
            {
                valueOption = opcion + "|" + palabras;
                textOption = "["+ Resources.Resources.Demo_OriginCountry + "] " + palabras;

            }
            else if (opcion == "1PAE")
            {
                valueOption = opcion + "|" + palabras;
                textOption = "[" + Resources.Resources.Demo_DestinationCountry + "] " + palabras;

            }
            else if (opcion == "2P_")
            {
                if (Session["hdfNandinaB"] == null) Session["hdfNandinaB"] = palabras;
                else Session["hdfNandinaB"] = Session["hdfNandinaB"] + "|" + palabras;

                var idPartida = Funciones.BuscaIdPartida(palabras, codPais);
                var subCapitulo = Funciones.BuscaSubCapitulo(palabras);
                var hts6 = Funciones.BuscaHts6(palabras);
                var todos = "TODOS";
                if (codPais == "BR" || codPais == "CN" || codPais == "IN" || codPais == "US")
                {
                    subCapitulo = Funciones.BuscaSubCapitulo(palabras, "EN");
                    hts6 = Funciones.BuscaHts6(palabras, "EN");
                    todos = "ALL";
                }

                if (idPartida != "")
                {
                    textOption = "["+Resources.Resources.Nandina_FormField_Filtro + "] " + Funciones.BuscaPartida(idPartida, codPais);
                }
                else if (subCapitulo != "")
                {
                    textOption = "[" + Resources.Resources.Nandina_FormField_Filtro + "] " + palabras + " [" + todos + " 4] " + subCapitulo;
                }
                else if (hts6 != "")
                {
                    textOption = "[" + Resources.Resources.Nandina_FormField_Filtro + "] " + palabras + " [" + todos + " 6] " + hts6;
                }
                else
                {
                    textOption = "[" + Resources.Resources.Nandina_FormField_Filtro + "] " + palabras + " [" + todos + "]";

                }

                valueOption = opcion + '|' + palabras;
            }
            else if (opcion == "3I_")
            {
                if (Session["hdfImportadorB"] == null) Session["hdfImportadorB"] = palabras;
                else Session["hdfImportadorB"] = Session["hdfImportadorB"] + "|" + palabras;
                if (selected != "0")
                {
                    textOption = "["+Resources.Resources.Search_Form_Item05 + "] " + Funciones.BuscaEmpresa(selected, codPais);
                }
                else
                {
                    textOption = "[" + Resources.Resources.Search_Form_Item05 + "] " + palabras + " [TODOS]";
                }
                valueOption = opcion + '|' + palabras;
            }
            else if (opcion == "3E_")
            {
                if (Session["hdfExportadorB"] == null) Session["hdfExportadorB"] = palabras;
                else Session["hdfExportadorB"] = Session["hdfExportadorB"] + "|" + palabras;
                if (selected != "0")
                {
                    textOption = "["+Resources.Resources.Search_Form_Item06 + "] " + Funciones.BuscaEmpresa(selected, codPais);
                }
                else
                {
                    textOption = "[" + Resources.Resources.Search_Form_Item06 + "] " + palabras + " [TODOS]";
                }

                valueOption = opcion + '|' + palabras;
            }

            object nuevoFiltro = new
            {
                text = textOption,
                value = valueOption
            };

            List<OptionSelect> listFilters;
            if (Session["lstFiltros"] != null)
                listFilters = Session["lstFiltros"] as List<OptionSelect>;
            else
                listFilters = new List<OptionSelect>();

            if (listFilters != null && !listFilters.Exists(x => x.text == textOption))
            {
                listFilters.Add(new OptionSelect { value = valueOption, text = textOption });
            }

            if (listFilters != null && listFilters.Count > 0)
                Session["lstFiltros"] = listFilters;
            else
                Session.Remove("lstFiltros");

            return nuevoFiltro;
        }

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

        private ConsultaForm GetConsultaForm(string codPais, string selectedTipoOpe)
        {
            ConsultaForm consultaForm = new ConsultaForm
            {
                Importador = selectedTipoOpe.Equals("I"),
                Proveedor = false,
                PaisOrigen = true,
                Exportador = selectedTipoOpe.Equals("E"),
                ImportadorExp = false,
                CodPais = Session["CodPais"].ToString(),
                TipoOpe = Session["TipoOpe"].ToString(),
                FlagDescComercialB = false
            };
            return consultaForm;
        }

        private Boolean validarCodigo(string cod)
        {
            //return (new[] { "sni", "ahkperu", "amcham", "cciperu", "cocep" , "asean" }).Contains(cod.ToLower());
            //return (new[] { "sni", "ahkperu", "amcham", "cciperu", "cocep" }).Contains(cod.ToLower());
            //return (new[] { "sni", "ahkperu", "amcham", "cciperu", "cocep", "asean","camchal" }).Contains(cod.ToLower());
            // Ruben 202303
            return (new[] { "sni", "ahkperu", "amcham", "cciperu", "cocep", "asean", "camchal", "ahkarg" }).Contains(cod.ToLower());
        }

        private IEnumerable<SelectListItem> CargarPaises(string cod = "")
        {
            var lista = new List<SelectListItem>();
            if(cod.ToLower()=="asean" || cod.ToLower() == "camchal")
                lista.Add(new SelectListItem { Value = "CL", Text = @"Chile" });
            // Ruben 202303
            else if (cod.ToLower() == "ahkarg")
                lista.Add(new SelectListItem { Value = "AR", Text = @"Argentina" });
            else
                lista.Add(new SelectListItem { Value = "PE", Text = @"Perú" });
            return lista;
        }

        private IEnumerable<SelectListItem> CargarPeriodos(string CodPais ,string TipoOpe , string Idioma,string tipo)
        {
            string TrimIni = "", TrimFin = "", AñoIni = "", AñoFin = "";
            FuncionesBusiness.RangoFreemium(CodPais, TipoOpe, ref TrimIni, ref TrimFin, ref AñoIni, ref AñoFin);
            IEnumerable<SelectListItem> lista = new List<SelectListItem>();
            if (tipo == "T")
                lista = Funciones.CargarTrimestres(TrimIni, TrimFin, Idioma);
            else
            {
                lista = Funciones.CargarAños(AñoIni, AñoFin);
            }

            //var mayor = lista.Max(item => item.Value);
            var mayor = lista.LastOrDefault()?.Value;
            Session["MaxDataPeriod"] = mayor;
            return lista;
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

            _codPais = Session["CodPais"].ToString();


            switch (filtro)
            {
                case "Partida":

                    Session["PartidasB"] = IDsSeleccionados;
                    //hdfCantPartidasB.Value = IDsSeleccionados.Count.ToString();
                    tipo = "2PA";
                    string partidaFav =
                        FuncionesBusiness.ObtienePartidaFav(Session["IdUsuarioFree"].ToString(), _codPais, tipoOpe, id);

                    if (partidaFav == "")

                        partidaFav = FuncionesBusiness.BuscaPartida(id, _codPais, idioma);


                    nombre = "[" + Resources.Resources.Search_Form_Item04 + "] " + partidaFav;

                    break;
                case "Marca":
                    Session["MarcasB"] = IDsSeleccionados;
                    tipo = "2MA";
                    nombre = "[" + Resources.Resources.Search_Form_BrandField + "] " +
                             Funciones.BuscaMarca(id, _codPais);
                    break;
                case "Modelo":
                    Session["ModelosB"] = IDsSeleccionados;
                    tipo = "2MO";

                    nombre = "[" + Resources.AdminResources.Model_FormField_Label + "] " +
                             Funciones.BuscaModelo(id, _codPais);
                    break;
                case "Importador":

                    Session["ImportadoresB"] = IDsSeleccionados;
                    //hdfCantImportadoresB.Value = IDsSeleccionados.Count.ToString();
                    tipo = "3IM";
                    nombre = "[" + Resources.Resources.Search_Form_Item05 + "] " + Funciones.BuscaEmpresa(id, _codPais);
                    break;
                case "Notificado":
                    Session["NotificadosB"] = IDsSeleccionados;
                    tipo = "3NO";
                    nombre = "[" + Resources.Resources.Demo_Notif_Tab_Fil + "] " + Funciones.BuscaNotificado(id, _codPais);
                    break;

                case "Exportador":
                    Session["ExportadoresB"] = IDsSeleccionados;
                    //hdfCantExportadoresB.Value = IDsSeleccionados.Count.ToString();
                    tipo = "3EX";
                    nombre = "[" + Resources.Resources.Search_Form_Item06 + "] " + Funciones.BuscaEmpresa(id, _codPais);

                    break;
                case "Proveedor":

                    Session["ProveedoresB"] = IDsSeleccionados;
                    //hdfCantProveedoresB.Value = IDsSeleccionados.Count.ToString();
                    tipo = "4PR";

                    nombre = "[" + Resources.Resources.Search_Form_Item06 + "] " +
                             Funciones.BuscaProveedor(id, _codPais);
                    break;

                case "ImportadorExp":
                    Session["ImportadoresExpB"] = IDsSeleccionados;
                    //hdfCantImportadoresExpB.Value = IDsSeleccionados.Count.ToString();

                    tipo = "4IE";

                    nombre = "[" + @Resources.Resources.Search_Form_Item05 + "] " +
                             Funciones.BuscaImportadorExp(id, _codPais);
                    break;
                case "PaisEmbarque":
                    Session["PaisesEmbarqueB"] = IDsSeleccionados;
                    tipo = "5PE";
                    nombre = "[" + Resources.Resources.Ult_Paises_Embarque + "] " + Funciones.BuscaPais(id, _codPais);
                    break;

                case "PaisOrigen":
                    Session["PaisesOrigenB"] = IDsSeleccionados;
                    tipo = "5PO";
                    nombre = "[" + Resources.AdminResources.OriginCountry_FormField_Label + "] " +
                             Funciones.BuscaPais(id, _codPais);
                    break;
                case "PaisDestino":

                    Session["PaisesDestinoB"] = IDsSeleccionados;
                    tipo = "5PD";
                    nombre = "[" + Resources.AdminResources.DestinationCountry_FormField_Label + "] " +
                             Funciones.BuscaPais(id, _codPais);
                    break;
                case "ViaTransp":
                    Session["ViasTranspB"] = IDsSeleccionados;
                    tipo = "6VT";
                    nombre = "[" + Resources.AdminResources.FilterText_ViaTransp + "] " +
                             Funciones.BuscaVia(id, _codPais);
                    break;
                case "AduanaDUA":
                    Session["AduanaDUAsB"] = IDsSeleccionados;
                    tipo = "7AD";
                    nombre = "[" + Resources.AdminResources.FilterText_Aduana + " DUA] " +
                             Funciones.BuscaAduana(id.Split('-')[0], _codPais) + " - " +
                             id.Split('-')[1];
                    break;
                case "Aduana":
                    Session["AduanasB"] = IDsSeleccionados;
                    tipo = "7AA";
                    nombre = "[" + Resources.AdminResources.FilterText_Aduana + "] " +
                             Funciones.BuscaAduana(id, _codPais);
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
                             Funciones.BuscaDistrito(id, _codPais);
                    break;
                case "PtoDescarga":
                    Session["PtosDescargaB"] = IDsSeleccionados;
                    tipo = "6PD";
                    nombre = "[" + Resources.AdminResources.UnloadingPort_FilterText + "] " + Funciones.BuscaPuerto(id, _codPais);
                    break;
                case "PtoEmbarque":
                    Session["PtosEmbarqueB"] = IDsSeleccionados;
                    tipo = "6PE";
                    nombre = "[" + Resources.AdminResources.LastShipmentPort_FilterText + "] " + Funciones.BuscaPuerto(id, _codPais);
                    break;
                case "PtoDestino":
                    Session["PtosDestinoB"] = IDsSeleccionados;
                    tipo = "6DE";
                    nombre = "[" + Resources.AdminResources.DestinationPort_FilterText + "] " + Funciones.BuscaPuerto(id, _codPais);
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

        private void ValidaCodPais2(string codPais2, ref string codPais)
        {
            if (codPais2 == "4UE")
                codPais = "UE";
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

        private void ValidaCodPaisManif(ref string codPais, string tipoOpe)
        {
            if (codPais.Contains("_")) codPais = codPais.Replace("_", tipoOpe);
        }

        private void GetCifTot_PesoNeto_CantReg_ValueCifTot_Unidad_ValuePesoNeto(string codPais, string tipoOpe,
            string tabla, ref string cifTot, ref string pesoNeto,
            ref int cantReg, ref string valueCifTot, ref string unidad,
            ref decimal valuePesoNeto)
        {
            bool isManif = IsManifiesto(codPais);

            cifTot = GetCIFTot(codPais, tipoOpe,searchBD:false);   //Funciones.Incoterm(codPais, tipoOpe) + "Tot";
            if (!isManif) pesoNeto = Funciones.CampoPeso(codPais, tipoOpe);

            DataRow drTotales =
                FuncionesBusiness.CalculaTotalesFreemium(Session["SqlFiltro"].ToString(), cifTot, codPais, pesoNeto, tabla, isManif: isManif);

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

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult PageIndexChangingTable(string filtro, Enums.TipoFiltro tipoFiltro, string codPais2,
            int pagina, string codPais, string tipoOpe,
            string idTabla, string idsSeleccionados, string idsPagina)
        {

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
            objTabData.Filtro = filtro;
            objTabData.IdsSeleccionados = (ArrayList)Session["IDsSeleccionados"];
            string filasNuevaPagina ;
            if (tipoFiltro == Enums.TipoFiltro.Resumen)
            {
                objTabData.ListRows =
                    GetDataTableToListGridRow(listaAux[0], filtro, cifTot, valueCifTot, codPais, GetSpecificCulture());
                filasNuevaPagina = RenderViewToString(ControllerContext, "GridViews/GridRowView", objTabData);
            }
            else
            {
                if (filtro == Enums.Filtro.AduanaDUA.ToString() || filtro == Enums.Filtro.Aduana.ToString())
                {
                    if (filtro == Enums.Filtro.Aduana.ToString())
                        objTabData.GridHead.IsVisibleDuas = false;
                    objTabData.ListRowsTab = GetListGridRowAduanaDua(listaAux[0], filtro, cifTot, GetSpecificCulture());
                    filasNuevaPagina =
                        RenderViewToString(ControllerContext, "GridViews/AduanaDuaGridRowView", objTabData);
                }
                else
                {
                    objTabData.ListRowsTab = GetDataTableToListGridRowTab(listaAux[0], filtro, cifTot, valueCifTot, codPais,
                        GetSpecificCulture(), pesoNeto, unidad);
                    objTabData.GridHead.IsVisblePrecio = filtro != "Marca" && filtro != "Modelo" && unidad != "";
                    objTabData.GridHead.IsVisibleTotalKg = filtro != "Marca" && filtro != "Modelo" && unidad != "";
                    filasNuevaPagina = RenderViewToString(ControllerContext, "GridViews/TabGridRowView", objTabData);
                }
            }

            return Json(new
            {
                filasNuevaPagina
            });
        }

        [SessionExpireFilter]
        [HttpPost]
        public ActionResult GetTabsView(string tipoOpe, string codPais, bool hideTabExcel = false)
        {

            ConsultaForm model = new ConsultaForm();
            model.TipoOpe = tipoOpe;

            model.CodPais = codPais;
            model.IsOcultoMarcasModelos = (Session["Plan"].ToString() != "BUSINESS" &&
                                           Session["Plan"].ToString() != "PREMIUM" &&
                                           Session["PLAN"].ToString() != "UNIVERSIDADES");
            model.Importador = Funciones.ExisteVariable(codPais, tipoOpe, "IdImportador");
            model.Proveedor = false;//Funciones.ExisteVariable(codPais, tipoOpe, "IdProveedor");
            model.Exportador = Funciones.ExisteVariable(codPais, tipoOpe, "IdExportador");
            model.ImportadorExp = false; Funciones.ExisteVariable(codPais, tipoOpe, "IdImportadorExp");
            model.PaisOrigen = Funciones.ExisteVariable(codPais, tipoOpe, "IdPaisOrigen");
            model.ExistePaisDestino = Funciones.ExisteVariable(codPais, tipoOpe, "IdPaisDestino");
            model.ExisteViaTransp = GetExisteViaTransp(codPais);
            //model.ExisteAduana = GetExisteAduana(codPais);
            //model.ExisteDua = GetExisteDua(model.ExisteAduana, codPais);
           //model.ExisteDistrito = GetExisteDistrito(codPais);

            model.HideTabExcel = hideTabExcel;

            model.FlagVarVisibles = new TabMisBusquedas(tipoOpe, codPais);   //new FlagVarVisibles(codPais, tipoOpe, IsManifiesto(codPais));

            model.FlagVarVisibles.ExisteAduana = false;
            model.FlagVarVisibles.ExisteDUA = false;
            model.FlagVarVisibles.ExisteDesComercial = false;
            model.FlagVarVisibles.ExisteImportadorExp = false;
            model.FlagVarVisibles.ExisteMarcasModelos = false;
            model.FlagVarVisibles.ExisteProveedor = false;

            return PartialView("TabsView", model);
        }

        #region VerRegistrosEnModal
        [SessionExpireFilter]
        [HttpPost]
        public JsonResult VerRegistros(string filtro, string idregistro, string tipoOpe,
            string codPais, string codPais2, string desde,
            string hasta, int indexCboPaisB, string codPaisB,
            int numFiltrosExistentes, string tipoBusqueda)
        {
            bool maximoLimiteFiltros = false;
            if (numFiltrosExistentes == LimiteFiltros)
            {
                maximoLimiteFiltros = true;
            }

            string auxCodPais = codPais;
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

            bool lnkAgregarFiltroSelVisible = (filtro != "Marca" && filtro != "Modelo");
            bool cboDescargas2Visible = (filtro != "Marca" && filtro != "Modelo");

            string[] filtroCondicion = { "Partida", "Importador", "Exportador", "Proveedor", "ImportadorExp" };

            bool lnkAgregarSelAFavoritosVisible = filtroCondicion.Contains(filtro);

            var tabla = "";
            var dua = "";
            GetTablaAndDua(tipoOpe, codPais, ref tabla, ref dua, isVerRegistros:true);

            if (filtro != "Manifiesto" && filtro != Enums.Filtro.AduanaDUA.ToString())
            {
                Session["hdfVariable"] = "Id" + filtro;
                Session["hdfValor"] = idregistro;
            }
            else if (filtro == "Manifiesto")
            {
                Session["hdfVariable"] = "Manifiesto";
                Session["hdfValor"] = "'" + idregistro + "'";
            }
            else
            {
                Session["hdfVariable"] = "convert(varchar(3), IdAduana) + '-' + convert(varchar(20), " + dua + ")";
                Session["hdfValor"] = "'" + idregistro + "'";
            }

            Session["hdfDesComercialBB2"] = null;
            Session["hdfDUAB2"] = null;

            Session["hdfFiltroSel"] = filtro;
            Session["hdfIDSel"] = idregistro;

            string lblFiltroSel = "";
            string lnkAgregarSelAFavoritos = "";


            string hdfValor = Session["hdfValor"].ToString();

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
            }

            var cifTot = GetCIFTot(codPais, tipoOpe); //Funciones.Incoterm(codPais, tipoOpe) + "Tot";
            var pesoNeto = Funciones.CampoPeso(codPais, tipoOpe); //

            Session["SqlFiltroR2"] = GeneraSqlFiltroR2(tipoOpe, codPais, codPais2,  desde, hasta,
                indexCboPaisB, codPaisB, dua,tipoBusqueda, auxCodPais);

            DataRow drTotales =
                FuncionesBusiness.CalculaTotalesFreemium(Session["SqlFiltroR2"].ToString(), cifTot, codPais, pesoNeto, tabla, isManif: isManif, isModal :true );

            string lblRecordsFound = idioma == "es" ?
                $"Se encontraron {Convert.ToInt32(drTotales["CantReg"]) :n0} registros" :
                $"{Convert.ToInt32(drTotales["CantReg"]):n0} records were found";
            if (idioma == "es")
            {
                lblRecordsFound = "Se encontraron " + Convert.ToInt32(drTotales["CantReg"]).ToString("n0") +
                                  " registros";
            }
            else
            {
                lblRecordsFound = Convert.ToInt32(drTotales["CantReg"]).ToString("n0") + " records were found";
            }

            string sqlFinal = "";

            DataTable dtRegistros = FuncionesBusiness.BuscarRegistros(codPais2, tipoOpe, tabla,
                Session["SqlFiltroR2"].ToString(),
                Session["hdfIdGrupoB"]?.ToString(), ref sqlFinal, isVerRegistros:true);

            object objMensaje = null;
            string idDescargaDefault = "";
            EnumerableRowCollection<OptionSelect> optionsDescargas = null;
            string tablaVerRegistro = "";
            int totalPages = 0;
            if (dtRegistros.Rows.Count > 0)
            {
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
                if (Session["Plan"].ToString() == "ESENCIAL" && !isManif)
                {
                    objMiBusqueda.ImportadorExp = false;
                }

                objMiBusqueda.EsFreeTrial = true;
                objMiBusqueda.ExisteAduana = existeAduana;
                objMiBusqueda.ExisteDua = existeDua;
                objMiBusqueda.Dua = dua;


                VerRegistroTable objVerRegistroTable = new VerRegistroTable
                {
                    TotalPaginas = (int)Math.Ceiling(Convert.ToDecimal(dtRegistros.Rows.Count) / VerRegistrosPageSize),
                    TipoOpe = tipoOpe,
                    VerRegistroTableRows = GetListaVerRegistroTableRow(
                        dtRegistros.AsEnumerable().Take(VerRegistrosPageSize).CopyToDataTable(), objMiBusqueda,
                        GetSpecificCulture(), idioma, (sqlFinal.Contains("IdMarca") || sqlFinal.Contains("IdModelo"))),
                    VerRegistroTableHead = GetVerRegistroTableHead(objMiBusqueda,
                        (sqlFinal.Contains("IdMarca") || sqlFinal.Contains("IdModelo"))),
                    MiBusqueda = objMiBusqueda
                };

                totalPages = objVerRegistroTable.TotalPaginas;
                string idUsuario = Session["IdUsuarioFree"].ToString();

                cboDescargas2Visible = false;
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

                tablaVerRegistro = RenderViewToString(ControllerContext, "GridViews/VerRegistroGridView" + (isManif ? "_Manif" : ""),
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
                totalPages
            });
        }

        private string GeneraSqlFiltroR2(string tipoOpe, string codPais, string codPais2,
            string desde, string hasta, int indexCboPaisB,
            string codPaisB, string dua, string tipoBusqueda, string auxCodPais = "")
        {
            string sql = GeneraSqlFiltroR(tipoOpe, codPais, codPais2,tipoBusqueda, desde, hasta, indexCboPaisB, codPaisB,
                dua, auxCodPais);
            //sql = sql.Substring(0, sql.Length - 1);

            if (Session["hdfVariable"] != null)
                sql += "and " + Session["hdfVariable"] + " = " + Session["hdfValor"] + " ";
            if (Session["hdfDUAB2"] != null)
                sql += "and " + dua + " like '%" + Session["hdfDUAB2"] + "%' ";
            if (Session["hdfDesComercialBB2"] != null)
                sql += "and DesComercial like '%" + Session["hdfDesComercialBB2"]+ "%' ";

            if (!string.IsNullOrWhiteSpace(Session["hdfPalabrasY"]?.ToString()))
                sql += ") ";

            return sql;
        }

        string GeneraSqlFiltroR(string tipoOpe, string codPais, string codPais2,string tipoBusqueda,
            string desde, string hasta, int indexCboPaisB,
            string codPaisB, string dua, string auxCodPais = "")
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
                    if (inicio)
                    {
                        sql += "\"" + palabra + "*\" ";
                        inicio = false;
                    }
                    else
                        sql += "and \"" + palabra + "*\" ";
                }

                sql += "') ";

                if (tipoOpe == "I")
                    sql += "and IdImportacion in (select IdImportacion from Importacion_" + codPais + " where 1 = 1 ";
                else
                    sql += "and IdExportacion in (select IdExportacion from Exportacion_" + codPais + " where 1 = 1 ";
            }

            if (codPais2 == "4UE")
            {
                if (tipoOpe == "I")
                    sql += "and IdPaisImp = " + auxCodPais + " ";
                else
                    sql += "and IdPaisExp = " + auxCodPais + " ";
            }

            sql += tipoBusqueda == "T" ?
                          "and FechaNum between " + FuncionesBusiness.ObtieneFechaIni(desde) + " and " + FuncionesBusiness.ObtieneFechaFin(hasta) + " " :
                          "and FechaNum between " + desde + "0100 and " + hasta + "1299 ";

            //sql += "and FechaNum >= " + anioMesIni + "00 and FechaNum <= " + anioMesFin + "99 ";
            //Session["FechaIni"] = anioMesIni + "00";
            //Session["FechaFin"] = anioMesFin + "99";

            if (indexCboPaisB > 0)
            {
                if (tipoOpe == "I")
                    sql += "and " + (!isManif ? "IdPaisOrigen" : "IdPaisEmbarque") + " = " + codPaisB + " ";
                else
                    sql += "and IdPaisDestino = " + codPaisB + " ";
            }

            if (Session["hdfNandinaB"] != null)
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

            if (Session["hdfImportadorB"] != null)
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
                        sql += "and IdExportador in (select IdEmpresa from Empresa_" + codPais + " where 1 = 1 ";

                    string[] palabras = Session["hdfImportadorB"].ToString().Split(' ');
                    foreach (string palabra in palabras)
                        sql += "and Empresa like '%" + palabra + "%' ";
                    sql += ") ";
                }
            }

            if (Session["hdfExportadorB"] != null)
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

                    string[] palabras = Session["hdfExportadorB"].ToString().Split(' ');
                    foreach (string palabra in palabras)
                        sql += "and Empresa like '%" + palabra + "%' ";
                    sql += ") ";
                }
            }

            if (Session["hdfProveedorB"] != null)
            {
                if (!isManif)
                {
                    sql += "and IdProveedor in (select IdProveedor from Proveedor_" + codPais + " where ";
                    string[] palabras = Session["hdfProveedorB"].ToString().Split('|');
                    foreach (string palabra in palabras)
                        sql += "Proveedor like '%" + palabra + "%' or ";
                    sql = sql.Substring(0, sql.Length - 3) + ")";
                }
                else
                {
                    sql += "and IdProveedor in (select IdProveedor from Proveedor_" + codPais + " PR where 1 = 1 ";
                    string[] palabras = Session["hdfProveedorB"].ToString().Split(' ');
                    foreach (string palabra in palabras)
                        sql += "and Proveedor like '%" + palabra + "%' ";
                    sql += ") ";
                }
            }

            if (Session["hdfImportadorExpB"] != null)
            {
                if (!isManif)
                {
                    sql += "and IdImportadorExp in (select IdImportadorExp from ImportadorExp_" + codPais + " where ";

                    string[] palabras = Session["hdfImportadorExpB"].ToString().Split('|');

                    foreach (string palabra in palabras)
                        sql += "ImportadorExp like '%" + palabra + "%' or ";
                    sql = sql.Substring(0, sql.Length - 3) + ")";
                }
                else
                {
                    sql += "and IdImportadorExp in (select IdImportadorExp from ImportadorExp_" + codPais +
                           " IE where 1 = 1 ";
                    string[] Palabras = Session["hdfImportadorExpB"].ToString().Split(' ');
                    foreach (string palabra in Palabras)
                        sql += "and IE.ImportadorExp like '%" + palabra + "%' ";
                    sql += ") ";
                }
            }

            if (!isManif)
            {
                if (Session["PartidasB"] != null)
                    sql += "and IdPartida in " + Funciones.ListaItems((ArrayList)Session["PartidasB"]) + " ";
                if (Session["MarcasB"] != null)
                    sql += "and IdMarca in " + Funciones.ListaItems((ArrayList)Session["MarcasB"]) + " ";
                if (Session["ModelosB"] != null)
                    sql += "and IdModelo in " + Funciones.ListaItems((ArrayList)Session["ModelosB"]) + " ";
                if (Session["ImportadoresB"] != null)
                    sql += "and IdImportador in " + Funciones.ListaItems((ArrayList)Session["ImportadoresB"]) + " ";
                if (Session["ExportadoresB"] != null)
                    sql += "and IdExportador in " + Funciones.ListaItems((ArrayList)Session["ExportadoresB"]) + " ";
                if (Session["ProveedoresB"] != null)
                    sql += "and IdProveedor in " + Funciones.ListaItems((ArrayList)Session["ProveedoresB"]) + " ";
                if (Session["ImportadoresExpB"] != null)
                    sql += "and IdImportadorExp in " + Funciones.ListaItems((ArrayList)Session["ImportadoresExpB"]) + " ";
                if (Session["PaisesOrigenB"] != null)
                    sql += "and IdPaisOrigen in " + Funciones.ListaItems((ArrayList)Session["PaisesOrigenB"]) + " ";
                if (Session["PaisesDestinoB"] != null)
                    sql += "and IdPaisDestino in " + Funciones.ListaItems((ArrayList)Session["PaisesDestinoB"]) + " ";
                if (Session["ViasTranspB"] != null)
                    sql += "and IdViaTransp in " + Funciones.ListaItems((ArrayList)Session["ViasTranspB"]) + " ";
                if (Session["AduanaDUAsB"] != null)
                    sql += "and convert(varchar(3), IdAduana) + '-' + convert(varchar(20), " + dua + ") in " +
                           Funciones.ListaItemsS((ArrayList)Session["AduanaDUAsB"]) + " ";
                if (Session["AduanasB"] != null)
                    sql += "and IdAduana in " + Funciones.ListaItems((ArrayList)Session["AduanasB"]) + " ";
                //if (Session["DUAsB"] != null)
                //    sql += "and " + DUA + " like '" + Functions.ListaItems((ArrayList)Session["DUAsB"]).Replace("(", "").Replace(")", "") + "%' ";
                if (Session["DistritosB"] != null)
                    sql += "and IdDistrito in " + Funciones.ListaItems((ArrayList)Session["DistritosB"]) + " ";
            }
            else
            {
                if (Session["ImportadoresB"] != null)
                    sql += "and IdImportador in " + Funciones.ListaItems((ArrayList)Session["ImportadoresB"]) + " ";
                if (Session["NotificadosB"] != null)
                    sql += "and IdNotificado in " + Funciones.ListaItems((ArrayList)Session["NotificadosB"]) + " ";
                if (Session["ExportadoresB"] != null)
                    sql += "and IdExportador in " + Funciones.ListaItems((ArrayList)Session["ExportadoresB"]) + " ";
                if (Session["ProveedoresB"] != null)
                    sql += "and IdProveedor in " + Funciones.ListaItems((ArrayList)Session["ProveedoresB"]) + " ";
                if (Session["ImportadoresExpB"] != null)
                    sql += "and IdImportadorExp in " + Funciones.ListaItems((ArrayList)Session["ImportadoresExpB"]) + " ";
                if (Session["PaisesOrigenB"] != null)
                    sql += "and IdPaisOrigen in " + Funciones.ListaItems((ArrayList)Session["PaisesOrigenB"]) + " ";
                if (Session["PaisesEmbarqueB"] != null)
                    sql += "and IdPaisEmbarque in " + Funciones.ListaItems((ArrayList)Session["PaisesEmbarqueB"]) + " ";
                if (Session["PaisesDestinoB"] != null)
                    sql += "and IdPaisDestino in " + Funciones.ListaItems((ArrayList)Session["PaisesDestinoB"]) + " ";
                if (Session["PtosDescargaB"] != null)
                    sql += "and IdPtoDescarga in " + Funciones.ListaItems((ArrayList)Session["PtosDescargaB"]) + " ";
                if (Session["PtosEmbarqueB"] != null)
                    sql += "and IdPtoEmbarque in " + Funciones.ListaItems((ArrayList)Session["PtosEmbarqueB"]) + " ";
                if (Session["PtosDestinoB"] != null)
                    sql += "and IdPtoDestino in " + Funciones.ListaItems((ArrayList)Session["PtosDestinoB"]) + " ";
                if (Session["ManifiestosB"] != null)
                    sql += "and Manifiesto in " + Funciones.ListaItemsS((ArrayList)Session["ManifiestosB"]) + " ";
            }



            if (Session["hdfIdGrupoB"] != null)
                sql += "and Id" + Session["hdfTipoFavoritoB"]+
                       " in (select IdFavorito from FavoritoGrupo where IdGrupo = " +
                       Session["hdfIdGrupoB"] + ") ";

            return sql;
        }

        private List<VerRegistroTableRow> GetListaVerRegistroTableRow(DataTable dt, MiBusqueda objMiBusqueda,
            CultureInfo specificCulture,
            string idioma, bool flagFormatoB = false)
        {
            List<VerRegistroTableRow> lista = new List<VerRegistroTableRow>();
            int numero = 0;

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

                    var dataFieldCifOFobUnit = objMiBusqueda.Cif + "Unit";

                    var isVisibleCifImptoUnit = (objMiBusqueda.CodPais == "PE");
                    if (idioma == "es")
                    {
                        isVisibleCifImptoUnit = (!flagFormatoB && isVisibleCifImptoUnit);
                    }

                    foreach (DataRow dr in dt.Rows)
                    {
                        numero++;
                        string fecha = dr["FechaNum"].ToString();
                        if (!fecha.Contains("/"))
                        {
                            fecha = fecha.Substring(0, 4) + '/' + fecha.Substring(4, 2) + '/' + fecha.Substring(6, 2);
                        }
                            
                        //string fecha1 = Convert.ToDateTime(fecha).ToString("dd/MM/yyyy");
                        lista.Add(new VerRegistroTableRow()
                        {
                            Numero = numero,
                            Fecha = Convert.ToDateTime(fecha).ToString("dd/MM/yyyy", specificCulture),
                            Nandina = dr["Nandina"].ToString(),
                            Importador = objMiBusqueda.Importador ? dr["Importador"].ToString() : "",
                            Exportador = objMiBusqueda.Proveedor ? dr["Proveedor"].ToString() : "",
                            CampoPeso = isVisibleCampoPeso ? (dr[objMiBusqueda.CampoPeso].ToString() != "" ? Convert.ToDecimal(dr[objMiBusqueda.CampoPeso]).ToString("n2", specificCulture) : "") : "",
                            Cantidad = dr["Cantidad"].ToString() != "" ? Convert.ToDecimal(dr["Cantidad"]).ToString("n2", specificCulture) : "",
                            Unidad = dr["Unidad"].ToString(),
                            FobOFasUnit = isVisibleFobOFasUnit ? (dr[dataFieldFobOFasUnit].ToString() != "" ? Convert.ToDecimal(dr[dataFieldFobOFasUnit]).ToString("n2", specificCulture) : "") : "",
                            CifOFobUnit = (dr.GetValue<string>(dataFieldCifOFobUnit) != "" ? dr.GetValue<decimal>(dataFieldCifOFobUnit).ToString("n2", specificCulture) : ""),
                            CifImptoUnit = isVisibleCifImptoUnit ? (dr.GetValue<string>("CIFImptoUnit") != "" ? dr.GetValue<decimal>("CIFImptoUnit").ToString("n2", specificCulture) : "") : "",
                            Dua = objMiBusqueda.ExisteDua ? dr[objMiBusqueda.Dua].ToString() : "",
                            PaisOrigenODestino = objMiBusqueda.PaisOrigen ? dr["PaisOrigen"].ToString() : "",
                            DesComercial = objMiBusqueda.FlagDescComercialB ? dr["DesComercial"].ToString() : "",
                            Distrito = isVisibleDistrito ? dr["Distrito"].ToString() : ""
                        });
                    }
                }
                else
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        numero++;
                        string fecha = dr["Fechanum_date"].ToString();
                        if (!fecha.Contains("/"))
                        {
                            fecha = fecha.Substring(0, 4) + '/' + fecha.Substring(4, 2) + '/' + fecha.Substring(6, 2);
                        }
                        
                        lista.Add(new VerRegistroTableRow()
                        {
                            Numero = numero,
                            Fecha = Convert.ToDateTime(fecha).ToString("dd/MM/yyyy", specificCulture),
                            Nandina = dr["Nandina"].ToString(),
                            Exportador = objMiBusqueda.Exportador ? dr["Exportador"].ToString() : "",
                            Importador = objMiBusqueda.ImportadorExp ? dr["ImportadorExp"].ToString() : "",
                            CampoPeso = isVisibleCampoPeso ? (dr[objMiBusqueda.CampoPeso].ToString() != "" ? Convert.ToDecimal(dr[objMiBusqueda.CampoPeso]).ToString("n2", specificCulture) : "") : "",
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
                        Pto = dr.GetValue<string>(objMiBusqueda.FlagVarVisibles.ExistePtoDescarga ? "PtoDescarga" :
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

        private string GetFiltersNames()
        {
            var namesFiltros = Enum.GetNames(typeof(Enums.Filtro));
            return string.Join(",", namesFiltros);
        }

        private VerRegistroTableHead GetVerRegistroTableHead(MiBusqueda objMiBusqueda, bool flagFormatoB = false)
        {
            var isManif = objMiBusqueda.FlagVarVisibles.IsManifiesto;

            VerRegistroTableHead objVerRegistroTableHead = new VerRegistroTableHead();

            objVerRegistroTableHead.Numero = "No.";

            objVerRegistroTableHead.Nandina = Resources.Resources.Nandina_FormField_Label;
            objVerRegistroTableHead.Fecha = Resources.Resources.Date_Text;

            objVerRegistroTableHead.Importador = !isManif ? Resources.Resources.Search_Form_Item05 : Resources.Resources.Demo_Importers_Tab_Manif;
            objVerRegistroTableHead.Notificado = Resources.Resources.Demo_Notif_Tab_Fil;

            objVerRegistroTableHead.IsVisibleCampoPeso = !flagFormatoB && objMiBusqueda.CampoPeso != "";
            objVerRegistroTableHead.CampoPeso = objMiBusqueda.CampoPeso == "PesoNeto"
                ? Resources.Resources.NetKg_Text
                : (!isManif ? Resources.Resources.GrossKg_Text : Resources.Resources.PesoBruto_Text);

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
                objVerRegistroTableHead.Exportador = objMiBusqueda.CodPais != "CL"
                    ? (!isManif ? Resources.Resources.Search_Form_Item06 : Resources.Resources.Demo_Exporters_Tab_Manif)
                    : Resources.Resources.Search_Form_BrandField;
                objVerRegistroTableHead.IsVisibleExportador = objMiBusqueda.Proveedor;


                objVerRegistroTableHead.IsVisibleFobOFasUnit =
                    (objMiBusqueda.Cif != "FOB" && objMiBusqueda.CodPais != "CN" && objMiBusqueda.CodPais != "MXD" &&
                     objMiBusqueda.CodPais != "IN");

                objVerRegistroTableHead.CifOFobUnit = "US$ " + objMiBusqueda.Cif + " Unit.";

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

            return objVerRegistroTableHead;
        }
        [SessionExpireFilter]
        [HttpPost]
        public JsonResult VerRegistrosPageIndexChanging(int pagina, string codPais, string codPais2,
            string tipoOpe)
        {
            string idioma = GetCurrentIdioma();

            var isManif = IsManifiesto(codPais);

            var tabla = "";
            var dua = "";
            GetTablaAndDua(tipoOpe, codPais, ref tabla, ref dua);

            string sqlFinal = "";

            DataTable dtRegistros = FuncionesBusiness.BuscarRegistros(codPais2, tipoOpe, tabla,
                Session["SqlFiltroR2"].ToString(),
                Session["hdfIdGrupoB"]?.ToString(), ref sqlFinal);

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

            if (Session["Plan"].ToString() == "ESENCIAL" && !isManif)
            {
                objMiBusqueda.ImportadorExp = false;
            }

            objMiBusqueda.ExisteAduana = existeAduana;
            objMiBusqueda.ExisteDua = existeDua;
            objMiBusqueda.Dua = dua;

            List<VerRegistroTableRow> listaVerRegistroTableRows = GetListaVerRegistroTableRow(dtRegistros,
                objMiBusqueda, GetSpecificCulture(), idioma,
                (sqlFinal.Contains("IdMarca") || sqlFinal.Contains("IdModelo")));

            VerRegistroTable objVerRegistroTable = new VerRegistroTable
            {
                TipoOpe = tipoOpe,
                VerRegistroTableHead = GetVerRegistroTableHead(objMiBusqueda,
                    (sqlFinal.Contains("IdMarca") || sqlFinal.Contains("IdModelo"))),
                PagedListTableRows = listaVerRegistroTableRows.ToPagedList(pagina, VerRegistrosPageSize),
                MiBusqueda = objMiBusqueda
            };


            return Json(new
            {
                filasVerRegistro =
                    RenderViewToString(ControllerContext, "GridViews/VerRegistroRows" + (isManif ? "_Manif" : ""), objVerRegistroTable)
            });
        }
        [SessionExpireFilter]
        [HttpPost]
        public JsonResult BuscarPorDuaVerRegistros(string txtDuaB, string tipoOpe, string codPais,
            string codPais2, string desde, string hasta,
            int indexCboPaisB, string codPaisB,string tipoBusqueda)
        {
            string auxCodPais = codPais;
            ValidaCodPais2(codPais2, ref codPais);
            ValidaCodPaisManif(ref codPais, tipoOpe);
            var isManif = IsManifiesto(codPais);

            Session["hdfDUAB2"] = txtDuaB.Trim();
            Session["hdfDesComercialBB2"] = null;

            var tabla = "";
            var dua = "";
            GetTablaAndDua(tipoOpe, codPais, ref tabla, ref dua);

            Session["SqlFiltroR2"] = GeneraSqlFiltroR2(tipoOpe, codPais, codPais2, desde, hasta,
                indexCboPaisB, codPaisB, dua,tipoBusqueda, auxCodPais);

            string sqlFinal = "";
            DataTable dtRegistros = FuncionesBusiness.BuscarRegistros(codPais2, tipoOpe, tabla,
                Session["SqlFiltroR2"].ToString(),
                Session["hdfIdGrupoB"]?.ToString(), ref sqlFinal);

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

                if (Session["Plan"].ToString() == "ESENCIAL" && !isManif)
                {
                    objMiBusqueda.ImportadorExp = false;
                }

                objMiBusqueda.ExisteAduana = existeAduana;
                objMiBusqueda.ExisteDua = existeDua;
                objMiBusqueda.Dua = dua;

                string idioma = Session["Idioma"].ToString();
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

                tablaVerRegistro = RenderViewToString(ControllerContext, "GridViews/VerRegistroRows" + (isManif ? "_Manif" : ""), objVerRegistroTable);
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

        private void ActualizarSessionListaFitros(List<OptionSelect> nuevosFiltros, ref List<OptionSelect> agregarFiltros)
        {
            List<OptionSelect> listFilters;
            agregarFiltros = new List<OptionSelect>();
            if (Session["lstFiltros"] != null)
                listFilters = Session["lstFiltros"] as List<OptionSelect>;
            else

                listFilters = new List<OptionSelect>();

            if (listFilters != null)
            {
               
                foreach (var item in nuevosFiltros)
                {
                    if (!listFilters.Exists(x => x.text == item.text))
                    {
                        listFilters.Add(new OptionSelect
                        {
                            value = item.value,
                            text = item.text
                        });

                        agregarFiltros.Add(new OptionSelect
                            {
                                value = item.value,
                                text = item.text
                            });
                    }
                }
            }

            if (listFilters != null && listFilters.Count > 0)
                Session["lstFiltros"] = listFilters;

            else
                Session.Remove("lstFiltros");
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

        [HttpPost]
        public JsonResult VerSentinel(string nroDoc, string codPais, string tipoOpe, string desde,
            string hasta,string tipoBusqueda)
        {
            string Usuario = "07789552";
            string Password = Resources.Resources.SentinelPassword;
            int Servicio = 130773;

            string TipoDoc = "R";

            CultureInfo cultureInfo = new CultureInfo("es-PE");

            System.Net.ServicePointManager.SecurityProtocol = (System.Net.SecurityProtocolType)(0xc00);
            SentinelPeru.SentinelWS02 ws = new SentinelPeru.SentinelWS02();
            SentinelPeru.CNSDTConRapWS resultado = ws.Execute(Usuario, Password, Servicio, TipoDoc, nroDoc);

            InfoSentinel objInfoSentinel = new InfoSentinel();

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

            Funciones.GrabaLog(Session["IdUsuarioFree"].ToString(), codPais, tipoOpe, desde, hasta, "Sentinel",
                logs);
            Session["hdfIdLog"] = FuncionesBusiness.ObtieneMaxIdLog(IdUsuario, "Sentinel");
            Session["hdfLogs"] = logs;

            return Json(new
            {
                infoSentinel = RenderViewToString(ControllerContext, "Modals/_InfoSentinel", objInfoSentinel)
            });
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult AgregarAFavoritosClick(string filtro, string idTabla, string codPais,
            string codPais2, string tipoOpe, string idsSeleccionados,
            string idsPagina, string idioma)
        {
            ValidaCodPais2(codPais2, ref codPais);
            ValidaCodPaisManif(ref codPais, tipoOpe);

            var isManif = IsManifiesto(codPais);

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
                        tituloFavoritos = Resources.Resources.Demo_Exporters_Tab;
                        lblAgregarAFavorito = Resources.AdminResources.Add_ToMyExporters_Button;
                        break;
                }

                string codPaisT = codPais;
                if (codPais == "PEB" || codPais == "PEP")
                    codPaisT = "PE";
                else if (codPais2 == "4UE")
                    codPaisT = "UE" + codPais;


                DataTable dt = FuncionesBusiness.LlenaGrupos(false, Session["IdUsuarioFree"].ToString(), codPaisT, tipoOpe,
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
                listaGrupos
            });
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult OrdenarTabla(string filtro, Enums.TipoFiltro tipoFiltro, string orden,
            string codPais, string tipoOpe, int indexCboPaisB)
        {
            string auxCodPais2 = Session["CodPais"].ToString(); // al cambiar cboPais se actualiza esta variable de sesión
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
                    FuncionesBusiness.CalculaTotalesFreemium(Session["SqlFiltro"].ToString(), cifTot, codPais, pesoNeto, tabla, true, isManif: IsManifiesto);
                valueCifTotMarcaModelo = drTotales.GetValue<decimal>(cifTot);
            }
            else
            {
                GetCifTot_PesoNeto_CantReg_ValueCifTot_Unidad_ValuePesoNeto(codPais, tipoOpe, tabla, ref cifTot,
                    ref pesoNeto, ref cantReg, ref valueCifTot, ref unidad, ref valuePesoNeto);
            }

            var listaAux = CargaFiltro(filtro, -1, codPais, cifTot, pesoNeto, idioma, dua, tabla,
                indexCboPaisB, tipoFiltro, orden);

            bool ordenarTabla = false;
            string filasOrdenas = "";
            if (listaAux != null && listaAux[0] != null && listaAux[0].Rows.Count > 1)
            {
                ordenarTabla = true;
                TabData objTabData = new TabData();
                objTabData.Filtro = filtro;

                if (tipoFiltro == Enums.TipoFiltro.Resumen)
                {
                    objTabData.ListRows =
                        GetDataTableToListGridRow(listaAux[0], filtro, cifTot, valueCifTot, codPais, GetSpecificCulture());
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
                            objTabData.ListRowsTab = GetDataTableToListGridRowTab(listaAux[0], filtro, cifTot, valueCifTot, codPais,
                            GetSpecificCulture(), pesoNeto, unidad);
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
                        IsVisibleSentinel = false
                    });
                }
            }

            return lista;
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult AgregarFiltrosClick(string filtro, string idTabla, string codPais,
            string tipoOpe, string codPais2, string desde,
            string hasta, int indexCboPaisB, string codPaisB,
            string idsSeleccionados, string idsPagina, int numFiltrosExistentes, string idioma , string tipoBusqueda)
        {
            bool maximoLimiteFiltros = false;

            object objMensaje = null;



            if (numFiltrosExistentes == LimiteFiltros)
            {
                objMensaje = new
                {
                    titulo = "Buscar",
                    mensaje = "Se ha llegado al maximo número de filtros: <b>" + LimiteFiltros +
                              "</b>.<br>Por favor elimine alguna palabra de la busqueda actual",
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

            string[] arrayIdsSeleccionados = idsSeleccionados.Split(new char[] { ',' });
            string[] arrayIdsPagina = idsPagina.Split(new char[] { ',' });

            bool existenFiltros = false;

            listaIdsSeleccionados = FuncionesBusiness.GuardaSeleccionadosAbuscar(filtro, listaIdsSeleccionados,
                arrayIdsSeleccionados, arrayIdsPagina, (ArrayList)Session[GetIdSessionByFiltro(filtro)], ref existenFiltros);

            //if (idTabla == "Aduanas" && !existeDua)
            //    idGdv = "AduanaDUAs";

            bool hideTabExcel = false;
            List<OptionSelect> nuevosFiltros = null;
            List<OptionSelect> agregarFiltros = null;
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
                }


                ActualizarSessionListaFitros(nuevosFiltros, ref agregarFiltros);

                var tabla = "";
                var dua = "";

                if (agregarFiltros != null && agregarFiltros.Count > 0)
                {
                    GetTablaAndDua(tipoOpe, codPais, ref tabla, ref dua);


                    Session["SqlFiltro"] = GeneraSqlFiltro(codPais, tipoOpe, tipoBusqueda, desde, hasta,
                        dua, codPaisB);
                    Session["UltSqlFiltro"] = Session["SqlFiltro"];

                    string sql = Session["SqlFiltro"].ToString();

                    string cifTot = "";
                    string pesoNeto = "";
                    string valueCifTot = "";

                    decimal valuePesoNeto = 0;
                    string unidad = "";
                    GetCifTot_PesoNeto_CantReg_ValueCifTot_Unidad_ValuePesoNeto(codPais, tipoOpe, tabla, ref cifTot,
                        ref pesoNeto, ref cantReg, ref valueCifTot, ref unidad, ref valuePesoNeto);

                    bool FlagPalabras = (!string.IsNullOrWhiteSpace(Session["hdfPalabrasY"]?.ToString()));
                    hideTabExcel = cantReg > CantRegMax && !FlagPalabras;


                    listGridData = Busca(codPais, tipoOpe, cifTot, pesoNeto, idioma, dua, tabla, indexCboPaisB, valueCifTot,
                        valuePesoNeto, unidad,
                        cantReg, specificCulture, hideTabExcel);
                }
                else
                {
                    agregarFiltros = null;
                    return Json(new

                    {
                        maximoLimiteFiltros,
                        existenFiltros,
                        objMensaje,
                        nuevosFiltros,
                        gridData = listGridData,
                        hideTabExcel = hideTabExcel,
                        agregarFiltros 
                    });
                }
               
                
            }

            return Json(new

            {
                maximoLimiteFiltros,
                existenFiltros,
                objMensaje,
                nuevosFiltros,
                totalRecordsFound = GetDescriptionRecordsFound(cantReg, specificCulture, idioma),
                gridData = listGridData,
                hideTabExcel = hideTabExcel,
                agregarFiltros
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
                case "ViaTransp":
                    idSession = "ViasTranspB";
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
                case "PtoDescarga":
                    idSession = "PtosDescargaB";
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

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult CboFiltroSelectedIndexChanged(string filtro, string valorFiltro, string tipoOpe,
            string codPais, string codPais2, int indexCboPaisB,
            string codPaisB, string desde, string hasta, int numFiltrosExistentes,
            string idioma,string tipoBusqueda)
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
            bool hideTabExcel = cantReg > CantRegMax && !FlagPalabras;

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


                Session["SqlFiltro"] = GeneraSqlFiltro(codPais, tipoOpe, tipoBusqueda, desde, hasta,
                     dua, codPaisB);
                Session["UltSqlFiltro"] = Session["SqlFiltro"];
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
                totalRecordsFound = GetDescriptionRecordsFound(cantReg, specificCulture, idioma),
                gridData = listGridData,
                hideTabExcel = hideTabExcel,
            });
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult BuscarPorDesComercialVerRegistro(string txtDesComercialBB, string tipoOpe, string codPais,
            string codPais2, string desde, string hasta,
            int indexCboPaisB, string codPaisB,string tipoBusqueda)
        {
            string auxCodPais = codPais;
            ValidaCodPais2(codPais2, ref codPais);
            ValidaCodPaisManif(ref codPais, tipoOpe);

            var isManif = IsManifiesto(codPais);

            Session["hdfDesComercialBB2"] = txtDesComercialBB.Trim();
            Session["hdfDUAB2"] = null;

            var tabla = "";
            var dua = "";
            GetTablaAndDua(tipoOpe, codPais, ref tabla, ref dua);

            Session["SqlFiltroR2"] = GeneraSqlFiltroR2(tipoOpe, codPais, codPais2, desde, hasta,
                indexCboPaisB, codPaisB, dua,tipoBusqueda, auxCodPais);

            string sqlFinal = "";
            DataTable dtRegistros = FuncionesBusiness.BuscarRegistros(codPais2, tipoOpe, tabla,
                Session["SqlFiltroR2"].ToString(),
                Session["hdfIdGrupoB"]?.ToString(), ref sqlFinal);

            object objMensaje = null;
            var tablaVerRegistro = "";
            var totalPages = 0;
            var resultadoDesComercialVerRegistro = "";
            var registrosEncontrados = dtRegistros.Rows.Count;

            if (registrosEncontrados > 0)
            {
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

                if (Session["Plan"].ToString() == "ESENCIAL" && !isManif)
                {
                    objMiBusqueda.ImportadorExp = false;
                }

                objMiBusqueda.ExisteAduana = existeAduana;
                objMiBusqueda.ExisteDua = existeDua;
                objMiBusqueda.Dua = dua;

                string idioma = Session["Idioma"].ToString();

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
        

        private string GeneraSqlFiltro(string codPais, string tipoOpe,
            string tipoBusqueda, string desde, string hasta, string dua, string codPais2)
        {
            string sql = tipoBusqueda == "T" ?
                          "and Trimestre between " + desde + " and " + hasta + " " :
                          "and Trimestre between " + desde + "1 and " + hasta + "4 ";

            //Session["FechaIni"] = anioMesIni + "00";
            //Session["FechaFin"] = anioMesFin + "99";
            if (!string.IsNullOrEmpty(codPais2.ToString()) && existePaisFiltro && !codPais2.Equals("0"))
            {
                sql += tipoOpe == "I" ?
                         "and IdPaisOrigen = " + codPais2 + " " :
                        "and IdPaisDestino = " + codPais2 + " ";
            }


            if (!string.IsNullOrWhiteSpace(Session["hdfPalabrasY"]?.ToString()))
            {
                sql += "and contains(Descomercial, '";
                string[] palabrasY = Session["hdfPalabrasY"].ToString().Split(' ');
                bool inicio = true;
                foreach (string palabra in palabrasY)

                {
                    if (inicio)
                    {
                        sql += "\"" + palabra + "*\" ";
                        inicio = false;
                    }
                    else
                        sql += "and \"" + palabra + "*\" ";
                }

                sql += "') ";
            }

            if (Session["hdfNandinaB"] != null)
            {
                sql += "and (";
                string[] nandinas = Session["hdfNandinaB"].ToString().Split('|');
                foreach (string nandina in nandinas)

                {
                    bool existeNandina = (Funciones.BuscaIdPartida(nandina, codPais) != "");
                    if (existeNandina)
                        sql += "IdPartida = " + Funciones.BuscaIdPartida(nandina, codPais) + " or ";
                    else

                        sql += "IdPartida in (select IdPartida from Partida_" + codPais + " where Nandina like '" +
                               nandina + "%') or ";
                }


                sql = sql.Substring(0, sql.Length - 3) + ") ";
            }

            if (Session["hdfImportadorB"] != null)

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


            if (Session["hdfExportadorB"] != null)
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




            if (Session["PartidasB"] != null)
                sql += "and IdPartida in " + Funciones.ListaItems((ArrayList)Session["PartidasB"]) + " ";
            if (Session["MarcasB"] != null)
                sql += "and IdMarca in " + Funciones.ListaItems((ArrayList)Session["MarcasB"]) + " ";
            if (Session["ModelosB"] != null)

                sql += "and IdModelo in " + Funciones.ListaItems((ArrayList)Session["ModelosB"]) + " ";

            if (Session["ImportadoresB"] != null)

                sql += "and IdImportador in " + Funciones.ListaItems((ArrayList)Session["ImportadoresB"]) + " ";

            if (Session["ExportadoresB"] != null)
                sql += "and IdExportador in " + Funciones.ListaItems((ArrayList)Session["ExportadoresB"]) + " ";
            if (Session["ProveedoresB"] != null)
                sql += "and IdProveedor in " + Funciones.ListaItems((ArrayList)Session["ProveedoresB"]) + " ";
            if (Session["ImportadoresExpB"] != null)
                sql += "and IdImportadorExp in " + Funciones.ListaItems((ArrayList)Session["ImportadoresExpB"]) + " ";
            if (Session["PaisesOrigenB"] != null)
                sql += "and IdPaisOrigen in " + Funciones.ListaItems((ArrayList)Session["PaisesOrigenB"]) + " ";
            if (Session["PaisesDestinoB"] != null)
                sql += "and IdPaisDestino in " + Funciones.ListaItems((ArrayList)Session["PaisesDestinoB"]) + " ";
            if (Session["ViasTranspB"] != null)
                sql += "and IdViaTransp in " + Funciones.ListaItems((ArrayList)Session["ViasTranspB"]) + " ";
            if (Session["AduanaDUAsB"] != null)
                sql += "and convert(varchar(3), IdAduana) + '-' + convert(varchar(20), " + dua + ") in " +
                        Funciones.ListaItemsS((ArrayList)Session["AduanaDUAsB"]) + " ";
            if (Session["AduanasB"] != null)
                sql += "and IdAduana in " + Funciones.ListaItems((ArrayList)Session["AduanasB"]) + " ";
            //if (Session["DUAsB"] != null)
            //    sql += "and " + DUA + " like '" + Functions.ListaItems((ArrayList)Session["DUAsB"]).Replace("(", "").Replace(")", "") + "%' ";
            if (Session["DistritosB"] != null)
                sql += "and IdDistrito in " + Funciones.ListaItems((ArrayList)Session["DistritosB"]) + " ";


            return sql;
        }

        /*private string GeneraSqlFiltro(string codPais, string codPais2, string tipoOpe,
            string anioMesIni, string anioMesFin, int indexCboPaisB,
            string codPaisB, string dua, string auxCodPais = "")
        {
            bool isManif = IsManifiesto(codPais);
            string sql = "";

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
                if (tipoOpe == "I")
                    sql += "and " + (!isManif ? "IdPaisOrigen" : "IdPaisEmbarque") + " = " + codPaisB + " ";
                else

                {
                    sql += "and IdPaisDestino = " + codPaisB + " ";
                }
            }

            if (!string.IsNullOrWhiteSpace(Session["hdfPalabrasY"]?.ToString()))
            {
                sql += "and contains(Descomercial, '";
                string[] palabrasY = Session["hdfPalabrasY"].ToString().Split(' ');
                bool inicio = true;
                foreach (string palabra in palabrasY)

                {
                    if (inicio)
                    {
                        sql += "\"" + palabra + "*\" ";
                        inicio = false;
                    }
                    else
                        sql += "and \"" + palabra + "*\" ";
                }

                sql += "') ";
            }

            if (Session["hdfNandinaB"] != null)
            {
                sql += "and (";
                string[] nandinas = Session["hdfNandinaB"].ToString().Split('|');
                foreach (string nandina in nandinas)

                {
                    bool existeNandina = (Funciones.BuscaIdPartida(nandina, codPais) != "");
                    if (existeNandina)
                        sql += "IdPartida = " + Funciones.BuscaIdPartida(nandina, codPais) + " or ";
                    else

                        sql += "IdPartida in (select IdPartida from Partida_" + codPais + " where Nandina like '" +
                               nandina + "%') or ";
                }


                sql = sql.Substring(0, sql.Length - 3) + ") ";
            }

            if (Session["hdfImportadorB"] != null)

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
                        sql += "and IdImportador in (select IdEmpresa from Empresa_PE_con_datos where 1 = 1 ";
                    else
                        sql += "and IdImportador in (select IdEmpresa from Empresa_" + codPais + " where 1 = 1 ";

                    string[] Palabras = Session["hdfImportadorB"].ToString().Split(' ');
                    foreach (string Palabra in Palabras)
                        sql += "and Empresa like '%" + Palabra + "%' ";
                    sql += ") ";
                }
            }


            if (Session["hdfExportadorB"] != null)
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


            if (Session["hdfProveedorB"] != null)
            {
                if (!isManif)
                {
                    sql += "and IdProveedor in (select IdProveedor from Proveedor_" + codPais + " where ";

                    string[] palabras = Session["hdfProveedorB"].ToString().Split('|');

                    foreach (string palabra in palabras)
                        sql += "Proveedor like '%" + palabra + "%' or ";
                    sql = sql.Substring(0, sql.Length - 3) + ")";
                }
                else
                {
                    sql += "and IdProveedor in (select IdProveedor from Proveedor_" + codPais + " PR where 1 = 1 ";
                    string[] Palabras = Session["hdfProveedorB"].ToString().Split(' ');
                    foreach (string Palabra in Palabras)
                        sql += "and Proveedor like '%" + Palabra + "%' ";
                    sql += ") ";
                }

            }

            if (Session["hdfImportadorExpB"] != null)
            {
                if (!isManif)
                {
                    sql += "and IdImportadorExp in (select IdImportadorExp from ImportadorExp_" + codPais + " where ";

                    string[] palabras = Session["hdfImportadorExpB"].ToString().Split('|');

                    foreach (string palabra in palabras)
                        sql += "ImportadorExp like '%" + palabra + "%' or ";
                    sql = sql.Substring(0, sql.Length - 3) + ")";
                }
                else
                {
                    sql += "and IdImportadorExp in (select IdImportadorExp from ImportadorExp_" + codPais +
                           " IE where 1 = 1 ";
                    string[] Palabras = Session["hdfImportadorExpB"].ToString().Split(' ');
                    foreach (string Palabra in Palabras)
                        sql += "and IE.ImportadorExp like '%" + Palabra + "%' ";
                    sql += ") ";
                }
            }

            if (!isManif)
            {
                if (Session["PartidasB"] != null)
                    sql += "and IdPartida in " + Funciones.ListaItems((ArrayList)Session["PartidasB"]) + " ";
                if (Session["MarcasB"] != null)
                    sql += "and IdMarca in " + Funciones.ListaItems((ArrayList)Session["MarcasB"]) + " ";
                if (Session["ModelosB"] != null)

                    sql += "and IdModelo in " + Funciones.ListaItems((ArrayList)Session["ModelosB"]) + " ";

                if (Session["ImportadoresB"] != null)

                    sql += "and IdImportador in " + Funciones.ListaItems((ArrayList)Session["ImportadoresB"]) + " ";

                if (Session["ExportadoresB"] != null)
                    sql += "and IdExportador in " + Funciones.ListaItems((ArrayList)Session["ExportadoresB"]) + " ";
                if (Session["ProveedoresB"] != null)
                    sql += "and IdProveedor in " + Funciones.ListaItems((ArrayList)Session["ProveedoresB"]) + " ";
                if (Session["ImportadoresExpB"] != null)
                    sql += "and IdImportadorExp in " + Funciones.ListaItems((ArrayList)Session["ImportadoresExpB"]) + " ";
                if (Session["PaisesOrigenB"] != null)
                    sql += "and IdPaisOrigen in " + Funciones.ListaItems((ArrayList)Session["PaisesOrigenB"]) + " ";
                if (Session["PaisesDestinoB"] != null)
                    sql += "and IdPaisDestino in " + Funciones.ListaItems((ArrayList)Session["PaisesDestinoB"]) + " ";
                if (Session["ViasTranspB"] != null)
                    sql += "and IdViaTransp in " + Funciones.ListaItems((ArrayList)Session["ViasTranspB"]) + " ";
                if (Session["AduanaDUAsB"] != null)
                    sql += "and convert(varchar(3), IdAduana) + '-' + convert(varchar(20), " + dua + ") in " +
                           Funciones.ListaItemsS((ArrayList)Session["AduanaDUAsB"]) + " ";
                if (Session["AduanasB"] != null)
                    sql += "and IdAduana in " + Funciones.ListaItems((ArrayList)Session["AduanasB"]) + " ";
                //if (Session["DUAsB"] != null)
                //    sql += "and " + DUA + " like '" + Functions.ListaItems((ArrayList)Session["DUAsB"]).Replace("(", "").Replace(")", "") + "%' ";
                if (Session["DistritosB"] != null)
                    sql += "and IdDistrito in " + Funciones.ListaItems((ArrayList)Session["DistritosB"]) + " ";
            }
            else
            {
                if (Session["ImportadoresB"] != null)
                    sql += "and IdImportador in " + Funciones.ListaItems((ArrayList)Session["ImportadoresB"]) + " ";
                if (Session["NotificadosB"] != null)
                    sql += "and IdNotificado in " + Funciones.ListaItems((ArrayList)Session["NotificadosB"]) + " ";
                if (Session["ExportadoresB"] != null)
                    sql += "and IdExportador in " + Funciones.ListaItems((ArrayList)Session["ExportadoresB"]) + " ";
                if (Session["ProveedoresB"] != null)
                    sql += "and IdProveedor in " + Funciones.ListaItems((ArrayList)Session["ProveedoresB"]) + " ";
                if (Session["ImportadoresExpB"] != null)
                    sql += "and IdImportadorExp in " + Funciones.ListaItems((ArrayList)Session["ImportadoresExpB"]) + " ";
                if (Session["PaisesOrigenB"] != null)
                    sql += "and IdPaisOrigen in " + Funciones.ListaItems((ArrayList)Session["PaisesOrigenB"]) + " ";
                if (Session["PaisesEmbarqueB"] != null)
                    sql += "and IdPaisEmbarque in " + Funciones.ListaItems((ArrayList)Session["PaisesEmbarqueB"]) + " ";
                if (Session["PaisesDestinoB"] != null)
                    sql += "and IdPaisDestino in " + Funciones.ListaItems((ArrayList)Session["PaisesDestinoB"]) + " ";
                if (Session["PtosDescargaB"] != null)
                    sql += "and IdPtoDescarga in " + Funciones.ListaItems((ArrayList)Session["PtosDescargaB"]) + " ";
                if (Session["PtosEmbarqueB"] != null)
                    sql += "and IdPtoEmbarque in " + Funciones.ListaItems((ArrayList)Session["PtosEmbarqueB"]) + " ";
                if (Session["PtosDestinoB"] != null)
                    sql += "and IdPtoDestino in " + Funciones.ListaItems((ArrayList)Session["PtosDestinoB"]) + " ";
                if (Session["ManifiestosB"] != null)
                    sql += "and Manifiesto in " + Funciones.ListaItemsS((ArrayList)Session["ManifiestosB"]) + " ";
            }


            if (Session["hdfIdGrupoB"] != null)
                sql += "and Id" + Session["hdfTipoFavoritoB"].ToString() +
                       " in (select IdFavorito from FavoritoGrupo where IdGrupo = " + Session["hdfIdGrupoB"] + ") ";

            return sql;
        }*/

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


            DataTable dt = FuncionesBusiness.LlenaGrupos(false, Session["IdUsuarioFree"].ToString(), codPaisT, tipoOpe,
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
        private CultureInfo GetSpecificCulture()
        {
            string c = Session["culture"].ToString();
            return Session["culture"].ToString().Equals("es")
                ? CultureInfo.CreateSpecificCulture("es-pe")
                : CultureInfo.CreateSpecificCulture("en-us");
        }

        [HttpPost]
        public ActionResult SetCulture(string culture)
        {
            culture = CultureHelper.GetImplementedCulture(culture);
            RouteData.Values["culture"] = culture; // set culture 
            return RedirectToAction("Index","Freemium", new {culture ,  cod = Session["codigoOperacion"].ToString() });
        }


        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            string cultureName = RouteData.Values["culture"] as string;

            // Attempt to read the culture cookie from Request
            if (cultureName == null)
                cultureName = Request.UserLanguages != null && Request.UserLanguages.Length > 0 ? Request.UserLanguages[0] : null; // obtain it from HTTP header AcceptLanguages

            // Validate culture name
            cultureName = CultureHelper.GetImplementedCulture(cultureName); // This is safe


            if (RouteData.Values["culture"] as string != cultureName)
            {

                // Force a valid culture in the URL
                RouteData.Values["culture"] = cultureName.ToLowerInvariant(); // lower case too

                // Redirect user
                Response.RedirectToRoute(RouteData.Values);
            }

            // Modify current thread's cultures            
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            return base.BeginExecuteCore(callback, state);
        }


        [HttpPost]
        public ActionResult MostrarConsole()
        {
            var listaFiltro = Session["lstFiltros"] as List<OptionSelect>;
            var sql = Session["UltSqlFiltro"]??"";

            return Json(new
            {
                listaFiltro,
                sql
            });
        }

        
    }
}