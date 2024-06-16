using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Veritrade2018.Helpers;
using Veritrade2018.Util;

namespace Veritrade2018.Models.Admin
{
    public class MySearchForm
    {
        public string FilterDescription { get; set; }
        public bool IsCheckedImportacion { get; set; }
        public bool IscheckedExportacion { get; set; }

        public IEnumerable<SelectListItem> ListItemsPais2 { get; set; }

        public string CodPais2Selected { get; set; }

        public IEnumerable<SelectListItem> ListItemsOpcion { get; set; }

        public IEnumerable<SelectListItem>  ListItemsPais { get; set; }
        public string CodPaisSelected { get; set; }

        public IEnumerable<SelectListItem> ListItemsMyFilters { get; set; }

        public FiltroPeriodo FiltroPeriodo { get; set; }

        public bool EnabledBtnVerGraficos { get; set; }

        public string TipoOperacion { get; set; }

        public bool IsVisibleRdbUsd { get; set; }

        public bool IsVisibleRdbUnid { get; set; }

        public MySearchForm()
        {
            TipoOperacion = "I";
            ListItemsPais2 = new List<SelectListItem>();
            ListItemsOpcion = new List<SelectListItem>();
            ListItemsPais = new List<SelectListItem>();
            ListItemsMyFilters = new List<SelectListItem>();
        }

        public IEnumerable<SelectListItem> GetPeriod(string idioma, string TipoUsuario)
        {
            var lista = new List<SelectListItem>();
            if(TipoUsuario != "Free Trial")
            {
                if (idioma == "es")
                {
                    lista.Add(new SelectListItem { Text = @"Meses", Value = "MESES" });
                    lista.Add(new SelectListItem { Text = @"Ultimos 12 meses", Value = "ULT12" });
                    lista.Add(new SelectListItem { Text = @"Enero a ", Value = "YTD" });
                    lista.Add(new SelectListItem { Text = @"Años", Value = "AÑOS" });
                }
                else
                {
                    lista.Add(new SelectListItem { Text = @"Months", Value = "MESES" });
                    lista.Add(new SelectListItem { Text = @"Last 12 months", Value = "ULT12" });
                    lista.Add(new SelectListItem { Text = @"January to ", Value = "YTD" });
                    lista.Add(new SelectListItem { Text = @"Years", Value = "AÑOS" });
                }
            }
            else
            {
                if (idioma == "es")
                {
                    lista.Add(new SelectListItem { Text = @"Meses", Value = "MESES" });
                }
                else
                {
                    lista.Add(new SelectListItem { Text = @"Months", Value = "MESES" });
                }
            }
            return lista;
        }

        public List<SelectListItem> GetCountries(string codPais2, string idioma)
        {
            /*
            var lista = new List<SelectListItem>();

            switch (codPais2)
            {
                case "1LAT":
                    if (idioma == "es")
                    {
                        lista.Add(new SelectListItem { Text = @"Argentina", Value = "AR" });
                        lista.Add(new SelectListItem { Text = @"Bolivia", Value = "BO" });
                        lista.Add(new SelectListItem { Text = @"Brasil", Value = "BR" });
                        lista.Add(new SelectListItem { Text = @"Chile", Value = "CL" });
                        lista.Add(new SelectListItem { Text = @"Colombia", Value = "CO" });
                        lista.Add(new SelectListItem { Text = @"Costa Rica", Value = "CR" });
                        lista.Add(new SelectListItem { Text = @"Ecuador", Value = "EC" });
                        lista.Add(new SelectListItem { Text = @"México", Value = "MX" });
                        lista.Add(new SelectListItem { Text = @"México Detalle Marítimo", Value = "MXD" });
                        lista.Add(new SelectListItem { Text = @"Panamá", Value = "PA" });
                        lista.Add(new SelectListItem { Text = @"Paraguay", Value = "PY" });
                        lista.Add(new SelectListItem { Text = @"Perú", Value = "PE" });
                        lista.Add(new SelectListItem { Text = @"Uruguay", Value = "UY" });
                    }
                    else
                    {
                        lista.Add(new SelectListItem { Text = @"Argentina", Value = "AR" });
                        lista.Add(new SelectListItem { Text = @"Bolivia", Value = "BO" });
                        lista.Add(new SelectListItem { Text = @"Brazil", Value = "BR" });
                        lista.Add(new SelectListItem { Text = @"Chile", Value = "CL" });
                        lista.Add(new SelectListItem { Text = @"Colombia", Value = "CO" });
                        lista.Add(new SelectListItem { Text = @"Costa Rica", Value = "CR" });
                        lista.Add(new SelectListItem { Text = @"Ecuador", Value = "EC" });
                        lista.Add(new SelectListItem { Text = @"Mexico", Value = "MX" });
                        lista.Add(new SelectListItem { Text = @"Mexico Maritime Detail", Value = "MXD" });
                        lista.Add(new SelectListItem { Text = @"Panama", Value = "PA" });
                        lista.Add(new SelectListItem { Text = @"Paraguay", Value = "PY" });
                        lista.Add(new SelectListItem { Text = @"Peru", Value = "PE" });
                        lista.Add(new SelectListItem { Text = @"Uruguay", Value = "UY" });
                    }
                    break;
                case "2US":
                    lista.Add(new SelectListItem { Text = @"USA", Value = "US" });
                    break;
                case "3CN":
                    lista.Add(new SelectListItem { Text = @"China", Value = "CN" });
                    break;
                case "4UE":
                    lista = getPaisesDatatableToListItem(FuncionesBusiness.CargaPaisesUE(idioma));
                    break;
                case "5IN":
                    lista.Add(new SelectListItem { Text = @"India", Value = "IN" });
                    break;
            }

            return lista;
            */
            return new ListaPaises().GetPaisesAdmin(idioma, codPais2, k_excludePais: Enums.VarId.EXCLUDE_PAIS_MP.GetDn()).ToList();
        }

        public List<SelectListItem> GetCountriesAlert(string codPais2, string idioma)
        {            
            return new ListaPaises().GetPaisesAdmin(idioma, codPais2, k_excludePais: Enums.VarId.EXCLUDE_PAIS_MP.GetDn()).ToList();
        }

        private  List<SelectListItem> getPaisesDatatableToListItem(DataTable dt)
        {
            var lista = new List<SelectListItem>();
            foreach (DataRow dr in dt.Rows)
            {
                lista.Add(new SelectListItem { Text = dr["Pais"].ToString(), Value = dr["IdPais"].ToString() });
            }
            return lista;
        }

        public IEnumerable<SelectListItem> GetCountries2MyCompanies(string idioma)
        {
            //var lista = new List<SelectListItem>();

            //lista.Add(idioma == "es"
            //    ? new SelectListItem {Text = @"Latinoamérica", Value = "1LAT"}
            //    : new SelectListItem {Text = @"Latin America", Value = "1LAT"});
            //return lista;
            return new ListaPaises().GetPaises2(idioma, Enums.VarId.EXCLUDE_REG_MC.GetDn());
        }
        public IEnumerable<SelectListItem> GetCountriesMyCompanies(string idioma, string codPais2 = "1LAT")
        {
            //var lista = new List<SelectListItem>();
            // if (idioma == "es")
            // {
            //     lista.Add(new SelectListItem { Text = @"Argentina", Value = "AR" });
            //     lista.Add(new SelectListItem { Text = @"Bolivia", Value = "BO" });
            //     lista.Add(new SelectListItem { Text = @"Chile", Value = "CL" });
            //     lista.Add(new SelectListItem { Text = @"Colombia", Value = "CO" });
            //     lista.Add(new SelectListItem { Text = @"Costa Rica", Value = "CR" });
            //     lista.Add(new SelectListItem { Text = @"Ecuador", Value = "EC" });
            //     lista.Add(new SelectListItem { Text = @"México Detalle Marítimo", Value = "MXD" });
            //     lista.Add(new SelectListItem { Text = @"Panamá", Value = "PA" });
            //     lista.Add(new SelectListItem { Text = @"Perú", Value = "PE" });
            //     lista.Add(new SelectListItem { Text = @"Paraguay", Value = "PY" });
            //     lista.Add(new SelectListItem { Text = @"Uruguay", Value = "UY" });
            // }
            // else
            // {
            //     lista.Add(new SelectListItem { Text = @"Argentina", Value = "AR" });
            //     lista.Add(new SelectListItem { Text = @"Bolivia", Value = "BO" });
            //     lista.Add(new SelectListItem { Text = @"Chile", Value = "CL" });
            //     lista.Add(new SelectListItem { Text = @"Colombia", Value = "CO" });
            //     lista.Add(new SelectListItem { Text = @"Costa Rica", Value = "CR" });
            //     lista.Add(new SelectListItem { Text = @"Ecuador", Value = "EC" });
            //     lista.Add(new SelectListItem { Text = @"Mexico Maritime Detail", Value = "MXD" });
            //     lista.Add(new SelectListItem { Text = @"Panama", Value = "PA" });
            //     lista.Add(new SelectListItem { Text = @"Peru", Value = "PE" });
            //     lista.Add(new SelectListItem { Text = @"Paraguay", Value = "PY" });
            //     lista.Add(new SelectListItem { Text = @"Uruguay", Value = "UY" });
            // }
            // return lista;
            return new ListaPaises().GetPaisesAdmin(idioma, codPais2, k_excludePais: Enums.VarId.EXCLUDE_PAIS_MC.GetDn()).ToList();
        }


    }
}