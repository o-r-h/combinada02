using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Veritrade2018.Helpers;
using Veritrade2018.Util;
using System.Text.RegularExpressions;
using Veritrade2018.Data;

namespace Veritrade2018.Controllers.Admin
{
    public class AdminMisAlertasFavoritasController : BaseController
    {        
        private readonly string[] _CodManifiestosModificado = new[] { "PEI", "PEE", "ECI", "ECE", "USI", "USE", "BRI", "BRE" };
        // GET: AdminMisAlertasFavoritas
        public ActionResult Index(string culture, string tipoFavorito)
        {
            if (Session["IdUsuario"] == null)
                return RedirectToAction("Logout", "Common");

            string filtro = "";
            string exclude = "";

            string codPais2 = Session["CodPais2"].ToString();
            string codPais = Session["CodPais"].ToString();
            string tipoOpe = Session["TipoOpe"].ToString();

            string codPaisAux = codPais;

            if (_CodManifiestosModificado.Contains(codPaisAux))
            {
                codPaisAux = codPaisAux.Substring(0, 2) + "_";
            }

            Session["tipoFavorito"] = tipoFavorito;

            if (tipoFavorito == "Producto" || tipoFavorito == "ProductoCompañia")
            {
                filtro = "PA";
                exclude = Enums.VarId.EXCLUDE_PAIS_MAF.GetDn();

            }
            else if (tipoFavorito == "Compañia" || tipoFavorito == "CompañiaProducto")
            {
                filtro = "IM\',\'EX";
                exclude = Enums.VarId.EXCLUDE_PAIS_MC.GetDn();
            }
            var listaPC = new List<SelectListItem>();
            var listaPC2 = new List<SelectListItem>();

            string idUsuario = Session["IdUsuario"].ToString();

            var idPlan = Funciones.ObtieneIdPlan(idUsuario);
            Session["idPlan"] = idPlan;

            string tipofiltro = tipoFavorito == "Producto" ? Enums.VarId.ALERT_MP.GetDn() :
                                    tipoFavorito == "Compañia" ? Enums.VarId.ALERT_MC.GetDn() :
                                    tipoFavorito == "ProductoCompañia" ? Enums.VarId.ALERT_PC.GetDn() : Enums.VarId.ALERT_CP.GetDn();

            var limitAlert = FuncionesBusiness.GetLimitAlert(idPlan, tipofiltro);                    

            var listaMyFavourites = FuncionesBusiness.SearchMyFavoriteAlert(culture, idUsuario,filtro, tipofiltro, codPais, tipoOpe, codPais2);

            var lista = FuncionesBusiness.SearchAllFavoriteAlert(culture, idUsuario, filtro,codPais,tipoOpe, codPais2, exclude , 
                tipoFavorito == "ProductoCompañia" || tipoFavorito == "CompañiaProducto");
            string nameBotonPC = "";
            if (tipoFavorito == "ProductoCompañia" || tipoFavorito == "CompañiaProducto")
            {
                listaPC = FuncionesBusiness.SearchMyFavoriteAlertPC(culture, filtro, idUsuario, tipofiltro,codPais,tipoOpe, codPais2);
                if(lista.Count > 0)
                    listaPC2 = FuncionesBusiness.SearchProductOrCompanyPC(culture, lista.FirstOrDefault().Value, tipofiltro);
                //var listaPC3 = lista.FirstOrDefault().Value;
            }
            var titlePerfil = "";
            switch (tipoFavorito)
            {
                case "Producto":
                    titlePerfil = @Resources.AdminResources.NavBar_Item02;
                    break;
                case "Compañia":
                    titlePerfil = Resources.AdminResources.NavBar_Item03;
                    break;
                case "ProductoCompañia":
                    titlePerfil = Resources.AdminResources.NavBar_Item04;
                    break;
                case "CompañiaProducto":
                    titlePerfil = Resources.AdminResources.NavBar_Item05;
                    break;
            }

            var alertasUtilizadas = FuncionesBusiness.CantAlertasByPaisOrRegimen(tipofiltro, idUsuario, codPais, tipoOpe);
            var othersAlert = FuncionesBusiness.CantAlertasByPais(tipofiltro, idUsuario, codPais, tipoOpe); 
            Session["ListaMyFavourites"] = listaMyFavourites;
            Session["TipoFiltro"] = tipofiltro;
            ViewData["Lista"] = lista;
            ViewData["ListaPC"] = listaPC;
            ViewData["ListaPC2"] = listaPC2;
            ViewData["TitlePerfilBar"] = titlePerfil;
            ViewData["DescPais"] = "";
            ViewData["NumFavoritos"] = (tipofiltro == "AMP" || tipofiltro == "AMC") ? listaMyFavourites.Count : listaPC.Count;
            ViewData["FlagCombo2"] = tipoFavorito.Equals("CompañiaProducto") || tipoFavorito.Equals("ProductoCompañia");
            ViewData["LimiteAlerta"] = limitAlert - alertasUtilizadas;//limitAlert - othersAlert;
            ViewData["AlertasUtilizadas"] = alertasUtilizadas;
            ViewData["LimiteAlertaTotal"] = limitAlert;
            ViewData["DescPais"] = codPais2 == "4UE" ? FuncionesBusiness.BuscarPaisUE(codPaisAux,culture) : AdminMisProductosDAO.GetNamePais(culture, codPaisAux);
            ViewData["Regimen"] = tipoOpe == "I" ? culture.Equals("es") ? "Importaciones" : "Imports" : culture.Equals("es") ? "Exportaciones" : "Exports";
            ViewBag.nombreUsuario = Funciones.BuscaUsuario(Session["IdUsuario"].ToString());

            if(alertasUtilizadas > limitAlert)
            {
                alertasUtilizadas = limitAlert;
            }
            ViewData["addAlert"] = alertasUtilizadas < limitAlert;
            ViewData["othersAlert"] = othersAlert;
            ViewData["CantidadAlertas"] = $"{Resources.AdminResources.YouHave_Text} {alertasUtilizadas} {Resources.AdminResources.AlertsOf_Text} {limitAlert}";

            ViewBag.tipoUsuario = Session["TipoUsuario"].ToString();
            ViewBag.plan = Session["idPlan"].ToString();
            ViewData["IngresoComoFreeTrial"] = Session["IngresoComoFreeTrial"] != null && (bool)Session["IngresoComoFreeTrial"];

            return View();
        }

        [HttpPost]
        public JsonResult CargarCombo2(string culture, string idPC)
        {
            var listaPC2 = new List<SelectListItem>();
            
            listaPC2 = FuncionesBusiness.SearchProductOrCompanyPC(culture, idPC, Session["TipoFiltro"].ToString());

            return Json( 
                new { listaPC2 }
                );
        }

        [HttpPost]
        public JsonResult BuscarPorDescripcion(string txtDescripcion, string culture)
        {
            string tipoFavorito = Session["tipoFavorito"].ToString();
            string idUsuario = Session["IdUsuario"].ToString();
            
            string filtro = "";
            string exclude = "";
            string codPais = Session["CodPais"].ToString();
            string codPais2 = Session["CodPais2"].ToString();
            string tipoOpe = Session["TipoOpe"].ToString();
            if (tipoFavorito == "Producto" || tipoFavorito == "ProductoCompañia")
            {
                filtro = "PA";
                exclude = Enums.VarId.EXCLUDE_PAIS_MAF.GetDn();

            }
            else if (tipoFavorito == "Compañia" || tipoFavorito == "CompañiaProducto")
            {
                filtro = "IM\',\'EX";
                exclude = Enums.VarId.EXCLUDE_PAIS_MC.GetDn();
            }
            string tipofiltro = tipoFavorito == "Producto" ? Enums.VarId.ALERT_MP.GetDn() :
                                    tipoFavorito == "Compañia" ? Enums.VarId.ALERT_MC.GetDn() :
                                    tipoFavorito == "ProductoCompañia" ? Enums.VarId.ALERT_PC.GetDn() : Enums.VarId.ALERT_CP.GetDn();

            var lista = FuncionesBusiness.SearchAllFavoriteAlert(culture, idUsuario, filtro, codPais, tipoOpe, codPais2, exclude,
               tipoFavorito == "ProductoCompañia" || tipoFavorito == "CompañiaProducto", txtDescripcion:txtDescripcion);

            var listaMyFavourites = FuncionesBusiness.SearchMyFavoriteAlert(culture, idUsuario, filtro, tipofiltro, codPais, tipoOpe, codPais2);
            Session["ListaMyFavourites"] = listaMyFavourites;
            var resultadoDescripcion = lista.Count + " " + Resources.AdminResources.Records_Text;

            var tablaVerRegistro = RenderViewToString(this.ControllerContext, "GridViews/GridViewAlertas", lista);

            if(txtDescripcion == "")
            {
                resultadoDescripcion = "";
            }

            return Json(new
            {
                //objMensaje,
                tablaVerRegistro,
                //totalPages,
                resultadoDescripcion
            });
        }


        public JsonResult BtnGuardarFavorito_Click(string culture, string idFavorito, string idValorPadre)
        {
            var filtro = "";
            string tipoFavorito = Session["tipoFavorito"].ToString();
            if (tipoFavorito == "Producto" || tipoFavorito == "ProductoCompañia")
            {
                filtro = "PA";

            }
            else if (tipoFavorito == "Compañia" || tipoFavorito == "CompañiaProducto")
            {
                filtro = "IM\',\'EX";
            }
                string codPais2 = Session["CodPais2"].ToString();
            string codPais = Session["CodPais"].ToString();
            string tipoOpe = Session["TipoOpe"].ToString();
            string codPaixAux = codPais;
            var idUsuario = Session["IdUsuario"].ToString();
            string tipofiltro = Session["TipoFiltro"].ToString();

           List<string> listaIdFavoritos = idFavorito.Split(',').ToList();
           string viewTableView = "";
           listaIdFavoritos.RemoveAt(listaIdFavoritos.Count-1);

            if(codPais2 == "4UE")
            {
                codPaixAux = "UE" + codPaixAux;
            }

            FuncionesBusiness.DeleteAlertFavoritos(tipofiltro, idUsuario, codPaixAux, tipoOpe);
            foreach (var item in listaIdFavoritos)
            {
                string[] valores = item.Split('-');
                FuncionesBusiness.GuardarAlertFavorito(tipofiltro, idUsuario, valores[0], valores[1], "NULL", valores[2]);
            }
            string mensaje = Resources.AdminResources.Message_Alerts_Succes_Saved;
            List<SelectListItem> listaMyFavourites = null;
            if (tipoFavorito == "Producto" || tipoFavorito == "Compañia")
            {
                listaMyFavourites = FuncionesBusiness.SearchMyFavoriteAlert(culture, idUsuario, filtro, tipofiltro, codPais, tipoOpe, codPais2);

            }            
            object objMensaje = null;
            objMensaje = new
            {
                titulo = "Veritrade",
                mensaje,
                flagContactenos = false
            };

            var alertasUtilizadas = FuncionesBusiness.CantAlertasByPaisOrRegimen(tipofiltro, idUsuario, codPais, tipoOpe);
            var limitAlert = FuncionesBusiness.GetLimitAlert(Session["idPlan"].ToString(), tipofiltro);

            string cantidadAlertas = $"{Resources.AdminResources.YouHave_Text} {alertasUtilizadas} {Resources.AdminResources.AlertsOf_Text} {limitAlert}";

            return Json(new{
                objMensaje,
                viewTableView,
                listaMyFavourites,
                cantidadAlertas
            });
        }
        public JsonResult BtnGuardarDetalleFavorito_Click(string culture, string idFavorito, string idValorPadre)
        {
            string[] idConcatenado = idValorPadre.Split('-');
            var idUsuario = Session["IdUsuario"].ToString();
            string tipoFavorito = Session["tipoFavorito"].ToString();
            string tipofiltro = Session["TipoFiltro"].ToString();
            var filtro = tipoFavorito.Equals("ProductoCompañia") ? "PA" : "IM\',\'EX";
            string codPais = Session["CodPais"].ToString();
            string codPais2 = Session["CodPais2"].ToString();
            string tipoOpe = Session["TipoOpe"].ToString();
            List<string> listaIdFavoritos = idFavorito.Split(',').ToList();

            listaIdFavoritos.RemoveAt(listaIdFavoritos.Count - 1);
            string[] valores = idValorPadre.Split('-');
            FuncionesBusiness.DeleteAlertFavoritosPC(tipofiltro, idUsuario, idConcatenado[1]);
            FuncionesBusiness.GuardarAlertFavorito(tipofiltro, idUsuario, valores[0], valores[1], "null" , valores[2]);
            foreach (var item in listaIdFavoritos)
            {                
                FuncionesBusiness.GuardarAlertFavorito(tipofiltro, idUsuario, valores[0], item, valores[1], valores[2]);
            }

            var listaPC = FuncionesBusiness.SearchMyFavoriteAlertPC(culture, filtro, idUsuario, tipofiltro,codPais, tipoOpe, codPais2);

            string viewTableView = RenderViewToString(this.ControllerContext,"GridViews/TableView", listaPC);
            string mensaje = Resources.AdminResources.Message_Alerts_Succes_Saved;
            object objMensaje = null;
            var cont = listaPC.Count;
            string nameBotonPC = "";
            if (cont == 0)
            {
                nameBotonPC = "Añadir Alertas";
            }
            else
            {
                nameBotonPC = "Actualizar Alertas";
            }
            objMensaje = new
            {
                titulo = "Veritrade",
                mensaje,
                flagContactenos = false
            };

            var alertasUtilizadas = FuncionesBusiness.CantAlertasByPaisOrRegimen(tipofiltro, idUsuario, codPais, tipoOpe);
            var limitAlert = FuncionesBusiness.GetLimitAlert(Session["idPlan"].ToString(), tipofiltro);

            if(alertasUtilizadas > limitAlert)
            {
                alertasUtilizadas = limitAlert;
            }

            bool limiteAlerta = alertasUtilizadas == limitAlert;

            string cantidadAlertas = $"{Resources.AdminResources.YouHave_Text} {alertasUtilizadas} {Resources.AdminResources.AlertsOf_Text} {limitAlert}";

            return Json(new {
                objMensaje,
                viewTableView,
                nameBotonPC,
                cantidadAlertas,
                limiteAlerta
            });
        }
        [HttpPost]
        public JsonResult BuscarProductoOCompania(string descripcion, string culture, string codigo)
        {
           
            //string tipoFav = (Session["tipoFavorito"].ToString() == "ProductoCompañia") ? "apc" :
             //                  (Session["tipoFavorito"].ToString() == "CompañiaProducto") ? "acp" : "";
            var json = FuncionesBusiness.SearchProductOrCompany(descripcion, culture, codigo , Session["TipoFiltro"].ToString());
            return Json(json);
        }
        [HttpPost]
        public JsonResult AgregarDetalleFavorito(string culture, string idSeleccionado, string descripcion, int numFiltrosExistentes)
        {
            int limiteFiltros = FuncionesBusiness.GetLimitAlert(Session["idPlan"].ToString(), Session["TipoFiltro"].ToString(), true);
            string tipoFavorito = Session["tipoFavorito"].ToString();
            object objMensaje = null;
            if (idSeleccionado == "")
                return Json(new

                {
                    mensaje = "No hay un elemento seleccionado"
                });

            List<object> nuevoFiltro = new List<object>();
              
            if (numFiltrosExistentes != limiteFiltros)
            {
                nuevoFiltro.Add(AgregarPalabraFiltro(culture, idSeleccionado, descripcion, tipoFavorito));
            }
            else
            {
                string mensaje = "";
                if (Session["TipoFiltro"].ToString() == "APC")
                {
                    mensaje = Resources.AdminResources.Message_Alerts_Max_Selected_03;
                }
                else if (Session["TipoFiltro"].ToString() == "ACP")
                {
                    mensaje = Resources.AdminResources.Message_Alerts_Max_Selected_04;
                }
                                 
                objMensaje = new
                {
                    titulo = "Veritrade",
                    mensaje,
                    flagContactenos = false
                };
            }

            return Json(new
            {
                objMensaje,
                nuevoFiltro
            });
        }
        [HttpPost]
        public object AgregarPalabraFiltro(string culture, string idSeleccionado, string descripcion, string tipoFavorito)
        {
            var valueOption = idSeleccionado;
            var textOption = "";

            if (tipoFavorito.Equals("ProductoCompañia"))
            {                
                textOption = culture == "es" ? "[" + "Compañía" + "] " + descripcion : "[" + "Company" + "] " + descripcion;
            }
            else
            {
                textOption = culture == "es" ? "[" + "Producto" +"] " + descripcion : "[" + "Product" + "] " + descripcion;
            }
            return new
            {
                text = textOption,
                value = valueOption
                
            };
        }
        [HttpPost]
        public JsonResult VerDetalleFavorito(string culture, string idSeleccionado)
        {
            string[] idSeleccion = idSeleccionado.Split('-');
            var idValorPadre = idSeleccion[1];
            var idUsuario = Session["IdUsuario"].ToString();
            string tipoFavorito = Session["tipoFavorito"].ToString();
            var filtro = tipoFavorito.Equals("ProductoCompañia") ? "IM\',\'EX" : "PA";
            string tipofiltro = Session["TipoFiltro"].ToString();
            string codPais2 = Session["CodPais2"].ToString();
            List<object> listDetalle = new List<object>();
            var detalleFavorito = FuncionesBusiness.SearchMyFavoriteAlerDetailtPC(culture, filtro,idUsuario, tipofiltro, idValorPadre , codPais2);

            foreach (var dF in detalleFavorito)
            {
                string[] valores = dF.Value.Split('-');
                listDetalle.Add(AgregarPalabraFiltro(culture, valores[1], dF.Text, tipoFavorito));
            }

            return Json(new
            {
                idSeleccionado,
                listDetalle
            });
        }
        [HttpPost]
        public JsonResult BtnEliminarFavoritoPC_Click(string culture, string idSeleccionado)
        {
            var idUsuario = Session["IdUsuario"].ToString();
            string tipoFavorito = Session["tipoFavorito"].ToString();
            var filtro = tipoFavorito.Equals("ProductoCompañia") ? "PA" : "IM\',\'EX";
            string tipofiltro = Session["TipoFiltro"].ToString();

            string[] idConcatenado = idSeleccionado.Split(',');

            foreach (var id in idConcatenado)
            {
                if (id != "")
                {
                    string[] idEliminar = id.Split('-');
                    FuncionesBusiness.DeleteAlertFavoritosPC(tipofiltro, idUsuario, idEliminar[1]);
                }
            }
            string codPais2 = Session["CodPais2"].ToString();
            string codPais = Session["CodPais"].ToString();
            string tipoOpe = Session["TipoOpe"].ToString();

            var listaPC = FuncionesBusiness.SearchMyFavoriteAlertPC(culture, filtro, idUsuario, tipofiltro, codPais, tipoOpe, codPais2);
            var cont = listaPC.Count;
            string viewTableView = RenderViewToString(this.ControllerContext, "GridViews/TableView", listaPC);
            string nameBotonPC = "";
            if (cont == 0)
            {
                nameBotonPC = "Añadir Alertas";
            }
            else
            {
                nameBotonPC = "Actualizar Alertas";
            }

            var alertasUtilizadas = FuncionesBusiness.CantAlertasByPaisOrRegimen(tipofiltro, idUsuario, codPais, tipoOpe);
            var limitAlert = FuncionesBusiness.GetLimitAlert(Session["idPlan"].ToString(), tipofiltro);

            if(alertasUtilizadas > limitAlert)
            {
                alertasUtilizadas = limitAlert;
            }

            string cantidadAlertas = $"{Resources.AdminResources.YouHave_Text} {alertasUtilizadas} {Resources.AdminResources.AlertsOf_Text} {limitAlert}";
            bool limiteAlerta = alertasUtilizadas == limitAlert;
            return Json(new
            {
                viewTableView,
                nameBotonPC,
                cantidadAlertas,
                limiteAlerta
            });
        }
        public JsonResult BtnEliminarDetalleFavoritoPC_Click(string culture, string idFavorito, string idSeleccionado, string descripcion)
        {
            List<string> listIdFavorito = idFavorito.Split(',').ToList();
            List<string> listIdSeleccionado = idSeleccionado.Split(',').ToList();
            List<string> listDescripcion = Regex.Split(descripcion,"/-/").ToList(); 
            listIdFavorito.RemoveAt(listIdFavorito.Count - 1);
            listIdSeleccionado.RemoveAt(listIdSeleccionado.Count-1);
            listDescripcion.RemoveAt(listDescripcion.Count - 1);
            //var idUsuario = Session["IdUsuario"].ToString();
            string tipoFavorito = Session["tipoFavorito"].ToString();
            //var filtro = tipoFavorito.Equals("ProductoCompañia") ? "PA" : "IM\',\'EX";
            //string tipofiltro = Session["TipoFiltro"].ToString();

            //string[] idPadre = idValorPadre.Split('-');
            //string[] idConcatenado = idSeleccionado.Split(',');

            //foreach (var id in idConcatenado)
            //{
            //    if (id != "")
            //    {
            //        string[] idEliminar = id.Split('-');
            //        FuncionesBusiness.DeleteAlertFavoritosDetallePC(tipofiltro, idUsuario, idEliminar[1], idPadre[1]);
            //    }
            //}
            int cont = 0;
            List<object> nuevoFiltro = new List<object>();
            string text = "";
            if (listIdFavorito.Count > 0)
            {
                foreach (var value in listIdFavorito)
                {
                    if (!listIdSeleccionado.Contains(value))
                    { 
                        text = listDescripcion[cont];
                        //nuevoFiltro.Add(AgregarPalabraFiltro(culture, fav, listDescripcion[cont], tipoFavorito));
                        nuevoFiltro.Add(new { text, value });
                    }
                    cont++;
                }                
            }
            //var listaPC = FuncionesBusiness.SearchMyFavoriteAlertPC(filtro, idUsuario, tipofiltro);

            //string viewTableView = RenderViewToString(this.ControllerContext, "GridViews/TableView", listaPC);

            return Json(new
            {
                nuevoFiltro
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
        public ActionResult SetCulture(string culture)
        {
            culture = CultureHelper.GetImplementedCulture(culture);
            RouteData.Values["culture"] = culture; // set culture 
            string tipoFavorito = Session["tipoFavorito"].ToString();
            //return RedirectToAction("Index");
            return RedirectToRoute("MisAlertasFavoritas", new { culture = culture, tipoFavorito = tipoFavorito, action = "index" });
        }
    }
}
