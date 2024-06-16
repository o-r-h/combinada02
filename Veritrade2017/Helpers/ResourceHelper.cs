using System.Globalization;
using System.Resources;

namespace Veritrade2017.Helpers
{
	public static class ResourceHelper
	{
		private static readonly ResourceManager ResourceManager = new ResourceManager("Veritrade2017.ResourceCp", typeof(ResourceHelper).Assembly);

		public static string GetResourceString(string key, string language = "es")
		{
			CultureInfo culture = new CultureInfo(language);
			return ResourceManager.GetString(key, culture);
		}
	}
}