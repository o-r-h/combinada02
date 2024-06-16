using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Veritrade2018.Models.Admin
{
    public class InfoSentinel
    {
        public string RazonSocial { get; set; }
        public string Documento { get; set; }
        public string FechaInicioActvidades { get; set; }
        public string TipoActividad { get; set; }

        public string[] Semaforos { set; get; }

        //public List<string> LiteralMes { set; get; }

        public InfoSentinelTabla1 InfoSentinelTabla1 { set; get; }

        public InfoSentinelTabla2 InfoSentinelTabla2 { get; set; }

        public InfoSentinel()
        {
            InfoSentinelTabla1 = new InfoSentinelTabla1();
            InfoSentinelTabla2 = new InfoSentinelTabla2();
            //LiteralMes = new List<string>();
        }
    }

    
}