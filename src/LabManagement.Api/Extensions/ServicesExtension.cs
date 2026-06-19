using LabManagement.Api.Services;

namespace LabManagement.Api.Extensions;

public static class ServicesExtension
{
    public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
    {

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
        return builder;
    }
}