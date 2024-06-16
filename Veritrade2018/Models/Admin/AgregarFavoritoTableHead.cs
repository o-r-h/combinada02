using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Veritrade2018.Models.Admin
{
    public class AgregarFavoritoTableHead
    {
        public string Nandina { set; get; }
        public string Favorito { set; get; }

        public string NombreFavorito { set; get; }

        public bool IsVisibleNandina { set; get; }

        public bool IsVisbleNombreFavorito { set; get; }

        public AgregarFavoritoTableHead()
        {
        }
    }
}