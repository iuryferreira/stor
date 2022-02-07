using System.Text.Json;
using System.Text.Json.Serialization;
using Akka.Actor;
using Akka.Cluster;
using Akka.Cluster.Metrics;
using Akka.Cluster.Metrics.Events;
using Akka.Cluster.Metrics.Serialization;
using Akka.Event;
using Akka.Util;

namespace Stor.Shop.HealthChecks;

public class MetricsListener : ReceiveActor
{
    private readonly ILoggingAdapter _log = Context.GetLogger();
    private readonly Cluster _cluster = Cluster.Get(Context.System);
    private readonly ClusterMetrics _metricsExtension = ClusterMetrics.Get(Context.System);
    public readonly ClusterMetric Metric = new();

    public MetricsListener()
    {
        _cluster.Subscribe(Self, ClusterEvent.SubscriptionInitialStateMode.InitialStateAsSnapshot, typeof(ClusterEvent.IMemberEvent));

        Receive<GetClusterMetric>(_ => Sender.Tell(Metric));
        
        Receive<ClusterEvent.IMemberEvent>(evt =>
        {
            var member = Metric.Members.FirstOrDefault(x => x.Address.Equals(evt.Member.UniqueAddress.Address.ToString()));
            if (member is null)
            {
                //_members[evt.Member.Address.Host] = evt.Member;
                Metric.Members.Add(new ClusterMetric.Node(evt.Member.UniqueAddress.Address.ToString(), evt.Member.Status));
                return;
            }

            member.ChangeStatus(evt.Member.Status);
        });

        Receive<ClusterMetricsChanged>(clusterMetrics =>
        {
            foreach (var nodeMetrics in clusterMetrics.NodeMetrics)
            {
                if (!nodeMetrics.Address.Equals(_cluster.SelfAddress)) continue;
                LogMetric(nodeMetrics);
            }
        });
    }

    protected override void PreStart()
    {
        base.PreStart();
        _metricsExtension.Subscribe(Self);
    }

    protected override void PostStop()
    {
        base.PostStop();
        _metricsExtension.Unsubscribe(Self);
    }

    private void LogMetric(NodeMetrics nodeMetrics)
    {
        Option<StandardMetrics.Memory> memory = StandardMetrics.ExtractMemory(nodeMetrics);
        Option<StandardMetrics.Cpu> cpu = StandardMetrics.ExtractCpu(nodeMetrics);

        if (memory.HasValue) Metric.Cluster.SetMemory(memory.Value.Used);
        if (!cpu.HasValue) return;
        

        Metric.Cluster.SetCpu(cpu.Value.TotalUsage);
        Metric.Cluster.SetProcessors(cpu.Value.ProcessorsNumber);
        Metric.Cluster.SetLeader(_cluster.State.Leader.ToString());
        _log.Info(JsonSerializer.Serialize(Metric, new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() }}));
    }
}