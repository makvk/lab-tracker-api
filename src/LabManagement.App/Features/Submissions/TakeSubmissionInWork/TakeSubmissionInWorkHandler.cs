using LabManagement.App.Common;
using LabManagement.App.Common.Exceptions;
using LabManagement.App.Domain.Entities;
using MediatR;

namespace LabManagement.App.Features.Submissions.TakeSubmissionInWork;

public class TakeSubmissionInWorkHandler(ILabDbContext context): IRequestHandler<TakeSubmissionInWorkCommand, Submission>
{
    private readonly ILabDbContext _context = context;
    public async Task<Submission> Handle(TakeSubmissionInWorkCommand request, CancellationToken cancellationToken)
    {
        Submission? work = await _context.GetSubmissionByIdAsync(request.SubmissionId, cancellationToken)
            ?? throw new NotFoundException($"Работы с id: {request.SubmissionId} не существует.");
        
        work.TakeWork(request.TeacherId);

        await _context.SaveChangesAsync(cancellationToken);
        return work;
    }
}