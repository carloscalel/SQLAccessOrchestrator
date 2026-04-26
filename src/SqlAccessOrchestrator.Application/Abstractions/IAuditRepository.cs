using SqlAccessOrchestrator.Domain.Entities;

namespace SqlAccessOrchestrator.Application.Abstractions;

public interface IAuditRepository
{
    Task WriteAsync(AuditEntry entry, CancellationToken cancellationToken);
}
