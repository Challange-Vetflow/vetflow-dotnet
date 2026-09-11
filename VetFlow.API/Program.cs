using Serilog;
using VetFlow.API.Extensions;
using VetFlow.API.Security;
using VetFlow.Infrastructure.Persistence;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using HealthChecks.UI.Client;

namespace VetFlow.API;

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // ---------- Logging estruturado (Serilog) ----------
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", "VetFlow.API")
            .WriteTo.Console(outputTemplate:
                "[{Timestamp:HH:mm:ss} {Level:u3}] {CorrelationId} {SourceContext}: {Message:lj}{NewLine}{Exception}")
            .WriteTo.File("logs/vetflow-.log",
                rollingInterval: RollingInterval.Day,
                outputTemplate:
                "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {CorrelationId} {SourceContext}: {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        builder.Host.UseSerilog();

        builder.Services.AddVetFlowDbContext(builder.Configuration);
        builder.Services.AddVetFlowRepositories();
        builder.Services.AddVetFlowSwagger(builder.Configuration);
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddControllers();
        builder.Services.AddHttpContextAccessor();

        // ---------- Autenticação (API Key) ----------
        builder.Services
            .AddAuthentication(ApiKeyAuthenticationOptions.DefaultScheme)
            .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
                ApiKeyAuthenticationOptions.DefaultScheme, options => { });
        builder.Services.AddAuthorization();

        // ---------- Health Checks ----------
        var useSqlite = builder.Configuration.GetValue<bool>("Database:UseSqlite");
        var healthChecksBuilder = builder.Services.AddHealthChecks();

        if (!useSqlite)
        {
            var oracleConn = builder.Configuration.GetConnectionString("VetFlowOracle");
            if (!string.IsNullOrWhiteSpace(oracleConn))
            {
                healthChecksBuilder.AddOracle(
                    oracleConn,
                    name: "oracle-database",
                    tags: ["db", "oracle", "ready"]);
            }
        }

        healthChecksBuilder.AddCheck(
            "self",
            () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("API respondendo."),
            tags: ["live"]);

        builder.Services.AddHealthChecksUI(o =>
        {
            o.SetEvaluationTimeInSeconds(30);
            o.MaximumHistoryEntriesPerEndpoint(50);
            o.AddHealthCheckEndpoint("VetFlow API", "/health");
        }).AddInMemoryStorage();

        // ---------- Tracing e Métricas (OpenTelemetry) ----------
        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService("VetFlow.API"))
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation(o =>
                {
                    o.RecordException = true;
                })
                .AddHttpClientInstrumentation()
                .AddSource("VetFlow.API")
                .AddConsoleExporter())
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddMeter("VetFlow.API")
                .AddConsoleExporter());

        var app = builder.Build();

        // ---------- Middleware de correlação de requisições ----------
        app.Use(async (context, next) =>
        {
            var correlationId = context.Request.Headers["X-Correlation-Id"].FirstOrDefault()
                ?? Guid.NewGuid().ToString();
            context.Response.Headers["X-Correlation-Id"] = correlationId;
            using (Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId))
            {
                await next();
            }
        });

        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment() && builder.Configuration.GetValue<bool>("Database:UseSqlite"))
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<VetFlowContext>();
            db.Database.EnsureCreated();
        }

        app.UseSwagger();
        app.UseSwaggerUI(o =>
        {
            o.SwaggerEndpoint("/swagger/v1/swagger.json", "VetFlow API v1");
            o.RoutePrefix = string.Empty; // Swagger na raiz
        });

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        // ---------- Endpoints de Health Check ----------
        app.MapHealthChecks("/health", new()
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        app.MapHealthChecks("/health/live", new()
        {
            Predicate = check => check.Tags.Contains("live")
        });
        app.MapHealthChecks("/health/ready", new()
        {
            Predicate = check => check.Tags.Contains("ready")
        });
        app.MapHealthChecksUI(o => o.UIPath = "/health-ui");

        app.Run();
    }
}

public partial class Program { } // exposto para WebApplicationFactory nos testes de integração
