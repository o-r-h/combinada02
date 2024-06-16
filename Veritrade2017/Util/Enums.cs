using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Veritrade2017.Util
{
    public class Enums
    {
        public enum TipoFiltro
        {
            Resumen,
            Tab,
            Todos
        }

        public enum Filtro
        {
            Partida,
            Marca,
            Modelo,
            Importador,
            Proveedor,
            Exportador,
            ImportadorExp,
            PaisOrigen,
            PaisDestino,
            MarcaEC,
            ViaTransp,
            AduanaDUA,
            Aduana,
            Distrito,
            DUA,
            Embarcador,
            Notificado,
            PaisEmbarque,
            PtoEmbarque,
            PtoDestino,
            PtoDescarga,
            Manifiesto,
            NewTab,//NUEVO TAB,
            InfoTabla,//NUEVO TAB
        }

        public enum TipoOpe
        {
            I,
            E
        }

        public enum VarGrupo
        {
            [Display(Name = "TFI")] TIPO_FILTRO,
            [Display(Name = "FIL")] FILTRO,
            [Display(Name = "REG")] REGION,
            [Display(Name = "SRE")] SUB_REGION,
            [Display(Name = "TOP")] TIPO_OPERACION
        }

        public enum VarId
        {
            [Display(Name = "DEC")] DESC_COMERCIAL,
            [Display(Name = "PAR")] PARTIDAS,
            [Display(Name = "EXP")] EXPORTADOR,
            [Display(Name = "IMP")] IMPORTADOR,
            [Display(Name = "PAI")] PAIS,
            [Display(Name = "MAR")] MARCA, //SOLO ETIQUETA
            [Display(Name = "MAE")] MARCAEC,
            [Display(Name = "VIA")] VIA,
            [Display(Name = "ADU")] ADUANA,
            [Display(Name = "FMB")] MIS_BUS,
            [Display(Name = "MCP")] MIS_COMPA,
            [Display(Name = "MPR")] MIS_PROD,
            [Display(Name = "TMB")] TABS_MIS_BUS,
            [Display(Name = "DEA")] DESC_ADICIONAL,
            [Display(Name = "NOT")] NOTIFICADO,
            [Display(Name = "PTD")] PTO_DESCARGA,
            [Display(Name = "PTE")] PTO_EMBARQUE,
            [Display(Name = "PDN")] PTO_DESTINO,
            [Display(Name = "MFT")] MANIFIESTO,
            [Display(Name = "DIS")] DISTRITO,
            [Display(Name = "MMO")] MARCAS_MODELOS,
            [Display(Name = "XPMAP")] EXCLUDE_PAIS_MAF,
            [Display(Name = "XPMP")] EXCLUDE_PAIS_MP,
            [Display(Name = "XRMC")] EXCLUDE_REG_MC,
            [Display(Name = "XPMC")] EXCLUDE_PAIS_MC,
            [Display(Name = "ACP")] ALERT_CP,
            [Display(Name = "AMC")] ALERT_MC,
            [Display(Name = "AMP")] ALERT_MP,
            [Display(Name = "APC")] ALERT_PC,
        }

        public enum TipoUsuario
        {
            [Display(Name = "Free Trial")] FREE_TRIAL,
            [Display(Name = "Cliente")] CLIENTE,
            [Display(Name = "Gratis")] GRATIS,
            [Display(Name = "Convenio")] CONVENIO,
        }

        public enum Planes
        {
            [Display(Name = "Business")] BUSINESS,
            [Display(Name = "Premium")] PREMIUM,
            [Display(Name = "Esencial")] ESENCIAL,
            [Display(Name = "Peru Uno")] PERU_UNO,
            [Display(Name = "Peru ImEx")] PERU_IMEX,
            [Display(Name = "Ecuador Uno")] ECUADOR_UNO,
            [Display(Name = "Ecuador ImEx")] ECUADOR_IMEX,
        }

    }
}