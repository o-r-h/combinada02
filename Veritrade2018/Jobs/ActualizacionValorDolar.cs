

using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Veritrade2018.Helpers;
using Veritrade2018.Models;

namespace Veritrade2018.Jobs
{

    
    public class ActualizacionValorDolar : IJob
    {
        private static readonly HttpClient client = new HttpClient();
        public async void Execute(IJobExecutionContext context)
        {
            Console.WriteLine("Obteniendo valor Dolar para hoy");
            //Se envían alertas automáticas a usuarios por país habilitado para esta función
            try
            {
                await obtenerValorDolar(DateTime.Today);
            }
            catch (Exception ex)
            {
                ExceptionUtility.LogException(ex, "ActualizacionValorDolar.Execute");
            }
            
        }

        private static async Task obtenerValorDolar(DateTime date)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");
            string key = Properties.Settings.Default.ApiKey_UF;
            string url = $"https://api.sbif.cl/api-sbifv3/recursos_api/dolar?apikey={key}&formato=json";
            using (Stream s = client.GetStreamAsync(url).Result)
            using (StreamReader sr = new StreamReader(s))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                var settings = new JsonSerializerSettings()
                {
                    DateFormatString = "yyyy-MM-dd"
                };
                settings.Culture = new CultureInfo("es-ES", false);
                JsonSerializer serializer = JsonSerializer.CreateDefault(settings);
                ValoresDolar valores = serializer.Deserialize<ValoresDolar>(reader);
                ValorDolar.guardarValores(valores.Dolares);
            }

        }
    }

    public class ValoresDolar
    {
        public IList<ValorDolar> Dolares { get; set; }
    }


}