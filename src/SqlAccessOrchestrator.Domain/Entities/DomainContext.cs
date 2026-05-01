namespace SqlAccessOrchestrator.Domain.Entities;

public sealed record DomainContext(string DomainCode, string RequestedBy, string CorrelationId);
