using FluentValidation;

namespace LabManagement.App.Features.Submissions.TakeSubmissionInWork;

public class TakeSubmissionInWorkValidator: AbstractValidator<TakeSubmissionInWorkCommand>
{
    public TakeSubmissionInWorkValidator()
    {
        RuleFor(command => command.SubmissionId)
            .NotEmpty().WithMessage("Идентификатор сдачи обязателен.");

        RuleFor(command => command.TeacherId)
            .NotEmpty().WithMessage("Идентификатор преподавателя обязателен.");
    }
}