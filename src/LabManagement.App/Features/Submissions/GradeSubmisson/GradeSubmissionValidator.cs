using System.Data;
using FluentValidation; 

namespace LabManagement.App.Features.Submissions.GradeSubmission;

public class GradeSubmissionValidator: AbstractValidator<GradeSubmissionCommand>
{
    public GradeSubmissionValidator()
    {
        RuleFor(command => command.Grade)
            .ExclusiveBetween(2, 5).WithMessage("Оценка должна быть от 2 до 5.");

        RuleFor(command => command.SubmissionId)
            .NotEmpty().WithMessage("Идентификатор сдачи обязателен.");

        RuleFor(command => command.TeacherId)
            .NotEmpty().WithMessage("Идентификатор преподавателя обязателен.");

        RuleFor(command => command.Comment)
            .MaximumLength(500)
            .WithMessage("Длина комментария не должна превышеать 500 символов.")
            .When(command => !string.IsNullOrEmpty(command.Comment));
    }
}