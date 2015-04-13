using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nerdamigo.NerdyForms
{
	public class NerdyFormKeyDecodeException : Exception
	{
		public NerdyFormKeyDecodeException(string aMessage) :
			base(aMessage)
		{

		}

		public NerdyFormKeyDecodeException(string aMessage, Exception aInnerException) :
			base(aMessage, aInnerException)
		{

		}
	}
}
