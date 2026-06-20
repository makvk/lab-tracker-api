using LabManagement.App.Common;
using LabManagement.App.Domain.Entities;
using MediatR;

namespace LabManagement.App.Features.Submissions.SubmitWork;

public class SubmitWorkHandler(ILabDbContext context): IRequestHandler<SubmitWorkCommand, Submission>
{
    private readonly ILabDbContext _context = context;

    public async Task<Submission> Handle(SubmitWorkCommand request, CancellationToken cancellationToken)
    {
        // Определяем базовую директорию для хранения файлов
        // Directory.GetCurrentDirectory() вернет корень запущенного API-проекта
        var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Uploads/SubmissionWorks");

        // Если папки "Uploads" еще нет на сервере - создаем её
        if (!Directory.Exists(uploadsDirectory))
        {
            Directory.CreateDirectory(uploadsDirectory);
        }

        // Генерируем уникальное имя файла, сохраняя оригинальное расширение
        var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(request.FileName)}";
        
        // Абсолютный путь для физического сохранения на жесткий диск сервера
        var absoluteFilePath = Path.Combine(uploadsDirectory, uniqueFileName);

        // Асинхронно записываем поток файла на диск
        using (var fileStream = new FileStream(absoluteFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await request.FileStream.CopyToAsync(fileStream, cancellationToken);
        }

        // Путь, который мы сохраним в базу данных (относительный)
        string dbFilePath = Path.Combine("Uploads/SubmissionWorks", uniqueFileName);

        // 5. Создаем сущность и пушим в БД
        Submission submission = new Submission(request.LabWorkId, request.StudentId, dbFilePath);
        
        await _context.AddSubmissionAsync(submission, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return submission;
    }
}