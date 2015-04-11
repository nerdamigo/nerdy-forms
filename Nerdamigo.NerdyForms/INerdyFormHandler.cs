using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nerdamigo.NerdyForms
{
	public interface INerdyFormHandler
	{
		void Handle(dynamic aFormData);
	}
}
