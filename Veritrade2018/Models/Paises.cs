using System;
using System.Collections.Generic;
using System.Data;
using Veritrade2018.Helpers;

namespace Veritrade2018.Models
{
    public class Paises
    {
        public string CodPais { get; set; }
        public string PaisRegimen { get; set; }
        public string Bandera { get; set; }
        public string FechaFin { get; set; }

        public List<Paises> GeList(string culture, bool limit = false)
        {
            var paisRegimen = culture.Equals("es") ? "PaisRegimen" : "PaisRegimenEN";
            var sql = "SELECT DISTINCT " + (limit ? "TOP 9" : "") + " CodPais3 'CodPais', " + paisRegimen +
                      " as PaisRegimen, Bandera, FechaFin " +
                      "FROM PaisRegimen M, BaseDatos B " +
                      "INNER JOIN [AdminPaisN] ON [AdminPaisN].CodPais = B.CodPais " +
                      "WHERE M.CodPais = B.CodPais " +
                      "ORDER BY FechaFin DESC, " + paisRegimen;

            var dt = Conexion.SqlDataTable(sql);
            var model = new List<Paises>();
            foreach (DataRow row in dt.Rows)
            {
                var p = new Paises
                {
                    CodPais = Convert.ToString(row["CodPais"]),
                    PaisRegimen = Convert.ToString(row["PaisRegimen"]),
                    Bandera = Convert.ToString(row["Bandera"])
                };

                var fechaExact = DateTime.ParseExact(Convert.ToString(row["FechaFin"]), "yyyyMMdd",
                    System.Globalization.CultureInfo.InvariantCulture);
                var cultureFormat = new System.Globalization.CultureInfo(culture);
                p.FechaFin = Convert.ToDateTime(fechaExact).ToString("dd-MMM-yyyy", cultureFormat).ToUpper();
                model.Add(p);
            }

            return model;
        }

        public List<Paises> GeListHome(string culture)
        {
            var paisRegimen = culture.Equals("es") ? "PaisRegimen" : "PaisRegimenEN";

            var sql = "select distinct M.CodPais, " + paisRegimen +
                      " as PaisRegimen, Bandera, FechaFin ";
            sql += "from PaisRegimen M, BaseDatos B ";
            sql += "where M.CodPais = B.CodPais order by FechaFin desc, " + paisRegimen;

            var dt = Conexion.SqlDataTable(sql);
            var model = new List<Paises>();
            foreach (DataRow row in dt.Rows)
            {
                var p = new Paises
                {
                    CodPais = Convert.ToString(row["CodPais"]),
                    PaisRegimen = Convert.ToString(row["PaisRegimen"]),
                    Bandera = Convert.ToString(row["Bandera"])
                };

                var fechaExact = DateTime.ParseExact(Convert.ToString(row["FechaFin"]), "yyyyMMdd",
                    System.Globalization.CultureInfo.InvariantCulture);

                var fortarDate = culture.Equals("es") ? "dd-MMM-yyyy" : "MMM dd, yyyy";
                p.FechaFin = Convert.ToDateTime(fechaExact).ToString(fortarDate, null).ToUpper();
                model.Add(p);
            }

            return model;
        }

        public string GetCod3(string codPais)
        {
            var codigo = "";
            var sql = "SELECT CodPais3 FROM [AdminPais] WHERE CodPais = '" + codPais + "'";

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                codigo = row["CodPais3"].ToString();
            }

            return codigo;
        }
    }
}