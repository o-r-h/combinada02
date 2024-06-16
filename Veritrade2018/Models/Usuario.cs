using System;
using System.Data;
using Veritrade2018.Helpers;

namespace Veritrade2018.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string Empresa { get; set; }
        public string CodPais { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Telefono { get; set; }
        public string Email1 { get; set; }

        public static Usuario GetUsuario(int id)
        {
            var sql = "SELECT [Empresa],[CodPais],[Nombres],[Apellidos],[Telefono],[Email1] " +
                      "FROM [dbo].[Usuario] where [IdUsuario] = " + id;

            var dt = Conexion.SqlDataTable(sql);
            var model = new Usuario();

            foreach (DataRow row in dt.Rows)
            {
                model.Empresa = row["Empresa"].ToString();
                model.CodPais = row["CodPais"].ToString();
                model.Nombres = row["Nombres"].ToString();
                model.Apellidos = row["Apellidos"].ToString();
                model.Telefono = row["Telefono"].ToString();
                model.Email1 = row["Email1"].ToString();
            }

            return model;
        }

        public static string GetPassword(string email)
        {
            var clave = "";
            var sql = "SELECT [Password] FROM [dbo].[Usuario] where [Email1] = '" + email + "'";

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                clave = row["Password"].ToString();
            }

            return clave;
        }

        public static void UpdateState(string email, decimal importe, string campania, string nombres, string apellidos, string empresa)
        {
            var sql = "UPDATE [dbo].[Usuario] SET IdTipo = 10, " +
                      " Nombres = '" + nombres + "'," +
                      " Apellidos = '" + apellidos + "'," +
                      " Empresa = '" + empresa + "'," +
                      " FecInicio = '" + DateTime.Now.ToString("yyyyMMdd") + "'," +
                      " FecFin = '" + DateTime.Now.AddDays(364).ToString("yyyyMMdd") + "'," +
                      " CodEstado = 'A'," +
                      " ImporteUSD = " + importe +
                      ", CodCampaña = '" + campania +
                      "' WHERE Email1 = '" + email + "'";
            Conexion.SqlExecute(sql);
        }

        public static void UpdateFactura(string email, string ruc, string direccion, string razonSocial)
        {
            var sql = "UPDATE [dbo].[Usuario] " +
                      " SET RUC = '" + ruc + "'," +
                      " Direccion = '" + direccion + "'," +
                      " ComentAdmin = '" + razonSocial + "'" +
                      " WHERE Email1 = '" + email + "'";
            Conexion.SqlExecute(sql);
        }

        public static bool ExisUsuarioPlan(string email)
        {
            var sql = "SELECT * FROM [dbo].[Usuario] where [Email1] = '" + email + "' AND IdTipo = 10";

            var dt = Conexion.SqlDataTable(sql);
            return dt.Rows.Count > 0;
        }

        public static bool ExisUsuario(string email)
        {
            var sql = "SELECT * FROM [dbo].[Usuario] where [Email1] = '" + email + "'";

            var dt = Conexion.SqlDataTable(sql);
            return dt.Rows.Count > 0;
        }

        public static string GetIdUsuario(string codUsuario)
        {
            var id = "";
            var sql = "SELECT IdUsuario FROM [dbo].[Usuario] " +
                      "WHERE CodUsuario = '" + codUsuario + "'";

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                id = row["IdUsuario"].ToString();
            }

            return id;
        }
    }
}