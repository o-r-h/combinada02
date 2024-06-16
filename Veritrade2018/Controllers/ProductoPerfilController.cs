using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Veritrade2018.Helpers;
using Veritrade2018.Models.Admin;
using System.Globalization;
using Veritrade2018.Controllers.Admin;
using Veritrade2018.Models;
using System.IO;
using Microsoft.Ajax.Utilities;

namespace Veritrade2018.Controllers
{
    public class ProductoPerfilController : BaseController
    {
        // GET: ProductoPerfil
        [HttpGet]
        public ActionResult Index(string culture)
        {
            return View();
        }

        [HttpPost]
        public JsonResult BuscarProducto(string description, string codPais = "", string opcion = "")
        {
            string idioma = "es";
            var json = MisProductos.SearchProduct(description, idioma);
            return Json(json);
        }

        [HttpPost]
        public JsonResult BuscarClick(int codProducto, string idioma)
        {
            List<MisProductos> listaProducto = null;
            List<ListProductByPaises> listaConsolidado = null;
            DataTable dtProducto = FuncionesBusiness.SearchProductData(codProducto, idioma);
            List<Object> dataValorCIF = null;
            List<Object> dataPrecioProm = null;
            List<Object> dataCompCIF = null;
            //listaProductos = dtProducto.AsEnumerable().Select(m => new MisProductos()
            //{
            //    CodProducto = m.Field<string>("CodProducto"),
            //    Descripcion = m.Field<string>("Descripcion")
            //}).ToList();
            string viewListConsolidate = "";
            MisProductos objMiProducto = null;
            ListProductByPaises objProductoByPais = null;


            ChartPP chartCompCif = new ChartPP();

            if (dtProducto != null && dtProducto.Rows.Count > 0)
            {
                DataTable dtProductByPais = FuncionesBusiness.SearchConsolidateCountries(Convert.ToInt32(dtProducto.Rows[0]["IdProducto"]));
                listaConsolidado = dtProductByPais.AsEnumerable().Select(m => new ListProductByPaises()
                {
                    IdPaisAduana = m.Field<int>("IdPaisAduana"),
                    PaisAduana = m.Field<string>("PaisAduana"),
                    Importaciones = m.Field<string>("Importaciones"),
                    Exportaciones = m.Field<string>("Exportaciones"),
                    Importadores = m.Field<int>("Importadores"),
                    Exportadores = m.Field<int>("Exportadores")
                }).ToList();
                viewListConsolidate = RenderViewToString(this.ControllerContext, "Partials/ListPaises", listaConsolidado);
                DataTable dtFlag = FuncionesBusiness.SearchProductFlag(Convert.ToInt32(dtProducto.Rows[0]["IdProducto"]));
                DataTable dtImports = FuncionesBusiness.SearchImportsData(Convert.ToInt32(dtProducto.Rows[0]["IdProducto"]));
                objMiProducto = new MisProductos
                {
                    CodProducto = dtProducto.Rows[0]["CodProducto"].ToString(),
                    Descripcion = dtProducto.Rows[0]["Descripcion"].ToString(),
                    PaisAduana = dtFlag.Rows[0]["PaisAduana"].ToString(),
                    ValorTotal = dtImports.Rows[0]["ValorTotal"].ToString(),
                    CantidadTotal = dtImports.Rows[0]["CantidadTotal"].ToString(),
                    PrecioUnitTotal = dtImports.Rows[0]["PrecioUnitTotal"].ToString()
                };
                DataTable dtConsolidado =
                    FuncionesBusiness.SearchConsolidateCountry(Convert.ToInt32(dtProducto.Rows[0]["IdProducto"]),
                        Convert.ToInt32(dtFlag.Rows[0]["IdPaisAduana"]));
                objProductoByPais = new ListProductByPaises()
                {
                    IdPaisAduana = Convert.ToInt32(dtConsolidado.Rows[0]["IdPaisAduana"]),
                    Importaciones = dtConsolidado.Rows[0]["Importaciones"].ToString(),
                    Importadores = Convert.ToInt32(dtConsolidado.Rows[0]["Importadores"]),
                    Exportaciones = dtConsolidado.Rows[0]["Exportaciones"].ToString(),
                    Exportadores = Convert.ToInt32(dtConsolidado.Rows[0]["Exportadores"])
                };
                //listaProducto = dtConsolidado.AsEnumerable().Select(m => new MisProductos()
                //{
                //    IdPaisAduana = m.Field<int>("IdPaisAduana"),
                //    Valor = m.Field<string>("Valor"),
                //    Cantidad = m.Field<int>("CantidadEmpresas")
                //}).ToList();
                DataTable dtValorCIF = FuncionesBusiness.SearchCifImports(Convert.ToInt32(dtProducto.Rows[0]["IdProducto"]),
                    Convert.ToInt32(dtFlag.Rows[0]["IdPaisAduana"]));
                if (dtValorCIF != null)
                {
                    dataValorCIF = GetJsonDataChar(dtValorCIF);
                }
                else
                {
                    dataValorCIF = null;
                }

                DataTable dtPrecioProm = FuncionesBusiness.SearchPrecioProm(Convert.ToInt32(dtProducto.Rows[0]["IdProducto"]),
                    Convert.ToInt32(dtFlag.Rows[0]["IdPaisAduana"]));
                if (dtPrecioProm != null)
                {
                    dataPrecioProm = GetJsonDataLine(dtPrecioProm);
                }
                else
                {
                    dataPrecioProm = null;
                }

                DataTable dtYear = FuncionesBusiness.SearchYear(Convert.ToInt32(dtProducto.Rows[0]["IdProducto"]),
                    Convert.ToInt32(dtFlag.Rows[0]["IdPaisAduana"]));
                DataTable dtCompCIF = FuncionesBusiness.SearchCompCIF(Convert.ToInt32(dtProducto.Rows[0]["IdProducto"]),
                    Convert.ToInt32(dtFlag.Rows[0]["IdPaisAduana"]));
                if (dtCompCIF != null)
                {
                    //dataCompCIF = GetJsonDataLine2(dtCompCIF);
                    chartCompCif = GetComparativoCif(dtYear, dtCompCIF);

                }
                else
                {
                    dataCompCIF = null;
                }
            }

            return Json(new
            {
                objMiProducto,
                objProductoByPais,
                listaConsolidado,
                vistaPaises = viewListConsolidate,
                charData = dataValorCIF,
                lineData = dataPrecioProm,
                //lineData2 = dataCompCIF,
                chartComparativoCif = chartCompCif
            });
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
        public ActionResult SetCulture(string culture)
        {
            culture = CultureHelper.GetImplementedCulture(culture);
            RouteData.Values["culture"] = culture; // set culture 

            return RedirectToAction("Index");
        }

        private List<object> GetJsonDataChar(DataTable dt)
        {
            var json = new List<object>();

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    json.Add(new
                    {
                        name = dr["Año"].ToString(),
                        y = Math.Round(Convert.ToDouble(Convert.ToSingle(dr["Valor"])), 1)
                    });
                }

            }
            return json;
        }
        private List<object> GetJsonDataLine(DataTable dt)
        {
            var json = new List<object>();

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    json.Add(new
                    {
                        name = dr["Mes"].ToString(),
                        y = Math.Round(Convert.ToDouble(Convert.ToSingle(dr["PrecioUnit"])), 1)
                    });
                }

            }
            return json;
        }
        private List<object> GetJsonDataLine2(DataTable dt)
        {
            var json = new List<object>();
            
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    json.Add(new
                    {
                        name = dr["Año"].ToString(),
                        data = Math.Round(Convert.ToDouble(Convert.ToSingle(dr["Valor"])), 1)
                    });
                }

            }
            return json;
        }



        private List<string> GetListStringOfDatatbleColumn( DataTable dt)
        {
            List<string> lista = new List<string>();
            foreach (DataRow dataRow in dt.Rows)
            {
                if (dataRow["Año"].ToString() == "2016")
                {
                    lista.Add(dataRow["Mes"].ToString());
                }
                else
                {
                    break;
                }
                
            }
            return lista;
        }

        private List<ChartSeriePP> GetSeries( DataTable series, DataTable dtValores)
        {
            List<ChartSeriePP> listaSeries = new List<ChartSeriePP>();

            //var auxData = dtValores.AsEnumerable().Select(x => new {anio = x["Año"]}).ToList().GroupBy(x => x.anio);

            for (int i = 0; i < series.Rows.Count; i++)
            {
                string  auxSerie = series.Rows[i]["Año"].ToString();
                List<Decimal>  lista = new List<Decimal>();
               
                for (int j = 0; j < dtValores.Rows.Count; j++)
                {

                    if (auxSerie == dtValores.Rows[j]["Año"].ToString())
                    {
                        lista.Add( Convert.ToDecimal(dtValores.Rows[j]["Valor"]));
                    }
                }

                listaSeries.Add(new ChartSeriePP()
                {
                    name = auxSerie,
                    data = lista
                });
               
            }

            return listaSeries;
        }

        private ChartPP GetComparativoCif(DataTable series, DataTable dtValores)
        {
            ChartPP objChart = new ChartPP();
            objChart.Categories = GetListStringOfDatatbleColumn(dtValores);

            List<ChartSeriePP> lista = GetSeries(series, dtValores);
            objChart.Series = lista;
            return objChart;

        }
    }
}