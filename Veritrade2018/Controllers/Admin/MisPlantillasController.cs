using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Veritrade2018.App_Start;
using Veritrade2018.Helpers;
using Veritrade2018.Models.Admin;
using Veritrade2018.Data;
using Veritrade2018.Util;
using System.Reflection;

namespace Veritrade2018.Controllers.Admin
{
    public class MisPlantillasController : BaseController
    {
        private readonly VeritradeAdmin.Seguridad _ws = new VeritradeAdmin.Seguridad();
        private const string COD_COUNTRY_MXD = "MXD";
        private const string COD_COUNTRY_PEB = "PEB";
        private const string PLAN_PERU_IMEX = "PERU IMEX";
        private const string PLAN_ECUADOR_IMEX = "ECUADOR IMEX";
        private const string PLAN_ESENCIAL = "ESENCIAL";
        private const string PLAN_PERU_UNO= "PERU UNO";
        private const string PLAN_ECUADOR_UNO= "ECUADOR UNO";
        private readonly string[] _CodManifiestosModificado = new[] { "PE_", "EC_", "US_" };
        private readonly string[] _CodManifiestos = new[] { "PEI", "PEE", "ECI", "ECE", "USI", "USE" };
        // GET: MisPlantillas
        [AuthorizedNoReferido]
        public ActionResult Index(string culture)
        {
            if (Session["IdUsuario"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            string idUsuario = Session["IdUsuario"].ToString();

            var codPais2 = Session["CodPais2"].ToString();
            var codPais = Session["CodPais"].ToString();
            var tipoOpe = Session["TipoOpe"].ToString();

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

            var countries = new ListaPaises().GetPaisesAdmin(culture, codPais2);//objListaPaises.GetPaisesPerfil(culture, codPais2).ToList();
            objMiPerfil.ListItemsPais = countries;

            if (codPais.Length <= 2 || codPais == COD_COUNTRY_MXD)
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

            if (Session["Plan"].ToString() == PLAN_PERU_IMEX || Session["Plan"].ToString() == PLAN_ECUADOR_IMEX)
            {
                objMiPerfil.IsEnabledCodPais = false;
            }
            else if (Session["Plan"].ToString() == PLAN_ESENCIAL || /*Session["Plan"].ToString() == PLAN_PERU_UNO||*/
                     Session["Plan"].ToString() == PLAN_ECUADOR_UNO)
            {
                //objMiPerfil.IsEnabledCodPais = false;
                ////objMiPerfil.IsEnabledImportacion = tipoOpe == "I";
                //objMiPerfil.IsCheckedImportacion = tipoOpe == "I";
                ////objMiPerfil.IsEnabledExportacion = tipoOpe == "E";
                //objMiPerfil.IscheckedExportacion = tipoOpe == "E";
                //if (Session["Plan"].ToString() != "ESENCIAL")
                //{
                //    objMiPerfil.IsEnabledImportacion = tipoOpe == "I";
                //    objMiPerfil.IsEnabledExportacion = tipoOpe == "E";
                //}
                //else
                //{
                //    objMiPerfil.IsEnabledCodPaisB = false;
                //}

                string CodPais = "", TipoOpe = "";
                Funciones.BuscaDatosPlanEspecial(Session["idUsuario"].ToString(), ref CodPais, ref TipoOpe);
                objMiPerfil.IsEnabledCodPais = false;
                objMiPerfil.IsEnabledCodPaisB = false;
                if (TipoOpe == "T")
                {
                    objMiPerfil.IsEnabledImportacion = objMiPerfil.IsEnabledExportacion = objMiPerfil.IsCheckedImportacion = true;
                    objMiPerfil.IscheckedExportacion = false;
                }
                else
                {
                    objMiPerfil.IsCheckedImportacion = tipoOpe == "I";
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

            }

            string plan = Session["Plan"].ToString();
            objMiPerfil.TipoOpe = tipoOpe;

            UpdateCountry(objMiPerfil,objMiPerfil.CodPais2Selected,objMiPerfil.CodPaisSelected);

            Session["Idioma"] = culture;

            object objMensaje = new
            {
                titulo = "Veritrade",
                mensaje = @Resources.Resources.Plan_Text,
                flagContactenos = true
            };
            string codPaisAux = codPais;

            if (_CodManifiestosModificado.Contains(codPaisAux))
            {
                codPaisAux = codPaisAux.Substring(0, 2) + tipoOpe;
            }

            List<SelectListItem> listDownloads = GetTemplates(idUsuario, objMiPerfil.TipoOpe,
                objMiPerfil.CodPais2Selected, codPaisAux, culture);
            
            AdminMyTemplate objAdminMyTemplate =
                GetDataFieldsTemplate(listDownloads.First().Value, 0, Session["CodPaisAux"].ToString(), false);
            objAdminMyTemplate.Downloads = listDownloads;

            ViewBag.nombreUsuario = Funciones.BuscaUsuario(idUsuario);
            ViewBag.tipoUsuario = Session["TipoUsuario"].ToString();
            ViewBag.plan = Session["Plan"].ToString();
            ViewData["TitlePerfilBar"] = Resources.MiPerfil.CustomizeMyExcelTemplates_Text;
            ViewData["Plan"] = Session["Plan"].ToString();
            ViewData["Mensaje"] = objMensaje;
            ViewData["objMiPerfil"] = objMiPerfil;
            ViewData["objAdminMyTemplate"] = objAdminMyTemplate;
            ViewData["IngresoComoFreeTrial"] = Session["IngresoComoFreeTrial"] != null && (bool)Session["IngresoComoFreeTrial"];
            return View();
        }

        #region MétodosAuxiliares

        List<SelectListItem> GetTemplates(string idUsuario, string tipoOpe, string  codPais2, string codPais, string idioma)
        {
            DataTable dataDownloads = AdminMyTemplateDAO.GetDataDownloads(idUsuario, tipoOpe,
                codPais2, codPais, idioma);

            List<SelectListItem> listDownloads = new List<SelectListItem>();
            if (dataDownloads != null && dataDownloads.Rows.Count > 0)
            {
                listDownloads = dataDownloads.AsEnumerable().Select(m => new SelectListItem()
                {
                    Text = m.Field<string>("Descarga"),
                    Value = m.Field<Int32>("IdDescargaCab").ToString()
                }).ToList();
            }

            return listDownloads;
        }

        private void UpdateCountry(MiPerfil objMiPerfil, string codPais2, string codPais)
        {
            if (codPais == COD_COUNTRY_PEB)
            {
                objMiPerfil.IsCheckedImportacion = true;
                objMiPerfil.IscheckedExportacion = false;
                objMiPerfil.IsEnabledExportacion = false;
            }
            else
            {
                objMiPerfil.IsEnabledExportacion = true;
            }

            objMiPerfil.CodPais2Selected = codPais2;

            string tipoOpe = objMiPerfil.IsCheckedImportacion ? "I" : "E";

            string auxCodPais = "";
            if (codPais.Length <= 2 || codPais == COD_COUNTRY_MXD)
            {
                auxCodPais = codPais;
            }
            else
            {
                auxCodPais = codPais.Substring(0, 2) + tipoOpe;
            }

            objMiPerfil.TipoOpe = tipoOpe;

            Session["CodPais2"] = codPais2;
            Session["CodPais"] = codPais;
            Session["TipoOpe"] = tipoOpe;

            Session["CodPaisAux"] = auxCodPais;
        }

        private string GetCurrentIdioma()
        {
            return Session["Idioma"] != null ? Session["Idioma"].ToString() : "es";
        }

        private List<MyTemplate> GetFieldsTemplates(string codPlantilla)
        {
            string tipoOpe = Session["TipoOpe"].ToString();
            string codPais2 = Session["CodPais2"].ToString();
            string codPais = Session["CodPaisAux"].ToString();
            string plan = Session["Plan"].ToString();
            string idioma = GetCurrentIdioma();

            List<MyTemplate> fieldsByTemplate =
                AdminMyTemplateDAO.GetDataFieldsFavorites(tipoOpe, codPais2, codPais, codPlantilla, idioma).DataTableToList<MyTemplate>();

            List<MyTemplate> myTemplates = AdminMyTemplateDAO.GetDataFields(tipoOpe, codPais2, codPais, plan, idioma).DataTableToList<MyTemplate>();

            int index = 0;
            myTemplates.ForEach(m =>
            {
                m.Index = ++index;

                var auxFields = fieldsByTemplate.Where(x => x.Campo == m.Campo).ToList();
                if (auxFields.Count == 1)
                {
                    m.CampoPersonalizado = auxFields.First().CampoFavorito;
                    m.IsChecked = true;
                }
                else
                {
                    m.CampoPersonalizado = m.CampoFavorito;
                }
            });

            return myTemplates;
        }

        private AdminMyTemplate GetDataFieldsTemplate(string codPlantilla, int indexCboPlantilla, string codPais, bool fieldsInHtml)
        {
            AdminMyTemplate objAdminMyTemplate = new AdminMyTemplate { MyTemplates = GetFieldsTemplates(codPlantilla) };

            if (fieldsInHtml)
                objAdminMyTemplate.MyFieldsTemplateInHtml = RenderViewToString(this.ControllerContext, "GridViews/TableRowView", objAdminMyTemplate);

            objAdminMyTemplate.IsVisibleFormTemplate = indexCboPlantilla != 0;
            objAdminMyTemplate.IsVisibleBtnNewTemplate = true;
                //AdminMyTemplateDAO.VlidationTemplate(Session["IdUsuario"].ToString(), codPais, Session["TipoOpe"].ToString());

            return objAdminMyTemplate;
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
        #endregion

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult DescargasChange(string codPlantilla)
        {
            string tipoOpe = Session["TipoOpe"].ToString();
            string auxCodPais = Session["CodPaisAux"].ToString();
            string codPais2 = Session["CodPais2"].ToString();
            string codPaisT = auxCodPais;
            if (codPais2 == "4UE")
                codPaisT = "UE" + auxCodPais;

            string idUsuario = Session["IdUsuario"].ToString();

            string idDownloadDefault = FuncionesBusiness.BuscaDescargaDefault(idUsuario, codPaisT, tipoOpe);

            bool IsCheckedDefault = codPlantilla == idDownloadDefault;


            AdminMyTemplate objAdminMyTemplate = new AdminMyTemplate {MyTemplates = GetFieldsTemplates(codPlantilla)};

            string rowsFieldsTemplate =
                RenderViewToString(this.ControllerContext, "GridViews/TableRowView", objAdminMyTemplate);

            return Json(new
            {
                rowsFieldsTemplate,
                IsCheckedDefault
            });
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult SaveTemplate(int indexPlantilla, string codPlantilla, string textTemplate,
            bool isCheckedDefault, string fieldsTemplates, string checkFieldsTemplates,
            string fieldsCustomizeTemplates)
        {
            
            string tipoOpe = Session["TipoOpe"].ToString();
            string auxCodPais = Session["CodPaisAux"].ToString();
            string codPais2 = Session["CodPais2"].ToString();
            string idUsuario = Session["IdUsuario"].ToString();

            string codPaisT = auxCodPais;
            if (codPais2 == "4UE")
                codPaisT = "UE" + auxCodPais;

            textTemplate = (textTemplate.Replace("[Default]", "")).Trim();
            textTemplate = (textTemplate.Replace("[Por Defecto]", "")).Trim();
            AdminMyTemplate objAdminMyTemplate = new AdminMyTemplate();
            objAdminMyTemplate.Downloads = null;
            if (indexPlantilla == 0)
            {
                
                codPlantilla = AdminMyTemplateDAO.CreateTemplate(idUsuario, tipoOpe, codPaisT,
                    FuncionesBusiness.CambiaNombre(textTemplate.Trim()), isCheckedDefault);

                AdminMyTemplateDAO.SaveFieldsNewTemplate(codPlantilla, fieldsTemplates,checkFieldsTemplates, fieldsCustomizeTemplates);

               
                indexPlantilla = -9; //indica que se ha generado un nueva plantilla

            }
            else
            {
                AdminMyTemplateDAO.UpdateTemplate(idUsuario, tipoOpe, codPaisT,FuncionesBusiness.CambiaNombre(textTemplate.Trim()), codPlantilla, isCheckedDefault);
                AdminMyTemplateDAO.SaveFieldsUpdate(codPlantilla, fieldsTemplates, checkFieldsTemplates, fieldsCustomizeTemplates);
            }

            objAdminMyTemplate = GetDataFieldsTemplate(codPlantilla, indexPlantilla, auxCodPais, true);

            objAdminMyTemplate.Downloads = GetTemplates(Session["IdUsuario"].ToString(), tipoOpe,
                        codPais2, Session["CodPaisAux"].ToString(), GetCurrentIdioma());
            objAdminMyTemplate.CurrentCodTemplate = codPlantilla;
            
            return Json(new
            {
                objAdminMyTemplate
            });
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult DeleteTemplate(string codigo)
        {
            AdminMyTemplateDAO.DeleteDescargas(codigo);
            string tipoOpe = Session["TipoOpe"].ToString();
            string codPais2 = Session["CodPais2"].ToString();
            AdminMyTemplate objAdminMyTemplate = new AdminMyTemplate();
            objAdminMyTemplate.Downloads = GetTemplates(Session["IdUsuario"].ToString(), tipoOpe,
                codPais2, Session["CodPaisAux"].ToString(), GetCurrentIdioma());

            return Json(new
            {
                objAdminMyTemplate
            });
        }


        [SessionExpireFilter]
        [HttpPost]
        public JsonResult TipoOpeChange(bool isCheckedExportacion)
        {
            string codPaisTemporal = Session["CodPais"].ToString();

            string idioma = GetCurrentIdioma();
            string codPais2 = Session["CodPais2"].ToString();

            ListaPaises objListaPaises = new ListaPaises();
            var countries = new ListaPaises().GetPaisesAdmin(idioma, codPais2);//objListaPaises.GetPaisesPerfil(idioma, codPais2).ToList();

            if (isCheckedExportacion && codPaisTemporal == "PEB")
            {
                codPaisTemporal = "PE";
            }

            MiPerfil objMiPerfil = new MiPerfil
            {
                CodPais2Selected = codPais2,
                ListItemsPais = countries
            };
            if (countries.FirstOrDefault(x => x.Value == codPaisTemporal) == null)
            {
                codPaisTemporal = countries.First().Value;
                objMiPerfil.CodPaisSelected = codPaisTemporal;
            }
            else
            {
                objMiPerfil.CodPaisSelected = codPaisTemporal;
            }

            objMiPerfil.IsCheckedImportacion = !isCheckedExportacion;
            objMiPerfil.IscheckedExportacion = isCheckedExportacion;

            //Session["CodPaisAux"] = objMiPerfil.CodPaisSelected;
            UpdateCountry(objMiPerfil, objMiPerfil.CodPais2Selected, objMiPerfil.CodPaisSelected);

            string codPaisAux = objMiPerfil.CodPaisSelected;
            ValidaCodPaisManif(ref codPaisAux, objMiPerfil.TipoOpe);

            List<SelectListItem> listDownloads = GetTemplates(Session["IdUsuario"].ToString(), objMiPerfil.TipoOpe,
                objMiPerfil.CodPais2Selected, codPaisAux, idioma);

            AdminMyTemplate objAdminMyTemplate =
                GetDataFieldsTemplate(listDownloads.First().Value, 0, Session["CodPaisAux"].ToString(), true);
            objAdminMyTemplate.Downloads = listDownloads;

            return Json(new
            {
                objAdminMyTemplate,
                objMiPerfil
            });
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult Pais2Change(string codPais2)
        {
            string tipoOpe = Session["TipoOpe"].ToString();
            string idioma = GetCurrentIdioma();
            ListaPaises objListaPaises = new ListaPaises();

            //var countries = codPais2.Equals("6RES")
            //    ? new ListaPaises().GetPaisesAdmin(idioma, codPais2)
            //    : objListaPaises.GetPaisesPerfil(idioma, codPais2).ToList();

            var countries = new ListaPaises().GetPaisesAdmin(idioma, codPais2);


           /* if (codPais2.Equals("6RES"))
            {
                IEnumerable<SelectListItem> countries = new ListaPaises().GetPaisesAdmin(idioma, codPais2);
            }
            else
            {
                List<SelectListItem> countries = objListaPaises.GetPaisesPerfil(idioma, codPais2).ToList();
            }*/

            //

            string textCodPais = countries.FirstOrDefault()?.Text;
            string auxCodPais2 = codPais2;
            string auxCodPais = "";

            object objMensaje = ValidaPais2(ref auxCodPais2, ref auxCodPais, textCodPais);

            MiPerfil objMiPerfil = new MiPerfil();
            objMiPerfil.CodPais2Selected = auxCodPais2;
            if (objMensaje != null)
            {
                /*List<SelectListItem> countries2 = objListaPaises.GetPaises2(idioma).ToList();

                if (countries2.FirstOrDefault(x => x.Value == auxCodPais2) == null)
                {
                    auxCodPais2 = countries2.First().Value;
                    objMiPerfil.CodPais2Selected = auxCodPais2;
                }*/
                //countries= codPais2.Equals("6RES")?
                //    new ListaPaises().GetPaisesAdmin(idioma, auxCodPais2):
                //    objListaPaises.GetPaisesPerfil(auxCodPais2, idioma).ToList(); 
                auxCodPais = Session["CodPais"].ToString();
                auxCodPais2 = new ListaPaises().BuscarCodPais2(auxCodPais);
                auxCodPais2 = (auxCodPais2 == "") ? "1LAT" : auxCodPais2;
                countries = new ListaPaises().GetPaisesAdmin(idioma, auxCodPais2);
                objMiPerfil.CodPais2Selected = auxCodPais2;
                Session["CodPais2"] = auxCodPais2;

                Session["CodPais"] = auxCodPais;
            }

            if (countries.FirstOrDefault(x => x.Value == auxCodPais) == null)
            {
                auxCodPais = countries.First().Value;
            }

            objMiPerfil.CodPaisSelected = auxCodPais;
            objMiPerfil.ListItemsPais = countries;
            objMiPerfil.IsCheckedImportacion = tipoOpe == "I";
            UpdateCountry(objMiPerfil, objMiPerfil.CodPais2Selected, objMiPerfil.CodPaisSelected);

            List<SelectListItem> listDownloads = GetTemplates(Session["IdUsuario"].ToString(), objMiPerfil.TipoOpe,
                objMiPerfil.CodPais2Selected, objMiPerfil.CodPaisSelected, idioma);

            AdminMyTemplate objAdminMyTemplate =
                GetDataFieldsTemplate(listDownloads.First().Value, 0, Session["CodPaisAux"].ToString(), true);
            objAdminMyTemplate.Downloads = listDownloads;

            return Json(new
            {
                objMensaje,
                objAdminMyTemplate,
                objMiPerfil
            });
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult PaisChange(string codPais, string textCodPais)
        {
            string tipoOpe = Session["TipoOpe"].ToString();
            string auxCodPais2 = Session["CodPais2"].ToString();
            string auxCodPais2Actual = auxCodPais2;
            string auxCodPais = codPais;
            string antCodPais = Session["CodPais"].ToString();

            MiPerfil objMiPerfil = new MiPerfil();

            object objMensaje = ValidaPais(ref auxCodPais2, ref auxCodPais, textCodPais);

            objMiPerfil.CodPais2Selected = auxCodPais2;
            objMiPerfil.CodPaisSelected = auxCodPais;

            string idioma = GetCurrentIdioma();
            if (objMensaje != null)
            {
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
                    auxCodPais = countries2.First().Value;
                    objMiPerfil.CodPaisSelected = auxCodPais;
                }
            }
            objMiPerfil.IsCheckedImportacion = tipoOpe == "I";
            UpdateCountry(objMiPerfil, objMiPerfil.CodPais2Selected, objMiPerfil.CodPaisSelected);

            string codPaisAux = objMiPerfil.CodPaisSelected;
            ValidaCodPaisManif(ref codPaisAux, tipoOpe);

            List<SelectListItem> listDownloads = GetTemplates(Session["IdUsuario"].ToString(), objMiPerfil.TipoOpe,
                objMiPerfil.CodPais2Selected, codPaisAux, idioma);

            AdminMyTemplate objAdminMyTemplate =
                GetDataFieldsTemplate(listDownloads.First().Value, 0, Session["CodPaisAux"].ToString(), true);
            objAdminMyTemplate.Downloads = listDownloads;

            return Json(new
            {
                objMensaje,
                objAdminMyTemplate,
                objMiPerfil
            });

        }

        private void ValidaCodPaisManif(ref string codPais, string tipoOpe)
        {
            if (codPais.Contains("_"))
            {
                codPais = codPais.Replace("_", tipoOpe);
            }
            else if (_CodManifiestos.Contains(codPais))
            {
                //_CodManifiesto = CodPais;
                codPais = codPais.Substring(0, 2) + "_";
            }
        }


        [SessionExpireFilter]
        [HttpPost]
        public JsonResult EnviarMensajeContactenos(string Mensaje)
        {
            EnviarSolicitud(Mensaje);

            return Json(new { Respuesta = "OK" });
        }

        private void EnviarSolicitud(string Mensaje)
        {
            VeritradeServicios.ServiciosCorreo wsc = new VeritradeServicios.ServiciosCorreo();
            try
            {
#if DEBUG
                string DireccionIP = Properties.Settings.Default.IP_Debug;
#else
                string DireccionIP = Request.ServerVariables["REMOTE_ADDR"];
#endif

                string CodPais = "";
                string Ubicacion = _ws.BuscaUbicacionIP2(DireccionIP, ref CodPais);

                string sIdUsuario = Session["IdUsuario"].ToString();

                string IdSolicitud =
                    FuncionesBusiness.CreaSolicitud("CONTACTENOS2", sIdUsuario, Mensaje, DireccionIP, Ubicacion);

                string EmailEnvio1 = "", EmailEnvioNombre1 = "", EmailEnvioPassword1 = "";
                Funciones.BuscaDatosCorreoEnvio("4", ref EmailEnvio1, ref EmailEnvioNombre1, ref EmailEnvioPassword1);


                string FromName, FromEmail, ToName, ToEmail, Subject, URL;

                FromName = EmailEnvioNombre1;
                FromEmail = EmailEnvio1;
                ToName = "VERITRADE";
                ToEmail = "info@veritrade-ltd.com";


                Subject = "Solicitud de Contacto o Soporte - Usuario";
                URL = Properties.Settings.Default.FrontEnd + "/Correo/Index?Opcion=SOLICITUD&IdSolicitud=" +
                      IdSolicitud;


                wsc.EnviaEmail3(FromName, FromEmail, ToName, ToEmail, null, null, null, null, Subject, URL, EmailEnvio1,
                    EmailEnvioPassword1);
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult NewTemplateClick(string codPlantilla)
        {
            string idUsuario = Session["IdUsuario"].ToString();
            var codPais = Session["CodPais"].ToString();
            var tipoOpe = Session["TipoOpe"].ToString();
            object objMensaje = null;
            /*string plan1 = Session["Plan"].ToString();
            string plan2 = Enums.Planes.PERU_UNO.GetDn();*/
            if (//Session["Plan"].ToString().ToLower() == Enums.Planes.PERU_UNO.GetDn().ToLower() &&
                !FuncionesBusiness.CantidadPlantillasUsuario(idUsuario,tipoOpe,codPais))
            {
                
                string mensaje = "No puede crear más plantillas, porque supera el límite de acuerdo al plan contratado.";
                if (GetCurrentIdioma() == "en")
                    mensaje = "You cannot create more templates, because it exceeds the limit according to the contracted plan.";

                objMensaje = new
                {
                    titulo = "Veritrade",
                    mensaje,
                    flagContactenos = false
                };

                return Json(new
                {
                    objMensaje
                });
            }

            AdminMyTemplate objAdminMyTemplate =
                GetDataFieldsTemplate(codPlantilla, 0, Session["CodPaisAux"].ToString(), true);
            return Json(new
            {
                objAdminMyTemplate,
                objMensaje
            });
        }

        [SessionExpireFilter]
        [HttpPost]
        public JsonResult CancelTemplate(string codPlantilla, int indexPlantilla)
        {
            AdminMyTemplate objAdminMyTemplate =
                GetDataFieldsTemplate(codPlantilla, indexPlantilla, Session["CodPaisAux"].ToString(), true);
            return Json(new
            {
                objAdminMyTemplate
            });
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