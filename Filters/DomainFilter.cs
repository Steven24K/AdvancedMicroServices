public class DomainFilter(IConfiguration config) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        List<string>? domains = config.GetSection("Domains").Get<List<string>>();
        if (domains == null) return Results.BadRequest("Missing configuration");

        var hostName = context.HttpContext.Request.Host.Host.ToString();
        Console.WriteLine(hostName);
        if (!domains.Contains(hostName)) return Results.Unauthorized();

        return await next(context);
    }
}