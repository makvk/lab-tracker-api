using LabManagement.Api.Middleware;

namespace LabManagement.Api.Extensions;

public static class MiddlewareExtension
{
    public static WebApplication AddMiddleware(this WebApplication app)
    {
        app.UseMiddleware<LoggingMiddleware>();
        return app;
    }
}