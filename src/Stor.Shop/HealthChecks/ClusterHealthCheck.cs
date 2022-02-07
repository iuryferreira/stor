using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using Akka.Cluster;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Stor.Shop.HealthChecks;

public class ClusterHealthCheck : IHealthCheck
{
    private readonly ClusterMetric _metric;

    public ClusterHealthCheck(ClusterMetric metric)
    {
        _metric = metric;
    }
    
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new())
    {
        var status = _metric.Members.All(a => a.Status == MemberStatus.Up)
            ? HealthStatus.Healthy
            : HealthStatus.Degraded;

        var message = status != HealthStatus.Healthy ? "Not all nodes are up" : "";

        var dataSerialized = JsonSerializer.Serialize(_metric, new JsonSerializerOptions( JsonSerializerDefaults.Web) { Converters = { new JsonStringEnumConverter() }});
        var data = JsonSerializer.Deserialize<ImmutableDictionary<string, dynamic>>(dataSerialized);
        var healthCheck = new HealthCheckResult(status, message, data: data);
        return Task.FromResult(healthCheck);
    }
}