using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Veritrade2018.App_Start;
using Veritrade2018.Data;
using Veritrade2018.Helpers;
using Veritrade2018.Models.Admin;

namespace Veritrade2018.Controllers.Admin
{
    public class AdminMisGruposController : BaseController
    {
        private string IdGrupoBloqueado = "335159";
        // GET: AdminMisGrupos
        public ActionResult Index(string culture, string tipoFavorito)
        {
            if (Session["IdUsuario"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            Session.Remove("IDsSeleccionados");

            string idUsuario = Session["IdUsuario"].ToString();
            string codPais2 = Session["CodPais2"].ToString();
            string codPais = Session["CodPais"].ToString();
            ValidCodPais2(codPais2, ref codPais);
            string tipoOpe = Session["TipoOpe"].ToString();

            int pageInit = 1;

            int cantRegistros = AdminMyGroupsDAO.GetQuantityGroups(idUsuario, tipoOpe, codPais, tipoFavorito);
            DataTable dtGroups = AdminMyGroupsDAO.GetDataGroups(idUsuario, tipoOpe, codPais, tipoFavorito, pageInit, Funciones.fav_x_pag);
            AdminMyGroup objAdminMyGroup =
                new AdminMyGroup
                {
                    DescripcionCantidad = GetDescriptionQuantityGroups(cantRegistros, culture),
                    GruposFavoritos = GetGroups(dtGroups,pageInit),
                    TotalPaginas = GetTotalPaginas(cantRegistros)
                };
            
            Session["Idioma"] = culture;
            Session["TipoFavorito"] = tipoFavorito;
            Session["CodPais"] = codPais;
            Session["CurrentPageTableGroups"] = pageInit;

            ViewData["TipoFavorito"] = tipoFavorito;
            ViewBag.nombreUsuario = Funciones.BuscaUsuario(idUsuario);
            ViewBag.tipoUsuario = Session["TipoUsuario"].ToString();
            ViewBag.plan = Session["Plan"].ToString();
            ViewData["TitlePerfilBar"] = GetTitlePerfilBar(tipoFavorito, codPais, culture);
            ViewData["objAdminMyGroup"] = objAdminMyGroup;
            ViewData["IngresoComoFreeTrial"] = Session["IngresoComoFreeTrial"] != null && (bool)Session["IngresoComoFreeTrial"];
            return View();
        }

        #region MétodosAuxiliares
        private string GetTitlePerfilBar(string tipoFavorito, string codPais, string idioma)
        {
            string auxTipoFavorito;
            switch (tipoFavorito)
            {
                case "Partida":
                    auxTipoFavorito = Resources.AdminResources.Product_Text;
                    break;
                case "Importador":
                case "ImportadorExp":
                    auxTipoFavorito = Resources.Resources.Search_Form_Item05;
                    break;
                default:
                    if (tipoFavorito == "Proveedor" & codPais != "CL")
                    {
                        auxTipoFavorito = Resources.Resources.Search_Form_Item06;
                    }
                    else
                    {
                        auxTipoFavorito = Resources.Resources.Search_Form_BrandField;
                    }
                    break;
            }

            if (idioma == "es")
            {
                return "Mis Grupos de " +
                       (auxTipoFavorito != Resources.AdminResources.Product_Text &&
                        auxTipoFavorito != Resources.Resources.Search_Form_BrandField
                           ? auxTipoFavorito + "es"
                           : auxTipoFavorito + "s");
            }
            else
            {
                return  "My "+auxTipoFavorito + "s Groups";
            }
        }

        private void ValidCodPais2(string codPais2, ref string codPais)
        {
            if (codPais2 == "4UE")
                codPais = "UE" + codPais;
        }
        private string GetDescriptionQuantityGroups(int quantityRecords, string idioma)
        {
            if (idioma == "es")
            {
                return "Se tiene(n) " + quantityRecords + " Grupos";
            }
            else
            {
                return "There are " + quantityRecords + " Groups";
            }

        }

        private List<GrupoFavorito> GetGroups(DataTable dataGroups, int page)
        {
            int numero = (page - 1) * Funciones.fav_x_pag;
            List<GrupoFavorito> grupos = dataGroups.DataTableToList<GrupoFavorito>();
            grupos.ForEach(m => { m.Index = ++numero; });
            return grupos;
        }

        private List<Favorite> GetFavorites(DataTable dataFavorites, int page)
        {
            int numero = (page - 1) * Funciones.fav_x_pag;
            List<Favorite> favorites = dataFavorites.DataTableToList<Favorite>();
            favorites.ForEach(m => { m.Index = ++numero; });

            return favorites;
        }
        private string GetCurrentIdioma()
        {
            return Session["Idioma"] != null ? Session["Idioma"].ToString() : "es";
        }

        private bool ExistRUC(string codPais)
        {
            string[] countriesNoExistRuc = {"PY", "PEI" , "PEE", "USI", "USE"};
            return !countriesNoExistRuc.Contains(codPais);
        }
        private int GetTotalPaginas(int cantidadRegistros)
        {
            return (int)Math.Ceiling(Convert.ToDecimal(cantidadRegistros) / Funciones.fav_x_pag);
        }

        private ArrayList GuardaSeleccionados(ArrayList actualesIdsSeleccionados, string[] arrayIdsSeleccionados,
            string[] arrayIdsPagina)
        {
            bool existeSelected = true ;
            if (arrayIdsSeleccionados != null && arrayIdsSeleccionados.Length > 0)
            {
                for (var i = 0; i < arrayIdsSeleccionados.Length; i++)
                {
                    arrayIdsSeleccionados[i] =
                        arrayIdsSeleccionados[i].Substring(1, arrayIdsSeleccionados[i].Length - 1);
                }
            }
            else
            {
                existeSelected = false;
            }

            if (actualesIdsSeleccionados == null)
                actualesIdsSeleccionados = new ArrayList();

            foreach (string id in arrayIdsPagina)
            {
                var idRow = id.Substring(1, id.Length - 1);
                //if (id.Substring(0, 1) == "F" || id.Substring(0, 1) == "G")
                //{
                //    id = id.Substring(1, id.Length - 1);
                //}

                if (existeSelected && arrayIdsSeleccionados.Contains(idRow))
                {
                    if (!actualesIdsSeleccionados.Contains(idRow))
                        actualesIdsSeleccionados.Add(idRow);
                }
                else
                {
                    if (actualesIdsSeleccionados.Contains(idRow))
                        actualesIdsSeleccionados.Remove(idRow);
                }
            }
            return actualesIdsSeleccionados;
        }

        private void ActualizarSeleccionados(string idsSeleccionados, string idsPagina)
        {
            string[] arrayIdsSeleccionados = null;
            if (!string.IsNullOrEmpty(idsSeleccionados))
            {
                arrayIdsSeleccionados = idsSeleccionados.Split(',');
            }
            
            string[] arrayIdsPagina = idsPagina.Split(',');

            Session["IDsSeleccionados"] = GuardaSeleccionados((ArrayList)Session["IDsSeleccionados"],
                arrayIdsSeleccionados, arrayIdsPagina);
        }

        private void UpdateSelectedFavorites(string idsSeleccionados, string idsPagina)
        {
            string[] arrayIdsSeleccionados = null;
            if (!string.IsNullOrEmpty(idsSeleccionados))
            {
                arrayIdsSeleccionados = idsSeleccionados.Split(',');
            }

            string[] arrayIdsPagina = idsPagina.Split(',');

            Session["IDsSelectedFavorites"] = GuardaSeleccionados((ArrayList)Session["IDsSelectedFavorites"],
                arrayIdsSeleccionados, arrayIdsPagina);
        }

        private ArrayList GetIdsSelectedGroups(ArrayList idsSelecteds)
        {
            ArrayList auxIds = new ArrayList();
            foreach (string id in idsSelecteds)
            {
                auxIds.Add("G" + id);
            }
            return auxIds;
        }

        private ArrayList GetIdsSelectedFavorites(ArrayList idsSelecteds)
        {
            ArrayList auxIds = new ArrayList();
            foreach (string id in idsSelecteds)
            {
                auxIds.Add("F" + id);
            }
            return auxIds;
        }

        #endregion

        private string GetGroupsByPage(int page)
        {
            DataTable dtGroups = AdminMyGroupsDAO.GetDataGroups(Session["IdUsuario"].ToString(), Session["TipoOpe"].ToString(),
                Session["CodPais"].ToString(), Session["TipoFavorito"].ToString(), page, Funciones.fav_x_pag);
;
            Session["CurrentPageTableGroups"] = page;

            AdminMyGroup objAdminMyGroup = new AdminMyGroup
            {
                GruposFavoritos = GetGroups(dtGroups, page),
                IdsSeleccionados = Session["IDsSeleccionados"] != null
                    ? GetIdsSelectedGroups((ArrayList) Session["IDsSeleccionados"])
                    : null
            };

            return RenderViewToString(this.ControllerContext, "GridViews/TableRowView", objAdminMyGroup);

        }
        [SessionExpireFilter]
        [HttpPost]
        public JsonResult PageIndexChanging(int pagina, string idsSeleccionados, string idsPagina)
        {
            if (!string.IsNullOrEmpty(idsPagina))
            {
                ActualizarSeleccionados(idsSeleccionados, idsPagina);
            }

            Session["CurrentPageTableGroups"] = pagina;
            string rowsGroups = GetGroupsByPage(pagina);
            
            return Json(new
            {
                rowsGroups
            });
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult UpdateGroup(string idGroup, string textGroup)
        {
            idGroup = idGroup.Substring(1, idGroup.Length - 1);
            AdminMyGroupsDAO.UpdateGroup(idGroup, textGroup.Trim());

            int currentPageTableGroups = Convert.ToInt32(Session["CurrentPageTableGroups"]);

            Session.Remove("IDsSeleccionados");

            string rowsGroups = GetGroupsByPage(currentPageTableGroups);

            return Json(new
            {
                rowsGroups
            });
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult DeleteGroups(string idsSeleccionados, string idsPagina)
        {
            if (!string.IsNullOrEmpty(idsPagina))
            {
                ActualizarSeleccionados(idsSeleccionados, idsPagina);
            }

            ArrayList currentIdsSeleccionados = new ArrayList();
            if (Session["IDsSeleccionados"] != null)
                currentIdsSeleccionados = (ArrayList)Session["IDsSeleccionados"];

            object objMensaje = null;
            AdminMyGroup objAdminMyGroup = null;
            if (currentIdsSeleccionados.Count <= 0)
            {
                objMensaje = new
                {
                    titulo = Resources.MiPerfil.DeleteSelectedGroups_Text,
                    mensaje = Resources.MiPerfil.SelectAtLeastOneGroupToDelete_Text,
                    flagContactenos = false
                };
            }
            else
            {
                AdminMyGroupsDAO.DeleteGroups(currentIdsSeleccionados);

                Session.Remove("IDsSeleccionados");

                int pageInit = 1;
                string idUsuario = Session["IdUsuario"].ToString();
                string codPais = Session["CodPais"].ToString();
                string tipoOpe = Session["TipoOpe"].ToString();
                string tipoFavorito = Session["TipoFavorito"].ToString();

                Session["CurrentPageTableGroups"] = pageInit;

                int cantRegistros = AdminMyGroupsDAO.GetQuantityGroups(idUsuario, tipoOpe, codPais, tipoFavorito);
                DataTable dtGroups = AdminMyGroupsDAO.GetDataGroups(idUsuario, tipoOpe, codPais, tipoFavorito, pageInit, Funciones.fav_x_pag);

                objAdminMyGroup = new AdminMyGroup
                    {
                        DescripcionCantidad = GetDescriptionQuantityGroups(cantRegistros, GetCurrentIdioma()),
                        GruposFavoritos = GetGroups(dtGroups, pageInit),
                        TotalPaginas = GetTotalPaginas(cantRegistros)
                    };

                objAdminMyGroup.GroupsFavoritesInHtml = RenderViewToString(this.ControllerContext,"GridViews/TableRowView", objAdminMyGroup);
            }

            return Json(new
            {
                objMensaje,
                objAdminMyGroup
            });
        }

        #region ShowFavorites

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult GetFavoritesByGroup(string idGrupo)
        {
            Session.Remove("IDsSelectedFavorites");

            string tipoFavorito = Session["TipoFavorito"].ToString();
            string idioma = GetCurrentIdioma();
            string codPais = Session["CodPais"].ToString();

            if (codPais.Contains("UE"))
            {
                codPais = "UE";
            }

            idGrupo = idGrupo.Substring(1, idGrupo.Length-1);

            int pagina = 1;

            int countFavoritesByGroup = AdminMyGroupsDAO.GetQuantityFavoritesByGroup(idGrupo, codPais, tipoFavorito);
            DataTable dataFavorites = AdminMyGroupsDAO.GetDataFavoritesByGroup(idGrupo, codPais, tipoFavorito, ExistRUC(codPais), idioma, pagina, Funciones.fav_x_pag);

            FavoriteByGroup objFavoriteByGroup = new FavoriteByGroup
            {
                IsVisibleDelete = idGrupo != IdGrupoBloqueado,
                FavoriteHead = GetFavoriteHead(tipoFavorito, codPais),
                Favorites = GetFavorites(dataFavorites,pagina),
                TotalPaginas = GetTotalPaginas(countFavoritesByGroup)
            };

            Session["CurrentIdGrupo"] = idGrupo;

            objFavoriteByGroup.FavoritesByGroupInHtml = RenderViewToString(this.ControllerContext,
                "GridViews/FavoritesByGroupTableView",
                objFavoriteByGroup);
            
            return Json(new
            {
                objFavoriteByGroup
            });
        }

        private FavoriteHead GetFavoriteHead(string tipoFavorito, string codPais)
        {
            string auxTipoFavorito = tipoFavorito.Substring(0, 2).ToUpper();

            if (tipoFavorito == "ImportadorExp")
                auxTipoFavorito = "IE";

            FavoriteHead objFavoriteHead = new FavoriteHead();
            switch (auxTipoFavorito)
            {
                case "PA":
                    objFavoriteHead.IsVisibleRuc = false;
                    break;
                case "IM":
                case "EX":
                    objFavoriteHead.IsVisibleNandina = false;
                    objFavoriteHead.IsVisibleRuc = ExistRUC(codPais);
                    break;
                case "PR":
                case "IE":
                case "DI":
                    objFavoriteHead.IsVisibleNandina = false;
                    objFavoriteHead.IsVisibleRuc = false;
                    break;
            }

            return objFavoriteHead;
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult PageIndexChangingFavoritesByGroup(int pagina, string idsSeleccionados, string idsPagina)
        {
            if (!string.IsNullOrEmpty(idsPagina))
            {
                UpdateSelectedFavorites(idsSeleccionados, idsPagina);
            }

            string tipoFavorito = Session["TipoFavorito"].ToString();
            string idioma = GetCurrentIdioma();
            string codPais = Session["CodPais"].ToString();

            string idGrupo = Session["CurrentIdGrupo"].ToString();

            DataTable dataFavorites = AdminMyGroupsDAO.GetDataFavoritesByGroup(idGrupo, codPais, tipoFavorito, ExistRUC(codPais), idioma, pagina, Funciones.fav_x_pag);

            FavoriteByGroup objFavoriteByGroup = new FavoriteByGroup
            {
                FavoriteHead = GetFavoriteHead(tipoFavorito, codPais),
                Favorites = GetFavorites(dataFavorites, pagina),
                IdsSeleccionados = Session["IDsSelectedFavorites"] != null ? GetIdsSelectedFavorites((ArrayList)Session["IDsSelectedFavorites"]) : null
            };

            string rowsFavoritesByGroup = RenderViewToString(this.ControllerContext, "GridViews/FavoritesByGroupTableRowView",
                objFavoriteByGroup);

            return Json(new
            {
                rowsFavoritesByGroup
            });
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult DeleteFavorites(string idsSeleccionados, string idsPagina)
        {
            if (!string.IsNullOrEmpty(idsPagina))
            {
                UpdateSelectedFavorites(idsSeleccionados, idsPagina);
            }

            ArrayList currentIdsSeleccionados = new ArrayList();
            if (Session["IDsSelectedFavorites"] != null)
                currentIdsSeleccionados = (ArrayList)Session["IDsSelectedFavorites"];

            object objMensaje = null;
            FavoriteByGroup objFavoriteByGroup = null;
            string rowsGroups = "";
            if (currentIdsSeleccionados.Count <= 0)
            {
                objMensaje = new
                {
                    titulo = Resources.MiPerfil.DeleteFavorites_Text,
                    mensaje = Resources.MiPerfil.SelectAtLeastOneFavoriteToDelete_Text,
                    flagContactenos = false
                };
            }else if (currentIdsSeleccionados.Count ==
                      FuncionesBusiness.CantFavoritosGrupo(Session["CurrentIdGrupo"].ToString()))
            {
                objMensaje = new
                {
                    titulo = Resources.MiPerfil.DeleteFavorites_Text,
                    mensaje = Resources.MiPerfil.Msg_CouldNotDeleteAllSelectedFavorites,
                    flagContactenos = false
                };
            }
            else
            {
                string idGrupo = Session["CurrentIdGrupo"].ToString();
                AdminMyGroupsDAO.DeleteFavoritesOfGroup((ArrayList)Session["IDsSelectedFavorites"],idGrupo);

                string tipoFavorito = Session["TipoFavorito"].ToString();
                string idioma = GetCurrentIdioma();
                string codPais = Session["CodPais"].ToString();

                int pagina = 1;

                int countFavoritesByGroup = AdminMyGroupsDAO.GetQuantityFavoritesByGroup(idGrupo, codPais, tipoFavorito);
                DataTable dataFavorites = AdminMyGroupsDAO.GetDataFavoritesByGroup(idGrupo, codPais, tipoFavorito, ExistRUC(codPais), idioma, pagina, Funciones.fav_x_pag);

                objFavoriteByGroup = new FavoriteByGroup
                {
                    IsVisibleDelete = idGrupo != IdGrupoBloqueado,
                    FavoriteHead = GetFavoriteHead(tipoFavorito, codPais),
                    Favorites = GetFavorites(dataFavorites, pagina),
                    TotalPaginas = GetTotalPaginas(countFavoritesByGroup)
                };
                
                objFavoriteByGroup.FavoritesByGroupInHtml = RenderViewToString(this.ControllerContext,"GridViews/FavoritesByGroupTableView",objFavoriteByGroup);

                Session.Remove("IDsSelectedFavorites");

                int currentPageTableGroups = Convert.ToInt32(Session["CurrentPageTableGroups"]);

                Session.Remove("IDsSeleccionados");

                rowsGroups = GetGroupsByPage(currentPageTableGroups);
            }

            return Json(new
            {
                objMensaje,
                objFavoriteByGroup,
                rowsGroups
            });
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
