using LabManagement.App.Domain.Entities;
using LabManagement.App.Domain.Enums;
using MediatR;
using Microsoft.IdentityModel.Tokens;

namespace LabManagement.App.Features.Submissions.SubmitWork;

public class SubmitWorkCommand : IRequest<Submission>
{
    public Guid LabWorkId { get; set; }
    public Guid StudentId { get; set; }
    public Stream FileStream { get; set; } = Stream.Null;
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
}