namespace LabManagement.Api.DTOs;

public class CreateLabWorkRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTimeOffset Deadline { get; set; }
    public Guid GroupId { get; set; }
    
    public IReadOnlyList<IFormFile> Files { get; set; } = null!; 
}