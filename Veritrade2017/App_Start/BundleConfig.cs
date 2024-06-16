using System.Web.Optimization;

namespace Veritrade2017
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/Scripts/bundles").Include(
                "~/Scripts/bootstrap.js",
                "~/Scripts/Assets/menu.js"
            ));

            bundles.Add(new StyleBundle("~/Content/bundles").Include(
                "~/Content/bootstrap.css",
                "~/Content/fontawesome/css/all.css",
                "~/Content/Site.css",
                "~/Content/Responsive/_375px.css",
                "~/Content/Responsive/_480px.css",
                "~/Content/Responsive/_768px.css",
                "~/Content/Responsive/_992px.css",
                "~/Content/Responsive/_1360px.css",
                "~/Content/Responsive/_1560px.css",
                "~/Content/Components/dropdown-menu.css",
                "~/Content/Assets/home.css",
                "~/Content/Assets/modals.css",
                "~/Content/Assets/loaderLogin.css"
            ));

            bundles.Add(new StyleBundle("~/Content/admin").Include(
                "~/Content/bootstrap.css",
                "~/Content/themes/base/jquery-ui.min.css",
                "~/Content/Assets/admin/admin.css",
                "~/Content/Assets/loading.css"
            ));

            bundles.Add(new StyleBundle("~/Content/home").Include(
                "~/Content/Assets/consulta.css",
                "~/Content/Assets/servicios.css",
                "~/Content/Components/ui-autocomplete.css"
            ));

            bundles.Add(new StyleBundle("~/Content/minisite").Include(
                "~/Content/Assets/minisite.css",
                "~/Content/Assets/modals.css",
                "~/Content/Components/ui-autocomplete.css"
            ));
           
        }
    }
}