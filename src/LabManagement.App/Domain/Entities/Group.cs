namespace LabManagement.App.Domain.Entities;

public class Group
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty; 
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
}