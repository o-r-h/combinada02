using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Veritrade2018.Models.Admin
{
    public class MyTemplate
    {
        public int Index { get; set; }
        public int NroCampo { get; set; }
        public string Campo { get; set; }
        public string CampoFavorito { get; set; }

        public bool IsChecked { get; set; }

        public string CampoPersonalizado { get; set; }

        public MyTemplate()
        {
        }
    }
}