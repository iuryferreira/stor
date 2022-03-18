using Akka.Actor;
using Akka.Cluster;

namespace Stor.Shop.HealthChecks;

public class MetricsListener : ReceiveActor
{
    private readonly Cluster _cluster = Cluster.Get(Context.System);
    private readonly ClusterMetric _metric = new();

    public MetricsListener()
    {
        _cluster.Subscribe(Self, ClusterEvent.SubscriptionInitialStateMode.InitialStateAsEvents,
            typeof(ClusterEvent.IMemberEvent));

        Receive<GetClusterMetric>(_ => Sender.Tell(_metric));

        Receive<ClusterEvent.IMemberEvent>(evt =>
        {
            _metric.SetLeader(_cluster.State.Leader?.ToString() ?? string.Empty);

            var address = evt.Member.Address.ToString();

            if (evt.Member.Address.Equals(_cluster.SelfAddress)) return;

            var member = _metric.Members.FirstOrDefault(x => x.Address.Equals(address));

            if (member is null)
            {
                _metric.Members.Add(new ClusterMetric.Node(address, evt.Member.Status));
                return;
            }

            member.ChangeStatus(evt.Member.Status);
        });
    }
}