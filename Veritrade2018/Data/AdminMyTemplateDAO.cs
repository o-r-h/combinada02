using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Transactions;
using Veritrade2018.Helpers;

namespace Veritrade2018.Data
{
    public class AdminMyTemplateDAO
    {
        private static string CamposExpressImpo = "('Nandina', 'Partida', 'Aduana', 'FechaNum', 'RUC', 'Importador', 'Proveedor', 'PesoNeto', 'Cantidad', 'Unidad', 'FOBTot', 'CIFTot', 'FOBUnit', 'CIFUnit', 'PaisOrigen', 'DesComercial') ";
        private static string CamposExpressExpo = "('Nandina', 'Partida', 'Aduana', 'FechaNum', 'RUC', 'Exportador', 'PesoNeto', 'Cantidad', 'Unidad', 'FOBTot', 'FOBUnit', 'PaisDestino', 'DesComercial') ";

        public static DataTable GetDataDownloads(string idUsuario, string tipoOpe, string codPais2, 
            string codPais, string idioma)
        {
            string lTodos = "TODOS LOS CAMPOS";
            if (idioma == "en")
                lTodos = "ALL FIELDS";

            string CodPaisT = codPais;
            if (codPais2 == "4UE")
                CodPaisT = "UE" + codPais;

            string sql = "select 0 as IdDescargaCab, '[" + lTodos + "]' as Descarga ";
            sql += "union ";
            sql += "select IdDescargaCab, case when FlagDefault = 'S' then Descarga + ' [" + Resources.MiPerfil.Default_Text + "]' else Descarga end as Descarga ";
            sql += "from DescargaCab where IdUsuario = " + idUsuario + " and CodPais = '" + CodPaisT + "' and TipoOpe = '" + tipoOpe + "' order by 1";

            DataTable dt = new DataTable();
            try
            {
                dt = Conexion.SqlDataTable(sql);
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }
            return dt;
        }

        public static DataTable GetDataFields(string tipoOpe, string codPais2, string codPais,
            string plan, string idioma)
        {
            string codPaisT = codPais;
            if (codPais2 == "4UE")
                codPaisT = "UE";

            string campoFav = "CampoFav";
            if (idioma == "en")
                campoFav = "CampoFav_en";

            string sql = "select NroCampo, Campo, " + campoFav + " as CampoFavorito from DescargaDet2 D ";
            sql += "where CodPais = '" + codPaisT + "' and TipoOpe = '" + tipoOpe + "' ";

            if (plan == "ESENCIAL")
                if (tipoOpe== "I")
                    sql += "and Campo in " + CamposExpressImpo;
                else
                    sql += "and Campo in " + CamposExpressExpo;

            sql += "order by NroCampo";

            DataTable data = new DataTable();
            try
            {
                data = Conexion.SqlDataTable(sql);
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                Console.WriteLine(ex);
                throw;
            }

            return data;
        }

        public static DataTable GetDataFieldsFavorites(string tipoOpe, string codPais2, string codPais, string codPlantilla, string idioma)
        {
            string CodPaisT = codPais;
            if (codPais2== "4UE")
                CodPaisT = "UE";

            string sql;
            if (codPlantilla== "0")
            {
                string CampoFav = "CampoFav";
                if (idioma == "en")
                    CampoFav = "CampoFav_en";
                sql = "select Campo, " + CampoFav + " as CampoFavorito from DescargaDet2 ";
                sql += "where CodPais = '" + CodPaisT + "' and TipoOpe = '" + tipoOpe + "'";
            }
            else
            {
                sql = "select Campo, CampoFav as CampoFavorito from DescargaCab C, DescargaDet D ";
                sql += "where C.IdDescargaCab = D.IdDescargaCab and C.IdDescargaCab = " + codPlantilla;
            }

            DataTable data = new DataTable();
            try
            {
                data = Conexion.SqlDataTable(sql);
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                Console.WriteLine(ex);
                throw;
            }

            return data;

        }

        public static string CreateTemplate(string idUsuario, string tipoOpe, string codPais, 
            string download,  bool Default = false)
        {
           
            SqlDataReader dr;
            string idDescargaCab = "";

            string flagDefault = "null";
            if (Default)
                flagDefault = "'S'";
            try
            {
                string sql = "";
                SqlCommand cmd = null;
                using (TransactionScope scope = new TransactionScope())
                {
                    using (SqlConnection oConnection = Conexion.ObtenerConexion())
                    {
                        if (oConnection.State == ConnectionState.Closed)
                            oConnection.Open();

                        if (Default)
                        {
                            sql = "update DescargaCab set FlagDefault = null where IdUsuario = " + idUsuario + " and CodPais = '" + codPais + "' and TipoOpe = '" + tipoOpe + "' ";
                            cmd = new SqlCommand(sql, oConnection);
                            cmd.ExecuteNonQuery();
                        }

                        sql = "select max(IdDescargaCab) + 1 as IdDescargaCab from DescargaCab where IdDescargaCab >= 101";
                        cmd = new SqlCommand(sql, oConnection);
                        dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            idDescargaCab = dr["IdDescargaCab"].ToString() != "" ? dr["IdDescargaCab"].ToString() : "101";
                        }
                        dr.Close();


                        sql = "insert into DescargaCab(IdDescargaCab, IdUsuario, CodPais, TipoOpe, Descarga, FlagDefault) ";
                        sql += "values (" + idDescargaCab + "," + idUsuario + ", '" + codPais + "','" + tipoOpe + "', '" + download + "', " + flagDefault + ")";

                        cmd = new SqlCommand(sql, oConnection);
                        cmd.ExecuteNonQuery();
                    }

                    scope.Complete();
                }
            }
            catch (TransactionAbortedException ex)
            {
                //writer.WriteLine("TransactionAbortedException Message: {0}", ex.Message);
            }
            catch (ApplicationException ex)
            {
                //writer.WriteLine("ApplicationException Message: {0}", ex.Message);
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }

            return idDescargaCab;
        }

        public static void UpdateTemplate(string idUsuario, string tipoOpe, string codPais, 
            string download, string codPlantilla, bool Default = false)
        {
            string flagDefault = "null";
            if (Default)
                flagDefault = "'S'";

            try
            {
                string sql = "";
                SqlCommand cmd = null;
                using (TransactionScope scope = new TransactionScope())
                {
                    using (SqlConnection oConnection = Conexion.ObtenerConexion())
                    {
                        if (oConnection.State == ConnectionState.Closed)
                            oConnection.Open();

                        if (Default)
                        {
                            sql = "update DescargaCab set FlagDefault = null where IdUsuario = " + idUsuario + " and CodPais = '" + codPais + "' and TipoOpe = '" + tipoOpe + "' ";
                            cmd = new SqlCommand(sql, oConnection);
                            cmd.ExecuteNonQuery();
                        }

                        sql = "update DescargaCab set Descarga = '" + download + "', FlagDefault = " + flagDefault + " where IdDescargaCab = " + codPlantilla;
                        cmd = new SqlCommand(sql, oConnection);
                        cmd.ExecuteNonQuery();
                    }

                    scope.Complete();
                }
            }
            catch (TransactionAbortedException ex)
            {
                //writer.WriteLine("TransactionAbortedException Message: {0}", ex.Message);
            }
            catch (ApplicationException ex)
            {
                //writer.WriteLine("ApplicationException Message: {0}", ex.Message);
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }
        }

        public static void SaveFieldsUpdate(string codPlantilla, string fieldsTemplates, string checkFieldsTemplates,
            string fieldsCustomizeTemplates)
        {
            string[] arrayFieldsTemplate = fieldsTemplates.Split('¬');
            string[] arrayCheckFieldsTemplate = checkFieldsTemplates.Split('¬');
            string[] arrayFieldsCustomizeTemplate = fieldsCustomizeTemplates.Split('¬');

            int nroCampo = 0;
            for (int i = 0; i < arrayFieldsTemplate.Length; i++)
            {
                if (Convert.ToBoolean(arrayCheckFieldsTemplate[i]))
                {
                    nroCampo += 1;
                    if (ExistDescargaDet(codPlantilla, arrayFieldsTemplate[i]))
                    {
                        UpdateDescargaDet(arrayFieldsCustomizeTemplate[i], codPlantilla, arrayFieldsTemplate[i]);
                    }
                    else
                    {
                        CreateDescargaDet(codPlantilla, nroCampo, arrayFieldsTemplate[i],arrayFieldsCustomizeTemplate[i]);
                    }
                }
                else
                {
                    DeleteDescargaDet(codPlantilla,arrayFieldsTemplate[i]);
                }
            }
        }

        public static void SaveFieldsNewTemplate(string codPlantilla, string fieldsTemplates,
            string checkFieldsTemplates,
            string fieldsCustomizeTemplates)
        {
            string[] arrayFieldsTemplate = fieldsTemplates.Split('¬');
            string[] arrayCheckFieldsTemplate = checkFieldsTemplates.Split('¬');
            string[] arrayFieldsCustomizeTemplate = fieldsCustomizeTemplates.Split('¬');

            int nroCampo = 0;
            for (int i = 0; i < arrayFieldsTemplate.Length; i++)
            {
                if (Convert.ToBoolean(arrayCheckFieldsTemplate[i]))
                {
                    nroCampo += 1;
                    if (ExistDescargaDet(codPlantilla, arrayFieldsTemplate[i]))
                    {
                        UpdateDescargaDet(arrayFieldsCustomizeTemplate[i], codPlantilla, arrayFieldsTemplate[i]);
                    }
                    else
                    {
                        CreateDescargaDet(codPlantilla, nroCampo, arrayFieldsTemplate[i], arrayFieldsCustomizeTemplate[i]);
                    }
                }
            }
        }

        public static void UpdateDescargaDet(string campoFavorito, string idDescargaCab, string campo)
        {
            string sql = "update DescargaDet set CampoFav = '" + campoFavorito+ "' where IdDescargaCab = " + idDescargaCab + " and Campo = '" + campo+ "' ";

            try
            {
                Conexion.SqlExecute(sql);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static bool ExistDescargaDet(string idDescargaCab, string campo)
        {
            int numCoincidencias = -1;
            try
            {
                string sql = "select count(*) as Cant from DescargaDet where IdDescargaCab = " + idDescargaCab+ " and Campo = '" + campo+ "' ";
                var dt = Conexion.SqlDataTable(sql);

                if (dt.Rows.Count > 0)
                {
                    numCoincidencias = Convert.ToInt32(dt.Rows[0]["Cant"]);
                }

            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                throw;
            }

            return numCoincidencias != 0;
        }

        public static void CreateDescargaDet(string idDescargaCab, int nroCampo, string campo,
            string campoFavorito)
        {

            string sql = "insert into DescargaDet(IdDescargaCab, NroCampo, Campo, CampoFav) values ";
            sql += "(" + idDescargaCab+ ", " + nroCampo+ ", '" + campo+ "', '" + campoFavorito+ "')";

            try
            {
                Conexion.SqlExecute(sql);
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                Console.WriteLine(ex);
                throw;
            }
        }

        public static void DeleteDescargaDet(string idDescargaCab, string campo)
        {
            string sql = "delete from DescargaDet where IdDescargaCab = " + idDescargaCab+ " and Campo = '" + campo+ "' ";
            try
            {
                Conexion.SqlExecute(sql);
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                Console.WriteLine(ex);
                throw;
            }
        }

        public static bool VlidationTemplate(string idUsuario, string codPais, string tipoOpe)
        {
            string idPlan = Funciones.ObtieneIdPlan(idUsuario);
            int limitePlantillas =0;
            int numPlantillasDeUsuario=0;
            try
            {
                string sql = "select LimitePlantillas from [Plan] where IdPlan = " + idPlan;
                var dt = Conexion.SqlDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    limitePlantillas = Convert.ToInt32(dt.Rows[0]["LimitePlantillas"]);
                }

                sql = "select count(*) as Plantillas from DescargaCab ";
                sql += "where IdUsuario = " + idUsuario+ " and CodPais = '" + codPais + "' and TipoOpe = '" + tipoOpe + "'";

                var dtTemplatesUsuario = Conexion.SqlDataTable(sql);

                if (dtTemplatesUsuario.Rows.Count > 0)
                {
                    numPlantillasDeUsuario = Convert.ToInt32(dtTemplatesUsuario.Rows[0]["Plantillas"]);
                }

            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                Console.WriteLine(ex);
                throw;
            }

            return numPlantillasDeUsuario < limitePlantillas;
        }

        public static void DeleteDescargas(string idDescargaCab)
        {
            string sqlDescargaDet = string.Format(@"Delete DescargaDet where IdDescargaCab = {0}", idDescargaCab);

            string sqlDescargaCab = string.Format(@"Delete DescargaCab where IdDescargaCab = {0}", idDescargaCab);

            try
            {
                Conexion.SqlExecute(sqlDescargaDet);

                Conexion.SqlExecute(sqlDescargaCab);
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}