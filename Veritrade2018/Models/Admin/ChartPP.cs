using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Veritrade2018.Models.Admin
{
    //Chart Product Profile
    public class ChartPP
    {
        public string Title { get; set; }
        public List<string> Categories { get; set; }

        public List<ChartSeriePP> Series { get; set; }

        public ChartPP()
        {
            Categories = new List<string>();
            Series = new List<ChartSeriePP>();
        }
    }
}