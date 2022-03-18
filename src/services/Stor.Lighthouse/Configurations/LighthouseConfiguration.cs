using System.Reflection;

namespace Stor.Lighthouse.Configurations;

public class LighthouseConfiguration
{
    public const string SettingsName = "Lighthouse";

    private static readonly string? AkkaConfigurationPath =
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

    public string SystemName { get; set; } = "stor";

    public string AkkaConfigurationFileName { get; set; } = Path.Combine(AkkaConfigurationPath ?? "", "akka.conf");
}