using System.Web.Mvc;
using System.Web.Routing;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Health.Plumbing;

namespace Health
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        private static IWindsorContainer _container;

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "About", // Route name
                "Health/About", // URL with parameters
                new { controller = "Health", action = "About", id = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                "Probe", // Route name
                "Health/New", // URL with parameters
                new { controller = "Health", action = "New", id = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                "Home", // Route name
                "Health", // URL with parameters
                new { controller = "Health", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                "MachineHealth", // Route name
                "Health/{envname}/{machinename}", // URL with parameters
                new { controller = "Health", action = "Machine" } // Parameter defaults
            );

            routes.MapRoute(
                "EnvHealth", // Route name
                "Health/{envname}", // URL with parameters
                new { controller = "Health", action = "Environment" } // Parameter defaults
            );


            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Health", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            BootstrapContainer();

        }

        protected void Application_End()
        {
            _container.Dispose();
        }

        private static void BootstrapContainer()
        {
            _container = new WindsorContainer();
            _container.Install(FromAssembly.This());
            var controllerFactory = new WindsorControllerFactory(_container.Kernel);
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);
        }
    }
}