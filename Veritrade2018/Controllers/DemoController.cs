using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Veritrade2018.Controllers.Admin;
using Veritrade2018.Helpers;
using Veritrade2018.Models;
using Veritrade2018.Models.Admin;

namespace Veritrade2018.Controllers
{
    public class DemoController : MisBusquedasController
    {
        private readonly string[] _CodManifiestosModificado = new[] { "PEI", "PEE", "ECI", "ECE", "USI", "USE", "BRI", "BRE" };
        // GET: Demo
        public ActionResult Index3(string culture, string compra, string ruta)
        {
            //Session["IsDemo"] = true;
            Restablecer();
            string url = Request.Url.AbsoluteUri;
            Dictionary<string, string> queryValues = Extensiones.GetQueryValues(url);

            bool opcion = (queryValues.ContainsKey("acc"));

            Extensiones.SetCookie("IsDemo","1", TimeSpan.FromDays(360));

            string CodPaisIP = "";
            string CodPais = "PE";
            string CodPais2 = "1LAT";
            string TipoOpe = "I";

            VeritradeAdmin.Seguridad _ws = new VeritradeAdmin.Seguridad();
#if DEBUG

            string DireccionIP = Properties.Settings.Default.IP_Debug;
#else
            string DireccionIP = Request.ServerVariables["REMOTE_ADDR"];

#endif

            _ws.BuscaUbicacionIP2(DireccionIP, ref CodPaisIP);

            

            string[] auxArrayCodPaisIP = { "AR", "BO", "BR", "CL", "CO", "CR", "EC", "MX", "PA", "PY", "PE", "UY" };


            if (Array.Exists(auxArrayCodPaisIP, x => x == CodPaisIP))
            {
                CodPais = CodPaisIP;
            }
            if (queryValues.ContainsKey("cp"))
            {
                CodPais = queryValues["cp"];
            }
            CodPais2 = new ListaPaises().BuscarCodPais2(CodPais);
            CodPais2 = (CodPais2 == "") ? "1LAT" : CodPais2;
            if (CodPais == "UE") CodPais2 = "4UE";
            List<object> nuevoFiltro = new List<object>();
            
            if (opcion)
            {
                CodPais = queryValues["cp"];
                CodPais2 = new ListaPaises().BuscarCodPais2(CodPais);
                CodPais2 = (CodPais2 == "") ? "1LAT" : CodPais2;
                TipoOpe = queryValues["to"];
                Session["CodPais2"] = CodPais2;
                if(queryValues["acc"] == "demo")
                {
                    if (queryValues["op"] == "DES")
                    {
                        nuevoFiltro.Add(AgregaPalabrasFiltros(queryValues["nom"], "1DE", queryValues["cp"], queryValues["to"]));
                    }else if(queryValues["op"] == "PAR")
                    {
                        if(queryValues["par"].ToString() != "")
                        {
                            nuevoFiltro.Add(AgregaPalabrasFiltros(queryValues["par"], "2P_", queryValues["cp"], queryValues["to"], culture));
                        }
                        else
                        {
                            nuevoFiltro.Add(AgregaPalabrasFiltros(queryValues["nom"], "2P_", queryValues["cp"], queryValues["to"], culture));
                        }
                        
                    }
                }

            }

            MiBusqueda objMisBusqueda = new MiBusqueda
            {
                TipoOpe = TipoOpe,
                CodPais = CodPais,
                CodPais2 = CodPais2,
                EsFreeTrial = true,
            };

            
            var IdUsuario = Funciones.GetIdUserFreeTrial();
            if (!string.IsNullOrEmpty(IdUsuario))
            {
                Session["IdUsuario"] = IdUsuario;
                Session["TipoUsuario"] = Funciones.BuscaTipoUsuario(IdUsuario);
                Session["Plan"] = Funciones.ObtienePlan(IdUsuario).ToUpper();
                Session["Origen"] = Funciones.ObtieneOrigen(IdUsuario).ToUpper();
            }
            bool tipoUsuarioEsFreeTrial = (Session["TipoUsuario"] != null && Session["TipoUsuario"].ToString() == "Free Trial") || (bool)(Session["opcionFreeTrial"] ?? false);
            string Idioma = culture;
            objMisBusqueda.RangoInfoEnLinea = FuncionesBusiness.InfoEnLinea(CodPais, TipoOpe, Idioma, tipoUsuarioEsFreeTrial);
            string rangoFreeTrial = FuncionesBusiness.InfoEnLinea(CodPais, TipoOpe, Idioma, false);

            objMisBusqueda.Periodos = CargaPeriodos(CodPais, TipoOpe);

            Session["culture"] = culture;
            Session["TipoOpe"] = TipoOpe;
            ViewData["TipoOpe"] = TipoOpe;
            ViewData["CodPais"]  = Session["CodPais"] = CodPais;
            ViewData["CodPais2"] = Session["CodPais2"]  = CodPais2;
            ViewData["objMiBusqueda"] = objMisBusqueda;
            ViewData["rangoFreeTrial"] = rangoFreeTrial;
            ViewData["IsDemo"] = true;
            ViewData["listFilter"] = nuevoFiltro;

            return View("~/Views/Demo/Index.cshtml");
        }

        private void ValidaCodPais2(string codPais2, ref string codPais)
        {
            if (codPais2 == "4UE")
                codPais = "UE";
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

                    PalAux = PalAux.Replace("[TODOS]", "");
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

            /*Session["isVisibleInfoComplementario"] = Session["hdfPalabrasY"] == null && (Session["hdfNandinaB"] != null || Session["PartidasB"] != null) && Session["hdfImportadorB"] == null &&
                                                        Session["hdfExportadorB"] == null && Session["hdfProveedorB"] == null && Session["hdfImportadorExpB"] == null &&
                                                        Session["MarcasB"] == null && Session["ModelosB"] == null && Session["ImportadoresB"] == null && Session["ExportadoresB"] == null &&
                                                        Session["ProveedoresB"] == null && Session["ImportadoresExpB"] == null && Session["PaisesOrigenB"] == null && Session["PaisesDestinoB"] == null &&
                                                        Session["MarcaECB"] == null && Session["ViasTranspB"] == null && Session["AduanaDUAsB"] == null && Session["AduanasB"] == null &&
                                                        Session["DistritosB"] == null && Session["PtosEmbarqueB"] == null && Session["PtosDestinoB"] == null && !(bool)(Session["codPaisB"] ?? false);
*/
            ActualizarSessionListaFitros(valueOption, textOption);


            return new

            {
                text = textOption,
                value = valueOption
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

            if (Session["codPaisB"] != null)
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
    }
}