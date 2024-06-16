using Dapper;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Veritrade2018.Helpers;

namespace Veritrade2018.Jobs
{
    public class EnvioAlertasJob : IJob
    {
        public async void Execute(IJobExecutionContext context)
        {
            //Exception exceptionPrueba = new Exception("Ejecutando envío de alertas");
            //ExceptionUtility.LogException(exceptionPrueba, "Ejecutando envío de alertas");
            Console.WriteLine("Ejecutando envío de alertas");
            Alertas alerta = new Alertas();
            //Se envían alertas automáticas a usuarios por país habilitado para esta función
            try {
                //await alerta.EnviarAlertaUsuariosPorPais();
            }
            catch(Exception ex)
            {
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }
        }
    }
}