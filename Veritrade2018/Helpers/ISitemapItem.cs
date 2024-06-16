namespace Veritrade2018.Helpers
{
    public interface ISitemapItem
    {
        string Url { get; }
        string Rel { get; set; }
        string Hreflang { get; set; }
        string Href { get; set; }
    }
}