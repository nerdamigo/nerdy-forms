using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Nerdamigo.NerdyForms.Tests
{
	[TestClass]
	public class NerdyFormTests
	{
		[TestMethod]
		public void EmptyHandlerListThrowsConstructionException()
		{
		}

		[TestMethod]
		public void ListContainingNullHandlerThrowsConstructionException()
		{
		}
	}
}
