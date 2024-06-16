using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Veritrade2018.Models.Admin
{
    public class AgregarFavoritoTable
    {
       
        public AgregarFavoritoTableHead ObjTableHead { set; get; }

        public List<MiFavorito> ListaFavoritos { set; get; }

        public AgregarFavoritoTable()
        {
            ObjTableHead = new AgregarFavoritoTableHead();
            ListaFavoritos = new List<MiFavorito>();
        }

    }
}