﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;
using Veritrade2017.Models;
using Veritrade2017.Helpers;
using Veritrade2017.Models.Admin;
using PagedList;
using Veritrade2017.Models.ProductProfile;

namespace Veritrade2017.Controllers
{
    public class ProductoPerfilController : BaseController
    {
        private readonly VeritradeAdmin.Seguridad _ws = new VeritradeAdmin.Seguridad();
        private int PageSize = 6;
        string[] arrayPais = new string[] { "AR", "BO", "PE", "PY", "UY", "CR", "PA", "MX", "BR", "CL", "CO", "DO", "GT", "HN", "SV" };
        // GET: ProductoPerfil
        [HttpGet]
        [DonutOutputCache(CacheProfile = "CacheProductProfile")]
        public ActionResult Index(string culture, string pais, string uri, string codPartida)
        {
            if (Session["c"] != null)
            {
                ViewData["CodCampaña"] = Session["c"].ToString();
            }
            else
            {
                ViewData["CodCampaña"] = culture.Equals("es") ? "12100" : "12100I";
                Session["c"] = ViewData["CodCampaña"].ToString();
            }
            
            // Ruben 202303
            // https://www.veritradecorp.com/es/chile/importaciones-y-exportaciones/barras-perfiles-y-alambre-de-estano/800300
            if (culture == "es")
                ViewBag.Canonical = "https://www.veritradecorp.com/es/" + pais + "/importaciones-y-exportaciones/" + uri + "/" + codPartida;
            else
                ViewBag.Canonical = "https://www.veritradecorp.com/en/" + pais + "/imports-and-exports/" + uri + "/" + codPartida;

            // Ruben 202307
            pais = pais.Replace("-", " ");

            ViewBag.Menu = "planes";
            ViewData["serviciosMenu"] = new Servicios().GetServicios(culture);
            ViewData["paisesMenu"] = ServiciosPaises.GetList(culture);
            int IdProducto = MisProductos.SearchProductByUri(uri, codPartida);
            
            DataTable idPais = MisProductos.SearchCountry(pais, culture); // Ruben 202403
            
            int IdPaisAduana = Convert.ToInt32(idPais.Rows[0]["IdPaisAduana"].ToString());
            if (!MisProductos.ExistProduct(IdProducto))
            {
                //return RedirectToRoute("Default");
                return RedirectToRoute("Root");
            }
            else //saving for last searches
            {
                new BuscarProducto().SaveLastSearches(uri, codPartida, IdPaisAduana, "es");
                new BuscarProducto().SaveLastSearches_English(uri, codPartida, IdPaisAduana, "en");
            }
            var uriEs = "";
            var uriEn = "";
            if (culture.Equals("es"))
            {
                uriEs = uri;
                uriEn = MisProductos.TranslateUri(uri, culture, codPartida);
            }
            else
            {
                uriEn = uri;
                uriEs = MisProductos.TranslateUri(uri, culture, codPartida);
            }
            //codigo que agregare
            MisProductos objMiProducto = null;

            //for (int i = 0; i < arrayPais.Length; i++)
            //{
            //    if (arrayPais[i].Contains(CodPaisIP) && MisProductos.ValidatePaisIP(IdProducto, i+1))
            //    {
            //        IdPaisAduana = i + 1;
            //        break;
            //    }
            //}

            ListProductByPaises objProductoByPais = null;
            DataTable regimen = MisProductos.SearchRegimen(IdProducto);
            string TipoOpe = regimen.Rows[0]["Regimen"].ToString();
            Session["Regimen"] = TipoOpe;
            DataTable dtProducto = FuncionesBusiness.SearchProductData(IdProducto, culture, codPartida);
            DataTable dtFlag = FuncionesBusiness.SearchProductFlag(IdProducto);
            DataTable dtImports = FuncionesBusiness.SearchImportsData(IdProducto, IdPaisAduana, TipoOpe);
            DataTable dtExports = FuncionesBusiness.SearchImportsData(IdProducto, IdPaisAduana, TipoOpe);
            DataTable dtPais = MisProductos.SearchNameCountry(IdPaisAduana);

            string PaisAduana = dtPais.Rows[0]["PaisAduana"].ToString(); // Ruben 202403
            string PaisAduanaEN = dtPais.Rows[0]["PaisAduanaEN"].ToString(); // Ruben 202403

            #region objMiProducto

            if (dtImports.Rows.Count > 0)
            {
                objMiProducto = new MisProductos
                {
                    IdProducto = Convert.ToInt32(dtProducto.Rows[0]["IdProducto"]),
                    CodProducto = dtProducto.Rows[0]["Partida"].ToString(),
                    Descripcion = dtProducto.Rows[0]["Descripcion"].ToString(),
                    PaisAduana = dtPais.Rows[0]["PaisAduana"].ToString(),
                    ValorTotal = Convert.ToDecimal(dtImports.Rows[0]["ValorTotal"]),
                    CantTotal = Convert.ToDecimal(dtImports.Rows[0]["CantidadTotal"]),
                    PreUnitTotal = Convert.ToDecimal(dtImports.Rows[0]["PrecioTotal"]),
                    RegimenActual = TipoOpe,
                    TotalByRegimen = Convert.ToDecimal(dtImports.Rows[0]["ValorTotal"])
                };
            }
            else
            {
                objMiProducto = new MisProductos
                {
                    IdProducto = Convert.ToInt32(dtProducto.Rows[0]["IdProducto"]),
                    CodProducto = dtProducto.Rows[0]["Partida"].ToString(),
                    Descripcion = dtProducto.Rows[0]["Descripcion"].ToString(),
                    PaisAduana = dtPais.Rows[0]["PaisAduana"].ToString(),
                    ValorTotal = 0,
                    CantTotal = 0,
                    PreUnitTotal = 0,
                    RegimenActual = TipoOpe,
                    TotalByRegimen = 0
                };
            }

            Session["NombreProducto"] = objMiProducto.Descripcion;

            #endregion

            #region listOtherProduct

            DataTable dtPartidaRelacionada = FuncionesBusiness.SearchOtherProduct(culture, objMiProducto.CodProducto);
            List<MisProductos> listOtherProduct = null;
            listOtherProduct = dtPartidaRelacionada.AsEnumerable().Select(m => new MisProductos()
            {
                CodProducto = m.Field<string>("Partida"),
                Descripcion = m.Field<string>("Descripcion").ToLower(),
                PaisAduana = m.Field<string>("PaisAduana"),
                Uri = m.Field<string>("Uri")
            }).ToList();

            #endregion

            #region objProductoByPais

            DataTable dtConsolidado =
                FuncionesBusiness.SearchConsolidateCountry(IdProducto, IdPaisAduana);

            //PRODUCTO CONSOLIDADO
            objProductoByPais = new ListProductByPaises()
            {
                IdPaisAduana = Convert.ToInt32(dtConsolidado.Rows[0]["IdPaisAduana"]),
                Importaciones = Convert.ToDecimal(dtConsolidado.Rows[0]["Importaciones"]),
                Importadores = Convert.ToInt32(dtConsolidado.Rows[0]["Importadores"]),
                Exportaciones = Convert.ToDecimal(dtConsolidado.Rows[0]["Exportaciones"]),
                Exportadores = Convert.ToInt32(dtConsolidado.Rows[0]["Exportadores"]),
                RegistrosI = Convert.ToInt32(dtConsolidado.Rows[0]["RegistrosI"]),
                RegistrosE = Convert.ToInt32(dtConsolidado.Rows[0]["RegistrosE"])
            };

            #endregion

            //LISTA PAISES
            #region listaConsolidado
            List<ListProductByPaises> listaConsolidado = null;
            DataTable dtProductByPais = FuncionesBusiness.SearchConsolidateCountries(IdProducto, TipoOpe);

            if (TipoOpe == "Importaciones")
            {
                listaConsolidado = dtProductByPais.AsEnumerable().Select(m => new ListProductByPaises()
                {
                    Regimen = TipoOpe.ToString(),
                    IdPaisAduana = m.Field<int>("IdPaisAduana"),
                    PaisAduana = m.Field<string>("PaisAduana"),
                    PaisAduanaEN = m.Field<string>("PaisAduanaEN"), //  Ruben 202404
                    AbrevPais = m.Field<string>("Abreviatura"),
                    Importaciones = m.Field<decimal>("Importaciones"),
                    Importadores = m.Field<int>("Importadores")
                }).ToList();
            }
            else
            {
                listaConsolidado = dtProductByPais.AsEnumerable().Select(m => new ListProductByPaises()
                {
                    Regimen = TipoOpe.ToString(),
                    IdPaisAduana = m.Field<int>("IdPaisAduana"),
                    PaisAduana = m.Field<string>("PaisAduana"),
                    PaisAduanaEN = m.Field<string>("PaisAduanaEN"), // Ruben 202404
                    AbrevPais = m.Field<string>("Abreviatura"),
                    Exportaciones = m.Field<decimal>("Exportaciones"),
                    Exportadores = m.Field<int>("Exportadores")
                }).ToList();
            }

            #endregion

            ViewData["TipoOpe"] = TipoOpe;
            int Registros = (TipoOpe == "Importaciones")
                ? Convert.ToInt32(objProductoByPais.RegistrosI)
                : Convert.ToInt32(objProductoByPais.RegistrosE);
            #region listDetalle
            List<Detalle> listDetalle = null;
            DataTable dtDetalle = Detalle.DataDetalle(TipoOpe.Substring(0, TipoOpe.Length - 2), pais, Registros, IdProducto, IdPaisAduana);

            listDetalle = dtDetalle.AsEnumerable().Select(m => new Detalle()
            {
                Regimen = m.Field<string>("Regimen"),
                Registro = m.Field<int>("Registros"),
                PaisProced = m.Field<string>("PaisAduana"),
                Fecha = m.Field<DateTime>("FechaNum"),
                Partida = m.Field<string>("Nandina"),
                Exportador = m.Field<string>("Proveedor"),
                Importador = m.Field<string>("Importador"),
                PesoNeto = m.Field<decimal>("PesoNeto"),
                Cantidad = m.Field<decimal>("Cantidad"),
                CifUnit = m.Field<decimal>("CifUnit"),
                FobUnit = m.Field<decimal>("FOBUnit"),
                Unidad = m.Field<string>("Unidad"),
                Dua = m.Field<string>("Dua"),
                PaisOrigen = m.Field<string>("PaisOrigen"),
                DesComercial = m.Field<string>("DesComercial")
            }).ToList();
            #endregion

            List<Detalle> listDetalleModal = null;
            DataTable dtDetalleModal = Detalle.DataDetalleModal(TipoOpe.Substring(0, TipoOpe.Length - 2), IdProducto, IdPaisAduana);

            #region listDetalleModal
            listDetalleModal = dtDetalleModal.AsEnumerable().Select(m => new Detalle()
            {
                CodProducto = dtProducto.Rows[0]["Partida"].ToString(),
                Descripcion = dtProducto.Rows[0]["Descripcion"].ToString(),
                Fecha = m.Field<DateTime>("FechaNum"),
                Partida = m.Field<string>("Nandina"),
                Exportador = m.Field<string>("Proveedor"),
                Importador = m.Field<string>("Importador"),
                PesoNeto = m.Field<decimal>("PesoNeto"),
                Cantidad = m.Field<decimal>("Cantidad"),
                CifUnit = m.Field<decimal>("CifUnit"),
                FobUnit = m.Field<decimal>("FOBUnit"),
                Unidad = m.Field<string>("Unidad"),
                Dua = m.Field<string>("Dua"),
                PaisOrigen = m.Field<string>("PaisProced"),
                DesComercial = m.Field<string>("DesComercial")
            }).ToList();
            #endregion

            //var viewTabDetalle = "";
            //viewTabDetalle = RenderViewToString(this.ControllerContext, "Partials/TableDetalle", listDetalle);
            var modelDetalle = Detalle.DataModalDetalle(IdProducto,IdPaisAduana, culture);

            ViewData["OtherProductos"] = listOtherProduct;
            ViewData["ModalDetalle"] = modelDetalle;
            ViewData["Detalle"] = listDetalle;
            ViewData["DetalleModal"] = listDetalleModal;
            ViewData["Carousel"] = listaConsolidado;
            ViewData["Producto"] = objMiProducto;
            ViewData["ImportsExports"] = objProductoByPais;

            Session["Pais"] = PaisAduana; // Ruben 202403
            Session["PaisEN"] = PaisAduanaEN; // Ruben 202403
            
            Session["IdProducto"] = IdProducto;
            Session["UriEs"] = uriEs;
            Session["UriEn"] = uriEn;
            Session["CodPartida"] = codPartida;
            ViewData["CodPartida"] = codPartida;
            Session["FlagPais"] = "flag_" + (dtPais.Rows[0]["PaisAduana"].ToString().ToLower().Replace(" ", "_")).Replace(".", "_") + ".png";
            Session["DescProducto"] = objMiProducto.CodProducto.ToUpper() + ": " + objMiProducto.Descripcion.ToUpper();
            ViewData["DescProducto"] = objMiProducto.Descripcion.ToUpper();
            ViewData["TitlePage"] = "Veritrade | Importaciones y Exportaciones de " + objMiProducto.Descripcion.ToUpper() ;
            ViewData["Description"] = "Información de comercio exterior: precios, productos, clientes, competencia, proveedores y más";
            if (culture != "es")
            {
                ViewData["TitlePage"] = "Veritrade | Imports & Exports from " + objMiProducto.Descripcion.ToUpper();
                ViewData["Description"] = "Foreign trade information: Prices, products, clients, competitors, suppliers and more.";
            }
            
            Session["Registros"] = Registros;
            ViewData["FlagPais"] = "flag_" + dtPais.Rows[0]["PaisAduana"].ToString().ToLower() + ".png";
            return View();
        }
        [HttpPost]
        public JsonResult BuscarProducto(string description, string idioma, string codPais = "", string opcion = "")
        {
            string CodPaisIP = "";

#if DEBUG

            string DireccionIP = Properties.Settings.Default.IP_Debug;
#else
            string DireccionIP = Request.ServerVariables["REMOTE_ADDR"];

#endif

            string Ubicacion = _ws.BuscaUbicacionIP2(DireccionIP, ref CodPaisIP);
            //string idioma = "es";
            var json = MisProductos.SearchProduct(description, CodPaisIP, idioma);
            return Json(json);
        }

        
        [HttpPost]
        [DonutOutputCache(CacheProfile = "CacheProductProfile")]
        public JsonResult CargarRankPais(int IdProducto, int IdPaisAduana, string tipoOpe, int pagina = 1)
        {
            string viewListRankingImp = "";
            var stringPaging = "";
            //List < ListProductByPaises > listaTabRankImp = null;
            DataTable dtTabRankImp = FuncionesBusiness.SearchTableRankImp(IdProducto, IdPaisAduana, tipoOpe);
            var listaTabRankImp = dtTabRankImp.AsEnumerable().Select(m => new ListProductByPaises()
            {
                Regimen = m.Field<string>("Regimen"),
                PaisAduana = m.Field<string>("Pais"),
                ImportsExports = m.Field<decimal>("Valor"),
                Porcentaje = m.Field<decimal>("Porcentaje")
            }).ToList().ToPagedList(pagina, PageSize);

            viewListRankingImp = RenderViewToString(this.ControllerContext, "Partials/TableRankImp", listaTabRankImp);
            if (dtTabRankImp.Rows.Count > PageSize)
            {
                stringPaging =
                    RenderViewToString(this.ControllerContext, "Partials/RankPaisPaging", listaTabRankImp);
            }
            return Json(new
            {
                vistaRankPais = viewListRankingImp,
                pagingPais = stringPaging
            });
        }

        [DonutOutputCache(CacheProfile = "CacheProductProfile")]
        public JsonResult CargarRankRegimen(int IdProducto, int IdPaisAduana, string tipoOpe, int pagina = 1)
        {
            string viewListRankingPais = "";
            var stringPaging = "";
            //List < ListProductByPaises > listaTabRankImp = null;
            DataTable dtTabRankPais = FuncionesBusiness.SearchTableRankPais(IdProducto, IdPaisAduana, tipoOpe);

            var listaTabRankPais = dtTabRankPais.AsEnumerable().Select(m => new ListProductByPaises()
            {
                Regimen = m.Field<string>("Regimen"),
                Empresa = m.Field<string>("Empresa"),
                ImportsExports = m.Field<decimal>("Valor"),
                Porcentaje = m.Field<decimal>("Porcentaje")
            }).ToList().ToPagedList(pagina, PageSize);


            viewListRankingPais = RenderViewToString(this.ControllerContext, "Partials/TableRankPais", listaTabRankPais);
            if (dtTabRankPais.Rows.Count > PageSize)
            {
                stringPaging =
                    RenderViewToString(this.ControllerContext, "Partials/RankingPaging", listaTabRankPais);
            }
            return Json(new
            {
                vistaRankRegimen = viewListRankingPais,
                pagingReg = stringPaging
            });
        }

        [HttpPost]
        public JsonResult LastSearches2(string culture/*, int cont*/)
        {
            List<BuscarProducto> lastSearches = null;
            if (culture.Equals("es"))
            {
                lastSearches = new BuscarProducto().GetLastSearches();
            }
            else
            {
                lastSearches = new BuscarProducto().GetLastSearchesEnglish();
            }
            string viewLastSearches = RenderViewToString(this.ControllerContext, "Partials/LastSearches", lastSearches);
            return Json(new
            {
                viewLastSearches
            });
        }

        [DonutOutputCache(CacheProfile = "CacheProductProfile")]
        public JsonResult BuscarClick(int IdProducto, int IdPaisAduana, string TipoOpe, string idioma)
        {
            List<MisProductos> listaProducto = null;
            //List<ListProductByPaises> listaConsolidado = null;
            var CodPartida = Session["CodPartida"].ToString();
            List<ListProductByPaises> listaTabRankImp = null;
            DataTable dtProducto = FuncionesBusiness.SearchProductData(IdProducto, idioma, CodPartida);
            List<Object> dataValorCIF = null, dataValorCIF2 = null;
            List<Object> dataPrecioProm = null, dataPrecioProm2 = null;
            List<Object> dataCompCIF = null, dataCompCIF2 = null;
            List<Object> dataRankingImp = null, dataRankingImp2 = null;
            List<Object> dataRankingPais = null, dataRankingPais2 = null;
            List<ListProductByPaises> listaConsolidado = null;
            List<ListProductByPaises> listComparative = null;
            List<ListProductByPaises> listPrecProm = null;
            DataTable dtPais = MisProductos.SearchNameCountry(IdPaisAduana);
            DataTable dtConsolidado = null;
            Session["Regimen"] = TipoOpe;
            Session["FlagPais"] = "flag_" + (dtPais.Rows[0]["PaisAduana"].ToString().ToLower().Replace(" ", "_")).Replace(".", "_") + ".png";

            ViewData["Pais"] = dtPais.ToString().ToUpper();
            Session["Pais"] = dtPais.Rows[0]["PaisAduana"].ToString().ToUpper();
            ViewData["FlagPais"] = "flag_" + dtPais.Rows[0]["PaisAduana"].ToString().ToLower() + ".png";
            
            string viewListConsolidate = "", viewRegimen = "", viewCarrusel = "", viewProductByPais = "", viewProductByPais2 = "";
            string viewListRankingImp = "";
            string viewListRankingPais = "";
            string viewTableComparative = "";
            string viewTablePrecProm = "";
            MisProductos objMiProducto = null;
            ListProductByPaises objProductoByPais = null;
            Chart chartCompCif = new Chart();
            Chart chartCompCif2 = new Chart();
            Chart chartPrecioUnit = new Chart();

            if (dtProducto != null && dtProducto.Rows.Count > 0)
            {
                //LISTA PAISES

                #region listaConsolidado

                DataTable dtProductByPais = FuncionesBusiness.SearchConsolidateCountries(IdProducto, TipoOpe);
                if (TipoOpe == "Importaciones")
                {
                    listaConsolidado = dtProductByPais.AsEnumerable().Select(m => new ListProductByPaises()
                    {
                        Regimen = TipoOpe.ToString(),
                        IdPaisAduana = m.Field<int>("IdPaisAduana"),
                        PaisAduana = m.Field<string>("PaisAduana"),
                        AbrevPais = m.Field<string>("Abreviatura"),
                        Importaciones = m.Field<decimal>("Importaciones"),
                        Importadores = m.Field<int>("Importadores")
                    }).ToList();


                }
                else
                {
                    listaConsolidado = dtProductByPais.AsEnumerable().Select(m => new ListProductByPaises()
                    {
                        Regimen = TipoOpe.ToString(),
                        IdPaisAduana = m.Field<int>("IdPaisAduana"),
                        PaisAduana = m.Field<string>("PaisAduana"),
                        AbrevPais = m.Field<string>("Abreviatura"),
                        Exportaciones = m.Field<decimal>("Exportaciones"),
                        Exportadores = m.Field<int>("Exportadores")
                    }).ToList();
                }

                viewCarrusel = RenderViewToString(this.ControllerContext, "Partials/Carousel", listaConsolidado);

                #endregion

                #region objMiProducto

                DataTable dtImports = FuncionesBusiness.SearchImportsData(IdProducto, IdPaisAduana, TipoOpe);
                DataTable dtExports = FuncionesBusiness.SearchImportsData(IdProducto, IdPaisAduana, TipoOpe);
                if (dtImports.Rows.Count > 0)
                {
                    objMiProducto = new MisProductos
                    {
                        IdProducto = Convert.ToInt32(dtProducto.Rows[0]["IdProducto"]),
                        CodProducto = dtProducto.Rows[0]["Partida"].ToString(),
                        Descripcion = dtProducto.Rows[0]["Descripcion"].ToString(),
                        PaisAduana = dtPais.Rows[0]["PaisAduana"].ToString(),
                        ValorTotal = Convert.ToDecimal(dtImports.Rows[0]["ValorTotal"]),
                        CantTotal = Convert.ToDecimal(dtImports.Rows[0]["CantidadTotal"]),
                        PreUnitTotal = Convert.ToDecimal(dtImports.Rows[0]["PrecioTotal"]),
                        RegimenActual = TipoOpe,
                        TotalByRegimen = Convert.ToDecimal(dtImports.Rows[0]["ValorTotal"])
                    };
                }
                else
                {
                    objMiProducto = new MisProductos
                    {
                        IdProducto = Convert.ToInt32(dtProducto.Rows[0]["IdProducto"]),
                        CodProducto = dtProducto.Rows[0]["Partida"].ToString(),
                        Descripcion = dtProducto.Rows[0]["Descripcion"].ToString(),
                        PaisAduana = dtPais.Rows[0]["PaisAduana"].ToString(),
                        ValorTotal = 0,
                        CantTotal = 0,
                        PreUnitTotal = 0,
                        RegimenActual = TipoOpe,
                        TotalByRegimen = 0
                    };
                }

                #endregion
                Session["DescProducto"] = objMiProducto.CodProducto.ToUpper() + ": " + objMiProducto.Descripcion.ToUpper();
                viewProductByPais = RenderViewToString(this.ControllerContext, "Partials/ProductByPais", objMiProducto);
                if (TipoOpe == "Importaciones")
                {
                    viewRegimen = RenderViewToString(this.ControllerContext, "Partials/Importaciones", objMiProducto);
                }
                else
                {
                    viewRegimen = RenderViewToString(this.ControllerContext, "Partials/Exportaciones", objMiProducto);
                }
                //Vista Parcial de Ranking Importaciones
                DataTable dtTabRankImp = FuncionesBusiness.SearchTableRankImp(IdProducto, IdPaisAduana, TipoOpe);
                listaTabRankImp = dtTabRankImp.AsEnumerable().Select(m => new ListProductByPaises()
                {
                    PaisAduana = m.Field<string>("Pais"),
                    ImportsExports = m.Field<decimal>("Valor"),
                    Porcentaje = m.Field<decimal>("Porcentaje")
                }).ToList();

                dtConsolidado =
                    FuncionesBusiness.SearchConsolidateCountry(IdProducto, IdPaisAduana);
                //PRODUCTO CONSOLIDADO
                objProductoByPais = new ListProductByPaises()
                {
                    IdPaisAduana = Convert.ToInt32(dtConsolidado.Rows[0]["IdPaisAduana"]),
                    Importaciones = Convert.ToDecimal(dtConsolidado.Rows[0]["Importaciones"]),
                    Importadores = Convert.ToInt32(dtConsolidado.Rows[0]["Importadores"]),
                    Exportaciones = Convert.ToDecimal(dtConsolidado.Rows[0]["Exportaciones"]),
                    Exportadores = Convert.ToInt32(dtConsolidado.Rows[0]["Exportadores"]),
                    RegistrosI = Convert.ToInt32(dtConsolidado.Rows[0]["RegistrosI"]),
                    RegistrosE = Convert.ToInt32(dtConsolidado.Rows[0]["RegistrosE"])
                };

                viewProductByPais2 = RenderViewToString(this.ControllerContext, "Partials/ProductByPais2", objProductoByPais);

                #region Gráficos

                // Ruben 202403
                int año = 2019;
                DataTable dtValorCIF = FuncionesBusiness.SearchCif(IdProducto, IdPaisAduana, TipoOpe, año);

                if (dtValorCIF.Rows.Count > 0)
                {
                    dataValorCIF = GetJsonDataChar(dtValorCIF);
                }

                // Ruben 202310
                int[] años = { 2019, 2020, 2021, 2022, 2023 };
                //int[] años = { 2014, 2015, 2016, 2017, 2018 };

                DataTable dtPrecioProm = FuncionesBusiness.SearchPrecioProm(IdProducto, IdPaisAduana, TipoOpe);

                if (dtPrecioProm.Rows.Count > 0)
                {
                    chartPrecioUnit = GetPrecioUnit(años, dtPrecioProm);
                }
                else
                {
                    chartPrecioUnit = null;
                }

                DataTable dtYear = FuncionesBusiness.SearchYear(IdProducto, IdPaisAduana, TipoOpe);
                DataTable dtCompCIF = FuncionesBusiness.SearchCompCIF(IdProducto, IdPaisAduana, TipoOpe);
                if (dtCompCIF.Rows.Count > 0)
                {
                    chartCompCif = GetComparativoCif(años, dtCompCIF);

                }
                else
                {
                    chartCompCif = null;
                }

                DataTable dtRanking = FuncionesBusiness.SearchRankingImp(IdProducto, IdPaisAduana, TipoOpe);
                if (dtRanking.Rows.Count > 0)
                {
                    dataRankingImp = GetJsonDataPie(dtRanking);
                }

                DataTable dtRankPais = FuncionesBusiness.SearchRankPais(IdProducto, IdPaisAduana, TipoOpe);
                if (dtRankPais.Rows.Count > 0)
                {
                    dataRankingPais = GetJsonDataPie2(dtRankPais);
                }

                #endregion

            }

            string PaisAduana = dtPais.Rows[0]["PaisAduana"].ToString();
            int Registros = (TipoOpe == "Importaciones")
                ? Convert.ToInt32(objProductoByPais.RegistrosI)
                : Convert.ToInt32(objProductoByPais.RegistrosE);

            Session["Registros"] = Registros;
            #region listDetalle

            List<Detalle> listDetalle = null;
            DataTable dtDetalle = Detalle.DataDetalle(TipoOpe.Substring(0, TipoOpe.Length - 2), PaisAduana, Registros, IdProducto, IdPaisAduana);

            listDetalle = dtDetalle.AsEnumerable().Select(m => new Detalle()
            {
                Regimen = m.Field<string>("Regimen"),
                Registro = m.Field<int>("Registros"),
                PaisProced = m.Field<string>("PaisAduana"),
                Fecha = m.Field<DateTime>("FechaNum"),
                Partida = m.Field<string>("Nandina"),
                Exportador = m.Field<string>("Proveedor"),
                Importador = m.Field<string>("Importador"),
                PesoNeto = m.Field<decimal>("PesoNeto"),
                Cantidad = m.Field<decimal>("Cantidad"),
                CifUnit = m.Field<decimal>("CifUnit"),
                FobUnit = m.Field<decimal>("FOBUnit"),
                Unidad = m.Field<string>("Unidad"),
                Dua = m.Field<string>("Dua"),
                PaisOrigen = m.Field<string>("PaisOrigen"),
                DesComercial = m.Field<string>("DesComercial")
            }).ToList();

            var viewTabDetalle = "";

            viewTabDetalle = RenderViewToString(this.ControllerContext, "Partials/TableDetalle", listDetalle);
            #endregion

            #region lisDetalleModal

            List<Detalle> listDetalleModal = null;
            DataTable dtDetalleModal = Detalle.DataDetalleModal(TipoOpe.Substring(0, TipoOpe.Length - 2), IdProducto, IdPaisAduana);

            listDetalleModal = dtDetalleModal.AsEnumerable().Select(m => new Detalle()
            {
                Regimen = m.Field<string>("regimen"),
                CodProducto = dtProducto.Rows[0]["Partida"].ToString(),
                Descripcion = dtProducto.Rows[0]["Descripcion"].ToString(),
                Fecha = m.Field<DateTime>("FechaNum"),
                Partida = m.Field<string>("Nandina"),
                Exportador = m.Field<string>("Proveedor"),
                Importador = m.Field<string>("Importador"),
                PesoNeto = m.Field<decimal>("PesoNeto"),
                Cantidad = m.Field<decimal>("Cantidad"),
                CifUnit = m.Field<decimal>("CifUnit"),
                FobUnit = m.Field<decimal>("FOBUnit"),
                CIFImptoUnit = m.Field<decimal>("CIFImptoUnit"),
                Unidad = m.Field<string>("Unidad"),
                Dua = m.Field<string>("Dua"),
                PaisOrigen = m.Field<string>("PaisProced"),
                DesComercial = m.Field<string>("DesComercial")
            }).ToList();


            var viewTabDetalleModal = "";
            viewTabDetalleModal = RenderViewToString(this.ControllerContext, "Modals/Modal_Chart", listDetalleModal);

            #endregion

            #region Table Comparative And PrecProm

            DataTable dtTableComparative = FuncionesBusiness.SearchTableCompCIF(IdProducto, IdPaisAduana, TipoOpe);
            listComparative = dtTableComparative.AsEnumerable().Select(m => new ListProductByPaises()
            {
                Año = m.Field<int>("Año"),
                CifOrPrecProm = m.Field<decimal>("Valor"),
                RegistrosI = m.Field<int>("Registros")
            }).ToList();
            viewTableComparative = RenderViewToString(this.ControllerContext,"Partials/TableComparative",listComparative);
            DataTable dtTablePrecProm = FuncionesBusiness.SearchTablePrecProm(IdProducto, IdPaisAduana, TipoOpe);

            listPrecProm = dtTablePrecProm.AsEnumerable().Select(m => new ListProductByPaises()
            {
                Año = m.Field<int>("Año"),
                CifOrPrecProm = m.Field<decimal>("PrecioUnit"),
                RegistrosI = m.Field<int>("Registros")
            }).ToList();
            viewTablePrecProm = RenderViewToString(this.ControllerContext, "Partials/TablePrecProm", listPrecProm);
            #endregion            

            DataTable dtCountRankPais = FuncionesBusiness.SearchTableRankImp(IdProducto, IdPaisAduana, TipoOpe);

            int CountRankPais = dtCountRankPais.Rows.Count;

            ViewData["TipoOpe"] = TipoOpe;
            var model = Detalle.DataModalDetalle(IdProducto, IdPaisAduana, idioma);
            string modalDetalle = RenderViewToString(this.ControllerContext,"Modals/Modal_Detail",model);
            return Json(new
            {
                CountRankPais,
                viewTableComparative,
                viewTablePrecProm,
                objMiProducto,
                viewProductByPais,
                viewProductByPais2,
                //objProductoByPais,
                modalDetalle,
                viewCarrusel,
                viewTabDetalle,
                viewTabDetalleModal,
                viewRegimen,
                charData = dataValorCIF,
                lineData = dataPrecioProm,
                //lineData2 = dataCompCIF,
                chartCompCif,
                chartCompCif2,
                pieRanking = dataRankingImp,
                pieRanking2 = dataRankingPais,
                viewListRankingImp,
                viewListRankingPais,
                chartPrecioUnit
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
        public override ActionResult SetCulture(string culture, string slug_pais_ruc_trib = "")
        {
            culture = CultureHelper.GetImplementedCulture(culture);
            RouteData.Values["culture"] = culture; // set culture 
            
            //var id = "";
            //if (Session["IdProducto"] != null)
            //    id = Session["IdProducto"].ToString();
            
            var codPartida = "";
            if (Session["CodPartida"] != null)
                codPartida = Session["CodPartida"].ToString();

            var pais = "";
            var uri = "";
            if (culture == "es")
            {
                if (Session["Pais"] != null)
                    pais = Session["Pais"].ToString().ToLower(); // Ruben 202305
                if (Session["UriEs"] != null)
                    uri = Session["UriEs"].ToString();

                return RedirectToRoute("PerfilSearch", new { culture, pais, uri, codPartida });
            }
            else
            {
                if (Session["PaisEN"] != null)
                    pais = Session["PaisEN"].ToString().ToLower(); // Ruben 202403
                if (Session["UriEn"] != null)
                    uri = Session["UriEn"].ToString();

                return RedirectToRoute("PerfilSearchEN", new { culture, pais, uri, codPartida }); // Ruben 202403
            }
            //return RedirectToAction("Index", "ProductoPerfil", new {culture,Pais, IdProducto});
        }
        #region GetDataGraficos

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
        private List<object> GetJsonDataPie(DataTable dt)
        {
            var json = new List<object>();

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    json.Add(new
                    {
                        name = dr["Empresa"].ToString(),
                        y = Math.Round(Convert.ToDouble(Convert.ToSingle(dr["Valor"])), 1)
                    });
                }

            }
            return json;
        }
        private List<object> GetJsonDataPie2(DataTable dt)
        {
            var json = new List<object>();

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    json.Add(new
                    {
                        name = dr["Pais"].ToString(),
                        y = Math.Round(Convert.ToDouble(Convert.ToSingle(dr["Valor"])), 1)
                    });
                }

            }
            return json;
        }

        #endregion
        private List<string> GetListStringOfDatatbleColumn(DataTable dt)
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
        private List<ChartSerie> GetSeries(int[] series, DataTable dtValores)
        {
            List<ChartSerie> listaSeries = new List<ChartSerie>();

            //var auxData = dtValores.AsEnumerable().Select(x => new {anio = x["Año"]}).ToList().GroupBy(x => x.anio);

            for (int i = 0; i < series.Length; i++)
            {
                string auxSerie = series[i].ToString();
                List<Decimal> lista = new List<Decimal>();

                for (int j = 0; j < dtValores.Rows.Count; j++)
                {

                    if (auxSerie == dtValores.Rows[j]["Año"].ToString())
                    {
                        lista.Add(Convert.ToDecimal(dtValores.Rows[j]["Valor"]));
                    }
                }

                listaSeries.Add(new ChartSerie()
                {
                    name = auxSerie,
                    data = lista
                });

            }

            return listaSeries;
        }
        private Chart GetComparativoCif(int[] series, DataTable dtValores)
        {
            Chart objChart = new Chart();
            objChart.Categories = GetListStringOfDatatbleColumn(dtValores);

            List<ChartSerie> lista = GetSeries(series, dtValores);
            objChart.Series = lista;
            return objChart;
        }
        private Chart GetPrecioUnit(int[] series, DataTable dtValores)
        {
            Chart objChart = new Chart();
            objChart.Categories = GetListStringOfDatatbleColumn(dtValores);

            List<ChartSerie> lista = GetSeriesPrecioUnit(series, dtValores);
            objChart.Series = lista;
            return objChart;
        }
        private List<ChartSerie> GetSeriesPrecioUnit(int[] series, DataTable dtValores)
        {
            List<ChartSerie> listaSeries = new List<ChartSerie>();

            //var auxData = dtValores.AsEnumerable().Select(x => new {anio = x["Año"]}).ToList().GroupBy(x => x.anio);

            for (int i = 0; i < series.Length; i++)
            {
                string auxSerie = series[i].ToString();
                List<Decimal> lista = new List<Decimal>();

                for (int j = 0; j < dtValores.Rows.Count; j++)
                {

                    if (auxSerie == dtValores.Rows[j]["Año"].ToString())
                    {
                        lista.Add(Convert.ToDecimal(dtValores.Rows[j]["PrecioUnit"]));
                    }
                }

                listaSeries.Add(new ChartSerie()
                {
                    name = auxSerie,
                    data = lista
                });

            }

            return listaSeries;
        }
    }
}