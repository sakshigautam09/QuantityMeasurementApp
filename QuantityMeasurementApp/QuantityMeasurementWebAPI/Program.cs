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
    Log.Information("=== Quantity Measurement API (UC17) starting ===");

    var builder = WebApplication.CreateBuilder(args);

    // Single source of truth: WebAPI/Config (copied to output). Ensures EF uses the same SQL Server you configure for SSMS.
    var configDir = Path.Combine(builder.Environment.ContentRootPath, "Config");
    builder.Configuration
        .AddJsonFile(Path.Combine(configDir, "appsettings.json"), optional: false, reloadOnChange: true)
        .AddJsonFile(Path.Combine(configDir, $"appsettings.{builder.Environment.EnvironmentName}.json"), optional: true, reloadOnChange: true);

    builder.Host.UseSerilog((context, services, loggerConfiguration) =>
    {
        var logsRoot = Path.Combine(context.HostingEnvironment.ContentRootPath, "Logs");
        Directory.CreateDirectory(logsRoot);
        var logFile = Path.Combine(logsRoot, "quantityapi-.txt");

        loggerConfiguration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .WriteTo.File(logFile, rollingInterval: Serilog.RollingInterval.Day, retainedFileCountLimit: 14);
    });

    builder.Services.AddControllers()
        .AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = null);

    builder.Services.AddPersistence(builder.Configuration);
    builder.Services.AddBusinessServices();

    var config = builder.Configuration;
    string secret = config["JwtSettings:Secret"]
        ?? throw new InvalidOperationException("JwtSettings:Secret is not configured.");
    string issuer = config["JwtSettings:Issuer"] ?? "QuantityMeasurementAPI";
    string audience = config["JwtSettings:Audience"] ?? "QuantityMeasurementAPI";

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
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

    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Quantity Measurement API",
            Version = "v1",
            Description = "UC17: Clean Architecture | JWT Auth | BCrypt | Redis | EF Core → SQL Server"
        });
        var securityScheme = new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Description = "Enter: Bearer {your_jwt_token}",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT"
        };
        options.AddSecurityDefinition("Bearer", securityScheme);
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            { new OpenApiSecurityScheme { Reference = new OpenApiReference { Id = "Bearer", Type = ReferenceType.SecurityScheme } }, new List<string>() }
        });
        string xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath)) options.IncludeXmlComments(xmlPath);
    });
    builder.Services.AddEndpointsApiExplorer();

    if (!builder.Environment.IsEnvironment("Testing"))
    {
        string sqlConn = config.GetConnectionString("QuantityMeasurementDb") ?? string.Empty;
        string redisConn = config.GetConnectionString("Redis") ?? "localhost:6379";
        builder.Services.AddHealthChecks()
            .AddSqlServer(sqlConn, name: "sqlserver", tags: new[] { "db" })
            .AddRedis(redisConn, name: "redis", tags: new[] { "cache" });
    }

    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    builder.Services.AddCors(options =>
        options.AddPolicy("AllowAll", policy =>
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

    var app = builder.Build();

    if (!app.Environment.IsEnvironment("Testing"))
    {
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            try
            {
                logger.LogInformation("Applying EF Core migrations (Config/ConnectionStrings:QuantityMeasurementDb)...");
                await db.Database.MigrateAsync();
                logger.LogInformation("Database ready. Tables: users, quantity_measurements");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "MIGRATION FAILED: {Msg}. Fix WebAPI/Config/appsettings*.json then restart.", ex.Message);
                if (app.Environment.IsDevelopment())
                    throw new InvalidOperationException(
                        "Database migration failed. Update ConnectionStrings:QuantityMeasurementDb to match your SSMS server (see Config/DATABASE_SETUP.txt), then restart or run: dotnet ef database update --project QuantityMeasurementRepository --startup-project QuantityMeasurementWebAPI",
                        ex);
            }
        }
    }

    app.UseExceptionHandler();
    app.UseSerilogRequestLogging(opts =>
        opts.MessageTemplate = "HTTP {RequestMethod} {RequestPath} → {StatusCode} in {Elapsed:0.0000} ms");

    if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Testing"))
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Quantity Measurement API v1");
            c.RoutePrefix = "swagger";
            c.DisplayRequestDuration();
        });
    }

    app.UseCors("AllowAll");
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
                        name = e.Key,
                        status = e.Value.Status.ToString(),
                        description = e.Value.Description ?? string.Empty,
                        exception = e.Value.Exception?.Message
                    }),
                    duration = report.TotalDuration.TotalMilliseconds
                };
                await context.Response.WriteAsync(
                    JsonSerializer.Serialize(result, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
            }
        });
    }

    Log.Information("=== Swagger  → http://localhost:5000/swagger ===");
    Log.Information("=== Health   → http://localhost:5000/health   (SQL + Redis) ===");
    Log.Information("=== File logs → QuantityMeasurementWebAPI/Logs/ (rolling daily) ===");

    await app.RunAsync();
    Console.WriteLine("DB CONNECTION: " + config.GetConnectionString("QuantityMeasurementDb"));
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
