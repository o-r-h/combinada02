using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Veritrade2018.Models.Admin
{
    public class OptionFavoriteTable
    {
        public string AddFavoriteBtnLabel { get; set; }
        public string AddToGroupBtnLabel { get; set; }
        public string DeleteFavoriteBtnLabel { get; set; }
        public string BackBtnLabel { get; set; }

        public OptionFavoriteTable()
        {
            BackBtnLabel = Resources.MiPerfil.Btn_Back;
            AddToGroupBtnLabel = Resources.MiPerfil.Btn_AddSelectionToAGroup;
            DeleteFavoriteBtnLabel = Resources.MiPerfil.Btn_DeleteSelection;
        }

    }
}