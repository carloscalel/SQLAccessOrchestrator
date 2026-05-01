namespace SqlAccessOrchestrator.Infrastructure.Configuration;

public sealed class DomainConnectionOptions
{
    public Dictionary<string, List<string>> DomainSqlInstances { get; init; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> ConnectionStringsByInstance { get; init; } = new(StringComparer.OrdinalIgnoreCase);
}
