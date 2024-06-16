using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Veritrade2018.Models.Admin
{
    public class InfoSentinelTabla2
    {
        public string NroEntidades { get; set; }
        public string CalificacionSBSMicrof {  get; set; }

        public string DeudaSBSMicrof { get; set; }

        public string DeudaSBSMicrofVencida { get; set; }
        public string TotalTarjetasCtasAnuladasCerradas { get; set; }

        public string Veces24m { get; set; }

        public string EstadoDomicilioFiscal { get; set; }

        public string CondicionDomicioFiscal { get; set; }
        public string TotalReportesNegativos { get; set; }

        public InfoSentinelTabla2()
        {

        }
    }
}