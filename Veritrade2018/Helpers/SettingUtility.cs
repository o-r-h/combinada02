using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Veritrade2018.Helpers
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

        public static string GetUrlFrontLogout()
        {
            return  string.Concat(Properties.Settings.Default.UrlWeb, "/login/logout");
        }

        public static string GetAlignetEnviroment()
        {
            return Properties.Settings.Default.AlignetEnviroment;
        }
        public static string GetStripeKey()
        {
            return Properties.Settings.Default.StripeWebKey;
        }
    }

}