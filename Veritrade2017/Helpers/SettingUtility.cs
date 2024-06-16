namespace Veritrade2017.Helpers
{
    public class SettingUtility
    {
        public static string GetUrlBack()
        {
            return Properties.Settings.Default.UrlAdmin;
        }

        public static string GetUrlFront()
        {
            return Properties.Settings.Default.UrlWeb;
        }

        public static string GetAlignetEnviroment()
        {
            return Properties.Settings.Default.AlignetEnviroment;
        }
        public static string GetStripeKey()
        {
            return Properties.Settings.Default.StripeWebKey;
        }

        public static string GetUrlBackHome(string culture)
        {
            //var urlAdmin = SettingUtility.GetUrlBack() + (culture.Equals("es") ? "/Veritrade/MisBusquedas.aspx" : "/Veritrade/MisBusquedas.aspx?l=en");
            return string.Concat( Properties.Settings.Default.UrlAdmin, "/", culture, "/", "mis-busquedas");
        }

        public static string GetUrlBackOld()
        {
            //var urlAdmin = SettingUtility.GetUrlBack() + (culture.Equals("es") ? "/Veritrade/MisBusquedas.aspx" : "/Veritrade/MisBusquedas.aspx?l=en");
            return string.Concat(Properties.Settings.Default.UrlAdminOld);
        }
    }
}