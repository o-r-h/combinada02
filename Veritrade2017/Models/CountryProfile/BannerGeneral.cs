

namespace Veritrade2017.Models.CountryProfile
{
	public class BannerGeneral
	{
		public int IdPais { get; set; }
		public string NombreES { get; set; }
		public string NombreEN { get; set; }
		public string Uri { get; set; }

		public string ShortNombreES
		{
			get
			{
				if (NombreES != null)
				{
					if (NombreES.Length > 30)
					{
						return NombreES.Substring(0, 30) + "...";
					}
					else
					{
						return NombreES;
					}
				}
				return "";
			}
		}

	}
}