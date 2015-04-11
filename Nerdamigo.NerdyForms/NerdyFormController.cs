using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Nerdamigo.NerdyForms
{
	public class NerdyFormController : Controller
	{
		private List<INerdyFormHandler> mFormHandlerList;
		public NerdyFormController(List<INerdyFormHandler> aFormHandlerList)
		{
			if (aFormHandlerList == null || aFormHandlerList.Count == 0 || aFormHandlerList.Any(a => a == null))
			{
				throw new ArgumentException("aFormHandlerList");
			}

			mFormHandlerList = aFormHandlerList;
		}

		public virtual ActionResult Handle(string FormName)
		{
			if (String.IsNullOrWhiteSpace(FormName))
			{
				throw new ArgumentException("Form Name is Required");
			}

			var s = Request.Form["test"];
			Console.Write(s);

			return new EmptyResult();
		}
	}
}
