using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using LabManagement.Api.Services;
using Microsoft.AspNetCore.Identity.Data;
using LabManagement.App.Common;
using LabManagement.App.Features.Auth.Login;
using MediatR;

namespace LabManagement.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.Map("/", () =>
        {
            return Results.Redirect("/scalar");
        });

        app.MapGet("/api/auth-test", (ICurrentUserService currentUser) =>
        {
            return Results.Ok($"Вы {currentUser.Role}");
        }).RequireAuthorization(
            new AuthorizeAttribute {Roles = "Teacher,Student"}
        );

        app.MapGet("api/auth/test-token-teacher", (IConfiguration configuration) =>
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret не найден!");
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "99999999-9999-9999-9999-999999999999"), // ID преподавателя
                new Claim(ClaimTypes.Name, "Иван Иванович (Преподаватель)"),
                new Claim(ClaimTypes.Role, "Teacher") // Наша ключевая роль!
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return Results.Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        });

        app.MapPost("/api/auth/login", async (LoginCommand command, 
            IConfiguration configuration, 
            ILabDbContext labDbContext,
            IMediator mediator) =>
        {
            AuthResult res = await mediator.Send(command);
            return Results.Ok(res);
        });

        return app;
    }
}