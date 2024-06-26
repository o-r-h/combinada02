﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Veritrade2018.Models;
using Veritrade2018.Util;
using Dapper;
using System.Reflection;
using System.Data.SqlClient;

namespace Veritrade2018.Helpers
{
    public class FuncionesBusiness
    {

        public static List<SelectListItem> SearchAllFavoriteAlert(string culture, string idUsuario, string filtro,string codigoPais , string TipoOpe,string codPais2, string excludePais = null, bool isCombo = false,
            string txtDescripcion = "")
        {
            List<SelectListItem> obj = new List<SelectListItem>();

            string validar = "''";
            string IdFavorito = "";
            string tabla = "";
            string campo1 = "";
            var _k = new VarValues();
            if (!string.IsNullOrEmpty(excludePais))
            {
                _k = VarGeneral.Instance.ValuesDict[excludePais];
            }

            if (filtro == "PA")
            {
                IdFavorito = "IdPartida";
                tabla = "PartidaFav";
                validar = $"CodPais not in('{_k.Valores.Replace(",", "\',\'")}')";
                campo1 = ", ISNULL(PartidaFav,'') AS PartidaFav";
            }
            else if (filtro == "IM\',\'EX")
            {
                IdFavorito = "IdEmpresa";
                //tabla = "EmpresaFav";
                tabla = $@"Empresa_{codigoPais} E
                            INNER JOIN V_FavUnicos F on f.IdFavorito = E.IdEmpresa ";
                string[] _exc = null;
                string paises = "";
                if (_k != null)
                    _exc = _k.Valores.Split(',');

                var _v = (from e in VarGeneral.Instance.Values
                    where e.IdGrupo == Enums.VarGrupo.SUB_REGION.GetDn()
                          && e.IdParent == "1LAT"
                          && ((_exc != null && !_exc.Contains(e.IdVariable)) || _exc == null)
                    select e).ToList();
                _v.ForEach(i => paises += i.IdVariable + "','");
                var tip = TipoOpe == "I" ? "IM" : "EX";
                validar = $"CodPais in('{paises}') and TipoFav='{tip}'";

            }

            if(codPais2 == "4UE")
            {
                codigoPais = "UE" + codigoPais;
            }

            string sql = $@" SELECT CodPais , { IdFavorito } as IdFavorito, TipoOpe {campo1} FROM { tabla } 
                                            where IdUsuario = '{idUsuario}' and {validar} and CodPais = '{codigoPais}' and TipoOpe = '{TipoOpe}'" ;

            //Console.WriteLine(sql);

            int x = 0;
            var dt = Conexion.SqlDataTable(sql);
            //string codPaisAnt = "" ,codPaisAntAux = "";
            string tipoOpe = "";
            validar = "";
            foreach (DataRow row in dt.Rows)
            {

                string codPais = row["CodPais"].ToString();
                if (row["IdFavorito"].ToString() == "4284108")
                {
                    var asdx = "";
                }
                string idFavorito = row["IdFavorito"].ToString();
                tipoOpe = row["TipoOpe"].ToString();
                // Ruben 202210
                if (codPais != "MXD" && codPais != "MXM" && codPais2 != "4UE")
                {
                    codPais = codPais.Substring(0, 2);
                }

                //if (codPaisAnt != codPais && codPaisAnt != "")
                //{
                if (!string.IsNullOrEmpty(campo1))
                {
                    campo1 = row["PartidaFav"].ToString();
                }

                DescripcionPorId2(culture, filtro, codPais, idFavorito, obj, codPais2, isCombo, tipoOpe, txtDescripcion, campoFav:campo1);

                //    validar = "";
                //}

                //codPaisAnt = codPais;
                //validar += idFavorito + "','";
                //string descripcion = DescripcionPorId(filtro, codPais, idFavorito);


                /*if (descripcion.Length > 60)
                {
                    descripcion = descripcion.Substring(0, 60);
                }
                if(descripcion != "0")
                    obj.Add(new SelectListItem { Text = descripcion, Value = codPais + "-" + idFavorito});*/

            }

            //DescripcionPorId2(filtro, codPaisAnt, validar, obj,isCombo,tipoOpe);
            obj = obj.OrderBy(y => y.Text).ToList();
            return obj;
        }

        public static List<SelectListItem> SearchMyFavoriteAlert(string culture, string idUsuario, string filtro, string tipoFiltro, string codPais , string TipoOpe, string codPais2)
        {
            List<SelectListItem> obj = new List<SelectListItem>();

            if(codPais2 == "4UE")
            {
                codPais = "UE" + codPais;
            }

            string sql = $@" SELECT IdValor FROM dbo.AlertaPreferencias IdTipoVariable 
                                             WHERE IdTipoAlerta = '{tipoFiltro}' AND IdUsuario = {idUsuario} and CodPais = '{codPais}' AND idTipoOpe = '{TipoOpe}'";

            var dt = Conexion.SqlDataTable(sql);
            string codPaisAnt = "", codPaisAntAux = "";

            foreach (DataRow row in dt.Rows)
            {
                if (tipoFiltro.Equals("AMP") || tipoFiltro.Equals("AMC"))
                {
                    DescripcionPorId2(culture, filtro, codPais, row["IdValor"].ToString(), obj, codPais2, true, TipoOpe);
                }
                else
                {
                    var descripcion = row["IdValor"].ToString();
                    var value = row["IdValor"].ToString();
                    obj.Add(new SelectListItem { Text = descripcion, Value = value });
                }

            }

            return obj;
        }

        public static string SearchLangUsuario(string idUsuario)
        {
            string idioma = "";

            string sql = $@" select isnull(idioma,'ESP') as idioma from usuario where idUsuario = '{idUsuario}'";
            var dt = Conexion.SqlDataTable(sql);

            foreach (DataRow row in dt.Rows)
            {
                idioma = row["idioma"].ToString();
            }

            return idioma;
        }

        // Ruben 202311
        public static bool IsVisibleSentinal(string idPlan)
        {
            string flag = "";
            string sql = @"SELECT FlagSentinel FROM [dbo].[Plan] where IdPlan = @idPlan";

            using (var db = new ConexProvider().Open)
            {
                dynamic v = db.Query(sql, new { idPlan = idPlan }).FirstOrDefault();

                flag = v.FlagSentinel;
            }

            flag = "N"; // Ruben 202311

            return flag == "S";

        }

        // Ruben 202311
        public static bool IsVisibleSentinal2023(string idPlan)
        {
            string flag = "";
            string sql = @"SELECT FlagSentinel FROM [dbo].[Plan] where IdPlan = @idPlan";

            using (var db = new ConexProvider().Open)
            {
                dynamic v = db.Query(sql, new { idPlan = idPlan }).FirstOrDefault();

                flag = v.FlagSentinel;
            }

            return flag == "S";

        }

        public static List<String> SearchCountryInfoComplementario(String codPais)
        {
            List<String> obj = new List<String>();
            string sql = $@"SELECT ID_PAIS_ORIGEN,
                                   ID_PAIS_COMPLEMENTARIO,
                                   COD_PAIS_COMPLEMENTARIO
                            FROM paises_complementarios
                            WHERE ID_PAIS_ORIGEN = '{codPais}'";

            var dt = Conexion.SqlDataTable(sql);

            foreach (DataRow row in dt.Rows)
            {

                obj.Add(row["COD_PAIS_COMPLEMENTARIO"].ToString());

            }

            return obj;

        }


        public static List<SelectListItem> SearchCountryAlert()
        {
            List<SelectListItem> obj = new List<SelectListItem>();
            /* string sql = @"select distinct 
                           CASE when substring(CodPais,3,1) in ('I','E') then SUBSTRING(CodPais,0,3) + '_'
                           ELSE CodPais
                           END as CodPais,
                           vg.Descripcion
                          FROM [dbo].[AlertaPreferencias] ap , VariableGeneral vg
                          WHERE case when substring(CodPais,3,1) in ('I','E') then SUBSTRING(CodPais,0,3) + '_'
                           else CodPais
                           end  = vg.IdVariable
                         UNION 
                             SELECT DISTINCT
                                             CodPais,
                                            pue.Pais	 as Descripcion  
                                     FROM [dbo].[AlertaPreferencias] ap,
                                     PaisUEImpExp pue
                                     WHERE SUBSTRING(CodPais, 0, 3) = 'UE'
                                         and SUBSTRING(CodPais, 3, 5) = pue.IdPais";*/
            string sql = @"select distinct 
                          CASE when substring(CodPais,3,1) in ('I','E') then SUBSTRING(CodPais,0,3) + '_'
                          ELSE CodPais
                          END as CodPais,
                          vg.Descripcion
                         FROM [dbo].[AlertaPreferencias] ap , VariableGeneral vg
                         WHERE case when substring(CodPais,3,1) in ('I','E') then SUBSTRING(CodPais,0,3) + '_'
                          else CodPais
                          end  = vg.IdVariable
                        UNION 
                            SELECT DISTINCT
                                            SUBSTRING(CodPais, 0, 3),
	                                       'Unión Europea' as Descripcion  
                                    FROM [dbo].[AlertaPreferencias] ap
                                    WHERE SUBSTRING(CodPais, 0, 3) = 'UE'";

            var dt = Conexion.SqlDataTable(sql);
            obj.Add(
                new SelectListItem
                {
                    Text = "TODOS",
                    Value = ""
                });
            foreach (DataRow row in dt.Rows)
            {
                
                obj.Add(
                    new SelectListItem {
                        Text = row["Descripcion"].ToString(),
                        Value = row["CodPais"].ToString()
                    });

            }

            return obj;

        }

        public static List<SelectListItem> SearchPeriodByCountry(string codPais)
        {
            string[] arrayMes = { "Ene", "Feb", "Mar", "Abr","May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dec" };
            List<SelectListItem> obj = new List<SelectListItem>();
            //string sql = $@"select Distinct SUBSTRING(CONVERT(VARCHAR(10),AC.FECHAENVIO,112),0,7) as Fecha  from dbo.AlertaCorreos ac WHERE ESTADO = 'S' AND CodPais = '{codPais}'";
            //string sql = @"select distinct 
            //              CASE when substring(CodPais,3,1) in ('I','E') then SUBSTRING(CodPais,0,3) + '_'
            //              ELSE CodPais
            //              END as CodPais,
            //              vg.Descripcion
            //             FROM [dbo].[AlertaPreferencias] ap , VariableGeneral vg
            //             WHERE  CodPais = vg.IdVariable";

            if(codPais.Length > 0 && codPais.Substring(0,2) == "UE")
            {
                codPais = codPais.Substring(0, 2);
            }

            string sql = $@"SELECT top 1    
                                SUBSTRING(CONVERT(VARCHAR(10), FechaFin, 112), 0, 7) as Fecha
                                FROM BaseDatos
                                WHERE CodPais = '{codPais}' order by 1 desc";

            var dt = Conexion.SqlDataTable(sql);
            var anio = "";
            var mes = 0;
            obj.Add(
                new SelectListItem
                {
                    Text = "TODOS",
                    Value = ""
                });
            foreach (DataRow row in dt.Rows)
            {
                anio = row["Fecha"].ToString().Substring(0,4);
                mes = Convert.ToInt32(row["Fecha"].ToString().Substring(4,2));

                /*obj.Add(
                new SelectListItem
                {
                    Text = arrayMes[mes - 1] + " - " + anio,
                    Value = anio + (mes).ToString()
                });*/
            }

            if(anio != "")
            {
                for (int i = 1; i <= 12; i++)
                {
                    int anioAux = Convert.ToInt32(anio);
                    var day = "" + i;
                    if (i < 10)
                    {
                        day = "0" + i;
                    }
                    obj.Add(
                        new SelectListItem
                        {
                            Text = arrayMes[i - 1] + " - " + (anioAux - 1),
                            Value = (anioAux - 1) + (day).ToString()
                        });
                }
            }

            

            for (int i = 1 ; i <= mes; i++)
            {
                var day = ""+i;
                if (i < 10)
                {
                    day = "0" + i;
                }
                obj.Add(
                    new SelectListItem
                    {
                        Text = arrayMes[i - 1] + " - " + anio,
                        Value = anio + (day).ToString()
                    });
            }

            return obj;

        }

        public static bool IsVisibleFobUnit(string codPais, string tipoOpe)
        {
            int cont = 0;
            string sql = $@"SELECT COUNT(*) AS CONT FROM DescargaDet2 WHERE CAMPO IN('FobUnit','FasUnit') AND CodPais = '{codPais}' AND TipoOpe = '{tipoOpe}'";
            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                cont = row.GetValue<int>("CONT");
            }

            return cont > 0;

        }
        public static int GetLimitAlert(string idPlan, string tipoFiltro , Boolean isChildren = false)
        {
            int limit = 0;

            string campoFiltro = tipoFiltro == "AMP" ? "LimitAlertProducto" : 
                                    tipoFiltro == "AMC" ? "LimitAlertCompania" : 
                                    tipoFiltro == "APC" ? "LimitAlertCompaniaProd1" : "LimitAlertCompaniaProd1";

            if (isChildren)
                campoFiltro = tipoFiltro == "APC" ? "LimitAlertCompaniaProd2" : "LimitAlertCompaniaProd2";

            string sql = $@"SELECT ISNULL({campoFiltro},0) AS LimiteAlerta
                                FROM [dbo].[Plan] where IdPlan = @idPlan";

            using (var db = new ConexProvider().Open)
            {
                dynamic v = db.Query(sql, new { idPlan = idPlan }).FirstOrDefault();

                limit = v.LimiteAlerta;
            }          

            return limit;

        }

        public static List<string> SearchMyFavoriteAlertPorIdPadre(string idUsuario, string IdTipoAlerta, string codPais, string IdFavorito, string TipoOpe)
        {
            List<string> obj = new List<string>();

            string sql = $@" SELECT IdValor FROM dbo.AlertaPreferencias  
                                             WHERE IdTipoAlerta = '{IdTipoAlerta}' AND IdUsuario = {idUsuario} AND CodPais = '{codPais}' AND IdTipoOpe = '{TipoOpe}' AND IdValorPadre = {IdFavorito}
                                           ";
            var dt = Conexion.SqlDataTable(sql);

            foreach (DataRow row in dt.Rows)
            {
                obj.Add(row["IdValor"].ToString());

            }

            return obj;
        }

        public static List<SelectListItem> SearchMyFavoriteAlertPC(string culture, string filtro, string idUsuario, string tipoFiltro,string codPais,string TipoOpe, string codPais2)
        {
            List<SelectListItem> obj = new List<SelectListItem>();

            string sql = $@" SELECT IdValorPadre, CodPais, IdTipoOpe FROM dbo.AlertaPreferencias  
                                             WHERE IdTipoAlerta = '{tipoFiltro}' AND IdUsuario = {idUsuario} AND CodPais = '{codPais}' AND IdTipoOpe = '{TipoOpe}' AND IdValorPadre is not null
                                            GROUP BY IdValorPadre, CodPais, IdTipoOpe";
            var dt = Conexion.SqlDataTable(sql);

            foreach (DataRow row in dt.Rows)
            {
                var value = row["IdValorPadre"].ToString();
                var pais = row["CodPais"].ToString();
                var tipoOpe = row["IdTipoOpe"].ToString();

                DescripcionPorId2(culture, filtro, pais, value, obj, codPais2, false, tipoOpe);
            }
            obj = obj.OrderBy(x => x.Text).ToList();
            return obj;
        }

        public static List<SelectListItem> SearchProductOrCompanyPC(string culture, string codigo, string fav)
        {
            //VALORES 0 -> COD PAIS , 1 -> VALOR , 2 -> TIPO OPERACION


            var json = new List<SelectListItem>();
            var valores = codigo.Split('-');
            var camposMuestra = "";
            var campos1 = "";
            var campos2 = "";
            var camposInner = "";
            var camposWhere = "";
            var tipoOpe = valores[2].Equals("I") ? "Im" : "Ex";
            var tabla1 = $"{tipoOpe}portacion_{valores[0]}";//valores[2].Equals("I") ? "Importacion_" + valores[0] : "Exportacion_" + valores[0];
            var tabla2 = "";
            if (fav == "ACP")
            {
                campos1 = "IdPartida";
                campos2 = culture == "es" ? "Partida" : "Partida_en";
                camposMuestra = $" P.IdPartida, P.{campos2},P.Nandina ";
                camposInner = "p.IdPartida = CO.IdPartida";
                camposWhere = $"CO.Id{tipoOpe}portador = {valores[1]}";
                tabla2 = "Partida";
            }
            else if (fav == "APC")
            {
                campos1 = "IdEmpresa";
                campos2 = "Empresa";
                camposMuestra = "p.IdEmpresa, P.Empresa";
                camposInner = $"p.IdEmpresa = CO.Id{tipoOpe}portador";
                camposWhere = $"CO.IdPartida = {valores[1]} ";
                tabla2 = "Empresa";
            }
            else
            {
                return null;
            }

            var sql = $@"SELECT {camposMuestra} 
                            FROM {tabla1} CO
                            INNER JOIN {tabla2}_{valores[0]} P ON {camposInner}
                            WHERE {camposWhere} GROUP BY {camposMuestra} ORDER BY 2";

            try
            {
                var dt = Conexion.SqlDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        var nandina = "";
                        if (fav == "ACP")
                            nandina = $"[{dr.GetValue<string>("Nandina")}] ";
                        var campoTexto = nandina + dr.GetValue<string>(campos2);
                        if(campoTexto.Trim().Length > 63)
                        {
                            campoTexto = campoTexto.Substring(0, 60)+ "...";
                        }
                        json.Add(new SelectListItem { Text = campoTexto, Value = dr.GetValue<int>(campos1).ToString() });
                        
                    }
                }
                else
                {
                    json.Add(new SelectListItem { Text = "-", Value = "-" });
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                json.Add(new SelectListItem { Text = "-", Value = "-" });
            }

            json = json.OrderBy(x => x.Text).ToList();
            return json;
        }


        public static List<object> SearchProductOrCompany(string descripcion, string culture, string codigo, string fav)
        {
            //VALORES 0 -> COD PAIS , 1 -> VALOR , 2 -> TIPO OPERACION


            var json = new List<object>();
            var valores = codigo.Split('-');
            var camposMuestra = "";
            var campos1 = "";
            var campos2 = "";
            var camposInner = "";
            var camposWhere = "";
            var tipoOpe = valores[2].Equals("I") ? "Im" : "Ex";
            var tabla1 = $"{tipoOpe}portacion_{valores[0]}";//valores[2].Equals("I") ? "Importacion_" + valores[0] : "Exportacion_" + valores[0];
            var tabla2 = "";
            if (fav == "ACP")
            {
                campos1 = "IdPartida";
                campos2 = culture == "es" ? "Partida" : "Partida_en";
                camposMuestra = $" P.IdPartida, P.{campos2},P.Nandina ";
                camposInner = "p.IdPartida = CO.IdPartida";
                camposWhere = $"CO.Id{tipoOpe}portador = {valores[1]} AND (P.{campos2} LIKE '%{descripcion}%' OR P.Nandina like '{descripcion}%')";
                tabla2 = "Partida";
            }
            else if (fav == "APC")
            {
                campos1 = "IdEmpresa";
                campos2 = "Empresa";
                camposMuestra = "p.IdEmpresa, P.Empresa";
                camposInner = $"p.IdEmpresa = CO.Id{tipoOpe}portador";
                camposWhere = $"CO.IdPartida = {valores[1]} AND P.Empresa LIKE '%{descripcion}%'";
                tabla2 = "Empresa";
            }
            else
            {
                return null;
            }

            var sql = $@"SELECT {camposMuestra} 
                            FROM {tabla1} CO
                            INNER JOIN {tabla2}_{valores[0]} P ON {camposInner}
                            WHERE {camposWhere} GROUP BY {camposMuestra}";

            /*var tabla1 = valores[2].Equals("I") ? "Importacion_" + valores[0] : "Exportacion_" + valores[0];

            var compania = valores[2].Equals("I") ? "Importador" : "Exportador";

            var tabla2 = "Partida_" + valores[0];

            var partida = culture.Equals("es") ? "P.Partida" : "P.Partida_en";

            var sql = $@"SELECT CO.IdPartida, {partida} FROM {tabla1} CO
                        INNER JOIN {tabla2} P ON P.IdPartida = CO.IdPartida
                        WHERE CO.IdCompania = {valores[1]} AND {partida} LIKE '%{descripcion}%' GROUP BY CO.IdPartida, {partida} ";*/

            //var sql2 = $@"SELECT C.IdPartida, {partida} FROM (SELECT IdPartida, IdCompania = IdImportador, Compania = Importador FROM {tabla1}
            //            UNION ALL SELECT IdPartida, IdCompania = IdExportador, Compania = Exportador FROM {tabla2}) AS C
            //            INNER JOIN {tabla3} P ON P.IdPartida = C.IdPartida
            //            WHERE C.IdCompania = {valores[1]} AND P.Partida LIKE '%{descripcion}%' GROUP BY C.IdPartida, {partida} ";
            try
            {
                var dt = Conexion.SqlDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        var nandina = "";
                        if (fav == "ACP")
                            nandina = $"[{dr.GetValue<string>("Nandina")}] ";
                        json.Add(new
                        {
                            id = dr.GetValue<int>(campos1),
                            value = nandina + dr.GetValue<string>(campos2)
                        });
                    }
                }
                else
                {
                    json.Add(new { id = 0, value = "-" });
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                json.Add(new { id = 0, value = "-" });
            }

            return json;
        }

        public static List<SelectListItem> SearchMyFavoriteAlerDetailtPC(string culture, string filtro, string idUsuario, string tipoFiltro, string idValorPadre, string codPais2)
        {
            List<SelectListItem> obj = new List<SelectListItem>();

            string sql = $@" SELECT IdValor, CodPais FROM dbo.AlertaPreferencias IdTipoVariable 
                                             WHERE IdTipoAlerta = '{tipoFiltro}' AND IdUsuario = {idUsuario} AND IdValorPadre = {idValorPadre} ";
            var dt = Conexion.SqlDataTable(sql);

            foreach (DataRow row in dt.Rows)
            {
                var value = row["IdValor"].ToString();
                var pais = row["CodPais"].ToString();

                DescripcionPorId2(culture, filtro, pais, value, obj, codPais2, false);
            }

            return obj;
        }

        public static void GuardarAlertFavorito(string IdTipoAlerta, string idUsuario,string codPais, string idFavorito, string idValorPadre, string tipoOpe)
        {
            var sql = $@"INSERT INTO dbo.AlertaPreferencias(IdTipoAlerta,IdUsuario,CodPais,IdValor, IdValorPadre, IdTipoOpe) 
                            Values('{IdTipoAlerta}','{idUsuario}','{codPais}','{idFavorito}',{idValorPadre}, '{tipoOpe}' )";

            
            Conexion.SqlDataTable(sql);
        }

      

        public static void GrabarMisBusquedasAlerta(int limitAlert, int cantSeleccionados,string IdTipoAlerta, string IdUsuario, string CodPais, string IdFavorito, string TipoOpe, ref string mensaje, ref bool flagSave)
        {
            //if (CantAlertas(IdTipoAlerta, IdUsuario) + cantSeleccionados <= limitAlert)
            if (CantAlertasByPaisOrRegimen(IdTipoAlerta, IdUsuario, CodPais, TipoOpe) + cantSeleccionados <= limitAlert)
            {
                if (!ExisteAlerta(IdTipoAlerta, IdUsuario, IdFavorito))
                {
                    GuardarAlertFavorito(IdTipoAlerta, IdUsuario, CodPais,IdFavorito, "null", TipoOpe);
                    mensaje = Resources.AdminResources.SaveAlert_Text;//"Se registró la alerta";
                    flagSave = true;
                }
                else
                {
                    mensaje = Resources.AdminResources.ExistsAlert_Text;//"La alerta ya existe";
                }
            }
            else
            {
                mensaje = Resources.AdminResources.AlertExceed_Text;//"La cantidad máxima de alertas se excedió de acuerdo con su plan";
            }
        }

        public static bool ExisteAlerta(string IdTipoAlerta, string IdUsuario, string IdFavorito)
        {
            bool flagExiste = false;

            string sql = $@"SELECT COUNT(*) AS Cantidad FROM ALERTAPREFERENCIAS WHERE IdTipoAlerta = '{IdTipoAlerta}'
                            AND IdUsuario = {IdUsuario} AND IdValor = {IdFavorito}";

            DataTable dt;
            int cant = 0;
            try
            {
                dt = Conexion.SqlDataTable(sql);

                foreach (DataRow dr in dt.Rows)
                {
                   cant  = Convert.ToInt32(dr["Cantidad"]);
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                throw;
            }
            if (cant>0)
            {
                flagExiste = true;
            }
            //return flagExiste;
            return cant > 0;
        }

        public static int CantAlertas(string IdTipoAlerta, string IdUsuario)
        {
            int cant = 0;
            string sql = $@"SELECT COUNT(IdValor) AS Cantidad FROM ALERTAPREFERENCIAS WHERE IdTipoAlerta = '{IdTipoAlerta}'
                            AND IdUsuario = {IdUsuario}";
            DataTable dt;

            try
            {
                dt = Conexion.SqlDataTable(sql);

                foreach (DataRow dr in dt.Rows)
                {
                    cant = Convert.ToInt32(dr["Cantidad"]);
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                throw;
            }
            return cant;
        }

        public static int CantAlertasByPais(string IdTipoAlerta, string IdUsuario, string CodPais,string tipOpe , bool flagPaisExclude = true)
        {
            int cant = 0;
            string sql = $@"SELECT COUNT(IdValor) AS Cantidad FROM ALERTAPREFERENCIAS WHERE IdTipoAlerta = '{IdTipoAlerta}'
                            AND IdUsuario = {IdUsuario} ";

            string tipoAux = tipOpe == "I" ? "E" : "I";

            if (flagPaisExclude)
            {
                sql += $"AND ((CodPais <> '{CodPais}' and IdTipoOpe = '{tipOpe}') or IdTipoOpe = '{tipoAux}' )";
            }

            if (IdTipoAlerta == "ACP" || IdTipoAlerta == "APC")
            {
                sql += "and IdValorPadre is null";
            }

            DataTable dt;

            try
            {
                dt = Conexion.SqlDataTable(sql);

                foreach (DataRow dr in dt.Rows)
                {
                    cant = Convert.ToInt32(dr["Cantidad"]);
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                throw;
            }
            return cant;
        }

        public static int CantAlertasByPaisOrRegimen(string IdTipoAlerta, string IdUsuario, string CodPais, string tipOpe)
        {
            int cant = 0;
            string sql = $@"SELECT COUNT(IdValor) AS Cantidad FROM ALERTAPREFERENCIAS WHERE IdTipoAlerta = '{IdTipoAlerta}'
                            AND IdUsuario = {IdUsuario} and CodPais = '{CodPais}' and IdTipoOpe = '{tipOpe}' ";
                    
            

            if (IdTipoAlerta == "ACP" || IdTipoAlerta == "APC")
            {
                sql += "and IdValorPadre is null";
            }

            DataTable dt;

            try
            {
                dt = Conexion.SqlDataTable(sql);

                foreach (DataRow dr in dt.Rows)
                {
                    cant = Convert.ToInt32(dr["Cantidad"]);
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                throw;
            }
            return cant;
        }
        public static void DeleteAlertFavoritos(string idTipoAlerta, string idUsuario ,string codPais , string tipoOpe)
        {
            var sql = $@"DELETE FROM dbo.AlertaPreferencias WHERE IdTipoAlerta = '{idTipoAlerta}' 
                            AND IdUsuario = '{idUsuario}' AND CodPais = '{codPais}' AND IdTipoOpe = '{tipoOpe}'";

            Conexion.SqlDataTable(sql);
        }
        public static void DeleteAlertFavoritosPC(string idTipoAlerta, string idUsuario, string idValorPadre)
        {
            var sql = $@"DELETE FROM dbo.AlertaPreferencias WHERE IdTipoAlerta = '{idTipoAlerta}' 
                            AND IdUsuario = '{idUsuario}' AND IdValorPadre = {idValorPadre} ";
            var sql2 = $@"DELETE FROM dbo.AlertaPreferencias WHERE IdTipoAlerta = '{idTipoAlerta}' 
                            AND IdUsuario = '{idUsuario}' AND IdValor = {idValorPadre} ";
            Conexion.SqlDataTable(sql);
            Conexion.SqlDataTable(sql2);
        }
        public static void DeleteAlertFavoritosDetallePC(string idTipoAlerta, string idUsuario, string idFavorito, string idValorPadre)
        {
            var sql = $@"DELETE FROM dbo.AlertaPreferencias WHERE IdTipoAlerta = '{idTipoAlerta}' 
                            AND IdUsuario = '{idUsuario}' AND IdValor = {idFavorito} AND IdValorPadre = {idValorPadre} ";

            Conexion.SqlDataTable(sql);
        }
        static void DescripcionPorId2(string culture, string filtro, string codPais, string id, List<SelectListItem> obj , string codPais2 , bool isCombo = false, string tipoOpe = "", string txtDescripcion = "" , string campoFav = "")
        {
            string tabla = "Partida_" + codPais;
            string descCampo = "Partida";
            string idCampo = "idPartida";
            if (filtro == "PA")
            {
                if(codPais2 != "4UE")
                {
                    tabla = "Partida_" + codPais;
                }
                else
                {
                    tabla = "Partida_UE";
                }
                
                descCampo = culture == "es" ? "Partida" : "Partida_en";
                idCampo = "idPartida";
            }
            else if (filtro == "IM\',\'EX")
            {
                if(codPais2 != "4UE")
                {
                    tabla = "Empresa_" + codPais;
                }
                else
                {
                    tabla = "Empresa_UE";
                }
                
                descCampo = "Empresa";
                idCampo = "idEmpresa";
            }
            
            var sql = $@"Select * from {tabla} WHERE {idCampo} in ({id})";

            if(txtDescripcion != "")
            {
                if(filtro == "PA")
                {
                    sql += " and (Nandina like '%" + txtDescripcion + "%' or "+ descCampo + " LIKE '%"+txtDescripcion+"%')";
                }
                else
                {
                    sql += " and "+ descCampo+" like '%" + txtDescripcion + "%'";
                }
            }

            var dt = Conexion.SqlDataTable(sql);
            //descripcion = dt.Rows.Count.ToString();   

            //if(dt.Rows.Count == 0) obj.Add(new SelectListItem { Text = sql, Value = sql });

            foreach (DataRow row in dt.Rows)
            {
                string descripcion ;
                if (campoFav == "")
                    descripcion = row[descCampo].ToString();
                else
                    descripcion = campoFav;
                if (filtro == "PA")
                    descripcion = $"[{row["Nandina"]}] {descripcion}";
                if(isCombo && descripcion.Length >= 60)
                    descripcion = descripcion.Substring(0, 60);
                obj.Add(new SelectListItem { Text = descripcion, Value = codPais + "-" + row[idCampo] + "-" + tipoOpe });
            }

            //return descripcion.Trim();
        }

        static string DescripcionPorId(string filtro, string codPais,string id)
        {
            string tabla = "Partida_" + codPais;
            string descCampo = "Partida";
            string idCampo = "idPartida";
            if (filtro == "PA")
            {
                tabla = "Partida_" + codPais;
                descCampo = "Partida";
                idCampo = "idPartida";
            }
            else if(filtro == "IM\',\'EX")
            {
                tabla = "Empresa_" + codPais;
                descCampo = "Empresa";
                idCampo = "idEmpresa";
            }

            var descripcion = "";

            var sql = $@"Select * from {tabla} 
                                        WHERE {idCampo}in ({id})";

            var dt = Conexion.SqlDataTable(sql);
            descripcion = dt.Rows.Count.ToString();
            foreach (DataRow row in dt.Rows)
            {
                descripcion = row[descCampo].ToString();
            }

            return descripcion.Trim();
        }

        
        public static DataRow ObtienePlanesPrecios(string codPais)
        {
            var planPrecio = BuscaPlanPrecio(codPais);
            var sql = "select top 1 IdiomaDefecto from PlanPrecio where PlanPrecio = '" + planPrecio + "'";

            var dt = Conexion.SqlDataTable(sql);
            return dt.Rows[0];
        }

        public static bool CantidadPlantillasUsuario(string idUsuario, string tipoOpe,string codPais)
        {
            int numPlantillas = 0;
            int plantillasDisponibles = 0;
            var sql = string.Format(@"select count(IdDescargaCab) as cantidad
                                        from DescargaCab 
                                        where IdUsuario ='{0}' and TipoOpe = '{1}' and CodPais = '{2}'",
                                        idUsuario,
                                        tipoOpe,
                                        codPais
                                    );

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                numPlantillas = Convert.ToInt32(row["cantidad"]);
            }

            sql = string.Format(@"select p.LimitePlantillas from [Plan] p
                                    inner join Usuario u on p.IdPlan = u.IdPlan
                                    where u.IdUsuario = '{0}'",idUsuario);
            dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                plantillasDisponibles = Convert.ToInt32(row["LimitePlantillas"]);
            }

            return numPlantillas < plantillasDisponibles;
        }
        
        static string BuscaPlanPrecio(string codPais)
        {
            var planPrecio = "USA";
            var sql = "select PlanPrecio from PlanPrecioPais where CodPais = '" + codPais + "'";

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                planPrecio = row["PlanPrecio"].ToString();
            }

            return planPrecio;
        }

        public static string BuscaPartida(string idPartida, string codPais, string idioma = "es")
        {
            string partida = "";

            var sql = "select Nandina + ' ' + " + (idioma == "es" ? "Partida" : "Partida_en")
                                                + " as Partida from Partida_" + codPais + " where IdPartida = " +
                                                idPartida;

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                partida = row["Partida"].ToString();
            }

            return partida;
        }

        public static void RangoFreemium(string CodPais, string TipoOpe, ref string TrimIni, ref string TrimFin, ref string AñoIni, ref string AñoFin)
        {

            var sql = "select * from BaseDatos where CodPais = '" + CodPais + "' and TipoOpe = '" + TipoOpe + "' ";

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                TrimIni = row["TrimIni"].ToString();
                TrimFin = row["TrimFin"].ToString();
                AñoIni = row["AñoIni"].ToString();
                AñoFin = row["AñoFin"].ToString();
            }

        }
        public static void RangoUniversidades(string CodPais, string TipoOpe, ref string TrimIni, ref string TrimFin, ref string AñoIni, ref string AñoFin)
        {

            var sql = "select * from BaseDatos where CodPais = '" + CodPais + "' and TipoOpe = '" + TipoOpe + "' ";

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                TrimIni = row["TrimIni"].ToString();
                TrimFin = row["TrimFin"].ToString();
                AñoIni = row["AñoIni"].ToString();
                AñoFin = row["AñoFin"].ToString();
            }

        }
        public static bool ValidaRangoIP(string DireccionIP, string Origen)
        {
            bool flag = false;
            Int64 DireccionIP2 = CompletaDireccionIP(DireccionIP);
            string sql = "select IpDesde,IPHasta , Origen from RangoIP where Origen = '" + Origen + "' order by IPDesde";
            try
            {
                var dt = Conexion.SqlDataTable(sql,deadlock:true);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string IPDesde = row["IPDesde"].ToString().Trim();
                        string IPHasta = row["IPHasta"].ToString().Trim();
                        Int64 IPDesde2 = CompletaDireccionIP(IPDesde);
                        Int64 IPHasta2 = CompletaDireccionIP(IPHasta);

                        if (IPDesde2 <= DireccionIP2 && IPHasta2 >= DireccionIP2){ flag = true;
                            break;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }
            return flag;

        }

        static Int64 CompletaDireccionIP(string DireccionIP)
        {
            Int64 DireccionIP2 = 0;

            string[] Partes = DireccionIP.Split('.');

            string Parte1 = Partes[0];
            if (Parte1.Length == 1) Parte1 = "100" + Parte1;
            else if (Parte1.Length == 2) Parte1 = "10" + Parte1;
            else if (Parte1.Length == 3) Parte1 = "1" + Parte1;

            string Parte2 = Partes[1];
            if (Parte2.Length == 1) Parte2 = "100" + Parte2;
            else if (Parte2.Length == 2) Parte2 = "10" + Parte2;
            else if (Parte2.Length == 3) Parte2 = "1" + Parte2;

            string Parte3 = Partes[2];
            if (Parte3.Length == 1) Parte3 = "100" + Parte3;
            else if (Parte3.Length == 2) Parte3 = "10" + Parte3;
            else if (Parte3.Length == 3) Parte3 = "1" + Parte3;

            string Parte4 = Partes[3];
            if (Parte4.Length == 1) Parte4 = "100" + Parte4;
            else if (Parte4.Length == 2) Parte4 = "10" + Parte4;
            else if (Parte4.Length == 3) Parte4 = "1" + Parte4;

            DireccionIP2 = Convert.ToInt64(Parte1 + Parte2 + Parte3 + Parte4);

            return DireccionIP2;
        }
        static Int64 Dot2LongIP(string DottedIP)
        {
            int i;
            string[] arrDec;
            double num = 0;

            if (DottedIP == "")
                return 0;
            else
            {
                arrDec = DottedIP.Split('.');
                for (i = arrDec.Length - 1; i >= 0; i--)
                    num += ((int.Parse(arrDec[i]) % 256) * Math.Pow(256, (3 - i)));
                return Convert.ToInt64(num);
            }
        }

        static void BuscaDatosIP(string NumeroIP, ref string CodPais, ref string Pais, ref string Region,
            ref string Ciudad)
        {
            var sql = "select CodPais, Pais, Region, Ciudad ";
            sql += "from BaseDatosIP where IPDesde <= " + NumeroIP + " and IPHasta >= " + NumeroIP;

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                CodPais = row["CodPais"].ToString();
                Pais = row["Pais"].ToString().Replace("'", "");
                Region = row["Region"].ToString().Replace("'", "");
                Ciudad = row["Ciudad"].ToString().Replace("'", "");
            }
        }

        public static void GrabaHistorial(string IdUsuario, string DireccionIP, string Navegador, string CodEstado)
        {
            string NumeroIP = Dot2LongIP(DireccionIP).ToString();
            string CodPais = "", Pais = "", Region = "", Ciudad = "";
            BuscaDatosIP(NumeroIP, ref CodPais, ref Pais, ref Region, ref Ciudad);
            if (CodEstado == "") CodEstado = "null";
            else CodEstado = "'" + CodEstado + "'";

            var sql =
                "insert into Historial(IdUsuario, DireccionIP, NumeroIP, CodPais, Pais, Region, Ciudad, Navegador, FecVisita, IdAñoMes, VersionLogin, CodEstado) ";
            sql += "values (" + IdUsuario + ", '" + DireccionIP + "', " + NumeroIP + ", '" + CodPais + "', '" + Pais +
                   "', '" + Region + "', ";
            sql += "'" + Ciudad + "', '" + Navegador +
                   "', getdate(), year(getdate()) * 100 + month(getdate()), 'Web', " + CodEstado + ")";

            Conexion.SqlExecute(sql);
        }

        public static bool OcultarVideo(string IdUsuario)
        {
            string ocultarVideo = "";
            string sql = "select isnull(OcultarVideo, '') as OcultarVideo from Usuario where IdUsuario = " + IdUsuario;

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                ocultarVideo = row["OcultarVideo"].ToString();
            }

            return (ocultarVideo == "S");
        }

        public static void ActualizaOcultarVideo(string IdUsuario)
        {
            string sql;
           
            sql = "update Usuario set OcultarVideo = 'S' where IdUsuario = " + IdUsuario;
            Conexion.SqlExecute(sql);
        }

        public static void ActualizaContTemp(string IdUsuario, int Max)
        {
            string sql = "update Usuario set ContTemp = U2.ContTemp ";
            sql += "from Usuario U, (select IdUsuario, isnull(ContTemp, 0) + 1 as ContTemp from Usuario) U2 ";
            sql += "where U.IdUsuario = U2.IdUsuario and U.IdUsuario = " + IdUsuario + " and isnull(U.ContTemp, 0) < " +
                   Max.ToString();

            Conexion.SqlExecute(sql);
        }

        public static int ContTemp(string IdUsuario)
        {
            int ContTemp1 = 0;

            string sql = "select isnull(ContTemp, 0) as ContTemp from Usuario where IdUsuario = " + IdUsuario;

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                ContTemp1 = Convert.ToInt32(row["ContTemp"]);
            }

            return ContTemp1;
        }

        public static bool ForzarLinkExcel(string IdUsuario)
        {
            string ForzarLinkExcel = "";
            string sql = "select isnull(ForzarLinkExcel, '') as ForzarLinkExcel from Usuario where IdUsuario = " +
                         IdUsuario;

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                ForzarLinkExcel = row["ForzarLinkExcel"].ToString();
            }

            return (ForzarLinkExcel == "S");
        }

        public static DataTable CargaPaisesUE(string Idioma)
        {
            string lPais = "Pais";

            if (Idioma == "en")
                lPais = "Pais_en";

            string sql = "select IdPais, " + lPais + " as Pais from PaisUEImpExp where Pais <> '092' order by Pais";
            DataTable dt;

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

        public static string BuscarPaisUE(string codPais, string Idioma)
        {
            string lPais = "Pais";
            string pais = codPais;
            if (Idioma == "en")
                lPais = "Pais_en";

            string sql = $"select IdPais, {lPais} as Pais from PaisUEImpExp where idPais = {codPais} order by Pais";
            DataTable dt;

            try
            {
                dt = Conexion.SqlDataTable(sql);
                foreach (DataRow row in dt.Rows)
                {
                    pais = row["Pais"].ToString();
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                pais = codPais;
            }

            return pais;
        }

        public static string ObtieneCodPaisAcceso(string IdUsuario, ref string CodPais2,
            bool FlagConsideraCarga = false)
        {
            string CodPais = "";

            string sql = "select top 1 CodPais2, B.CodPais from BaseDatos B, Suscripcion S ";
            sql += "where B.CodPais = S.CodPais and IdUsuario = " + IdUsuario + " ";
            if (CodPais2 != "")
                sql += "and CodPais2 = '" + CodPais2 + "' ";
            if (FlagConsideraCarga)
                sql += "and FlagCarga = 'N' ";
            sql += "order by 1, 2";

            DataTable dt;

            try
            {
                dt = Conexion.SqlDataTable(sql);
                foreach (DataRow row in dt.Rows)
                {
                    CodPais2 = row["CodPais2"].ToString();
                    CodPais = row["CodPais"].ToString();
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                throw;
            }

            return CodPais;
        }

        public static string ObtieneCodPaisAccesoFlag(string IdUsuario, ref string CodPais2)
        {
            string CodPais = "";

            string sql = "select top 1 CodPais2, B.CodPais from BaseDatos B, Suscripcion S ";
            sql += "where B.CodPais = S.CodPais and IdUsuario = " + IdUsuario + " ";
            sql += "and FlagCarga = 'N' ";
            sql += "order by 1, 2";

            DataTable dt;

            try
            {
                dt = Conexion.SqlDataTable(sql);
                foreach (DataRow row in dt.Rows)
                {
                    CodPais2 = row["CodPais2"].ToString();
                    CodPais = row["CodPais"].ToString();
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                throw;
            }

            return CodPais;
        }

        public static string InfoEnLinea(string CodPais, string TipoOpe, string Idioma, bool EsTrial)
        {
            // Ruben 202203
            //if (EsTrial) return Idioma == "es" ? string.Format("En la prueba gratuita solo se muestran {0:D2} meses de información", Math.Abs(Properties.Settings.Default.FreeTrial_Periodo_Count-1) ) :
            //                                     string.Format("In the free trial only {0:D2} months of information is shown", Math.Abs(Properties.Settings.Default.FreeTrial_Periodo_Count-1));

            string FechaIni = "", FechaFin = "", Info = "";
            string sql = "select FechaIni, FechaFin from BaseDatos where CodPais ='" + CodPais + "' and TipoOpe = '" +
                         TipoOpe + "' ";
            DataTable dt;
            try
            {
                dt = Conexion.SqlDataTable(sql);
                foreach (DataRow row in dt.Rows)
                {
                    FechaIni = row["FechaIni"].ToString();
                    FechaIni = FechaIni.Substring(0, 4) + "-" + FechaIni.Substring(4, 2) + "-" +
                               FechaIni.Substring(6, 2);
                    FechaFin = row["FechaFin"].ToString();
                    FechaFin = FechaFin.Substring(0, 4) + "-" + FechaFin.Substring(4, 2) + "-" +
                               FechaFin.Substring(6, 2);

                    if (Idioma == "es")
                    {
                        System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("es-PE");
                        FechaIni = Convert.ToDateTime(FechaIni).ToString("dd-MMM-yyyy", culture).ToUpper();
                        FechaFin = Convert.ToDateTime(FechaFin).ToString("dd-MMM-yyyy", culture).ToUpper();
                        Info = "Información en línea desde " + FechaIni + " hasta " + FechaFin;
                        
                    }
                    else
                    {
                        System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US");
                        FechaIni = Convert.ToDateTime(FechaIni).ToString("MMM dd, yyyy", culture).ToUpper();
                        FechaFin = Convert.ToDateTime(FechaFin).ToString("MMM dd, yyyy", culture).ToUpper();
                        Info = "Online information from " + FechaIni + " to " + FechaFin;
                        
                    }
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                throw;
            }
            return Info;
        }

        /// <summary>
        /// Obtiene la lista de paises para el filtro País Origen dentro de un DataTable
        /// </summary>
        /// <param name="codPais"></param>
        /// <param name="idioma"></param>
        /// <returns></returns>
        public static DataTable CargaPaises(string codPais, string codPais2, string idioma)
        {
            bool isUE = false;
            string codPaisAux = codPais;
            if (codPais == "PEB" || codPais == "PEP")
                codPaisAux = "PE";
            else if (codPais2 == "4UE")
            {
                codPaisAux = "UE";
                isUE = true;
            }

            string lTodos = "TODOS LOS PAISES";
            string campo = "pais";
            //if (!isUE)
            //{
                
            //}

            campo = "pais_es";
            if (idioma == "en")
            {
                lTodos = "ALL COUNTRIES";
                campo = "pais_en";
            }

            string sql = "select 1 as Orden, 0 as IdPais, '[" + lTodos + "]' as Pais union ";
            sql += "select 2 as Orden, IdPais, isnull(" + campo + ",Pais) as Pais from Pais_" + codPaisAux + " ";
            sql += "order by 1, 3";

            DataTable dt;
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


        public static IEnumerable<SelectListItem> CargarPaises(string codPais, string idioma)
        {
            
            string lTodos = "TODOS";
            if (idioma == "en") lTodos = "ALL";

            string sql = "select 1 as Orden, 0 as IdPais, '[" + lTodos + "]' as Pais union ";
            sql += "select 2 as Orden, IdPais, Pais from Pais_" + codPais + " ";
            sql += "order by 1, 3";
            var lista = new List<SelectListItem>();
            DataTable dt;
            try
            {
                dt = Conexion.SqlDataTable(sql);
                foreach (DataRow row in dt.Rows)
                {
                    lista.Add(new SelectListItem { Value = row["IdPais"].ToString(), Text = @row["Pais"].ToString() });
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                dt = null;
            }

            return lista;
        }

        /// <summary>
        /// Obtiene la lista de paises para el filtro Pais Origen en formato IEnumerable<SelectListItem>
        /// </summary>
        /// <param name="CodPais">Código de pais de acceso</param>
        /// <param name="Idioma">Idioma de acceso</param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> getOriginCountries(string codPais, string codPais2, string idioma)
        {
            var lista = new List<SelectListItem>();
            DataTable dt = CargaPaises(codPais, codPais2, idioma);
            if(dt != null)
                foreach (DataRow dr in dt.Rows)
                {
                    lista.Add(new SelectListItem { Text = dr["Pais"].ToString(), Value = dr["IdPais"].ToString() });
                }

            return lista;
        }

        public static DataRow ObtieneSolicitud(string IdSolicitud)
        {
            string sql = "select * from Solicitud where IdSolicitud = " + IdSolicitud;
            DataRow dr;
            try
            {
                var dt = Conexion.SqlDataTable(sql);
                dr = dt.Rows[0];
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                dr = null;
            }

            return dr;
        }

        public static DataRow ObtieneUsuario(string IdUsuario)
        {
            string sql =
                "select CodUsuario, EmpPer, Nombres + ' ' + Apellidos as Nombres, Empresa, Telefono, Email1, Pais, Password ";
            sql += "from Usuario U, AdminPais P ";
            sql += "where U.CodPais = P.CodPais and IdUsuario = " + IdUsuario;
            var dt = Conexion.SqlDataTable(sql);
            return dt.Rows[0];
        }

        public static string CreaSolicitud(string CodSolicitud, string IdUsuario, string Mensaje, string DireccionIP,
            string Ubicacion)
        {
            string sql;
            string IdSolicitud = "";

            try
            {
                DataRow dr = ObtieneUsuario(IdUsuario);

                sql =
                    "insert into Solicitud(CodSolicitud, CodUsuario, Nombres, Empresa, Telefono, Email1, Mensaje, DireccionIP, Ubicacion, Fecha) values ";
                sql += "('" + CodSolicitud + "', '" + dr["CodUsuario"].ToString() + "', '" + dr["Nombres"].ToString() +
                       "', '" + dr["Empresa"].ToString() + "', '" + dr["Telefono"].ToString() + "', '" +
                       dr["Email1"].ToString() + "', '" + Mensaje + "', ";
                sql += "'" + DireccionIP + "', '" + Ubicacion + "', getdate())";

                Conexion.SqlExecute(sql);

                sql = "select max(IdSolicitud) as IdSolicitud from Solicitud";

                var dt = Conexion.SqlDataTable(sql);
                foreach (DataRow row in dt.Rows)
                {
                    IdSolicitud = row["IdSolicitud"].ToString();
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }

            return IdSolicitud;
        }

        public static DataTable BuscaFavoritos(string IdUsuario, string CodPais, string TipoOpe, string TipoFavorito,
            string FavoritoB, bool flagSeleccione, string Idioma)
        {
            string sql = "";
            string CodPais2 = CodPais;
            if (CodPais.Substring(0, 2) == "UE")
                CodPais2 = "UE";

            string lSeleccione = "Seleccione";
            if (Idioma == "en") lSeleccione = "Select";

            if (flagSeleccione)
                sql += "select 0 as IdFavorito, '[ " + lSeleccione + "]' as Favorito union ";

            switch (TipoFavorito)
            {
                case "Partida":
                    string Partida = "Partida";
                    if (Idioma == "en")
                        Partida = "Partida_en";

                    sql += "select IdGrupo as IdFavorito, '[G] ' + Grupo as Favorito ";
                    sql += "from Grupo ";
                    sql += "where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' and TipoOpe = '" +
                           TipoOpe + "' and TipoFav = 'PA' ";
                    if (FavoritoB != "")
                        sql += "and Grupo like '%" + FavoritoB + "%' ";
                    sql += "union ";
                    sql +=
                        "select IdFavorito, Nandina + ' ' + case when PartidaFav is not null then PartidaFav when len(" +
                        Partida + ") > 80 then substring(" + Partida + ", 1, 80) else " + Partida + " end as Favorito ";
                    sql += "from V_FavUnicos F, Partida_" + CodPais2 + " P, PartidaFav PF ";
                    sql +=
                        "where F.IdFavorito = P.IdPartida and F.IdUsuario = PF.IdUsuario and F.CodPais = PF.CodPais and F.TipoOpe = PF.TipoOpe and F.IdFavorito = PF.IdPartida ";
                    sql += "and F.IdUsuario = " + IdUsuario + " and F.CodPais = '" + CodPais + "' and F.TipoOpe = '" +
                           TipoOpe + "' and TipoFav = 'PA' ";
                    if (FavoritoB != "")
                        sql += "and (Nandina like '" + FavoritoB + "%' or Partida like '%" + FavoritoB +
                               "%' or PartidaFav like '%" + FavoritoB + "%') ";
                    sql += "order by 2";
                    break;
                case "Importador":
                case "Exportador":
                    sql += "select IdGrupo as IdFavorito, '[G] ' + Grupo as Favorito  ";
                    sql += "from Grupo ";
                    sql += "where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' and TipoOpe = '" +
                           TipoOpe + "' and TipoFav = '" + (TipoOpe == "I" ? "IM" : "EX") + "' ";
                    if (FavoritoB != "")
                        sql += "and Grupo like '%" + FavoritoB + "%' ";
                    sql += "union ";
                    sql += "select IdFavorito, Empresa as Favorito ";
                    sql += "from V_FavUnicos F, Empresa_" + CodPais + " E ";
                    sql += "where F.IdFavorito = E.IdEmpresa ";
                    sql += "and IdUsuario = " + IdUsuario + " and F.CodPais = '" + CodPais + "' and TipoOpe = '" +
                           TipoOpe + "' and TipoFav = '" + (TipoOpe == "I" ? "IM" : "EX") + "' ";
                    if (FavoritoB != "")
                        sql += "and Empresa like '%" + FavoritoB + "%' ";
                    sql += "order by 2";
                    break;
                case "Proveedor":
                    sql += "select IdGrupo as IdFavorito, '[G] ' + Grupo as Favorito ";
                    sql += "from Grupo ";
                    sql += "where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' and TipoFav = 'PR' ";
                    if (FavoritoB != "")
                        sql += "and Grupo like '%" + FavoritoB + "%' ";
                    sql += "union ";
                    sql += "select IdFavorito, Proveedor as Favorito ";
                    sql += "from V_FavUnicos F, Proveedor_" + CodPais + " P ";
                    sql += "where F.IdFavorito = P.IdProveedor ";
                    sql += "and IdUsuario = " + IdUsuario + " and F.CodPais = '" + CodPais + "' and TipoFav = 'PR' ";
                    if (FavoritoB != "")
                        sql += "and Proveedor like '%" + FavoritoB + "%' ";
                    sql += "order by 2 ";
                    break;
                case "ImportadorExp":
                    sql += "select IdGrupo as IdFavorito, '[G] ' + Grupo as Favorito ";
                    sql += "from Grupo ";
                    sql += "where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' and TipoFav = 'IE' ";
                    if (FavoritoB != "")
                        sql += "and Grupo like '%" + FavoritoB + "%' ";
                    sql += "union ";
                    sql += "select IdFavorito, ImportadorExp as Favorito ";
                    sql += "from V_FavUnicos F, ImportadorExp_" + CodPais + " I ";
                    sql += "where F.IdFavorito = I.IdImportadorExp ";
                    sql += "and IdUsuario = " + IdUsuario + " and F.CodPais = '" + CodPais + "' and TipoFav = 'IE' ";
                    if (FavoritoB != "")
                        sql += "and ImportadorExp like '%" + FavoritoB + "%' ";
                    sql += "order by 2";
                    break;
            }

            DataTable dt;
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

        public static string ObtienePartidaFav(string idUsuario, string codPais, string tipoOpe, string idPartida)
        {
            string partidaFav = "";

            string sql = "select Nandina, PartidaFav, Partida from PartidaFav PF, Partida_" + codPais + " P ";
            sql += "where PF.IdPartida = P.IdPartida and IdUsuario = " + idUsuario + " ";
            sql += "and CodPais = '" + codPais + "' and TipoOpe = '" + tipoOpe + "' and PF.IdPartida = " + idPartida;

            try
            {
                DataTable dt = Conexion.SqlDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    if (dr["PartidaFav"].ToString() != "")
                        partidaFav = dr["Nandina"].ToString() + " " + dr["PartidaFav"].ToString();
                    else
                        partidaFav = dr["Nandina"].ToString() + " " + dr["Partida"].ToString();
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }

            return partidaFav;
        }

        private static string ObtenerFavoritoPorIdFavorito(string idioma, string codPais, string idFavorito,
            string idUsuario, string auxcodPais ="")
        {
            string favorito = "";
            string codPais2 = codPais;
            if (codPais.Substring(0, 2) == "UE")
            {
                codPais2 = "UE";
                codPais = codPais + auxcodPais;
            }
                
            string partida = "Partida";
            if (idioma == "en")
                partida = "Partida_en";

            string sql =
                "SELECT TOP 1 P.Nandina + ' ' + CASE WHEN PF.PartidaFav is not null THEN PF.PartidaFav WHEN len(P." +
                partida + ") > 80 THEN substring(P." + partida + ", 1, 80) ELSE P." + partida + " END AS Favorito ";
            sql += "FROM V_FavUnicos F INNER JOIN  Partida_" + codPais2 +
                   " P ON F.IdFavorito = P.IdPartida INNER JOIN  PartidaFav PF ON F.IdFavorito = PF.IdPartida WHERE F.IdFavorito = " +
                   idFavorito;
            sql += " AND F.IdUsuario = PF.IdUsuario AND  F.IdUsuario = " + idUsuario + " AND F.CodPais = '" + codPais +
                   "'";
            try
            {
                DataTable dt = Conexion.SqlDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    favorito = (dt.Rows[0])["Favorito"].ToString();
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }

            return favorito;
        }

        public static string BuscaEmpresa(string idEmpresa, string codPais)
        {
            string empresa = "";
            var sql = "select rtrim(Empresa) as Empresa from Empresa_" + codPais + " where IdEmpresa = " + idEmpresa;
            try
            {
                DataTable dt = Conexion.SqlDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    empresa = (dt.Rows[0])["Empresa"].ToString();
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }

            return empresa;
        }

        public static string BuscaProveedor(string idProveedor, string codPais)
        {
            string proveedor = "";
            var sql = "select Proveedor from Proveedor_" + codPais + " where IdProveedor = " + idProveedor;
            try
            {
                DataTable dt = Conexion.SqlDataTable(sql);
                if (dt.Rows.Count > 0)
                    proveedor = (dt.Rows[0])["Proveedor"].ToString();
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }

            return proveedor;
        }

        public static string BuscaImportadorExp(string idImportadorExp, string codPais)
        {
            string importadorExp = "";
            string sql = "select ImportadorExp from ImportadorExp_" + codPais + " where IdImportadorExp = " +
                         idImportadorExp;
            try
            {
                DataTable dt = Conexion.SqlDataTable(sql);
                if (dt.Rows.Count > 0)
                    importadorExp = (dt.Rows[0])["ImportadorExp"].ToString();
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }

            return importadorExp;
        }

        public static ArrayList GuardaSeleccionados(string codPais, string tipoOpe,
            string tipoFiltro, string[] favoritosSeleccionados, ArrayList idsSeleccionados,
            ref List<object> listaNuevosFiltros, int numFiltrosExistentes, string idUsuario,
            int limiteFiltros = -1, string idioma = "es", string auxCodPais ="")
        {
            string tipo = "", nombre = "";

            if (idsSeleccionados == null)
                idsSeleccionados = new ArrayList();

            foreach (var t in favoritosSeleccionados)
            {
                if (limiteFiltros > 0 && (listaNuevosFiltros.Count + numFiltrosExistentes == limiteFiltros))
                    break;

                var id = t;

                if (id.Substring(0, 1) == "F")
                    id = id.Substring(1, id.Length - 1);

                if (!idsSeleccionados.Contains(id))
                {
                    idsSeleccionados.Add(id);
                    switch (tipoFiltro)
                    {
                        case "Partida":
                            tipo = "2PA";
                            nombre = "[" + Resources.Resources.Nandina_FormField_Label + "] " +
                                     ObtenerFavoritoPorIdFavorito(idioma, codPais, id, idUsuario, auxCodPais);
                            break; //BuscaPartida(ID, CodPais); break; 
                        case "Importador":
                        case "Exportador":
                            if (tipoOpe == "I")
                            {
                                tipo = "3IM";
                                nombre = "[" + Resources.Resources.Search_Form_Item05 + "] " +
                                         BuscaEmpresa(id, codPais);
                            }
                            else
                            {
                                tipo = "3EX";
                                nombre = "[" + Resources.Resources.Search_Form_Item06 + "] " +
                                         BuscaEmpresa(id, codPais);
                            }

                            break;
                        case "Proveedor":
                        case "ImportadorExp":
                            if (tipoOpe == "I")
                            {
                                tipo = "4PR";
                                nombre = ((codPais != "CL" )
                                             ? "[" + Resources.Resources.Search_Form_Item06 + "] "
                                             : "[" + Resources.Resources.Search_Form_BrandField + "] ") +
                                         BuscaProveedor(id, codPais);
                            }
                            else
                            {
                                tipo = "4IE";
                                nombre = "[" + Resources.Resources.Search_Form_Item05 + "] " +
                                         BuscaImportadorExp(id, codPais);
                            }

                            break;
                    }

                    listaNuevosFiltros.Add(new
                    {
                        value = tipo + id,
                        text = nombre
                    });
                }
            }

            return idsSeleccionados;
        }


        public static ArrayList GuardaSeleccionados(string filtro, ArrayList idsSeleccionados, string[] arrayIdsSeleccionados,
            string[] arrayIdsPagina)
        {
            //string id;

            if (idsSeleccionados == null)
                idsSeleccionados = new ArrayList();

            foreach (var id in arrayIdsPagina)
            {
                //if (filtro != Enums.Filtro.AduanaDUA.ToString())
                //    id = item;
                //else
                //{
                //    //Falta  la logica
                //    id = item;
                //}

                if (arrayIdsSeleccionados.Contains(id))
                {
                    if (!idsSeleccionados.Contains(id))
                        idsSeleccionados.Add(id);
                }
                else
                {
                    if (idsSeleccionados.Contains(id))
                        idsSeleccionados.Remove(id);
                }
            }
            return idsSeleccionados;
        }

        public static ArrayList GuardaSeleccionadosAbuscar(string filtro, ArrayList idsSeleccionados, string[] arrayIdsSeleccionados,
            string[] arrayIdsPagina, ArrayList filtrosUsados, ref bool existenFiltros)
        {
            //string id;

            if (idsSeleccionados == null)
                idsSeleccionados = new ArrayList();

            if (filtrosUsados == null)
            {
                filtrosUsados = new ArrayList();
            }

            foreach (var id in arrayIdsPagina)
            {
                //if (filtro != "AduanaDUAs")
                //    id = item;
                //else
                //{
                //    //Falta  la logica
                //    id = item;
                //}

                if (arrayIdsSeleccionados.Contains(id))
                {
                    if (!idsSeleccionados.Contains(id))
                    {
                        if (!filtrosUsados.Contains(id))
                        {
                            idsSeleccionados.Add(id);
                        }
                        else
                        {
                            existenFiltros = true;
                        }
                    }
                }
                else
                {
                    if (idsSeleccionados.Contains(id))
                        idsSeleccionados.Remove(id);
                }
            }

            return idsSeleccionados;
        }

        public static DataRow ObtenerImportador(string codPais, int idImportacion)
        {
            string sql = $@"SELECT IdImportador, RUC, Importador
                            FROM Importacion_{codPais} 
                            WHERE IdImportacion={idImportacion}";
            DataRow dr;
            try
            {
                var dt = Conexion.SqlDataTable(sql);
                dr = dt.Rows[0];
            }
            catch (Exception ex)
            {
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                dr = null;

            }

            return dr;
        }

        public static DataRow CalculaTotales(string sqlFiltro, string cifTot, string codPais,
            string pesoNeto, string tabla, bool flagFormatoB = false, bool isManif = false, string filtro = "")
        {
            string cifTot1 = cifTot;

            if (codPais == "BR" || codPais == "IN")
                cifTot1 = "convert(decimal(19,2), " + cifTot + ")";

            var pesoNeto1 = pesoNeto;
            var pesoNeto2 = pesoNeto;

            if (pesoNeto1 == "")
            {
                pesoNeto1 = "0";
                pesoNeto2 = "PesoNeto";
            }

            var tabla1 = tabla;

            if (flagFormatoB || ((sqlFiltro.Contains("IdMarca") || sqlFiltro.Contains("IdModelo")) && codPais != "EC"))
            {
                cifTot1 = "FOBTot";
                pesoNeto1 = "0";
                pesoNeto2 = "PesoNeto";
                tabla1 = "Importacion_PEB";
            }

            var sql = string.Empty;
            if (!isManif)
            {
                sql = "select count(*) as CantReg, sum(" + cifTot1 + ") as " + cifTot + ", sum (isnull(" + pesoNeto1 +
                          ",0)) as " +
                          pesoNeto2 + ", sum(Cantidad) as Cantidad ";
                sql += "from " + tabla1 + " T where 1 = 1 ";
                sql += sqlFiltro;
            }
            else
            {
                sql = "select count(*) as CantReg, sum(" + cifTot + ") / 1000 as " + cifTot + " ";
                sql += "from " + tabla + " T where 1 = 1 ";
                sql += sqlFiltro;
            }

            

            DataRow dr;
            try
            {
                var dt = Conexion.SqlDataTable(sql);
                dr = dt.Rows[0];
            }
            catch (Exception ex)
            {
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                dr = null;
            }

            return dr;
        }

        public static DataRow CalculaTotalesInfo(string sqlFiltro)
        {
            string sql = $@"SELECT COUNT(*) CantReg,
                            SUM(cantidad) TotalData,
		                    SUM(ciftot) as ciftot,
		                    sum(pesoNeto) as pesoNeto
                               FROM ({sqlFiltro}) T";
            DataRow dr;
            try
            {
                var dt = Conexion.SqlDataTable(sql);
                dr = dt.Rows[0];
            }
            catch (Exception ex)
            {
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                dr = null;
            }

            return dr;
        }

        public static DataRow CalculaTotalesFreemium(string sqlFiltro, string cifTot, string codPais,
            string pesoNeto, string tabla, bool flagFormatoB = false, bool isManif = false,bool isModal = false)
        {
            string cifTot1 = cifTot;

            if (codPais == "BR" || codPais == "IN")
                cifTot1 = "convert(decimal(19,2), " + cifTot + ")";

            var pesoNeto1 = pesoNeto;
            var pesoNeto2 = pesoNeto;

            if (pesoNeto1 == "")
            {
                pesoNeto1 = "0";
                pesoNeto2 = "PesoNeto";
            }

            var tabla1 = tabla;

            if (flagFormatoB || sqlFiltro.Contains("IdMarca") || sqlFiltro.Contains("IdModelo"))
            {
                cifTot1 = "FOBTot";
                pesoNeto1 = "0";
                pesoNeto2 = "PesoNeto";
                tabla1 = "Importacion_PEB";
            }

            var sql = string.Empty;
            if (!isManif)
            {
                if (isModal)
                {
                    sql = "select count(*) as CantReg, sum(" + cifTot1 + ") as " + cifTot + ", sum (" + pesoNeto1 +
                          ") as " +
                          pesoNeto2 + ", sum(Cantidad) as Cantidad ";
                    sql += "from " + tabla1 + " T where 1 = 1 ";
                    sql += sqlFiltro;
                }
                else
                {
                    sql = "select sum(Registros) as CantReg, sum(" + cifTot1 + ") as " + cifTot + ", sum (" + pesoNeto1 +
                          ") as " +
                          pesoNeto2 + ", sum(Cantidad) as Cantidad ";
                    sql += "from " + tabla1 + " T where 1 = 1 ";
                    sql += sqlFiltro;
                }
            }
            else
            {
                sql = "select sum(Registros) as CantReg, sum(" + cifTot + ") / 1000 as " + cifTot + " ";
                sql += "from " + tabla + " T where 1 = 1 ";
                sql += sqlFiltro;
            }



            DataRow dr;
            try
            {
                var dt = Conexion.SqlDataTable(sql);
                dr = dt.Rows[0];
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                dr = null;
            }

            return dr;
        }

        public static DataRow CalculaTotalesUniversidades(string sqlFiltro, string cifTot, string codPais,
            string pesoNeto, string tabla, bool flagFormatoB = false, bool isManif = false, bool isModal = false)
        {
            string cifTot1 = cifTot;

            if (codPais == "BR" || codPais == "IN")
                cifTot1 = "convert(decimal(19,2), " + cifTot + ")";

            var pesoNeto1 = pesoNeto;
            var pesoNeto2 = pesoNeto;

            if (pesoNeto1 == "")
            {
                pesoNeto1 = "0";
                pesoNeto2 = "PesoNeto";
            }

            var tabla1 = tabla;

            if (flagFormatoB || sqlFiltro.Contains("IdMarca") || sqlFiltro.Contains("IdModelo"))
            {
                cifTot1 = "FOBTot";
                pesoNeto1 = "0";
                pesoNeto2 = "PesoNeto";
                tabla1 = "Importacion_PEB";
            }

            var sql = string.Empty;
            if (!isManif)
            {
                if (isModal)
                {
                    sql = "select count(*) as CantReg, sum(" + cifTot1 + ") as " + cifTot + ", sum (" + pesoNeto1 +
                          ") as " +
                          pesoNeto2 + ", sum(Cantidad) as Cantidad ";
                    sql += "from " + tabla1 + " T where 1 = 1 ";
                    sql += sqlFiltro;
                }
                else
                {
                    sql = "select sum(Registros) as CantReg, sum(" + cifTot1 + ") as " + cifTot + ", sum (" + pesoNeto1 +
                          ") as " +
                          pesoNeto2 + ", sum(Cantidad) as Cantidad ";
                    sql += "from " + tabla1 + " T where 1 = 1 ";
                    sql += sqlFiltro;
                }
            }
            else
            {
                sql = "select sum(Registros) as CantReg, sum(" + cifTot + ") / 1000 as " + cifTot + " ";
                sql += "from " + tabla + " T where 1 = 1 ";
                sql += sqlFiltro;
            }



            DataRow dr;
            try
            {
                var dt = Conexion.SqlDataTable(sql);
                dr = dt.Rows[0];
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                dr = null;
            }

            return dr;
        }


        public static string Unidades(string sqlFiltro, string tabla, ref int  numRegistros)
        {
            string unidad = "";
            string sql = "select distinct Unidad ";
            sql += "from " + tabla + " T where 1 = 1 ";
            sql += sqlFiltro;

            try
            {
                DataTable dt = Conexion.SqlDataTable(sql);
                if (dt.Rows.Count > 0 && dt.Rows.Count ==1)
                {
                    unidad = dt.Rows[0]["Unidad"].ToString();
                }
                numRegistros = dt.Rows.Count;
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                unidad = "";
            }

            return unidad;
        }

        public static void RegistraConsumo(string idUsuario, string codPais, string tipoOpe, string accion, string pSQL)
        {
            if (pSQL != "")
                pSQL = "'" + pSQL + "'";
            else
                pSQL = "null";

            string sql = "insert into Consumo(IdUsuario, CodPais, TipoOpe, Accion, SQL, Fecha, IdAñoMes) ";
            sql += "values(" + idUsuario + ", '" + codPais + "', '" + tipoOpe + "', '" + accion +
                   "', null, getdate(), year(getdate()) * 100 + month(getdate()))";

            try
            {
                Conexion.SqlExecute(sql);
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }
        }

        public static DataTable Lista2(string sqlO)
        {
            DataTable dt;
            try
            {
                dt = Conexion.SqlDataTable(sqlO);
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                dt = null;
            }

            return dt;
        }

        public static DataTable ObtenerNotaYDetallesPais(string codigoPais, string lenguaje)
        {
            DataTable dt;
            try
            {
                string sql = "SELECT IdMensajePais, Nota, Detalle FROM Mensaje_Pais " +
                             "WHERE CodPais='"+codigoPais+"' AND Lenguaje='"+lenguaje+"'";
                dt = Conexion.SqlDataTable(sql);
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                dt = null;
            }

            return dt;
        }

        public static string TotalInfoComplementario(string sqlO)
        {

            string sql = "select sum(ciftot) as total from (" + sqlO + ") T ";
            string total = "0";
            DataTable dt;
            try
            {
                dt = Conexion.SqlDataTable(sql);
                total = dt.Rows[0]["total"].ToString();
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                total = "0";
            }

            return total;
        }

        public static DataTable Lista(string sqlO, int page, int maximumRows, bool excel = false, bool useRand = false)
        {
            int startRowIndex = (page - 1) * maximumRows;

            string rand = " ";

            // Ruben 202203
            if (useRand && !sqlO.Contains("_MXD"))
            {
                rand = " or rand()<0 ";
            }
            
            string sql = "";
            /*if (excel)
            {
                string sql2 = sqlO.Replace("from", "into #table1 from");

                int pos = sqlO.IndexOf("from");

                string sql3 = sqlO.Substring(0,pos)+ "into #table1 from"+sqlO.Substring(pos+4,sqlO.Length-pos-4);

                sql = sql3 + ";";
                sql += $@"SELECT *
                    FROM #table1
                    WHERE Nro BETWEEN {(startRowIndex + 1).ToString()} AND {(startRowIndex + maximumRows).ToString()}
                                ORDER BY NRO;
                                drop table #table1;";
            }
            else
            {*/
                sql = "select * from (" + sqlO + ") T ";
                sql += "where Nro between " + (startRowIndex + 1).ToString() + " and " +
                       (startRowIndex + maximumRows).ToString() + rand + "order by Nro";
            //}



            DataTable dt;
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

        public static DataTable ListaInfoTabla(string sqlO, int page, int maximumRows, bool excel = false, string order = "ciftot desc")
        {
            int startRowIndex = (page - 1) * maximumRows;

            if(order == "InfoTabla")
            {
                order = "Pais";
            }
            else
            {
                order = order.Replace("CantReg", "Cantidad");
                order = order.Replace("FOB", "cif");
            }

            string sql = $@"select * , ROW_NUMBER() over (order by {order}) as Nro  from
                            ({sqlO}) X";
        

            return Lista(sql, page,maximumRows,excel);
        }


        public static DataTable SearchImportsData(int IdProducto)
        {
            var sql = " SELECT CONVERT(VARCHAR,CONVERT(MONEY,SUM(VALOR)),1) AS ValorTotal, " +
                      "CONVERT(VARCHAR, CONVERT(MONEY, SUM(CANTIDAD)), 1) AS CantidadTotal, " +
                      "CONVERT(VARCHAR, CONVERT(MONEY, SUM(PRECIOUNIT)),1) AS PrecioUnitTotal " +
                      "FROM TOTALES WHERE REGIMEN = 'Importaciones' AND IDPRODUCTO = " + IdProducto +
                      " GROUP BY IDPRODUCTO ";

            DataTable dt;
            try
            {
                dt = Conexion.SqlDataTableProductProfile(sql);
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                dt = null;
            }

            return dt;
        }
        public static DataTable SearchProductData(int codProducto, string idioma)
        {
            var sql = "";

            if (idioma == "es")
            {
                sql = "SELECT IdProducto, CodProducto, DescripcionES as Descripcion FROM Producto WHERE IdProducto = " + codProducto;
            }
            else
            {
                sql = "SELECT IdProducto, CodProducto, DescripcionEN as Descripcion FROM Producto WHERE IdProducto = " + codProducto;
            }

            DataTable dt;
            try
            {
                dt = Conexion.SqlDataTableProductProfile(sql);
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                dt = null;
            }

            return dt;
        }
        public static DataTable SearchProductFlag(int IdProducto)
        {
            var sql = "SELECT TOP 1 PA.IdPaisAduana, PA.PaisAduana FROM TOTALES T INNER JOIN PAISADUANA PA ON PA.IdPaisAduana = T.IdPaisAduana " +
                      "WHERE T.IDPRODUCTO = " + IdProducto + " ORDER BY T.VALOR DESC ";

            DataTable dt;
            try
            {
                dt = Conexion.SqlDataTableProductProfile(sql);
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                dt = null;
            }

            return dt;
        }
        public static DataTable SearchConsolidateCountry(int IdProducto, int IdPaisAduana)
        {
            var sql = "SELECT PA.IdPaisAduana, " +
                      " PA.PaisAduana," +
                      " Importaciones = ISNULL((SELECT CONVERT(VARCHAR, CONVERT(MONEY, VALOR), 1) FROM TOTALES WHERE IDPRODUCTO = " + IdProducto + " AND REGIMEN = 'Importaciones' AND IDPAISADUANA = PA.IDPAISADUANA),'0'), " +
                      " Exportaciones = ISNULL((SELECT CONVERT(VARCHAR, CONVERT(MONEY, VALOR), 1) FROM TOTALES WHERE IDPRODUCTO = " + IdProducto + " AND REGIMEN = 'Exportaciones' AND IDPAISADUANA = PA.IDPAISADUANA),'0')," +
                      " Importadores = ISNULL((SELECT CantidadEmpresas FROM TOTALES WHERE IDPRODUCTO = " + IdProducto + " AND REGIMEN = 'Importaciones' AND IDPAISADUANA = PA.IDPAISADUANA),0)," +
                      "Exportadores = ISNULL((SELECT CantidadEmpresas FROM TOTALES WHERE IDPRODUCTO = " + IdProducto + " AND REGIMEN = 'Exportaciones' AND IDPAISADUANA = PA.IDPAISADUANA),0)" +
                      " FROM TOTALES T INNER JOIN PAISADUANA PA ON PA.IdPaisAduana = T.IdPaisAduana GROUP BY PA.IDPAISADUANA, T.IDPRODUCTO , PA.PAISADUANA HAVING T.IDPRODUCTO = " + IdProducto +
                      " AND PA.IDPAISADUANA = " + IdPaisAduana;

            DataTable dt;
            try
            {
                dt = Conexion.SqlDataTableProductProfile(sql);
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                dt = null;
            }

            return dt;
        }
        public static DataTable SearchCifImports(int IdProducto, int IdPaisAduana)
        {
            var sql = "SELECT IDPRODUCTO, IDPAISADUANA, REGIMEN, AÑO, SUM(VALOR) as VALOR " +
                      "FROM TOTALESAÑO_MES WHERE IDPRODUCTO = " + IdProducto +
                      " AND IDPAISADUANA = " + IdPaisAduana +
                      " AND REGIMEN = 'Importaciones' GROUP BY IDPRODUCTO, IDPAISADUANA, REGIMEN, AÑO";

            DataTable dt;
            try
            {
                dt = Conexion.SqlDataTableProductProfile(sql);
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                dt = null;
            }

            return dt;
        }
        public static DataTable SearchPrecioProm(int IdProducto, int IdPaisAduana)
        {
            var sql = "SELECT Mes = CASE Mes WHEN 1 THEN 'Enero' WHEN 2 THEN 'Febrero' WHEN 3 THEN 'Marzo' WHEN 4 THEN 'Abril' " +
                      "WHEN 5 THEN 'Mayo' WHEN 6 THEN 'Junio' WHEN 7 THEN 'Julio' WHEN 8 THEN 'Agosto' WHEN 9 THEN 'Septiembre' " +
                      "WHEN 10 THEN 'Octubre' WHEN 11 THEN 'Noviembre' WHEN 12 THEN 'Diciembre' END, ISNULL(PrecioUnit,0) AS PrecioUnit " +
                      "FROM TOTALESAÑO_MES WHERE REGIMEN = 'Importaciones' AND" +
                      " IDPRODUCTO = " + IdProducto +
                      " AND IDPAISADUANA = " + IdPaisAduana +
                      " AND AÑO = (SELECT TOP 1 AÑO FROM TOTALESAÑO_MES WHERE IDPRODUCTO = " + IdProducto + " AND IDPAISADUANA = " + IdPaisAduana +
                      " GROUP BY AÑO ORDER BY AÑO DESC)";

            DataTable dt;
            try
            {
                dt = Conexion.SqlDataTableProductProfile(sql);
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                dt = null;
            }

            return dt;
        }
        public static DataTable SearchConsolidateCountries(int IdProducto)
        {
            var sql = "SELECT PA.IdPaisAduana, " +
                      " PA.PaisAduana," +
                      " Importaciones = ISNULL((SELECT CONVERT(VARCHAR, CONVERT(MONEY, VALOR), 1) FROM TOTALES WHERE IDPRODUCTO = " + IdProducto + " AND REGIMEN = 'Importaciones' AND IDPAISADUANA = PA.IDPAISADUANA),'0'), " +
                      " Exportaciones = ISNULL((SELECT CONVERT(VARCHAR, CONVERT(MONEY, VALOR), 1) FROM TOTALES WHERE IDPRODUCTO = " + IdProducto + " AND REGIMEN = 'Exportaciones' AND IDPAISADUANA = PA.IDPAISADUANA),'0')," +
                      " Importadores = ISNULL((SELECT CantidadEmpresas FROM TOTALES WHERE IDPRODUCTO = " + IdProducto + " AND REGIMEN = 'Importaciones' AND IDPAISADUANA = PA.IDPAISADUANA),0)," +
                      "Exportadores = ISNULL((SELECT CantidadEmpresas FROM TOTALES WHERE IDPRODUCTO = " + IdProducto + " AND REGIMEN = 'Exportaciones' AND IDPAISADUANA = PA.IDPAISADUANA),0)" +
                      " FROM TOTALES T INNER JOIN PAISADUANA PA ON PA.IdPaisAduana = T.IdPaisAduana GROUP BY PA.IDPAISADUANA, T.IDPRODUCTO , PA.PAISADUANA HAVING T.IDPRODUCTO = " + IdProducto;

            DataTable dt;
            try
            {
                dt = Conexion.SqlDataTableProductProfile(sql);
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                dt = null;
            }

            return dt;
        }
        public static DataTable SearchCompCIF(int IdProducto, int IdPaisAduana)
        {
            var sql = " SELECT Año, Mes = CASE Mes WHEN 1 THEN 'Ene' WHEN 2 THEN 'Feb' WHEN 3 THEN 'Mar' WHEN 4 " +
                      "THEN 'Abr' WHEN 5 THEN 'May' WHEN 6 THEN 'Jun' WHEN 7 THEN 'Jul' WHEN 8 THEN 'Ago' " +
                      "WHEN 9 THEN 'Sep' WHEN 10 THEN 'Oct' WHEN 11 THEN 'Nov' WHEN 12 THEN 'Dic' END," +
                      " Valor FROM TOTALESAÑO_MES WHERE IDPRODUCTO = " + IdProducto + " AND IDPAISADUANA = " + IdPaisAduana + 
                      " AND REGIMEN = 'IMPORTACIONES' AND(AÑO BETWEEN(SELECT TOP 1 AÑO FROM TOTALESAÑO_MES  GROUP BY AÑO ORDER BY AÑO ASC) + 2 " +
                      "AND(SELECT TOP 1 AÑO FROM TOTALESAÑO_MES GROUP BY AÑO ORDER BY AÑO DESC))";

            DataTable dt;
            try
            {
                dt = Conexion.SqlDataTableProductProfile(sql);
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                dt = null;
            }

            return dt;
        }
        public static DataTable SearchYear(int IdProducto, int IdPaisAduana)
        {
            var sql = " SELECT TOP 3 AÑO FROM TOTALESAÑO_MES WHERE IDPAISADUANA = "+ IdPaisAduana +" AND IDPRODUCTO = "+IdProducto+" GROUP BY AÑO ORDER BY AÑO DESC";

            DataTable dt;
            try
            {
                dt = Conexion.SqlDataTableProductProfile(sql);
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                dt = null;
            }

            return dt;
        }
        public static int CuentaRegistros(string sql)
        {
            int cuenta = 0;
            try
            {
                var dt = Conexion.SqlDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    cuenta = Convert.ToInt32((dt.Rows[0])["Cant"]);
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }

            return cuenta;
        }

        public static List<OptionSelect> AgregaFiltros(string filtro, string codPais, ArrayList idsSeleccionados)
        {
            string id, tipo = "", nombre = "";
            var json = new List<OptionSelect>();

            for (int i = 0; i < idsSeleccionados.Count; i += 1)
            {
                id = idsSeleccionados[i].ToString();

                switch (filtro)
                {
                    case "Partida":
                        tipo = "2PA";
                        nombre = "[" + Resources.AdminResources.Nandina2_FilterText + "] " + BuscaPartida(id, codPais);
                        break;
                    case "Marca":
                        tipo = "2MA";
                        nombre = "[" + Resources.Resources.Search_Form_BrandField + "] " + BuscaMarca(id, codPais);
                        break;
                    case "Modelo":
                        tipo = "2MO";
                        nombre = "[" + Resources.AdminResources.Model_FormField_Label + "] " + BuscaModelo(id, codPais);
                        break;
                    case "Importador":
                        tipo = "3IM";
                        nombre = "[" + Resources.Resources.Search_Form_Item05 + "] " + BuscaEmpresa(id, codPais);
                        break;
                    case "Exportador":
                        tipo = "3EX";
                        nombre = "[" + Resources.Resources.Search_Form_Item06 + "] " + BuscaEmpresa(id, codPais);
                        break;
                    //case "Importador":
                    //    if (TipoOpe == "I")
                    //    {
                    //        Tipo = "3IM";
                    //        Nombre = "[Importador] " + BuscaEmpresa(ID, CodPais);
                    //    }
                    //    else
                    //    {
                    //        Tipo = "3EX";
                    //        Nombre = "[Exportador] " + BuscaEmpresa(ID, CodPais);
                    //    }
                    //    break;
                    case "Notificado":
                        tipo = "3NO";
                        nombre = "[" + Resources.AdminResources.Notified_FilterText + "] " + BuscaNotificado(id, codPais);
                        break;
                    case "Proveedor":
                        tipo = "4PR";
                        nombre = ((codPais != "CL" ) ? "[" + Resources.Resources.Search_Form_Item06 + "] " : "[" + Resources.Resources.Search_Form_BrandField + "] ") + BuscaProveedor(id, codPais);
                        break;
                    case "ImportadorExp":
                        tipo = "4IE";
                        nombre = "[" + Resources.Resources.Search_Form_Item05 + "] " + BuscaImportadorExp(id, codPais);
                        break;
                    //case "Proveedor":
                    //    if (TipoOpe == "I")
                    //    {
                    //        Tipo = "4PR";
                    //        Nombre = ((CodPais != "CL") ? "[Exportador] " : "[Marca] ") + BuscaProveedor(ID, CodPais);
                    //    }
                    //    else
                    //    {
                    //        Tipo = "4IE";
                    //        Nombre = "[Importador] " + BuscaImportadorExp(ID, CodPais);
                    //    }

                    //    break;
                    case "PaisEmbarque":
                    case "PaisOrigen":

                        if (codPais != "USI" && codPais != "PEI")
                        {
                            tipo = "5PO";
                            nombre = "[" + Resources.AdminResources.OriginCountry_FormField_Label + "] " + BuscaPais(id, codPais);
                        }
                        else
                        {
                            tipo = "5PE";
                            nombre = "[" + Resources.AdminResources.LastShipmentCountry_FilterText + "] " + BuscaPais(id, codPais);
                        }

                        //if (tipoOpe == "I" && codPais != "USI" && codPais != "PEI")
                        //{
                        //    tipo = "5PO";
                        //    nombre = "["+ Resources.AdminResources.OriginCountry_FormField_Label+"] " + BuscaPais(id, codPais);
                        //}
                        //else if (tipoOpe == "I" && (codPais == "USI" || codPais == "PEI"))
                        //{
                        //    tipo = "5PE";
                        //    nombre = "["+Resources.AdminResources.LastShipmentCountry_FilterText + "] " + BuscaPais(id, codPais);
                        //}
                        //else
                        //{
                        //    tipo = "5PD";
                        //    nombre = "["+ Resources.AdminResources.DestinationCountry_FormField_Label+"] " + BuscaPais(id, codPais);
                        //}
                        break;
                    case "PaisDestino":
                        tipo = "5PD";
                        nombre = "[" + Resources.AdminResources.DestinationCountry_FormField_Label + "] " + BuscaPais(id, codPais);
                        break;
                    case "PtoDescarga":
                    case "PtoDestino":
                    case "PtoEmbarque":
                    case "ViaTransp":
                        if (codPais == "USI")
                        {
                            tipo = "6PD";
                            nombre = "[" + Resources.AdminResources.UnloadingPort_FilterText + "] " + BuscaPuerto(id, codPais);
                        }
                        else if (codPais == "PEE" || codPais == "ECE")
                        {
                            tipo = "6DE";
                            nombre = "[" + Resources.AdminResources.DestinationPort_FilterText + "] " + BuscaPuerto(id, codPais);
                        }
                        else if (codPais == "USE" || codPais == "PEI" || codPais == "BRI" || codPais == "BRE" || codPais == "ECI" )
                        {
                            tipo = "6PE";
                            nombre = "[" + Resources.AdminResources.LastShipmentPort_FilterText + "] " + BuscaPuerto(id, codPais);
                        }
                        else
                        {
                            tipo = "6VT";
                            nombre = "[" + Resources.AdminResources.FilterText_ViaTransp + "] " + Funciones.BuscaVia(id, codPais);
                        }

                        break;
                    case "AduanaDUA":
                        tipo = "7AD";
                        nombre = "[" + Resources.AdminResources.CustomsDUA_FilterText + "] " + BuscaAduana(id.Split('-')[0], codPais) + ' ' + id.Split('-')[1];
                        break;
                    case "Aduana":
                        tipo = "7AA";
                        nombre = "[" + Resources.AdminResources.Customs_FilterText + "] " + BuscaAduana(id, codPais);
                        break;
                    case "Distrito":
                        tipo = "8DI";
                        nombre = "[" + Resources.AdminResources.FilterText_District + "] " + BuscaDistrito(id, codPais);
                        break;
                    case "Manifiesto":
                        tipo = "9MA";
                        nombre = "[" + Resources.AdminResources.Manifest_FilterText + "] " + id;
                        break;
                    case "MarcaEC":
                        tipo = "6MA";
                        nombre = "[" + Resources.Resources.Search_Form_BrandField + "] " + Funciones.BuscaMarcaEC(id, codPais);
                        break;
                }

                json.Add(new OptionSelect
                {
                    text = nombre,
                    value = tipo + id
                });
            }

            return json;
        }

        public static string BuscaMarca(string idMarca, string codPais)
        {
            var marca = "";
            var sql = "select Marca from Marca_" + codPais + " where IdMarca = " + idMarca;
            try
            {
                var dt = Conexion.SqlDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    marca = (dt.Rows[0])["Marca"].ToString();
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }
            return marca;
        }

        public static string BuscaModelo(string idModelo, string codPais)
        {
            var modelo = "";
            var sql = "select Marca, Modelo from Modelo_" + codPais + " where IdModelo = " + idModelo;
            try
            {
                var dt = Conexion.SqlDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    modelo = (dt.Rows[0])["Marca"].ToString() + " - " + (dt.Rows[0])["Modelo"].ToString();
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }
            return modelo;
        }

        public static string BuscaNotificado(string idNotificado, string codPais)
        {
            var notificado = "";
            var sql = "select Notificado from Notificado_" + codPais + " where IdNotificado = " + idNotificado;
            try
            {
                var dt = Conexion.SqlDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    notificado = (dt.Rows[0])["Notificado"].ToString();
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }

            return notificado;
        }

        public static string BuscaPais(string idPais, string codPais)
        {
            var pais = "";
            var sql = "select Pais from Pais_" + codPais + " where IdPais = " + idPais;
            try
            {
                var dt = Conexion.SqlDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    pais = (dt.Rows[0])["Pais"].ToString();
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }
            return pais;
        }

        public static string BuscaPuerto(string idPto, string codPais)
        {
            var puerto = "";
            var sql = "select Puerto from Puerto_" + codPais + " where IdPto = " + idPto;
            try
            {
                var dt = Conexion.SqlDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    puerto = (dt.Rows[0])["Puerto"].ToString();
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }

            return puerto;
        }

        public static string BuscaAduana(string idAduana, string codPais)
        {
            var aduana = "";
            var sql = "select Aduana from Aduana_" + codPais + " where IdAduana = " + idAduana;

            try
            {
                var dt = Conexion.SqlDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    aduana = (dt.Rows[0])["Aduana"].ToString();
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }

            return aduana;
        }

        public static string BuscaDistrito(string idDistrito, string codPais)
        {
            var distrito = "";
            var sql = "select Distrito from Distrito_" + codPais + " where IdDistrito = " + idDistrito;
            try
            {
                var dt = Conexion.SqlDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    distrito = (dt.Rows[0])["Distrito"].ToString();
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }
            return distrito;
        }

        public static ArrayList GuardaSeleccionados(ArrayList idsSeleccionadosOrigen, ArrayList idsSeleccionadosDestino)
        {
            string id;
            if (idsSeleccionadosDestino == null)
                idsSeleccionadosDestino = new ArrayList();

            for (int i = 0; i < idsSeleccionadosOrigen.Count; i += 1)
            {
                id = idsSeleccionadosOrigen[i].ToString();
                if (!idsSeleccionadosDestino.Contains(id))
                    idsSeleccionadosDestino.Add(id);
            }
            return idsSeleccionadosDestino;
        }

        public static string BuscaNandina(string idPartida, string codPais)
        {
            var nandina = "";

            string sql = "select Nandina from Partida_" + codPais + " where IdPartida = " + idPartida;
            try
            {
                var dt = Conexion.SqlDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    nandina = (dt.Rows[0])["Nandina"].ToString();
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }
            return nandina;
        }

        public static string BuscaPartida2(string idPartida, string codPais, string idioma)
        {
            var partida = "";

            var lPartida = idioma == "en" ? "Partida_en" : "Partida";

            var sql = "select " + lPartida + " as Partida from Partida_" + codPais + " where IdPartida = " + idPartida;

            try
            {
                var dt = Conexion.SqlDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    partida = (dt.Rows[0])["Partida"].ToString();
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }
            return partida;
        }

        public static DataTable LlenaGrupos(bool flagFiltro, string idUsuario, string codPais,
            string tipoOpe, string tipoFavorito, string idioma)
        {
            string sql = "";
            var lTodos = "TODOS";
            var lIndividual = "INDIVIDUAL";

            if (idioma == "en")
            {
                lTodos = "ALL";
                lIndividual = "SINGLE";
            }

            string tipoFav = tipoFavorito != "ImportadorExp" ? tipoFavorito.Substring(0, 2) : "IE";

            if (flagFiltro)
            {
                sql = "select 0 as Orden, -1 as IdGrupo, '[" + lTodos + "]' as Grupo union select 1 as Orden, 0 as IdGrupo, '[" + lIndividual + "]' as Grupo ";
                sql += "union select 2 as Orden, IdGrupo, '[G] ' + Grupo as Grupo from Grupo ";
            }
            else
                sql = "select 2 as Orden, IdGrupo, Grupo from Grupo ";

            sql += "where IdUsuario = " + idUsuario + " and CodPais = '" + codPais + "' and TipoOpe = '" + tipoOpe + "' ";
            sql += "and TipoFav = '" + tipoFav + "' ";
            sql += "and IdGrupo <> 335159"; // IdGrupoBloqueado
            sql += "order by 1, 3";

            DataTable dt = null;
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
       
        public static bool ExisteGrupo(string grupo, string idUsuario, string codPais,
            string tipoOpe, string tipoFav)
        {
            var cant = 0;
            var sql = "select count(*) as cant from Grupo where Grupo = '" + grupo + "' and IdUsuario = " + idUsuario + " ";
            sql += "and CodPais = '" + codPais + "' and TipoOpe = '" + tipoOpe + "' and TipoFav = '" + tipoFav + "'";

            try
            {
                var dt = Conexion.SqlDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    cant = Convert.ToInt32((dt.Rows[0])["cant"]);
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }

            return (cant > 0);
        }

        public static int CantFavUnicos(string idUsuario, string codPais, string tipoOpe,
            string tipoFavorito)
        {
            var tipoFav = tipoFavorito != "ImportadorExp" ? tipoFavorito.Substring(0, 2).ToUpper() : "IE";

            string sql = "select count(*) as Cant from V_FavUnicos ";
            sql += "where IdUsuario = " + idUsuario + " and CodPais = '" + codPais + "' and TipoOpe = '" + tipoOpe + "' and TipoFav = '" + tipoFav + "' ";

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
            }

            return cant;
        }

        public static bool ExisteFavUnico(string idUsuario, string codPais, string tipoOpe,
            string tipoFavorito, string idFavorito)
        {
            var cant = 0;
            var tipoFav = tipoFavorito.Substring(0, 2);
            if (tipoFavorito == "ImportadorExp")
                tipoFav = "IE";

            var sql = "select count(*) as Cant from V_FavUnicos ";
            sql += "where IdUsuario = " + idUsuario + " and CodPais = '" + codPais + "' and TipoOpe = '" + tipoOpe + "' and TipoFav = '" + tipoFav + "' and IdFavorito = " + idFavorito;

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
            }

            return cant > 0;

        }

        public static bool AgregaFavoritos(string idUsuario, string codPais, string tipoOpe, string tipoFavorito,
            ArrayList listaIdsSeleccionados, string idioma, ref string mensaje)
        {
            bool flagLimFavUnicos = false;
            bool flagOk = true;

            var cantSelec = listaIdsSeleccionados.Count;
            var cantGrab = GrabaFavoritos(listaIdsSeleccionados, tipoFavorito, idUsuario, codPais, tipoOpe, ref flagLimFavUnicos);
            var idPlan = Funciones.ObtieneIdPlan(idUsuario);
            var limiteFavUnicos = Funciones.ObtieneLimite(idPlan, "LimiteFavUnicos");

            if (cantSelec == 1)
            {
                if (cantGrab == 1)
                {
                    mensaje = Resources.AdminResources.Message_FavoriteAdded;
                }
                else
                {
                    mensaje = !flagLimFavUnicos ? Resources.AdminResources.Message_FavoriteExists : Resources.AdminResources.Message_FavoriteNotAdded;
                }

            }
            else
            {
                if (cantSelec == cantGrab)
                {
                    mensaje = Resources.AdminResources.Message_FavoritesAdded;
                }
                else
                {
                    if (cantGrab == 0 && !flagLimFavUnicos)
                    {
                        mensaje = Resources.AdminResources.Message_FavoritesExists;
                    }
                    else
                    {
                        mensaje = Resources.AdminResources.Message_SomeoneFavoritesAdded;
                    }
                }
            }


            if (flagLimFavUnicos)
            {
                mensaje += "<br>Nota: Se ha alcanzado el límite de " + limiteFavUnicos.ToString() + " Favoritos";
                if (idioma == "en")
                    mensaje = "<br>Note: It reached favorites limit: " + limiteFavUnicos.ToString();

                flagOk = false;
            }

            return flagOk;
        }


        public static int GrabaFavoritos(ArrayList listaIdsSeleccionados, string tipoFavorito, string idUsuario,
            string codPais, string tipoOpe, ref bool flagLimFavUnicos)
        {
            flagLimFavUnicos = false;
            int cont = 0;
            if (listaIdsSeleccionados != null && listaIdsSeleccionados.Count > 0)
            {
                for (int i = 0; i < listaIdsSeleccionados.Count; i++)
                {
                    if (!ExisteFavorito(tipoFavorito, idUsuario, codPais, tipoOpe, (listaIdsSeleccionados[i]).ToString()))
                        if (ValidaFavUnicos(idUsuario, codPais, tipoOpe, tipoFavorito))
                        {
                            AgregaFavorito(tipoFavorito, idUsuario, codPais, tipoOpe, (listaIdsSeleccionados[i]).ToString());
                            cont += 1;
                        }
                        else
                            flagLimFavUnicos = true;
                }
            }
            return cont;
        }

        public static bool ExisteFavorito(string tipoFavorito, string idUsuario, string codPais,
            string tipoOpe, string idFavorito)
        {
            int cant = 0;
            string sql = "";

            switch (tipoFavorito)
            {
                case "Partida":
                    sql = "select count(*) as cant from PartidaFav where IdUsuario = " + idUsuario +
                          " and CodPais = '" + codPais + "' ";
                    sql += "and TipoOpe = '" + tipoOpe + "' and IdPartida = " + idFavorito + " ";
                    break;
                case "Importador":
                    sql = "select count(*) as cant from EmpresaFav where IdUsuario = " + idUsuario +
                          " and CodPais = '" + codPais + "' ";
                    sql += "and TipoOpe = 'I' and IdEmpresa = " + idFavorito + " ";
                    break;
                case "Proveedor":
                    sql = "select count(*) as cant from ProveedorFav where IdUsuario = " + idUsuario +
                          " and CodPais = '" + codPais + "' ";
                    sql += "and IdProveedor = " + idFavorito + " ";
                    break;
                case "Exportador":
                    sql = "select count(*) as cant from EmpresaFav where IdUsuario = " + idUsuario +
                          " and CodPais = '" + codPais + "' ";
                    sql += "and TipoOpe = 'E' and IdEmpresa = " + idFavorito + " ";
                    break;
                case "ImportadorExp":
                    sql = "select count(*) as cant from ImportadorExpFav where IdUsuario = " + idUsuario +
                          " and CodPais = '" + codPais + "' ";
                    sql += "and IdImportadorExp = " + idFavorito + " ";
                    break;
            }

            try
            {
                var dt = Conexion.SqlDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    cant = Convert.ToInt32((dt.Rows[0])["cant"]);
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }

            return cant > 0;
        }

        public static bool ValidaFavUnicos(string idUsuario, string codPais, string tipoOpe,
            string tipoFavorito)
        {
            int limiteFavUnicos = 0, favUnicos = 0;

            var tipoFav = tipoFavorito != "ImportadorExp" ? tipoFavorito.Substring(0, 2).ToUpper() : "IE";

            var idPlan = Funciones.ObtieneIdPlan(idUsuario);

            var sql = "select LimiteFavUnicos from [Plan] where IdPlan = " + idPlan;

            try
            {
                var dt = Conexion.SqlDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    limiteFavUnicos = Convert.ToInt32((dt.Rows[0])["LimiteFavUnicos"]);
                }

                sql = "select count(*) as FavUnicos from V_FavUnicos ";
                sql += "where IdUsuario = " + idUsuario + " and CodPais = '" + codPais + "' ";
                sql += "and TipoOpe = '" + tipoOpe + "' and TipoFav = '" + tipoFav + "' ";


                var dt2 = Conexion.SqlDataTable(sql);

                if (dt2.Rows.Count > 0)
                {
                    favUnicos = Convert.ToInt32((dt2.Rows[0])["FavUnicos"]);
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }

            return favUnicos < limiteFavUnicos;
        }

        public static void AgregaFavorito(string tipoFavorito, string idUsuario, string codPais, string tipoOpe,
            string idFavorito)
        {
            string sql = "";

            switch (tipoFavorito)
            {
                case "Partida":
                    sql = "insert into PartidaFav(IdUsuario, CodPais, TipoOpe, IdPartida, FecCreacion) ";
                    sql += "values (" + idUsuario + ", '" + codPais + "', '" + tipoOpe + "', " + idFavorito +
                           ", getdate())";
                    /*
                    string PartidaFav;
                    PartidaFav = BuscaPartida(IdFavorito, CodPais);
                    if (PartidaFav.Length > 60) PartidaFav = PartidaFav.Substring(0, 60);
                    sql = "insert into PartidaFav(IdUsuario, CodPais, TipoOpe, IdPartida, PartidaFav, FecCreacion) ";
                    sql += "values (" + IdUsuario + ", '" + CodPais + "', '" + TipoOpe + "', " + IdFavorito + ", '" + PartidaFav.Replace("'", "''") + "', getdate())";
                    */
                    break;
                case "Importador":
                    sql = "insert into EmpresaFav(IdUsuario, CodPais, TipoOpe, IdEmpresa, FecCreacion) ";
                    sql += "values (" + idUsuario + ", '" + codPais + "', 'I'," + idFavorito + ", getdate())";
                    break;
                case "Proveedor":
                    sql = "insert into ProveedorFav(IdUsuario, CodPais, TipoOpe, IdProveedor, FecCreacion) ";
                    sql += "values (" + idUsuario + ", '" + codPais + "', 'I', " + idFavorito + ", getdate())";
                    break;
                case "Exportador":
                    sql = "insert into EmpresaFav(IdUsuario, CodPais, TipoOpe, IdEmpresa, FecCreacion) ";
                    sql += "values (" + idUsuario + ", '" + codPais + "', 'E', " + idFavorito + ", getdate())";
                    break;
                case "ImportadorExp":
                    sql = "insert into ImportadorExpFav(IdUsuario, CodPais, TipoOpe, IdImportadorExp, FecCreacion) ";
                    sql += "values (" + idUsuario + ", '" + codPais + "', 'E', " + idFavorito + ", getdate())";
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

        public static void ActualizaPartidaFavorita(string idUsuario, string codPais, string tipoOpe, string idPartida,
            string partidaFav)
        {
            partidaFav = partidaFav.Trim().Replace("'", "");

            if (partidaFav.Length > 80)
                partidaFav = partidaFav.Substring(0, 80);

            partidaFav = CambiaNombre(partidaFav);

            string sql = "update PartidaFav set PartidaFav = '" + partidaFav + "'";
            sql += "where IdUsuario = " + idUsuario + " and CodPais = '" + codPais + "' and TipoOpe = '" + tipoOpe + "' and IdPartida = " + idPartida;

            try
            {
                Conexion.SqlExecute(sql);
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }
        }

        public static string CambiaNombre(string nombre)
        {
            if (nombre == "")
                return "";

            var letra = nombre.Substring(0, 1).ToUpper();
            if (nombre.Length == 1)
                return letra;

            var resto = nombre.Substring(1, nombre.Length - 1).ToLower();

            return letra + resto;
        }

        public static bool ActualizaGrupo(string idUsuario, string codPais, string tipoOpe, string tipoFavorito,
            bool flagCreaGrupo, string nuevoGrupo, string idGrupo, ArrayList IDsSeleccionados, string idioma,
            ref string mensaje)
        {
            string TipoFav;
            bool flagOk = true;

            TipoFav = tipoFavorito != "ImportadorExp" ? tipoFavorito.Substring(0, 2).ToUpper() : "IE";

            bool validaGrupos = ValidaGrupos(idUsuario, codPais, tipoOpe, tipoFavorito);

            if (flagCreaGrupo && !validaGrupos)
            {
                string IdPlan = Funciones.ObtieneIdPlan(idUsuario);
                int LimiteGrupos = Funciones.ObtieneLimite(IdPlan, "LimiteGrupos");

                if (mensaje != "")
                    mensaje += "<br>";
                if (idioma == "es")
                    mensaje += "No se pudo crear el Grupo.<br>Nota: Se ha alcanzado el máximo de " + LimiteGrupos.ToString() + " Grupos";
                else
                    mensaje += "Could not create group. <br>Note: It reached Groups number limit: " + LimiteGrupos.ToString();
                flagOk = false;
            }
            else
            {
                bool flagLimFavPorGrupo = false;
                bool flagLimFavPorUnicos = false;
                if (flagCreaGrupo)
                    idGrupo = CreaGrupo(nuevoGrupo, idUsuario, codPais, tipoOpe, TipoFav);

                int CantFavIni, CantSelec, CantGrab, CantFav, LimiteFavPorGrupo;

                CantFavIni = CantFavoritosGrupo(idGrupo);
                string IdPlan = Funciones.ObtieneIdPlan(idUsuario);
                LimiteFavPorGrupo = Funciones.ObtieneLimite(IdPlan, "LimiteFavPorGrupo");

                if (CantFavIni >= LimiteFavPorGrupo)
                {
                    if (mensaje != "")
                        mensaje += "<br>";

                    if (idioma == "es")
                        mensaje += "No se pudo agregar los favoritos seleccionados al grupo.<br>Nota: Se ha alcanzado el máximo de " + LimiteFavPorGrupo.ToString() + " Favoritos en el Grupo";
                    else
                        mensaje += "Could not add selected favorites to group.<br>Note: It reached Favorites number by group limit: " + LimiteFavPorGrupo.ToString();

                    flagOk = false;

                    return false;
                }

                ArrayList IDsSeleccionadosFavUnicos = SeleccionFavUnicos(IDsSeleccionados);
                CantSelec = IDsSeleccionadosFavUnicos.Count;
                CantGrab = GrabaFavoritosAGrupo(idUsuario, IDsSeleccionadosFavUnicos, idGrupo,  codPais,  tipoOpe,  tipoFavorito, ref flagLimFavPorGrupo,ref flagLimFavPorUnicos);
                CantFav = CantFavoritosGrupo(idGrupo);

                if (mensaje != "") mensaje += "<br>";
                if (idioma == "es")
                    mensaje += "Los favoritos seleccionados se agregaron al grupo";
                else
                    mensaje += "Selected favorites added to group successfully";


                if (flagLimFavPorGrupo)
                {
                    if (idioma == "es")
                        mensaje += "<br>Nota: Se ha alcanzado el máximo de " + LimiteFavPorGrupo.ToString() + " Favoritos en el Grupo";
                    else
                        mensaje += "<br>Note: It reached Favorites number by group limit: " + LimiteFavPorGrupo.ToString();
                    flagOk = false;
                }
                else if(flagLimFavPorUnicos)
                {
                    mensaje += "<br>"+Resources.AdminResources.MessageLimitPlanFavorites;
                    flagOk = false;
                }

            }

            return flagOk;
        }

        public static bool ValidaGrupos(string idUsuario, string codPais, string tipoOpe,
            string tipoFavorito)
        {
            int limiteGrupos = 0, grupos = 0;

            //string TipoFav;
            //TipoFav = tipoFavorito != "ImportadorExp" ? tipoFavorito.Substring(0, 2).ToUpper() : "IE";

            var idPlan = Funciones.ObtieneIdPlan(idUsuario);

            string sql = "select LimiteGrupos from [Plan] where IdPlan = " + idPlan;

            try
            {
                var dt = Conexion.SqlDataTable(sql);

                if (dt.Rows.Count > 0)
                {
                    limiteGrupos = Convert.ToInt32((dt.Rows[0])["LimiteGrupos"]);
                }

                sql = "select count(*) as Grupos from Grupo where IdUsuario = " + idUsuario + " and CodPais = '" + codPais +
                      "' ";
                sql += "and TipoOpe = '" + tipoOpe + "' "; //and TipoFav = '" + TipoFav + "'";

                var dt2 = Conexion.SqlDataTable(sql);

                if (dt2.Rows.Count > 0)
                {
                    grupos = Convert.ToInt32((dt2.Rows[0])["Grupos"]);
                }

            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }

            return grupos < limiteGrupos;
        }


        public static string CreaGrupo(string grupo, string idUsuario, string codPais, string tipoOpe,
            string tipoFav)
        {
            string idGrupo = "";

            string sql = "insert into Grupo(IdUsuario, CodPais, TipoOpe, TipoFav, Grupo, FecCreacion) ";
            sql += "values (" + idUsuario + ", '" + codPais + "','" + tipoOpe + "', '" + tipoFav + "', '" + grupo +
                   "', getdate())";

            try
            {
                Conexion.SqlExecute(sql);

                sql = "select max(IdGrupo) as IdGrupo from Grupo";
                var dt = Conexion.SqlDataTable(sql);

                if (dt.Rows.Count > 0)
                {
                    idGrupo = (dt.Rows[0])["IdGrupo"].ToString();
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }

            return idGrupo;

        }

        public static int CantFavoritosGrupo(string idGrupo)
        {
            int cant = 0;
            string sql = "select count(*) as Cant from FavoritoGrupo where IdGrupo = " + idGrupo;

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
            }

            return cant;
        }

        public static ArrayList SeleccionFavUnicos(ArrayList seleccion)
        {
            ArrayList aux = new ArrayList();
            string id, idFavorito;

            if (seleccion != null && seleccion.Count > 0)
                for (int i = 0; i < seleccion.Count; i++)
                {
                    id = (seleccion[i]).ToString();

                    if (id.IndexOf("-") > 0)
                        idFavorito = id.Substring(0, id.IndexOf("-"));
                    else
                        idFavorito = id;

                    if (!aux.Contains(idFavorito)) aux.Add(idFavorito);
                }

            return aux;
        }

        public static int GrabaFavoritosAGrupo(string IdUsuario, ArrayList IDsSeleccionados, string IdGrupo, string codPais, string tipoOpe, string tipoFavorito,
            ref bool flagLimFavPorGrupo, ref bool flagLimFavUnicos)
        {
            flagLimFavPorGrupo = false;
            flagLimFavUnicos = false;
            int cont = 0;

            if (IDsSeleccionados != null && IDsSeleccionados.Count > 0)
                for (int i = 0; i < IDsSeleccionados.Count; i++)
                {
                    string IdFavorito = (IDsSeleccionados[i]).ToString();
                    if (!ExisteFavoritoEnGrupo(IdGrupo, IdFavorito))
                        if (ValidaFavPorGrupo(IdUsuario, IdGrupo))
                        {
                            //if (ValidaLimitAddFav(IdUsuario, codPais, tipoOpe, tipoFavorito))
                            //{
                                AgregaFavoritoAGrupo(IdGrupo, IdFavorito , IdUsuario, codPais , tipoOpe , tipoFavorito);
                                cont += 1;
                            //}
                            //else
                            //    flagLimFavUnicos = true;
                        }
                        else
                            flagLimFavPorGrupo = true;
                }

            return cont;
        }

        public static bool ValidaLimitAddFav(string idUsuario, string codPais, string tipoOpe, string tipoFavorito , string idFavorito)
        {
            var idPlan = Funciones.ObtieneIdPlan(idUsuario);
            var limiteFavUnicos = Funciones.ObtieneLimite(idPlan, "LimiteFavUnicos");
            var cantFavUnicos = FuncionesBusiness.CantFavUnicos(idUsuario, codPais, tipoOpe, tipoFavorito);

            var tipoFav = tipoFavorito != "ImportadorExp" ? tipoFavorito.Substring(0, 2).ToUpper() : "IE";

            string sql = "select count(*) as Cant from V_FavUnicos ";
            sql += "where IdUsuario = " + idUsuario + " and CodPais = '" + codPais + "' and TipoOpe = '" 
                + tipoOpe + "' and TipoFav = '" + tipoFav + "' and idFavorito = '" + idFavorito + "'";

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
            }


            return (cant != 0 || limiteFavUnicos > cantFavUnicos);
        }

        public static bool ExisteFavoritoEnGrupo(string idGrupo, string idFavorito)
        {
            int cant = 0;
            var sql = "select count(*) as Cant from FavoritoGrupo where IdGrupo = " + idGrupo + " and IdFavorito = " +
                         idFavorito + " ";
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
            }

            return cant > 0;
        }

        public static bool ValidaFavPorGrupo(string idUsuario, string idGrupo)
        {
            int limiteFavPorGrupo = 0, favPorGrupo = 0;

            var idPlan = Funciones.ObtieneIdPlan(idUsuario);

            string sql = "select LimiteFavPorGrupo from [Plan] where IdPlan = " + idPlan;

            try
            {
                var dt = Conexion.SqlDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    limiteFavPorGrupo = Convert.ToInt32((dt.Rows[0])["LimiteFavPorGrupo"]);
                }

                sql = "select count(*) as FavPorGrupo from FavoritoGrupo where IdGrupo = " + idGrupo;

                var dt2 = Conexion.SqlDataTable(sql);
                if (dt2.Rows.Count > 0)
                {
                    favPorGrupo = Convert.ToInt32((dt2.Rows[0])["FavPorGrupo"]);
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                Console.WriteLine(ex);
                throw;
            }

            return (favPorGrupo < limiteFavPorGrupo);
        }



        public static void AgregaFavoritoAGrupo(string idGrupo, string idFavorito, string IdUsuario, string codPais, string tipoOpe, string tipoFavorito)
        {
            var cant = -1;

            var sql = "select count(*) as Cant from FavoritoGrupo where IdGrupo = " + idGrupo + " and IdFavorito = " +
                         idFavorito + " ";
            try
            {
                var dt = Conexion.SqlDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    cant = Convert.ToInt32((dt.Rows[0])["Cant"]);
                }

                if (cant == 0 && ValidaLimitAddFav(IdUsuario, codPais, tipoOpe, tipoFavorito , idFavorito))
                {
                    sql = "insert into FavoritoGrupo(IdGrupo, IdFavorito, FecCreacion) ";
                    sql += "values (" + idGrupo + ", " + idFavorito + ", getdate())";

                    Conexion.SqlExecute(sql);
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }
        }

        public static DataTable BuscarRegistros(string codPais2, string tipoOpe, string tabla,
            string sqlFiltroR2, string hdfIdGrupoB, ref string sqlFinal,bool isVerRegistros = false,string sqlCampos = "" , int pagina = 0 , string filtro = "")
        {
            string sql = "";

            if (codPais2 == "4UE" && tipoOpe == "I")
            {
                sql =
                    "select 0 as IdImportacion, FechaNum_date, Nandina, PesoBruto, Cantidad, Unidad, FOBUnit, PaisOrigen ";                
            }
            else if (codPais2 == "4UE" && tipoOpe == "E")
                sql =
                    "select 0 as IdExportacion, FechaNum_date, Nandina, PesoBruto, Cantidad, Unidad, FOBUnit, PaisDestino ";
            else
            {
                if(sqlCampos != "")
                {
                    if(pagina > 0)
                    {
                        sql = "select * from ( ";
                    }

                    if (isVerRegistros)
                        sql += $"select top 3 {sqlCampos} ";
                    else
                        sql += $"select {sqlCampos} ";
                }
                else
                {
                    if (isVerRegistros)
                        sql += "select top 3 * ";
                    else
                        sql = "select * ";
                }
                
            }

            sql += "from " + tabla + " ";
            sql += "where 1 = 1 ";

            sql += sqlFiltroR2;

            if (hdfIdGrupoB == null)
            {
                if(!tabla.Contains("Importacion_") && !tabla.Contains("Exportacion_"))
                    if(!isVerRegistros)
                    sql += "order by Trimestre desc";
            }

            if (sql.Contains("IdMarca") || sql.Contains("IdModelo"))
                sql = sql.Replace("Importacion_PE", "Importacion_PEB");
            if (sql.Contains("PEBB"))
                sql = sql.Replace("PEBB", "PEB");

            sqlFinal = sql;

            if(sqlCampos != "" && pagina > 0)
            {
                
                sql += $@") as T where Nro between "+((pagina-1)*10 + 1) + " and "+(pagina*10) ;
            }

            if((filtro == "InfoTabla" || filtro == "InfoComplementario") && sqlCampos == "")
            {
                sql += " ORDER BY Fechanum_date DESC";
            }

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

        public static string ObtieneFechaIni(string TrimIni)
        {
            string FechaIni = "";
            string IdAño = TrimIni.Substring(0, 4);
            string IdTrim = TrimIni.Substring(4, 1);

            if (IdTrim == "1") FechaIni = IdAño + "0101";
            else if (IdTrim == "2") FechaIni = IdAño + "0401";
            else if (IdTrim == "3") FechaIni = IdAño + "0701";
            else FechaIni = IdAño + "1001";

            return FechaIni;
        }

        public static string ObtieneFechaFin(string TrimFin)
        {
            string FechaFin = "";
            string IdAño = TrimFin.Substring(0, 4);
            string IdTrim = TrimFin.Substring(4, 1);

            if (IdTrim == "1") FechaFin = IdAño + "0399";
            else if (IdTrim == "2") FechaFin = IdAño + "0699";
            else if (IdTrim == "3") FechaFin = IdAño + "0999";
            else FechaFin = IdAño + "1299";

            return FechaFin;
        }

        public static DataTable CargaDescargas(string idUsuario, string codPais, string codPais2,
            string tipoOpe, string idioma)
        {
            string lTodos = "TODOS LOS CAMPOS";
            string lDefecto = "Por defecto";
            if (idioma == "en")
            {
                lTodos = "ALL FIELDS";
                lDefecto = "Default";
            }

            string codPaisT = codPais;
            if (codPais == "PEP")
                codPaisT = "PE";
            else if (codPais2 == "4UE")
                codPaisT = "UE" + codPais;

            string sql = "select 0 as IdDescargaCab, '[" + lTodos + "]' as Descarga ";
            sql += "union ";
            sql += "select IdDescargaCab, case when FlagDefault = 'S' then Descarga + ' [" + lDefecto +
                   "]' else Descarga end as Descarga ";
            sql += "from DescargaCab where IdUsuario = " + idUsuario + " and CodPais = '" + codPaisT +
                   "' and TipoOpe = '" + tipoOpe + "' order by 1";

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
        public static string BuscaDescargaDefault(string idUsuario, string codPais, string tipoOpe)
        {
            string idDescargaCab = "";
            string sql = "select IdDescargaCab from DescargaCab where IdUsuario = " + idUsuario + " and CodPais = '" + codPais + "' and TipoOpe = '" + tipoOpe + "' and FlagDefault = 'S' ";

            try
            {
                var dt = Conexion.SqlDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    idDescargaCab = ((dt.Rows[0])["IdDescargaCab"]).ToString();
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }

            return idDescargaCab;
        }

        public static string BuscaRUC(string idEmpresa, string codPais)
        {
            string ruc = "";
            string sql = "select RUC from Empresa_" + codPais + " where IdEmpresa = " + idEmpresa + " ";
            try
            {
                var dt = Conexion.SqlDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    ruc = (dt.Rows[0])["RUC"].ToString();
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }
            return ruc;
        }

        public static string BuscaDescripcionSentinel(string codTabla, string valor)
        {
            string descripcion = "";
            string sql = "select Descripcion from Sentinel where CodTabla = '" + codTabla + "' and Valor = '" + valor + "'";

            try
            {
                var dt = Conexion.SqlDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    descripcion = (dt.Rows[0])["Descripcion"].ToString();
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }

            return descripcion;
        }

        public static int CantidadRegistrosPorConsulta(string sqlFiltroR2 , string tabla)
        {
            int cantidad = 0;
            string sql = "select COUNT(*) as CANTIDAD ";
            sql += "from " + tabla + " ";
            sql += "where 1 = 1 ";
            sql += sqlFiltroR2;
            try
            {
                var dt = Conexion.SqlDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    cantidad = Convert.ToInt32((dt.Rows[0])["CANTIDAD"].ToString());
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }

            return cantidad;
        }

        public static string ObtieneMaxIdLog(string idUsuario, string pagina)
        {
            string idLog = "";
            string sql = "select max(IdLog) as IdLog from Logs ";
            sql += "where IdUsuario = " + idUsuario + " and Pagina = '" + pagina + "'";
            try
            {
                var dt = Conexion.SqlDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    idLog = (dt.Rows[0])["IdLog"].ToString();
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }

            return idLog;
        }

        public static void ActualizaLog(string idLog, string logs)
        {
            string sql = "update Logs set Logs = '" + logs + "' where IdLog = " + idLog;
            try
            {
                Conexion.SqlExecute(sql);
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }

        }

        public static DataTable GeneraDt(string sql)
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

        public static string Pais(string codPais, string idioma)
        {
            string paisT = "";
            string lPais = "PaisES";
            if (idioma == "en")
                lPais = "Pais";
            string sql = "select ISNULL(" + lPais + ",Pais) as "+ lPais+" from AdminPais2 where CodPais = '" + codPais + "'";
            try
            {
                var dt = Conexion.SqlDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    paisT = (dt.Rows[0])[lPais].ToString();
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }

            return paisT;
        }

        public static string BuscaCodUsuario(string idUsuario)
        {
            string codUsuario = "";
            string sql = "select CodUsuario from Usuario where IdUsuario = " + idUsuario;
            try
            {
                var dt = Conexion.SqlDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    codUsuario = (dt.Rows[0])["CodUsuario"].ToString();
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }

            return codUsuario;
        }

        public static bool ValidaDescargasMes(string idUsuario, string codPais, string tipoOpe,
            ref int limiteDescargas, ref int descargas)
        {
            string IdPlan;

            IdPlan = Funciones.ObtieneIdPlan(idUsuario);

            string sql = "select LimiteDescargas from [Plan] where IdPlan = " + IdPlan;

            try
            {
                DataTable dt = new DataTable();
                dt = Conexion.SqlDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    limiteDescargas = Convert.ToInt32((dt.Rows[0])["LimiteDescargas"]);
                }

                sql = "select count(*) as Descargas from HistorialDescargas ";
                sql += "where IdUsuario = " + idUsuario + " ";
                sql += "and year(FecDescarga) * 100 + month(FecDescarga) = " +
                       (DateTime.Today.Year * 100 + DateTime.Today.Month).ToString();

                dt = Conexion.SqlDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    descargas = Convert.ToInt32((dt.Rows[0])["Descargas"]);
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                Funciones.GrabaLog(idUsuario, codPais, tipoOpe, "0", "0", "ValidaDescargasMes", ex.Message.Replace(",", ".").Replace("'", ""));
            }
            return descargas < limiteDescargas;
        }
        public static bool ForzarZip(string codUsuario)
        {
            bool aux = true;
            string sql = "select ForzarZip from Usuario where CodUsuario = '" + codUsuario + "' ";
            try
            {
                var dt = Conexion.SqlDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    aux = ((dt.Rows[0])["ForzarZip"].ToString() == "S");
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }

            return aux;
        }

        public static DataTable BuscarPais(string IdVariable , string idioma)
        {
            string tabla = "Descripcion";
            if (idioma == "en")
            {
                tabla = "Descripcion_Eng";
            }

            string sql = $"select IdVariable,{tabla} as Descripcion from VariableGeneral where IdVariable in ({IdVariable})";
            //string pais = "";
            DataTable dt;
            try
            {
                dt = Conexion.SqlDataTable(sql);
                //foreach (DataRow dataRow in dt.Rows)
                //{
                //    pais = dataRow["Descripcion"].ToString();
                //}
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                dt = null;
            }
            return dt;

        }

        public static string BuscarPaisNombre(string IdVariable, string idioma)
        {
            string tabla = "Descripcion";
            if (idioma == "en")
            {
                tabla = "Descripcion_Eng";
            }
            string pais = "";
            string sql = $"select IdVariable,{tabla} as Descripcion from VariableGeneral where IdVariable = '{IdVariable}'";
            //string pais = "";
            DataTable dt;
            try
            {
                dt = Conexion.SqlDataTable(sql);
                foreach (DataRow dataRow in dt.Rows)
                {
                    pais = dataRow["Descripcion"].ToString();
                }
            }
            catch (Exception ex)
            {
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                dt = null;
            }
            return pais;

        }

        public static int BuscarIdPaisNombre(string nombre, string codPais)
        {
            int idPais = 0;
            string sql = $"select IdPais from Pais_{codPais} where Pais like '%{nombre}%' or Pais_en like '%{nombre}%' or Pais_es like '%{nombre}%'";
            //string pais = "";
            DataTable dt;
            try
            {
                dt = Conexion.SqlDataTable(sql);
                foreach (DataRow dataRow in dt.Rows)
                {
                    idPais = Convert.ToInt32(dataRow["IdPais"]);
                }
            }
            catch (Exception ex)
            {
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                dt = null;
            }
            return idPais;

        }

        public static string ListaTitulosDescarga(string idDescargaCab, string campos, string codPais = "",
            string tipoOpe = "", string idioma = "es")
        {
            string lista = "";
            string sql;
            if (idDescargaCab != "0")
                sql = "select CampoFav from DescargaDet where IdDescargaCab = " + idDescargaCab + " ";
            else
            {
                string campoFav = "CampoFav";
                if (idioma == "en")
                    campoFav = "CampoFav_en";
                sql = "select " + campoFav + " as CampoFav from DescargaDet2 where CodPais = '" + codPais + "' and TipoOpe = '" + tipoOpe + "' ";
            }
            if (campos != "")
                sql += "and Campo in " + campos;

            sql += "order by NroCampo";

            try
            {
                var dt = Conexion.SqlDataTable(sql);
                foreach (DataRow dataRow in dt.Rows)
                {
                    lista += "''" + dataRow["CampoFav"].ToString() + "'', ";
                }
                lista = lista.Substring(0, lista.Length - 2);
                lista = " " + lista + " ";
            }
            catch (Exception ex)
            {
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }
            return lista;
        }

        public static string ListaCamposDescarga(string idDescargaCab, string campos, string codPais = "",
            string tipoOpe = "", string idioma = "es")
        {
            string campo, lista = "";
            string sql;

            if (idDescargaCab != "0")
                sql = "select Campo from DescargaDet where IdDescargaCab = " + idDescargaCab + " ";
            else
                sql = "select Campo from DescargaDet2 where CodPais = '" + codPais + "' and TipoOpe = '" + tipoOpe + "' ";

            if (campos != "") sql += "and Campo in " + campos;

            sql += "order by NroCampo";

            try
            {
                var dt = Conexion.SqlDataTable(sql);
                foreach (DataRow dataRow in dt.Rows)
                {
                    campo = dataRow["Campo"].ToString();
                    if (idioma == "en" && campo.ToLower() == "partida")
                        campo = "partida_en";
                    lista += "[" + campo + "], ";
                }
                lista = lista.Substring(0, lista.Length - 2);
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }

            return lista;
        }

        public static void GeneraArchivoBusqueda(int idUsuario, bool esFreeTrial, string codPais1,
            string tipoOpe, string titulos, string campos,
            string sqlFiltro, int cantReg, string nombreArchivo,
            char excel)
        {
            try
            {
                Conexion.SqlSpGeneraArchivoBusqueda(idUsuario, esFreeTrial, codPais1, tipoOpe, titulos, campos, sqlFiltro, cantReg, nombreArchivo, excel);
            }
            catch (Exception ex)
            {
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }
        }

        #region FuncionesDeMisProductos
        public static DataTable GetUnidades(string tipoOpe, string tabla, string codPais2,
            string codPais, string textMyFilter, string fechaIni2,
            string fechaFin2, string idPartida)
        {
            string sql = "select distinct Unidad from " + tabla + " ";
            sql += "where ltrim(rtrim(Unidad)) <> '' and FechaNum between " + fechaIni2 + " and " + fechaFin2 + " ";
            if (codPais2 == "4UE")
            {
                sql += "and " + (tipoOpe == "I" ? "IdPaisImp" : "IdPaisExp") + " = " + codPais + " ";
            }
            if (idPartida != "")
            {
                if (textMyFilter.Substring(0, 3) != "[G]")
                    sql += "and IdPartida = " + idPartida + " ";
                else
                    sql += "and IdPartida in (select IdFavorito from FavoritoGrupo where IdGrupo = " + idPartida + ") ";
            }

            DataTable dt;
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
        public static string ObtieneUnidad2(string Unidad, string Idioma)
        {
            string Unidad2 = "";

            string lUnidad2 = "Descripcion";
            if (Idioma == "en")
                lUnidad2 = "isnull(Descripcion_en, Descripcion)";

            string sql = "select " + lUnidad2 + " as Unidad2 from Unidad where Unidad = '" + Unidad + "' ";

            try
            {
                var dt = Conexion.SqlDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    Unidad2 = (dt.Rows[0])["Unidad2"].ToString();
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }

            return Unidad2;
        }
        public static DataRow GetTotales(string idPartida, string textPartida, string desComercial,
            string idEmpresa, string fechaIni2,string fechaFin2,
            string tipoOpe,  string codPais2, string codPais,
            string auxCodPais, string cif, string pesoNeto,
            string tabla, string importador)
        {
            string cifTot1 = cif + "Tot";
            string pesoNeto1 = pesoNeto;

            if (codPais == "BR" || codPais == "IN")
                cifTot1 = "convert(decimal(19,2), " + cif + "Tot)";
            if (codPais == "BR")
                pesoNeto1 = "convert(decimal(19,2), " + pesoNeto + ")";

            string sql = "select count(*) as CantReg, sum(Cantidad) as Cantidad, sum(" + cifTot1 + ") as " + cif +
                  "Tot, avg(" + cif + "Unit) as " + cif + "Unit ";
            if (pesoNeto != "") sql += ", sum(" + pesoNeto1 + ") as " + pesoNeto + " ";
            sql += "from " + tabla + " ";
            sql += "where FechaNum between " + fechaIni2 + " and " + fechaFin2 + " ";

            if (codPais2 == "4UE")
                sql += "and " + (tipoOpe == "I" ? "IdPaisImp" : "IdPaisExp") + " = " + auxCodPais + " ";
            if (idPartida != "")
            {
                if (textPartida.Substring(0, 3) != "[G]")
                    sql += "and IdPartida = " + idPartida + " ";
                else
                    sql += "and IdPartida in (select IdFavorito from FavoritoGrupo where IdGrupo = " + idPartida + ") ";

                if (desComercial != "")
                    sql += "and contains(DesComercial, '\"" + desComercial + "\"') ";
            }
            else
                sql += "and Id" + importador + " = " + idEmpresa + " ";

            DataRow dr;
            try
            {
                var dt = Conexion.SqlDataTable(sql);
                dr = dt.Rows[0];
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                dr = null;
            }

            return dr;
        }

        public static string RangoFechas(string fechaIni, string fechaFin, string idioma)
        {
            string rango;
            var culture = idioma == "es" ? new CultureInfo("es-PE") : new System.Globalization.CultureInfo("en-US");
            var fechaIniT = fechaIni.Substring(0, 4) + "-" + fechaIni.Substring(4, 2) + "-01";
            fechaIniT = Convert.ToDateTime(fechaIniT).ToString("MMM-yyyy", culture).ToUpper();

            var fechaFinT = fechaFin.Substring(0, 4) + "-" + fechaFin.Substring(4, 2) + "-01";
            fechaFinT = Convert.ToDateTime(fechaFinT).ToString("MMM-yyyy", culture).ToUpper();

            if (idioma == "es")
                rango = "DE " + fechaIniT + " A " + fechaFinT;
            else
                rango = "FROM " + fechaIniT + " TO " + fechaFinT;

            return rango;
        }

        public static DataTable Mensuales(string idPartida, string textPartida, string desComercial,
            string idEmpresa, string fechaIni2,string fechaFin2,
            string tipoOpe, string codPais2,string auxCodPais, 
            string cif, string pesoNeto, string tabla,
            string importador, string idioma)
        {
            string anioMes = "AñoMes0ES";

            if (idioma == "en")
                anioMes = "AñoMes0";

            string sql = "select A.IdAñoMes as Id, A." + anioMes + " as IdAñoMes, substring(A." + anioMes +
                  ", 1, 3) as IdMes, 'Serie' as Serie, ";
            sql += "isnull(Cantidad, 0) as Cantidad, isnull(" + cif + "Tot, 0) as " + cif + "Tot, isnull(" + cif +
                   "Unit, 0) as " + cif + "Unit ";
            if (pesoNeto != "")
                sql += ", isnull(" + pesoNeto + ", 0) as " + pesoNeto + ", isnull(" + cif + "Unit2, 0) as " + cif +
                       "Unit2 ";
            sql += "from (select * from AñoMes where IdAñoMes between " + fechaIni2.Substring(0, 6) + " and " +
                   fechaFin2.Substring(0, 6) + ") A left join ";

            sql +=
                "(select substring(convert(char(10), FechaNum), 1, 6) as IdAñoMes, substring(convert(char(10), FechaNum), 5, 2) as IdMes, 'Serie' as Serie, sum(Cantidad) as Cantidad, ";
            sql += "sum(" + cif + "Tot) as " + cif +
                   "Tot, case when isnull(sum(Cantidad), 0) > 0 then convert(float, sum(" + cif +
                   "Tot), 0) / convert(float, sum(Cantidad)) else null end as " + cif +
                   "Unit "; // avg(" + CIF + "Unit) as " + CIF + "Unit ";
            if (pesoNeto != "")
                sql += ", sum(" + pesoNeto + ") as " + pesoNeto + ", case when isnull(sum(" + pesoNeto +
                       "), 0) > 0 then convert(float, sum(" + cif + "Tot)) / convert(float, sum(" + pesoNeto +
                       ")) else null end as " + cif + "Unit2 ";
            sql += "from " + tabla + " ";
            sql += "where FechaNum between " + fechaIni2 + " and " + fechaFin2 + " ";

            if (codPais2 == "4UE")
                sql += "and " + (tipoOpe == "I" ? "IdPaisImp" : "IdPaisExp") + " = " + auxCodPais + " ";
            if (idPartida != "")
            {
                if (textPartida.Substring(0, 3) != "[G]")
                    sql += "and IdPartida = " + idPartida + " ";
                else
                    sql += "and IdPartida in (select IdFavorito from FavoritoGrupo where IdGrupo = " + idPartida + ") ";

                if (desComercial != "")
                    sql += "and contains(DesComercial, '\"" + desComercial + "\"') ";
            }
            else
                sql += "and Id" + importador + " = " + idEmpresa + " ";
            sql +=
                "group by substring(convert(char(10), FechaNum), 1, 6), substring(convert(char(10), FechaNum), 5, 2) ) T ";
            sql += "on A.IdAñoMes = T.IdAñoMes order by 1";

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

        public static DataTable Anuales(string idPartida, string textPartida, string desComercial, 
            string idEmpresa, string fechaIni2,string fechaFin2,
            string tipoOpe, string codPais2, string codPais,
            string auxCodPais, string cif, string cifTot,
            string pesoNeto, string tabla, string importador)
        {
            string cifTot1 = cif + "Tot";
            if (codPais == "BR" || codPais == "IN")
                cifTot1 = "convert(decimal(19,2), " + cifTot + ")";

            string sql =
                "select substring(convert(char(10), FechaNum), 1, 4) as IdAño, 'Anual' as Anual, 'Serie' as Serie, sum(Cantidad) as Cantidad, ";
            sql += "sum(" + cifTot1 + ") as " + cifTot1 + ", case when sum(Cantidad) > 0 then convert(float, sum(" +
                   cifTot1 + ")) / convert(float, sum(Cantidad)) else null end as " + cif +
                   "Unit "; // avg(" + CIF + "Unit) as " + CIF + "Unit
            if (pesoNeto != "")
                sql += ", sum(" + pesoNeto + ") as " + pesoNeto + ", case when sum(" + pesoNeto +
                       ") > 0 then convert(float, sum(" + cifTot1 + ")) / convert(float, sum(" + pesoNeto +
                       ")) else null end as " + cif + "Unit2 ";
            sql += "from " + tabla + " ";
            sql += "where FechaNum between " + fechaIni2 + " and " + fechaFin2 + " ";
            if (codPais2 == "4UE")
                sql += "and " + (tipoOpe == "I" ? "IdPaisImp" : "IdPaisExp") + " = " + auxCodPais + " ";
            if (idPartida != "")
            {
                if (textPartida.Substring(0, 3) != "[G]")
                    sql += "and IdPartida = " + idPartida + " ";
                else
                    sql += "and IdPartida in (select IdFavorito from FavoritoGrupo where IdGrupo = " + idPartida + ") ";

                if (desComercial != "")
                    sql += "and contains(DesComercial, '\"" + desComercial + "\"') ";
            }
            else
                sql += "and Id" + importador + " = " + idEmpresa + " ";
            sql += "group by substring(convert(char(10), FechaNum), 1, 4) order by 1";

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

        public static int GetCantidadRegistros(string tableAndWhereSql,  string filtro)
        {
            string sql = "SELECT count(*) as Cant FROM  (";
            sql  += "select Id" + filtro ;
            sql  += " "+tableAndWhereSql;
            sql  += " ) T";

            return CuentaRegistros(sql);
        }

        public static DataTable GetRegistrosByFiltro(string tableAndWhereSql, string filtro, string cifTot,
            string valueCifTot, int page, int maximumRows,
            string auxCifTot, bool paginar = true, bool esFree = false)
        {
            string sql = "select Id" + filtro + ", " + filtro + ", sum(" + cifTot + ") as " + auxCifTot + ", sum(" +
                         cifTot + ") * 100 / CONVERT(FLOAT," + valueCifTot + ") as Participacion ";
            if (esFree)
                sql = "select top 20 Id" + filtro + ", " + filtro + ", sum(" + cifTot + ") as " + auxCifTot + ", sum(" +
                      cifTot + ") * 100 / CONVERT(FLOAT," + valueCifTot + ") as Participacion ";

            if (paginar)
            {
                sql += " , ROW_NUMBER() over (order by SUM(" + cifTot + ") DESC) as Nro";
            }
            sql += " " + tableAndWhereSql;

            if (!paginar)
            {
                sql += " order by 4 desc";

                DataTable dt;
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
            else
            {
                return Lista(sql, page, maximumRows);
            }
        }

        public static DataTable GetPiesData(string idPartida, string desComercial, string filtro,
            string cifTot, string tipoOpe,string codPais2,
            string codPais, string auxCodPais, string textPartida,
            string fechaIni, string fechaFin, string tabla, 
            string valueCifTot)
        {
            string cifTot1 = cifTot;
            if (codPais == "BR" || codPais == "IN")
                cifTot1 = "convert(decimal(19,2), " + cifTot + ")";

            string sql = "select 1 as Orden, Id" + filtro + ", " + filtro + ", 'Serie' as Serie, " + cifTot + ", " + cifTot1 +
                  " * 100 / CONVERT(FLOAT," + valueCifTot + ") as Participacion ";
            sql += "from (select top 5 Id" + filtro + ", " + filtro + ", " + cifTot + " ";
            sql += "from (select Id" + filtro + ", case when ltrim(rtrim(" + filtro + ")) <> '' then ltrim(rtrim(" +
                   filtro + ")) else 'N/A' end as " + filtro + ", sum(" + cifTot1 + ") as " + cifTot + " ";
            sql += "from " + tabla + " where FechaNum between " + fechaIni + " and " + fechaFin + " ";
            if (codPais2 == "4UE")
                sql += "and " + (tipoOpe == "I" ? "IdPaisImp" : "IdPaisExp") + " = " + auxCodPais + " ";
            if (textPartida.Substring(0, 3) != "[G]")
                sql += "and IdPartida = " + idPartida + " ";
            else
                sql += "and IdPartida in (select IdFavorito from FavoritoGrupo where IdGrupo = " + idPartida + ") ";
            sql += "group by Id" + filtro + ", " + filtro + ") T ";
            sql += "order by " + cifTot + " desc) T ";
            sql += "union ";
            sql += "select 2 as Orden, 0 as Id" + filtro + ", '[" + Resources.Resources.Others_Text.ToUpper() + "]' as " + filtro +
                   ", 'Serie' as Serie, sum(" + cifTot1 + ") as " + cifTot + ", sum(" + cifTot1 + ") * 100 / CONVERT(FLOAT," +
                   valueCifTot + ") as Participacion ";
            sql += "from " + tabla + " where FechaNum between " + fechaIni + " and " + fechaFin + " ";
            if (codPais2 == "4UE")
                sql += "and " + (tipoOpe == "I" ? "IdPaisImp" : "IdPaisExp") + " = " + auxCodPais + " ";
            if (textPartida.Substring(0, 3) != "[G]")
                sql += "and IdPartida = " + idPartida + " ";
            else
                sql += "and IdPartida in (select IdFavorito from FavoritoGrupo where IdGrupo = " + idPartida + ") ";
            sql += "and Id" + filtro + " not in ";
            sql += "(select top 5 Id" + filtro + " ";
            sql += "from (select Id" + filtro + ", " + filtro + ", sum(" + cifTot1 + ") as " + cifTot + " ";
            sql += "from " + tabla + " where FechaNum between " + fechaIni + " and " + fechaFin + " ";
            if (codPais2 == "4UE")
                sql += "and " + (tipoOpe == "I" ? "IdPaisImp" : "IdPaisExp") + " = " + auxCodPais + " ";
            if (textPartida.Substring(0, 3) != "[G]")
                sql += "and IdPartida = " + idPartida + " ";
            else
                sql += "and IdPartida in (select IdFavorito from FavoritoGrupo where IdGrupo = " + idPartida + ") ";
            sql += "group by Id" + filtro + ", " + filtro + ") T ";
            sql += "order by " + cifTot + " desc) ";
            sql += "order by 1, 5 desc";

            DataTable dt;
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
        
        public static int GetCantidadRegistrosByIdRegistro(string tableAndWhereSql, string tipoOpe)
        {
            string sql = "SELECT count(*) as Cant FROM  ( SELECT ";
            if (tipoOpe == "I")
            {
                sql += " IdImportacion";
            }
            else
            {
                sql += " IdExportacion";
            }

            sql += " " + tableAndWhereSql;
            sql += " ) T";
            return CuentaRegistros(sql);
        }

        public static DataTable GetRegistrosByIdRegistro(string tableAndWhereSql, string tipoOpe, string codPais2,
            int page, int maximumRows )
        {
            string sql = "";
            if (tipoOpe == "I" && codPais2 == "4UE")
                sql =
                    "select 0 as IdImportacion, FechaNum_date, Nandina, PesoBruto, Cantidad, Unidad, FOBUnit, PaisOrigen ";
            else if (tipoOpe == "E" && codPais2 == "4UE")
                sql =
                    "select 0 as IdExportacion, FechaNum_date, Nandina, PesoBruto, Cantidad, Unidad, FOBUnit, PaisDestino ";
            else
                sql = "select * ";

            sql += " , ROW_NUMBER() over (order by   Fechanum_date ASC ) as Nro";
            sql += " " + tableAndWhereSql;

            return Lista(sql, page, maximumRows);
        }

        #endregion

        public static DataTable GetOpcionesDeDescarga(string idUsuario, string tipoOpe, string codPais2,
            string codPais, string idioma, string auxCodPais)
        {
            string lTodos = "TODOS LOS CAMPOS";
            string lDefecto = "Por defecto";
            if (idioma == "en")
            {
                lTodos = "ALL FIELDS";
                lDefecto = "Default";
            }

            string codPaisT = codPais;
            if (codPais == "PEP")
                codPaisT = "PE";
            else if (codPais2 == "4UE")
                codPaisT = "UE" + auxCodPais;

            string sql = "select 0 as IdDescargaCab, '[" + lTodos + "]' as Descarga,'N' AS FlagDefault ";
            sql += "union ";
            sql += "select IdDescargaCab, case when FlagDefault = 'S' then Descarga + ' [" + lDefecto +
                   "]' else Descarga end as Descarga,isnull(FlagDefault,'N') as FlagDefault ";
            sql += "from DescargaCab where IdUsuario = " + idUsuario + " and CodPais = '" + codPaisT +
                   "' and TipoOpe = '" + tipoOpe + "' order by 1";

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

        public static string BuscaIdAnioMes(string anioMes, string idioma)
        {
            string columnWhere = "AñoMes0ES";
            if (idioma == "en")
            {
                columnWhere = "AñoMes0";
            }

            string idAnioMes = "";

            string sql = "select IdAñoMes from AñoMes where "+ columnWhere + " = '" + anioMes + "' ";
            try
            {
                var dt = Conexion.SqlDataTable(sql);
                if (dt != null && dt.Rows.Count > 0)
                {
                    idAnioMes = (dt.Rows[0])["IdAñoMes"].ToString();
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }

            return idAnioMes;
        }

        public static string BuscaCodMes(string mes, string idioma)
        {
            string codMes = "";
            string columnWhere = "MesES";
            if (idioma == "en")
            {
                columnWhere = "Mes";
            }

            string sql = "select CodMes from Mes where substring("+ columnWhere + ", 1, 3) = '" + mes + "' ";

            try
            {
                var dt = Conexion.SqlDataTable(sql);
                if (dt != null && dt.Rows.Count > 0)
                {
                    codMes = (dt.Rows[0])["CodMes"].ToString();
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }

            return codMes;
        }

        #region MyCompanies
        public static DataRow GetTotalesMyCompanies(string idPartida, string desComercial, string idEmpresa,
            string textEmpresa, string fechaIni, string fechaFin,  string cif, 
            string pesoNeto, string tabla, string importador)
        {
            string sql = "select count(*) as CantReg, sum(Cantidad) as Cantidad, sum(" + cif + "Tot) as " + cif + "Tot, avg(" +
                         cif + "Unit) as " + cif + "Unit ";

            if (pesoNeto != "")
            {
                sql += ", sum(" + pesoNeto + ") as " + pesoNeto + " ";
            }

            sql += "from " + tabla + " ";
            sql += "where FechaNum between " + fechaIni + " and " + fechaFin + " ";

            if (idPartida != "")
            {
                sql += "and IdPartida = " + idPartida + " ";
                if (desComercial != "")
                    sql += "and contains(DesComercial, '\"" + desComercial + "\"') ";
            }
            else
            {
                if (textEmpresa.Substring(0, 3) != "[G]")
                    sql += "and Id" + importador + " = " + idEmpresa + " ";
                else
                    sql += "and Id" + importador + " in (select IdFavorito from FavoritoGrupo where IdGrupo = " +
                           idEmpresa + ") ";
            }

            DataRow dr;
            try
            {
                var dt = Conexion.SqlDataTable(sql);
                dr = dt.Rows[0];
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                dr = null;
            }

            return dr;
        }

        public static DataTable GetMensualesMyCompanies(string idPartida, string desComercial, string idEmpresa, 
            string textEmpresa, string fechaIni, string fechaFin, 
            string cif,string pesoNeto, string tabla,
            string importador, string idioma)
        {
            string anioMes = "AñoMes0ES";

            if (idioma == "en")
                anioMes = "AñoMes0";

            string sql = "select A.IdAñoMes as Id, A." + anioMes + " as IdAñoMes, substring(A." + anioMes +
                         ", 1, 3) as IdMes, 'Serie' as Serie, ";
            sql += "isnull(Cantidad, 0) as Cantidad, isnull(" + cif + "Tot, 0) as " + cif + "Tot, isnull(" + cif +
                   "Unit, 0) as " + cif + "Unit ";
            if (pesoNeto != "")
                sql += ", isnull(" + pesoNeto + ", 0) as " + pesoNeto + " ";

            sql += "from (select * from AñoMes where IdAñoMes between " + fechaIni.Substring(0, 6) + " and " +
                   fechaFin.Substring(0, 6) + ") A left join ";

            sql +=
                "(select substring(convert(char(10), FechaNum), 1, 6) as IdAñoMes, substring(convert(char(10), FechaNum), 5, 2) as IdMes, 'Serie' as Serie, sum(Cantidad) as Cantidad, ";

            sql += "sum(" + cif + "Tot) as " + cif + "Tot, avg(" + cif + "Unit) as " + cif + "Unit ";

            if (pesoNeto != "")
                sql += ", sum(" + pesoNeto + ") as " + pesoNeto + " ";

            sql += "from " + tabla + " ";
            sql += "where FechaNum between " + fechaIni + " and " + fechaFin + " ";

            if (idPartida != "")
            {
                sql += "and IdPartida = " + idPartida + " ";
                if (desComercial != "")
                    sql += "and contains(DesComercial, '\"" + desComercial + "\"') ";
            }
            else
            {
                if (textEmpresa.Substring(0, 3) != "[G]")
                    sql += "and Id" + importador + " = " + idEmpresa + " ";
                else
                    sql += "and Id" + importador + " in (select IdFavorito from FavoritoGrupo where IdGrupo = " +
                           idEmpresa + ") ";
            }
            sql +=
                "group by substring(convert(char(10), FechaNum), 1, 6), substring(convert(char(10), FechaNum), 5, 2) ) T ";
            sql += "on A.IdAñoMes = T.IdAñoMes order by 1";

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

        public static DataTable GetAnualesMyCompanies(string idPartida, string desComercial, string idEmpresa, 
            string textEmpresa,  string fechaIni, string fechaFin,
            string cif, string pesoNeto, string tabla,
            string importador)
        {
            string sql =
                "select substring(convert(char(10), FechaNum), 1, 4) as IdAño, 'Anual' as Anual, 'Serie' as Serie, sum(Cantidad) as Cantidad, ";
            sql += "sum(" + cif + "Tot) as " + cif + "Tot, sum(Cantidad) as Cantidad, avg(" + cif + "Unit) as " + cif +
                   "Unit ";
            if (pesoNeto != "") sql += ", sum(" + pesoNeto + ") as " + pesoNeto + " ";
            sql += "from " + tabla + " ";
            sql += "where FechaNum between " + fechaIni + " and " + fechaFin + " ";
            if (idPartida != "")
            {
                sql += "and IdPartida = " + idPartida + " ";
                if (desComercial != "")
                    sql += "and contains(DesComercial, '\"" + desComercial + "\"') ";
            }
            else
            {
                if (textEmpresa.Substring(0, 3) != "[G]")
                    sql += "and Id" + importador + " = " + idEmpresa + " ";
                else
                    sql += "and Id" + importador + " in (select IdFavorito from FavoritoGrupo where IdGrupo = " +
                           idEmpresa + ") ";
            }
            sql += "group by substring(convert(char(10), FechaNum), 1, 4) order by 1";

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

        public static DataTable GetPartidasMyCompanies(string idEmpresa, string textEmpresa,  string fechaIni,
            string fechaFin,string cifTot, string valueCifTot, 
            string tabla, string importador)
        {
            string sql = "select 1 as Orden, IdPartida, Nandina, 'Serie' as Serie, " + cifTot + ", " + cifTot + " * 100 / " +
                         valueCifTot + " as Participacion ";
            sql += "from (select top 5 IdPartida, Nandina, " + cifTot + " ";
            sql += "from (select IdPartida, Nandina, sum(" + cifTot + ") as " + cifTot + " ";
            sql += "from " + tabla + " where FechaNum between " + fechaIni + " and " + fechaFin + " ";
            if (textEmpresa.Substring(0, 3) != "[G]")
                sql += "and Id" + importador + " = " + idEmpresa + " ";
            else
                sql += "and Id" + importador + " in (select IdFavorito from FavoritoGrupo where IdGrupo = " +
                       idEmpresa + ") ";
            sql += "group by IdPartida, Nandina) T ";
            sql += "order by " + cifTot + " desc) T ";
            sql += "union ";
            sql += "select 2 as Orden, 0 as IdPartida, '[" + Resources.Resources.Others_Text.ToUpper() + "]' as Nandina, 'Serie' as Serie, sum(" + cifTot +
                   ") as " + cifTot + ", sum(" + cifTot + ") * 100 / " + valueCifTot +
                   " as Participacion ";
            sql += "from " + tabla + " where FechaNum between " + fechaIni + " and " + fechaFin + " ";
            if (textEmpresa.Substring(0, 3) != "[G]")
                sql += "and Id" + importador + " = " + idEmpresa + " ";
            else
                sql += "and Id" + importador + " in (select IdFavorito from FavoritoGrupo where IdGrupo = " +
                       idEmpresa + ") ";
            sql += "and IdPartida not in ";
            sql += "(select top 5 IdPartida ";
            sql += "from (select IdPartida, Nandina, sum(" + cifTot + ") as " + cifTot + " ";
            sql += "from " + tabla + " where FechaNum between " + fechaIni + " and " + fechaFin + " ";
            if (textEmpresa.Substring(0, 3) != "[G]")
                sql += "and Id" + importador + " = " + idEmpresa + " ";
            else
                sql += "and Id" + importador + " in (select IdFavorito from FavoritoGrupo where IdGrupo = " +
                       idEmpresa + ") ";
            sql += "group by IdPartida, Nandina) T ";
            sql += "order by " + cifTot + " desc) ";
            sql += "order by 1, 5 desc";

            DataTable dt;
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

        public static DataTable GetTablePartidasDataMyCompanies( string tableAndWhereSql, string cifTot, string valueCifTot ,
            string idioma, int page, int maximumRows, bool paginar = true ,bool esFree = false)
        {
            var partida = "Partida";
            if (idioma == "en")
                partida = "Partida_en";

            string sql = "select T.IdPartida, T.Nandina, " + partida + " as Partida, sum(" + cifTot + ") as " + cifTot +
                  ", sum(" + cifTot + ") * 100 / " + valueCifTot  + " as Participacion ";

            if (esFree)
            {
                sql = "select top 20 T.IdPartida, T.Nandina, " + partida + " as Partida, sum(" + cifTot + ") as " + cifTot +
                             ", sum(" + cifTot + ") * 100 / " + valueCifTot + " as Participacion ";
            }


            if (paginar)
            {
                sql += " , ROW_NUMBER() over (order by SUM(" + cifTot + ") DESC) as Nro";
            }

            sql += " " + tableAndWhereSql;

            if (!paginar)
            {
                sql += " order by 4 desc";

                DataTable dt;
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
            else
            {
                return Lista(sql, page, maximumRows);
            }
        }
        public static int GetCantidadRegistrosPartidas(string tableAndWhereSql)
        {
            string sql = "SELECT count(*) as Cant FROM  (";
            sql += "select T.IdPartida" ;
            sql += " " + tableAndWhereSql;
            sql += " ) T";

            return CuentaRegistros(sql);
        }

        public static DataTable GetPiesDataMyCompanie(string idEmpresa, string textEmpresa, string filtro,
            string cifTot, string valueCifTot, string fechaIni, string fechaFin, string tabla, string importador )
        {
           string sql = "select 1 as Orden, Id" + filtro + ", " + filtro + ", 'Serie' as Serie, " + cifTot + ", " + cifTot +
                  " * 100 / " + valueCifTot + " as Participacion ";
            sql += "from (select top 5 Id" + filtro + ", " + filtro + ", " + cifTot + " ";
            sql += "from (select Id" + filtro + ", case when ltrim(rtrim(" + filtro + ")) <> '' then ltrim(rtrim(" +
                   filtro + ")) else 'N/A' end as " + filtro + ", sum(" + cifTot + ") as " + cifTot + " ";
            sql += "from " + tabla + " where FechaNum between " + fechaIni + " and " + fechaFin + " ";
            if (textEmpresa.Substring(0, 3) != "[G]")
                sql += "and Id" + importador + " = " + idEmpresa + " ";
            else
                sql += "and Id" + importador + " in (select IdFavorito from FavoritoGrupo where IdGrupo = " +
                       idEmpresa + ") ";
            sql += "group by Id" + filtro + ", " + filtro + ") T ";
            sql += "order by " + cifTot + " desc) T ";
            sql += "union ";
            sql += "select 2 as Orden, 0 as Id" + filtro + ", '[" + Resources.Resources.Others_Text.ToUpper() + "]' as " + filtro +
                   ", 'Serie' as Serie, sum(" + cifTot + ") as " + cifTot + ", sum(" + cifTot + ") * 100 / " +
                   valueCifTot + " as Participacion ";
            sql += "from " + tabla + " where FechaNum between " + fechaIni + " and " + fechaFin + " ";
            if (textEmpresa.Substring(0, 3) != "[G]")
                sql += "and Id" + importador + " = " + idEmpresa + " ";
            else
                sql += "and Id" + importador + " in (select IdFavorito from FavoritoGrupo where IdGrupo = " +
                       idEmpresa + ") ";
            sql += "and Id" + filtro + " not in ";
            sql += "(select top 5 Id" + filtro + " ";
            sql += "from (select Id" + filtro + ", " + filtro + ", sum(" + cifTot + ") as " + cifTot + " ";
            sql += "from " + tabla + " where FechaNum between " + fechaIni + " and " + fechaFin + " ";
            if (textEmpresa.Substring(0, 3) != "[G]")
                sql += "and Id" + importador + " = " + idEmpresa + " ";
            else
                sql += "and Id" + importador + " in (select IdFavorito from FavoritoGrupo where IdGrupo = " +
                       idEmpresa + ") ";
            sql += "group by Id" + filtro + ", " + filtro + ") T ";
            sql += "order by " + cifTot + " desc) ";
            sql += "order by 1, 5 desc";

            DataTable dt;
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



        #endregion

        /* JANAQ 150620
        * Funcion que permite crear o modificar usuarios mix panel
        */
        public static void CrearUsuarioMixPanel(string idUsuario, string host)
        {
            if (Convert.ToBoolean(Properties.Settings.Default.HabilitarTrackingMixPanel))
            {

                UsuarioMixPanel usuario = Funciones.ObtieneDatosUsuarioPorUsuario(idUsuario);

                var productionHost = Properties.Settings.Default.UrlProdHostAdmin;
                var token = "";

                //Si el hostname es diferente a produccion se usara el token de desarrollo 
                if (productionHost.ToLower().IndexOf(host) < 0)
                {
                    token = Properties.Settings.Default.TokenDevMixPanel;
                }
                else
                {
                    token = Properties.Settings.Default.TokenProdMixPanel;
                }

                var sisOperativo = "Windows";
                var browser = "Chrome";
                var browser_version = "1.21.11.0";
                string responseData;

                //Creacion del Objeto JSON del usuario
                var data = "{\"$token\":\"" + token + "\",\"$distinct_id\":\"" + usuario.IdUsuario +
                           "\",\"$ip\":\"" + usuario.DireccionIP + "\",\"$set\":{" +
                            "\"$name\":\"" + usuario.Cliente + "\",\"$email\":\"" + usuario.Email1 +
                             "\",\"$os\":\"" + sisOperativo + "\",\"$region\":\"" + usuario.Ciudad +
                             "\",\"$city\":\"" + usuario.Ciudad + "\",\"$country_code\":\"" + usuario.codPais +
                             "\",\"$browser\":\"" + browser + "\",\"$browser_version\":\"" + browser_version +
                              "\",\"Cliente\":\"" + usuario.Nombres + "\",\"Email2\":\"" + usuario.Email2 +
                             "\",\"CodUsuario\":\"" + usuario.CodUsuario + "\",\"Plan\":\"" + usuario.Plan +
                              "\",\"Tipo\":\"" + usuario.Tipo + "\",\"Sector\":\"" + usuario.Sector +
                              "\",\"IdTipo\":\"" + usuario.IdTipo +
                              "\"}}";

                string encodeDataBase64 = Base64.encode(data);
                try
                {
                    var url = string.Format("{0}?data={1}", Properties.Settings.Default.UrlMixPanel, encodeDataBase64);

                    //Consumo del Servicio Rest de Mix Panel
                    System.Net.WebRequest hwrequest = System.Net.WebRequest.Create(url);
                    hwrequest.ContentType = "application/json";
                    hwrequest.Method = "GET";

                    System.Net.WebResponse hwresponse = hwrequest.GetResponse();
                    System.IO.StreamReader responseStream = new System.IO.StreamReader(hwresponse.GetResponseStream());

                    // ResponseData si es igual 1=Ok , 0=Hubo un error
                    responseData = responseStream.ReadToEnd();

                    hwresponse.Close();
                    if (responseData == "0") throw new Exception("Ocurrio un error en CrearUsuarioMixPanel");
                }
                catch (Exception ex)
                {
                    ExceptionUtility.LogException(ex, "CrearUsuarioMixPanel");
                }
            }

        }
    }
}