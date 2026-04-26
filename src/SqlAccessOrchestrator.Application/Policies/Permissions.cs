namespace SqlAccessOrchestrator.Application.Policies;

public static class Permissions
{
    public const string CreateUser = "CREATE_USER";
    public const string DeleteUser = "DELETE_USER";
    public const string AssignRole = "ASSIGN_ROLE";
    public const string ViewUsers = "VIEW_USERS";
    public const string ViewAudit = "VIEW_AUDIT";
}
