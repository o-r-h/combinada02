using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using Dapper;
using Mustache;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Veritrade2018.Helpers;
using System.Reflection;
using System.Globalization;

namespace Veritrade2018.Controllers.SuperAdmin
{
    [SuperAdminAuthentication("admin", "admin")]
    public class AdminAlertasController : Controller
    {
        // GET: AdminAlertas
        private int commandTime = 0;
        public ActionResult Index()
        {
            var obj = FuncionesBusiness.SearchCountryAlert();
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetDataTable()
        {
            var listaPeriodos = FuncionesBusiness.SearchPeriodByCountry(Request.Form["country"].ToString());
            using (var db = new ConexProvider().Open)
            {
                var pSearch = Request.Form["search[value]"];
                var pDraw = Request.Form["draw"];
                var pStart = Request.Form["start"];
                var pLength = Request.Form["length"];
                var pCountry = Request.Form["country"];
                var pPeriodo = Request.Form["periodo"];

                Console.WriteLine("country: " + pCountry);

                var qp = new DynamicParameters();
                qp.Add("sSearch", pSearch  );
                qp.Add("nStartRow", pStart);
                qp.Add("nLimitRow", pLength);
                qp.Add("sCodPais", pCountry);
                qp.Add("sPeriodo", pPeriodo);
                qp.Add("nTotalCount", dbType: DbType.Int32, direction: ParameterDirection.Output);
                qp.Add("nFilteredCount", dbType: DbType.Int32, direction: ParameterDirection.Output);


                var result = await db.QueryAsync<TableResult>("dbo.pa_alertas_getusers5", qp, commandType: CommandType.StoredProcedure);
                var tableResults = result.ToList();

                var nTotalCount = qp.Get<int>("nTotalCount");
                var nFilteredCount = qp.Get<int>("nFilteredCount");

                foreach (var item in tableResults)
                {
                    item.Check = item.Check.Replace("IdUsuario", item.IdUsuario+"");
                }

                return Json(new
                {
                    draw       = int.Parse(pDraw),
                    recordsTotal =  nTotalCount,
                    recordsFiltered =   nFilteredCount,
                    data = tableResults,
                    listaPeriodos
                });
            }
        }
        public JsonResult GetPeriod(string codPais)
        {
            var listaPeriodos = FuncionesBusiness.SearchPeriodByCountry(codPais);

            return Json(new
            {
                listaPeriodos
            });

        }
        #region TEST
        //var data = new
        //{
        //    FullName = "John Doe",
        //    Empresa = "Peru S.A.C.",
        //    Pais = "Perú",
        //    MesAnio = "Octubre 2018",
        //    Alertas = new[]
        //    {
        //        new
        //        {
        //            Regimen = "Importaciones",
        //            AMP = new
        //            {
        //                LabelCif = "CIF",
        //                Productos = new[]
        //                {
        //                    new {
        //                        Descripcion = "1108130000 FECULA DE PAPA(PATATA)",
        //                        Total = "1 522 368",
        //                        Variacion = "47.07%",
        //                        IconVariacion = "flecha_verde.png"
        //                    },
        //                    new {
        //                        Descripcion = "1108130000 FECULA DE SOYA(SOYA)",
        //                        Total = "1 697 368",
        //                        Variacion = "-10.07%",
        //                        IconVariacion = "flecha_roja.png"
        //                    },
        //                }
        //            },
        //            AMC = new
        //            {
        //                LabelCompanias = "Importadores",
        //                LabelCif = "CIF",
        //                Companias = new[]
        //                {
        //                    new {
        //                        Descripcion = "FRUTOS Y ESPECIAS SAC",
        //                        Total = "5 522 368",
        //                        Variacion = "12.07%",
        //                        IconVariacion = "flecha_verde.png"
        //                    },
        //                    new {
        //                        Descripcion = "G W YICHANG & CIA SA",
        //                        Total = "697 368",
        //                        Variacion = "10.07%",
        //                        IconVariacion = "flecha_verde.png"
        //                    },
        //                }
        //            },
        //            APC = new
        //            {
        //                LabelCompanias = "Importadores",
        //                LabelCif = "CIF",
        //                Productos = new[]
        //                {
        //                    new {
        //                        Descripcion = "1108130000 FECULA DE PAPA(PATATA)",
        //                        Total = "7 522 368",
        //                        Variacion = "88.07%",
        //                        IconVariacion = "flecha_verde.png"
        //                    }
        //                },
        //                Companias = new[]
        //                {
        //                    new {
        //                        Descripcion = "FRUTOS Y ESPECIAS SAC",
        //                        Total = "5 522 368",
        //                        Variacion = "22.07%",
        //                        IconVariacion = "flecha_verde.png"
        //                    },
        //                    new {
        //                        Descripcion = "G W YICHANG & CIA SA",
        //                        Total = "697 368",
        //                        Variacion = "10.07%",
        //                        IconVariacion = "flecha_verde.png"
        //                    },
        //                }
        //            },
        //            ACP = new
        //            {
        //                LabelCompanias = "Importadores",
        //                LabelCif = "CIF",
        //                Companias = new[]
        //                {   
        //                    new {
        //                        Descripcion = "G W YICHANG & CIA SA",
        //                        Total = "697 368",
        //                        Variacion = "10.07%",
        //                        IconVariacion = "flecha_verde.png"
        //                    },
        //                },
        //                Productos = new[]
        //                {
        //                    new {
        //                        Descripcion = "1108130000 FECULA DE PAPA(PATATA)",
        //                        Total = "9 522 368",
        //                        Variacion = "47.07%",
        //                        IconVariacion = "flecha_verde.png"
        //                    },
        //                    new {
        //                        Descripcion = "1108130000 FECULA DE SOYA(SOYA)",
        //                        Total = "8 697 368",
        //                        Variacion = "-10.07%",
        //                        IconVariacion = "flecha_roja.png"
        //                    },
        //                }
        //            }
        //        },
        //        new
        //        {
        //            Regimen = "Exportaciones",
        //            AMP = new
        //            {
        //                LabelCif = "FOB",
        //                Productos = new[]
        //                {
        //                    new {
        //                        Descripcion = "1108130000 FECULA DE PAPA(PATATA)",
        //                        Total = "1 522 368",
        //                        Variacion = "47.07%",
        //                        IconVariacion = "flecha_verde.png"
        //                    },
        //                    new {
        //                        Descripcion = "1108130000 FECULA DE SOYA(SOYA)",
        //                        Total = "1 697 368",
        //                        Variacion = "-10.07%",
        //                        IconVariacion = "flecha_roja.png"
        //                    },
        //                }
        //            },
        //            AMC = new
        //            {
        //                LabelCompanias = "Exportadores",
        //                LabelCif = "FOB",
        //                Companias = new[]
        //                {
        //                    new {
        //                        Descripcion = "FRUTOS Y ESPECIAS SAC",
        //                        Total = "5 522 368",
        //                        Variacion = "12.07%",
        //                        IconVariacion = "flecha_verde.png"
        //                    },
        //                    new {
        //                        Descripcion = "G W YICHANG & CIA SA",
        //                        Total = "697 368",
        //                        Variacion = "10.07%",
        //                        IconVariacion = "flecha_verde.png"
        //                    },
        //                }
        //            },
        //            APC = new
        //            {
        //                LabelCompanias = "Exportadores",
        //                LabelCif = "FOB",
        //                Productos = new[]
        //                {
        //                    new {
        //                        Descripcion = "1108130000 FECULA DE PAPA(PATATA)",
        //                        Total = "7 522 368",
        //                        Variacion = "88.07%",
        //                        IconVariacion = "flecha_verde.png"
        //                    }
        //                },
        //                Companias = new[]
        //                {
        //                    new {
        //                        Descripcion = "FRUTOS Y ESPECIAS SAC",
        //                        Total = "5 522 368",
        //                        Variacion = "22.07%",
        //                        IconVariacion = "flecha_verde.png"
        //                    },
        //                    new {
        //                        Descripcion = "G W YICHANG & CIA SA",
        //                        Total = "697 368",
        //                        Variacion = "10.07%",
        //                        IconVariacion = "flecha_verde.png"
        //                    },
        //                }
        //            },
        //            ACP = new
        //            {
        //                LabelCompanias = "Exportadores",
        //                LabelCif = "FPB",
        //                Companias = new[]
        //                {
        //                    new {
        //                        Descripcion = "G W YICHANG & CIA SA",
        //                        Total = "697 368",
        //                        Variacion = "10.07%",
        //                        IconVariacion = "flecha_verde.png"
        //                    },
        //                },
        //                Productos = new[]
        //                {
        //                    new {
        //                        Descripcion = "1108130000 FECULA DE PAPA(PATATA)",
        //                        Total = "9 522 368",
        //                        Variacion = "47.07%",
        //                        IconVariacion = "flecha_verde.png"
        //                    },
        //                    new {
        //                        Descripcion = "1108130000 FECULA DE SOYA(SOYA)",
        //                        Total = "8 697 368",
        //                        Variacion = "-10.07%",
        //                        IconVariacion = "flecha_roja.png"
        //                    },
        //                }
        //            }
        //        }
        //    }
        //};
        #endregion

        [HttpPost]
        public JsonResult SendAlertaMail(string IdUsuario, string CodPais, string Fecha)
        {
            try
            {
                string ToEmail = "";
                string Subject = "";
                string fromEmail = "";

                var body = GetAlertaMail(IdUsuario, CodPais, Fecha, ref ToEmail, ref Subject);

                //ToEmail = "jhon.grandez@janaq.com";

                using (var tx = new TransactionScope())
                {
                    using (var db = new ConexProvider().Open)
                    {
                        var sql = "insert into dbo.AlertaCorreos (IdUsuario, CodPais, PeriodoData, EmailOrigen, EmailDestino, ContenidoCorreo," +
                                    "  FechaEnvio, Estado ) Values (@IdUsuario, @CodPais, @PeriodoData, @EmailOrigen, @EmailDestino, @ContenidoCorreo," +
                                    "   @FechaEnvio, @Estado ); ";
                        db.Execute(sql, new
                        {
                            IdUsuario,
                            CodPais,
                            PeriodoData = Fecha,
                            EmailOrigen = ToEmail,
                            EmailDestino = fromEmail,
                            ContenidoCorreo = body,
                            FechaEnvio = DateTime.Now,
                            Estado = "S"
                        });
                        //Funciones.SendMail(ToEmail, Subject, body, ref fromEmail, ConfigurationManager.AppSettings["Alerta_BccAddress"].Split(new [] {";"}, StringSplitOptions.RemoveEmptyEntries) );
                        Funciones.SendMail(ToEmail, Subject, body, ref fromEmail);
                        tx.Complete();
                    }
                }
                return Json(new { success = true, message = "Envio de correo con éxito."});
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                return Json(new { success = false, message = "Se ha produccido un error, intente nuevamente.", log = ex.Message  });
            }
        }



        [HttpPost]
        public async Task<JsonResult> SendAlertaMailMasivo2(string codPais, string fecha, string search)
        {
            using (var db = new ConexProvider().Open)
            {
                var pSearch = search;
                int pStart = 0;
                var pLength = 20;
                var pCountry = codPais;
                var pPeriodo = fecha;
                var tableResults = new List<TableResult>();
                string idUsuario = "''";
                int cont = 0;
                bool cargoData = false;

                try
                {
                    var qp = new DynamicParameters();
                    do
                    {
                        cargoData = false;
                        tableResults = new List<TableResult>();
                        
                        try
                        {                            
                            qp.Add("sSearch", pSearch);
                            qp.Add("nStartRow", pStart);
                            qp.Add("nLimitRow", pLength);
                            qp.Add("sCodPais", pCountry);
                            qp.Add("sPeriodo", pPeriodo);
                            qp.Add("nTotalCount", dbType: DbType.Int32, direction: ParameterDirection.Output);
                            qp.Add("nFilteredCount", dbType: DbType.Int32, direction: ParameterDirection.Output);


                            var result = await db.QueryAsync<TableResult>("dbo.pa_alertas_getusers5", qp, commandType: CommandType.StoredProcedure);
                            tableResults = result.ToList();
                            db.Close();
                            var nTotalCount = qp.Get<int>("nTotalCount");
                            var nFilteredCount = qp.Get<int>("nFilteredCount");
                            cargoData = true;
                        }
                        catch(Exception ex)
                        {
                            ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                        }

                        foreach (var item in tableResults)
                        {
                            var x = item.IdUsuario;
                            var codPaisAux = "";

                            var tipoFiltro = "ap.codPais";

                            var filtro2 = "";

                            cont++;

                            if (!string.IsNullOrEmpty(codPais))
                            {
                                if (codPais == "UE")
                                {
                                    tipoFiltro = "SUBSTRING(AP.CODpaIS,0,3)";
                                }

                                codPaisAux = $"AND {tipoFiltro} = '" + codPais + "'";
                            }
                            else
                            {
                                filtro2 = "or (SUBSTRING(AP.CODpaIS, 0, 3) = 'UE' and SUBSTRING(AP.CODpaIS, 0, 3) =  bd.CodPais)";
                            }
                            var fechaAux = "FechaFin";
                            if (!string.IsNullOrEmpty(fecha))
                            {
                                fechaAux = "'" + fecha + "'";
                            }

                            var tableResults2 = new List<TableMasivo>();
                            string sql1 = $@"select ap.CodPais ,cast(cast(left(cast({fechaAux} as varchar), 6) +'01' as date) as varchar(10)) as  FechaFin
                                                                        from AlertaPreferencias ap
                                                                        INNER JOIN BaseDatos bd on {tipoFiltro} = bd.CodPais {filtro2} 
                                                                        left join 
                                                                        (select Valor as TipoUsuario, cast(CAST(u.FecInicio AS VARCHAR) as date) as fecInicio, U.IdUsuario from Usuario U, AdminValor V 
                                                                        where  V.CodVariable = '03TIP' 
                                                                        and U.IdTipo = V.IdAdminValor ) tipoUser on tipoUser.IdUsuario = ap.IdUsuario
                                                                        where ap.IdUsuario = '" + item.IdUsuario + "' " +
                                                                            "and bd.TipoOpe='I' " + codPaisAux +
                                                                            " and (tipoUser.TipoUsuario <> 'Free Trial' or DATEADD(month, 4, tipoUser.fecInicio) >= GETDATE()) " +
                                                                            " group by ap.CodPais, bd.FechaFin";
                            try
                            {
                                using (var db2 = new ConexProvider().Open)
                                {
                                    
                                    var result2 = await db2.QueryAsync<TableMasivo>(sql1, qp);
                                    db2.Close();
                                    tableResults2 = result2.ToList();
                                }
                            }
                            catch(Exception ex3)
                            {
                                try
                                {
                                    using (var db2 = new ConexProvider().Open)
                                    {
                                        var result2 = await db2.QueryAsync<TableMasivo>(sql1, qp);
                                        db2.Close();
                                        tableResults2 = result2.ToList();
                                    }
                                }
                                catch(Exception ex)
                                {
                                    idUsuario += "," + item.IdUsuario.ToString();
                                }
                                
                            }
                            //item.Check = item.Check.Replace("IdUsuario", item.IdUsuario + "");

                            foreach (var item2 in tableResults2)
                            {
                                
                                string IdUsuario = item.IdUsuario.ToString();
                                string CodPais = item2.CodPais;
                                string Fecha = item2.FechaFin;
                                string ToEmail = "";
                                string Subject = "";
                                string fromEmail = "";
                                //idUsuario = IdUsuario;
                                var body = "";
                                try
                                {
                                    body = GetAlertaMail(IdUsuario, CodPais, Fecha, ref ToEmail, ref Subject);
                                }
                                catch(Exception ex)
                                {
                                    ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                                }

                                if (body == "")
                                {
                                    idUsuario += "," + IdUsuario;
                                }
                                
                                //ToEmail = "jhon.grandez@janaq.com";

                                using (var tx = new TransactionScope())
                                {
                                    using (var db1 = new ConexProvider().Open)
                                    {
                                        try
                                        {
                                            var sql = "insert into dbo.AlertaCorreos (IdUsuario, CodPais, PeriodoData, EmailOrigen, EmailDestino, ContenidoCorreo," +
                                                    "  FechaEnvio, Estado ) Values (@IdUsuario, @CodPais, @PeriodoData, @EmailOrigen, @EmailDestino, @ContenidoCorreo," +
                                                    "   @FechaEnvio, @Estado ); ";
                                            db1.Execute(sql, new
                                            {
                                                IdUsuario,
                                                CodPais,
                                                PeriodoData = Fecha,
                                                EmailOrigen = ToEmail,
                                                EmailDestino = fromEmail,
                                                ContenidoCorreo = body,
                                                FechaEnvio = DateTime.Now,
                                                Estado = "S"
                                            });
                                            //Funciones.SendMail(ToEmail, Subject, body, ref fromEmail, ConfigurationManager.AppSettings["Alerta_BccAddress"].Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries));
                                            Funciones.SendMail(ToEmail, Subject, body, ref fromEmail);
                                            db1.Close();
                                            tx.Complete();
                                        }
                                        catch(Exception ex)
                                        {
                                            ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                                        }
                                        
                                    }
                                }
                            }

                        }

                        if (!cargoData)
                        {
                            pStart -= pLength;
                        }

                        pStart +=  pLength;

                    } while (tableResults.Count > 0 || !cargoData);

                    if(idUsuario.Length > 3)
                    {
                       /* bool tipoServidor = Properties.Settings.Default.TableVarGeneral_InDev;
                        string path = ConfigurationManager.AppSettings["directorio_logs"];
                        Logs oLog = new Logs(path);
                        try
                        {

                            oLog.Add("IdUsuario Que no se enviaron => " + idUsuario, "000");
                            oLog.Add("Servidor => " + (tipoServidor ? "Desarrollo" : "Produccion"), "000");
                            oLog.Add("Controlador => Admin Alertas ", "000");
                        }
                        catch (Exception ex)
                        {
                            oLog.Add("Excepcion => " + ex.ToString(), Session["idUsuario"].ToString());
                        }*/
                    }
                    return Json(new { success = true, message = "Envio de correo con éxito." });
                }
                catch (Exception ex)
                {
                    ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                    return Json(new { success = false, message = "Se ha produccido un error, intente nuevamente.", log = ex.Message });
                }
            }
        }

        /*
        
        [HttpPost]
        public async Task<JsonResult> SendAlertaMailMasivo2(string codPais, string fecha, string search)
        {
            using (var db = new ConexProvider().Open)
            {
                var pSearch = search;
                var pStart = 3000;
                var pLength = 500;
                var pCountry = codPais;
                var pPeriodo = fecha;


                var qp = new DynamicParameters();
                qp.Add("sSearch", pSearch);
                qp.Add("nStartRow", pStart);
                qp.Add("nLimitRow", pLength);
                qp.Add("sCodPais", pCountry);
                qp.Add("sPeriodo", pPeriodo);
                qp.Add("nTotalCount", dbType: DbType.Int32, direction: ParameterDirection.Output);
                qp.Add("nFilteredCount", dbType: DbType.Int32, direction: ParameterDirection.Output);
                string idUsuario = "";
                int cont = 0;
                try
                {
                    var result = await db.QueryAsync<TableResult>("dbo.pa_alertas_getusers5", qp, commandType: CommandType.StoredProcedure);
                    var tableResults = result.ToList();
                    db.Close();
                    var nTotalCount = qp.Get<int>("nTotalCount");
                    var nFilteredCount = qp.Get<int>("nFilteredCount");

                    foreach (var item in tableResults)
                    {
                        var x = item.IdUsuario;
                        var codPaisAux = "";

                        var tipoFiltro = "ap.codPais";

                        var filtro2 = "";

                        

                        if (!string.IsNullOrEmpty(codPais))
                        {
                            if (codPais == "UE")
                            {
                                tipoFiltro = "SUBSTRING(AP.CODpaIS,0,3)";
                            }

                            codPaisAux = $"AND {tipoFiltro} = '"+codPais+"'";
                        }
                        else
                        {
                            filtro2 = "or (SUBSTRING(AP.CODpaIS, 0, 3) = 'UE' and SUBSTRING(AP.CODpaIS, 0, 3) =  bd.CodPais)";
                        }
                        var fechaAux = "FechaFin";
                        if (!string.IsNullOrEmpty(fecha))
                        {
                            fechaAux = "'"+fecha+"'";
                        }

                        var tableResults2 = new List<TableMasivo>();
                        using (var db2 = new ConexProvider().Open)
                        {
                            string sql1 = $@"select ap.CodPais ,cast(cast(left(cast({fechaAux} as varchar), 6) +'01' as date) as varchar(10)) as  FechaFin
                                                                    from AlertaPreferencias ap
                                                                    INNER JOIN BaseDatos bd on {tipoFiltro} = bd.CodPais {filtro2} 
                                                                    where ap.IdUsuario = '" + item.IdUsuario + "' " +
                                                                    "and bd.TipoOpe='I' " + codPaisAux +
                                                                    " group by ap.CodPais, bd.FechaFin";
                            var result2 = await db2.QueryAsync<TableMasivo>(sql1, qp);
                            db2.Close();
                            tableResults2 = result2.ToList();
                        }

                            
                        //item.Check = item.Check.Replace("IdUsuario", item.IdUsuario + "");
                        
                        foreach (var item2 in tableResults2)
                        {
                            cont++;

                            if(cont== 242)
                            {

                            }

                            if (item.IdUsuario.ToString() == "91425")
                            {
                                //idUsuario = "";
                            }
                            string IdUsuario = item.IdUsuario.ToString();
                            string CodPais = item2.CodPais;
                            string Fecha = item2.FechaFin;
                            string ToEmail = "";
                            string Subject = "";
                            string fromEmail = "";
                            idUsuario = IdUsuario;
                            var body = GetAlertaMail(IdUsuario, CodPais, Fecha, ref ToEmail, ref Subject);

                            if(body == "")
                            {

                            }

                            //ToEmail = "jhon.grandez@janaq.com";

                            using (var tx = new TransactionScope())
                            {
                                using (var db1 = new ConexProvider().Open)
                                {
                                    var sql = "insert into dbo.AlertaCorreos (IdUsuario, CodPais, PeriodoData, EmailOrigen, EmailDestino, ContenidoCorreo," +
                                                "  FechaEnvio, Estado ) Values (@IdUsuario, @CodPais, @PeriodoData, @EmailOrigen, @EmailDestino, @ContenidoCorreo," +
                                                "   @FechaEnvio, @Estado ); ";
                                    db.Execute(sql, new
                                    {
                                        IdUsuario,
                                        CodPais,
                                        PeriodoData = Fecha,
                                        EmailOrigen = ToEmail,
                                        EmailDestino = fromEmail,
                                        ContenidoCorreo = body,
                                        FechaEnvio = DateTime.Now,
                                        Estado = "S"
                                    });
                                    //Funciones.SendMail(ToEmail, Subject, body, ref fromEmail, ConfigurationManager.AppSettings["Alerta_BccAddress"].Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries));
                                    Funciones.SendMail(ToEmail, Subject, body, ref fromEmail);
                                    tx.Complete();
                                }
                            }
    }

}
                    return Json(new { success = true, message = "Envio de correo con éxito." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Se ha produccido un error, intente nuevamente.", log = ex.Message });
                }
            }
        }
         */

        [HttpPost]
        public JsonResult SendAlertaMailMasivo(AlertArrayModel model )
        {
            try
            {
                foreach (var item in model.alert)
                {
                    string IdUsuario = item.IdUsuario;
                    string CodPais = item.CodPais;
                    string Fecha = item.Fecha;
                    string ToEmail = "";
                    string Subject = "";
                    string fromEmail = "";

                    var body = GetAlertaMail(IdUsuario, CodPais, Fecha, ref ToEmail, ref Subject);

                    //ToEmail = "jhon.grandez@janaq.com";

                    using (var tx = new TransactionScope())
                    {
                        using (var db = new ConexProvider().Open)
                        {
                            var sql = "insert into dbo.AlertaCorreos (IdUsuario, CodPais, PeriodoData, EmailOrigen, EmailDestino, ContenidoCorreo," +
                                        "  FechaEnvio, Estado ) Values (@IdUsuario, @CodPais, @PeriodoData, @EmailOrigen, @EmailDestino, @ContenidoCorreo," +
                                        "   @FechaEnvio, @Estado ); ";
                            db.Execute(sql, new
                            {
                                IdUsuario,
                                CodPais,
                                PeriodoData = Fecha,
                                EmailOrigen = ToEmail,
                                EmailDestino = fromEmail,
                                ContenidoCorreo = body,
                                FechaEnvio = DateTime.Now,
                                Estado = "S"
                            });
                            //Funciones.SendMail(ToEmail, Subject, body, ref fromEmail, ConfigurationManager.AppSettings["Alerta_BccAddress"].Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries));
                            Funciones.SendMail(ToEmail, Subject, body, ref fromEmail);
                            tx.Complete();
                        }
                    }
                }
                
                return Json(new { success = true, message = "Envio de correo con éxito." });
            }
            catch (Exception ex)
            {
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                return Json(new { success = false, message = "Se ha produccido un error, intente nuevamente.", log = ex.Message });
            }
        }


        //[HttpGet]
        //public ContentResult PreviewAlertaMail(string IdUsuario, string CodPais, string Fecha)
        //{
        //    string v1 = "", v2 = "";
        //    return Content(GetAlertaMail(IdUsuario, CodPais, Fecha, ref v1, ref v2), "text/html");
        //}

        [HttpGet]
        public ContentResult PreviewAlertaMail(AlertModel m)
        {
            string v1 = "", v2 = "";
            return Content(GetAlertaMail(m.IdUsuario, m.CodPais, m.Fecha, ref v1, ref v2), "text/html");
        }

        public string GetAlertaMail(string IdUsuario, string CodPais, string Fecha, ref string ToEmail, ref string Subject, string base_url = "", string tplPath = "" , string culture = "" , int veces = 0 )
        {
            Fecha = DateTime.Parse(Fecha).ToString("MM/dd/yyyy H:mm");
            string fechaFormateada = DateTime.Parse(Fecha).ToString("MM/dd/yyyy H:mm"); 
            //string base_url = _base_url;
            if (string.IsNullOrEmpty(base_url)) base_url = Request.Url.GetLeftPart(UriPartial.Authority);
           
            //string path = HttpContext.Server.MapPath("~/Content/tpl/alertas.mail.html");
            if (string.IsNullOrEmpty(tplPath)) tplPath = HttpContext.Server.MapPath("~/Content/tpl/alertas.mail.html");

            string template = System.IO.File.ReadAllText(tplPath);
            string idioma = FuncionesBusiness.SearchLangUsuario(IdUsuario);
            if (!string.IsNullOrEmpty(culture))
            {
                if (culture == "es")
                {
                    //fechaFormateada = DateTime.Parse(Fecha).ToString("MM/dd/yyyy H:mm");
                    idioma = "ESP";
                }
                else
                {
                    idioma = culture;
                    //fechaFormateada = DateTime.Parse(Fecha).ToString("MM/dd/yyyy H:mm");
                }
            }
            
            using (var db = new ConexProvider().Open)
            {
               
                var sqlPais = "select top 1 Descripcion  from dbo.VariableGeneral where idgrupo='SRE' and IdVariable=@codPais";
                var sqlBaseData = "@codPais";
                var sqlVisibleUE = "and exists(select 1 from dbo.variablegeneral where idvariable=ap.codpais)";
                if (CodPais.Substring(0, 2) == "UE" && CodPais.Length > 2)
                {
                    string cp = CodPais.Substring(2, 1);
                    if (CodPais.Length > 3)
                    {
                        cp = CodPais.Substring(2, 2);
                    }

                    sqlPais = $"SELECT TOP 1 Pais FROM dbo.PaisUEImpExp WHERE idPais = '{cp}'";
                    sqlBaseData = "'UE'";
                    sqlVisibleUE = " and CodPais = '" + CodPais + "'";
                }

                var lang = idioma == "ESP" ? "Spanish" : "English";
                var sql = $@"SET LANGUAGE {lang};
                            select 
                                Email = Email1, 
	                            FullName = Nombres + ' ' + Apellidos, 
	                            Empresa = IsNull(Empresa, '-'),    
	                            Pais = IsNull(({sqlPais}), ''),
	                            MesAnio = DATENAME(MM, @d) + ' ' +DATENAME(YY, @d),
                                MesAlerta = SUBSTRING(DATENAME(MM, @d),0,4),
                                AnioAlerta = cast(YEAR(@d) as varchar(50)),
	                            CampoTot_I = IsNull((select Top 1 isnull(Incoterm, '') from BaseDatos where CodPais = {sqlBaseData} and TipoOpe ='I'),''),
	                            CampoTot_E = IsNull((select Top 1 isnull(Incoterm, '') from BaseDatos where CodPais = {sqlBaseData} and TipoOpe ='E'),''),
                                HasAlerta_I = Cast( case when exists(Select 1 from dbo.alertapreferencias ap where ap.Idusuario=@idusu And ap.idtipoope='I' and ap.CodPais = @codPais
                                                {sqlVisibleUE} ) Then 1 Else 0 End  as bit),
                                HasAlerta_E = Cast( case when exists(Select 1 from dbo.alertapreferencias ap where ap.Idusuario=@idusu And ap.idtipoope='E' and ap.CodPais = @codPais
                                                {sqlVisibleUE} ) Then 1 Else 0 End  as bit)
                            from dbo.usuario where idusuario=@idusu;";
                bool flagException = Funciones.BuscarVariableGeneral("VVVA", CodPais);
                var r1 = db.QueryFirstOrDefault<dynamic>(sql, new { d = fechaFormateada, codPais = CodPais, idusu = IdUsuario });

                var primerMes = idioma == "ESP" ? "Ene-" : "Jan-";

                var data = new AlertaMail()
                {
                    FullName = r1.FullName,
                    Empresa = r1.Empresa,
                    Pais = r1.Pais,
                    MesAnio = r1.MesAnio,
                    CampoTotI = r1.CampoTot_I,
                    CampoTotE = r1.CampoTot_E,
                    Email = r1.Email,
                    LinkGoAlertas = base_url+ "/"+ (idioma == "ESP" ? "es" : "en") +"/mis-alertas-favoritos?acc=ss",
                    HasAlertaI = r1.HasAlerta_I && !flagException,
                    HasAlertaE = r1.HasAlerta_E,
                    fechaVerialertas = r1.MesAlerta + " " + r1.AnioAlerta
                };


                int anioAnterior = int.Parse(r1.AnioAlerta) - 1;
                string mesAlerta = r1.MesAlerta;

                data.fechasVerialertasAnioActual = mesAlerta != "Ene" || mesAlerta  != "Jan" ? primerMes + data.fechaVerialertas : data.fechaVerialertas;
                data.fechasVerialertasAnioAnterior = mesAlerta != "Ene" || mesAlerta != "Jan" ? primerMes + mesAlerta + " " + anioAnterior : mesAlerta + " " + anioAnterior;

                if (idioma == "ESP")
                {
                    MailLang obj = new MailLang()
                    {
                        LabelImportadores = "MIS IMPORTADORES",
                        LabelExportadores = "MIS EXPORTADORES",
                        LabelMensajeInformativo = "Revisa toda la información de tus últimas alertas de",
                        LabelProduct = "MIS PRODUCTOS",
                        LabelSaludo = "HOLA",
                        LabelVariacion = "Variación",
                        LabelConfig = "Configura tus Alertas",
                        LabelVariacion2 = "respecto a"
                    };

                    data.objLang = obj;
                }
                else { 
                    MailLang obj = new MailLang()
                    {
                        LabelImportadores = "MY IMPORTERS",
                        LabelExportadores = "MY EXPORTERS",
                        LabelMensajeInformativo = "Review all the information of your latest alerts",
                        LabelProduct = "MY PRODUCTS",
                        LabelSaludo = "HI",
                        LabelVariacion = "Variation",
                        LabelConfig = "Configure your Alerts",
                        LabelVariacion2 = "about"

                    };

                    data.objLang = obj;

                }

                var asunto = idioma == "ESP" ? "VeriAlertas" : "VeriAlerts";

                ToEmail = data.Email;
                Subject = $"{asunto} - {data.Pais} - {data.MesAnio}";
                //Subject = $"{asunto} - {data.Pais} - {data.MesAnio} | veritradecorp.com";

                data.Alertas = new List<AlertaDetail>()
                {
                    new AlertaDetail
                    {
                        HasAlerta = data.HasAlertaI,
                        Regimen = idioma == "ESP" ? "IMPORTACIONES" : "IMPORTS",
                        TipoOperacion = idioma == "ESP" ? "MIS IMPORTADORES" : "MY IMPORTERS",
                        AMP = new TableResume
                        {
                            LabelCif = data.CampoTotI
                        },
                        AMC = new TableResume
                        {
                            LabelCompanias = idioma == "ESP" ?"MIS IMPORTADORES" : "MY IMPORTERS",
                            LabelCif = data.CampoTotI
                        },
                        APC = new TableResume
                        {
                            LabelCompanias = idioma == "ESP" ?"IMPORTADORES" : "IMPORTERS",
                            LabelCif = data.CampoTotI,
                        },
                        ACP = new TableResume
                        {
                            LabelCompanias = idioma == "ESP" ?"MI IMPORTADOR" : "MY IMPORTER",
                            LabelCif = data.CampoTotI,
                        }
                    },
                    new AlertaDetail
                    {
                        HasAlerta = data.HasAlertaE,
                        Regimen = idioma == "ESP" ? "EXPORTACIONES" : "EXPORTS",
                        TipoOperacion = idioma == "ESP" ? "MIS EXPORTADORES" : "MY EXPORTERS",
                        AMP = new TableResume
                        {
                            LabelCif = data.CampoTotE
                        },
                        AMC = new TableResume
                        {
                            LabelCompanias = idioma == "ESP" ?"MIS EXPORTADORES" : "MY EXPORTERS",
                            LabelCif = data.CampoTotE
                        },
                        APC = new TableResume
                        {
                            LabelCompanias = idioma == "ESP" ?"EXPORTADORES" : "EXPORTERS",
                            LabelCif = data.CampoTotE,
                        },
                        ACP = new TableResume
                        {
                            LabelCompanias = idioma == "ESP" ?"MI EXPORTADOR" : "MY EXPORTER",
                            LabelCif = data.CampoTotE,
                        }
                    }
                };

                bool validar = false;
                var IdFav2 = 0;
                //Importadores
                if (data.HasAlertaI)
                {
                    var r2 = db.Query<dynamic>("dbo.pa_alertas_getpreferences2", new
                    {
                        idUsuario = IdUsuario,
                        codPais = CodPais,
                        fecha = Fecha,
                        tipoAlerta = "AMP",
                        tipoOperacion = "I",
                        lang = idioma == "ESP" ? "es" : "en"
                    }, commandType: CommandType.StoredProcedure, commandTimeout: commandTime);

                    if (r2 != null)
                    {
                        data.Alertas[0].AMP.Productos = new List<TableResume.TableDetail>();

                        Parallel.ForEach(r2, (e =>
                        {
                            try
                            {
                                data.Alertas[0].AMP.Productos.Add(new TableResume.TableDetail
                                {
                                    Descripcion = e.Partida,
                                    Total = e.TotalMes.ToString("N0", CultureInfo.InvariantCulture),
                                    Variacion = e.Variacion.ToString("N2", CultureInfo.InvariantCulture),
                                    Link = TableResume.TableDetail.GiveMeLink(base_url, e.IdFavorito, "AMP", CodPais, "I", Fecha, idioma),
                                    MostrarLink = e.TotalMes > 0,
                                    OcultarLink = e.TotalMes == 0,
                                    TotalAnio = e.TotalAnio.ToString("N0", CultureInfo.InvariantCulture)
                                });
                            }
                            catch (Exception ex)
                            {
                                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                            }
                        }));
                    }

                    var r3 = db.Query<dynamic>("dbo.pa_alertas_getpreferences2", new
                    {
                        idUsuario = IdUsuario,
                        codPais = CodPais,
                        fecha = Fecha,
                        tipoAlerta = "AMC",
                        tipoOperacion = "I",
                        lang = idioma == "ESP" ? "es" : "en"
                    }, commandType: CommandType.StoredProcedure, commandTimeout: commandTime);

                    if (CodPais == "BR" || CodPais.Substring(0, 2) == "")
                    {
                        r3 = null;
                    }



                    if (r3 != null)
                    {
                        r3.ToString();
                        data.Alertas[0].AMC.Companias = new List<TableResume.TableDetail>();

                        Parallel.ForEach(r3, (e =>
                        {
                            try
                            {
                                data.Alertas[0].AMC.Companias.Add(new TableResume.TableDetail
                                {
                                    Descripcion = e.Empresa,
                                    Total = e.TotalMes.ToString("N0", CultureInfo.InvariantCulture),
                                    Variacion = e.Variacion.ToString("N2", CultureInfo.InvariantCulture),
                                    Link = TableResume.TableDetail.GiveMeLink(base_url, e.IdFavorito, "AMC", CodPais, "I", Fecha, idioma),
                                    MostrarLink = e.TotalMes > 0,
                                    OcultarLink = e.TotalMes == 0,
                                    TotalAnio = e.TotalAnio.ToString("N0", CultureInfo.InvariantCulture)
                                }
                                );
                            }
                            catch (Exception ex)
                            {
                                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                            }
                        }));


                    }
                    
                    var r4 = db.Query<dynamic>("dbo.pa_alertas_getpreferences2", new
                    {
                        idUsuario = IdUsuario,
                        codPais = CodPais,
                        fecha = Fecha,
                        tipoAlerta = "APC",
                        tipoOperacion = "I",
                        lang = idioma == "ESP" ? "es" : "en"
                    }, commandType: CommandType.StoredProcedure, commandTimeout: commandTime);

                    
                    if (r4 != null)
                    {
                        data.Alertas[0].APC.Productos = new List<TableResume.TableDetail>();

                        Parallel.ForEach(r4, (e =>
                        {
                            try
                            {
                                //var x = TableResume.TableDetail.Details(IdUsuario, base_url, e.IdFavorito, "APC", CodPais, "I", Fecha, idioma, ref validar);
                                data.Alertas[0].APC.Productos.Add(new TableResume.TableDetail
                                {
                                    Descripcion = e.Partida,
                                    Total = e.TotalMes.ToString("N0", CultureInfo.InvariantCulture),
                                    Variacion = e.Variacion.ToString("N2", CultureInfo.InvariantCulture),
                                    Link = TableResume.TableDetail.GiveMeLink(base_url, e.IdFavorito, "APC", CodPais, "I", Fecha, idioma),
                                    FavoritosDetails = TableResume.TableDetail.Details(IdUsuario, base_url, e.IdFavorito, "APC", CodPais, "I", Fecha, idioma, ref validar),
                                    MostrarLink = validar,
                                    OcultarLink = !validar,
                                    TotalAnio = e.TotalAnio.ToString("N0", CultureInfo.InvariantCulture)
                                });
                                IdFav2 = e.IdFavorito;

                            }
                            catch (Exception ex)
                            {
                                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                            }
                        }));

                    }

                    var r41 = db.Query<dynamic>("dbo.pa_alertas_getpreferences2", new
                    {
                        idUsuario = IdUsuario,
                        codPais = CodPais,
                        fecha = Fecha,
                        tipoAlerta = "APC",
                        tipoOperacion = "I",
                        lang = idioma == "ESP" ? "es" : "en",
                        isDetails = 1
                    }, commandType: CommandType.StoredProcedure, commandTimeout: commandTime);

                    if (CodPais == "BR")
                    {
                        r41 = null;
                    }

                    if (r41 != null)
                    {

                        data.Alertas[0].APC.Companias = new List<TableResume.TableDetail>();
                        Parallel.ForEach(r41, (e1 =>
                        {
                            try
                            {
                                data.Alertas[0].APC.Companias.Add(new TableResume.TableDetail
                                {
                                    Descripcion = e1.Empresa,
                                    Total = e1.TotalMes.ToString("N0", CultureInfo.InvariantCulture),
                                    Variacion = e1.Variacion.ToString("N2", CultureInfo.InvariantCulture),
                                    Link = TableResume.TableDetail.GiveMeLink(base_url, IdFav2, "APC", CodPais, "I", Fecha, idioma, e1.IdFavorito),
                                    MostrarLink = e1.TotalMes > 0,
                                    OcultarLink = e1.TotalMes == 0,
                                    TotalAnio = e1.TotalAnio.ToString("N0", CultureInfo.InvariantCulture)
                                });
                            }
                            catch (Exception ex)
                            {
                                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                            }
                        }));
                    }


                    var r5 = db.Query<dynamic>("dbo.pa_alertas_getpreferences2", new
                    {
                        idUsuario = IdUsuario,
                        codPais = CodPais,
                        fecha = Fecha,
                        tipoAlerta = "ACP",
                        tipoOperacion = "I",
                        lang = idioma == "ESP" ? "es" : "en"
                    }, commandType: CommandType.StoredProcedure, commandTimeout: commandTime);

                    if (CodPais == "BR")
                    {
                        r5 = null;
                    }

                    if (r5 != null)
                    {
                        data.Alertas[0].ACP.Companias = new List<TableResume.TableDetail>();
                        Parallel.ForEach(r5, (e =>
                        {
                            try
                            {
                                data.Alertas[0].ACP.Companias.Add(new TableResume.TableDetail
                                {
                                    Descripcion = e?.Empresa,
                                    Total = e.TotalMes.ToString("N0", CultureInfo.InvariantCulture),
                                    Variacion = e.Variacion.ToString("N2", CultureInfo.InvariantCulture),
                                    Link = TableResume.TableDetail.GiveMeLink(base_url, e.IdFavorito, "ACP", CodPais, "I", Fecha, idioma),
                                    FavoritosDetails = TableResume.TableDetail.Details(IdUsuario, base_url, e.IdFavorito, "ACP", CodPais, "I", Fecha, idioma, ref validar),
                                    MostrarLink = validar,
                                    OcultarLink = !validar,
                                    TotalAnio = e.TotalAnio.ToString("N0", CultureInfo.InvariantCulture)
                                });
                                IdFav2 = e.IdFavorito;
                            }
                            catch (Exception ex)
                            {
                                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                            }
                        }));
                    }

                    var r51 = db.Query<dynamic>("dbo.pa_alertas_getpreferences2", new
                    {
                        idUsuario = IdUsuario,
                        codPais = CodPais,
                        fecha = Fecha,
                        tipoAlerta = "ACP",
                        tipoOperacion = "I",
                        lang = idioma == "ESP" ? "es" : "en",
                        isDetails = 1
                    }, commandType: CommandType.StoredProcedure, commandTimeout: commandTime);

                    if (CodPais == "BR")
                    {
                        r51 = null;
                    }

                    if (r51 != null)
                    {
                        data.Alertas[0].ACP.Productos = new List<TableResume.TableDetail>();
                        Parallel.ForEach(r51, (e1 =>
                        {
                            try
                            {
                                data.Alertas[0].ACP.Productos.Add(new TableResume.TableDetail
                                {
                                    Descripcion = e1.Partida,
                                    Total = e1.TotalMes.ToString("N0", CultureInfo.InvariantCulture),
                                    Variacion = e1.Variacion.ToString("N2", CultureInfo.InvariantCulture),
                                    Link = TableResume.TableDetail.GiveMeLink(base_url, IdFav2, "ACP", CodPais, "I", Fecha, idioma, e1.IdFavorito),
                                    MostrarLink = e1.TotalMes > 0,
                                    OcultarLink = e1.TotalMes == 0,
                                    TotalAnio = e1.TotalAnio.ToString("N0", CultureInfo.InvariantCulture)
                                });
                            }
                            catch (Exception ex)
                            {
                                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                            }
                        }));
                    }
                }

                //Exportadores
                if (data.HasAlertaE)
                {
                    
                    var r2 = db.Query<dynamic>("dbo.pa_alertas_getpreferences2", new
                    {
                        idUsuario = IdUsuario,
                        codPais = CodPais,
                        fecha = Fecha,
                        tipoAlerta = "AMP",
                        tipoOperacion = "E",
                        lang = idioma == "ESP" ? "es" : "en"
                    }, commandType: CommandType.StoredProcedure, commandTimeout: commandTime);


                    if (r2 != null)
                    {
                        data.Alertas[1].AMP.Productos = new List<TableResume.TableDetail>();
                        Parallel.ForEach(r2, (e =>
                        {
                            try
                            {
                                data.Alertas[1].AMP.Productos.Add(new TableResume.TableDetail
                                {
                                    Descripcion = e.Partida,
                                    Total = e.TotalMes.ToString("N0", CultureInfo.InvariantCulture),
                                    Variacion = e.Variacion.ToString("N2", CultureInfo.InvariantCulture),
                                    Link = TableResume.TableDetail.GiveMeLink(base_url, e.IdFavorito, "AMP", CodPais, "E", Fecha, idioma),
                                    MostrarLink = e.TotalMes > 0,
                                    OcultarLink = e.TotalMes == 0,
                                    TotalAnio = e.TotalAnio.ToString("N0", CultureInfo.InvariantCulture)
                                });
                            }
                            catch (Exception ex)
                            {
                                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                            }
                        }));
                    }

                    var r3 = db.Query<dynamic>("dbo.pa_alertas_getpreferences2", new
                    {
                        idUsuario = IdUsuario,
                        codPais = CodPais,
                        fecha = Fecha,
                        tipoAlerta = "AMC",
                        tipoOperacion = "E",
                        lang = idioma == "ESP" ? "es" : "en"
                    }, commandType: CommandType.StoredProcedure, commandTimeout: commandTime);

                    if (CodPais == "BR")
                    {
                        r3 = null;
                    }

                    if (r3 != null)
                    {
                        data.Alertas[1].AMC.Companias = new List<TableResume.TableDetail>();
                        Parallel.ForEach(r3, (e =>
                        {
                            try
                            {
                                data.Alertas[1].AMC.Companias.Add(new TableResume.TableDetail
                                {
                                    Descripcion = e.Empresa,
                                    Total = e.TotalMes.ToString("N0", CultureInfo.InvariantCulture),
                                    Variacion = e.Variacion.ToString("N2", CultureInfo.InvariantCulture),
                                    Link = TableResume.TableDetail.GiveMeLink(base_url, e.IdFavorito, "AMC", CodPais, "E", Fecha, idioma),
                                    MostrarLink = e.TotalMes > 0,
                                    OcultarLink = e.TotalMes == 0,
                                    TotalAnio = e.TotalAnio.ToString("N0", CultureInfo.InvariantCulture)
                                });
                            }
                            catch (Exception ex)
                            {
                                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                            }
                        }));
                    }

                    var r4 = db.Query<dynamic>("dbo.pa_alertas_getpreferences2", new
                    {
                        idUsuario = IdUsuario,
                        codPais = CodPais,
                        fecha = Fecha,
                        tipoAlerta = "APC",
                        tipoOperacion = "E",
                        lang = idioma == "ESP" ? "es" : "en"
                    }, commandType: CommandType.StoredProcedure, commandTimeout: commandTime);


                    if (r4 != null)
                    {
                        data.Alertas[1].APC.Productos = new List<TableResume.TableDetail>();
                        Parallel.ForEach(r4, (e =>
                        {
                            try
                            {
                                data.Alertas[1].APC.Productos.Add(new TableResume.TableDetail
                                {
                                    Descripcion = e.Partida,
                                    Total = e.TotalMes.ToString("N0", CultureInfo.InvariantCulture),
                                    Variacion = e.Variacion.ToString("N2", CultureInfo.InvariantCulture),
                                    Link = TableResume.TableDetail.GiveMeLink(base_url, e.IdFavorito, "APC", CodPais, "E", Fecha, idioma),
                                    FavoritosDetails = TableResume.TableDetail.Details(IdUsuario, base_url, e.IdFavorito, "APC", CodPais, "E", Fecha, idioma, ref validar),
                                    MostrarLink = validar,
                                    OcultarLink = !validar,
                                    TotalAnio = e.TotalAnio.ToString("N0", CultureInfo.InvariantCulture)

                                });
                                IdFav2 = e.IdFavorito;
                            }
                            catch (Exception ex)
                            {
                                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                            }
                        }));
                    }

                    var r41 = db.Query<dynamic>("dbo.pa_alertas_getpreferences2", new
                    {
                        idUsuario = IdUsuario,
                        codPais = CodPais,
                        fecha = Fecha,
                        tipoAlerta = "APC",
                        tipoOperacion = "E",
                        lang = idioma == "ESP" ? "es" : "en",
                        isDetails = 1
                    }, commandType: CommandType.StoredProcedure, commandTimeout: commandTime);

                    if (CodPais == "BR")
                    {
                        r41 = null;
                    }

                    if (r41 != null)
                    {
                        data.Alertas[1].APC.Companias = new List<TableResume.TableDetail>();
                        Parallel.ForEach(r41, (e1 =>
                        {
                            try
                            {
                                data.Alertas[1].APC.Companias.Add(new TableResume.TableDetail
                                {
                                    Descripcion = e1.Empresa,
                                    Total = e1.TotalMes.ToString("N0", CultureInfo.InvariantCulture),
                                    Variacion = e1.Variacion.ToString("N2", CultureInfo.InvariantCulture),
                                    Link = TableResume.TableDetail.GiveMeLink(base_url, IdFav2, "APC", CodPais, "E", Fecha, idioma, e1.IdFavorito),
                                    MostrarLink = e1.TotalMes > 0,
                                    OcultarLink = e1.TotalMes == 0,
                                    TotalAnio = e1.TotalAnio.ToString("N0", CultureInfo.InvariantCulture)
                                });
                            }
                            catch (Exception ex)
                            {
                                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                            }

                        }));
                    }


                    var r5 = db.Query<dynamic>("dbo.pa_alertas_getpreferences2", new
                    {
                        idUsuario = IdUsuario,
                        codPais = CodPais,
                        fecha = Fecha,
                        tipoAlerta = "ACP",
                        tipoOperacion = "E",
                        lang = idioma == "ESP" ? "es" : "en"
                    }, commandType: CommandType.StoredProcedure, commandTimeout: commandTime);

                    if (CodPais == "BR")
                    {
                        r5 = null;
                    }

                    if (r5 != null)
                    {
                        data.Alertas[1].ACP.Companias = new List<TableResume.TableDetail>();
                        Parallel.ForEach(r5, (e =>
                        {
                            try
                            {
                                data.Alertas[1].ACP.Companias.Add(new TableResume.TableDetail
                                {
                                    Descripcion = e.Empresa,
                                    Total = e.TotalMes.ToString("N0", CultureInfo.InvariantCulture),
                                    Variacion = e.Variacion.ToString("N2", CultureInfo.InvariantCulture),
                                    Link = TableResume.TableDetail.GiveMeLink(base_url, e.IdFavorito, "ACP", CodPais, "E", Fecha, idioma),
                                    FavoritosDetails = TableResume.TableDetail.Details(IdUsuario, base_url, e.IdFavorito, "ACP", CodPais, "E", Fecha, idioma, ref validar),
                                    MostrarLink = validar,
                                    OcultarLink = !validar,
                                    TotalAnio = e.TotalAnio.ToString("N0", CultureInfo.InvariantCulture)
                                });
                                IdFav2 = e.IdFavorito;

                            }
                            catch (Exception ex)
                            {
                                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                            }

                        }));
                    }

                    var r51 = db.Query<dynamic>("dbo.pa_alertas_getpreferences2", new
                    {
                        idUsuario = IdUsuario,
                        codPais = CodPais,
                        fecha = Fecha,
                        tipoAlerta = "ACP",
                        tipoOperacion = "E",
                        lang = idioma == "ESP" ? "es" : "en",
                        isDetails = 1
                    }, commandType: CommandType.StoredProcedure, commandTimeout: commandTime);

                    if (CodPais == "BR")
                    {
                        r51 = null;
                    }

                    if (r51 != null)
                    {
                        data.Alertas[1].ACP.Productos = new List<TableResume.TableDetail>();
                        Parallel.ForEach(r51, (e1 =>
                        {
                            try
                            {
                                data.Alertas[1].ACP.Productos.Add(new TableResume.TableDetail
                                {
                                    Descripcion = e1.Partida,
                                    Total = e1.TotalMes.ToString("N0", CultureInfo.InvariantCulture),
                                    Variacion = e1.Variacion.ToString("N2", CultureInfo.InvariantCulture),
                                    Link = TableResume.TableDetail.GiveMeLink(base_url, IdFav2, "ACP", CodPais, "E", Fecha, idioma, e1.IdFavorito),
                                    MostrarLink = e1.TotalMes > 0,
                                    OcultarLink = e1.TotalMes == 0,
                                    TotalAnio = e1.TotalAnio.ToString("N0", CultureInfo.InvariantCulture)
                                });
                            }
                            catch (Exception ex)
                            {
                                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                            }
                        }));
                    }
                }

                db.Close();

                try
                {
                    FormatCompiler compiler = new FormatCompiler();
                    Generator generator = compiler.Compile(template);

                    var x = data.Alertas;
                    foreach (var item in data.Alertas)
                    {

                        if (item.APC.Productos != null)
                        {
                            foreach (var item2 in item.APC.Productos)
                            {
                                validar = item2.MostrarLink;
                                if (item2.MostrarLink)
                                {
                                    foreach (var item3 in item2.FavoritosDetails)
                                    {
                                        item3.MostrarLink = true;
                                        item3.OcultarLink = false;
                                    }
                                }
                            }
                        }

                        


                        if(item.ACP.Companias != null)
                        {
                            foreach (var item2 in item.ACP.Companias)
                            {
                                validar = item2.MostrarLink;
                                if (item2.MostrarLink)
                                {
                                    foreach (var item3 in item2.FavoritosDetails)
                                    {
                                        item3.MostrarLink = true;
                                        item3.OcultarLink = false;
                                    }
                                }
                            }
                        }                    
                    }

                    return generator.Render(data);

                }
                catch (Exception ex)
                {
                    ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                    Console.Write("" + ex.ToString());
                    string body2 = "";

                    if(veces < 2)
                    {
                        body2 = GetAlertaMail(IdUsuario, CodPais, Fecha, ref ToEmail, ref Subject,veces:veces++);

                    }
                    else
                    {

                    }

                        
                    return body2;
                }
               

            }
        }
    }

    public class AlertModel
    {
        public string IdUsuario { get; set; }
        public string CodPais { get; set; }
        public string Fecha { get; set; }
    }

    public class AlertArrayModel
    {
        public List<AlertModel> alert { get; set; }
        
    }

    public class MailLang
    {
        public string LabelSaludo { get; set; }
        public string LabelMensajeInformativo { get; set; }
        public string LabelProduct { get; set; }
        public string LabelVariacion { get; set; }
        public string LabelVariacion2 { get; set; }
        public string LabelImportadores { get; set; }
        public string LabelExportadores { get; set; }
        public string LabelConfig { get; set; }
    }

    public class AlertaMail
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Empresa { get; set; }
        public string Pais { get; set; }
        public string MesAnio { get; set; }
        public string CampoTotI { get; set; }
        public string CampoTotE { get; set; }
        public List<AlertaDetail> Alertas { get; set; }

        public bool HasAlertaI { get; set; }
        public bool HasAlertaE { get; set; }

        public string LinkGoAlertas { get; set; }

        public MailLang objLang { get; set; } 

        public string fechaVerialertas { get; set; }
        public string fechasVerialertasAnioActual { get; set; }
        public string fechasVerialertasAnioAnterior { get; set; }
    }

    public class AlertaDetail
    {
        public string Regimen;
        public string TipoOperacion;
        public TableResume AMP { get; set; }
        public TableResume AMC { get; set; }
        public TableResume APC { get; set; }
        public TableResume ACP { get; set; }
        public bool HasAlerta { get; set; } = true;

    }

    

    public class TableResume
    {
        public string LabelCompanias { get; set; }
        public string LabelCif { get; set; }
        public string LabelCifRange { get; set; }
        public List<TableDetail> Productos { get; set; }
        public List<TableDetail> Companias { get; set; }

        public class TableDetail
        {  
            public string Descripcion { get; set; }
            public string Link { get; set; }
            public string Total { get; set; }
            public string Variacion { get; set; }
            public string TotalAnio { get; set; }
            public string IconVariacion {
                get {
                    if (Variacion != null)
                    {
                        return decimal.Parse(Variacion) > 0 ? "flecha_verde.png" : "flecha_roja.png";  
                    }
                    return string.Empty;
                }
            }
            public bool MostrarLink { get; set; }
            public bool OcultarLink { get; set; }
            public List<TableDetail> FavoritosDetails { get; set; } 

            public static string GiveMeLink(string base_url, int IdFav, string TipoAlerta, string CodPais, string TipoOpe, string Fecha, string idioma,int IdFav2 = 0 )
            {   
                var _params = string.Empty;
                var _path = string.Empty;
                TipoAlerta = TipoAlerta.ToUpper();

                switch (TipoAlerta) {
                    case "AMP":
                        _path = "mis-productos";
                        break;
                    case "AMC":
                        _path = "mis-compañias";
                        break;
                    case "APC":
                        _path = "mis-busquedas";
                        break;
                    case "ACP":
                        _path = "mis-busquedas";
                        break;
                }
                _params = $"acc={TipoAlerta}&p={CodPais}&to={TipoOpe}&idFav={IdFav}&fecha={Fecha}";
                if (IdFav2 >0) _params += $"&idFav2={IdFav2}";

                string lang = "es";

                if(idioma == "ENG")
                {
                    lang = "en";
                }

                return $"{base_url}/{lang}/{_path}?{_params}";
            }

            public static List<TableDetail> Details(string idusuario,string base_url, int IdFav, string TipoAlerta, string CodPais, string TipoOpe, string Fecha, string idioma , ref bool hasData)
            {
                bool validar = false;
                List<TableDetail> tableDetail = new List<TableDetail>();
                using (var db = new ConexProvider().Open) {

                    var r41 = db.Query<dynamic>("dbo.pa_alertas_getpreferences3", new
                    {
                        idUsuario = idusuario,
                        codPais = CodPais,
                        fecha = Fecha,
                        tipoAlerta = TipoAlerta,
                        tipoOperacion = TipoOpe,
                        lang = idioma == "ESP" ? "es" : "en",
                        idValorPadre = IdFav,
                        isDetails = 1
                    }, commandType: CommandType.StoredProcedure, commandTimeout: 0);

                    if (CodPais == "BR")
                    {
                        r41 = null;
                    }

                    if (r41 != null)
                    {
                        if (TipoAlerta == "APC")
                        {
                            Parallel.ForEach(r41, (e1 => {
                                try
                                {
                                    validar = e1.TotalMes > 0;
                                    tableDetail.Add(new TableDetail
                                    {
                                        Descripcion = e1.Empresa,
                                        Total = e1.TotalMes.ToString("N0", CultureInfo.InvariantCulture),
                                        Variacion = e1.Variacion.ToString("N2", CultureInfo.InvariantCulture),
                                        Link = GiveMeLink(base_url, IdFav, TipoAlerta, CodPais, TipoOpe, Fecha, idioma, e1.IdFavorito),
                                        MostrarLink = e1.TotalMes > 0,
                                        OcultarLink = e1.TotalMes == 0,
                                        TotalAnio = e1.TotalAnio.ToString("N0", CultureInfo.InvariantCulture)
                                    });
                                }
                                catch (Exception ex)
                                {
                                    ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                                }
                            }));
                        }
                        else
                        {
                            Parallel.ForEach(r41, (e1 => {
                                try
                                {
                                    validar = e1.TotalMes > 0;
                                    tableDetail.Add(new TableDetail
                                    {
                                        Descripcion = e1.Partida,
                                        Total = e1.TotalMes.ToString("N0", CultureInfo.InvariantCulture),
                                        Variacion = e1.Variacion.ToString("N2", CultureInfo.InvariantCulture),
                                        Link = GiveMeLink(base_url, IdFav, TipoAlerta, CodPais, TipoOpe, Fecha, idioma, e1.IdFavorito),
                                        MostrarLink = e1.TotalMes > 0,
                                        OcultarLink = e1.TotalMes == 0,
                                        TotalAnio = e1.TotalAnio.ToString("N0", CultureInfo.InvariantCulture)
                                    });
                                }
                                catch (Exception ex)
                                {
                                    ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                                }
                            }));
                        }
                    }
                }

                hasData = validar;

                return tableDetail;
            }
        }
    }

    public class TableDetails
    {
        public static string idFavorito { set; get; }
        public static string empresa { set; get; }
        public static string idPartida { set; get; }
        public static string registros { set; get; }
        public static string total { set; get; }
        public static string campoTot { set; get; }
        public static string variacion { set; get; }
    }

    public class TableMasivo
    {
        public string CodPais { get; set; }
        public string FechaFin { get; set; }

        public TableMasivo()
        {

        }
    }

    public class TableResult
    {
        public string Check { get; set; }
        public int IdUsuario { get; set; }
        public string Usuario { get; set; }
        public string Empresa { get; set; }
        public string Correo { get; set; }
        public int IdPlan { get; set; }
        public string sPlan { get; set; }
        public string Paises { get; set; }


        public TableResult()
        {
           Check = "<input type='checkbox' name='chkUsuario' id='chkUsuario - IdUsuario' value='IdUsuario'>";
        }
    }
}