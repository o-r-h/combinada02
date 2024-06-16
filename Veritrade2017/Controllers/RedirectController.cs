using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Veritrade2017.Controllers
{
    public class RedirectController : Controller
    {
        // GET: Redirect
        public ActionResult Index(string culture)
        {
            return RedirectPermanent("/");
        }
    }
}