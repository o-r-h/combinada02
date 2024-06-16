using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Veritrade2018.Models.Admin
{
    public class AdminMyGroup
    {
        public string DescripcionCantidad { get; set; }

        public List<GrupoFavorito> GruposFavoritos { get; set; }

        public int TotalPaginas { get; set; }

        public int CountVisiblePages { get; set; }

        public string GroupsFavoritesInHtml { get; set; }

        public ArrayList IdsSeleccionados { get; set; }

        public AdminMyGroup()
        {
            GruposFavoritos = new List<GrupoFavorito>();
            CountVisiblePages = 10;
            IdsSeleccionados = new ArrayList();
        }
    }
}