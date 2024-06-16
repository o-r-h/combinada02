using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Veritrade2018.Models.Admin
{
    public class TabDataTabMy
    {
        public string Filtro { set; get; }
        public GridHeadTabMy GridHead;
        public List<GridRowTabMy> ListRows { set; get; }
        public string CiFoFobTotal { set; get; }
        public string CodPais { get; set; }
        public int TotalPaginasTab { set; get; }
        public TabDataTabMy()
        {
            GridHead = new GridHeadTabMy();
            ListRows = new List<GridRowTabMy>();
        }
    }
}