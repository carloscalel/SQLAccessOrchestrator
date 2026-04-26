using Microsoft.AspNetCore.Authentication.Negotiate;
using SqlAccessOrchestrator.Application.Abstractions;
using SqlAccessOrchestrator.Application.Policies;
using SqlAccessOrchestrator.Application.Services;
using SqlAccessOrchestrator.Infrastructure.Configuration;
using SqlAccessOrchestrator.Infrastructure.Repositories;

namespace SqlAccessOrchestrator.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPlatformServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DomainConnectionOptions>(configuration.GetSection("DomainConnections"));
        services.AddScoped<SqlAccessGovernanceService>();
        services.AddScoped<ISqlSecurityRepository, SqlSecurityRepository>();
        services.AddScoped<IAuditRepository, AuditRepository>();

        services.AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate();
        services.AddAuthorization(options =>
        {
            options.AddPolicy(Permissions.CreateUser, policy => policy.RequireClaim("Permission", Permissions.CreateUser));
            options.AddPolicy(Permissions.ViewUsers, policy => policy.RequireClaim("Permission", Permissions.ViewUsers));
            options.AddPolicy(Permissions.ViewAudit, policy => policy.RequireClaim("Permission", Permissions.ViewAudit));
        });

        services.AddHealthChecks();
        services.AddRateLimiter(_ => { });
        return services;
    }
}
