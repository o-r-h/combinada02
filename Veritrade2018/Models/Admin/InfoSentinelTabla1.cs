using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Veritrade2018.Models.Admin
{
    public class InfoSentinelTabla1
    {
        public string FechaProceso { get; set; }
        public string Score { get; set; }
        public string DeudaTotal { get; set; }
        public string SemaforoSemanaActual { get; set; }

        public string SemaforoSemanaPrevio { get; set; }
        public string DeudaTributaria { get; set; }
        public string DeudaLaboral { get; set; }

        public string DeudaImpaga { get; set; }

        public string DeudaProtestos { get; set; }

        public string ScorePromedio { get; set; }

        public InfoSentinelTabla1()
        {

        }
    }
}