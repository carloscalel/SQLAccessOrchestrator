namespace SqlAccessOrchestrator.UI.Services;

public sealed class AccessApiClient(HttpClient httpClient)
{
    public Task<HttpResponseMessage> GetUsersAsync(string activeDomain, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "api/users");
        request.Headers.Add("X-Active-Domain", activeDomain);
        return httpClient.SendAsync(request, cancellationToken);
    }
}
