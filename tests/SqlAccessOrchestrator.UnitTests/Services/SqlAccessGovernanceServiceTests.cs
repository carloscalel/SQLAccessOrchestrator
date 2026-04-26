using FluentAssertions;
using SqlAccessOrchestrator.Application.Abstractions;
using SqlAccessOrchestrator.Application.DTOs;
using SqlAccessOrchestrator.Application.Services;
using SqlAccessOrchestrator.Domain.Entities;

namespace SqlAccessOrchestrator.UnitTests.Services;

public sealed class SqlAccessGovernanceServiceTests
{
    [Fact]
    public async Task CreateUser_ShouldThrow_WhenAlreadyExists()
    {
        var sqlRepo = new FakeSqlRepo(exists: true);
        var auditRepo = new FakeAuditRepo();
        var sut = new SqlAccessGovernanceService(sqlRepo, auditRepo);

        var act = () => sut.CreateUserAsync(new DomainContext("CORP", "corp\\john", "corr-1"), new CreateSqlUserRequest("corp\\john", "john"), default);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    private sealed class FakeSqlRepo(bool exists) : ISqlSecurityRepository
    {
        public Task<bool> UserExistsAsync(DomainContext context, string userName, CancellationToken cancellationToken) => Task.FromResult(exists);
        public Task CreateUserAsync(DomainContext context, CreateSqlUserRequest request, CancellationToken cancellationToken) => Task.CompletedTask;
        public Task DisableUserAsync(DomainContext context, DisableSqlUserRequest request, CancellationToken cancellationToken) => Task.CompletedTask;
        public Task<IReadOnlyCollection<string>> ListUserRolesAsync(DomainContext context, string userName, CancellationToken cancellationToken) => Task.FromResult<IReadOnlyCollection<string>>([]);
        public Task ClonePermissionsAsync(DomainContext context, PermissionCloneRequest request, CancellationToken cancellationToken) => Task.CompletedTask;
    }

    private sealed class FakeAuditRepo : IAuditRepository
    {
        public Task WriteAsync(AuditEntry entry, CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
