using Microsoft.AspNetCore.HttpOverrides;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
	.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
	.AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
	.AddEnvironmentVariables();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
	options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
	options.KnownNetworks.Clear();
	options.KnownProxies.Clear();
});

builder.Services.AddHealthChecks();
builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

app.UseForwardedHeaders();

app.MapGet("/", (IConfiguration configuration, IWebHostEnvironment environment) =>
{
	return Results.Ok(new
	{
		service = configuration["Gateway:ServiceName"] ?? "dragon-api-gateway",
		environment = environment.EnvironmentName,
		status = "ok",
		timestamp = DateTimeOffset.UtcNow
	});
});

app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready");

await app.UseOcelot();

app.Run();
