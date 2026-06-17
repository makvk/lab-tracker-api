using System.ComponentModel;
using System.ComponentModel.Design;
using LabManagement.App.Common;
using LabManagement.App.Common.Exceptions;
using LabManagement.App.Domain.Entities;
using LabManagement.App.Domain.Enums;
using MediatR;

namespace LabManagement.App.Features.Submissions.GradeSubmission;

public class GradeSubmissionHandler(ILabDbContext context): IRequestHandler<GradeSubmissionCommand, Submission>
{
    private readonly ILabDbContext _context = context;

    public async Task<Submission> Handle(GradeSubmissionCommand request, CancellationToken cancellationToken)
    {
        Submission? work = await _context.GetSubmissionByIdAsync(request.SubmissionId, cancellationToken) 
            ?? throw new NotFoundException($"Работы с id: {request.SubmissionId} не существует.");

        if (work.Status != SubmissionStatus.Checking)
        {
            throw new InvalidOperationException("Нельзя оценить работу, которая не находится на проверке");
        }
        if (work.TeacherId != request.TeacherId)
        {
            throw new Exception("Работу проверяет другой человек");
        }

        work.GradeWork(request.Grade, request.Comment);

        await _context.SaveChangesAsync(cancellationToken);

        return work;
    }
}