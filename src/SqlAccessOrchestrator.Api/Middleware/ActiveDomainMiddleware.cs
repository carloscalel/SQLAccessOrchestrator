namespace SqlAccessOrchestrator.Api.Middleware;

public sealed class ActiveDomainMiddleware(RequestDelegate next)
{
    public const string HeaderName = "X-Active-Domain";

    public async Task Invoke(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(HeaderName, out var domain) || string.IsNullOrWhiteSpace(domain))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { error = "Debe enviar X-Active-Domain." });
            return;
        }

        context.Items[HeaderName] = domain.ToString();
        await next(context);
    }
}
