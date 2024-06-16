using System.Web.Mvc;
using System.Web.Routing;
using Veritrade2018.Helpers;

namespace Veritrade2018
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{resource}.ashx/{*pathInfo}");

            #region SuperAdmin
            routes.MapRoute(
                name: "SuperAdmin",
                url: "admin/{action}/{id}",
                defaults: new
                {
                    controller = "AdminAlertas",
                    action = "Index",
                    id = UrlParameter.Optional
                }
            );
            #endregion

            /* Web */

            #region Inicio

            routes.MapRoute(
                name: "Default", // Route name
                url: "{culture}/", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Home",
                    action = "Index"
                } // Parameter defaults
            );
            routes.MapRoute(
                name: "Sitemap", // Route name
                url: "{culture}/sitemap", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Home",
                    action = "Xml"
                } // Parameter defaults
            );

            routes.MapRoute(
                name: "Contactanos", // Route name
                url: "{culture}/home/contactanos", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Home",
                    action = "Contactanos"
                } // Parameter defaults
            );

            #endregion

            routes.MapRoute(
                name: "Cultura", // Route name
                url: "{culture}/{controller}/SetCulture", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Home",
                    action = "SetCulture"
                } // Parameter defaults
            );

            #region Consultas

            routes.MapRoute(
                name: "Consultas", // Route name
                url: "{culture}/consultas/{action}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Consulta",
                    action = "Index"
                } // Parameter defaults
            );

            #endregion

            #region Demo

            routes.MapRoute(
                name: "Demo", // Route name
                url: "{culture}/demo/{action}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Demo",
                    action = "Index3"
                } // Parameter defaults
            );

            #endregion

            #region Servicios

            routes.MapRoute(
                name: "Servicios", // Route name
                url: "{culture}/servicios/{slug}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Servicios",
                    action = "Index",
                    slug = UrlParameter.Optional
                } // Parameter defaults
            );

            #endregion

            #region Ayuda

            routes.MapRoute(
                name: "Ayuda", // Route name
                url: "{culture}/ayuda/{action}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Ayuda",
                    action = "Index"
                } // Parameter defaults
            );

            #endregion

            #region Paises

            routes.MapRoute(
                name: "Paises", // Route name
                url: "{culture}/paises/{slug}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Paises",
                    action = "Index",
                    slug = UrlParameter.Optional
                } // Parameter defaults
            );

            #endregion

            #region Planes

            routes.MapRoute(
                name: "Planes", // Route name
                url: "{culture}/planes", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Planes",
                    action = "Index"
                } // Parameter defaults
            );

            routes.MapRoute(
                name: "PlanesAdmin", // Route name
                url: "{culture}/planes", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Planes",
                    action = "IndexPost"
                } // Parameter defaults
            );

            routes.MapRoute(
                name: "ExportPdf", // Route name
                url: "{culture}/planes/export", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Planes",
                    action = "Pdf"
                } // Parameter defaults
            );

            routes.MapRoute(
                name: "Cotizacion", // Route name
                url: "{culture}/planes/cotizacion", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Planes",
                    action = "Cotizacion"
                } // Parameter defaults
            );

            routes.MapRoute(
                name: "ValidarPerfil", // Route name
                url: "{culture}/compra/validarPerfil", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Planes",
                    action = "ValidarPerfil"
                } // Parameter defaults
            );

            routes.MapRoute(
                name: "EditarPerfil", // Route name
                url: "{culture}/compra/editarPerfil", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Planes",
                    action = "EditarPerfil"
                } // Parameter defaults
            );

            routes.MapRoute(
                name: "CompraPayMe", // Route name
                url: "{culture}/compra/procesar", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Planes",
                    action = "Charged"
                } // Parameter defaults
            );

            routes.MapRoute(
                name: "CompraStripe", // Route name
                url: "{culture}/compra/payments", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Planes",
                    action = "Payment"
                } // Parameter defaults
            );

            routes.MapRoute(
                name: "Compra", // Route name
                url: "{culture}/planes/compra/{id}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Planes",
                    action = "Compra"
                } // Parameter defaults
            );

            #endregion

            #region Prueba Gratis

            routes.MapRoute(
                name: "PruebaGratisData", // Route name
                url: "{culture}/pruebagratis/ajaxForm", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "PruebaGratis",
                    action = "AjaxForm"
                } // Parameter defaults
            );

            routes.MapRoute(
                name: "PruebaGratis", // Route name
                url: "{culture}/pruebagratis/{campania}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "PruebaGratis",
                    action = "Index",
                    campania = UrlParameter.Optional
                } // Parameter defaults
            );

            routes.MapRoute(
                name: "PruebaGratisPreview", // Route name
                url: "{culture}/preview", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "PruebaGratis",
                    action = "Preview"
                } // Parameter defaults
            );

            #endregion

            #region Contactanos
            routes.MapRoute(
                name: "ContactanosLading", // Route name
                url: "{culture}/contact_landing", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Contactanos",
                    action = "Index"
                } // Parameter defaults
            );
            #endregion

            #region Blog

            routes.MapRoute(
                name: "Blog", // Route name
                url: "{culture}/blog", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Blog",
                    action = "Index"
                } // Parameter defaults
            );

            routes.MapRoute(
                name: "BlogPost", // Route name
                url: "{culture}/blog/post/{id}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Blog",
                    action = "Post"
                } // Parameter defaults
            );

            #endregion

            #region Common

            routes.MapRoute(
                name: "GetPaises", // Route name
                url: "ajax/getPaises", // URL with parameters
                defaults: new
                {
                    controller = "Common",
                    action = "GetPais"
                } // Parameter defaults
            );

            routes.MapRoute(
                name: "IsValidCountrie", // Route name
                url: "ajax/validateCountrie", // URL with parameters
                defaults: new
                {
                    controller = "Common",
                    action = "IsValidCountrie"
                } // Parameter defaults
            );

            routes.MapRoute(
                name: "IsValidPlan", // Route name
                url: "ajax/validatePlan", // URL with parameters
                defaults: new
                {
                    controller = "Common",
                    action = "IsValidPlan"
                } // Parameter defaults
            );

            routes.MapRoute(
                name: "Auth", // Route name
                url: "ajax/auth", // URL with parameters
                defaults: new
                {
                    controller = "Common",
                    action = "Login"
                } // Parameter defaults
            );

            routes.MapRoute(
                name: "Logout", // Route name
                url: "login/logout", // URL with parameters
                defaults: new
                {
                    controller = "Common",
                    action = "Logout"
                } // Parameter defaults
            );

            #endregion

            #region Minisite

            routes.MapRoute(
                name: "MinisiteEmpresas", // Route name
                url: "{culture}/minisite/buscarEmpresa", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Minisite",
                    action = "BuscarEmpresa"
                } // Parameter defaults
            );

            routes.MapRoute(
                name: "Minisite", // Route name
                url: "{culture}/{pais}/importaciones-y-exportaciones-{slug}/{trib}-{ruc}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    pais = "pe",
                    controller = "Minisite",
                    action = "Index"
                } // Parameter defaults
            );

            routes.MapRoute(
                name: "MinisiteUS", // Route name
                url: "{culture}/{pais}/imports-and-exports-{slug}/{trib}-{ruc}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    pais = "pe",
                    controller = "Minisite",
                    action = "Index"
                } // Parameter defaults
            );

            routes.MapRoute(
                name: "MinisiteAction", // Route name
                url: "{culture}/{pais}/importaciones-y-exportaciones-{slug}/{trib}-{ruc}/{action}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    pais = "es",
                    controller = "Minisite",
                    action = "ChartExpMinisite"
                } // Parameter defaults
            );

            #endregion

            #region ProductoPerfil
            routes.MapRoute(
                name: "Perfilproducto", // Route name
                url: "{culture}/producto-perfil", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "ProductoPerfil",
                    action = "Index"
                } // Parameter defaults
            );

            routes.MapRoute(
                name: "ProductoPerfilBuscador", // Route name
                url: "{culture}/producto-perfil/buscarProducto", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "ProductoPerfil",
                    action = "BuscarProducto"
                } // Parameter defaults
            );

            routes.MapRoute(
                name: "ProductoPerfilListar", // Route name
                url: "{culture}/producto-perfil/listarDataProducto", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "ProductoPerfil",
                    action = "BuscarClick"
                } // Parameter defaults
            );
            #endregion

            #region Freemium
            routes.MapRoute(
                name: "Freemium", // Route name
                url: "{culture}/freemium/{action}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Freemium",
                    action = "Index"
                } // Parameter defaults
            );
            #endregion

            #region Referido
            routes.MapRoute(
                name: "Referido", // Route name
                url: "{culture}/referido/{action}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Referido",
                    action = "Index"
                } // Parameter defaults
            );
            #endregion
            /* Admin */

            #region MisBusquedas

            routes.MapRoute(
                name: "MisBusquedas", // Route name
                url: "{culture}/mis-busquedas/{action}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "MisBusquedas",
                    action = "Index"
                } // Parameter defaults
            );

            #endregion

            //#region LoginAlerts

            //routes.MapRoute(
            //    name: "LoginAlerts", // Route name
            //    url: "{culture}/login-alerts/{action}", // URL with parameters
            //    defaults: new
            //    {
            //        culture = CultureHelper.GetDefaultCulture(),
            //        controller = "LoginAlerts",
            //        action = "Index"
            //    } // Parameter defaults
            //);

            //#endregion

            #region MisProductos

            routes.MapRoute(
                name: "MisProductos", // Route name
                url: "{culture}/mis-productos/{action}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "MisProductos",
                    action = "Index"
                } // Parameter defaults
            );

            routes.MapRoute(
                name: "MisProductosProducto", // Route name
                url: "{culture}/mis-productos/buscarProducto", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "MisProductos",
                    action = "BuscarProducto"
                } // Parameter defaults
            );


            #endregion

            #region MisCompanias

            routes.MapRoute(
                name: "MisCompanias", // Route name
                url: "{culture}/mis-compañias/{action}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "MisCompanias",
                    action = "Index"
                } // Parameter defaults
            );

            #endregion

            #region Mi Perfil

            routes.MapRoute(
                name: "MiPerfil", // Route name
                url: "{culture}/mi-perfil/{action}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "MiPerfil",
                    action = "Index"
                } // Parameter defaults
            );

            #endregion

            #region Mis Grupos

            routes.MapRoute(
                name: "MisGrupos", // Route name
                url: "{culture}/mis-grupos/{action}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "MisGrupos",
                    action = "Index"
                } // Parameter defaults
            );

            #endregion

            #region Mis Favoritos

            routes.MapRoute(
                name: "MisFavorito", // Route name
                url: "{culture}/mis-favoritos/{action}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "MisFavoritos",
                    action = "Index"
                } // Parameter defaults
            );

            #endregion

            #region Mi Cuenta

            routes.MapRoute(
                name: "MiCuenta", // Route name
                url: "{culture}/mi-cuenta/{action}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "MiCuenta",
                    action = "Index"
                } // Parameter defaults
            );

            #endregion

            #region Mis AlertasFavoritos
            routes.MapRoute(
                name: "MisAlertasFavoritos", // Route name
                url: "{culture}/mis-alertas-favoritos/{action}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "MisAlertasFavoritos",
                    action = "Index"
                } // Parameter defaults
            );


            #endregion

            #region Admmin Mis AlertasFavoritas

            routes.MapRoute(
                name: "MisAlertasFavoritas", // Route name
                url: "{culture}/admin-mis-alertas-favoritas/{tipoFavorito}/{action}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "AdminMisAlertasFavoritas",
                    action = "Index"
                } // Parameter defaults
            );

            #endregion

            #region Mis Plantillas

            routes.MapRoute(
                name: "MisPlantillas", // Route name
                url: "{culture}/mis-plantillas/{action}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "MisPlantillas",
                    action = "Index"
                } // Parameter defaults
            );

            #endregion

            #region Admmin Mis Productos

            routes.MapRoute(
                name: "AdminMisProductos", // Route name
                url: "{culture}/admin-mis-productos/{action}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "AdminMisProductos",
                    action = "Index"
                } // Parameter defaults
            );

            #endregion

            #region Admin Mis Grupos
            routes.MapRoute(
                name: "AdminMisGrupos",//Route name
                url: "{culture}/admin-mis-grupos/{tipoFavorito}/{action}",
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "AdminMisGrupos",
                    action = "Index"
                    //,tipoFavorito = UrlParameter.Optional
                }
            );

            #endregion

            #region Admmin Mis Favoritos

            routes.MapRoute(
                name: "MisFavoritos", // Route name
                url: "{culture}/admin-mis-favoritos/{tipoFavorito}/{action}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "AdminMisFavoritos",
                    action = "Index"
                } // Parameter defaults
            );
            
            #endregion

            #region MisBusquedasUS
            routes.MapRoute(
                name: "MisBusquedasUS", // Route name
                url: "{culture}/mis-busquedasUS/{action}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "MisBusquedasUS",
                    action = "Index"
                } // Parameter defaults
            );


            #endregion

            #region Error
            routes.MapRoute(
                name: "Error",
                url: "{controller}/{action}",
                defaults: new
                {
                    controller = "Error",
                    action = "Index"
                }
            );

            routes.MapRoute(
                name: "MostrarError",
                url:"Error",
                defaults:new
                {
                    controller ="Error",
                    action ="Mostrar"
                });
            
            #endregion

            #region Correo
            routes.MapRoute(
                name: "Correo",
                url: "{controller}/{action}",
                defaults: new
                {
                    controller ="Correo", 
                    action = "Index"                 
                }
                );
            #endregion

           

        }
    }
}
