using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Veritrade2018.Models
{
    public class UsuarioPlan
    {
        public string CodVariable { get; set; }
        public string Valor { get; set; }
        public int IdPlan { get; set; }
        public int LimiteFavUnicos { get; set; }
        public int LimiteFavPorGrupo { get; set; }
        public int LimiteGrupos { get; set; }
        public int LimiteVisitas { get; set; }
        public int LimiteDescargas { get; set; }
        public int LimiteRegistros { get; set; }
        public int LimitePlantillas { get; set; }
        public string FlagMisProductos { get; set; }
        public string FlagMisCompañias { get; set; }
    }
}