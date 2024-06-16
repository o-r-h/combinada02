using System.Collections.Generic;
using System.Web.Mvc;

namespace Veritrade2018.Models.Admin
{
    public class MiPerfil
    {
        public bool IsCheckedImportacion { get; set; }

        public bool IsEnabledImportacion { get; set; }

        public bool IscheckedExportacion { get; set; }

        public bool IsEnabledExportacion { get; set; }

        public IEnumerable<SelectListItem> ListItemsPais2 { get; set; }

        public string CodPais2Selected { get; set; }

        public IEnumerable<SelectListItem> ListItemsPais { get; set; }

        public string CodPaisSelected { get; set; }

        public bool IsEnabledCodPais { get; set; }

        public bool IsEnabledCodPaisB { get; set; }

        public string TipoOpe { get; set; }

        public bool IsAlertasDeshabilitadas { get; set; }

        public MyFavoriteAndGroup MyFavoriteAndGroup { get; set; }

        public MiPerfil()
        {
            IsCheckedImportacion = true;
            IscheckedExportacion = false;
            IsEnabledImportacion = true;
            IsEnabledExportacion = true;            

            IsEnabledCodPais = true;
            IsEnabledCodPaisB = true;
            TipoOpe = "I";
        }
    }
}