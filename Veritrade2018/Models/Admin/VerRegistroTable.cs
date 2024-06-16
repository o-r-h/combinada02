using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;

namespace Veritrade2018.Models.Admin
{
    public class VerRegistroTable
    {
        public string TipoOpe { get; set; }
        public VerRegistroTableHead  VerRegistroTableHead { get; set; }
        public List<VerRegistroTableRow> VerRegistroTableRows { get; set; }

        public IPagedList<VerRegistroTableRow> PagedListTableRows { get; set; }

        public int TotalPaginas { set; get; }
        public MiBusqueda MiBusqueda { get; set; }

        public VerRegistroTable()
        {
            MiBusqueda = new MiBusqueda();
            VerRegistroTableHead = new VerRegistroTableHead();
            VerRegistroTableRows = new List<VerRegistroTableRow>();
        }
    }
}