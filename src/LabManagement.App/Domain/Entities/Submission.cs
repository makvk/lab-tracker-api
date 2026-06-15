using LabManagement.App.Domain.Enums;

namespace LabManagement.App.Domain.Entities;

public class Submission
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid LabWorkId { get; init; }
    public Guid StudentId { get; init; }
    public string FilePath { get; private set; } = string.Empty;
    public SubmissionStatus Status { get; private set; } = SubmissionStatus.Submitted;
    public DateTimeOffset SubmittedAt { get; init; } = DateTimeOffset.UtcNow;
    public int? Grade { get; private set; }
    public string? Comment { get; private set; }
    public Guid? TeacherId { get; private set; }
    public void GradeWork(int grade, string? comment)
    {
        if (Status != SubmissionStatus.Checking)
        {
            throw new InvalidOperationException("Нельзя оценить работу, которая не находится на проверке");
        }
        if ( !(2 <= grade && grade <= 5) )
        {
            throw new ArgumentException("Неверно указана оценка");
        }
        Grade = grade;
        Comment = comment;
        Status = SubmissionStatus.Graded;
    }
    public void TakeWork(Guid teacherId)
    {
        if (Status == SubmissionStatus.Checking || Status == SubmissionStatus.Graded)
        {
            throw new Exception("Эту работу нельзя взять на проверку");
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