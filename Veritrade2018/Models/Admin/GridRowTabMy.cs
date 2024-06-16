using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Veritrade2018.Models.Admin
{
    public class GridRowTabMy
    {
        public string Id { set; get; }
        public string Descripcion { set; get; }
        public string CiFoFobTot { set; get; }

        public string CiFoFobPor { set; get; }

        public string CodPartida { get; set; }
        public string numInformaColombia { get; set; }
        public bool IsVisibleLupaPartida { get; set; }

        public GridRowTabMy()
        {

        }
    }
}