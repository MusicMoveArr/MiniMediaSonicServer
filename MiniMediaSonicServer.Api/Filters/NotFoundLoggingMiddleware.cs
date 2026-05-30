namespace MiniMediaSonicServer.Api.Filters;

public class NotFoundLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<NotFoundLoggingMiddleware> _logger;

    public NotFoundLoggingMiddleware(RequestDelegate next, ILogger<NotFoundLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        await _next(context);

        if (context.Response.StatusCode == StatusCodes.Status404NotFound)
        {
            _logger.LogWarning(
                "404 Not Found: {Method} {Path} from {IP}",
                context.Request.Method,
                context.Request.Path,
                context.Connection.RemoteIpAddress
            );
        }
    }
}