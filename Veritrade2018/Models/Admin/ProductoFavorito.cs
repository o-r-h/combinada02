using System.Collections.Generic;

namespace Veritrade2018.Models.Admin
{
    public class ProductoFavorito
    {
        public int Index { get; set; }
        public string IdPartida { get; set; }
        public string Nandina { get; set; }

        public string Partida { get; set; }

        public string PartidaFav { get; set; }

        public string FlagIndividual { get; set; }

        public List<GrupoFavorito> GrupoFavoritos { get; set; }

        public bool IsVisibleActualizar { get; set; }

        public ProductoFavorito()
        {
            GrupoFavoritos =  new List<GrupoFavorito>();
        }
    }
}