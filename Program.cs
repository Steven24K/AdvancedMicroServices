var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHttpClient();
builder.Services.AddControllersWithViews();


builder.Services.AddSingleton<AuthorizationFilter>();
builder.Services.AddScoped<IProxyService, ProxyService>();

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

app.MapGet("/proxy-html", ApiActions.ProxyHtml)
    .AddEndpointFilter<DomainFilter>()
    .WithName("Proxy HTML");

app.MapControllers();

app.Run();

