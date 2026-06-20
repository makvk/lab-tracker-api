using LabManagement.Api.Endpoints;

namespace LabManagement.Api.Extensions;

public static class EndpointExtension
{
    public static IEndpointRouteBuilder AddEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapSubmissionsEndpoints();
        app.MapAuthEndpoints();
        app.MapBuisnessEndpoints();
        return app;
    }
}