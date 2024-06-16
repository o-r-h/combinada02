using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Veritrade2018.Models.Admin
{
    //Chart Product Profile
    public class ChartSeriePP
    {
        public string name { get; set; }
        public List<Decimal> data { get; set; }
        public ChartSeriePP()
        {
            data = new List<decimal>();
        }
    }
}