using LabManagement.Api.Services;
using LabManagement.App.Common;
using LabManagement.App.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LabManagement.Api.Endpoints;

public static class GroupEndpoints
{
    public static IEndpointRouteBuilder MapGroupEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/groups", (ILabDbContext labDbContext) => {
            return Results.Ok(labDbContext.Groups);
        });

        app.MapGet("/api/groups/{id:guid}", async (
            Guid id,
            ILabDbContext labDbContext,
            CancellationToken cancellationToken) =>
        {
            Group? group = await labDbContext.GetGroupByIdAsync(id, cancellationToken);
            if (group == null)
            {
                return Results.NotFound("Группа не найдена");
            }
            return Results.Ok(group);
        });

        app.MapGet("/api/groups/{id:guid}/works", async (
            Guid id,
            ILabDbContext labDbContext,
            ICurrentUserService currentUserService) =>
        {
            switch (currentUserService.Role) {
                case "Teacher":
                    var labWorks = await labDbContext.LabWorks.Where(
                        lw => lw.GroupId == id && lw.TeacherId == currentUserService.UserId
                        ).ToListAsync();
                    return Results.Ok(labWorks);
                case "Student":
                    labWorks = await labDbContext.LabWorks.Where(
                        lw => lw.GroupId == id
                        ).ToListAsync();
                    return Results.Ok(labWorks);
                default:
                    return Results.BadRequest();
            }
        });

        return app;
    }
}