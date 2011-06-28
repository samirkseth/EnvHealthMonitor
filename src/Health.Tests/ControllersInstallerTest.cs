using System.Linq;
using System.Web.Mvc;
using Castle.Core.Internal;
using Castle.MicroKernel;
using Castle.Windsor;
using Health.Controllers;
using Health.Plumbing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Health.Tests
{
    [TestClass]
    public class ControllersInstallerTest
    {
        private readonly IWindsorContainer _containerWithControllers;

        public ControllersInstallerTest()
        {
            _containerWithControllers = new WindsorContainer()
                .Install(new ControllersInstaller());
        }

        [TestMethod]
        public void AllControllersImplementIController()
        {
	        var allHandlers = GetAllHandlers(_containerWithControllers);
	        var controllerHandlers = GetHandlersFor(typeof(IController), _containerWithControllers);

	        Assert.IsNotNull(allHandlers);
            CollectionAssert.AreEqual(allHandlers, controllerHandlers);
        }

        [TestMethod]
        public void AllControllersAreRegistered()
        {
            // Is<TType> is an helper, extension method from Windsor
            // which behaves like 'is' keyword in C# but at a Type, not instance level
            var allControllers = GetPublicClassesFromApplicationAssembly(c => c.Is<IController>());
            var registeredControllers = GetImplementationTypesFor(typeof(IController), _containerWithControllers);
            CollectionAssert.AreEqual(allControllers, registeredControllers);
        }

        [TestMethod]
        public void AllAndOnlyControllersHaveControllersSuffix()
        {
            var allControllers = GetPublicClassesFromApplicationAssembly(c => c.Name.EndsWith("Controller"));
            var registeredControllers = GetImplementationTypesFor(typeof(IController), _containerWithControllers);
            CollectionAssert.AreEqual(allControllers, registeredControllers);
        }

        [TestMethod]
        public void AllAndOnlyControllersLiveInControllersNamespace()
        {
            var allControllers = GetPublicClassesFromApplicationAssembly(c => c.Namespace.Contains("Controllers"));
            var registeredControllers = GetImplementationTypesFor(typeof(IController), _containerWithControllers);
            CollectionAssert.AreEqual(allControllers, registeredControllers);
        }

        private static IHandler[] GetAllHandlers(IWindsorContainer container)
        {
            return GetHandlersFor(typeof(object), container);
        }

        private static IHandler[] GetHandlersFor(Type type, IWindsorContainer container)
        {
            return container.Kernel.GetAssignableHandlers(type);
        }

        private static Type[] GetImplementationTypesFor(Type type, IWindsorContainer container)
        {
            return GetHandlersFor(type, container)
                .Select(h => h.ComponentModel.Implementation)
                .OrderBy(t => t.Name)
                .ToArray();
        }

        private static Type[] GetPublicClassesFromApplicationAssembly(Predicate<Type> where)
        {
            return typeof(HealthController).Assembly.GetExportedTypes()
                .Where(t => t.IsClass)
                .Where(t => t.IsAbstract == false)
                .Where(where.Invoke)
                .OrderBy(t => t.Name)
                .ToArray();
        }
    }
}
