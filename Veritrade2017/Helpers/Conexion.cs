using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Veritrade2017.Helpers
{
    public class Conexion
    {
        //this is a shortcut for your connection string
#if DEBUG
        public static readonly string DataContentString = System.Configuration.ConfigurationManager.ConnectionStrings["LocalContent"].ConnectionString;
        public static readonly string DataMinisiteString = System.Configuration.ConfigurationManager.ConnectionStrings["LocalMinisite"].ConnectionString;
        public static readonly string DataSystemString = System.Configuration.ConfigurationManager.ConnectionStrings["LocalSystem"].ConnectionString;
        public static readonly string DataProductProfileString = System.Configuration.ConfigurationManager.ConnectionStrings["LocalProductProfile"].ConnectionString;
#else
        public static readonly string DataContentString = System.Configuration.ConfigurationManager.ConnectionStrings["RemoteContent"].ConnectionString;
        public static readonly string DataMinisiteString = System.Configuration.ConfigurationManager.ConnectionStrings["RemoteMinisite"].ConnectionString;
        public static readonly string DataSystemString = System.Configuration.ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
        // Ruben 202301
        public static readonly string DataProductProfileString = System.Configuration.ConfigurationManager.ConnectionStrings["RemoteProductProfile"].ConnectionString;
        //public static readonly string DataProductProfileString = System.Configuration.ConfigurationManager.ConnectionStrings["LocalProductProfile"].ConnectionString;
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
        public static DataTable SqlDataTable(string sql, bool contenido = false)
        {
            var databaseConnectionString = contenido ? DataContentString : DataSystemString;
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
                    cmd.CommandTimeout = 120;
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

        public static async System.Threading.Tasks.Task<System.Data.DataTable> SqlDataTableAsync(string ConnectionString, string qry)
        {
            // Create a connection, open it and create a command on the connection
            try
            {

                System.Data.DataTable dt = new System.Data.DataTable();
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    await connection.OpenAsync().ConfigureAwait(false);
                    using (SqlCommand command = new SqlCommand(qry, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            dt.Load(reader);
                            return dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine("Exception Raised ... ");
                System.Diagnostics.Debug.WriteLine(ex.Message);

                return new System.Data.DataTable(qry);
            }

        }

        //public static async Task<DataTable> SqlDataTableAsync(string connStr, string sql)
        //{
        //    var dt = new DataTable();
        //    var connection = new SqlConnection(connStr);
        //    await connection.OpenAsync();
        //    var reader = await connection.CreateCommand().ExecuteReaderAsync();
        //    dt.Load(reader);

        //    return dt;
        //}

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
    }
}
