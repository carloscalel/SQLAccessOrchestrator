using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlAccessOrchestrator.Api.Middleware;
using SqlAccessOrchestrator.Application.DTOs;
using SqlAccessOrchestrator.Application.Policies;
using SqlAccessOrchestrator.Application.Services;
using SqlAccessOrchestrator.Domain.Entities;

namespace SqlAccessOrchestrator.Api.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController(SqlAccessGovernanceService service) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = Permissions.CreateUser)]
    public async Task<IActionResult> Create([FromBody] CreateSqlUserRequest request, CancellationToken cancellationToken)
    {
        var context = BuildContext();
        await service.CreateUserAsync(context, request, cancellationToken);
        return CreatedAtAction(nameof(Create), new { request.UserName }, null);
    }

    [HttpPost("disable")]
    [Authorize(Policy = Permissions.DeleteUser)]
    public async Task<IActionResult> Disable([FromBody] DisableSqlUserRequest request, CancellationToken cancellationToken)
    {
        await service.DisableUserAsync(BuildContext(), request, cancellationToken);
        return NoContent();
    }

    [HttpPost("clone")]
    [Authorize(Policy = Permissions.AssignRole)]
    public async Task<IActionResult> ClonePermissions([FromBody] PermissionCloneRequest request, CancellationToken cancellationToken)
    {
        await service.ClonePermissionsAsync(BuildContext(), request, cancellationToken);
        return Ok(new { status = "preview supported via dry-run SP parameter" });
    }

    private DomainContext BuildContext() =>
        new(
            HttpContext.Items[ActiveDomainMiddleware.HeaderName]?.ToString() ?? throw new InvalidOperationException("Domain requerido"),
            User.Identity?.Name ?? "unknown",
            HttpContext.Items[CorrelationIdMiddleware.HeaderName]?.ToString() ?? Guid.NewGuid().ToString("N"));
}
