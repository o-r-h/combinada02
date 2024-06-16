
namespace Veritrade2017.Models.CountryProfile
{
	public class SubcategoriaDTO
	{
        public int IdPais { get; set; }
        public string PaisCorto { get; set; }
        public string Pais { get; set;}
        public string Regimen { get; set; }
		public int IdCategoria { get; set; }
		public int CategoriaES { get; set; }
		public int CategoriaEN { get; set; }
		public int Nsubcategorias { get; set; }
		public int Nsubproductos { get; set; }
		public decimal Valor { get; set; }

	}



}