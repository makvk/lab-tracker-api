using FluentValidation; 

namespace LabManagement.App.Features.Submissions.SubmitWork;

public class SubmitWorkValidator: AbstractValidator<SubmitWorkCommand>
{
    private const long MaxFileSize = 20 * 1024 * 1024;
    public SubmitWorkValidator()
    {
        RuleFor(command => command.LabWorkId)
            .NotEmpty().WithMessage("Идентификатор лабораторной обязателен.");

        RuleFor(command => command.StudentId)
            .NotEmpty().WithMessage("Идентификатор студента обязателен.");

        RuleFor(command => command.FileStream)
            .Must(stream => stream.Length <= MaxFileSize)
            .WithMessage($"Размер файла превышает допустимый лимит ({MaxFileSize / 1024 / 1024} МБ).");  
    }
}