public static class ApiActions
{
    public static async Task<IResult> ProxyHtml(HttpContext context, IProxyService proxyService)
    {
        var url = context.Request.Query["url"].FirstOrDefault();
        if (string.IsNullOrEmpty(url)) return Results.BadRequest("Missing parameter 'url'");

        var html = await proxyService.ProxyHtml(url);
        
        return Results.Content(html, "text/html");
    }
}
