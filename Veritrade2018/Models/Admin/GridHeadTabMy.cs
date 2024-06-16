using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Veritrade2018.Models.Admin
{
    public class GridHeadTabMy
    {
        public string Descripcion { set; get; }
        public string CiFoFobTot { set; get; }
        public string CiFoFobPor { set; get; }

        public string TitleColumnTotalReg { get; set; }

        public string CodPartida { get; set; }
        public bool mostrarColInformaCo { get; set; }

        public GridHeadTabMy()
        {
            CiFoFobPor = "%";
            TitleColumnTotalReg = Resources.Resources.Grid_Column_SeeRecords;
            mostrarColInformaCo = false;
        }
    }
}