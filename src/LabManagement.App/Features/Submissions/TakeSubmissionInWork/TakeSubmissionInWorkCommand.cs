using MediatR;

namespace LabManagement.App.Features.Submissions.TakeSubmissionInWork;

public class TakeSubmissionInWorkCommand: IRequest
{
    public Guid SubmissionId { get; init; }
    public Guid TeacherId { get; init; }
}