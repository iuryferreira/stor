using System.Text.Json;
using System.Text.Json.Serialization;
using Akka.Actor;
using Akka.Configuration;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Stor.Shared.Messages;
using Stor.Shop;
using Stor.Shop.Actors;
using Stor.Shop.Data;
using Stor.Shop.HealthChecks;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<Repository>();

// Akka Config File
var akkaConfigFilename = builder.Configuration["Akka:File"] ?? "akka.conf";
var content = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, akkaConfigFilename));
var akkaConfig = ConfigurationFactory.ParseString(content);
var actorSystem = ActorSystem.Create("stor-system", akkaConfig);

var repository = new Repository();
var shop = actorSystem.ActorOf(ShopActor.Props(repository), ShopActor.ActorName);
var metrics = actorSystem.ActorOf<MetricsListener>();

builder.Services.AddScoped(_ => metrics.Ask<ClusterMetric>(new GetClusterMetric()).Result);

builder.Services.AddHealthChecks().AddTypeActivatedCheck<ClusterHealthCheck>("akka", HealthStatus.Unhealthy);


var app = builder.Build();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = (context, result) => context.Response.WriteAsync(JsonSerializer.Serialize(result.Entries, new JsonSerializerOptions
    {
        Converters = {new JsonStringEnumConverter()},
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    }))
});

app.MapGet("/", () =>
{
    shop.Tell(new ProductMessage.GetAllProducts());
    return Results.Ok(repository.All());
});

app.Run();