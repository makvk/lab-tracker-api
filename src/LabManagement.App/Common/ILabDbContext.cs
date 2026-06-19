using LabManagement.App.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LabManagement.App.Common;

public interface ILabDbContext
{
    DbSet<Submission> Submissions { get; set; }
    DbSet<Teacher> Teachers { get; set; }
    DbSet<Student> Students { get; set; }
    DbSet<LabWork> LabWorks { get; set; }
    Task<Submission?> GetSubmissionByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Teacher?> GetTeacherByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Student?> GetStudentByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
