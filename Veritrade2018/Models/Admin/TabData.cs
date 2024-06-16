using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Veritrade2018.Helpers;

namespace Veritrade2018.Models.Admin
{
    public class TabData
    {
        public  string Filtro { set; get; }
        public GridHead GridHead;
        public List<GridRow> ListRows { set; get; }
        
        public List<GridRow> ListRowsTab { set; get; }

        public string TituloTab { set; get; }
        public string CodPais { get; set; }

        public List<GridRow> ListRowsCbo { set; get; }

        public string TotalRegistros { set; get; }
        public string CiFoFobTotal { set; get; }

        public  string PesoNeto { set; get; }

        public string AddMyFavouritesButton { set; get; }
        public string AddToFiltersAndSearchButton { set; get; }

        public int TotalPaginasResumen { set; get; }

        public int TotalPaginasTab { set; get; }

        public ArrayList IdsSeleccionados;

        public bool IsVisibleButtons { get; set; }

        public bool FlagRegMax { get; set; }

        public bool IsVisbleOpcionDescarga { get; set; }

        public string Lang { get; set; }

        public List<GridHead> HeadTitles { get; set; }
        //public List<VerDetalleModal> HeadTitles { get; set; }

        public SelectList DropDownDescarga { get; set; }

        public /*FlagVarVisibles*/ TabMisBusquedas  FlagVarVisibles { get; set; }

        public bool HideTabExcel { get; set; }

        public bool IsVisibleCheck { get; set; }
        public string CodPaisComplementario { get; set; }

        public bool IsVisibleInfoComplementario { get; set; }

        public TabData()
        {
            GridHead = new GridHead();
            ListRows = new List<GridRow>();
            ListRowsTab = new List<GridRow>();
            ListRowsCbo = new List<GridRow>();
            AddToFiltersAndSearchButton = "";
            AddMyFavouritesButton = "";
            IdsSeleccionados = new ArrayList();
            IsVisibleButtons = true;
            IsVisbleOpcionDescarga = true;
            DropDownDescarga = null;
            IsVisibleCheck = true;
            CodPaisComplementario = "";
            IsVisibleInfoComplementario = true;//Properties.Settings.Default.TableVarGeneral_InDev;//true;
        }
    }
}