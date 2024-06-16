using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using Veritrade2018.App_Start;
using Veritrade2018.Data;
using Veritrade2018.Helpers;
using Veritrade2018.Models.Admin;

namespace Veritrade2018.Controllers.Admin
{
    public class AdminMisProductosController : BaseController
    {
        private readonly string[] _CodManifiestosModificado = new[] { "PEI", "PEE", "ECI", "ECE", "USI", "USE", "BRI", "BRE" };
        // GET: AdminMisProductos
        [AuthorizedNoReferido]
        public ActionResult Index(string culture)
        {
            if (Session["IdUsuario"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            Session.Remove("IDsSeleccionados");
            var idioma = culture;

            string codPais2 = Session["CodPais2"].ToString();
            string codPais = Session["CodPais"].ToString();

            string codPaisAux = codPais;

            if (_CodManifiestosModificado.Contains(codPaisAux))
            {
                codPaisAux = codPaisAux.Substring(0, 2) + "_";
            }

            if (codPais2 == "4UE")
            {
                codPais = "UE" + codPais;
            }

            string idUsuario = Session["IdUsuario"].ToString();
            string tipoFavorito = "Partida";
            string tipoOpe = Session["TipoOpe"].ToString();

            SetSessionGruposFavorito(idUsuario, tipoOpe, codPais, tipoFavorito, idioma);

            string nandinaF = "";
            string partidaF = "";
            int indexCboGruposF = -1;
            int pageInit = 1;

            int cantRegigstros = AdminMisProductosDAO.GetCantidadFavoritosUnicos(idUsuario, tipoOpe, codPais,
                tipoFavorito, nandinaF, partidaF, indexCboGruposF, "", idioma);
            
            DataTable dtFavoritosUnicos = AdminMisProductosDAO.GetDataFavoritosUnicos(idUsuario, tipoOpe, codPais,
                tipoFavorito, nandinaF, partidaF, indexCboGruposF, "", idioma, pageInit, Funciones.fav_x_pag);

            Session["Idioma"] = idioma;
            Session["CodPaisAux"] = codPais;
            Session["TipoOpe"] = tipoOpe;
            Session["TipoFavorito"] = tipoFavorito;
            Session["IndexCboGruposF"] = indexCboGruposF;
            Session["ValueCboGruposF"] = "";
            Session["TextCboGruposF"] = "";
            Session["TxtNandina"] = "";
            Session["TxtPartida"] = "";
            Session["CurrentPageTableProduct"] = pageInit;

            List<SelectListItem> listSoloGruposFavoritos =
                GetSoloGruposFavoritos(idUsuario, codPais, tipoOpe, tipoFavorito, idioma);
            AdminMyProduct objAdminMyProduct = new AdminMyProduct
            {
                ProductosFavoritos = GetListProductosFavoritos(dtFavoritosUnicos,pageInit, indexCboGruposF, ""),
                GruposFavoritos = GetGruposFavoritos(idUsuario, codPais, tipoOpe, tipoFavorito, idioma),
                SoloGruposFavoritos = listSoloGruposFavoritos,
                EnabledCboGroupsForm   = listSoloGruposFavoritos.Any(),
                DescripcionCantidad = GetCantidadProductos(nandinaF, partidaF, "", indexCboGruposF, cantRegigstros),
                TotalPaginas = GetTotalPaginas(cantRegigstros),
                TipoFavorito = tipoFavorito
            };

            ViewData["objAdminMyProduct"] = objAdminMyProduct;

            ViewBag.nombreUsuario = Funciones.BuscaUsuario(idUsuario);
            ViewBag.tipoUsuario = Session["TipoUsuario"].ToString();
            ViewBag.plan = Session["Plan"].ToString();
            ViewData["TitlePerfilBar"] = Resources.AdminResources.Filter_MyProducts_Button;
            ViewData["IngresoComoFreeTrial"] = Session["IngresoComoFreeTrial"] != null && (bool)Session["IngresoComoFreeTrial"];
            ViewData["DescPais"] = AdminMisProductosDAO.GetNamePais(culture, codPaisAux);
            ViewData["Regimen"] = tipoOpe == "I" ? culture.Equals("es") ? "Importaciones" : "Imports" : culture.Equals("es") ? "Exportaciones" : "Exports";
            return View();
        }

        #region MétodosAuxiliares
        private string GetCurrentIdioma()
        {
            return Session["Idioma"] != null ? Session["Idioma"].ToString() : "es";
        }

        private int GetTotalPaginas(int cantidadRegistros)
        {
            return (int)Math.Ceiling(Convert.ToDecimal(cantidadRegistros) / Funciones.fav_x_pag);
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

            Session["IDsSeleccionados"] = GuardaSeleccionados((ArrayList) Session["IDsSeleccionados"],
                arrayIdsSeleccionados, arrayIdsPagina);

        }

        private void SetSessionGruposFavorito(string idUsuario, string tipoOpe, string codPais, 
            string tipoFavorito, string idioma)
        {
            DataTable dtGruposFavoritos = AdminMisProductosDAO.GetDataGruposFavoritos(idUsuario, tipoOpe, codPais, tipoFavorito, idioma);

            List<GrupoFavorito> listaGruposFavoritos = dtGruposFavoritos.AsEnumerable().Select(m => new GrupoFavorito()
            {
                IdFavorito = Convert.ToString(m.Field<Int32>("IdFavorito")),
                Orden = Convert.ToString(m.Field<Int32>("IdFavorito")),
                IdGrupo = Convert.ToString(m.Field<Int32>("IdGrupo")),
                Grupo = m.Field<string>("Grupo")
            }).ToList();

            Session["GruposFavoritos"] = listaGruposFavoritos;
        }

        private List<SelectListItem> GetGruposFavoritos(string idUsuario, string codPais, string tipoOpe, 
            string tipoFavorito, string idioma)
        {

            DataTable dtCboGruposFavoritos = FuncionesBusiness.LlenaGrupos(true, idUsuario, codPais, tipoOpe, tipoFavorito, idioma);
            List<SelectListItem> listGruposFavoritos = dtCboGruposFavoritos.AsEnumerable().Select(m => new SelectListItem()
            {
                Value = Convert.ToString(m.Field<Int32>("IdGrupo")),
                Text = m.Field<string>("Grupo")
            }).ToList();
            return listGruposFavoritos;
        }

        private List<SelectListItem> GetSoloGruposFavoritos(string idUsuario, string codPais, string tipoOpe, 
            string tipoFavorito, string idioma)
        {
            DataTable dtSoloGruposFavorito = FuncionesBusiness.LlenaGrupos(false, idUsuario, codPais, tipoOpe, tipoFavorito, idioma);
            List<SelectListItem> listSoloGruposFavoritos = dtSoloGruposFavorito.AsEnumerable().Select(m => new SelectListItem()
            {
                Value = Convert.ToString(m.Field<Int32>("IdGrupo")),
                Text = m.Field<string>("Grupo")
            }).ToList();
            return listSoloGruposFavoritos;
        }
        #endregion

        private List<ProductoFavorito> GetListProductosFavoritos(DataTable dtFavoritosUnicos, int pagina , int indexCboGruposF,
            string textCboGrupoF)
        {
            int numero = (pagina - 1) * Funciones.fav_x_pag;
            List<GrupoFavorito> listGrupoFavoritos =(List<GrupoFavorito>)Session["GruposFavoritos"];

            bool flagSelectedGrupo = indexCboGruposF > 0;

            List<ProductoFavorito> productosFavoritos = dtFavoritosUnicos.AsEnumerable().Select(m =>
                new ProductoFavorito()
                {
                    Index = ++numero,
                    IdPartida = m.Field<string>("IdPartida"),
                    Nandina = m.Field<string>("Nandina"),
                    Partida = m.Field<string>("Partida"),
                    PartidaFav = m.Field<string>("PartidaFav"),
                    FlagIndividual = m.Field<string>("FlagIndividual"),
                    IsVisibleActualizar = m.Field<string>("FlagIndividual") == "S",
                    GrupoFavoritos = listGrupoFavoritos.Where(
                                                         x => flagSelectedGrupo ?  x.IdFavorito == m.Field<string>("IdPartida").Replace("F", "") && x.Grupo == textCboGrupoF
                                                             : x.IdFavorito == m.Field<string>("IdPartida").Replace("F", "")).ToList()
                }).ToList();

            return productosFavoritos;
        }

        private string GetCantidadProductos(string nandinaFavorito, string partidaFavorito, string partidaFavF,
            int indexCboGruposF, int cantidadRegistros)
        {
            var idUsuario = Session["IdUsuario"].ToString();
            var codPais = Session["CodPaisAux"].ToString();
            var tipoOpe = Session["TipoOpe"].ToString();
            var tipoFavorito = Session["TipoFavorito"].ToString();

            var idioma = GetCurrentIdioma();

            string cantidad;
            if (idioma == "es")
            {
                cantidad = "Se tiene(n) "+FuncionesBusiness.CantFavUnicos(idUsuario,codPais, tipoOpe, tipoFavorito )+ " Producto(s)";
            }
            else
            {
                cantidad = "There are " + FuncionesBusiness.CantFavUnicos(idUsuario, codPais, tipoOpe, tipoFavorito)+ " Products";
            }

            if (nandinaFavorito != "" || partidaFavorito != "" || partidaFavF != "" || indexCboGruposF > 0)
            {
                if (idioma == "es")
                {
                    cantidad += " | "+ cantidadRegistros  +" Filtrado(s)";
                }
                else
                {
                    cantidad += " | "+cantidadRegistros + " Filtered";
                }
            }
            return cantidad;
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult PageIndexChanging(int pagina, string idsSeleccionados, string idsPagina)
        {
            if (!string.IsNullOrEmpty(idsPagina))
            {
                ActualizarSeleccionados(idsSeleccionados, idsPagina);
            }

            Session["CurrentPageTableProduct"] = pagina;

            string filasProductosFavoritos = GetFilasProductosFavoritosXpagina(pagina);

            return Json(new
            {
                filasProductosFavoritos
            });
        }

        private string GetFilasProductosFavoritosXpagina(int pagina)
        {
            int indexCboGruposF = Convert.ToInt32(Session["IndexCboGruposF"]);
            string valueCboGruposF = Session["ValueCboGruposF"].ToString();

            string textNandina = Session["TxtNandina"].ToString();
            string textPartida = Session["TxtPartida"].ToString();

            string idioma = GetCurrentIdioma();
            DataTable dtFavoritosUnicos = AdminMisProductosDAO.GetDataFavoritosUnicos(Session["IdUsuario"].ToString(), Session["TipoOpe"].ToString(), Session["CodPaisAux"].ToString(),
                Session["TipoFavorito"].ToString(), textNandina, textPartida, indexCboGruposF, valueCboGruposF, idioma, pagina, Funciones.fav_x_pag);

            List<ProductoFavorito> listaProductosFavoritos =
                GetListProductosFavoritos(dtFavoritosUnicos, pagina, indexCboGruposF, Session["TextCboGruposF"].ToString());

            AdminMyProduct objAdminMyProduct = new AdminMyProduct
            {
                ProductosFavoritos = listaProductosFavoritos,
                IdsSeleccionados = Session["IDsSeleccionados"] != null ? (ArrayList)Session["IDsSeleccionados"]: null
            };

            Session["CurrentPageTableProduct"] = pagina;

            return RenderViewToString(this.ControllerContext, "GridViews/TableRowView", objAdminMyProduct);
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult SearchByNandina(string textNandina)
        {
            int indexCboGruposF = Convert.ToInt32(Session["IndexCboGruposF"]);
            string valueCboGruposF = "";
            if (indexCboGruposF > 1)
                valueCboGruposF = Session["ValueCboGruposF"].ToString();


            string idUsuario = Session["IdUsuario"].ToString();
            string tipoOpe = Session["TipoOpe"].ToString();
            string codPais = Session["CodPaisAux"].ToString();
            string tipoFavorito = Session["TipoFavorito"].ToString();

            string idioma = GetCurrentIdioma();

            int cantRegigstros = AdminMisProductosDAO.GetCantidadFavoritosUnicos(idUsuario, tipoOpe, codPais,
                tipoFavorito,textNandina.Trim(),"", indexCboGruposF, valueCboGruposF, idioma);

            DataTable dtFavoritosUnicos = AdminMisProductosDAO.GetDataFavoritosUnicos(idUsuario, tipoOpe, codPais,
                tipoFavorito, textNandina.Trim(), "", indexCboGruposF, valueCboGruposF, idioma, 1, Funciones.fav_x_pag);

            List<ProductoFavorito> listaProductosFavoritos =
                GetListProductosFavoritos(dtFavoritosUnicos,1,indexCboGruposF, Session["TextCboGruposF"].ToString());
            

            AdminMyProduct objAdminMyProduct = new AdminMyProduct
            {
                DescripcionCantidad = GetCantidadProductos(textNandina.Trim(), "", "", indexCboGruposF, cantRegigstros),
                TotalPaginas = GetTotalPaginas(cantRegigstros),
                ProductosFavoritos = listaProductosFavoritos
            };

            string filasProductosFavoritos =
                RenderViewToString(this.ControllerContext, "GridViews/TableRowView", objAdminMyProduct);

            objAdminMyProduct.FilasProductosFavoritos = filasProductosFavoritos;

            Session["TxtNandina"] = textNandina.Trim();
            Session["TxtPartida"] = "";

            return Json(new
            {
                objAdminMyProduct
            });
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult SearchByPartida(string textPartida)
        {
            int indexCboGruposF = Convert.ToInt32(Session["IndexCboGruposF"]);
            string valueCboGruposF = "";
            if (indexCboGruposF > 1)
                valueCboGruposF = Session["ValueCboGruposF"].ToString();

            string idUsuario = Session["IdUsuario"].ToString();
            string tipoOpe = Session["TipoOpe"].ToString();
            string codPais = Session["CodPaisAux"].ToString();
            string tipoFavorito = Session["TipoFavorito"].ToString();

            string idioma = GetCurrentIdioma();

            int cantRegigstros = AdminMisProductosDAO.GetCantidadFavoritosUnicos(idUsuario, tipoOpe, codPais,
                tipoFavorito, "", textPartida.Trim(), indexCboGruposF, valueCboGruposF, idioma);
            
            DataTable dtFavoritosUnicos = AdminMisProductosDAO.GetDataFavoritosUnicos(idUsuario, tipoOpe, codPais,
                tipoFavorito, "", textPartida.Trim(), indexCboGruposF, valueCboGruposF, idioma, 1, Funciones.fav_x_pag);
            
            List<ProductoFavorito> listaProductosFavoritos =
                GetListProductosFavoritos(dtFavoritosUnicos, 1,indexCboGruposF, Session["TextCboGruposF"].ToString());

            AdminMyProduct objAdminMyProduct = new AdminMyProduct
            {
                DescripcionCantidad = GetCantidadProductos("", textPartida.Trim(), "", indexCboGruposF, cantRegigstros),
                TotalPaginas = GetTotalPaginas(cantRegigstros),
                ProductosFavoritos = listaProductosFavoritos
            };

            string filasProductosFavorito =
                RenderViewToString(this.ControllerContext, "GridViews/TableRowView", objAdminMyProduct);
            objAdminMyProduct.FilasProductosFavoritos = filasProductosFavorito;

            Session["TxtNandina"] = "";
            Session["TxtPartida"] = textPartida.Trim();

            return Json(new
            {
                objAdminMyProduct
            });
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult GruposFavoritosChange(int indexCboGruposF, string valueCboGruposF, string textCboGruposF)
        {
            Session["IndexCboGruposF"] = indexCboGruposF;
            Session["ValueCboGruposF"] = valueCboGruposF;
            Session["TextCboGruposF"] = textCboGruposF;

            string textNandina = Session["TxtNandina"].ToString();
            string textPartida = Session["TxtPartida"].ToString();

            string idUsuario = Session["IdUsuario"].ToString();
            string tipoOpe = Session["TipoOpe"].ToString();
            string codPais = Session["CodPaisAux"].ToString();
            string tipoFavorito = Session["TipoFavorito"].ToString();

            string idioma = GetCurrentIdioma();

            int cantRegigstros = AdminMisProductosDAO.GetCantidadFavoritosUnicos(idUsuario, tipoOpe, codPais,
                tipoFavorito, textNandina, textPartida, indexCboGruposF, valueCboGruposF, idioma);

            DataTable dtFavoritosUnicos = AdminMisProductosDAO.GetDataFavoritosUnicos(idUsuario, tipoOpe, codPais,
                tipoFavorito, textNandina,textPartida, indexCboGruposF, valueCboGruposF, idioma, 1, Funciones.fav_x_pag);

            List<ProductoFavorito> listaProductosFavoritos =
                GetListProductosFavoritos(dtFavoritosUnicos, 1, indexCboGruposF, textCboGruposF);

            AdminMyProduct objAdminMyProduct = new AdminMyProduct
            {
                DescripcionCantidad = GetCantidadProductos(textNandina, textPartida, "", indexCboGruposF, cantRegigstros),
                TotalPaginas = GetTotalPaginas(cantRegigstros),
                ProductosFavoritos = listaProductosFavoritos
            };

            string filasProductosFavoritos =
                RenderViewToString(this.ControllerContext, "GridViews/TableRowView", objAdminMyProduct);

            objAdminMyProduct.FilasProductosFavoritos = filasProductosFavoritos;

            return Json(new
            {
                objAdminMyProduct
            });
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult ActualizarPartidaFav(string idPartida, string textPartidaFav)
        {
            string auxIdPartida = idPartida.Replace("F", "");

            if (textPartidaFav.Trim() != "")
            {
                FuncionesBusiness.ActualizaPartidaFavorita(Session["IdUsuario"].ToString(), Session["CodPaisAux"].ToString(), Session["TipoOpe"].ToString(), auxIdPartida, textPartidaFav);
            }

            int currentPage = Session["CurrentPageTableProduct"] != null
                ? Convert.ToInt32(Session["CurrentPageTableProduct"])
                : 1;

            string filasProductosFavoritos = GetFilasProductosFavoritosXpagina(currentPage);

            return Json(new
            {
                filasProductosFavoritos
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
            if(Session["IDsSeleccionados"] != null)
                listIdsSeleccionados = (ArrayList) Session["IDsSeleccionados"];

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
            
            if (isCheckedCreateGroup &&
                FuncionesBusiness.ExisteGrupo(textNewGroup.Trim(), idUsuario,codPais, tipoOpe, tipoFavorito))
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
                itemsGruposFavoritos = GetGruposFavoritos(idUsuario, codPais, tipoOpe, tipoFavorito, idioma);
                itemsSoloGruposFavoritos = GetSoloGruposFavoritos(idUsuario, codPais, tipoOpe, tipoFavorito, idioma);
            }

            objMensaje = new
            {
                titulo = Resources.MiPerfil.Btn_AddToGroup,
                mensaje = msgActualizar,
                flagContactenos = flagOk
            };

            int currentPage = Session["CurrentPageTableProduct"] != null
                ? Convert.ToInt32(Session["CurrentPageTableProduct"])
                : 1;

            Session["IDsSeleccionados"] = null;
            SetSessionGruposFavorito(idUsuario, tipoOpe, codPais, tipoFavorito, idioma);
            string filasProductosFavoritos = GetFilasProductosFavoritosXpagina(currentPage);
            

            return Json(new
            {
                objMensaje,
                flagExisteGrupo = false,
                filasProductosFavoritos,
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
            AdminMyProduct objAdminMyProduct = null;
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

                int cantEliminados = AdminMisProductosDAO.EliminarFavoritos(listIdsSeleccionados, tipoFavorito,
                    idUsuario, codPais, tipoOpe);

                //MANUEL: Se adiciona la funcionalidad que permite eliminar en Peru y en Peru_B Importadores a la vez
                if (codPais.Equals("PE") && tipoOpe.Equals("I"))
                {
                    AdminMisProductosDAO.EliminarFavoritos(listIdsSeleccionados, tipoFavorito, idUsuario, "PEB", tipoOpe);
                }else if (codPais.Equals("PEB") && tipoOpe.Equals("I"))
                {
                    AdminMisProductosDAO.EliminarFavoritos(listIdsSeleccionados, tipoFavorito, idUsuario, "PE", tipoOpe);
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
                    
                    objAdminMyProduct = new AdminMyProduct();

                    int indexCboGruposF = Convert.ToInt32(Session["IndexCboGruposF"]);
                    string valueCboGruposF = Session["ValueCboGruposF"].ToString();

                    string textNandina = Session["TxtNandina"].ToString();
                    string textPartida = Session["TxtPartida"].ToString();
                    
                    int cantRegigstros = AdminMisProductosDAO.GetCantidadFavoritosUnicos(idUsuario, tipoOpe, codPais,
                        tipoFavorito,textNandina, textPartida, indexCboGruposF, valueCboGruposF, idioma);

                    objAdminMyProduct.FilasProductosFavoritos = GetFilasProductosFavoritosXpagina(1);
                    objAdminMyProduct.DescripcionCantidad = GetCantidadProductos(textNandina, textPartida, "",
                        indexCboGruposF, cantRegigstros);
                    objAdminMyProduct.TotalPaginas = GetTotalPaginas(cantRegigstros);
                }
            }

            return Json(new
            {
                objMensaje,
                objAdminMyProduct
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

            string textNandina = Session["TxtNandina"].ToString();
            string textPartida = Session["TxtPartida"].ToString();

            
            string tipoOpe = Session["TipoOpe"].ToString();
            string codPais = Session["CodPaisAux"].ToString();
            string tipoFavorito = Session["TipoFavorito"].ToString();

            string idioma = GetCurrentIdioma();
            DataTable dtFavoritosUnicos = AdminMisProductosDAO.GetDataFavoritosUnicos(idUsuario,tipoOpe, codPais,
                tipoFavorito, textNandina, textPartida, indexCboGruposF, valueCboGruposF, idioma, -1, Funciones.fav_x_pag, false);

            dtFavoritosUnicos.Columns.Remove("IdPartida");
            dtFavoritosUnicos.Columns.Remove("FlagIndividual");

            string nombreArchivo;
            string templateName = HostingEnvironment.MapPath("~/ExcelTemplate/VeritradeMisFavoritos.xlsx");

            using (OfficeOpenXml.ExcelPackage package =
                new OfficeOpenXml.ExcelPackage(new System.IO.FileInfo(templateName)))
            {
                OfficeOpenXml.ExcelWorksheet ws;

                ws = package.Workbook.Worksheets["Partidas"];
                ws.Name = Resources.AdminResources.Filter_MyProducts_Button;

                ws.Cells["A6"].Value = Resources.AdminResources.Nandina2_FilterText;
                ws.Cells["B6"].Value = Resources.AdminResources.HTS_Description_Text;
                ws.Cells["C6"].Value = Resources.AdminResources.FavoriteName_Text;

                ws.Cells["A7"].LoadFromDataTable(dtFavoritosUnicos, false);


                package.Workbook.Worksheets["Importadores1"].Hidden = OfficeOpenXml.eWorkSheetHidden.VeryHidden;
                package.Workbook.Worksheets["Exportadores1"].Hidden = OfficeOpenXml.eWorkSheetHidden.VeryHidden;

                nombreArchivo= "Veritrade_" + Resources.AdminResources.Filter_MyProducts_Button.Replace(" ", "") + "_" + codUsuario + "_" + codPais +
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

            return RedirectToAction("Index");
        }
    }
}