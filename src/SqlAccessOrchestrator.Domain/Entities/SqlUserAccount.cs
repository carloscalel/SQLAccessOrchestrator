namespace SqlAccessOrchestrator.Domain.Entities;

public sealed class SqlUserAccount
{
    public required string LoginName { get; init; }
    public required string UserName { get; init; }
    public bool IsDisabled { get; private set; }
    public DateTimeOffset? DisabledAtUtc { get; private set; }

    public void Disable(DateTimeOffset utcNow)
    {
        IsDisabled = true;
        DisabledAtUtc = utcNow;
    }
}
