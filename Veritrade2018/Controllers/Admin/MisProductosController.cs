using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using Veritrade2018.App_Start;
using Veritrade2018.Helpers;
using Veritrade2018.Models;
using Veritrade2018.Models.Admin;
using Veritrade2018.Util;

namespace Veritrade2018.Controllers.Admin
{
    public class MisProductosController : BaseController
    {
        private int LimitTitleAddChartTitle = 50;
        private int TablePageSize = 10;
        private int VerRegistrosPageSize = 10;
        // GET: MisProductos
        [AuthorizedAlerts]
        [AuthorizedPlan]        
        [AuthorizedNoReferido]
        public ActionResult Index(string culture, string ruta)
        {
            if (Session["IdUsuario"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            string url = Request.Url.AbsoluteUri;

            int pos = url.IndexOf("?");
            if (!string.IsNullOrEmpty(ruta))
            {
                url = url.Substring(0, pos) + ruta;
            }


            url = url.Replace("&amp;", "&");
            Dictionary<string, string> queryValues = Extensiones.GetQueryValues(url);

            bool opcion = (queryValues.ContainsKey("acc"));

            if (opcion)
            {
                string cxPais = queryValues["p"];

                if(cxPais.Length>0 && cxPais.Substring(0, 2) == "UE")
                {
                    cxPais = queryValues["p"].Substring(2, 1);
                    if (queryValues["p"].Length > 3)
                        cxPais = queryValues["p"].Substring(2, 2);
                }

                Session["CodPais"] = cxPais;
                Session["TipoOpe"] = queryValues["to"];
            }


            string idUsuario = Session["IdUsuario"].ToString();
            bool ocultarVideo = FuncionesBusiness.OcultarVideo(idUsuario);

            string codPais2 = (string)(Session["CodPais2"]??"");
            string codPais = Session["CodPais"].ToString();
            string tipoOpe = Session["TipoOpe"] != null ? Session["TipoOpe"].ToString() : "I";
            if (!ocultarVideo)
            {
                Extensiones.SetCookie("OcultarVideo" + "_id" + idUsuario,
                    (Extensiones.ConvertToInt(Extensiones.GetCookie("OcultarVideo" + "_id" + idUsuario)) + 1).ToString(), TimeSpan.FromDays(1));
                ocultarVideo = Extensiones.ConvertToInt(Extensiones.GetCookie("OcultarVideo" + "_id" + idUsuario)) > 3;
            }


            string tipoUsuarioT = Session["TipoUsuario"].ToString();
            if (tipoUsuarioT == "Free Trial" && Session["ShowVideoProductos"] == null)
            {
                ocultarVideo = false;
                Session["ShowVideoProductos"] = "show";
            }
            else if (tipoUsuarioT == "Free Trial" && Session["ShowVideoProductos"] != null)
            {
                ocultarVideo = true;
            }

            MySearchForm objMySearchForm = new MySearchForm();
            ListaPaises objListaPaises = new ListaPaises();

            List<SelectListItem> countries2 = objListaPaises.GetPaises2(culture).ToList();
            objMySearchForm.ListItemsPais2 = countries2;

            if (opcion)
            {
                if (queryValues["p"].Length > 0 && queryValues["p"].Substring(0, 2) == "UE")
                {
                    codPais2 = "4UE";
                }
                else
                {
                    codPais2 = new ListaPaises().BuscarCodPais2(Session["CodPais"].ToString());
                }
                //codPais2 = (codPais2 == "") ? "1LAT" : codPais2;
                Session["CodPais2"] = codPais2;
            }

            if (countries2.FirstOrDefault(x => x.Value == codPais2) == null)
            {
                objMySearchForm.CodPais2Selected = countries2.First().Value;
                codPais2 = countries2.First().Value;
            }
            else
            {
                objMySearchForm.CodPais2Selected = codPais2;
            }

            

            objMySearchForm.ListItemsOpcion = objMySearchForm.GetPeriod(culture, Session["TipoUsuario"].ToString());
            List<SelectListItem> countries = objMySearchForm.GetCountries(codPais2, culture).ToList();
            objMySearchForm.ListItemsPais = countries;

            if (countries.FirstOrDefault(x => x.Value == codPais) == null)
            {
                objMySearchForm.CodPaisSelected = countries.First().Value;
                codPais = countries.First().Value;
            }
            else
            {
                objMySearchForm.CodPaisSelected = codPais;
            }

            objMySearchForm.IsCheckedImportacion = tipoOpe == Enums.TipoOpe.I.ToString();
            objMySearchForm.IscheckedExportacion = tipoOpe == Enums.TipoOpe.E.ToString();

            bool enabledBtnVerGraficos = false;
            objMySearchForm.ListItemsMyFilters = GetMyProducts(idUsuario, tipoOpe, codPais2, codPais, culture, ref enabledBtnVerGraficos, codPais);
            objMySearchForm.FilterDescription = Resources.AdminResources.Filter_MyProducts_Button;
            objMySearchForm.EnabledBtnVerGraficos = enabledBtnVerGraficos;
            objMySearchForm.FiltroPeriodo = GetPeriods(tipoOpe, codPais2, codPais);
            string seleccionado = "";
            if (opcion)
            {
                DateTime fechaDefault = Convert.ToDateTime(queryValues["fecha"]);
                //objMySearchForm.FiltroPeriodo.DefaultFechaInfoIni = Convert.ToDateTime(fechaDefault.Year + "-" + "01-01");
                objMySearchForm.FiltroPeriodo.DefaultFechaInfoIni = Convert.ToDateTime(fechaDefault.Year + "-" + fechaDefault.Month + "-01");
                objMySearchForm.FiltroPeriodo.DefaultFechaInfoFin = Convert.ToDateTime(fechaDefault.Year + "-" + fechaDefault.Month +"-01");
                seleccionado = queryValues["idFav"]; //FuncionesBusiness.BuscaNandina(queryValues["idFav"], queryValues["p"]);
            }

            objMySearchForm.TipoOperacion = tipoOpe;

            objMySearchForm.IsVisibleRdbUsd = false;
            objMySearchForm.IsVisibleRdbUnid = false;

            Session["Idioma"] = culture;
            Session["CodPais2"] = codPais2;
            Session["CodPais"] = codPais;
            Session["TipoOpe"] = tipoOpe;

            ViewData["Seleccionado"] = seleccionado;
            ViewData["OcultarVideo"] = ocultarVideo;
            ViewData["objMySearchForm"] = objMySearchForm;
            ViewData["UrlVideo"] = Resources.AdminResources.UrlVideo_MyProducts;

            ViewBag.nombreUsuario = Funciones.BuscaUsuario(idUsuario);
            ViewBag.tipoUsuario = tipoUsuarioT;
            ViewBag.plan = Session["Plan"].ToString();


            //desabilitar cboPais2 y cboPais usar la variable flagEnabledCboPais
            bool flagEnabledCboPais2AndPais = true;


            bool flagEnabledrdbImp = true;

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

            if (Session["Plan"].ToString() == "PERU IMEX" || Session["Plan"].ToString() == "ECUADOR IMEX")
            {
                flagEnabledCboPais2AndPais = false;
            }

            else if ((!(bool)(Session["opcionFreeTrial"] ?? false) &&  Session["Plan"].ToString() == "ESENCIAL") || Session["Plan"].ToString() == "PERU UNO" ||
                     Session["Plan"].ToString() == "ECUADOR UNO")

            {
                flagEnabledCboPais2AndPais = false;

                flagEnabledrdbImp = flagCheckedrdbImp = (Session["TipoOpe"].ToString() == "I");

                flagEnabledrdbExp = flagCheckedrdbExp = (Session["TipoOpe"].ToString() == "E");
            }

            ViewData["flagEnabledCboPais2AndPais"] = flagEnabledCboPais2AndPais;
            ViewData["flagCheckedrdbImp"] = flagCheckedrdbImp;
            ViewData["flagCheckedrdbExp"] = flagCheckedrdbExp;
            ViewData["flagEnabledrdbImp"] = flagEnabledrdbImp;
            ViewData["flagEnabledrdbExp"] = flagEnabledrdbExp;
            ViewData["IngresoComoFreeTrial"] = Session["IngresoComoFreeTrial"] != null && (bool)Session["IngresoComoFreeTrial"];

            return View();
        }

        #region MétodosAuxiliares
        private CultureInfo GetSpecificCulture()
        {
            if (Session["culture"] != null)
            {
                return Session["culture"].ToString().Equals("es")
                    ? CultureInfo.CreateSpecificCulture("es-pe")
                    : CultureInfo.CreateSpecificCulture("en-us");
            }
            else
            {
                return CultureInfo.CreateSpecificCulture("es-pe");
            }
        }
        private void ValidaCodPais2(string codPais2, ref string codPais)
        {
            if (codPais2 == "4UE")
                codPais = "UE";
        }
        private string GetCurrentIdioma()
        {
            return Session["Idioma"] != null ? Session["Idioma"].ToString() : "es";
        }
        private IEnumerable<SelectListItem> GetMyProducts(string idUsuario, string tipoOpe, string codPais2,
            string codPais, string idioma, ref bool enabledVerGraficos,
            string auxCodPais = "")
        {
            string codPaisT = codPais;
            if (codPais2 == "4UE")
                codPaisT = "UE" + auxCodPais;
            DataTable dtPartidas =
                FuncionesBusiness.BuscaFavoritos(idUsuario, codPaisT, tipoOpe, "Partida", "", true, idioma);

            var lista = new List<SelectListItem>();
            if (dtPartidas != null && dtPartidas.Rows.Count > 1)
            {
                foreach (DataRow dataRow in dtPartidas.Rows)
                {
                    lista.Add(new SelectListItem { Text = dataRow["Favorito"].ToString(), Value = dataRow["IdFavorito"].ToString() });
                }

                enabledVerGraficos = true;
            }
            else
            {
                lista.Add(new SelectListItem { Text = Resources.AdminResources.DoNotFavoriteProducts_Text, Value = "0" });
                enabledVerGraficos = false;
            }

            return lista;
        }
        private FiltroPeriodo GetPeriods(string tipoOpe, string codPais2, string codPais)
        {
            ValidaCodPais2(codPais2, ref codPais);
            string anioIni = "", mesIni = "", anioFin = "", mesFin = "";
            Funciones.Rango(codPais, tipoOpe, ref anioIni, ref mesIni, ref anioFin, ref mesFin);

            DateTime auxFechaInfoIni = Convert.ToDateTime(anioIni + "-" + mesIni + "-01");
            DateTime auxFechaInfoFin = Convert.ToDateTime(anioFin + "-" + mesFin + "-01");

            if (Session["TipoUsuario"].ToString() == "Free Trial" || (bool)(Session["opcionFreeTrial"]??false))
            {
                auxFechaInfoFin = auxFechaInfoFin.AddMonths(-6);
            }

            int meses = (auxFechaInfoFin.Year - auxFechaInfoIni.Year) * 12 + (auxFechaInfoFin.Month -
                        auxFechaInfoIni.Month);
            meses = (meses < 60) ? meses + 1 : 60 + (auxFechaInfoFin.Month <= 12 ? auxFechaInfoFin.Month : 0);

            if (Session["TipoUsuario"].ToString() == "Free Trial" || (bool)(Session["opcionFreeTrial"] ?? false))
            {
                meses = 3;
            }

            FiltroPeriodo filtroPeriodo = new FiltroPeriodo();

            DateTime tempFechaInfoFin = auxFechaInfoFin;
            filtroPeriodo.FechaInfoFin = auxFechaInfoFin;
            filtroPeriodo.FechaInfoIni = tempFechaInfoFin.AddMonths(-(meses - 1));


            filtroPeriodo.DefaultFechaInfoIni = filtroPeriodo.FechaInfoFin;

            filtroPeriodo.DefaultFechaInfoFin = filtroPeriodo.FechaInfoFin;
            filtroPeriodo.DefaultFechaInfoIni = filtroPeriodo.DefaultFechaInfoIni.AddMonths(-2);


            DateTime auxFechaAnioFin = auxFechaInfoFin;
            var anios = auxFechaInfoFin.Year - auxFechaInfoIni.Year;

            int auxAnios = anios < 6 ? anios + 1 : 6;

            if (anios < 6)
            {
                filtroPeriodo.FechaAnioIni = auxFechaInfoIni;
                filtroPeriodo.FechaAnioFin = auxFechaInfoFin;
            }
            else
            {
                filtroPeriodo.FechaAnioFin = auxFechaInfoFin;
                filtroPeriodo.FechaAnioIni = auxFechaInfoFin.AddYears(-5);
            }

            filtroPeriodo.DefaultFechaAnioFin = auxFechaAnioFin;

            if (auxAnios >= 3)
            {
                filtroPeriodo.DefaultFechaAnioIni = auxFechaAnioFin.AddYears(-2);
            }
            else
            {
                filtroPeriodo.DefaultFechaAnioIni = auxFechaAnioFin.AddYears(-(anios - 1));
            }

            return filtroPeriodo;
        }

        private object ValidaPais2(ref string refCodPais2, ref string refCodPais, string textCodPais)
        {
            object objMensaje = null;
            var codPais2T = refCodPais2;
            string auxCodPais2 = codPais2T;

            var codPaisT = FuncionesBusiness.ObtieneCodPaisAcceso(Session["IdUsuario"].ToString(), ref codPais2T);
            if (codPaisT != "")
            {
                refCodPais = codPaisT;
            }
            else
            {
                var paisT = textCodPais;
                if (auxCodPais2 == "4UE")
                    paisT = "UE - " + paisT;
                string mensaje = "El país seleccionado (" + paisT + ") no está incluído en el plan contratado.";
                if (GetCurrentIdioma() == "en")
                    mensaje = "Selected country: " + paisT + " is not included in your plan.";

                objMensaje = new
                {
                    titulo = "Veritrade",
                    mensaje,
                    flagContactenos = true
                };

                refCodPais2 = codPais2T;
                refCodPais = codPaisT;
            }
            return objMensaje;
        }

        private object ValidaPais(ref string refCodPais2, ref string refCodPais, string textCodPais)
        {
            object objMensaje = null;
            var codPaisT = refCodPais;
            var paisT = textCodPais;

            if (refCodPais2 == "4UE")
            {
                codPaisT = "UE";
                paisT = "UE - " + paisT;
            }

            string idUsuario = Session["IdUsuario"].ToString();

            if (!Funciones.ValidaPais(idUsuario, codPaisT))
            {
                string mensaje = "El país seleccionado (" + paisT + ") no está incluído en el plan contratado.";
                if (GetCurrentIdioma() == "en")
                    mensaje = "Selected country: " + paisT + " is not included in your plan.";

                objMensaje = new
                {
                    titulo = "Veritrade",
                    mensaje,
                    flagContactenos = true
                };

                var codPais2T = refCodPais2;
                codPaisT = FuncionesBusiness.ObtieneCodPaisAcceso(idUsuario, ref codPais2T);
                refCodPais2 = codPais2T;
                refCodPais = codPaisT;
            }
            else
            {
                if (Funciones.FlagCarga(codPaisT))
                {
                    string mensaje = "Estamos actualizando la información de " + paisT +
                                     ". Por favor consulte mas tarde.";
                    if (GetCurrentIdioma() == "en")
                        mensaje = "We are updating data for " + paisT + ". Please return later.";


                    objMensaje = new
                    {
                        titulo = "Veritrade",
                        mensaje,
                        flagContactenos = true
                    };

                    var codPais2T = refCodPais2;

                    codPaisT = FuncionesBusiness.ObtieneCodPaisAcceso(idUsuario, ref codPais2T,true);
                    refCodPais2 = codPais2T;
                    refCodPais = codPaisT;


                }
            }

            return objMensaje;
        }

        private void GetFechasByOpcion(string opcion, string fechaIni, string fechaFin,
            ref string refFechaIni, ref string refFechaFin)
        {
            switch (opcion)
            {
                case "MESES":
                    refFechaIni = fechaIni + "01";
                    break;
                case "ULT12":
                    refFechaIni = Convert.ToDateTime(fechaIni.Substring(0, 4) + "-" + fechaIni.Substring(4, 2) + "-01")
                                      .AddMonths(-11).ToString("yyyyMM") + "01";
                    break;
                case "YTD":
                    refFechaIni = fechaIni.Substring(0, 4) + "0101";
                    break;
                case "AÑOS":
                    refFechaIni = fechaIni.Substring(0, 4) + "0101";
                    break;
            }

            if (opcion != "AÑOS")
            {
                refFechaFin = fechaFin + "99";
            }
            else
            {
                refFechaFin = fechaFin.Substring(0, 4) + "1299";
            }
        }

        private string GetTabla(string tipoOpe, string codPais)
        {
            if (tipoOpe == Enums.TipoOpe.I.ToString())
            {
                return "Importacion_" + codPais;
            }
            else
            {
                return "Exportacion_" + codPais;
            }
        }

        private List<string> GetListStringOfDatatbleColumn(string nameColumn, DataTable dt)
        {
            List<string> lista = new List<string>();
            foreach (DataRow dataRow in dt.Rows)
            {
                lista.Add(dataRow[nameColumn].ToString());
            }
            return lista;
        }

        private List<Decimal> GetListDecimalsOfDataTableColumn(string nameColumn, DataTable dt)
        {
            List<Decimal> lista = new List<decimal>();
            foreach (DataRow dataRow in dt.Rows)
            {
                lista.Add(Math.Round(dataRow.GetValue<decimal>(nameColumn), 3));
            }
            return lista;
        }
        /// <summary>
        /// Obtiene un objeto con propiedades que indican la existencia de variables en la BD
        /// </summary>
        /// <param name="tipoOpe">I: Importaciones, E: Exportaciones</param>
        /// <param name="codPais">Codígo del pais seleccionado en los fitros</param>
        /// <returns></returns>
        private ConsultaForm GetConsultaForm(string tipoOpe, string codPais)
        {
            ConsultaForm model =
                new ConsultaForm
                {
                    Importador = Funciones.ExisteVariable(codPais, tipoOpe, "IdImportador"),
                    Proveedor = Funciones.ExisteVariable(codPais, tipoOpe, "IdProveedor"),
                    Exportador = Funciones.ExisteVariable(codPais, tipoOpe, "IdExportador"),
                    ImportadorExp = Funciones.ExisteVariable(codPais, tipoOpe, "IdImportadorExp"),
                    PaisOrigen = Funciones.ExisteVariable(codPais, tipoOpe, "IdPaisOrigen"),
                    ExistePaisDestino = Funciones.ExisteVariable(codPais, tipoOpe, "IdPaisDestino"),
                    TipoOpe = tipoOpe,
                    CodPais = codPais
                };
            return model;

        }

        private int GetTotalPaginas(int cantidadRegistros)
        {
            return (int)Math.Ceiling(Convert.ToDecimal(cantidadRegistros) / TablePageSize);
        }

        private Dictionary<string, string> GetFiltersByTypeOpe(string tipoOpe)
        {
            Dictionary<string, string> filters = new Dictionary<string, string>();
            if (tipoOpe == Enums.TipoOpe.I.ToString())
            {
                filters.Add("importerOrExporter", Enums.Filtro.Importador.ToString());
                filters.Add("supplierOrIporterExp", Enums.Filtro.Proveedor.ToString());
                filters.Add("originOrDestinationCountry", Enums.Filtro.PaisOrigen.ToString());
            }
            else
            {
                filters.Add("importerOrExporter", Enums.Filtro.Exportador.ToString());
                filters.Add("supplierOrIporterExp", Enums.Filtro.ImportadorExp.ToString());
                filters.Add("originOrDestinationCountry", Enums.Filtro.PaisDestino.ToString());
            }
            return filters;
        }
        #endregion

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult TipoOpeChange(string tipoOpe)
        {
            string idUsuario = Session["IdUsuario"].ToString();
            string codPais2 = Session["CodPais2"].ToString();
            string codPais = Session["CodPais"].ToString();
            Session["TipoOpe"] = tipoOpe;
            string idioma = GetCurrentIdioma();

            MySearchForm objMySearchForm = new MySearchForm();
            bool enabledBtnVerGraficos = false;
            objMySearchForm.ListItemsMyFilters = GetMyProducts(idUsuario, tipoOpe, codPais2, codPais, idioma, ref enabledBtnVerGraficos, codPais);
            objMySearchForm.FiltroPeriodo = GetPeriods(tipoOpe, codPais2, codPais);
            objMySearchForm.EnabledBtnVerGraficos = enabledBtnVerGraficos;
            return Json(new
            {
                objMySearchForm
            });
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult Pais2Change(string codPais2, string pais2Text)
        {
            MySearchForm objMySearchForm = new MySearchForm();

            string idioma = GetCurrentIdioma();
            var auxCountries = objMySearchForm.GetCountries(codPais2, idioma);

            string textCodPais = auxCountries.FirstOrDefault()?.Text;

            if (codPais2 == "4UE")
            {
                textCodPais = pais2Text;
            }

            string auxCodPais2 = codPais2;
            string auxCodPais = "";

            object objMensaje = ValidaPais2(ref auxCodPais2, ref auxCodPais, textCodPais);


            objMySearchForm.CodPais2Selected = auxCodPais2;
            if (objMensaje != null)
            {
                ListaPaises objListaPaises = new ListaPaises();
                List<SelectListItem> countries2 = objListaPaises.GetPaises2(idioma).ToList();

                if (Session["plan"].ToString() == "ESENCIAL")
                {
                    auxCodPais2 = "1LAT";
                    objMySearchForm.CodPais2Selected = auxCodPais2;
                }

                if (countries2.FirstOrDefault(x => x.Value == auxCodPais2) == null)
                {
                    auxCodPais2 = countries2.First().Value;
                    objMySearchForm.CodPais2Selected = auxCodPais2;
                }
                auxCountries = objMySearchForm.GetCountries(auxCodPais2, idioma);
            }
            else
            {
                if (Funciones.FlagCarga(auxCodPais))
                {
                    string mensaje = "Estamos actualizando la información de " + textCodPais +
                                     ". Por favor consulte mas tarde.";
                    if (GetCurrentIdioma() == "en")
                        mensaje = "We are updating data for " + textCodPais + ". Please return later.";


                    objMensaje = new
                    {
                        titulo = "Veritrade",
                        mensaje,
                        flagContactenos = true
                    };
                    auxCodPais = FuncionesBusiness.ObtieneCodPaisAcceso(Session["IdUsuario"].ToString(), ref auxCodPais2, true);
                    if (auxCodPais == "")
                    {
                        auxCodPais = FuncionesBusiness.ObtieneCodPaisAccesoFlag(Session["IdUsuario"].ToString(), ref auxCodPais2);
                        auxCountries = objMySearchForm.GetCountries(auxCodPais2, idioma);//new ListaPaises().GetPaisesAdmin(idioma, auxCodPais2);
                        codPais2 = auxCodPais2;
                    }
                    objMySearchForm.CodPais2Selected = auxCodPais2;
                    //codPaisAselecconar = CodPaisT;
                }
            }

            if (auxCountries.FirstOrDefault(x => x.Value == auxCodPais) == null)
            {
                if (Session["plan"].ToString() == "ESENCIAL")
                {
                    auxCodPais = "PE";
                }
                else
                {
                    auxCodPais = auxCountries.First().Value;
                }
                
            }

            objMySearchForm.CodPaisSelected = auxCodPais;
            objMySearchForm.ListItemsPais = auxCountries;

            string tipoOpe = Session["TipoOpe"].ToString();
            bool enabledBtnVerGraficos = false;

            objMySearchForm.ListItemsMyFilters = GetMyProducts(Session["IdUsuario"].ToString(),
                tipoOpe, auxCodPais2, auxCodPais, idioma, ref enabledBtnVerGraficos, auxCodPais);
            objMySearchForm.EnabledBtnVerGraficos = enabledBtnVerGraficos;
            objMySearchForm.FiltroPeriodo = GetPeriods(tipoOpe, auxCodPais2, auxCodPais);

            Session["CodPais2"] = auxCodPais2;
            Session["CodPais"] = auxCodPais;

            return Json(new
            {
                objMensaje,
                objMySearchForm
            });
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult PaisChange(string codPais, string textCodPais)
        {
            string auxCodPais2 = Session["CodPais2"].ToString();
            string auxCodPais2Actual = auxCodPais2;
            string auxCodPais = codPais;

            MySearchForm objMySearchForm = new MySearchForm();

            object objMensaje = ValidaPais(ref auxCodPais2, ref auxCodPais, textCodPais);

            string tipoOpe = Session["TipoOpe"].ToString();
            bool enabledBtnVerGraficos = false;

            objMySearchForm.CodPais2Selected = auxCodPais2;
            objMySearchForm.CodPaisSelected = auxCodPais;

            if (objMensaje != null)
            {
                string idioma = GetCurrentIdioma();
                ListaPaises objListaPaises = new ListaPaises();

                List<SelectListItem> countries2 = objListaPaises.GetPaises2(idioma).ToList();
                if (countries2.FirstOrDefault(x => x.Value == auxCodPais2) == null)
                {
                    if (Session["plan"].ToString() == "ESENCIAL")
                    {
                        auxCodPais2 = "1LAT";
                    }
                    else
                    {
                        auxCodPais2 = countries2.First().Value;
                    }                    
                    objMySearchForm.CodPais2Selected = auxCodPais2;
                }

                List<SelectListItem> auxCountries = objMySearchForm.GetCountries(auxCodPais2Actual, idioma);
                if (auxCountries.FirstOrDefault(x => x.Value == auxCodPais) == null)
                {
                    if (Session["plan"].ToString() == "ESENCIAL")
                    {
                        auxCodPais = "PE";
                    }else
                    {
                        auxCodPais = auxCountries.First().Value;
                    }
                    
                    objMySearchForm.CodPaisSelected = auxCodPais;
                }
            }

            objMySearchForm.ListItemsMyFilters = GetMyProducts(Session["IdUsuario"].ToString(),
                tipoOpe, auxCodPais2, auxCodPais, GetCurrentIdioma(), ref enabledBtnVerGraficos, auxCodPais);
            objMySearchForm.EnabledBtnVerGraficos = enabledBtnVerGraficos;
            objMySearchForm.FiltroPeriodo = GetPeriods(tipoOpe, auxCodPais2, auxCodPais);

            Session["CodPais2"] = auxCodPais2;
            Session["CodPais"] = auxCodPais;

            return Json(new
            {
                objMensaje,
                objMySearchForm
            });
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult MyFiltersChange(string idFilter, string textPartida, string opcion,
             string fechaIni, string fechaFin)
        {
            string codPais2 = Session["CodPais2"].ToString();
            string tipoOpe = Session["TipoOpe"].ToString();

            string codPais = Session["CodPais"].ToString();

            string auxCodPais = codPais;

            ValidaCodPais2(codPais2, ref auxCodPais);


            string auxFechaIni = "";
            string auxFechaFin = "";
            GetFechasByOpcion(opcion, fechaIni, fechaFin, ref auxFechaIni, ref auxFechaFin);

            string pesoNeto = Funciones.CampoPeso(auxCodPais, tipoOpe);

            string textUnidad = "";

            bool visibleUnid;
            bool checkedUSD = false;

            bool flagChangeCheckedRadio = false;

            DataTable dtUnidad = null;
            if (codPais2 != "4UE")
            {
                dtUnidad = FuncionesBusiness.GetUnidades(tipoOpe, GetTabla(tipoOpe, auxCodPais), codPais2, codPais,
                    textPartida, auxFechaIni, auxFechaFin, idFilter);
            }

            if (dtUnidad != null && dtUnidad.Rows.Count == 1)
            {
                string unidad = dtUnidad.Rows[0]["Unidad"].ToString();

                string auxUnidad = FuncionesBusiness.ObtieneUnidad2(unidad, GetCurrentIdioma());

                if (auxUnidad != "")
                {
                    unidad = auxUnidad;
                }

                textUnidad = unidad;

                if(codPais == "MXD" && unidad == "KIL")
                {
                    textUnidad = Resources.AdminResources.TypeUNIT_Text;
                }

                visibleUnid = true;
            }
            else
            {
                if (pesoNeto != "")
                {
                    textUnidad = Resources.AdminResources.TypeUNIT_Text;
                    visibleUnid = true;
                }
                else
                {
                    checkedUSD = true;
                    flagChangeCheckedRadio = true;
                    visibleUnid = false;
                }
            }


            return Json(new
            {
                textUnidad,
                visibleUSD = true,
                visibleUnid,
                flagChangeCheckedRadio,
                checkedUSD
            });
        }

        #region  VerGráficos
        [SessionExpireFilter]
        [HttpPost]
        public JsonResult VerGraficos(string textPais, string idPartida, string textPartida,
            bool isCheckedUSD, string textUnid, string opcion,
            string anioIniAnioMesIni, string fechaFinAnioMesFin, string fechaIni,
            string fechaFin)
        {
            string auxTextPartida = textPartida.Trim();

            string tipoOpe = Session["TipoOpe"].ToString();
            string codPais2 = Session["CodPais2"].ToString();
            string codPais = Session["CodPais"].ToString();

            string auxCodPais = codPais;
            ValidaCodPais2(codPais2, ref codPais);

            string cif = Funciones.Incoterm(codPais, tipoOpe);
            string cifTot = "";
            string pesoNeto = Funciones.CampoPeso(codPais, tipoOpe);
            if (isCheckedUSD)
            {
                cifTot = cif + "Tot";
            }
            else
            {
                var lkilos = Resources.AdminResources.TypeUNIT_Text_CN;
                if (pesoNeto == "")// <> CN and USA and peso neto = 0
                {
                    if (textUnid != lkilos)
                    {
                        cifTot = "Cantidad";
                    }
                    else
                    {
                        cifTot = pesoNeto;
                    }
                }
                else
                {
                    if (textUnid != Resources.AdminResources.TypeUNIT_Text)
                    {
                        cifTot = "Cantidad";
                    }
                    else
                    {
                        cifTot = pesoNeto;
                    }
                }

            }

            string auxFechaIni = "";
            string auxFechaFin = "";
            GetFechasByOpcion(opcion, fechaIni, fechaFin, ref auxFechaIni, ref auxFechaFin);

            string tabla = GetTabla(tipoOpe, codPais);

            Dictionary<string, string> filtersByTipoOpe = GetFiltersByTypeOpe(tipoOpe);

            DataRow drTotales = FuncionesBusiness.GetTotales(idPartida, auxTextPartida, "", "", auxFechaIni, auxFechaFin, tipoOpe,
                codPais2, codPais, auxCodPais, cif, pesoNeto, tabla, filtersByTipoOpe["importerOrExporter"]);

            string lblMessage = "";

            string idUsuario = Session["IdUsuario"].ToString();

            int cantReg = 0;
            int cantImporterorExporter = 0;
            int cantOriginOrDestinationCountry = 0;
            int cantAux = 0;
            int difCantidad = 0;
            
            if (drTotales == null)
            {
                cantReg = 0;
            }
            else
            {
                cantReg = Convert.ToInt32(drTotales["CantReg"]);
            }
            if (cantReg == 0)
            {
                lblMessage = tipoOpe == Enums.TipoOpe.I.ToString() ? Resources.AdminResources.DoNotFoundImports_Text : Resources.AdminResources.DoNotFoundExports_Text;
                difCantidad = Math.Abs(cantImporterorExporter - cantOriginOrDestinationCountry);
                Session["cantImporterorExporter"] = cantImporterorExporter;
                Session["cantOriginOrDestinationCountry"] = cantOriginOrDestinationCountry;
                FuncionesBusiness.RegistraConsumo(idUsuario, codPais, tipoOpe, "Mis Productos", "");
                return Json(new
                {
                    lblMessage
                });
            }
            else
            {
                IEnumerable<SelectListItem> opcionesDeDescarga = null;
                string idioma = GetCurrentIdioma();
                var specificCulture = GetSpecificCulture();

                var lblTitle = (tipoOpe == Enums.TipoOpe.I.ToString()
                                     ? Resources.AdminResources.Imports_Text.ToUpper()
                                     : Resources.AdminResources.Exports_Text.ToUpper()) + " " +
                                 Resources.AdminResources.From_Text.ToUpper() + " " + textPais.ToUpper() + " " + Resources.AdminResources.Of_text.ToUpper() + " " + auxTextPartida + " ";
                lblTitle += FuncionesBusiness.RangoFechas(auxFechaIni, auxFechaFin, idioma) + " -  ";
                lblTitle += cantReg.ToString("n0", specificCulture) + " " + Resources.AdminResources.Records_Text;



                ResumenPeriodo objResumenPeriodo = GetResumenPeriodo(tipoOpe, tabla, codPais2, auxCodPais,
                    auxTextPartida, auxFechaIni, auxFechaFin, idPartida, cif, pesoNeto, idioma, drTotales);

                string resumenPeriodo =
                    RenderViewToString(this.ControllerContext, "Partials/_ResumenPeriodo", objResumenPeriodo);


                List<string> categories = new List<string>();
                DataTable objDataTable = GetDataTableByOpcion(idPartida, auxTextPartida, opcion, auxFechaIni, auxFechaFin,
                    tipoOpe, codPais2, codPais, auxCodPais, cif, cifTot, pesoNeto, tabla, filtersByTipoOpe["importerOrExporter"], idioma,
                    ref categories);

                Dictionary<string, string> chartsTitles = GetChartsTitles(tipoOpe, cif, cifTot, textUnid,
                    objResumenPeriodo.UnidadesCantidad, idioma, codPais);
                int lenghtTextParitda = auxTextPartida.Length;

                string parteFinalTituloChart = "-" + (lenghtTextParitda > LimitTitleAddChartTitle
                    ? auxTextPartida.Substring(0, LimitTitleAddChartTitle)
                    : auxTextPartida) + "<br>";


                /*foreach (DataRow row in objDataTable.Rows)
                {
                    aux = row["Incoterm"].ToString();
                }*/

                Chart chartMyProductsValorImp = GetDataMyProductsValueImp(objDataTable, categories, cifTot);
                decimal mayor = 0;
                foreach (var lista in chartMyProductsValorImp.Series[0].data)
                {
                    if (lista > mayor)
                        mayor = lista;
                }

                string numero = Convert.ToInt64(mayor).ToString();
                string cadena = "";
                for (int i = 0; i < numero.Length - 1; i++)
                    cadena += "0";

                chartMyProductsValorImp.TitleContainer = chartsTitles["titleValueImp"];
                chartMyProductsValorImp.Title = chartMyProductsValorImp.TitleContainer + parteFinalTituloChart;
                chartMyProductsValorImp.TickInterval = Convert.ToInt64("1" + cadena);
                Chart chartMyProductsPricesProm = null;
                if (auxCodPais != "RD")
                {
                    if (cifTot == "Cantidad" || codPais == "US")
                    {
                        chartMyProductsPricesProm = GetDataMyProductsPricesProm(objDataTable, categories, cif, "Cantidad", !string.IsNullOrEmpty(pesoNeto));
                    }
                    else
                    {
                        chartMyProductsPricesProm = GetDataMyProductsPricesProm(objDataTable, categories, cif, objResumenPeriodo.UnidadCifUnit, !string.IsNullOrEmpty(pesoNeto) );
                    }
                    chartMyProductsPricesProm.TitleContainer = chartsTitles["titlePricesProm"];
                    chartMyProductsPricesProm.Title = chartMyProductsPricesProm.TitleContainer + parteFinalTituloChart;
                }                


                Chart chartMyProductsComparative = GetDataMyProductsComparative(idPartida, auxTextPartida, opcion,
                    anioIniAnioMesIni, fechaFinAnioMesFin, auxFechaIni, auxFechaFin, tipoOpe, codPais2,
                    codPais, auxCodPais, cif, cifTot, pesoNeto, tabla, filtersByTipoOpe["importerOrExporter"], idioma);
                chartMyProductsComparative.TitleContainer = chartsTitles["titleComparative"];
                chartMyProductsComparative.Title = chartMyProductsComparative.TitleContainer + parteFinalTituloChart;


                ConsultaForm objConsultaForm = GetConsultaForm(tipoOpe, codPais);

                string textCiFoFobTot = "Total " + (isCheckedUSD ? cif + " US$ " : objResumenPeriodo.UnidadesCantidad);

                Session["textCiFoFobTot"] = textCiFoFobTot;

                GridHeadTabMy objGridHead = new GridHeadTabMy
                {
                    CiFoFobPor = "%",
                    CiFoFobTot = textCiFoFobTot
                };

                
                decimal valCifTot = !string.IsNullOrEmpty(drTotales[cifTot].ToString())
                    ? Convert.ToDecimal(drTotales[cifTot])
                    : 0;

                string valueCifTot = Convert.ToDecimal(valCifTot).ToString(specificCulture);

                string valueCifTotWithFormat = Convert.ToDecimal(valCifTot).ToString("n0", specificCulture);

                MyProducts objMyProducts = new MyProducts();

                

                if (tipoOpe == Enums.TipoOpe.I.ToString())
                {
                    if (objConsultaForm.Importador && valCifTot > 0)
                    {
                        objMyProducts.ExistImporterOrExporter = true;
                        objMyProducts.ChartImporterOrExporter = GetPieDataByFilter(idPartida, filtersByTipoOpe["importerOrExporter"], cifTot,
                            tipoOpe, codPais2, codPais, auxCodPais, auxTextPartida, auxFechaIni, auxFechaFin, tabla,
                            valueCifTot, chartsTitles["titleRankingImporters"], parteFinalTituloChart);
                        if (textPais == "Colombia")
                        {
                            objGridHead.mostrarColInformaCo = true;
                        }
                        string tableAndWhereSqlImporters = GetTableAndWhereSqlImporters(idPartida, "", auxFechaIni, auxFechaFin,
                            filtersByTipoOpe["importerOrExporter"], tabla, tipoOpe, codPais2, auxCodPais, auxTextPartida);

                        int auxTotalPaginas = 0;
                        
                        objMyProducts.HtmlTableImporterOrExporter = GetTableInHtmlByFilter(objGridHead,
                            tableAndWhereSqlImporters, filtersByTipoOpe["importerOrExporter"], cifTot, valueCifTot, valueCifTotWithFormat,
                            Resources.Resources.Search_Form_Item05, cifTot, specificCulture, objMyProducts.IdImporterOrExporter, ref auxTotalPaginas, ref cantImporterorExporter);

                        objMyProducts.TotalPagesImporterOrExporter = auxTotalPaginas;
                        objGridHead.mostrarColInformaCo = false;
                    }
                    else
                    {
                        objMyProducts.ExistImporterOrExporter = false;
                    }

                    if (objConsultaForm.PaisOrigen && valCifTot > 0)
                    {
                        objMyProducts.ExistOriginOrDestinationCountry = true;
                        objMyProducts.ChartOriginOrDestinationCountry = GetPieDataByFilter(idPartida, filtersByTipoOpe["originOrDestinationCountry"], cifTot,
                            tipoOpe, codPais2, codPais, auxCodPais, auxTextPartida, auxFechaIni, auxFechaFin, tabla,
                            valueCifTot, chartsTitles["titleRankingOriginCountries"], parteFinalTituloChart);

                        string tableAndWhereSqlOriginCountry = GetTableAndWhereSqlProveedoresAndCountries(idPartida, "", "", auxFechaIni,
                            auxFechaFin, tipoOpe, filtersByTipoOpe["originOrDestinationCountry"], codPais2, auxCodPais, tabla, auxTextPartida, filtersByTipoOpe["importerOrExporter"]);

                        string auxCifTot = cifTot;
                        if (codPais == "BR" || codPais == "IN")
                        {
                            auxCifTot = "convert(decimal(19,2), " + cifTot + ")";
                        }

                        int auxTotalPaginas = 0;

                        objMyProducts.HtmlTableOriginOrDestinationCountry = GetTableInHtmlByFilter(objGridHead,
                            tableAndWhereSqlOriginCountry, filtersByTipoOpe["originOrDestinationCountry"], auxCifTot, valueCifTot, valueCifTotWithFormat,
                            Resources.AdminResources.OriginCountry_FormField_Label, cifTot, specificCulture, objMyProducts.IdOriginOrDestinationCountry, ref auxTotalPaginas, ref cantOriginOrDestinationCountry);

                        objMyProducts.TotalPagesOriginOrDestinationCountry = auxTotalPaginas;
                    }
                    else
                    {
                        objMyProducts.ExistOriginOrDestinationCountry = false;
                    }

                    if (objConsultaForm.Proveedor && valCifTot > 0)
                    {
                        objMyProducts.ExistSupplierOrImporterExp = true;
                        objMyProducts.ChartSupplierOrImporterExp = GetPieDataByFilter(idPartida, filtersByTipoOpe["supplierOrIporterExp"], cifTot,
                            tipoOpe, codPais2, codPais, auxCodPais, auxTextPartida, auxFechaIni, auxFechaFin, tabla,
                            valueCifTot, chartsTitles["titleRankingExporters"], parteFinalTituloChart);

                        string tableAndWhereSqlsupplierOrIporterExp = GetTableAndWhereSqlProveedoresAndCountries(idPartida, "", "", auxFechaIni,
                            auxFechaFin, tipoOpe, filtersByTipoOpe["supplierOrIporterExp"], codPais2, auxCodPais, tabla, auxTextPartida, filtersByTipoOpe["importerOrExporter"]);

                        int auxTotalPaginas = 0;
                        objMyProducts.HtmlTableSupplierOrImporterExp = GetTableInHtmlByFilter(objGridHead,
                            tableAndWhereSqlsupplierOrIporterExp, filtersByTipoOpe["supplierOrIporterExp"], cifTot, valueCifTot, valueCifTotWithFormat, codPais.Equals("CL") ?
                            Resources.Resources.Search_Form_BrandField : Resources.Resources.Search_Form_Item06, cifTot, specificCulture, objMyProducts.IdSupplierOrImporterExp, ref auxTotalPaginas, ref cantAux);

                        objMyProducts.TotalPagesSupplierOrImporterExp = auxTotalPaginas;
                    }
                    else
                    {
                        objMyProducts.ExistSupplierOrImporterExp = false;
                    }
                }
                else
                {
                    if (objConsultaForm.Exportador && valCifTot > 0)
                    {
                        if (textPais == "Colombia")
                        {
                            objGridHead.mostrarColInformaCo = true;
                        }
                        objMyProducts.ExistImporterOrExporter = true;
                        objMyProducts.ChartImporterOrExporter = GetPieDataByFilter(idPartida, filtersByTipoOpe["importerOrExporter"], cifTot,
                            tipoOpe, codPais2, codPais, auxCodPais, auxTextPartida, auxFechaIni, auxFechaFin, tabla,
                            valueCifTot, chartsTitles["titleRankingExporters"], parteFinalTituloChart);

                        string tableAndWhereSqlImporters = GetTableAndWhereSqlImporters(idPartida, "", auxFechaIni, auxFechaFin,
                            filtersByTipoOpe["importerOrExporter"], tabla, tipoOpe, codPais2, auxCodPais, auxTextPartida);

                        int auxTotalPaginas = 0;
                        objMyProducts.HtmlTableImporterOrExporter = GetTableInHtmlByFilter(objGridHead,
                            tableAndWhereSqlImporters, filtersByTipoOpe["importerOrExporter"], cifTot, valueCifTot, valueCifTotWithFormat,
                            Resources.Resources.Search_Form_Item06, cifTot, specificCulture, objMyProducts.IdImporterOrExporter, ref auxTotalPaginas, ref cantImporterorExporter);
                        objMyProducts.TotalPagesImporterOrExporter = auxTotalPaginas;
                        objGridHead.mostrarColInformaCo = false;
                    }
                    else
                    {
                        objMyProducts.ExistImporterOrExporter = false;
                    }

                    if (objConsultaForm.ExistePaisDestino && valCifTot > 0)
                    {
                        objMyProducts.ExistOriginOrDestinationCountry = true;
                        objMyProducts.ChartOriginOrDestinationCountry = GetPieDataByFilter(idPartida, filtersByTipoOpe["originOrDestinationCountry"], cifTot,
                            tipoOpe, codPais2, codPais, auxCodPais, auxTextPartida, auxFechaIni, auxFechaFin, tabla,
                            valueCifTot, chartsTitles["titleRankingDestinationCountries"], parteFinalTituloChart);

                        string tableAndWhereSqlOriginCountry = GetTableAndWhereSqlProveedoresAndCountries(idPartida, "", "", auxFechaIni,
                            auxFechaFin, tipoOpe, filtersByTipoOpe["originOrDestinationCountry"], codPais2, auxCodPais, tabla, auxTextPartida, filtersByTipoOpe["importerOrExporter"]);

                        string auxCifTot = cifTot;
                        if (codPais == "BR" || codPais == "IN")
                        {
                            auxCifTot = "convert(decimal(19,2), " + cifTot + ")";
                        }
                        int auxTotalPaginas = 0;
                        objMyProducts.HtmlTableOriginOrDestinationCountry = GetTableInHtmlByFilter(objGridHead,
                            tableAndWhereSqlOriginCountry, filtersByTipoOpe["originOrDestinationCountry"], auxCifTot, valueCifTot, valueCifTotWithFormat,
                            Resources.AdminResources.DestinationCountry_FormField_Label, cifTot, specificCulture, objMyProducts.IdOriginOrDestinationCountry, ref auxTotalPaginas, ref cantOriginOrDestinationCountry);

                        objMyProducts.TotalPagesOriginOrDestinationCountry = auxTotalPaginas;
                    }
                    else
                    {
                        objMyProducts.ExistOriginOrDestinationCountry = false;
                    }

                    if (objConsultaForm.ImportadorExp && valCifTot > 0)
                    {
                        objMyProducts.ExistSupplierOrImporterExp = true;
                        objMyProducts.ChartSupplierOrImporterExp = GetPieDataByFilter(idPartida, filtersByTipoOpe["supplierOrIporterExp"], cifTot,
                            tipoOpe, codPais2, codPais, auxCodPais, auxTextPartida, auxFechaIni, auxFechaFin, tabla,
                            valueCifTot, chartsTitles["titleRankingImporters"], parteFinalTituloChart);

                        string tableAndWhereSqlsupplierOrIporterExp = GetTableAndWhereSqlProveedoresAndCountries(idPartida, "", "", auxFechaIni,
                            auxFechaFin, tipoOpe, filtersByTipoOpe["supplierOrIporterExp"], codPais2, auxCodPais, tabla, auxTextPartida, filtersByTipoOpe["importerOrExporter"]);

                        int auxTotalPaginas = 0;
                        objMyProducts.HtmlTableSupplierOrImporterExp = GetTableInHtmlByFilter(objGridHead,
                            tableAndWhereSqlsupplierOrIporterExp, filtersByTipoOpe["supplierOrIporterExp"], cifTot, valueCifTot, valueCifTotWithFormat,
                            Resources.Resources.Search_Form_Item05, cifTot, specificCulture, objMyProducts.IdSupplierOrImporterExp, ref auxTotalPaginas, ref cantAux);

                        objMyProducts.TotalPagesSupplierOrImporterExp = auxTotalPaginas;
                    }
                    else
                    {
                        objMyProducts.ExistSupplierOrImporterExp = false;
                    }
                }

                DataTable dtOpcionesDeDescarga =
                    FuncionesBusiness.GetOpcionesDeDescarga(idUsuario, tipoOpe, codPais2, codPais,
                        idioma, auxCodPais);

                opcionesDeDescarga = new List<SelectListItem>();

                if (dtOpcionesDeDescarga != null && dtOpcionesDeDescarga.Rows.Count > 0)
                {
                    opcionesDeDescarga = dtOpcionesDeDescarga.AsEnumerable().Select(m => new SelectListItem()
                    {
                        Text = m.Field<string>("Descarga"),
                        Value = (m.Field<Int32>("IdDescargaCab")).ToString(),
                        Selected = m.Field<string>("FlagDefault") == "S"
                    });
                }

                object objMensajeCantRegMax = null;

                if (cantReg > Variables.CantRegMax)
                {
                    //string descMensaje = "No se puede descargar a Excel, porque supera el límite de " + cantReg.ToString("n0", specificCulture) + " registros por descarga";
                    ////if (idioma == "en")
                    ////{

                    ////}

                    string descMensaje = "Su búsqueda supera el límite de " + Variables.CantRegMax.ToString("n0", specificCulture) +
                                         " registros y no puede ser descargada en Excel. ";
                    descMensaje +=
                        "Si desea bajar información, reduzca el rango de fechas.";

                    if (idioma == "en")
                    {
                        descMensaje = "Your search exceeds " + Variables.CantRegMax.ToString("n0", specificCulture) +
                                      " records limit and it can not be download to Excel. ";
                        descMensaje +=
                            "If you want to download information, reduce the range of dates.";
                    }

                    objMensajeCantRegMax = new
                    {
                        titulo = Resources.Resources.Search_Button,
                        mensaje = descMensaje,
                        flagContactenos = true
                    };
                }

                Session["cifTot"] = cifTot;
                Session["valueCifTot"] = valueCifTot;
                Session["idPartida"] = idPartida;
                Session["textPartida"] = auxTextPartida;
                Session["fechaIni"] = auxFechaIni;
                Session["fechaFin"] = auxFechaFin;

                Session["auxFechaIni"] = auxFechaIni; //Se usa el método de exportar todo
                Session["auxFechaFin"] = auxFechaFin; // Se usa en el método de exportar todo
                Session["auxCantReg"] = cantReg; //Usado en la función de exportar todo

                FuncionesBusiness.RegistraConsumo(idUsuario, codPais, tipoOpe, "Mis Productos", "");
                difCantidad = Math.Abs(cantImporterorExporter - cantOriginOrDestinationCountry);
                Session["cantImporterorExporter"] = cantImporterorExporter;
                Session["cantOriginOrDestinationCountry"] = cantOriginOrDestinationCountry;
                return Json(new
                {
                    lblMessage,
                    lblTitle,
                    opcionesDeDescarga,
                    resumenPeriodo,
                    chartMyProductsValorImp,
                    chartMyProductsPricesProm,
                    chartMyProductsComparative,
                    objMyProducts,
                    objMensajeCantRegMax,
                    difCantidad
                });
            }
        }

        private DataTable GetDataTableByOpcion(string idPartida, string textPartida, string opcion,
            string fechaIni, string fechaFin, string tipoOpe,
            string codPais2, string codPais, string auxCodPais,
            string cif, string cifTot, string pesoNeto,
            string tabla, string importador, string idioma, ref List<string> refListCategories)
        {
            DataTable dataTable;
            if (opcion != "AÑOS")
            {
                dataTable = FuncionesBusiness.Mensuales(idPartida, textPartida, "", "", fechaIni, fechaFin, tipoOpe, codPais2,
                    auxCodPais, cif, pesoNeto, tabla, importador, idioma);

                refListCategories = GetListStringOfDatatbleColumn("IdAñoMes", dataTable);
            }
            else
            {
                dataTable = FuncionesBusiness.Anuales(idPartida, textPartida, "", "", fechaIni, fechaFin, tipoOpe, codPais2, codPais,
                    auxCodPais, cif, cifTot, pesoNeto, tabla, importador);
                refListCategories = GetListStringOfDatatbleColumn("IdAño", dataTable);
            }

            return dataTable;
        }

        private Chart GetDataMyProductsValueImp(DataTable dataSource, List<string> categories, string cifTot)
        {
            //analizar si es neceario usar la siguiente lógica   string FormatoMoneda = "{V:C0}";
            //if (CIFTot != CIF + "Tot") FormatoMoneda = "{V:N0}";

            var objChart = new Chart { Categories = categories };
            objChart.Series.Add(new ChartSerie
            {
                data = GetListDecimalsOfDataTableColumn(cifTot, dataSource)
            });
            return objChart;
        }

        private Chart GetDataMyProductsPricesProm(DataTable dataSource, List<string> categories, string cif,
            string unidad, bool hasPeso = false)
        {
            var objChart = new Chart { Categories = categories };
            if (unidad != Resources.AdminResources.TypeUNIT_Text || !hasPeso )
            {
                objChart.Series.Add(new ChartSerie
                {
                    data = GetListDecimalsOfDataTableColumn(cif + "Unit", dataSource)
                });
            }
            else
            {
                objChart.Series.Add(new ChartSerie
                {
                    data = GetListDecimalsOfDataTableColumn(cif + "Unit2", dataSource)
                });
            }
            return objChart;
        }

        private Chart GetDataMyProductsComparative(string idPartida, string textPartida, string opcion,
            string anioIniAnioMesIni, string fechaFinAnioMesFin, string fechaIni, string fechaFin,
            string tipoOpe, string codPais2, string codPais,
            string auxCodPais, string cif, string cifTot,
            string pesoNeto, string tabla, string importador, string idioma)
        {

            //analizar si es neceario usar la siguiente lógica   string FormatoMoneda = "{V:C0}";
            //if (CIFTot != CIF + "Tot") FormatoMoneda = "{V:N0}";

            Chart objChart = new Chart();

            DataTable objDataTable;
            if (opcion != "AÑOS")
            {
                string auxFechaIni, auxFechaFin;

                var anioInfoIni = Convert.ToInt32(anioIniAnioMesIni);
                var anioFin = Convert.ToInt32(fechaFin.Substring(0, 4));
                var anioIni = Convert.ToInt32(fechaIni.Substring(0, 4));

                if (anioFin - anioIni < 2)
                    anioIni = anioFin - 2;
                if (anioIni < anioInfoIni)
                    anioIni = anioInfoIni;


                List<string> listCategories = new List<string>();

                for (int anio = anioFin; anio >= anioIni; anio -= 1)
                {
                    auxFechaIni = anio + "0101";
                    if (anio == anioFin && Convert.ToInt32(fechaFinAnioMesFin) < Convert.ToInt32(anio + "12"))
                    {
                        auxFechaFin = fechaFinAnioMesFin + "99";
                    }
                    else
                    {
                        auxFechaFin = anio + "1299";
                    }

                    objDataTable = FuncionesBusiness.Mensuales(idPartida, textPartida, "", "", auxFechaIni, auxFechaFin, tipoOpe, codPais2,
                        auxCodPais, cif, pesoNeto, tabla, importador, idioma);


                    listCategories = GetListStringOfDatatbleColumn("IdMes", objDataTable);

                    if (listCategories.Count > objChart.Categories.Count)
                        objChart.Categories = listCategories;

                    var objChartSerie = new ChartSerie
                    {
                        name = anio.ToString(),
                        data = GetListDecimalsOfDataTableColumn(cifTot, objDataTable)
                    };
                    objChart.Series.Add(objChartSerie);
                }

            }
            else
            {
                objDataTable = FuncionesBusiness.Anuales(idPartida, textPartida, "", "", fechaIni, fechaFin, tipoOpe, codPais2, codPais,
                    auxCodPais, cif, cifTot, pesoNeto, tabla, importador);

                objChart.Categories = GetListStringOfDatatbleColumn("IdAño", objDataTable);
                objChart.Series.Add(new ChartSerie
                {
                    data = GetListDecimalsOfDataTableColumn(cifTot, objDataTable)
                });
            }

            return objChart;
        }

        private Dictionary<string, string> GetChartsTitles(string tipoOpe, string cif, string cifTot,
            string textUnit, string unidadesCantidad, string idioma, string codPais = "")
        {
            Dictionary<string, string> titles = new Dictionary<string, string>();
            if (idioma == "es")
            {
                titles.Add("titlePricesProm", "Precio Promedio U$ / " + unidadesCantidad);
                if (tipoOpe == Enums.TipoOpe.I.ToString())
                {
                    if (cifTot == cif + "Tot")
                    {
                        titles.Add("titleValueImp", "Valor " + cif + " Importado US$");
                        titles.Add("titleComparative", "Comparativo " + cif + " US$");
                    }
                    else
                    {
                        titles.Add("titleValueImp", "Cantidad Importada (" + textUnit + ")");
                        titles.Add("titleComparative", "Comparativo(" + textUnit + ")");
                    }
                }
                else
                {
                    if (cifTot == cif + "Tot")
                    {
                        titles.Add("titleValueImp", "Valor " + cif + " Exportado US$");
                        titles.Add("titleComparative", "Comparativo " + cif + " US$");
                    }
                    else
                    {
                        titles.Add("titleValueImp", "Cantidad Exportada (" + textUnit + ")");
                        titles.Add("titleComparative", "Comparativo " + cif + " US$");
                    }
                }
            }
            else
            {
                titles.Add("titlePricesProm", "Average Price U$ / " + unidadesCantidad);
                if (tipoOpe == Enums.TipoOpe.I.ToString())
                {
                    if (cifTot == cif + "Tot")
                    {
                        titles.Add("titleValueImp", "Imported " + cif + " US$");
                        titles.Add("titleComparative", "Comparative " + cif + " US$");
                    }
                    else
                    {
                        titles.Add("titleValueImp", "Imported Quantity (" + textUnit + ")");
                        titles.Add("titleComparative", "Comparative (" + textUnit + ")");
                    }
                }
                else
                {
                    if (cifTot == cif + "Tot")
                    {
                        if (codPais.Equals("CL"))
                            titles.Add("titleValueImp", "Exporter " + cif + " US$");
                        else
                            titles.Add("titleValueImp", "Exporter " + cif + " US$");
                        titles.Add("titleComparative", "Comparative " + cif + " US$");
                    }
                    else
                    {
                        titles.Add("titleValueImp", "Exported Quantity (" + textUnit + ")");
                        titles.Add("titleComparative", "Comparative (" + textUnit + ")");
                    }
                }
            }


            titles.Add("titleRankingImporters", Resources.AdminResources.ImportersSummary_Text);
            titles.Add("titleRankingExporters", codPais.Equals("CL") ? Resources.AdminResources.BrandSummary_Text : Resources.AdminResources.ExportersSummary_Text);
            titles.Add("titleRankingOriginCountries", Resources.AdminResources.OriginCountriesSummary_Text);
            titles.Add("titleRankingDestinationCountries", Resources.AdminResources.DestinationCountriesSummary_Text);

            return titles;
        }

        private ResumenPeriodo GetResumenPeriodo(string tipoOpe, string tabla, string codPais2,
            string auxCodPais, string auxTextPartida, string auxFechaIni,
            string auxFechaFin, string idPartida, string cif,
            string pesoNeto, string idioma, DataRow drTotales)
        {
            var specificCulture = GetSpecificCulture();

            ResumenPeriodo objResumenPeriodo =
                new ResumenPeriodo { DescripcionPrecioUnitario = Resources.AdminResources.UnitPrice_Text };
            if (tipoOpe == Enums.TipoOpe.I.ToString())
            {
                objResumenPeriodo.DescripcionTotal = Resources.AdminResources.TotalImported_Text;
                objResumenPeriodo.DescripcionCantidad = Resources.AdminResources.ImportedQuanty_Text;
            }
            else
            {
                objResumenPeriodo.DescripcionTotal = Resources.AdminResources.TotalExported_Text;
                objResumenPeriodo.DescripcionCantidad = Resources.AdminResources.ExportedQuanty_Text;
            }

            DataTable dtUnid = null;
            if (codPais2 != "4UE")
                dtUnid = FuncionesBusiness.GetUnidades(tipoOpe, tabla, codPais2, auxCodPais, auxTextPartida, auxFechaIni,
                    auxFechaFin, idPartida);

            objResumenPeriodo.CifTot = Convert.ToDecimal(drTotales[cif + "Tot"]).ToString("n0", specificCulture);

            if (dtUnid != null && dtUnid.Rows.Count == 1)
            {
                objResumenPeriodo.Cantidad = "";
                objResumenPeriodo.Cantidad = Convert.ToDecimal(drTotales["Cantidad"]).ToString("n0", specificCulture);
                if (objResumenPeriodo.Cantidad != "0")
                {
                    objResumenPeriodo.CifUnit = (Convert.ToDecimal(drTotales[cif + "Tot"]) / Convert.ToDecimal(drTotales["Cantidad"])).ToString("n3", specificCulture);
                    objResumenPeriodo.UnidadesCantidad = objResumenPeriodo.UnidadCifUnit = FuncionesBusiness.ObtieneUnidad2(dtUnid.Rows[0]["Unidad"].ToString(), idioma);
                }
                else
                {
                    objResumenPeriodo.Cantidad = "-";
                }
            }
            else if (pesoNeto != "")
            {
                objResumenPeriodo.Cantidad = Convert.ToDecimal(drTotales[pesoNeto]).ToString("n0", specificCulture);
                objResumenPeriodo.CifUnit = auxCodPais == "RD" ? "0.000" : (Convert.ToDecimal(drTotales[cif + "Tot"]) / Convert.ToDecimal(drTotales[pesoNeto])).ToString("n3", specificCulture);
                objResumenPeriodo.UnidadesCantidad = objResumenPeriodo.UnidadCifUnit = Resources.AdminResources.TypeUNIT_Text;
            }

            return objResumenPeriodo;
        }

        private List<PieData> GetListPieData(DataTable dt, string filtro)
        {
            var lista = new List<PieData>();
            foreach (DataRow dataRow in dt.Rows)
            {
                if (!string.IsNullOrEmpty(dataRow["Participacion"].ToString()))
                {
                    lista.Add(new PieData
                    {
                        id = dataRow["Id" + filtro].ToString(),
                        name = dataRow[filtro].ToString(),
                        y = Convert.ToDecimal(dataRow["Participacion"])
                    });
                }
            }
            return lista;
        }

        private List<GridRowTabMy> GetGridRowsByFilter(DataTable dt, string filtro, string cifTot,
            CultureInfo cultureInfo)
        {
            var lista = new List<GridRowTabMy>();
            foreach (DataRow dataRow in dt.Rows)
            {
                if (!string.IsNullOrEmpty(dataRow[cifTot].ToString()) &&
                    !string.IsNullOrEmpty(dataRow["Participacion"].ToString()))
                {
                    string numInformaCo = null;
                    if (dataRow.Table.Columns.Contains("numero_id") && !DBNull.Value.Equals(dataRow["numero_id"]))
                    {
                        numInformaCo = Convert.ToString(dataRow["numero_id"]);
                    }
                    lista.Add(new GridRowTabMy
                    {
                        Id = dataRow["Id" + filtro].ToString(),
                        Descripcion = dataRow[filtro].ToString(),
                        CiFoFobTot = Convert.ToDecimal(dataRow[cifTot]).ToString("n0", cultureInfo),
                        CiFoFobPor = Convert.ToDecimal(dataRow["Participacion"]).ToString("n1", cultureInfo),
                        numInformaColombia = numInformaCo
                        
                    });
                }
            }
            return lista;
        }

        private string GetTableAndWhereSqlImporters(string idPartida, string desComercial, string fechaIni,
            string fechaFin, string importador, string tabla,
            string tipoOpe, string codPais2, string auxCodPais,
            string textPartida)
        {
            string sql = "";
            if(auxCodPais == "CO")
            {
                sql += $" ,informa_colombia.numero_id from {tabla} left join informa_colombia on {tabla}.ruc = informa_colombia.numero_id ";
            } else
            {
                sql += " from " + tabla;
            }
            sql += " where FechaNum between " + fechaIni + " and " + fechaFin + " ";

            if (codPais2 == "4UE")
                sql += "and " + (tipoOpe == "I" ? "IdPaisImp" : "IdPaisExp") + " = " + auxCodPais + " ";
            if (textPartida.Substring(0, 3) != "[G]")
                sql += "and IdPartida = " + idPartida + " ";
            else
                sql += "and IdPartida in (select IdFavorito from FavoritoGrupo where IdGrupo = " + idPartida + ") ";
            if (desComercial != "")
                sql += "and contains(DesComercial, '\"" + desComercial + "\"') ";
            sql += "group by Id" + importador + ", " + importador;
            if (auxCodPais == "CO")
            {
                sql += $" ,informa_colombia.numero_id ";
            }

            return sql;
        }

        private string GetTableAndWhereSqlProveedoresAndCountries(string idPartida, string desComercial, string idEmpresa,
            string fechaIni, string fechaFin, string tipoOpe,
            string filtro, string codPais2, string auxCodPais,
            string tabla, string textPartida, string importador)
        {
            string sql = " from " + tabla + " where FechaNum between " + fechaIni + " and " + fechaFin + " ";
            if (codPais2 == "4UE")
                sql += "and " + (tipoOpe == "I" ? "IdPaisImp" : "IdPaisExp") + " = " + auxCodPais + " ";
            if (idPartida != "")
            {
                if (textPartida.Substring(0, 3) != "[G]")
                    sql += "and IdPartida = " + idPartida + " ";
                else
                    sql += "and IdPartida in (select IdFavorito from FavoritoGrupo where IdGrupo = " + idPartida + ") ";
                if (desComercial != "")
                    sql += "and contains(DesComercial, '\"" + desComercial + "\"') ";
            }
            else
                sql += "and Id" + importador + " = " + idEmpresa + " ";
            sql += "group by Id" + filtro + ", " + filtro;

            return sql;
        }

        private Chart GetPieDataByFilter(string idPartida, string filtro,
            string cifTot, string tipoOpe, string codPais2,
            string codPais, string auxCodPais, string textPartida,
            string fechaIni, string fechaFin, string tabla,
            string valueCifTot, string chartTitle, string parteFinalChartTitle)
        {
            DataTable pieData = FuncionesBusiness.GetPiesData(idPartida, "", filtro, cifTot,
                tipoOpe, codPais2, codPais, auxCodPais, textPartida, fechaIni, fechaFin, tabla,
                valueCifTot);

            var chart = new Chart
            {
                PieDatas = GetListPieData(pieData, filtro),
                TitleContainer = chartTitle
            };
            chart.Title = chart.TitleContainer + parteFinalChartTitle;
            return chart;
        }

        private string GetTableInHtmlByFilter(GridHeadTabMy objGridHead, string sqlTableAndWhere, string filtro,
            string cifTot, string valueCifTot, string valueCifTotWithFormat,
            string headColumnDescription, string auxCifTot, CultureInfo cultureInfo,
            string idUlPaging, ref int totalPaginas, ref int cantPorPagina)
        {
            int cantidadRegistros = FuncionesBusiness.GetCantidadRegistros(sqlTableAndWhere, filtro);
            totalPaginas = GetTotalPaginas(cantidadRegistros);

            DataTable dtTable = FuncionesBusiness.GetRegistrosByFiltro(sqlTableAndWhere,
                filtro, cifTot, valueCifTot, 1, TablePageSize, auxCifTot);
            cantPorPagina = dtTable.Rows.Count;            
            TabDataTabMy viewData = new TabDataTabMy { GridHead = objGridHead };

            viewData.GridHead.Descripcion = headColumnDescription;
            viewData.ListRows = GetGridRowsByFilter(dtTable, filtro, auxCifTot, cultureInfo);
            viewData.CiFoFobTotal = valueCifTotWithFormat;
            viewData.TotalPaginasTab = totalPaginas;
            viewData.Filtro = idUlPaging;

            return RenderViewToString(this.ControllerContext, "GridViews/TableView", viewData);
        }

        [HttpPost]
        public JsonResult VerInformaColombia(string ruc)
        {
            InformaColombia informa = new InformaColombia();
            if (ruc != null && ruc != "")
            {
                informa = InformaColombia.obtenerInformePorRuc(ruc);
            }

            return Json(new
            {
                informaColombia = RenderViewToString(this.ControllerContext, "Modals/_InformaColombia", informa)
            });
        }


        [SessionExpireFilter]
        [HttpPost]
        public JsonResult PageIndexChanging(string idFiltro, int pagina)
        {
           
            string tipoOpe = Session["TipoOpe"].ToString();
            string codPais2 = Session["CodPais2"].ToString();
            string codPais = Session["CodPais"].ToString();

            string auxCodPais = codPais;
            ValidaCodPais2(codPais2, ref codPais);

            string auxTextPartida = Session["textPartida"].ToString();
            string auxFechaIni = Session["auxFechaIni"].ToString();
            string auxFechaFin = Session["auxFechaFin"].ToString();
            string idPartida = Session["idPartida"].ToString();
            string cifTot = Session["cifTot"].ToString();
            string valueCifTot = Session["valueCifTot"].ToString();

            string tabla = GetTabla(tipoOpe, codPais);
            var specificCulture = GetSpecificCulture();
            Dictionary<string, string> filtersByTipoOpe = GetFiltersByTypeOpe(tipoOpe);

            string rowsByPage = "";

            int cantImporterOrExporter = 0;
            int cantOriginOrDestinationCountry = 0;
            int cantAux = 0;
            bool mostrarInformaColombia = false;
            switch (idFiltro)
            {
                case "ImporterOrExporter":
                    string tableAndWhereSqlImporters = GetTableAndWhereSqlImporters(idPartida, "", auxFechaIni, auxFechaFin,
                        filtersByTipoOpe["importerOrExporter"], tabla, tipoOpe, codPais2, auxCodPais, auxTextPartida);
                    if (codPais == "CO")
                    {
                        mostrarInformaColombia = true;
                    }
                    rowsByPage = GetRowsInHtmlByFilter(tableAndWhereSqlImporters, filtersByTipoOpe["importerOrExporter"],
                        cifTot, valueCifTot, cifTot, pagina, specificCulture, idFiltro, ref cantImporterOrExporter, mostrarInformaColombia);
                    break;
                case "OriginOrDestinationCountry":
                    string tableAndWhereSqlOriginCountry = GetTableAndWhereSqlProveedoresAndCountries(idPartida, "", "", auxFechaIni,
                        auxFechaFin, tipoOpe, filtersByTipoOpe["originOrDestinationCountry"], codPais2, auxCodPais, tabla, auxTextPartida, filtersByTipoOpe["importerOrExporter"]);
                    string auxCifTot = cifTot;
                    if (codPais == "BR" || codPais == "IN")
                    {
                        auxCifTot = "convert(decimal(19,2), " + cifTot + ")";
                    }
                    rowsByPage = GetRowsInHtmlByFilter(tableAndWhereSqlOriginCountry, filtersByTipoOpe["originOrDestinationCountry"],
                        cifTot, valueCifTot, auxCifTot, pagina, specificCulture, idFiltro, ref cantOriginOrDestinationCountry);
                    break;
                case "SupplierOrImporterExp":
                    string tableAndWhereSqlsupplierOrIporterExp = GetTableAndWhereSqlProveedoresAndCountries(idPartida, "", "", auxFechaIni,
                        auxFechaFin, tipoOpe, filtersByTipoOpe["supplierOrIporterExp"], codPais2, auxCodPais, tabla, auxTextPartida, filtersByTipoOpe["importerOrExporter"]);

                    rowsByPage = GetRowsInHtmlByFilter(tableAndWhereSqlsupplierOrIporterExp, filtersByTipoOpe["supplierOrIporterExp"],
                        cifTot, valueCifTot, cifTot, pagina, specificCulture, idFiltro, ref cantAux);
                    break;
            }

            int cantAnte = 0;
            int cantNueva = 0;

            if (idFiltro == "OriginOrDestinationCountry")
            {
                cantAnte = Convert.ToInt32(Session["cantImporterorExporter"]);
                cantNueva = cantOriginOrDestinationCountry;
                Session["cantOriginOrDestinationCountry"] = cantOriginOrDestinationCountry;
            }
            else if (idFiltro == "ImporterOrExporter")
            {
                cantAnte = Convert.ToInt32(Session["cantOriginOrDestinationCountry"]);
                cantNueva = cantImporterOrExporter;
                Session["cantImporterOrExporter"] = cantImporterOrExporter;
            }

            return Json(new
            {
                rowsByPage,
                difCantPorPagina = Math.Abs(cantAnte - cantNueva)
        });
        }

        private string GetRowsInHtmlByFilter(string sqlTableAndWhere, string filtro,
            string cifTot, string valueCifTot, string auxCifTot,
            int pagina, CultureInfo cultureInfo, string idFiltro, ref int cantRegistros)
        {
            var dtTable = FuncionesBusiness.GetRegistrosByFiltro(sqlTableAndWhere,
                filtro, cifTot, valueCifTot, pagina, TablePageSize, auxCifTot);

            cantRegistros = dtTable.Rows.Count;
            
            var viewData = new TabDataTabMy()
            {
                Filtro = idFiltro,
                ListRows = GetGridRowsByFilter(dtTable, filtro, auxCifTot, cultureInfo)
            };

            return RenderViewToString(this.ControllerContext, "GridViews/TableRowView", viewData);
        }

        private string GetRowsInHtmlByFilter(string sqlTableAndWhere, string filtro,
            string cifTot, string valueCifTot, string auxCifTot,
            int pagina, CultureInfo cultureInfo, string idFiltro, ref int cantRegistros, bool mostrarInformaColombia)
        {
            var dtTable = FuncionesBusiness.GetRegistrosByFiltro(sqlTableAndWhere,
                filtro, cifTot, valueCifTot, pagina, TablePageSize, auxCifTot);

            cantRegistros = dtTable.Rows.Count;
            var GridHead = new GridHeadTabMy()
            {
                mostrarColInformaCo = mostrarInformaColombia
            };
            var viewData = new TabDataTabMy()
            {
                Filtro = idFiltro,
                ListRows = GetGridRowsByFilter(dtTable, filtro, auxCifTot, cultureInfo),
                GridHead = GridHead
            };

            return RenderViewToString(this.ControllerContext, "GridViews/TableRowView", viewData);
        }

        #endregion

        #region ModalVerRegistros
        [SessionExpireFilter]
        [HttpPost]
        public JsonResult VerRegistros(string idFiltro, string idRegistro, string opcion,
            string fechaIni, string fechaFin)
        {
            Session["IdFiltroVerRegistros"] = idFiltro;
            Session["IdRegistroVerRegistros"] = idRegistro;

            string auxFechaIni = "";
            string auxFechaFin = "";
            GetFechasByOpcion(opcion, fechaIni, fechaFin, ref auxFechaIni, ref auxFechaFin);

            Session["fechaIni"] = auxFechaIni;
            Session["fechaFin"] = auxFechaFin;

            return GetDataModalVerRegistros(idFiltro, idRegistro);
        }

        private VerRegistroTableHead GetVerRegistroTableHead(ConsultaForm objConsultaForm)
        {
            VerRegistroTableHead objVerRegistroTableHead = new VerRegistroTableHead();

            objVerRegistroTableHead.Numero = "No.";
            objVerRegistroTableHead.Fecha = Resources.Resources.Date_Text;
            objVerRegistroTableHead.Nandina = Resources.AdminResources.HTS_Code_Text;
            objVerRegistroTableHead.Importador = Resources.Resources.Search_Form_Item05;

            objVerRegistroTableHead.Exportador = Resources.Resources.Search_Form_Item06;

            objVerRegistroTableHead.CampoPeso = objConsultaForm.CampoPeso == "PesoNeto"
                ? Resources.AdminResources.WeightNet_Text + " kg"
                : Resources.AdminResources.Weight_Gross_Text + " kg";

            objVerRegistroTableHead.IsVisibleCampoPeso = objConsultaForm.CampoPeso != "";

            objVerRegistroTableHead.Cantidad = Resources.Resources.Quantity_Text;
            objVerRegistroTableHead.Unidad = Resources.AdminResources.Unit_Text;

            objVerRegistroTableHead.FobOFasUnit = "FOB Unit. US$";

            objVerRegistroTableHead.CifOFobUnit = objConsultaForm.Cif + " Unit. US$";
            objVerRegistroTableHead.IsVisibleCifOFobUnit = true;

            objVerRegistroTableHead.CifImptoUnit = "CIF Unit. + Imptos. US$";

            objVerRegistroTableHead.DesComercial = Resources.AdminResources.Commercial_Description_Text;
            objVerRegistroTableHead.IsVisibleDesComercial = objConsultaForm.FlagDescComercialB;

            if (objConsultaForm.TipoOpe == Enums.TipoOpe.I.ToString())
            {
                objVerRegistroTableHead.IsVisibleImportador = objConsultaForm.Importador;
                objVerRegistroTableHead.IsVisibleExportador = objConsultaForm.Proveedor;
                objVerRegistroTableHead.IsVisibleFobOFasUnit = (objConsultaForm.Cif != "FOB" && objConsultaForm.CodPais != "CN" && objConsultaForm.CodPais != "IN" && objConsultaForm.CodPais != "MXD" &&
                                                                objConsultaForm.CodPais != "US");
                objVerRegistroTableHead.IsVisibleCifImptoUnit = objConsultaForm.CodPais == "PE";
                objVerRegistroTableHead.PaisOrigenODestino = Resources.AdminResources.OriginCountry_FormField_Label;
                objVerRegistroTableHead.Exportador = objConsultaForm.CodPais == "CL"
                    ? Resources.Resources.Search_Form_BrandField
                    : Resources.Resources.Search_Form_Item06;
            }
            else
            {
                objVerRegistroTableHead.IsVisibleExportador = objConsultaForm.Exportador;
                objVerRegistroTableHead.IsVisibleImportador = objConsultaForm.ImportadorExp;
                objVerRegistroTableHead.IsVisibleFobOFasUnit = false;
                objVerRegistroTableHead.IsVisibleCifImptoUnit = false;
                objVerRegistroTableHead.PaisOrigenODestino = Resources.AdminResources.DestinationCountry_FormField_Label;
                objVerRegistroTableHead.Importador = objConsultaForm.CodPais == "CL" ?
                    Resources.Resources.Search_Form_BrandField
                    : Resources.Resources.Search_Form_Item05;
            }

            return objVerRegistroTableHead;
        }

        private string GetTableAndWhereSqlVerRegistros(string tipoOpe, string codPais2, string auxCodPais,
           string idPartida, string textPartida, string fechaIni,
            string fechaFin, string tabla, bool existeDesComercial,
            string idFiltro, string idRegistro, string desComercial = "")
        {
            string sql = "from " + tabla + " ";
            sql += "where 1 = 1 ";

            string sqlFiltro = "and FechaNum between " + fechaIni + " and " + fechaFin + " ";

            if (codPais2 == "4UE")
            {
                sql += "and " + (tipoOpe == "I" ? "IdPaisImp" : "IdPaisExp") + " = " + auxCodPais + " ";
            }

            if (idPartida != "")
            {
                if (textPartida.Substring(0, 3) != "[G]")
                    sqlFiltro += "and IdPartida = " + idPartida + " ";
                else
                    sqlFiltro += "and IdPartida in (select IdFavorito from FavoritoGrupo where IdGrupo = " + idPartida +
                                 ") ";
            }

            if (tipoOpe == Enums.TipoOpe.I.ToString())
            {
                switch (idFiltro)
                {
                    case "ImporterOrExporter":
                        sqlFiltro += "and IdImportador = " + idRegistro + " ";
                        break;
                    case "OriginOrDestinationCountry":
                        sqlFiltro += "and IdPaisOrigen = " + idRegistro + " ";
                        break;
                    case "SupplierOrImporterExp":
                        sqlFiltro += "and IdProveedor = " + idRegistro + " ";
                        break;
                }
            }
            else
            {
                switch (idFiltro)
                {
                    case "ImporterOrExporter":
                        sqlFiltro += "and IdExportador = " + idRegistro + " ";
                        break;
                    case "OriginOrDestinationCountry":
                        sqlFiltro += "and IdPaisDestino = " + idRegistro + " ";
                        break;
                    case "SupplierOrImporterExp":
                        sqlFiltro += "and IdImportadorExp = " + idRegistro + " ";
                        break;
                }
            }

            if (existeDesComercial)
                sqlFiltro += "and DesComercial like '%" + desComercial + "%' ";

            sql += sqlFiltro;

            Session["MyProductsSqlFiltro"] = sqlFiltro;

            Session["TableAndWhereVerRegistros"] = sql;

            return sql;
        }

        private List<VerRegistroTableRow> GetRowsVerRegistros(DataTable dt, ConsultaForm objConsultaForm, CultureInfo specificCulture,
            int pagina)
        {
            List<VerRegistroTableRow> lista = new List<VerRegistroTableRow>();

            var isVisibleCampoPeso = objConsultaForm.CampoPeso != "";
            var dataFieldFobOFasUnit = objConsultaForm.CodPais != "US" ? "FOBUnit" : "FASUnit";
            var dataFieldCifOFobUnit = objConsultaForm.Cif + "Unit";

            int numero = (pagina - 1) * VerRegistrosPageSize;

            if (objConsultaForm.TipoOpe == Enums.TipoOpe.I.ToString())
            {
                var isVisibleFobOFasUnit = (objConsultaForm.Cif != "FOB"
                                            && objConsultaForm.CodPais != "CA"
                                            && objConsultaForm.CodPais != "CN"
                                            && objConsultaForm.CodPais != "IN"
                                            && objConsultaForm.CodPais != "MXD"
                                            && objConsultaForm.CodPais != "US");


                var isVisibleCifImptoUnit = (objConsultaForm.CodPais == "PE");

                foreach (DataRow dr in dt.Rows)
                {
                    numero++;
                    lista.Add(new VerRegistroTableRow
                    {
                        Numero = numero,
                        Fecha = Convert.ToDateTime(dr["Fechanum_date"]).ToString("dd/MM/yyyy", specificCulture),
                        Nandina = dr["Nandina"].ToString(),
                        Importador = objConsultaForm.Importador ? dr["Importador"].ToString() : "",
                        Exportador = objConsultaForm.Proveedor ? dr["Proveedor"].ToString() : "",
                        CampoPeso = isVisibleCampoPeso ? (dr[objConsultaForm.CampoPeso].ToString() != "" ? Convert.ToDecimal(dr[objConsultaForm.CampoPeso]).ToString("n2", specificCulture) : "") : "",
                        Cantidad = dr["Cantidad"].ToString() != "" ? Convert.ToDecimal(dr["Cantidad"]).ToString("n2", specificCulture) : "",
                        Unidad = dr["Unidad"].ToString(),
                        FobOFasUnit = isVisibleFobOFasUnit ? (dr[dataFieldFobOFasUnit].ToString() != "" ? Convert.ToDecimal(dr[dataFieldFobOFasUnit]).ToString("n2", specificCulture) : "") : "",
                        CifOFobUnit = (dr[dataFieldCifOFobUnit].ToString() != "" ? Convert.ToDecimal(dr[dataFieldCifOFobUnit]).ToString("n2", specificCulture) : ""),
                        CifImptoUnit = isVisibleCifImptoUnit ? (dr["CIFImptoUnit"].ToString() != "" ? Convert.ToDecimal(dr["CIFImptoUnit"]).ToString("n2", specificCulture) : "") : "",
                        PaisOrigenODestino = objConsultaForm.PaisOrigen ? dr["PaisOrigen"].ToString() : "",
                        DesComercial = objConsultaForm.FlagDescComercialB ? dr["DesComercial"].ToString() : "",
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
                        Fecha = Convert.ToDateTime(dr["Fechanum_date"]).ToString("dd/MM/yyyy", specificCulture),
                        Nandina = dr["Nandina"].ToString(),
                        Exportador = objConsultaForm.Exportador ? dr["Exportador"].ToString() : "",
                        Importador = objConsultaForm.ImportadorExp ? dr["ImportadorExp"].ToString() : "",
                        CampoPeso = isVisibleCampoPeso ? (dr[objConsultaForm.CampoPeso].ToString() != "" ? Convert.ToDecimal(dr[objConsultaForm.CampoPeso]).ToString("n2", specificCulture) : "") : "",
                        Cantidad = dr["Cantidad"].ToString() != "" ? Convert.ToDecimal(dr["Cantidad"]).ToString("n2", specificCulture) : "",
                        Unidad = dr["Unidad"].ToString(),
                        CifOFobUnit = (dr[dataFieldCifOFobUnit].ToString() != "" ? Convert.ToDecimal(dr[dataFieldCifOFobUnit]).ToString("n2", specificCulture) : ""),
                        PaisOrigenODestino = objConsultaForm.ExistePaisDestino ? dr["PaisDestino"].ToString() : "",
                        DesComercial = objConsultaForm.FlagDescComercialB ? dr["DesComercial"].ToString() : ""
                    });
                }
            }


            return lista;
        }
        [SessionExpireFilter]
        [HttpPost]
        public JsonResult VerRegistrosPageIndexChanging(int pagina)
        {
            string tipoOpe = Session["TipoOpe"].ToString();
            string codPais2 = Session["CodPais2"].ToString();
            string codPais = Session["CodPais"].ToString();

            string tableAndWhereSqlVerRegistros = Session["TableAndWhereVerRegistros"].ToString();

            ValidaCodPais2(codPais2, ref codPais);

            bool existeDesComercial = Funciones.FlagDesComercial(codPais, tipoOpe);
            ConsultaForm objConsultaForm = GetConsultaForm(tipoOpe, codPais);
            objConsultaForm.FlagDescComercialB = existeDesComercial;
            objConsultaForm.CampoPeso = Funciones.CampoPeso(codPais, tipoOpe);
            objConsultaForm.Cif = Funciones.Incoterm(codPais, tipoOpe);

            DataTable dtRegistros = FuncionesBusiness.GetRegistrosByIdRegistro(tableAndWhereSqlVerRegistros,
                tipoOpe, codPais2, pagina, VerRegistrosPageSize);

            VerRegistroTable objVerRegistroTable = new VerRegistroTable
            {
                TipoOpe = tipoOpe,
                VerRegistroTableHead = GetVerRegistroTableHead(objConsultaForm),
                VerRegistroTableRows = GetRowsVerRegistros(dtRegistros, objConsultaForm, GetSpecificCulture(), pagina)
            };

            return Json(new
            {
                rowsByPage =
                    RenderViewToString(this.ControllerContext, "GridViews/VerRegistroRows", objVerRegistroTable)
            });
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult BuscarPorDesComercial(string textDesComercial, bool enabledFiltros)
        {
            string tipoOpe = Session["TipoOpe"].ToString();

            string idPartida = Session["idPartida"].ToString();
            string auxTextPartida = Session["textPartida"].ToString();
            string auxFechaIni = Session["fechaIni"].ToString();
            string auxFechaFin = Session["fechaFin"].ToString();

            string codPais2 = Session["CodPais2"].ToString();
            string codPais = Session["CodPais"].ToString();

            string idFiltro;
            string idRegistro;
            if (enabledFiltros)
            {
                idFiltro = Session["IdFiltroVerRegistros"].ToString();
                idRegistro = Session["IdRegistroVerRegistros"].ToString();
            }
            else
            {
                idFiltro = "";
                idRegistro = "";
            }


            string auxCodPais = codPais;
            ValidaCodPais2(codPais2, ref codPais);

            bool existeDesComercial = Funciones.FlagDesComercial(codPais, tipoOpe);
            string tabla = GetTabla(tipoOpe, codPais);

            string tableAndWhereSqlVerRegistros = GetTableAndWhereSqlVerRegistros(tipoOpe, codPais2, auxCodPais,
                idPartida, auxTextPartida, auxFechaIni, auxFechaFin, tabla, existeDesComercial,
                idFiltro, idRegistro, textDesComercial.Trim());

            int cantRegistros =
                FuncionesBusiness.GetCantidadRegistrosByIdRegistro(tableAndWhereSqlVerRegistros, tipoOpe);

            object objMensaje = null;
            int totalPages = 0;
            var resultadoDesComercialVerRegistro = "";
            string tablaVerRegistro = "";
            if (cantRegistros > 0)
            {

                ConsultaForm objConsultaForm = GetConsultaForm(tipoOpe, codPais);
                objConsultaForm.FlagDescComercialB = existeDesComercial;
                objConsultaForm.CampoPeso = Funciones.CampoPeso(codPais, tipoOpe);
                objConsultaForm.Cif = Funciones.Incoterm(codPais, tipoOpe);

                int pagina = 1;

                DataTable dtRegistros = FuncionesBusiness.GetRegistrosByIdRegistro(tableAndWhereSqlVerRegistros,
                    tipoOpe, codPais2, pagina, VerRegistrosPageSize);

                totalPages = (int)Math.Ceiling(Convert.ToDecimal(cantRegistros) / VerRegistrosPageSize);

                VerRegistroTable objVerRegistroTable = new VerRegistroTable
                {
                    TipoOpe = tipoOpe,
                    VerRegistroTableHead = GetVerRegistroTableHead(objConsultaForm),
                    VerRegistroTableRows = GetRowsVerRegistros(dtRegistros, objConsultaForm, GetSpecificCulture(), pagina)
                };

                tablaVerRegistro = RenderViewToString(this.ControllerContext, "GridViews/VerRegistroRows", objVerRegistroTable);

                Session["MPCantRegVerRegistros"] = cantRegistros; // Se usa en exportar excel del Modal Ver Registros

                if (textDesComercial.Trim() != "")
                {
                    resultadoDesComercialVerRegistro = cantRegistros + " " + Resources.AdminResources.Records_Text;
                }
            }
            else
            {
                objMensaje = new
                {
                    titulo = Resources.AdminResources.Filter_MyProducts_Button,
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
        #endregion

        private JsonResult GetDataModalVerRegistros(string idFiltro, string idRegistro)
        {
            string tipoOpe = Session["TipoOpe"].ToString();

            string idPartida = Session["idPartida"].ToString();
            string auxTextPartida = Session["textPartida"].ToString();
            string auxFechaIni = Session["fechaIni"].ToString();
            string auxFechaFin = Session["fechaFin"].ToString();

            string codPais2 = Session["CodPais2"].ToString();
            string codPais = Session["CodPais"].ToString();


            string auxCodPais = codPais;
            ValidaCodPais2(codPais2, ref codPais);

            string idioma = GetCurrentIdioma();

            string tituloRegistros = "";
            if (idPartida != "")
            {
                if (auxTextPartida.Substring(0, 3) != "[G]")
                {
                    tituloRegistros += FuncionesBusiness.BuscaPartida(idPartida, codPais, idioma) + " - ";
                }
                else
                {
                    tituloRegistros += auxTextPartida + " - ";
                }
            }

            switch (idFiltro)
            {
                case "ImporterOrExporter":
                    tituloRegistros += Funciones.BuscaEmpresa(idRegistro, codPais) + " - ";
                    break;
                case "OriginOrDestinationCountry":
                    tituloRegistros += Funciones.BuscaPais(idRegistro, codPais) + " - ";
                    break;
                case "SupplierOrImporterExp":
                    tituloRegistros += Funciones.BuscaProveedor(idRegistro, codPais) + " - ";
                    break;
            }

            tituloRegistros += FuncionesBusiness.RangoFechas(auxFechaIni, auxFechaFin, idioma);
            Session["MPTituloVerRegistros"] = tituloRegistros;

            bool existeDesComercial = Funciones.FlagDesComercial(codPais, tipoOpe);
            string tabla = GetTabla(tipoOpe, codPais);

            string tableAndWhereSqlVerRegistros = GetTableAndWhereSqlVerRegistros(tipoOpe, codPais2, auxCodPais,
                idPartida, auxTextPartida, auxFechaIni, auxFechaFin, tabla, existeDesComercial,
                idFiltro, idRegistro);

            int cantRegistros =
                FuncionesBusiness.GetCantidadRegistrosByIdRegistro(tableAndWhereSqlVerRegistros, tipoOpe);

            object objMensaje = null;
            string tablaVerRegistros = "";
            int totalPages = 0;

            IEnumerable<SelectListItem> opcionesDeDescarga = null;
            if (cantRegistros > 0)
            {
                ConsultaForm objConsultaForm = GetConsultaForm(tipoOpe, codPais);
                objConsultaForm.FlagDescComercialB = existeDesComercial;
                objConsultaForm.CampoPeso = Funciones.CampoPeso(codPais, tipoOpe);
                objConsultaForm.Cif = Funciones.Incoterm(codPais, tipoOpe);

                int pagina = 1;

                DataTable dtRegistros = FuncionesBusiness.GetRegistrosByIdRegistro(tableAndWhereSqlVerRegistros,
                    tipoOpe, codPais2, pagina, VerRegistrosPageSize);

                VerRegistroTable objVerRegistroTable = new VerRegistroTable
                {
                    TotalPaginas = (int)Math.Ceiling(Convert.ToDecimal(cantRegistros) / VerRegistrosPageSize),
                    TipoOpe = tipoOpe,
                    VerRegistroTableHead = GetVerRegistroTableHead(objConsultaForm),
                    VerRegistroTableRows = GetRowsVerRegistros(dtRegistros, objConsultaForm, GetSpecificCulture(), pagina)
                };

                tablaVerRegistros = RenderViewToString(this.ControllerContext, "GridViews/VerRegistroGridView",
                    objVerRegistroTable);
                totalPages = objVerRegistroTable.TotalPaginas;

                DataTable dtOpcionesDeDescarga =
                    FuncionesBusiness.GetOpcionesDeDescarga(Session["IdUsuario"].ToString(), tipoOpe, codPais2, codPais,
                        idioma, auxCodPais);

                opcionesDeDescarga = new List<SelectListItem>();

                if (dtOpcionesDeDescarga != null && dtOpcionesDeDescarga.Rows.Count > 0)
                {
                    opcionesDeDescarga = dtOpcionesDeDescarga.AsEnumerable().Select(m => new SelectListItem()
                    {
                        Text = m.Field<string>("Descarga"),
                        Value = (m.Field<Int32>("IdDescargaCab")).ToString()
                    });
                }

                Session["MPCantRegVerRegistros"] = cantRegistros; // Se usa en exportar excel del Modal Ver Registros

            }
            else
            {
                objMensaje = new
                {
                    titulo = Resources.AdminResources.Filter_MyProducts_Button,
                    mensaje = Resources.Resources.NoResultsWereFound_Text,
                    flagContactenos = false
                };
            }

            return Json(new
            {
                objMensaje,
                tituloRegistros,
                opcionesDeDescarga,
                tablaVerRegistros,
                totalPages
            });
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult VerRegistrosByChartSeriesPoint(string opcion, string category)
        {
            string fechaIni;
            string fechaFin;
            if (opcion != "AÑOS")
            {
                string idAnioMes = FuncionesBusiness.BuscaIdAnioMes(category, GetCurrentIdioma());
                fechaIni = idAnioMes + "01";
                fechaFin = idAnioMes + "99";
            }
            else
            {
                string idAnio = category;
                fechaIni = idAnio + "0101";
                fechaFin = idAnio + "1299";
            }

            Session["fechaIni"] = fechaIni;
            Session["fechaFin"] = fechaFin;

            return GetDataModalVerRegistros("", "");
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult VerRegistrosByCategoryAndSerie(string opcion, string category, string serie)
        {
            string fechaIni;
            string fechaFin;

            if (opcion != "AÑOS")
            {
                string idAnio = serie;
                string idMes = FuncionesBusiness.BuscaCodMes(category, GetCurrentIdioma());
                fechaIni = idAnio + idMes + "01";
                fechaFin = idAnio + idMes + "99";
            }
            else
            {
                string idAnio = category;
                fechaIni = idAnio + "0101";
                fechaFin = idAnio + "1299";
            }

            Session["fechaIni"] = fechaIni;
            Session["fechaFin"] = fechaFin;

            return GetDataModalVerRegistros("", "");
        }

        private bool ValidarConsulta(string sql)
        {

            return sql.Contains("'%'");

        }
        #region ExportarArchivosExcel

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult GetAllExcelFile(string codDescarga)
        {
            string idPartida = Session["idPartida"].ToString();
            int cantReg = Convert.ToInt32(Session["auxCantReg"]);
            string fechaIni = Session["auxFechaIni"].ToString();
            string fechaFin = Session["auxFechaFin"].ToString();
            string textPartida = Session["textPartida"].ToString();

            string codPais2 = Session["CodPais2"].ToString();
            string codPais = Session["CodPais"].ToString();
            string auxCodPais = codPais;
            ValidaCodPais2(codPais2, ref codPais);
            string tipoOpe = Session["TipoOpe"].ToString();
            string idioma = GetCurrentIdioma();
            string tituloRegistros = "";

            string sqlFiltro = "and FechaNum between " + fechaIni + " and " + fechaFin + " ";

            if (codPais2 == "4UE")
                sqlFiltro += "and " + (tipoOpe == "I" ? "IdPaisImp" : "IdPaisExp") + " = " + auxCodPais + " ";

            if (textPartida.Substring(0, 3) != "[G]")
            {
                sqlFiltro += "and IdPartida = " + idPartida + " ";
                tituloRegistros = FuncionesBusiness.BuscaPartida(idPartida, codPais, idioma) + " - "; 
            }
            else
            {
                sqlFiltro += "and IdPartida in (select IdFavorito from FavoritoGrupo where IdGrupo = " + idPartida + ") ";
                tituloRegistros = textPartida + " - ";
            }
            Session["MyProductsSqlFiltro"] = sqlFiltro;

            object objMensaje = null;

            if (sqlFiltro.Length < 55 || ValidarConsulta(sqlFiltro))
            {
                /*bool tipoServidor = Properties.Settings.Default.TableVarGeneral_InDev;

                string path = ConfigurationManager.AppSettings["directorio_logs"];
                Logs oLog = new Logs(path);

                try
                {

                    oLog.Add("IdUsuario => " + Session["idUsuario"].ToString(), Session["idUsuario"].ToString());
                    oLog.Add("Servidor => " + (tipoServidor ? "Desarrollo" : "Produccion"), Session["idUsuario"].ToString());
                    oLog.Add("Controlador => MisProductos", Session["idUsuario"].ToString());
                    oLog.Add("Metodo Generado => GetAllExcelFile()", Session["idUsuario"].ToString());
                    oLog.Add("Consulta Session => " + sqlFiltro, Session["idUsuario"].ToString());

                }catch(Exception ex)
                {
                    oLog.Add("Excepcion => " + ex.ToString(), Session["idUsuario"].ToString());
                }*/

                
                

                string mensaje = "Esta descarga no puede ser realizada porque no se ha seleccionado ningun filtro";

                objMensaje = new
                {
                    titulo = Resources.AdminResources.DownloadToExcel_Text,
                    mensaje,
                    flagContactenos = true
                };

                return Json(new
                {
                    objMensaje 
                });
            }

            tituloRegistros += FuncionesBusiness.RangoFechas(fechaIni, fechaFin, idioma);
            object objMensajeFlag = null;
            if (FuncionesBusiness.ForzarLinkExcel(Session["IdUsuario"].ToString()))
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
                infoArchivo = GeneraExcelRegistros(codDescarga, cantReg, tituloRegistros),
                objMensajeFlag,
                objMensaje
            });
        }

        private object GeneraExcelRegistros(string codDescarga, int cantReg, string tituloRegistros)
        {
            string idUsuario = Session["IdUsuario"].ToString();
            string codUsuario = FuncionesBusiness.BuscaCodUsuario(idUsuario);
            bool flagExcel = !FuncionesBusiness.ForzarZip(codUsuario);

            string tipoOpe = Session["TipoOpe"].ToString();
            string codPais = Session["CodPais"].ToString();
            string codPais2 = Session["CodPais2"].ToString();

            ValidaCodPais2(codPais2, ref codPais);

            string idioma = GetCurrentIdioma();

            var titulos = FuncionesBusiness.ListaTitulosDescarga(codDescarga, "", codPais, tipoOpe, idioma);
            var campos = FuncionesBusiness.ListaCamposDescarga(codDescarga, "", codPais, tipoOpe, idioma);

            var sqlFiltro = Session["MyProductsSqlFiltro"].ToString();

            

            string ahora = DateTime.Now.ToString("yyyyMMddHHmmss");

            string nombreArchivo = "Veritrade_" + codUsuario + "_" + codPais + "_" + tipoOpe + "_" + ahora;


            bool isFreeTrial = (Session["TipoUsuario"].ToString() == "Free Trial") || (bool)Session["opcionFreeTrial"];

            FuncionesBusiness.GeneraArchivoBusqueda(Convert.ToInt32(idUsuario), isFreeTrial, codPais,
                tipoOpe, titulos, campos, sqlFiltro.Replace("'", "''").Replace("\"", "\"\""), cantReg, nombreArchivo, ((cantReg <= Variables.CantRegMaxExcel && flagExcel) ? 'S' : 'N'));


            Funciones.GrabaLog(idUsuario, codPais, tipoOpe, "0", "0", "MisProductosController", "");

            string extensionArchivo = ".xlsx";
            if (cantReg <= Variables.CantRegMaxExcel && flagExcel)
            {
                nombreArchivo += ".xlsx";

                using (OfficeOpenXml.ExcelPackage package = new OfficeOpenXml.ExcelPackage(
                    new System.IO.FileInfo(ConfigurationManager.AppSettings["directorio_descarga"] + nombreArchivo)))
                {
                    var sheet = package.Workbook.Worksheets["Veritrade"];
                    sheet.Cells["B1"].Value = FuncionesBusiness.Pais(codPais, idioma).ToUpper() + " - " +
                                              (tipoOpe == "I" ? Resources.AdminResources.Imports_Text.ToUpper() : Resources.AdminResources.Exports_Text.ToUpper());

                    sheet.Cells["B2"].Value = tituloRegistros;

                    sheet.Cells["B3"].Value = Resources.AdminResources.RecordsFirsLetterUpperCase_Text + ": " +
                                              cantReg.ToString("n0", GetSpecificCulture());

                    if (isFreeTrial )//&& cantReg > Variables.CantRegMaxFreeTrial)
                        sheet.Cells["B5"].Value = (idioma == "es") ? "Muestra: 20 registros" : "Sample: 20 records";

                    package.Save();
                }
            }
            else
            {
                nombreArchivo += ".zip";
                extensionArchivo = ".zip";
            }

            return new
            {
                fileName = nombreArchivo,
                extensionArchivo
            };
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult GetVerRegistrosExcelFile(string codDescarga)
        {

            object objMensajeFlag = null;
            if (FuncionesBusiness.ForzarLinkExcel(Session["IdUsuario"].ToString()))
            {
                objMensajeFlag = new
                {
                    titulo = Resources.AdminResources.DownloadToExcel_Text,
                    mensaje = Resources.Resources.DownloadToExcel_Text,
                    flagContactenos = false
                };

            }

            string sqlFiltro = Session["MyProductsSqlFiltro"].ToString();

            object objMensaje = null;

            if (sqlFiltro.Length < 55 || ValidarConsulta(sqlFiltro))
            {
                /*bool tipoServidor = Properties.Settings.Default.TableVarGeneral_InDev;                

                string path = ConfigurationManager.AppSettings["directorio_logs"];
                Logs oLog = new Logs(path);

                try
                {

                    oLog.Add("IdUsuario => " + Session["idUsuario"].ToString(), Session["idUsuario"].ToString());
                    oLog.Add("Servidor => " + (tipoServidor ? "Desarrollo" : "Produccion"), Session["idUsuario"].ToString());
                    oLog.Add("Controlador => MisProductos", Session["idUsuario"].ToString());
                    oLog.Add("Metodo Generado => GetVerRegistrosExcelFile()", Session["idUsuario"].ToString());
                    oLog.Add("Consulta Session => " + sqlFiltro, Session["idUsuario"].ToString());

                }
                catch(Exception ex)
                {
                    oLog.Add("Excepcion => " + ex.ToString(), Session["idUsuario"].ToString());
                }*/

                
                

                string mensaje = "Esta descarga no puede ser realizada porque no se ha seleccionado ningun filtro";

                objMensaje = new
                {
                    titulo = Resources.AdminResources.DownloadToExcel_Text,
                    mensaje,
                    flagContactenos = true
                };

                return Json(new
                {
                    objMensaje
                });
            }

            return Json(new
            {
                infoArchivo = GeneraExcelRegistros(codDescarga, Convert.ToInt32(Session["MPCantRegVerRegistros"]), Session["MPTituloVerRegistros"].ToString()),
                objMensajeFlag,
                objMensaje
            });
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

        [HttpGet]
        public ActionResult DownloadExcelFile(string fileName)
        {
            string fullPath = ConfigurationManager.AppSettings["directorio_descarga"] + fileName;
            return File(fullPath, "application/vnd.ms-excel", fileName);
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult GetExportTableToExcel(string opcion)
        {
            return GeneraExcelResumen(opcion);
        }

        private JsonResult GeneraExcelResumen(string opcion)
        {
            string codUsuario = FuncionesBusiness.BuscaCodUsuario(Session["IdUsuario"].ToString());
            string idPartida = Session["idPartida"].ToString();
            string tipoOpe = Session["TipoOpe"].ToString();
            string codPais = Session["CodPais"].ToString();
            string codPais2 = Session["CodPais2"].ToString();
            string textPartida = Session["textPartida"].ToString();
            string fechaIni = Session["auxFechaIni"].ToString();
            string fechaFin = Session["auxFechaFin"].ToString();

            string cifTot = Session["cifTot"].ToString();
            string valueCifTot = Session["valueCifTot"].ToString();

            string auxCodPais = codPais;
            ValidaCodPais2(codPais2, ref codPais);

            string idioma = GetCurrentIdioma();
            string nombreArchivo;
            string templateName = HostingEnvironment.MapPath("~/ExcelTemplate/VeritradeResumenTemplate2.xlsx");

            using (OfficeOpenXml.ExcelPackage package =
                new OfficeOpenXml.ExcelPackage(new System.IO.FileInfo(templateName)))
            {
                if (opcion != "Partidas")
                    package.Workbook.Worksheets["Partidas"].Hidden = OfficeOpenXml.eWorkSheetHidden.VeryHidden;
                if (opcion != "Importadores")
                    package.Workbook.Worksheets["Importadores1"].Hidden = OfficeOpenXml.eWorkSheetHidden.VeryHidden;
                if (opcion != "Proveedores")
                    package.Workbook.Worksheets["Exportadores1"].Hidden = OfficeOpenXml.eWorkSheetHidden.VeryHidden;
                if (opcion != "PaisesOrigen")
                    package.Workbook.Worksheets["Países Origen"].Hidden = OfficeOpenXml.eWorkSheetHidden.VeryHidden;

                int cantReg = Convert.ToInt32(Session["auxCantReg"]);

                var EsFreeTrial = (Session["TipoUsuario"].ToString() == "Free Trial") || (bool)Session["opcionFreeTrial"];
                

                string paisTipoOpe = FuncionesBusiness.Pais(codPais, idioma).ToUpper() + " - " +
                                     (tipoOpe == "I" ? Resources.AdminResources.Imports_Text.ToUpper() : Resources.AdminResources.Exports_Text.ToUpper());
                string filtros = "[" + Resources.AdminResources.Product_Text + "] " + textPartida;
                string rango = Resources.AdminResources.Period_Text + ": " + FuncionesBusiness.RangoFechas(fechaIni, fechaFin, idioma);

                string registros = Resources.AdminResources.RecordsFirsLetterUpperCase_Text + ": " +
                                   cantReg.ToString("n0", GetSpecificCulture());

                Dictionary<string, string> filtersByTipoOpe = GetFiltersByTypeOpe(tipoOpe);

                string tabla = GetTabla(tipoOpe, codPais);

                DataTable dt;
                OfficeOpenXml.ExcelWorksheet ws;
                switch (opcion)
                {
                    case "Importadores":
                        string tableAndWhereSqlImporters = GetTableAndWhereSqlImporters(idPartida, "", fechaIni, fechaFin,
                            filtersByTipoOpe["importerOrExporter"], tabla, tipoOpe, codPais2, auxCodPais, textPartida);

                        dt = FuncionesBusiness.GetRegistrosByFiltro(tableAndWhereSqlImporters,
                            filtersByTipoOpe["importerOrExporter"], cifTot, valueCifTot, -1, -1, cifTot, false,esFree: EsFreeTrial);

                        dt.Columns.Remove(tipoOpe == "I" ? "IdImportador" : "IdExportador");

                        ws = package.Workbook.Worksheets["Importadores1"];
                        ws.Name = (tipoOpe == "I"
                            ? Resources.Resources.Demo_Importers_Tab
                            : Resources.Resources.Demo_Exporters_Tab);

                        if (EsFreeTrial)
                        {
                            ws.Cells["B5"].Value = Resources.Resources.Trial_Excel_Text;
                            ws.Cells["B5"].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                        }
                        ws.Cells["B1"].Value = paisTipoOpe;
                        ws.Cells["B2"].Value = filtros;
                        ws.Cells["B3"].Value = rango;
                        ws.Cells["B4"].Value = registros;
                        ws.Cells["A6"].Value = (tipoOpe == "I" ? Resources.Resources.Search_Form_Item05 : Resources.Resources.Search_Form_Item06);
                        ws.Cells["B6"].Value = Session["textCiFoFobTot"].ToString();
                        ws.Cells["A7"].LoadFromDataTable(dt, false);
                        CalculaFormulasExcel2(ws, dt);
                        break;
                    case "Proveedores":
                        string tableAndWhereSqlsupplierOrIporterExp = GetTableAndWhereSqlProveedoresAndCountries(idPartida, "", "", fechaIni,
                            fechaFin, tipoOpe, filtersByTipoOpe["supplierOrIporterExp"], codPais2, auxCodPais, tabla, textPartida, filtersByTipoOpe["importerOrExporter"]);

                        dt = FuncionesBusiness.GetRegistrosByFiltro(tableAndWhereSqlsupplierOrIporterExp,
                            filtersByTipoOpe["supplierOrIporterExp"], cifTot, valueCifTot, -1, -1, cifTot, false, esFree: EsFreeTrial);

                        dt.Columns.Remove(tipoOpe == "I" ? "IdProveedor" : "IdImportadorExp");

                        ws = package.Workbook.Worksheets["Exportadores1"];

                        if (EsFreeTrial)
                        {
                            ws.Cells["B5"].Value = Resources.Resources.Trial_Excel_Text;
                            ws.Cells["B5"].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                        }

                        ws.Cells["B1"].Value = paisTipoOpe;
                        ws.Cells["B2"].Value = filtros;
                        ws.Cells["B3"].Value = rango;
                        ws.Cells["B4"].Value = registros;
                        if (codPais == "CL")
                        {
                            ws.Name = (tipoOpe == "I" ? Resources.Resources.Demo_Brands_Tab : Resources.Resources.Demo_Importers_Tab);
                            ws.Cells["A6"].Value = (tipoOpe == "I" ? Resources.Resources.Search_Form_BrandField : Resources.Resources.Search_Form_Item05);
                        }
                        else
                        {
                            ws.Name = (tipoOpe == "I" ? Resources.Resources.Demo_Exporters_Tab : Resources.Resources.Demo_Importers_Tab);
                            ws.Cells["A6"].Value = (tipoOpe == "I" ? Resources.Resources.Search_Form_Item06 : Resources.Resources.Search_Form_Item05);
                        }
                        ws.Cells["B6"].Value = Session["textCiFoFobTot"].ToString();
                        ws.Cells["A7"].LoadFromDataTable(dt, false);
                        CalculaFormulasExcel2(ws, dt);
                        break;
                    case "PaisesOrigen":
                        string tableAndWhereSqlOriginCountry = GetTableAndWhereSqlProveedoresAndCountries(idPartida, "", "", fechaIni,
                            fechaFin, tipoOpe, filtersByTipoOpe["originOrDestinationCountry"], codPais2, auxCodPais, tabla, textPartida, filtersByTipoOpe["importerOrExporter"]);

                        string auxCifTot = cifTot;
                        if (codPais == "BR" || codPais == "IN")
                        {
                            auxCifTot = "convert(decimal(19,2), " + cifTot + ")";
                        }

                        dt = FuncionesBusiness.GetRegistrosByFiltro(tableAndWhereSqlOriginCountry,
                            filtersByTipoOpe["originOrDestinationCountry"], cifTot, valueCifTot, -1, -1, auxCifTot, false, esFree: EsFreeTrial);

                        dt.Columns.Remove(tipoOpe == "I" ? "IdPaisOrigen" : "IdPaisDestino");

                        ws = package.Workbook.Worksheets["Países Origen"];
                        ws.Name = (tipoOpe == "I" ? Resources.Resources.Demo_OriginCountries_Tab : Resources.Resources.Demo_DestinationCountries_Tab);

                        if (EsFreeTrial)
                        {
                            ws.Cells["B5"].Value = Resources.Resources.Trial_Excel_Text;
                            ws.Cells["B5"].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                        }

                        ws.Cells["B1"].Value = paisTipoOpe;
                        ws.Cells["B2"].Value = filtros;
                        ws.Cells["B3"].Value = rango;
                        ws.Cells["B4"].Value = registros;
                        ws.Cells["A6"].Value = (tipoOpe == "I" ? Resources.AdminResources.OriginCountry_FormField_Label : Resources.AdminResources.DestinationCountry_FormField_Label);
                        ws.Cells["B6"].Value = Session["textCiFoFobTot"].ToString();
                        ws.Cells["A7"].LoadFromDataTable(dt, false);
                        CalculaFormulasExcel2(ws, dt);
                        break;
                }

                nombreArchivo = "Veritrade_" + Resources.Resources.Demo_Summay_Tab + "_" + codUsuario + "_" + codPais + "_" + tipoOpe + "_" +
                                DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";

                package.SaveAs(
                    new System.IO.FileInfo(ConfigurationManager.AppSettings["directorio_descarga"] + nombreArchivo));

            }
            object objMensajeFlag = null;
            if (FuncionesBusiness.ForzarLinkExcel(Session["IdUsuario"].ToString()))
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
                fileName= nombreArchivo,
                objMensajeFlag

            });
        }

        private void CalculaFormulasExcel2(OfficeOpenXml.ExcelWorksheet ws, DataTable dt)
        {
            int i = 7;
            int t = i + dt.Rows.Count;

            ws.Cells["A" + t].Value = "Total";
            ws.Cells["B" + t].Formula = "=SUM(B7:B" + (t - 1) + ")";
            ws.Cells["C" + t].Formula = "=SUM(C7:C" + (t - 1) + ")";

            foreach (DataRow dr in dt.Rows)
            {
                ws.Cells["C" + i].FormulaR1C1 = "=R[0]C[-1] / R" + t + "C[-1] * 100";
                i += 1;
            }
            ws.Cells["A" + t + ":C" + t].Style.Font.Color
                .SetColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));
            ws.Cells["A" + t + ":C" + t].Style.Font.Bold = true;
            ws.Cells["A" + t + ":C" + t].Style.Fill.PatternType =
                OfficeOpenXml.Style.ExcelFillStyle.Solid;
            ws.Cells["A" + t + ":C" + t].Style.Fill.BackgroundColor
                .SetColor(System.Drawing.Color.FromArgb(48, 167, 229));
        }

        #endregion

        [SessionExpireFilter]
        [HttpPost]
        public ActionResult SetCulture(string culture)
        {
            culture = CultureHelper.GetImplementedCulture(culture);
            RouteData.Values["culture"] = culture; // set culture 

            return RedirectToAction("Index");
        }
    }
}

