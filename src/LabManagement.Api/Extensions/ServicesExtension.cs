using LabManagement.Api.Services;
using LabManagement.App.Common;

namespace LabManagement.Api.Extensions;

public static class ServicesExtension
{
    public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
        builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        return builder;
    }
}