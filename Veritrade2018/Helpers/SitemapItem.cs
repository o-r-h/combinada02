namespace Veritrade2018.Helpers
{
    public class SitemapItem : ISitemapItem
    {
        public SitemapItem(string url, string hreflang)
        {
            Url = url;
            Hreflang = hreflang;
            Rel = "alternate";
        }

        public string Url { get; set; }

        public string Rel { get; set; }

        public string Hreflang { get; set; }

        public string Href { get; set; }
    }
}