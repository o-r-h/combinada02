using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using Veritrade2018.Helpers;

namespace Veritrade2018.Data
{
    public class AdminMyGroupsDAO
    {
        public static DataTable GetDataGroups(string idUsuario, string tipoOpe, string codPais,
            string tipoFavorito, int page, int maximumRows, bool paginar = true)
        {
            var auxTipoFavorito = tipoFavorito != "ImportadorExp" ? tipoFavorito.Substring(0, 2).ToUpper() : "IE";
            
            string sql = "select 'G' + convert(varchar(10), G.IdGrupo) as IdGrupo, Grupo, count(*) as CantidadFavoritos ";

            if (paginar)
            {
                sql += " , ROW_NUMBER() OVER (ORDER BY Grupo ) AS Nro ";
            }

            sql += "from Grupo G, FavoritoGrupo F ";
            sql += "where G.IdGrupo = F.IdGrupo and IdUsuario = " + idUsuario+ " and CodPais = '" + codPais+ "' ";
            sql += "and TipoOpe = '" + tipoOpe + "' and TipoFav = '" + auxTipoFavorito+ "' ";
            sql += "group by G.IdGrupo, Grupo ";
 

            if (!paginar)
            {
                sql += "order by Grupo";
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

        public static int GetQuantityGroups(string idUsuario, string tipoOpe, string codPais,
            string tipoFavorito)
        {
            var auxTipoFavorito = tipoFavorito != "ImportadorExp" ? tipoFavorito.Substring(0, 2).ToUpper() : "IE";

            string sql = "SELECT count(*) as Cant FROM  (";

            sql += " select G.IdGrupo ";
            sql += "from Grupo G, FavoritoGrupo F ";
            sql += "where G.IdGrupo = F.IdGrupo and IdUsuario = " + idUsuario + " and CodPais = '" + codPais + "' ";
            sql += "and TipoOpe = '" + tipoOpe + "' and TipoFav = '" + auxTipoFavorito + "' ";
            sql += "group by G.IdGrupo, Grupo ";

            sql += " ) T";

            return FuncionesBusiness.CuentaRegistros(sql);
        }

        public static DataTable GetDataFavoritesByGroup(string idGrupo, string codPais, string tipoFavorito,
            bool existeRuc, string idioma, int page, int maximumRows, bool paginar = true)
        {
            string auxTipoFavorito = tipoFavorito.Substring(0, 2).ToUpper();
            if (tipoFavorito == "ImportadorExp")
                auxTipoFavorito = "IE";

            string sql = "";
            switch (auxTipoFavorito)
            {
                case "PA":
                    string lPartida = "Partida";
                    if (idioma == "en")
                        lPartida = "Partida_en";
                    sql = "select 'F' + convert(varchar(10), IdFavorito) as IdFavorito, Nandina, " + lPartida +
                          " as Favorito, null as RUC ";

                    if (paginar)
                    {
                        sql += " , ROW_NUMBER() OVER (ORDER BY Nandina ) AS Nro ";
                    }

                    sql += " from FavoritoGrupo F, Partida_" + codPais + " P ";
                    sql += "where IdFavorito = IdPartida and IdGrupo = " + idGrupo + " ";

                    if (!paginar)
                    {
                        sql += "order by Nandina";
                    }
                    break;
                case "IM":
                    sql = "select 'F' + convert(varchar(10), F.IdFavorito) as IdFavorito, Empresa as Favorito, " +
                          (existeRuc ? "RUC" : "null as RUC") + ", null as Nandina ";

                    if (paginar)
                    {
                        sql += " , ROW_NUMBER() OVER (ORDER BY Empresa ) AS Nro ";
                    }

                    sql +=" from FavoritoGrupo F, Empresa_" +codPais + " E ";
                    sql += "where IdFavorito = IdEmpresa and IdGrupo = " + idGrupo + " ";
                    
                    if (!paginar)
                    {
                        sql += "order by Empresa";
                    }

                    break;
                case "PR":
                    sql ="select 'F' + convert(varchar(10), F.IdFavorito) as IdFavorito, Proveedor as Favorito, null as Nandina, null as RUC ";

                    if (paginar)
                    {
                        sql += " , ROW_NUMBER() OVER (ORDER BY Proveedor ) AS Nro ";
                    }

                    sql +=" from FavoritoGrupo F, Proveedor_" +codPais + " P ";
                    sql += "where IdFavorito = IdProveedor and IdGrupo = " + idGrupo + " ";
                    
                    if (!paginar)
                    {
                        sql += "order by Proveedor";
                    }

                    break;
                case "EX":
                    sql = "select 'F' + convert(varchar(10), F.IdFavorito) as IdFavorito, Empresa as Favorito, " +
                          (existeRuc ? "RUC" : "null as RUC") + ", null as Nandina ";

                    if (paginar)
                    {
                        sql += " , ROW_NUMBER() OVER (ORDER BY Empresa ) AS Nro ";
                    }

                    sql += " from FavoritoGrupo F, Empresa_" +codPais + " E ";
                    sql += "where IdFavorito = IdEmpresa and IdGrupo = " + idGrupo + " ";
                    
                    if (!paginar)
                    {
                        sql += "order by Empresa";
                    }
                    break;
                case "IE":
                    sql ="select 'F' + convert(varchar(10), F.IdFavorito) as IdFavorito, ImportadorExp as Favorito, null as Nandina, null as RUC ";

                    if (paginar)
                    {
                        sql += " , ROW_NUMBER() OVER (ORDER BY ImportadorExp ) AS Nro ";
                    }

                    sql +=" from FavoritoGrupo F, ImportadorExp_" +codPais + " I ";
                    sql += "where IdFavorito = IdImportadorExp and IdGrupo = " + idGrupo + " ";
                    
                    if (!paginar)
                    {
                        sql += "order by ImportadorExp";
                    }
                    break;
                case "DI":
                    sql =
                        "select 'F' + convert(varchar(10), F.IdFavorito) as IdFavorito, Distrito as Favorito, null as Nandina, null as RUC ";

                    if (paginar)
                    {
                        sql += " , ROW_NUMBER() OVER (ORDER BY Distrito ) AS Nro ";
                    }

                    sql += " from FavoritoGrupo F, Distrito_US D ";
                    sql += "where IdFavorito = IdDistrito and IdGrupo = " + idGrupo + " ";
                    
                    if (!paginar)
                    {
                        sql += "order by Distrito";
                    }
                    break;
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

        public static int GetQuantityFavoritesByGroup(string idGrupo, string codPais, string tipoFavorito)
        {
            string auxTipoFavorito = tipoFavorito.Substring(0, 2).ToUpper();
            if (tipoFavorito == "ImportadorExp")
                auxTipoFavorito = "IE";

            string sql = "SELECT count(*) as Cant FROM  ( ";
            
            switch (auxTipoFavorito)
            {
                case "PA":
                    sql += "select  IdFavorito from FavoritoGrupo F, Partida_" + codPais + " P ";
                    sql += "where IdFavorito = IdPartida and IdGrupo = " + idGrupo + " ";
                    break;
                case "IM":
                    sql += "select  F.IdFavorito from FavoritoGrupo F, Empresa_" +codPais + " E ";
                    sql += "where IdFavorito = IdEmpresa and IdGrupo = " + idGrupo + " ";
                    break;
                case "PR":
                    sql +="select F.IdFavorito from FavoritoGrupo F, Proveedor_" +codPais + " P ";
                    sql += "where IdFavorito = IdProveedor and IdGrupo = " + idGrupo + " ";
                    break;
                case "EX":
                    sql += "select F.IdFavorito from FavoritoGrupo F, Empresa_" +codPais + " E ";
                    sql += "where IdFavorito = IdEmpresa and IdGrupo = " + idGrupo + " ";
                    break;
                case "IE":
                    sql +="select  F.IdFavorito from FavoritoGrupo F, ImportadorExp_" +codPais + " I ";
                    sql += "where IdFavorito = IdImportadorExp and IdGrupo = " + idGrupo + " ";
                    break;
                case "DI":
                    sql +="select F.IdFavorito from FavoritoGrupo F, Distrito_US D ";
                    sql += "where IdFavorito = IdDistrito and IdGrupo = " + idGrupo + " ";
                    break;
            }

            sql += " ) T";

            return FuncionesBusiness.CuentaRegistros(sql);
        }

        public static void UpdateGroup(string idGroup, string textGroup)
        {
            string sql = "update Grupo set Grupo = '" + textGroup + "' where IdGrupo = " + idGroup;

            try
            {
                Conexion.SqlExecute(sql);
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }
        }

        public static void DeleteGroup(string idGroup)
        {
            string sql = "delete from FavoritoGrupo where IdGrupo = " + idGroup + " ; "+ " delete from Grupo where IdGrupo = " + idGroup+ " ;";

            try
            {
                Conexion.SqlExecute(sql);
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }
        }

        public static void DeleteGroups(ArrayList idsSelected)
        {
            if (idsSelected != null && idsSelected.Count > 0)
                foreach (string t in idsSelected)
                    DeleteGroup(t);
        }

        public static void DeleteFavoritesOfGroup(ArrayList idsSelected, string idGroup)
        {
            string[] idsFavorites = new string[idsSelected.Count];

            int index = 0;
            foreach (string id in idsSelected)
            {
                idsFavorites[index] = id;
                index++;
            }
            string auxIdsFavorites = string.Join(",", idsFavorites);

            string sql = "DELETE FROM FavoritoGrupo where IdGrupo = " + idGroup + " AND IdFavorito IN ( " +
                         auxIdsFavorites + " )";

            try
            {
                Conexion.SqlExecute(sql);
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }
            
        }
    }
}