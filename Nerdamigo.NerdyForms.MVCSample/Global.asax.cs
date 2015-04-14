using Nerdamigo.NerdyForms.MVCSample.Code;
using SimpleInjector;
using SimpleInjector.Integration.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Nerdamigo.NerdyForms.MVCSample
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
			Container tContainer = new Container();

			tContainer.Register<INerdyFormHandler, InMemoryNerdyFormHandler>();

			tContainer.RegisterMvcControllers(Assembly.GetExecutingAssembly());
			tContainer.RegisterMvcIntegratedFilterProvider();

			tContainer.Verify();

			DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(tContainer));

            GlobalFilters.Filters.Add(new HandleErrorAttribute());

			RouteTable.Routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			RouteTable.Routes.MapRoute(
				name: "NerdyForms",
				url: "Form/{FormName}",
				defaults: new { controller = "NerdyForm", action = "Handle" }
			);

			RouteTable.Routes.MapRoute(
				name: "NerdyForms_Derived",
				url: "DerivedForm/{FormName}",
				defaults: new { controller = "DerivedForm", action = "Handle" }
			);

			RouteTable.Routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
			);
        }
    }
}
