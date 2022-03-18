using Akka.Actor;
using Akka.Configuration;
using Stor.Inventory;
using Stor.Inventory.Actors;

Container.BuildProvider();

var config = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "akka.conf"));
var akkaConfig = ConfigurationFactory.ParseString(config);
//.WithFallback(ClusterMetrics.DefaultConfig());

var system = ActorSystem.Create("stor", akkaConfig);
system.ActorOf(InventoryActor.Props(), InventoryActor.ActorName);

Console.CancelKeyPress += delegate { system.Dispose(); };

Console.ReadKey();