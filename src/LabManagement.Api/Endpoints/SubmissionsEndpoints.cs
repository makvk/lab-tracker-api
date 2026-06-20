using LabManagement.Api.Services;
using LabManagement.App.Common.Exceptions;
using LabManagement.App.Features.Submissions.GradeSubmission;
using LabManagement.App.Features.Submissions.SubmitWork;
using LabManagement.App.Features.Submissions.TakeSubmissionInWork;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace LabManagement.Api.Endpoints;

public static class SubmissionsEndpoints
{
    public static IEndpointRouteBuilder MapSubmissionsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPut("api/submissions/{id:guid}/grade", async (
            Guid id, 
            GradeSubmissionCommand command, 
            IMediator mediator,
            ICurrentUserService currentUserService) =>
        {
            try
            {
                command.TeacherId = currentUserService.UserId ?? Guid.Empty;
                command.SubmissionId = id;
                var updatedSubmission = await mediator.Send(command);
                return Results.Ok(updatedSubmission);
            } 
            catch (NotFoundException ex)
            {
                return Results.NotFound(ex.Message);
            }
        }).RequireAuthorization(new AuthorizeAttribute {Roles = "Teacher"});

        app.MapPut("api/submissions/{id:guid}/take-in-work", async (
            Guid id, 
            IMediator mediator,
            ICurrentUserService currentUserService) =>
        {
            try{
                var command = new TakeSubmissionInWorkCommand{
                    TeacherId = currentUserService.UserId ?? Guid.Empty,
                    SubmissionId = id,
                };
                var updatedSubmission = await mediator.Send(command);
                return Results.Ok(updatedSubmission);
            } 
            catch (NotFoundException ex)
            {
                return Results.NotFound(ex.Message);
            }
        }).RequireAuthorization(new AuthorizeAttribute {Roles = "Teacher"});

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

        return app;
    }
}