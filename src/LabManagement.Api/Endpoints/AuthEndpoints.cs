using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using LabManagement.Api.Services;
using LabManagement.App.Common;
using LabManagement.App.Features.Auth.Login;
using MediatR;
using LabManagement.App.Domain.Entities;


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

        app.MapPost("/api/auth/login", async (LoginCommand command, 
            IConfiguration configuration, 
            ILabDbContext labDbContext,
            IMediator mediator) =>
        {
            AuthResult res = await mediator.Send(command);
            return Results.Ok(res);
        });

        app.MapGet("api/auth/me", async (
            ICurrentUserService currentUserService,
            ILabDbContext labDbContext,
            CancellationToken cancellationToken
        ) => 
        {
            Guid userId = currentUserService.UserId ?? Guid.Empty;

            if (currentUserService.Role == "Student")
            {
                Student? student = await labDbContext.GetStudentByIdAsync(userId, cancellationToken);
                if (student == null)
                {
                    return Results.NotFound();
                }
                Group? group = await labDbContext.GetGroupByIdAsync(student.GroupId, cancellationToken);
                string groupName = string.Empty;
                Guid groupId = Guid.Empty;
                if (group != null)
                {
                    groupName = group.Name;
                    groupId = group.Id;
                }
                return Results.Ok(
                    new Dictionary<string, object>
                    {
                        {"userId", student.Id.ToString()},
                        {"role", "Student"},
                        {"name", student.FirstName},
                        {"group", groupName},
                        {"groupId", groupId.ToString()},
                    }
                );
            } else if (currentUserService.Role == "Teacher")
            {
                Teacher? teacher = await labDbContext.GetTeacherByIdAsync(userId, cancellationToken);
                if (teacher == null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(
                    new Dictionary<string, string>
                    {
                        {"userId", teacher.Id.ToString()},
                        {"role", "Teacher"},
                        {"name", teacher.FirstName + ' ' + teacher.LastName},
                    }
                );
            }
            return Results.NotFound();

        }).RequireAuthorization(
            new AuthorizeAttribute {Roles = "Teacher,Student"}
        );

        return app;
    }
}