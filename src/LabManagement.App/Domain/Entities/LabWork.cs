namespace LabManagement.App.Domain.Entities;

public class LabWork
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string FilePath { get; private set; } = string.Empty;
    public Guid GroupId { get; private set; }
    public Guid TeacherId { get; init; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset Deadline { get; private set; }
}