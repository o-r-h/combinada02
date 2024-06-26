﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Veritrade2017.Helpers;

namespace Veritrade2017.Models.ProductProfile
{
    public class Detalle
    {
        public string Regimen { set; get; }
        public string CodProducto { set; get; }
        public string Descripcion { set; get; }
        public DateTime Fecha { get; set; }
        public string Partida { set; get; }
        public string Exportador { set; get; }
        public string Importador { set; get; }
        public decimal PesoNeto { set; get; }
        public decimal Cantidad { set; get; }
        public string Unidad { set; get; }
        public decimal CifUnit { set; get; }
        public decimal FobUnit { set; get; }
        public decimal CIFImptoUnit { set; get; }
        public string Dua { set; get; }
        public string PaisOrigen { set; get; }
        public string DesComercial { set; get; }
        public string Aduana { set; get; }
        public string NroCorre { set; get; }
        public int NroSerie { set; get; }
        public string Manifiesto { set; get; }
        public int IdPartida { set; get; }
        public string PartidaDesc { set; get; }
        public string Ruc { set; get; }
        public string Proveedor { set; get; }
        public string PaisProced { set; get; }
        public string PtoEmbarque { set; get; }
        public string ViaTransp { get; set; }
        public string Transporte { get; set; }
        public string Agente { get; set; }
        public string Banco { get; set; }
        public string Almacen { get; set; }
        public string Estado { get; set; }
        public decimal PesoBruto { get; set; }
        public decimal FobTot { get; set; }
        public decimal FleteTot { get; set; }
        public decimal CfrTot { get; set; }
        public decimal SeguroTot { get; set; }
        public decimal CifTot { get; set; }
        public decimal ImptoTot { get; set; }
        public decimal FleteUnit { get; set; }
        public decimal CfrUnit { get; set; }
        public decimal SeguroUnit { get; set; }
        public decimal ImptoUnit { get; set; }
        public int Registro { set; get; }
        public static DataTable DataDetalle(string tipoOpe, string pais, int registro, int IdProducto, int IdPaisAduana)
        {
            var sql = "SELECT TOP 8 Regimen, PaisAduana = '" + pais + "', Registros = " + registro +
                      ",  CONVERT(date,FechaNum) AS FechaNum, ISNULL(Nandina,'') AS Nandina,ISNULL(Proveedor,'') AS Proveedor," +
                      " ISNULL(Importador,'') AS Importador, ISNULL(PesoNeto,0) AS PesoNeto, ISNULL(Cantidad,0) AS Cantidad, " +
                      " ISNULL(CIFUnit,0) AS CifUnit, ISNULL(FOBUnit,0) AS FOBUnit, ISNULL(Unidad,'') AS Unidad, ISNULL(Dua,'') AS Dua, " +
                      " ISNULL(PaisOrigen,'') AS PaisOrigen, ISNULL(DesComercial,'') AS DesComercial" +
                      " FROM Detalle WHERE Regimen = '" + tipoOpe + "' AND IdProducto = " + IdProducto + " AND IdPaisAduana = " + IdPaisAduana;

            // Ruben 202310
            //Functions3.Log("DataDetalle | " + sql);
            
            DataTable dt;
            try
            {
                dt = Conexion.SqlDataTableProductProfile(sql);
            }
            catch (Exception e)
            {
                dt = null;
            }
            return dt;
        }
        public static DataTable DataDetalleModal(string tipoOpe, int IdProducto,int IdPaisAduana)
        {
            var sql = "SELECT TOP 6 regimen, CONVERT(date,FechaNum) AS FechaNum, ISNULL(Nandina,'') AS Nandina,ISNULL(Proveedor,'') AS Proveedor," +
                      " ISNULL(Importador,'') AS Importador, ISNULL(PesoNeto,0) AS PesoNeto, ISNULL(Cantidad,0) AS Cantidad, " +
                      " ISNULL(CIFUnit,0) AS CifUnit, ISNULL(FOBUnit,0) AS FOBUnit, ISNULL(Unidad,'') AS Unidad, ISNULL(Dua,'') AS Dua, " +
                      " ISNULL(PaisProced,'') AS PaisProced, ISNULL(DesComercial,'') AS DesComercial, ISNULL(CIFImptoUnit,0) AS CIFImptoUnit " +
                      " FROM Detalle WHERE Regimen = '" + tipoOpe + "' AND IdProducto = " + IdProducto +  " AND IdPaisAduana = " + IdPaisAduana;
            DataTable dt;
            try
            {
                dt = Conexion.SqlDataTableProductProfile(sql);
            }
            catch (Exception e)
            {
                dt = null;
            }
            return dt;
        }
        public static Detalle DataModalDetalle(int IdProducto, int IdPaisAduana, string idioma)
        {
            var s = new Detalle();
            var sql = "";
            if (idioma.Equals("es"))
            {
                sql = "SELECT TOP 1 Regimen, CONVERT(date,FechaNum) AS FechaNum, Aduana, NroCorre, " +
                      "NroSerie,DUA,Manifiesto,Nandina,PartidaDesc= P.Descripcion_ES,RUC,Importador,Proveedor,PaisOrigen," +
                      "PaisProced,PtoEmbarque,ViaTransp,Transporte,Agente,Banco,Almacen,Estado,DesComercial," +
                      "PesoBruto,PesoNeto,Cantidad,Unidad,FOBTot,FleteTot,CFRTot,SeguroTot,CIFTot,ImptoTot," +
                      "FOBUnit,FleteUnit,CFRUnit,SeguroUnit,CIFUnit,ImptoUnit,CIFImptoUnit FROM Detalle D " +
                      "INNER JOIN PARTIDA P ON P.IdPartida = D.IdPartida WHERE D.IdProducto = " + IdProducto +
                      " AND D.IdPaisAduana = " + IdPaisAduana;
            }
            else
            {
                sql = "SELECT TOP 1 Regimen, CONVERT(date,FechaNum) AS FechaNum, Aduana, NroCorre, " +
                      "NroSerie,DUA,Manifiesto,Nandina,PartidaDesc= P.Descripcion_EN,RUC,Importador,Proveedor,PaisOrigen," +
                      "PaisProced,PtoEmbarque,ViaTransp,Transporte,Agente,Banco,Almacen,Estado,DesComercial," +
                      "PesoBruto,PesoNeto,Cantidad,Unidad,FOBTot,FleteTot,CFRTot,SeguroTot,CIFTot,ImptoTot," +
                      "FOBUnit,FleteUnit,CFRUnit,SeguroUnit,CIFUnit,ImptoUnit,CIFImptoUnit FROM Detalle D " +
                      "INNER JOIN PARTIDA P ON P.IdPartida = D.IdPartida WHERE D.IdProducto = " + IdProducto +
                      " AND D.IdPaisAduana = " + IdPaisAduana;
            }


            var dt = Conexion.SqlDataTableProductProfile(sql);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    s = new Detalle
                    {
                        Regimen = row.GetValue<string>("regimen"),
                        Fecha = row.GetValue<DateTime>("FechaNum"),
                        Aduana = row.GetValue<string>("Aduana"),
                        NroCorre = row.GetValue<string>("NroCorre"),
                        NroSerie = row.GetValue<int>("NroSerie"),
                        Dua = row.GetValue<string>("DUA"),
                        Manifiesto = row.GetValue<string>("Manifiesto"),
                        Partida = row.GetValue<string>("Nandina"),
                        PartidaDesc = row.GetValue<string>("PartidaDesc"),
                        Ruc = row.GetValue<string>("RUC"),
                        Importador = row.GetValue<string>("Importador"),
                        Proveedor = row.GetValue<string>("Proveedor"),
                        PaisOrigen = row.GetValue<string>("PaisOrigen"),
                        PaisProced = row.GetValue<string>("PaisProced"),
                        PtoEmbarque = row.GetValue<string>("PtoEmbarque"),
                        ViaTransp = row.GetValue<string>("ViaTransp"),
                        Transporte = row.GetValue<string>("Transporte"),
                        Agente = row.GetValue<string>("Agente"),
                        Banco = row.GetValue<string>("Banco"),
                        Almacen = row.GetValue<string>("Almacen"),
                        Estado = row.GetValue<string>("Estado"),
                        DesComercial = row.GetValue<string>("DesComercial"),
                        PesoBruto = row.GetValue<decimal>("PesoBruto"),
                        PesoNeto = row.GetValue<decimal>("PesoNeto"),
                        Cantidad = row.GetValue<decimal>("Cantidad"),
                        Unidad = row.GetValue<string>("Unidad"),
                        FobTot = row.GetValue<decimal>("FobTot"),
                        FleteTot = row.GetValue<decimal>("FleteTot"),
                        CfrTot = row.GetValue<decimal>("CfrTot"),
                        SeguroTot = row.GetValue<decimal>("SeguroTot"),
                        CifTot = row.GetValue<decimal>("CifTot"),
                        ImptoTot = row.GetValue<decimal>("ImptoTot"),
                        FobUnit = row.GetValue<decimal>("FobUnit"),
                        FleteUnit = row.GetValue<decimal>("FleteUnit"),
                        CfrUnit = row.GetValue<decimal>("CfrUnit"),
                        SeguroUnit = row.GetValue<decimal>("SeguroUnit"),
                        CifUnit = row.GetValue<decimal>("CifUnit"),
                        ImptoUnit = row.GetValue<decimal>("ImptoUnit"),
                        CIFImptoUnit = row.GetValue<decimal>("CifImptoUnit")
                    };
                }
            }
            return s;
        }
    }
}