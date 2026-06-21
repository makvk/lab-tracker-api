using LabManagement.App.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LabManagement.App.Common;

public interface ILabDbContext
{
    DbSet<Submission> Submissions { get; set; }
    DbSet<Teacher> Teachers { get; set; }
    DbSet<Student> Students { get; set; }
    DbSet<LabWork> LabWorks { get; set; }
    DbSet<Group> Groups { get; set; }
    Task AddSubmissionAsync(Submission submission, CancellationToken cancellationToken);
    Task AddLabWorkAsync(LabWork labWork, CancellationToken cancellationToken);
    Task<Submission?> GetSubmissionByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Teacher?> GetTeacherByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Student?> GetStudentByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Group?> GetGroupByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Submission?> GetSubmissionByWorkAndStudentAsync(
        Guid workId, 
        Guid studentId, 
        CancellationToken cancellationToken);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
