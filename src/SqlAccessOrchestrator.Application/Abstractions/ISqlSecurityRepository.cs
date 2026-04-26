using SqlAccessOrchestrator.Application.DTOs;
using SqlAccessOrchestrator.Domain.Entities;

namespace SqlAccessOrchestrator.Application.Abstractions;

public interface ISqlSecurityRepository
{
    Task<bool> UserExistsAsync(DomainContext context, string userName, CancellationToken cancellationToken);
    Task CreateUserAsync(DomainContext context, CreateSqlUserRequest request, CancellationToken cancellationToken);
    Task DisableUserAsync(DomainContext context, DisableSqlUserRequest request, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<string>> ListUserRolesAsync(DomainContext context, string userName, CancellationToken cancellationToken);
    Task ClonePermissionsAsync(DomainContext context, PermissionCloneRequest request, CancellationToken cancellationToken);
}
