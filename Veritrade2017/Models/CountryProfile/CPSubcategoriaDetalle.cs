
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static Veritrade2017.Models.CountryProfile.Enums;
using System.Threading.Tasks;
using System.Data;
using Veritrade2017.Helpers;
using DevExpress.XtraCharts.Native;
using DevExpress.Office.Utils;

namespace Veritrade2017.Models.CountryProfile
{
	public class CPSubcategoriaDetalleHeader
	{
		public CPSubcategoriaDetalleHeader(List<CPSubcategociriaDetalleEmpresa> listaEmpresas)
		{
			ListaEmpresas = new List<CPSubcategociriaDetalleEmpresa>();
		}

		public string SubCategoriaES { get; set; }
		public string SubcategoriaEN { get; set; }
		public int CantidadEmpresas { get; set; }
		public decimal Valor { get; set; }
		public int IdPais { get; set; }
		public string Regimen { get; set; }

		public List<CPSubcategociriaDetalleEmpresa> ListaEmpresas { get; set; }
		public string ShorCategoriaES
		{
			get
			{
				if (SubCategoriaES != null)
				{
					if (SubCategoriaES.Length > 26)
					{
						return SubCategoriaES.Substring(0, 26) + "...";
					}
					else
					{
						return SubCategoriaES;
					}
				}
				return "";
			}
		}
		public string ShortCategoriasEN
		{
			get
			{
				if (SubcategoriaEN != null)
				{
					if (SubcategoriaEN.Length > 26)
					{
						return SubcategoriaEN.Substring(0, 26) + "...";
					}
					else
					{
						return SubcategoriaEN;
					}
				}
				return "";
			}
		}
		public string ValorStr
		{
			get
			{
				return this.Valor.ToString("#,##0").Replace(".", ",");
			}
		}


	}



	public class CPSubcategociriaDetalleEmpresa {

		public decimal Valor { get; set; }
		public string EmpresaNombre { get; set; }
		public string UrlVal { get; set; }
		public string UrlValEn { get; set; }

	}


	public class CPSubcategoriaDetalleHeaderSolo
	{

		public int IdCategoria { get; set; }
		public int IdSubcategoria { get; set; }
		public string SubCategoriaES { get; set; }
		public int CantEmpresas { get; set; }
		public decimal TotalSuma { get; set; }

	}


	public class CPSubcategoriaDetalleUrlSolo
	{

		public int IdCategoria { get; set; }
		public int IdSubcategoria { get; set; }
		public string Nombre { get; set; }
		public string NombreEN { get; set; }
		public decimal Total { get; set; }
		public string UrlES { get; set; }
		public string UrlEN { get; set; }

		public string ShortNombre
		{
			get
			{
				if (Nombre != null)
				{
					if (Nombre.Length > 26)
					{
						return Nombre.Substring(0, 26) + "...";
					}
					else
					{
						return Nombre.PadRight(26);
					}
				}
				return "";
			}
		}

		public string ShortNombre60
		{
			get
			{
				if (Nombre != null)
				{
					if (Nombre.Length > 60)
					{
						return Nombre.Substring(0, 60) + "...";
					}
					else
					{
						return Nombre.PadRight(60);
					}
				}
				return "";
			}
		}

		public string ShortNombre26
		{
			get
			{
				if (Nombre != null)
				{
					if (Nombre.Length > 20)
					{
						return Nombre.Substring(0, 20) ;
					}
					else
					{
						return Nombre.PadRight(60);
					}
				}
				return "";
			}
		}


		public string ShortNombre30
		{
			get
			{
				if (Nombre != null)
				{
					if (Nombre.Length > 29)
					{
						return Nombre.Substring(0, 29);
					}
					else
					{
						return Nombre.PadRight(60);
					}
				}
				return "";
			}
		}


	}



	public class SubDetalleHeaderyDetalle
	{
		public SubDetalleHeaderyDetalle()
		{
		}

		public SubDetalleHeaderyDetalle(List<CPSubcategoriaDetalleUrlSolo> listaDetalle)
		{
			ListaDetalle = new List<CPSubcategoriaDetalleUrlSolo>();
		}

		public int IdCategoria { get; set; }
		public int IdSubcategoria { get; set; }
		public string SubCategoriaES { get; set; }
		public int CantEmpresas { get; set; }
		public decimal TotalSuma { get; set; }

		public List<CPSubcategoriaDetalleUrlSolo> ListaDetalle { get; set; }

	}


	public class ListadoHeaderyDetalle
	{
		public List<SubDetalleHeaderyDetalle> lista { get; set; }

	}
	

	public class NombresItems{
		public string NombreIngles { get; set; }
		public string NombreEspanol { get; set; }
	}

}



	