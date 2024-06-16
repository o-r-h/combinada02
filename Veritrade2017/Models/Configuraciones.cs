using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Veritrade2017.Helpers;

namespace Veritrade2017.Models
{
    public class Configuraciones
    {
        public Idiomas Idiomas { get; set; }
        public string Campo { get; set; }
        public string Valor { get; set; }

        public Configuraciones()
        {
            Idiomas = new Idiomas();
        }

        public static Configuraciones GetConfig(string idioma, string campo)
        {
            var sql = "SELECT campo, valor FROM [dbo].[configuraciones] " +
                      "INNER JOIN [dbo].[idiomas] ON [configuraciones].idioma_id = [idiomas].id " +
                      "WHERE [idiomas].nombre = '" + idioma + "' AND [configuraciones].campo = '" + campo + "'";

            var dt = Conexion.SqlDataTable(sql, true);
            var model = new Configuraciones();
            foreach (DataRow row in dt.Rows)
            {
                var s = new Configuraciones
                {
                    Campo = Convert.ToString(row["campo"]),
                    Valor = Convert.ToString(row["valor"])
                };
                model = s;
            }

            return model;
        }
    }

    public class Campania
    {
        public enum TYPE_CAMPANIA
        {
            NONE,
            ORGANIC,
            WITH_CAMPANIA,
            HOME_CAMPANIA
        }

        public enum CONTROL
        {
            NONE,
            PRUEBA_GRATIS,
            BUSCAR_EMPRESAS,
            PLANES,
            PLANES_EN,
            BLOG,
            PRUEBA_GRATIS_EN,
            BUSCAR_EMPRESAS_EN,
            BUSCAR_PRODUCTOS,
            BUSCAR_PRODUCTOS_EN,
            BUSCAR_DEMO,
            BUSCAR_DEMO_EN
        }

        public static Dictionary<CONTROL, string> CodesWithCampanias => new Dictionary<CONTROL, string>()
        {
            {CONTROL.NONE, "" },
            {CONTROL.PRUEBA_GRATIS, "12100" },
            {CONTROL.PRUEBA_GRATIS_EN, "13100" },
            {CONTROL.BUSCAR_EMPRESAS, "20100" },
            {CONTROL.BUSCAR_EMPRESAS_EN, "20100I" },
            {CONTROL.BUSCAR_PRODUCTOS, "20200" },
            {CONTROL.BUSCAR_PRODUCTOS_EN, "20200I" },
            {CONTROL.PLANES, "12100" }, //51001
            {CONTROL.PLANES_EN, "13100" },
            {CONTROL.BLOG, "14100" },
            {CONTROL.BUSCAR_DEMO, "12101"},
            {CONTROL.BUSCAR_DEMO_EN, "12102"},
        };

        public static  Dictionary<CONTROL, string> CodesOrganics => new Dictionary<CONTROL, string>()
        {
            {CONTROL.NONE, "" },
            {CONTROL.PRUEBA_GRATIS, "12100" },
            {CONTROL.PRUEBA_GRATIS_EN, "13100" },
            {CONTROL.BUSCAR_EMPRESAS, "20100" },
            {CONTROL.BUSCAR_EMPRESAS_EN, "20100I" },
            {CONTROL.BUSCAR_PRODUCTOS, "20200" },
            {CONTROL.BUSCAR_PRODUCTOS_EN, "20200I" },
            {CONTROL.PLANES, "12100" },
            {CONTROL.PLANES_EN, "13100" },
            { CONTROL.BLOG, "14100" },
            {CONTROL.BUSCAR_DEMO, "12101"},
            {CONTROL.BUSCAR_DEMO_EN, "12102"},
        };

        public static Dictionary<CONTROL, string> CodesHomeCampanias (string camp)
        {
            return new Dictionary<CONTROL, string>()
            {
                {CONTROL.PRUEBA_GRATIS, camp },
                {CONTROL.PRUEBA_GRATIS_EN, camp },
                {CONTROL.BUSCAR_EMPRESAS, camp },
                {CONTROL.BUSCAR_EMPRESAS_EN, camp },
                {CONTROL.BUSCAR_PRODUCTOS, camp },
                {CONTROL.BUSCAR_PRODUCTOS_EN, camp },
                {CONTROL.PLANES, camp },
                {CONTROL.PLANES_EN, camp },
                {CONTROL.BLOG, camp },
                {CONTROL.BUSCAR_DEMO, camp },
                {CONTROL.BUSCAR_DEMO_EN, camp },
            };
        }


        public Dictionary<CONTROL, string> Codes { get; set; }  
        public Campania()
        {
            Codes = new Dictionary<CONTROL, string>()
            {
                {CONTROL.PRUEBA_GRATIS, String.Empty },
                {CONTROL.BUSCAR_EMPRESAS, String.Empty },
                {CONTROL.BUSCAR_PRODUCTOS, String.Empty },
                {CONTROL.PLANES, String.Empty },
                {CONTROL.BLOG, String.Empty },
            };
        }

        public static CONTROL GetTheControl(string ctl)
        {
            var ret = CONTROL.NONE;
            switch (ctl.ToLower())
            {
                case "pruebagratis":
                    ret = CONTROL.PRUEBA_GRATIS;
                    break;
                case "minisite":
                case "empresas":
                    ret = CONTROL.BUSCAR_EMPRESAS;
                    break;
                case "planes":
                    ret = CONTROL.PLANES;
                    break;
                case "blog":
                    ret = CONTROL.BLOG;
                    break;
            }
            return ret;
        }

        public static string GetTheControl(CONTROL ctl)
        {
            var ret = string.Empty;
            switch (ctl)
            {
                case CONTROL.PRUEBA_GRATIS:
                    ret = "pruebagratis";
                    break;
                case CONTROL.BUSCAR_EMPRESAS:
                    ret = "empresas";
                    break;
                case CONTROL.BUSCAR_PRODUCTOS:
                    ret = "productos";
                    break;
            }
            return ret;
        }

        //public static string CodFormated(string code, string culture,
        //    bool isMobile)
        //{
        //    return code.Replace("m", "").Replace("I", "")  + (isMobile ? "m" : "") + (culture != "es" ? "I" : "");
        //}


        //public static Dictionary<CONTROL, string> CodFormateds(Dictionary<CONTROL, string> codes, string culture, bool isMobile)
        //{
        //    Dictionary<CONTROL, string> ret = CodesHomeCampanias(string.Empty);

        //    foreach (var entry in codes)
        //    {
        //        ret[entry.Key] = CodFormated(entry.Value, culture, isMobile);
        //    }
        //    return ret;
        //}

        //public static Dictionary<CONTROL, string> CodFormateds2(Dictionary<CONTROL, string> codes, string culture, bool isMobile, TYPE_CAMPANIA tyc)
        //{
        //    Dictionary<CONTROL, string> ret = CodesHomeCampanias(string.Empty);

        //    foreach (var entry in codes)
        //    {
        //        ret[entry.Key] = CodFormated2(entry.Value, culture, isMobile, tyc, entry.Key);
        //    }
        //    return ret;
        //}

        //public static string CodFormated2(string code, string culture,
        //    bool isMobile, TYPE_CAMPANIA tyc, CONTROL ctl)
        //{
        //    var ret = code?.Replace("m", "").Replace("I", "") + (isMobile ? "m" : "");

        //    var _eng = culture != "es";
        //    if (tyc == TYPE_CAMPANIA.WITH_CAMPANIA || tyc == TYPE_CAMPANIA.HOME_CAMPANIA)
        //    {
        //        //if (culture != "es")
        //        //{
        //            if (ctl == CONTROL.BUSCAR_EMPRESAS && Microsoft.VisualBasic.Strings.Left(CodesWithCampanias[CONTROL.BUSCAR_EMPRESAS], 5).Equals(code))
        //            {
        //                ret = _eng ? "20100I" : "20100" + (isMobile ? "m" : ""); ;

        //            }
        //            else if (ctl == CONTROL.PRUEBA_GRATIS)
        //            {
        //                ret = _eng ?  "13100" : "12100"  + (isMobile ? "m" : "");
        //            }
        //        //}
        //    }


        //    return ret;
        //}


        public static Dictionary<CONTROL, string> CodFormateds3(Dictionary<CONTROL, string> codes, string culture, bool isMobile, TYPE_CAMPANIA tyc)
        {
            Dictionary<CONTROL, string> ret = CodesHomeCampanias(string.Empty);

            foreach (var entry in codes)
            {
                ret[entry.Key] = SetCampMobilOrEnglish(entry.Value, (culture!= "es"), tyc, GetTheControl(entry.Key), isMobile );
            }
            return ret;
        }

        public static string SetCampMobilOrEnglish(string code, bool _eng, Campania.TYPE_CAMPANIA sty, string controller, bool isMobil, bool bFlag = false)
        {   
            code = code.Replace("m", "").Replace("I", "") + (isMobil ? "m" : "");
            if ( CodesOrganics.Values.Contains(code)  || CodesWithCampanias.Values.Contains(code)  )

            {
                switch (controller?.ToLower())
                {
                    case "empresas":
                    case "minisite":
                        code = _eng ? "20100" + (isMobil ? "m" : "") + "I" : "20100" + (isMobil ? "m" : "") ;
                        break;
                    case "productos":
                    case "productoperfil":
                        code = _eng ? "20200" + (isMobil ? "m" : "") + "I" : "20200" + (isMobil ? "m" : "");
                        break;
                    case "pruebagratis":
                    case "planes":
                        code = _eng ? "13100" : "12100" + (isMobil ? "m" : "") ;
                        break;
                }

                if (bFlag)
                {
                    if (code == "20100" && _eng)
                        code = "20100" + (isMobil ? "m" : "") + "I";
                    else if (code == "20100" && !_eng)
                        code = "20100" + (isMobil ? "m" : "");
                    else if (code == "20200" && _eng)
                        code = "20200" + (isMobil ? "m" : "") + "I";
                    else if (code == "20100" && !_eng)
                        code = "20200" + (isMobil ? "m" : "");
                    else if (code == "12100" && _eng)
                        code = "13100" + (isMobil ? "m" : "");
                    else if (code == "13100" && !_eng)
                        code = "12100" + (isMobil ? "m" : "");
                }
            }
            return code;
        }



    }
}