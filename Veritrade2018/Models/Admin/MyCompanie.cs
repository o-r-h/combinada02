using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Veritrade2018.Models.Admin
{

    public class MyCompanie
    {
        public int CountVisiblePages { get; set; }

        public string HtmlTableProducts { get; set; }
        public int TotalPaginasProducts { get; set; }

        public string IdProducts { get; set; }

        public bool ExistOriginOrDestinationCountry { get; set; }
        public Chart ChartOriginOrDestinationCountry { get; set; }

        public string HtmlTableOriginOrDestinationCountry { get; set; }

        public string IdOriginOrDestinationCountry { get; set; }

        public int TotalPagesOriginOrDestinationCountry { get; set; }

        public bool ExistSupplierOrImporterExp { get; set; }
        public Chart ChartSupplierOrImporterExp { get; set; }

        public string HtmlTableSupplierOrImporterExp { get; set; }

        public string IdSupplierOrImporterExp { get; set; }

        public int TotalPagesSupplierOrImporterExp { get; set; }

        public MyCompanie()
        {
            IdProducts = "Partida";
            IdOriginOrDestinationCountry = "OriginOrDestinationCountry";
            IdSupplierOrImporterExp = "SupplierOrImporterExp";
            CountVisiblePages = 10;
        }
    }
}