using Akka.Actor;
using Akka.Cluster.Metrics;
using Akka.Configuration;
using Stor.Inventory;
using Stor.Inventory.Actors;
using Stor.Shared.Messages;

Container.BuildProvider();

var config = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "akka.conf"));
var akkaConfig = ConfigurationFactory.ParseString(config)
    .WithFallback(ClusterMetrics.DefaultConfig());

using var system = ActorSystem.Create("stor-system", akkaConfig);
var inventory = system.ActorOf(InventoryActor.Props(), InventoryActor.ActorName);
Console.ReadLine();