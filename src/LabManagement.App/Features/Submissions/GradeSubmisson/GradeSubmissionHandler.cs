using System.ComponentModel;
using System.ComponentModel.Design;
using LabManagement.App.Domain.Entities;
using LabManagement.App.Domain.Enums;
using MediatR;

namespace LabManagement.App.Features.Submissions.GradeSubmission;


public interface ILabDbContext
{
    Task<Submission?> GetSubmissionByIdAsync(Guid id, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
public class GradeSubmissionHandler(ILabDbContext context): IRequestHandler<GradeSubmissionCommand>
{
    private readonly ILabDbContext _context = context;

    public async Task Handle(GradeSubmissionCommand request, CancellationToken cancellationToken)
    {
        Submission? work = await _context.GetSubmissionByIdAsync(request.SubmissionId, cancellationToken) 
            ?? throw new Exception($"Работы с id: {request.SubmissionId} не существует.");
        
        if (work.TeacherId != request.TeacherId)
        {
            throw new Exception("Работу проверяет другой человек");
        }

        work.GradeWork(request.Grade, request.Comment);

        await _context.SaveChangesAsync(cancellationToken);
    }
}