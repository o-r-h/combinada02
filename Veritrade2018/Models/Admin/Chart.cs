using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Veritrade2018.Models.Admin
{
    public class Chart
    {
        public string Title { get; set; }
        public List<string> Categories { get; set; }

        public List<ChartSerie> Series { get; set; }

        public string TitleContainer { get; set; }

        public List<PieData> PieDatas { get; set; }

        public long TickInterval { get; set; }
        public List<ChartColumn> Column { get; set; }

        public Chart()
        {
            Categories = new List<string>();
            Series = new List<ChartSerie>();
            Column = new List<ChartColumn>();
            PieDatas = new List<PieData>();
        }
    }
}