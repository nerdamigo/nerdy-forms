using Nerdamigo.NerdyForms.MVCSample.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nerdamigo.NerdyForms.MVCSample.Controllers
{
	public class DerivedFormController : NerdyForms.NerdyFormController
	{
		public DerivedFormController()
			: base(new InMemoryNerdyFormHandler())
		{

		}

		protected override dynamic ProcessFormData()
		{
			var tBaseData = base.ProcessFormData();

			//modify the processed incoming data as you see fit
			tBaseData.RandomNumber = (new Random()).Next(int.MaxValue);

			//before returning it to the base controller for handling
			return tBaseData;
		}
	}
}