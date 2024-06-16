using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Configuration;

using System.Reflection;
using System.Web.SessionState;

namespace Veritrade2017.Helpers
{
    public static class Functions3
    {
        //public const string servidor = "VERINFO01"; // PRODUCCION
        //public const string servidor = "VERIDEV"; //  DESARROLLO
        public const string dominio = "www.veritrade.info"; //veritrade.dyndns.biz veritrade.info
        public const string base_datos = "VeritradeBusiness";
        //public const string cadena_conexion = "Data Source=" + servidor + "; Initial Catalog=" + base_datos + "; User ID=veritrade; Password=elgatofelix; MultipleActiveResultSets=True;";
        public const int reg_x_pag = 20;
        public const int res_x_pag = 40;
        public const int fav_x_pag = 20;

        //public const string ruta_descarga = "http://" + servidor + "/VeritradeBusinessDownloads/";
        //public const string directorio_descarga = "D:\\VeritradeBusinessDownloads\\";
        public const string ruta_descarga = "/VeritradeDownloads/";
        public const string ruta_winrar = "C:\\Program Files\\WinRAR\\winrar.exe";

        public const string ServidorEmail = "smtp.gmail.com";
        public const int ServidorEmailPuerto = 587;
        public const string EmailEnvio = "verinews@veritrade-ltd.com";
        public const string EmailEnvioNombre = "Informativo VERINEWS";

        public const int MaxFavoritosProcesoAuto = 3;

        public static string prepararParaSql(string cadena)
        {   //Función que retorna la cadena sin caracteres extranios
            //Evitar SQL Injection.
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

        public static string AvisoSuscripcion(string IdUsuario)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string IdTipo, FecFin;
            int auxDias, DiasAviso = 10, DiasAdicionales = 5;
            string auxAviso = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;

            try
            {
                cn.Open();
                sql = "select IdTipo, isnull(FecFin, 0) as FecFin from Usuario where IdUsuario = " + IdUsuario;

                cmd = new SqlCommand(sql, cn);
                dtr = cmd.ExecuteReader();
                dtr.Read();

                IdTipo = dtr["IdTipo"].ToString();
                FecFin = dtr["FecFin"].ToString();

                if (IdTipo != "49") // Gratis
                {
                    if (FecFin != "0") FecFin = FecFin.Substring(0, 4) + "-" + FecFin.Substring(4, 2) + "-" + FecFin.Substring(6, 2);

                    if (FecFin == "0")
                        auxDias = 365;
                    else
                        auxDias = (Convert.ToDateTime(FecFin) - DateTime.Now).Days;

                    if (IdTipo != "8") // No es Free Trial
                    {
                        if (auxDias <= DiasAviso && auxDias > 0)
                            auxAviso = "<span style='color:Red;background-color:Yellow;'><a href='mailto:info@veritrade-ltd.com' style='color:Red;'>Renew your contract</a>, it will expire in <b>" + auxDias.ToString() + "</b> day(s)</span>";
                        else if (auxDias == 0)
                            auxAviso = "<span style='color:Red;background-color:Yellow;'><a href='mailto:info@veritrade-ltd.com' style='color:Red;'>Renew your contract</a>, it will expire <b>today</b></span>";
                        else if (auxDias < 0 && auxDias > -1 * DiasAdicionales)
                            auxAviso = "<span style='color:White;background-color:Red;'>Your contract has expired. <a href='mailto:info@veritrade-ltd.com' style='color:White;'>Renew it now</a> or your user/password will be disabled in <b>" + (DiasAdicionales - auxDias).ToString() + "</b> day(s)</span>";
                        else if (auxDias <= -1 * DiasAdicionales)
                            auxAviso = "<span style='color:White;background-color:Red;'>Your contract has expired. <a href='mailto:info@veritrade-ltd.com' style='color:White;'>Renew it now</a> or your user/password will be disabled <b>today</b></span>";
                    }
                    else
                    {
                        if (auxDias > 0)
                            auxAviso = "<span style='color:Red;background-color:Yellow;'>This is a Free Trial version that will expire in <b>" + auxDias.ToString() + "</b> day(s)</span>";
                        else
                            auxAviso = "<span style='color:White;background-color:Red;'>This is a Free Trial version that expired and it will be disabled <b>today</b></span>";
                    }
                }
                dtr.Close();
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                string sMensaje = ex.Message;
                auxAviso = "";
            }
            finally
            {
                cn.Close();
            }

            return auxAviso;
        }

        //VARIABLE GLOBALES POR IDIOMA
        public static string sBusquedaAvanzada(string idioma)
        {
            switch (idioma)
            {
                case "Spanish": return "Búsqueda Avanzada";
                case "es": return "Búsqueda Avanzada";
                case "English": return "Advanced Search";
                case "en": return "Advanced Search";
                default: return ".";
            }
        }

        public static string sMiVeritrade(string idioma)
        {
            switch (idioma)
            {
                case "Spanish": return "Mi Veritrade";
                case "es": return "Mi Veritrade";
                case "English": return "My Veritrade";
                case "en": return "My Veritrade";
                default: return "";
            }
        }

        public static string sAgregaFavorito(string idioma)
        {
            switch (idioma)
            {
                case "Spanish": return "Agregar a la Búsqueda Avanzada";
                case "es": return "Agregar a la Búsqueda Avanzada";
                case "English": return "Add to Advanced Search";
                case "en": return "Add to Advanced Search";
                default: return "";
            }
        }

        public static string sStatus(string idioma)
        {
            switch (idioma)
            {
                case "Spanish": return "Este producto ya está en tus favoritos";
                case "es": return "Este producto ya está en tus favoritos";
                case "English": return "This product is already in your Favorites";
                case "en": return "This product is already in your Favorites";
                default: return "";
            }
        }

        public static string sBuscarProducto(string idioma)
        {
            switch (idioma)
            {
                case "Spanish": return "Buscar Producto";
                case "es": return "Buscar Producto";
                case "English": return "Find Product";
                case "en": return "Find Product";
                default: return ".";
            }
        }

        public static string sDescargarExcel(string idioma)
        {
            switch (idioma)
            {
                case "Spanish": return "Descargar Excel";
                case "es": return "Descargar Excel";
                case "English": return "Download Excel";
                case "en": return "Download Excel";
                default: return ".";
            }
        }

        public static string sReportes(string idioma)
        {
            switch (idioma)
            {
                case "Spanish": return "Reportes";
                case "es": return "Reportes";
                case "English": return "Reports";
                case "en": return "Reports";
                default: return ".";
            }
        }

        public static string sProductosFavoritos(string idioma)
        {
            switch (idioma)
            {
                case "Spanish": return "Productos Favoritos";
                case "es": return "Productos Favoritos";
                case "English": return "Favorite Products";
                case "en": return "Favorite Products";
                default: return ".";
            }
        }

        public static string sEncuentraImportador(string idioma)
        {
            switch (idioma)
            {
                case "Spanish": return "Encuentra Importador";
                case "es": return "Encuentra Importador";
                case "English": return "Find Importer";
                case "en": return "Find Importer";
                default: return ".";
            }
        }

        public static string sEncuentraExportador(string idioma)
        {
            switch (idioma)
            {
                case "Spanish": return "Encuentra Exportador";
                case "es": return "Encuentra Exportador";
                case "English": return "Find Exporter";
                case "en": return "Find Exporter";
                default: return ".";
            }
        }

        public static string sImportadoresFavoritos(string idioma)
        {
            switch (idioma)
            {
                case "Spanish": return "Importadores Favoritos";
                case "es": return "Importadores Favoritos";
                case "English": return "Favorite Importers";
                case "en": return "Favorite Importers";
                default: return ".";
            }
        }

        public static string sExportadoresFavoritos(string idioma)
        {
            switch (idioma)
            {
                case "Spanish": return "Exportadores Favoritos";
                case "es": return "Exportadores Favoritos";
                case "English": return "Favorite Exporters";
                case "en": return "Favorite Exporters";
                default: return ".";
            }
        }

        public static string sResumenEjecutivo(string idioma)
        {
            switch (idioma)
            {
                case "Spanish": return "Resumen Ejecutivo";
                case "es": return "Resumen Ejecutivo";
                case "English": return "Executive Summary";
                case "en": return "Executive Summary";
                default: return ".";
            }
        }

        public static string sSituacionFinanciera(string idioma)
        {
            switch (idioma)
            {
                case "Spanish": return "Situación Financiera";
                case "es": return "Situación Financiera";
                case "English": return "Financial Situation";
                case "en": return "Financial Situation";
                default: return ".";
            }
        }

        //*********************************************************************************************
        //FUNCION DESCOMENTADA PARA VALIDAR VARIOS USUARIOS EN LAS PRUEBAS DE CHRISTIAN
        //*********************************************************************************************
        public static bool Valida(string CodUsuario, string Password, ref string IdUsuario)
        {   //Funcion que autentifica a un usuario
            //Parametros: 
            //string CodUsuario: Codigo del usuario
            //string Password: Contrasenia
            //ref string IdUsuario: Se retorna el ID del usuario en la Variable
            //Return: Verdadero o Falso
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            IdUsuario = "";
            CodUsuario = prepararParaSql(CodUsuario);
            Password = prepararParaSql(Password);
            sql = "select IdUsuario from Usuario where CodUsuario = '" + CodUsuario + "' and Password = '" + Password + "' and CodEstado = 'A'";
            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            dtr.Read();
            if (dtr.HasRows) IdUsuario = dtr["IdUsuario"].ToString();
            dtr.Close();
            if (IdUsuario != "") return true; else return false;
        }
        //*********************************************************************************************

        //public static void GrabaHistorial2(string IdUsuario, string DireccionIP, string Navegador)
        //{
        //    string sql;
        //    SqlConnection cn;
        //    SqlCommand cmd;

        //    cn = new SqlConnection();
        //    cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
        //    cn.Open();

        //    sql = "insert into Historial(IdUsuario, DireccionIP, Navegador, FecVisita) ";
        //    sql += "values (" + IdUsuario + ", '" + DireccionIP + "', '" + Navegador + "', getdate())";
        //    cmd = new SqlCommand(sql, cn);
        //    cmd.ExecuteNonQuery();
        //    cn.Close();
        //}

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
        {   //Función que verifica si el usuario esta suscrito al pais
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

            sql = "select count(*) as Cant from Suscripcion S, BaseDatos B ";
            sql += "where (S.CodPais = B.CodPaisSuscripcion or S.CodPais = B.CodPais) ";
            sql += "and IdUsuario = " + IdUsuario + " and B.CodPais = '" + CodPais + "' ";
            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            dtr.Read();
            Cant = (int)dtr["Cant"];
            dtr.Close();
            if (Cant > 0) return true; else return false;
        }

        public static string Pais(string CodPais, string Idioma)
        {   //Retorna el nombre del pais
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
                if (Idioma == "es") Pais = dtr["PaisES"].ToString().ToUpper(); else Pais = dtr["Pais"].ToString().ToUpper();

            cn.Close();

            return Pais;
        }

        public static string TipoOpe(string TipoOpe1, string Idioma)
        {   //Retorna el nombre del tipo de operacion
            //Parametros: 
            //string TipoOpe1: identificador 
            string aux = "";
            if (Idioma == "es")
            {
                if (TipoOpe1 == "I") aux = "Importaciones";
                if (TipoOpe1 == "E") aux = "Exportaciones";
            }
            else
            {
                if (TipoOpe1 == "I") aux = "Imports";
                if (TipoOpe1 == "E") aux = "Exports";
            }
            return aux;
        }

        public static string Cambia(string Palabra, string CodPais)
        {   //Retorna el nombre en el idioma del pais
            //Parametros: 
            //string Palabra: en castellano 
            //string CodPais: identificador 
            string aux = "";

            string Idioma = "en";

            if (Idioma == "es")
            {
                if (Palabra == "Partida") aux = "Producto";
                if (Palabra == "Importador") aux = "Importador";
                if (Palabra == "Proveedor")
                    if (CodPais != "CL") aux = "Exportador"; else aux = "Marca";
                if (Palabra == "Distrito") aux = "Distrito";
                if (Palabra == "PaisOrigen") aux = "Origen";
                if (Palabra == "Exportador") aux = "Exportador";
                if (Palabra == "ImportadorExp") aux = "Importador";
                if (Palabra == "PaisDestino") aux = "Destino";
            }
            else
            {
                if (Palabra == "Partida") aux = "Product";
                if (Palabra == "Importador") aux = "Importer";
                if (Palabra == "Proveedor")
                    if (CodPais != "CL") aux = "Exporter"; else aux = "Brand";
                if (Palabra == "Distrito") aux = "District";
                if (Palabra == "PaisOrigen") aux = "Origin";
                if (Palabra == "Exportador") aux = "Exporter";
                if (Palabra == "ImportadorExp") aux = "Importer";
                if (Palabra == "PaisDestino") aux = "Destination";
            }

            return aux;
        }

        public static void Rango(string CodPais, string TipoOpe, ref string AñoIni, ref string MesIni, ref string AñoFin, ref string MesFin)
        {   //Retorna el rango de fecha de operación de un pais en las variables por referencia
            //Parametros: 
            //string CodPais: SIGLA internacional de un pais
            //string TipoOpe: identificador de operacion
            //ref string AñoIni 
            //ref string MesIni 
            //ref string AñoFin 
            //ref string MesFin
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string FechaIni, FechaFin;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();
            sql = "select FechaIni, FechaFin from BaseDatos where CodPais ='" + CodPais + "' and TipoOpe = '" + TipoOpe + "' ";
            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read())
            {
                FechaIni = dtr["FechaIni"].ToString();
                AñoIni = FechaIni.Substring(0, 4);
                MesIni = FechaIni.Substring(4, 2);
                FechaFin = dtr["FechaFin"].ToString();
                AñoFin = FechaFin.Substring(0, 4);
                MesFin = FechaFin.Substring(4, 2);
            }
            dtr.Close();
            cn.Close();
        }

        public static void Rango(string CodPais, string TipoOpe, ref string AñoMesIni, ref string AñoMesFin)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();
            sql = "select FechaIni, FechaFin from BaseDatos where CodPais ='" + CodPais + "' and TipoOpe = '" + TipoOpe + "' ";
            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read())
            {
                AñoMesIni = dtr["FechaIni"].ToString();
                AñoMesFin = dtr["FechaFin"].ToString();
            }
            dtr.Close();
            cn.Close();
        }

        public static string OnlineInformation(string CodPais, string TipoOpe)
        {   //Retorna el intervalo de fecha "Online Information from" un pais
            //Parametros: 
            //string CodPais: SIGLA internacional de un pais
            //string TipoOpe: identificador de operacion
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string FechaIni, FechaFin;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();
            sql = "select FechaIni, FechaFin from BaseDatos where CodPais ='" + CodPais + "' and TipoOpe = '" + TipoOpe + "' ";
            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            dtr.Read();
            FechaIni = dtr["FechaIni"].ToString();
            FechaIni = FechaIni.Substring(0, 4) + "-" + FechaIni.Substring(4, 2) + "-" + FechaIni.Substring(6, 2);
            FechaIni = string.Format("{0:MMM dd, yyyy}", Convert.ToDateTime(FechaIni));
            FechaFin = dtr["FechaFin"].ToString();
            FechaFin = FechaFin.Substring(0, 4) + "-" + FechaFin.Substring(4, 2) + "-" + FechaFin.Substring(6, 2);
            FechaFin = string.Format("{0:MMM dd, yyyy}", Convert.ToDateTime(FechaFin));
            cn.Close();
            return "Online Information from " + FechaIni + " to " + FechaFin;
        }

        public static bool ExisteVariable(string CodPais, string TipoOpe, string Variable1)
        {   //Retorna 
            //Parametros: 
            //string CodPais: SIGLA internacional de un pais
            //string TipoOpe: identificador de operacion
            //string Variable1
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            bool aux;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();
            sql = "select distinct codpais, tipoope, variable1 from VariableAgrupTest where CodPais = '" + CodPais + "' and TipoOpe = '" + TipoOpe + "' ";
            sql += "and Variable1 = '" + Variable1 + "' ";
            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            dtr.Read();
            aux = dtr.HasRows;
            cn.Close();
            return aux;
        }

        public static void VariablesAgrupacion(string CodPais, string TipoOpe, string Variable1, string AgrupaPor, ref string AgrupaPor2, ref string AgrupaPor3)
        {   //Retorna 
            //Parametros: 
            //string CodPais: SIGLA internacional de un pais
            //string TipoOpe: identificador de operacion
            //string Variable1
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();
            sql = "select AgrupaPor2, AgrupaPor3 from VariableAgrupTest where CodPais = '" + CodPais + "' and TipoOpe = '" + TipoOpe + "' ";
            sql += "and Variable1 = '" + Variable1 + "' and AgrupaPor = '" + AgrupaPor + "' ";
            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            dtr.Read();
            AgrupaPor2 = dtr["AgrupaPor2"].ToString();
            AgrupaPor3 = dtr["AgrupaPor3"].ToString();
            cn.Close();
        }

        public static string BuscaUsuario(string IdUsuario)
        {   //Retorna el nombre completo de un usuario
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
                sql = "select Nombres + ' ' + Apellidos + ' - ' + Empresa as Usuario from Usuario where IdUsuario = " + IdUsuario;
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

        public static string BuscaCodUsuario(string IdUsuario)
        {   //Retorna el codigo de un usuario
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

        public static string BuscarIdUsuario(string CodUsuario, string IdAplicacion)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string IdUsuario = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select IdUsuario from Usuario where CodUsuario = '" + CodUsuario + "' and IdAplicacion = " + IdAplicacion;

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) IdUsuario = dtr["IdUsuario"].ToString();
            dtr.Close();

            cn.Close();

            return IdUsuario;
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

        /*
        public static string BuscaIdPartida(string Nandina, string CodPais)
        {   //Retorna el identificador de la partida
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
        */

        public static string BuscaPartida(string IdPartida, string CodPais)
        {   //Retorna el nombre de la partida
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
            sql = "select Nandina + ' ' + Partida as Partida from Partida_" + CodPais + " where IdPartida = " + IdPartida;
            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) Partida = dtr["Partida"].ToString();
            cn.Close();
            return Partida;
        }

        /*
        public static string BuscaSubCapitulo(string CodSubCapitulo)
        {   
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string SubCapitulo = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();
            sql = "select CodSubCapitulo + ' ' + SubCapitulo as SubCapitulo from SubCapitulo where CodSubCapitulo = '" + CodSubCapitulo + "' ";
            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) SubCapitulo = dtr["SubCapitulo"].ToString();
            cn.Close();
            return SubCapitulo;
        }
        */

        public static string BuscaEmpresa(string IdEmpresa, string CodPais)
        {   //Retorna el nombre de la empresa
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

            sql = "select Empresa from Empresa_" + CodPais + " where IdEmpresa = " + IdEmpresa;
            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) Empresa = dtr["Empresa"].ToString();
            cn.Close();

            return Empresa;
        }

        public static string BuscaIdEmpresa(string CodPais, string RUC)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string IdEmpresa = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select IdEmpresa from Empresa_" + CodPais + " where RUC = '" + RUC + "'";

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) IdEmpresa = dtr["IdEmpresa"].ToString();
            dtr.Close();

            cn.Close();

            return IdEmpresa;
        }

        public static string BuscaRUC(string IdEmpresa, string CodPais)
        {   //Retorna el numero de RUC
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

        public static string BuscaProveedor(string IdProveedor, string CodPais)
        {   //Retorna el nombre del Proveedor
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

        public static string BuscaDistrito(string IdDistrito, string CodPais)
        {   //Retorna el nombre del Distrito
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

        public static string BuscaImportadorExp(string IdImportadorExp, string CodPais)
        {   //Retorna el nombre del Imporator/Exportador
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
        {   //Retorna el nombre del pais
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

        public static string BuscaVia(string IdViaTransp, string CodPais)
        {   //Retorna 
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
        {   //Retorna 
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
        {   //Se usa?
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string AñoMes = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();
            if (IdAñoMes.Length == 8) IdAñoMes = IdAñoMes.Substring(0, 6);
            sql = "select AñoMes from AñoMes where IdAñoMes = " + IdAñoMes;
            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) AñoMes = dtr["AñoMes"].ToString();
            cn.Close();
            return AñoMes;
        }

        public static string BuscaGrupo(string IdGrupo)
        {   //Retorna el nombre del grupo
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
        {   //Retorna el nombre del identificador de la variable
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

        public static string BuscaVariable(string Variable, string TipoIdVariable, string CodPais)
        {   //Retorna el nombre del identificador de la variable
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

        public static string ListaTitulosDescarga(string IdDescargaCab, string Campos, string CodPais = "", string TipoOpe = "")
        {   //Retorna 
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
                sql = "select CampoFav from DescargaDet where IdDescargaCab in (select IdDescargaCab from DescargaCab where IdUsuario = 0 and CodPais = '" + CodPais + "' and TipoOpe = '" + TipoOpe + "') ";

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

        public static string ListaCamposDescarga(string IdDescargaCab, string Campos, string CodPais = "", string TipoOpe = "")
        {   //Retorna 
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
                sql = "select Campo from DescargaDet where IdDescargaCab in (select IdDescargaCab from DescargaCab where IdUsuario = 0 and CodPais = '" + CodPais + "' and TipoOpe = '" + TipoOpe + "') ";

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

        public static string ProcesaDescarga(string Accion, string IdDescargaCab, string Descarga, string IdUsuario, string CodPais, string TipoOpe)
        {   //Retorna 
            //Parametros: 
            //string IdDescargaCab:
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();
            switch (Accion)
            {
                case "Crea":
                    sql = "select max(IdDescargaCab) + 1 from DescargaCab where IdDescargaCab > 100";
                    cmd = new SqlCommand(sql, cn);
                    dtr = cmd.ExecuteReader();
                    dtr.Read();
                    if (dtr[0].ToString() != "") IdDescargaCab = dtr[0].ToString(); else IdDescargaCab = "101";
                    dtr.Close();
                    sql = "insert into DescargaCab(IdDescargaCab, IdUsuario, CodPais, TipoOpe, Descarga) ";
                    sql += "values (" + IdDescargaCab + "," + IdUsuario + ", '" + CodPais + "','" + TipoOpe + "', '" + Descarga + "')";
                    cmd = new SqlCommand(sql, cn);
                    cmd.ExecuteScalar();
                    break;
                case "Actualiza":
                    sql = "update DescargaCab set Descarga = '" + Descarga + "' where IdDescargaCab = " + IdDescargaCab;
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
        {   //Retorna 
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

            sql = "select count(*) from DescargaDet where IdDescargaCab = " + IdDescargaCab + " and Campo = '" + Campo + "' ";
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
                sql = "update DescargaDet set CampoFav = '" + CampoFav + "' where IdDescargaCab = " + IdDescargaCab + " and Campo = '" + Campo + "' ";
                cmd = new SqlCommand(sql, cn);
                cmd.ExecuteScalar();
            }
            cn.Close();
        }

        public static void EliminaCampoDescarga(string IdDescargaCab, string Campo)
        {   //Retorna 
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

            sql = "select count(*) from DescargaDet where IdDescargaCab = " + IdDescargaCab + " and Campo = '" + Campo + "' ";
            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            dtr.Read();
            cantidad = (int)dtr[0];
            dtr.Close();
            if (cantidad > 0)
            {
                sql = "delete from DescargaDet where IdDescargaCab = " + IdDescargaCab + " and Campo = '" + Campo + "' ";
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

        public static bool ExisteFavorito(string TipoFavorito, string IdUsuario, string CodPais, string TipoOpe, string IdFavorito)
        {   //Retorna Verdadero si existen favoritos almacenados para los parametros
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
                    sql = "select count(*) as cant from PartidaFav where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' ";
                    sql += "and TipoOpe = '" + TipoOpe + "' and IdPartida = " + IdFavorito + " ";
                    break;
                case "Importador":
                    sql = "select count(*) as cant from EmpresaFav where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' ";
                    sql += "and TipoOpe = 'I' and IdEmpresa = " + IdFavorito + " ";
                    break;
                case "Proveedor":
                    sql = "select count(*) as cant from ProveedorFav where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' ";
                    sql += "and IdProveedor = " + IdFavorito + " ";
                    break;
                case "Exportador":
                    sql = "select count(*) as cant from EmpresaFav where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' ";
                    sql += "and TipoOpe = 'E' and IdEmpresa = " + IdFavorito + " ";
                    break;
                case "ImportadorExp":
                    sql = "select count(*) as cant from ImportadorExpFav where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' ";
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
                    sql = "select count(*) as Cant from PartidaFav where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' ";
                    sql += "and TipoOpe = '" + TipoOpe + "' ";
                    break;
                case "Importador":
                    sql = "select count(*) as Cant from EmpresaFav where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' ";
                    sql += "and TipoOpe = 'I' ";
                    break;
                case "Proveedor":
                    sql = "select count(*) as Cant from ProveedorFav where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' ";
                    break;
                case "Exportador":
                    sql = "select count(*) as Cant from EmpresaFav where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' ";
                    sql += "and TipoOpe = 'E' ";
                    break;
                case "ImportadorExp":
                    sql = "select count(*) as Cant from ImportadorExpFav where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' ";
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

        public static void AgregaFavorito(string TipoFavorito, string IdUsuario, string CodPais, string TipoOpe, string IdFavorito)
        {   //Inserta un nuevo favorito en TipoFavorito
            //Parametros: 
            //string TipoFavorito: tabla
            //string IdUsuario: identificador del usuario
            //string CodPais: identificador de la tabla pais
            //string TipoOpe: Importación / Exportación 
            //string IdFavorito: Identificador de la tabla TipoFavorito            
            string sql = "";
            SqlConnection cn;
            SqlCommand cmd;
            string PartidaFav;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            switch (TipoFavorito)
            {
                case "Partida":
                    PartidaFav = BuscaPartida(IdFavorito, CodPais);
                    if (PartidaFav.Length > 60) PartidaFav = PartidaFav.Substring(0, 60);
                    sql = "insert into PartidaFav(IdUsuario, CodPais, TipoOpe, IdPartida, PartidaFav) values (" + IdUsuario + ", '" + CodPais + "', '" + TipoOpe + "', " + IdFavorito + ", '" + PartidaFav.Replace("'", "''") + "')";
                    break;
                case "Importador":
                    sql = "insert into EmpresaFav(IdUsuario, CodPais, TipoOpe, IdEmpresa) values (" + IdUsuario + ", '" + CodPais + "', 'I'," + IdFavorito + ")";
                    break;
                case "Proveedor":
                    sql = "insert into ProveedorFav(IdUsuario, CodPais, TipoOpe, IdProveedor) values (" + IdUsuario + ", '" + CodPais + "', 'I', " + IdFavorito + ")";
                    break;
                case "Exportador":
                    sql = "insert into EmpresaFav(IdUsuario, CodPais, TipoOpe, IdEmpresa) values (" + IdUsuario + ", '" + CodPais + "', 'E', " + IdFavorito + ")";
                    break;
                case "ImportadorExp":
                    sql = "insert into ImportadorExpFav(IdUsuario, CodPais, TipoOpe, IdImportadorExp) values (" + IdUsuario + ", '" + CodPais + "', 'E', " + IdFavorito + ")";
                    break;
            }

            cmd = new SqlCommand(sql, cn);
            cmd.ExecuteScalar();

            cn.Close();
        }

        public static string BuscaFavorito(string TipoFavorito, string IdUsuario, string CodPais, string TipoOpe, string IdFavorito)
        {   //Retorna el Identificador de: Partida, Empresa, Proveedor, ImportadorExp
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

        public static void ActualizaPartidaFavorita(string IdUsuario, string CodPais, string TipoOpe, string IdPartida, string PartidaFav)
        {   //Actualiza un registro en Partida_favorito
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

            sql = "update PartidaFav set PartidaFav = @PartidaFav where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' and TipoOpe = '" + TipoOpe + "' and IdPartida = " + IdPartida;
            cmd = new SqlCommand(sql, cn);
            cmd.Parameters.Add(param);
            cmd.ExecuteNonQuery();
            cn.Close();
        }

        public static void EliminaFavorito(string TipoFavorito, string IdUsuario, string CodPais, string TipoOpe, string IdFavorito)
        {   //Elimina un registro favorito en TipoFavorito
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
                    sql = "delete from PartidaFav where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' and TipoOpe = '" + TipoOpe + "' and IdPartida = " + IdFavorito;
                    break;
                case "Importador":
                    sql = "delete from EmpresaFav where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' and TipoOpe = 'I' and IdEmpresa = " + IdFavorito;
                    break;
                case "Proveedor":
                    sql = "delete from ProveedorFav where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' and IdProveedor = " + IdFavorito;
                    break;
                case "Exportador":
                    sql = "delete from EmpresaFav where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' and TipoOpe = 'E' and IdEmpresa = " + IdFavorito;
                    break;
                case "ImportadorExp":
                    sql = "delete from ImportadorExpFav where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' and IdImportadorExp = " + IdFavorito;
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

            sql = "delete from PartidaFav where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' and TipoOpe = '" + TipoOpe + "' ";
            if (ExisteVariable(CodPais, TipoOpe, "IdImportador"))
                sql += "delete from EmpresaFav where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' and TipoOpe = 'I' ";
            if (ExisteVariable(CodPais, TipoOpe, "IdProveedor"))
                sql += "delete from ProveedorFav where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' ";
            if (ExisteVariable(CodPais, TipoOpe, "IdExportador"))
                sql += "delete from EmpresaFav where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' and TipoOpe = 'E' ";
            if (ExisteVariable(CodPais, TipoOpe, "IdImportadorExp"))
                sql += "delete from ImportadorExpFav where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' ";

            cmd = new SqlCommand(sql, cn);
            cmd.ExecuteNonQuery();

            cn.Close();
        }

        public static int CantFavUnicos(string IdUsuario, string CodPais, string TipoOpe, string TipoFavorito)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;

            int FavUnicos = 0;

            string TipoFav;
            if (TipoFavorito != "ImportadorExp") TipoFav = TipoFavorito.Substring(0, 2).ToUpper(); else TipoFav = "IE";

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

        public static DataView LlenaGrupos(bool flagFiltro, string IdUsuario, string CodPais, string TipoOpe, string TipoFavorito)
        {
            string sql = "";
            SqlConnection cn;
            SqlCommand cmd;
            DataSet dtaset;
            SqlDataAdapter dtadap;

            DataView Grupos;

            string TipoFav;
            if (TipoFavorito != "ImportadorExp") TipoFav = TipoFavorito.Substring(0, 2); else TipoFav = "IE";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            if (flagFiltro)
            {
                sql = "select 0 as Orden, -1 as IdGrupo, '[TODOS]' as Grupo union select 1 as Orden, 0 as IdGrupo, 'INDIVIDUAL' as Grupo ";
                sql += "union select 2 as Orden, IdGrupo, '[G] ' + Grupo as Grupo from Grupo ";
            }
            else
                sql = "select 2 as Orden, IdGrupo, Grupo from Grupo ";
            sql += "where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' and TipoOpe = '" + TipoOpe + "' ";
            sql += "and TipoFav = '" + TipoFav + "' ";
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

        public static string CreaGrupo(string Grupo, string IdUsuario, string CodPais, string TipoOpe, string TipoFav)
        {   //Inserta un nuevo grupo
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

            sql = "insert into Grupo(IdUsuario, CodPais, TipoOpe, TipoFav, Grupo) values (" + IdUsuario + ", '" + CodPais + "','" + TipoOpe + "', '" + TipoFav + "', '" + Grupo + "')";
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
        {   //Modifica el nombre del grupo
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

            sql = "select count(*) as Cant from FavoritoGrupo where IdGrupo = " + IdGrupo + " and IdFavorito = " + IdFavorito + " ";
            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            dtr.Read();
            Cant = Convert.ToInt32(dtr["Cant"]);
            dtr.Close();

            cn.Close();

            return (Cant > 0);
        }

        public static void AgregaFavoritoAGrupo(string IdGrupo, string IdFavorito)
        {   //Agrega un elemento al grupo
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

            sql = "select count(*) as Cant from FavoritoGrupo where IdGrupo = " + IdGrupo + " and IdFavorito = " + IdFavorito + " ";
            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            dtr.Read();
            Cant = Convert.ToInt32(dtr["Cant"]);
            dtr.Close();
            if (Cant == 0)
            {
                sql = "insert into FavoritoGrupo(IdGrupo, IdFavorito) values (" + IdGrupo + ", " + IdFavorito + ")";
                cmd = new SqlCommand(sql, cn);
                cmd.ExecuteScalar();
            }

            cn.Close();
        }

        public static void EliminaFavoritoDeGrupo(string IdGrupo, string IdFavorito)
        {   //Elimina un elemento del grupo
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

        public static void EliminaGrupo(string IdGrupo)
        {   //Elimina el grupo
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
            if (TipoFavorito != "ImportadorExp") TipoFav = TipoFavorito.Substring(0, 2).ToUpper(); else TipoFav = "IE";

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

        public static int CantFavoritosGrupo(string IdGrupo)
        {   //Retrona el numero de registros del grupo
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

        public static DataView LlenaGruposFavoritos(string IdUsuario, string CodPais, string TipoOpe, string TipoFavorito)
        {
            String sql = "";
            SqlConnection cn;
            SqlCommand cmd;
            DataSet dtaset;
            SqlDataAdapter dtadap;

            DataView GruposFavoritos;

            string TipoFav;
            if (TipoFavorito != "ImportadorExp") TipoFav = TipoFavorito.Substring(0, 2); else TipoFav = "IE";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();
            switch (TipoFavorito)
            {
                case "Partida":
                    sql = "select IdPartida as IdFavorito, 1 as Orden, 0 as IdGrupo, 'INDIVIDUAL' as Grupo from PartidaFav ";
                    sql += "where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' and TipoOpe = '" + TipoOpe + "'";
                    break;
                case "Importador":
                    sql = "select IdEmpresa as IdFavorito, 1 as Orden, 0 as IdGrupo, 'INDIVIDUAL' as Grupo from EmpresaFav ";
                    sql += "where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' and TipoOpe = 'I'";
                    break;
                case "Proveedor":
                    sql = "select IdProveedor as IdFavorito, 1 as Orden, 0 as IdGrupo, 'INDIVIDUAL' as Grupo from ProveedorFav ";
                    sql += "where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "'";
                    break;
                case "Exportador":
                    sql = "select IdEmpresa as IdFavorito, 1 as Orden, 0 as IdGrupo, 'INDIVIDUAL' as Grupo from EmpresaFav ";
                    sql += "where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' and TipoOpe = 'E'";
                    break;
                case "ImportadorExp":
                    sql = "select IdImportadorExp as IdFavorito, 1 as Orden, 0 as IdGrupo, 'INDIVIDUAL' as Grupo from ImportadorExpFav ";
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

        public static string ListaFavoritosGrupo(string TipoIdFavorito)
        {   //Retorna una lista con todos los registros del TipoIdFavorito
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

        public static ArrayList GuardaSeleccionados(GridView grid, ArrayList IDsSeleccionados)
        {
            string ID;

            if (IDsSeleccionados == null) IDsSeleccionados = new ArrayList();
            foreach (GridViewRow row in grid.Rows)
            {
                ID = grid.DataKeys[row.RowIndex].Value.ToString();
                if (ID.Substring(0, 1) == "F" || ID.Substring(0, 1) == "G")
                    ID = ID.Substring(1, ID.Length - 1);
                bool FlagSeleccionado = ((CheckBox)row.FindControl("chkSel")).Checked;
                if (FlagSeleccionado)
                {
                    if (!IDsSeleccionados.Contains(ID)) IDsSeleccionados.Add(ID);
                }
                else
                    IDsSeleccionados.Remove(ID);
            }
            return IDsSeleccionados;
        }

        public static void RecuperaSeleccionados(ref GridView grid, ArrayList IDsSeleccionados)
        {
            string ID;
            if (IDsSeleccionados != null && IDsSeleccionados.Count > 0)
            {
                foreach (GridViewRow row in grid.Rows)
                {
                    ID = grid.DataKeys[row.RowIndex].Value.ToString();
                    ID = ID.Substring(1, ID.Length - 1);
                    if (IDsSeleccionados.Contains(ID))
                    {
                        CheckBox chkSel = (CheckBox)row.FindControl("chkSel");
                        chkSel.Checked = true;
                    }
                }
            }
        }

        public static ArrayList GuardaSeleccionados2(GridView grid, ArrayList IDsSeleccionados)
        {
            string ID;

            if (IDsSeleccionados == null) IDsSeleccionados = new ArrayList();

            foreach (GridViewRow row in grid.Rows)
            {
                GridView gdvGrupos = (GridView)row.FindControl("gdvGrupos");
                foreach (GridViewRow row2 in gdvGrupos.Rows)
                {
                    ID = gdvGrupos.DataKeys[row2.RowIndex].Values["IdFavorito"].ToString() + "-";
                    ID += gdvGrupos.DataKeys[row2.RowIndex].Values["IdGrupo"].ToString();
                    bool FlagSeleccionado = ((CheckBox)row2.FindControl("chkSel")).Checked;
                    if (FlagSeleccionado)
                    {
                        if (!IDsSeleccionados.Contains(ID)) IDsSeleccionados.Add(ID);
                    }
                    else
                        IDsSeleccionados.Remove(ID);
                }
            }
            return IDsSeleccionados;
        }

        public static void RecuperaSeleccionados2(ref GridView grid, ArrayList IDsSeleccionados)
        {
            string ID;

            if (IDsSeleccionados != null && IDsSeleccionados.Count > 0)
                foreach (GridViewRow row in grid.Rows)
                {
                    GridView gdvGrupos = (GridView)row.FindControl("gdvGrupos");
                    foreach (GridViewRow row2 in gdvGrupos.Rows)
                    {
                        ID = gdvGrupos.DataKeys[row2.RowIndex].Values["IdFavorito"].ToString() + "-";
                        ID += gdvGrupos.DataKeys[row2.RowIndex].Values["IdGrupo"].ToString();
                        if (IDsSeleccionados.Contains(ID))
                        {
                            CheckBox chkSel = (CheckBox)row2.FindControl("chkSel");
                            chkSel.Checked = true;
                        }
                    }
                }
        }

        //public static ArrayList GuardaSeleccionadosResume(GridView grid, ArrayList IDsSeleccionados)
        //{
        //    string ID;

        //    if (IDsSeleccionados == null) IDsSeleccionados = new ArrayList();
        //    foreach (GridViewRow row in grid.Rows)
        //    {
        //        ID = ((HiddenField)row.FindControl("hdfId")).Value;
        //        if (ID != "") ID = ID.Substring(1, ID.Length - 1);
        //        bool FlagSeleccionado = ((CheckBox)row.FindControl("chkSel")).Checked;
        //        if (FlagSeleccionado)
        //        {
        //            if (!IDsSeleccionados.Contains(ID)) IDsSeleccionados.Add(ID);
        //        }
        //        else
        //            IDsSeleccionados.Remove(ID);
        //    }
        //    return IDsSeleccionados;
        //}

        //public static void RecuperaSeleccionadosResume(ref GridView grid, ArrayList IDsSeleccionados)
        //{
        //    string ID;
        //    if (IDsSeleccionados != null && IDsSeleccionados.Count > 0)
        //    {
        //        foreach (GridViewRow row in grid.Rows)
        //        {
        //            ID = ((HiddenField)row.FindControl("hdfId")).Value;
        //            if (ID != "") ID = ID.Substring(1, ID.Length - 1);
        //            if (IDsSeleccionados.Contains(ID))
        //            {
        //                CheckBox chkSel = (CheckBox)row.FindControl("chkSel");
        //                chkSel.Checked = true;
        //            }
        //        }
        //    }
        //}

        public static int GrabaFavoritos(ArrayList IDsSeleccionados, string TipoFavorito, string IdUsuario, string CodPais, string TipoOpe, ref bool flagLimFavUnicos)
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

        public static void EliminaFavoritos(ArrayList IDsSeleccionados, string TipoFavorito, string IdUsuario, string CodPais, string TipoOpe)
        {
            if (IDsSeleccionados != null && IDsSeleccionados.Count > 0)
                for (int i = 0; i < IDsSeleccionados.Count; i++)
                    EliminaFavorito(TipoFavorito, IdUsuario, CodPais, TipoOpe, (IDsSeleccionados[i]).ToString());
        }

        public static int EliminaFavoritos2(ArrayList IDsSeleccionados, string TipoFavorito, string IdUsuario, string CodPais, string TipoOpe)
        {
            string IdFavorito, IdGrupo;
            int Cant = 0;

            if (IDsSeleccionados != null && IDsSeleccionados.Count > 0)
                for (int i = 0; i < IDsSeleccionados.Count; i++)
                {
                    string ID = (IDsSeleccionados[i]).ToString();
                    if (ID.Contains("-0"))
                    {
                        IdFavorito = ID.Substring(0, ID.IndexOf("-0"));
                        EliminaFavorito(TipoFavorito, IdUsuario, CodPais, TipoOpe, ID);
                        Cant++;
                    }
                    else
                    {
                        IdFavorito = ID.Substring(0, ID.IndexOf("-"));
                        IdGrupo = ID.Substring(ID.IndexOf("-") + 1, ID.Length - ID.IndexOf("-") - 1);
                        if (CantFavoritosGrupo(IdGrupo) > 1)
                        {
                            EliminaFavoritoDeGrupo(IdGrupo, IdFavorito);
                            Cant++;
                        }
                    }
                }

            return Cant;
        }

        public static int GrabaFavoritosAGrupo(string IdUsuario, ArrayList IDsSeleccionados, string IdGrupo, ref bool flagLimFavPorGrupo)
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

        public static int GrabaFavoritosAGrupo2(string IdUsuario, string Aplicacion, ArrayList IDsSeleccionados, string IdGrupo)
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

        public static void EnviaEmail(string FromName, string FromEmail, string ToName, string ToEmail, string Subject, string URL)
        {
            System.Net.Mail.MailAddress From, To;

            From = new System.Net.Mail.MailAddress(FromEmail, FromName);
            To = new System.Net.Mail.MailAddress(ToEmail, ToName);

            //(1) Create the MailMessage instance
            System.Net.Mail.MailMessage mm;
            mm = new System.Net.Mail.MailMessage(From, To);
            mm.IsBodyHtml = true;

            //(2) Assign the MailMessage's properties
            mm.Subject = Subject;
            // Converting URL to HTML
            System.Net.WebRequest objRequest = System.Net.HttpWebRequest.Create(URL);
            System.IO.StreamReader sr = new System.IO.StreamReader(objRequest.GetResponse().GetResponseStream());
            string html = sr.ReadToEnd();
            sr.Close();

            mm.Body = html;

            //(3) Create the SmtpClient object
            System.Net.Mail.SmtpClient smtp;
            smtp = new System.Net.Mail.SmtpClient(ServidorEmail);
            smtp.Port = ServidorEmailPuerto;
            //smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential(EmailEnvio, Resources.Resources.EmailEnvioPassword);

            //(4) Send the MailMessage (will use the Web.config settings)
            smtp.Send(mm);

        }

        public static String _GetPostJS(string URL, string IdUsuario)
        {
            System.Text.StringBuilder strScript = new System.Text.StringBuilder();
            strScript.Append("<script language='javascript'>");
            strScript.Append("document.forms[0].method = 'post';");
            strScript.Append("document.forms[0].action = '{0}';");
            strScript.Append("document.forms[0].IdUsuario.value = '{1}';");
            strScript.Append("document.forms[0].inicio.value = 'ok';");
            strScript.Append("document.forms[0].submit();");
            strScript.Append("</script>");
            return String.Format(strScript.ToString(), URL, IdUsuario);
        }

        public static String _GetPostJS(string URL, string IdUsuario, string CodPais, string TipoOpe)
        {
            System.Text.StringBuilder strScript = new System.Text.StringBuilder();
            strScript.Append("<script language='javascript'>");
            strScript.Append("document.forms[0].method = 'post';");
            strScript.Append("document.forms[0].action = '{0}';");
            strScript.Append("document.forms[0].IdUsuario.value = '{1}';");
            strScript.Append("document.forms[0].inicio.value = 'ok';");
            strScript.Append("document.forms[0].CodPais.value = '{2}';");
            strScript.Append("document.forms[0].TipoOpe.value = '{3}';");
            strScript.Append("document.forms[0].submit();");
            strScript.Append("</script>");
            return String.Format(strScript.ToString(), URL, IdUsuario, CodPais, TipoOpe);
        }

        public static String _GetPostJS(string URL, string inicio, string origen, string CodUsuario, string Password)
        {
            System.Text.StringBuilder strScript = new System.Text.StringBuilder();
            strScript.Append("<script language='javascript'>");
            strScript.Append("document.forms[0].method = 'post';");
            strScript.Append("document.forms[0].action = '{0}';");
            strScript.Append("document.forms[0].inicio.value = '{1}';");
            strScript.Append("document.forms[0].origen.value = '{2}';");
            strScript.Append("document.forms[0].txtCodUsuario.value = '{3}';");
            strScript.Append("document.forms[0].txtPassword.value = '{4}';");
            strScript.Append("document.forms[0].submit();");
            strScript.Append("</script>");
            return String.Format(strScript.ToString(), URL, inicio, origen, CodUsuario, Password);
        }

        //public static String _GetForm(string URL, string IdUsuario)
        //{
        //    System.Text.StringBuilder strForm = new System.Text.StringBuilder();
        //    strForm.Append("<form id=\"_xclick\" name=\"_xclick\" action=\"{0}\" method=\"post\">");
        //    strForm.Append("<input type=\"hidden\" name=\"IdUsuario\" value=\"{1}\">");
        //    strForm.Append("</form>");
        //    return String.Format(strForm.ToString(), URL, IdUsuario);
        //}

        public static string BuscaAnalyticsID(string CodPais, string TipoOpe)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string AnalyticsID = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();
            sql = "select AnalyticsID from BaseDatos where CodPais = '" + CodPais + "' and TipoOpe = '" + TipoOpe + "' ";
            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) AnalyticsID = dtr["AnalyticsID"].ToString();
            cn.Close();
            return AnalyticsID;
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

        public static void BuscaURLReferido(string Origen, string IdUsuario, ref string URL1, ref string URL2, ref string URL3)
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

            sql = "select isnull(FlagDesComercial, '') as FlagDesComercial from BaseDatos where CodPais = '" + CodPais + "' and TipoOpe = '" + TipoOpe + "'";

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
                sql += "select 2 as Orden, CodPais, PaisES from AdminPais2 where CodPais not in ('PE', 'PEB', 'CN', 'US', 'PEI', 'PEE', 'USI', 'PE_I', 'PE_E', 'EC_I', 'EC_E', 'XX') ";
            }
            else
            {
                sql = "select 1 as Orden, CodPais, PaisES from AdminPais2 where CodPais in ('PE') union ";
                sql += "select 2 as Orden, CodPais, PaisES from AdminPais2 where CodPais not in ('PE', 'PEB', 'CN', 'US', 'PEI', 'PEE', 'USI', 'PE_I', 'PE_E', 'EC_I', 'EC_E', 'XX') ";
            }
            sql += "order by Orden, CodPais";

            cmd = new SqlCommand(sql, cn);
            dtaset = new DataSet();
            dtadap = new SqlDataAdapter(cmd);
            dtadap.Fill(dtaset);

            cn.Close();

            return dtaset.Tables[0];
        }

        public static void GrabaLog(string IdUsuario, string CodPais, string TipoOpe, string AñoMesIni, string AñoMesFin, string Pagina, string Logs)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "insert into Logs(IdUsuario, CodPais, TipoOpe, AñoMesIni, AñoMesFin, Pagina, Logs, Fecha) ";
            sql += "values(" + IdUsuario + ", '" + CodPais + "', '" + TipoOpe + "', " + AñoMesIni + ", " + AñoMesFin + ", '" + Pagina + "', '" + Logs.Replace("'", "''") + "', getdate())";

            cmd = new SqlCommand(sql, cn);
            cmd.ExecuteScalar();

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

        //public static String TrackingCode(string AnalyticsID)
        //{
        //    System.Text.StringBuilder strForm = new System.Text.StringBuilder();
        //    strForm.Append("<script type=\"text/javascript\">");
        //    strForm.Append("var _gaq = _gaq || [];");
        //    strForm.Append("_gaq.push(['_setAccount', '{0}']);");
        //    strForm.Append("_gaq.push(['_trackPageview']);");
        //    strForm.Append("(function() {{");
        //    strForm.Append("var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;");
        //    strForm.Append("ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';");
        //    strForm.Append("var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);");
        //    strForm.Append("}})();");
        //    //strForm.Append("window.setTimeout(\"document.location = 'MyVeritrade.aspx';\", 1000);");
        //    strForm.Append("</script>");

        //    return String.Format(strForm.ToString(), AnalyticsID);
        //}

        public static void BuscaDatosExpress(string IdUsuario, ref string CodPais, ref string TipoOpe)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select CodPais, IdVersion from Suscripcion where IdUsuario = " + IdUsuario;

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read())
            {
                CodPais = dtr["CodPais"].ToString();
                if (dtr["IdVersion"].ToString() == "55") TipoOpe = "I"; else TipoOpe = "E";
            }
            dtr.Close();
            cn.Close();
        }

        /*
        public static int Limite(string Aplicacion, string TipoLimite)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            int aux = 0;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select " + TipoLimite + " from Parametro where Aplicacion = '" + Aplicacion + "'";

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            dtr.Read();
            aux = Convert.ToInt32(dtr[TipoLimite]);
            dtr.Close();

            cn.Close();

            return aux;
        }

        public static bool ValidaVisitasMes(string IdUsuario, string Aplicacion, ref int LimiteVisitas, ref int Visitas)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;

            string CodUsuario;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            CodUsuario = BuscaCodUsuario(IdUsuario);

            sql = "select LimiteVisitas from Parametro where Aplicacion = '" + Aplicacion + "'";

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            dtr.Read();
            LimiteVisitas = Convert.ToInt32(dtr["LimiteVisitas"]);
            dtr.Close();

            sql = "select count(*) as Visitas from Historial where CodEstado is null and IdUsuario = " + IdUsuario + " ";
            sql += "and year(FecVisita) * 100 + month(FecVisita) = " + (DateTime.Today.Year * 100 + DateTime.Today.Month).ToString();

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read())
                Visitas = Convert.ToInt32(dtr["Visitas"]);
            dtr.Close();

            cn.Close();

            return ((CodUsuario == "UPC") || (Visitas < LimiteVisitas));
        }

        public static bool ValidaDescargasMes(string IdUsuario, string Aplicacion, string CodPais, string TipoOpe, ref int LimiteDescargas, ref int Descargas)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;

            string CodUsuario;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            CodUsuario = BuscaCodUsuario(IdUsuario);

            sql = "select LimiteDescargas from Parametro where Aplicacion = '" + Aplicacion + "'";

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read())
                LimiteDescargas = Convert.ToInt32(dtr["LimiteDescargas"]);
            dtr.Close();

            sql = "select count(*) as Descargas from HistorialDescargas ";
            sql += "where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' and TipoOpe = '" + TipoOpe + "' ";
            sql += "and year(FecDescarga) * 100 + month(FecDescarga) = " + (DateTime.Today.Year * 100 + DateTime.Today.Month).ToString();

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read())
                Descargas = Convert.ToInt32(dtr["Descargas"]);
            dtr.Close();

            cn.Close();

            return ((CodUsuario == "UPC") || (Descargas < LimiteDescargas));
        }

        public static bool ValidaFavUnicos(string IdUsuario, string Aplicacion, string CodPais, string TipoOpe, string TipoFavorito)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;

            string CodUsuario;

            int LimiteFavUnicos, FavUnicos = 0;

            string TipoFav;
            if (TipoFavorito != "ImportadorExp") TipoFav = TipoFavorito.Substring(0, 2).ToUpper(); else TipoFav = "IE";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            CodUsuario = BuscaCodUsuario(IdUsuario);

            sql = "select LimiteFavUnicos from Parametro where Aplicacion = '" + Aplicacion + "'";

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

            return ((CodUsuario == "UPC") || (FavUnicos < LimiteFavUnicos));
        }

        public static bool ValidaFavPorGrupo(string IdUsuario, string Aplicacion, string IdGrupo)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;

            string CodUsuario;

            int LimiteFavPorGrupo, FavPorGrupo = 0;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            CodUsuario = BuscaCodUsuario(IdUsuario);

            sql = "select LimiteFavPorGrupo from Parametro where Aplicacion = '" + Aplicacion + "'";

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

            return ((CodUsuario == "UPC") || (FavPorGrupo < LimiteFavPorGrupo));
        }

        public static bool ValidaGrupos(string IdUsuario, string Aplicacion, string CodPais, string TipoOpe, string TipoFavorito)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;

            string CodUsuario;

            int LimiteGrupos, Grupos = 0;

            string TipoFav;
            if (TipoFavorito != "ImportadorExp") TipoFav = TipoFavorito.Substring(0, 2).ToUpper(); else TipoFav = "IE";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            CodUsuario = BuscaCodUsuario(IdUsuario);

            sql = "select LimiteGrupos from Parametro where Aplicacion = '" + Aplicacion + "'";

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            dtr.Read();
            LimiteGrupos = Convert.ToInt32(dtr["LimiteGrupos"]);
            dtr.Close();

            sql = "select count(*) as Grupos from Grupo where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' ";
            sql += "and TipoOpe = '" + TipoOpe + "' and TipoFav = '" + TipoFav  + "'";

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            dtr.Read();
            Grupos = Convert.ToInt32(dtr["Grupos"]);
            dtr.Close();

            cn.Close();

            return ((CodUsuario == "UPC") || (Grupos < LimiteGrupos));
        }
        */

        public static List<String> ObtieneUsuariosEnLinea()
        {
            List<String> UsuariosEnLinea = new List<String>();

            object obj = typeof(HttpRuntime).GetProperty("CacheInternal", BindingFlags.Static).GetValue(null, null);
            object[] obj2 = (object[])obj.GetType().GetField("_caches", BindingFlags.Instance).GetValue(obj);

            for (int i = 0; i < obj2.Length; i++)
            {
                Hashtable c2 = (Hashtable)obj2[i].GetType().GetField("_entries", BindingFlags.Instance).GetValue(obj2[i]);
                foreach (DictionaryEntry entry in c2)
                {
                    object o1 = entry.Value.GetType().GetProperty("Value", BindingFlags.Instance).GetValue(entry.Value, null);
                    if (o1.GetType().ToString() == "System.Web.SessionState.InProcSessionState")
                    {
                        SessionStateItemCollection sess = (SessionStateItemCollection)o1.GetType().GetField("_sessionItems", BindingFlags.Instance).GetValue(o1);
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

        public static bool ExisteUsuarioEnLinea(string IdUsuario)
        {
            bool aux = false;

            List<String> UsuariosEnLinea = ObtieneUsuariosEnLinea();

            foreach (var Usuario in UsuariosEnLinea)
                if (IdUsuario == Usuario.ToString()) aux = true;

            return aux;
        }

        public static System.Text.StringBuilder MensajeJScript(string Mensaje, string Pagina)
        {
            System.Text.StringBuilder aux = new System.Text.StringBuilder();
            aux.Append("<script language='javascript'>");
            aux.Append("alert('" + Mensaje + "');");
            if (Pagina != "") aux.Append("window.location = '" + Pagina + "';");
            aux.Append("</script>");

            return aux;
        }

        public static string BuscaValor(string IdAdminValor)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string Valor = "";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select Valor from AdminValor where IdAdminValor = " + IdAdminValor;

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read()) Valor = dtr["Valor"].ToString();
            dtr.Close();

            cn.Close();

            return Valor;
        }

        public static string ObtieneIdAdminValor(string CodVariable, string Valor)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;
            string IdAdminValor;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select IdAdminValor from AdminValor where CodVariable = '" + CodVariable + "' and Valor = '" + Valor + "' ";

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            dtr.Read();
            IdAdminValor = dtr["IdAdminValor"].ToString();
            dtr.Close();

            cn.Close();

            return IdAdminValor;
        }

        public static DataTable ObtieneAdminValores(string CodVariable)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            DataSet dtaset;
            SqlDataAdapter dtadap;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select IdAdminValor, Valor from AdminValor where CodVariable = '" + CodVariable + "' order by Orden";

            cmd = new SqlCommand(sql, cn);
            dtaset = new DataSet();
            dtadap = new SqlDataAdapter(cmd);
            dtadap.Fill(dtaset);

            cn.Close();

            return dtaset.Tables[0];
        }

        public static string CrearUsuarioFreeTrial(string EmpPer, string CodUsuario, string Password, string Nombres, string Apellidos, string DNI, string Empresa, string RUC,
            string Telefono, string Celular, string Email1, string IdActividad, string Mensaje, string IdConocio, string DireccionIP, string CodCampaña, string URL, string Referido, string Gclid = "")
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            string IdUsuario;
            string IdAplicacion, IdVersion, IdTipo, IdOrigen, IdCargo;
            string IdEstadoVenta, IdEstadoXQNo;
            string CodPais = "", Ubicacion;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            IdAplicacion = ObtieneIdAdminValor("01APL", "Business");
            IdVersion = ObtieneIdAdminValor("02VER", "Starter Pack");
            IdTipo = ObtieneIdAdminValor("03TIP", "Free Trial");
            IdOrigen = ObtieneIdAdminValor("04ORG", "Veritrade");
            IdCargo = ObtieneIdAdminValor("05CAR", "Otro");
            IdEstadoVenta = ObtieneIdAdminValor("06EVT", "Nuevo");
            IdEstadoXQNo = ObtieneIdAdminValor("07EXN", "N/A");
            //IdActividad = ObtieneIdAdminValor("08ACT", "Importación");
            //IdConocio = ObtieneIdAdminValor("09CON", "Web");

            VeritradeAdmin.Seguridad ws = new VeritradeAdmin.Seguridad();

            Ubicacion = ws.BuscaUbicacionIP2(DireccionIP, ref CodPais);
            if (CodPais == "-") CodPais = "PE";

            sql = "insert into Usuario(CodUsuario, Password, EmpPer, Empresa, RUC, Nombres, Apellidos, DNI, Telefono, Celular, Email1, ";
            sql += "IdAplicacion, IdTipo, IdOrigen, IdCargo, IdEstadoVenta, IdEstadoXQNo, IdActividad, IdConocio, Mensaje, DireccionIP, ";
            sql += "CodPaisIP, CodPaisIP2, CodPais, Ubicacion, CodEstado, FecInicio, FecFin, CodSeguridad, CantUsuariosUnicos, CodCampaña, URLRegistro, URLReferido, Gclid, FecRegistro, FecActualizacion) values ";
            sql += "('" + CodUsuario + "', '" + Password + "', '" + EmpPer + "', '" + Empresa + "', '" + RUC + "', '" + Nombres + "', '" + Apellidos + "', ";
            sql += "'" + DNI + "', '" + Telefono + "', '" + Celular + "', '" + Email1 + "', ";
            sql += IdAplicacion + ", " + IdTipo + ", " + IdOrigen + ", " + IdCargo + ", " + IdEstadoVenta + ", " + IdEstadoXQNo + ", ";
            sql += IdActividad + ", " + IdConocio + ", '" + Mensaje + "', '" + DireccionIP + "', '" + CodPais + "', '" + CodPais + "', '" + CodPais + "', '" + Ubicacion + "', ";
            sql += "'A', " + DateTime.Now.ToString("yyyyMMdd") + "," + DateTime.Now.AddDays(5).ToString("yyyyMMdd") + ", 'Off', 1, '" + CodCampaña + "', '" + URL + "', '" + Referido + "', '" + Gclid + "', getdate(), getdate())";
            cmd = new SqlCommand(sql, cn);
            cmd.ExecuteScalar();

            IdUsuario = BuscarIdUsuario(CodUsuario, IdAplicacion);

            sql = "insert into Suscripcion(IdUsuario, IdAplicacion, IdVersion, CodPais) ";
            sql += "select " + IdUsuario + " as IdUsuario, " + IdAplicacion + " as IdAplicacion, " + IdVersion + " as IdVersion, CodPais ";
            sql += "from AdminPais2 where CodPais not in ('XX', 'PE_I', 'PE_E', 'EC_I', 'EC_E')";
            cmd = new SqlCommand(sql, cn);
            cmd.ExecuteScalar();

            sql = "CreaFavoritos";
            cmd = new SqlCommand(sql, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@IdUsuario", SqlDbType.Int, 4).Value = IdUsuario;
            cmd.ExecuteNonQuery();

            cn.Close();

            return IdUsuario;
        }

        public static void BuscaDatosUsuario(string IdUsuario, ref string EmpPer1, ref string CodUsuario, ref string Password, ref string Empresa, ref string RUC,
            ref string Cargo, ref string Nombres, ref string Apellidos, ref string DNI, ref string Telefono, ref string Celular, ref string Email1,
            ref string Actividad, ref string Conocio, ref string Mensaje, ref string DireccionIP, ref string Ubicacion)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;

            string IdCargo, IdActividad, IdConocio;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select * from Usuario where IdUsuario = " + IdUsuario;

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            dtr.Read();

            if (dtr["EmpPer"].ToString() == "EMP") EmpPer1 = "Empresa"; else EmpPer1 = "Persona";
            CodUsuario = dtr["CodUsuario"].ToString();
            Password = dtr["Password"].ToString();
            Empresa = dtr["Empresa"].ToString();
            RUC = dtr["RUC"].ToString();
            IdCargo = dtr["IdCargo"].ToString();
            Cargo = BuscaValor(IdCargo);
            Nombres = dtr["Nombres"].ToString();
            Apellidos = dtr["Apellidos"].ToString();
            DNI = dtr["DNI"].ToString();
            Telefono = dtr["Telefono"].ToString();
            Celular = dtr["Celular"].ToString();
            Email1 = dtr["Email1"].ToString();
            IdActividad = dtr["IdActividad"].ToString();
            Actividad = BuscaValor(IdActividad);
            IdConocio = dtr["IdConocio"].ToString();
            Conocio = BuscaValor(IdConocio);
            Mensaje = dtr["Mensaje"].ToString();
            DireccionIP = dtr["DireccionIP"].ToString();
            Ubicacion = dtr["Ubicacion"].ToString();

            dtr.Close();
            cn.Close();
        }

        public static void BuscaDatosCorreoEnvio(string IdCorreo, ref string Correo, ref string Nombre, ref string Clave)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select Correo, Nombre, Clave from Correo where IdCorreo = " + IdCorreo;
            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read())
            {
                Correo = dtr["Correo"].ToString();
                Nombre = dtr["Nombre"].ToString();
                Clave = dtr["Clave"].ToString();
            }

            dtr.Close();
            cn.Close();
        }

        public static DataTable ObtienePartidas(string CodPais, string TipoOpe, string IdEmpresa, string FechaIni, string FechaFin)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            DataSet dtaset;
            SqlDataAdapter dtadap;
            string Tabla, IdEmpresa1;

            if (TipoOpe == "I") Tabla = "Importacion"; else Tabla = "Exportacion";
            if (TipoOpe == "I") IdEmpresa1 = "IdImportador"; else IdEmpresa1 = "IdExportador";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select IdPartida, sum(FOBTot) from " + Tabla + "_" + CodPais + " ";
            sql += "where " + IdEmpresa1 + " = " + IdEmpresa + " ";
            sql += "and FechaNum >= '" + FechaIni + "' and FechaNum <= '" + FechaFin + "'";
            sql += "group by IdPartida order by 2 desc";

            cmd = new SqlCommand(sql, cn);
            dtaset = new DataSet();
            dtadap = new SqlDataAdapter(cmd);
            dtadap.Fill(dtaset);

            cn.Close();

            return dtaset.Tables[0];
        }

        public static DataTable ObtieneEmpresas(string CodPais, string TipoOpe, string IdPartida, string FechaIni, string FechaFin)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            DataSet dtaset;
            SqlDataAdapter dtadap;
            string Tabla, Empresa1;

            if (TipoOpe == "I") Tabla = "Importacion"; else Tabla = "Exportacion";
            if (TipoOpe == "I") Empresa1 = "Importador"; else Empresa1 = "Exportador";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select Id" + Empresa1 + ", sum(FOBTot) from " + Tabla + "_" + CodPais + " ";
            sql += "where IdPartida = " + IdPartida + " and " + Empresa1 + " not in ('', 'N/A') ";
            sql += "and FechaNum >= '" + FechaIni + "' and FechaNum <= '" + FechaFin + "'";
            sql += "group by Id" + Empresa1 + " order by 2 desc";

            cmd = new SqlCommand(sql, cn);
            dtaset = new DataSet();
            dtadap = new SqlDataAdapter(cmd);
            dtadap.Fill(dtaset);

            cn.Close();

            return dtaset.Tables[0];
        }

        public static DataTable ObtieneProveedores(string CodPais, string IdPartida, string FechaIni, string FechaFin)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            DataSet dtaset;
            SqlDataAdapter dtadap;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select IdProveedor, sum(FOBTot) from Importacion_" + CodPais + " ";
            sql += "where IdPartida = " + IdPartida + " and Proveedor not in ('', 'N/A') ";
            sql += "and FechaNum >= '" + FechaIni + "' and FechaNum <= '" + FechaFin + "'";
            sql += "group by IdProveedor order by 2 desc";

            cmd = new SqlCommand(sql, cn);
            dtaset = new DataSet();
            dtadap = new SqlDataAdapter(cmd);
            dtadap.Fill(dtaset);

            cn.Close();

            return dtaset.Tables[0];
        }

        public static DataTable ObtieneImportadoresExp(string CodPais, string IdPartida, string FechaIni, string FechaFin)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            DataSet dtaset;
            SqlDataAdapter dtadap;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select IdImportadorExp, sum(FOBTot) from Exportacion_" + CodPais + " ";
            sql += "where IdPartida = " + IdPartida + " and ImportadorExp not in ('', 'N/A') ";
            sql += "and FechaNum >= '" + FechaIni + "' and FechaNum <= '" + FechaFin + "'";
            sql += "group by IdImportadorExp order by 2 desc";

            cmd = new SqlCommand(sql, cn);
            dtaset = new DataSet();
            dtadap = new SqlDataAdapter(cmd);
            dtadap.Fill(dtaset);

            cn.Close();

            return dtaset.Tables[0];
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

        public static DataTable ObtienePartidasSimilares(string CodPais, string TipoOpe, string IdPartidaPeru, string FechaIni, string FechaFin)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            DataSet dtaset;
            SqlDataAdapter dtadap;
            string Tabla;

            string NandinaPeru = BuscaNandina(IdPartidaPeru, "PE");

            if (TipoOpe == "I") Tabla = "Importacion"; else Tabla = "Exportacion";

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select T.IdPartida, sum(FOBTot) from " + Tabla + "_" + CodPais + " T, Partida_" + CodPais + " P ";
            sql += "where T.IdPartida = P.IdPartida and P.Nandina like '" + NandinaPeru.Substring(0, 6) + "%' ";
            sql += "and FechaNum >= '" + FechaIni + "' and FechaNum <= '" + FechaFin + "'";
            sql += "group by T.IdPartida order by 2 desc";

            cmd = new SqlCommand(sql, cn);
            cmd.CommandTimeout = 120;
            dtaset = new DataSet();
            dtadap = new SqlDataAdapter(cmd);
            dtadap.Fill(dtaset);

            cn.Close();

            return dtaset.Tables[0];
        }

        public static DataTable BuscaEmpresas(string CodPais, string TipoOpe, string Opcion, string EmpresaB, string Palabra1, string Palabra2, string Palabra3, bool flagEmpiezaCon, bool flagY, string FechaIni, string FechaFin)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            DataSet dtaset;
            SqlDataAdapter dtadap;
            string tabla;
            string Empresa;
            string RUC;

            string CampoPeso = Functions3.CampoPeso(CodPais, TipoOpe);
            string CIF = Functions3.Incoterm(CodPais, TipoOpe);

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            if (CodPais != "PY") RUC = "RUC, "; else RUC = "";

            if (TipoOpe == "I") tabla = "Importacion_" + CodPais; else tabla = "Exportacion_" + CodPais;
            if (TipoOpe == "I") Empresa = "Importador"; else Empresa = "Exportador";

            sql = "select 'F' + convert(varchar(10), Id" + Empresa + ") as IdEmpresa, " + Empresa + " as Empresa, " + ((RUC == "") ? "'' as RUC, " : RUC) + "count(*) as CantReg, ";
            if (CampoPeso != "")
                sql += "sum(" + CampoPeso + ") as " + CampoPeso + ", ";
            sql += "sum(" + CIF + "Tot) as " + CIF + "Tot ";
            sql += "from " + tabla + " T ";
            sql += "where 1 = 1 ";

            if (Opcion == "RUC")
                sql += "and RUC like '%" + EmpresaB + "%' ";
            else if (flagEmpiezaCon)
                sql += "and " + Empresa + " like '" + EmpresaB + "%' ";
            else
                if (flagY)
                {
                    sql += "and " + Empresa + " like '%" + Palabra1 + "%' ";
                    if (Palabra2 != "") sql += "and " + Empresa + " like '%" + Palabra2 + "%' ";
                    if (Palabra3 != "") sql += "and " + Empresa + " like '%" + Palabra3 + "%' ";
                }
                else
                {
                    sql += "and " + Empresa + " like '%" + Palabra1 + "%' ";
                    if (Palabra2 != "") sql += "or " + Empresa + " like '%" + Palabra2 + "%' ";
                    if (Palabra3 != "") sql += "or " + Empresa + " like '%" + Palabra3 + "%' ";
                }

            sql += "and FechaNum >= '" + FechaIni + "' and FechaNum <= '" + FechaFin + "' ";
            sql += "group by Id" + Empresa + ", " + RUC + Empresa + " ";
            sql += "order by " + CIF + "Tot desc, " + Empresa;

            cmd = new SqlCommand(sql, cn);
            cmd.CommandTimeout = 120;
            dtaset = new DataSet();
            dtadap = new SqlDataAdapter(cmd);
            dtadap.Fill(dtaset);

            cn.Close();

            return dtaset.Tables[0];
        }

        public static DataTable BuscaPartidas(string CodPais, string TipoOpe, string Opcion, string Palabra1, string Palabra2, string Palabra3, bool flagY, string FechaIni, string FechaFin, string Orden)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            DataSet dtaset;
            SqlDataAdapter dtadap;
            string tabla;

            string CampoPeso = Functions3.CampoPeso(CodPais, TipoOpe);
            string CIF = Functions3.Incoterm(CodPais, TipoOpe);

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            if (TipoOpe == "I") tabla = "Importacion_" + CodPais; else tabla = "Exportacion_" + CodPais;

            sql = "select 'F' + convert(varchar(10), P.IdPartida) as IdPartida, P.Nandina, Partida, count(*) as CantReg, ";
            if (CampoPeso != "")
                sql += "sum(" + CampoPeso + ") as " + CampoPeso + ", ";
            sql += "sum(" + CIF + "Tot) as " + CIF + "Tot ";
            sql += "from " + tabla + " T, Partida_" + CodPais + " P ";
            sql += "where T.IdPartida = P.IdPartida ";
            if (Opcion == "NAN")
                sql += "and P.Nandina like '" + Palabra1 + "%' ";
            /*
            else if (Opcion == "PAR")
                if (flagY)
                {
                    sql += "and (P.Partida like '%" + Palabra1 + "%' ";
                    if (Palabra2 != "") sql += "and P.Partida like '%" + Palabra2 + "%' ";
                    if (Palabra3 != "") sql += "and P.Partida like '%" + Palabra3 + "%' ";
                    sql += ") ";
                }
                else
                {
                    sql += "and (P.Partida like '" + Palabra1 + "%' ";
                    if (Palabra2 != "") sql += "or P.Partida like '" + Palabra2 + "%' ";
                    if (Palabra3 != "") sql += "or P.Partida like '" + Palabra3 + "%' ";
                    sql += ") ";
                }
            */
            else
                if (Palabra1.Substring(0, 1) == "\"" && Palabra1.Substring(Palabra1.Length - 1, 1) == "\"")
                    sql += "and contains(Descomercial, '" + Palabra1 + "') ";
                else if (flagY)
                {
                    sql += "and contains(Descomercial, '\"" + Palabra1 + "*\" ";
                    if (Palabra2 != "") sql += "and \"" + Palabra2 + "*\" ";
                    if (Palabra3 != "") sql += "and \"" + Palabra3 + "*\" ";
                    sql += "') ";
                    //sql += "or contains(Partida, '\"" + Palabra1 + "*\" ";
                    //if (Palabra2 != "") sql += "and \"" + Palabra2 + "*\" ";
                    //if (Palabra3 != "") sql += "and \"" + Palabra3 + "*\" ";
                    //sql += "') )";
                }
                else
                {
                    sql += "and contains(Descomercial, '\"" + Palabra1 + "*\" ";
                    if (Palabra2 != "") sql += "or \"" + Palabra2 + "*\" ";
                    if (Palabra3 != "") sql += "or \"" + Palabra3 + "*\" ";
                    sql += "') ";
                    //sql += "or contains(Partida, '\"" + Palabra1 + "*\" ";
                    //if (Palabra2 != "") sql += "or \"" + Palabra2 + "*\" ";
                    //if (Palabra3 != "") sql += "or \"" + Palabra3 + "*\" ";
                    //sql += "') )";
                }
            sql += "and FechaNum > " + FechaIni + " and FechaNum < " + FechaFin + " ";
            sql += "group by P.IdPartida, P.Nandina, P.Partida ";
            if (Opcion == "NAN")
            {
                sql += "union select 'F' + convert(varchar(10), IdPartida) as IdPartida, Nandina, Partida, 0 as CantReg, " + (CampoPeso != "" ? "0 as CampoPeso, " : "") + "0 as " + CIF + "Tot ";
                sql += "from Partida_" + CodPais + " ";
                sql += "where Nandina like '" + Palabra1 + "%' ";
                sql += "and IdPartida not in (select distinct IdPartida from " + tabla + " ";
                sql += "where Nandina like '" + Palabra1 + "%' ";
                sql += "and FechaNum > " + FechaIni + " and FechaNum < " + FechaFin + ") ";
            }
            if (Orden == "")
                sql += "order by " + CIF + "Tot desc, Nandina";
            else
                sql += "order by " + Orden;

            cmd = new SqlCommand(sql, cn);
            cmd.CommandTimeout = 120;
            dtaset = new DataSet();
            dtadap = new SqlDataAdapter(cmd);
            dtadap.Fill(dtaset);

            cn.Close();

            return dtaset.Tables[0];
        }

        public static DataTable BuscaDesComercial(string CodPais, string TipoOpe, string Opcion, string IdPartida, string Palabra1, string Palabra2, string Palabra3, bool flagY, string FechaIni, string FechaFin)
        {
            string sql = "";
            SqlConnection cn;
            SqlCommand cmd;
            DataSet dtaset;
            SqlDataAdapter dtadap;
            string tabla;

            string CampoPeso = Functions3.CampoPeso(CodPais, TipoOpe);
            string CIF = Functions3.Incoterm(CodPais, TipoOpe);

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            if (TipoOpe == "I") tabla = "Importacion_" + CodPais; else tabla = "Exportacion_" + CodPais;

            sql = "select top 20 FechaNum, DesComercial, Cantidad, Unidad, " + (CampoPeso != "" ? CampoPeso + ", " : "") + CIF + "Tot as " + CIF + "Tot ";
            sql += "from " + tabla + " ";
            sql += "where IdPartida = " + IdPartida + " ";
            if (Opcion == "DES")
                if (flagY)
                {
                    sql += "and contains(Descomercial, '\"" + Palabra1 + "*\" ";
                    if (Palabra2 != "") sql += "and \"" + Palabra2 + "*\" ";
                    if (Palabra3 != "") sql += "and \"" + Palabra3 + "*\" ";
                    sql += "') ";
                }
                else
                {
                    sql += "and contains(Descomercial, '\"" + Palabra1 + "*\" ";
                    if (Palabra2 != "") sql += "or \"" + Palabra2 + "*\" ";
                    if (Palabra3 != "") sql += "or \"" + Palabra3 + "*\" ";
                    sql += "') ";
                }
            sql += "and FechaNum > " + FechaIni + " and FechaNum < " + FechaFin + " ";
            sql += "order by FechaNum, " + CIF + "Tot desc";

            cmd = new SqlCommand(sql, cn);
            dtadap = new SqlDataAdapter(cmd);
            dtaset = new DataSet();
            dtadap.Fill(dtaset);

            cn.Close();

            return dtaset.Tables[0];
        }

        public static bool CreacionAutomaticaPartidas(string IdUsuario, string CodPais, string TipoOpe, string IdEmpresa, DataTable Partidas, string FechaIni, string FechaFin)
        {
            string IdPartida;

            int MaxPartidas = MaxFavoritosProcesoAuto, CantPartidas;

            CantPartidas = 0;
            foreach (DataRow row in Partidas.Rows)
                if (CantPartidas < MaxPartidas)
                {
                    CantPartidas += 1;
                    IdPartida = row["IdPartida"].ToString();
                    CreacionAutomaticaFavoritos(IdUsuario, CodPais, TipoOpe, IdPartida, FechaIni, FechaFin);
                    if (CodPais == "PE") CreacionAutomaticaFavoritos(IdUsuario, "PEB", TipoOpe, IdPartida, FechaIni, FechaFin);
                }

            string Empresa1;
            if (TipoOpe == "I") Empresa1 = "Importador"; else Empresa1 = "Exportador";
            if (!ExisteFavorito(Empresa1, IdUsuario, CodPais, TipoOpe, IdEmpresa))
                AgregaFavorito(Empresa1, IdUsuario, CodPais, TipoOpe, IdEmpresa);

            return CantPartidas > 0;
        }

        public static void CreacionAutomaticaFavoritos(string IdUsuario, string CodPais, string TipoOpe, string IdPartida, string FechaIni, string FechaFin)
        {
            string IdEmpresa, IdProveedor, IdImportadorExp;
            DataTable Empresas, Proveedores, ImportadoresExp;

            int MaxEmpresas = MaxFavoritosProcesoAuto, MaxProveedores = MaxFavoritosProcesoAuto, MaxImportadoresExp = MaxFavoritosProcesoAuto;
            int CantEmpresas, CantProveedores, CantImportadoresExp;

            string Empresa1;
            if (TipoOpe == "I") Empresa1 = "Importador"; else Empresa1 = "Exportador";

            if (!ExisteFavorito("Partida", IdUsuario, CodPais, TipoOpe, IdPartida))
                AgregaFavorito("Partida", IdUsuario, CodPais, TipoOpe, IdPartida);

            //Agrega Empresas
            if (ExisteVariable(CodPais, TipoOpe, "Id" + Empresa1))
            {
                Empresas = ObtieneEmpresas(CodPais, TipoOpe, IdPartida, FechaIni, FechaFin);
                CantEmpresas = 0;
                foreach (DataRow row2 in Empresas.Rows)
                    if (CantEmpresas < MaxEmpresas)
                    {
                        CantEmpresas += 1;
                        IdEmpresa = row2["Id" + Empresa1].ToString();
                        if (!ExisteFavorito(Empresa1, IdUsuario, CodPais, TipoOpe, IdEmpresa))
                        {
                            AgregaFavorito(Empresa1, IdUsuario, CodPais, TipoOpe, IdEmpresa);
                            if (CodPais == "PE") AgregaFavorito(Empresa1, IdUsuario, "PEB", TipoOpe, IdEmpresa);
                        }
                    }
            }

            //Agrega Proveedores
            if (TipoOpe == "I" && ExisteVariable(CodPais, TipoOpe, "IdProveedor"))
            {
                Proveedores = ObtieneProveedores(CodPais, IdPartida, FechaIni, FechaFin);
                CantProveedores = 0;
                foreach (DataRow row2 in Proveedores.Rows)
                    if (CantProveedores < MaxProveedores)
                    {
                        CantProveedores += 1;
                        IdProveedor = row2["IdProveedor"].ToString();
                        if (!ExisteFavorito("Proveedor", IdUsuario, CodPais, TipoOpe, IdProveedor))
                        {
                            AgregaFavorito("Proveedor", IdUsuario, CodPais, TipoOpe, IdProveedor);
                            if (CodPais == "PE") AgregaFavorito("Proveedor", IdUsuario, "PEB", TipoOpe, IdProveedor);
                        }
                    }
            }

            //Agrega ImportadoresExp
            if (TipoOpe == "E" && ExisteVariable(CodPais, TipoOpe, "IdImportadorExp"))
            {
                ImportadoresExp = ObtieneImportadoresExp(CodPais, IdPartida, FechaIni, FechaFin);
                CantImportadoresExp = 0;
                foreach (DataRow row2 in ImportadoresExp.Rows)
                    if (CantImportadoresExp < MaxImportadoresExp)
                    {
                        CantImportadoresExp += 1;
                        IdImportadorExp = row2["IdImportadorExp"].ToString();
                        if (!ExisteFavorito("ImportadorExp", IdUsuario, CodPais, TipoOpe, IdImportadorExp))
                        {
                            AgregaFavorito("ImportadorExp", IdUsuario, CodPais, TipoOpe, IdImportadorExp);
                            if (CodPais == "PE") AgregaFavorito("ImportadorExp", IdUsuario, "PEB", TipoOpe, IdImportadorExp);
                        }
                    }
            }
        }

        public static bool CreacionAutomaticaPartidasSimilares(string IdUsuario, string CodPais, string TipoOpe, System.Data.DataTable PartidasPeru, string FechaIni, string FechaFin)
        {
            string IdPartidaPeru, IdPartida;
            DataTable Partidas;
            bool flag = false;

            int CantPartidasPeru, CantPartidas, MaxPartidas = 1; //MaxFavoritosProcesoAuto;

            CantPartidasPeru = 0;
            foreach (DataRow rowPeru in PartidasPeru.Rows)
                if (CantPartidasPeru < MaxPartidas)
                {
                    CantPartidasPeru += 1;
                    IdPartidaPeru = rowPeru["IdPartida"].ToString();

                    Partidas = ObtienePartidasSimilares(CodPais, TipoOpe, IdPartidaPeru, FechaIni, FechaFin);
                    CantPartidas = 0;
                    foreach (DataRow row in Partidas.Rows)
                        if (CantPartidas < MaxPartidas)
                        {
                            CantPartidas += 1;
                            IdPartida = row["IdPartida"].ToString();
                            CreacionAutomaticaFavoritos(IdUsuario, CodPais, TipoOpe, IdPartida, FechaIni, FechaFin);
                            flag = true;
                        }
                }

            return flag;
        }

        public static DataTable ObtieneFavoritos(string IdUsuario, string CodPais, string TipoOpe, string TipoFavorito)
        {
            string sql = "";
            SqlConnection cn;
            SqlCommand cmd;
            DataSet dtaset;
            SqlDataAdapter dtadap;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            switch (TipoFavorito)
            {
                case "Partida":
                    sql = "select P.Nandina as [Partidas Arancelarias], P.Partida as [Descripción de Partidas Arancelarias] from PartidaFav F, Partida_" + CodPais + " P ";
                    sql += "where F.IdPartida = P.IdPartida and IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' ";
                    sql += "and TipoOpe = '" + TipoOpe + "' order by 1";
                    break;
                case "Importador":
                    sql = "select E.Empresa as Importadores from EmpresaFav F, Empresa_" + CodPais + " E ";
                    sql += "where F.IdEmpresa = E.IdEmpresa and IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' ";
                    sql += "and TipoOpe = 'I' order by 1";
                    break;
                case "Proveedor":
                    sql = "select P.Proveedor as Exportadores from ProveedorFav F, Proveedor_" + CodPais + " P ";
                    sql += "where F.IdProveedor = P.IdProveedor and IdUsuario = " + IdUsuario + " and F.CodPais = '" + CodPais + "' ";
                    sql += "order by 1";
                    break;
                case "Exportador":
                    sql = "select E.Empresa as Exportadores from EmpresaFav F, Empresa_" + CodPais + " E ";
                    sql += "where F.IdEmpresa = E.IdEmpresa and IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' ";
                    sql += "and TipoOpe = 'E' order by 1";
                    break;
                case "ImportadorExp":
                    sql = "select I.ImportadorExp as Importadores from ImportadorExpFav F, ImportadorExp_" + CodPais + " I ";
                    sql += "where F.IdImportadorExp = I.IdImportadorExp and IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' ";
                    sql += "order by 1";
                    break;
            }

            cmd = new SqlCommand(sql, cn);
            dtaset = new DataSet();
            dtadap = new SqlDataAdapter(cmd);
            dtadap.Fill(dtaset);

            cn.Close();

            return dtaset.Tables[0];
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

        public static DataTable ObtieneTablaSentinel(string CodTabla)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            DataSet dtaset;
            SqlDataAdapter dtadap;


            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select Valor, Descripcion from Sentinel where CodTabla = '" + CodTabla + "'";

            cmd = new SqlCommand(sql, cn);
            dtaset = new DataSet();
            dtadap = new SqlDataAdapter(cmd);
            dtadap.Fill(dtaset);

            cn.Close();

            return dtaset.Tables[0];
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

            sql = "select Valor as [Plan] from Usuario U, AdminValor A where U.IdPlan = A.IdAdminValor and IdUsuario = " + IdUsuario;

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            dtr.Read();
            Plan = dtr["Plan"].ToString();
            dtr.Close();

            cn.Close();

            return Plan;
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

            sql = "select count(*) as Visitas from Historial where CodEstado is null and IdUsuario = " + IdUsuario + " ";
            sql += "and year(FecVisita) * 100 + month(FecVisita) = " + (DateTime.Today.Year * 100 + DateTime.Today.Month).ToString();

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read())
                Visitas = Convert.ToInt32(dtr["Visitas"]);
            dtr.Close();

            cn.Close();

            return (Visitas < LimiteVisitas);
        }

        public static bool ValidaDescargasMes(string IdUsuario, string CodPais, string TipoOpe, ref int LimiteDescargas, ref int Descargas)
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
            sql += "and year(FecDescarga) * 100 + month(FecDescarga) = " + (DateTime.Today.Year * 100 + DateTime.Today.Month).ToString();

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
            if (TipoFavorito != "ImportadorExp") TipoFav = TipoFavorito.Substring(0, 2).ToUpper(); else TipoFav = "IE";

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
            if (TipoFavorito != "ImportadorExp") TipoFav = TipoFavorito.Substring(0, 2).ToUpper(); else TipoFav = "IE";

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

            sql = "select count(*) as Grupos from Grupo where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' ";
            sql += "and TipoOpe = '" + TipoOpe + "' and TipoFav = '" + TipoFav + "'";

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

        public static void ActualizaGrupo(string IdUsuario, string CodPais, string TipoOpe, string TipoFavorito, bool flagCreaGrupo, string NuevoGrupo, string IdGrupo, ArrayList IDsSeleccionados, ref string Mensaje, ref string Pagina)
        {
            string TipoFav;

            if (TipoFavorito != "ImportadorExp") TipoFav = TipoFavorito.Substring(0, 2).ToUpper(); else TipoFav = "IE";

            bool ValidaGrupos = Functions3.ValidaGrupos(IdUsuario, CodPais, TipoOpe, TipoFavorito);

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
                    Mensaje += "Nota: Se ha alcanzado el máximo de " + LimiteFavPorGrupo.ToString() + " Favoritos en el Grupo";
                    //flagContactenos = true;
                }
                Pagina = "Groups.aspx?TipoFavorito=" + TipoFavorito;
            }
        }

        public static void ActualizaGrupo2(string IdUsuario, string CodPais, string TipoOpe, string TipoFavorito, bool flagCreaGrupo, string NuevoGrupo, string IdGrupo, ArrayList IDsSeleccionados, ref string Mensaje, ref string Pagina)
        {
            string TipoFav;

            if (TipoFavorito != "ImportadorExp") TipoFav = TipoFavorito.Substring(0, 2).ToUpper(); else TipoFav = "IE";

            bool ValidaGrupos = Functions3.ValidaGrupos(IdUsuario, CodPais, TipoOpe, TipoFavorito);

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
                    Mensaje += "Nota: Se ha alcanzado el máximo de " + LimiteFavPorGrupo.ToString() + " Favoritos en el Grupo";
                    //flagContactenos = true;
                }
                Pagina = "MisGrupos.aspx?TipoFavorito=" + TipoFavorito;
            }
        }

        public static void AgregaFavoritos(string IdUsuario, string CodPais, string TipoOpe, string TipoFavorito, ArrayList IDsSeleccionados, ref string Mensaje, ref string Pagina)
        {
            int CantSelec, CantGrab, LimiteFavUnicos;
            bool flagLimFavUnicos = false, flagLimFavUnicos2 = false;

            CantSelec = IDsSeleccionados.Count;
            CantGrab = Functions3.GrabaFavoritos(IDsSeleccionados, TipoFavorito, IdUsuario, CodPais, TipoOpe, ref flagLimFavUnicos);
            //CantFav = Functions.CantFavUnicos(IdUsuario, CodPais, TipoOpe, AgrupaPor);
            string IdPlan = Functions3.ObtieneIdPlan(IdUsuario);
            LimiteFavUnicos = Functions3.ObtieneLimite(IdPlan, "LimiteFavUnicos");

            //MANUEL: Se adiciona la funcionalidad que permite grabar en Peru y en Peru_B Importadores a la vez
            if (ObtienePlan(IdUsuario) != "Express" && CodPais.Equals("PE") && TipoOpe.Equals("I"))
                Functions3.GrabaFavoritos(IDsSeleccionados, TipoFavorito, IdUsuario, "PEB", TipoOpe, ref flagLimFavUnicos2);
            else if (ObtienePlan(IdUsuario) != "Express" && CodPais.Equals("PEB") && TipoOpe.Equals("I"))
                Functions3.GrabaFavoritos(IDsSeleccionados, TipoFavorito, IdUsuario, "PE", TipoOpe, ref flagLimFavUnicos2);

            /*
            if (CantGrab > 0)
                if (CantGrab == 1)
                    Mensaje = "Se agregó 1 favorito satisfactoriamente. ";
                else
                    Mensaje = "Se agregaron " + CantGrab.ToString() + " favoritos satisfactoriamente. ";
            else
            {
                Mensaje = "No se pudo agregar los favoritos seleccionados. ";
                flagOk = false;
            }
            */

            if (flagLimFavUnicos)
            {
                Mensaje += "Nota: Se ha alcanzado el límite de " + LimiteFavUnicos.ToString() + " Favoritos";
            }

            if (TipoFavorito == "Partida") Pagina = "FavProducts.aspx";
            if (TipoFavorito == "Importador" || TipoFavorito == "Exportador") Pagina = "FavImportersExporters.aspx";
            if (TipoFavorito == "Proveedor") Pagina = "FavExporters.aspx";
            if (TipoFavorito == "ImportadorExp") Pagina = "FavImportersExp.aspx";
        }

        public static void AgregaFavoritos2(string IdUsuario, string CodPais, string TipoOpe, string TipoFavorito, ArrayList IDsSeleccionados, ref string Mensaje, ref string Pagina)
        {
            int CantSelec, CantGrab, LimiteFavUnicos;
            bool flagLimFavUnicos = false, flagLimFavUnicos2 = false;

            CantSelec = IDsSeleccionados.Count;
            CantGrab = Functions3.GrabaFavoritos(IDsSeleccionados, TipoFavorito, IdUsuario, CodPais, TipoOpe, ref flagLimFavUnicos);
            //CantFav = Functions.CantFavUnicos(IdUsuario, CodPais, TipoOpe, AgrupaPor);
            string IdPlan = Functions3.ObtieneIdPlan(IdUsuario);
            LimiteFavUnicos = Functions3.ObtieneLimite(IdPlan, "LimiteFavUnicos");

            //MANUEL: Se adiciona la funcionalidad que permite grabar en Peru y en Peru_B Importadores a la vez
            if (ObtienePlan(IdUsuario) != "Express" && CodPais.Equals("PE") && TipoOpe.Equals("I"))
                Functions3.GrabaFavoritos(IDsSeleccionados, TipoFavorito, IdUsuario, "PEB", TipoOpe, ref flagLimFavUnicos2);
            else if (ObtienePlan(IdUsuario) != "Express" && CodPais.Equals("PEB") && TipoOpe.Equals("I"))
                Functions3.GrabaFavoritos(IDsSeleccionados, TipoFavorito, IdUsuario, "PE", TipoOpe, ref flagLimFavUnicos2);

            /*
            if (CantGrab > 0)
                if (CantGrab == 1)
                    Mensaje = "Se agregó 1 favorito satisfactoriamente. ";
                else
                    Mensaje = "Se agregaron " + CantGrab.ToString() + " favoritos satisfactoriamente. ";
            else
            {
                Mensaje = "No se pudo agregar los favoritos seleccionados. ";
                flagOk = false;
            }
            */

            if (flagLimFavUnicos)
            {
                Mensaje += "Nota: Se ha alcanzado el límite de " + LimiteFavUnicos.ToString() + " Favoritos";
            }

            if (TipoFavorito == "Partida") Pagina = "AdminMisProductos.aspx";
            if (TipoFavorito == "Importador" || TipoFavorito == "Exportador") Pagina = "AdminMisCompanias.aspx";
            if (TipoFavorito == "Proveedor") Pagina = "AdminMisExportadores.aspx";
            if (TipoFavorito == "ImportadorExp") Pagina = "AdminMisImportadores.aspx";
        }

        public static bool ValidaCadena(string Cadena)
        {
            string Permitidos = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            bool flag = true;

            for (int i = 0; i < Cadena.Length; i = i + 1)
            {
                bool flag1 = false;
                for (int j = 0; j < Permitidos.Length; j = j + 1)
                {
                    if (Cadena.Substring(i, 1) == Permitidos.Substring(j, 1)) flag1 = true;
                }
                flag = flag && flag1;
            }
            return flag;
        }

        public static ArrayList GuardaSeleccionados(string CodPais, string TipoOpe, GridView gdv, ArrayList IDsSeleccionados, ListBox lstSeleccionados)
        {
            string ID, Tipo = "", Nombre = "";

            if (IDsSeleccionados == null) IDsSeleccionados = new ArrayList();
            foreach (GridViewRow row in gdv.Rows)
            {
                if (gdv.ID != "gdvAduanaDUAs")
                    ID = gdv.DataKeys[row.RowIndex].Value.ToString();
                else
                    ID = gdv.DataKeys[row.RowIndex].Values[0].ToString() + '-' + gdv.DataKeys[row.RowIndex].Values[1].ToString();

                if (ID.Substring(0, 1) == "F" || ID.Substring(0, 1) == "G")
                    ID = ID.Substring(1, ID.Length - 1);
                bool FlagSeleccionado = ((CheckBox)row.FindControl("chkSel")).Checked;
                if (FlagSeleccionado)
                {
                    if (!IDsSeleccionados.Contains(ID))
                    {
                        IDsSeleccionados.Add(ID);
                        switch (gdv.ID)
                        {
                            case "gdvPartidas":
                            case "gdvPartidas2": Tipo = "2PA"; Nombre = "[Partida] " + BuscaPartida(ID, CodPais); break;
                            case "gdvMarcas":
                            case "gdvMarcas2": Tipo = "2MA"; Nombre = "[Marca] " + BuscaMarca(ID, CodPais); break;
                            case "gdvModelos": Tipo = "2MO"; Nombre = "[Modelo] " + BuscaModelo(ID, CodPais); break;
                            case "gdvImportadores":
                            case "gdvImportadores2":
                                if (TipoOpe == "I")
                                { Tipo = "3IM"; Nombre = "[Importador] " + BuscaEmpresa(ID, CodPais); }
                                else
                                { Tipo = "3EX"; Nombre = "[Exportador] " + BuscaEmpresa(ID, CodPais); }
                                break;
                            case "gdvNotificados": Tipo = "3NO"; Nombre = "[Notificado] " + BuscaNotificado(ID, CodPais); break;
                            case "gdvProveedores":
                            case "gdvProveedores2":
                                if (TipoOpe == "I")
                                { Tipo = "4PR"; Nombre = ((CodPais != "CL") ? "[Exportador] " : "[Marca] ") + BuscaProveedor(ID, CodPais); }
                                else
                                { Tipo = "4IE"; Nombre = "[Importador] " + BuscaImportadorExp(ID, CodPais); }
                                break;
                            case "gdvPaisesOrigen":
                            case "gdvPaisesOrigen2":
                                if (TipoOpe == "I" && CodPais != "USI")
                                { Tipo = "5PO"; Nombre = "[País Origen] " + BuscaPais(ID, CodPais); }
                                else if (TipoOpe == "I" && CodPais == "USI")
                                { Tipo = "5PE"; Nombre = "[Último País Embarque] " + BuscaPais(ID, CodPais); }
                                else
                                { Tipo = "5PD"; Nombre = "[País Destino] " + BuscaPais(ID, CodPais); }
                                break;
                            case "gdvViasTransp":
                            case "gdvViasTransp2":
                                if (CodPais == "USI")
                                { Tipo = "6PD"; Nombre = "[Puerto Descarga] " + BuscaPuerto(ID, CodPais); }
                                else if (CodPais == "PEE")
                                { Tipo = "6DE"; Nombre = "[Puerto Destino] " + BuscaPuerto(ID, CodPais); }
                                else if (CodPais == "USE" || CodPais == "PEI")
                                { Tipo = "6PE"; Nombre = "[Último Puerto Embarque] " + BuscaPuerto(ID, CodPais); }
                                else
                                { Tipo = "6VT"; Nombre = "[Vía Transporte] " + Functions3.BuscaVia(ID, CodPais); }
                                break;
                            case "gdvAduanaDUAs": Tipo = "7AD"; Nombre = "[Aduana DUA] " + BuscaAduana(gdv.DataKeys[row.RowIndex].Values[0].ToString(), CodPais) + ' ' + gdv.DataKeys[row.RowIndex].Values[1].ToString(); break;
                            case "gdvAduanas": Tipo = "7AA"; Nombre = "[Aduana] " + BuscaAduana(gdv.DataKeys[row.RowIndex].Values[0].ToString(), CodPais); break;
                            case "gdvDistritos":
                            case "gdvDistritos2": Tipo = "8DI"; Nombre = "[Distrito] " + BuscaDistrito(ID, CodPais); break;
                            case "gdvManifiestos": Tipo = "9MA"; Nombre = "[Manifiesto] " + ID; break;
                        }
                        lstSeleccionados.Items.Add(new ListItem(Nombre, Tipo + ID));
                    }
                }
                //else
                //    IDsSeleccionados.Remove(ID);
            }
            return IDsSeleccionados;
        }

        public static string GeneraExcel(string IdUsuario, string CodPais, string TipoOpe, string IdPartida, string Palabra1, string Palabra2, string Palabra3, string Opcion, string FechaIni, string FechaFin)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;

            string CodUsuario = Functions3.BuscaCodUsuario(IdUsuario).ToUpper();
            string ahora = DateTime.Now.ToString("yyyyMMddHHmmss");
            string NombreArchivo = "Veritrade_" + CodUsuario + "_" + CodPais + "_" + TipoOpe + "_" + ahora;
            string Titulos = Functions3.ListaTitulosDescarga("", "", CodPais, TipoOpe);
            string Campos = Functions3.ListaCamposDescarga("", "", CodPais, TipoOpe);

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "GeneraArchivoZIP2";

            cmd = new SqlCommand(sql, cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@IdUsuario", SqlDbType.Int, 4).Value = IdUsuario;
            if (Functions3.BuscaTipoUsuario(IdUsuario) == "Free Trial")
                cmd.Parameters.Add("@FreeTrial", SqlDbType.Char, 1).Value = "S";
            cmd.Parameters.Add("@CodPais", SqlDbType.VarChar, 3).Value = CodPais;
            cmd.Parameters.Add("@TipoOpe", SqlDbType.VarChar, 1).Value = TipoOpe;
            cmd.Parameters.Add("@Titulos", SqlDbType.VarChar, 1000).Value = Titulos;
            cmd.Parameters.Add("@Campos", SqlDbType.VarChar, 1000).Value = Campos;
            cmd.Parameters.Add("@Variable1", SqlDbType.VarChar, 20).Value = "IdPartida";
            cmd.Parameters.Add("@Valor1", SqlDbType.VarChar, 8000).Value = "(" + IdPartida + ")";
            if (Palabra1 != "")
                cmd.Parameters.Add("@Palabra1", SqlDbType.VarChar, 20).Value = Palabra1;
            if (Palabra2 != "")
                cmd.Parameters.Add("@Palabra2", SqlDbType.VarChar, 20).Value = Palabra2;
            if (Palabra3 != "")
                cmd.Parameters.Add("@Palabra3", SqlDbType.VarChar, 20).Value = Palabra3;
            if (Opcion != "")
                cmd.Parameters.Add("@Opcion", SqlDbType.Char, 1).Value = Opcion;
            cmd.Parameters.Add("@FechaIni", SqlDbType.VarChar, 10).Value = FechaIni;
            cmd.Parameters.Add("@FechaFin", SqlDbType.VarChar, 10).Value = FechaFin;
            cmd.Parameters.Add("@BaseDatos", SqlDbType.VarChar, 40).Value = Functions3.base_datos;
            cmd.Parameters.Add("@NombreArchivo", SqlDbType.VarChar, 100).Value = NombreArchivo;
            cmd.Parameters.Add("@Directorio", SqlDbType.VarChar, 40).Value = ConfigurationManager.AppSettings["directorio_descarga"];//Functions.directorio_descarga;
            cmd.Parameters.Add("@RutaWinrar", SqlDbType.VarChar, 40).Value = Functions3.ruta_winrar;
            cmd.CommandTimeout = 600;
            cmd.ExecuteScalar();

            cn.Close();

            return NombreArchivo;
        }

        public static DataTable LlenaFavoritosFreeTrial(string IdUsuario, string CodPais, string TipoOpe, string IdPartidas, string IdEmpresas)
        {
            String sql = "";
            SqlConnection cn;
            SqlCommand cmd;
            DataSet dtaset;
            SqlDataAdapter dtadap;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            if (IdPartidas != "")
            {
                sql = "select F.IdPartida as IdFavorito, 'Producto' as TipoFavorito, P.Nandina + ' ' + P.Partida as Favorito, F.PartidaFav as Personalizado ";
                sql += "from PartidaFav F, Partida_" + CodPais + " P ";
                sql += "where F.IdPartida = P.IdPartida and IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' and TipoOpe = '" + TipoOpe + "' ";
                sql += "and F.IdPartida in (" + IdPartidas + ")";
            }
            if (IdPartidas != "" && IdEmpresas != "") sql += "union ";
            if (IdEmpresas != "")
            {
                sql += "select F.IdEmpresa as IdFavorito, 'Empresa' as TipoFavorito, Empresa + '(RUC ' + RUC + ')' as Favorito, '-' as Personalizado ";
                sql += "from EmpresaFav F, Empresa_" + CodPais + " E ";
                sql += "where F.IdEmpresa = E.IdEmpresa and IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' and TipoOpe = '" + TipoOpe + "' ";
                sql += "and F.IdEmpresa in (" + IdEmpresas + ")";
            }
            cmd = new SqlCommand(sql, cn);
            dtadap = new SqlDataAdapter(cmd);
            dtaset = new DataSet();
            dtadap.Fill(dtaset);

            cn.Close();

            return dtaset.Tables[0];
        }

        public static bool ExisteCodCampaña(string CodCampaña)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;

            bool flag = false;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select count(*) as CantCodCampaña from Campaña where CodCampaña = '" + CodCampaña + "'";

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            if (dtr.Read())
                flag = Convert.ToInt32(dtr["CantCodCampaña"]) > 0;
            dtr.Close();

            cn.Close();

            return flag;
        }

        public static string ObtieneOrigen(string IdUsuario)
        {
            string sql;
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader dtr;

            string Origen;

            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["RemoteSystem"].ConnectionString;
            cn.Open();

            sql = "select Valor as [Origen] from Usuario U, AdminValor A where U.IdOrigen = A.IdAdminValor and IdUsuario = " + IdUsuario;

            cmd = new SqlCommand(sql, cn);
            dtr = cmd.ExecuteReader();
            dtr.Read();
            Origen = dtr["Origen"].ToString();
            dtr.Close();

            cn.Close();

            return Origen;
        }

        // Ruben 202212
        public static void Log(string Line)
        {
            Line = DateTime.Now.ToString() + " - " + Line;

            string LogFile = "~/App_Data/DebugLog.txt";

            LogFile = System.Web.Hosting.HostingEnvironment.MapPath(LogFile);
            
            System.IO.StreamWriter sw = new System.IO.StreamWriter(LogFile, true);

            sw.WriteLine(Line);
            
            sw.Close();
        }
    }
}