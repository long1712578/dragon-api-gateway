using Microsoft.AspNetCore.HttpOverrides;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
	.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// Nếu không chạy trên Kubernetes thì mới nạp thêm đè cấu hình local
if (Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_HOST") == null)
{
	builder.Configuration.AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
}

builder.Configuration.AddEnvironmentVariables();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
	options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
	options.KnownNetworks.Clear();
	options.KnownProxies.Clear();
});

// CORS — cho phép CV website (và các frontend khác) gọi API qua gateway
builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(policy =>
	{
		policy
			.SetIsOriginAllowed(_ => true)     // Develop: cho phép tất cả origin
			.AllowAnyHeader()
			.AllowAnyMethod()
			.AllowCredentials();
	});
});

builder.Services.AddHealthChecks();
builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

app.UseForwardedHeaders();
app.UseCors();

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
