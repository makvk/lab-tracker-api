using System.IO.Compression;
using LabManagement.Api.DTOs;
using LabManagement.Api.Services;
using LabManagement.App.Common;
using LabManagement.App.Domain.Entities;
using LabManagement.App.Features.LabWorks;
using LabManagement.App.Features.Submissions.SubmitWork;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabManagement.Api.Endpoints;

public static class WorkEndpoints
{
    public record UpdateDeadlineDto(DateTimeOffset Deadline);
    public record DownloadLabWorkDto(string FilePath);
    public static IEndpointRouteBuilder MapWorkEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/works/{id:guid}", async (
            Guid id,
            ILabDbContext labDbContext) =>
        {
            LabWork? labWork = labDbContext.LabWorks.FirstOrDefault(lw => lw.Id == id);

            if (labWork == null)
            {
                return Results.NotFound("Работа не найдена");
            }

            return Results.Ok(labWork);
        });

        app.MapGet("/api/works/{id:guid}/submissions", async (
            Guid id,
            ILabDbContext labDbContext) =>
        {
            List<Submission> submissions = await labDbContext.Submissions
                .Include(s => s.Student)
                .Include(s => s.LabWork)
                .Where(s => s.LabWorkId == id).ToListAsync();

            if (submissions== null)
            {
                return Results.NotFound("Работа не найдена");
            }
            
            return Results.Ok(submissions);
        });

        app.MapPost("/api/works/{id:guid}/submit", async (
            [FromRoute] Guid id,
            IFormFile file, // Принимаем файл через форму 
            [FromServices] IMediator mediator,
            [FromServices] ICurrentUserService currentUserService,
            CancellationToken cancellationToken) =>
        {
            // Валидация на то, что файл вообще пришел
            if (file == null || file.Length == 0)
            {
                return Results.BadRequest(new { error = "Файл не выбран или пуст" });
            }

            // Открываем поток для чтения файла
            using var stream = file.OpenReadStream();

            var studentId = currentUserService.UserId ?? Guid.Empty;

            var command = new SubmitWorkCommand
            {
                LabWorkId = id,
                StudentId = studentId,
                FileStream = stream,
                FileName = file.FileName,
                ContentType = file.ContentType
            };

            var submission = await mediator.Send(command, cancellationToken);

            // Возвращаем 201 Created. Первый параметр — URL созданного ресурса, второй — сам объект
            return Results.Created($"/api/submissions/{submission.Id}", submission);
        })
        .RequireAuthorization(new AuthorizeAttribute { Roles = "Student" })
        .DisableAntiforgery()
        .Accepts<IFormFile>("multipart/form-data");

        app.MapPost("api/works/create-work", async (
            [FromForm] CreateLabWorkRequest request,
            [FromServices] IMediator mediator,
            [FromServices] ICurrentUserService currentUserService
        ) =>
        {
            IReadOnlyList<IFormFile> files = request.Files;
            if (files == null || files.Count == 0)
            {
                return Results.BadRequest(new { error = "Файл не выбран или пуст" });
            }

            var filesList = new List<FileUploadModel>();
            var openedStreams = new List<Stream>();

            try {
                foreach (var file in files)
                {
                    var stream = file.OpenReadStream(); 
                    openedStreams.Add(stream);
                    filesList.Add(new FileUploadModel(stream, file.FileName));
                }

                var command = new CreateLabWorkCommand()
                {
                    Title = request.Title,
                    Description = request.Description,
                    Deadline = request.Deadline,
                    GroupId = request.GroupId,
                    TeacherId = currentUserService.UserId ?? Guid.Empty,
                    Files = filesList
                };

                LabWork labWork = await mediator.Send(command);

                return Results.Created($"/api/groups/{labWork.GroupId}/works", labWork);
            }
            finally
            {
                foreach (var stream in openedStreams)
                {
                    await stream.DisposeAsync();
                }
            }
        })
        .RequireAuthorization(
            new AuthorizeAttribute { Roles = "Teacher"} 
        )
        .DisableAntiforgery()
        .Accepts<IFormFile>("multipart/form-data");

        app.MapPut("/api/works/{id:guid}", async (
            Guid id,
            UpdateDeadlineDto updateDeadlineDto,
            ILabDbContext labDbContext,
            CancellationToken cancellationToken
        ) =>
        {
            await labDbContext.ChangeLabWorkDeadline(id, updateDeadlineDto.Deadline.UtcDateTime, cancellationToken);
            await labDbContext.SaveChangesAsync(cancellationToken);
            return Results.NoContent();
        }).RequireAuthorization( new AuthorizeAttribute{ Roles = "Teacher" });

        app.MapDelete("api/works/{id:guid}", async (
            Guid id,
            ILabDbContext labDbContext,
            CancellationToken cancellationToken
        ) =>
        {
            await labDbContext.DeleteLabWorkByIdAsync(id, cancellationToken);
            await labDbContext.SaveChangesAsync(cancellationToken);
            return Results.NoContent();
        })
        .RequireAuthorization( new AuthorizeAttribute { Roles = "Teacher" } );

        app.MapGet("/api/works/{id:guid}/download", async (
            Guid id,
            ILabDbContext labDbContext,
            CancellationToken cancellationToken
        ) =>
        {
            // 1. Находим лабораторную работу в базе, чтобы узнать путь к её папке
            var labWork = await labDbContext.GetLabWorkByIdAsync(id, cancellationToken);
            if (labWork == null)
            {
                return Results.NotFound("Лабораторная работа не найдена.");
            }

            // 2. Формируем абсолютный путь к директории на сервере
            var absoluteFolderPath = Path.Combine(Directory.GetCurrentDirectory(), labWork.FilePath);

            // Проверяем, существует ли папка физически
            if (!Directory.Exists(absoluteFolderPath))
            {
                return Results.NotFound("Директория с файлами пуста или отсутствует на сервере.");
            }

            // Имя для будущего архива (например, "Лабораторная №1.zip")
            string zipFileName = $"{labWork.Title}.zip";

            // 3. Создаем поток в памяти для сборки ZIP-архива
            var memoryStream = new MemoryStream();
            
            try
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true))
                {
                    var directoryInfo = new DirectoryInfo(absoluteFolderPath);
                    
                    // Перебираем все файлы в папке и добавляем в архив
                    foreach (var file in directoryInfo.GetFiles())
                    {
                        var entry = archive.CreateEntry(file.Name, CompressionLevel.Fastest);
                        using var entryStream = entry.Open();
                        using var fileStream = file.OpenRead();
                        await fileStream.CopyToAsync(entryStream, cancellationToken);
                    }
                }

                // Сбрасываем позицию стрима на начало для чтения фронтендом
                memoryStream.Position = 0;

                // 4. Возвращаем архив в ответе
                return Results.File(
                    fileStream: memoryStream, 
                    contentType: "application/zip", 
                    fileDownloadName: zipFileName
                );
            }
            catch (Exception)
            {
                await memoryStream.DisposeAsync();
                throw;
            }
        })
        .RequireAuthorization(new AuthorizeAttribute { Roles = "Teacher,Student" });

        return app;
    }
}