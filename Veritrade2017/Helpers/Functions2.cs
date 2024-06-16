using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Veritrade2017.Helpers
{
    public static class Funciones2
    {
        public static string BuscaIdPartida(string Nandina, string CodPais)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string IdPartida = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select IdPartida from Partida_" + CodPais + " where Nandina = '" + Nandina + "' ";

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) IdPartida = dtr["IdPartida"].ToString();

            cn.Close();

            return IdPartida;
        }

        public static string BuscaIdEmpresa(string Empresa, string CodPais)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string IdEmpresa = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select IdEmpresa from Empresa_" + CodPais + " where Empresa = '" + Empresa + "' ";

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) IdEmpresa = dtr["IdEmpresa"].ToString();

            cn.Close();

            return IdEmpresa;
        }

        public static string BuscaIdProveedor(string Proveedor, string CodPais)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string IdProveedor = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select IdProveedor from Proveedor_" + CodPais + " where Proveedor = '" + Proveedor + "' ";

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) IdProveedor = dtr["IdProveedor"].ToString();

            cn.Close();

            return IdProveedor;
        }

        public static string BuscaIdImportadorExp(string ImportadorExp, string CodPais)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string IdImportadorExp = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select IdImportadorExp from ImportadorExp_" + CodPais + " where ImportadorExp = '" + ImportadorExp + "' ";

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) IdImportadorExp = dtr["IdImportadorExp"].ToString();

            cn.Close();

            return IdImportadorExp;
        }

        public static string BuscaIdPais(string Pais, string CodPais)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string IdPais = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select IdPais from Pais_" + CodPais + " where Pais = '" + Pais + "' ";

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) IdPais = dtr["IdPais"].ToString();

            cn.Close();

            return IdPais;
        }

        public static string BuscaIdViaTransp(string ViaTransp, string CodPais)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string IdViaTransp = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select IdViaTransp from ViaTransp_" + CodPais + " where ViaTransp = '" + ViaTransp + "' ";

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) IdViaTransp = dtr["IdViaTransp"].ToString();

            cn.Close();

            return IdViaTransp;
        }

        public static string BuscaIdAduana(string Aduana, string CodPais)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string IdAduana = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select IdAduana from Aduana_" + CodPais + " where Aduana = '" + Aduana + "' ";

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) IdAduana = dtr["IdAduana"].ToString();

            cn.Close();

            return IdAduana;
        }

        public static string BuscaIdMarca(string Marca, string CodPais)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string IdMarca = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select IdMarca from Marca_" + CodPais + " where Marca = '" + Marca + "' ";

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) IdMarca = dtr["IdMarca"].ToString();

            cn.Close();

            return IdMarca;
        }

        public static string BuscaIdDistrito(string Distrito, string CodPais)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string IdDistrito = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select IdDistrito from Distrito_" + CodPais + " where Distrito = '" + Distrito + "' ";

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) IdDistrito = dtr["IdDistrito"].ToString();

            cn.Close();

            return IdDistrito;
        }

    }
}