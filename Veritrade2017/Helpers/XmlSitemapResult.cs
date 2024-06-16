using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Veritrade2017.Helpers
{
    public class XmlSitemapResult : ActionResult
    {
        private SortedList<string, List<ISitemapItem>> _items;
        readonly XNamespace xhtml = "http://www.w3.org/1999/xhtml";
        readonly XNamespace sitemap = "http://www.sitemaps.org/schemas/sitemap/0.9";

        public XmlSitemapResult(SortedList<string, List<ISitemapItem>> items)
        {
            _items = items;
        }

        // Ruben 202209
        public override void ExecuteResult(ControllerContext context)
        {
            // Ruben 202209
            /*
            string encoding = context.HttpContext.Response.ContentEncoding.WebName;
            string xml = "";

            XElement urlSetElement;
            XDocument sitemapElement = null;
            for (int i = 0; i < _items.Count; i++)
            {
                 urlSetElement = new XElement(sitemap + "urlset",
                    new XAttribute("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9"),
                    new XAttribute(XNamespace.Xmlns + "xhtml", xhtml.NamespaceName),
                    from item in _items.Values[i]
                    select CreateItemElement(item)
                );

                 sitemapElement = new XDocument(new XDeclaration("1.0", encoding, "no"), urlSetElement);
                
                string path = HttpContext.Current.Server.MapPath("..");
                string uri = path + "/" + _items.Keys[i] + ".xml";
                sitemapElement.Save(uri);
            }
            xml = sitemapElement.ToString();
            context.HttpContext.Response.ContentType = "application/xml";
            context.HttpContext.Response.Flush();
            context.HttpContext.Response.Write(xml);
            */
        }

        private XElement CreateItemElement(ISitemapItem item)
        {
            XElement urlElement = new XElement(sitemap + "url", new XElement(sitemap + "loc", item.Url.ToLower()));

            XElement linkElement = new XElement(xhtml + "link",
                new XAttribute("rel", "alternate"),
                new XAttribute("hreflang", item.Hreflang.ToLower()),
                new XAttribute("href", item.Href.ToLower())
            );
            urlElement.Add(linkElement);
            return urlElement;
        }
    }
}