using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Veritrade2017.Helpers
{
    public interface ISitemapItem
    {
        string Url { get; }
        string Rel { get; set; }
        string Hreflang { get; set; }
        string Href { get; set; }
    }
}