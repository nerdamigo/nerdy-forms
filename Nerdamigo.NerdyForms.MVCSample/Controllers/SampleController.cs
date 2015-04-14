using Nerdamigo.NerdyForms.MVCSample.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nerdamigo.NerdyForms.MVCSample.Controllers
{
	public class SampleController : Controller
	{
		public ActionResult Basic()
		{
			ViewBag.RecentSubmissions = InMemoryNerdyFormHandler.GetSubmissions("Basic").Take(5);
			return View();
		}

		public ActionResult Complex()
		{
			ViewBag.RecentSubmissions = InMemoryNerdyFormHandler.GetSubmissions("Complex").Take(5);
			return View();
		}

		public ActionResult Inheritance()
		{
			ViewBag.RecentSubmissions = InMemoryNerdyFormHandler.GetSubmissions("Inheritance").Take(5);
			return View();
		}

		public ActionResult IOC()
		{
			return View();
		}

		public ActionResult MultiHandler()
		{
			return View();
		}
	}
}