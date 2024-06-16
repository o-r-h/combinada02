using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Veritrade2018.Helpers;

namespace Veritrade2018.App_Start
{
    public class SessionExpireFilterAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;
            var isDemo = Extensiones.GetCookie("IsDemo") == "1";
            // check  sessions here
            if (ctx.Session["IdUsuario"] == null && ctx.Session["IdUsuarioFree"] == null && !isDemo )
            {
                //ActiveSessions.Sessions.TryRemove(ctx.Session.SessionID, out _);

                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.HttpContext.Response.ClearContent();
                    filterContext.HttpContext.Items["AjaxPermissionDenied"] = true;
                }
                //// check if a new session id was generated  
                //else
                //{
                //    filterContext.Result = new RedirectResult("~/Home/Index");
                //}

                filterContext.Result = new RedirectResult("~/Home/Index");
                return;
            } else
            {

                //Console.WriteLine("URL>>"+ filterContext.HttpContext.Request.Url);

                var IsMb = Extensiones.GetCookie("Mb") == "1";
                if (isDemo && ctx.Session["IdUsuario"] == null && !IsMb) 
                {
                    var IdUsuario = Funciones.GetIdUserFreeTrial();
                    if (!string.IsNullOrEmpty(IdUsuario))
                    {
                        ctx.Session["IdUsuario"] = IdUsuario;
                        ctx.Session["TipoUsuario"] = Funciones.BuscaTipoUsuario(IdUsuario);
                        ctx.Session["Plan"] = Funciones.ObtienePlan(IdUsuario).ToUpper();
                        ctx.Session["Origen"] = Funciones.ObtieneOrigen(IdUsuario).ToUpper();
                    }
                }
            }
            base.OnActionExecuting(filterContext);
        }

        //protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        //{
        //    HttpContext ctx = HttpContext.Current;
        //    // check if session is supported  
        //    if (ctx.Session != null)
        //    {
        //        // check if a new session id was generated  
        //        if (HttpContext.Current.Session["IdUsuario"] == null)
        //        {

        //            //Check is Ajax request  
        //            if (filterContext.HttpContext.Request.IsAjaxRequest())
        //            {
        //                filterContext.HttpContext.Response.ClearContent();
        //                filterContext.HttpContext.Items["AjaxPermissionDenied"] = true;
        //            }
        //            // check if a new session id was generated  
        //            else
        //            {
        //                filterContext.Result = new RedirectResult("~/Home/Index");
        //            }
        //            base.HandleUnauthorizedRequest(filterContext);
        //        }
        //    }
            
        //}
    }
}