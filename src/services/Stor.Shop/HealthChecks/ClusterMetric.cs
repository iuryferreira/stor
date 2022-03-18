using Akka.Cluster;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Stor.Shop.HealthChecks;

public class GetClusterMetric
{
}

public class ClusterMetric
{
    public HealthStatus Status { get; set; } = HealthStatus.Healthy;
    public List<Node> Members { get; } = new();
    public string Leader { get; private set; } = "";
    public void SetLeader(string address) => Leader = address;

    public record Node(string Address, MemberStatus Status)
    {
        public MemberStatus Status { get; private set; } = Status;

        public void ChangeStatus(MemberStatus status) => Status = status;
    }
}