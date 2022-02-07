using System.Text.Json;
using Akka.Actor;
using Akka.Event;
using Stor.Shared.Messages;
using Stor.Shop.Data;

namespace Stor.Shop.Actors;

internal class ShopActor : ReceiveActor
{
    private const string ApplicationName = "Stor - Shop";
    internal const string ActorName = "stor-shop";

    private readonly ActorSelection _inventory = Context.ActorSelection(RemoteActors.StorInventory);
    public ShopActor(Repository? repository)
    {
        if(repository is null) throw new ArgumentException(null, nameof(repository));

        Receive<ProductMessage.GetAllProducts>(message =>
        {
            _inventory.Tell(message);
        });
        Receive<ProductMessage.AllProducts>(message =>
        {
            Console.WriteLine(JsonSerializer.Serialize(message));
            repository.SetProductList(message.Products);
        });
    }

    private ILoggingAdapter Log { get; } = Context.GetLogger();

    protected override void PreStart() => Log.Info($"[{ApplicationName}] | Application started");
    protected override void PostStop() => Log.Info($"[{ApplicationName}] | Application stopped");

    internal static Props Props(Repository repository) => Akka.Actor.Props.Create(() => new ShopActor(repository));
}