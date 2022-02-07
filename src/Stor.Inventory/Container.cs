using Microsoft.Extensions.DependencyInjection;
using Stor.Inventory.Data;

namespace Stor.Inventory;

public static class Container
{
    public static ServiceProvider Services { get; private set; } = new ServiceCollection().BuildServiceProvider();
    
    public static void BuildProvider()
    {
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        Services = serviceCollection.BuildServiceProvider();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<Repository>();
    }
}