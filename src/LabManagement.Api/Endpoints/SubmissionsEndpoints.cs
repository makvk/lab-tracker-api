using LabManagement.App.Common.Exceptions;
using LabManagement.App.Features.Submissions.GradeSubmission;
using LabManagement.App.Features.Submissions.TakeSubmissionInWork;
using MediatR;
using Microsoft.AspNetCore.Authorization;


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

        return app;
    }
}