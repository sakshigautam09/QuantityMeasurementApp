using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
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
            _env = env;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            // Get full exception message (including inner exceptions)
            string fullMessage = BuildFullMessage(exception);

            int status;

            // Map exception → HTTP status code
            (status, fullMessage) = exception switch
            {
                UnauthorizedAccessException   => (401, exception.Message),
                InvalidOperationException     => (409, exception.Message),
                ArgumentException             => (400, exception.Message),
                KeyNotFoundException          => (404, exception.Message),
                QuantityMeasurementException  => (400, exception.Message),
                _                             => (500, fullMessage)
            };

            // Log error
            _logger.LogError(exception,
                "[GlobalExceptionHandler] {Status} | {ExType} | {Message} | {Method} {Path}",
                status,
                exception.GetType().FullName,
                fullMessage,
                httpContext.Request.Method,
                httpContext.Request.Path);

            httpContext.Response.StatusCode = status;
            httpContext.Response.ContentType = "application/json";

            var response = new ErrorResponseDto
            {
                Timestamp = DateTime.UtcNow,
                Status = status,
                Error = status switch
                {
                    400 => "Bad Request",
                    401 => "Unauthorized",
                    403 => "Forbidden",
                    404 => "Not Found",
                    409 => "Conflict",
                    500 => "Internal Server Error",
                    _   => "Internal Server Error"
                },
                Message = fullMessage,
                Path = httpContext.Request.Path
            };

            await httpContext.Response.WriteAsync(
                JsonSerializer.Serialize(response, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }),
                cancellationToken
            );

            return true;
        }

        /// <summary>
        /// Builds full exception message including inner exceptions
        /// </summary>
        private static string BuildFullMessage(Exception ex)
        {
            var messages = new List<string>();
            var current = ex;
            int depth = 0;

            while (current != null && depth < 5)
            {
                messages.Add($"[{current.GetType().Name}] {current.Message}");
                current = current.InnerException;
                depth++;
            }

            return string.Join(" → ", messages);
        }
    }
}