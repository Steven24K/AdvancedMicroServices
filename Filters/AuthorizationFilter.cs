public class AuthorizationFilter(IConfiguration config) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        string? secret = config["ApiKey"];
        if (string.IsNullOrEmpty(secret)) return Results.BadRequest("Missing API key in config");
        if (!context.HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeaderValues)) return Results.Unauthorized();

        var authHeaderValue = authHeaderValues.ToString();

        if (string.IsNullOrEmpty(authHeaderValue)) return Results.Unauthorized();

        if (authHeaderValue != secret) return Results.Unauthorized();

        return await next(context);
    }
}