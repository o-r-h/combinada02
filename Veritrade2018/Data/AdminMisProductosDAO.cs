using System;
using System.Collections;
using System.Data;
using System.Reflection;
using Veritrade2018.Helpers;

namespace Veritrade2018.Data
{
    public class AdminMisProductosDAO
    {
        public static DataTable GetDataFavoritosUnicos(string idUsuario,  string tipoOpe,  string codPais, 
            string tipoFavorito, string txtNandinaF, string txtPartidaF, 
            int indexCboGruposF, string valueCboGruposF , string idioma,
            int page, int maximumRows, bool paginar= true)
        {

            string codPaisT = codPais;
            if (codPais.Substring(0, 2) == "UE")
                codPaisT = "UE";

            string lPartida = "Partida";
            if (idioma == "en")
                lPartida = "Partida_en";

            string sql = "select 'F' + convert(varchar(10), F.IdFavorito) as IdPartida, Nandina, " + lPartida +
                  " as Partida, ";
            sql +=
                "case when PF.IdPartida is not null and PartidaFav is not null then PartidaFav when PF.IdPartida is not null and PartidaFav is null then " +
                lPartida + " else null end as PartidaFav, ";
            sql += "case when PF.IdPartida is not null then 'S' else 'N' end as FlagIndividual ";

            if (paginar)
            {
                sql += ", ROW_NUMBER() OVER (ORDER BY Nandina ) AS Nro ";
            }

            sql += "from V_FavUnicos F left join PartidaFav PF ";
            sql +=
                "on F.IdUsuario = PF.IdUsuario and F.CodPais = PF.CodPais and F.TipoOpe = PF.TipoOpe and F.IdFavorito = PF.IdPartida ";
            sql += "join Partida_" + codPaisT + " P on F.IdFavorito = P.IdPartida ";
            sql += "where F.IdUsuario = " + idUsuario + " and F.CodPais = '" + codPais + "' and F.TipoOpe = '" +
                   tipoOpe + "' ";
            sql += "and TipoFav = '" + tipoFavorito.Substring(0, 2) + "' ";
            if (txtNandinaF != "")
                sql += "and Nandina like '%" + txtNandinaF + "%' ";
            if (txtPartidaF != "")
            {
                if (idioma == "es")
                    sql += "and (Partida like '%" + txtPartidaF + "%' ";
                else
                    sql += "and (Partida_en like '%" + txtPartidaF + "%' ";

                sql += "or isnull(PartidaFav, '') like '%" + txtPartidaF + "%') ";
            }
            if (indexCboGruposF == 1)
                sql += "and IdFavorito in (select IdPartida from PartidaFav where IdUsuario = " + idUsuario +
                       " and CodPais = '" + codPais + "' and TipoOpe = '" + tipoOpe + "') ";
            if (indexCboGruposF > 1)
                sql +=
                    "and IdFavorito in (select IdFavorito from Grupo G, FavoritoGrupo F where G.IdGrupo = F.IdGrupo and G.IdGrupo = " +
                    valueCboGruposF + ") ";

            if (!paginar)
            {
                sql += " order by Nandina";
            }

            if (paginar)
            {
                return FuncionesBusiness.Lista(sql, page, maximumRows);
            }
            else
            {
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
        }

        public static int GetCantidadFavoritosUnicos(string idUsuario, string tipoOpe, string codPais,
            string tipoFavorito, string txtNandinaF, string txtPartidaF,
            int indexCboGruposF, string valueCboGruposF, string idioma)
        {
            string sql = "SELECT count(*) as Cant FROM  (";

            string codPaisT = codPais;
            if (codPais.Substring(0, 2) == "UE")
                codPaisT = "UE";

            sql += "select  F.IdFavorito ";
            sql += "from V_FavUnicos F left join PartidaFav PF ";
            sql +=
                "on F.IdUsuario = PF.IdUsuario and F.CodPais = PF.CodPais and F.TipoOpe = PF.TipoOpe and F.IdFavorito = PF.IdPartida ";
            sql += "join Partida_" + codPaisT + " P on F.IdFavorito = P.IdPartida ";
            sql += "where F.IdUsuario = " + idUsuario + " and F.CodPais = '" + codPais + "' and F.TipoOpe = '" +
                   tipoOpe + "' ";
            sql += "and TipoFav = '" + tipoFavorito.Substring(0, 2) + "' ";
            if (txtNandinaF != "")
                sql += "and Nandina like '%" + txtNandinaF + "%' ";
            if (txtPartidaF != "")
            {
                if (idioma == "es")
                    sql += "and (Partida like '%" + txtPartidaF + "%' ";
                else
                    sql += "and (Partida_en like '%" + txtPartidaF + "%' ";

                sql += "or isnull(PartidaFav, '') like '%" + txtPartidaF + "%') ";
            }
            if (indexCboGruposF == 1)
                sql += "and IdFavorito in (select IdPartida from PartidaFav where IdUsuario = " + idUsuario +
                       " and CodPais = '" + codPais + "' and TipoOpe = '" + tipoOpe + "') ";
            if (indexCboGruposF > 1)
                sql +=
                    "and IdFavorito in (select IdFavorito from Grupo G, FavoritoGrupo F where G.IdGrupo = F.IdGrupo and G.IdGrupo = " +
                    valueCboGruposF + ") ";

            sql += " ) T";

            return FuncionesBusiness.CuentaRegistros(sql);
        }

        public static DataTable GetDataGruposFavoritos(string idUsuario, string tipoOpe,  string codPais, 
            string tipoFavorito,  string idioma)
        {
            string lIndividual = "INDIVIDUAL";
            if (idioma == "en")
                lIndividual = "SINGLE";

            var tipoFav = tipoFavorito != "ImportadorExp" ? tipoFavorito.Substring(0, 2) : "IE";

            string sql ="";

            switch (tipoFavorito)
            {
                case "Partida":
                    sql = "select IdPartida as IdFavorito, 1 as Orden, 0 as IdGrupo, '[" + lIndividual + "]' as Grupo from PartidaFav ";
                    sql += "where IdUsuario = " + idUsuario + " and CodPais = '" + codPais + "' and TipoOpe = '" + tipoOpe + "'";
                    break;
                case "Importador":
                    sql = "select IdEmpresa as IdFavorito, 1 as Orden, 0 as IdGrupo, '[" + lIndividual + "]' as Grupo from EmpresaFav ";
                    sql += "where IdUsuario = " + idUsuario + " and CodPais = '" + codPais + "' and TipoOpe = 'I'";
                    break;
                case "Proveedor":
                    sql = "select IdProveedor as IdFavorito, 1 as Orden, 0 as IdGrupo, '[" + lIndividual + "]' as Grupo from ProveedorFav ";
                    sql += "where IdUsuario = " + idUsuario + " and CodPais = '" + codPais + "'";
                    break;
                case "Exportador":
                    sql = "select IdEmpresa as IdFavorito, 1 as Orden, 0 as IdGrupo, '[" + lIndividual + "]' as Grupo from EmpresaFav ";
                    sql += "where IdUsuario = " + idUsuario + " and CodPais = '" + codPais + "' and TipoOpe = 'E'";
                    break;
                case "ImportadorExp":
                    sql = "select IdImportadorExp as IdFavorito, 1 as Orden, 0 as IdGrupo, '[" + lIndividual + "]' as Grupo from ImportadorExpFav ";
                    sql += "where IdUsuario = " + idUsuario + " and CodPais = '" + codPais + "'";
                    break;
            }

            sql += "union select IdFavorito, 2 as Orden, G.IdGrupo, '[G] ' + Grupo as Grupo from Grupo G, FavoritoGrupo F ";
            sql += "where G.IdGrupo = F.IdGrupo and IdUsuario = " + idUsuario + " ";
            sql += "and CodPais = '" + codPais + "' and TipoOpe = '" + tipoOpe + "' ";
            sql += "and TipoFav = '" + tipoFav + "' order by 1, 2, 4";

            DataTable dt = new DataTable();
            try
            {
                dt = Conexion.SqlDataTable(sql);
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                dt = null;
            }
            return dt;
        }

        public static int EliminarFavoritos(ArrayList IDsSeleccionados, string tipoFavorito, string idUsuario,
            string codPais, string tipoOpe)
        {
            string idFavorito, idGrupo;
            int cant = 0;
            if (IDsSeleccionados != null && IDsSeleccionados.Count > 0)
            {
                for (int i = 0; i < IDsSeleccionados.Count; i++)
                {
                    string id = IDsSeleccionados[i].ToString();
                    if (id.Contains("-0"))
                    {
                        //IdFavorito = ID.Substring(0, ID.IndexOf("-0"));
                        EliminaFavorito(tipoFavorito, idUsuario, codPais, tipoOpe, id);
                        cant++;
                    }
                    else
                    {
                        idFavorito = id.Substring(0, id.IndexOf("-"));
                        idGrupo = id.Substring(id.IndexOf("-") + 1, id.Length - id.IndexOf("-") - 1);
                        
                        if (CantFavoritosGrupo(idGrupo) > 1)
                        {
                            EliminaFavoritoDeGrupo(idGrupo, idFavorito);
                            cant++;
                        }
                    }
                }
            }

            return cant;
        }

        public static void EliminaFavorito(string tipoFavorito, string idUsuario, string codPais, string tipoOpe,
            string idFavorito)
        {
            string sql = "";
            switch (tipoFavorito)
            {
                case "Partida":
                    sql = "delete from PartidaFav where IdUsuario = " + idUsuario + " and CodPais = '" + codPais +
                          "' and TipoOpe = '" + tipoOpe + "' and IdPartida = " + idFavorito;
                    break;
                case "Importador":
                    sql = "delete from EmpresaFav where IdUsuario = " + idUsuario + " and CodPais = '" + codPais +
                          "' and TipoOpe = 'I' and IdEmpresa = " + idFavorito;
                    break;
                case "Proveedor":
                    sql = "delete from ProveedorFav where IdUsuario = " + idUsuario + " and CodPais = '" + codPais +
                          "' and IdProveedor = " + idFavorito;
                    break;
                case "Exportador":
                    sql = "delete from EmpresaFav where IdUsuario = " + idUsuario + " and CodPais = '" + codPais +
                          "' and TipoOpe = 'E' and IdEmpresa = " + idFavorito;
                    break;
                case "ImportadorExp":
                    sql = "delete from ImportadorExpFav where IdUsuario = " + idUsuario + " and CodPais = '" + codPais +
                          "' and IdImportadorExp = " + idFavorito;
                    break;
            }

            try
            {
                Conexion.SqlExecute(sql);
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }
        }

        public static int CantFavoritosGrupo(string IdGrupo)
        {
            string sql = "select count(*) as Cant from FavoritoGrupo where IdGrupo = " + IdGrupo;

            int cant = 0;
            try
            {
                var dt = Conexion.SqlDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    cant = Convert.ToInt32((dt.Rows[0])["Cant"]);
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                cant = -1;
            }

            return cant;

        }

        public static void EliminaFavoritoDeGrupo(string idGrupo, string idFavorito)
        {
            string sql = "delete from FavoritoGrupo where IdGrupo = " + idGrupo + " and IdFavorito = " + idFavorito;
            try
            {
                Conexion.SqlReturn(sql);
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }
        }

        public static string GetNamePais(string idioma, string codPais)
        {
            string DescPais = "";
            string sql = "";

            if (idioma == "es")
            {
                sql = "SELECT Descripcion FROM VARIABLEGENERAL WHERE IdVariable = '" + codPais + "'";
            }
            else
            {
                sql = "SELECT Descripcion = Descripcion_Eng FROM VARIABLEGENERAL WHERE IdVariable = '" + codPais + "'";
            }
            DataTable dt = new DataTable();

            try
            {
                dt = Conexion.SqlDataTable(sql);
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                dt = null;
            }

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    DescPais = row.GetValue<string>("Descripcion");
                }
            }

            return DescPais;
        }
    }
}