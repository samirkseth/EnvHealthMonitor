using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Health.Controllers;

namespace Health.Plumbing
{
    public class ControllersInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(FindControllers().Configure(ConfigureControllers()));
        }

        private static BasedOnDescriptor FindControllers()
        {
            return AllTypes.FromThisAssembly()
                .BasedOn<IController>()
                .If(Component.IsInSameNamespaceAs<HealthController>())
                .If(t => t.Name.EndsWith("Controller"));
        }

        private static ConfigureDelegate ConfigureControllers()
        {
            return c => c.LifeStyle.Transient;
        }
    }
}