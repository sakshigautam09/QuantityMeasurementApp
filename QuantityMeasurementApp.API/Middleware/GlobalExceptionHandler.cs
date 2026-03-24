// ============================================================
// PROJECT : QuantityMeasurementApp.API
// FILE    : Exception/GlobalExceptionHandler.cs
// UC-17   : Centralized exception handling for ALL controllers.
//
// WHY THIS FILE IS IN Exception/ FOLDER (NOT Middleware/):
//   Handles business/application exceptions — belongs with
//   exception code, not with pipeline middleware (auth, CORS).
//   Equivalent to Spring's @ControllerAdvice + @ExceptionHandler.
//
// WHY THIS FILE EXISTS:
//   Without this, every controller needs its own try-catch.
//   This catches ALL unhandled exceptions and returns a
//   consistent JSON error response format every time.
//
// REGISTERED IN Program.cs as:
//   app.UseMiddleware<GlobalExceptionHandler>();
// ============================================================

using System.Net;
using System.Text.Json;
using QuantityMeasurementApp.BusinessLayer;
using QuantityMeasurementApp.ModelLayer.DTO;  

namespace QuantityMeasurementApp.API.Exception
{
    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate                 _next;
        private readonly ILogger<GlobalExceptionHandler> _logger;

        // Constructor — ASP.NET Core injects RequestDelegate automatically
        public GlobalExceptionHandler(
            RequestDelegate                  next,
            ILogger<GlobalExceptionHandler>  logger)
        {
            _next   = next;
            _logger = logger;
        }

        // InvokeAsync — called for every HTTP request in the pipeline
        public async Task InvokeAsync(HttpContext ctx)
        {
            try
            {
                await _next(ctx);   // pass to next middleware/controller
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex,
                    "Unhandled exception on {Method} {Path}",
                    ctx.Request.Method, ctx.Request.Path);

                await WriteErrorResponse(ctx, ex);
            }
        }

        // ── Map exception type → HTTP status + error label ────────────────────────

        private static Task WriteErrorResponse(HttpContext ctx, System.Exception ex)
        {
            var (code, error) = ex switch
            {
                QuantityMeasurementException =>
                    (HttpStatusCode.BadRequest,          "Quantity Measurement Error"),

                ArgumentException =>
                    (HttpStatusCode.BadRequest,          "Invalid Input"),

                UnauthorizedAccessException =>
                    (HttpStatusCode.Unauthorized,        "Unauthorized"),

                KeyNotFoundException =>
                    (HttpStatusCode.NotFound,            "Not Found"),

                _ =>
                    (HttpStatusCode.InternalServerError, "Internal Server Error")
            };

            ctx.Response.StatusCode  = (int)code;
            ctx.Response.ContentType = "application/json";

            var body = JsonSerializer.Serialize(
                new ApiErrorResponse
                {
                    StatusCode = (int)code,
                    Error      = error,
                    Message    = ex.Message,
                    Timestamp  = DateTime.UtcNow
                },
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

            return ctx.Response.WriteAsync(body);
        }
    }
}