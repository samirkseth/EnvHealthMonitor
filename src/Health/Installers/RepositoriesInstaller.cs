using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Health.Repository;

namespace Health.Installers
{
    public class RepositoriesInstaller:IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IHealthRepository>().ImplementedBy(typeof(XmlRepository)));
        }
    }
}