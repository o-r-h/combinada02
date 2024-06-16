using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Veritrade2018.Helpers;
using Veritrade2018.Jobs;

namespace Veritrade2018.JobScheduler
{
    public class JobScheduler
    {
        public static void Start()
        {
            //Exception exceptionPrueba = new Exception("Registro de jobs");
            //ExceptionUtility.LogException(exceptionPrueba, "registrando los jobs");
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();

           scheduler.Start();

           IJobDetail job = JobBuilder.Create<EnvioAlertasJob>().Build();

           string configuracionCron = Properties.Settings.Default.Configuracion_Cron;

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("EnvioAlertasJob", "EnvioAlertas")
                .WithSchedule(CronScheduleBuilder.CronSchedule(configuracionCron))
                .Build();
            scheduler.ScheduleJob(job, trigger);

            IJobDetail jobDolar = JobBuilder.Create<ActualizacionValorDolar>().Build();

            string configuracionCronDolar = Properties.Settings.Default.Configuracion_Cron_Dolar;

            ITrigger triggerDolar = TriggerBuilder.Create()
                .WithIdentity("ValorDolarJob", "ActualizacionValorDolar")
                .WithSchedule(CronScheduleBuilder.CronSchedule(configuracionCronDolar))
                .Build();
            scheduler.ScheduleJob(jobDolar, triggerDolar);

        }
    }
}