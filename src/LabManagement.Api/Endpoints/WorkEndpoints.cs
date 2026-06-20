using LabManagement.Api.Services;
using LabManagement.App.Common;
using LabManagement.App.Domain.Entities;
using LabManagement.App.Features.Submissions.SubmitWork;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabManagement.Api.Endpoints;

public static class WorkEndpoints
{
    public static IEndpointRouteBuilder MapWorkEndpoints(this IEndpointRouteBuilder app)
    {
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

        app.MapPost("/api/works/{id:guid}/submit", async (
            [FromRoute] Guid id,
            IFormFile file, // Принимаем файл через форму 
            [FromServices] IMediator mediator,
            [FromServices] ICurrentUserService currentUserService,
            CancellationToken cancellationToken) =>
        {
            // Валидация на то, что файл вообще пришел
            if (file == null || file.Length == 0)
            {
                return Results.BadRequest(new { error = "Файл не выбран или пуст" });
            }

            // Открываем поток для чтения файла
            using var stream = file.OpenReadStream();

            var studentId = currentUserService.UserId ?? Guid.Empty;

            var command = new SubmitWorkCommand
            {
                LabWorkId = id,
                StudentId = studentId,
                FileStream = stream,
                FileName = file.FileName,
                ContentType = file.ContentType
            };

            var submission = await mediator.Send(command, cancellationToken);

            // Возвращаем 201 Created. Первый параметр — URL созданного ресурса, второй — сам объект
            return Results.Created($"/api/submissions/{submission.Id}", submission);
        })
        .RequireAuthorization(new AuthorizeAttribute { Roles = "Student" })
        .DisableAntiforgery()
        .Accepts<IFormFile>("multipart/form-data");

        // app.MapPost("api/works/create-work", (
        //     IFormFile file,

        // ) => {});
        return app;
    }
}