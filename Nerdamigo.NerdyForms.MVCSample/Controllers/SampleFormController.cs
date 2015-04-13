using Nerdamigo.NerdyForms.MVCSample.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nerdamigo.NerdyForms.MVCSample.Controllers
{
	public class SampleFormController : NerdyForms.NerdyFormController
	{
		public SampleFormController()
			: base(new InMemoryNerdyFormHandler())
		{

		}
	}
}