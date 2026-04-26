namespace SqlAccessOrchestrator.Application.DTOs;

public sealed record CreateSqlUserRequest(string LoginName, string UserName);
public sealed record DisableSqlUserRequest(string UserName, string Reason);
public sealed record PermissionCloneRequest(string SourceUser, string TargetUser, bool IncludeObjectPermissions, bool IncludeDeny);
