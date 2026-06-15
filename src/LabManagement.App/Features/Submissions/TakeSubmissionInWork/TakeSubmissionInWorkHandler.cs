using LabManagement.App.Domain.Entities;
using MediatR;

namespace LabManagement.App.Features.Submissions.TakeSubmissionInWork;

public interface ILabDbContext
{
    Task<Submission?> GetSubmissionByIdAsync(Guid id, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}

public class TakeSubmissionInWorkHandler(ILabDbContext context): IRequestHandler<TakeSubmissionInWorkCommand>
{
    private readonly ILabDbContext _context = context;
    public async Task Handle(TakeSubmissionInWorkCommand request, CancellationToken cancellationToken)
    {
        Submission? work = await _context.GetSubmissionByIdAsync(request.SubmissionId, cancellationToken)
            ?? throw new Exception($"Работы с id: {request.SubmissionId} не существует.");
        
        work.TakeWork(request.TeacherId);

        await _context.SaveChangesAsync(cancellationToken);
    }
}