using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Veritrade.Veritrade
{
    public class ObjectDS
    {
        public int Cuenta(string sqlO, string Cant)
        {
            return Convert.ToInt32(Cant);
        }

        /*public int Cuenta(string sqlO, string sqlO2)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            int Cant = 0;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select max(nro) as Cant from (" + sqlO + ") T ";

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) Cant = Convert.ToInt32(dtr["Cant"]);

            dtr.Close();
            cn.Close();

            return Cant;
        }
        */

        public DataTable Lista(string sqlO, int Cant, int startRowIndex, int maximumRows)
        {
            string sql = "";
            SqlConnection cn;
            SqlCommand cmd;
            DataSet dtaset;
            SqlDataAdapter dtadap;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select * from (" + sqlO + ") T ";
            sql += "where Nro between " + (startRowIndex + 1).ToString() + " and " + (startRowIndex + maximumRows).ToString();

            cmd = new SqlCommand(sql, cn);
            dtadap = new SqlDataAdapter(cmd);
            dtaset = new DataSet();
            dtadap.Fill(dtaset);

            cn.Close();

            return dtaset.Tables[0];
        }

        public static DataTable Top(string sqlO)
        {
            int n = 5 + 1;
            string sql = "";
            SqlConnection cn;
            SqlCommand cmd;
            DataSet dtaset;
            SqlDataAdapter dtadap;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select top " + n.ToString() + " * from (" + sqlO + ") T ";

            cmd = new SqlCommand(sql, cn);
            dtadap = new SqlDataAdapter(cmd);
            dtaset = new DataSet();
            dtadap.Fill(dtaset);

            cn.Close();

            return dtaset.Tables[0];
        }

        public static DataTable Lista2(string sqlO)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            DataSet dtaset;
            SqlDataAdapter dtadap;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = sqlO;

            cmd = new SqlCommand(sql, cn);
            dtadap = new SqlDataAdapter(cmd);
            dtaset = new DataSet();
            dtadap.Fill(dtaset);

            cn.Close();

            return dtaset.Tables[0];
        }

        /*
        public SqlDataReader Lista(string sqlO, int StartRowIndex, int MaximumRows, String SortExpression)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select * from (" + sqlO + ") T ";
            sql += "where Nro between " + (StartRowIndex + 1).ToString() + " and " + (StartRowIndex + MaximumRows).ToString();

            cmd = new SqlCommand(sql, cn);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@StartRowIndex", StartRowIndex);
            cmd.Parameters.AddWithValue("@MaximumRows", MaximumRows);
            cmd.Parameters.AddWithValue("@SortExpression", SortExpression);
                       
            return cmd.ExecuteReader(CommandBehavior.CloseConnection);
        }
        */
    }
}