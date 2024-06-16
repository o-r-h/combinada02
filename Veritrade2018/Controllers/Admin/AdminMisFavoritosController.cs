using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using Veritrade2018.App_Start;
using Veritrade2018.Data;
using Veritrade2018.Helpers;
using Veritrade2018.Models.Admin;

namespace Veritrade2018.Controllers.Admin
{
    public class AdminMisFavoritosController : BaseController
    {
        private readonly string[] _CodManifiestosModificado = new[] { "PEI", "PEE", "ECI", "ECE", "USI", "USE", "BRI", "BRE" };
        //Get: AdminMisFavoritos
        public ActionResult Index(string culture, string tipoFavorito)
        {
            if (Session["IdUsuario"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            Session.Remove("IDsSeleccionados");
            string idUsuario = Session["IdUsuario"].ToString();
            string codPais = Session["CodPais"].ToString();
            string tipoOpe = Session["TipoOpe"].ToString();

            string codPaisAux = codPais;

            if (_CodManifiestosModificado.Contains(codPaisAux))
            {
                codPaisAux = codPaisAux.Substring(0, 2) + "_";
            }

            if (codPais == "US_" || codPais == "PE_" || codPais == "EC_" || codPais == "BR_")
                codPais = codPais.Substring(0, 2) + tipoOpe;

            SetSessionGruposFavorito(idUsuario, tipoOpe, codPais, tipoFavorito, culture);

            string textSeachCompany = "";
            string textRuc = "";
            int indexCboGruposF = -1;
            
            Session["Idioma"] = culture;
            Session["CodPaisAux"] = codPais;
            Session["TipoOpe"] = tipoOpe;
            Session["TipoFavorito"] = tipoFavorito;
            Session["IndexCboGruposF"] = indexCboGruposF;
            Session["ValueCboGruposF"] = "";
            Session["TextCboGruposF"] = "";
            Session["TextCompany"] = "";

            int pageInit = 1;

            int cantRegistros = AdminMyFavoriteDAO.GetCantidadFavoritosUnicos(idUsuario, tipoOpe, codPais, tipoFavorito,
                textSeachCompany, textRuc, indexCboGruposF, "");

            DataTable dtFavoritosUnicos = AdminMyFavoriteDAO.GetDataFavoritosUnicos(idUsuario, tipoOpe, codPais,
                tipoFavorito, textSeachCompany, textRuc, indexCboGruposF, "", pageInit, Funciones.fav_x_pag);

            AdminMyFavorite objAdminMyFavorite =
                new AdminMyFavorite
                {
                    OptionFavoriteTable = GetOptionFavoriteTable(tipoFavorito, codPais),
                    FavoriteUniqueHead = GetFavoriteUniqueHead(tipoFavorito, codPais),
                    GruposFavoritos = GetGruposFavoritos(true, idUsuario, codPais, tipoOpe, tipoFavorito, culture)
                };
            objAdminMyFavorite.FavoriteUniqueHead.EnabledCboGroups = objAdminMyFavorite.GruposFavoritos.Any();
            objAdminMyFavorite.SoloGruposFavoritos = GetGruposFavoritos(false, idUsuario, codPais, tipoOpe, tipoFavorito, culture);
            objAdminMyFavorite.EnabledCboGroupsForm = objAdminMyFavorite.SoloGruposFavoritos.Any();
            objAdminMyFavorite.FavoritesUniques = GetListFavoriteUnique(dtFavoritosUnicos, pageInit, indexCboGruposF, "");
            objAdminMyFavorite.DescripcionCantidad = GetQuantityFavoritesUniques(textSeachCompany, textRuc,
                indexCboGruposF, objAdminMyFavorite.FavoriteUniqueHead.FavoriteDescription, cantRegistros);
            objAdminMyFavorite.TotalPaginas = GetTotalPaginas(cantRegistros);
            objAdminMyFavorite.TipoFavorito = tipoFavorito;

            Session["CurrentPageTableFavorites"] = pageInit;
            ViewBag.nombreUsuario = Funciones.BuscaUsuario(idUsuario);
            ViewBag.tipoUsuario = Session["TipoUsuario"].ToString();
            ViewBag.plan = Session["Plan"].ToString();
            ViewData["TitlePerfilBar"] = GetTitlePerfilBar(tipoFavorito, codPais, culture);

            ViewData["objAdminMyFavorite"] = objAdminMyFavorite;

            ViewData["DescPais"] = AdminMyFavoriteDAO.GetNamePais(culture, codPaisAux);
            ViewData["Regimen"] = tipoOpe == "I" ? culture.Equals("es") ? "Importaciones" : "Imports" : culture.Equals("es") ? "Exportaciones" : "Exports";
            ViewData["IngresoComoFreeTrial"] = Session["IngresoComoFreeTrial"] != null && (bool)Session["IngresoComoFreeTrial"];
            return View();
        }

        #region MétodosAuxiliares

        private OptionFavoriteTable GetOptionFavoriteTable(string tipoFavorito, string codPais)
        {
            OptionFavoriteTable optionFavoriteTable = new OptionFavoriteTable();

            string auxTipoFavorito = tipoFavorito == "Importador" || tipoFavorito == "ImportadorExp"
                ? Resources.Resources.Search_Form_Item05
                : (tipoFavorito == "Proveedor" && codPais == "CL"
                    ? Resources.Resources.Search_Form_BrandField
                    : Resources.Resources.Search_Form_Item06);

            optionFavoriteTable.AddFavoriteBtnLabel = auxTipoFavorito != Resources.Resources.Search_Form_BrandField
                ? Resources.MiPerfil.Add_new_Text + " " + auxTipoFavorito
                : Resources.MiPerfil.Add_newBrand_Text;

            return optionFavoriteTable;
        }

        private string GetTitlePerfilBar(string tipoFavorito, string codPais, string idioma)
        {

            string auxTipoFavorito = tipoFavorito == "Importador" || tipoFavorito == "ImportadorExp"
                ? Resources.Resources.Search_Form_Item05
                : (tipoFavorito == "Proveedor" && codPais == "CL"
                    ? Resources.Resources.Search_Form_BrandField
                    : Resources.Resources.Search_Form_Item06);

            if (idioma == "es")
            {
                return auxTipoFavorito != Resources.Resources.Search_Form_BrandField
                    ? "Mis " + auxTipoFavorito + "es"
                    : Resources.AdminResources.Filter_MyBrands_Button;
            }
            else
            {
                return "My " + auxTipoFavorito + "s";
            }
        }

        private bool IsVisibleRuc(string codPais, string tipoFavorito)
        {
            string[] coutriesNoRuc = { "PY", "PEI", "PEE", "USI", "USE" };
            return !coutriesNoRuc.Contains(codPais) && (tipoFavorito == "Importador" || tipoFavorito == "Exportador");
        }

        private string GetFavoriteDescription(string codPais, string tipoFavorito)
        {
            return (tipoFavorito == "Importador" || tipoFavorito == "ImportadorExp")
                ? Resources.Resources.Search_Form_Item05
                : (tipoFavorito == "Proveedor" && codPais == "CL"
                    ? Resources.Resources.Search_Form_BrandField
                    : Resources.Resources.Search_Form_Item06);
        }

        private FavoriteUniqueHead GetFavoriteUniqueHead(string tipoFavorito, string codPais)
        {
            FavoriteUniqueHead objFavoriteUniqueHead = new FavoriteUniqueHead
            {
                FavoriteDescription = GetFavoriteDescription(codPais, tipoFavorito),
                FavoriteRuc = codPais == "PE" ? "R.U.C" : "Tax Id",
                IsVisibleRuc = IsVisibleRuc(codPais,tipoFavorito)
                
            };
            return objFavoriteUniqueHead;
        }
        private int GetTotalPaginas(int cantidadRegistros)
        {
            return (int)Math.Ceiling(Convert.ToDecimal(cantidadRegistros) / Funciones.fav_x_pag);
        }

        private List<SelectListItem> GetGruposFavoritos(bool flagAllGroups, string idUsuario, string codPais, string tipoOpe,
            string tipoFavorito, string idioma)
        {
            DataTable dtCboGruposFavoritos = FuncionesBusiness.LlenaGrupos(flagAllGroups, idUsuario, codPais, tipoOpe, tipoFavorito, idioma);

            List<SelectListItem> listGruposFavoritos = new List<SelectListItem>();
            if (dtCboGruposFavoritos != null && dtCboGruposFavoritos.Rows.Count > 0)
            {
                listGruposFavoritos = dtCboGruposFavoritos.AsEnumerable().Select(m => new SelectListItem()
                {
                    Value = Convert.ToString(m.Field<Int32>("IdGrupo")),
                    Text = m.Field<string>("Grupo")
                }).ToList();
            }
            return listGruposFavoritos;
        }

        private void SetSessionGruposFavorito(string idUsuario, string tipoOpe, string codPais,
            string tipoFavorito, string idioma)
        {
            DataTable dtGruposFavoritos = AdminMyFavoriteDAO.GetDataGruposFavoritos(idUsuario, tipoOpe, codPais, tipoFavorito, idioma);

            List<GrupoFavorito> listaGruposFavoritos = dtGruposFavoritos.AsEnumerable().Select(m => new GrupoFavorito()
            {
                IdFavorito = Convert.ToString(m.Field<Int32>("IdFavorito")),
                Orden = Convert.ToString(m.Field<Int32>("IdFavorito")),
                IdGrupo = Convert.ToString(m.Field<Int32>("IdGrupo")),
                Grupo = m.Field<string>("Grupo")
            }).ToList();

            Session["GruposFavoritos"] = listaGruposFavoritos;
        }

        private List<FavoriteUnique> GetListFavoriteUnique(DataTable dtFavoritosUnicos, int pagina, int indexCboGruposF,
            string textCboGrupoF)
        {
            List<FavoriteUnique> listFavoritesUniques = new List<FavoriteUnique>();
            if (dtFavoritosUnicos != null){
                int numero = (pagina - 1) * Funciones.fav_x_pag;
                List<GrupoFavorito> listaGruposFavoritos = (List<GrupoFavorito>)Session["GruposFavoritos"];

                bool flagSelectedGrupo = indexCboGruposF > 0;

                listFavoritesUniques = dtFavoritosUnicos.DataTableToList<FavoriteUnique>();

                listFavoritesUniques.ForEach(m =>
                {
                    m.Index = ++numero;
                    m.GroupsFavories = listaGruposFavoritos.Where(
                        x => flagSelectedGrupo
                            ? x.IdFavorito == m.IdFavorito && x.Grupo == textCboGrupoF
                            : x.IdFavorito == m.IdFavorito
                    ).ToList();
                });
            }
            return listFavoritesUniques;
        }

        private string GetQuantityFavoritesUniques(string textCompany, string textRUC, int indexCboGrupos,
            string textTipoFavorito, int cantidadRegistros)
        {
            var idUsuario = Session["IdUsuario"].ToString();
            var codPais = Session["CodPaisAux"].ToString();
            var tipoOpe = Session["TipoOpe"].ToString();
            var tipoFavorito = Session["TipoFavorito"].ToString();

            var idioma = GetCurrentIdioma();

            string descQuantity;
            if (idioma == "es")
            {
                descQuantity = "Se tiene(n) " + FuncionesBusiness.CantFavUnicos(idUsuario, codPais, tipoOpe, tipoFavorito) + " " + (textTipoFavorito != "Marca" ? textTipoFavorito + "(es)": "Marca(s)");
            }
            else
            {
                descQuantity = "There are " + FuncionesBusiness.CantFavUnicos(idUsuario, codPais, tipoOpe, tipoFavorito) + " " + textTipoFavorito + "s";
            }

            if (textCompany != "" || textRUC != "" || indexCboGrupos > 0)
            {
                if (idioma == "es")
                {
                    descQuantity += " | " + cantidadRegistros + " Filtrado(s)";
                }
                else
                {
                    descQuantity += " | " + cantidadRegistros + " Filtered";
                }
            }

            return descQuantity;
        }

        private string GetCurrentIdioma()
        {
            return Session["Idioma"] != null ? Session["Idioma"].ToString() : "es";
        }

        private ArrayList GuardaSeleccionados(ArrayList idsSeleccionados, string[] arrayIdsSeleccionados,
            string[] arrayIdsPagina)
        {
            if (idsSeleccionados == null)
                idsSeleccionados = new ArrayList();

            foreach (var id in arrayIdsPagina)
            {
                if (arrayIdsSeleccionados.Contains(id))
                {
                    if (!idsSeleccionados.Contains(id))
                        idsSeleccionados.Add(id);
                }
                else
                {
                    if (idsSeleccionados.Contains(id))
                        idsSeleccionados.Remove(id);
                }
            }
            return idsSeleccionados;
        }

        private void ActualizarSeleccionados(string idsSeleccionados, string idsPagina)
        {
            string[] arrayIdsSeleccionados = idsSeleccionados.Split(',');
            string[] arrayIdsPagina = idsPagina.Split(',');

            Session["IDsSeleccionados"] = GuardaSeleccionados((ArrayList)Session["IDsSeleccionados"],
                arrayIdsSeleccionados, arrayIdsPagina);

        }

        #endregion

        private string GetRowsFavoritesByPage(int page)
        {
            string codPais = Session["CodPaisAux"].ToString();
            string tipoFavorito = Session["TipoFavorito"].ToString();
            int indexCboGruposF = Convert.ToInt32(Session["IndexCboGruposF"]);
            string valueCboGruposF = Session["ValueCboGruposF"].ToString();
            string textCompany= Session["TextCompany"].ToString();

            
            DataTable dtFavoritosUnicos = AdminMyFavoriteDAO.GetDataFavoritosUnicos(Session["IdUsuario"].ToString(), Session["TipoOpe"].ToString(), codPais,
                tipoFavorito, textCompany, "", indexCboGruposF, valueCboGruposF, page, Funciones.fav_x_pag);

            List<FavoriteUnique> favoritesUniques = GetListFavoriteUnique(dtFavoritosUnicos, page, indexCboGruposF,
                Session["TextCboGruposF"].ToString());

            FavoriteUniqueHead favoriteUniqueHead =
                new FavoriteUniqueHead {IsVisibleRuc = IsVisibleRuc(codPais, tipoFavorito)};
            AdminMyFavorite objAdminMyFavorite = new AdminMyFavorite
            {
                FavoriteUniqueHead = favoriteUniqueHead,
                FavoritesUniques = favoritesUniques,
                IdsSeleccionados = Session["IDsSeleccionados"] != null ? (ArrayList)Session["IDsSeleccionados"] : null
            };
            
            Session["CurrentPageTableFavorites"] = page;

            return RenderViewToString(this.ControllerContext, "GridViews/TableRowView", objAdminMyFavorite);
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult PageIndexChanging(int pagina, string idsSeleccionados, string idsPagina)
        {
            if (!string.IsNullOrEmpty(idsPagina))
            {
                ActualizarSeleccionados(idsSeleccionados, idsPagina);
            }

            Session["CurrentPageTableFavorites"] = pagina;
            string rowsFavorites = GetRowsFavoritesByPage(pagina);
            return Json(new
            {
                rowsFavorites
            });
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult SearchByCompany(string textCompany)
        {
            int indexCboGruposF = Convert.ToInt32(Session["IndexCboGruposF"]);
            string valueCboGruposF = "";
            if (indexCboGruposF > 1)
                valueCboGruposF = Session["ValueCboGruposF"].ToString();
            
            string idUsuario = Session["IdUsuario"].ToString();
            string tipoOpe = Session["TipoOpe"].ToString();
            string codPais = Session["CodPaisAux"].ToString();
            string tipoFavorito = Session["TipoFavorito"].ToString();

            textCompany = textCompany.Trim();
            int cantRegistros = AdminMyFavoriteDAO.GetCantidadFavoritosUnicos(idUsuario, tipoOpe, codPais, tipoFavorito,
                textCompany, "", indexCboGruposF, valueCboGruposF);

            DataTable dtFavoritosUnicos = AdminMyFavoriteDAO.GetDataFavoritosUnicos(idUsuario, tipoOpe, codPais,
                tipoFavorito, textCompany, "", indexCboGruposF, valueCboGruposF, 1, Funciones.fav_x_pag);

            FavoriteUniqueHead objFavoriteUniqueHead =
                new FavoriteUniqueHead {IsVisibleRuc = IsVisibleRuc(codPais, tipoFavorito)};

            AdminMyFavorite objAdminMyFavorite = new AdminMyFavorite
            {
                DescripcionCantidad = GetQuantityFavoritesUniques(textCompany, "",
                    indexCboGruposF, GetFavoriteDescription(codPais, tipoFavorito), cantRegistros),
                FavoritesUniques = GetListFavoriteUnique(dtFavoritosUnicos, 1, indexCboGruposF,
                    Session["TextCboGruposF"].ToString()),
                TotalPaginas = GetTotalPaginas(cantRegistros),
                FavoriteUniqueHead = objFavoriteUniqueHead
            };

            objAdminMyFavorite.RowsFavoritesUniquesInHtml = RenderViewToString(this.ControllerContext, "GridViews/TableRowView", objAdminMyFavorite);

            Session["TextCompany"] = textCompany;

            return Json(new
            {
                objAdminMyFavorite
            });
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult GruposFavoritosChange(int indexCboGruposF, string valueCboGruposF, string textCboGruposF)
        {
            Session["IndexCboGruposF"] = indexCboGruposF;
            Session["ValueCboGruposF"] = valueCboGruposF;
            Session["TextCboGruposF"] = textCboGruposF;

            string textCompany= Session["TextCompany"].ToString();
            
            string idUsuario = Session["IdUsuario"].ToString();
            string tipoOpe = Session["TipoOpe"].ToString();
            string codPais = Session["CodPaisAux"].ToString();
            string tipoFavorito = Session["TipoFavorito"].ToString();

            int cantRegistros = AdminMyFavoriteDAO.GetCantidadFavoritosUnicos(idUsuario, tipoOpe, codPais, tipoFavorito,
                textCompany, "", indexCboGruposF, valueCboGruposF);

            DataTable dtFavoritosUnicos = AdminMyFavoriteDAO.GetDataFavoritosUnicos(idUsuario, tipoOpe, codPais,
                tipoFavorito, textCompany, "", indexCboGruposF, valueCboGruposF, 1, Funciones.fav_x_pag);

            FavoriteUniqueHead objFavoriteUniqueHead =
                new FavoriteUniqueHead { IsVisibleRuc = IsVisibleRuc(codPais, tipoFavorito) };

            AdminMyFavorite objAdminMyFavorite = new AdminMyFavorite
            {
                DescripcionCantidad = GetQuantityFavoritesUniques(textCompany, "",
                    indexCboGruposF, GetFavoriteDescription(codPais, tipoFavorito), cantRegistros),
                FavoritesUniques = GetListFavoriteUnique(dtFavoritosUnicos, 1, indexCboGruposF,textCboGruposF),
                TotalPaginas = GetTotalPaginas(cantRegistros),
                FavoriteUniqueHead = objFavoriteUniqueHead
            };

            objAdminMyFavorite.RowsFavoritesUniquesInHtml = RenderViewToString(this.ControllerContext, "GridViews/TableRowView", objAdminMyFavorite);
            
            return Json(new
            {
                objAdminMyFavorite
            });
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult AddToGroup(string idsSeleccionados, string idsPagina)
        {
            if (!string.IsNullOrEmpty(idsPagina))
            {
                ActualizarSeleccionados(idsSeleccionados, idsPagina);
            }

            ArrayList listIdsSeleccionados = new ArrayList();
            if (Session["IDsSeleccionados"] != null)
                listIdsSeleccionados = (ArrayList)Session["IDsSeleccionados"];

            object objMensaje = null;
            if (listIdsSeleccionados.Count <= 0)
            {
                objMensaje = new
                {
                    titulo = Resources.AdminResources.AddToGroup_Text,
                    mensaje = Resources.MiPerfil.SelectAtLeastOneFavorite_Text,
                    flagContactenos = false
                };
            }

            return Json(new
            {
                objMensaje
            });
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult UpdateGroup(bool isCheckedCreateGroup, string textNewGroup, string codGrupo)
        {
            string idUsuario = Session["IdUsuario"].ToString();
            string tipoOpe = Session["TipoOpe"].ToString();
            string codPais = Session["CodPaisAux"].ToString();
            string tipoFavorito = Session["TipoFavorito"].ToString();
            
            string idioma = GetCurrentIdioma();
            object objMensaje = null;

            string auxTipoFavorito = tipoFavorito.Substring(0, 2);
            if (tipoFavorito == "ImportadorExp")
                auxTipoFavorito = "IE";

            if (isCheckedCreateGroup &&
                FuncionesBusiness.ExisteGrupo(textNewGroup.Trim(), idUsuario, codPais, tipoOpe, auxTipoFavorito))
            {
                string msg = "Ya existe el Grupo: <b>" + textNewGroup.Trim() + "</b>";
                if (idioma == "en")
                {
                    msg = "That group already exists: <b>" + textNewGroup.Trim() + "</b>"; ;
                }
                objMensaje = new
                {
                    titulo = Resources.MiPerfil.Btn_AddToGroup,
                    mensaje = msg,
                    flagContactenos = false
                };

                return Json(new
                {
                    objMensaje,
                    flagExisteGrupo = true
                });
            }

            ArrayList listIdsSeleccionados = (ArrayList)Session["IDsSeleccionados"];
            string msgActualizar = "";
            bool flagOk = FuncionesBusiness.ActualizaGrupo(idUsuario,
                ((codPais != "PEB" && codPais != "PEP") ? codPais : "PE"), tipoOpe, tipoFavorito, isCheckedCreateGroup,
                textNewGroup.Trim(), codGrupo, listIdsSeleccionados, idioma, ref msgActualizar);

            List<SelectListItem> itemsGruposFavoritos = null;
            List<SelectListItem> itemsSoloGruposFavoritos = null;
            if (isCheckedCreateGroup)
            {
                itemsGruposFavoritos = GetGruposFavoritos(true,idUsuario, codPais, tipoOpe, tipoFavorito, idioma);
                itemsSoloGruposFavoritos =
                    GetGruposFavoritos(false, idUsuario, codPais, tipoOpe, tipoFavorito, idioma);
            }

            objMensaje = new
            {
                titulo = Resources.MiPerfil.Btn_AddToGroup,
                mensaje = msgActualizar,
                flagContactenos = flagOk
            };

            int currentPage = Session["CurrentPageTableFavorites"] != null
                ? Convert.ToInt32(Session["CurrentPageTableFavorites"])
                : 1;

            Session["IDsSeleccionados"] = null;
            SetSessionGruposFavorito(idUsuario, tipoOpe, codPais, tipoFavorito, idioma);
            string rowsFavorites = GetRowsFavoritesByPage(currentPage);

            return Json(new
            {
                objMensaje,
                flagExisteGrupo = false,
                rowsFavorites,
                itemsGruposFavoritos,
                itemsSoloGruposFavoritos
            });
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult DeleteFavorite(string idsSeleccionados, string idsPagina)
        {
            if (!string.IsNullOrEmpty(idsPagina))
            {
                ActualizarSeleccionados(idsSeleccionados, idsPagina);
            }

            ArrayList listIdsSeleccionados = new ArrayList();
            if (Session["IDsSeleccionados"] != null)
                listIdsSeleccionados = (ArrayList)Session["IDsSeleccionados"];

            object objMensaje = null;
            AdminMyFavorite objAdminMyFavorite= null;
            if (listIdsSeleccionados.Count <= 0)
            {
                objMensaje = new
                {
                    titulo = Resources.MiPerfil.DeleteFavorite_Text,
                    mensaje = Resources.MiPerfil.SelectAtLeastOneFavoriteToDelete_Text,
                    flagContactenos = false
                };
            }
            else
            {
                string idUsuario = Session["IdUsuario"].ToString();
                string tipoOpe = Session["TipoOpe"].ToString();
                string codPais = Session["CodPaisAux"].ToString();
                string tipoFavorito = Session["TipoFavorito"].ToString();

                int cantEliminados = AdminMyFavoriteDAO.EliminarFavoritos(listIdsSeleccionados, tipoFavorito,
                    idUsuario, codPais, tipoOpe);

                //MANUEL: Se adiciona la funcionalidad que permite eliminar en Peru y en Peru_B Importadores a la vez
                if (codPais.Equals("PE") && tipoOpe.Equals("I"))
                {
                    AdminMyFavoriteDAO.EliminarFavoritos(listIdsSeleccionados, tipoFavorito, idUsuario, "PEB", tipoOpe);
                }
                else if (codPais.Equals("PEB") && tipoOpe.Equals("I"))
                {
                    AdminMyFavoriteDAO.EliminarFavoritos(listIdsSeleccionados, tipoFavorito, idUsuario, "PE", tipoOpe);
                }

                if (cantEliminados < listIdsSeleccionados.Count)
                {
                    objMensaje = new
                    {
                        titulo = Resources.MiPerfil.DeleteFavorite_Text,
                        mensaje = Resources.MiPerfil.Msg_CouldNotDeleteAllSelectedFavorites,
                        flagContactenos = false
                    };
                }

                if (cantEliminados > 0)
                {
                    Session.Remove("IDsSeleccionados");
                    string idioma = GetCurrentIdioma();
                    SetSessionGruposFavorito(idUsuario, tipoOpe, codPais, tipoFavorito, idioma);

                    objAdminMyFavorite = new AdminMyFavorite();

                    int indexCboGruposF = Convert.ToInt32(Session["IndexCboGruposF"]);
                    string valueCboGruposF = Session["ValueCboGruposF"].ToString();

                    string textCompany = Session["TextCompany"].ToString();
                    
                    int cantRegigstros = AdminMyFavoriteDAO.GetCantidadFavoritosUnicos(idUsuario, tipoOpe, codPais,
                        tipoFavorito, textCompany, "", indexCboGruposF, valueCboGruposF);
                    objAdminMyFavorite.RowsFavoritesUniquesInHtml = GetRowsFavoritesByPage(1);
                    objAdminMyFavorite.DescripcionCantidad = GetQuantityFavoritesUniques(textCompany, "",
                        indexCboGruposF, GetFavoriteDescription(codPais, tipoFavorito), cantRegigstros);
                    objAdminMyFavorite.TotalPaginas = GetTotalPaginas(cantRegigstros);
                }
            }

            return Json(new
            {
                objMensaje,
                objAdminMyFavorite
            });
        }

        [SessionExpireFilter]
        [HttpPost]

        public JsonResult GetExcelFile()
        {
            string idUsuario = Session["IdUsuario"].ToString();
            string codUsuario = FuncionesBusiness.BuscaCodUsuario(idUsuario).ToUpper();

            int indexCboGruposF = Convert.ToInt32(Session["IndexCboGruposF"]);
            string valueCboGruposF = Session["ValueCboGruposF"].ToString();
            

            string tipoOpe = Session["TipoOpe"].ToString();
            string codPais = Session["CodPaisAux"].ToString();
            string tipoFavorito = Session["TipoFavorito"].ToString();

            string idioma = GetCurrentIdioma();

            DataTable dtFavoritosUnicos = AdminMyFavoriteDAO.GetDataFavoritosUnicos(idUsuario, tipoOpe, codPais,
                tipoFavorito, "", "", indexCboGruposF, valueCboGruposF, -1, Funciones.fav_x_pag,false);
            dtFavoritosUnicos.Columns.Remove("IdFavorito");

            string nombreArchivo;
            string templateName = HostingEnvironment.MapPath("~/ExcelTemplate/VeritradeMisFavoritos.xlsx");

            using (OfficeOpenXml.ExcelPackage package =
                new OfficeOpenXml.ExcelPackage(new System.IO.FileInfo(templateName)))
            {
                OfficeOpenXml.ExcelWorksheet ws;

                if (tipoFavorito == "Importador" || tipoFavorito == "Exportador")
                {
                    ws = package.Workbook.Worksheets["Importadores1"];
                    ws.Name = GetTitlePerfilBar(tipoFavorito, codPais, idioma);
                    ws.Cells["A6"].Value = GetFavoriteDescription(codPais, tipoFavorito);
                    ws.Column(2).Hidden = !IsVisibleRuc(codPais, tipoFavorito);
                    package.Workbook.Worksheets["Exportadores1"].Hidden = OfficeOpenXml.eWorkSheetHidden.VeryHidden;
                }
                else
                {
                    ws = package.Workbook.Worksheets["Exportadores1"];
                    ws.Name = GetTitlePerfilBar(tipoFavorito, codPais, idioma);
                    ws.Cells["A6"].Value = GetFavoriteDescription(codPais, tipoFavorito);
                    package.Workbook.Worksheets["Importadores1"].Hidden = OfficeOpenXml.eWorkSheetHidden.VeryHidden;
                }

                package.Workbook.Worksheets["Partidas"].Hidden = OfficeOpenXml.eWorkSheetHidden.VeryHidden;
                
                ws.Cells["A7"].LoadFromDataTable(dtFavoritosUnicos, false);
                

                nombreArchivo = "Veritrade_" + GetTitlePerfilBar(tipoFavorito, codPais, idioma) + "_" + codUsuario + "_" + codPais +
                                "_" + tipoOpe + "_" + tipoFavorito + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") +
                                ".xlsx";

                package.SaveAs(
                    new System.IO.FileInfo(ConfigurationManager.AppSettings["directorio_descarga"] + nombreArchivo));
            }

            return Json(new
            {
                fileName = nombreArchivo
            });
        }

        [HttpGet]
        public ActionResult DownloadExcelFile(string fileName)
        {
            string fullPath = ConfigurationManager.AppSettings["directorio_descarga"] + fileName;
            return File(fullPath, "application/vnd.ms-excel", fileName);
        }

        [SessionExpireFilter]
        [HttpPost]
        public ActionResult SetCulture(string culture)
        {
            culture = CultureHelper.GetImplementedCulture(culture);
            RouteData.Values["culture"] = culture; // set culture 
            string tipoFavorito = Session["TipoFavorito"].ToString();
            //return RedirectToAction("Index");
            return RedirectToRoute("MisFavoritos", new { culture  = culture , tipoFavorito = tipoFavorito, action = "index"});
        }

    }
}