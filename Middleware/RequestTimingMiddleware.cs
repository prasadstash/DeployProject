using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace DeployProject.Middleware
{
    public class RequestTimingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestTimingMiddleware> _logger;

        public RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            // Continue down the pipeline
            await _next(context);

            stopwatch.Stop();

            var elapsedMs = stopwatch.ElapsedMilliseconds;

            // Add response header
            context.Response.Headers["X-Response-Time-ms"] = elapsedMs.ToString();

            // Log only if request is slow
            if (elapsedMs > 500)
            {
                _logger.LogWarning("⏱️ Slow Request: {Method} {Path} took {Elapsed}ms",
                    context.Request.Method,
                    context.Request.Path,
                    elapsedMs);
            }
            else
            {
                _logger.LogInformation("✅ Request: {Method} {Path} took {Elapsed}ms",
                    context.Request.Method,
                    context.Request.Path,
                    elapsedMs);
            }
        }
    }
}
