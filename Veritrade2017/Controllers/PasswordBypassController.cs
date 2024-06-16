using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Veritrade2017.Models;

namespace Veritrade2017.Controllers
{
	public class PasswordBypassController : BaseController
	{

		[HttpGet]
		public ActionResult Index() {

			return View("Index",null,null);
		}


		[HttpPost]
		public ActionResult Index(FakeCredentials fakeCredentials)
		{
		    Helpers.Variables.DevelopInitialtime = DateTime.Now;
			return RedirectToRoute("Root", new { culture = "es" });
			//return RedirectToRoute("Root");

		}

	}

	
}