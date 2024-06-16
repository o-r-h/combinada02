using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Veritrade2018.Models.Admin
{
    public class AdminMyFavorite
    {
        public string DescripcionCantidad { get; set; }
        public OptionFavoriteTable OptionFavoriteTable { get; set; }

        public FavoriteUniqueHead FavoriteUniqueHead { get; set; }

        public IEnumerable<SelectListItem> GruposFavoritos { get; set; }
        public IEnumerable<SelectListItem> SoloGruposFavoritos { get; set; }

        public bool EnabledCboGroupsForm { get; set; }

        public List<FavoriteUnique> FavoritesUniques { get; set; }

        public int TotalPaginas { get; set; }

        public int CountVisiblePages { get; set; }

        public string RowsFavoritesUniquesInHtml { get; set; }

        public ArrayList IdsSeleccionados { get; set; }

        public string TipoFavorito { get; set; }
        public AdminMyFavorite()
        {
            OptionFavoriteTable = new OptionFavoriteTable();
            FavoriteUniqueHead = new FavoriteUniqueHead();
            FavoritesUniques = new List<FavoriteUnique>();
            GruposFavoritos = new List<SelectListItem>();
            SoloGruposFavoritos = new List<SelectListItem>();
            IdsSeleccionados = new ArrayList();
            CountVisiblePages = 10;
        }
    }
}