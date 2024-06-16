using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Veritrade2017.Models;
using Veritrade2017.Util;

namespace Veritrade2017.Helpers
{
    public class ListaPaises
    {
        public IEnumerable<object> Listado(string idioma = "es", string like = "")
        {
            var lista = new List<object>();
            string sql;

            if (idioma == "es")
            {
                sql = "SELECT CodPais, LTRIM(RTRIM([Pais])) as 'Pais' FROM AdminPaisN WHERE Pais like '" + like + "%' AND CodPais != '-'  ORDER BY 'Pais'";
            }
            else
            {
                sql = "SELECT CodPais, LTRIM(RTRIM([PaisEn])) as 'Pais' FROM AdminPaisN WHERE PaisEn like '" + like + "%' AND CodPais != '-' ORDER BY 'Pais'";
            }

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                lista.Add(new { Value = row["CodPais"], Text = row["Pais"] });
            }

            return lista.ToList();
        }
        public string BuscarCodPais2(string pais)
        {
            string codPais2 = "";
            var sql = string.Format(@"select idParent from dbo.VariableGeneral Where Estado=1 
                                            and idVariable = '{0}' Order By iOrden", pais);
            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                codPais2 = row["idParent"].ToString();
            }

            return codPais2;
        }
        public bool IsValid(string pais)
        {
            var sql = "SELECT * FROM AdminPaisN WHERE CodPais = '" + pais + "' AND CodPais != '-'";
            var dt = Conexion.SqlDataTable(sql);
            return dt.Rows.Count > 0;
        }
        public IEnumerable<SelectListItem> GetPaises2(string idioma, string k_excludeReg = null)
        {
            string[] _exc = null;
            try
            {
                if (!string.IsNullOrEmpty(k_excludeReg))
                {
                    var _k = VarGeneral.Instance.ValuesDict[k_excludeReg];
                    if (_k != null)
                        _exc = _k.Valores.Split(',');
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }

            var _v = (from e in VarGeneral.Instance.Values
                      where e.IdGrupo == Enums.VarGrupo.REGION.GetDn()
                        && ((_exc != null && !_exc.Contains(e.IdVariable)) || _exc == null)
                      select e).ToList();

            var lista = new List<SelectListItem>();
            _v.ForEach(i => lista.Add(new SelectListItem { Text = idioma == "es" ? i.Descripcion : i.Descripcion_Eng, Value = i.IdVariable }));
            return lista;
        }
        public IEnumerable<SelectListItem> PaisesConsulta(bool flagPeb = true)
        {
            var lista = new List<SelectListItem>();
            lista.Add(new SelectListItem { Value = "AR", Text = @"Argentina" });
            lista.Add(new SelectListItem { Value = "BO", Text = @"Bolivia" });
            lista.Add(new SelectListItem { Value = "BR", Text = @"Brasil" });
            lista.Add(new SelectListItem { Value = "CL", Text = @"Chile" });
            lista.Add(new SelectListItem { Value = "CN", Text = @"China" });
            lista.Add(new SelectListItem { Value = "CO", Text = @"Colombia" });
            lista.Add(new SelectListItem { Value = "CR", Text = @"Costa Rica" });
            lista.Add(new SelectListItem { Value = "EC", Text = @"Ecuador" });
            lista.Add(new SelectListItem { Value = "IN", Text = @"India" });
            lista.Add(new SelectListItem { Value = "MX", Text = @"México" });
            lista.Add(new SelectListItem { Value = "PA", Text = @"Panamá" });
            lista.Add(new SelectListItem { Value = "PY", Text = @"Paraguay" });
            lista.Add(new SelectListItem { Value = "PE", Text = @"Perú" });
            if (flagPeb) lista.Add(new SelectListItem { Value = "PEB", Text = @"Perú Formato B" });
            lista.Add(new SelectListItem { Value = "UY", Text = @"Uruguay" });
            lista.Add(new SelectListItem { Value = "US", Text = @"USA" });

            return lista;
        }
        public IEnumerable<SelectListItem> GetPaisesAdmin(string idioma, string valuePaises2 = "1LAT", bool flagPEB = true
                                , string k_excludePais = null)
        {
            var lista = new List<SelectListItem>();

            if (valuePaises2 == "4UE")
            {
                lista = getPaisesDatatableToListItem(FuncionesBusiness.CargaPaisesUE(idioma));
            }
            else
            {
                string[] _exc = null;
                try
                {
                    if (!string.IsNullOrEmpty(k_excludePais))
                    {
                        var _k = VarGeneral.Instance.ValuesDict[k_excludePais];
                        if (_k != null)
                            _exc = _k.Valores.Split(',');
                    }
                }
                catch (Exception ex)
                {
                    
                    
                    
                    ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                }


                var _v = (from e in VarGeneral.Instance.Values
                          where e.IdGrupo == Enums.VarGrupo.SUB_REGION.GetDn()
                                && e.IdParent == valuePaises2
                                && ((_exc != null && !_exc.Contains(e.IdVariable)) || _exc == null)
                          select e).OrderBy(x => x.Descripcion).ToList();

                _v.ForEach(i => lista.Add(new SelectListItem { Text = idioma == "es" ? i.Descripcion : i.Descripcion_Eng, Value = i.IdVariable }));
            }
            return lista;
        }
        private List<SelectListItem> getPaisesDatatableToListItem(DataTable dt)
        {
            var lista = new List<SelectListItem>();
            foreach (DataRow dr in dt.Rows)
            {
                lista.Add(new SelectListItem { Text = dr["Pais"].ToString(), Value = dr["IdPais"].ToString() });
            }
            return lista;
        }
    }
}