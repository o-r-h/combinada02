using System.Web;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;

namespace Veritrade2017 {
    public class FilterConfig {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
            filters.Add(new HandleErrorAttribute());
        }
    }
}