using System;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;

namespace Veritrade2018.Helpers
{
    public class Conexion
    {
        //this is a shortcut for your connection string
#if DEBUG
        private static readonly string DataContentString = System.Configuration.ConfigurationManager.ConnectionStrings["LocalContent"].ConnectionString;
        private static readonly string DataMinisiteString = System.Configuration.ConfigurationManager.ConnectionStrings["LocalMinisite"].ConnectionString;
        private static readonly string DataSystemString = System.Configuration.ConfigurationManager.ConnectionStrings["LocalSystem"].ConnectionString;
        private static readonly string DataProductProfileString = System.Configuration.ConfigurationManager.ConnectionStrings["LocalProductProfile"].ConnectionString;
#else
        private static readonly string DataContentString = System.Configuration.ConfigurationManager.ConnectionStrings["RemoteContent"].ConnectionString;
        private static readonly string DataMinisiteString = System.Configuration.ConfigurationManager.ConnectionStrings["RemoteMinisite"].ConnectionString;
        private static readonly string DataSystemString = System.Configuration.ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
        private static readonly string DataProductProfileString = System.Configuration.ConfigurationManager.ConnectionStrings["RemoteProductProfile"].ConnectionString;
#endif

        // this is for just executing sql command with no value to return
        public static void SqlExecute(string sql, bool contenido = false)
        {
            var databaseConnectionString = contenido ? DataContentString : DataSystemString;
            using (var conn = new SqlConnection(databaseConnectionString))
            {
                var cmd = new SqlCommand(sql, conn);
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
            }
        }

        // with this you will be able to return a value
        public static object SqlReturn(string sql, bool contenido = false)
        {
            var databaseConnectionString = contenido ? DataContentString : DataSystemString;
            using (var conn = new SqlConnection(databaseConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(sql, conn);
                object result = cmd.ExecuteScalar();
                cmd.Connection.Close();
                return result;
            }
        }

        // with this you can retrieve an entire table or part of it
        public static DataTable SqlDataTable(string sql, bool contenido = false,bool deadlock = false)
        {
            var databaseConnectionString = contenido ? DataContentString : DataSystemString;
            try
            {
                using (var conn = new SqlConnection(databaseConnectionString))
                {
                    var cmd = new SqlCommand(sql, conn);

                    if (!deadlock)
                        cmd.CommandTimeout = 240;
                    cmd.Connection.Open();
                    var tempTable = new DataTable();
                    tempTable.Load(cmd.ExecuteReader());
                    cmd.Connection.Close();
                    return tempTable;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
            
        }

        // sooner or later you will probably use stored procedures. 
        // you can use this in order to execute a stored procedure with 1 parameter
        // it will work for returning a value or just executing with no returns
        public static object SqlStoredProcedure1Param(string storedProcedure, string prmName1, object param1, bool contenido = false)
        {
            var databaseConnectionString = contenido ? DataContentString : DataSystemString;
            using (var conn = new SqlConnection(databaseConnectionString))
            {
                var cmd = new SqlCommand(storedProcedure, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter(prmName1, param1.ToString()));
                cmd.Connection.Open();
                var obj = cmd.ExecuteScalar();
                cmd.Connection.Close();
                return obj;
            }
        }

        //retrive data from Minisite
        public static DataTable SqlDataTableMinisite(string sql)
        {
            var databaseConnectionString = DataMinisiteString;
            try
            {
                using (var conn = new SqlConnection(databaseConnectionString))
                {
                    var cmd = new SqlCommand(sql, conn);
                    cmd.Connection.Open();
                    var tempTable = new DataTable();
                    tempTable.Load(cmd.ExecuteReader());
                    cmd.Connection.Close();
                    return tempTable;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        //retrive data from ProductProfile
        public static DataTable SqlDataTableProductProfile(string sql)
        {
            var databaseConnectionString = DataProductProfileString;
            try
            {
                using (var conn = new SqlConnection(databaseConnectionString))
                {
                    var cmd = new SqlCommand(sql, conn);
                    cmd.Connection.Open();
                    var tempTable = new DataTable();
                    tempTable.Load(cmd.ExecuteReader());
                    cmd.Connection.Close();
                    return tempTable;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void SqlSpGeneraArchivoBusqueda( int idUsuario, bool esFreeTrial,  string codPais1, 
           string tipoOpe, string titulos, string campos, 
           string sqlFiltro, int cantReg, string nombreArchivo,
           char excel ,  bool contenido = false)
        {
            var databaseConnectionString = contenido ? DataContentString : DataSystemString;
            using (var conn = new SqlConnection(databaseConnectionString))
            {
                var cmd = new SqlCommand("GeneraArchivoBusqueda", conn);
                //var cmd = new SqlCommand("GeneraArchivoExcelBusqueda", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@IdUsuario", SqlDbType.Int, 4).Value = idUsuario;
                if (esFreeTrial)
                    cmd.Parameters.Add("@FreeTrial", SqlDbType.Char, 1).Value = "S";

                cmd.Parameters.Add("@CodPais", SqlDbType.VarChar, 3).Value = codPais1;
                cmd.Parameters.Add("@TipoOpe", SqlDbType.VarChar, 1).Value = tipoOpe;
                cmd.Parameters.Add("@Titulos", SqlDbType.VarChar, 1200).Value = titulos; // Ruben 202308
                cmd.Parameters.Add("@Campos", SqlDbType.VarChar, 1200).Value = campos; // Ruben 202308
                cmd.Parameters.Add("@SqlFiltro", SqlDbType.VarChar, 8000).Value = sqlFiltro;
                cmd.Parameters.Add("@CantReg", SqlDbType.Int).Value = cantReg;
                cmd.Parameters.Add("@NombreArchivo", SqlDbType.VarChar, 100).Value = nombreArchivo;
                cmd.Parameters.Add("@Excel", SqlDbType.Char, 1).Value = excel;
                cmd.CommandTimeout = 600;
                //cmd.Parameters.Add(new SqlParameter(prmName1, param1.ToString()));
                cmd.Connection.Open();
                cmd.ExecuteScalar();
                cmd.Connection.Close();
            }
        }

        private static Int32 _inttiempofuerapredeterminado = 30;
        #region manejador de conexion
        //public SqlConnection ObtenerConexion()
        //{
        //    return ObtenerConexion(_inttiempofuerapredeterminado);
        //}

        public static SqlConnection ObtenerConexion(bool contenido = false)
        {
            SqlConnection cn;

            var databaseConnectionString = contenido ? DataContentString : DataSystemString;
            try
            {
                cn = new SqlConnection(databaseConnectionString);
                //cn.InfoMessage += new SqlInfoMessageEventHandler(conexion_InfoMessage);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return cn;
        }


        #endregion
        

    }
}