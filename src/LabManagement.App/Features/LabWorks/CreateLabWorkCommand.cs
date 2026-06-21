using LabManagement.App.Domain.Entities;
using MediatR;

namespace LabManagement.App.Features.LabWorks;

public record FileUploadModel(Stream Stream, string FileName);

public class CreateLabWorkCommand : IRequest<LabWork>
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTimeOffset Deadline { get; set; }
    public Guid GroupId { get; set; }
    public Guid TeacherId { get; set; }
    public List<FileUploadModel> Files { get; set; } = [];
}