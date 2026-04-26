namespace SqlAccessOrchestrator.Infrastructure.Security;

public static class SqlPermissionGuard
{
    private static readonly HashSet<string> BlockedServerActions =
    [
        "WITH GRANT OPTION", "ALTER ANY USER", "ALTER ANY ROLE", "CONTROL", "db_securityadmin", "db_owner"
    ];

    public static void EnsureSafe(string statement)
    {
        foreach (var blocked in BlockedServerActions)
        {
            if (statement.Contains(blocked, StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException($"Operación bloqueada por política de seguridad: {blocked}");
            }
        }
    }
}
