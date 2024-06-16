using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;
using Veritrade2018.Helpers;

namespace Veritrade2018.Models
{
    public class SesionUsuario
    {
        public int IdSesionUsuario { get; set; }
        public int IdUsuario { get; set; }
        public bool SesionActiva { get; set; }

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
            catch (Exception ex)
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
    }
}