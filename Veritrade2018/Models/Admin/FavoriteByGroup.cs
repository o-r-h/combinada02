using System.Collections;
using System.Collections.Generic;

namespace Veritrade2018.Models.Admin
{
    public class FavoriteByGroup
    {
        public FavoriteHead FavoriteHead { get; set; }
        public List<Favorite> Favorites { get; set; }

        public int TotalPaginas { get; set; }

        public int CountVisiblePages { get; set; }

        public bool IsVisibleDelete { get; set; }

        public string FavoritesByGroupInHtml { get; set; }

        public ArrayList IdsSeleccionados { get; set; }

        public FavoriteByGroup()
        {
            FavoriteHead = new FavoriteHead();
            Favorites = new List<Favorite>();
            CountVisiblePages = 10;
            IsVisibleDelete = true;
            IdsSeleccionados = new ArrayList();
        }
    }
}