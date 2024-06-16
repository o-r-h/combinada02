using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Veritrade2018.Models.Admin
{
    public class GrupoFavorito
    {
        public string IdFavorito { get; set; }

        public string Orden { get; set; }

        public string IdGrupo { get; set; }

        public string  Grupo { get; set; }

        public int CantidadFavoritos { get; set; }

        public int Index { get; set; }

        public GrupoFavorito()
        {
            
        }
    }
}