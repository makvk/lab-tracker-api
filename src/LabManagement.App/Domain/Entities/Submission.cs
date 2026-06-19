using LabManagement.App.Domain.Enums;

namespace LabManagement.App.Domain.Entities;

public class Submission
{
    // EF Core нужен пустой конструктор (можно приватный), чтобы собирать объект из базы
    private Submission() { }

    // Конструктор для создания новой сдачи в твоем коде
    public Submission(Guid labWorkId, Guid studentId, string filePath)
    {
        Id = Guid.NewGuid();
        LabWorkId = labWorkId;
        StudentId = studentId;
        FilePath = string.IsNullOrWhiteSpace(filePath) 
            ? throw new ArgumentException("Путь к файлу не может быть пустым") 
            : filePath;
        Status = SubmissionStatus.Submitted;
        SubmittedAt = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; init; }
    
    // --- FOREIGN KEYS ---
    public Guid LabWorkId { get; init; }
    public Guid StudentId { get; init; }
    public Guid? TeacherId { get; private set; }

    // --- НАВИГАЦИОННЫЕ СВОЙСТВА ---
    public LabWork LabWork { get; init; } = null!;
    public Student Student { get; init; } = null!;
    public Teacher? Teacher { get; private set; }

    // --- ОСТАЛЬНЫЕ СВОЙСТВА ---
    public string FilePath { get; private set; } = string.Empty; // Теперь меняется только через конструктор или спец. метод
    public SubmissionStatus Status { get; private set; }
    public DateTimeOffset SubmittedAt { get; init; }
    public int? Grade { get; private set; }
    public string? Comment { get; private set; }

    // --- ДОМЕННАЯ ЛОГИКА ---
    public void GradeWork(int grade, string? comment)
    {
        if (grade < 2 || grade > 5)
        {
            throw new ArgumentException("Оценка должна быть от 2 до 5");
        }
        
        if (Status != SubmissionStatus.Checking)
        {
            throw new InvalidOperationException("Нельзя оценить работу, которая еще не взята на проверку");
        }

        Grade = grade;
        Comment = comment;
        Status = SubmissionStatus.Graded;
    }

    public void TakeWork(Guid teacherId)
    {
        if (Status == SubmissionStatus.Checking || Status == SubmissionStatus.Graded)
        {
            throw new InvalidOperationException("Эту работу нельзя взять на проверку (уже проверяется или оценена)");
        }
        Status = SubmissionStatus.Checking;
        TeacherId = teacherId;
    }

    public void MarkAsExpired(DateTimeOffset deadline)
    {
        if (Status == SubmissionStatus.Graded || Status == SubmissionStatus.Checking)
        {
            return;
        }

        if (DateTimeOffset.UtcNow > deadline)
        {
            Status = SubmissionStatus.Expired;
        }
    }
}