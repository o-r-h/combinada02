using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Veritrade2018.Models.Admin
{
    public class FavoriteUnique
    {
        public int Index { get; set; }

        public string IdFavorito { get; set; }
        public string Favorito { get; set; }

        public string RUC { get; set; }

        public List<GrupoFavorito> GroupsFavories { get; set; }

        public FavoriteUnique()
        {
            GroupsFavories = new List<GrupoFavorito>();
        }
    }
}