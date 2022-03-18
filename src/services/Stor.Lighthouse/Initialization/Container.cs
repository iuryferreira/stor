using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stor.Lighthouse.Factories;
using Stor.Lighthouse.Services;

namespace Stor.Lighthouse.Initialization;

public static class Container
{
    public static ServiceProvider ServiceProvider { get; private set; } =
        new ServiceCollection().BuildServiceProvider();

    public static void BuildProvider()
    {
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        ServiceProvider = serviceCollection.BuildServiceProvider();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true)
            .AddEnvironmentVariables()
            .Build();

        services.AddSingleton<IConfiguration>(configuration);
        services.AddSingleton<ILighthouseFactory, LighthouseFactory>();
        services.AddSingleton<ILighthouseService, LighthouseService>();
    }
}