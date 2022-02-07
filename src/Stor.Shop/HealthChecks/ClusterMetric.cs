using System.Text.Json;
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

    public Metric Cluster { get; } = new();

    public record Metric
    {
        public string Leader { get; private set; }
        public double MemoryUsage { get; private set; }
        public double CpuTotalUsage { get; private set; }
        public int Processors { get; private set; }

        public void SetMemory(double value) => MemoryUsage = value;
        public void SetCpu(double percentage) => CpuTotalUsage = percentage;
        public void SetProcessors(int quantity) => Processors = quantity;
        
        public void SetLeader(string address) => Leader = address;


        public override string ToString()
        {
            return JsonSerializer.Serialize(new
            {
                MemoryUsage = Math.Ceiling(MemoryUsage / 1024 / 1024),
                CpuTotalUsagePercentage = Math.Ceiling(CpuTotalUsage / 100),
                Processors,
                Leader
            }, new JsonSerializerOptions( JsonSerializerDefaults.Web ));
        }
    }

    public record Node(string Address, MemberStatus Status)
    {
        public MemberStatus Status { get; private set; } = Status;

        public void ChangeStatus(MemberStatus status) => Status = status;
    }
}