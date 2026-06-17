using LabManagement.App.Domain.Entities;

namespace LabManagement.App.Common;

public interface ILabDbContext
{
    Task<Submission?> GetSubmissionByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
