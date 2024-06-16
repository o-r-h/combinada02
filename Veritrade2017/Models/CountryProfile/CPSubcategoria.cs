using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Veritrade2017.Helpers;

namespace Veritrade2017.Models.CountryProfile
{

	
	public class CPSubcategoria
	{
        public int IdPais { get; set; }
        public string Regimen { get; set; }
		public int  IdCategoria { get; set; }
		public int NEmpresas { get; set; }
		public int Nsubcategorias { get; set; }
		public decimal Valor { get; set; }
		public string CategoriasES { get; set; }
		public string CategoriasEN { get; set; }
		public string CategoriaSlugEN { get; set; }
		public string CategoriaSlugES { get; set; }
		public string RutaCategoriaSlugEN { get; set; }
		public string RutaCategoriaSlugES { get; set; }

		public string ShorCategoriaES
		{
			get
			{
				if (CategoriasES != null)
				{
					if (CategoriasES.Length > 26)
					{
						return CategoriasES.Substring(0, 26) + "...";
					}
					else
					{
						return CategoriasES;
					}
				}
				return "";
			}
		}
		public string ShortCategoriasEN
		{
			get
			{
				if (CategoriasEN != null)
				{
					if (CategoriasEN.Length > 26)
					{
						return CategoriasEN.Substring(0, 26) + "...";
					}
					else
					{
						return CategoriasEN;
					}
				}
				return "";
			}
		}
		public string ValorStr { get { 
		    return  this.Valor.ToString("#,##0").Replace(".", ",");
		} }









	}





}