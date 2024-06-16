using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Veritrade2018.Models.Admin
{
    public class AdminMyProduct
    {
        public string DescripcionCantidad { get; set; }
        public List<ProductoFavorito> ProductosFavoritos { get; set; }

        public int TotalPaginas { get; set; }

        public int CountVisiblePages { get; set; }

        public IEnumerable<SelectListItem> GruposFavoritos { get; set; }
        public IEnumerable<SelectListItem> SoloGruposFavoritos { get; set; }

        public bool EnabledCboGroupsForm { get; set; }

        public ArrayList IdsSeleccionados { get; set; }

        public string FilasProductosFavoritos { get; set; }

        public string TipoFavorito { get; set; }
        public AdminMyProduct()
        {
            ProductosFavoritos =  new List<ProductoFavorito>();
            GruposFavoritos = new List<SelectListItem>();
            SoloGruposFavoritos = new List<SelectListItem>();
            IdsSeleccionados = new ArrayList();
            CountVisiblePages = 10;
        }
    }
}