using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Veritrade2018.Models.Admin
{
    public class ChartSerie
    {
        public string name { get; set; }
        public List<Decimal> data { get; set; }
        
        public ChartSerie()
        {
            data = new List<decimal>();            
        }
    }
}