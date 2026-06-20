using LabManagement.App.Common;
using LabManagement.App.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabManagement.Api.Endpoints;

public static class BuisnessEndpoints
{
    public static IEndpointRouteBuilder MapBuisnessEndpoints(this IEndpointRouteBuilder app)
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
            ILabDbContext labDbContext) =>
        {
            var labWorks = await labDbContext.LabWorks.Where(lw => lw.GroupId == id).ToListAsync();

            return Results.Ok(labWorks);
        });

        app.MapGet("/api/works/{id:guid}", async (
            Guid id,
            ILabDbContext labDbContext) =>
        {
            LabWork? labWork = labDbContext.LabWorks.FirstOrDefault(lw => lw.Id == id);

            if (labWork == null)
            {
                return Results.NotFound("Работа не найдена");
            }

            return Results.Ok(labWork);
        });

        app.MapGet("/api/works/{id:guid}/submissions", async (
            Guid id,
            ILabDbContext labDbContext) =>
        {
            List<Submission> submissions = await labDbContext.Submissions.Where(s => s.LabWorkId == id).ToListAsync();

            if (submissions== null)
            {
                return Results.NotFound("Работа не найдена");
            }

            return Results.Ok(submissions);
        });
        return app;
    }
}