using DevTrends.MvcDonutCaching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;

namespace Veritrade2017.Helpers
{
    public class ChildActionOutputCacheAttribute : OutputCacheAttribute
    {
        private readonly object lockObj = new object();
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.IsChildAction && !string.IsNullOrWhiteSpace(CacheProfile))
            {
                lock (lockObj)
                {
                    if (!string.IsNullOrWhiteSpace(CacheProfile))
                    {
                        // OutputCacheAttribute for child actions only supports
                        // Duration, VaryByCustom, and VaryByParam values.
                        var outputCache = (OutputCacheSettingsSection)WebConfigurationManager.GetSection("system.web/caching/outputCacheSettings");
                        var profile = outputCache.OutputCacheProfiles[CacheProfile];
                        if (profile.Enabled)
                        {
                            Duration = profile.Duration > 0 ? profile.Duration : Duration;
                            VaryByCustom = string.IsNullOrWhiteSpace(profile.VaryByCustom)
                                ? VaryByCustom : profile.VaryByCustom;
                            VaryByParam = string.IsNullOrWhiteSpace(profile.VaryByParam)
                                ? VaryByParam : profile.VaryByParam;
                        }
                        CacheProfile = null;
                    }
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }

    public class FpcBasicAuthenticationAttribute : ActionFilterAttribute
    {
        public string BasicRealm { get; set; }
        protected string Username { get; set; }
        protected string Password { get; set; }

        public FpcBasicAuthenticationAttribute(string username, string password)
        {
            this.Username = Properties.Settings.Default.Credencial_User.ToString();
            this.Password = Properties.Settings.Default.Credencial_Pass.ToString();
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
            filterContext.Result = new EmptyResult();
            var res = filterContext.HttpContext.Response;
            res.StatusCode = 401;
            res.AddHeader("WWW-Authenticate", String.Format("Basic realm=\"{0}\"", BasicRealm ?? "Ryadel"));
            res.End();
        }
    }

    public class AuthorizedCultureContact : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)

        {
            string url = filterContext.HttpContext.Request.Url.AbsoluteUri;
            int posIni = url.IndexOf("es");
            if (posIni == -1)
            {
                posIni = url.IndexOf("en");
            }                
            string culture = url.Substring(posIni, 2);
            posIni += 3;
            string urlName = url.Substring(posIni, url.Length - posIni - 1);
            if ((urlName == "contacto_landing" && culture == "en") || (urlName == "contact_landing" && culture == "es"))
            {
                var routeDictionary = new RouteValueDictionary { { "action", "Index" }, { "controller", "Home" } };
                filterContext.Result = new RedirectToRouteResult(routeDictionary);
            }


        }
    }
}