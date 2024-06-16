using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Veritrade2018.Models.Admin
{
    public class FavoriteUniqueHead
    {
        public string FavoriteDescription { get; set; }

        public string FavoriteRuc { get; set; }

        public bool IsVisibleRuc { get; set; }

        public string MyGroupsDescription { get; set; }

        public bool EnabledCboGroups { get; set; }

        public FavoriteUniqueHead()
        {
            MyGroupsDescription = Resources.MiPerfil.MyGroups_Text;
            EnabledCboGroups = true;
        }
    }
}