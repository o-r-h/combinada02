using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Veritrade2018.Models
{
    public class ListProducto
    {
        public string IdPartida { get; set; }
        public string Nandina { get; set; }
        public string Partida { get; set; }
        public string PartidaFav { get; set; }
        public string FlagIndividual { get; set; }

        public List<TableProducts> ListaGrupos { get; set; }
        public ListProducto()
        {

        }
    }
}