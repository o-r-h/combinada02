
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Veritrade2018.App_Start;
using Veritrade2018.Helpers;
using Veritrade2018.Models.Admin;
using Veritrade2018.Util;

namespace Veritrade2018.Controllers.Admin
{
    public class MisAlertasFavoritosController : BaseController
    {
        // GET: MisAlertasFavoritos
        [AuthorizedAlerts]
        [AuthorizedNoReferido]
        public ActionResult Index()
        {
            
            if (Session["IdUsuario"] == null)
                return RedirectToAction("Logout", "Common");

            var codPais = Session["CodPais"].ToString();
            var codPais2 = (Session["CodPais2"]??"").ToString();

            if (string.IsNullOrEmpty(codPais2))
            {
                // Ruben 202207
                if (codPais == "CRS" || codPais == "MXD" || codPais == "MXM")
                {
                    codPais2 = new ListaPaises().BuscarCodPais2(Session["CodPais"].ToString()); //"7NOR";
                }
                else
                {
                    codPais2 = new ListaPaises().BuscarCodPais2(Session["CodPais"].ToString());
                }
                
                Session["CodPais2"] = codPais2;
            }

            var tipoOpe = (Session["TipoOpe"]??"I").ToString();
            Session["TipoOpe"] = tipoOpe;
            string culture = CultureHelper.GetCurrentNeutralCulture();
            MiPerfil objMiPerfil = new MiPerfil();

            ListaPaises objListaPaises = new ListaPaises();

            List<SelectListItem> countries2 = objListaPaises.GetPaises2(culture).ToList();
            objMiPerfil.ListItemsPais2 = countries2;
            if (countries2.FirstOrDefault(x => x.Value == codPais2) == null)
            {
                objMiPerfil.CodPais2Selected = countries2.First().Value;
                codPais2 = countries2.First().Value;
            }
            else
            {
                objMiPerfil.CodPais2Selected = codPais2;
            }

            //var countries = new ListaPaises().GetPaisesAdmin(culture, codPais2);//objListaPaises.GetPaisesPerfil(culture, codPais2).ToList();
            var countries = new ListaPaises().GetPaisesAdmin(culture, codPais2, k_excludePais: Enums.VarId.EXCLUDE_PAIS_MA.GetDn()).ToList();
            objMiPerfil.ListItemsPais = countries;
            if (codPais2 == "4UE")
            {
                codPais = countries.First().Value;
                objMiPerfil.CodPaisSelected = codPais;
            }
            // Ruben 202207
            else if (codPais.Length == 2 || codPais == "CRS" || codPais == "MXD" || codPais == "MXM")
            {
                if (countries.FirstOrDefault(x => x.Value == codPais) == null)
                {
                    codPais = countries.First().Value;
                    objMiPerfil.CodPaisSelected = codPais;
                }
                else
                {
                    objMiPerfil.CodPaisSelected = codPais;
                }
            }
            else
            {
                if (countries.FirstOrDefault(x => x.Value == codPais.Substring(0, 2) + "_") == null)
                {
                    codPais = countries.First().Value;
                    objMiPerfil.CodPaisSelected = codPais;
                }
                else
                {
                    objMiPerfil.CodPaisSelected = codPais.Substring(0, 2) + "_";
                }
            }

            objMiPerfil.IsCheckedImportacion = tipoOpe == "I";
            objMiPerfil.IscheckedExportacion = tipoOpe == "E";

            if (Session["Plan"].ToString() == "PERU IMEX"
                || Session["Plan"].ToString() == "ECUADOR IMEX")
            {
                objMiPerfil.IsEnabledCodPais = false;
            }
            else if (!(bool)(Session["opcionFreeTrial"] ?? false) && Session["Plan"].ToString() == "ESENCIAL")
            {
                objMiPerfil.IsEnabledCodPais = false;
                //objMiPerfil.IsEnabledImportacion = tipoOpe == "I";
                objMiPerfil.IsCheckedImportacion = tipoOpe == "I";
                //objMiPerfil.IsEnabledExportacion = tipoOpe == "E";
                objMiPerfil.IscheckedExportacion = tipoOpe == "E";
                if (Session["Plan"].ToString() != "ESENCIAL")
                {
                    objMiPerfil.IsEnabledImportacion = tipoOpe == "I";
                    objMiPerfil.IsEnabledExportacion = tipoOpe == "E";
                }
                else
                {
                    objMiPerfil.IsEnabledCodPaisB = false;
                }
            }
            else if (/*Session["Plan"].ToString() == "PERU UNO" ||*/
                    Session["Plan"].ToString() == "ECUADOR UNO")

            {
                objMiPerfil.IsEnabledCodPais = false;
                objMiPerfil.IsEnabledCodPaisB = false;
                //flagEnabledrdbImp = flagCheckedrdbImp = (Session["TipoOpe"].ToString() == "I");

                //flagEnabledrdbExp = flagCheckedrdbExp = (Session["TipoOpe"].ToString() == "E");
                codPais = ""; tipoOpe = "";
                Funciones.BuscaDatosPlanEspecial(Session["IdUsuario"].ToString(), ref codPais, ref tipoOpe);
                codPais = codPais.Substring(0, 2);
                Session["CodPais"] = codPais;
                Session["TipoOpe"] = tipoOpe;

                if (tipoOpe == "T")
                {
                    objMiPerfil.IsCheckedImportacion = true;
                    objMiPerfil.IscheckedExportacion = false;
                    Session["TipoOpe"] = "I";
                    tipoOpe = "I";
                }
                else
                {
                    objMiPerfil.IsEnabledImportacion = tipoOpe == "I";
                    objMiPerfil.IsEnabledExportacion = tipoOpe == "E";
                    objMiPerfil.IsCheckedImportacion = true;
                    objMiPerfil.IscheckedExportacion = false;
                }
            }

            objMiPerfil.TipoOpe = tipoOpe;
            objMiPerfil.MyFavoriteAndGroup = GetMyFavoriteAndGroup(tipoOpe, codPais2, codPais);

            objMiPerfil.IsAlertasDeshabilitadas = Funciones.BuscaAlertasDeshabilitadasUsuario(Session["IdUsuario"].ToString());

            ViewData["objMiPerfil"] = objMiPerfil;
            ViewData["TitlePerfilBar"] = Resources.AdminResources.TitleAlerts_Text;
            ViewBag.nombreUsuario = Funciones.BuscaUsuario(Session["IdUsuario"].ToString());
            ViewBag.tipoUsuario = Session["TipoUsuario"].ToString();
            ViewBag.plan = Session["Plan"].ToString();
            ViewData["IngresoComoFreeTrial"] = Session["IngresoComoFreeTrial"] != null && (bool)Session["IngresoComoFreeTrial"];
            return View();
        }
        public ActionResult SetCulture(string culture)
        {
            culture = CultureHelper.GetImplementedCulture(culture);
            RouteData.Values["culture"] = culture; // set culture 

            return RedirectToAction("Index");
        }

        private string GetCurrentIdioma()
        {
            return Session["Idioma"] != null ? Session["Idioma"].ToString() : "es";
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

            return objMensaje;
        }

        [SessionExpireFilter]
        [HttpPost]
        public bool ToggleAlertSend()
        {
            string idUsuario = Session["IdUsuario"].ToString();
            bool isAlertasDeshabilitadas = Funciones.BuscaAlertasDeshabilitadasUsuario(idUsuario);
            isAlertasDeshabilitadas = Funciones.CambiarEstadoAlerta(idUsuario, !isAlertasDeshabilitadas);
            return isAlertasDeshabilitadas;
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult Pais2Change(string codPais2)
        {
            MiPerfil objMiPerfil = new MiPerfil();

            string idioma = GetCurrentIdioma();
            ListaPaises objListaPaises = new ListaPaises();

            //var auxCountries = codPais2.Equals("6RES")
            //? new ListaPaises().GetPaisesAdmin(idioma, codPais2)
            //: objListaPaises.GetPaisesPerfil(idioma, codPais2).ToList();

            //List<SelectListItem> auxCountries = objListaPaises.GetPaisesPerfil(idioma, codPais2).ToList();

            //var auxCountries = new ListaPaises().GetPaisesAdmin(idioma, codPais2);
            var auxCountries = new ListaPaises().GetPaisesAdmin(idioma, codPais2, k_excludePais: Enums.VarId.EXCLUDE_PAIS_MA.GetDn()).ToList();
            string textCodPais = auxCountries.FirstOrDefault()?.Text;
            string auxCodPais2 = codPais2;
            string auxCodPais = "";

            object objMensaje = (bool)(Session["opcionFreeTrial"] ?? false) ? null : ValidaPais2(ref auxCodPais2, ref auxCodPais, textCodPais);

            objMiPerfil.CodPais2Selected = auxCodPais2;
            if (objMensaje != null)
            {
                /*List<SelectListItem> countries2 = objListaPaises.GetPaises2(idioma).ToList();

                if (countries2.FirstOrDefault(x => x.Value == auxCodPais2) == null)
                {
                    auxCodPais2 = countries2.First().Value;
                    objMiPerfil.CodPais2Selected = auxCodPais2;
                }*/

                //auxCountries = codPais2.Equals("6RES") ?
                //    new ListaPaises().GetPaisesAdmin(idioma, auxCodPais2) :
                //    objListaPaises.GetPaisesPerfil(auxCodPais2, idioma).ToList();
                //auxCountries = objListaPaises.GetPaisesPerfil(auxCodPais2, idioma).ToList();
                //auxCountries = new ListaPaises().GetPaisesAdmin(idioma, auxCodPais2);

                auxCodPais = Session["CodPais"].ToString();
                auxCodPais2 = new ListaPaises().BuscarCodPais2(auxCodPais);
                auxCodPais2 = (auxCodPais2 == "") ? "1LAT" : auxCodPais2;

                auxCountries = new ListaPaises().GetPaisesAdmin(idioma, auxCodPais2, k_excludePais: Enums.VarId.EXCLUDE_PAIS_MA.GetDn()).ToList();
                objMiPerfil.CodPais2Selected = auxCodPais2;
                Session["CodPais2"] = auxCodPais2;

                Session["CodPais"] = auxCodPais;

                /*auxCodPais = Session["CodPais"].ToString();
                auxCodPais2 = new ListaPaises().BuscarCodPais2(CodPaisT);
                auxCodPais2 = (auxCodPais2 == "") ? "1LAT" : auxCodPais2;
                listaPaises = new ListaPaises().GetPaisesAdmin(idioma, CodPais2T);
                codPaisAselecconar = CodPaisT;
                if (listaPaises.Where(x => x.Value == CodPaisT).ToList().Count <= 0)
                {
                    codPaisAselecconar = "0";
                }



                Session["CodPais2"] = CodPais2T;

                Session["CodPais"] = CodPaisT;*/

            }

            if (auxCountries.FirstOrDefault(x => x.Value == auxCodPais) == null)
            {
                auxCodPais = auxCountries.First().Value;
            }

            objMiPerfil.CodPaisSelected = auxCodPais;
            objMiPerfil.ListItemsPais = auxCountries;

            string tipoOpe = Session["TipoOpe"].ToString();
            objMiPerfil.TipoOpe = tipoOpe;

            objMiPerfil.MyFavoriteAndGroup = GetMyFavoriteAndGroup(tipoOpe, auxCodPais2, auxCodPais);

            string myFavoritesAndMyGroups =
               RenderViewToString(this.ControllerContext, "Partials/MyFavoritesAndMyGroups", objMiPerfil);

            Session["CodPais2"] = auxCodPais2;
            Session["CodPais"] = auxCodPais;

            return Json(new
            {
                objMensaje,
                objMiPerfil,
                myFavoritesAndMyGroups
            });
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult PaisChange(string codPais, string textCodPais)
        {
            string auxCodPais2 = Session["CodPais2"].ToString();
            string auxCodPais2Actual = auxCodPais2;
            string auxCodPais = codPais;

            MiPerfil objMiPerfil = new MiPerfil();

            object objMensaje = (bool)(Session["opcionFreeTrial"] ?? false) ? null : ValidaPais(ref auxCodPais2, ref auxCodPais, textCodPais);

            objMiPerfil.CodPais2Selected = auxCodPais2;
            objMiPerfil.CodPaisSelected = auxCodPais;

            if (objMensaje != null)
            {
                string idioma = GetCurrentIdioma();
                ListaPaises objListaPaises = new ListaPaises();

                List<SelectListItem> countries2 = objListaPaises.GetPaises2(idioma).ToList();
                if (countries2.FirstOrDefault(x => x.Value == auxCodPais2) == null)
                {
                    auxCodPais2 = countries2.First().Value;
                    objMiPerfil.CodPais2Selected = auxCodPais2;
                }
                var auxCountries = new ListaPaises().GetPaisesAdmin(idioma, auxCodPais2Actual);//objListaPaises.GetPaisesPerfil(auxCodPais2Actual, idioma).ToList();
                if (auxCountries.FirstOrDefault(x => x.Value == auxCodPais) == null)
                {
                    //auxCodPais = countries2.First().Value;
                    objMiPerfil.CodPaisSelected = auxCodPais;
                }
            }

            string tipoOpe = Session["TipoOpe"].ToString();
            objMiPerfil.MyFavoriteAndGroup = GetMyFavoriteAndGroup(tipoOpe, auxCodPais2, auxCodPais);
            objMiPerfil.TipoOpe = tipoOpe;
            string myFavoritesAndMyGroups =
                RenderViewToString(this.ControllerContext, "Partials/MyFavoritesAndMyGroups", objMiPerfil);

            Session["CodPais2"] = auxCodPais2;
            Session["CodPais"] = auxCodPais;

            return Json(new
            {
                objMensaje,
                objMiPerfil,
                myFavoritesAndMyGroups
            });
        }

        [SessionExpireFilter]
        [HttpGet]
        public ActionResult ShowMeAlertPreview(string codPais, string culture)
        {
            string base_url = Request.Url.GetLeftPart(UriPartial.Authority);
            string tplPath = HttpContext.Server.MapPath("~/Content/tpl/alertas.mail.html");
            string v1 = "", v2 = "";
            /*try
            {*/
                var ret = new SuperAdmin.AdminAlertasController().GetAlertaMail(Session["IdUsuario"].ToString(), codPais, GiveMeLastPeriodo(codPais), ref v1, ref v2, base_url, tplPath, culture);
                return Content(ret, "text/html", System.Text.Encoding.UTF8);
            /*}
            catch
            {
                return RedirectToAction("Index");
            }*/
           
        }

        private string GiveMeLastPeriodo(string codPais)
        {
            if (codPais.Length > 0 && codPais.Substring(0, 2) == "UE")
            {
                codPais = codPais.Substring(0, 2);
            }

            string sql = @"SELECT top 1    
                            cast(left(cast(FechaFin as varchar), 6) +'01' as date) As Fecha
                            FROM BaseDatos
                            WHERE CodPais = @codPais order by 1 desc";
            using (var db = new ConexProvider().Open)
            {
                var r1 = db.QueryFirstOrDefault<dynamic>(sql, new { codPais });
                if (r1 != null)
                {
                    return r1.Fecha.ToString();//.ToString("MM/dd/yyyy H:mm");
                }
            }
            return string.Empty;
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult TipoOpeChange(string tipoOpe)
        {
            string codPais2 = Session["CodPais2"].ToString();
            string codPais = Session["CodPais"].ToString();

            Session["TipoOpe"] = tipoOpe;

            var objMiPerfil = new MiPerfil
            {
                TipoOpe = tipoOpe,
                CodPais2Selected = codPais2,
                CodPaisSelected = codPais,
                MyFavoriteAndGroup = GetMyFavoriteAndGroup(tipoOpe, codPais2, codPais)
            };

            string myFavoritesAndMyGroups =
                RenderViewToString(this.ControllerContext, "Partials/MyFavoritesAndMyGroups", objMiPerfil);

            return Json(new
            {
                myFavoritesAndMyGroups
            });
        }
        private MyFavoriteAndGroup GetMyFavoriteAndGroup(string tipoOpe, string codPais2, string codPais)
        {

            var _k = new VarValues();
            var excludePais = Enums.VarId.EXCLUDE_PAIS_MC.GetDn();
            var excludeRegion = Enums.VarId.EXCLUDE_REG_MC.GetDn();
            if (!string.IsNullOrEmpty(excludePais))
            {
                _k = VarGeneral.Instance.ValuesDict[excludePais];
            }

            var codPaisAux = codPais;
            if(codPais.Length > 2 && codPais.Substring(2) == "E")
            {
                codPaisAux = codPais.Substring(0, 2) + "_";
            }


            var validar = _k.Valores.Split(',').ToList().Exists(t => t == codPaisAux);

            
            if (!validar)
            {
                if (!string.IsNullOrEmpty(excludeRegion))
                {
                    _k = VarGeneral.Instance.ValuesDict[excludeRegion];
                }
                validar = _k.Valores.Split(',').ToList().Exists(t => t == codPais2);
            }
            

            MyFavoriteAndGroup objMyFavoriteAndGroup = new MyFavoriteAndGroup();

            // Ruben 202203
            if (!(codPais.Length == 2 || codPais == "CRS" || codPais == "MXD" || codPais == "MXM"))
            {
                codPais = codPais.Substring(0, 2) + tipoOpe;
            }

            var tabs = new TabMisBusquedas(tipoOpe, codPais);

            if (codPais2 != "4UE")
            {
                // Ruben 202203
                objMyFavoriteAndGroup.ExistPartida = codPais.Length == 2 || codPais == "CRS" || codPais == "MXD" || codPais == "MXM" || tabs.ExistePartida;
                objMyFavoriteAndGroup.ExistImporter =  //Funciones.ExisteVariable(codPais, tipoOpe, "IdImportador") ||
                                                      (tipoOpe == "I" && (codPais == "PEI" || codPais == "USI" || tabs.ExisteImportador));
                objMyFavoriteAndGroup.ExistSupplier =//Funciones.ExisteVariable(codPais, tipoOpe, "IdProveedor") ||
                                                      (tipoOpe == "I" && (tabs.ExisteProveedor || codPais == "PEI" || codPais == "USI"));

                objMyFavoriteAndGroup.ExistExporter = //Funciones.ExisteVariable(codPais, tipoOpe, "IdExportador") ||
                                                      (tipoOpe == "E" && (tabs.ExisteExportador || codPais == "PEE" || codPais == "USE"));

                objMyFavoriteAndGroup.ExistImporterExp = //Funciones.ExisteVariable(codPais, tipoOpe, "IdImportadorExp") ||
                                                      (tipoOpe == "E" && (tabs.ExisteImportadorExp || codPais == "PEE"));
                objMyFavoriteAndGroup.ExistRestriccion = validar;

            }
            else
            {
                objMyFavoriteAndGroup.ExistPartida = true;
                objMyFavoriteAndGroup.ExistImporter = false;
                objMyFavoriteAndGroup.ExistSupplier = false;
                objMyFavoriteAndGroup.ExistExporter = false;
                objMyFavoriteAndGroup.ExistImporterExp = false;
                objMyFavoriteAndGroup.ExistRestriccion = false;
            }

            if (Session["Plan"].ToString() == "ESENCIAL")
            {
                objMyFavoriteAndGroup.ExistSupplier = false;
                objMyFavoriteAndGroup.ExistImporterExp = false;
            }

            return objMyFavoriteAndGroup;
        }
    }
}
