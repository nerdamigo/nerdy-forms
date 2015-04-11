using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Nerdamigo.NerdyForms.Tests
{
	public class ContextMocks
	{
		public Moq.Mock<HttpContextBase> HttpContext { get; set; }
		public Moq.Mock<HttpRequestBase> Request { get; set; }
		public NameValueCollection RequestHeaders { get; private set; }
		public RouteData RouteData { get; set; }

		public ContextMocks(Controller controller)
		{
			//define context objects
			HttpContext = new Moq.Mock<HttpContextBase>();
			Request = new Moq.Mock<HttpRequestBase>();
			RequestHeaders = new NameValueCollection();

			HttpContext.Setup(x => x.Request).Returns(Request.Object);
			HttpContext.Setup(x => x.Request.Headers).Returns(RequestHeaders);

			//setup Response, Session, etc similarly with either mocks or fakes

			//apply context to controller
			RequestContext rc = new RequestContext(HttpContext.Object, new RouteData());
			controller.ControllerContext = new ControllerContext(rc, controller);
		}
	}
}
