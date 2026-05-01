using System.Net.Http.Json;
using SqlAccessOrchestrator.Application.DTOs;

namespace SqlAccessOrchestrator.UI.Services;

public sealed class AccessApiClient(HttpClient httpClient)
{
    public async Task<IReadOnlyCollection<SqlUserSummary>> GetUsersAsync(string activeDomain, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "api/users");
        request.Headers.Add("X-Active-Domain", activeDomain);

        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<IReadOnlyCollection<SqlUserSummary>>(cancellationToken: cancellationToken)
               ?? [];
    }
}
