using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Veritrade2017.Helpers;
using Veritrade2017.Models;

namespace Veritrade2017.Controllers
{
    public class ConsultaController : BaseController
    {

        private readonly VeritradeAdmin.Seguridad _ws = new VeritradeAdmin.Seguridad();
        private readonly string[] _CodManifiestosModificado = new[] { "PEI", "PEE", "ECI", "ECE", "USI", "USE", "BRI", "BRE" };

        public bool ExisteImportador, ExisteProveedor, ExistePaisOrigen;
        public bool ExisteExportador, ExisteImportadorExp;

        int CantRegMax = 200000;

        // GET: Consulta
        [HttpGet]
        public ActionResult Index(string culture, string codPaisParam)
        {
            //if (culture.Equals("es"))
            //{
            //    return Redirect(SettingUtility.GetUrlBack() + "/ConsultaGratis.aspx");
            //}
            //else
            //{
            //    return Redirect(SettingUtility.GetUrlBack() + "/en/FreeSearch.aspx");
            //}

            string Origen = "";
            string codPais = "";
            string tipoOpe = "";

            ViewBag.Menu = "consulta";
            if (Session["Origen"] != null)
                Origen = Session["Origen"].ToString();

            //if (Origen == "")
            //    Restablecer();


            //ViewData["CodCampaña"] = culture.Equals("es") ? "12100" : "13100";

            string c = "";
            if (Session["c"] != null) c = Session["c"].ToString();

            var codPaisIp = string.Empty;
            if (Session["CodPaisIP"] != null) codPaisIp = Session["CodPaisIP"].ToString();
            if(codPaisParam != "")
            {
                ViewData["codigoPais"] = codPaisParam;
                codPais = codPaisParam;
                Session["CodPais2"] = null;
            }
            else if (Session["CodPais"] != null)
            {
                ViewData["codigoPais"] = Session["CodPais"].ToString();
                codPais = Session["CodPais"].ToString(); ;
            }
            else if (codPaisIp == "AR" || codPaisIp == "BO" || codPaisIp == "BR" || codPaisIp == "CL" ||
                         codPaisIp == "CO" || codPaisIp == "CR" || codPaisIp == "EC" || codPaisIp == "MX" ||
                         codPaisIp == "PA" || codPaisIp == "PY" || codPaisIp == "PE" || codPaisIp == "UY")
            {
                ViewData["codigoPais"] = codPaisIp;
                codPais = codPaisIp;
            }
            else
            {
#if DEBUG
                var direccionIp = Properties.Settings.Default.IP_Debug;
#else
            var direccionIp = Request.ServerVariables["REMOTE_ADDR"];
#endif
                _ws.BuscaUbicacionIP2(direccionIp, ref codPaisIp);

                ViewData["codigoPais"] = codPaisIp;
                codPais = codPaisIp;
            }

            ViewData["idUsuario"] = string.Empty;
            ViewData["serviciosMenu"] = new Servicios().GetServicios(culture);
            ViewData["paisesMenu"] = ServiciosPaises.GetList(culture);

            //ViewData["codigoPais"] = codPaisIp;


            if (Session["TipoOpe"] != null)
            {
                ViewData["TipoOpe"] = Session["TipoOpe"].ToString();
                tipoOpe = Session["TipoOpe"].ToString();
            }
            else
            {
                ViewData["TipoOpe"] = "I";
                Session["TipoOpe"] = "I";
                tipoOpe = "I";
            }

            Session["culture"] = culture;

            //valuepais2
            string valuePaises2 = "";
            if (Session["CodPais2"] != null)
            {
                valuePaises2 = Session["CodPais2"].ToString();
            }
            else
            {
                valuePaises2 = new ListaPaises().BuscarCodPais2(codPais);
                valuePaises2 = (valuePaises2 == "") ? "1LAT" : valuePaises2;
            }
            ConsultaForm consultaForm = new ConsultaForm
            {
                Importador = Consulta.ExisteVariable(codPais, tipoOpe, "IdImportador"),
                Proveedor = Consulta.ExisteVariable(codPais, tipoOpe, "IdProveedor"),
                PaisOrigen = Consulta.ExisteVariable(codPais, tipoOpe, "IdPaisOrigen"),
                Exportador = Consulta.ExisteVariable(codPais, tipoOpe, "IdExportador"),
                ImportadorExp = Consulta.ExisteVariable(codPais, tipoOpe, "IdImportadorExp"),
                CodPais = codPais,
                TipoOpe = tipoOpe,
                FlagDescComercialB = Consulta.FlagDesComercial(codPais, tipoOpe),
                CodPais2 = valuePaises2
            };

            consultaForm.IsManifiesto = IsManifiesto(codPais);

            Session["CodPais2"] = valuePaises2;
            ViewData["metodo"] = "GET";
            ViewData["consultaForm"] = consultaForm;
            
            // Ruben 202303
            if (culture == "es")
                ViewBag.Canonical = "https://www.veritradecorp.com/es/consultas";
            else
                ViewBag.Canonical = "https://www.veritradecorp.com/en/consultas";

            return View("~/Views/Consulta/Index_2.cshtml");
        }


        [HttpPost]
        public ActionResult Index(string cboPais,string rdbTipoOpe,string cboOpcion,string txtDesComercialB, string culture,string idAutocompletado)
        {
            //if (culture.Equals("es"))
            //{
            //    return Redirect(SettingUtility.GetUrlBack() + "/ConsultaGratis.aspx");
            //}
            //else
            //{
            //    return Redirect(SettingUtility.GetUrlBack() + "/en/FreeSearch.aspx");
            //}

            string Origen = "";
            string codPais = "";
            string tipoOpe = "";

            ViewBag.Menu = "consulta";
            if (Session["Origen"] != null)
                Origen = Session["Origen"].ToString();

            //if (Origen == "")
            //    Restablecer();


            //ViewData["CodCampaña"] = culture.Equals("es") ? "12100" : "13100";

            string c = "";
            if (Session["c"] != null) c = Session["c"].ToString();

            var codPaisIp = string.Empty;
            if (Session["CodPaisIP"] != null) codPaisIp = Session["CodPaisIP"].ToString();

            if (Session["CodPais"] != null)
            {
                ViewData["codigoPais"] = Session["CodPais"].ToString();
                codPais = Session["CodPais"].ToString(); ;
            }
            else if (codPaisIp == "AR" || codPaisIp == "BO" || codPaisIp == "BR" || codPaisIp == "CL" ||
                         codPaisIp == "CO" || codPaisIp == "CR" || codPaisIp == "EC" || codPaisIp == "MX" ||
                         codPaisIp == "PA" || codPaisIp == "PY" || codPaisIp == "PE" || codPaisIp == "UY")
            {
                ViewData["codigoPais"] = codPaisIp;
                codPais = codPaisIp;
            }
            else
            {
#if DEBUG
                var direccionIp = Properties.Settings.Default.IP_Debug;
#else
            var direccionIp = Request.ServerVariables["REMOTE_ADDR"];
#endif
                _ws.BuscaUbicacionIP2(direccionIp, ref codPaisIp);

                ViewData["codigoPais"] = codPaisIp;
                codPais = codPaisIp;
            }

            ViewData["idUsuario"] = string.Empty;
            ViewData["serviciosMenu"] = new Servicios().GetServicios(culture);
            ViewData["paisesMenu"] = ServiciosPaises.GetList(culture);

            //ViewData["codigoPais"] = codPaisIp;


            if (Session["TipoOpe"] != null)
            {
                ViewData["TipoOpe"] = Session["TipoOpe"].ToString();
                tipoOpe = Session["TipoOpe"].ToString();
            }
            else
            {
                ViewData["TipoOpe"] = "I";
                Session["TipoOpe"] = "I";
                tipoOpe = "I";
            }

            Session["culture"] = culture;

            //valuepais2
            string valuePaises2 = "";
            if (Session["CodPais2"] != null)
            {
                valuePaises2 = Session["CodPais2"].ToString();
            }
            else
            {
                valuePaises2 = new ListaPaises().BuscarCodPais2(codPais);
                valuePaises2 = (valuePaises2 == "") ? "1LAT" : valuePaises2;
            }
            ConsultaForm consultaForm = new ConsultaForm
            {
                Importador = Consulta.ExisteVariable(codPais, tipoOpe, "IdImportador"),
                Proveedor = Consulta.ExisteVariable(codPais, tipoOpe, "IdProveedor"),
                PaisOrigen = Consulta.ExisteVariable(codPais, tipoOpe, "IdPaisOrigen"),
                Exportador = Consulta.ExisteVariable(codPais, tipoOpe, "IdExportador"),
                ImportadorExp = Consulta.ExisteVariable(codPais, tipoOpe, "IdImportadorExp"),
                CodPais = codPais,
                TipoOpe = tipoOpe,
                FlagDescComercialB = Consulta.FlagDesComercial(codPais, tipoOpe),
                CodPais2 = valuePaises2
            };

            consultaForm.IsManifiesto = IsManifiesto(codPais);

            Session["CodPais2"] = valuePaises2;

            ViewData["consultaForm"] = consultaForm;
            ViewData["CodPais"] = cboPais;
            ViewData["rdbTipoOpe"] = rdbTipoOpe;
            ViewData["cboOpcion"] = cboOpcion;
            ViewData["txtDesComercialB"] = txtDesComercialB;
            ViewData["culture"] = culture;
            ViewData["metodo"] = "POST";
            ViewData["CodPartida"] = idAutocompletado;

            // Ruben 202303
            if (culture == "es")
                ViewBag.Canonical = "https://www.veritradecorp.com/es/consultas";
            else
                ViewBag.Canonical = "https://www.veritradecorp.com/en/consultas";

            return View("~/Views/Consulta/Index_2.cshtml");
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
        bool IsManifiesto(string codPais)
        {
            //AGREGANDO BRASIL MANIFIESTOS Y ECUADOR MANIFIESTOS
            return (new[] { "PE_", "US_", "PE_", "US_", "EC_", "EC_", "BR_", "BR_" }).Contains(codPais) || _CodManifiestosModificado.Contains(codPais);
        }

        public Dictionary<string, DateTime> CargaPeriodos(string codPais, string tipoOpe)
        {
            //string añoMesInfoIni = "", añoMesInfoFin = "";
            //Funciones.Rango(codPais, tipoOpe, ref añoMesInfoIni, ref añoMesInfoFin);
            //ValidaCodPaisManif(ref codPais, tipoOpe);
            string anioIni = "", mesIni = "", anioFin = "", mesFin = "";

            if (IsManifiesto(codPais))
            {
                codPais = codPais.Substring(0, 2) + tipoOpe;
            }

            Funciones.Rango(codPais, tipoOpe, ref anioIni, ref mesIni, ref anioFin, ref mesFin);

            

            //var culture = new System.Globalization.CultureInfo(idioma);
            DateTime fechaInfoFin = Convert.ToDateTime(anioFin + "-" + mesFin + "-01");

            //var fechaInfoFin =
            //    Convert.ToDateTime(añoMesInfoFin.Substring(0, 4) + "-" + añoMesInfoFin.Substring(4, 2) + "-01");

            // Consulta Gratis
            fechaInfoFin = fechaInfoFin.AddMonths(-6);
            DateTime auxFechaInicio = fechaInfoFin;
            DateTime fechaInfoIni = auxFechaInicio.AddMonths(-2);

            //var meses = 3;

            //var listMesIni = new List<ListItem>();
            //var listMesFin = new List<ListItem>();

            //for (var i = 0; i < meses; i++)
            //{
            //    listMesIni.Add(new ListItem(
            //        fechaInfoFin.AddMonths(-meses + i + 1).ToString("MMM yyyy", culture).ToUpper(),
            //        fechaInfoFin.AddMonths(-meses + i + 1).ToString("yyyyMM")));
            //}
            //listMesFin.Add(listMesIni[listMesIni.Count - 1]);

            var respuesta = new Dictionary<string, DateTime>
            {
                {"mesesInicio", fechaInfoIni},
                {"mesesFin", fechaInfoFin}
            };

            return respuesta;
        }

        private ConsultaForm GetConsultaForm(string codPais, string codPais2, string selectedTipoOpe)
        {
            ConsultaForm consultaForm = new ConsultaForm
            {
                Importador = Consulta.ExisteVariable(codPais, selectedTipoOpe, "IdImportador"),
                Proveedor = Consulta.ExisteVariable(codPais, selectedTipoOpe, "IdProveedor"),
                PaisOrigen = Consulta.ExisteVariable(codPais, selectedTipoOpe, "IdPaisOrigen"),
                Exportador = Consulta.ExisteVariable(codPais, selectedTipoOpe, "IdExportador"),
                ImportadorExp = Consulta.ExisteVariable(codPais, selectedTipoOpe, "IdImportadorExp"),
                CodPais = codPais,
                CodPais2 = codPais2,
                TipoOpe = selectedTipoOpe,
                IsManifiesto = this.IsManifiesto(codPais),
                FlagDescComercialB = Consulta.FlagDesComercial(codPais, selectedTipoOpe)
            };
            return consultaForm;
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

        [HttpPost]
        public JsonResult rdbtnTipoOpeChange(string selectedTipoOpe, string codPais, string idiomaCulture)
        {
            string auxCodPais = codPais;
            string auxCodPais2 = Session["CodPais2"].ToString();
            //var lista = new ListaPaises().PaisesConsulta(selectedTipoOpe.Equals("I"));
            IEnumerable<SelectListItem> lista = new ListaPaises().GetPaisesAdmin(idiomaCulture, auxCodPais2);

            if (selectedTipoOpe.Equals("E") && codPais == "PEB")
                auxCodPais = "PE";

            Restablecer();
            Session["CodPais2"] = auxCodPais2;
            Session["CodPais"] = auxCodPais;
            Session["TipoOpe"] = selectedTipoOpe;


            var consultaForm = GetConsultaForm(auxCodPais, auxCodPais2, selectedTipoOpe);

            return Json(new
            {
                htmlFormSection = RenderViewToString(this.ControllerContext, "Consulta/_FormSection", consultaForm),
                listCountries = lista,
                selectedIndex = auxCodPais,
                filtroRangoFechas = new
                {
                    rangoFechas = CargaPeriodos(codPais, selectedTipoOpe),
                    periodo = Funciones.InfoEnLinea(codPais, selectedTipoOpe, idiomaCulture)
                }
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult cboPaisSelectedIndexChanged(string tipoOpe, string codPais, string idiomaCulture)
        {
            Session["CodPais"] = codPais;
            ValidaCodPaisManif(ref codPais, tipoOpe);
            string auxCodPais2 = Session["CodPais2"].ToString();
            var consultaForm = GetConsultaForm(codPais, auxCodPais2, tipoOpe);
            Restablecer();
            Session["CodPais2"] = auxCodPais2;
            return Json(new
            {
                htmlFormSection = RenderViewToString(this.ControllerContext, "Consulta/_FormSection", consultaForm),
                filtroRangoFechas = new
                {
                    rangoFechas = CargaPeriodos(codPais, tipoOpe),
                    periodo = Funciones.InfoEnLinea(codPais, tipoOpe, idiomaCulture)
                }
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult cboPais2SelectedIndexChanged(string tipoOpe, string codPais2, string idiomaCulture)
        {
            string CodPaisT, PaisT, CodPais2T, CodPais2AuxT, codPaisAseleccionar;

            IEnumerable<SelectListItem> listaPaises = new ListaPaises().GetPaisesAdmin(idiomaCulture, codPais2);
            CodPais2T = codPais2;
            CodPaisT = listaPaises.FirstOrDefault().Value;
            codPaisAseleccionar = CodPaisT;

            if (codPais2 == "4UE")
            {
                CodPaisT = "UE";
            }


            Session["CodPais"] = CodPaisT;
            string auxCodPais2 = codPais2;
            Session["CodPais2"] = auxCodPais2;
            var consultaForm = GetConsultaForm(CodPaisT, codPais2, tipoOpe);
            Restablecer();

            return Json(new
            {
                codPaisSeleccionar = codPaisAseleccionar,
                codPais2Seleccionar = Session["CodPais2"].ToString(),
                listaPaises,
                htmlFormSection = RenderViewToString(this.ControllerContext, "Consulta/_FormSection", consultaForm),
                filtroRangoFechas = new
                {
                    rangoFechas = CargaPeriodos(CodPaisT, tipoOpe),
                    periodo = Funciones.InfoEnLinea(CodPaisT, tipoOpe, idiomaCulture)
                }
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FiltersContent()
        {
            return PartialView("Consulta/_FormFilters");
        }
        private void ValidaCodPais2(string codPais2, ref string codPais)
        {
            if (codPais2 == "4UE")
                codPais = "UE";
        }
        public ActionResult GetTabsView(string TipoOpe, string CodPais)
        {
            string CodPais2 = Session["CodPais2"].ToString();
            ValidaCodPais2(CodPais2, ref CodPais);
            ValidaCodPaisManif(ref CodPais, TipoOpe);
            Session["CodPais"] = CodPais;
            ConsultaForm model = new ConsultaForm();
            model.TipoOpe = TipoOpe;
            model.CodPais = CodPais;
            model.CampoPeso = Funciones.CampoPeso(CodPais, TipoOpe);
            model.Cif = (Funciones.Incoterm(CodPais, TipoOpe)).ToUpper();
            model.ExisteDistrito = (CodPais == "CN" || CodPais == "US");
            model.ExisteAduana = (CodPais != "CN" && CodPais != "IN" && CodPais != "PY" && CodPais != "US");
            model.IsManifiesto = IsManifiesto(CodPais);
            model.FlagVarVisibles = new TabMisBusquedas(TipoOpe, CodPais);
            return PartialView("TabsView", model);
        }

        public JsonResult GetRangoFechas(string codPais, string tipoOpe, string idiomaCulture = "es-pe")
        {
            var rangoFechas = CargaPeriodos(codPais, tipoOpe);
            var periodo = Funciones.InfoEnLinea(codPais, tipoOpe, idiomaCulture);
            return Json(new { rangoFechas, periodo }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Elimina las palabras no aceptadas por los filtros
        /// </summary>
        /// <param name="desComercialB"></param>
        /// <returns></returns>
        public string EliminaStopWords(string desComercialB)
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

        public object AgregaPalabrasFiltros(string palabras, string opcion, string codPais, string selected = "0")
        {
            string valueOption = "";
            string textOption = "";
            object nuevoFiltro = null;
            //var codPais = Session["CodPais"].ToString();

            if (opcion == "1DE")
            {
                int cantPalabrasY = 0;
                if (Session["hdfCantPalabrasY"] != null)
                    cantPalabrasY = Convert.ToInt32(Session["hdfCantPalabrasY"].ToString());

                Session["hdfPalabrasY"] = (Session["hdfPalabrasY"] + " " + palabras).Trim();
                cantPalabrasY += 1;

                valueOption = opcion + "|" + palabras;
                textOption = "[Desc. Comercial] " + palabras;

                Session["hdfCantPalabrasY"] = cantPalabrasY.ToString();
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
                    textOption = "[Partida] " + Funciones.BuscaPartida(idPartida, codPais);
                }
                else if (subCapitulo != "")
                {
                    textOption = "[Partida] " + palabras + " [" + todos + " 4] " + subCapitulo;
                }
                else if (hts6 != "")
                {
                    textOption = "[Partida] " + palabras + " [" + todos + " 6] " + hts6;
                }
                else
                {
                    textOption = "[Partida] " + palabras + " [" + todos + "]";

                }

                valueOption = opcion + '|' + palabras;
            }
            else if (opcion == "3I_")
            {
                if (Session["hdfImportadorB"] == null) Session["hdfImportadorB"] = palabras;
                else Session["hdfImportadorB"] = Session["hdfImportadorB"] + "|" + palabras;
                if (selected != "0")
                {
                    textOption = "[Importador] " + Funciones.BuscaEmpresa(selected, codPais);
                }
                else
                {
                    textOption = "[Importador] " + palabras + " [TODOS]";
                }
                valueOption = opcion + '|' + palabras;
            }
            else if (opcion == "3E_")
            {
                if (Session["hdfExportadorB"] == null) Session["hdfExportadorB"] = palabras;
                else Session["hdfExportadorB"] = Session["hdfExportadorB"] + "|" + palabras;

                if (selected != "0")
                {
                    textOption = "[Exportador] " + Funciones.BuscaEmpresa(selected, codPais);
                }
                else
                {
                    textOption = "[Exportador] " + palabras + " [TODOS]";
                }

                valueOption = opcion + '|' + palabras;
            }
            else if (opcion == "4P_")
            {
                if (Session["hdfProveedorB"] == null) Session["hdfProveedorB"] = palabras;
                else Session["hdfProveedorB"] = Session["hdfProveedorB"] + "|" + palabras;

                textOption = (codPais != "CL" ? "[Exportador] " : "[Marca] ") + palabras + " [TODOS]";
                valueOption = opcion + '|' + palabras;
            }
            else if (opcion == "4I_")
            {
                if (Session["hdfImportadorExpB"] == null) Session["hdfImportadorExpB"] = palabras;
                else Session["hdfImportadorExpB"] = Session["hdfImportadorExpB"] + "|" + palabras;

                textOption = "[Importador] " + palabras;// + " [TODOS]";
                valueOption = opcion + '|' + palabras;
            }

            nuevoFiltro = new
            {
                text = textOption,
                value = valueOption
            };

            List<OptionSelect> listFilters;
            if (Session["lstFiltros"] != null)
                listFilters = Session["lstFiltros"] as List<OptionSelect>;
            else
                listFilters = new List<OptionSelect>();
            listFilters.Add(new OptionSelect { value = valueOption, text = textOption });

            if (listFilters.Count > 0)
                Session["lstFiltros"] = listFilters;
            else
                Session.Remove("lstFiltros");

            return nuevoFiltro;
        }

        [HttpPost]
        public JsonResult AgregarDesComercial(string filtro, string CodPais)
        {
            var listaPalabras = new List<object>();
            var texto = EliminaStopWords(filtro);
            if (texto == "")
            {
                var mensaje = "Las palabras ingresadas se eliminarán de su búsqueda: <b>" + filtro +
                              "</b>.<br>Por favor ingrese otras palabras";
                return Json(new { response = false, mensaje }, JsonRequestBehavior.AllowGet);
            }

            var palabras = texto.Split(' ');
            foreach (var palabra in palabras)
            {
                listaPalabras.Add(AgregaPalabrasFiltros(palabra, "1DE", CodPais));
            }

            return Json(new { response = true, listaPalabras }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult BuscarPartida(string nandina, string codPais = "", string opcion = "")
        {
            var json = new List<object>();
            if (opcion == "PAR")
            {
                json = GeneraJsonBuscaPartidas(nandina, codPais, false);
            }
            else if (string.IsNullOrEmpty(opcion))
            {
                json = GeneraJsonBuscaPartidas(nandina, codPais);
            }

            return Json(json, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult BuscarEmpresas(string importadorB, string codPais)
        {
            var json = GeneraJsonBuscaEmpresas(importadorB, codPais);
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult BuscarProveedores(string proveedorB)
        {
            var proveedor = proveedorB.ToUpper();
            var json = new List<object> { new { id = 0, value = proveedor + " [TODOS]", texto = proveedor } };
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult CboPaisBChange(string tipoOpe, string textoCboPaisB, string valorCboPaisB,
        int indexCboPaisB)
        {
            var listaPalabras = new List<object>();
            string codPais = Session["codPais"].ToString();
            if (codPais.Contains("_"))
            {
                ValidaCodPaisManif(ref codPais, tipoOpe);
            }
            bool isManifiesto = IsManifiesto(codPais);
            if (indexCboPaisB > 0)
            {
                if (tipoOpe == "I")
                {
                    listaPalabras.Add(new

                    {
                        text =
                            "[" + (isManifiesto ? Resources.Resources.EmbarqueCountry_FormField_Label : Resources.Resources.OriginCountry_FormField_Label) + "] " + textoCboPaisB + " ",
                        value = "5O_|" + valorCboPaisB
                    });
                }
                else
                {
                    listaPalabras.Add(new
                    {
                        text = "[" + Resources.Resources.DestinationCountry_FormField_Label + "] " +
                               textoCboPaisB + " ",
                        value = "5D_|" + valorCboPaisB
                    });
                }
            }

            //Restablecer();

            return Json(new { response = true, listaPalabras }, JsonRequestBehavior.AllowGet);
        }
        public List<object> GeneraJsonBuscaPartidas(string nandina, string codPais, bool flagConsultaGratis = true)
        {
            var json = new List<object>();

            var dt = Consulta.BuscaPartidas(nandina, codPais, flagConsultaGratis);
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

        public JsonResult AgregaPartida(string filtro, string seleccionado, string codPais)
        {
            if (seleccionado == "") return Json("No hay un elemento seleccionado", JsonRequestBehavior.AllowGet);

            var newItem = new List<object>();
            var id = seleccionado;

            newItem.Add(AgregaPalabrasFiltros(id, "2P_", codPais));

            return Json(newItem, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AgregaImportador(string filtro, string seleccionado, string codPais)
        {
            var id = seleccionado;
            if (id == "") return Json("No hay un elemento seleccionado", JsonRequestBehavior.AllowGet);

            var newItem = new List<object>();

            if (id != "0")
            {
                newItem.Add(AgregaPalabrasFiltros("[" + id + "]", "3I_", codPais, id));
            }
            else
            {
                newItem.Add(AgregaPalabrasFiltros(filtro.ToUpper().Trim(), "3I_", codPais));
            }

            return Json(newItem, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AgregaProveedor(string filtro, string seleccionado, string CodPais)
        {
            var id = seleccionado;
            if (id != "0") return Json("No hay un elemento seleccionado", JsonRequestBehavior.AllowGet);

            var newItem = new List<object>();
            newItem.Add(AgregaPalabrasFiltros(filtro.ToUpper().Trim(), "4P_", CodPais));
            return Json(newItem, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AgregaExportador(string filtro, string seleccionado, string CodPais)
        {
            var id = seleccionado;
            if (id == "") return Json("No hay un elemento seleccionado", JsonRequestBehavior.AllowGet);

            var newItem = new List<object>();

            if (id != "0")
            {
                newItem.Add(AgregaPalabrasFiltros("[" + id + "]", "3E_", CodPais, id));
            }
            else
            {
                newItem.Add(AgregaPalabrasFiltros(filtro.ToUpper().Trim(), "3E_", CodPais));
            }

            return Json(newItem, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AgregaImportadorExp(string filtro, string seleccionado, string CodPais)
        {
            var id = seleccionado;
            if (id != "0") return Json("No hay un elemento seleccionado", JsonRequestBehavior.AllowGet);

            var newItem = new List<object>();
            newItem.Add(AgregaPalabrasFiltros(filtro.ToUpper().Trim(), "4I_", CodPais));
            return Json(newItem, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Buscar(string fechaDesde, string fechasHasta, string TipoOpe, string CodPais, int indexCboPaisB,
            string codPaisB, string idioma)
        {
            //return BuscarPorFiltro(fechaDesde, fechasHasta, TipoOpe, CodPais, indexCboPaisB, codPaisB, idioma);
            var json = BuscarPorFiltro(fechaDesde, fechasHasta, TipoOpe, CodPais, indexCboPaisB, codPaisB, idioma);
            Console.WriteLine(json);
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

        private object BuscarPorFiltro(string fechaIni, string fechaFin, string TipoOpe, string CodPais, int indexCboPaisB,
            string codPaisB, string idioma)
        {
            Session["CodPais"] = CodPais;
            string CodPais2 = Session["CodPais2"].ToString();
            var specificCulture = Session["culture"].ToString().Equals("es") ? CultureInfo.CreateSpecificCulture("es-pe") : CultureInfo.CreateSpecificCulture("en-us");
            //para variables CIF y CIFTot
            string cif = GetCIFTot(CodPais, TipoOpe);
            ValidaCodPaisManif(ref CodPais, TipoOpe);
            //Session["CIF"]  = cif;
            //string CIFTot = cif + "Tot";

            string tabla = "";
            string dua = "";
            if (TipoOpe == "I")
            {
                tabla = "Importacion_" + CodPais;
                if (CodPais == "PE" || CodPais == "PEB" || CodPais == "PEP")
                    dua = "NroCorre";
                else if (CodPais == "EC")
                    dua = "Refrendo";
                else
                    dua = "DUA";
                // if (CodPais != "CL")
            }
            else
            {
                tabla = "Exportacion_" + CodPais;
                if (CodPais == "PE" || CodPais == "PEP") dua = "NroOrden";
                else if (CodPais == "EC") dua = "Refrendo";
                else dua = "DUA";
            }

            var flagPalabras = (Session["hdfPalabrasY"] != null);

            var mensaje = "";
            var resultado = "";

            #region Filtros
            Session["SqlFiltro"] = GeneraSqlFiltro(fechaIni, fechaFin, dua, indexCboPaisB, codPaisB);
            if (Session["UltSqlFiltro"] != null &&
                Session["UltSqlFiltro"].ToString() == Session["SqlFiltro"].ToString())
            {
                mensaje = "Ya realizó esta búsqueda.<br>Le sugerimos que reduzca el rango de fechas y/o modifique sus filtros de búsqueda";
                if (idioma == "en")
                    mensaje =
                        "You already did this search.<br>We suggest to change dates range and/or modify search filters";
                return new { error = true, mensaje };
            }
            else
            {
                Session["UltSqlFiltro"] = Session["SqlFiltro"].ToString();
            }
            #endregion


            //var cantReg = Convert.ToInt32(drTotales["CantReg"]);
            int cantReg = 0;
            string CIFTot = "";

            string pesoNeto = "";
            string valueCIFTot = "";
            decimal valuePesoNeto = 0;
            string unidad = "";

            GetCifTot_PesoNeto_CantReg_ValueCifTot_Unidad_ValuePesoNeto(CodPais, TipoOpe, tabla, ref CIFTot,
                    ref pesoNeto, ref cantReg, ref valueCIFTot, ref unidad, ref valuePesoNeto);
            //string valueCIFTot = drTotales[CIFTot].ToString();
            //Por el momento --- var drTotales = CalculaTotales(Session["SqlFiltro"].ToString(), CIFTot, CodPais, tabla);
            if (cantReg == 0)
            {
                if (Session["UltSqlFiltro"] == null ||
                    Session["UltSqlFiltro"].ToString() != Session["SqlFiltro"].ToString())
                {
                    Session["UltSqlFiltro"] = Session["SqlFiltro"].ToString();
                    mensaje =
                        "Su búsqueda no encontró resultados.<br>Le sugerimos que reduzca el rango de fechas y/o modifique sus filtros de búsqueda";

                    if (idioma == "en")
                        mensaje =
                            "Your search found no results.<br>We suggest to change dates range and/or modify search filters";
                    //return Json(new { error = true, mensaje }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    mensaje = "La búsqueda con el(los) último(s) filtros NO encontró resultados";
                    if (idioma == "en")

                        mensaje = "Search with last filters found no results";
                    //foreach (ListItem item in lstEliminar.Items)
                    //    lstFiltros.Items.Remove(item);
                    //lstEliminar.Items.Clear();
                }

                //resultado = "No se encontraron registros"
                mensaje += "<br> No se encontraron registros";
                return new { error = true, mensaje, resultado };
            }
            else if (cantReg > CantRegMax && flagPalabras)
            {
                mensaje = "Su búsqueda incluye Descripción Comercial y supera el límite de " +
                          CantRegMax.ToString("n0") + " registros.<br>";
                mensaje += "Reduzca el rango de fechas y/o modifique sus filtros de búsqueda";
                return new { error = true, mensaje };

            }
            else if (cantReg > CantRegMax && !flagPalabras)
            {
                //REVISAR
                mensaje = "Su búsqueda supera el límite de " + CantRegMax.ToString("n0") + " registros.<br>";
                mensaje +=
                    "Si desea ver todas las pestañas habilitadas, reduzca el rango de fechas y/o modifique sus filtros de búsqueda";

                if (idioma == "en")
                {
                    mensaje = "Your search exceeds " + CantRegMax.ToString("n0") +
                              " records limit and it can not be download to Excel. ";
                    mensaje +=
                        "If you want to see all tabs enabled and/or download to Excel, reduce the dates range and/or modify your filters search";
                }

                //Por el momento --- Session["SqlFiltro"] = GeneraSqlFiltro(fechaIni, fechaFin, dua, indexCboPaisB, codPaisB);
                //Por el momento --- drTotales = CalculaTotales(Session["SqlFiltro"].ToString(), CIFTot, CodPais, tabla);
                //Por el momento --- cantReg = Convert.ToInt32(drTotales["CantReg"]);
                //hdfCantReg.Value = drTotales["CantReg"].ToString();
                //hdfCIFTot.Value = drTotales[CIFTot].ToString();

                //lblResultado.Text = "Se encontraron " + CantReg.ToString("n0") + " registros";
                //CargaFiltro("Partida", -1);
                //DataTable dt = Top("Partida");
                //GeneraPie("Nandina", wccPartidas, dt);
                //Session["dtTopPartida"] = dt;
                return null;
            }
            else
            {
                //Busca(fechaIni, fechaFin);
                return new
                {
                    error = false,
                    gridData = BuscarDatosPorFiltro(fechaIni, fechaFin, TipoOpe, CodPais, CIFTot, tabla, dua, valueCIFTot, specificCulture),
                    footerGridData = new
                    {
                        RegisterCount = cantReg,
                        CIFTotal = Convert.ToSingle(valueCIFTot).ToString("n0", specificCulture)
                    }
                };
            }
        }
        string GetCIFTot(string codPais, string tipoOpe)
        {
            if (!IsManifiesto(codPais) && !_CodManifiestosModificado.Contains(codPais))
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
                FuncionesBusiness.CalculaTotales(Session["SqlFiltro"].ToString(), cifTot, codPais, pesoNeto, tabla, isManif: isManif);

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
        public List<object> BuscarDatosPorFiltro(string fechaIni, string fechaFin, string TipoOpe, string CodPais, string CIFTot,
                                                string tabla, string dua, string valueCIFTot, CultureInfo specificCulture)
        {
            //Esta función contiene la lògica de Busca
            var flags = new TabMisBusquedas(TipoOpe, CodPais);
            //Tener en cuenta que los PieChart usan la data almacenada en Session["dt" + Filtro];
            // que se setea en CargarFiltro

            DataTable dataTable;
            var json = new List<object>();

            bool isManifiesto = IsManifiesto(CodPais);

            string SessionSqlFiltro = Session["SqlFiltro"].ToString();
            if (flags.ExistePartida)//Consulta.ExisteVariable(CodPais, TipoOpe, "Partida"))
            {
                dataTable = CargaFiltro(CodPais, CIFTot, SessionSqlFiltro, tabla, dua, "Partida", -1);
                json.Add(new
                {
                    tabDataName = "partidas",
                    tabDataList = GeneraJsonDataPartida(dataTable, CIFTot, valueCIFTot, specificCulture),
                    tabDataPie = GeneraJsonDataPie("Nandina", dataTable, CIFTot, valueCIFTot, specificCulture)
                });
            }
            else
            {
                json.Add(new
                {
                    tabDataName = "partidas",
                    tabDataNumRows = 0
                });
            }

            int numRows;

            if (flags.ExisteMarcasModelos)//CodPais == "PEB")
            {
                numRows = 0;
                dataTable = CargaFiltro(CodPais, CIFTot, SessionSqlFiltro, tabla, dua, "Marca", -1);
                numRows = dataTable.Rows.Count;
                if (numRows > 0)
                {
                    json.Add(new
                    {
                        tabDataName = "marcas",
                        tabDataNumRows = numRows,
                        tabDataList = GeneraJsonDataMarca(dataTable, valueCIFTot, specificCulture),
                        tabDataPie = GeneraJsonDataPie("Marca", dataTable, CIFTot, valueCIFTot, specificCulture)
                    });
                }
                else
                {
                    json.Add(new
                    {
                        tabDataName = "marcas",
                        tabDataNumRows = 0
                    });
                }

                numRows = 0;
                dataTable = CargaFiltro(CodPais, CIFTot, SessionSqlFiltro, tabla, dua, "Modelo", -1);
                numRows = dataTable.Rows.Count;
                if (numRows > 0)
                {
                    json.Add(new
                    {
                        tabDataName = "modelos",
                        tabDataNumRows = numRows,
                        tabDataList = GeneraJsonDataModelos(dataTable, valueCIFTot, specificCulture),
                        tabDataPie = GeneraJsonDataPie("Modelo", dataTable, CIFTot, valueCIFTot, specificCulture)
                    });
                }
                else
                {
                    json.Add(new
                    {
                        tabDataName = "modelos",
                        tabDataNumRows = 0
                    });
                }
            }


            if (TipoOpe == "I")
            {
                if (flags.ExisteImportador)//Consulta.ExisteVariable(CodPais, TipoOpe, "IdImportador"))
                {
                    numRows = 0;
                    dataTable = CargaFiltro(CodPais, CIFTot, SessionSqlFiltro, tabla, dua, "Importador", -1);
                    numRows = dataTable.Rows.Count;
                    if (numRows > 0)
                    {
                        json.Add(new
                        {
                            tabDataName = "importadores",
                            tabDataNumRows = numRows,
                            tabDataList = GeneraJsonDataImportadores(dataTable, CIFTot, valueCIFTot, specificCulture),
                            tabDataPie = GeneraJsonDataPie("Importador", dataTable, CIFTot, valueCIFTot, specificCulture)
                        });
                    }
                    else
                    {
                        json.Add(new
                        {
                            tabDataName = "importadores",
                            tabDataNumRows = numRows
                        });
                    }
                }
                else
                {
                    json.Add(new
                    {
                        tabDataName = "importadores",
                        tabDataNumRows = 0
                    });
                }

                if (flags.ExisteProveedor)//Consulta.ExisteVariable(CodPais, TipoOpe, "IdProveedor"))
                {
                    numRows = 0;
                    dataTable = CargaFiltro(CodPais, CIFTot, SessionSqlFiltro, tabla, dua, "Proveedor", -1);
                    numRows = dataTable.Rows.Count;
                    if (numRows > 0)
                    {
                        json.Add(new
                        {
                            tabDataName = "proveedores",
                            tabDataNumRows = numRows,
                            tabDataList = GeneraJsonDataProveedores(dataTable, CIFTot, valueCIFTot, specificCulture),
                            tabDataPie = GeneraJsonDataPie("Proveedor", dataTable, CIFTot, valueCIFTot, specificCulture)
                        });
                    }
                    else
                    {
                        json.Add(new
                        {
                            tabDataName = "proveedores",
                            tabDataNumRows = numRows
                        });
                    }
                }
                else
                {
                    json.Add(new
                    {
                        tabDataName = "proveedores",
                        tabDataNumRows = 0
                    });

                }
                if (flags.ExistePaisOrigen)//Consulta.ExisteVariable(CodPais, TipoOpe, "IdPaisOrigen"))
                {
                    numRows = 0;
                    dataTable = CargaFiltro(CodPais, CIFTot, SessionSqlFiltro, tabla, dua, isManifiesto ? "PaisEmbarque" : "PaisOrigen", -1);
                    numRows = dataTable.Rows.Count;
                    if (numRows > 0)
                    {
                        json.Add(new
                        {
                            tabDataName = "paisesOrigen",
                            tabDataNumRows = numRows,
                            tabDataList = isManifiesto ? GeneraJsonDataPaisEmbarque(dataTable, CIFTot, valueCIFTot, specificCulture) : GeneraJsonDataPaisOrigen(dataTable, CIFTot, valueCIFTot, specificCulture),
                            tabDataPie = GeneraJsonDataPie(isManifiesto ? "PaisEmbarque" : "PaisOrigen", dataTable, CIFTot, valueCIFTot, specificCulture)
                        });
                    }
                    else
                    {
                        json.Add(new
                        {
                            tabDataName = "paisesOrigen",
                            tabDataNumRows = numRows
                        });
                    }
                }
                else
                {
                    json.Add(new
                    {
                        tabDataName = "paisesOrigen",
                        tabDataNumRows = 0
                    });
                }
            }
            else
            {
                if (Consulta.ExisteVariable(CodPais, TipoOpe, "IdExportador"))
                {
                    numRows = 0;
                    dataTable = CargaFiltro(CodPais, CIFTot, SessionSqlFiltro, tabla, dua, "Exportador", -1);
                    numRows = dataTable.Rows.Count;
                    if (numRows > 0)
                    {
                        json.Add(new
                        {
                            tabDataName = "exportadores",
                            tabDataNumRows = numRows,
                            tabDataList = GeneraJsonDataExportadores(dataTable, valueCIFTot, specificCulture),
                            tabDataPie = GeneraJsonDataPie("Exportador", dataTable, CIFTot, valueCIFTot, specificCulture)
                        });
                    }
                    else
                    {
                        json.Add(new
                        {
                            tabDataName = "exportadores",
                            tabDataNumRows = numRows
                        });
                    }
                }
                else
                {
                    json.Add(new
                    {
                        tabDataName = "exportadores",
                        tabDataNumRows = 0
                    });
                }

                if (Consulta.ExisteVariable(CodPais, TipoOpe, "IdImportadorExp"))
                {
                    numRows = 0;
                    dataTable = CargaFiltro(CodPais, CIFTot, SessionSqlFiltro, tabla, dua, "ImportadorExp", -1);
                    numRows = dataTable.Rows.Count;
                    if (numRows > 0)
                    {
                        json.Add(new
                        {
                            tabDataName = "impotadoresExp",
                            tabDataNumRows = numRows,
                            tabDataList = GeneraJsonDataImportadoresExp(dataTable, valueCIFTot, specificCulture),
                            tabDataPie = GeneraJsonDataPie("ImportadorExp", dataTable, CIFTot, valueCIFTot, specificCulture)
                        });
                    }
                    else
                    {
                        json.Add(new
                        {
                            tabDataName = "impotadoresExp",
                            tabDataNumRows = numRows
                        });
                    }
                }
                else
                {
                    json.Add(new
                    {
                        tabDataName = "impotadoresExp",
                        tabDataNumRows = 0
                    });
                }

                if (Consulta.ExisteVariable(CodPais, TipoOpe, "IdPaisDestino"))
                {
                    numRows = 0;
                    dataTable = CargaFiltro(CodPais, CIFTot, SessionSqlFiltro, tabla, dua, "PaisDestino", -1);
                    numRows = dataTable.Rows.Count;
                    if (numRows > 0)
                    {
                        json.Add(new
                        {
                            tabDataName = "paisesDestino",
                            tabDataNumRows = numRows,
                            tabDataList = GeneraJsonDataPaisDestino(dataTable, valueCIFTot, specificCulture),
                            tabDataPie = GeneraJsonDataPie("PaisDestino", dataTable, CIFTot, valueCIFTot, specificCulture)
                        });
                    }
                    else
                    {
                        json.Add(new
                        {
                            tabDataName = "paisesDestino",
                            tabDataNumRows = 0
                        });
                    }
                }
                else
                {
                    json.Add(new
                    {
                        tabDataName = "paisesDestino",
                        tabDataNumRows = 0
                    });
                }
            }
            //ExisteViaTransp
            if (flags.ExisteViaTransp)//CodPais != "BO" && CodPais != "PY" && CodPais != "US")
            {
                numRows = 0;
                dataTable = CargaFiltro(CodPais, CIFTot, SessionSqlFiltro, tabla, dua, "ViaTransp", -1);
                numRows = dataTable.Rows.Count;
                if (numRows > 0)
                {
                    json.Add(new
                    {
                        tabDataName = "viasTransporte",
                        tabDataNumRows = numRows,
                        tabDataList = GeneraJsonDataViasTransporte(dataTable, CIFTot, valueCIFTot, specificCulture),
                        tabDataPie = GeneraJsonDataPie("ViaTransp", dataTable, CIFTot, valueCIFTot, specificCulture)
                    });
                }
                else
                {
                    json.Add(new
                    {
                        tabDataName = "viasTransporte",
                        tabDataNumRows = 0
                    });
                }
            }
            else
            {
                json.Add(new
                {
                    tabDataName = "viasTransporte",
                    tabDataNumRows = 0
                });
            }
            //ExisteAduana
            if (flags.ExisteAduana)//CodPais != "CN" && CodPais != "IN" && CodPais != "PY" && CodPais != "US")
            {
                bool flagExisteDUA = (CodPais != "AR" && CodPais != "BR" && CodPais != "CO" && CodPais != "MX" && CodPais != "UY");
                numRows = 0;
                dataTable = CargaFiltro(CodPais, CIFTot, SessionSqlFiltro, tabla, dua, "AduanaDUA", -1);
                numRows = dataTable.Rows.Count;
                if (numRows > 0)
                {
                    var dataTableAxu = Consulta.AgrupadoDataPie("AduanaDUA", CodPais, CIFTot, SessionSqlFiltro, tabla, dua);
                    json.Add(new
                    {
                        tabDataName = "AduanaDUAs",
                        tabDataNumRows = numRows,
                        tabDataExisteDUA = flagExisteDUA,
                        tabDataList = GeneraJsonDataAduanaDUA(dataTable, CIFTot, valueCIFTot, specificCulture, flagExisteDUA),
                        tabDataCbo = GeneraJsonDataCboAduanaDUA(dataTableAxu),
                        tabDataPie = GeneraJsonDataPie("Aduana", dataTableAxu, CIFTot, valueCIFTot, specificCulture)
                    });
                }
                else
                {
                    json.Add(new
                    {
                        tabDataName = "AduanaDUAs",
                        tabDataNumRows = 0
                    });
                }
            }
            else
            {
                json.Add(new
                {
                    tabDataName = "AduanaDUAs",
                    tabDataNumRows = 0
                });
            }
            //ExisteDistrito
            if (flags.ExisteDistrito)//CodPais == "CN" || CodPais == "US")
            {
                numRows = 0;
                dataTable = CargaFiltro(CodPais, CIFTot, SessionSqlFiltro, tabla, dua, "Distrito", -1);
                numRows = dataTable.Rows.Count;
                if (numRows > 0)
                {
                    json.Add(new
                    {
                        tabDataName = "Distritos",
                        tabDataList = GeneraJsonDataDistritos(dataTable, CIFTot, valueCIFTot, specificCulture),
                        tabDataPie = GeneraJsonDataPie("Distrito", dataTable, CIFTot, valueCIFTot, specificCulture)
                    });
                }
            }

            //Implementar lógica de la parte final de CargaFiltro
            //if (Filtro != "Modelo" && Filtro != "AduanaDUA")

            return json;
        }

        public JsonResult SearchByFilterPostFilterUpdate(string fechaDesde, string fechasHasta, string TipoOpe, string CodPais, int indexCboPaisB,
            string codPaisB)
        {
            var specificCulture = Session["culture"].ToString().Equals("es") ? CultureInfo.CreateSpecificCulture("es-pe") : CultureInfo.CreateSpecificCulture("en-us");

            //para variables CIF y CIFTot
            string cif = Funciones.Incoterm(CodPais, TipoOpe);
            //Session["CIF"]  = cif;
            string CIFTot = cif + "Tot";

            string tabla = "";
            string dua = "";
            if (TipoOpe == "I")
            {
                tabla = "Importacion_" + CodPais;
                if (CodPais == "PE" || CodPais == "PEB")
                    dua = "NroCorre";
                else if (CodPais == "EC")
                    dua = "Refrendo";
                else
                    dua = "DUA";
                // if (CodPais != "CL")
            }
            else
            {
                tabla = "Exportacion_" + CodPais;
                if (CodPais == "PE") dua = "NroOrden";
                else if (CodPais == "EC") dua = "Refrendo";
                else dua = "DUA";
            }

            Session["CodPais"] = CodPais;

            Session["SqlFiltro"] = GeneraSqlFiltro(fechaDesde, fechasHasta, dua, indexCboPaisB, codPaisB);

            var drTotales = CalculaTotales(Session["SqlFiltro"].ToString(), CIFTot, CodPais, tabla);
            var cantReg = Convert.ToInt32(drTotales["CantReg"]);
            string valueCIFTot = drTotales[CIFTot].ToString();

            string mensaje = "";
            if (cantReg == 0)
            {

                //resultado = "No se encontraron registros"
                mensaje = "No se encontraron registros";
                return Json(new { error = true, mensaje }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                error = false,
                gridData = BuscarDatosPorFiltro(fechaDesde, fechasHasta, TipoOpe, CodPais, CIFTot, tabla, dua, valueCIFTot, specificCulture),
                footerGridData = new
                {
                    RegisterCount = cantReg,
                    CIFTotal = Convert.ToSingle(valueCIFTot).ToString("n0", specificCulture) //string.Format("{0:#,#}", valueCIFTot) //
                }
            }, JsonRequestBehavior.AllowGet);
        }

        private List<object> GeneraJsonDataPartida(DataTable dt, string CIFTot, string valueCIFTot, CultureInfo specificCulture)
        {
            var json = new List<object>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    json.Add(new
                    {
                        IdPartida = Convert.ToUInt64(dr["IdPartida"]),
                        Nandina = dr["Nandina"].ToString(),
                        Partida = dr["Partida"].ToString(),
                        CantReg = Convert.ToUInt64(dr["CantReg"]),
                        CIFTot = (Convert.ToSingle(dr[CIFTot])).ToString("n0", specificCulture),
                        CIFTotPercentaje = (Convert.ToSingle(dr[CIFTot]) / Convert.ToSingle(valueCIFTot) * 100).ToString("n2", specificCulture) + "%"
                    });
                }
            }
            return json;
        }

        private List<Object> GeneraJsonDataPaisOrigen(DataTable dt, string CIFTot, string valueCIFTot, CultureInfo specificCulture)
        {
            var json = new List<object>();
            foreach (DataRow dr in dt.Rows)
            {
                json.Add(new
                {
                    IdPaisOrigen = Convert.ToUInt64(dr["IdPaisOrigen"]),
                    PaisOrigen = dr["PaisOrigen"].ToString(),
                    CantReg = Convert.ToUInt64(dr["CantReg"]),
                    CIFTot = (Convert.ToSingle(dr[CIFTot])).ToString("n0", specificCulture),
                    CIFTotPercentaje = (Convert.ToSingle(dr[CIFTot]) / Convert.ToSingle(valueCIFTot) * 100).ToString("n2", specificCulture) + "%"
                });
            }
            return json;
        }

        private List<Object> GeneraJsonDataPaisEmbarque(DataTable dt, string CIFTot, string valueCIFTot, CultureInfo specificCulture)
        {
            var json = new List<object>();
            foreach (DataRow dr in dt.Rows)
            {
                json.Add(new
                {
                    IdPaisOrigen = Convert.ToUInt64(dr["IdPaisEmbarque"]),
                    PaisOrigen = dr["PaisEmbarque"].ToString(),
                    CantReg = Convert.ToUInt64(dr["CantReg"]),
                    CIFTot = (Convert.ToSingle(dr[CIFTot])).ToString("n0", specificCulture),
                    CIFTotPercentaje = (Convert.ToSingle(dr[CIFTot]) / Convert.ToSingle(valueCIFTot) * 100).ToString("n2", specificCulture) + "%"
                });
            }
            return json;
        }

        private List<object> GeneraJsonDataImportadores(DataTable dt, string CIFTot, string valueCIFTot, CultureInfo specificCulture)
        {
            var json = new List<object>();
            foreach (DataRow dr in dt.Rows)
            {
                json.Add(new
                {
                    IdImportador = Convert.ToUInt64(dr["IdImportador"]),
                    Importador = dr["Importador"].ToString(),
                    CantReg = Convert.ToUInt64(dr["CantReg"]),
                    CIFTot = (Convert.ToSingle(dr[CIFTot])).ToString("n0", specificCulture),
                    CIFTotPercentaje = (Convert.ToSingle(dr[CIFTot]) / Convert.ToSingle(valueCIFTot) * 100).ToString("n2", specificCulture) + "%"
                });
            }
            return json;
        }

        private List<object> GeneraJsonDataProveedores(DataTable dt, string CIFTot, string valueCIFTot, CultureInfo specificCulture)
        {
            var json = new List<object>();
            foreach (DataRow dr in dt.Rows)
            {
                json.Add(new
                {
                    IdProveedor = Convert.ToUInt64(dr["IdProveedor"]),
                    Proveedor = dr["Proveedor"].ToString() == "" ? "N/A" : dr["Proveedor"].ToString(),
                    CantReg = Convert.ToUInt64(dr["CantReg"]),
                    CIFTot = (Convert.ToSingle(dr[CIFTot])).ToString("n0", specificCulture),
                    CIFTotPercentaje = (Convert.ToSingle(dr[CIFTot]) / Convert.ToSingle(valueCIFTot) * 100).ToString("n2", specificCulture) + "%"
                });
            }
            return json;
        }

        private List<object> GeneraJsonDataViasTransporte(DataTable dt, string CIFTot, string valueCIFTot, CultureInfo specificCulture)
        {
            var json = new List<object>();
            foreach (DataRow dr in dt.Rows)
            {
                json.Add(new
                {
                    IdViaTransp = Convert.ToUInt64(dr["IdViaTransp"]),
                    ViaTransp = dr["ViaTransp"].ToString(),
                    CantReg = Convert.ToUInt64(dr["CantReg"]),
                    CIFTot = (Convert.ToSingle(dr[CIFTot])).ToString("n0", specificCulture),
                    CIFTotPercentaje = (Convert.ToSingle(dr[CIFTot]) / Convert.ToSingle(valueCIFTot) * 100).ToString("n2", specificCulture) + "%"
                });
            }
            return json;
        }

        private List<object> GeneraJsonDataAduanaDUA(DataTable dt, string CIFTot, string valueCIFTot, CultureInfo specificCulture, bool flagExisteDUA)
        {
            var json = new List<object>();
            foreach (DataRow dr in dt.Rows)
            {
                json.Add(new
                {
                    IdAduana = flagExisteDUA ? (dr["IdAduana"].ToString() + "-" + dr["DUA"].ToString()) : dr["IdAduana"].ToString(),
                    Aduana = dr["Aduana"].ToString(),
                    DUA = dr["DUA"].ToString(),
                    IdAduanaDUA = dr["IdAduanaDUA"].ToString(),
                    AduanaDUA = dr["AduanaDUA"].ToString(),
                    CantReg = Convert.ToUInt64(dr["CantReg"]),
                    CIFTot = (Convert.ToSingle(dr[CIFTot])).ToString("n0", specificCulture)
                });
            }
            return json;
        }

        private List<object> GeneraJsonDataExportadores(DataTable dt, string valueCIFTot, CultureInfo specificCulture)
        {
            var json = new List<object>();
            foreach (DataRow dr in dt.Rows)
            {
                json.Add(new
                {
                    IdExportador = Convert.ToUInt64(dr["IdExportador"]),
                    Exportador = dr["Exportador"].ToString(),
                    CantReg = Convert.ToUInt64(dr["CantReg"]),
                    FOBTot = (Convert.ToSingle(dr["FOBTot"])).ToString("n0", specificCulture),
                    FOBTotPercentaje = (Convert.ToSingle(dr["FOBTot"]) / Convert.ToSingle(valueCIFTot) * 100).ToString("n2", specificCulture) + "%"
                });
            }
            return json;
        }

        private List<Object> GeneraJsonDataPaisDestino(DataTable dt, string valueCIFTot, CultureInfo specificCulture)
        {
            var json = new List<object>();
            foreach (DataRow dr in dt.Rows)
            {
                json.Add(new
                {
                    IdPaisDestino = Convert.ToUInt64(dr["IdPaisDestino"]),
                    PaisDestino = dr["PaisDestino"].ToString(),
                    CantReg = Convert.ToUInt64(dr["CantReg"]),
                    FOBTot = (Convert.ToSingle(dr["FOBTot"])).ToString("n0", specificCulture),
                    FOBTotPercentaje = (Convert.ToSingle(dr["FOBTot"]) / Convert.ToSingle(valueCIFTot) * 100).ToString("n2", specificCulture) + "%"
                });
            }
            return json;
        }

        private List<object> GeneraJsonDataImportadoresExp(DataTable dt, string valueCIFTot, CultureInfo specificCulture)
        {
            var json = new List<object>();
            foreach (DataRow dr in dt.Rows)
            {
                json.Add(new
                {
                    IdImportadorExp = Convert.ToUInt64(dr["IdImportadorExp"]),
                    ImportadorExp = dr["ImportadorExp"].ToString(),
                    CantReg = Convert.ToUInt64(dr["CantReg"]),
                    FOBTot = (Convert.ToSingle(dr["FOBTot"])).ToString("n0", specificCulture),
                    FOBTotPercentaje = (Convert.ToSingle(dr["FOBTot"]) / Convert.ToSingle(valueCIFTot) * 100).ToString("n2", specificCulture) + "%"
                });
            }
            return json;
        }

        private List<object> GeneraJsonDataDistritos(DataTable dt, string CIFTot, string valueCIFTot, CultureInfo specificCulture)
        {
            var json = new List<object>();
            foreach (DataRow dr in dt.Rows)
            {
                json.Add(new
                {
                    IdDistrito = Convert.ToInt64(dr["IdDistrito"]),
                    Distrito = dr["Distrito"].ToString(),
                    CantReg = Convert.ToInt64(dr["CantReg"]),
                    CIForFOBTot = (Convert.ToSingle(dr[CIFTot])).ToString("n0", specificCulture),
                    CIForFOBTotPercentaje = (Convert.ToSingle(dr[CIFTot]) / Convert.ToSingle(valueCIFTot) * 100).ToString("n2", specificCulture) + "%"
                });
            }
            return json;
        }

        private List<object> GeneraJsonDataMarca(DataTable dt, string valueCIFTot, CultureInfo specificCulture)
        {
            var json = new List<object>();
            foreach (DataRow dr in dt.Rows)
            {
                json.Add(new
                {
                    IdMarca = Convert.ToUInt64(dr["IdMarca"]),
                    Marca = dr["Marca"].ToString(),
                    CantReg = Convert.ToUInt64(dr["CantReg"]),
                    FOBTot = (Convert.ToSingle(dr["FOBTot"])).ToString("n0", specificCulture),
                    FOBTotPercentaje = (Convert.ToSingle(dr["FOBTot"]) / Convert.ToSingle(valueCIFTot) * 100).ToString("n2", specificCulture) + "%"
                });
            }
            return json;
        }
        private List<object> GeneraJsonDataModelos(DataTable dt, string valueCIFTot, CultureInfo specificCulture)
        {
            var json = new List<object>();
            foreach (DataRow dr in dt.Rows)
            {
                json.Add(new
                {
                    IdModelo = Convert.ToUInt64(dr["IdModelo"]),
                    Modelo = dr["Modelo"].ToString(),
                    CantReg = Convert.ToUInt64(dr["CantReg"]),
                    FOBTot = (Convert.ToSingle(dr["FOBTot"])).ToString("n0", specificCulture),
                    FOBTotPercentaje = (Convert.ToSingle(dr["FOBTot"]) / Convert.ToSingle(valueCIFTot) * 100).ToString("n2", specificCulture) + "%"
                });
            }
            return json;
        }

        private List<object> GeneraJsonDataPie(string filtro, DataTable dt, string textCIFTot, string valueCIFTot, CultureInfo specificCulture)
        {
            var json = new List<object>();
            int numSectoresCirculares = 5;
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows.Count <= numSectoresCirculares)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        json.Add(new
                        {
                            name = dr[filtro].ToString(),
                            y = Math.Round(Convert.ToDouble(Convert.ToSingle(dr[textCIFTot]) / Convert.ToSingle(valueCIFTot) * 100), 2)
                        });
                    }
                }
                else
                {
                    var dtAux = dt.AsEnumerable().Take(numSectoresCirculares);
                    double sumPercentage = 0;
                    foreach (DataRow dr in dtAux)
                    {
                        var percentage = Math.Round(Convert.ToDouble(Convert.ToSingle(dr[textCIFTot]) / Convert.ToSingle(valueCIFTot) * 100), 2);
                        sumPercentage += percentage;
                        json.Add(new
                        {
                            name = dr[filtro].ToString(),
                            y = percentage
                        });
                    }
                    json.Add(new
                    {
                        name = "[" + Resources.Resources.Others_Text.ToUpper() + "]",
                        y = 100 - sumPercentage
                    });
                }
            }
            return json;
        }

        private List<object> GeneraJsonDataCboAduanaDUA(DataTable dt)
        {
            var json = new List<object>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    json.Add(new
                    {
                        IdAduana = Convert.ToUInt64(dr["IdAduana"]),
                        Aduana = dr["Aduana"].ToString()
                    });
                }
            }
            return json;
        }

        private DataTable CargaFiltro(string CodPais, string CIFTot, string SessionSqlFiltro,
                                        string tabla, string dua, string Filtro,
                                        int pagina = 0)
        {
            var isManif = IsManifiesto(CodPais);
            DataTable dtAgrupado;
            if (pagina == -1)
            {
                dtAgrupado = Consulta.Agrupado(Filtro, CodPais, CIFTot, SessionSqlFiltro, tabla, dua);
                Session["dt" + Filtro] = dtAgrupado;

                string Filtro2;
                DataTable dt;

                //if (Filtro == "Partida") Filtro2 = "Nandina";
                if (Filtro == "AduanaDUA")
                    Filtro2 = "Aduana";
                else
                    Filtro2 = Filtro;

                if (Filtro != "AduanaDUA")
                    dt = dtAgrupado.Copy();
                else
                {
                    dt = Consulta.Agrupado("Aduana", CodPais, CIFTot, SessionSqlFiltro, tabla, dua);
                    Session["dtAduana"] = dt;
                }

                // codigo que no usa el dr1
                //if (Filtro == "Proveedor")
                //    foreach (DataRow dr1 in dt.Rows)
                //        if (dr1["Proveedor"].ToString() == "")
                //        {
                //            dr1["Proveedor"] = "N/A";
                //            break;
                //        }


                // --- Revisar xq da excepción y de que manera se usa
                //DataRow dr = dt.NewRow();
                //dr[0] = "0";
                //dr[Filtro2] = "[TODOS]";
                //dt.Rows.Add(dr);
                //dt.DefaultView.Sort = Filtro2;

                //Session["dt" + Filtro + "2"] = dt;
            }
            else
                dtAgrupado = (DataTable)Session["dt" + Filtro];


            return dtAgrupado;
        }

        public string GeneraSqlFiltro(string mesIni, string mesFin, string DUA, int indexCboPaisB,
            string codPaisB)
        {
            var sql = "";
            string codPais = Session["CodPais"].ToString();
            string auxCodPais = codPais;
            string CodPais2 = Session["CodPais2"].ToString();
            bool isManif = IsManifiesto(codPais);
            var tipoOpe = Session["TipoOpe"].ToString();
            ValidaCodPaisManif(ref codPais, tipoOpe);

            if (CodPais2 == "4UE")
            {
                if (tipoOpe == "I")
                    sql += "and IdPaisImp = " + auxCodPais + " ";
                else
                {
                    sql += "and IdPaisExp = " + auxCodPais + " ";
                }
            }

            sql += "and FechaNum >= " + mesIni + "00 and FechaNum <= " + mesFin + "99 ";
            Session["FechaIni"] = mesIni + "00";
            Session["FechaFin"] = mesFin + "99";


            if (indexCboPaisB > 0)
            {
                if (tipoOpe == "I")
                {
                    sql += "and " + (!isManif ? "IdPaisOrigen" : "IdPaisEmbarque") + " = " + codPaisB + " ";
                }
                else

                {
                    sql += "and IdPaisDestino = " + codPaisB + " ";
                }
            }

            if (!string.IsNullOrWhiteSpace(Session["hdfPalabrasY"]?.ToString()))
            {
                sql += "and contains(Descomercial, '";
                var palabrasY = Session["hdfPalabrasY"].ToString().Split(' ');
                var inicio = true;
                foreach (var palabra in palabrasY)
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
                var nandinas = Session["hdfNandinaB"].ToString().Split('|');
                bool contiene = false;
                foreach (var nandina in nandinas)
                {
                    if (nandina != "")
                    {
                        var existeNandina = (Funciones.BuscaIdPartida(nandina, codPais) != "");
                        if (existeNandina)
                            sql += "IdPartida = " + Funciones.BuscaIdPartida(nandina, codPais) + " or ";
                        else
                            sql += "IdPartida in (select IdPartida from Partida_" + codPais + " where Nandina like '" +
                                   nandina + "%') or ";
                        contiene = true;

                    }
                }
                if (contiene)
                {
                    sql = sql.Substring(0, sql.Length - 3) + ") ";
                }
                else
                {
                    sql = sql.Substring(0, sql.Length - 5);
                }

            }

            if (Session["hdfImportadorB"] != null)
            {
                if (!isManif)
                {
                    sql += "and (";
                    string[] importadores = Session["hdfImportadorB"].ToString().Split('|');
                    foreach (var importador in importadores)
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
                            foreach (var palabra in palabras)
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
                    foreach (var exportador in exportadores)
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
                            foreach (var palabra in palabras)
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
                string word = "";
                if (!isManif)
                {
                    sql += "and IdProveedor in (select IdProveedor from Proveedor_" + codPais + " where ";
                    string[] palabras = Session["hdfProveedorB"].ToString().Split(' ');
                    foreach (var palabra in palabras)
                    {
                        word = (palabra.Replace("[TODOS]", "")).Trim();
                        word = (word.Replace("[ALL]", "")).Trim();
                        sql += "Proveedor like '%" + word + "%' or ";
                    }
                    sql = sql.Substring(0, sql.Length - 3) + ")";
                }
                else
                {
                    sql += "and IdProveedor in (select IdProveedor from Proveedor_" + codPais + " PR where 1 = 1 ";
                    string[] Palabras = Session["hdfProveedorB"].ToString().Split(' ');
                    foreach (string Palabra in Palabras)
                    {
                        word = (Palabra.Replace("[TODOS]", "")).Trim();
                        word = (Palabra.Replace("[ALL]", "")).Trim();
                        sql += "and Proveedor like '%" + word + "%' ";
                    }
                    sql += ") ";
                }
            }

            if (Session["hdfImportadorExpB"] != null)
            {
                if (!isManif)
                {
                    sql += "and IdImportadorExp in (select IdImportadorExp from ImportadorExp_" + codPais + " where 1 = 1 ";
                    string[] Palabras = Session["hdfImportadorExpB"].ToString().Split(' ');
                    foreach (string Palabra in Palabras)
                        sql += "and ImportadorExp like '%" + Palabra + "%' ";
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
                if (Session["MarcaECB"] != null)
                {
                    sql += "and IdMarca in " + Funciones.ListaItems((ArrayList)Session["MarcaECB"]) + " ";
                }
                if (Session["ViasTranspB"] != null)
                    sql += "and IdViaTransp in " + Funciones.ListaItems((ArrayList)Session["ViasTranspB"]) + " ";
                if (Session["AduanaDUAsB"] != null)
                    sql += "and convert(varchar(3), IdAduana) + '-' + convert(varchar(20), " + DUA + ") in " +
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
                {
                    sql += "and IdImportador in " + Funciones.ListaItems((ArrayList)Session["ImportadoresB"]) + " ";
                }
                if (Session["NotificadosB"] != null)
                {
                    sql += "and IdNotificado in " + Funciones.ListaItems((ArrayList)Session["NotificadosB"]) + " ";
                }
                if (Session["ExportadoresB"] != null)
                {
                    sql += "and IdExportador in " + Funciones.ListaItems((ArrayList)Session["ExportadoresB"]) + " ";
                }
                if (Session["ProveedoresB"] != null)
                {
                    sql += "and IdProveedor in " + Funciones.ListaItems((ArrayList)Session["ProveedoresB"]) + " ";
                }
                if (Session["ImportadoresExpB"] != null)
                {
                    sql += "and IdImportadorExp in " + Funciones.ListaItems((ArrayList)Session["ImportadoresExpB"]) + " ";
                }
                if (Session["PaisesOrigenB"] != null)
                {
                    sql += "and IdPaisOrigen in " + Funciones.ListaItems((ArrayList)Session["PaisesOrigenB"]) + " ";
                }
                if (Session["PaisesEmbarqueB"] != null)
                {
                    sql += "and IdPaisEmbarque in " + Funciones.ListaItems((ArrayList)Session["PaisesEmbarqueB"]) + " ";
                }
                if (Session["PaisesDestinoB"] != null)
                {
                    sql += "and IdPaisDestino in " + Funciones.ListaItems((ArrayList)Session["PaisesDestinoB"]) + " ";
                }
                if (Session["PtosDescargaB"] != null)
                {
                    sql += "and IdPtoDescarga in " + Funciones.ListaItems((ArrayList)Session["PtosDescargaB"]) + " ";
                }
                if (Session["PtosEmbarqueB"] != null)
                {
                    sql += "and IdPtoEmbarque in " + Funciones.ListaItems((ArrayList)Session["PtosEmbarqueB"]) + " ";
                }
                if (Session["PtosDestinoB"] != null)
                {
                    sql += "and IdPtoDestino in " + Funciones.ListaItems((ArrayList)Session["PtosDestinoB"]) + " ";
                }
                if (Session["ManifiestosB"] != null)
                {
                    sql += "and Manifiesto in " + Funciones.ListaItemsS((ArrayList)Session["ManifiestosB"]) + " ";
                }
            }
            if (Session["hdfIdGrupoB"] != null)
            {
                sql += "and Id" + Session["hdfTipoFavoritoB"].ToString() +
                       " in (select IdFavorito from FavoritoGrupo where IdGrupo = " + Session["hdfIdGrupoB"] + ") ";
            }

            return sql;
        }

        public DataRow CalculaTotales(string sqlFiltro, string CIFTot, string codPais, string tabla)
        {
            var cifTot1 = CIFTot;
            if (codPais == "BR" || codPais == "IN") cifTot1 = "convert(decimal(19,2), " + CIFTot + ")";

            var sql = "select count(*) as CantReg, sum(" + cifTot1 + ") as " + CIFTot + " ";
            sql += "from " + tabla + " T where 1 = 1 ";
            sql += sqlFiltro;

            var dt = Conexion.SqlDataTable(sql);
            return dt.Rows[0];
        }

        public JsonResult DeleteFilters(string[] valueOptionsSelected)
        {
            if (!ValidarFiltros(valueOptionsSelected))
            {
                return Json(new { estado = true, mensaje = "Los filtros País, Vías y/o Aduanas tienen  que estar acompañados de algún otro filtro" }, JsonRequestBehavior.AllowGet);
            }

            //string[] valueOptions = valueOptionsSelected.Split(',')

            string palabraItem;
            foreach (var item in valueOptionsSelected)
            {
                string ID = item.Substring(3, item.Length - 3);
                switch (item.Substring(0, 3))
                {
                    case "1DE":
                        //string Palabra = item.Text.Replace("[Desc. Comercial] ", "");
                        //hdfPalabrasY.Value = (hdfPalabrasY.Value).Replace(Palabra, "").Replace("  ", " ");

                        palabraItem = item.Substring(4).ToString();
                        string[] palabras = Session["hdfPalabrasY"].ToString().Split(new Char[] { ' ' });
                        int numIndex = Array.IndexOf(palabras, palabraItem);
                        palabras = palabras.Where((val, idx) => idx != numIndex).ToArray();
                        Session["hdfPalabrasY"] = string.Join(" ", palabras);
                        break;
                    case "2P_":
                        ID = item.Substring(4, item.Length - 4);
                        Session["hdfNandinaB"] = (" " + Session["hdfNandinaB"].ToString() + " ").Replace(" " + ID + " ", "")
                                        .Replace("  ", " ").Trim();
                        break;
                    case "2PA":
                        palabraItem = item.Substring(4).ToString();
                        if (!palabraItem.Contains("[G]"))
                        {
                            Session["PartidasB"] = Funciones.EliminaSeleccion((ArrayList)Session["PartidasB"], ID);

                            //if (Session["PartidasB"] != null)
                            //    hdfCantPartidasB.Value =
                            //        ((ArrayList)Session["PartidasB"]).Count.ToString();
                            //else hdfCantPartidasB.Value = "";
                        }
                        break;
                    case "2MA":
                        Session["MarcasB"] = Funciones.EliminaSeleccion((ArrayList)Session["MarcasB"], ID);
                        break;
                    case "2MO":
                        Session["ModelosB"] = Funciones.EliminaSeleccion((ArrayList)Session["ModelosB"], ID);
                        break;
                    case "3I_":
                        ID = item.Substring(4, item.Length - 4);
                        string[] importadoresB = Session["hdfImportadorB"].ToString().Split(new char[] { '|' });
                        int numIndexImpoB = Array.IndexOf(importadoresB, ID);
                        importadoresB = importadoresB.Where((val, idx) => idx != numIndexImpoB).ToArray();
                        Session["hdfImportadorB"] = string.Join("|", importadoresB);
                        break;
                    case "3IM":
                        palabraItem = item.Substring(4).ToString();
                        if (!palabraItem.Contains("[G]"))
                        {
                            Session["ImportadoresB"] = Funciones.EliminaSeleccion((ArrayList)Session["ImportadoresB"], ID);

                            //if (Session["ImportadoresB"] != null)
                            //    hdfCantImportadoresB.Value =
                            //        ((ArrayList)Session["ImportadoresB"]).Count.ToString();
                            //else hdfCantImportadoresB.Value = "";
                        }
                        break;
                    case "3E_":
                        ID = item.Substring(4, item.Length - 4);
                        Session["hdfExportadorB"] = (" " + Session["hdfExportadorB"].ToString() + " ")
                            .Replace(" " + ID + " ", "").Replace("  ", " ").Trim();
                        break;
                    case "3EX":
                        palabraItem = item.Substring(4).ToString();
                        if (!palabraItem.Contains("[G]"))
                        {
                            Session["ExportadoresB"] = Funciones.EliminaSeleccion((ArrayList)Session["ExportadoresB"], ID);

                            //if (Session["ExportadoresB"] != null)
                            //    hdfCantExportadoresB.Value =
                            //        ((ArrayList)Session["ExportadoresB"]).Count.ToString();
                            //else hdfCantExportadoresB.Value = "";
                        }
                        break;
                    case "4P_":
                        ID = item.Substring(4, item.Length - 4);
                        Session["hdfProveedorB"] = (" " + Session["hdfProveedorB"].ToString() + " ").Replace(" " + ID + " ", "")
                            .Replace("  ", " ").Trim();
                        break;
                    case "4PR":
                        palabraItem = item.Substring(4).ToString();
                        if (!palabraItem.Contains("[G]"))
                        {
                            Session["ProveedoresB"] = Funciones.EliminaSeleccion((ArrayList)Session["ProveedoresB"], ID);

                            //if (Session["ProveedoresB"] != null)
                            //    hdfCantProveedoresB.Value =
                            //        ((ArrayList)Session["ProveedoresB"]).Count.ToString();
                            //else hdfCantProveedoresB.Value = "";
                        }
                        break;
                    case "4I_":
                        ID = item.Substring(4, item.Length - 4);
                        Session["hdfImportadorExpB"] = (" " + Session["hdfImportadorExpB"].ToString() + " ")
                            .Replace(" " + ID + " ", "").Replace("  ", " ").Trim();
                        break;
                    case "4IE":
                        palabraItem = item.Substring(4).ToString();
                        if (!palabraItem.Contains("[G]"))
                        {
                            Session["ImportadoresExpB"] = Funciones.EliminaSeleccion((ArrayList)Session["ImportadoresExpB"], ID);

                            //if (Session["ImportadoresExpB"] != null)
                            //    hdfCantImportadoresExpB.Value =
                            //        ((ArrayList)Session["ImportadoresExpB"]).Count.ToString();
                            //else hdfCantImportadoresExpB.Value = "";
                        }
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
                        Session["AduanasB"] = Funciones.EliminaSeleccion((ArrayList)Session["AduanasB"], ID);
                        break;
                    case "8DI":
                        Session["DistritosB"] =
                            Funciones.EliminaSeleccion((ArrayList)Session["DistritosB"], ID);
                        break;
                }
            }
            return Json(new { estado = false, mensaje = "" }, JsonRequestBehavior.AllowGet);
        }

        private bool ValidarFiltros(string[] valuesOption)
        {
            bool bValida = false;
            List<OptionSelect> listOption;
            if (Session["lstFiltros"] != null)
            {
                listOption = Session["lstFiltros"] as List<OptionSelect>;

                for (int i = 0; i < listOption.Count; i++)
                {
                    if (valuesOption.Contains(listOption[i].value))
                        listOption.RemoveAt(i);
                }

                foreach (OptionSelect item in listOption)
                {
                    if (item.text.Contains("[Desc. Comercial]") || item.text.Contains("[Partida]") ||
                    item.text.Contains("[Importador]") || item.text.Contains("[Exportador]"))
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
            return bValida;
        }
        public JsonResult Restablecer()
        {
            string restablece = null;
            Session["hdfPalabrasY"] = restablece;
            Session["hdfImportadorB"] = restablece;
            Session["hdfExportadorB"] = restablece;
            Session["hdfNandinaB"] = restablece;
            Session["hdfProveedorB"] = restablece;

            Session["hdfCantPalabrasY"] = restablece;

            string TipoOpeT = "", CodPaisT = "", CodPais2 = "";
            if (Session["TipoOpe"] != null)
                TipoOpeT = Session["TipoOpe"].ToString();
            if (Session["CodPais"] != null)
                CodPaisT = Session["CodPais"].ToString();
            if (Session["CodPais2"] != null)
                CodPais2 = Session["CodPais2"].ToString();
            string c = "";
            if (Session["c"] != null) c = Session["c"].ToString();
            string cultureAux = Session["culture"] != null ? Session["culture"].ToString() : "";

            Session.RemoveAll();
            Session["c"] = c;
            Session["TipoOpe"] = TipoOpeT;
            Session["CodPais"] = CodPaisT;
            Session["CodPais2"] = CodPais2;
            Session["culture"] = cultureAux;

            return Json(new { estado = true }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AgregarCboFiltro(string filtro, string valorFiltro, string CodPais)
        {
            string tipo = "", nombre = "";
            bool nuevoFiltro = true;
            nombre = AgregarFiltro(filtro, valorFiltro, CodPais, ref tipo);
            if (nombre == "")
                nuevoFiltro = false;

            if (nuevoFiltro)
            {
                List<OptionSelect> listFilters;
                if (Session["lstFiltros"] != null)
                    listFilters = Session["lstFiltros"] as List<OptionSelect>;
                else
                    listFilters = new List<OptionSelect>();

                listFilters.Add(new OptionSelect
                {
                    value = tipo + valorFiltro,
                    text = nombre
                });
            }


            return Json(new
            {
                estado = true,
                nuevoFiltro,
                valueOption = tipo + valorFiltro,
                labelOption = nombre
            }
            , JsonRequestBehavior.AllowGet);
        }

        private string AgregarFiltro(string Filtro, string ID, string CodPais, ref string tipo)
        {
            ArrayList IDsSeleccionados = null;
            string Tipo = "", Nombre = "";
            switch (Filtro)
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
                case "ViaTransp":
                    IDsSeleccionados = (ArrayList)Session["ViasTranspB"];
                    break;
                case "AduanaDUAs":
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
            }

            if (IDsSeleccionados == null) IDsSeleccionados = new ArrayList();
            if (!IDsSeleccionados.Contains(ID))
                IDsSeleccionados.Add(ID);
            else
                return Nombre;

            switch (Filtro)
            {
                case "Partida":
                    Session["PartidasB"] = IDsSeleccionados;
                    //hdfCantPartidasB.Value = IDsSeleccionados.Count.ToString();
                    Tipo = "2PA";
                    Nombre = "[Partida] " + Funciones.BuscaPartida(ID, CodPais);
                    break;
                case "Marca":
                    Session["MarcasB"] = IDsSeleccionados;
                    Tipo = "2MA";
                    Nombre = "[Marca] " + Funciones.BuscaMarca(ID, CodPais);
                    break;
                case "Modelo":
                    Session["ModelosB"] = IDsSeleccionados;
                    Tipo = "2MO";
                    Nombre = "[Modelo] " + Funciones.BuscaModelo(ID, CodPais);
                    break;
                case "Importador":
                    Session["ImportadoresB"] = IDsSeleccionados;
                    //hdfCantImportadoresB.Value = IDsSeleccionados.Count.ToString();
                    Tipo = "3IM";
                    Nombre = "[Importador] " + Funciones.BuscaEmpresa(ID, CodPais);
                    break;
                case "Exportador":
                    Session["ExportadoresB"] = IDsSeleccionados;
                    //hdfCantExportadoresB.Value = IDsSeleccionados.Count.ToString();
                    Tipo = "3EX";
                    Nombre = "[Exportador] " + Funciones.BuscaEmpresa(ID, CodPais);
                    break;
                case "Proveedor":
                    Session["ProveedoresB"] = IDsSeleccionados;
                    //hdfCantProveedoresB.Value = IDsSeleccionados.Count.ToString();
                    Tipo = "4PR";
                    Nombre = "[Exportador] " + Funciones.BuscaProveedor(ID, CodPais);
                    break;
                case "ImportadorExp":
                    Session["ImportadoresExpB"] = IDsSeleccionados;
                    //hdfCantImportadoresExpB.Value = IDsSeleccionados.Count.ToString();
                    Tipo = "4IE";
                    Nombre = "[Importador] " + Funciones.BuscaImportadorExp(ID, CodPais);
                    break;
                case "PaisOrigen":
                    Session["PaisesOrigenB"] = IDsSeleccionados;
                    Tipo = "5PO";
                    Nombre = "[País Origen] " + Funciones.BuscaPais(ID, CodPais);
                    break;
                case "PaisDestino":
                    Session["PaisesDestinoB"] = IDsSeleccionados;
                    Tipo = "5PD";
                    Nombre = "[País Destino] " + Funciones.BuscaPais(ID, CodPais);
                    break;
                case "ViaTransp":
                    Session["ViasTranspB"] = IDsSeleccionados;
                    Tipo = "6VT";
                    Nombre = "[Vía Transporte] " + Funciones.BuscaVia(ID, CodPais);
                    break;
                case "AduanaDUAs":
                    Session["AduanaDUAsB"] = IDsSeleccionados;
                    Tipo = "7AD";
                    Nombre = "[Aduana DUA] " + Funciones.BuscaAduana(ID.Split('-')[0], CodPais) + " - " +
                             ID.Split('-')[1];
                    break;
                case "Aduana":
                    Session["AduanasB"] = IDsSeleccionados;
                    Tipo = "7AA";
                    Nombre = "[Aduana] " + Funciones.BuscaAduana(ID, CodPais);
                    break;
                case "DUA":
                    Session["DUAsB"] = IDsSeleccionados;
                    Tipo = "7DU";
                    Nombre = "[DUA] " + ID;
                    break;
                case "Distrito":
                    Session["DistritosB"] = IDsSeleccionados;
                    Tipo = "8DI";
                    Nombre = "[Distrito] " + Funciones.BuscaDistrito(ID, CodPais);
                    break;
            }
            tipo = Tipo;
            return Nombre;
        }

        public JsonResult AgregarCheckBoxFiltro(string checkSeleccionados, string filtro, string CodPais, string TipoOpe)
        {
            bool ExisteAduana, ExisteDUA;
            ExisteAduana = (CodPais != "CN" && CodPais != "IN" && CodPais != "PY" && CodPais != "US");
            ExisteDUA = ExisteAduana && (CodPais != "AR" && CodPais != "BR" && CodPais != "CO" && CodPais != "MX" &&
                                         CodPais != "UY");

            if (filtro == "AduanaDUA" && !ExisteDUA)
                filtro = "Aduana";

            string[] listSeleccionados = checkSeleccionados.Split(new char[] { ',' });

            //GridView gdv;
            //if (lnkAgregarFiltros.ID.Substring(lnkAgregarFiltros.ID.Length - 1, 1) == "2")
            //    gdv = ObtieneGrid2(Filtro);
            //else
            //    gdv = ObtieneGrid(Filtro);

            var json = new List<object>();
            switch (filtro)
            {
                case "Partida":
                    Session["PartidasB"] = Funciones.GuardaSeleccionados(CodPais, TipoOpe, filtro,
                                                     listSeleccionados, (ArrayList)Session["PartidasB"], ref json);
                    //hdfCantPartidasB.Value = ((ArrayList)Session["PartidasB"]).Count.ToString();
                    break;
                case "Marca":
                    Session["MarcasB"] = Funciones.GuardaSeleccionados(CodPais, TipoOpe, filtro,
                                                   listSeleccionados, (ArrayList)Session["MarcasB"], ref json);
                    break;
                case "Modelo":
                    Session["ModelosB"] = Funciones.GuardaSeleccionados(CodPais, TipoOpe, filtro,
                                                   listSeleccionados, (ArrayList)Session["ModelosB"], ref json);
                    break;
                case "Importador":
                    Session["ImportadoresB"] = Funciones.GuardaSeleccionados(CodPais, TipoOpe, filtro,
                                                       listSeleccionados, (ArrayList)Session["ImportadoresB"], ref json);
                    //hdfCantImportadoresB.Value = ((ArrayList)Session["ImportadoresB"]).Count.ToString();
                    break;
                case "Exportador":
                    Session["ExportadoresB"] = Funciones.GuardaSeleccionados(CodPais, TipoOpe, filtro,
                                                        listSeleccionados, (ArrayList)Session["ExportadoresB"], ref json);
                    //hdfCantExportadoresB.Value = ((ArrayList)Session["ExportadoresB"]).Count.ToString();
                    break;
                case "Proveedor":
                    Session["ProveedoresB"] = Funciones.GuardaSeleccionados(CodPais, TipoOpe, filtro,
                                                    listSeleccionados, (ArrayList)Session["ProveedoresB"], ref json);
                    //hdfCantProveedoresB.Value = ((ArrayList)Session["ProveedoresB"]).Count.ToString();
                    break;
                case "ImportadorExp":
                    Session["ImportadoresExpB"] = Funciones.GuardaSeleccionados(CodPais, TipoOpe, filtro,
                                                   listSeleccionados, (ArrayList)Session["ImportadoresExpB"], ref json);
                    //hdfCantImportadoresExpB.Value = ((ArrayList)Session["ImportadoresExpB"]).Count.ToString();
                    break;
                case "PaisOrigen":
                    Session["PaisesOrigenB"] = Funciones.GuardaSeleccionados(CodPais, TipoOpe, filtro,
                                                         listSeleccionados, (ArrayList)Session["PaisesOrigenB"], ref json);
                    break;
                case "PaisDestino":
                    Session["PaisesDestinoB"] = Funciones.GuardaSeleccionados(CodPais, TipoOpe, filtro,
                                                        listSeleccionados, (ArrayList)Session["PaisesDestinoB"], ref json);
                    break;
                case "ViaTransp":
                    Session["ViasTranspB"] = Funciones.GuardaSeleccionados(CodPais, TipoOpe, filtro,
                                                       listSeleccionados, (ArrayList)Session["ViasTranspB"], ref json);
                    break;
                case "AduanaDUA":
                    Session["AduanaDUAsB"] = Funciones.GuardaSeleccionados(CodPais, TipoOpe, filtro,
                                                        listSeleccionados, (ArrayList)Session["AduanaDUAsB"], ref json);
                    break;
                case "Aduana":
                    //gdv.ID = "gdvAduanas";
                    Session["AduanasB"] = Funciones.GuardaSeleccionados(CodPais, TipoOpe, filtro,
                                                       listSeleccionados, (ArrayList)Session["AduanasB"], ref json);
                    //gdv.ID = "gdvAduanaDUAs";
                    break;
                case "Distrito":
                    Session["DistritosB"] = Funciones.GuardaSeleccionados(CodPais, TipoOpe, filtro,
                                                    listSeleccionados, (ArrayList)Session["DistritosB"], ref json);
                    break;

            }

            bool nuevosFiltros = json.Count > 0 ? true : false;

            return Json(new
            {
                estado = true,
                nuevosFiltros,
                listNuevosFiltros = json
            }
            , JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadDetalle(string TipoOpe, string anioMesIni, string anioMesFin,
                                        string CodPais, string filtro = "")
        {
            var specificCulture = Session["culture"].ToString().Equals("es") ? CultureInfo.CreateSpecificCulture("es-pe") : CultureInfo.CreateSpecificCulture("en-us");
            string tabla = "";
            string dua = "";
            if (TipoOpe == "I")
            {
                tabla = "Importacion_" + CodPais;
                if (CodPais == "PE" || CodPais == "PEB")
                    dua = "NroCorre";
                else if (CodPais == "EC")
                    dua = "Refrendo";
                else
                    dua = "DUA";
                // if (CodPais != "CL")
            }
            else
            {
                tabla = "Exportacion_" + CodPais;
                if (CodPais == "PE") dua = "NroOrden";
                else if (CodPais == "EC") dua = "Refrendo";
                else dua = "DUA";
            }

            string sessionSqlFiltroR = GeneraSqlFiltroR(TipoOpe, anioMesIni, anioMesFin, CodPais, dua, filtro);
            DataTable dt = Consulta.BuscarRegistrosDetalle(sessionSqlFiltroR, tabla);

            var json = (new
            {
                error = false,
                gridData = GeneraJsonColumnAndDataDeDetalle(dt, specificCulture, TipoOpe, CodPais)
            });

            return new JsonResult()
            {
                ContentEncoding = Encoding.Default,
                ContentType = "application/json",
                Data = json,
                //JsonRequestBehavior = requestBehavior,
                MaxJsonLength = int.MaxValue
            };
        }

        private string GeneraSqlFiltroR(string TipoOpe, string anioMesIni, string anioMesFin, string CodPais, string DUA, string filtro)
        {
            string sql = "";

            if (Session["hdfPalabrasY"] != null)
            {
                sql += "and contains(Descomercial, '";
                string[] PalabrasY = Session["hdfPalabrasY"].ToString().Split(' ');
                bool inicio = true;
                foreach (string Palabra in PalabrasY)
                {
                    if (inicio)
                    {
                        sql += "\"" + Palabra + "*\" ";
                        inicio = false;
                    }
                    else
                        sql += "and \"" + Palabra + "*\" ";
                }
                sql += "') ";

                if (TipoOpe == "I")
                    sql += "and IdImportacion in (select IdImportacion from Importacion_" + CodPais + " where 1 = 1 ";
                else
                    sql += "and IdExportacion in (select IdExportacion from Exportacion_" + CodPais + " where 1 = 1 ";
            }

            sql += "and FechaNum >= " + anioMesIni + "00 and FechaNum <= " +
                   anioMesFin + "99 ";
            Session["FechaIni"] = anioMesIni + "00";
            Session["FechaFin"] = anioMesFin + "99";

            if (Session["hdfNandinaB"] != null)
            {
                sql += "and (";
                string[] Nandinas = Session["hdfNandinaB"].ToString().Split('|');
                foreach (string Nandina in Nandinas)
                {
                    bool ExisteNandina = (Funciones.BuscaIdPartida(Nandina, CodPais) != "");
                    if (ExisteNandina)
                        sql += "IdPartida in (" + Funciones.BuscaIdPartida(Nandina, CodPais) + ") or ";
                    //sql += "IdPartida = " + Functions.BuscaIdPartida(Nandina, CodPais) + " or ";
                    else
                        sql += "IdPartida in (select IdPartida from Partida_" + CodPais + " where Nandina like '" +
                               Nandina + "%') or ";
                }
                sql = sql.Substring(0, sql.Length - 3) + ") ";
            }

            if (Session["hdfImportadorB"] != null)
            {
                sql += "and (";
                string[] Importadores = Session["hdfImportadorB"].ToString().Split('|');
                foreach (string Importador in Importadores)
                {
                    if (Importador.Substring(0, 1) == "[")
                        sql += "IdImportador = " + Importador.Replace("[", "").Replace("]", "") + " or ";
                    else
                    {
                        if (CodPais == "PE")
                            sql += "IdImportador in (select IdEmpresa from Empresa_PE_con_datos where 1 = 1 ";
                        else
                            sql += "IdImportador in (select IdEmpresa from Empresa_" + CodPais + " where 1 = 1 ";

                        string[] Palabras = Importador.Split(' ');
                        foreach (string Palabra in Palabras)
                            sql += "and Empresa like '%" + Palabra + "%' ";
                        sql += ") or ";
                    }
                }
                sql = sql.Substring(0, sql.Length - 3) + ") ";
            }

            if (Session["hdfExportadorB"] != null)
            {
                sql += "and (";
                string[] Exportadores = Session["hdfExportadorB"].ToString().Split('|');
                foreach (string Exportador in Exportadores)
                {
                    if (Exportador.Substring(0, 1) == "[")
                        sql += "IdExportador = " + Exportador.Replace("[", "").Replace("]", "") + " or ";
                    else
                    {
                        if (CodPais == "PE")
                            sql += "IdExportador in (select IdEmpresa from Empresa_PE_con_datos where 1 = 1 ";
                        else
                            sql += "IdExportador in (select IdEmpresa from Empresa_" + CodPais + " where 1 = 1 ";

                        string[] Palabras = Exportador.Split(' ');
                        foreach (string Palabra in Palabras)
                            sql += "and Empresa like '%" + Palabra + "%' ";
                        sql += ") or ";
                    }
                }
                sql = sql.Substring(0, sql.Length - 3) + ") ";
            }

            if (Session["hdfProveedorB"] != null)
            {
                sql += "and IdProveedor in (select IdProveedor from Proveedor_" + CodPais + " where 1 = 1 ";
                string[] Palabras = Session["hdfProveedorB"].ToString().Split(' ');
                foreach (string Palabra in Palabras)
                    sql += "and Proveedor like '%" + Palabra + "%' ";
                sql += ") ";
            }

            if (Session["hdfImportadorExpB"] != null)
            {
                sql += "and IdImportadorExp in (select IdImportadorExp from ImportadorExp_" + CodPais + " where 1 = 1 ";
                string[] Palabras = Session["hdfImportadorExpB"].ToString().Split(' ');
                foreach (string Palabra in Palabras)
                    sql += "and ImportadorExp like '%" + Palabra + "%' ";
                sql += ") ";
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
                sql += "and convert(varchar(3), IdAduana) + '-' + convert(varchar(20), " + DUA + ") in " +
                       Funciones.ListaItemsS((ArrayList)Session["AduanaDUAsB"]) + " ";
            if (Session["AduanasB"] != null)
                sql += "and IdAduana in " + Funciones.ListaItems((ArrayList)Session["AduanasB"]) + " ";
            //if (Session["DUAsB"] != null)
            //    sql += "and " + DUA + " like '" + Functions.ListaItems((ArrayList)Session["DUAsB"]).Replace("(", "").Replace(")", "") + "%' ";
            if (Session["DistritosB"] != null)
                sql += "and IdDistrito in " + Funciones.ListaItems((ArrayList)Session["DistritosB"]) + " ";

            //if (hdfIdGrupoB.Value != "")
            //    sql += "and Id" + hdfTipoFavoritoB.Value + " in (select IdFavorito from FavoritoGrupo where IdGrupo = " + hdfIdGrupoB.Value + ") ";

            if (filtro != "")
            {
                sql += "and DesComercial like '%" + filtro + "%' ";
            }

            if (Session["hdfPalabrasY"] != null)
            {
                sql += ") ";
            }

            return sql;
        }

        private object GeneraJsonColumnAndDataDeDetalle(DataTable dt, CultureInfo specificCulture, string tipoOpe, string codPais)
        {

            var jsonData = new List<object>();
            var jsonColumn = new List<object>();
            if (dt.Rows.Count > 0)
            {
                string campoPeso = Funciones.CampoPeso(codPais, tipoOpe);
                bool existeDesComercial = Consulta.FlagDesComercial(codPais, tipoOpe);
                var contador = 0;

                if (tipoOpe == "I")
                {
                    string cif = Funciones.Incoterm(codPais, tipoOpe);
                    bool existeImportador, existeProveedor, existePaisOrigen;
                    existeImportador = Consulta.ExisteVariable(codPais, tipoOpe, "IdImportador");
                    existeProveedor = Consulta.ExisteVariable(codPais, tipoOpe, "IdProveedor");
                    existePaisOrigen = Consulta.ExisteVariable(codPais, tipoOpe, "IdPaisOrigen");

                    jsonColumn = GetSourceColumnsTableDetalle(tipoOpe, codPais, cif,
                                                            existeImportador, existeProveedor, existePaisOrigen,
                                                            false, false, false,
                                                            campoPeso, existeDesComercial);

                    string dataFieldFOBUnit = codPais != "US" ? "FOBUnit" : "FASUnit";
                    bool bFOBUnit = (cif != "FOB" && codPais != "CN" && codPais != "IN");
                    string dataFieldUnit = cif + "Unit";
                    bool CIFImptoUnit = (codPais == "PE");

                    foreach (DataRow dr in dt.Rows)
                    {
                        contador++;
                        jsonData.Add(new
                        {
                            Numero = contador,
                            FechaNumDate = Convert.ToDateTime(dr["Fechanum_date"]).ToString("dd/MM/yyyy", specificCulture),
                            Nandina = dr["Nandina"].ToString(),
                            Importador = existeImportador ? dr["Importador"].ToString() : "",
                            Proveedor = existeProveedor ? dr["Proveedor"].ToString() : "",
                            PesoYYY = campoPeso != "" ? Convert.ToDecimal(dr[campoPeso]).ToString("n2", specificCulture) : "",
                            Cantidad = Convert.ToDecimal(dr["Cantidad"]).ToString("n2", specificCulture),
                            Unidad = dr["Unidad"].ToString(),
                            FOBorFASUnit = bFOBUnit ? Convert.ToDecimal(dr[dataFieldFOBUnit]).ToString("n2", specificCulture) : "",
                            UnitYYY = Convert.ToDecimal(dr[dataFieldUnit]).ToString("n2", specificCulture),
                            CIFImptoUnit = CIFImptoUnit ? Convert.ToDecimal(dr["CIFImptoUnit"]).ToString("n2", specificCulture) : "",
                            PaisOrigen = existePaisOrigen ? dr["PaisOrigen"].ToString() : "",
                            DesComercial = existeDesComercial ? dr["DesComercial"].ToString() : ""
                        });
                    }
                }
                else
                {
                    bool existeExportador, existeImportadorExp, existePaisDestino;
                    existeExportador = Consulta.ExisteVariable(codPais, tipoOpe, "IdExportador");
                    existeImportadorExp = Consulta.ExisteVariable(codPais, tipoOpe, "IdImportadorExp");
                    existePaisDestino = Consulta.ExisteVariable(codPais, tipoOpe, "IdPaisDestino");

                    jsonColumn = GetSourceColumnsTableDetalle(tipoOpe, codPais, "",
                                                            false, false, false,
                                                            existeExportador, existeImportadorExp, existePaisDestino,
                                                            campoPeso, existeDesComercial);

                    foreach (DataRow dr in dt.Rows)
                    {
                        contador++;
                        jsonData.Add(new
                        {
                            Numero = contador,
                            FechaNumDate = Convert.ToDateTime(dr["fechanum_date"]).ToString("dd/MM/yyyy", specificCulture),
                            Nandina = dr["Nandina"].ToString(),
                            Exportador = existeExportador ? dr["Exportador"].ToString() : "",
                            ImportadorExp = existeImportadorExp ? dr["ImportadorExp"].ToString() : "",
                            PesoYYY = campoPeso != "" ? Convert.ToDecimal(dr[campoPeso]).ToString("n2", specificCulture) : "",
                            Cantidad = Convert.ToDecimal(dr["Cantidad"]).ToString("n2", specificCulture),
                            Unidad = dr["Unidad"].ToString(),
                            FOBUnit = Convert.ToDecimal(dr["FOBUnit"]).ToString("n2", specificCulture),
                            PaisDestino = existePaisDestino ? dr["PaisDestino"].ToString() : "",
                            DesComercial = existeDesComercial ? dr["DesComercial"].ToString() : ""
                        });
                    }
                }
            }

            return new { sourceColumns = jsonColumn, sourceData = jsonData };
        }

        private List<object> GetSourceColumnsTableDetalle(string TipoOpe, string CodPais, string CIF,
                                                        bool existeImportador, bool existeProveedor, bool existePaisOrigen,
                                                        bool existeExportador, bool existeImportadorExp, bool existePaisDestino,
                                                        string campoPeso, bool existeDesComercial)
        {
            var list = new List<object>();

            list.Add(new { data = "Numero" }); //column 0
            list.Add(new { data = "FechaNumDate" }); //column 1
            list.Add(new { data = "Nandina" }); //column 2

            if (TipoOpe == "I")
            {
                list.Add(new { data = "Importador", visible = existeImportador }); //column 3 
                list.Add(new { data = "Proveedor", visible = existeProveedor }); //column 4 , render= renderExportador
                list.Add(new { data = "PesoYYY", visible = campoPeso != "" }); //column 5
                list.Add(new { data = "Cantidad" }); //column 6
                list.Add(new { data = "Unidad" }); //column 7
                list.Add(new { data = "FOBorFASUnit", visible = (CIF != "FOB" && CodPais != "CN" && CodPais != "IN") }); //column 8                                
                list.Add(new { data = "UnitYYY" }); //column 9
                list.Add(new { data = "CIFImptoUnit", visible = (CodPais == "PE") }); //column 10
                list.Add(new { data = "PaisOrigen", visible = existePaisOrigen }); //column 11               
            }
            else
            {
                list.Add(new { data = "Exportador", visible = existeExportador });  //column 3                
                list.Add(new { data = "ImportadorExp", visible = existeImportadorExp }); //column 4     
                list.Add(new { data = "PesoYYY", visible = campoPeso != "" }); //column 5
                list.Add(new { data = "Cantidad" }); //column 6
                list.Add(new { data = "Unidad" }); //column 7
                list.Add(new { data = "FOBUnit" }); //column 8
                //list.Add(new { data = "CIFUnit" }); //column 9
                //list.Add(new { data = "CIFImptoUnit" }); //column 10
                list.Add(new { data = "PaisDestino", visible = existePaisDestino }); //column 11               
            }

            list.Add(new { data = "DesComercial", visible = existeDesComercial }); //column 12

            return list;
        }

        //[HttpPost]
        //public ActionResult SetCulture(string culture)
        //{
        //    culture = CultureHelper.GetImplementedCulture(culture);
        //    RouteData.Values["culture"] = culture; // set culture 

        //    return RedirectToAction("Index");
        //}
    }
}