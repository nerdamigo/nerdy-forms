using System;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Nerdamigo.NerdyForms.Tests
{
	[TestClass]
	public class NerdyFormTests
	{
		[TestMethod]
		[ExpectedException(typeof(ArgumentException), "A Null form handler list was allowed.")]
		public void NullHandlerListThrowsConstructionException()
		{
			NerdyFormController tController = new NerdyFormController(null);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException), "An empty form handler list was allowed.")]
		public void EmptyHandlerListThrowsConstructionException()
		{
			NerdyFormController tController = new NerdyFormController(new List<INerdyFormHandler>());
		}
		

		[TestMethod]
		[ExpectedException(typeof(ArgumentException), "An form handler list containing a null handler was allowed.")]
		public void ListContainingNullHandlerThrowsConstructionException()
		{
			NerdyFormController tController = new NerdyFormController(new List<INerdyFormHandler>() { (INerdyFormHandler)null });
		}

		[TestMethod]
		public void DynamicFormObjectConstructed()
		{
			var tRequest = new HttpRequest(null, "http://nerdyforms.nerdamigo.com", null);
			HttpContext.Current = new HttpContext(tRequest, new HttpResponse(null));
		}
	}
}
