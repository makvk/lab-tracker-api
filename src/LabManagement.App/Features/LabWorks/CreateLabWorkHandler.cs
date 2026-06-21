using System.Data.Common;
using System.Runtime.CompilerServices;
using LabManagement.App.Common;
using LabManagement.App.Domain.Entities;
using MediatR;

namespace LabManagement.App.Features.LabWorks;

public class CreateLabWorkHandler(ILabDbContext context) : IRequestHandler<CreateLabWorkCommand, LabWork>
{
    private readonly ILabDbContext _context = context; 
    public async Task<LabWork> Handle(CreateLabWorkCommand request, CancellationToken cancellationToken)
    {
        var uploadsDirectory = Path.Combine(
            Directory.GetCurrentDirectory(), 
            $"Uploads/LabWorks/{Guid.NewGuid()}_{request.Title}"
        );
        
            Directory.CreateDirectory(uploadsDirectory);
        try {
            foreach (var fileRecord in request.Files)
            {
                // Абсолютный путь для физического сохранения на жесткий диск сервера
                var absoluteFilePath = Path.Combine(uploadsDirectory, Path.GetFileName(fileRecord.FileName));
                // Асинхронно записываем поток файла на диск
                using var fileStream = new FileStream(absoluteFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
                await fileRecord.Stream.CopyToAsync(fileStream, cancellationToken);
            }
            string dbFilePath = uploadsDirectory;

            LabWork labWork = new LabWork(
                request.Title,
                request.Description,
                request.GroupId,
                request.TeacherId,
                dbFilePath,
                request.Deadline.UtcDateTime
            );

            await _context.AddLabWorkAsync(labWork, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return labWork;
        } catch (Exception)
        {
            // Если база упала или пользователь отменил загрузку — удаляем созданную папку с файлами
            if (Directory.Exists(uploadsDirectory))
            {
                Directory.Delete(uploadsDirectory, recursive: true);
            }
            throw; // Пробрасываем ошибку дальше для обработки в Middleware
        }
        // return new LabWork("", "", Guid.Empty, Guid.Empty, "", DateTimeOffset.Now);
    }
}