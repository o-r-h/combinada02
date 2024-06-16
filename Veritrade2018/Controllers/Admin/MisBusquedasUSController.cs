using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Veritrade2018.Helpers;

namespace Veritrade2018.Controllers.Admin
{
    public class MisBusquedasUSController : BaseController
    {
        // GET: MisBusquedasUS
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SetCulture(string culture)
        {
            culture = CultureHelper.GetImplementedCulture(culture);
            RouteData.Values["culture"] = culture; // set culture 

            return RedirectToAction("Index");
        }
    }
}