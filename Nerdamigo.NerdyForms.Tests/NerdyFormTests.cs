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
			Assert.IsTrue(tData._Metadata.Request.Headers.ContainsKey("TestHeader"), "Metadata.Request.Headers[\"TestHeader\"] was null");
		}

		[TestMethod]
		public void DynamicFormObjectContainsFormData()
		{
			var tMock = new Mock<INerdyFormHandler>();
			NerdyFormController tController = new NerdyFormController(new List<INerdyFormHandler>() { tMock.Object });
			var tContextMocks = new ContextMocks(tController);
			tContextMocks.RequestForm.Add("EmailAddress", "test@test.com");
			tContextMocks.RequestForm.Add("FirstName", "Bob");
			tContextMocks.RequestForm.Add("LastName", "McTaggart");

			dynamic tData = null;
			tMock.Setup(handler => handler.Handle(It.IsAny<object>())).Callback<dynamic>(data =>
			{
				tData = data;
			});

			tController.Handle("Test");

			tMock.Verify(handler => handler.Handle(It.IsAny<object>()));

			Assert.IsNotNull(tData.EmailAddress);
			Assert.IsNotNull(tData.FirstName);
			Assert.IsNotNull(tData.LastName);
		}

		[TestMethod]
		public void DynamicFormObjectSubObject()
		{
			var tMock = new Mock<INerdyFormHandler>();
			NerdyFormController tController = new NerdyFormController(new List<INerdyFormHandler>() { tMock.Object });
			var tContextMocks = new ContextMocks(tController);
			tContextMocks.RequestForm.Add("Person.EmailAddress", "test@test.com");
			tContextMocks.RequestForm.Add("Person.FirstName", "Bob");
			tContextMocks.RequestForm.Add("Person.LastName", "McTaggart");
			tContextMocks.RequestForm.Add("Person2[0].EmailAddress", "test@test.com");
			tContextMocks.RequestForm.Add("Person2[0].FirstName", "Bob");
			tContextMocks.RequestForm.Add("Person2[0].LastName", "McTaggart");
			tContextMocks.RequestForm.Add("Person2[1].EmailAddress", "test2@test.com");
			tContextMocks.RequestForm.Add("Person2[1].FirstName", "Bob2");
			tContextMocks.RequestForm.Add("Person2[1].LastName", "McTaggart2");
			tContextMocks.RequestForm.Add("Person2[2].EmailAddress", "test3@test.com");
			tContextMocks.RequestForm.Add("Person2[2].FirstName", "Bob3");
			tContextMocks.RequestForm.Add("Person2[2].LastName", "McTaggart3");
			tContextMocks.RequestForm.Add("two_d[0][0]", "00");
			tContextMocks.RequestForm.Add("two_d[0][1]", "01");
			tContextMocks.RequestForm.Add("two_d[1][0]", "10");
			tContextMocks.RequestForm.Add("two_d[1][1]", "11");
			tContextMocks.RequestForm.Add("ride[0]", "goesaway");
			tContextMocks.RequestForm.Add("ride[0].property", "overidden!");

			dynamic tData = null;
			tMock.Setup(handler => handler.Handle(It.IsAny<object>())).Callback<dynamic>(data =>
			{
				tData = data;
			});

			tController.Handle("Test");

			tMock.Verify(handler => handler.Handle(It.IsAny<object>()));

			Assert.IsNotNull(tData.Person, "Person null");
			Assert.AreEqual(tData.Person.EmailAddress, tContextMocks.RequestForm["Person.EmailAddress"], "Person.EmailAddress");
			Assert.AreEqual(tData.Person.FirstName, tContextMocks.RequestForm["Person.FirstName"], "Person.FirstName");
			Assert.AreEqual(tData.Person.LastName, tContextMocks.RequestForm["Person.LastName"], "Person.LastName");

			Assert.IsNotNull(tData.Person2[0], "Person2[0] null");
			Assert.AreEqual(tData.Person2[0].EmailAddress, tContextMocks.RequestForm["Person2[0].EmailAddress"], "Person2[0].EmailAddress");
			Assert.AreEqual(tData.Person2[0].FirstName, tContextMocks.RequestForm["Person2[0].FirstName"], "Person2[0].FirstName");
			Assert.AreEqual(tData.Person2[0].LastName, tContextMocks.RequestForm["Person2[0].LastName"], "Person2[0].LastName");
			Assert.AreEqual(tData.Person2[1].EmailAddress, tContextMocks.RequestForm["Person2[1].EmailAddress"], "Person2[1].EmailAddress");
			Assert.AreEqual(tData.Person2[1].FirstName, tContextMocks.RequestForm["Person2[1].FirstName"], "Person2[1].FirstName");
			Assert.AreEqual(tData.Person2[1].LastName, tContextMocks.RequestForm["Person2[1].LastName"], "Person2[1].LastName");
			Assert.AreEqual(tData.Person2[2].EmailAddress, tContextMocks.RequestForm["Person2[2].EmailAddress"], "Person2[2].EmailAddress");
			Assert.AreEqual(tData.Person2[2].FirstName, tContextMocks.RequestForm["Person2[2].FirstName"], "Person2[2].FirstName");
			Assert.AreEqual(tData.Person2[2].LastName, tContextMocks.RequestForm["Person2[2].LastName"], "Person2[2].LastName");

			Assert.IsNotNull(tData.two_d, "two_d null");
			Assert.IsNotNull(tData.two_d[0], "two_d[0] null");
			Assert.AreEqual(tData.two_d[0][0], tContextMocks.RequestForm["two_d[0][0]"], "two_d[0][0]");
			Assert.AreEqual(tData.two_d[0][1], tContextMocks.RequestForm["two_d[0][1]"], "two_d[0][1]");
			Assert.AreEqual(tData.two_d[1][0], tContextMocks.RequestForm["two_d[1][0]"], "two_d[1][0]");
			Assert.AreEqual(tData.two_d[1][1], tContextMocks.RequestForm["two_d[1][1]"], "two_d[1][1]");

			Assert.AreEqual(tData.ride[0].property, tContextMocks.RequestForm["ride[0].property"], "ride[0].property");
		}
	}
}
