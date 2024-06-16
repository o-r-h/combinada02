using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Veritrade2018.Util;

namespace Veritrade2018.Helpers
{

    public class AuthorizedAlertsAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var url = filterContext.HttpContext.Request.Url.AbsoluteUri;

            int pos = url.IndexOf("?");
            Dictionary<string, string> queryValues = Extensiones.GetQueryValues(url);
            if (pos != -1 && filterContext.HttpContext.Session["IdUsuario"] == null)
            {
                filterContext.HttpContext.Session["url"] = url.Substring(pos, url.Length - pos);
            }
            else if(pos != -1)
            {
                filterContext.HttpContext.Session["ruta"] = url.Substring(pos, url.Length - pos);
            }
            filterContext.HttpContext.Session["queryValues"] = queryValues;

            

            if (filterContext.HttpContext.Session["IdUsuario"] == null)
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        { "controller", "Common" },
                        { "action", "Logout" }
                    });

            /*string culture = filterContext.RouteData.Values["culture"].ToString();
            string idUsuario = filterContext.HttpContext.Session["IdUsuario"] as string;
            string IdPlan = "";
            if (!string.IsNullOrEmpty(idUsuario))
                IdPlan = Funciones.ObtieneIdPlan(idUsuario);




            switch (filterContext.RouteData.Values["controller"].ToString().ToLower())
            {
                case "misproductos":
                    if ((bool)(filterContext.HttpContext.Session["opcionFreeTrial"] ?? false) || !Funciones.ObtieneFlagPlan(IdPlan, "FlagMisProductos"))
                        filterContext.Result = new RedirectResult(string.Format("/{0}/mis-busquedas?f=mp", culture));

                    break;
                case "miscompanias":
                    if ((bool)(filterContext.HttpContext.Session["opcionFreeTrial"] ?? false) || !Funciones.ObtieneFlagPlan(IdPlan, "FlagMisCompañias"))
                        filterContext.Result = new RedirectResult(string.Format("/{0}/mis-busquedas?f=mc", culture));

                    break;

            }*/

        }
    }

    public class AuthorizedPlanAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {  
            string culture = filterContext.RouteData.Values["culture"].ToString();
            string idUsuario = filterContext.HttpContext.Session["IdUsuario"] as string;
            string IdPlan = "";
            if (!string.IsNullOrEmpty(idUsuario))
             IdPlan = Funciones.ObtieneIdPlan(idUsuario);

            Dictionary<string, string> query = (Dictionary<string, string>)filterContext.HttpContext.Session["queryValues"];
            var bandera = query.ContainsKey("acc");
            

            switch (filterContext.RouteData.Values["controller"].ToString().ToLower())
            {
                case "misproductos":
                    if (!(bool)(filterContext.HttpContext.Session["opcionFreeTrial"] ?? false) && !Funciones.ObtieneFlagPlan(IdPlan, "FlagMisProductos"))
                    {
                        if (bandera)
                        {
                            filterContext.Result = new RedirectResult(string.Format("/{0}/mis-busquedas"+ filterContext.HttpContext.Session["ruta"], culture));
                        }
                        else
                        {
                            filterContext.Result = new RedirectResult(string.Format("/{0}/mis-busquedas?f=mp", culture));
                        }
                        
                    }
                    break;
                case "miscompanias":
                    if (!(bool)(filterContext.HttpContext.Session["opcionFreeTrial"] ?? false) && !Funciones.ObtieneFlagPlan(IdPlan, "FlagMisCompañias"))
                    {
                        if (bandera)
                        {
                            filterContext.Result = new RedirectResult(string.Format("/{0}/mis-busquedas" + filterContext.HttpContext.Session["ruta"], culture));
                        }
                        else
                        {
                            filterContext.Result = new RedirectResult(string.Format("/{0}/mis-busquedas?f=mc", culture));
                        }                        
                    }
                    break;

            }

        }
    }

    public class AuthorizedNoReferidoAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if ((filterContext.HttpContext.Session["Plan"] as string) == "UNIVERSIDADES")
            {
                string Origen = Funciones.ObtieneOrigen(filterContext.HttpContext.Session["IdUsuario"] as string).ToUpper();

                if(Origen != "DELOITTE" && Origen != "ITP")
                    filterContext.Result = new RedirectResult("~/Home/Index");
            }
                


        }
    }

    public class SuperAdminAuthenticationAttribute : ActionFilterAttribute
    {
        public string BasicRealm { get; set; }
        protected string Username { get; set; }
        protected string Password { get; set; }

        public SuperAdminAuthenticationAttribute(string username, string password)
        {

            this.Username = VarGeneral.Instance.ValuesDict[Enums.VarId.CREDENCIAL_USER.GetDn()].Valores;
            this.Password = VarGeneral.Instance.ValuesDict[Enums.VarId.CREDENCIAL_PASSWORD.GetDn()].Valores;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var req = filterContext.HttpContext.Request;
            var auth = req.Headers["Authorization"];
            if (!String.IsNullOrEmpty(auth))
            {
                var cred = System.Text.ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(auth.Substring(6))).Split(':');
                var user = new { Name = cred[0], Pass = cred[1] };
                if (user.Name == Username && user.Pass == Password) return;
            }
            var res = filterContext.HttpContext.Response;
            res.StatusCode = 401;
            res.AddHeader("WWW-Authenticate", String.Format("Basic realm=\"{0}\"", BasicRealm ?? "Ryadel"));
            res.End();
        }
    }
}