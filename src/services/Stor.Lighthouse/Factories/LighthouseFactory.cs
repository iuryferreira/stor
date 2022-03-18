using Akka.Actor;
using Akka.Configuration;
using Stor.Lighthouse.Configurations;

namespace Stor.Lighthouse.Factories;

public class LighthouseFactory : ILighthouseFactory
{
    public ActorSystem CreateInstance(LighthouseConfiguration configuration)
    {
        var clusterConfig = ConfigurationFactory.ParseString(File.ReadAllText(configuration.AkkaConfigurationFileName));
        return ActorSystem.Create(configuration.SystemName, clusterConfig);
    }
}

public interface ILighthouseFactory
{
    ActorSystem CreateInstance(LighthouseConfiguration configuration);
}