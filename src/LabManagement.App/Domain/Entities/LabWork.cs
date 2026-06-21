using System.Security.Cryptography.X509Certificates;

namespace LabManagement.App.Domain.Entities;

public class LabWork
{
    public LabWork(
        string title,
        string description,
        Guid groupId,
        Guid teacherId,
        string filePath,
        DateTimeOffset deadline
    )
    {
        Title = title;
        Description = description;
        GroupId = groupId;
        TeacherId = teacherId;
        FilePath = filePath;
        Deadline = deadline;
        CreatedAt = DateTimeOffset.UtcNow;
    }
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    
    // --- СВЯЗЬ С ГРУППОЙ ЧЕРЕЗ ГУИД ---
    public Guid GroupId { get; set; }
    public Group Group { get; set; } = null!;
    
    public Guid TeacherId { get; set; }
    public Teacher Teacher { get; set; } = null!; 
    
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset Deadline { get; set; }
}