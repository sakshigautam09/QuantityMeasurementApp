// ============================================================
// PROJECT : QuantityMeasurementApp.API
// FILE    : Program.cs
// UC-17   : ASP.NET Core Web API bootstrap.
// ============================================================

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QuantityMeasurementApp.API.Exception;
using QuantityMeasurementApp.BusinessLayer;
using QuantityMeasurementApp.BusinessLayer.Interface;
using QuantityMeasurementApp.BusinessLayer.Services;
using QuantityMeasurementApp.Core.Interfaces;
using QuantityMeasurementApp.Core.Services;
using QuantityMeasurementApp.RepositoryLayer.Context;
using QuantityMeasurementApp.RepositoryLayer.Interface;
using QuantityMeasurementApp.RepositoryLayer.Repository;
using QuantityMeasurementApp.RepositoryLayer.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// ── Controllers ───────────────────────────────────────────────────────────────
builder.Services.AddControllers();

// ══════════════════════════════════════════════════════════════════════════════
// EF CORE
// ══════════════════════════════════════════════════════════════════════════════
builder.Services.AddDbContext<QuantityMeasurementDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("QuantityMeasurementDb"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount:     3,
            maxRetryDelay:     TimeSpan.FromSeconds(5),
            errorNumbersToAdd: null)));

// ══════════════════════════════════════════════════════════════════════════════
// REDIS — graceful fallback if Redis not running
// ══════════════════════════════════════════════════════════════════════════════
var redisConnStr = builder.Configuration["Redis:ConnectionString"] ?? "localhost:6379";

try
{
    var redisConfig = ConfigurationOptions.Parse(redisConnStr);
    redisConfig.AbortOnConnectFail = false;
    redisConfig.ConnectRetry       = 3;
    redisConfig.ConnectTimeout     = 3000;

    var multiplexer = ConnectionMultiplexer.Connect(redisConfig);
    builder.Services.AddSingleton<IConnectionMultiplexer>(multiplexer);

    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisConnStr;
        options.InstanceName  = builder.Configuration["Redis:InstanceName"]
                                ?? "QuantityMeasurementApp:";
    });

    Console.WriteLine("[Redis] Connected successfully.");
}
catch (Exception ex)
{
    Console.WriteLine($"[Redis] Not available: {ex.Message}");
    Console.WriteLine("[Redis] Falling back to in-memory cache.");
    builder.Services.AddDistributedMemoryCache();
}

// ── JWT Authentication ────────────────────────────────────────────────────────
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key missing from appsettings.json");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken            = false;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer           = true,
        ValidateAudience         = true,
        ValidateLifetime         = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer              = builder.Configuration["Jwt:Issuer"],
        ValidAudience            = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey         = new SymmetricSecurityKey(
                                       Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew                = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnChallenge = ctx =>
        {
            ctx.HandleResponse();
            ctx.Response.StatusCode  = 401;
            ctx.Response.ContentType = "application/json";
            return ctx.Response.WriteAsync(
                """{"statusCode":401,"error":"Unauthorized","message":"Missing or invalid JWT token. Login via POST /api/auth/login"}""");
        }
    };
});

builder.Services.AddAuthorization();

// ── Swagger / OpenAPI ─────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title       = "Quantity Measurement API",
        Version     = "v1",
        Description =
            "REST API for quantity measurement (Length, Weight, Volume, Temperature).\n\n" +
            "**Auth flow:**\n" +
            "1. `POST /api/auth/register` — Sign Up\n" +
            "2. `POST /api/auth/login` — Sign In → copy the `token`\n" +
            "3. Click **Authorize** → paste token (without 'Bearer' prefix)\n" +
            "4. All `/api/v1/quantities/*` endpoints are now accessible."
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name         = "Authorization",
        Type         = SecuritySchemeType.Http,
        Scheme       = "Bearer",
        BearerFormat = "JWT",
        In           = ParameterLocation.Header,
        Description  = "Paste JWT token here (without 'Bearer' prefix)."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath)) c.IncludeXmlComments(xmlPath);
});

// ── CORS ──────────────────────────────────────────────────────────────────────
builder.Services.AddCors(o => o.AddPolicy("AllowAll", p =>
    p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

// ══════════════════════════════════════════════════════════════════════════════
// DEPENDENCY INJECTION — Repository Layer
// ══════════════════════════════════════════════════════════════════════════════
builder.Services.AddScoped<IQuantityMeasurementRepository, QuantityMeasurementEfRepository>();
builder.Services.AddScoped<IUserRepository,                UserEfRepository>();
builder.Services.AddScoped<IRedisCache,                    RedisCacheService>();

// ══════════════════════════════════════════════════════════════════════════════
// DEPENDENCY INJECTION — Business Layer
// ══════════════════════════════════════════════════════════════════════════════
builder.Services.AddScoped<IQuantityModelService,       QuantityModelServiceImpl>();
builder.Services.AddScoped<ITemperatureService,         TemperatureService>();
builder.Services.AddScoped<IQuantityMeasurementService, QuantityMeasurementServiceImpl>();

// ══════════════════════════════════════════════════════════════════════════════
// DEPENDENCY INJECTION — Auth Layer
// ══════════════════════════════════════════════════════════════════════════════
builder.Services.AddScoped<IPasswordHasher,    BCryptPasswordHasher>();
builder.Services.AddScoped<IEncryptionService, AesEncryptionService>();
builder.Services.AddScoped<IJwtService,        JwtService>();
builder.Services.AddScoped<IAuthService,       AuthService>();

// ══════════════════════════════════════════════════════════════════════════════
// BUILD & PIPELINE
// ══════════════════════════════════════════════════════════════════════════════
var app = builder.Build();

// Auto-run EF Core migrations on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
                  .GetRequiredService<QuantityMeasurementDbContext>();
    db.Database.Migrate();
}

// GlobalExceptionHandler MUST be first in pipeline
app.UseMiddleware<GlobalExceptionHandler>();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Quantity Measurement API v1");
    c.RoutePrefix   = "swagger";
    c.DocumentTitle = "Quantity Measurement API";
});

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("╔══════════════════════════════════════════════════════╗");
Console.WriteLine("║      QuantityMeasurementApp.API  is running          ║");
Console.WriteLine("╠══════════════════════════════════════════════════════╣");
Console.WriteLine("║  Swagger UI  :  http://localhost:5000/swagger        ║");
Console.WriteLine("║  ORM         :  Entity Framework Core                ║");
Console.WriteLine("║  Cache       :  Redis (localhost:6379)               ║");
Console.WriteLine("║  Security    :  BCrypt + AES-256 + JWT               ║");
Console.WriteLine("║                                                      ║");
Console.WriteLine("║  1. POST /api/auth/register   → Sign Up              ║");
Console.WriteLine("║  2. POST /api/auth/login      → Get JWT token        ║");
Console.WriteLine("║  3. Authorize in Swagger      → Paste token          ║");
Console.WriteLine("║  4. Use /api/v1/quantities/*  → All operations       ║");
Console.WriteLine("╚══════════════════════════════════════════════════════╝");
Console.ResetColor();

app.Run();