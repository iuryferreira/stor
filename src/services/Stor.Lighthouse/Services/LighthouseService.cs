using Akka.Actor;
using Microsoft.Extensions.Configuration;
using Petabridge.Cmd.Cluster;
using Petabridge.Cmd.Cluster.Sharding;
using Petabridge.Cmd.Host;
using Petabridge.Cmd.Remote;
using Stor.Lighthouse.Configurations;
using Stor.Lighthouse.Factories;

namespace Stor.Lighthouse.Services;

public class LighthouseService : ILighthouseService
{
    private readonly IConfiguration _configuration;
    private readonly ILighthouseFactory _factory;

    private ActorSystem? _system;

    public LighthouseService(ILighthouseFactory factory, IConfiguration configuration)
    {
        _factory = factory;
        _configuration = configuration;
        Configure();
    }

    public Task Start()
    {
        var command = PetabridgeCmd.Get(_system);

        command.RegisterCommandPalette(ClusterCommands.Instance);
        command.RegisterCommandPalette(ClusterShardingCommands.Instance);
        command.RegisterCommandPalette(new RemoteCommands());
        command.Start();

        return Task.CompletedTask;
    }

    public async Task Stop() => await CoordinatedShutdown.Get(_system).Run(CoordinatedShutdown.ClrExitReason.Instance);

    private void Configure()
    {
        var lighthouseConfiguration = new LighthouseConfiguration();
        _configuration.GetSection(LighthouseConfiguration.SettingsName).Bind(lighthouseConfiguration);
        _system = _factory.CreateInstance(lighthouseConfiguration);
    }
}

public interface ILighthouseService
{
    Task Start();
    Task Stop();
}