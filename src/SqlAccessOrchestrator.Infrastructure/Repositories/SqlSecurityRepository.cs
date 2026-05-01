using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using SqlAccessOrchestrator.Application.Abstractions;
using SqlAccessOrchestrator.Application.DTOs;
using SqlAccessOrchestrator.Infrastructure.Configuration;
using SqlAccessOrchestrator.Infrastructure.Security;
using SqlAccessOrchestrator.Domain.Entities;

namespace SqlAccessOrchestrator.Infrastructure.Repositories;

public sealed class SqlSecurityRepository(IOptionsMonitor<DomainConnectionOptions> optionsMonitor) : ISqlSecurityRepository
{
    public async Task<bool> UserExistsAsync(DomainContext context, string userName, CancellationToken cancellationToken)
    {
        await using var conn = new SqlConnection(GetConnection(context));
        const string query = "SELECT 1 FROM sys.database_principals WHERE name = @UserName";
        return await conn.ExecuteScalarAsync<int?>(new CommandDefinition(query, new { UserName = userName }, cancellationToken: cancellationToken)) is 1;
    }

    public async Task CreateUserAsync(DomainContext context, CreateSqlUserRequest request, CancellationToken cancellationToken)
    {
        var sql = "CREATE LOGIN [" + request.LoginName + "] FROM WINDOWS; CREATE USER [" + request.UserName + "] FOR LOGIN [" + request.LoginName + "];";
        SqlPermissionGuard.EnsureSafe(sql);

        await using var conn = new SqlConnection(GetConnection(context));
        await conn.ExecuteAsync(new CommandDefinition(sql, cancellationToken: cancellationToken));
    }

    public async Task DisableUserAsync(DomainContext context, DisableSqlUserRequest request, CancellationToken cancellationToken)
    {
        const string sql = "ALTER LOGIN [{0}] DISABLE;";
        var statement = string.Format(sql, request.UserName);
        SqlPermissionGuard.EnsureSafe(statement);

        await using var conn = new SqlConnection(GetConnection(context));
        await conn.ExecuteAsync(new CommandDefinition(statement, cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyCollection<string>> ListUserRolesAsync(DomainContext context, string userName, CancellationToken cancellationToken)
    {
        const string sql = @"
SELECT role_principal.name
FROM sys.database_role_members drm
JOIN sys.database_principals role_principal ON drm.role_principal_id = role_principal.principal_id
JOIN sys.database_principals member_principal ON drm.member_principal_id = member_principal.principal_id
WHERE member_principal.name = @UserName;";

        await using var conn = new SqlConnection(GetConnection(context));
        var roles = await conn.QueryAsync<string>(new CommandDefinition(sql, new { UserName = userName }, cancellationToken: cancellationToken));
        return roles.ToArray();
    }


    public async Task<IReadOnlyCollection<SqlUserSummary>> ListUsersAsync(DomainContext context, CancellationToken cancellationToken)
    {
        const string sql = @"
SELECT sp.name AS LoginName, dp.name AS UserName, CAST(sl.is_disabled AS bit) AS IsDisabled
FROM sys.database_principals dp
LEFT JOIN sys.server_principals sp ON dp.sid = sp.sid
LEFT JOIN sys.sql_logins sl ON sp.principal_id = sl.principal_id
WHERE dp.type IN ('S','U','G')
  AND dp.principal_id > 4
  AND dp.name NOT IN ('dbo','guest','INFORMATION_SCHEMA','sys')
ORDER BY dp.name;";

        await using var conn = new SqlConnection(GetConnection(context));
        var users = await conn.QueryAsync<SqlUserSummary>(new CommandDefinition(sql, cancellationToken: cancellationToken));
        return users.ToArray();
    }

    public async Task ClonePermissionsAsync(DomainContext context, PermissionCloneRequest request, CancellationToken cancellationToken)
    {
        await using var conn = new SqlConnection(GetConnection(context));
        await conn.OpenAsync(cancellationToken);
        await using var tx = await conn.BeginTransactionAsync(cancellationToken);

        var cloneSql = "EXEC dbo.usp_CloneUserPermissions @SourceUser, @TargetUser, @IncludeObjectPermissions, @IncludeDeny";
        await conn.ExecuteAsync(new CommandDefinition(cloneSql,
            new
            {
                request.SourceUser,
                request.TargetUser,
                request.IncludeObjectPermissions,
                request.IncludeDeny
            },
            transaction: tx,
            cancellationToken: cancellationToken));

        await tx.CommitAsync(cancellationToken);
    }

    private string GetConnection(DomainContext context)
    {
        var options = optionsMonitor.CurrentValue;
        if (!options.DomainSqlInstances.TryGetValue(context.DomainCode, out var instances) || instances.Count == 0)
        {
            throw new InvalidOperationException($"No existe configuración para el dominio {context.DomainCode}");
        }

        var instance = instances[0];
        return options.ConnectionStringsByInstance[instance];
    }
}
