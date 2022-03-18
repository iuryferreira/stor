using Microsoft.Extensions.DependencyInjection;
using Stor.Lighthouse.Initialization;
using Stor.Lighthouse.Services;

Container.BuildProvider();

var service = Container.ServiceProvider.GetService<ILighthouseService>() ??
              throw new Exception("Service cannot build.");
await service.Start();

Console.CancelKeyPress += async (_, _) => { await service.Stop(); };
Console.ReadKey();