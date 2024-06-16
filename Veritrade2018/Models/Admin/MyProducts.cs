using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Veritrade2018.Models.Admin
{
    public class MyProducts
    {
        public bool ExistImporterOrExporter { get; set; }

        public Chart ChartImporterOrExporter { get; set; }

        public string HtmlTableImporterOrExporter { get; set; }

        public string IdImporterOrExporter { get; set; }

        public int TotalPagesImporterOrExporter { get; set; }

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

        public int CountVisiblePages { get; set; }
        
        public MyProducts()
        {
            IdImporterOrExporter = "ImporterOrExporter";
            IdOriginOrDestinationCountry = "OriginOrDestinationCountry";
            IdSupplierOrImporterExp = "SupplierOrImporterExp";
            CountVisiblePages = 10;
        }

    }
}