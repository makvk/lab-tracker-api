using System.Text.Json.Serialization;

namespace LabManagement.App.Domain.Entities;

public class Student
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    [JsonIgnore]
    public string PasswordHash { get; set; } = string.Empty;
    
    // --- СВЯЗЬ С ГРУППОЙ ЧЕРЕЗ ГУИД ---
    public Guid GroupId { get; set; }
    public Group Group { get; set; } = null!;
    
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
}