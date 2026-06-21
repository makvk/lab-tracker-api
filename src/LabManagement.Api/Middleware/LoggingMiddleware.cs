namespace LabManagement.Api.Middleware;

public class LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<LoggingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation("Начало обработки запроса");
        await _next.Invoke(context);
        _logger.LogInformation("Конец обработки запроса");
    }
}