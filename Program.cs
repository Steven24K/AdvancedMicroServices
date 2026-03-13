var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<AuthorizationFilter>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


// Middleware
app.Use(async (context, next) =>
{
    await next();
});

app.MapGet("/", () => "It works, services up and running!").WithName("Home");

app.MapGet("/proxy-html", async (HttpContext context, IHttpClientFactory httpClientFactory) =>
{
    var url = context.Request.Query["url"].FirstOrDefault();
    if (string.IsNullOrEmpty(url)) return Results.BadRequest("Missing parameter 'url'");

    var sanatized_url = new Uri(url);
    var protocol = sanatized_url.OriginalString.Replace($"{sanatized_url.Host}{sanatized_url.AbsolutePath}", "");

    var client = httpClientFactory.CreateClient();

    var response = await client.GetAsync(url);
    var html = await response.Content.ReadAsStringAsync();

    // Inject a <base> tag so images/css don't break
    var baseTag = $"<base href='{sanatized_url.GetLeftPart(UriPartial.Authority)}/'>";
    var modifiedHtml = html.Replace("<head>", $"<head>{baseTag}");


    return Results.Content(modifiedHtml, "text/html");

})
.AddEndpointFilter<DomainFilter>()
.WithName("Proxy HTML");

app.Run();

