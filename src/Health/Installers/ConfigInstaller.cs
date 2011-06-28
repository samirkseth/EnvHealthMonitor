using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Health.Config;

namespace Health.Installers
{
    public class ConfigInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var config = (IEnvironmentConfiguration) System.Configuration.ConfigurationManager.GetSection(
                                    "HealthApplication/EnvironmentConfiguration");
            container.Register(Component.For<IEnvironmentConfiguration>().Instance((config)));
        }
    }
}