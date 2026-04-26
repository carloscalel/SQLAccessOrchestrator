namespace SqlAccessOrchestrator.Domain.Entities;

public sealed class AuditEntry
{
    public required string DomainCode { get; init; }
    public required string Action { get; init; }
    public required string Actor { get; init; }
    public required string PayloadJson { get; init; }
    public required string CorrelationId { get; init; }
    public DateTimeOffset CreatedAtUtc { get; init; } = DateTimeOffset.UtcNow;
}
