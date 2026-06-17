using LabManagement.App.Domain.Entities;
using System.Text.Json.Serialization;
using MediatR;

namespace LabManagement.App.Features.Submissions.TakeSubmissionInWork;

public class TakeSubmissionInWorkCommand: IRequest<Submission>
{
    [JsonIgnore]
    public Guid SubmissionId { get; set; }
    public Guid TeacherId { get; set; }
}