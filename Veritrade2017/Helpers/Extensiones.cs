using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Veritrade2017.Helpers
{
    public static class Extensiones
    {
        public static T GetValue<T>(this DataRow row, string columnName)
        {

            int index = -1;
            if (row != null) index = row.Table.Columns.IndexOf(columnName);

            return (index < 0 || index > row.ItemArray.Count() || row[index] == DBNull.Value)
                ? default(T)
                //: (T)row[index];
                : (T)Convert.ChangeType(row[index], typeof(T));
        }

        public static string GetUntilOrEmpty(this string text, string stopAt = "-")
        {
            if (!String.IsNullOrWhiteSpace(text))
            {
                int charLocation = text.IndexOf(stopAt, StringComparison.Ordinal);

                if (charLocation > 0)
                {
                    return text.Substring(0, charLocation);
                }
            }

            return String.Empty;
        }

        public static Route MapRouteWithName(this RouteCollection routes,
            string name, string url, object defaults, object constraints)
        {
            Route route = routes.MapRoute(name, url, defaults, constraints);
            route.DataTokens = new RouteValueDictionary();
            route.DataTokens.Add("RouteName", name);

            return route;
        }

        public static async Task<bool> UrlIsReachable(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(url);
                    return response.StatusCode == HttpStatusCode.OK;
                }
            }
            catch
            {
                return false;
            }
        }

        public static string GetDn(this Enum enumType)
        {
            return enumType.GetType().GetMember(enumType.ToString())
                           .First()
                           .GetCustomAttribute<DisplayAttribute>()
                           .Name;
        }

        public static void SetCookie(string key, string value, TimeSpan expires)
        {
            HttpCookie encodedCookie = /*HttpSecureCookie.Encode(*/ new HttpCookie(key, value)/*)*/;

            if (HttpContext.Current.Request.Cookies[key] != null)
            {
                var cookieOld = HttpContext.Current.Request.Cookies[key];
                //cookieOld.Expires = DateTime.Now.Add(expires);
                cookieOld.Value = encodedCookie.Value;
                HttpContext.Current.Response.Cookies.Add(cookieOld);
            }
            else
            {
                encodedCookie.Expires = DateTime.Now.Add(expires);
                HttpContext.Current.Response.Cookies.Add(encodedCookie);
            }
        }
        public static string GetCookie(string key)
        {
            string value = string.Empty;
            HttpCookie cookie = HttpContext.Current.Request.Cookies[key];

            //if (cookie != null)
            //{
            //    // For security purpose, we need to encrypt the value.
            //    HttpCookie decodedCookie = HttpSecureCookie.Decode(cookie);
            //    value = decodedCookie.Value;
            //}
            if (cookie != null) value = cookie.Value.ToString();
            return value;
        }
    }

    public class RouteDataContext : HttpContextBase
    {
        public override HttpRequestBase Request { get; }

        private RouteDataContext(Uri uri)
        {
            if (uri != null)
            {
                try
                {
                    var url = uri.GetLeftPart(UriPartial.Path);
                    var qs = uri.GetComponents(UriComponents.Query, UriFormat.UriEscaped);

                    Request = new HttpRequestWrapper(new HttpRequest(null, url, qs));
                }
                catch (Exception)
                {


                }
            }
        }

        public static RouteValueDictionary RouteValuesFromUri(Uri uri)
        {
            try
            {
                return RouteTable.Routes.GetRouteData(new RouteDataContext(uri))?.Values;
            }
            catch
            {
                return null;
            }
        }

       

    }
}