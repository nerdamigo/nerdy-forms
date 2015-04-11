using System;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Moq;
using System.Diagnostics;

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
		public void DynamicFormObjectNotNull()
		{
			var tMock = new Mock<INerdyFormHandler>();
			
			NerdyFormController tController = new NerdyFormController(new List<INerdyFormHandler>() { tMock.Object });
			var tContextMocks = new ContextMocks(tController);

			tController.Handle("Test");
	
			tMock.Verify(h => h.Handle(It.IsNotNull<object>()));
		}

		[TestMethod]
		public void DynamicFormObjectContainsMetadata()
		{
			var tMock = new Mock<INerdyFormHandler>();
			NerdyFormController tController = new NerdyFormController(new List<INerdyFormHandler>() { tMock.Object });
			var tContextMocks = new ContextMocks(tController);
			tContextMocks.RequestHeaders.Add("TestHeader", "TestValue");

			dynamic tData = null;
			tMock.Setup(handler => handler.Handle(It.IsAny<object>())).Callback<dynamic>(data =>
			{
				tData = data;
			});

			tController.Handle("Test");

			tMock.Verify(handler => handler.Handle(It.IsAny<object>()));

			Assert.IsNotNull(tData._Metadata, "_Metadata missing");
			Assert.IsNotNull(tData._Metadata.Request, "Metadata.Request was null");
			Assert.IsNotNull(tData._Metadata.Request.Headers["TestHeader"], "Metadata.Request.Headers[\"TestHeader\"] was null");
		}
	}
}
