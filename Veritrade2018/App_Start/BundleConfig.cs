using System.Web;
using System.Web.Optimization;

namespace Veritrade2018
{
    public class BundleConfig
    {
        // Para obtener más información sobre las uniones, visite https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.unobtrusive-ajax.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate.js",
                        "~/Scripts/jquery.validate.unobtrusive.js"));

            // Utilice la versión de desarrollo de Modernizr para desarrollar y obtener información. De este modo, estará
            // para la producción, use la herramienta de compilación disponible en https://modernizr.com para seleccionar solo las pruebas que necesite.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/highcharts").Include(
                    "~/Scripts/Plugins/Highcharts/highcharts.js",
                    "~/Scripts/Plugins/Highcharts/modules/exporting.js",
                    "~/Scripts/Plugins/BootstrapColorSelector/js/bootstrap-colorselector.js",
                    "~/Scripts/Admin/ThemesHighcharts.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/bootstrapDatepicker").Include(
                "~/Scripts/Plugins/BootstrapDatepicker/js/bootstrap-datepicker.js",
                "~/Scripts/Plugins/BootstrapDatepicker/locales/bootstrap-datepicker.es.min.js"
                ));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap.css",
                "~/Content/site.css",
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

            bundles.Add(new StyleBundle("~/Content/error").Include(
                "~/Content/bootstrap.css",
                "~/Content/Assets/error.css"
                ));

            bundles.Add(new ScriptBundle("~/bundles/mis-busquedas").Include(
                "~/Scripts/Admin/admin.js",
                "~/Scripts/Admin/GlobalVariables.js",
                "~/Scripts/Admin/MisBusquedas/MisBusquedas.js"
                ));


            BundleTable.EnableOptimizations = true;
        }
    }
}
