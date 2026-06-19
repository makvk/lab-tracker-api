using System.Text.Json.Serialization;
using LabManagement.App.Domain.Entities;
using MediatR;

namespace LabManagement.App.Features.Submissions.GradeSubmission;

public class GradeSubmissionCommand: IRequest<Submission>
{
    [JsonIgnore]
    public Guid SubmissionId { get; set; }
    [JsonIgnore]
    public Guid TeacherId { get; set; }
    public int Grade { get; init; }
    public string? Comment { get; init; }
    
}