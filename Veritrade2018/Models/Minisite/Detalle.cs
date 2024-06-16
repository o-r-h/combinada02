using System;
using System.Collections.Generic;
using System.Data;
using Veritrade2018.Helpers;

namespace Veritrade2018.Models.Minisite
{
    public class Detalle
    {
        public int IdEmpresa { get; set; }
        public string Regimen { get; set; }
        public DateTime FechaNum { get; set; }
        public string Aduana { get; set; }
        public string NroCorre { get; set; }
        public int NroSerie { get; set; }
        public string Dua { get; set; }
        public string Manifiesto { get; set; }
        public int IdPartida { get; set; }
        public string Nandina { get; set; }
        public string PartidaDesc { get; set; }
        public string Ruc { get; set; }
        public string Importador { get; set; }
        public string Proveedor { get; set; }
        public string PaisOrigen { get; set; }
        public string PaisProced { get; set; }
        public string PtoEmbarque { get; set; }
        public string ViaTransp { get; set; }
        public string Transporte { get; set; }
        public string Agente { get; set; }
        public string Banco { get; set; }
        public string Almacen { get; set; }
        public string Estado { get; set; }
        public string DesComercial { get; set; }
        public decimal PesoBruto { get; set; }
        public decimal PesoNeto { get; set; }
        public decimal Cantidad { get; set; }
        public string Unidad { get; set; }
        public decimal FobTot { get; set; }
        public decimal FleteTot { get; set; }
        public decimal CfrTot { get; set; }
        public decimal SeguroTot { get; set; }
        public decimal CifTot { get; set; }
        public decimal ImptoTot { get; set; }
        public decimal FobUnit { get; set; }
        public decimal FleteUnit { get; set; }
        public decimal CfrUnit { get; set; }
        public decimal SeguroUnit { get; set; }
        public decimal CifUnit { get; set; }
        public decimal ImptoUnit { get; set; }
        public decimal CifImptoUnit { get; set; }

        public static List<Detalle> GetData(int empresa, int tipo = 0)
        {
            var detalles = new List<Detalle>();
            string sql;

            if (tipo == 0)
            {
                sql = "SELECT TOP(8) * FROM [Detalle] WHERE Regimen = 'Importacion' AND IdEmpresa = " + empresa;
            }
            else
            {
                sql = "SELECT TOP(8) * FROM [Detalle] WHERE Regimen = 'Exportacion' AND IdEmpresa = " + empresa;
            }

            var dt = Conexion.SqlDataTableMinisite(sql);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    var s = new Detalle
                    {
                        Regimen = row["regimen"].ToString(),
                        FechaNum = Convert.ToDateTime(row["FechaNum"].ToString()),
                        Nandina = row["Nandina"].ToString(),
                        Dua = row["DUA"].ToString(),
                        IdPartida = Convert.ToInt32(row["IdPartida"].ToString()),
                        Importador = row["Importador"].ToString(),
                        Proveedor = row["Proveedor"].ToString(),
                        PaisProced = row["PaisProced"].ToString(),
                        DesComercial = row["DesComercial"].ToString(),
                        PesoBruto = Convert.ToDecimal(row["PesoBruto"].ToString()),
                        PesoNeto = Convert.ToDecimal(row["PesoNeto"].ToString()),
                        Cantidad = Convert.ToDecimal(row["Cantidad"].ToString()),
                        Unidad = row["Unidad"].ToString(),
                        FobUnit = Convert.ToDecimal(row["FobUnit"].ToString()),
                        CifUnit = Convert.ToDecimal(row["CifUnit"].ToString())
                    };
                    detalles.Add(s);
                }
            }

            return detalles;
        }

        public static List<Detalle> GetDataPartida(int empresa)
        {
            var promedio = new List<Detalle>();
            var sql = "SELECT TOP (6) FechaNum, Nandina, Importador, PesoNeto, Cantidad, Unidad, " +
                      " FobUnit, CifUnit, CifImptoUnit, PaisOrigen, DesComercial FROM [Detalle] WHERE IdEmpresa = " +
                      empresa;

            var dt = Conexion.SqlDataTableMinisite(sql);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    var s = new Detalle
                    {
                        FechaNum = Convert.ToDateTime(row["FechaNum"].ToString()),
                        Nandina = row["Nandina"].ToString(),
                        Importador = row["Importador"].ToString(),
                        PesoNeto = Convert.ToDecimal(row["PesoNeto"].ToString()),
                        Cantidad = Convert.ToDecimal(row["Cantidad"].ToString()),
                        Unidad = row["Unidad"].ToString(),
                        FobUnit = Convert.ToDecimal(row["FobUnit"].ToString()),
                        CifUnit = Convert.ToDecimal(row["CifUnit"].ToString()),
                        CifImptoUnit = Convert.ToDecimal(row["CifImptoUnit"].ToString()),
                        PaisOrigen = row["PaisOrigen"].ToString(),
                        DesComercial = row["DesComercial"].ToString()
                    };
                    promedio.Add(s);
                }
            }

            return promedio;
        }

        public static Detalle GetFirstDetalle(int empresa)
        {
            var s = new Detalle();
            var sql = "SELECT TOP (1) Nandina, PartidaDesc FROM [Detalle] WHERE IdEmpresa = " + empresa;

            var dt = Conexion.SqlDataTableMinisite(sql);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    s = new Detalle
                    {
                        Nandina = row["Nandina"].ToString(),
                        PartidaDesc = row["PartidaDesc"].ToString(),
                    };
                }
            }
            return s;
        }

        public static Detalle GetDataEmbarque(int empresa)
        {
            var s = new Detalle();
            var sql = "SELECT TOP (1) * FROM [Detalle] WHERE IdEmpresa = " + empresa;

            var dt = Conexion.SqlDataTableMinisite(sql);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    s = new Detalle
                    {
                        Regimen = row["regimen"].ToString(),
                        FechaNum = Convert.ToDateTime(row["FechaNum"].ToString()),
                        Aduana = row["Aduana"].ToString(),
                        NroCorre = row["NroCorre"].ToString(),
                        NroSerie = Convert.ToInt32(row["NroSerie"].ToString()),
                        Dua = row["DUA"].ToString(),
                        Manifiesto = row["Manifiesto"].ToString(),
                        IdPartida = Convert.ToInt32(row["IdPartida"].ToString()),
                        Nandina = row["Nandina"].ToString(),
                        PartidaDesc = row["PartidaDesc"].ToString(),
                        Ruc = row["RUC"].ToString(),
                        Importador = row["Importador"].ToString(),
                        Proveedor = row["Proveedor"].ToString(),
                        PaisOrigen = row["PaisOrigen"].ToString(),
                        PaisProced = row["PaisProced"].ToString(),
                        PtoEmbarque = row["PtoEmbarque"].ToString(),
                        ViaTransp = row["ViaTransp"].ToString(),
                        Transporte = row["Transporte"].ToString(),
                        Agente = row["Agente"].ToString(),
                        Banco = row["Banco"].ToString(),
                        Almacen = row["Almacen"].ToString(),
                        Estado = row["Estado"].ToString(),
                        DesComercial = row["DesComercial"].ToString(),
                        PesoBruto = Convert.ToDecimal(row["PesoBruto"].ToString()),
                        PesoNeto = Convert.ToDecimal(row["PesoNeto"].ToString()),
                        Cantidad = Convert.ToDecimal(row["Cantidad"].ToString()),
                        Unidad = row["Unidad"].ToString(),
                        FobTot = Convert.ToDecimal(row["FobTot"].ToString()),
                        FleteTot = Convert.ToDecimal(row["FleteTot"].ToString()),
                        CfrTot = Convert.ToDecimal(row["CfrTot"].ToString()),
                        SeguroTot = Convert.ToDecimal(row["SeguroTot"].ToString()),
                        CifTot = Convert.ToDecimal(row["CifTot"].ToString()),
                        ImptoTot = Convert.ToDecimal(row["ImptoTot"].ToString()),
                        FobUnit = Convert.ToDecimal(row["FobUnit"].ToString()),
                        FleteUnit = Convert.ToDecimal(row["FleteUnit"].ToString()),
                        CfrUnit = Convert.ToDecimal(row["CfrUnit"].ToString()),
                        SeguroUnit = Convert.ToDecimal(row["SeguroUnit"].ToString()),
                        CifUnit = Convert.ToDecimal(row["CifUnit"].ToString()),
                        ImptoUnit = Convert.ToDecimal(row["ImptoUnit"].ToString()),
                        CifImptoUnit = Convert.ToDecimal(row["CifImptoUnit"].ToString())
                    };
                }
            }

            return s;
        }

        public static double GetTotalFob(int empresa, int tipo = 0)
        {
            var total = 0.0;
            string sql;

            if (tipo == 0)
            {
                sql =
                    "SELECT ISNULL(SUM(FOBTot),0) as 'Total' FROM Detalle WHERE Regimen = 'Importacion' AND IdEmpresa = '" +
                    empresa + "'";
            }
            else
            {
                sql =
                    "SELECT ISNULL(SUM(FOBTot),0) as 'Total' FROM Detalle WHERE Regimen = 'Exportacion' AND IdEmpresa = '" +
                    empresa + "'";
            }

            var dt = Conexion.SqlDataTableMinisite(sql);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    total = Convert.ToDouble(row["Total"].ToString());
                }
            }

            return total;
        }

        public static double GetTotalCif(int empresa, int tipo = 0)
        {
            var total = 0.0;
            string sql;

            if (tipo == 0)
            {
                sql =
                    "SELECT ISNULL(SUM(CIFTot),0) as 'Total' FROM Detalle WHERE Regimen = 'Importacion' AND IdEmpresa = '" +
                    empresa + "'";
            }
            else
            {
                sql =
                    "SELECT ISNULL(SUM(CIFTot),0) as 'Total' FROM Detalle WHERE Regimen = 'Exportacion' AND IdEmpresa = '" +
                    empresa + "'";
            }

            var dt = Conexion.SqlDataTableMinisite(sql);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    total = Convert.ToDouble(row["Total"].ToString());
                }
            }

            return total;
        }
    }
}