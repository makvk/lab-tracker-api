using System.IdentityModel.Tokens.Jwt;
using LabManagement.App.Common.Exceptions;
using System.Security.Claims;
using LabManagement.App.Features.Submissions.GradeSubmission;
using LabManagement.App.Features.Submissions.TakeSubmissionInWork;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace LabManagement.Api.Endpoints;

public static class SubmissionsEndpoints
{
    public static IEndpointRouteBuilder MapSubmissionsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("api/submissions/{id:guid}/grade", async (
            Guid id, 
            GradeSubmissionCommand command, 
            IMediator mediator) =>
        {
            try
            {
                command.SubmissionId = id;
                var updatedSubmission = await mediator.Send(command);
                return Results.Ok(updatedSubmission);
            } 
            catch (NotFoundException ex)
            {
                return Results.NotFound(ex.Message);
            }
        }).RequireAuthorization(new AuthorizeAttribute {Roles = "Teacher"});

        app.MapPost("api/submissions/{id:guid}/take-in-work", async (
            Guid id, 
            TakeSubmissionInWorkCommand command, 
            IMediator mediator) =>
        {
            try{
                command.SubmissionId = id;
                var updatedSubmission = await mediator.Send(command);
                return Results.Ok(updatedSubmission);
            } 
            catch (NotFoundException ex)
            {
                return Results.NotFound(ex.Message);
            }
        }).RequireAuthorization(new AuthorizeAttribute {Roles = "Teacher"});

        app.MapGet("/api/auth-test", () =>
        {
            return Results.Ok("Авторизация корректна");
        }).RequireAuthorization(new AuthorizeAttribute {Roles = "Teacher"});

        app.MapGet("api/auth/test-token-teacher", () =>
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "99999999-9999-9999-9999-999999999999"), // ID преподавателя
                new Claim(ClaimTypes.Name, "Иван Иванович (Преподаватель)"),
                new Claim(ClaimTypes.Role, "Teacher") // Наша ключевая роль!
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SuperSecretKey12345678901234567890!"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "LabManagementAuthServer",
                audience: "LabManagementApi",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return Results.Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        });
        
        return app;
    }
}