using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Data.SqlClient;
using QuantityMeasurementBusinessLayer;
using QuantityMeasurementModel.Dto;

namespace QuantityMeasurementWebAPI.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IWebHostEnvironment _env;

        public GlobalExceptionHandler(
            ILogger<GlobalExceptionHandler> logger,
            IWebHostEnvironment env)
        {
            _logger = logger;
            _env    = env;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception   exception,
            CancellationToken cancellationToken)
        {
            // Walk the entire inner exception chain for the real SQL error
            string fullMessage = BuildFullMessage(exception);

            int status;
            if (FindSqlException(exception) is { } sqlEx)
            {
                status = 503;
                fullMessage =
                    $"{sqlEx.Message} (SQL error {sqlEx.Number}). " +
                    "Ensure SQL Server is running and ConnectionStrings:QuantityMeasurementDb matches your instance. " +
                    "Try (localdb)\\MSSQLLocalDB for Visual Studio LocalDB, or localhost\\SQLEXPRESS for SQL Express. " +
                    "Apply schema: dotnet ef database update --project QuantityMeasurementRepository --startup-project QuantityMeasurementWebAPI";
            }
            else
            {
                (status, fullMessage) = exception switch
                {
                    UnauthorizedAccessException   => (401, exception.Message),
                    InvalidOperationException       => (409, exception.Message),
                    ArgumentException               => (400, exception.Message),
                    KeyNotFoundException            => (404, exception.Message),
                    QuantityMeasurementException    => (400, exception.Message),
                    _                               => (500, fullMessage)
                };
            }

            // Log full details including inner exceptions and stack trace
            _logger.LogError(exception,
                "[GlobalExceptionHandler] {Status} | {ExType} | {FullMsg} | {Method} {Path}",
                status,
                exception.GetType().FullName,
                fullMessage,
                httpContext.Request.Method,
                httpContext.Request.Path);

            httpContext.Response.StatusCode  = status;
            httpContext.Response.ContentType = "application/json";

            var body = new ErrorResponseDto
            {
                Timestamp = DateTime.UtcNow,
                Status    = status,
                Error = status switch
                {
                    400 => "Bad Request",
                    401 => "Unauthorized",
                    403 => "Forbidden",
                    404 => "Not Found",
                    409 => "Conflict",
                    503 => "Service Unavailable",
                    _   => "Internal Server Error"
                },
                Message = fullMessage,   // Always show full message (inner exceptions included)
                Path    = httpContext.Request.Path
            };

            await httpContext.Response.WriteAsync(
                JsonSerializer.Serialize(body,
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    }),
                cancellationToken);

            return true;
        }

        /// <summary>
        /// Walks the full InnerException chain and concatenates all messages.
        /// This surfaces the real SQL Server error (table not found, etc).
        /// </summary>
        private static string BuildFullMessage(Exception ex)
        {
            var parts = new List<string>();
            var current = ex;
            int depth = 0;
            while (current != null && depth < 5)
            {
                parts.Add($"[{current.GetType().Name}] {current.Message}");
                current = current.InnerException;
                depth++;
            }
            return string.Join(" → ", parts);
        }

        private static SqlException? FindSqlException(Exception? ex)
        {
            while (ex != null)
            {
                if (ex is SqlException sql) return sql;
                ex = ex.InnerException;
            }
            return null;
        }
    }
}