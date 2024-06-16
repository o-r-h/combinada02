using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.SessionState;
using Veritrade2017.Models;

namespace Veritrade2017.Helpers
{
    public static class Functions
    {
        //public const string servidor = "VERINFO01"; // PRODUCCION
        //public const string servidor = "VERIDEV"; //  DESARROLLO
        public const string Dominio = "www.veritrade.info"; //veritrade.dyndns.biz veritrade.info

        public const string BaseDatos = "VeritradeBusiness";

        //public const string cadena_conexion = "Data Source=" + servidor + "; Initial Catalog=" + base_datos + "; User ID=veritrade; Password=elgatofelix; MultipleActiveResultSets=True;";
        public const int RegXPag = 20;

        public const int ResXPag = 40;
        public const int FavXPag = 20;

        //public const string ruta_descarga = "http://" + servidor + "/VeritradeBusinessDownloads/";
        public const string ruta_descarga = "/VeritradeDownloads/";

        public const string ruta_winrar = "C:\\Program Files\\WinRAR\\winrar.exe";

        //public const string directorio_descarga = "D:\\VeritradeBusinessDownloads\\";
        public const string RutaDescarga = "/VeritradeDownloads/";

        public const string RutaWinrar = "C:\\Program Files\\WinRAR\\winrar.exe";

        public const string ServidorEmail = "smtp.gmail.com";
        public const int ServidorEmailPuerto = 587;
        public const string EmailEnvio = "verinews@veritrade-ltd.com";
        public const string EmailEnvioNombre = "Informativo VERINEWS";

        public const int MaxFavoritosProcesoAuto = 3;

        /// <summary>
        /// Función que retorna la cadena sin caracteres extraños (Evitar SQL Injection)
        /// </summary>
        /// <param name="cadena">cadena a limpiar</param>
        /// <returns></returns>
        public static string PrepararParaSql(string cadena)
        {
            cadena = cadena.ToUpper();
            cadena = cadena.Replace("*", "");
            cadena = cadena.Replace("%", "");
            cadena = cadena.Replace("'", "");
            cadena = cadena.Replace("#", "");
            cadena = cadena.Replace("\\", "");
            cadena = cadena.Replace("INSERT ", "");
            cadena = cadena.Replace("INTO ", "");
            cadena = cadena.Replace("UPDATE ", "");
            cadena = cadena.Replace("DELETE ", "");
            cadena = cadena.Replace("SELECT ", "");
            cadena = cadena.Replace(" IN ", "");
            cadena = cadena.Replace(" OR ", "");
            cadena = cadena.Replace(" UNION ", "");
            cadena = cadena.Replace(" DROP ", "");
            cadena = cadena.Replace(" TRUNCATE ", "");
            return cadena;
        }

        /// <summary>
        /// Retorna el rango de fecha de operación de un pais en las variables por referencia
        /// </summary>
        /// <param name="codPais">SIGLA internacional de un pais</param>
        /// <param name="tipoOpe">identificador de operacion</param>
        /// <param name="añoIni">ref string añoIni</param>
        /// <param name="mesIni">ref string mesIni</param>
        /// <param name="añoFin">ref string añoFin</param>
        /// <param name="mesFin">ref string mesFin</param>
        public static void Rango(string codPais, string tipoOpe, ref string añoIni, ref string mesIni,
            ref string añoFin, ref string mesFin)
        {
            var sql = "select FechaIni, FechaFin from BaseDatos where CodPais ='" + codPais + "' and TipoOpe = '" +
                      tipoOpe + "' ";

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                var fechaIni = row["FechaIni"].ToString();
                añoIni = fechaIni.Substring(0, 4);
                mesIni = fechaIni.Substring(4, 2);
                var fechaFin = row["FechaFin"].ToString();
                añoFin = fechaFin.Substring(0, 4);
                mesFin = fechaFin.Substring(4, 2);
            }
        }

        /// <summary>
        /// Retorna el rango de meses de operación de un pais en las variables por referencia
        /// </summary>
        /// <param name="codPais">SIGLA internacional de un pais</param>
        /// <param name="tipoOpe">identificador de operacion</param>
        /// <param name="añoMesIni">ref string añoMesIni</param>
        /// <param name="añoMesFin">ref string añoMesFin</param>
        public static void Rango(string codPais, string tipoOpe, ref string añoMesIni, ref string añoMesFin)
        {
            var sql = "select FechaIni, FechaFin from BaseDatos where CodPais ='" + codPais + "' and TipoOpe = '" +
                      tipoOpe + "' ";

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                añoMesIni = row["FechaIni"].ToString();
                añoMesFin = row["FechaFin"].ToString();
            }
        }

        /// <summary>
        /// Comprueba que el código de campaña este registrado
        /// </summary>
        /// <param name="codCampaña"></param>
        /// <returns>Valor booleano</returns>
        public static bool ExisteCodCampaña(string codCampaña)
        {
            if(codCampaña == null) {
                return false;
            }

            var flag = false;
            codCampaña = codCampaña.Replace("m", "").Replace("I", "");
            var sql = "select count(*) as CantCodCampaña from Campaña where   CodCampaña like '" + codCampaña + "%'";

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                flag = Convert.ToInt32(row["CantCodCampaña"]) > 0;
            }

            return flag;
        }
        public static List<string> ObtenerUsuariosToken()
        {
            SqlConnection cn;
            SqlCommand cmd;

            cn = new SqlConnection
            {
                ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString
            };
            cn.Open();
            try
            {
                cmd = new SqlCommand(null, cn)
                {
                    CommandText = "SELECT CodUsuario FROM UsuarioToken"
                };
                SqlDataReader reader = cmd.ExecuteReader();
                List<string> usuarios = new List<string>();
                while (reader.Read())
                {
                    usuarios.Add(String.Format("{0}", reader[0]));
                }

                return usuarios;
            }
            catch (Exception ex)
            {
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                return null;
            }
        }

        public static string CreateEditSql(string table, IDictionary<string, string> parameterMap, string werename, string weretext)
        {
            var keys = parameterMap.Keys.ToList();
            // ToList() LINQ extension method used because order is NOT
            // guaranteed with every implementation of IDictionary<TKey, TValue>

            var sql = new StringBuilder("UPDATE ").Append(table).Append(" SET ");

            for (var i = 0; i < keys.Count; i++)
            {
                sql.Append(keys[i]).Append(" = @").Append(keys[i]);
                if (i < keys.Count - 1)
                    sql.Append(", ");
            }

            return sql.Append(" WHERE ").Append("`").Append(werename).Append("`").Append(" = ").Append(weretext).ToString();
        }

        public static bool ActualizarTokensUsuario(IDictionary<string, string> usuarios)
        {
            SqlConnection cn;
            SqlCommand cmd;

            cn = new SqlConnection
            {
                ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString
            };
            
            try
            {
                cn.Open();

                foreach (KeyValuePair<string, string> usuario in usuarios)
                {
                    cmd = new SqlCommand(null, cn)
                    {
                        CommandText = "UPDATE UsuarioToken SET Token=@Token WHERE CodUsuario=@CodUsuario"
                    };
                    cmd.Parameters.AddWithValue("@Token", usuario.Value);
                    cmd.Parameters.AddWithValue("@CodUsuario", usuario.Key);
                    cmd.ExecuteNonQuery();
                }
                cn.Close();
                return true;
            }
            catch (Exception ex)
            {
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                return false;
            }
        }

        public static string CrearUsuarioFreeTrial(string empPer, string codUsuario, string password, string nombres,
            string apellidos, string dni, string empresa, string ruc,
            string telefono, string celular, string email1, string idActividad, string mensaje, string idConocio,
            string direccionIp, string codCampaña, string url, string referido, string gclid = "", string pais = "")
        {
            var codPais = "";
            string accion = "Creación";
            string sql = "";
            try
            {
                var idAplicacion = ObtieneIdAdminValor("01APL", "Business");
                var idVersion = ObtieneIdAdminValor("02VER", "Starter Pack");
                var idTipo = ObtieneIdAdminValor("03TIP", "Free Trial");
                var idOrigen = ObtieneIdAdminValor("04ORG", "Veritrade");
                var idCargo = ObtieneIdAdminValor("05CAR", "Otro");
                var idEstadoVenta = ObtieneIdAdminValor("06EVT", "Nuevo");
                var idEstadoXqNo = ObtieneIdAdminValor("07EXN", "N/A");
                //IdActividad = ObtieneIdAdminValor("08ACT", "Importación");
                //IdConocio = ObtieneIdAdminValor("09CON", "Web");

                VeritradeAdmin.Seguridad ws = new VeritradeAdmin.Seguridad();
                var ubicacion = ws.BuscaUbicacionIP2(direccionIp, ref codPais);
                if (codPais == "-") codPais = "PE";

                SqlConnection cn;
                SqlCommand cmd;

                cn = new SqlConnection
                {
                    ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString
                };
                cn.Open();
                cmd = new SqlCommand(null, cn)
                {
                    CommandText = "INSERT INTO Usuario(CodUsuario, Password, EmpPer, Empresa, RUC, Nombres, Apellidos, DNI, Telefono, Celular, Email1, " +
                                  "IdAplicacion, IdTipo, IdOrigen, IdCargo, IdEstadoVenta, IdEstadoXQNo, IdActividad, IdConocio, Mensaje, DireccionIP, " +
                                  "CodPaisIP, CodPaisIP2, CodPais, Ubicacion, CodEstado, FecInicio, FecFin, CodSeguridad, CantUsuariosUnicos, CodCampaña, " +
                                  "URLRegistro, URLReferido, Gclid, FecRegistro, FecActualizacion, FecFinAct) " +
                                  "VALUES(@CodUsuario, @Password, @EmpPer, @Empresa, @RUC, @Nombres, @Apellidos, @DNI, @Telefono, @Celular, @Email1, " +
                                  "@IdAplicacion, @IdTipo, @IdOrigen, @IdCargo, @IdEstadoVenta, @IdEstadoXQNo, @IdActividad, @IdConocio, @Mensaje, @DireccionIP, " +
                                  "@CodPaisIP, @CodPaisIP2, @CodPais, @Ubicacion, @CodEstado, @FecInicio, @FecFin, @CodSeguridad, @CantUsuariosUnicos, @CodCampaña, " +
                                  "@URLRegistro, @URLReferido, @Gclid, getdate(), getdate(), @FecFinAct)"
                };


                cmd.Parameters.AddWithValue("@CodUsuario", codUsuario);
                cmd.Parameters.AddWithValue("@Password", password);
                cmd.Parameters.AddWithValue("@EmpPer", empPer);
                cmd.Parameters.AddWithValue("@Empresa", empresa);
                cmd.Parameters.AddWithValue("@RUC", ruc);
                cmd.Parameters.AddWithValue("@Nombres", nombres);
                cmd.Parameters.AddWithValue("@Apellidos", apellidos);
                cmd.Parameters.AddWithValue("@DNI", dni);
                cmd.Parameters.AddWithValue("@Telefono", telefono);
                cmd.Parameters.AddWithValue("@Celular", celular);
                cmd.Parameters.AddWithValue("@Email1", email1);

                cmd.Parameters.AddWithValue("@IdAplicacion", idAplicacion);
                cmd.Parameters.AddWithValue("@IdTipo", idTipo);
                cmd.Parameters.AddWithValue("@IdOrigen", idOrigen);
                cmd.Parameters.AddWithValue("@IdCargo", idCargo);
                cmd.Parameters.AddWithValue("@IdEstadoVenta", idEstadoVenta);
                cmd.Parameters.AddWithValue("@IdEstadoXQNo", idEstadoXqNo);
                cmd.Parameters.AddWithValue("@IdActividad", idActividad);
                cmd.Parameters.AddWithValue("@IdConocio", idConocio);
                cmd.Parameters.AddWithValue("@Mensaje", mensaje);
                cmd.Parameters.AddWithValue("@DireccionIP", direccionIp);

                cmd.Parameters.AddWithValue("@CodPaisIP2", codPais);
                cmd.Parameters.AddWithValue("@CodPaisIP", codPais);
                cmd.Parameters.AddWithValue("@CodPais", pais);
                cmd.Parameters.AddWithValue("@Ubicacion", ubicacion);
                cmd.Parameters.AddWithValue("@CodEstado", 'A');
                cmd.Parameters.AddWithValue("@FecInicio", DateTime.Now.ToString("yyyyMMdd"));
                cmd.Parameters.AddWithValue("@FecFin", DateTime.Now.AddDays(5).ToString("yyyyMMdd"));
                cmd.Parameters.AddWithValue("@CodSeguridad", "Off");
                cmd.Parameters.AddWithValue("@CantUsuariosUnicos", 1);
                cmd.Parameters.AddWithValue("@CodCampaña", codCampaña);
                cmd.Parameters.AddWithValue("@URLRegistro", url);
                cmd.Parameters.AddWithValue("@URLReferido", referido);
                cmd.Parameters.AddWithValue("@Gclid", gclid);
                cmd.Parameters.AddWithValue("@FecFinAct", DateTime.Now.AddDays(5).ToString("yyyyMMdd"));

                sql = GetQueryExecuted(cmd);

                foreach (SqlParameter sp in cmd.Parameters)
                {
                    if (sp.Value == null)
                    {
                        sp.IsNullable = true;
                        sp.Value = DBNull.Value;
                    }
                }

                cmd.ExecuteNonQuery();

                GrabaLogUsuarioPruebaGratis(email1, nombres, apellidos, accion, "Exitoso", "", sql);

                cn.Close();

                string idUsuario = BuscarIdUsuario(codUsuario, idAplicacion);

                sql = "insert into Suscripcion(IdUsuario, IdAplicacion, IdVersion, CodPais) ";
                sql += "select " + idUsuario + " as IdUsuario, " + idAplicacion + " as IdAplicacion, " + idVersion +
                       " as IdVersion, CodPais ";
                sql += "from AdminPais2 where CodPais not in ('XX', 'PE_I', 'PE_E', 'EC_I', 'EC_E')";

                Conexion.SqlExecute(sql);
                GrabaLogUsuarioPruebaGratis(email1, nombres, apellidos, accion, "Exitoso", "", sql);

                sql = "CreaFavoritos";

                Conexion.SqlStoredProcedure1Param(sql, "IdUsuario", idUsuario);

                GrabaLogUsuarioPruebaGratis(email1, nombres, apellidos, accion, "Exitoso", "", sql);
                return idUsuario;
            }
            catch (Exception ex)
            {
                GrabaLogUsuarioPruebaGratis(email1, nombres, apellidos, accion, "Fallido", ex.Message, sql);
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                GrabaLog("0", "", "", "0", "0", "CrearUsuarioFreeTrial", ex.Message.Replace(",", ".").Replace("'", ""));
                return null;
            }
        }

        public static string ActualizarUsuarioFreeTrial(string empPer, string codUsuario, string password, string nombres,
            string apellidos, string dni, string empresa, string ruc,
            string telefono, string celular, string email1, string idActividad, string mensaje, string idConocio,
            string direccionIp, string codCampaña, string url, string referido, string gclid = "", string pais = "")
        {
            string accion = "Actualización";
            string sql = "";
            try
            {
                var codPais = "";
                var idAplicacion = ObtieneIdAdminValor("01APL", "Business");
                VeritradeAdmin.Seguridad ws = new VeritradeAdmin.Seguridad();
                var ubicacion = ws.BuscaUbicacionIP2(direccionIp, ref codPais);
                if (codPais == "-") codPais = "PE";

                SqlConnection cn;
                SqlCommand cmd;

                cn = new SqlConnection
                {
                    ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString
                };

                cn.Open();

                cmd = new SqlCommand(null, cn)
                {
                    CommandText = "UPDATE USUARIO SET EmpPer = @EmpPer, Empresa = @Empresa, RUC = @RUC, Nombres = @Nombres, Apellidos = @Apellidos, DNI = @DNI, Telefono = @Telefono, " +
                            "Celular = @Celular, IdActividad = @IdActividad, IdConocio = @IdConocio, Mensaje = @Mensaje, DireccionIP = @DireccionIP, CodPaisIP = @CodPaisIP, " +
                            "CodPaisIP2 = @CodPaisIP2, CodPais = @CodPais, Ubicacion = @Ubicacion, FecInicio = @FecInicio, FecFin = @FecFin, CodCampaña = @CodCampaña, " +
                            "URLRegistro = @URLRegistro, URLReferido = @URLReferido, Gclid = @Gclid, FecActualizacion = getdate(), FecFinAct = @FecFinAct, CodEstado = @CodEstado, " +
                            "FecRegistro = getdate() WHERE CodUsuario = @CodUsuario"
                };

                cmd.Parameters.AddWithValue("@EmpPer", empPer);
                cmd.Parameters.AddWithValue("@Empresa", empresa);
                cmd.Parameters.AddWithValue("@RUC", ruc);
                cmd.Parameters.AddWithValue("@Nombres", nombres);
                cmd.Parameters.AddWithValue("@Apellidos", apellidos);
                cmd.Parameters.AddWithValue("@DNI", dni);
                cmd.Parameters.AddWithValue("@Telefono", telefono);
                cmd.Parameters.AddWithValue("@Celular", celular);
                cmd.Parameters.AddWithValue("@IdActividad", idActividad);
                cmd.Parameters.AddWithValue("@IdConocio", idConocio);
                cmd.Parameters.AddWithValue("@Mensaje", mensaje);
                cmd.Parameters.AddWithValue("@DireccionIP", direccionIp);
                cmd.Parameters.AddWithValue("@CodPaisIP2", codPais);
                cmd.Parameters.AddWithValue("@CodPaisIP", codPais);
                cmd.Parameters.AddWithValue("@CodPais", pais);
                cmd.Parameters.AddWithValue("@Ubicacion", ubicacion);
                cmd.Parameters.AddWithValue("@FecInicio", DateTime.Now.ToString("yyyyMMdd"));
                cmd.Parameters.AddWithValue("@FecFin", DateTime.Now.AddDays(5).ToString("yyyyMMdd"));
                cmd.Parameters.AddWithValue("@CodCampaña", codCampaña);
                cmd.Parameters.AddWithValue("@URLRegistro", url);
                cmd.Parameters.AddWithValue("@URLReferido", referido);
                cmd.Parameters.AddWithValue("@Gclid", gclid);
                cmd.Parameters.AddWithValue("@FecFinAct", DateTime.Now.AddDays(5).ToString("yyyyMMdd"));
                cmd.Parameters.AddWithValue("@CodEstado", 'A');
                cmd.Parameters.AddWithValue("@CodUsuario", codUsuario);

                sql = GetQueryExecuted(cmd);

                foreach (SqlParameter sp in cmd.Parameters)
                {
                    if (sp.Value == null)
                    {
                        sp.IsNullable = true;
                        sp.Value = DBNull.Value;
                    }
                }

                cmd.ExecuteNonQuery();

                string idUsuario = BuscarIdUsuario(codUsuario, idAplicacion);
                GrabaLogUsuarioPruebaGratis(email1, nombres, apellidos, accion, "Exitoso", "", sql);
                return idUsuario;
            }
            catch (Exception ex)
            {
                GrabaLogUsuarioPruebaGratis(email1, nombres, apellidos, accion, "Fallido", ex.Message, sql);
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                GrabaLog("0", "", "", "0", "0", "CrearUsuarioFreeTrial", ex.Message.Replace(",", ".").Replace("'", ""));
                return null;
            }

        }

        public static string GetQueryExecuted(SqlCommand cmd)
        {
            string query = cmd.CommandText;

            foreach (SqlParameter p in cmd.Parameters)
            {
                if (p.Value != null)
                {
                    query = query.Replace(p.ParameterName, "\"" + p.Value.ToString() + "\"");
                }
                else
                {
                    query = query.Replace(p.ParameterName, "NULL");
                }
            }

            return (query);
        }

        public static string CrearUsuarioPlan(string empPer, string codUsuario, string password, string nombres,
            string apellidos, string dni, string empresa, string ruc, string telefono, string celular, string email1,
            string idActividad, string mensaje, string idConocio, string direccionIp, string codCampaña, string url,
            string referido, string direccion, decimal importe, int plan, string gclid = "", string pais = "")
        {
            var codPais = "";
            try
            {
                var idAplicacion = ObtieneIdAdminValor("01APL", "Business");
                var idVersion = ObtieneIdAdminValor("02VER", "Starter Pack");
                var idTipo = ObtieneIdAdminValor("03TIP", "Cliente");
                var idOrigen = ObtieneIdAdminValor("04ORG", "Veritrade");
                var idCargo = ObtieneIdAdminValor("05CAR", "Otro");
                var idEstadoVenta = ObtieneIdAdminValor("06EVT", "Nuevo");
                var idEstadoXqNo = ObtieneIdAdminValor("07EXN", "N/A");

                VeritradeAdmin.Seguridad ws = new VeritradeAdmin.Seguridad();
                var ubicacion = ws.BuscaUbicacionIP2(direccionIp, ref codPais);
                if (codPais == "-") codPais = "PE";

                var sql =
                    "insert into Usuario(CodUsuario, Password, EmpPer, Empresa, RUC, Nombres, Apellidos, DNI, Telefono, Celular, Email1, Direccion, ImporteUSD, ";
                sql +=
                    "IdAplicacion, IdTipo, IdOrigen, IdCargo, IdEstadoVenta, IdEstadoXQNo, IdActividad, IdConocio, ComentAdmin, DireccionIP, ";
                sql +=
                    "CodPaisIP, CodPaisIP2, CodPais, Ubicacion, CodEstado, FecInicio, FecFin, CodSeguridad, CantUsuariosUnicos, CodCampaña, URLRegistro, URLReferido, Gclid, ";
                sql +=
                    "FecRegistro, FecActualizacion, flagventaOnline , FecFinAct) values ";
                sql += "('" + codUsuario + "', '" + password + "', '" + empPer + "', '" + empresa + "', '" + ruc +
                       "', '" + nombres + "', '" + apellidos + "', ";
                sql += "'" + dni + "', '" + telefono + "', '" + celular + "', '" + email1 + "', '" + direccion + "', " +
                       importe.ToString().Replace(",",".") + ", ";
                sql += idAplicacion + ", " + idTipo + ", " + idOrigen + ", " + idCargo + ", " + idEstadoVenta + ", " +
                       idEstadoXqNo + ", ";
                sql += idActividad + ", " + idConocio + ", '" + mensaje + "', '" + direccionIp + "', '" + codPais +
                       "', '" + codPais + "', '" + pais + "', '" + ubicacion + "', ";
                sql += "'A', " + DateTime.Now.ToString("yyyyMMdd") + "," +
                       DateTime.Now.AddDays(364).ToString("yyyyMMdd") + ", 'Off', 1, '" + codCampaña + "', '" + url +
                       "', '" + referido + "', '" + gclid + "', getdate(), getdate() , 'S' , " + DateTime.Now.AddDays(364).ToString("yyyyMMdd") + " )";
                Conexion.SqlExecute(sql);

                string idUsuario = BuscarIdUsuario(codUsuario, idAplicacion);

                //sql = "insert into Suscripcion(IdUsuario, IdAplicacion, IdVersion, CodPais) ";
                //sql += "select " + idUsuario + " as IdUsuario, " + idAplicacion + " as IdAplicacion, " + idVersion +
                //       " as IdVersion, CodPais ";
                //sql += "from AdminPais2 where CodPais not in ('XX', 'PE_I', 'PE_E', 'EC_I', 'EC_E')";

                var listaPaises = new PlanesPaisSuscription().GetCodPaisesPlan(plan);
                foreach (var nombrePais in listaPaises)
                {
                    var sql2 = "INSERT INTO [Suscripcion] ([IdUsuario],[IdAplicacion],[IdVersion],[CodPais]) " +
                           "VALUES(" + idUsuario + "," + idAplicacion + "," + idVersion + ",'" + nombrePais + "')";
                    Conexion.SqlExecute(sql2);
                }

                sql = "CreaFavoritos";
                Conexion.SqlStoredProcedure1Param(sql, "IdUsuario", idUsuario);
                return idUsuario;
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                GrabaLog("0", "", "", "0", "0", "CrearUsuarioFreeTrial", ex.Message.Replace(",", ".").Replace("'", ""));
                return null;
            }
        }

        public static string ObtieneIdAdminValor(string codVariable, string valor)
        {
            var idAdminValor = "";
            var sql = "select IdAdminValor from AdminValor where CodVariable = '" + codVariable + "' and Valor = '" +
                      valor + "' ";

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                idAdminValor = row["IdAdminValor"].ToString();
            }

            return idAdminValor;
        }

        public static string BuscarIdUsuario(string codUsuario, string idAplicacion)
        {
            var idUsuario = "";
            var sql = "select IdUsuario from Usuario where CodUsuario = '" + codUsuario + "' and IdAplicacion = " +
                      idAplicacion;

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                idUsuario = row["IdUsuario"].ToString();
            }

            return idUsuario;
        }

        public static void BuscaDatosCorreoEnvio(string idCorreo, ref string correo, ref string nombre,
            ref string clave)
        {
            var sql = "select Correo, Nombre, Clave from Correo where IdCorreo = " + idCorreo;

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                correo = row["Correo"].ToString();
                nombre = row["Nombre"].ToString();
                clave = row["Clave"].ToString();
            }
        }

        public static string ObtieneOrigen(string idUsuario)
        {
            var origen = "";
            var sql =
                "select Valor as [Origen] from Usuario U, AdminValor A where U.IdOrigen = A.IdAdminValor and IdUsuario = " +
                idUsuario;

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                origen = row["Origen"].ToString();
            }

            return origen;
        }

        public static string BuscaUsuario(string IdUsuario)
        {
            //Retorna el nombre completo de un usuario
            //Parametros: 
            //string IdUsuario: identificador de usuario
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string Usuario = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;

            try
            {
                cn.Open();
                sql = "select Nombres + ' ' + Apellidos + ' - ' + Empresa as Usuario from Usuario where IdUsuario = " +
                      IdUsuario;
                cmd = new SqlCommand(sql, cn);
                dtr = cmd.ExecuteReader();
                if (dtr.Read()) Usuario = dtr["Usuario"].ToString();
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                string sMensaje = ex.Message;
                Usuario = "";
            }
            finally
            {
                cn.Close();
            }
            return Usuario;
        }

        public static void BuscaDatosPlanEspecial(string IdUsuario, ref string CodPais, ref string TipoOpe)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select CodPais from Suscripcion where IdUsuario = " + IdUsuario;

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read())
            {
                CodPais = dtr["CodPais"].ToString().Substring(0, 2);
                if (dtr["CodPais"].ToString().Length > 2) TipoOpe = dtr["CodPais"].ToString().Substring(3, 1);
                else TipoOpe = "I";
            }
            dtr.Close();
            cn.Close();
        }

        public static bool ExisteUsuarioEnLinea(string IdUsuario)
        {
            bool aux = false;

            List<String> UsuariosEnLinea = ObtieneUsuariosEnLinea();

            foreach (var Usuario in UsuariosEnLinea)
                if (IdUsuario == Usuario.ToString()) aux = true;

            return aux;
        }

        public static List<String> ObtieneUsuariosEnLinea()
        {
            List<String> UsuariosEnLinea = new List<String>();

            List<Hashtable> hTables = new List<Hashtable>();
            object obj = typeof(HttpRuntime).GetProperty("CacheInternal", BindingFlags.Static)
                .GetValue(null, null);
            dynamic fieldInfo = obj.GetType().GetField("_caches", BindingFlags.Instance);

            //If server uses "_caches" to store session info
            if (fieldInfo != null)
            {
                object[] _caches = (object[])fieldInfo.GetValue(obj);
                for (int i = 0; i <= _caches.Length - 1; i++)
                {
                    Hashtable hTable = (Hashtable)_caches[i].GetType()
                        .GetField("_entries", BindingFlags.Instance).GetValue(_caches[i]);
                    hTables.Add(hTable);
                }
            }
            //If server uses "_cachesRefs" to store session info
            else
            {
                fieldInfo = obj.GetType().GetField("_cachesRefs", BindingFlags.Instance);
                object[] cacheRefs = fieldInfo.GetValue(obj);
                for (int i = 0; i <= cacheRefs.Length - 1; i++)
                {
                    var target = cacheRefs[i].GetType().GetProperty("Target").GetValue(cacheRefs[i], null);
                    Hashtable hTable = (Hashtable)target.GetType()
                        .GetField("_entries", BindingFlags.Instance).GetValue(target);
                    hTables.Add(hTable);
                }
            }

            foreach (Hashtable hTable in hTables)
            {
                foreach (DictionaryEntry entry in hTable)
                {
                    object o1 = entry.Value.GetType()
                        .GetProperty("Value", BindingFlags.Instance)
                        .GetValue(entry.Value, null);
                    if (o1.GetType().ToString() == "System.Web.SessionState.InProcSessionState")
                    {
                        SessionStateItemCollection sess = (SessionStateItemCollection)o1.GetType()
                            .GetField("_sessionItems", BindingFlags.Instance).GetValue(o1);
                        if (sess != null)
                        {
                            if (sess["IdUsuario"] != null)
                            {
                                UsuariosEnLinea.Add(sess["IdUsuario"].ToString());
                            }
                        }
                    }
                }
            }

            return UsuariosEnLinea;
        }

        public static string BuscaRUC(string IdEmpresa, string CodPais)
        {
            //Retorna el numero de RUC
            //Parametros: 
            //string IdEmpresa: Identificador de la tabla empresa_(CodPais)
            //string CodPais: SIGLA internacional de un pais
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string RUC = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select RUC from Empresa_" + CodPais + " where IdEmpresa = " + IdEmpresa + " ";
            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) RUC = dtr["RUC"].ToString();
            cn.Close();

            return RUC;
        }

        public static string BuscaMarca(string IdMarca, string CodPais)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string Marca = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select Marca from Marca_" + CodPais + " where IdMarca = " + IdMarca;

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) Marca = dtr["Marca"].ToString();
            cn.Close();

            return Marca;
        }

        public static string BuscaModelo(string IdModelo, string CodPais)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string Modelo = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select Marca, Modelo from Modelo_" + CodPais + " where IdModelo = " + IdModelo;

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) Modelo = dtr["Marca"].ToString() + " - " + dtr["Modelo"].ToString();
            cn.Close();

            return Modelo;
        }

        public static string BuscaProveedor(string IdProveedor, string CodPais)
        {
            //Retorna el nombre del Proveedor
            //Parametros: 
            //string IdProveedor: Identificador de la tabla Proveedor_(CodPais)
            //string CodPais: SIGLA internacional de un pais
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string Proveedor = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();
            sql = "select Proveedor from Proveedor_" + CodPais + " where IdProveedor = " + IdProveedor;
            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) Proveedor = dtr["Proveedor"].ToString();
            cn.Close();
            return Proveedor;
        }

        public static string BuscaEmpresa(string IdEmpresa, string CodPais)
        {
            //Retorna el nombre de la empresa
            //Parametros: 
            //string IdEmpresa: Identificador de la tabla empresa_(CodPais)
            //string CodPais: SIGLA internacional de un pais
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string Empresa = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select rtrim(Empresa) as Empresa from Empresa_" + CodPais + " where IdEmpresa = " + IdEmpresa;
            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) Empresa = dtr["Empresa"].ToString();
            cn.Close();

            return Empresa;
        }

        public static string BuscaImportadorExp(string IdImportadorExp, string CodPais)
        {
            //Retorna el nombre del Imporator/Exportador
            //Parametros: 
            //string IdImportadorExp: Identificador de la tabla ImportadorExp_(CodPais)
            //string CodPais: SIGLA internacional de un pais
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string ImportadorExp = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select ImportadorExp from ImportadorExp_" + CodPais + " where IdImportadorExp = " + IdImportadorExp;
            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) ImportadorExp = dtr["ImportadorExp"].ToString();

            cn.Close();

            return ImportadorExp;
        }

        public static string BuscaPais(string IdPais, string CodPais)
        {
            //Retorna el nombre del pais
            //Parametros: 
            //string IdPais: Identificador de la tabla Pais_(CodPais)
            //string CodPais: SIGLA internacional de un pais
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string Pais = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();
            sql = "select Pais from Pais_" + CodPais + " where IdPais = " + IdPais;
            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) Pais = dtr["Pais"].ToString();
            cn.Close();
            return Pais;
        }

        public static string BuscaVia(string IdViaTransp, string CodPais)
        {
            //Retorna 
            //Parametros: 
            //string IdViaTransp: Identificador de la tabla ViaTransp_(CodPais)
            //string CodPais: SIGLA internacional de un pais
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string ViaTransp = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();
            sql = "select ViaTransp from ViaTransp_" + CodPais + " where IdViaTransp = " + IdViaTransp;
            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) ViaTransp = dtr["ViaTransp"].ToString();
            cn.Close();
            return ViaTransp;
        }

        public static string BuscaAduana(string IdAduana, string CodPais)
        {
            //Retorna 
            //Parametros: 
            //string IdAduana: Identificador de la tabla Aduana_(CodPais)
            //string CodPais: SIGLA internacional de un pais
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string Aduana = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();
            sql = "select Aduana from Aduana_" + CodPais + " where IdAduana = " + IdAduana;
            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) Aduana = dtr["Aduana"].ToString();
            cn.Close();
            return Aduana;
        }

        public static string BuscaAñoMes(string IdAñoMes)
        {
            //Se usa?
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string AñoMes = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();
            if (IdAñoMes.Length == 8) IdAñoMes = IdAñoMes.Substring(0, 6);
            sql = "select AñoMesES from AñoMes where IdAñoMes = " + IdAñoMes;
            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) AñoMes = dtr["AñoMesES"].ToString();
            cn.Close();
            return AñoMes;
        }

        public static string BuscaGrupo(string IdGrupo)
        {
            //Retorna el nombre del grupo
            //Parametros: 
            //string IdGrupo: Identificador de la tabla Grupo
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string Grupo = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();
            sql = "select Grupo from Grupo where IdGrupo = " + IdGrupo;
            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) Grupo = "(GROUP) " + dtr["Grupo"].ToString();
            cn.Close();
            return Grupo;
        }

        public static string BuscaNombre(string Variable, string Valor, string CodPais)
        {
            //Retorna el nombre del identificador de la variable
            //Parametros: 
            //string Variable: nombre del identificador de la tabla
            //string Valor: Identificador de la tabla
            //string CodPais: SIGLA internacional de un pais

            string aux = "";

            switch (Variable)
            {
                case "IdPartida":
                    aux = BuscaPartida(Valor, CodPais);
                    break;
                case "IdImportador":
                    aux = BuscaEmpresa(Valor, CodPais);
                    break;
                case "IdProveedor":
                    aux = BuscaProveedor(Valor, CodPais);
                    break;
                case "IdDistrito":
                    aux = BuscaDistrito(Valor, CodPais);
                    break;
                case "IdPaisOrigen":
                    aux = BuscaPais(Valor, CodPais);
                    break;
                case "IdExportador":
                    aux = BuscaEmpresa(Valor, CodPais);
                    break;
                case "IdImportadorExp":
                    aux = BuscaImportadorExp(Valor, CodPais);
                    break;
                case "IdPaisDestino":
                    aux = BuscaPais(Valor, CodPais);
                    break;
                case "IdViaTransp":
                    aux = BuscaVia(Valor, CodPais);
                    break;
                case "IdAduana":
                    aux = BuscaAduana(Valor, CodPais);
                    break;
            }
            return aux;
        }


        public static string ListaTitulosDescarga(string IdDescargaCab, string Campos, string CodPais = "",
            string TipoOpe = "")
        {
            //Retorna 
            //Parametros: 
            //string IdDescargaCab: 
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string lista = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            if (IdDescargaCab != "")
                sql = "select CampoFav from DescargaDet where IdDescargaCab = " + IdDescargaCab + " ";
            else
                sql =
                    "select CampoFav from DescargaDet where IdDescargaCab in (select IdDescargaCab from DescargaCab where IdUsuario = 0 and CodPais = '" +
                    CodPais + "' and TipoOpe = '" + TipoOpe + "') ";

            if (Campos != "") sql += "and Campo in " + Campos;

            sql += "order by NroCampo";

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            while (dtr.Read())
            {
                lista += "''" + dtr["CampoFav"].ToString() + "'', ";
            }
            lista = lista.Substring(0, lista.Length - 2);
            lista = " " + lista + " ";
            cn.Close();
            return lista;
        }

        public static string ListaCamposDescarga(string IdDescargaCab, string Campos, string CodPais = "",
            string TipoOpe = "")
        {
            //Retorna 
            //Parametros: 
            //string IdDescargaCab:
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string lista = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            if (IdDescargaCab != "")
                sql = "select Campo from DescargaDet where IdDescargaCab = " + IdDescargaCab + " ";
            else
                sql =
                    "select Campo from DescargaDet where IdDescargaCab in (select IdDescargaCab from DescargaCab where IdUsuario = 0 and CodPais = '" +
                    CodPais + "' and TipoOpe = '" + TipoOpe + "') ";

            if (Campos != "") sql += "and Campo in " + Campos;

            sql += "order by NroCampo";

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            while (dtr.Read())
            {
                lista += "[" + dtr["Campo"].ToString() + "], ";
            }
            lista = lista.Substring(0, lista.Length - 2);
            cn.Close();
            return lista;
        }

        public static string ProcesaDescarga(string Accion, string IdDescargaCab, string Descarga, string IdUsuario,
            string CodPais, string TipoOpe, bool Default = false)
        {
            //Retorna 
            //Parametros: 
            //string IdDescargaCab:
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;

            string FlagDefault = "null";
            if (Default) FlagDefault = "'S'";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();
            switch (Accion)
            {
                case "Crea":
                    if (Default)
                    {
                        sql = "update DescargaCab set FlagDefault = null where IdUsuario = " + IdUsuario +
                              " and CodPais = '" + CodPais + "' and TipoOpe = '" + TipoOpe + "' ";
                        cmd = new SqlCommand(sql, cn);
                        cmd.ExecuteScalar();
                    }
                    sql = "select max(IdDescargaCab) + 1 from DescargaCab where IdDescargaCab > 100";
                    cmd = new SqlCommand(sql, cn);
                    dtr = cmd.ExecuteReader();
                    dtr.Read();
                    if (dtr[0].ToString() != "") IdDescargaCab = dtr[0].ToString();
                    else IdDescargaCab = "101";
                    dtr.Close();
                    sql = "insert into DescargaCab(IdDescargaCab, IdUsuario, CodPais, TipoOpe, Descarga, FlagDefault) ";
                    sql += "values (" + IdDescargaCab + "," + IdUsuario + ", '" + CodPais + "','" + TipoOpe + "', '" +
                           Descarga + "', " + FlagDefault + ")";
                    cmd = new SqlCommand(sql, cn);
                    cmd.ExecuteScalar();
                    break;
                case "Actualiza":
                    if (Default)
                    {
                        sql = "update DescargaCab set FlagDefault = null where IdUsuario = " + IdUsuario +
                              " and CodPais = '" + CodPais + "' and TipoOpe = '" + TipoOpe + "' ";
                        cmd = new SqlCommand(sql, cn);
                        cmd.ExecuteScalar();
                    }
                    sql = "update DescargaCab set Descarga = '" + Descarga + "', FlagDefault = " + FlagDefault +
                          " where IdDescargaCab = " + IdDescargaCab;
                    cmd = new SqlCommand(sql, cn);
                    cmd.ExecuteScalar();
                    break;
                case "Elimina":
                    sql = "delete from DescargaDet where IdDescargaCab = " + IdDescargaCab;
                    cmd = new SqlCommand(sql, cn);
                    cmd.ExecuteScalar();
                    sql = "delete from DescargaCab where IdDescargaCab = " + IdDescargaCab;
                    cmd = new SqlCommand(sql, cn);
                    cmd.ExecuteScalar();
                    break;
            }
            cn.Close();
            return IdDescargaCab;
        }

        public static void AgregaCampoDescarga(string IdDescargaCab, string NroCampo, string Campo, string CampoFav)
        {
            //Retorna 
            //Parametros: 
            //string IdDescargaCab:

            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            int cantidad;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select count(*) from DescargaDet where IdDescargaCab = " + IdDescargaCab + " and Campo = '" + Campo +
                  "' ";
            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            dtr.Read();
            cantidad = (int)dtr[0];
            dtr.Close();
            if (cantidad == 0)
            {
                sql = "insert into DescargaDet(IdDescargaCab, NroCampo, Campo, CampoFav) values ";
                sql += "(" + IdDescargaCab + ", " + NroCampo + ", '" + Campo + "', '" + CampoFav + "')";
                cmd = new SqlCommand(sql, cn);
                cmd.ExecuteScalar();
            }
            else
            {
                sql = "update DescargaDet set CampoFav = '" + CampoFav + "' where IdDescargaCab = " + IdDescargaCab +
                      " and Campo = '" + Campo + "' ";
                cmd = new SqlCommand(sql, cn);
                cmd.ExecuteScalar();
            }
            cn.Close();
        }

        public static void EliminaCampoDescarga(string IdDescargaCab, string Campo)
        {
            //Retorna 
            //Parametros: 
            //string IdDescargaCab:

            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            int cantidad;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select count(*) from DescargaDet where IdDescargaCab = " + IdDescargaCab + " and Campo = '" + Campo +
                  "' ";
            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            dtr.Read();
            cantidad = (int)dtr[0];
            dtr.Close();
            if (cantidad > 0)
            {
                sql = "delete from DescargaDet where IdDescargaCab = " + IdDescargaCab + " and Campo = '" + Campo +
                      "' ";
                cmd = new SqlCommand(sql, cn);
                cmd.ExecuteScalar();
            }
            cn.Close();
        }

        public static string ListaItems(ArrayList Lista)
        {
            string aux = "(";
            for (int i = 0; i < Lista.Count; i++)
                aux += Lista[i].ToString() + ", ";
            aux = aux.Substring(0, aux.Length - 2);
            aux += ")";
            return aux;
        }

        public static string ListaItemsS(ArrayList Lista)
        {
            string aux = "(";
            for (int i = 0; i < Lista.Count; i++)
                aux += "'" + Lista[i].ToString() + "', ";
            aux = aux.Substring(0, aux.Length - 2);
            aux += ")";
            return aux;
        }

        public static string ListaNombres(ArrayList Lista, string Variable, string CodPais)
        {
            string aux = "";
            for (int i = 0; i < Lista.Count; i++)
                aux += BuscaVariable(Variable, "F" + Lista[i].ToString(), CodPais);
            return aux;
        }

        public static int CantFavoritos(string TipoFavorito, string IdUsuario, string CodPais, string TipoOpe)
        {
            string sql = "";
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            int Cant = 0;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            switch (TipoFavorito)
            {
                case "Partida":
                    sql = "select count(*) as Cant from PartidaFav where IdUsuario = " + IdUsuario +
                          " and CodPais = '" + CodPais + "' ";
                    sql += "and TipoOpe = '" + TipoOpe + "' ";
                    break;
                case "Importador":
                    sql = "select count(*) as Cant from EmpresaFav where IdUsuario = " + IdUsuario +
                          " and CodPais = '" + CodPais + "' ";
                    sql += "and TipoOpe = 'I' ";
                    break;
                case "Proveedor":
                    sql = "select count(*) as Cant from ProveedorFav where IdUsuario = " + IdUsuario +
                          " and CodPais = '" + CodPais + "' ";
                    break;
                case "Exportador":
                    sql = "select count(*) as Cant from EmpresaFav where IdUsuario = " + IdUsuario +
                          " and CodPais = '" + CodPais + "' ";
                    sql += "and TipoOpe = 'E' ";
                    break;
                case "ImportadorExp":
                    sql = "select count(*) as Cant from ImportadorExpFav where IdUsuario = " + IdUsuario +
                          " and CodPais = '" + CodPais + "' ";
                    break;
            }

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read())
                Cant = Convert.ToInt32(dtr["Cant"]);
            dtr.Close();

            cn.Close();

            return Cant;
        }

        public static string BuscaFavorito(string TipoFavorito, string IdUsuario, string CodPais, string TipoOpe,
            string IdFavorito)
        {
            //Retorna el Identificador de: Partida, Empresa, Proveedor, ImportadorExp
            //Parametros: 
            //string TipoFavorito: tabla
            //string IdUsuario: identificador del usuario
            //string CodPais: identificador de la tabla pais
            //string TipoOpe: Importación / Exportación 
            //string IdFavorito: Identificador de la tabla TipoFavorito
            string sql = "";
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string Id = "";
            string PartidaFav;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();
            switch (TipoFavorito)
            {
                case "Partida":
                    PartidaFav = BuscaPartida(IdFavorito, CodPais);
                    if (PartidaFav.Length > 60) PartidaFav = PartidaFav.Substring(0, 60);
                    sql = "Select IdPartida as Id From PartidaFav ";
                    sql += " Where IdUsuario = " + IdUsuario + " ";
                    sql += " and CodPais = '" + CodPais + "' ";
                    sql += " and TipoOpe = '" + TipoOpe + "' ";
                    sql += " and IdPartida = " + IdFavorito + " ";
                    sql += " and PartidaFav = '" + PartidaFav + "' ";
                    break;
                case "Importador":
                    sql = "Select IdEmpresa as Id From EmpresaFav ";
                    sql += " Where IdUsuario = " + IdUsuario + " ";
                    sql += " and CodPais = '" + CodPais + "' ";
                    sql += " and TipoOpe = 'I' ";
                    sql += " and IdEmpresa = " + IdFavorito + " ";
                    break;
                case "Proveedor":
                    sql = "Select IdProveedor as Id From ProveedorFav ";
                    sql += " Where IdUsuario = " + IdUsuario + " ";
                    sql += " and CodPais = '" + CodPais + "' ";
                    sql += " and TipoOpe = 'I' ";
                    sql += " and IdProveedor = " + IdFavorito + " ";
                    break;
                case "Exportador":
                    sql = "Select IdEmpresa as Id From EmpresaFav ";
                    sql += " Where IdUsuario = " + IdUsuario + " ";
                    sql += " and CodPais = '" + CodPais + "' ";
                    sql += " and TipoOpe = 'E' ";
                    sql += " and IdEmpresa = " + IdFavorito + " ";
                    break;
                case "ImportadorExp":
                    sql = "Select IdImportadorExp as Id From ImportadorExpFav ";
                    sql += " Where IdUsuario = " + IdUsuario + " ";
                    sql += " and CodPais = '" + CodPais + "' ";
                    sql += " and TipoOpe = 'E' ";
                    sql += " and IdImportadorExp = " + IdFavorito + " ";
                    break;
            }
            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) Id = dtr["Id"].ToString();
            cn.Close();
            return Id;
        }

        public static void ActualizaPartidaFavorita(string IdUsuario, string CodPais, string TipoOpe, string IdPartida,
            string PartidaFav)
        {
            //Actualiza un registro en Partida_favorito
            //Parametros: 
            //string IdUsuario: identificador del usuario
            //string CodPais: identificador de la tabla pais
            //string TipoOpe: Importación / Exportación 
            //string IdFavorito: Identificador de la tabla TipoFavorito
            //string PartidaFav:
            string sql = "";
            SqlConnection cn;
            SqlCommand cmd;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            SqlParameter param = new SqlParameter();
            param.ParameterName = "@PartidaFav";
            param.Value = PartidaFav;

            sql = "update PartidaFav set PartidaFav = @PartidaFav where IdUsuario = " + IdUsuario + " and CodPais = '" +
                  CodPais + "' and TipoOpe = '" + TipoOpe + "' and IdPartida = " + IdPartida;
            cmd = new SqlCommand(sql, cn);
            cmd.Parameters.Add(param);
            cmd.ExecuteNonQuery();
            cn.Close();
        }

        public static void EliminaFavorito(string TipoFavorito, string IdUsuario, string CodPais, string TipoOpe,
            string IdFavorito)
        {
            //Elimina un registro favorito en TipoFavorito
            //Parametros: 
            //string TipoFavorito: tabla
            //string IdUsuario: identificador del usuario
            //string CodPais: identificador de la tabla pais
            //string TipoOpe: Importación / Exportación 
            //string IdFavorito: Identificador de la tabla TipoFavorito

            string sql = "";
            SqlConnection cn;
            SqlCommand cmd;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();
            switch (TipoFavorito)
            {
                case "Partida":
                    sql = "delete from PartidaFav where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais +
                          "' and TipoOpe = '" + TipoOpe + "' and IdPartida = " + IdFavorito;
                    break;
                case "Importador":
                    sql = "delete from EmpresaFav where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais +
                          "' and TipoOpe = 'I' and IdEmpresa = " + IdFavorito;
                    break;
                case "Proveedor":
                    sql = "delete from ProveedorFav where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais +
                          "' and IdProveedor = " + IdFavorito;
                    break;
                case "Exportador":
                    sql = "delete from EmpresaFav where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais +
                          "' and TipoOpe = 'E' and IdEmpresa = " + IdFavorito;
                    break;
                case "ImportadorExp":
                    sql = "delete from ImportadorExpFav where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais +
                          "' and IdImportadorExp = " + IdFavorito;
                    break;
            }
            cmd = new SqlCommand(sql, cn);
            cmd.ExecuteNonQuery();
            cn.Close();
        }

        public static void EliminaFavoritos(string IdUsuario, string CodPais, string TipoOpe)
        {
            string sql = "";
            SqlConnection cn;
            SqlCommand cmd;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "delete from PartidaFav where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais +
                  "' and TipoOpe = '" + TipoOpe + "' ";
            if (ExisteVariable(CodPais, TipoOpe, "IdImportador"))
                sql += "delete from EmpresaFav where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais +
                       "' and TipoOpe = 'I' ";
            if (ExisteVariable(CodPais, TipoOpe, "IdProveedor"))
                sql += "delete from ProveedorFav where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' ";
            if (ExisteVariable(CodPais, TipoOpe, "IdExportador"))
                sql += "delete from EmpresaFav where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais +
                       "' and TipoOpe = 'E' ";
            if (ExisteVariable(CodPais, TipoOpe, "IdImportadorExp"))
                sql += "delete from ImportadorExpFav where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais +
                       "' ";

            cmd = new SqlCommand(sql, cn);
            cmd.ExecuteNonQuery();

            cn.Close();
        }

        public static string BuscaDistrito(string IdDistrito, string CodPais)
        {
            //Retorna el nombre del Distrito
            //Parametros: 
            //string IdDistrito: Identificador de la tabla Distrito_(CodPais)
            //string CodPais: SIGLA internacional de un pais
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string Distrito = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();
            sql = "select Distrito from Distrito_" + CodPais + " where IdDistrito = " + IdDistrito;
            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) Distrito = dtr["Distrito"].ToString();
            cn.Close();
            return Distrito;
        }

        public static bool FlagCarga(string CodPais)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            bool aux = true;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            if (CodPais.Contains("_")) CodPais = CodPais.Replace("_", "I");

            sql = "select FlagCarga from BaseDatos where CodPais = '" + CodPais + "' ";

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            dtr.Read();
            aux = (dtr["FlagCarga"].ToString() == "S");
            cn.Close();

            return aux;
        }

        public static string ObtieneCodPaisAcceso(string IdUsuario)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string CodPais;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select top 1 B.CodPais from BaseDatos B, Suscripcion S ";
            sql += "where B.CodPais = S.CodPais and FlagCarga = 'N' and IdUsuario = " + IdUsuario + " order by 1";

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            dtr.Read();
            CodPais = dtr["CodPais"].ToString();

            dtr.Close();

            return CodPais;
        }

        public static string ObtieneCodPaisAccesoManifiesto(string IdUsuario)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string CodPais;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select top 1 B.CodPais from BaseDatos B, Suscripcion S ";
            sql += "where B.CodPais = S.CodPais and FlagCarga = 'N' and IdUsuario = " + IdUsuario +
                   " and S.CodPais in ('PEI', 'PEE', 'USI', 'USE') order by 1";

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            dtr.Read();
            CodPais = dtr["CodPais"].ToString();

            dtr.Close();

            return CodPais;
        }

        public static string BuscaCodEstado(string CodUsuario)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string CodEstado = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select CodEstado ";
            sql += "from Usuario where CodUsuario = '" + CodUsuario + "' and IdAplicacion = 1";

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            dtr.Read();
            if (dtr.HasRows)
                CodEstado = dtr["CodEstado"].ToString();
            dtr.Close();

            cn.Close();

            return CodEstado;
        }


        public static bool ExisteVariable(string CodPais, string TipoOpe, string Variable1)
        {
            bool aux;

            if (CodPais == "PEP") CodPais = "PE";

            var sql = "select distinct codpais, tipoope, variable1 from VariableAgrupTest where CodPais = '" + CodPais +
                      "' and TipoOpe = '" + TipoOpe + "' ";
            sql += "and Variable1 = '" + Variable1 + "' ";

            var dt = Conexion.SqlDataTable(sql);
            aux = dt.Rows.Count > 0 ? true : false;

            return aux;
        }

        public static bool ValidaVisitasMes(string IdUsuario, ref int LimiteVisitas, ref int Visitas)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;

            string IdPlan;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            IdPlan = ObtieneIdPlan(IdUsuario);

            sql = "select LimiteVisitas from [Plan] where IdPlan = " + IdPlan;

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            dtr.Read();
            LimiteVisitas = Convert.ToInt32(dtr["LimiteVisitas"]);
            dtr.Close();

            sql = "select count(*) as Visitas from Historial where CodEstado is null and IdUsuario = " + IdUsuario +
                  " ";
            sql += "and year(FecVisita) * 100 + month(FecVisita) = " +
                   (DateTime.Today.Year * 100 + DateTime.Today.Month).ToString();

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read())
                Visitas = Convert.ToInt32(dtr["Visitas"]);
            dtr.Close();

            cn.Close();

            return (Visitas < LimiteVisitas);
        }

        public static bool SessionUnica(string CodUsuario)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            bool aux = true;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select SesionUnica from Usuario where CodUsuario = '" + CodUsuario + "' ";

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            dtr.Read();
            aux = (dtr["SesionUnica"].ToString() == "S");
            cn.Close();

            return aux;
        }

        public static string BuscaTipoUsuario(string IdUsuario)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string TipoUsuario;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select Valor as TipoUsuario from Usuario U, AdminValor V ";
            sql += "where U.IdUsuario = " + IdUsuario + " and V.CodVariable = '03TIP' and U.IdTipo = V.IdAdminValor ";

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            dtr.Read();
            TipoUsuario = dtr["TipoUsuario"].ToString();
            dtr.Close();
            return TipoUsuario;
        }

        public static int BuscaContador(string IdUsuario)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            int Contador = 0;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select Contador from Contador where IdUsuario = " + IdUsuario;

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read())
                Contador = Convert.ToInt32(dtr["Contador"]);
            dtr.Close();
            return Contador;
        }

        public static void IncrementaContador(string IdUsuario)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            int Contador = 0;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select Contador from Contador where IdUsuario = " + IdUsuario;

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read())
                Contador = Convert.ToInt32(dtr["Contador"]);
            else
                dtr.Close();

            Contador = Contador + 1;

            if (Contador == 1)
                sql = "insert into Contador(IdUsuario, Contador, FecInicio) values (" + IdUsuario + ", 1, getdate())";
            else
                sql = "update Contador set Contador = " + Contador + " where IdUsuario = " + IdUsuario;

            cmd = new SqlCommand(sql, cn);
            cmd.ExecuteScalar();
            cn.Close();
        }

        public static bool ValidaPais(string IdUsuario, string CodPais)
        {
            //Función que verifica si el usuario esta suscrito al pais
            //Parametros: 
            //string IdUsuario: identificador del usuario
            //string CodPais: identificador de la tabla pais
            //Return: Verdadero o Falso
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            int Cant;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            if (CodPais.Contains("_")) CodPais = CodPais.Replace("_", "I");

            sql = "select count(*) as Cant from Suscripcion S, BaseDatos B ";
            sql += "where (S.CodPais = B.CodPaisSuscripcion or S.CodPais = B.CodPais) ";
            sql += "and IdUsuario = " + IdUsuario + " and B.CodPais = '" + CodPais + "' ";
            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            dtr.Read();
            Cant = (int)dtr["Cant"];
            dtr.Close();
            if (Cant > 0) return true;
            else return false;
        }

        public static string Pais(string CodPais, string Idioma)
        {
            //Retorna el nombre del pais
            //Parametros: 
            //string CodPais: identificador de la tabla pais

            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string Pais = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select Pais, PaisES from AdminPais2 where CodPais = '" + CodPais + "'";
            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();

            if (dtr.Read())
                if (Idioma == "es") Pais = dtr["PaisES"].ToString().ToUpper();
                else Pais = dtr["Pais"].ToString().ToUpper();

            cn.Close();

            return Pais;
        }


        public static string ObtienePlan(string IdUsuario)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;

            string Plan;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql =
                "select Valor as [Plan] from Usuario U, AdminValor A where U.IdPlan = A.IdAdminValor and IdUsuario = " +
                IdUsuario;

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            dtr.Read();
            Plan = dtr["Plan"].ToString();
            dtr.Close();

            cn.Close();

            return Plan;
        }

        public static bool ExisteGrupo(string Grupo, string IdUsuario, string CodPais, string TipoOpe, string TipoFav)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            int cant = 0;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select count(*) as cant from Grupo where Grupo = '" + Grupo + "' and IdUsuario = " + IdUsuario + " ";
            sql += "and CodPais = '" + CodPais + "' and TipoOpe = '" + TipoOpe + "' and TipoFav = '" + TipoFav + "'";

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            dtr.Read();
            cant = Convert.ToInt32(dtr["cant"]);
            dtr.Close();

            cn.Close();

            return (cant > 0);
        }

        public static string BuscaRUCUsuario(string IdUsuario)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string RUC = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select RUC from Usuario where IdUsuario = " + IdUsuario;

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) RUC = dtr["RUC"].ToString();
            dtr.Close();

            cn.Close();

            return RUC;
        }

        public static string BuscaIdPartida(string Nandina, string CodPais)
        {
            //Retorna el identificador de la partida
            //Parametros: 
            //string Nandina: 
            //string CodPais: SIGLA internacional de un pais
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string IdPartida = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();
            sql = "select IdPartida from Partida_" + CodPais + " where Nandina = '" + Nandina + "'";
            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) IdPartida = dtr["IdPartida"].ToString();
            cn.Close();
            return IdPartida;
        }

        public static string BuscaPartida(string IdPartida, string CodPais)
        {
            //Retorna el nombre de la partida
            //Parametros: 
            //string IdPartida: Identificador de la tabla partida_(CodPais)
            //string CodPais: SIGLA internacional de un pais
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string Partida = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();
            sql = "select Nandina + ' ' + Partida as Partida from Partida_" + CodPais + " where IdPartida = " +
                  IdPartida;
            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) Partida = dtr["Partida"].ToString();
            cn.Close();
            return Partida;
        }

        public static string BuscaSubCapitulo(string CodSubCapitulo, string CodIdioma = "")
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string SubCapitulo = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select " + (CodIdioma == "" ? "SubCapitulo" : "SubCapituloEN") +
                  " as SubCapitulo from SubCapitulo where CodSubCapitulo = '" + CodSubCapitulo + "' ";

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) SubCapitulo = dtr["SubCapitulo"].ToString();

            cn.Close();

            return SubCapitulo;
        }

        public static string BuscaHTS6(string CodHTS6, string CodIdioma = "")
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string HTS6 = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select " + (CodIdioma == "" ? "HTS6" : "HTS6_EN") + " as HTS6 from HTS6 where CodHTS6 = '" +
                  CodHTS6 + "' ";

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) HTS6 = dtr["HTS6"].ToString();

            cn.Close();

            return HTS6;
        }

        public static string BuscaDescripcionSentinel(string CodTabla, string Valor)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string Descripcion = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select Descripcion from Sentinel where CodTabla = '" + CodTabla + "' and Valor = '" + Valor + "'";

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) Descripcion = dtr["Descripcion"].ToString();
            dtr.Close();

            cn.Close();

            return Descripcion;
        }

        public static string ObtieneIdPlan(string IdUsuario)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;

            string IdPlan;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select IdPlan from Usuario where IdUsuario = " + IdUsuario;

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            dtr.Read();
            IdPlan = dtr["IdPlan"].ToString();
            dtr.Close();

            cn.Close();

            return IdPlan;
        }

        public static int ObtieneLimite(string IdPlan, string TipoLimite)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            int aux = 0;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select " + TipoLimite + " from [Plan] where IdPlan = '" + IdPlan + "'";

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            dtr.Read();
            aux = Convert.ToInt32(dtr[TipoLimite]);
            dtr.Close();

            cn.Close();

            return aux;
        }

        public static int CantFavUnicos(string IdUsuario, string CodPais, string TipoOpe, string TipoFavorito)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;

            int FavUnicos = 0;

            string TipoFav;
            if (TipoFavorito != "ImportadorExp") TipoFav = TipoFavorito.Substring(0, 2).ToUpper();
            else TipoFav = "IE";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select count(*) as FavUnicos from V_FavUnicos ";
            sql += "where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' ";
            sql += "and TipoOpe = '" + TipoOpe + "' and TipoFav = '" + TipoFav + "' ";

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read())
                FavUnicos = Convert.ToInt32(dtr["FavUnicos"]);
            dtr.Close();

            cn.Close();

            return FavUnicos;
        }

        public static bool ValidaDescargasMes(string IdUsuario, string CodPais, string TipoOpe, ref int LimiteDescargas,
            ref int Descargas)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;

            string IdPlan;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            IdPlan = ObtieneIdPlan(IdUsuario);

            sql = "select LimiteDescargas from [Plan] where IdPlan = " + IdPlan;

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read())
                LimiteDescargas = Convert.ToInt32(dtr["LimiteDescargas"]);
            dtr.Close();

            sql = "select count(*) as Descargas from HistorialDescargas ";
            sql += "where IdUsuario = " + IdUsuario + " ";
            sql += "and year(FecDescarga) * 100 + month(FecDescarga) = " +
                   (DateTime.Today.Year * 100 + DateTime.Today.Month).ToString();

            /*
            sql = "select count(*) as Descargas from HistorialDescargas ";
            sql += "where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' and TipoOpe = '" + TipoOpe + "' ";
            sql += "and year(FecDescarga) * 100 + month(FecDescarga) = " + (DateTime.Today.Year * 100 + DateTime.Today.Month).ToString();
            */

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read())
                Descargas = Convert.ToInt32(dtr["Descargas"]);
            dtr.Close();

            cn.Close();

            return (Descargas < LimiteDescargas);
        }

        public static bool ValidaFavUnicos(string IdUsuario, string CodPais, string TipoOpe, string TipoFavorito)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;

            string IdPlan;

            int LimiteFavUnicos, FavUnicos = 0;

            string TipoFav;
            if (TipoFavorito != "ImportadorExp") TipoFav = TipoFavorito.Substring(0, 2).ToUpper();
            else TipoFav = "IE";

            IdPlan = ObtieneIdPlan(IdUsuario);

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select LimiteFavUnicos from [Plan] where IdPlan = " + IdPlan;

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            dtr.Read();
            LimiteFavUnicos = Convert.ToInt32(dtr["LimiteFavUnicos"]);
            dtr.Close();

            sql = "select count(*) as FavUnicos from V_FavUnicos ";
            sql += "where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' ";
            sql += "and TipoOpe = '" + TipoOpe + "' and TipoFav = '" + TipoFav + "' ";

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read())
                FavUnicos = Convert.ToInt32(dtr["FavUnicos"]);
            dtr.Close();
            cn.Close();

            return (FavUnicos < LimiteFavUnicos);
        }

        public static bool ValidaFavPorGrupo(string IdUsuario, string IdGrupo)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;

            string IdPlan;

            int LimiteFavPorGrupo, FavPorGrupo = 0;

            IdPlan = ObtieneIdPlan(IdUsuario);

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select LimiteFavPorGrupo from [Plan] where IdPlan = " + IdPlan;

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            dtr.Read();
            LimiteFavPorGrupo = Convert.ToInt32(dtr["LimiteFavPorGrupo"]);
            dtr.Close();

            sql = "select count(*) as FavPorGrupo from FavoritoGrupo where IdGrupo = " + IdGrupo;

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            dtr.Read();
            FavPorGrupo = Convert.ToInt32(dtr["FavPorGrupo"]);
            dtr.Close();

            cn.Close();

            return (FavPorGrupo < LimiteFavPorGrupo);
        }

        public static bool ValidaGrupos(string IdUsuario, string CodPais, string TipoOpe, string TipoFavorito)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;

            string IdPlan;

            int LimiteGrupos, Grupos = 0;

            string TipoFav;
            if (TipoFavorito != "ImportadorExp") TipoFav = TipoFavorito.Substring(0, 2).ToUpper();
            else TipoFav = "IE";

            IdPlan = ObtieneIdPlan(IdUsuario);

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select LimiteGrupos from [Plan] where IdPlan = " + IdPlan;

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            dtr.Read();
            LimiteGrupos = Convert.ToInt32(dtr["LimiteGrupos"]);
            dtr.Close();

            sql = "select count(*) as Grupos from Grupo where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais +
                  "' ";
            sql += "and TipoOpe = '" + TipoOpe + "' "; //and TipoFav = '" + TipoFav + "'";

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            dtr.Read();
            Grupos = Convert.ToInt32(dtr["Grupos"]);
            dtr.Close();

            cn.Close();

            return (Grupos < LimiteGrupos);
        }

        public static void ObtienePaisTipoOperacion(string IdUsuario, ref string CodPais, ref string TipoOpe)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;

            string CodPaisAux;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select CodPais from Suscripcion where IdUsuario = " + IdUsuario;

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            dtr.Read();

            CodPaisAux = dtr["CodPais"].ToString();

            CodPais = CodPaisAux.Substring(0, 2);
            TipoOpe = CodPaisAux.Substring(3, 1);

            dtr.Close();
            cn.Close();
        }

        public static bool ActualizaGrupo(string IdUsuario, string CodPais, string TipoOpe, string TipoFavorito,
            bool flagCreaGrupo, string NuevoGrupo, string IdGrupo, ArrayList IDsSeleccionados, ref string Mensaje,
            ref string Pagina)
        {
            string TipoFav;
            bool flagOk = true;

            if (TipoFavorito != "ImportadorExp") TipoFav = TipoFavorito.Substring(0, 2).ToUpper();
            else TipoFav = "IE";

            bool ValidaGrupos = Functions.ValidaGrupos(IdUsuario, CodPais, TipoOpe, TipoFavorito);

            if (flagCreaGrupo && !ValidaGrupos)
            {
                string IdPlan = ObtieneIdPlan(IdUsuario);
                int LimiteGrupos = ObtieneLimite(IdPlan, "LimiteGrupos");
                if (Mensaje != "") Mensaje += "<br>";
                Mensaje += "No se pudo crear el Grupo.";
                Mensaje += "<br>Nota: Se ha alcanzado el máximo de " + LimiteGrupos.ToString() + " Grupos";
                flagOk = false;
                Pagina = "Groups.aspx?TipoFavorito=" + TipoFavorito;
            }
            else
            {
                bool flagLimFavPorGrupo = false;

                if (flagCreaGrupo)
                    IdGrupo = CreaGrupo(NuevoGrupo, IdUsuario, CodPais, TipoOpe, TipoFav);

                int CantSelec, CantGrab, CantFav, LimiteFavPorGrupo;
                ArrayList IDsSeleccionadosFavUnicos = SeleccionFavUnicos(IDsSeleccionados);
                CantSelec = IDsSeleccionadosFavUnicos.Count;
                CantGrab = GrabaFavoritosAGrupo(IdUsuario, IDsSeleccionadosFavUnicos, IdGrupo, ref flagLimFavPorGrupo);
                CantFav = CantFavoritosGrupo(IdGrupo);
                string IdPlan = ObtieneIdPlan(IdUsuario);
                LimiteFavPorGrupo = ObtieneLimite(IdPlan, "LimiteFavPorGrupo");

                if (CantGrab > 0)
                {
                    if (CantGrab == 1)
                        Mensaje = "Se agregó 1 favorito satisfactoriamente al Grupo.";
                    else
                        Mensaje = "Se agregaron " + CantGrab.ToString() + " favoritos satisfactoriamente al Grupo.";
                }
                else
                {
                    Mensaje = "No se agregaron los favoritos seleccionados al Grupo.";
                }

                if (flagLimFavPorGrupo)
                {
                    Mensaje += "<br>Nota: Se ha alcanzado el máximo de " + LimiteFavPorGrupo.ToString() +
                               " Favoritos en el Grupo";
                    flagOk = false;
                }
                Pagina = "Groups.aspx?TipoFavorito=" + TipoFavorito;
            }
            return flagOk;
        }

        public static void ActualizaGrupo2(string IdUsuario, string CodPais, string TipoOpe, string TipoFavorito,
            bool flagCreaGrupo, string NuevoGrupo, string IdGrupo, ArrayList IDsSeleccionados, ref string Mensaje,
            ref string Pagina)
        {
            string TipoFav;

            if (TipoFavorito != "ImportadorExp") TipoFav = TipoFavorito.Substring(0, 2).ToUpper();
            else TipoFav = "IE";

            bool ValidaGrupos = Functions.ValidaGrupos(IdUsuario, CodPais, TipoOpe, TipoFavorito);

            if (flagCreaGrupo && !ValidaGrupos)
            {
                string IdPlan = ObtieneIdPlan(IdUsuario);
                int LimiteGrupos = ObtieneLimite(IdPlan, "LimiteGrupos");
                if (Mensaje != "") Mensaje += "<br>";
                Mensaje += "No se pudo crear el Grupo.";
                Mensaje += "<br>Nota: Se ha alcanzado el máximo de " + LimiteGrupos.ToString() + " Grupos";
                //flagOk = false;
                //flagContactenos = true;
                Pagina = "Groups.aspx?TipoFavorito=" + TipoFavorito;
            }
            else
            {
                bool flagLimFavPorGrupo = false;

                if (flagCreaGrupo)
                    IdGrupo = CreaGrupo(NuevoGrupo, IdUsuario, CodPais, TipoOpe, TipoFav);

                int CantSelec, CantGrab, CantFav, LimiteFavPorGrupo;
                ArrayList IDsSeleccionadosFavUnicos = SeleccionFavUnicos(IDsSeleccionados);
                CantSelec = IDsSeleccionadosFavUnicos.Count;
                CantGrab = GrabaFavoritosAGrupo(IdUsuario, IDsSeleccionadosFavUnicos, IdGrupo, ref flagLimFavPorGrupo);
                CantFav = CantFavoritosGrupo(IdGrupo);
                string IdPlan = ObtieneIdPlan(IdUsuario);
                LimiteFavPorGrupo = ObtieneLimite(IdPlan, "LimiteFavPorGrupo");

                /*
                if (CantGrab > 0)
                    if (CantGrab == 1)
                        Mensaje = "Se agregó 1 favorito satisfactoriamente al Grupo.";
                    else
                        Mensaje = "Se agregaron " + CantGrab.ToString() + " favoritos satisfactoriamente al Grupo.";
                else
                {
                    Mensaje = "No se pudo agregar los favoritos seleccionados al Grupo.";
                    flagOk = false;
                }
                */

                if (flagLimFavPorGrupo)
                {
                    if (Mensaje != "") Mensaje += "<br>";
                    Mensaje += "Nota: Se ha alcanzado el máximo de " + LimiteFavPorGrupo.ToString() +
                               " Favoritos en el Grupo";
                    //flagContactenos = true;
                }
                Pagina = "MisGrupos.aspx?TipoFavorito=" + TipoFavorito;
            }
        }


        public static string BuscaVariable(string Variable, string TipoIdVariable, string CodPais)
        {
            //Retorna el nombre del identificador de la variable
            //Parametros: 
            //string Variable: nombre del identificador de la tabla
            //string TipoIdVariable: 
            //string CodPais: SIGLA internacional de un pais

            string Tipo, IdVariable;
            string aux = "";

            Tipo = TipoIdVariable.Substring(0, 1);
            IdVariable = TipoIdVariable.Substring(1, TipoIdVariable.Length - 1);
            switch (Variable)
            {
                case "IdPartida":
                    if (Tipo == "F")
                        aux = "Product: " + BuscaPartida(IdVariable, CodPais);
                    else
                        aux = "Product: " + BuscaGrupo(IdVariable);
                    break;
                case "IdImportador":
                    if (Tipo == "F")
                        aux = "Importer: " + BuscaEmpresa(IdVariable, CodPais);
                    else
                        aux = "Importer: " + BuscaGrupo(IdVariable);
                    break;
                case "IdProveedor":
                    if (Tipo == "F")
                        aux = "Exporter: " + BuscaProveedor(IdVariable, CodPais);
                    else
                        aux = "Exporter: " + BuscaGrupo(IdVariable);
                    break;
                case "IdDistrito":
                    if (Tipo == "F")
                        aux = "District: " + BuscaDistrito(IdVariable, CodPais);
                    else
                        aux = "District: " + BuscaGrupo(IdVariable);
                    break;
                case "IdExportador":
                    if (Tipo == "F")
                        aux = "Exporter: " + BuscaEmpresa(IdVariable, CodPais);
                    else
                        aux = "Exporter: " + BuscaGrupo(IdVariable);
                    break;
                case "IdImportadorExp":
                    if (Tipo == "F")
                        aux = "Importer: " + BuscaImportadorExp(IdVariable, CodPais);
                    else
                        aux = "Importer: " + BuscaGrupo(IdVariable);
                    break;
                case "IdPaisOrigen":
                    aux = "Origin: " + BuscaPais(IdVariable, CodPais);
                    break;
                case "IdPaisDestino":
                    aux = "Destination:" + BuscaPais(IdVariable, CodPais);
                    break;
                case "IdViaTransp":
                    aux = "Via of Transport: " + BuscaVia(TipoIdVariable, CodPais);
                    break;
                case "IdAduana":
                    aux = "Custom: " + BuscaAduana(TipoIdVariable, CodPais);
                    break;
            }
            return aux;
        }

        public static int GrabaFavoritosAGrupo(string IdUsuario, ArrayList IDsSeleccionados, string IdGrupo,
            ref bool flagLimFavPorGrupo)
        {
            flagLimFavPorGrupo = false;
            int cont = 0;

            if (IDsSeleccionados != null && IDsSeleccionados.Count > 0)
                for (int i = 0; i < IDsSeleccionados.Count; i++)
                {
                    string IdFavorito = (IDsSeleccionados[i]).ToString();
                    if (!ExisteFavoritoEnGrupo(IdGrupo, IdFavorito))
                        if (ValidaFavPorGrupo(IdUsuario, IdGrupo))
                        {
                            AgregaFavoritoAGrupo(IdGrupo, IdFavorito);
                            cont += 1;
                        }
                        else
                            flagLimFavPorGrupo = true;
                }

            return cont;
        }

        public static int GrabaFavoritosAGrupo2(string IdUsuario, string Aplicacion, ArrayList IDsSeleccionados,
            string IdGrupo)
        {
            int cont = 0;
            string ID, IdFavorito;

            if (IDsSeleccionados != null && IDsSeleccionados.Count > 0)
                for (int i = 0; i < IDsSeleccionados.Count; i++)
                {
                    ID = (IDsSeleccionados[i]).ToString();
                    IdFavorito = ID.Substring(0, ID.IndexOf("-"));
                    if (!ExisteFavoritoEnGrupo(IdGrupo, IdFavorito) && ValidaFavPorGrupo(IdUsuario, IdGrupo))
                    {
                        AgregaFavoritoAGrupo(IdGrupo, IdFavorito);
                        cont += 1;
                    }
                }

            return cont;
        }

        public static void EliminaFavoritosDeGrupo(ArrayList IDsSeleccionados, string IdGrupo)
        {
            if (IDsSeleccionados != null && IDsSeleccionados.Count > 0)
                for (int i = 0; i < IDsSeleccionados.Count; i++)
                    EliminaFavoritoDeGrupo(IdGrupo, (IDsSeleccionados[i]).ToString());
        }

        public static void EliminaGrupos(ArrayList IDsSeleccionados)
        {
            if (IDsSeleccionados != null && IDsSeleccionados.Count > 0)
                for (int i = 0; i < IDsSeleccionados.Count; i++)
                    EliminaGrupo((IDsSeleccionados[i]).ToString());
        }

        public static ArrayList AcumulaSeleccion(ArrayList Acumulado, ArrayList Seleccion)
        {
            if (Seleccion == null) Seleccion = new ArrayList();
            if (Acumulado == null) Acumulado = new ArrayList();
            for (int i = 0; i < Seleccion.Count; i++)
                if (!Acumulado.Contains(Seleccion[i].ToString())) Acumulado.Add(Seleccion[i].ToString());
            return Acumulado;
        }

        public static ArrayList AcumulaSeleccion(ArrayList Acumulado, string Id)
        {
            if (Acumulado == null) Acumulado = new ArrayList();
            if (!Acumulado.Contains(Id)) Acumulado.Add(Id);
            return Acumulado;
        }

        public static ArrayList EliminaSeleccion(ArrayList Acumulado, string Id)
        {
            if (Acumulado.Contains(Id)) Acumulado.Remove(Id);
            if (Acumulado.Count == 0) Acumulado = null;
            return Acumulado;
        }

        public static ArrayList SeleccionFavUnicos(ArrayList Seleccion)
        {
            ArrayList aux = new ArrayList();
            string ID, IdFavorito;

            if (Seleccion != null && Seleccion.Count > 0)
                for (int i = 0; i < Seleccion.Count; i++)
                {
                    ID = (Seleccion[i]).ToString();

                    if (ID.IndexOf("-") > 0)
                        IdFavorito = ID.Substring(0, ID.IndexOf("-"));
                    else
                        IdFavorito = ID;

                    if (!aux.Contains(IdFavorito)) aux.Add(IdFavorito);
                }

            return aux;
        }

        public static string CreaGrupo(string Grupo, string IdUsuario, string CodPais, string TipoOpe, string TipoFav)
        {
            //Inserta un nuevo grupo
            //Parametros: 
            //string Grupo: nombre del grupo
            //string IdUsuario: identificador del usuario
            //string CodPais: identificador de la tabla pais
            //string TipoOpe: Importación / Exportación 
            //string TipoFav: 
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string IdGrupo;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "insert into Grupo(IdUsuario, CodPais, TipoOpe, TipoFav, Grupo, FecCreacion) ";
            sql += "values (" + IdUsuario + ", '" + CodPais + "','" + TipoOpe + "', '" + TipoFav + "', '" + Grupo +
                   "', getdate())";
            cmd = new SqlCommand(sql, cn);
            cmd.ExecuteScalar();

            sql = "select max(IdGrupo) as IdGrupo from Grupo";
            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            dtr.Read();
            IdGrupo = dtr["IdGrupo"].ToString();
            dtr.Close();

            cn.Close();

            return IdGrupo;
        }

        public static void ModificaGrupo(string IdGrupo, string Grupo)
        {
            //Modifica el nombre del grupo
            //Parametros: 
            //string IdGrupo: identificador de la tabla Grupo
            //string Grupo: nombre del grupo
            string sql;
            SqlConnection cn;
            SqlCommand cmd;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "update Grupo set Grupo = '" + Grupo + "' where IdGrupo = " + IdGrupo;
            cmd = new SqlCommand(sql, cn);
            cmd.ExecuteScalar();

            cn.Close();
        }

        public static bool ExisteFavoritoEnGrupo(string IdGrupo, string IdFavorito)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            int Cant;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select count(*) as Cant from FavoritoGrupo where IdGrupo = " + IdGrupo + " and IdFavorito = " +
                  IdFavorito + " ";
            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            dtr.Read();
            Cant = Convert.ToInt32(dtr["Cant"]);
            dtr.Close();

            cn.Close();

            return (Cant > 0);
        }

        public static void AgregaFavoritoAGrupo(string IdGrupo, string IdFavorito)
        {
            //Agrega un elemento al grupo
            //Parametros: 
            //string IdGrupo: identificador de la tabla Grupo
            //string IdFavorito: 

            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            int Cant;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select count(*) as Cant from FavoritoGrupo where IdGrupo = " + IdGrupo + " and IdFavorito = " +
                  IdFavorito + " ";
            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            dtr.Read();
            Cant = Convert.ToInt32(dtr["Cant"]);
            dtr.Close();
            if (Cant == 0)
            {
                sql = "insert into FavoritoGrupo(IdGrupo, IdFavorito, FecCreacion) ";
                sql += "values (" + IdGrupo + ", " + IdFavorito + ", getdate())";
                cmd = new SqlCommand(sql, cn);
                cmd.ExecuteScalar();
            }

            cn.Close();
        }

        public static void EliminaFavoritoDeGrupo(string IdGrupo, string IdFavorito)
        {
            //Elimina un elemento del grupo
            //Parametros: 
            //string IdGrupo: identificador de la tabla Grupo
            //string IdFavorito: 

            string sql;
            SqlConnection cn;
            SqlCommand cmd;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "delete from FavoritoGrupo where IdGrupo = " + IdGrupo + " and IdFavorito = " + IdFavorito;
            cmd = new SqlCommand(sql, cn);
            cmd.ExecuteScalar();

            cn.Close();
        }

        public static int CantFavoritosGrupo(string IdGrupo)
        {
            //Retrona el numero de registros del grupo
            //Parametros: 
            //string IdGrupo: identificador de la tabla Grupo
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            int Cant;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select count(*) as Cant from FavoritoGrupo where IdGrupo = " + IdGrupo;

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            dtr.Read();
            Cant = (int)dtr["Cant"];
            dtr.Close();

            cn.Close();

            return Cant;
        }

        public static DataView LlenaGruposFavoritos(string IdUsuario, string CodPais, string TipoOpe,
            string TipoFavorito)
        {
            String sql = "";
            SqlConnection cn;
            SqlCommand cmd;
            DataSet dtaset;
            SqlDataAdapter dtadap;

            DataView GruposFavoritos;

            string TipoFav;
            if (TipoFavorito != "ImportadorExp") TipoFav = TipoFavorito.Substring(0, 2);
            else TipoFav = "IE";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();
            switch (TipoFavorito)
            {
                case "Partida":
                    sql =
                        "select IdPartida as IdFavorito, 1 as Orden, 0 as IdGrupo, '[INDIVIDUAL]' as Grupo from PartidaFav ";
                    sql += "where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' and TipoOpe = '" +
                           TipoOpe + "'";
                    break;
                case "Importador":
                    sql =
                        "select IdEmpresa as IdFavorito, 1 as Orden, 0 as IdGrupo, '[INDIVIDUAL]' as Grupo from EmpresaFav ";
                    sql += "where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' and TipoOpe = 'I'";
                    break;
                case "Proveedor":
                    sql =
                        "select IdProveedor as IdFavorito, 1 as Orden, 0 as IdGrupo, '[INDIVIDUAL]' as Grupo from ProveedorFav ";
                    sql += "where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "'";
                    break;
                case "Exportador":
                    sql =
                        "select IdEmpresa as IdFavorito, 1 as Orden, 0 as IdGrupo, '[INDIVIDUAL]' as Grupo from EmpresaFav ";
                    sql += "where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' and TipoOpe = 'E'";
                    break;
                case "ImportadorExp":
                    sql =
                        "select IdImportadorExp as IdFavorito, 1 as Orden, 0 as IdGrupo, '[INDIVIDUAL]' as Grupo from ImportadorExpFav ";
                    sql += "where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "'";
                    break;
            }

            sql +=
                "union select IdFavorito, 2 as Orden, G.IdGrupo, '[G] ' + Grupo as Grupo from Grupo G, FavoritoGrupo F ";
            sql += "where G.IdGrupo = F.IdGrupo and IdUsuario = " + IdUsuario + " ";
            sql += "and CodPais = '" + CodPais + "' and TipoOpe = '" + TipoOpe + "' ";
            sql += "and TipoFav = '" + TipoFav + "' order by 1, 2, 4";

            cmd = new SqlCommand(sql, cn);
            dtadap = new SqlDataAdapter(cmd);
            dtaset = new DataSet();
            dtadap.Fill(dtaset);

            GruposFavoritos = new DataView();
            GruposFavoritos.Table = dtaset.Tables[0];

            cn.Close();

            return GruposFavoritos;
        }

        public static string ListaFavoritosGrupo(string TipoIdFavorito)
        {
            //Retorna una lista con todos los registros del TipoIdFavorito
            //Parametros: 
            //string TipoIdFavorito: 

            String sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string Tipo, IdFavorito;
            string lista;

            Tipo = TipoIdFavorito.Substring(0, 1);
            IdFavorito = TipoIdFavorito.Substring(1, TipoIdFavorito.Length - 1);

            lista = "(";
            if (Tipo == "F")
            {
                lista += IdFavorito;
            }
            else
            {
                cn = new SqlConnection();
                cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
                cn.Open();
                sql = "select IdFavorito from FavoritoGrupo where IdGrupo = " + IdFavorito;
                cmd = new SqlCommand(sql, cn);
                dtr = cmd.ExecuteReader();
                while (dtr.Read())
                {
                    lista += dtr["IdFavorito"].ToString() + ", ";
                }
                lista = lista.Substring(0, lista.Length - 2);
                cn.Close();
            }
            lista += ")";
            return lista;
        }

        public static int GrabaFavoritos(ArrayList IDsSeleccionados, string TipoFavorito, string IdUsuario,
            string CodPais, string TipoOpe, ref bool flagLimFavUnicos)
        {
            flagLimFavUnicos = false;
            int cont = 0;
            if (IDsSeleccionados != null && IDsSeleccionados.Count > 0)
                for (int i = 0; i < IDsSeleccionados.Count; i++)
                    if (!ExisteFavorito(TipoFavorito, IdUsuario, CodPais, TipoOpe, (IDsSeleccionados[i]).ToString()))
                        if (ValidaFavUnicos(IdUsuario, CodPais, TipoOpe, TipoFavorito))
                        {
                            AgregaFavorito(TipoFavorito, IdUsuario, CodPais, TipoOpe, (IDsSeleccionados[i]).ToString());
                            cont += 1;
                        }
                        else
                            flagLimFavUnicos = true;

            return cont;
        }

        public static void BuscaCodUsuario(string IdUsuario, ref string CodUsuario, ref string Password)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select CodUsuario, Password from Usuario where IdUsuario = " + IdUsuario;
            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read())
            {
                CodUsuario = dtr["CodUsuario"].ToString();
                Password = dtr["Password"].ToString();
            }
            dtr.Close();
            cn.Close();
        }

        public static void BuscaURLReferido(string Origen, string IdUsuario, ref string URL1, ref string URL2,
            ref string URL3)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select * from Referido where Origen = '" + Origen + "' and IdUsuario = " + IdUsuario;
            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read())
            {
                URL1 = dtr["URL1"].ToString().ToLower();
                URL2 = dtr["URL2"].ToString().ToLower();
                URL3 = dtr["URL3"].ToString().ToLower();
            }
            dtr.Close();
            cn.Close();
        }

        public static string Incoterm(string CodPais, string TipoOpe)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string aux = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select isnull(Incoterm, '') as Incoterm from BaseDatos ";
            sql += "where CodPais = '" + CodPais + "' and TipoOpe = '" + TipoOpe + "'";

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read())
                aux = dtr["Incoterm"].ToString();
            dtr.Close();

            cn.Close();

            return aux;
        }

        public static string CampoPeso(string CodPais, string TipoOpe)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string aux = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select isnull(CampoPeso, '') as CampoPeso from BaseDatos ";
            sql += "where CodPais = '" + CodPais + "' and TipoOpe = '" + TipoOpe + "'";

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read())
                aux = dtr["CampoPeso"].ToString();
            dtr.Close();

            cn.Close();

            return aux;
        }

        public static bool FlagDesComercial(string CodPais, string TipoOpe)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string aux = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            if (CodPais == "PEB") CodPais = "PE";

            sql = "select isnull(FlagDesComercial, '') as FlagDesComercial from BaseDatos where CodPais = '" + CodPais +
                  "' and TipoOpe = '" + TipoOpe + "'";

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read())
                aux = dtr["FlagDesComercial"].ToString().ToUpper();
            dtr.Close();

            cn.Close();

            return aux == "S";
        }

        public static bool FlagAduana(string CodPais)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            int aux = 0;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select count(*) as Cant from Aduana_" + CodPais;

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read())
                aux = Convert.ToInt32(dtr["Cant"]);
            dtr.Close();

            cn.Close();

            return (aux > 0);
        }

        public static bool FlagViaTransp(string CodPais)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            int aux = 0;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select count(*) as Cant from ViaTransp_" + CodPais;

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read())
                aux = Convert.ToInt32(dtr["Cant"]);
            dtr.Close();

            cn.Close();

            return (aux > 0);
        }

        public static bool FlagAgrupa2(string CodPais, string TipoOpe)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            int aux = 0;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select count(*) as Cant from VariableAgrupTest ";
            sql += "where CodPais = '" + CodPais + "' and TipoOpe = '" + TipoOpe + "' and AgrupaPor2 is not null";

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read())
                aux = Convert.ToInt32(dtr["Cant"]);
            dtr.Close();

            cn.Close();

            return (aux > 0);
        }

        public static bool FlagDescargaTodosCampos(string IdDescargaCab)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            int aux = 0;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select count(*) as Cant from DescargaCab where IdUsuario = 0 and IdDescargaCab = " + IdDescargaCab;

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read())
                aux = Convert.ToInt32(dtr["Cant"]);
            dtr.Close();

            cn.Close();

            return (aux > 0);
        }

        public static DataTable LlenaPaises(string Mensaje)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            DataSet dtaset;
            SqlDataAdapter dtadap;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            if (Mensaje != "")
            {
                sql = "select 0 as Orden, '' as CodPais, '" + Mensaje + "' as PaisES union ";
                sql += "select 1 as Orden, CodPais, PaisES from AdminPais2 where CodPais in ('PE', 'PEB') union ";
                sql +=
                    "select 2 as Orden, CodPais, PaisES from AdminPais2 where CodPais not in ('PE', 'PEB', 'CN', 'US', 'PEI', 'PEE', 'USI', 'PE_I', 'PE_E', 'EC_I', 'EC_E', 'XX') ";
            }
            else
            {
                sql = "select 1 as Orden, CodPais, PaisES from AdminPais2 where CodPais in ('PE') union ";
                sql +=
                    "select 2 as Orden, CodPais, PaisES from AdminPais2 where CodPais not in ('PE', 'PEB', 'CN', 'US', 'PEI', 'PEE', 'USI', 'PE_I', 'PE_E', 'EC_I', 'EC_E', 'XX') ";
            }
            sql += "order by Orden, CodPais";

            cmd = new SqlCommand(sql, cn);
            dtaset = new DataSet();
            dtadap = new SqlDataAdapter(cmd);
            dtadap.Fill(dtaset);

            cn.Close();

            return dtaset.Tables[0];
        }

        public static void GrabaLog(string IdUsuario, string CodPais, string TipoOpe, string AñoMesIni,
            string AñoMesFin, string Pagina, string Logs)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "insert into Logs(IdUsuario, CodPais, TipoOpe, AñoMesIni, AñoMesFin, Pagina, Logs, Fecha, IdAñoMes) ";
            sql += "values(" + IdUsuario + ", '" + CodPais + "', '" + TipoOpe + "', " + AñoMesIni + ", " + AñoMesFin +
                   ", '" + Pagina + "', '" + Logs.Replace("'", "''") +
                   "', getdate(), year(getdate()) * 100 + month(getdate()))";

            cmd = new SqlCommand(sql, cn);
            cmd.ExecuteScalar();

            cn.Close();
        }

        public static void GrabaLogUsuarioPruebaGratis(string codUsuario, string nombres, string apellidos, string accion, string estado,
            string mensajeError, string sqlEjecutado)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            //sql = "INSERT INTO Logs_Usuario_Prueba_Gratis(CodUsuario, Nombres, Apellidos, Accion, Estado, MensajeError, SqlEjecutado) ";
            //sql += $"VALUES('{codUsuario}', '{nombres}', '{apellidos}', '{accion}', '{estado}', '{mensajeError}', '{sqlEjecutado.Replace('\'', '"')}')";

            cmd = new SqlCommand(null, cn);

            cmd.CommandText = "INSERT INTO Logs_Usuario_Prueba_Gratis(CodUsuario, Nombres, Apellidos, Accion, Estado, MensajeError, SqlEjecutado) " +
                              "VALUES(@CodUsuario, @Nombres, @Apellidos, @Accion, @Estado, @MensajeError, @SqlEjecutado)";

            cmd.Parameters.AddWithValue("@CodUsuario", codUsuario);
            cmd.Parameters.AddWithValue("@Nombres", nombres);
            cmd.Parameters.AddWithValue("@Apellidos", apellidos);
            cmd.Parameters.AddWithValue("@Accion", accion);
            cmd.Parameters.AddWithValue("@Estado", estado);
            cmd.Parameters.AddWithValue("@MensajeError", mensajeError);
            cmd.Parameters.AddWithValue("@SqlEjecutado", sqlEjecutado);
            cmd.ExecuteNonQuery();
            //cmd.ExecuteScalar();
            cn.Close();
        }

        public static string ObtieneMaxIdLog(string IdUsuario, string Pagina)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;

            string IdLog = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select max(IdLog) as IdLog from Logs ";
            sql += "where IdUsuario = " + IdUsuario + " and Pagina = '" + Pagina + "'";

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            dtr.Read();

            IdLog = dtr["IdLog"].ToString();

            dtr.Close();
            cn.Close();

            return IdLog;
        }

        public static void ActualizaLog(string IdLog, string Logs)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "update Logs set Logs = '" + Logs + "' where IdLog = " + IdLog;

            cmd = new SqlCommand(sql, cn);
            cmd.ExecuteScalar();

            cn.Close();
        }

        public static bool ForzarZip(string CodUsuario)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            bool aux = true;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select ForzarZip from Usuario where CodUsuario = '" + CodUsuario + "' ";

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            dtr.Read();
            aux = (dtr["ForzarZip"].ToString() == "S");
            cn.Close();

            return aux;
        }

        public static string BuscaCodUsuario(string IdUsuario)
        {
            //Retorna el codigo de un usuario
            //Parametros: 
            //string IdUsuario: identificador de usuario
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string CodUsuario = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select CodUsuario from Usuario where IdUsuario = " + IdUsuario;
            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) CodUsuario = dtr["CodUsuario"].ToString();
            cn.Close();

            return CodUsuario;
        }

        public static string BuscaIdUsuario(string CodUsuario, string IdAplicacion)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string IdUsuario = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select IdUsuario from Usuario where CodUsuario = '" + CodUsuario + "' and IdAplicacion = " +
                  IdAplicacion;

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) IdUsuario = dtr["IdUsuario"].ToString();
            dtr.Close();

            cn.Close();

            return IdUsuario;
        }

        public static void EliminaGrupo(string IdGrupo)
        {
            //Elimina el grupo
            //Parametros: 
            //string IdGrupo: identificador de la tabla Grupo

            string sql;
            SqlConnection cn;
            SqlCommand cmd;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "delete from FavoritoGrupo where IdGrupo = " + IdGrupo;
            cmd = new SqlCommand(sql, cn);
            cmd.ExecuteScalar();
            sql = "delete from Grupo where IdGrupo = " + IdGrupo;
            cmd = new SqlCommand(sql, cn);
            cmd.ExecuteScalar();

            cn.Close();
        }

        public static int CantGrupos(string IdUsuario, string CodPais, string TipoOpe, string TipoFavorito)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;

            int Grupos = 0;

            string TipoFav;
            if (TipoFavorito != "ImportadorExp") TipoFav = TipoFavorito.Substring(0, 2).ToUpper();
            else TipoFav = "IE";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select count(*) as Grupos from Grupo ";
            sql += "where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' ";
            sql += "and TipoOpe = '" + TipoOpe + "' and TipoFav = '" + TipoFav + "' ";

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read())
                Grupos = Convert.ToInt32(dtr["Grupos"]);
            dtr.Close();

            cn.Close();

            return Grupos;
        }

        public static bool AgregaFavoritos(string IdUsuario, string CodPais, string TipoOpe, string TipoFavorito,
            ArrayList IDsSeleccionados, ref string Mensaje, ref string Pagina)
        {
            int CantSelec, CantGrab, LimiteFavUnicos;
            bool flagLimFavUnicos = false;
            bool flagOk = true;

            CantSelec = IDsSeleccionados.Count;
            CantGrab = Functions.GrabaFavoritos(IDsSeleccionados, TipoFavorito, IdUsuario, CodPais, TipoOpe,
                ref flagLimFavUnicos);
            string IdPlan = Functions.ObtieneIdPlan(IdUsuario);
            LimiteFavUnicos = Functions.ObtieneLimite(IdPlan, "LimiteFavUnicos");

            /*            
            
            //CantFav = Functions.CantFavUnicos(IdUsuario, CodPais, TipoOpe, AgrupaPor);
            
            //MANUEL: Se adiciona la funcionalidad que permite grabar en Peru y en Peru_B Importadores a la vez
            if (ObtienePlan(IdUsuario) != "Express" && CodPais.Equals("PE") && TipoOpe.Equals("I"))
                Functions.GrabaFavoritos(IDsSeleccionados, TipoFavorito, IdUsuario, "PEB", TipoOpe, ref flagLimFavUnicos2);
            else if (ObtienePlan(IdUsuario) != "Express" && CodPais.Equals("PEB") && TipoOpe.Equals("I"))
                Functions.GrabaFavoritos(IDsSeleccionados, TipoFavorito, IdUsuario, "PE", TipoOpe, ref flagLimFavUnicos2);

            if (CantGrab > 0)
            {
                if (CantGrab == 1)
                    Mensaje = "Se agregó 1 favorito satisfactoriamente. ";
                else
                    Mensaje = "Se agregaron " + CantGrab.ToString() + " favoritos satisfactoriamente. ";
            }
            else
            {
                Mensaje = "No se agregaron los favoritos seleccionados. ";
            }
            */

            if (CantSelec == 1)
                Mensaje = "Se agregó el favorito seleccionado.";
            else
                Mensaje = "Se agregaron los favoritos seleccionados.";

            if (flagLimFavUnicos)
            {
                Mensaje += "<br>Nota: Se ha alcanzado el límite de " + LimiteFavUnicos.ToString() + " Favoritos";
                flagOk = false;
            }

            if (TipoFavorito == "Partida") Pagina = "FavProducts.aspx";
            if (TipoFavorito == "Importador" || TipoFavorito == "Exportador") Pagina = "FavImportersExporters.aspx";
            if (TipoFavorito == "Proveedor") Pagina = "FavExporters.aspx";
            if (TipoFavorito == "ImportadorExp") Pagina = "FavImportersExp.aspx";

            return flagOk;
        }

        public static bool ExisteFavorito(string TipoFavorito, string IdUsuario, string CodPais, string TipoOpe,
            string IdFavorito)
        {
            //Retorna Verdadero si existen favoritos almacenados para los parametros
            //Parametros: 
            //string TipoFavorito: tabla
            //string IdUsuario: identificador del usuario
            //string CodPais: identificador de la tabla pais
            //string TipoOpe: Importación / Exportación 
            //string IdFavorito: Identificador de la tabla TipoFavorito
            string sql = "";
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            int cant;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            switch (TipoFavorito)
            {
                case "Partida":
                    sql = "select count(*) as cant from PartidaFav where IdUsuario = " + IdUsuario +
                          " and CodPais = '" + CodPais + "' ";
                    sql += "and TipoOpe = '" + TipoOpe + "' and IdPartida = " + IdFavorito + " ";
                    break;
                case "Importador":
                    sql = "select count(*) as cant from EmpresaFav where IdUsuario = " + IdUsuario +
                          " and CodPais = '" + CodPais + "' ";
                    sql += "and TipoOpe = 'I' and IdEmpresa = " + IdFavorito + " ";
                    break;
                case "Proveedor":
                    sql = "select count(*) as cant from ProveedorFav where IdUsuario = " + IdUsuario +
                          " and CodPais = '" + CodPais + "' ";
                    sql += "and IdProveedor = " + IdFavorito + " ";
                    break;
                case "Exportador":
                    sql = "select count(*) as cant from EmpresaFav where IdUsuario = " + IdUsuario +
                          " and CodPais = '" + CodPais + "' ";
                    sql += "and TipoOpe = 'E' and IdEmpresa = " + IdFavorito + " ";
                    break;
                case "ImportadorExp":
                    sql = "select count(*) as cant from ImportadorExpFav where IdUsuario = " + IdUsuario +
                          " and CodPais = '" + CodPais + "' ";
                    sql += "and IdImportadorExp = " + IdFavorito + " ";
                    break;
            }

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            dtr.Read();
            cant = Convert.ToInt32(dtr["cant"]);

            cn.Close();

            return (cant > 0);
        }

        public static void AgregaFavorito(string TipoFavorito, string IdUsuario, string CodPais, string TipoOpe,
            string IdFavorito)
        {
            //Inserta un nuevo favorito en TipoFavorito
            //Parametros: 
            //string TipoFavorito: tabla
            //string IdUsuario: identificador del usuario
            //string CodPais: identificador de la tabla pais
            //string TipoOpe: Importación / Exportación 
            //string IdFavorito: Identificador de la tabla TipoFavorito            
            string sql = "";
            SqlConnection cn;
            SqlCommand cmd;


            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            switch (TipoFavorito)
            {
                case "Partida":
                    sql = "insert into PartidaFav(IdUsuario, CodPais, TipoOpe, IdPartida, FecCreacion) ";
                    sql += "values (" + IdUsuario + ", '" + CodPais + "', '" + TipoOpe + "', " + IdFavorito +
                           ", getdate())";
                    /*
                    string PartidaFav;
                    PartidaFav = BuscaPartida(IdFavorito, CodPais);
                    if (PartidaFav.Length > 60) PartidaFav = PartidaFav.Substring(0, 60);
                    sql = "insert into PartidaFav(IdUsuario, CodPais, TipoOpe, IdPartida, PartidaFav, FecCreacion) ";
                    sql += "values (" + IdUsuario + ", '" + CodPais + "', '" + TipoOpe + "', " + IdFavorito + ", '" + PartidaFav.Replace("'", "''") + "', getdate())";
                    */
                    break;
                case "Importador":
                    sql = "insert into EmpresaFav(IdUsuario, CodPais, TipoOpe, IdEmpresa, FecCreacion) ";
                    sql += "values (" + IdUsuario + ", '" + CodPais + "', 'I'," + IdFavorito + ", getdate())";
                    break;
                case "Proveedor":
                    sql = "insert into ProveedorFav(IdUsuario, CodPais, TipoOpe, IdProveedor, FecCreacion) ";
                    sql += "values (" + IdUsuario + ", '" + CodPais + "', 'I', " + IdFavorito + ", getdate())";
                    break;
                case "Exportador":
                    sql = "insert into EmpresaFav(IdUsuario, CodPais, TipoOpe, IdEmpresa, FecCreacion) ";
                    sql += "values (" + IdUsuario + ", '" + CodPais + "', 'E', " + IdFavorito + ", getdate())";
                    break;
                case "ImportadorExp":
                    sql = "insert into ImportadorExpFav(IdUsuario, CodPais, TipoOpe, IdImportadorExp, FecCreacion) ";
                    sql += "values (" + IdUsuario + ", '" + CodPais + "', 'E', " + IdFavorito + ", getdate())";
                    break;
            }

            cmd = new SqlCommand(sql, cn);
            cmd.ExecuteScalar();

            cn.Close();
        }
    }
}