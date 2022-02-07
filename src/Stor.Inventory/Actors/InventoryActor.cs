using Akka.Actor;
using Akka.Event;
using Microsoft.Extensions.DependencyInjection;
using Stor.Inventory.Data;
using Stor.Shared.Messages;

namespace Stor.Inventory.Actors;

class InventoryActor : ReceiveActor
{
    private const string ApplicationName = "Stor - Inventory";
    internal const string ActorName = "stor-inventory";

    public InventoryActor(Repository? repository)
    {
        if(repository is null) throw new ArgumentException(null, nameof(repository));
        
        Receive<ProductMessage.GetAllProducts>(_ =>
        {
            var products = repository.All().Select(x => (ProductMessage.Product) x).ToList();
            Sender.Tell(new ProductMessage.AllProducts(products));
        });
    }

    private ILoggingAdapter Log { get; } = Context.GetLogger();
    internal static Props Props() => Akka.Actor.Props.Create(() => new InventoryActor(Container.Services.GetService<Repository>()));

    protected override void PreStart() => Log.Info($"[{ApplicationName}] | Application started");
    protected override void PostStop() => Log.Info($"[{ApplicationName}] | Application stopped");
}