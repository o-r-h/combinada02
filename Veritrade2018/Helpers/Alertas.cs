using Dapper;
using Mustache;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using Veritrade2018.Controllers.SuperAdmin;

namespace Veritrade2018.Helpers
{
    public class Alertas
    {
        private readonly int commandTime = 0;
        public async Task<bool> EnviarAlertaUsuariosPorPais()
        {
            try {
                //OBTIENE LA LISTA DE PAISES A LOS QUE SE LE ENVIARA LA ALERTA, DEBE TENER LAS ALERTAS HABILITADAS
                //Y LA ULTIMA ALERTA ENVIADA PARA EL PAIS DEBE SER DEL MES ANTERIOR
                DataRowCollection rows = ObtenerPaisesHabilitadosParaAlertasAutomaticas();

                foreach (DataRow row in rows)
                {
                    var item = @row["CodPais"].ToString();
                    DateTime fechaDb = (DateTime)@row["fechaFin"];

                    if (item == "PE" || item == "CO")
                    {
                        fechaDb = fechaDb.AddMonths(-1);
                    }
                    var fecha = fechaDb.ToString("yyyyMM");
                    if (@row["fechaenvio"] == DBNull.Value || fechaDb < (DateTime)@row["fechaenvio"])
                    {
                        await SendAlertaMailMasivo(item, fecha, "");
                    }
                    
                }
                /*

                if (listaPaises.Count > 0) {
                    foreach (var item in listaPaises)
                    {
                        //PARA CADA PAIS, HACE EL ENVIO DE ALERTAS PARA USUARIOS A LOS QUE
                        //NO SE LES HAYA ENVIADO LA ALERTA DENTRO DEL MES EN CURSO Y QUE TENGAN
                        //LAS ALERTAS HABILITADAS
                        string fecha = DateTime.Now.ToString("yyyyMM");
                        if (item == "PE" || item == "CO")
                        {
                            fecha = DateTime.Now.AddMonths(-1).ToString("yyyyMM");
                        }
                        
                        await SendAlertaMailMasivo(item, fecha, "");
                    }
                }
                */

                return true;
            
            }
            catch(Exception ex)
            {
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                return false;
            }
        }

        public DataRowCollection ObtenerPaisesHabilitadosParaAlertasAutomaticas()
        {
            string sql = "SELECT DISTINCT "; 
            sql += "pais.CodPais , ";
            sql += "CONVERT(DATETIME, CONVERT(CHAR(8), concat(LEFT(bd.FechaFin, 6), '01'))) as fechaFin, ";
            sql += "alertasenviadas.fechaenvio ";
            sql += "FROM ";
            sql += "PaisRegimen pais ";
            sql += "INNER JOIN ";
            sql += "( ";
            sql += "SELECT DISTINCT ";
            sql += "codpais, ";
            sql += "fechaFin ";
            sql += "FROM ";
            sql += "basedatos) bd ";
            sql += "ON ";
            sql += "pais.CodPais = bd.CodPais ";
            sql += "INNER JOIN ";
            sql += "ConfAlertaPais conf ";
            sql += "ON ";
            sql += "pais.CodPais = conf.CodPais ";
            sql += "AND conf.activo = 1 ";
            sql += "LEFT JOIN ";
            sql += "( ";
            sql += "SELECT ";
            sql += "codpais, ";
            sql += "MAX(fechaenvio) fechaenvio ";
            sql += "FROM ";
            sql += "alertasautomaticasenviadas ";
            sql += "GROUP BY ";
            sql += "codpais) alertasenviadas ";
            sql += "ON ";
            sql += "alertasenviadas.codpais = pais.CodPais ";
            sql += "WHERE ";
            sql += "alertasenviadas.fechaenvio IS NULL ";
            sql += "OR  CONVERT(DATETIME, CONVERT(CHAR(8), concat(LEFT(bd.FechaFin, 6), '01'))) > ";
            sql += "alertasenviadas.fechaenvio ";


            var listaPaises = new List<string>();
            var dt = Conexion.SqlDataTable(sql);
            return dt.Rows;

            
        }

        [HttpPost]
        public async Task<bool> SendAlertaMailMasivo(string codPais, string fecha, string search)
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
                        catch (Exception ex)
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
                            catch (Exception ex)
                            {
                                
                                
                                
                                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                                try
                                {
                                    using (var db2 = new ConexProvider().Open)
                                    {
                                        var result2 = await db2.QueryAsync<TableMasivo>(sql1, qp);
                                        db2.Close();
                                        tableResults2 = result2.ToList();
                                    }
                                }
                                catch (Exception e)
                                {
                                    idUsuario += "," + item.IdUsuario.ToString();
                                    string sourceCurrent = MethodBase.GetCurrentMethod().DeclaringType.FullName;
                                    
                                    
                                    ExceptionUtility.LogException(e, sourceCurrent);
                                }
                            }

                            foreach (var item2 in tableResults2)
                            {
                                string IdUsuario = item.IdUsuario.ToString();
                                string CodPais = item2.CodPais;
                                string Fecha = item2.FechaFin;
                                string ToEmail = "";
                                string Subject = "";
                                string fromEmail = "";
                                
                                var body = "";
                                try
                                {
                                    body = GetAlertaMail(IdUsuario, CodPais, Fecha, ref ToEmail, ref Subject);
                                }
                                catch (Exception ex)
                                {
                                    
                                    
                                    
                                    ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                                }

                                if (body == "")
                                {
                                    idUsuario += "," + IdUsuario;
                                }

                                using (var tx = new TransactionScope())
                                {
                                    using (var db1 = new ConexProvider().Open)
                                    {
                                        try
                                        {
                                            DateTime FechaEnvio = DateTime.Now;

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
                                                FechaEnvio,
                                                Estado = "S"
                                            });

                                            

                                           Funciones.SendMail(ToEmail, Subject, body, ref fromEmail);
                                            db1.Close();
                                            tx.Complete();
                                        }
                                        catch (Exception ex)
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

                        pStart += pLength;

                    } while (tableResults.Count > 0 || !cargoData);
                    using (var tx = new TransactionScope())
                    {
                        using (var db1 = new ConexProvider().Open)
                        {
                            try
                            {
                                var sqlInsertHistorico = "insert into dbo.AlertasAutomaticasEnviadas (CodPais, FechaEnvio)" +
                                                " Values (@codPais, @FechaEnvio); ";
                                DateTime FechaEnvio = DateTime.ParseExact(fecha, "yyyyMM", null);
                                db1.Execute(sqlInsertHistorico, new
                                {
                                    codPais,
                                    FechaEnvio
                                });
                                db1.Close();
                                tx.Complete();
                            }
                            catch (Exception ex)
                            {
                                ExceptionUtility.LogException(ex, "Alertas.SendAlertaMailMasivo insert historico alertas automaticas");
                            }
                        }
                    }
                                
                    
                    return true;
                }
                catch (Exception ex)
                {
                    ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                    return false;
                }
            }
        }


        public string GetAlertaMail(string IdUsuario, string CodPais, string Fecha, ref string ToEmail, ref string Subject, string base_url = "", string tplPath = "", string culture = "", int veces = 0)
        {
            if (string.IsNullOrEmpty(base_url))
                base_url = Properties.Settings.Default.UrlWeb;

            if (string.IsNullOrEmpty(tplPath))
                tplPath = HostingEnvironment.MapPath("~/Content/tpl/alertas.mail.html");

            string template = System.IO.File.ReadAllText(tplPath);
            string idioma = FuncionesBusiness.SearchLangUsuario(IdUsuario);
            if (!string.IsNullOrEmpty(culture))
            {
                if (culture == "es")
                    idioma = "ESP";
                else
                    idioma = culture;
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
                                FechaAlerta = SUBSTRING(DATENAME(MM, @d),0,4) + ' - ' + cast(YEAR(@d) as varchar(50)),
	                            CampoTot_I = IsNull((select Top 1 isnull(Incoterm, '') from BaseDatos where CodPais = {sqlBaseData} and TipoOpe ='I'),''),
	                            CampoTot_E = IsNull((select Top 1 isnull(Incoterm, '') from BaseDatos where CodPais = {sqlBaseData} and TipoOpe ='E'),''),
                                HasAlerta_I = Cast( case when exists(Select 1 from dbo.alertapreferencias ap where ap.Idusuario=@idusu And ap.idtipoope='I' and ap.CodPais = @codPais
                                                {sqlVisibleUE} ) Then 1 Else 0 End  as bit),
                                HasAlerta_E = Cast( case when exists(Select 1 from dbo.alertapreferencias ap where ap.Idusuario=@idusu And ap.idtipoope='E' and ap.CodPais = @codPais
                                                {sqlVisibleUE} ) Then 1 Else 0 End  as bit)
                            from dbo.usuario where idusuario=@idusu;";
                bool flagException = Funciones.BuscarVariableGeneral("VVVA", CodPais);
                var r1 = db.QueryFirstOrDefault<dynamic>(sql, new { d = Fecha, codPais = CodPais, idusu = IdUsuario });
                var data = new AlertaMail()
                {
                    FullName = r1.FullName,
                    Empresa = r1.Empresa,
                    Pais = r1.Pais,
                    MesAnio = r1.MesAnio,
                    CampoTotI = r1.CampoTot_I,
                    CampoTotE = r1.CampoTot_E,
                    Email = r1.Email,
                    LinkGoAlertas = base_url + "/" + (idioma == "ESP" ? "es" : "en") + "/mis-alertas-favoritos?acc=ss",
                    HasAlertaI = r1.HasAlerta_I && !flagException,
                    HasAlertaE = r1.HasAlerta_E,
                    fechaVerialertas = r1.FechaAlerta
                };

                if (idioma == "ESP")
                {
                    MailLang obj = new MailLang()
                    {
                        LabelImportadores = "MIS IMPORTADORES",
                        LabelExportadores = "MIS EXPORTADORES",
                        LabelMensajeInformativo = "Revisa toda la información de tus últimas alertas de",
                        LabelProduct = "MIS PRODUCTOS",
                        LabelSaludo = "HOLA",
                        LabelVariacion = "VARIACION",
                        LabelConfig = "Configura tus Alertas",
                        LabelVariacion2 = "MES ANTERIOR"
                    };

                    data.objLang = obj;
                }
                else
                {
                    MailLang obj = new MailLang()
                    {
                        LabelImportadores = "MY IMPORTERS",
                        LabelExportadores = "MY EXPORTERS",
                        LabelMensajeInformativo = "Review all the information of your latest alerts",
                        LabelProduct = "MY PRODUCTS",
                        LabelSaludo = "HI",
                        LabelVariacion = "VARIATION",
                        LabelConfig = "Configure your Alerts",
                        LabelVariacion2 = "LAST MONTH"

                    };
                    data.objLang = obj;
                }

                var asunto = idioma == "ESP" ? "VeriAlertas" : "VeriAlerts";

                ToEmail = data.Email;
                Subject = $"{asunto} - {data.Pais} - {data.MesAnio}";

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
                                    Total = e.Total,
                                    Variacion = e.Variacion,
                                    Link = TableResume.TableDetail.GiveMeLink(base_url, e.IdFavorito, "AMP", CodPais, "I", Fecha, idioma),
                                    MostrarLink = e.Total > 0,
                                    OcultarLink = e.Total == 0
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
                                    Total = e.Total,
                                    Variacion = e.Variacion,
                                    Link = TableResume.TableDetail.GiveMeLink(base_url, e.IdFavorito, "AMC", CodPais, "I", Fecha, idioma),
                                    MostrarLink = e.Total > 0,
                                    OcultarLink = e.Total == 0
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
                                data.Alertas[0].APC.Productos.Add(new TableResume.TableDetail
                                {
                                    Descripcion = e.Partida,
                                    Total = e.Total,
                                    Variacion = e.Variacion,
                                    Link = TableResume.TableDetail.GiveMeLink(base_url, e.IdFavorito, "APC", CodPais, "I", Fecha, idioma),
                                    FavoritosDetails = TableResume.TableDetail.Details(IdUsuario, base_url, e.IdFavorito, "APC", CodPais, "I", Fecha, idioma, ref validar),
                                    MostrarLink = validar,
                                    OcultarLink = !validar
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
                                    Total = e1.Total,
                                    Variacion = e1.Variacion,
                                    Link = TableResume.TableDetail.GiveMeLink(base_url, IdFav2, "APC", CodPais, "I", Fecha, idioma, e1.IdFavorito),
                                    MostrarLink = e1.Total > 0,
                                    OcultarLink = e1.Total == 0
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
                                    Total = e.Total,
                                    Variacion = e.Variacion,
                                    Link = TableResume.TableDetail.GiveMeLink(base_url, e.IdFavorito, "ACP", CodPais, "I", Fecha, idioma),
                                    FavoritosDetails = TableResume.TableDetail.Details(IdUsuario, base_url, e.IdFavorito, "ACP", CodPais, "I", Fecha, idioma, ref validar),
                                    MostrarLink = validar,
                                    OcultarLink = !validar
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
                                    Total = e1.Total,
                                    Variacion = e1.Variacion,
                                    Link = TableResume.TableDetail.GiveMeLink(base_url, IdFav2, "ACP", CodPais, "I", Fecha, idioma, e1.IdFavorito),
                                    MostrarLink = e1.Total > 0,
                                    OcultarLink = e1.Total == 0
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
                                    Total = e.Total,
                                    Variacion = e.Variacion,
                                    Link = TableResume.TableDetail.GiveMeLink(base_url, e.IdFavorito, "AMP", CodPais, "E", Fecha, idioma),
                                    MostrarLink = e.Total > 0,
                                    OcultarLink = e.Total == 0
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
                                    Total = e.Total,
                                    Variacion = e.Variacion,
                                    Link = TableResume.TableDetail.GiveMeLink(base_url, e.IdFavorito, "AMC", CodPais, "E", Fecha, idioma),
                                    MostrarLink = e.Total > 0,
                                    OcultarLink = e.Total == 0
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
                                    Total = e.Total,
                                    Variacion = e.Variacion,
                                    Link = TableResume.TableDetail.GiveMeLink(base_url, e.IdFavorito, "APC", CodPais, "E", Fecha, idioma),
                                    FavoritosDetails = TableResume.TableDetail.Details(IdUsuario, base_url, e.IdFavorito, "APC", CodPais, "E", Fecha, idioma, ref validar),
                                    MostrarLink = validar,
                                    OcultarLink = !validar

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
                                    Total = e1.Total,
                                    Variacion = e1.Variacion,
                                    Link = TableResume.TableDetail.GiveMeLink(base_url, IdFav2, "APC", CodPais, "E", Fecha, idioma, e1.IdFavorito),
                                    MostrarLink = e1.Total > 0,
                                    OcultarLink = e1.Total == 0
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
                                    Total = e.Total,
                                    Variacion = e.Variacion,
                                    Link = TableResume.TableDetail.GiveMeLink(base_url, e.IdFavorito, "ACP", CodPais, "E", Fecha, idioma),
                                    FavoritosDetails = TableResume.TableDetail.Details(IdUsuario, base_url, e.IdFavorito, "ACP", CodPais, "E", Fecha, idioma, ref validar),
                                    MostrarLink = validar,
                                    OcultarLink = !validar
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
                                    Total = e1.Total,
                                    Variacion = e1.Variacion,
                                    Link = TableResume.TableDetail.GiveMeLink(base_url, IdFav2, "ACP", CodPais, "E", Fecha, idioma, e1.IdFavorito),
                                    MostrarLink = e1.Total > 0,
                                    OcultarLink = e1.Total == 0
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

                        if (item.ACP.Companias != null)
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

                    if (veces < 2)
                    {
                        body2 = GetAlertaMail(IdUsuario, CodPais, Fecha, ref ToEmail, ref Subject, veces: veces++);

                    }

                    return body2;
                }
            }
        }
    }
}