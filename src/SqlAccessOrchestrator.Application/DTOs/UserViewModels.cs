namespace SqlAccessOrchestrator.Application.DTOs;

public sealed record SqlUserSummary(string LoginName, string UserName, bool IsDisabled);
