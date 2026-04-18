using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using QuantityMeasurementRepository.Context;
using QuantityMeasurementWebAPI.Middleware;
using QuantityMeasurementRepository.Extensions;
using QuantityMeasurementBusinessLayer.Extensions;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("=== Quantity Measurement API starting (PostgreSQL / Render) ===");

    var builder = WebApplication.CreateBuilder(args);

    // Config loading: WebAPI/Config folder (same as before)
    var configDir = Path.Combine(builder.Environment.ContentRootPath, "Config");
    builder.Configuration
        .AddJsonFile(Path.Combine(configDir, "appsettings.json"), optional: false, reloadOnChange: true)
        .AddJsonFile(Path.Combine(configDir, $"appsettings.{builder.Environment.EnvironmentName}.json"), optional: true, reloadOnChange: true)
        .AddEnvironmentVariables(); // ← Picks up DATABASE_URL, JWT_SECRET, etc. from Render

    builder.Host.UseSerilog((context, services, loggerConfiguration) =>
    {
        loggerConfiguration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .WriteTo.Console(); // Render streams logs from stdout — file logging not needed
    });

    builder.Services.AddControllers()
        .AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = null);

    builder.Services.AddPersistence(builder.Configuration);
    builder.Services.AddBusinessServices();

    var config = builder.Configuration;

    // Support both env var override (Render) and appsettings
    string secret = Environment.GetEnvironmentVariable("JWT_SECRET")
        ?? config["JwtSettings:Secret"]
        ?? throw new InvalidOperationException("JWT Secret is not configured. Set JWT_SECRET env var on Render.");
    string issuer   = config["JwtSettings:Issuer"]   ?? "QuantityMeasurementAPI";
    string audience = config["JwtSettings:Audience"] ?? "QuantityMeasurementAPIClients";

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            ValidateIssuer   = true,
            ValidIssuer      = issuer,
            ValidateAudience = true,
            ValidAudience    = audience,
            ValidateLifetime = true,
            ClockSkew        = TimeSpan.FromMinutes(1)
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = ctx =>
            {
                if (ctx.Exception is SecurityTokenExpiredException)
                    ctx.Response.Headers.Append("Token-Expired", "true");
                return Task.CompletedTask;
            }
        };
    });
    builder.Services.AddAuthorization();

    // Swagger — enabled in all environments (including Production on Render)
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title       = "Quantity Measurement API",
            Version     = "v1",
            Description = "Clean Architecture | JWT Auth | BCrypt | EF Core → PostgreSQL | Render"
        });
        var securityScheme = new OpenApiSecurityScheme
        {
            Name        = "Authorization",
            Description = "Enter: Bearer {your_jwt_token}",
            In          = ParameterLocation.Header,
            Type        = SecuritySchemeType.ApiKey,
            Scheme      = "Bearer",
            BearerFormat = "JWT"
        };
        options.AddSecurityDefinition("Bearer", securityScheme);
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            { new OpenApiSecurityScheme { Reference = new OpenApiReference { Id = "Bearer", Type = ReferenceType.SecurityScheme } }, new List<string>() }
        });
    });
    builder.Services.AddEndpointsApiExplorer();

    // Health checks — PostgreSQL only (Redis is optional/fallback)
    if (!builder.Environment.IsEnvironment("Testing"))
    {
        var dbConn = Environment.GetEnvironmentVariable("DATABASE_URL")
                     ?? config.GetConnectionString("QuantityMeasurementDb") ?? string.Empty;
        builder.Services.AddHealthChecks()
            .AddNpgSql(dbConn, name: "postgresql", tags: new[] { "db" });
    }

    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    // CORS — allow local dev + deployed frontend (update with your real Render frontend URL)
    var frontendUrl = Environment.GetEnvironmentVariable("FRONTEND_URL") ?? string.Empty;
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowFrontend", policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });

    // Render sets PORT env var — bind to it
    var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

    var app = builder.Build();

    // Auto-migrate on startup (works on Render)
    if (!app.Environment.IsEnvironment("Testing"))
    {
        using var scope = app.Services.CreateScope();
        var db     = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        try
        {
            logger.LogInformation("Applying EF Core migrations (PostgreSQL)...");
            await db.Database.MigrateAsync();
            logger.LogInformation("Database ready. Tables: users, quantity_measurements");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "MIGRATION FAILED: {Msg}. Check DATABASE_URL env var on Render.", ex.Message);
            if (app.Environment.IsDevelopment()) throw;
        }
    }

    app.UseCors("AllowFrontend");
    app.UseExceptionHandler();
    app.UseSerilogRequestLogging(opts =>
        opts.MessageTemplate = "HTTP {RequestMethod} {RequestPath} → {StatusCode} in {Elapsed:0.0000} ms");

    // Swagger enabled for all environments on Render
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Quantity Measurement API v1");
        c.RoutePrefix = "swagger";
        c.DisplayRequestDuration();
    });

    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    if (!app.Environment.IsEnvironment("Testing"))
    {
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                var result = new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(e => new
                    {
                        name        = e.Key,
                        status      = e.Value.Status.ToString(),
                        description = e.Value.Description ?? string.Empty,
                        exception   = e.Value.Exception?.Message
                    }),
                    duration = report.TotalDuration.TotalMilliseconds
                };
                await context.Response.WriteAsync(
                    JsonSerializer.Serialize(result, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
            }
        });
    }

    app.Lifetime.ApplicationStarted.Register(() =>
    {
        var urls = app.Urls.Any()
            ? app.Urls
            : new[] { $"http://localhost:{port}" };

        foreach (var url in urls)
        {
            Log.Information("🚀 Swagger UI: {SwaggerUrl}", $"{url}/swagger");
            Log.Information("❤️ Health Check: {HealthUrl}", $"{url}/health");
        }
    });

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly: {Msg}", ex.Message);
}
finally
{
    Log.CloseAndFlush();
}

/// <summary>Exposed for WebApplicationFactory integration tests.</summary>
public partial class Program { }