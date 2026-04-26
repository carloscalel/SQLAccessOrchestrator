using System.Text.Json;
using SqlAccessOrchestrator.Application.Abstractions;
using SqlAccessOrchestrator.Application.DTOs;
using SqlAccessOrchestrator.Domain.Entities;

namespace SqlAccessOrchestrator.Application.Services;

public sealed class SqlAccessGovernanceService(ISqlSecurityRepository sqlSecurityRepository, IAuditRepository auditRepository)
{
    public async Task CreateUserAsync(DomainContext context, CreateSqlUserRequest request, CancellationToken cancellationToken)
    {
        var exists = await sqlSecurityRepository.UserExistsAsync(context, request.UserName, cancellationToken);
        if (exists)
        {
            throw new InvalidOperationException($"El usuario {request.UserName} ya existe en el dominio {context.DomainCode}.");
        }

        await sqlSecurityRepository.CreateUserAsync(context, request, cancellationToken);
        await AuditAsync(context, "CREATE_USER", request, cancellationToken);
    }

    public async Task DisableUserAsync(DomainContext context, DisableSqlUserRequest request, CancellationToken cancellationToken)
    {
        var exists = await sqlSecurityRepository.UserExistsAsync(context, request.UserName, cancellationToken);
        if (!exists)
        {
            throw new InvalidOperationException($"El usuario {request.UserName} no existe en el dominio {context.DomainCode}.");
        }

        await sqlSecurityRepository.DisableUserAsync(context, request, cancellationToken);
        await AuditAsync(context, "DISABLE_USER", request, cancellationToken);
    }

    public async Task ClonePermissionsAsync(DomainContext context, PermissionCloneRequest request, CancellationToken cancellationToken)
    {
        if (request.SourceUser.Equals(request.TargetUser, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Usuario origen y destino deben ser diferentes para clonado de permisos.");
        }

        await sqlSecurityRepository.ClonePermissionsAsync(context, request, cancellationToken);
        await AuditAsync(context, "CLONE_PERMISSIONS", request, cancellationToken);
    }

    private Task AuditAsync(DomainContext context, string action, object payload, CancellationToken cancellationToken) =>
        auditRepository.WriteAsync(new AuditEntry
        {
            DomainCode = context.DomainCode,
            Action = action,
            Actor = context.RequestedBy,
            CorrelationId = context.CorrelationId,
            PayloadJson = JsonSerializer.Serialize(payload)
        }, cancellationToken);
}
