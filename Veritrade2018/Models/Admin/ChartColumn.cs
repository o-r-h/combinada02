using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Veritrade2018.Models.Admin
{
    public class ChartColumn
    {
        public List<ChartColumnPP> data { get; set; }
        public ChartColumn()
        {
            data = new List<ChartColumnPP>();
        }
    }
}