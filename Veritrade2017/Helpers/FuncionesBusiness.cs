using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Veritrade2017.Models;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Mvc;
using Dapper;
using System.Reflection;


namespace Veritrade2017.Helpers
{
    public class FuncionesBusiness
    {

        public static string SearchPassword(string codUsuario)
        {
            string password = "";
            var sql = $"select password from Usuario where codusuario = '{codUsuario}'";

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                password = row["password"].ToString();
            }

            return password;
        }
        public static bool ExisteCodUsuario(string CodUsuario)
        {
            string sql;
            sql = "select count(*) as Cant from Usuario where CodUsuario = '" + CodUsuario + "' ";

            using (var db = new ConexProvider().Open)
            {
                var result = db.QueryFirstOrDefault<int>(sql);
                return result > 0;
            }
        }

        public static string SearchIdTipoForCodUsusario(string codUsuario)
        {
            string idTipo = "";
            var sql = $"select idtipo from Usuario where codusuario = '{codUsuario}'";

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                idTipo = row["idtipo"].ToString();
            }

            return idTipo;
        }

        public static bool OcultarVideo(string idUsuario)
        {
            var flag = false;
            var sql = "select isnull(OcultarVideo, '') as OcultarVideo from Usuario where IdUsuario = " + idUsuario;

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                flag = (row["OcultarVideo"].ToString() == "S");
            }

            return flag;
        }        

        public static void ActualizaContTemp(string idUsuario, int max)
        {
            var sql = "update Usuario set ContTemp = U2.ContTemp ";
            sql += "from Usuario U, (select IdUsuario, isnull(ContTemp, 0) + 1 as ContTemp from Usuario) U2 ";
            sql += "where U.IdUsuario = U2.IdUsuario and U.IdUsuario = " + idUsuario + " and U.ContTemp < " + max.ToString();

            Conexion.SqlExecute(sql);
        }

        public static int ContTemp(string idUsuario)
        {
            var contTemp1 = 0;
            var sql = "select isnull(ContTemp, 0) as ContTemp from Usuario where IdUsuario = " + idUsuario;

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                contTemp1 = Convert.ToInt32(row["ContTemp"]);
            }

            return contTemp1;
        }
        public static TelefonoPais ObtenerTelefonoPaisPorCodPais(string CodPais, string culture)
        {
            string CodPaisPorDefecto = "US";

            if(culture == "es")
            {
                culture = "";
            }

            string idioma = "";

            switch(culture)
            {
                case "en-us":
                    idioma = "en";
                    break;
                default:
                    break;
            }

            string sql = $"Select tel.IdTelefono, adm.CodTelefono, tel.Telefono, tel.CodPais, tel.CodBandera, tel.Bandera, adm.Pais{idioma} ";
                   sql += "from TelefonoPais tel, AdminPaisN adm ";
                   sql += "Where tel.CodPais = adm.CodPais ";
                   sql += $"and tel.CodPais = '{CodPais}' ";
                   sql += $"or tel.CodPais IS NULL";

            var dt = Conexion.SqlDataTable(sql);

            if (dt.Rows.Count < 1 )
            {
                sql = $"Select adm.CodTelefono, tel.Telefono, tel.CodPais, tel.CodBandera, tel.Bandera, adm.Pais{idioma} ";
                sql += "from TelefonoPais tel, AdminPaisN adm ";
                sql += "Where tel.CodPais = adm.CodPais ";
                sql += $"and tel.CodPais = '{CodPaisPorDefecto}' ";

                dt = Conexion.SqlDataTable(sql);
            }

            DataRow dtTelefonoPais = dt.Rows[0];

            TelefonoPais telefonoPais = new TelefonoPais
            {
                TelefonoId = int.Parse(dtTelefonoPais[0].ToString()),
                CodTelefono = int.Parse(dtTelefonoPais[1].ToString()),
                Telefono = Int64.Parse(dtTelefonoPais[2].ToString()),
                CodPais = dtTelefonoPais[3].ToString(),
                CodBandera = dtTelefonoPais[4].ToString(),
                IconoBandera = dtTelefonoPais[5].ToString(),
                NombrePais = dtTelefonoPais[6].ToString()
            };

            return telefonoPais;
        }

        public static TelefonoPais ObtenerTelefonosPorId(int telefonoId, string culture)
        {
            if (culture == "es")
            {
                culture = "";
            }

            string idioma = "";

            switch (culture)
            {
                case "en-us":
                    idioma = "en";
                    break;
                case "en":
                    idioma = "en";
                    break;
                default:
                    break;
            }

            string sql = $"Select tel.IdTelefono, adm.CodTelefono, tel.Telefono, tel.CodPais, tel.CodBandera, tel.Bandera, adm.Pais{idioma} ";
            sql += "from TelefonoPais tel, AdminPaisN adm ";
            sql += "Where tel.CodPais = adm.CodPais ";
            sql += $"and tel.IdTelefono = '{telefonoId}' ";

            var dt = Conexion.SqlDataTable(sql);

            DataRow dtTelefonoPais = dt.Rows[0];
            if (dt.Rows.Count > 0)
            {
                TelefonoPais telefonoPais = new TelefonoPais
                {
                    TelefonoId = int.Parse(dtTelefonoPais[0].ToString()),
                    CodTelefono = int.Parse(dtTelefonoPais[1].ToString()),
                    Telefono = Int64.Parse(dtTelefonoPais[2].ToString()),
                    CodPais = dtTelefonoPais[3].ToString(),
                    CodBandera = dtTelefonoPais[4].ToString(),
                    IconoBandera = dtTelefonoPais[5].ToString(),
                    NombrePais = dtTelefonoPais[6].ToString()
                };
                return telefonoPais;
            }
            return null;
        }
        public static List<TelefonoPais> ObtenerTelefonosPaises(string culture)
        {
            if (culture == "es")
            {
                culture = "";
            }

            string idioma = "";

            switch (culture)
            {
                case "en-us":
                    idioma = "en";
                    break;
                case "en":
                    idioma = "en";
                    break;
                default:
                    break;
            }

            string sql = $"Select tel.IdTelefono, adm.CodTelefono, tel.Telefono, tel.CodPais, tel.CodBandera, tel.Bandera, adm.Pais{idioma} ";
            sql += "from TelefonoPais tel, AdminPaisN adm ";
            sql += "Where tel.CodPais = adm.CodPais ";

            var dt = Conexion.SqlDataTable(sql);

            List<TelefonoPais> telefonos = new List<TelefonoPais>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dtTelefonoPais = dt.Rows[i];
                TelefonoPais telefonoPais = new TelefonoPais
                {
                    TelefonoId = int.Parse(dtTelefonoPais[0].ToString()),
                    CodTelefono = int.Parse(dtTelefonoPais[1].ToString()),
                    Telefono = Int64.Parse(dtTelefonoPais[2].ToString()),
                    CodPais = dtTelefonoPais[3].ToString(),
                    CodBandera = dtTelefonoPais[4].ToString(),
                    IconoBandera = dtTelefonoPais[5].ToString(),
                    NombrePais = dtTelefonoPais[6].ToString()
                };
                telefonos.Add(telefonoPais);
            }

            return telefonos;
        }
        public static DataRow ObtienePlanesPrecios(string codPais)
        {
            var planPrecio = BuscaPlanPrecio(codPais);
            var sql = "select top 1 IdiomaDefecto from PlanPrecio where PlanPrecio = '" + planPrecio + "'";

            var dt = Conexion.SqlDataTable(sql);
            return dt.Rows[0];
        }

        static string BuscaPlanPrecio(string codPais)
        {
            if (codPais == ""){
                codPais = "- ";

			}
            var planPrecio = "USA";
            var sql = "select PlanPrecio from PlanPrecioPais where CodPais = '" + codPais + "'";

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                planPrecio = row["PlanPrecio"].ToString();
            }

            return planPrecio;
        }

        public static string BuscaPartida(string IdPartida, string CodPais, string Idioma = "es")
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string Partida = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select Nandina + ' ' + " + (Idioma == "es" ? "Partida" : "Partida_en") + " as Partida from Partida_" + CodPais + " where IdPartida = " + IdPartida;

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) Partida = dtr["Partida"].ToString();

            cn.Close();

            return Partida;
        }

        public static bool ForzarLinkExcel(string IdUsuario)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            bool flag = false;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select isnull(ForzarLinkExcel, '') as ForzarLinkExcel from Usuario where IdUsuario = " + IdUsuario;

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) flag = (dtr["ForzarLinkExcel"].ToString() == "S");

            cn.Close();

            return flag;
        }

        public static void ActualizaOcultarVideo(string IdUsuario)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "update Usuario set OcultarVideo = 'S' where IdUsuario = " + IdUsuario;

            cmd = new SqlCommand(sql, cn);
            cmd.ExecuteNonQuery();

            cn.Close();
        }
        public static ArrayList GuardaSeleccionados(GridView gdv, ArrayList IDsSeleccionados)
        {
            string ID;

            if (IDsSeleccionados == null) IDsSeleccionados = new ArrayList();

            foreach (GridViewRow row in gdv.Rows)
            {
                if (gdv.ID != "gdvAduanaDUAs")
                    ID = gdv.DataKeys[row.RowIndex].Value.ToString();
                else
                {
                    ID = gdv.DataKeys[row.RowIndex].Values[0].ToString() + '-' + gdv.DataKeys[row.RowIndex].Values[1].ToString();
                }
                bool FlagSeleccionado = ((CheckBox)row.FindControl("chkSel")).Checked;
                if (FlagSeleccionado)
                {
                    if (!IDsSeleccionados.Contains(ID))
                        IDsSeleccionados.Add(ID);
                }
                else
                {
                    if (IDsSeleccionados.Contains(ID))
                        IDsSeleccionados.Remove(ID);
                }
            }
            return IDsSeleccionados;
        }

        public static DataRow ObtieneUsuario(string IdUsuario)
        {
            string sql =
                "select CodUsuario, EmpPer, Nombres + ' ' + Apellidos as Nombres, Empresa, Telefono, Email1, Pais, Password ";
            sql += "from Usuario U, AdminPais P ";
            sql += "where U.CodPais = P.CodPais and IdUsuario = " + IdUsuario;
            var dt = Conexion.SqlDataTable(sql);
            return dt.Rows[0];
        }

        public static List<int> ObtenerSesionesActivas()
        {
            try
            {
                SqlConnection cn;
                SqlCommand cmd;

                cn = new SqlConnection
                {
                    ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString
                };
                cn.Open();
                cmd = new SqlCommand(null, cn)
                {
                    CommandText = "SELECT IdUsuario FROM SesionUsuario WHERE SesionActiva = 1"
                };

                SqlDataReader reader = cmd.ExecuteReader();

                List<int> IdsUsuario = new List<int>();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        IdsUsuario.Add((int)reader.GetValue(0));
                    }
                }
               
                reader.Close();
                cn.Close();
                return IdsUsuario;
            }
            catch(Exception ex)
            {
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                return null;
            }
        }

        public static int ObtenerSesionActivaPorUsuario(int idUsuario)
        {
            try
            {
                SqlConnection cn;
                SqlCommand cmd;

                cn = new SqlConnection
                {
                    ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString
                };
                cn.Open();
                cmd = new SqlCommand(null, cn)
                {
                    CommandText = "SELECT IdSesionUsuario FROM SesionUsuario WHERE IdUsuario = @IdUsuario"
                };
                cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                //equifax.CalificacionSBS = itemBody["CalificacionSBS"].InnerText != "" ? itemBody["CalificacionSBS"].InnerText : "SCAL";
                int idSesionUsuario = cmd.ExecuteScalar() != null ? (int)cmd.ExecuteScalar() : 0;
                cn.Close();

                return idSesionUsuario;
            }
            catch (Exception ex)
            {
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                return 0;
            }
        }

        public static bool InsertarSesionUsuario(int idUsuario, bool sesionActiva)
        {
            try
            {
                SqlConnection cn;
                SqlCommand cmd;

                cn = new SqlConnection
                {
                    ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString
                };
                cn.Open();
                cmd = new SqlCommand(null, cn)
                {
                    CommandText = "INSERT INTO SesionUsuario (IdUsuario, SesionActiva) VALUES (@IdUsuario, @SesionActiva)"
                };
                cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                cmd.Parameters.AddWithValue("@SesionActiva", sesionActiva);
                cmd.ExecuteNonQuery();
                cn.Close();
                return true;
            }
            catch (Exception ex)
            {
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                return false;
            }
        }

        public static bool ActualizarSesionUsuario(int idUsuario, bool sesionActiva)
        {
            try
            {
                SqlConnection cn;
                SqlCommand cmd;

                cn = new SqlConnection
                {
                    ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString
                };
                cn.Open();
                cmd = new SqlCommand(null, cn)
                {
                    CommandText = "UPDATE SesionUsuario SET SesionActiva = @SesionActiva WHERE IdUsuario = @IdUsuario"
                };
                cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                cmd.Parameters.AddWithValue("@SesionActiva", sesionActiva);
                cmd.ExecuteNonQuery();
                cn.Close();
                return true;
            }
            catch (Exception ex)
            {
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                return false;
            }
        }
        public static void AgregaFiltros(string Filtro, string CodPais, string TipoOpe, ArrayList IDsSeleccionados, ListBox lstSeleccionados, int LimiteFiltros = -1)
        {
            string ID, Tipo = "", Nombre = "";

            for (int i = 0; i < IDsSeleccionados.Count; i += 1)
            {
                ID = IDsSeleccionados[i].ToString();

                switch (Filtro)
                {
                    case "Partida": Tipo = "2PA"; Nombre = "[Partida] " + BuscaPartida(ID, CodPais); break;
                    case "Marca": Tipo = "2MA"; Nombre = "[Marca] " + BuscaMarca(ID, CodPais); break;
                    case "Modelo": Tipo = "2MO"; Nombre = "[Modelo] " + BuscaModelo(ID, CodPais); break;
                    case "Importador":
                        if (TipoOpe == "I")
                        { Tipo = "3IM"; Nombre = "[Importador] " + BuscaEmpresa(ID, CodPais); }
                        else
                        { Tipo = "3EX"; Nombre = "[Exportador] " + BuscaEmpresa(ID, CodPais); }
                        break;
                    case "Notificado": Tipo = "3NO"; Nombre = "[Notificado] " + BuscaNotificado(ID, CodPais); break;
                    case "Proveedor":
                        if (TipoOpe == "I")
                        { Tipo = "4PR"; Nombre = ((CodPais != "CL") ? "[Exportador] " : "[Marca] ") + BuscaProveedor(ID, CodPais); }
                        else
                        { Tipo = "4IE"; Nombre = "[Importador] " + BuscaImportadorExp(ID, CodPais); }
                        break;
                    case "PaisOrigen":
                        if (TipoOpe == "I" && CodPais != "USI" && CodPais != "PEI")
                        { Tipo = "5PO"; Nombre = "[País Origen] " + BuscaPais(ID, CodPais); }
                        else if (TipoOpe == "I" && (CodPais == "USI" || CodPais == "PEI"))
                        { Tipo = "5PE"; Nombre = "[Último País Embarque] " + BuscaPais(ID, CodPais); }
                        else
                        { Tipo = "5PD"; Nombre = "[País Destino] " + BuscaPais(ID, CodPais); }
                        break;
                    case "ViaTransp":
                        if (CodPais == "USI")
                        { Tipo = "6PD"; Nombre = "[Puerto Descarga] " + BuscaPuerto(ID, CodPais); }
                        else if (CodPais == "PEE")
                        { Tipo = "6DE"; Nombre = "[Puerto Destino] " + BuscaPuerto(ID, CodPais); }
                        else if (CodPais == "USE" || CodPais == "PEI")
                        { Tipo = "6PE"; Nombre = "[Último Puerto Embarque] " + BuscaPuerto(ID, CodPais); }
                        else
                        { Tipo = "6VT"; Nombre = "[Vía Transporte] " + Functions.BuscaVia(ID, CodPais); }
                        break;
                    case "AduanaDUA": Tipo = "7AD"; Nombre = "[Aduana DUA] " + BuscaAduana(ID.Split('-')[0], CodPais) + ' ' + ID.Split('-')[1]; break;
                    case "Aduana": Tipo = "7AA"; Nombre = "[Aduana] " + BuscaAduana(ID, CodPais); break;
                    case "Distrito": Tipo = "8DI"; Nombre = "[Distrito] " + BuscaDistrito(ID, CodPais); break;
                    case "Manifiesto": Tipo = "9MA"; Nombre = "[Manifiesto] " + ID; break;
                }

                lstSeleccionados.Items.Add(new ListItem(Nombre, Tipo + ID));
            }
        }

        public static string BuscaDistrito(string IdDistrito, string CodPais)
        {
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


        public static string BuscaPuerto(string IdPto, string CodPais)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string Puerto = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select Puerto from Puerto_" + CodPais + " where IdPto = " + IdPto;

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) Puerto = dtr["Puerto"].ToString();

            cn.Close();

            return Puerto;
        }

        public static string BuscaAduana(string IdAduana, string CodPais)
        {
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

        public static string BuscaPais(string IdPais, string CodPais)
        {
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
        public static string BuscaNotificado(string IdNotificado, string CodPais)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string Notificado = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select Notificado from Notificado_" + CodPais + " where IdNotificado = " + IdNotificado;

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) Notificado = dtr["Notificado"].ToString();

            cn.Close();

            return Notificado;
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

        public static void AgregaFiltros_en(string Filtro, string CodPais, string TipoOpe, ArrayList IDsSeleccionados, ListBox lstSeleccionados, int LimiteFiltros = -1)
        {
            string ID, Tipo = "", Nombre = "";

            for (int i = 0; i < IDsSeleccionados.Count; i += 1)
            {
                ID = IDsSeleccionados[i].ToString();

                switch (Filtro)
                {
                    case "Partida": Tipo = "2PA"; Nombre = "[HTS Code] " + BuscaPartida(ID, CodPais, "en"); break;
                    case "Marca": Tipo = "2MA"; Nombre = "[Brand] " + BuscaMarca(ID, CodPais); break;
                    case "Modelo": Tipo = "2MO"; Nombre = "[Model] " + BuscaModelo(ID, CodPais); break;
                    case "Importador":
                        if (TipoOpe == "I")
                        { Tipo = "3IM"; Nombre = "[Importer] " + BuscaEmpresa(ID, CodPais); }
                        else
                        { Tipo = "3EX"; Nombre = "[Exporter] " + BuscaEmpresa(ID, CodPais); }
                        break;
                    case "Notificado": Tipo = "3NO"; Nombre = "[Notified] " + BuscaNotificado(ID, CodPais); break;
                    case "Proveedor":
                        if (TipoOpe == "I")
                        { Tipo = "4PR"; Nombre = ((CodPais != "CL") ? "[Exporter] " : "[Brand] ") + BuscaProveedor(ID, CodPais); }
                        else
                        { Tipo = "4IE"; Nombre = "[Importer] " + BuscaImportadorExp(ID, CodPais); }
                        break;
                    case "PaisOrigen":
                        if (TipoOpe == "I" && CodPais != "USI" && CodPais != "PEI")
                        { Tipo = "5PO"; Nombre = "[Origin Country] " + BuscaPais(ID, CodPais); }
                        else if (TipoOpe == "I" && (CodPais == "USI" || CodPais == "PEI"))
                        { Tipo = "5PE"; Nombre = "[Last Shipment Country] " + BuscaPais(ID, CodPais); }
                        else
                        { Tipo = "5PD"; Nombre = "[Destination Country] " + BuscaPais(ID, CodPais); }
                        break;
                    case "ViaTransp":
                        if (CodPais == "USI")
                        { Tipo = "6PD"; Nombre = "[Unloading Port] " + BuscaPuerto(ID, CodPais); }
                        else if (CodPais == "PEE")
                        { Tipo = "6DE"; Nombre = "[Destination Port] " + BuscaPuerto(ID, CodPais); }
                        else if (CodPais == "USE" || CodPais == "PEI")
                        { Tipo = "6PE"; Nombre = "[Last Shipment Port] " + BuscaPuerto(ID, CodPais); }
                        else
                        { Tipo = "6VT"; Nombre = "[Via] " + Functions.BuscaVia(ID, CodPais); }
                        break;
                    case "AduanaDUA": Tipo = "7AD"; Nombre = "[Customs DUA] " + BuscaAduana(ID.Split('-')[0], CodPais) + ' ' + ID.Split('-')[1]; break;
                    case "Aduana": Tipo = "7AA"; Nombre = "[Customs] " + BuscaAduana(ID, CodPais); break;
                    case "Distrito": Tipo = "8DI"; Nombre = "[District] " + BuscaDistrito(ID, CodPais); break;
                    case "Manifiesto": Tipo = "9MA"; Nombre = "[Manifest] " + ID; break;
                }

                lstSeleccionados.Items.Add(new ListItem(Nombre, Tipo + ID));
            }
        }

        public static ArrayList GuardaSeleccionados(ArrayList IDsSeleccionadosOrigen, ArrayList IDsSeleccionadosDestino)
        {
            string ID;

            if (IDsSeleccionadosDestino == null) IDsSeleccionadosDestino = new ArrayList();

            for (int i = 0; i < IDsSeleccionadosOrigen.Count; i += 1)
            {
                ID = IDsSeleccionadosOrigen[i].ToString();
                if (!IDsSeleccionadosDestino.Contains(ID))
                    IDsSeleccionadosDestino.Add(ID);
            }

            return IDsSeleccionadosDestino;
        }

        public static void RecuperaSeleccionados(ref GridView grid, ArrayList IDsSeleccionados)
        {
            string ID;
            if (IDsSeleccionados != null && IDsSeleccionados.Count > 0)
            {
                foreach (GridViewRow row in grid.Rows)
                {
                    ID = grid.DataKeys[row.RowIndex].Value.ToString();
                    if (IDsSeleccionados.Contains(ID))
                    {
                        CheckBox chkSel = (CheckBox)row.FindControl("chkSel");
                        chkSel.Checked = true;
                    }
                }
            }
        }

        public static ArrayList GuardaSeleccionados(string CodPais, string TipoOpe, GridView gdv, ArrayList IDsSeleccionados, ListBox lstSeleccionados, int LimiteFiltros = -1, string Idioma = "es")
        {
            string ID, Tipo = "", Nombre = "";
            string lNandina = "Partida Aduanera", lImportador = "Importador", lExportador = "Exportador", lMarca = "Marca";

            if (Idioma == "en")
            {
                lNandina = "HTS Code"; lImportador = "Importer"; lExportador = "Exporter"; lMarca = "Brand";
            }

            if (IDsSeleccionados == null) IDsSeleccionados = new ArrayList();
            foreach (GridViewRow row in gdv.Rows)
            {
                if (LimiteFiltros > 0 && lstSeleccionados.Items.Count == LimiteFiltros) break;

                ID = gdv.DataKeys[row.RowIndex].Value.ToString();
                if (ID.Substring(0, 1) == "F") ID = ID.Substring(1, ID.Length - 1);
                bool FlagSeleccionado = ((CheckBox)row.FindControl("chkSel")).Checked;
                if (FlagSeleccionado)
                {
                    if (!IDsSeleccionados.Contains(ID))
                    {
                        IDsSeleccionados.Add(ID);
                        switch (gdv.ID)
                        {
                            case "gdvPartidas": Tipo = "2PA"; Nombre = "[" + lNandina + "] " + ((Label)row.FindControl("lblFavorito")).Text; break; //BuscaPartida(ID, CodPais); break;                            
                            case "gdvImportadores":
                                if (TipoOpe == "I")
                                { Tipo = "3IM"; Nombre = "[" + lImportador + "] " + BuscaEmpresa(ID, CodPais); }
                                else
                                { Tipo = "3EX"; Nombre = "[" + lExportador + "] " + BuscaEmpresa(ID, CodPais); }
                                break;
                            case "gdvProveedores":
                                if (TipoOpe == "I")
                                { Tipo = "4PR"; Nombre = ((CodPais != "CL") ? "[" + lExportador + "] " : "[" + lMarca + "] ") + BuscaProveedor(ID, CodPais); }
                                else
                                { Tipo = "4IE"; Nombre = "[" + lImportador + "] " + BuscaImportadorExp(ID, CodPais); }
                                break;
                        }
                        lstSeleccionados.Items.Add(new ListItem(Nombre, Tipo + ID));
                    }
                }
            }
            return IDsSeleccionados;
        }

        public static string BuscaImportadorExp(string IdImportadorExp, string CodPais)
        {
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

        public static string BuscaEmpresa(string IdEmpresa, string CodPais)
        {
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

        public static DataTable CargaPaises(string codPais, string codPais2, string idioma)
        {
            bool isUE = false;
            string codPaisAux = codPais;
            if (codPais == "PEB" || codPais == "PEP")
                codPaisAux = "PE";
            else if (codPais2 == "4UE")
            {
                codPaisAux = "UE";
                isUE = true;
            }

            string lTodos = "TODOS";
            string campo = "pais";
            //if (!isUE)
            //{

            //}

            campo = "pais_es";
            if (idioma == "en")
            {
                lTodos = "ALL";
                campo = "pais_en";
            }

            string sql = "select 1 as Orden, 0 as IdPais, '[" + lTodos + "]' as Pais union ";
            sql += "select 2 as Orden, IdPais, isnull(" + campo + ",Pais) as Pais from Pais_" + codPaisAux + " ";
            sql += "order by 1, 3";

            DataTable dt;
            try
            {
                dt = Conexion.SqlDataTable(sql);
            }
            catch (Exception)
            {
                dt = null;
            }

            return dt;
        }


        public static bool ValidaPlantillas(string IdUsuario, string CodPais, string TipoOpe, ref int LimitePlantillas, ref int Plantillas)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;

            string IdPlan;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            IdPlan = Functions3.ObtieneIdPlan(IdUsuario);

            sql = "select LimitePlantillas from [Plan] where IdPlan = " + IdPlan;

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            dtr.Read();
            LimitePlantillas = Convert.ToInt32(dtr["LimitePlantillas"]);
            dtr.Close();

            sql = "select count(*) as Plantillas from DescargaCab ";
            sql += "where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' and TipoOpe = '" + TipoOpe + "'";

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read())
                Plantillas = Convert.ToInt32(dtr["Plantillas"]);
            dtr.Close();

            cn.Close();

            return (Plantillas < LimitePlantillas);
        }

        public static string ObtieneUnidad2(string CodPais, string Unidad, string Idioma)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string Unidad2 = "";

            string lUnidad2 = "Descripcion";
            if (Idioma == "en") lUnidad2 = "isnull(Descripcion_en, Descripcion)";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select " + lUnidad2 + " as Unidad2 from Unidad where Unidad = '" + Unidad + "' ";

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) Unidad2 = dtr["Unidad2"].ToString();

            cn.Close();

            return Unidad2;
        }

        public static DataTable BuscaFavoritos(string IdUsuario, string CodPais, string TipoOpe, string TipoFavorito, string FavoritoB, bool flagSeleccione, string Idioma)
        {
            string sql = "";
            SqlConnection cn;
            SqlCommand cmd;
            DataSet dtaset;
            SqlDataAdapter dtadap;

            string CodPais2 = CodPais;
            if (CodPais.Substring(0, 2) == "UE") CodPais2 = "UE";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            string lSeleccione = "Seleccione";
            if (Idioma == "en") lSeleccione = "Select";

            if (flagSeleccione)
                sql += "select 0 as IdFavorito, '[ " + lSeleccione + "]' as Favorito union ";
            if (TipoFavorito == "Partida")
            {
                string Partida = "Partida";
                if (Idioma == "en") Partida = "Partida_en";

                sql += "select IdGrupo as IdFavorito, '[G] ' + Grupo as Favorito ";
                sql += "from Grupo ";
                sql += "where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' and TipoOpe = '" + TipoOpe + "' and TipoFav = 'PA' ";
                if (FavoritoB != "")
                    sql += "and Grupo like '%" + FavoritoB + "%' ";
                sql += "union ";
                sql += "select IdFavorito, Nandina + ' ' + case when PartidaFav is not null then PartidaFav when len(" + Partida + ") > 80 then substring(" + Partida + ", 1, 80) else " + Partida + " end as Favorito ";
                sql += "from V_FavUnicos F, Partida_" + CodPais2 + " P, PartidaFav PF ";
                sql += "where F.IdFavorito = P.IdPartida and F.IdUsuario = PF.IdUsuario and F.CodPais = PF.CodPais and F.TipoOpe = PF.TipoOpe and F.IdFavorito = PF.IdPartida ";
                sql += "and F.IdUsuario = " + IdUsuario + " and F.CodPais = '" + CodPais + "' and F.TipoOpe = '" + TipoOpe + "' and TipoFav = 'PA' ";
                if (FavoritoB != "")
                    sql += "and (Nandina like '" + FavoritoB + "%' or Partida like '%" + FavoritoB + "%' or PartidaFav like '%" + FavoritoB + "%') ";
                sql += "order by 2";
            }
            else if (TipoFavorito == "Importador" || TipoFavorito == "Exportador")
            {
                sql += "select IdGrupo as IdFavorito, '[G] ' + Grupo as Favorito  ";
                sql += "from Grupo ";
                sql += "where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' and TipoOpe = '" + TipoOpe + "' and TipoFav = '" + (TipoOpe == "I" ? "IM" : "EX") + "' ";
                if (FavoritoB != "")
                    sql += "and Grupo like '%" + FavoritoB + "%' ";
                sql += "union ";
                sql += "select IdFavorito, Empresa as Favorito ";
                sql += "from V_FavUnicos F, Empresa_" + CodPais + " E ";
                sql += "where F.IdFavorito = E.IdEmpresa ";
                sql += "and IdUsuario = " + IdUsuario + " and F.CodPais = '" + CodPais + "' and TipoOpe = '" + TipoOpe + "' and TipoFav = '" + (TipoOpe == "I" ? "IM" : "EX") + "' ";
                if (FavoritoB != "")
                    sql += "and Empresa like '%" + FavoritoB + "%' ";
                sql += "order by 2";
            }
            else if (TipoFavorito == "Proveedor")
            {
                sql += "select IdGrupo as IdFavorito, '[G] ' + Grupo as Favorito ";
                sql += "from Grupo ";
                sql += "where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' and TipoFav = 'PR' ";
                if (FavoritoB != "")
                    sql += "and Grupo like '%" + FavoritoB + "%' ";
                sql += "union ";
                sql += "select IdFavorito, Proveedor as Favorito ";
                sql += "from V_FavUnicos F, Proveedor_" + CodPais + " P ";
                sql += "where F.IdFavorito = P.IdProveedor ";
                sql += "and IdUsuario = " + IdUsuario + " and F.CodPais = '" + CodPais + "' and TipoFav = 'PR' ";
                if (FavoritoB != "")
                    sql += "and Proveedor like '%" + FavoritoB + "%' ";
                sql += "order by 2 ";
            }
            else if (TipoFavorito == "ImportadorExp")
            {
                sql += "select IdGrupo as IdFavorito, '[G] ' + Grupo as Favorito ";
                sql += "from Grupo ";
                sql += "where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' and TipoFav = 'IE' ";
                if (FavoritoB != "")
                    sql += "and Grupo like '%" + FavoritoB + "%' ";
                sql += "union ";
                sql = "select IdFavorito, ImportadorExp as Favorito ";
                sql += "from V_FavUnicos F, ImportadorExp_" + CodPais + " I ";
                sql += "where F.IdFavorito = I.IdImportadorExp ";
                sql += "and IdUsuario = " + IdUsuario + " and F.CodPais = '" + CodPais + "' and TipoFav = 'IE' ";
                if (FavoritoB != "")
                    sql += "and ImportadorExp like '%" + FavoritoB + "%' ";
                sql += "order by 2";
            }

            cmd = new SqlCommand(sql, cn);
            dtadap = new SqlDataAdapter(cmd);
            dtaset = new DataSet();
            dtadap.Fill(dtaset);

            cn.Close();

            return dtaset.Tables[0];
        }


        public static DataTable GeneraDt(string sql)
        {
            SqlConnection cn;
            SqlCommand cmd;
            DataSet dtaset;
            SqlDataAdapter dtadap;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            cmd = new SqlCommand(sql, cn);
            dtaset = new DataSet();
            dtadap = new SqlDataAdapter(cmd);
            dtadap.Fill(dtaset);

            cn.Close();

            return dtaset.Tables[0];
        }

        public static string Pais(string CodPais, string Idioma)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string PaisT = "";

            string lPais = "PaisES";
            if (Idioma == "en") lPais = "Pais";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select " + lPais + " from AdminPais2 where CodPais = '" + CodPais + "'";
            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();

            if (dtr.Read()) PaisT = dtr[lPais].ToString();

            cn.Close();

            return PaisT;
        }


        public static string BuscaDescargaDefault(string IdUsuario, string CodPais, string TipoOpe)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string IdDescargaCab = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select IdDescargaCab from DescargaCab where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' and TipoOpe = '" + TipoOpe + "' and FlagDefault = 'S' ";

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) IdDescargaCab = dtr["IdDescargaCab"].ToString();

            cn.Close();

            return IdDescargaCab;
        }


        public static string ListaTitulosDescarga(string IdDescargaCab, string Campos, string CodPais = "", string TipoOpe = "", string Idioma = "es")
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;

            string lista = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            if (IdDescargaCab != "0")
                sql = "select CampoFav from DescargaDet where IdDescargaCab = " + IdDescargaCab + " ";
            else
            {
                string CampoFav = "CampoFav";
                if (Idioma == "en") CampoFav = "CampoFav_en";
                sql = "select " + CampoFav + " as CampoFav from DescargaDet2 where CodPais = '" + CodPais + "' and TipoOpe = '" + TipoOpe + "' ";
            }
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

        public static string ListaCamposDescarga(string IdDescargaCab, string Campos, string CodPais = "", string TipoOpe = "", string Idioma = "es")
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;

            string campo, lista = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            if (IdDescargaCab != "0")
                sql = "select Campo from DescargaDet where IdDescargaCab = " + IdDescargaCab + " ";
            else
                sql = "select Campo from DescargaDet2 where CodPais = '" + CodPais + "' and TipoOpe = '" + TipoOpe + "' ";

            if (Campos != "") sql += "and Campo in " + Campos;

            sql += "order by NroCampo";

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            while (dtr.Read())
            {
                campo = dtr["Campo"].ToString();
                if (Idioma == "en" && campo.ToLower() == "partida") campo = "partida_en";
                lista += "[" + campo + "], ";
            }
            lista = lista.Substring(0, lista.Length - 2);

            cn.Close();

            return lista;
        }

        public static int CantFavUnicos(string IdUsuario, string CodPais, string TipoOpe, string TipoFavorito)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;

            int Cant = 0;

            string TipoFav;
            if (TipoFavorito != "ImportadorExp") TipoFav = TipoFavorito.Substring(0, 2).ToUpper(); else TipoFav = "IE";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select count(*) as Cant from V_FavUnicos ";
            sql += "where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' and TipoOpe = '" + TipoOpe + "' and TipoFav = '" + TipoFav + "' ";

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) Cant = Convert.ToInt32(dtr["Cant"]);
            dtr.Close();

            cn.Close();

            return Cant;
        }

        public static bool ExisteFavUnico(string IdUsuario, string CodPais, string TipoOpe, string TipoFavorito, string IdFavorito)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            bool flag = false;

            string TipoFav = TipoFavorito.Substring(0, 2);

            if (TipoFavorito == "ImportadorExp") TipoFav = "IE";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select count(*) as Cant from V_FavUnicos ";
            sql += "where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' and TipoOpe = '" + TipoOpe + "' and TipoFav = '" + TipoFav + "' and IdFavorito = " + IdFavorito;

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) flag = Convert.ToInt32(dtr["Cant"]) > 0;

            cn.Close();

            return flag;
        }

        public static string FiltrosEnExcel(ListBox lstFiltros)
        {
            string Filtros = "";

            foreach (ListItem item in lstFiltros.Items)
            {
                Filtros += item.Text + " | ";
            }
            Filtros = Filtros.Substring(0, Filtros.Length - 2);

            return Filtros;
        }

        public static void RegistraConsumo(string IdUsuario, string CodPais, string TipoOpe, string Accion, string SQL)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            if (SQL != "") SQL = "'" + SQL + "'"; else SQL = "null";

            sql = "insert into Consumo(IdUsuario, CodPais, TipoOpe, Accion, SQL, Fecha, IdAñoMes) ";
            sql += "values(" + IdUsuario + ", '" + CodPais + "', '" + TipoOpe + "', '" + Accion + "', null, getdate(), year(getdate()) * 100 + month(getdate()))";

            cmd = new SqlCommand(sql, cn);
            cmd.ExecuteScalar();

            cn.Close();
        }

        public static string BuscaProveedor(string IdProveedor, string CodPais)
        {
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
        public IEnumerable<SelectListItem> getOriginCountries(string codPais, string codPais2, string idioma)
        {
            var lista = new List<SelectListItem>();
            DataTable dt = CargaPaises(codPais, codPais2, idioma);
            if (dt != null)
                foreach (DataRow dr in dt.Rows)
                {
                    lista.Add(new SelectListItem { Text = dr["Pais"].ToString(), Value = dr["IdPais"].ToString() });
                }

            return lista;
        }
        public static bool AgregaFavoritos(string IdUsuario, string CodPais, string TipoOpe, string TipoFavorito, ArrayList IDsSeleccionados, string Idioma, ref string Mensaje)
        {
            int CantSelec, CantGrab, LimiteFavUnicos;
            bool flagLimFavUnicos = false;
            bool flagOk = true;

            CantSelec = IDsSeleccionados.Count;
            CantGrab = Functions.GrabaFavoritos(IDsSeleccionados, TipoFavorito, IdUsuario, CodPais, TipoOpe, ref flagLimFavUnicos);
            string IdPlan = Functions.ObtieneIdPlan(IdUsuario);
            LimiteFavUnicos = Functions.ObtieneLimite(IdPlan, "LimiteFavUnicos");

            if (CantSelec == 1)
            {
                Mensaje = "Se agregó el favorito seleccionado.";
                if (Idioma == "en") Mensaje = "Favorite added successfully";
            }
            else
            {
                Mensaje = "Se agregaron los favoritos seleccionados.";
                if (Idioma == "en") Mensaje = "Favorites added successfully";
            }

            if (flagLimFavUnicos)
            {
                Mensaje += "<br>Nota: Se ha alcanzado el límite de " + LimiteFavUnicos.ToString() + " Favoritos";
                if (Idioma == "en") Mensaje = "<br>Note: It reached favorites limit: " + LimiteFavUnicos.ToString();
                flagOk = false;
            }

            return flagOk;
        }

        public static bool ActualizaGrupo(string IdUsuario, string CodPais, string TipoOpe, string TipoFavorito, bool flagCreaGrupo, string NuevoGrupo, string IdGrupo, ArrayList IDsSeleccionados, string Idioma, ref string Mensaje)
        {
            string TipoFav;
            bool flagOk = true;

            if (TipoFavorito != "ImportadorExp") TipoFav = TipoFavorito.Substring(0, 2).ToUpper(); else TipoFav = "IE";

            bool ValidaGrupos = Functions.ValidaGrupos(IdUsuario, CodPais, TipoOpe, TipoFavorito);

            if (flagCreaGrupo && !ValidaGrupos)
            {
                string IdPlan = Functions.ObtieneIdPlan(IdUsuario);
                int LimiteGrupos = Functions.ObtieneLimite(IdPlan, "LimiteGrupos");
                if (Mensaje != "") Mensaje += "<br>";
                if (Idioma == "es")
                    Mensaje += "No se pudo crear el Grupo.<br>Nota: Se ha alcanzado el máximo de " + LimiteGrupos.ToString() + " Grupos";
                else
                    Mensaje += "Could not create group. <br>Note: It reached Groups number limit: " + LimiteGrupos.ToString();
                flagOk = false;
            }
            else
            {
                bool flagLimFavPorGrupo = false;

                if (flagCreaGrupo) IdGrupo = Functions.CreaGrupo(NuevoGrupo, IdUsuario, CodPais, TipoOpe, TipoFav);

                int CantFavIni, CantSelec, CantGrab, CantFav, LimiteFavPorGrupo;

                CantFavIni = Functions.CantFavoritosGrupo(IdGrupo);
                string IdPlan = Functions.ObtieneIdPlan(IdUsuario);
                LimiteFavPorGrupo = Functions.ObtieneLimite(IdPlan, "LimiteFavPorGrupo");

                if (CantFavIni == LimiteFavPorGrupo)
                {
                    if (Mensaje != "") Mensaje += "<br>";
                    if (Idioma == "es")
                        Mensaje += "No se pudo agregar los favoritos seleccionados al grupo.<br>Nota: Se ha alcanzado el máximo de " + LimiteFavPorGrupo.ToString() + " Favoritos en el Grupo";
                    else
                        Mensaje += "Could not add selected favorites to group.<br>Note: It reached Favorites number by group limit: " + LimiteFavPorGrupo.ToString();
                    flagOk = false;
                    return flagOk;
                }

                ArrayList IDsSeleccionadosFavUnicos = Functions.SeleccionFavUnicos(IDsSeleccionados);
                CantSelec = IDsSeleccionadosFavUnicos.Count;
                CantGrab = Functions.GrabaFavoritosAGrupo(IdUsuario, IDsSeleccionadosFavUnicos, IdGrupo, ref flagLimFavPorGrupo);
                CantFav = Functions.CantFavoritosGrupo(IdGrupo);

                if (Mensaje != "") Mensaje += "<br>";
                if (Idioma == "es")
                    Mensaje += "Los favoritos seleccionados se agregaron al grupo";
                else
                    Mensaje += "Selected favorites added to group successfully";

                /*
                if (CantGrab > 0)
                {
                    if (CantGrab == 1)
                    {
                        Mensaje = "Se agregó 1 favorito satisfactoriamente al Grupo.";
                    }
                    else
                    {
                        Mensaje = "Se agregaron " + CantGrab.ToString() + " favoritos satisfactoriamente al Grupo.";
                    }
                }
                else
                {
                    Mensaje = "No se agregaron los favoritos seleccionados al Grupo.";
                }
                */

                if (flagLimFavPorGrupo)
                {
                    if (Idioma == "es")
                        Mensaje += "<br>Nota: Se ha alcanzado el máximo de " + LimiteFavPorGrupo.ToString() + " Favoritos en el Grupo";
                    else
                        Mensaje += "<br>Note: It reached Favorites number by group limit: " + LimiteFavPorGrupo.ToString();
                    flagOk = false;
                }
            }
            return flagOk;
        }

        public static DataTable CargaPaisesUE(string Idioma)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            DataSet dtaset;
            SqlDataAdapter dtadap;

            string lPais = "Pais";
            if (Idioma == "en") lPais = "Pais_en";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select IdPais, " + lPais + " as Pais from PaisUEImpExp where Pais <> '092' order by Pais";

            cmd = new SqlCommand(sql, cn);
            dtaset = new DataSet();
            dtadap = new SqlDataAdapter(cmd);
            dtadap.Fill(dtaset);

            cn.Close();

            return dtaset.Tables[0];
        }

        public static string ObtieneCodPaisAcceso(string IdUsuario, ref string CodPais2, bool FlagConsideraCarga = false)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string CodPais = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select top 1 CodPais2, B.CodPais from BaseDatos B, Suscripcion S ";
            sql += "where B.CodPais = S.CodPais and IdUsuario = " + IdUsuario + " ";
            if (CodPais2 != "")
                sql += "and CodPais2 = '" + CodPais2 + "' ";
            if (FlagConsideraCarga)
                sql += "and FlagCarga = 'N' ";
            sql += "order by 1, 2";

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read())
            {
                CodPais2 = dtr["CodPais2"].ToString();
                CodPais = dtr["CodPais"].ToString();
            }
            dtr.Close();

            return CodPais;
        }

        public static string TipoOperacion(string TipoOpe, string Idioma)
        {
            string TipoOperacionT = "";
            if (Idioma == "es")
            {
                if (TipoOpe == "I") TipoOperacionT = "Importaciones";
                if (TipoOpe == "E") TipoOperacionT = "Exportaciones";
            }
            else
            {
                if (TipoOpe == "I") TipoOperacionT = "Imports";
                if (TipoOpe == "E") TipoOperacionT = "Exports";
            }
            return TipoOperacionT;
        }

        public static string InfoEnLinea(string CodPais, string TipoOpe, string Idioma)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string FechaIni = "", FechaFin = "";
            string Info = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select FechaIni, FechaFin from BaseDatos where CodPais ='" + CodPais + "' and TipoOpe = '" + TipoOpe + "' ";

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read())
            {
                FechaIni = dtr["FechaIni"].ToString();
                FechaIni = FechaIni.Substring(0, 4) + "-" + FechaIni.Substring(4, 2) + "-" + FechaIni.Substring(6, 2);
                FechaFin = dtr["FechaFin"].ToString();
                FechaFin = FechaFin.Substring(0, 4) + "-" + FechaFin.Substring(4, 2) + "-" + FechaFin.Substring(6, 2);

                if (Idioma == "es")
                {
                    System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("es-PE");
                    FechaIni = Convert.ToDateTime(FechaIni).ToString("dd-MMM-yyyy", culture).ToUpper();
                    FechaFin = Convert.ToDateTime(FechaFin).ToString("dd-MMM-yyyy", culture).ToUpper();
                    Info = "Información en línea desde " + FechaIni + " hasta " + FechaFin;
                }
                else
                {
                    System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US");
                    FechaIni = Convert.ToDateTime(FechaIni).ToString("MMM dd, yyyy", culture).ToUpper();
                    FechaFin = Convert.ToDateTime(FechaFin).ToString("MMM dd, yyyy", culture).ToUpper();
                    Info = "Online information from " + FechaIni + " to " + FechaFin;
                }
            }
            dtr.Close();
            cn.Close();

            return Info;
        }

        public static string ObtieneAñoMes(string IdAñoMes, string Idioma)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string AñoMes = "";

            string lAñoMes = "AñoMesES";
            if (Idioma == "en") lAñoMes = "AñoMes";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            if (IdAñoMes.Length == 8) IdAñoMes = IdAñoMes.Substring(0, 6);

            sql = "select " + lAñoMes + " from AñoMes where IdAñoMes = " + IdAñoMes;

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) AñoMes = dtr[lAñoMes].ToString();

            cn.Close();

            return AñoMes;
        }

        public static string BuscaNandina(string IdPartida, string CodPais)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string Nandina = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select Nandina from Partida_" + CodPais + " where IdPartida = " + IdPartida;

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) Nandina = dtr["Nandina"].ToString();

            cn.Close();

            return Nandina;
        }

        public static void ActualizaPartidaFavorita(string IdUsuario, string CodPais, string TipoOpe, string IdPartida, string PartidaFav)
        {
            //Validación Completa 2016-12-24
            string sql = "";
            SqlConnection cn;
            SqlCommand cmd;

            PartidaFav = PartidaFav.Trim().Replace("'", "");
            if (PartidaFav.Length > 80) PartidaFav = PartidaFav.Substring(0, 80);
            PartidaFav = CambiaNombre(PartidaFav);

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            SqlParameter param = new SqlParameter();
            param.ParameterName = "@PartidaFav";
            param.Value = PartidaFav;

            sql = "update PartidaFav set PartidaFav = @PartidaFav ";
            sql += "where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' and TipoOpe = '" + TipoOpe + "' and IdPartida = " + IdPartida;

            cmd = new SqlCommand(sql, cn);
            cmd.Parameters.Add(param);
            cmd.ExecuteNonQuery();

            cn.Close();
        }

        public static string BuscaPartida2(string IdPartida, string CodPais, string Idioma)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string Partida = "";

            string lPartida = "Partida";
            if (Idioma == "en") lPartida = "Partida_en";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select " + lPartida + " as Partida from Partida_" + CodPais + " where IdPartida = " + IdPartida;

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) Partida = dtr["Partida"].ToString();

            cn.Close();

            return Partida;
        }

        public static string CambiaNombre(string Nombre)
        {
            if (Nombre == "") return "";

            string Nombre2, Letra, Resto;

            Letra = Nombre.Substring(0, 1).ToUpper();
            if (Nombre.Length == 1) return Letra;

            Resto = Nombre.Substring(1, Nombre.Length - 1).ToLower();

            return Nombre2 = Letra + Resto;
        }

        public static string RangoFechas(string FechaIni, string FechaFin, string Idioma)
        {
            string FechaIniT, FechaFinT, Rango;

            System.Globalization.CultureInfo culture;
            if (Idioma == "es") culture = new System.Globalization.CultureInfo("es-PE"); else culture = new System.Globalization.CultureInfo("en-US");

            FechaIniT = FechaIni.Substring(0, 4) + "-" + FechaIni.Substring(4, 2) + "-01";
            FechaIniT = Convert.ToDateTime(FechaIniT).ToString("MMM-yyyy", culture).ToUpper();

            FechaFinT = FechaFin.Substring(0, 4) + "-" + FechaFin.Substring(4, 2) + "-01";
            FechaFinT = Convert.ToDateTime(FechaFinT).ToString("MMM-yyyy", culture).ToUpper();

            if (Idioma == "es")
                Rango = "DE " + FechaIniT + " A " + FechaFinT;
            else
                Rango = "FROM " + FechaIniT + " TO " + FechaFinT;

            return Rango;
        }

        public static string ObtienePartidaFav(string IdUsuario, string CodPais, string TipoOpe, string IdPartida)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string PartidaFav = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select Nandina, PartidaFav, Partida from PartidaFav PF, Partida_" + CodPais + " P ";
            sql += "where PF.IdPartida = P.IdPartida and IdUsuario = " + IdUsuario + " ";
            sql += "and CodPais = '" + CodPais + "' and TipoOpe = '" + TipoOpe + "' and PF.IdPartida = " + IdPartida;

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read())
            {
                if (dtr["PartidaFav"].ToString() != "")
                    PartidaFav = dtr["Nandina"].ToString() + " " + dtr["PartidaFav"].ToString();
                else
                    PartidaFav = dtr["Nandina"].ToString() + " " + dtr["Partida"].ToString();
            }
            cn.Close();

            return PartidaFav;
        }

        public static DataView LlenaGruposFavoritos(string IdUsuario, string CodPais, string TipoOpe, string TipoFavorito, string Idioma)
        {
            String sql = "";
            SqlConnection cn;
            SqlCommand cmd;
            DataSet dtaset;
            SqlDataAdapter dtadap;

            string lIndividual = "INDIVIDUAL";
            if (Idioma == "en") lIndividual = "SINGLE";

            DataView GruposFavoritos;

            string TipoFav;
            if (TipoFavorito != "ImportadorExp") TipoFav = TipoFavorito.Substring(0, 2); else TipoFav = "IE";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();
            switch (TipoFavorito)
            {
                case "Partida":
                    sql = "select IdPartida as IdFavorito, 1 as Orden, 0 as IdGrupo, '[" + lIndividual + "]' as Grupo from PartidaFav ";
                    sql += "where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' and TipoOpe = '" + TipoOpe + "'";
                    break;
                case "Importador":
                    sql = "select IdEmpresa as IdFavorito, 1 as Orden, 0 as IdGrupo, '[" + lIndividual + "]' as Grupo from EmpresaFav ";
                    sql += "where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' and TipoOpe = 'I'";
                    break;
                case "Proveedor":
                    sql = "select IdProveedor as IdFavorito, 1 as Orden, 0 as IdGrupo, '[" + lIndividual + "]' as Grupo from ProveedorFav ";
                    sql += "where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "'";
                    break;
                case "Exportador":
                    sql = "select IdEmpresa as IdFavorito, 1 as Orden, 0 as IdGrupo, '[" + lIndividual + "]' as Grupo from EmpresaFav ";
                    sql += "where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' and TipoOpe = 'E'";
                    break;
                case "ImportadorExp":
                    sql = "select IdImportadorExp as IdFavorito, 1 as Orden, 0 as IdGrupo, '[" + lIndividual + "]' as Grupo from ImportadorExpFav ";
                    sql += "where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "'";
                    break;
            }

            sql += "union select IdFavorito, 2 as Orden, G.IdGrupo, '[G] ' + Grupo as Grupo from Grupo G, FavoritoGrupo F ";
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
        public static string Unidades(string sqlFiltro, string tabla, ref int numRegistros)
        {
            string unidad = "";
            string sql = "select distinct Unidad ";
            sql += "from " + tabla + " T where 1 = 1 ";
            sql += sqlFiltro;

            try
            {
                DataTable dt = Conexion.SqlDataTable(sql);
                if (dt.Rows.Count > 0 && dt.Rows.Count == 1)
                {
                    unidad = dt.Rows[0]["Unidad"].ToString();
                }
                numRegistros = dt.Rows.Count;
            }
            catch (Exception e)
            {
                unidad = "";
            }

            return unidad;
        }
        public static DataRow CalculaTotales(string sqlFiltro, string cifTot, string codPais,
    string pesoNeto, string tabla, bool flagFormatoB = false, bool isManif = false, string filtro = "")
        {
            string cifTot1 = cifTot;

            if (codPais == "BR" || codPais == "IN")
                cifTot1 = "convert(decimal(19,2), " + cifTot + ")";

            var pesoNeto1 = pesoNeto;
            var pesoNeto2 = pesoNeto;

            if (pesoNeto1 == "")
            {
                pesoNeto1 = "0";
                pesoNeto2 = "PesoNeto";
            }

            var tabla1 = tabla;

            if (flagFormatoB || ((sqlFiltro.Contains("IdMarca") || sqlFiltro.Contains("IdModelo")) && codPais != "EC"))
            {
                cifTot1 = "FOBTot";
                pesoNeto1 = "0";
                pesoNeto2 = "PesoNeto";
                tabla1 = "Importacion_PEB";
            }

            var sql = string.Empty;
            if (!isManif)
            {
                sql = "select count(*) as CantReg, sum(" + cifTot1 + ") as " + cifTot + ", sum (isnull(" + pesoNeto1 +
                          ",0)) as " +
                          pesoNeto2 + ", sum(Cantidad) as Cantidad ";
                sql += "from " + tabla1 + " T where 1 = 1 ";
                sql += sqlFiltro;
            }
            else
            {
                sql = "select count(*) as CantReg, sum(" + cifTot + ") / 1000 as " + cifTot + " ";
                sql += "from " + tabla + " T where 1 = 1 ";
                sql += sqlFiltro;
            }



            DataRow dr;
            try
            {
                var dt = Conexion.SqlDataTable(sql);
                dr = dt.Rows[0];
            }
            catch (Exception e)
            {
                dr = null;
            }

            return dr;
        }
        public static DataView LlenaGrupos(bool flagFiltro, string IdUsuario, string CodPais, string TipoOpe, string TipoFavorito, string Idioma)
        {
            string sql = "";
            SqlConnection cn;
            SqlCommand cmd;
            DataSet dtaset;
            SqlDataAdapter dtadap;

            string lTodos = "TODOS";
            string lIndividual = "INDIVIDUAL";

            if (Idioma == "en")
            {
                lTodos = "ALL";
                lIndividual = "SINGLE";
            }

            DataView Grupos;

            string TipoFav;
            if (TipoFavorito != "ImportadorExp") TipoFav = TipoFavorito.Substring(0, 2); else TipoFav = "IE";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            if (flagFiltro)
            {
                sql = "select 0 as Orden, -1 as IdGrupo, '[" + lTodos + "]' as Grupo union select 1 as Orden, 0 as IdGrupo, '[" + lIndividual + "]' as Grupo ";
                sql += "union select 2 as Orden, IdGrupo, '[G] ' + Grupo as Grupo from Grupo ";
            }
            else
                sql = "select 2 as Orden, IdGrupo, Grupo from Grupo ";
            sql += "where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' and TipoOpe = '" + TipoOpe + "' ";
            sql += "and TipoFav = '" + TipoFav + "' ";
            sql += "and IdGrupo <> 335159"; // IdGrupoBloqueado
            sql += "order by 1, 3";

            cmd = new SqlCommand(sql, cn);
            dtadap = new SqlDataAdapter(cmd);
            dtaset = new DataSet();
            dtadap.Fill(dtaset);

            Grupos = new DataView();
            Grupos.Table = dtaset.Tables[0];

            cn.Close();

            return Grupos;
        }


        public static DataTable BuscarPartidaPorIdProducto(string codProducto)
        {
            var sql =  "SELECT IdProducto, CodProducto, DescripcionES, DescripcionEN, UriES, UriEN ";
                sql += "FROM producto WHERE ";
                sql +=$"CodProducto='{codProducto}'";

            DataTable dt;
            try
            {
                dt = Conexion.SqlDataTableProductProfile(sql);
            }
            catch (Exception e)
            {
                dt = null;
            }

            return dt;
        }

        public static DataTable SearchImportsData(int IdProducto, int IdPaisAduana, string tipoOpe)
        {
            var sql = " SELECT IdProducto, IdPaisAduana, Regimen,CantidadTotal=ISNULL(SUM(Cantidad),0)," +
                      " PrecioTotal = ISNULL(SUM(PrecioUnit), 0), ValorTotal = ISNULL(SUM(Valor), 0) FROM Totales " +
                      " WHERE IDPRODUCTO = " + IdProducto + " AND IDPAISADUANA = " + IdPaisAduana + " AND REGIMEN = '" + tipoOpe + "' GROUP BY IdProducto, IdPaisAduana, Regimen";

            DataTable dt;
            try
            {
                dt = Conexion.SqlDataTableProductProfile(sql);
            }
            catch (Exception e)
            {
                dt = null;
            }

            return dt;
        }
        public static DataTable SearchProductData(int IdProducto, string idioma, string codPartida)
        {
            var sql = "";

            if (idioma == "es")
            {
                sql = "SELECT IdProducto, Partida = CodProducto, DescripcionES as Descripcion FROM Producto WHERE IdProducto = " + IdProducto +
                      " AND CodProducto ='" + codPartida + "'";
            }
            else
            {
                sql = "SELECT IdProducto, Partida = CodProducto, DescripcionEN as Descripcion FROM Producto WHERE IdProducto = " + IdProducto +
                      " AND CodProducto ='" + codPartida + "'";
            }

            DataTable dt;
            try
            {
                dt = Conexion.SqlDataTableProductProfile(sql);
            }
            catch (Exception e)
            {
                dt = null;
            }

            return dt;
        }
        public static DataTable SearchProductFlag(int IdProducto)
        {
            var sql = "SELECT TOP 1 PA.IdPaisAduana, PA.PaisAduana FROM TOTALES T INNER JOIN PAISADUANA PA ON PA.IdPaisAduana = T.IdPaisAduana " +
                      "WHERE T.IDPRODUCTO = " + IdProducto + " ORDER BY T.VALOR DESC ";

            DataTable dt;
            try
            {
                dt = Conexion.SqlDataTableProductProfile(sql);
            }
            catch (Exception e)
            {
                dt = null;
            }

            return dt;
        }
        public static DataTable SearchConsolidateCountry(int IdProducto, int IdPaisAduana)
        {
            var sql = "SELECT PA.IdPaisAduana, " +
                      " PA.PaisAduana," +
                      " Importaciones = ISNULL((SELECT VALOR FROM TOTALES WHERE IDPRODUCTO = " + IdProducto + " AND REGIMEN = 'Importaciones' AND IDPAISADUANA = PA.IDPAISADUANA),0), " +
                      " Exportaciones = ISNULL((SELECT VALOR FROM TOTALES WHERE IDPRODUCTO = " + IdProducto + " AND REGIMEN = 'Exportaciones' AND IDPAISADUANA = PA.IDPAISADUANA),0)," +
                      " Importadores = ISNULL((SELECT CantidadEmpresas FROM TOTALES WHERE IDPRODUCTO = " + IdProducto + " AND REGIMEN = 'Importaciones' AND IDPAISADUANA = PA.IDPAISADUANA),0)," +
                      " Exportadores = ISNULL((SELECT CantidadEmpresas FROM TOTALES WHERE IDPRODUCTO = " + IdProducto + " AND REGIMEN = 'Exportaciones' AND IDPAISADUANA = PA.IDPAISADUANA),0)," +
                      " RegistrosI = ISNULL((SELECT Registros FROM TOTALES WHERE IDPRODUCTO = " + IdProducto + " AND REGIMEN = 'Importaciones' AND IDPAISADUANA = PA.IDPAISADUANA),0)," +
                      " RegistrosE = ISNULL((SELECT Registros FROM TOTALES WHERE IDPRODUCTO = " + IdProducto + " AND REGIMEN = 'Exportaciones' AND IDPAISADUANA = PA.IDPAISADUANA),0)" +
                      " FROM TOTALES T INNER JOIN PAISADUANA PA ON PA.IdPaisAduana = T.IdPaisAduana GROUP BY PA.IDPAISADUANA, T.IDPRODUCTO , PA.PAISADUANA HAVING T.IDPRODUCTO = " + IdProducto +
                      " AND PA.IDPAISADUANA = " + IdPaisAduana;

            DataTable dt;
            try
            {
                dt = Conexion.SqlDataTableProductProfile(sql);
            }
            catch (Exception e)
            {
                dt = null;
            }

            return dt;
        }
        
        // Ruben 202310
        public static DataTable SearchCif(int IdProducto, int IdPaisAduana, string tipoOpe, int año)
        {
            // Ruben 202310
            var sql = "SELECT IDPRODUCTO, IDPAISADUANA, REGIMEN, AÑO, SUM(VALOR) as VALOR " +
                      "FROM TOTALESAÑO_MES WHERE IDPRODUCTO = " + IdProducto +
                      " AND IDPAISADUANA = " + IdPaisAduana +
                      " AND REGIMEN = '" + tipoOpe + "' AND AÑO = " + año +  " GROUP BY IDPRODUCTO, IDPAISADUANA, REGIMEN, AÑO";

            DataTable dt;
            try
            {
                dt = Conexion.SqlDataTableProductProfile(sql);
            }
            catch (Exception e)
            {
                dt = null;
            }

            // Ruben 202310
            for (int i = año + 1; i < año + 5; i++)
            {
                var sql2 = "SELECT IDPRODUCTO, IDPAISADUANA, REGIMEN, AÑO, SUM(VALOR) as VALOR " +
                          "FROM TOTALESAÑO_MES WHERE IDPRODUCTO = " + IdProducto +
                          " AND IDPAISADUANA = " + IdPaisAduana +
                          " AND REGIMEN = '" + tipoOpe + "' AND AÑO = " + i + " GROUP BY IDPRODUCTO, IDPAISADUANA, REGIMEN, AÑO";
                DataTable dt2;
                try
                {
                    dt2 = Conexion.SqlDataTableProductProfile(sql2);
                }
                catch (Exception e)
                {
                    dt2 = null;
                }

                if (dt2.Rows.Count > 0)
                {
                    DataRow newRow = dt.NewRow();
                    newRow["IDPRODUCTO"] = IdProducto;
                    newRow["IDPAISADUANA"] = IdPaisAduana;
                    newRow["REGIMEN"] = tipoOpe;
                    newRow["AÑO"] = i;
                    newRow["VALOR"] = dt2.Rows[0]["VALOR"];
                    dt.Rows.Add(newRow);
                }
                else
                {
                    DataRow newRow = dt.NewRow();
                    newRow["IDPRODUCTO"] = IdProducto;
                    newRow["IDPAISADUANA"] = IdPaisAduana;
                    newRow["REGIMEN"] = tipoOpe;
                    newRow["AÑO"] = i;
                    newRow["VALOR"] = 0.00;
                    dt.Rows.Add(newRow);
                }
            }

            return dt;
        }
        public static DataTable SearchPrecioProm(int IdProducto, int IdPaisAduana, string tipoOpe)
        {
            string[] mes =
            {
                "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre",
                "Noviembre", "Diciemnbre"
            };
            int añoActual = DateTime.Today.Year;
            var sql = "SELECT Año, Mes = CASE Mes WHEN 1 THEN 'Enero' WHEN 2 THEN 'Febrero' WHEN 3 THEN 'Marzo' WHEN 4 THEN 'Abril' " +
                      "WHEN 5 THEN 'Mayo' WHEN 6 THEN 'Junio' WHEN 7 THEN 'Julio' WHEN 8 THEN 'Agosto' WHEN 9 THEN 'Septiembre' " +
                      "WHEN 10 THEN 'Octubre' WHEN 11 THEN 'Noviembre' WHEN 12 THEN 'Diciembre' END, ISNULL(PrecioUnit,0) AS PrecioUnit " +
                      "FROM TOTALESAÑO_MES WHERE REGIMEN = '" + tipoOpe + "' AND" +
                      " IDPRODUCTO = " + IdProducto +
                      " AND IDPAISADUANA = " + IdPaisAduana +
                      " AND AÑO = 2014 AND MES = 0 AND PrecioUnit IS NOT NULL";

            DataTable dt;
            try
            {
                dt = Conexion.SqlDataTableProductProfile(sql);
            }
            catch (Exception e)
            {
                dt = null;
            }

            int countZeros = 0;

            var sql2 = "";
            for (int i = añoActual-4; i <= añoActual; i++)
            {
                sql2 = "SELECT Año, Mes = CASE Mes WHEN 1 THEN 'Enero' WHEN 2 THEN 'Febrero' WHEN 3 THEN 'Marzo' WHEN 4 THEN 'Abril' " +
                           "WHEN 5 THEN 'Mayo' WHEN 6 THEN 'Junio' WHEN 7 THEN 'Julio' WHEN 8 THEN 'Agosto' WHEN 9 THEN 'Septiembre' " +
                           "WHEN 10 THEN 'Octubre' WHEN 11 THEN 'Noviembre' WHEN 12 THEN 'Diciembre' END, ISNULL(PrecioUnit,0) AS PrecioUnit " +
                           "FROM TOTALESAÑO_MES WHERE REGIMEN = '" + tipoOpe + "' AND" +
                           " IDPRODUCTO = " + IdProducto +
                           " AND IDPAISADUANA = " + IdPaisAduana +
                           " AND AÑO = " + i;

                for (int j = 1; j <= 12; j++)
                {
                    var sql3 = sql2 + " AND MES = " + j + " AND PrecioUnit IS NOT NULL";

                    DataTable dt2;
                    try
                    {
                        dt2 = Conexion.SqlDataTableProductProfile(sql3);
                    }
                    catch (Exception e)
                    {
                        dt2 = null;
                    }
                    if (dt2.Rows.Count > 0 && Convert.ToDecimal(dt2.Rows[0]["PrecioUnit"]) != 0)
                    {
                        DataRow newRow = dt.NewRow();
                        newRow["Año"] = dt2.Rows[0]["Año"];
                        newRow["Mes"] = dt2.Rows[0]["Mes"];
                        newRow["PrecioUnit"] = dt2.Rows[0]["PrecioUnit"];
                        dt.Rows.Add(newRow);
                    }
                    else
                    {
                        DataRow newRow = dt.NewRow();
                        newRow["Año"] = i;
                        newRow["Mes"] = mes[j - 1];
                        newRow["PrecioUnit"] = 0.00;
                        dt.Rows.Add(newRow);
                        countZeros++;
                    }
                    sql3 = "";
                }
            }

            if (countZeros == 60)
            {
                DataTable dt3 = new DataTable();
                dt3.Columns.Add("Año", typeof(int));
                dt3.Columns.Add("Mes", typeof(string));
                dt3.Columns.Add("PrecioUnit", typeof(decimal));
                return dt3;
            }
            else
            {
                return dt;
            }
        }
        
        // Ruben 202404
        public static DataTable SearchConsolidateCountries(int IdProducto, string tipoOpe)
        {
            var sql = "";
            if (tipoOpe == "Importaciones")
            {
                sql = "SELECT PA.IdPaisAduana, " +
                      " PA.PaisAduana, PA.PaisAduanaEN, " + // Ruben 202404
                      " PA.Abreviatura, " +
                      " Importaciones = ISNULL((SELECT VALOR FROM TOTALES WHERE IDPRODUCTO = " + IdProducto + " AND REGIMEN = 'Importaciones' AND IDPAISADUANA = PA.IDPAISADUANA),0), " +
                      " Importadores = ISNULL((SELECT CantidadEmpresas FROM TOTALES WHERE IDPRODUCTO = " + IdProducto + " AND REGIMEN = 'Importaciones' AND IDPAISADUANA = PA.IDPAISADUANA),0)" +
                      " FROM TOTALES T INNER JOIN PAISADUANA PA ON PA.IdPaisAduana = T.IdPaisAduana GROUP BY PA.IDPAISADUANA, T.IDPRODUCTO, PA.PAISADUANA, PA.PaisAduanaEN, PA.Abreviatura HAVING T.IDPRODUCTO = " + IdProducto + // Ruben 202404
                      " ORDER BY Importaciones DESC";
            }
            else
            {
                sql = "SELECT PA.IdPaisAduana, " +
                      " PA.PaisAduana, PA.PaisAduanaEN, " + // Ruben 202404
                      " PA.Abreviatura, " +
                      " Exportaciones = ISNULL((SELECT VALOR FROM TOTALES WHERE IDPRODUCTO = " + IdProducto + " AND REGIMEN = 'Exportaciones' AND IDPAISADUANA = PA.IDPAISADUANA),0)," +
                      "Exportadores = ISNULL((SELECT CantidadEmpresas FROM TOTALES WHERE IDPRODUCTO = " + IdProducto + " AND REGIMEN = 'Exportaciones' AND IDPAISADUANA = PA.IDPAISADUANA),0)" +
                      " FROM TOTALES T INNER JOIN PAISADUANA PA ON PA.IdPaisAduana = T.IdPaisAduana GROUP BY PA.IDPAISADUANA, T.IDPRODUCTO , PA.PAISADUANA, PA.PaisAduanaEN, PA.Abreviatura HAVING T.IDPRODUCTO = " + IdProducto + // Ruben 202404
                      " ORDER BY Exportaciones DESC";
            }

            DataTable dt;
            try
            {
                dt = Conexion.SqlDataTableProductProfile(sql);
            }
            catch (Exception e)
            {
                dt = null;
            }

            return dt;
        }
        public static DataTable SearchCompCIF(int IdProducto, int IdPaisAduana, string tipoOpe)
        {
            string[] mes =
            {
                "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct",
                "Nov", "Dic"
            };
            int añoActual = DateTime.Today.Year;
            var sql = " SELECT Año, Mes = CASE Mes WHEN 1 THEN 'Ene' WHEN 2 THEN 'Feb' WHEN 3 THEN 'Mar' WHEN 4 " +
                      "THEN 'Abr' WHEN 5 THEN 'May' WHEN 6 THEN 'Jun' WHEN 7 THEN 'Jul' WHEN 8 THEN 'Ago' " +
                      "WHEN 9 THEN 'Sep' WHEN 10 THEN 'Oct' WHEN 11 THEN 'Nov' WHEN 12 THEN 'Dic' END," +
                      " Valor FROM TOTALESAÑO_MES WHERE IDPRODUCTO = " + IdProducto + " AND IDPAISADUANA = " + IdPaisAduana +
                      " AND REGIMEN = '" + tipoOpe + "' AND AÑO = 2018 AND MES = 0";

            DataTable dt;
            try
            {
                dt = Conexion.SqlDataTableProductProfile(sql);
            }
            catch (Exception e)
            {
                dt = null;
            }

            var sql2 = "";
            for (int i = añoActual-4; i <= añoActual; i++)
            {
                sql2 = " SELECT Año, Mes = CASE Mes WHEN 1 THEN 'Ene' WHEN 2 THEN 'Feb' WHEN 3 THEN 'Mar' WHEN 4 " +
                           "THEN 'Abr' WHEN 5 THEN 'May' WHEN 6 THEN 'Jun' WHEN 7 THEN 'Jul' WHEN 8 THEN 'Ago' " +
                           "WHEN 9 THEN 'Sep' WHEN 10 THEN 'Oct' WHEN 11 THEN 'Nov' WHEN 12 THEN 'Dic' END," +
                           " Valor FROM TOTALESAÑO_MES WHERE IDPRODUCTO = " + IdProducto + " AND IDPAISADUANA = " +
                           IdPaisAduana +
                           " AND REGIMEN = '" + tipoOpe + "' AND AÑO = " + i;

                for (int j = 1; j <= 12; j++)
                {
                    var sql3 = sql2 + " AND MES = " + j;

                    DataTable dt2;
                    try
                    {
                        dt2 = Conexion.SqlDataTableProductProfile(sql3);
                    }
                    catch (Exception e)
                    {
                        dt2 = null;
                    }

                    if (dt2.Rows.Count > 0)
                    {
                        DataRow newRow = dt.NewRow();
                        newRow["Año"] = dt2.Rows[0]["Año"];
                        newRow["Mes"] = dt2.Rows[0]["Mes"];
                        newRow["Valor"] = dt2.Rows[0]["Valor"];
                        dt.Rows.Add(newRow);
                    }
                    else
                    {
                        DataRow newRow = dt.NewRow();
                        newRow["Año"] = i;
                        newRow["Mes"] = mes[j - 1];
                        newRow["Valor"] = 0.00;
                        dt.Rows.Add(newRow);
                    }

                    sql3 = "";
                }
            }
            return dt;
        }
        public static DataTable SearchTableCompCIF(int IdProducto, int IdPaisAduana, string tipoOpe)
        {
            Decimal sumTotal;
            int registros;
            int añoActual = DateTime.Today.Year;

            DataTable dt = new DataTable();
            dt.Columns.Add("Año", typeof(int));
            dt.Columns.Add("Valor", typeof(Decimal));
            dt.Columns.Add("Registros", typeof(int));

            var sql2 = "";
            for (int i = añoActual-4; i <= añoActual; i++)
            {
                sumTotal = 0;
                registros = 0;
                sql2 = " SELECT Año, Mes = CASE Mes WHEN 1 THEN 'Ene' WHEN 2 THEN 'Feb' WHEN 3 THEN 'Mar' WHEN 4 " +
                           "THEN 'Abr' WHEN 5 THEN 'May' WHEN 6 THEN 'Jun' WHEN 7 THEN 'Jul' WHEN 8 THEN 'Ago' " +
                           "WHEN 9 THEN 'Sep' WHEN 10 THEN 'Oct' WHEN 11 THEN 'Nov' WHEN 12 THEN 'Dic' END," +
                           " Valor, Registros FROM TOTALESAÑO_MES WHERE IDPRODUCTO = " + IdProducto + " AND IDPAISADUANA = " +
                           IdPaisAduana +
                           " AND REGIMEN = '" + tipoOpe + "' AND AÑO = " + i;

                for (int j = 1; j <= 12; j++)
                {
                    var sql3 = sql2 + " AND MES = " + j;

                    DataTable dt2;
                    try
                    {
                        dt2 = Conexion.SqlDataTableProductProfile(sql3);
                    }
                    catch (Exception e)
                    {
                        dt2 = null;
                    }

                    if (dt2.Rows.Count > 0)
                    {
                        sumTotal += (Decimal)dt2.Rows[0]["Valor"];
                        registros += (Int32) dt2.Rows[0]["Registros"];
                    }
                    if (j == 12)
                    {
                        dt.Rows.Add(i,sumTotal,registros);
                    }

                    sql3 = "";
                }
            }
            return dt;
        }
        public static DataTable SearchTablePrecProm(int IdProducto, int IdPaisAduana, string tipoOpe)
        {
            Decimal sumTotal;
            int registros;
            int mesExiste;
            int añoActual = DateTime.Today.Year;

            DataTable dt = new DataTable();
            dt.Columns.Add("Año", typeof(int));
            dt.Columns.Add("PrecioUnit", typeof(Decimal));
            dt.Columns.Add("Registros", typeof(int));

            var sql2 = "";
            for (int i = añoActual - 4; i <= añoActual; i++)
            {
                sumTotal = 0;
                registros = 0;
                mesExiste = 0;
                sql2 = "SELECT Año, Mes = CASE Mes WHEN 1 THEN 'Enero' WHEN 2 THEN 'Febrero' WHEN 3 THEN 'Marzo' WHEN 4 THEN 'Abril' " +
                       "WHEN 5 THEN 'Mayo' WHEN 6 THEN 'Junio' WHEN 7 THEN 'Julio' WHEN 8 THEN 'Agosto' WHEN 9 THEN 'Septiembre' " +
                       "WHEN 10 THEN 'Octubre' WHEN 11 THEN 'Noviembre' WHEN 12 THEN 'Diciembre' END, ISNULL(PrecioUnit,0) AS PrecioUnit, Registros " +
                       "FROM TOTALESAÑO_MES WHERE REGIMEN = '" + tipoOpe + "' AND" +
                       " IDPRODUCTO = " + IdProducto +
                       " AND IDPAISADUANA = " + IdPaisAduana +
                       " AND AÑO = " + i;

                for (int j = 1; j <= 12; j++)
                {
                    var sql3 = sql2 + " AND MES = " + j;

                    DataTable dt2;
                    try
                    {
                        dt2 = Conexion.SqlDataTableProductProfile(sql3);
                    }
                    catch (Exception e)
                    {
                        dt2 = null;
                    }

                    if (dt2.Rows.Count > 0)
                    {
                        sumTotal += (Decimal)dt2.Rows[0]["PrecioUnit"];
                        registros += (Int32)dt2.Rows[0]["Registros"];
                        mesExiste += 1;
                    }
                    if (j == 12)
                    {
                        if (mesExiste == 0)
                        {
                            mesExiste = 1;
                        }
                        dt.Rows.Add(i, sumTotal/mesExiste,registros);
                    }

                    sql3 = "";
                }
            }
            return dt;
        }
        public static DataTable SearchYear(int IdProducto, int IdPaisAduana, string tipoOpe)
        {
            var sql = "SELECT * FROM (SELECT TOP 3 AÑO FROM TOTALESAÑO_MES WHERE IDPAISADUANA = " + IdPaisAduana + " " +
                      "AND IDPRODUCTO = " + IdProducto + " AND REGIMEN='" + tipoOpe + "' " +
                      "GROUP BY AÑO ORDER BY AÑO DESC) AS TEMPAÑO ORDER BY  AÑO";

            DataTable dt;
            try
            {
                dt = Conexion.SqlDataTableProductProfile(sql);
            }
            catch (Exception e)
            {
                dt = null;
            }

            return dt;
        }
        public static DataTable SearchRankingImp(int IdProducto, int IdPaisAduana, string tipoOpe)
        {
            var sql = "SELECT Empresa, Valor FROM RESUMENEMPRESA WHERE IDPRODUCTO = " + IdProducto +
                      " AND IDPAISADUANA = " + IdPaisAduana + " AND REGIMEN = '" + tipoOpe + "' ORDER BY VALOR DESC ";

            DataTable dt;
            try
            {
                dt = Conexion.SqlDataTableProductProfile(sql);
            }
            catch (Exception e)
            {
                dt = null;
            }
            decimal ValorOtros = 0;
            DataTable dt2 = new DataTable();
            dt2.Columns.Add("Empresa", typeof(string));
            dt2.Columns.Add("Valor", typeof(decimal));
            if (dt.Rows.Count > 3)
            {
                for (int j = 3; j < dt.Rows.Count; j++)
                {
                    ValorOtros += Convert.ToDecimal(dt.Rows[j]["Valor"]);
                }

                for (int i = 0; i < 3; i++)
                {
                    dt2.Rows.Add(dt.Rows[i]["Empresa"].ToString(), Convert.ToDecimal(dt.Rows[i]["Valor"]));
                }
                dt2.Rows.Add("OTROS", ValorOtros);

                return dt2;
            }
            else
            {
                return dt;
            }
        }
        public static DataTable SearchRankPais(int IdProducto, int IdPaisAduana, string tipoOpe)
        {
            var sql = "SELECT Pais, Valor FROM RESUMENPAIS WHERE IDPRODUCTO = " + IdProducto +
                      " AND IDPAISADUANA = " + IdPaisAduana + " AND REGIMEN = '" + tipoOpe + "' ORDER BY VALOR DESC";

            DataTable dt;

            try
            {
                dt = Conexion.SqlDataTableProductProfile(sql);
            }
            catch (Exception e)
            {
                dt = null;
            }
            decimal ValorOtros = 0;
            DataTable dt2 = new DataTable();
            dt2.Columns.Add("Pais", typeof(string));
            dt2.Columns.Add("Valor", typeof(decimal));
            if (dt.Rows.Count > 3)
            {
                for (int j = 3; j < dt.Rows.Count; j++)
                {
                    ValorOtros += Convert.ToDecimal(dt.Rows[j]["Valor"]);
                }

                for (int i = 0; i < 3; i++)
                {
                    dt2.Rows.Add(dt.Rows[i]["Pais"].ToString(), Convert.ToDecimal(dt.Rows[i]["Valor"]));
                }
                dt2.Rows.Add("OTROS", ValorOtros);

                return dt2;
            }
            else
            {
                return dt;
            }
        }
        public static DataTable SearchTableRankImp(int IdProducto, int IdPaisAduana, string tipoOpe)
        {
            var sql = "SELECT Regimen, Pais, Valor FROM RESUMENPAIS WHERE IDPRODUCTO = " + IdProducto +
                      " AND IDPAISADUANA = " + IdPaisAduana + " AND REGIMEN = '" + tipoOpe + "' ORDER BY VALOR DESC";

            DataTable dt;
            decimal total = 0;
            try
            {
                dt = Conexion.SqlDataTableProductProfile(sql);
            }
            catch (Exception e)
            {
                dt = null;
            }
            DataTable dt2 = new DataTable();
            DataRow newRow = dt2.NewRow();
            dt2.Columns.Add("Regimen", typeof(string));
            dt2.Columns.Add("Pais", typeof(string));
            dt2.Columns.Add("Valor", typeof(decimal));
            dt2.Columns.Add("Porcentaje", typeof(decimal));

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                total += Convert.ToDecimal(dt.Rows[i]["Valor"]);
            }

            decimal ValorOtros = 0;

            if (dt.Rows.Count > 3)
            {
                for (int j = 3; j < dt.Rows.Count; j++)
                {
                    ValorOtros += Convert.ToDecimal(dt.Rows[j]["Valor"]);
                }

                for (int i = 0; i < 3; i++)
                {
                    dt2.Rows.Add(tipoOpe, dt.Rows[i]["Pais"].ToString(), Convert.ToDecimal(dt.Rows[i]["Valor"]), (Convert.ToDecimal(dt.Rows[i]["Valor"]) / total) * 100);
                }
                dt2.Rows.Add(tipoOpe, "OTROS", ValorOtros, (ValorOtros / total) * 100);
            }
            else
            {
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    dt2.Rows.Add(tipoOpe, dt.Rows[j]["Pais"].ToString(), Convert.ToDecimal(dt.Rows[j]["Valor"]), (Convert.ToDecimal(dt.Rows[j]["Valor"]) / total) * 100);
                }
            }

            return dt2;
        }
        public static DataTable SearchTableRankPais(int IdProducto, int IdPaisAduana, string tipoOpe)
        {
            var sql = "SELECT  Regimen, Empresa, Valor FROM RESUMENEMPRESA WHERE IDPRODUCTO = " + IdProducto +
                      " AND IDPAISADUANA = " + IdPaisAduana + " AND REGIMEN = '" + tipoOpe + "' ORDER BY VALOR DESC ";

            DataTable dt;
            decimal total = 0;
            try
            {
                dt = Conexion.SqlDataTableProductProfile(sql);
            }
            catch (Exception e)
            {
                dt = null;
            }

            DataTable dt2 = new DataTable();
            DataRow newRow = dt2.NewRow();
            dt2.Columns.Add("Regimen", typeof(string));
            dt2.Columns.Add("Empresa", typeof(string));
            dt2.Columns.Add("Valor", typeof(decimal));
            dt2.Columns.Add("Porcentaje", typeof(decimal));

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                total += Convert.ToDecimal(dt.Rows[i]["Valor"]);
            }



            decimal ValorOtros = 0;

            if (dt.Rows.Count > 3)
            {
                for (int j = 3; j < dt.Rows.Count; j++)
                {
                    ValorOtros += Convert.ToDecimal(dt.Rows[j]["Valor"]);
                }

                for (int i = 0; i < 3; i++)
                {
                    dt2.Rows.Add(tipoOpe, dt.Rows[i]["Empresa"].ToString(), Convert.ToDecimal(dt.Rows[i]["Valor"]), (Convert.ToDecimal(dt.Rows[i]["Valor"]) / total) * 100);
                }
                dt2.Rows.Add(tipoOpe, "OTROS", ValorOtros, (ValorOtros / total) * 100);
            }
            else
            {
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    dt2.Rows.Add(tipoOpe, dt.Rows[j]["Empresa"].ToString(), Convert.ToDecimal(dt.Rows[j]["Valor"]), (Convert.ToDecimal(dt.Rows[j]["Valor"]) / total) * 100);
                }
            }

            return dt2;
        }
        public static DataTable SearchOtherProduct(string culture, string partida)
        {
            var sql = "SELECT Partida = PR.CodProducto, Descripcion = PR.DescripcionES, " +
                      "PaisAduana = (SELECT TOP 1 PA.PaisAduana FROM TOTALES T INNER JOIN PAISADUANA PA ON PA.IdPaisAduana = T.IdPaisAduana WHERE T.IDPRODUCTO = PR.IDPRODUCTO ORDER BY T.VALOR DESC),  " +
                      "Uri = PR.UriES FROM PRODUCTO PR INNER JOIN TOTALES T ON PR.IdProducto = T.IdProducto" +
                      " WHERE PR.CodProducto = '-' GROUP BY PR.IdProducto, PR.CodProducto,PR.DescripcionES, PR.UriES";

            DataTable dt;
            try
            {
                dt = Conexion.SqlDataTableProductProfile(sql);
            }
            catch (Exception e)
            {
                dt = null;
            }

            var sql2 = "";
            if (culture == "es")
            {
                sql2 = "SELECT Partida = PR.CodProducto, Descripcion = PR.DescripcionES, " +
                       "PaisAduana = (SELECT TOP 1 PA.PaisAduana FROM TOTALES T INNER JOIN PAISADUANA PA ON PA.IdPaisAduana = T.IdPaisAduana WHERE T.IDPRODUCTO = PR.IDPRODUCTO ORDER BY T.VALOR DESC),  " +
                       "Uri = PR.UriES FROM PRODUCTO PR INNER JOIN TOTALES T ON PR.IdProducto = T.IdProducto";
            }
            else
            {
                sql2 = "SELECT Partida = PR.CodProducto, Descripcion = PR.DescripcionEN, " +
                       "PaisAduana = (SELECT TOP 1 PA.PaisAduana FROM TOTALES T INNER JOIN PAISADUANA PA ON PA.IdPaisAduana = T.IdPaisAduana WHERE T.IDPRODUCTO = PR.IDPRODUCTO ORDER BY T.VALOR DESC),  " +
                       "Uri = PR.UriEN FROM PRODUCTO PR INNER JOIN TOTALES T ON PR.IdProducto = T.IdProducto";
            }


            int tam = partida.Length;
            int codI = 0;
            int codPenU = 0;

            string partidaN = "";

            codI = Convert.ToInt32(partida.Substring(tam - 1, 1));
            codPenU = Convert.ToInt32(partida.Substring(tam - 2, 1));
            partidaN = partida.Substring(0, tam);
            string partidaRN = partidaN.Substring(0, tam - 2);
            //string partidaO = partida.Substring(j, partida.Length - j);
            //Console.WriteLine(codI);

            for (int k = codPenU; k <= 9; k++)
            {
                if (dt.Rows.Count == 5)
                {
                    break;
                }
                for (int i = codI; i <= 9; i++)
                {
                    partidaN = partidaRN + k.ToString() + i.ToString();
                    if (dt.Rows.Count == 5)
                    {
                        break;
                    }                    
                    var sql3 = "";
                    if (culture == "es")
                    {
                        sql3 = sql2 + " WHERE PR.CodProducto = '" + partidaN + "' " +
                                   "GROUP BY PR.IdProducto, PR.CodProducto,PR.DescripcionES, PR.UriES";
                    }
                    else
                    {
                        sql3 = sql2 + " WHERE PR.CodProducto = '" + partidaN + "' " +
                                   "GROUP BY PR.IdProducto, PR.CodProducto,PR.DescripcionEN, PR.UriEN";
                    }
                    //Console.WriteLine(i);
                    DataTable dt2;
                    try
                    {
                        dt2 = Conexion.SqlDataTableProductProfile(sql3);
                    }
                    catch (Exception e)
                    {
                        dt2 = null;
                    }
                    //if (codigoC.Contains(i.ToString()))
                    if (dt2.Rows.Count > 0 && partida != partidaN)
                    {
                        DataRow newRow = dt.NewRow();
                        newRow["Partida"] = dt2.Rows[0]["Partida"];
                        newRow["Descripcion"] = dt2.Rows[0]["Descripcion"];
                        newRow["PaisAduana"] = dt2.Rows[0]["PaisAduana"];
                        newRow["Uri"] = dt2.Rows[0]["Uri"];
                        dt.Rows.Add(newRow);
                    }
                    //codigoN[posN] = codigo[0].Substring(0,i);
                    sql3 = "";
                    dt2.Clear();
                }

                for (int i = codI-1; i >= 0; i--)
                {
                    partidaN = partidaRN + k.ToString() + i.ToString();
                    if (dt.Rows.Count == 5)
                    {
                        break;
                    }
                    
                    var sql3 = "";
                    if (culture == "es")
                    {
                        sql3 = sql2 + " WHERE PR.CodProducto = '" + partidaN + "' " +
                               "GROUP BY PR.IdProducto, PR.CodProducto,PR.DescripcionES, PR.UriES";
                    }
                    else
                    {
                        sql3 = sql2 + " WHERE PR.CodProducto = '" + partidaN + "' " +
                               "GROUP BY PR.IdProducto, PR.CodProducto,PR.DescripcionEN, PR.UriEN";
                    }
                    //Console.WriteLine(i);
                    DataTable dt2;
                    try
                    {
                        dt2 = Conexion.SqlDataTableProductProfile(sql3);
                    }
                    catch (Exception e)
                    {
                        dt2 = null;
                    }
                    //if (codigoC.Contains(i.ToString()))
                    if (dt2.Rows.Count > 0 && partida != partidaN)
                    {
                        DataRow newRow = dt.NewRow();
                        newRow["Partida"] = dt2.Rows[0]["Partida"];
                        newRow["Descripcion"] = dt2.Rows[0]["Descripcion"];
                        newRow["PaisAduana"] = dt2.Rows[0]["PaisAduana"];
                        newRow["Uri"] = dt2.Rows[0]["Uri"];
                        dt.Rows.Add(newRow);
                    }
                    //codigoN[posN] = codigo[0].Substring(0,i);
                    sql3 = "";
                    dt2.Clear();
                }
            }

            for (int k = codPenU-1; k >= 0; k--)
            {
                if (dt.Rows.Count == 5)
                {
                    break;
                }
                for (int i = codI; i <= 9; i++)
                {
                    partidaN = partidaRN + k.ToString() + i.ToString();
                    if (dt.Rows.Count == 5)
                    {
                        break;
                    }
                    
                    var sql3 = "";
                    if (culture == "es")
                    {
                        sql3 = sql2 + " WHERE PR.CodProducto = '" + partidaN + "' " +
                                   "GROUP BY PR.IdProducto, PR.CodProducto,PR.DescripcionES, PR.UriES";
                    }
                    else
                    {
                        sql3 = sql2 + " WHERE PR.CodProducto = '" + partidaN + "' " +
                                   "GROUP BY PR.IdProducto, PR.CodProducto,PR.DescripcionEN, PR.UriEN";
                    }
                    //Console.WriteLine(i);
                    DataTable dt2;
                    try
                    {
                        dt2 = Conexion.SqlDataTableProductProfile(sql3);
                    }
                    catch (Exception e)
                    {
                        dt2 = null;
                    }
                    //if (codigoC.Contains(i.ToString()))
                    if (dt2.Rows.Count > 0 && partida != partidaN)
                    {
                        DataRow newRow = dt.NewRow();
                        newRow["Partida"] = dt2.Rows[0]["Partida"];
                        newRow["Descripcion"] = dt2.Rows[0]["Descripcion"];
                        newRow["PaisAduana"] = dt2.Rows[0]["PaisAduana"];
                        newRow["Uri"] = dt2.Rows[0]["Uri"];
                        dt.Rows.Add(newRow);
                    }
                    //codigoN[posN] = codigo[0].Substring(0,i);
                    sql3 = "";
                    dt2.Clear();
                }

                for (int i = codI-1 ; i >= 0; i--)
                {
                    partidaN = partidaRN + k.ToString() + i.ToString();
                    if (dt.Rows.Count == 5)
                    {
                        break;
                    }
                    
                    var sql3 = "";
                    if (culture == "es")
                    {
                        sql3 = sql2 + " WHERE PR.CodProducto = '" + partidaN + "' " +
                               "GROUP BY PR.IdProducto, PR.CodProducto,PR.DescripcionES, PR.UriES";
                    }
                    else
                    {
                        sql3 = sql2 + " WHERE PR.CodProducto = '" + partidaN + "' " +
                               "GROUP BY PR.IdProducto, PR.CodProducto,PR.DescripcionEN, PR.UriEN";
                    }
                    //Console.WriteLine(i);
                    DataTable dt2;
                    try
                    {
                        dt2 = Conexion.SqlDataTableProductProfile(sql3);
                    }
                    catch (Exception e)
                    {
                        dt2 = null;
                    }
                    //if (codigoC.Contains(i.ToString()))
                    if (dt2.Rows.Count > 0 && partida != partidaN)
                    {
                        DataRow newRow = dt.NewRow();
                        newRow["Partida"] = dt2.Rows[0]["Partida"];
                        newRow["Descripcion"] = dt2.Rows[0]["Descripcion"];
                        newRow["PaisAduana"] = dt2.Rows[0]["PaisAduana"];
                        newRow["Uri"] = dt2.Rows[0]["Uri"];
                        dt.Rows.Add(newRow);
                    }
                    //codigoN[posN] = codigo[0].Substring(0,i);
                    sql3 = "";
                    dt2.Clear();
                }
            }

            return dt;
        }

        /* JANAQ 150620
       * Funcion que permite crear o modificar usuarios mix panel
       */
        public static void CrearUsuarioMixPanel(string idUsuario, string host)
        {
            if (Convert.ToBoolean(Properties.Settings.Default.HabilitarTrackingMixPanel))
            {

                UsuarioMixPanel usuario = Funciones.ObtieneDatosUsuarioPorUsuario(idUsuario);

                var productionHost = Properties.Settings.Default.UrlProdHostWeb;
                var token="";

                //Si el hostname es diferente a produccion se usara el token de desarrollo 
                if (productionHost.ToLower().IndexOf(host) < 0)
                {
                    token= Properties.Settings.Default.TokenDevMixPanel;
                }
                else
                {
                    token= Properties.Settings.Default.TokenProdMixPanel;
                }


                var sisOperativo = "Windows";
                var browser = "Chrome";
                var browser_version = "1.21.11.0";
                string responseData;

                //Creacion del Objeto JSON del usuario
                var data = "{\"$token\":\"" + token + "\",\"$distinct_id\":\"" + usuario.IdUsuario +
                           "\",\"$ip\":\"" + usuario.DireccionIP + "\",\"$set\":{" +
                            "\"$name\":\"" + usuario.Cliente + "\",\"$email\":\"" + usuario.Email1 +
                             "\",\"$os\":\"" + sisOperativo + "\",\"$region\":\"" + usuario.Ciudad +
                             "\",\"$city\":\"" + usuario.Ciudad + "\",\"$country_code\":\"" + usuario.codPais +
                             "\",\"$browser\":\"" + browser + "\",\"$browser_version\":\"" + browser_version +
                              "\",\"Cliente\":\"" + usuario.Nombres + "\",\"Email2\":\"" + usuario.Email2 +
                             "\",\"CodUsuario\":\"" + usuario.CodUsuario + "\",\"Plan\":\"" + usuario.Plan +
                              "\",\"Tipo\":\"" + usuario.Tipo + "\",\"Sector\":\"" + usuario.Sector +
                              "\",\"IdTipo\":\"" + usuario.IdTipo +
                              "\"}}";

                string encodeDataBase64 = Base64.encode(data);
                try
                {
                    var url = string.Format("{0}?data={1}", Properties.Settings.Default.UrlMixPanel, encodeDataBase64);

                    //Consumo del Servicio Rest de Mix Panel
                    System.Net.WebRequest hwrequest = System.Net.WebRequest.Create(url);
                    hwrequest.ContentType = "application/json";
                    hwrequest.Method = "GET";

                    System.Net.WebResponse hwresponse = hwrequest.GetResponse();
                    System.IO.StreamReader responseStream = new System.IO.StreamReader(hwresponse.GetResponseStream());

                    // ResponseData si es igual 1=Ok , 0=Hubo un error
                    responseData = responseStream.ReadToEnd();

                    hwresponse.Close();
                    if (responseData == "0") throw new Exception("Ocurrio un error en CrearUsuarioMixPanel");
                }
                catch (Exception ex)
                {
                    ExceptionUtility.LogException(ex, "CrearUsuarioMixPanel");
                }
            }

        }
    }
}