
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Veritrade2018.Util;

namespace Veritrade2018.Helpers
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

        public bool IsValid(string pais)
        {
            var sql = "SELECT * FROM AdminPaisN WHERE CodPais = '" + pais + "' AND CodPais != '-'";
            var dt = Conexion.SqlDataTable(sql);
            return dt.Rows.Count > 0;
        }

        public string BuscarCodPais2(string pais)
        {
            string codPais2= "";
            var sql = string.Format(@"select idParent from dbo.VariableGeneral Where Estado=1 
                                            and idVariable = '{0}' Order By iOrden",pais);
            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
               codPais2 = row["idParent"].ToString();
            }

            return codPais2;
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
        /// <summary>
        /// Obtine lista de paises usados en el primero combo del admin Mis Busquedas
        /// </summary>
        /// <param name="idioma">Código del idioma</param>
        /// <returns></returns>
        //public IEnumerable<SelectListItem> GetPaises2(string idioma)
        //{
        //    var lista = new List<SelectListItem>();
        //    if (idioma == "es")
        //    {
        //        lista.Add(new SelectListItem { Text = @"Latinoamérica", Value = "1LAT" });
        //        lista.Add(new SelectListItem { Text = @"USA", Value = "2US" });
        //        lista.Add(new SelectListItem { Text = @"China", Value = "3CN" });
        //        lista.Add(new SelectListItem { Text = @"Unión Europea", Value = "4UE" });
        //        lista.Add(new SelectListItem { Text = @"India", Value = "5IN" });
        //    }
        //    else
        //    {
        //        lista.Add(new SelectListItem { Text = @"Latin America", Value = "1LAT" });
        //        lista.Add(new SelectListItem { Text = @"USA", Value = "2US" });
        //        lista.Add(new SelectListItem { Text = @"China", Value = "3CN" });
        //        lista.Add(new SelectListItem { Text = @"European Union", Value = "4UE" });
        //        lista.Add(new SelectListItem { Text = @"India", Value = "5IN" });
        //    }
        //    return lista;
        //}

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


        /// <summary>
        /// Obtiene lista de paises listados en combo de paises del Admin
        /// </summary>
        /// <param name="idioma"></param>
        /// <param name="valuePaises2"></param>
        /// <param name="flagPEB"></param>
        /// <returns></returns>
        //public IEnumerable<SelectListItem> GetPaisesAdmin(string idioma, string valuePaises2 = "1LAT", bool flagPEB = true)
        //{
        //    var lista = new List<SelectListItem>();
        //    if (valuePaises2 == "1LAT")
        //    {
        //        if (idioma == "es")
        //        {
        //            lista.Add(new SelectListItem { Text = @"Argentina", Value = "AR" });
        //            lista.Add(new SelectListItem { Text = @"Bolivia", Value = "BO" });
        //            lista.Add(new SelectListItem { Text = @"Brasil", Value = "BR" });
        //            lista.Add(new SelectListItem { Text = @"Chile", Value = "CL" });
        //            lista.Add(new SelectListItem { Text = @"Colombia", Value = "CO" });
        //            lista.Add(new SelectListItem { Text = @"Costa Rica", Value = "CR" });
        //            lista.Add(new SelectListItem { Text = @"Ecuador", Value = "EC" });
        //            lista.Add(new SelectListItem { Text = @"México", Value = "MX" });
        //            lista.Add(new SelectListItem { Text = @"México Detalle Marítimo", Value = "MXD" });
        //            lista.Add(new SelectListItem { Text = @"Panamá", Value = "PA" });
        //            lista.Add(new SelectListItem { Text = @"Paraguay", Value = "PY" });
        //            lista.Add(new SelectListItem { Text = @"Perú", Value = "PE" });
        //            //if (flagPEB)lista.Add(new SelectListItem { Text ="Perú Formato B", "PEB"));
        //            lista.Add(new SelectListItem { Text = @"Perú Manifiestos", Value = "PE_" });
        //            lista.Add(new SelectListItem { Text = @"Perú Provisional", Value = "PEP" });
        //            lista.Add(new SelectListItem { Text = @"Uruguay", Value = "UY" });
        //        }
        //        else
        //        {
        //            lista.Add(new SelectListItem { Text = @"Argentina", Value = "AR" });
        //            lista.Add(new SelectListItem { Text = @"Bolivia", Value = "BO" });
        //            lista.Add(new SelectListItem { Text = @"Brazil", Value = "BR" });
        //            lista.Add(new SelectListItem { Text = @"Chile", Value = "CL" });
        //            lista.Add(new SelectListItem { Text = @"Colombia", Value = "CO" });
        //            lista.Add(new SelectListItem { Text = @"Costa Rica", Value = "CR" });
        //            lista.Add(new SelectListItem { Text = @"Ecuador", Value = "EC" });
        //            lista.Add(new SelectListItem { Text = @"Mexico", Value = "MX" });
        //            lista.Add(new SelectListItem { Text = @"Mexico Maritime Detail", Value = "MXD" });
        //            lista.Add(new SelectListItem { Text = @"Panama", Value = "PA" });
        //            lista.Add(new SelectListItem { Text = @"Paraguay", Value = "PY" });
        //            lista.Add(new SelectListItem { Text = @"Peru", Value = "PE" });
        //            //if (flagPEB) lista.Add(new SelectListItem { Text ="Peru Invoice Details",Value = "PEB"});
        //            lista.Add(new SelectListItem { Text = @"Peru B/L", Value = "PE_" });
        //            lista.Add(new SelectListItem { Text = @"Peru Weekly", Value = "PEP" });
        //            lista.Add(new SelectListItem { Text = @"Uruguay", Value = "UY" });

        //        }
        //    }
        //    else
        //        if (valuePaises2 == "2US")
        //    {
        //        if (idioma == "es")
        //        {
        //            lista.Add(new SelectListItem { Text = @"USA", Value = "US" });
        //            lista.Add(new SelectListItem { Text = @"USA Manifiestos", Value = "US_" });
        //        }
        //        else
        //        {
        //            lista.Add(new SelectListItem { Text = @"USA", Value = "US" });
        //            lista.Add(new SelectListItem { Text = @"USA B/L", Value = "US_" });
        //        }
        //    }
        //    else if (valuePaises2 == "3CN")
        //    {
        //        lista.Add(new SelectListItem { Text = @"China", Value = "CN" });
        //    }
        //    else if (valuePaises2 == "4UE")
        //    {
        //        lista = getPaisesDatatableToListItem(FuncionesBusiness.CargaPaisesUE(idioma));
        //    }
        //    else if (valuePaises2 == "5IN")
        //    {
        //        lista.Add(new SelectListItem { Text = @"India", Value = "IN" });
        //    }
        //    return lista;
        //}

        public IEnumerable<SelectListItem> GetPaisesAdmin(string idioma, string valuePaises2 = "1LAT", bool flagPEB = true
                                        , string k_excludePais = null )
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
                catch(Exception ex) 
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

        public IEnumerable<SelectListItem> GetPaisesPerfil(string idioma, string codPais2 = "1LAT")
        {
            var lista = new List<SelectListItem>();
            switch (codPais2)
            {
                case "1LAT":
                    if (idioma == "es")
                    {
                        lista.Add(new SelectListItem { Text = @"Argentina", Value = "AR" });
                        lista.Add(new SelectListItem { Text = @"Bolivia", Value = "BO" });
                        lista.Add(new SelectListItem { Text = @"Brasil", Value = "BR" });
                        lista.Add(new SelectListItem { Text = @"Chile", Value = "CL" });
                        lista.Add(new SelectListItem { Text = @"Colombia", Value = "CO" });
                        lista.Add(new SelectListItem { Text = @"Costa Rica", Value = "CR" });
                        lista.Add(new SelectListItem { Text = @"Ecuador", Value = "EC" });
                        lista.Add(new SelectListItem { Text = @"México", Value = "MX" });
                        lista.Add(new SelectListItem { Text = @"México Detalle Marítimo", Value = "MXD" });
                        lista.Add(new SelectListItem { Text = @"Panamá", Value = "PA" });
                        lista.Add(new SelectListItem { Text = @"Paraguay", Value = "PY" });
                        lista.Add(new SelectListItem { Text = @"Perú", Value = "PE" });
                        lista.Add(new SelectListItem { Text = @"Perú Manifiestos", Value = "PE_" });
                        lista.Add(new SelectListItem { Text = @"Uruguay", Value = "UY" });
                    }
                    else
                    {
                        lista.Add(new SelectListItem { Text = @"Argentina", Value = "AR" });
                        lista.Add(new SelectListItem { Text = @"Bolivia", Value = "BO" });
                        lista.Add(new SelectListItem { Text = @"Brazil", Value = "BR" });
                        lista.Add(new SelectListItem { Text = @"Chile", Value = "CL" });
                        lista.Add(new SelectListItem { Text = @"Colombia", Value = "CO" });
                        lista.Add(new SelectListItem { Text = @"Costa Rica", Value = "CR" });
                        lista.Add(new SelectListItem { Text = @"Ecuador", Value = "EC" });
                        lista.Add(new SelectListItem { Text = @"Mexico", Value = "MX" });
                        lista.Add(new SelectListItem { Text = @"Mexico Maritime Detail", Value = "MXD" });
                        lista.Add(new SelectListItem { Text = @"Panama", Value = "PA" });
                        lista.Add(new SelectListItem { Text = @"Paraguay", Value = "PY" });
                        lista.Add(new SelectListItem { Text = @"Peru", Value = "PE" });
                        lista.Add(new SelectListItem { Text = @"Peru B/L", Value = "PE_" });
                        lista.Add(new SelectListItem { Text = @"Uruguay", Value = "UY" });

                    }
                    break;
                case "2US":
                    if (idioma == "es")
                    {
                        lista.Add(new SelectListItem { Text = @"USA", Value = "US" });
                        lista.Add(new SelectListItem { Text = @"USA Manifiestos", Value = "US_" });
                    }
                    else
                    {
                        lista.Add(new SelectListItem { Text = @"USA", Value = "US" });
                        lista.Add(new SelectListItem { Text = @"USA B/L", Value = "US_" });
                    }
                    break;
                case "3CN":
                    lista.Add(new SelectListItem { Text = @"China", Value = "CN" });
                    break;
                case "4UE":
                    lista = getPaisesDatatableToListItem(FuncionesBusiness.CargaPaisesUE(idioma));
                    break;
                case "5IN":
                    lista.Add(new SelectListItem { Text = @"India", Value = "IN" });
                    break;

            }
            return lista;
        }
    }
}