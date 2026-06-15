using MediatR;

namespace LabManagement.App.Features.Submissions.GradeSubmission;

public class GradeSubmissionCommand: IRequest
{
    public Guid SubmissionId { get; init; }
    public int Grade { get; init; }
    public string? Comment { get; init; }
    public Guid TeacherId { get; init; }
}