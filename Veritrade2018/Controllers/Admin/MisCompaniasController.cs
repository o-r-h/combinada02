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
    public class MisCompaniasController : BaseController
    {
        private int LimitTitleAddChartTitle = 50;
        private int TablePageSize = 10;
        private int VerRegistrosPageSize = 10;

        [AuthorizedAlerts]
        [AuthorizedNoReferido]
        [AuthorizedPlan]
        // GET: MisCompanias
        public ActionResult Index(string culture , string ruta)
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
                Session["CodPais"] = queryValues["p"];
                Session["TipoOpe"] = queryValues["to"];
            }

            bool ocultarVideo = true;
            string idUsuario = Session["IdUsuario"].ToString();
            ocultarVideo = FuncionesBusiness.OcultarVideo(idUsuario);

            string codPais2 = (Session["CodPais2"]??"").ToString();
            string codPais = Session["CodPais"].ToString();
            string tipoOpe = Session["TipoOpe"] != null ? Session["TipoOpe"].ToString() : "I";

            bool enbaledOpcion = true;

            string tipoUsuarioT = Session["TipoUsuario"].ToString();

            if (!ocultarVideo)
            {
                Extensiones.SetCookie("OcultarVideo" + "_id" + idUsuario,
                    (Extensiones.ConvertToInt(Extensiones.GetCookie("OcultarVideo" + "_id" + idUsuario)) + 1).ToString(), TimeSpan.FromDays(1));
                ocultarVideo = Extensiones.ConvertToInt(Extensiones.GetCookie("OcultarVideo" + "_id" + idUsuario)) > 3;
            }

            if (tipoUsuarioT == "Free Trial")
            {
                enbaledOpcion = false;
                if (Session["ShowVideoCompanias"] == null)
                {
                    ocultarVideo = false;
                    Session["ShowVideoCompanias"] = "show";
                }
                else
                {
                    ocultarVideo = true;
                }
            }

            MySearchForm objMySearchForm = new MySearchForm();
            
            List<SelectListItem> countries2 = objMySearchForm.GetCountries2MyCompanies(culture).ToList();
            objMySearchForm.ListItemsPais2 = countries2;
            objMySearchForm.CodPais2Selected = countries2.First().Value;
            List<SelectListItem> countriesMyCompanies = objMySearchForm.GetCountriesMyCompanies(culture).ToList();
            
            string filtro = tipoOpe == "I" ? "Importador" : "Exportador";
            objMySearchForm.ListItemsPais = countriesMyCompanies;
            objMySearchForm.CodPaisSelected = countriesMyCompanies.FirstOrDefault(x => x.Value == codPais) == null ? countriesMyCompanies.First().Value : codPais;

            
            objMySearchForm.IsCheckedImportacion = tipoOpe == Enums.TipoOpe.I.ToString();
            objMySearchForm.IscheckedExportacion = tipoOpe == Enums.TipoOpe.E.ToString();
            objMySearchForm.ListItemsOpcion = objMySearchForm.GetPeriod(culture, Session["TipoUsuario"].ToString());
            objMySearchForm.FilterDescription = Resources.AdminResources.Filter_MyCompanies_Text;
            
            bool enabledBtnVerGraficos = false;
            objMySearchForm.ListItemsMyFilters =
                GetMyCompanies(idUsuario, tipoOpe, objMySearchForm.CodPaisSelected,filtro, culture, ref enabledBtnVerGraficos);
            objMySearchForm.EnabledBtnVerGraficos = enabledBtnVerGraficos;
            objMySearchForm.FiltroPeriodo = GetPeriods(tipoOpe, objMySearchForm.CodPaisSelected);

            string seleccionado = "";
            if (opcion)
            {
                DateTime fechaDefault = Convert.ToDateTime(queryValues["fecha"]);
                //objMySearchForm.FiltroPeriodo.DefaultFechaInfoIni = Convert.ToDateTime(fechaDefault.Year + "-" + "01-01");
                objMySearchForm.FiltroPeriodo.DefaultFechaInfoIni = Convert.ToDateTime(fechaDefault.Year + "-" + fechaDefault.Month + "-01");
                objMySearchForm.FiltroPeriodo.DefaultFechaInfoFin = Convert.ToDateTime(fechaDefault.Year + "-" + fechaDefault.Month + "-01");
                seleccionado = queryValues["idFav"]; //FuncionesBusiness.BuscaNandina(queryValues["idFav"], queryValues["p"]);
            }

            objMySearchForm.TipoOperacion = tipoOpe;


            string pesoNeto = Funciones.CampoPeso(codPais, tipoOpe);

            if (pesoNeto != "")
            {
                objMySearchForm.IsVisibleRdbUsd = true;
                objMySearchForm.IsVisibleRdbUnid = true;
            }
            else
            {
                objMySearchForm.IsVisibleRdbUsd = true;
                objMySearchForm.IsVisibleRdbUnid = false;
            }

            ViewData["Seleccionado"] = seleccionado;

            Session["Idioma"] = culture;
            Session["CodPais2"] = codPais2;
            Session["CodPais"] = codPais;
            Session["TipoOpe"] = tipoOpe;

            ViewData["OcultarVideo"] = ocultarVideo;
            ViewData["objMySearchForm"] = objMySearchForm;
            ViewData["UrlVideo"] = Resources.AdminResources.UrlVideo_MyCompanies;

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

            else if ((!(bool)(Session["opcionFreeTrial"] ?? false) && Session["Plan"].ToString() == "ESENCIAL") || Session["Plan"].ToString() == "PERU UNO" ||
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
        private IEnumerable<SelectListItem> GetMyCompanies(string idUsuario, string tipoOpe,string codPais,
            string filtro, string idioma, ref bool enabledVerGraficos)
        {
            DataTable dt = FuncionesBusiness.BuscaFavoritos(idUsuario, codPais, tipoOpe, filtro, "", true, idioma);
            var lista = new List<SelectListItem>();
            if (dt != null && dt.Rows.Count > 1)
            {
                foreach (DataRow dataRow in dt.Rows)
                {
                    lista.Add(new SelectListItem { Text = dataRow["Favorito"].ToString(), Value = dataRow["IdFavorito"].ToString() });
                }

                enabledVerGraficos = true;
            }
            else
            {
                lista.Add(new SelectListItem { Text = Resources.AdminResources.DoNotFavoriteCompanies_Text, Value = "0" });
                enabledVerGraficos = false;
            }

            return lista;
        }
        private void ValidaCodPais2(string codPais2, ref string codPais)
        {
            if (codPais2 == "4UE")
                codPais = "UE";
        }

        private FiltroPeriodo GetPeriods(string tipoOpe, string codPais)
        {
            string anioIni = "", mesIni = "", anioFin = "", mesFin = "";
            Funciones.Rango(codPais, tipoOpe, ref anioIni, ref mesIni, ref anioFin, ref mesFin);

            DateTime auxFechaInfoIni = Convert.ToDateTime(anioIni + "-" + mesIni + "-01");
            DateTime auxFechaInfoFin = Convert.ToDateTime(anioFin + "-" + mesFin + "-01");

            if (Session["TipoUsuario"].ToString() == "Free Trial" || (bool)(Session["opcionFreeTrial"] ?? false))
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

        private string GetCurrentIdioma()
        {
            return Session["Idioma"] != null ? Session["Idioma"].ToString() : "es";
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

                    codPaisT = FuncionesBusiness.ObtieneCodPaisAcceso(idUsuario, ref codPais2T);
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
        private List<string> GetListStringOfDatatbleColumn(string nameColumn, DataTable dt)
        {
            List<string> lista = new List<string>();
            foreach (DataRow dataRow in dt.Rows)
            {
                lista.Add(dataRow[nameColumn].ToString());
            }
            return lista;
        }

        private Dictionary<string, string> GetChartsTitles(string tipoOpe, string cif, string cifTot,
            string idioma, string codPais = "")
        {
            Dictionary<string, string> titles = new Dictionary<string, string>();
            if (idioma == "es")
            {
                if (tipoOpe == Enums.TipoOpe.I.ToString())
                {
                    if (cifTot == cif + "Tot")
                    {
                        titles.Add("titleValueImp", "Valor " + cif + " Importado US$");
                        titles.Add("titleComparative", "Comparativo " + cif + " US$");
                    }
                    else
                    {
                        titles.Add("titleValueImp", "Peso Importado ("+ Resources.AdminResources.TypeUNIT_Text + ")");
                        titles.Add("titleComparative", "Comparativo (" + Resources.AdminResources.TypeUNIT_Text + ")");
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
                        titles.Add("titleValueImp", "Peso Exportado (" + Resources.AdminResources.TypeUNIT_Text + ")");
                        titles.Add("titleComparative", "Comparativo (" + Resources.AdminResources.TypeUNIT_Text + ")");
                    }
                }
            }
            else
            {
                if (tipoOpe == Enums.TipoOpe.I.ToString())
                {
                    if (cifTot == cif + "Tot")
                    {
                        titles.Add("titleValueImp", cif + " Imported US$");
                        titles.Add("titleComparative", "Comparative " + cif + " US$");
                    }
                    else
                    {
                        titles.Add("titleValueImp", "Weight Imported (kilograms)");
                        titles.Add("titleComparative", "Comparative (kilograms)");
                    }
                }
                else
                {
                    if (cifTot == cif + "Tot")
                    {
                        titles.Add("titleValueImp", cif + " Exported US$");
                        titles.Add("titleComparative", "Comparative " + cif + " US$");
                    }
                    else
                    {
                        titles.Add("titleValueImp", "Weight Exported (kilograms)");
                        titles.Add("titleComparative", "Comparative (kilograms)");
                    }
                }
            }

            titles.Add("titleRankingImporters", Resources.AdminResources.ImportersSummary_Text);
            titles.Add("titleRankingExporters", codPais.Equals("CL") ? Resources.AdminResources.BrandSummary_Text : Resources.AdminResources.ExportersSummary_Text);
            titles.Add("titleRankingOriginCountries", Resources.AdminResources.OriginCountriesSummary_Text);
            titles.Add("titleRankingDestinationCountries", Resources.AdminResources.DestinationCountriesSummary_Text);
            titles.Add("titleRankingProducts", Resources.AdminResources.ProductsRanking_Text);

            return titles;
        }

        private List<Decimal> GetListDecimalsOfDataTableColumn(string nameColumn, DataTable dt)
        {
            List<Decimal> lista = new List<decimal>();
            foreach (DataRow dataRow in dt.Rows)
            {
                if (!string.IsNullOrEmpty(dataRow[nameColumn].ToString()))
                {
                    lista.Add(Math.Round(Convert.ToDecimal(dataRow[nameColumn]), 3));
                }
                else
                {
                    lista.Add(0);
                }
            }
            return lista;
        }

        private int GetTotalPaginas(int cantidadRegistros)
        {
            return (int)Math.Ceiling(Convert.ToDecimal(cantidadRegistros) / TablePageSize);
        }

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

        #endregion

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult TipoOpeChange(string tipoOpe)
        {
            string idUsuario = Session["IdUsuario"].ToString();
            string codPais = Session["CodPais"].ToString();
            Session["TipoOpe"] = tipoOpe;
            string idioma = GetCurrentIdioma();

            MySearchForm objMySearchForm = new MySearchForm();
            string filtro = tipoOpe == "I" ? "Importador" : "Exportador";
            var enabledBtnVerGraficos = false;

            objMySearchForm.ListItemsMyFilters =
                GetMyCompanies(idUsuario, tipoOpe, codPais, filtro, idioma, ref enabledBtnVerGraficos);
            objMySearchForm.EnabledBtnVerGraficos = enabledBtnVerGraficos;
            objMySearchForm.FiltroPeriodo = GetPeriods(tipoOpe, codPais);

            return Json(new
            {
                objMySearchForm
            });
        }

        // Ruben 202307
        private FiltroPeriodo GetPeriods(string tipoOpe, string codPais2, string codPais)
        {
            ValidaCodPais2(codPais2, ref codPais);
            string anioIni = "", mesIni = "", anioFin = "", mesFin = "";
            Funciones.Rango(codPais, tipoOpe, ref anioIni, ref mesIni, ref anioFin, ref mesFin);

            DateTime auxFechaInfoIni = Convert.ToDateTime(anioIni + "-" + mesIni + "-01");
            DateTime auxFechaInfoFin = Convert.ToDateTime(anioFin + "-" + mesFin + "-01");

            if (Session["TipoUsuario"].ToString() == "Free Trial" || (bool)(Session["opcionFreeTrial"] ?? false))
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

        // Ruben 202307
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

        // Ruben 202307
        [SessionExpireFilter]
        [HttpPost]
        public JsonResult Pais2Change(string codPais2, string pais2Text)
        {
            MySearchForm objMySearchForm = new MySearchForm();

            string idioma = GetCurrentIdioma();
            var auxCountries = objMySearchForm.GetCountries(codPais2, idioma);

            if (codPais2 == "8ASI") // Eliminando China
            {
                auxCountries.RemoveAt(0);
            }

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

            string filtro = tipoOpe == "I" ? "Importador" : "Exportador";
            objMySearchForm.ListItemsMyFilters = GetMyCompanies(Session["IdUsuario"].ToString(), tipoOpe, auxCodPais, filtro, idioma, ref enabledBtnVerGraficos);
            objMySearchForm.EnabledBtnVerGraficos = enabledBtnVerGraficos;
            objMySearchForm.FiltroPeriodo = GetPeriods(tipoOpe, objMySearchForm.CodPaisSelected);

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
            string auxCodPais = codPais;

            MySearchForm objMySearchForm = new MySearchForm();

            object objMensaje = ValidaPais(ref auxCodPais2, ref auxCodPais, textCodPais);

            string tipoOpe = Session["TipoOpe"].ToString();
            bool enabledBtnVerGraficos = false;

            objMySearchForm.CodPais2Selected = auxCodPais2;
            objMySearchForm.CodPaisSelected = auxCodPais;

            string idioma = GetCurrentIdioma();
            if (objMensaje != null)
            {
                
                List<SelectListItem> countries2 = objMySearchForm.GetCountries2MyCompanies(idioma).ToList();

                if (countries2.FirstOrDefault(x => x.Value == auxCodPais2) == null)
                {
                    auxCodPais2 = countries2.First().Value;
                    objMySearchForm.CodPais2Selected = auxCodPais2;
                }

                List<SelectListItem> countriesMyCompanies = objMySearchForm.GetCountriesMyCompanies(idioma).ToList();
                if (countriesMyCompanies.FirstOrDefault(x => x.Value == auxCodPais) == null)
                {
                    if(Session["plan"].ToString() == "ESENCIAL")
                    {
                        auxCodPais = "PE";
                    }
                    else
                    {
                        auxCodPais = countriesMyCompanies.First().Value;
                    }
                    
                    objMySearchForm.CodPaisSelected = auxCodPais;
                }

                codPais = auxCodPais;
            }

            string filtro = tipoOpe == "I" ? "Importador" : "Exportador";
            objMySearchForm.ListItemsMyFilters = GetMyCompanies(Session["IdUsuario"].ToString(), tipoOpe, codPais, filtro, idioma, ref enabledBtnVerGraficos);
            objMySearchForm.EnabledBtnVerGraficos = enabledBtnVerGraficos;
            objMySearchForm.FiltroPeriodo = GetPeriods(tipoOpe, objMySearchForm.CodPaisSelected);

            Session["CodPais"] = codPais;

            return Json(new
            {
                objMensaje,
                objMySearchForm
            });
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult VerGraficos( string textPais, string idEmpresa, string textEmpresa,
            bool isCheckedUSD,string opcion, string anioIniAnioMesIni, 
            string fechaFinAnioMesFin, string fechaIni,string fechaFin)
        {
            string tipoOpe = Session["TipoOpe"].ToString();
            string codPais = Session["CodPais"].ToString();

            string cif = Funciones.Incoterm(codPais, tipoOpe);
            string pesoNeto = Funciones.CampoPeso(codPais, tipoOpe);

            string cifTot = "";
            if (isCheckedUSD)
            {
                cifTot = cif + "Tot";
            }
            else
            {
                cifTot = pesoNeto;
            }

            string auxTextEmpresa = textEmpresa.Trim();

            string auxFechaIni = "";
            string auxFechaFin = "";
            GetFechasByOpcion(opcion, fechaIni, fechaFin, ref auxFechaIni, ref auxFechaFin);

            string tabla = GetTabla(tipoOpe, codPais);
            Dictionary<string, string> filtersByTipoOpe = GetFiltersByTypeOpe(tipoOpe);

            DataRow drTotales = FuncionesBusiness.GetTotalesMyCompanies("", "", idEmpresa, auxTextEmpresa, auxFechaIni,
                auxFechaFin, cif, pesoNeto, tabla, filtersByTipoOpe["importerOrExporter"]);


            string lblMessage = "";
            int cantReg = 0;
            if (drTotales==null)
            {
                 cantReg =0;
            }
            else
            {
             cantReg = Convert.ToInt32(drTotales["CantReg"]);
            }
            string idUsuario = Session["IdUsuario"].ToString();

            int cantPartida = 0;
            int cantOriginOrDestinationCountry = 0;
            int cantAux = 0;
            int difCantidad = 0;

            if (cantReg == 0)
            {
                lblMessage = tipoOpe == Enums.TipoOpe.I.ToString() ? Resources.AdminResources.DoNotFoundImports_Text : Resources.AdminResources.DoNotFoundExports_Text;

                FuncionesBusiness.RegistraConsumo(idUsuario, codPais, tipoOpe, "Mis Compañias", "");

                difCantidad = Math.Abs(cantPartida - cantOriginOrDestinationCountry);

                return Json(new
                {
                    lblMessage
                });
            }
            else
            {
                string idioma = GetCurrentIdioma();
                var specificCulture = GetSpecificCulture();

                var lblTitle = (tipoOpe == Enums.TipoOpe.I.ToString()
                                   ? Resources.AdminResources.Imports_Text.ToUpper()
                                   : Resources.AdminResources.Exports_Text.ToUpper()) + " " +
                               Resources.AdminResources.From_Text.ToUpper() + " " + textPais.ToUpper() + " " + Resources.AdminResources.Of_text.ToUpper() + " " + auxTextEmpresa + " ";

                lblTitle += FuncionesBusiness.RangoFechas(auxFechaIni, auxFechaFin, idioma) + " -  ";
                lblTitle += cantReg.ToString("n0", specificCulture) + " " + Resources.AdminResources.Records_Text;


                Dictionary<string, string> chartsTitles = GetChartsTitles(tipoOpe, cif, cifTot, idioma, codPais);

                List<string> categories = new List<string>();

                DataTable objDataTable = GetDataTableByOpcion(idEmpresa, auxTextEmpresa, opcion, auxFechaIni,
                    auxFechaFin, cif, pesoNeto, tabla, filtersByTipoOpe["importerOrExporter"], idioma, ref categories);

                int lenghtTextParitda = auxTextEmpresa.Length;

                string parteFinalTituloChart = "-" + (lenghtTextParitda > LimitTitleAddChartTitle
                                                   ? auxTextEmpresa.Substring(0, LimitTitleAddChartTitle)
                                                   : auxTextEmpresa);

                Chart chartValorImp = GetDataValueImp(objDataTable, categories, cifTot);

                decimal mayor = 0;
                foreach (var lista in chartValorImp.Series[0].data)
                {
                    if (lista > mayor)
                        mayor = lista;
                }

                string numero = Convert.ToInt32(mayor).ToString();
                string cadena = "";
                for (int i = 0; i < numero.Length - 1; i++)
                    cadena += "0";
                chartValorImp.TickInterval = Convert.ToInt64("1" + cadena);

                chartValorImp.TitleContainer = chartsTitles["titleValueImp"];
                chartValorImp.Title = chartValorImp.TitleContainer + parteFinalTituloChart;

                Chart chartComparative = GetDataComparative(idEmpresa, auxTextEmpresa, opcion,
                    anioIniAnioMesIni, fechaFinAnioMesFin, auxFechaIni, auxFechaFin, cif, cifTot, pesoNeto, tabla, filtersByTipoOpe["importerOrExporter"], idioma);
                chartComparative.TitleContainer = chartsTitles["titleComparative"];
                chartComparative.Title = chartComparative.TitleContainer + parteFinalTituloChart;

               
                string textCiFoFobTot = "Total " + (isCheckedUSD ? cif + " US$ " : "kg");
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


                MyCompanie objMyCompanie = new MyCompanie();

                Chart chartProducts = GetDataProducts(idEmpresa, auxTextEmpresa, auxFechaIni, auxFechaFin, cifTot,
                    valueCifTot, tabla, filtersByTipoOpe["importerOrExporter"]);
                chartProducts.TitleContainer = chartsTitles["titleRankingProducts"];
                chartProducts.Title = chartProducts.TitleContainer + parteFinalTituloChart;

                string tableAndWhereSqlPartidas = GetTableAndWhereSqlPartidas(idEmpresa, auxTextEmpresa, codPais, auxFechaIni, auxFechaFin,tabla, filtersByTipoOpe["importerOrExporter"],idioma);
                int auxTotalPaginas = 0;

                objMyCompanie.HtmlTableProducts = GetTablePartidasInHtml(objGridHead, tableAndWhereSqlPartidas, cifTot,
                    valueCifTot, valueCifTotWithFormat, idioma, specificCulture , objMyCompanie.IdProducts ,ref  auxTotalPaginas , ref cantPartida);
                objMyCompanie.TotalPaginasProducts = auxTotalPaginas;

                ConsultaForm objConsultaForm = GetConsultaForm(tipoOpe, codPais);

                if ((tipoOpe == "I" && objConsultaForm.PaisOrigen || tipoOpe=="E" && objConsultaForm.ExistePaisDestino) && valCifTot > 0)
                {
                    objMyCompanie.ExistOriginOrDestinationCountry = true;
                    objMyCompanie.ChartOriginOrDestinationCountry = GetPieDataByFilter(idEmpresa, auxTextEmpresa,
                        filtersByTipoOpe["originOrDestinationCountry"], cifTot, auxFechaIni, auxFechaFin, tabla,
                        valueCifTot, filtersByTipoOpe["importerOrExporter"],
                        (tipoOpe == "I" ? chartsTitles["titleRankingOriginCountries"] : chartsTitles["titleRankingDestinationCountries"])
                        , parteFinalTituloChart);


                    string tableAndWhereSqlOriginCountry = GetTableAndWhereSqlProveedoresAndCountries("", "", idEmpresa,
                        auxTextEmpresa, auxFechaIni, auxFechaFin, filtersByTipoOpe["originOrDestinationCountry"], tabla,
                        filtersByTipoOpe["importerOrExporter"]);

                    int totalPaginas = 0;
                    objMyCompanie.HtmlTableOriginOrDestinationCountry = GetTableInHtmlByFilter(objGridHead,
                        tableAndWhereSqlOriginCountry, filtersByTipoOpe["originOrDestinationCountry"], cifTot,
                        valueCifTot, valueCifTotWithFormat,
                        (tipoOpe == "I"
                            ? Resources.AdminResources.OriginCountry_FormField_Label
                            : Resources.AdminResources.DestinationCountry_FormField_Label), specificCulture,
                        objMyCompanie.IdOriginOrDestinationCountry,
                       ref totalPaginas , ref cantOriginOrDestinationCountry);

                    objMyCompanie.TotalPagesOriginOrDestinationCountry = totalPaginas;

                }
                else
                {
                    objMyCompanie.ExistOriginOrDestinationCountry = false;
                }

                if ((tipoOpe == "I" && objConsultaForm.Proveedor || tipoOpe == "E" && objConsultaForm.ImportadorExp) && valCifTot > 0)
                {
                    objMyCompanie.ExistSupplierOrImporterExp = true;
                    objMyCompanie.ChartSupplierOrImporterExp = GetPieDataByFilter(idEmpresa, auxTextEmpresa,
                        filtersByTipoOpe["supplierOrIporterExp"], cifTot, auxFechaIni, auxFechaFin, tabla,
                        valueCifTot, filtersByTipoOpe["importerOrExporter"],
                        (tipoOpe == "I" ? chartsTitles["titleRankingExporters"] : chartsTitles["titleRankingImporters"])
                        , parteFinalTituloChart);


                    string tableAndWhereSqlsupplierOrIporterExp = GetTableAndWhereSqlProveedoresAndCountries("", "", idEmpresa,
                        auxTextEmpresa, auxFechaIni, auxFechaFin, filtersByTipoOpe["supplierOrIporterExp"], tabla,
                        filtersByTipoOpe["importerOrExporter"]);

                    int totalPaginas = 0;
                    objMyCompanie.HtmlTableSupplierOrImporterExp = GetTableInHtmlByFilter(objGridHead,
                        tableAndWhereSqlsupplierOrIporterExp, filtersByTipoOpe["supplierOrIporterExp"], cifTot,
                        valueCifTot, valueCifTotWithFormat,
                        (tipoOpe == "I"
                            ? codPais.Equals("CL") ? Resources.Resources.Search_Form_BrandField : Resources.Resources.Search_Form_Item06
                            : codPais.Equals("CL")?  Resources.Resources.Search_Form_BrandField : Resources.Resources.Search_Form_Item05), 
                        specificCulture,
                        objMyCompanie.IdSupplierOrImporterExp,
                        ref totalPaginas , ref cantAux);
                    objMyCompanie.TotalPagesSupplierOrImporterExp = totalPaginas;
                }
                else
                {
                    objMyCompanie.ExistSupplierOrImporterExp = false;
                }

               
                string codPais2 = Session["CodPais2"].ToString();
                DataTable dtOpcionesDeDescarga =
                    FuncionesBusiness.GetOpcionesDeDescarga(idUsuario, tipoOpe, codPais2, codPais,
                        idioma, codPais);

                IEnumerable<SelectListItem> opcionesDeDescarga = null;
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
                Session["idEmpresa"] = idEmpresa;
                Session["textEmpresa"] = auxTextEmpresa;
                Session["fechaIni"] = auxFechaIni;
                Session["fechaFin"] = auxFechaFin;

                Session["auxFechaIni"] = auxFechaIni; //Se usa el método de exportar todo
                Session["auxFechaFin"] = auxFechaFin; // Se usa en el método de exportar todo
                Session["auxCantReg"] = cantReg; //Usado en la función de exportar todo

                FuncionesBusiness.RegistraConsumo(idUsuario, codPais, tipoOpe, "Mis Compañias", "");

                difCantidad = Math.Abs(cantPartida - cantOriginOrDestinationCountry);
                Session["cantPartida"] = cantPartida;
                Session["cantOriginOrDestinationCountry"] = cantOriginOrDestinationCountry;
                string informaColombiaHTML = null;
                if (textPais == "Colombia") {
                    InformaColombia informaColombia = InformaColombia.obtenerInforme(Convert.ToInt32(idEmpresa));

                    string logs = idEmpresa + " | " + informaColombia.RazonSocial;
                    Funciones.GrabaLog(Session["IdUsuario"].ToString(), codPais, tipoOpe, fechaIni, fechaFin, "InformaColombia", logs);

                    informaColombiaHTML = RenderViewToString(this.ControllerContext, "Modals/InformaColombia", informaColombia);
                }
                
                return Json(new
                {
                    lblMessage,
                    lblTitle,
                    opcionesDeDescarga,
                    chartValorImp,
                    chartComparative,
                    chartProducts,
                    objMyCompanie,
                    objMensajeCantRegMax,
                    difCantidad,
                    informaColombiaHTML
                });
            }
        }

        private DataTable GetDataTableByOpcion(string idEmpresa, string textEmpresa, string opcion,
            string fechaIni, string fechaFin, string cif, 
            string pesoNeto, string tabla, string importador,
            string idioma, ref List<string> refListCategories)
        {
            DataTable dataTable;
            if (opcion != "AÑOS")
            {
                dataTable = FuncionesBusiness.GetMensualesMyCompanies("", "", idEmpresa, textEmpresa, fechaIni,
                    fechaFin, cif, pesoNeto, tabla, importador, idioma);

                refListCategories = GetListStringOfDatatbleColumn("IdAñoMes", dataTable);
            }
            else
            {
                dataTable = FuncionesBusiness.GetAnualesMyCompanies("", "", idEmpresa, textEmpresa, fechaIni, fechaFin,
                    cif, pesoNeto, tabla, importador);
                refListCategories = GetListStringOfDatatbleColumn("IdAño", dataTable);
            }

            return dataTable;
        }

        private Chart GetDataValueImp(DataTable dataSource, List<string> categories, string cifTot)
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

        private Chart GetDataComparative(string idEmpresa, string textEmpresa, string opcion,
            string anioIniAnioMesIni, string fechaFinAnioMesFin, string fechaIni,
            string fechaFin,string cif, string cifTot,
            string pesoNeto, string tabla, string importador,
            string idioma)
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

                    objDataTable = FuncionesBusiness.GetMensualesMyCompanies("", "", idEmpresa, textEmpresa,
                        auxFechaIni, auxFechaFin, cif, pesoNeto, tabla, importador, idioma);


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
                objDataTable = FuncionesBusiness.GetAnualesMyCompanies("", "", idEmpresa, textEmpresa, fechaIni,
                    fechaFin, cif, pesoNeto, tabla, importador);

                objChart.Categories = GetListStringOfDatatbleColumn("IdAño", objDataTable);
                objChart.Series.Add(new ChartSerie
                {
                    data = GetListDecimalsOfDataTableColumn(cifTot, objDataTable)
                });
            }

            return objChart;
        }

        private Chart GetDataProducts(string idEmpresa, string textEmpresa, string fechaIni,
            string fechaFin ,string cifTot, string valueCifTot,
            string tabla,string importador)
        {
            DataTable objDataTable = FuncionesBusiness.GetPartidasMyCompanies(idEmpresa, textEmpresa, fechaIni, fechaFin, cifTot,valueCifTot, tabla, importador);

            var objChart = new Chart {Categories = GetListStringOfDatatbleColumn("Nandina", objDataTable)};

            objChart.Series.Add(new ChartSerie
            {
                data = GetListDecimalsOfDataTableColumn(cifTot, objDataTable)
            });

            return objChart;
        }

        private string GetTableAndWhereSqlPartidas(string idEmpresa, string textEmpresa, string codPais,
            string fechaIni, string fechaFin, string tabla, string importador, string idioma)
        {
            string partida = "Partida";
            if (idioma == "en")
                partida = "Partida_en";

            string sql = "from " + tabla + " T, Partida_" + codPais + " P ";
            sql += "where T.IdPartida = P.IdPartida and FechaNum between " + fechaIni + " and " + fechaFin + " ";
            if (textEmpresa.Substring(0, 3) != "[G]")
                sql += "and Id" + importador + " = " + idEmpresa + " ";
            else
                sql += "and Id" + importador + " in (select IdFavorito from FavoritoGrupo where IdGrupo = " +
                       idEmpresa + ") ";
            sql += "group by T.IdPartida, T.Nandina, " + partida ;
            return sql;
        }

        private string GetTablePartidasInHtml(GridHeadTabMy objGridHead, string sqlTableAndWhere, string cifTot, 
            string valueCifTot, string valueCifTotWithFormat, string idioma,
            CultureInfo cultureInfo, string idUlPaging, ref int totalPaginas , ref int cantPorPagina)
        {
            int cantidadRegistros = FuncionesBusiness.GetCantidadRegistrosPartidas(sqlTableAndWhere);
            totalPaginas = GetTotalPaginas(cantidadRegistros);

            DataTable dt = FuncionesBusiness.GetTablePartidasDataMyCompanies(sqlTableAndWhere, cifTot, valueCifTot,
                idioma, 1, TablePageSize);

            cantPorPagina = dt.Rows.Count;

            objGridHead.CodPartida = Resources.AdminResources.Nandina2_FilterText;
            objGridHead.Descripcion = Resources.AdminResources.HTS_Description_Text;
            TabDataTabMy viewData = new TabDataTabMy { GridHead = objGridHead };

            viewData.ListRows = GetListPartidas(dt, cifTot, cultureInfo);
            viewData.CiFoFobTotal = valueCifTotWithFormat;
            viewData.TotalPaginasTab = totalPaginas;
            viewData.Filtro = idUlPaging;
            viewData.CodPais = Session["CodPais"].ToString();

            return RenderViewToString(this.ControllerContext, "GridViews/PartidasTableView", viewData);
        }
        private string GetRowsPartidasInHtml(string sqlTableAndWhere,
            string cifTot, string valueCifTot,
            int pagina, CultureInfo cultureInfo, string idioma, string idFiltro,ref int cantRegistros)
        {
            var dtTable = FuncionesBusiness.GetTablePartidasDataMyCompanies(sqlTableAndWhere,
                 cifTot, valueCifTot, idioma, pagina, TablePageSize);
            cantRegistros = dtTable.Rows.Count;
            var viewData = new TabDataTabMy()
            {
                Filtro = idFiltro,
                ListRows = GetListPartidas(dtTable,cifTot, cultureInfo)
        };

            return RenderViewToString(this.ControllerContext, "GridViews/PartidasTableRowView", viewData);
        }


        private List<GridRowTabMy> GetListPartidas(DataTable dt, string cifTot, CultureInfo cultureInfo)
        {
            var lista = new List<GridRowTabMy>();
            foreach (DataRow dataRow in dt.Rows)
            {
                if (!string.IsNullOrEmpty(dataRow[cifTot].ToString()) &&
                    !string.IsNullOrEmpty(dataRow["Participacion"].ToString()))
                {
                    lista.Add(new GridRowTabMy
                    {
                        Id = dataRow["IdPartida"].ToString(),
                        CodPartida = dataRow["Nandina"].ToString(),
                        Descripcion = dataRow["Partida"].ToString(),
                        CiFoFobTot = Convert.ToDecimal(dataRow[cifTot]).ToString("n0", cultureInfo),
                        CiFoFobPor = Convert.ToDecimal(dataRow["Participacion"]).ToString("n1", cultureInfo)
                    });
                }
            }
            return lista;
        }
        private Chart GetPieDataByFilter(string idEmpresa, string textEmpresa,  string filtro,
            string cifTot, string fechaIni, string fechaFin, 
            string tabla, string valueCifTot, string importador,
            string chartTitle,string parteFinalChartTitle)
        {
            DataTable pieData = FuncionesBusiness.GetPiesDataMyCompanie(idEmpresa, textEmpresa, filtro, cifTot,
                valueCifTot, fechaIni, fechaFin, tabla, importador);

            var chart = new Chart
            {
                PieDatas = GetListPieData(pieData, filtro),
                TitleContainer = chartTitle
            };
            chart.Title = chart.TitleContainer + parteFinalChartTitle;
            return chart;
        }

        private string GetTableAndWhereSqlProveedoresAndCountries(string idPartida, string desComercial, string idEmpresa,
            string textEmpresa,string fechaIni, string fechaFin,
            string filtro, string tabla, string importador)
        {
            string sql = "from " + tabla + " where FechaNum between " + fechaIni + " and " + fechaFin + " ";
            if (idPartida != "")
            {
                sql += "and IdPartida = " + idPartida + " ";
                if (desComercial != "")
                    sql += "and contains(DesComercial, '\"" + desComercial + "\"') ";
            }
            else
            {
                if (textEmpresa.Substring(0, 3) != "[G]")
                    sql += "and Id" + importador + " = " + idEmpresa + " ";
                else
                    sql += "and Id" + importador + " in (select IdFavorito from FavoritoGrupo where IdGrupo = " +
                           idEmpresa + ") ";
            }
            sql += "group by Id" + filtro + ", " + filtro ;

            return sql;
        }

        private string GetTableInHtmlByFilter(GridHeadTabMy objGridHead, string sqlTableAndWhere, string filtro,
            string cifTot, string valueCifTot, string valueCifTotWithFormat,
            string headColumnDescription, CultureInfo cultureInfo,
            string idUlPaging, ref int totalPaginas , ref int cantPorPagina)
        {
            int cantidadRegistros = FuncionesBusiness.GetCantidadRegistros(sqlTableAndWhere, filtro);
            totalPaginas = GetTotalPaginas(cantidadRegistros);

            DataTable dtTable = FuncionesBusiness.GetRegistrosByFiltro(sqlTableAndWhere,
                filtro, cifTot, valueCifTot, 1, TablePageSize, cifTot);

            cantPorPagina = dtTable.Rows.Count;

            objGridHead.Descripcion = headColumnDescription;
            TabDataTabMy viewData = new TabDataTabMy
            {
                GridHead = objGridHead,
                ListRows = GetGridRowsByFilter(dtTable, filtro, cifTot, cultureInfo),
                CiFoFobTotal = valueCifTotWithFormat,
                TotalPaginasTab = totalPaginas,
                Filtro = idUlPaging
            };
            return RenderViewToString(this.ControllerContext, "GridViews/TableView", viewData);
        }

        private string GetRowsInHtmlByFilter(string sqlTableAndWhere, string filtro,
            string cifTot, string valueCifTot,
            int pagina, CultureInfo cultureInfo, string idFiltro, ref int cantRegistros)
        {
            var dtTable = FuncionesBusiness.GetRegistrosByFiltro(sqlTableAndWhere,
                filtro, cifTot, valueCifTot, pagina, TablePageSize, cifTot);
            cantRegistros = dtTable.Rows.Count;
            var viewData = new TabDataTabMy()
            {
                Filtro = idFiltro,
                ListRows = GetGridRowsByFilter(dtTable, filtro, cifTot, cultureInfo)
            };

            return RenderViewToString(this.ControllerContext, "GridViews/TableRowView", viewData);
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
                    lista.Add(new GridRowTabMy
                    {
                        Id = dataRow["Id" + filtro].ToString(),
                        Descripcion = dataRow[filtro].ToString(),
                        CiFoFobTot = Convert.ToDecimal(dataRow[cifTot]).ToString("n0", cultureInfo),
                        CiFoFobPor = Convert.ToDecimal(dataRow["Participacion"]).ToString("n1", cultureInfo)
                    });
                }
            }
            return lista;
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult PageIndexChanging(string idFiltro, int pagina)
        {
            string tipoOpe = Session["TipoOpe"].ToString();
            string codPais = Session["CodPais"].ToString();

            string auxTextEmpresa = Session["textEmpresa"].ToString();
            string auxFechaIni = Session["auxFechaIni"].ToString();
            string auxFechaFin = Session["auxFechaFin"].ToString();
            string idEmpresa = Session["idEmpresa"].ToString();
            string cifTot = Session["cifTot"].ToString();
            string valueCifTot = Session["valueCifTot"].ToString();

            string tabla = GetTabla(tipoOpe, codPais);
            string idioma = GetCurrentIdioma();
            var specificCulture = GetSpecificCulture();
            Dictionary<string, string> filtersByTipoOpe = GetFiltersByTypeOpe(tipoOpe);

            string rowsByPage = "";

            int cantPartida = 0;
            int cantOriginOrDestinationCountry = 0;
            int cantAux = 0;

            switch (idFiltro)
            {
                case "Partida":
                    string tableAndWhereSqlPartidas = GetTableAndWhereSqlPartidas(idEmpresa, auxTextEmpresa, codPais, auxFechaIni,
                        auxFechaFin, tabla, filtersByTipoOpe["importerOrExporter"], idioma);

                    rowsByPage = GetRowsPartidasInHtml(tableAndWhereSqlPartidas, cifTot, valueCifTot, pagina,
                        specificCulture, idioma, idFiltro, ref cantPartida);
                    break;
                case "OriginOrDestinationCountry":
                    string tableAndWhereSqlOriginCountry = GetTableAndWhereSqlProveedoresAndCountries("", "", idEmpresa,
                        auxTextEmpresa, auxFechaIni, auxFechaFin, filtersByTipoOpe["originOrDestinationCountry"], tabla,
                        filtersByTipoOpe["importerOrExporter"]);

                    rowsByPage = GetRowsInHtmlByFilter(tableAndWhereSqlOriginCountry,
                        filtersByTipoOpe["originOrDestinationCountry"], cifTot, valueCifTot, pagina, specificCulture,
                        idFiltro, ref cantOriginOrDestinationCountry);
                    break;
                case "SupplierOrImporterExp":
                    string tableAndWhereSqlsupplierOrIporterExp = GetTableAndWhereSqlProveedoresAndCountries("", "", idEmpresa,
                        auxTextEmpresa, auxFechaIni, auxFechaFin, filtersByTipoOpe["supplierOrIporterExp"], tabla,
                        filtersByTipoOpe["importerOrExporter"]);
                    rowsByPage = GetRowsInHtmlByFilter(tableAndWhereSqlsupplierOrIporterExp,
                        filtersByTipoOpe["supplierOrIporterExp"], cifTot, valueCifTot, pagina, specificCulture,
                        idFiltro, ref cantAux);
                    break;
            }

            int cantAnte = 0;
            int cantNueva = 0;

            if (idFiltro == "OriginOrDestinationCountry")
            {
                cantAnte = Convert.ToInt32(Session["cantPartida"]);
                cantNueva = cantOriginOrDestinationCountry;
                Session["cantOriginOrDestinationCountry"] = cantOriginOrDestinationCountry;
            }
            else if (idFiltro == "Partida")
            {
                cantAnte = Convert.ToInt32(Session["cantOriginOrDestinationCountry"]);
                cantNueva = cantPartida;
                Session["cantPartida"] = cantPartida;
            }

            return Json(new
            {
                rowsByPage,
                difCantPorPagina = Math.Abs(cantAnte - cantNueva)
            });
        }

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
        [SessionExpireFilter]
        [HttpPost]
        public JsonResult VerRegistrosPageIndexChanging(int pagina)
        {
            string tipoOpe = Session["TipoOpe"].ToString();
            string codPais2 = Session["CodPais2"].ToString();
            string codPais = Session["CodPais"].ToString();

            string tableAndWhereSqlVerRegistros = Session["TableAndWhereVerRegistros"].ToString();

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
        /// <summary>
        /// Obtiene datos de modal Ver Registros para un categoría seleccionada de un gráfico.
        /// </summary>
        /// <param name="opcion">Tipo de filtro periodo</param>
        /// <param name="category">Categoría de gráfico</param>
        /// <returns></returns>
        [SessionExpireFilter]
        [HttpPost]
        public JsonResult VerRegistrosByCategory(string opcion, string category)
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

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult VerRegistrosByNandina(string nandina)
        {
            Session["fechaIni"] =  Session["auxFechaIni"].ToString() ; 
            Session["fechaFin"] =  Session["auxFechaFin"].ToString();

            string codPais = Session["CodPais"].ToString();
            string idPartida = Funciones.BuscaIdPartida(nandina, codPais);

            string idFiltro = "Partida";
            Session["IdFiltroVerRegistros"] = idFiltro;
            Session["IdRegistroVerRegistros"] = idPartida;

            return GetDataModalVerRegistros(idFiltro, idPartida);
        }

        private JsonResult GetDataModalVerRegistros(string idFiltro, string idRegistro)
        {
            string tipoOpe = Session["TipoOpe"].ToString();
            string idEmpresa = Session["idEmpresa"].ToString();
            string auxTextEmpresa = Session["textEmpresa"].ToString();
            string auxFechaIni = Session["fechaIni"].ToString();
            string auxFechaFin = Session["fechaFin"].ToString();
            
            string codPais = Session["CodPais"].ToString();
            string codPais2 = Session["CodPais2"].ToString();

            string idioma = GetCurrentIdioma();

            string tituloRegistros = "";

            if (idEmpresa != "")
            {
                if (auxTextEmpresa.Substring(0, 3) != "[G]")
                {
                    tituloRegistros += FuncionesBusiness.BuscaEmpresa(idEmpresa, codPais)+ " - ";
                }
                else
                {
                    tituloRegistros += auxTextEmpresa + " - ";
                }
            }

            switch (idFiltro)
            {
                case "Partida":
                    tituloRegistros = FuncionesBusiness.BuscaPartida(idRegistro, codPais, idioma)+ " - "+ tituloRegistros;
                    break;
                case "OriginOrDestinationCountry":
                    tituloRegistros += Funciones.BuscaPais(idRegistro, codPais) + " - ";
                    break;
                case "SupplierOrImporterExp":
                    tituloRegistros += Funciones.BuscaProveedor(idRegistro, codPais) + " - ";
                    break;
            }
            tituloRegistros += FuncionesBusiness.RangoFechas(auxFechaIni, auxFechaFin, idioma);
            Session["MCTituloVerRegistros"] = tituloRegistros;

            bool existeDesComercial = Funciones.FlagDesComercial(codPais, tipoOpe);
            string tabla = GetTabla(tipoOpe, codPais);
            string tableAndWhereSqlVerRegistros = GetTableAndWhereSqlVerRegistros(tipoOpe,idEmpresa, auxTextEmpresa, auxFechaIni, auxFechaFin, tabla,existeDesComercial,idFiltro,idRegistro);

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

                Session["MCCantRegVerRegistros"] = cantRegistros;

                DataTable dtOpcionesDeDescarga =
                    FuncionesBusiness.GetOpcionesDeDescarga(Session["IdUsuario"].ToString(), tipoOpe, codPais2, codPais,
                        idioma, codPais);

                opcionesDeDescarga = new List<SelectListItem>();

                if (dtOpcionesDeDescarga != null && dtOpcionesDeDescarga.Rows.Count > 0)
                {
                    opcionesDeDescarga = dtOpcionesDeDescarga.AsEnumerable().Select(m => new SelectListItem()
                    {
                        Text = m.Field<string>("Descarga"),
                        Value = (m.Field<Int32>("IdDescargaCab")).ToString()
                    });
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
                tituloRegistros,
                opcionesDeDescarga,
                tablaVerRegistros,
                totalPages
            });
        }

        private string GetTableAndWhereSqlVerRegistros(string tipoOpe, string idEmpresa, string textEmpresa, string fechaIni,
            string fechaFin, string tabla, bool existeDesComercial, 
            string idFiltro, string idRegistro, string desComercial = "")
        {
            string sql = "from " + tabla + " ";
            sql += "where 1 = 1 ";

            string sqlFiltro = "and FechaNum between " + fechaIni + " and " + fechaFin + " ";
            
            if (tipoOpe == "I")
            {
                if (idEmpresa != "")
                {
                    if (textEmpresa.Substring(0, 3) != "[G]")
                    {
                        sqlFiltro += "and IdImportador = " + idEmpresa + " ";
                    }
                    else
                    {
                        sqlFiltro += "and IdImportador in (select IdFavorito from FavoritoGrupo where IdGrupo = " +idEmpresa + ") ";
                    }
                }

                switch (idFiltro)
                {
                    case "Partida":
                        sqlFiltro += "and IdPartida = " + idRegistro + " ";
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
                if (idEmpresa != "")
                {
                    if (textEmpresa.Substring(0, 3) != "[G]")
                    {
                        sqlFiltro += "and IdExportador = " + idEmpresa + " ";
                    }
                    else
                    {
                        sqlFiltro += "and IdExportador in (select IdFavorito from FavoritoGrupo where IdGrupo = " +idEmpresa + ") ";
                    }
                }

                switch (idFiltro)
                {
                    case "Partida":
                        sqlFiltro += "and IdPartida = " + idRegistro + " ";
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

            Session["MyCompanieSqlFiltro"] = sqlFiltro;
            Session["TableAndWhereVerRegistros"] = sql;
            return sql;
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
                objVerRegistroTableHead.IsVisibleFobOFasUnit = (objConsultaForm.Cif != "FOB" && objConsultaForm.CodPais != "MXD");
                objVerRegistroTableHead.IsVisibleCifImptoUnit = objConsultaForm.CodPais == "PE";
                objVerRegistroTableHead.PaisOrigenODestino = Resources.AdminResources.OriginCountry_FormField_Label;
                objVerRegistroTableHead.Exportador = objConsultaForm.CodPais == "CL" ? Resources.Resources.Search_Form_BrandField :  Resources.Resources.Search_Form_Item06;
            }
            else
            {
                objVerRegistroTableHead.IsVisibleExportador = objConsultaForm.Exportador;
                objVerRegistroTableHead.IsVisibleImportador = objConsultaForm.ImportadorExp;
                objVerRegistroTableHead.IsVisibleFobOFasUnit = false;
                objVerRegistroTableHead.IsVisibleCifImptoUnit = false;
                objVerRegistroTableHead.PaisOrigenODestino = Resources.AdminResources.DestinationCountry_FormField_Label;
                objVerRegistroTableHead.Importador = objConsultaForm.CodPais == "CL" ? Resources.Resources.Search_Form_BrandField : Resources.Resources.Search_Form_Item05;
            }

            return objVerRegistroTableHead;
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
        public JsonResult BuscarPorDesComercial(string textDesComercial, bool enabledFiltros)
        {
            string tipoOpe = Session["TipoOpe"].ToString();

            string idEmpresa = Session["idEmpresa"].ToString();
            string auxTextEmpresa = Session["textEmpresa"].ToString();
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

            bool existeDesComercial = Funciones.FlagDesComercial(codPais, tipoOpe);
            string tabla = GetTabla(tipoOpe, codPais);

            string tableAndWhereSqlVerRegistros = GetTableAndWhereSqlVerRegistros(tipoOpe, idEmpresa, auxTextEmpresa,
                auxFechaIni, auxFechaFin, tabla,
                existeDesComercial, idFiltro, idRegistro,
                textDesComercial.Trim());


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

                VerRegistroTable objVerRegistroTable = new VerRegistroTable
                {
                    TotalPaginas = (int)Math.Ceiling(Convert.ToDecimal(cantRegistros) / VerRegistrosPageSize),
                    TipoOpe = tipoOpe,
                    VerRegistroTableHead = GetVerRegistroTableHead(objConsultaForm),
                    VerRegistroTableRows = GetRowsVerRegistros(dtRegistros, objConsultaForm, GetSpecificCulture(), pagina)
                };

                tablaVerRegistro = RenderViewToString(this.ControllerContext, "GridViews/VerRegistroRows",
                    objVerRegistroTable);
                totalPages = objVerRegistroTable.TotalPaginas;

                Session["MCCantRegVerRegistros"] = cantRegistros;


                if (textDesComercial.Trim() != "")
                {
                    resultadoDesComercialVerRegistro = cantRegistros + " " + Resources.AdminResources.Records_Text;
                }
            }
            else
            {

                objMensaje = new
                {
                    titulo = Resources.AdminResources.Filter_MyCompanies_Text,
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

        #region ExportarArchivosExcel

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
        public JsonResult GetAllExcelFile(string codDescarga)
        {
            string idEmpresa = Session["idEmpresa"].ToString();
            int cantReg = Convert.ToInt32(Session["auxCantReg"]);

            string fechaIni = Session["auxFechaIni"].ToString();
            string fechaFin = Session["auxFechaFin"].ToString();
            string textEmpresa = Session["textEmpresa"].ToString();
            string tipoOpe = Session["TipoOpe"].ToString();
            string codPais = Session["CodPais"].ToString();

            string idioma = GetCurrentIdioma();
            Dictionary<string, string> filtersByTipoOpe = GetFiltersByTypeOpe(tipoOpe);
            string tituloRegistros;
            string sqlFiltro = "and FechaNum between " + fechaIni + " and " + fechaFin + " ";
            if (textEmpresa.Substring(0, 3) != "[G]")
            {
                sqlFiltro += "and Id"+filtersByTipoOpe["importerOrExporter"] +" = " + idEmpresa + " ";
                tituloRegistros = FuncionesBusiness.BuscaEmpresa(idEmpresa, codPais) + " - "; ;
            }
            else
            {
                sqlFiltro += "and Id" + filtersByTipoOpe["importerOrExporter"] + " in (select IdFavorito from FavoritoGrupo where IdGrupo = " + idEmpresa + ") ";
                tituloRegistros = textEmpresa + " - ";
            }
            Session["MyCompanieSqlFiltro"] = sqlFiltro;

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
                    oLog.Add("Controlador => MisCompanias", Session["idUsuario"].ToString());
                    oLog.Add("Metodo Generado => GetAllExcelFile()", Session["idUsuario"].ToString());
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

            string idioma = GetCurrentIdioma();

            var titulos = FuncionesBusiness.ListaTitulosDescarga(codDescarga, "", codPais, tipoOpe, idioma);
            var campos = FuncionesBusiness.ListaCamposDescarga(codDescarga, "", codPais, tipoOpe, idioma);

            var sqlFiltro = Session["MyCompanieSqlFiltro"].ToString();

            string ahora = DateTime.Now.ToString("yyyyMMddHHmmss");

            string nombreArchivo = "Veritrade_" + codUsuario + "_" + codPais + "_" + tipoOpe + "_" + ahora;


            bool isFreeTrial = Session["TipoUsuario"].ToString() == "Free Trial" || (bool)Session["opcionFreeTrial"];

            FuncionesBusiness.GeneraArchivoBusqueda(Convert.ToInt32(idUsuario), isFreeTrial, codPais,
                tipoOpe, titulos, campos, sqlFiltro.Replace("'", "''").Replace("\"", "\"\""), cantReg, nombreArchivo, ((cantReg <= Variables.CantRegMaxExcel && flagExcel) ? 'S' : 'N'));


            Funciones.GrabaLog(idUsuario, codPais, tipoOpe, "0", "0", "MisCompaniasController", "");

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

            string sqlFiltro = Session["MyCompanieSqlFiltro"].ToString();

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
                    oLog.Add("Controlador => MisCompanias", Session["idUsuario"].ToString());
                    oLog.Add("Metodo Generado => GetVerRegistrosExcelFile()", Session["idUsuario"].ToString());
                    oLog.Add("Consulta Session => " + sqlFiltro, Session["idUsuario"].ToString());

                }
                catch (Exception ex)
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
                infoArchivo = GeneraExcelRegistros(codDescarga, Convert.ToInt32(Session["MCCantRegVerRegistros"]), Session["MCTituloVerRegistros"].ToString()),
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
            string idEmpresa = Session["idEmpresa"].ToString();
            string tipoOpe = Session["TipoOpe"].ToString();
            string codPais = Session["CodPais"].ToString();
            string textEmpresa = Session["textEmpresa"].ToString();
            string fechaIni = Session["auxFechaIni"].ToString();
            string fechaFin = Session["auxFechaFin"].ToString();

            string cifTot = Session["cifTot"].ToString();
            string valueCifTot = Session["valueCifTot"].ToString();
            
            string idioma = GetCurrentIdioma();
            string nombreArchivo;
            string templateName = HostingEnvironment.MapPath("~/ExcelTemplate/VeritradeResumenTemplate2.xlsx");
            var EsFreeTrial = (Session["TipoUsuario"].ToString() == "Free Trial") || (bool)Session["opcionFreeTrial"];


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

                string paisTipoOpe = FuncionesBusiness.Pais(codPais, idioma).ToUpper() + " - " +
                                     (tipoOpe == "I" ? Resources.AdminResources.Imports_Text.ToUpper() : Resources.AdminResources.Exports_Text.ToUpper());
                string filtros = "[" + Resources.Resources.Search_Form_Item05 + "] " + textEmpresa;
                string rango = Resources.AdminResources.Period_Text + ": " + FuncionesBusiness.RangoFechas(fechaIni, fechaFin, idioma);

                string registros = Resources.AdminResources.RecordsFirsLetterUpperCase_Text + ": " +
                                   cantReg.ToString("n0", GetSpecificCulture());

                Dictionary<string, string> filtersByTipoOpe = GetFiltersByTypeOpe(tipoOpe);

                string tabla = GetTabla(tipoOpe, codPais);

                DataTable dt;
                OfficeOpenXml.ExcelWorksheet ws;
                switch (opcion)
                {
                    case "Partidas":
                        string tableAndWhereSqlPartidas = GetTableAndWhereSqlPartidas(idEmpresa, textEmpresa, codPais, fechaIni, fechaFin, tabla, filtersByTipoOpe["importerOrExporter"], idioma);
                        dt = FuncionesBusiness.GetTablePartidasDataMyCompanies(tableAndWhereSqlPartidas, cifTot, valueCifTot,
                            idioma, -1, -1,false, esFree: EsFreeTrial);
                        dt.Columns.Remove("IdPartida");

                        ws = package.Workbook.Worksheets["Partidas"];
                        ws.Name = Resources.Resources.Demo_Products_Tab;

                        if (EsFreeTrial)
                        {
                            ws.Cells["B5"].Value = Resources.Resources.Trial_Excel_Text;
                            ws.Cells["B5"].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                        }

                        ws.Cells["B1"].Value = paisTipoOpe;
                        ws.Cells["B2"].Value = filtros;
                        ws.Cells["B3"].Value = rango;
                        ws.Cells["B4"].Value = registros;
                        ws.Cells["A6"].Value = Resources.AdminResources.Nandina2_FilterText;
                        ws.Cells["B6"].Value = Resources.AdminResources.HTS_Description_Text;
                        ws.Cells["C6"].Value = Session["textCiFoFobTot"].ToString();
                        ws.Cells["A7"].LoadFromDataTable(dt, false);
                        CalculaFormulasExcel(ws, dt);
                        break;
                    case "Proveedores":
                        string tableAndWhereSqlsupplierOrIporterExp = GetTableAndWhereSqlProveedoresAndCountries("", "", idEmpresa,
                            textEmpresa, fechaIni, fechaFin, filtersByTipoOpe["supplierOrIporterExp"], tabla,
                            filtersByTipoOpe["importerOrExporter"]);

                        dt = FuncionesBusiness.GetRegistrosByFiltro(tableAndWhereSqlsupplierOrIporterExp,
                            filtersByTipoOpe["supplierOrIporterExp"], cifTot, valueCifTot, -1, -1, cifTot, false, esFree: EsFreeTrial);

                        dt.Columns.Remove(tipoOpe == "I" ? "IdProveedor" : "IdImportadorExp");

                        ws = package.Workbook.Worksheets["Exportadores1"];
                        ws.Name = (tipoOpe == "I" ? Resources.Resources.Demo_Exporters_Tab : Resources.Resources.Demo_Importers_Tab);

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

                        string tableAndWhereSqlOriginCountry = GetTableAndWhereSqlProveedoresAndCountries("", "", idEmpresa,
                            textEmpresa, fechaIni, fechaFin, filtersByTipoOpe["originOrDestinationCountry"], tabla,
                            filtersByTipoOpe["importerOrExporter"]);

                        dt = FuncionesBusiness.GetRegistrosByFiltro(tableAndWhereSqlOriginCountry,
                            filtersByTipoOpe["originOrDestinationCountry"], cifTot, valueCifTot, -1, -1, cifTot, false, esFree: EsFreeTrial);

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
                fileName = nombreArchivo,
                objMensajeFlag
            });
        }

        void CalculaFormulasExcel(OfficeOpenXml.ExcelWorksheet ws, DataTable dt)
        {
            int i = 7;
            int t = i + dt.Rows.Count;

            ws.Cells["A" + t].Value = "Total";
            ws.Cells["C" + t].Formula = "=SUM(C7:C" + (t - 1) + ")";
            ws.Cells["D" + t].Formula = "=SUM(D7:D" + (t - 1) + ")";

            foreach (DataRow dr in dt.Rows)
            {
                ws.Cells["D" + i].FormulaR1C1 = "=R[0]C[-1] / R" + t + "C[-1] * 100";
                i += 1;
            }
            ws.Cells["A" + t + ":D" + t].Style.Font.Color
                .SetColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));
            ws.Cells["A" + t + ":D" + t].Style.Font.Bold = true;
            ws.Cells["A" + t + ":D" + t].Style.Fill.PatternType =
                OfficeOpenXml.Style.ExcelFillStyle.Solid;
            ws.Cells["A" + t + ":D" + t].Style.Fill.BackgroundColor
                .SetColor(System.Drawing.Color.FromArgb(48, 167, 229));
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