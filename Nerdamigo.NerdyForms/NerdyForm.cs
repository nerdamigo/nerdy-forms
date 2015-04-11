using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Nerdamigo.NerdyForms
{
	public class NerdyForm : Controller
	{
		private List<INerdyFormHandler> mFormHandlerList;
		public NerdyForm(List<INerdyFormHandler> aFormHandlerList)
		{
			mFormHandlerList = aFormHandlerList;
		}

		public virtual ActionResult Handle(string FormName)
		{
			if (String.IsNullOrWhiteSpace(FormName))
			{
				throw new ArgumentException("Form Name is Required");
			}

			

			return new EmptyResult();
		}
	}
}
