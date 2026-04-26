using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using SqlAccessOrchestrator.Application.Abstractions;
using SqlAccessOrchestrator.Domain.Entities;
using SqlAccessOrchestrator.Infrastructure.Configuration;

namespace SqlAccessOrchestrator.Infrastructure.Repositories;

public sealed class AuditRepository(IOptionsMonitor<DomainConnectionOptions> optionsMonitor) : IAuditRepository
{
    public async Task WriteAsync(AuditEntry entry, CancellationToken cancellationToken)
    {
        var connStr = optionsMonitor.CurrentValue.ConnectionStringsByInstance.Values.First();
        await using var conn = new SqlConnection(connStr);
        const string sql = @"
INSERT INTO dbo.AuditoriaAccesos(DomainCode, ActionName, Actor, PayloadJson, CorrelationId, CreatedAtUtc)
VALUES(@DomainCode, @Action, @Actor, @PayloadJson, @CorrelationId, @CreatedAtUtc);";
        await conn.ExecuteAsync(new CommandDefinition(sql, entry, cancellationToken: cancellationToken));
    }
}
