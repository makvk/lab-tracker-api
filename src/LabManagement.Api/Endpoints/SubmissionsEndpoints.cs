using LabManagement.Api.Services;
using LabManagement.App.Common;
using LabManagement.App.Common.Exceptions;
using LabManagement.App.Domain.Entities;
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

        app.MapGet("api/submissions/{id:guid}", async (
            Guid id,
            ILabDbContext labDbContext,
            CancellationToken cancellationToken) =>
        {
            Submission? submission = await labDbContext.GetSubmissionByIdAsync(id, cancellationToken);
            if (submission == null)
            {
                return Results.NotFound("Работа не найдена");
            }
            return Results.Ok(submission);
        });

        app.MapGet("api/submissions/work/{workId:guid}/student/{studentId:guid}", async (
            Guid workId,
            Guid studentId,
            ILabDbContext labDbContext,
            CancellationToken cancellationToken
        ) => {
            Submission? submission = await labDbContext.GetSubmissionByWorkAndStudentAsync(
                workId,
                studentId,
                cancellationToken
            );
            if (submission == null)
            {
                return Results.NotFound("Не существует такой сдачи");
            }
            return Results.Ok(submission);
        });

        return app;
    }
}