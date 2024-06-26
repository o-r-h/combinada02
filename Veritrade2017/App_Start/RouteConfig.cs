using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Ajax.Utilities;
using Veritrade2017.Helpers;

namespace Veritrade2017
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{resource}.ashx/{*pathInfo}");

            // Ruben 202211
            routes.IgnoreRoute("{resource}.xml/{*pathInfo}");
            routes.IgnoreRoute("{resource}.txt/{*pathInfo}");

            routes.IgnoreRoute("Content/{*pathInfo}");

            // Ruben 202211
            routes.RouteExistingFiles = true;

            /* Web */
            #region Inicio 

            // Ruben 202212
            routes.MapRoute(
                name: "Root",
                url: "",
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(), // Ruben 202307
                    controller = "Home",
                    action = "Index"
                }
            );

            // Ruben 202212
            routes.MapRoute(
                name: "RootCulture",
                url: "{culture}", // Ruben 202212
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Home",
                    action = "Index"
                },
                constraints: new { culture = "es|en" }
            );

            // Ruben 202212
            /*
            routes.MapRoute(
                name: "RootEs",
                url: "es", // {culture} // Ruben 202212
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Home",
                    action = "Index"
                }
            );
            */

            // Ruben 202212
            /*
            routes.MapRoute(
                name: "RootEn",
                url: "en", // {culture} // Ruben 202212
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Home",
                    action = "Index"
                }
            );
            */

            // Ruben 202301
            routes.MapRoute(
                name: "RootCampania",
                url: "{campania}/{idc}", // Ruben 202301
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Home",
                    action = "Index",
                    campania = UrlParameter.Optional,
                    idc = UrlParameter.Optional // Ruben 202301
                },
                constraints: new { campania = "^[0-9]+$" }
            );

            // Ruben 202212
            /*
            routes.MapRoute(
                name: "Default",
                url: "es", // {culture} // Ruben 202212
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Home",
                    action = "Index"
                }
            );
            */

            routes.MapRoute(
                name: "HomeCampania", // Route name
                url: "{culture}/c/{campania}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Home",
                    action = "Index",
                    campania = UrlParameter.Optional
                } // Parameter defaults
            );
            
            routes.MapRoute(
                name: "SetLogin", // Route name
                url: "{culture}/setlogin", // URL with parameters
                //url: "{culture}/setlogin/{token}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Home",
                    action = "SetLogin"
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

            routes.MapRoute(
               name: "Telefonos", // Route name
               url: "{culture}/home/telefonos", // URL with parameters
               defaults: new
               {
                   culture = CultureHelper.GetDefaultCulture(),
                   controller = "Home",
                   action = "ObtenerTelefonosPaises"
               } // Parameter defaults
            );

            routes.MapRoute(
               name: "Telefono", // Route name
               url: "{culture}/home/telefono", // URL with parameters
               defaults: new
               {
                   culture = CultureHelper.GetDefaultCulture(),
                   controller = "Home",
                   action = "ObtenerTelefonosPorId"
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
                url: "{culture}/consultas/{action}/{codPaisParam}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Consulta",
                    action = "Index",
                    codPaisParam = ""
                } // Parameter defaults
            );  

            #endregion



            #region Servicios

            routes.MapRoute(
                name: "Servicios", // Route name
                url: "{culture}/servicios/{slug}/{codCampania}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Servicios",
                    action = "Index",
                    slug = UrlParameter.Optional,
                    codCampania = UrlParameter.Optional
                } // Parameter defaults
            );

            routes.MapRoute(
                name: "ServiciosUS", // Route name
                url: "{culture}/services/{slug}/{codCampania}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Servicios",
                    action = "Index",
                    slug = UrlParameter.Optional,
                    codCampania = UrlParameter.Optional
                } // Parameter defaults
            );

            #endregion

            #region Ayuda

            routes.MapRoute(
                name: "Ayuda", // Route name
                url: "{culture}/ayuda/{codCampania}", // URL with parameters
                //url: "{culture}/ayuda/{action}/{codCampania}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Ayuda",
                    action = "Index",
                    codCampania = UrlParameter.Optional
                } // Parameter defaults
            );

            routes.MapRoute(
                name: "AyudaUS", // Route name
                url: "{culture}/help/{codCampania}", // URL with parameters
                //url: "{culture}/help/{action}/{codCampania}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Ayuda",
                    action = "Index",
                    codCampania = UrlParameter.Optional
                } // Parameter defaults
            );

            #endregion

            #region Paises

            routes.MapRoute(
                name: "Paises", // Route name
                url: "{culture}/paises/{slug}/{codCampania}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Paises",
                    action = "Index",
                    slug = UrlParameter.Optional,
                    codCampania = UrlParameter.Optional
                } // Parameter defaults
            );

            routes.MapRoute(
                name: "PaisesUS", // Route name
                url: "{culture}/countries/{slug}/{codCampania}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Paises",
                    action = "Index",
                    slug = UrlParameter.Optional,
                    codCampania = UrlParameter.Optional
                } // Parameter defaults
            );

            #endregion

            #region Planes

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
                name: "Compra", // Route name
                url: "{culture}/planes/compra/{id}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Planes",
                    action = "Compra"
                } // Parameter defaults
            );

            routes.MapRoute(
                name: "Planes", // Route name
                url: "{culture}/planes/{campania}/{idc}", // Ruben 202301
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Planes",
                    action = "Index",
                    campania = UrlParameter.Optional,
                    idc = UrlParameter.Optional // Ruben 202301
                }, // Parameter defaults
                constraints: new { campania = "^[0-9]+$" }
            );

            // Ruben 202301
            /*
            routes.MapRoute(
                name: "Planes", // Route name
                url: "{culture}/planes/{campania}/{idc}", // Ruben 202301
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Planes",
                    action = "Index",
                    campania = UrlParameter.Optional,
                    idc = UrlParameter.Optional // Ruben 202301
                } // Parameter defaults
            );
            */

            routes.MapRoute(
                name: "PlanesUS", // Route name
                url: "{culture}/plans/{campania}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Planes",
                    action = "Index",
                    campania = UrlParameter.Optional
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
                name: "TransaccionPruebaTransbank", // Route name
                url: "{culture}/compra/TransaccionPruebaTransbank", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Planes",
                    action = "TransaccionPruebaTransbank"
                } // Parameter defaults
            ); 

            routes.MapRoute(
                name: "PruebaCorreo", // Route name
                url: "{culture}/compra/pruebaCorreo", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Planes",
                    action = "CorreoConfirmacionPrueba"
                } // Parameter defaults
            );

            routes.MapRoute(
                name: "ValidarPerfilTransbank", // Route name
                url: "{culture}/compra/ValidarPerfilTransbank", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Planes",
                    action = "ValidarPerfilTransbank"
                } // Parameter defaults
            );

            routes.MapRoute(
                name: "ValidarRetornoTransbank", // Route name
                url: "{culture}/compra/ValidarRetornoTransbank", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Planes",
                    action = "ValidarRetornoTransbank"
                } // Parameter defaults
            );

            routes.MapRoute(
                name: "FinTransbank", // Route name
                url: "{culture}/compra/FinTransbank/{buyOrder}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Planes",
                    action = "FinTransbank"
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
                name: "CompraUS", // Route name
                url: "{culture}/plans/purchase/{id}", // URL with parameters
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
                name: "PruebaGratisUS", // Route name
                url: "{culture}/freetrial/{campania}", // URL with parameters
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
                url: "{culture}/contacto_landing", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Contactanos",
                    action = "Index"
                } // Parameter defaults
            );

            routes.MapRoute(
                name: "ContactUsLading", // Route name
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
               name: "BlogPostError", // Route name
               url: "{culture}/blog/post/www.veritradecorp.com", // URL with parameters
               defaults: new
               {
                   culture = CultureHelper.GetDefaultCulture(),
                   controller = "Redirect",
                   action = "Index"
               } // Parameter defaults
           );

            routes.MapRoute(
                name: "Blog", // Route name
                url: "{culture}/blog/{campania}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Blog",
                    action = "Index",
                    campania = UrlParameter.Optional
                } // Parameter defaults
            );

            routes.MapRoute(
                name: "BlogPost", // Route name
                url: "{culture}/blog/post/{id}/{codCampania}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Blog",
                    action = "Post",
                    codCampania = UrlParameter.Optional
                } // Parameter defaults
            );

            #endregion

            #region Buscar Empresas

            routes.MapRoute(
                name: "EmpresasPost", // Route name
                url: "{culture}/buscar-empresas/{campania}/Post", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Redirect",
                    action = "Index"
                } // Parameter defaults
            );

            routes.MapRoute(
                name: "EmpresasCompra", // Route name
                url: "{culture}/buscar-empresas/{campania}/Compra", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Redirect",
                    action = "Index"
                } // Parameter defaults
            );

            routes.MapRoute(
                name: "Empresas", // Route name
                url: "{culture}/buscar-empresas/{campania}/{action}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Empresas",
                    action = "Index",
                    campania = UrlParameter.Optional
                } // Parameter defaults
            );

            routes.MapRoute(
                name: "EmpresasUS", // Route name
                url: "{culture}/search-companies/{campania}/{action}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Empresas",
                    action = "Index",
                    campania = UrlParameter.Optional
                } // Parameter defaults
            );

            #endregion

            #region Buscar Productos

            routes.MapRoute(
                name: "Productos", // Route name
                url: "{culture}/buscar-productos/{campania}/{action}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Productos",
                    action = "Index",
                    campania = UrlParameter.Optional
                } // Parameter defaults
            );

            routes.MapRoute(
                name: "ProductosUS", // Route name
                url: "{culture}/search-products/{campania}/{action}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Productos",
                    action = "Index",
                    campania = UrlParameter.Optional
                } // Parameter defaults
            );

            #endregion

            #region ProductoPerfil

            routes.MapRoute(
                name: "ProductoPerfil", // Route name
                url: "{culture}/perfil-producto/{action}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "ProductoPerfil",
                    action = "Index",
                    campania = UrlParameter.Optional
                } // Parameter defaults
            );

            routes.MapRoute(
                name: "ProductoPerfilUS", // Route name
                url: "{culture}/product-profile/{action}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "ProductoPerfil",
                    action = "Index",
                    campania = UrlParameter.Optional
                } // Parameter defaults
            );

            routes.MapRoute(
                name: "PerfilSearch", // Route name
                url: "{culture}/{pais}/importaciones-y-exportaciones/{uri}/{codPartida}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    pais = "pe",
                    controller = "ProductoPerfil",
                    action = "Index"
                } // Parameter defaults
            );
            routes.MapRoute(
                name: "PerfilSearchEN", // Route name
                url: "{culture}/{pais}/imports-and-exports/{uri}/{codPartida}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    pais = "pe",
                    controller = "ProductoPerfil",
                    action = "Index"
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
                name: "Auth2", // Route name
                url: "ajax/auth2", // URL with parameters
                defaults: new
                {
                    controller = "Common",
                    action = "Login2"
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

            #region AdminFPC
            routes.MapRoute(
                name: "AdminFPC", // Route name
                url: "fpc/admin", // URL with parameters
                defaults: new
                {
                    controller = "AdminFpc",
                    action = "Index"
                } // Parameter defaults
            );
            routes.MapRoute(
                name: "FpcAjax1", // Route name
                url: "fpc/GetCompanyByPos", // URL with parameters
                defaults: new
                {
                    controller = "AdminFpc",
                    action = "GetCompanyByPos"
                } // Parameter defaults
            );
            routes.MapRoute(
                name: "DoVisitaUrlCompanyEs", // Route name
                url: "fpc/DoVisitaUrlCompanyEs", // URL with parameters
                defaults: new
                {
                    controller = "AdminFpc",
                    action = "DoVisitaUrlCompanyEs"
                } // Parameter defaults
            );
            routes.MapRoute(
               name: "DoVisitaUrlCompanyEn", // Route name
               url: "fpc/DoVisitaUrlCompanyEn", // URL with parameters
               defaults: new
               {
                   controller = "AdminFpc",
                   action = "DoVisitaUrlCompanyEn"
               } // Parameter defaults
           );
            routes.MapRoute(
                name: "RemoveCacheCompanyProfile", // Route name
                url: "fpc/RemoveCacheCompanyProfile", // URL with parameters
                defaults: new
                {
                    controller = "AdminFpc",
                    action = "RemoveCacheCompanyProfile"
                } // Parameter defaults
            );

            #region FPC ProductProfile
            routes.MapRoute(
                name: "FpcAjax11", // Route name
                url: "fpc/GetProductByPos", // URL with parameters
                defaults: new
                {
                    controller = "AdminFpc",
                    action = "GetProductByPos"
                } // Parameter defaults
            );
            routes.MapRoute(
                name: "DoVisitaUrlProductEs", // Route name
                url: "fpc/DoVisitaUrlProductEs", // URL with parameters
                defaults: new
                {
                    controller = "AdminFpc",
                    action = "DoVisitaUrlProductEs"
                } // Parameter defaults
            );
            routes.MapRoute(
                name: "DoVisitaUrlProductEn", // Route name
                url: "fpc/DoVisitaUrlProductEn", // URL with parameters
                defaults: new
                {
                    controller = "AdminFpc",
                    action = "DoVisitaUrlProductEn"
                } // Parameter defaults
            );
            routes.MapRoute(
                name: "RemoveCacheProductProfile", // Route name
                url: "fpc/RemoveCacheProductProfile", // URL with parameters
                defaults: new
                {
                    controller = "AdminFpc",
                    action = "RemoveCacheProductProfile"
                } // Parameter defaults
            );

            routes.MapRoute(
                name: "GeneraAndVisitaGC", // Route name
                url: "fpc/GeneraAndVisitaGC", // URL with parameters
                defaults: new
                {
                    controller = "AdminFpc",
                    action = "GeneraAndVisitaGC"
                } // Parameter defaults
            );

            routes.MapRoute(
                name: "RemoveCacheContenidoGeneral", // Route name
                url: "fpc/RemoveCacheContenidoGeneral", // URL with parameters
                defaults: new
                {
                    controller = "AdminFpc",
                    action = "RemoveCacheContenidoGeneral"
                } // Parameter defaults
            );

            routes.MapRoute(
                name: "UrlOk", // Route name
                url: "fpc/UrlOk", // URL with parameters
                defaults: new
                {
                    controller = "AdminFpc",
                    action = "UrlOk"
                } // Parameter defaults
            );

            #endregion



            #endregion

            #region LoginAlerts

            routes.MapRoute(
                name: "LoginAlerts", // Route name
                url: "{culture}/login-alerts/{action}", // URL with parameters
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "LoginAlerts",
                    action = "Index"
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

            // Ruben 202212
            routes.MapRoute(
                name: "ExMinisite",
                url: "{culture}/importaciones-exportaciones-{pais}/{slug}/{trib}-{ruc}",
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Home",
                    action = "Redirect"
                } // Parameter defaults
            ).RouteHandler = new HyphenatedRouteHandler();

            // Ruben 202212
            routes.MapRoute(
                name: "ExMinisiteUS",
                url: "{culture}/imports-exports-{pais}/{slug}/{trib}-{ruc}",
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Home",
                    action = "Redirect"
                } // Parameter defaults
            ).RouteHandler = new HyphenatedRouteHandler();

            // Ruben 202212
            routes.MapRoute(
                name: "ExMinisiteAction",
                url: "{culture}/importaciones-exportaciones-{pais}/{slug}/{trib}-{ruc}/{action}",
                defaults: new
                {
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Home",
                    action = "Redirect"
                } // Parameter defaults
            ).RouteHandler = new HyphenatedRouteHandler();

            // Ruben 202311
            routes.MapRoute(
                name: "Minisite2",
                url: "es/{pais}/importaciones-y-exportaciones-importaciones-y-exportaciones-{slug}/{trib}-{ruc}",
                defaults: new
                {
                    culture = "aa",
                    pais = "peru",
                    controller = "Minisite",
                    action = "Index"
                }
            ).RouteHandler = new HyphenatedRouteHandler();

            // Ruben 202211
            // https://www.veritradecorp.com/es/importaciones-exportaciones-peru/alicorp/ruc-20100055237
            routes.MapRoute(
                name: "Minisite", // Route name
                //url: "{culture}/importaciones-exportaciones-{pais}/{slug}/{trib}-{ruc}", // Ruben 202211
                url: "{culture}/{pais}/importaciones-y-exportaciones-{slug}/{trib}-{ruc}", // Ruben 202304
                //url: "{culture}/{pais}/{slug}/{trib}-{ruc}", // Ruben 202209
                //url: "{culture}/{pais}/importaciones-y-exportaciones-{slug}/{trib}-{ruc}", // URL with parameters                
                defaults: new
                {
                    //culture = "es", // Ruben 202304
                    culture = CultureHelper.GetDefaultCulture(),
                    pais = "pe",
                    controller = "Minisite",
                    action = "Index"
                } // Parameter defaults
            ).RouteHandler = new HyphenatedRouteHandler();            

            // Ruben 202211
            // https://www.veritradecorp.com/en/imports-exports-uruguay/macro-seller/rut-21746072001
            routes.MapRoute(
                name: "MinisiteUS", // Route name
                //url: "{culture}/imports-exports-{pais}/{slug}/{trib}-{ruc}", // Ruben 202211
                url: "{culture}/{pais}/imports-and-exports-{slug}/{trib}-{ruc}", // Ruben 202304
                //url: "{culture}/{pais}/{slug}/{trib}-{ruc}", // Ruben 202209
                //url: "{culture}/{pais}/imports-and-exports-{slug}/{trib}-{ruc}", // URL with parameters
                defaults: new
                {
                    //culture = "en", // Ruben 202304
                    culture = CultureHelper.GetDefaultCulture(),
                    pais = "pe",
                    controller = "Minisite",
                    action = "Index"
                } // Parameter defaults
            ).RouteHandler = new HyphenatedRouteHandler();            

            // Ruben 202304
            routes.MapRoute(
                name: "MinisiteAction", // Route name
                url: "{culture}/{pais}/importaciones-y-exportaciones-{slug}/{trib}-{ruc}/{action}",
                defaults: new
                {
                    //culture = "es", // Ruben 202304
                    culture = CultureHelper.GetDefaultCulture(),
                    pais = "pe",
                    controller = "Minisite",
                    action = "ChartExpMinisite"
                } // Parameter defaults
            ).RouteHandler = new HyphenatedRouteHandler();

            // Ruben 202304
            routes.MapRoute(
                name: "MinisiteActionUS", // Route name
                url: "{culture}/{pais}/imports-and-exports-{slug}/{trib}-{ruc}/{action}",
                defaults: new
                {
                    //culture = "en", // Ruben 202304
                    culture = CultureHelper.GetDefaultCulture(),
                    pais = "pe",
                    controller = "Minisite",
                    action = "ChartExpMinisite"
                } // Parameter defaults
            ).RouteHandler = new HyphenatedRouteHandler();

            #endregion
        }
    }

    public class HyphenatedRouteHandler : MvcRouteHandler
    {
        protected override IHttpHandler GetHttpHandler(RequestContext requestContext)
        {   
            string trib = requestContext.RouteData.Values["trib"].ToString();
            string ruc = requestContext.RouteData.Values["ruc"].ToString();
            string _trib = trib.GetUntilOrEmpty("-");


            if (!_trib.IsNullOrWhiteSpace())
            {
                requestContext.RouteData.Values["trib"] = _trib;
                requestContext.RouteData.Values["ruc"] = trib.Substring(trib.IndexOf("-", StringComparison.Ordinal) + 1) + "-"+  ruc;
            }
            return base.GetHttpHandler(requestContext);
        }
    }
}