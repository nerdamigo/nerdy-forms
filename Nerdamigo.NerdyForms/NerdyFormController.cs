using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Dynamic;

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

			List<Exception> tEncounteredExceptions = new List<Exception>();
			foreach (INerdyFormHandler aHandler in mFormHandlerList)
			{
				try
				{
					dynamic tData = new NerdyFormDynamic();
					tData._Metadata = new NerdyFormDynamic();
					tData._Metadata.Request = new NerdyFormDynamic();
					tData._Metadata.Request.RawUrl = Request.RawUrl;
					tData._Metadata.Request.Headers = new Dictionary<string, string>();

					foreach (var iHeaderKey in Request.Headers.AllKeys)
					{
						tData._Metadata.Request.Headers.Add(iHeaderKey, Request.Headers[iHeaderKey]);
					}

					aHandler.Handle(tData);
				}
				catch(Exception ex)
				{
					tEncounteredExceptions.Add(ex);
				}
			}

			if (tEncounteredExceptions.Count > 0)
			{
				throw new AggregateException("One or more form handling exceptions encountered", tEncounteredExceptions);
			}

			return new EmptyResult();
		}
	}
}
