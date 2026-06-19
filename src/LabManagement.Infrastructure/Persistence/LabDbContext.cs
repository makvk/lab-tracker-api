using Microsoft.EntityFrameworkCore;
using LabManagement.App.Common;
using LabManagement.App.Domain.Entities;

namespace LabManagement.Infrastructure.Persistence; 

public class LabDbContext(DbContextOptions<LabDbContext> options) : DbContext(options), ILabDbContext
{
    public DbSet<Group> Groups { get; set; } // Добавили DbSet для групп
    public DbSet<Submission> Submissions { get; set; }
    public DbSet<Teacher> Teachers { get; set; } 
    public DbSet<Student> Students { get; set; } 
    public DbSet<LabWork> LabWorks { get; set; }

    public async Task<Submission?> GetSubmissionByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await Submissions
            .Include(sub => sub.Student)
            .Include(sub => sub.LabWork)
            .Include(sub => sub.Teacher)
            .FirstOrDefaultAsync(sub => sub.Id == id, cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // --- НАСТРОЙКА GROUP ---
        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(g => g.Id);
            
            entity.Property(g => g.Name)
                .IsRequired()
                .HasMaxLength(20);
                
            entity.HasIndex(g => g.Name)
                .IsUnique();

            entity.Property(g => g.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .IsRequired();
        });

        // --- НАСТРОЙКА STUDENT ---
        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(s => s.Id);
            
            entity.Property(s => s.Email)
                .IsRequired()
                .HasMaxLength(150);
                
            entity.HasIndex(s => s.Email)
                .IsUnique();

            entity.Property(s => s.FirstName).IsRequired().HasMaxLength(50);
            entity.Property(s => s.LastName).IsRequired().HasMaxLength(50);
            entity.Property(s => s.PasswordHash).IsRequired().HasMaxLength(255);
            
            entity.Property(s => s.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            // Связь с сущностью Group по Guid
            entity.HasOne(s => s.Group)
                .WithMany()
                .HasForeignKey(s => s.GroupId)
                .OnDelete(DeleteBehavior.Restrict); // Не удаляем студентов при удалении группы
        });

        // --- НАСТРОЙКА LABWORK ---
        modelBuilder.Entity<LabWork>(entity =>
        {
            entity.HasKey(l => l.Id);

            entity.Property(l => l.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(l => l.Description)
                .HasMaxLength(2000);

            entity.Property(l => l.FilePath)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(l => l.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            entity.Property(l => l.Deadline)
                .HasColumnType("timestamp with time zone")
                .IsRequired();
                
            // Связь с создателем (Преподавателем)
            entity.HasOne(l => l.Teacher)
                .WithMany()
                .HasForeignKey(l => l.TeacherId)
                .OnDelete(DeleteBehavior.Cascade);

            // Связь с Группой по Guid
            entity.HasOne(l => l.Group)
                .WithMany()
                .HasForeignKey(l => l.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // --- НАСТРОЙКА SUBMISSION ---
        modelBuilder.Entity<Submission>(entity =>
        {
            entity.HasKey(s => s.Id);

            entity.Property(s => s.FilePath)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(s => s.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(s => s.Comment)
                .HasMaxLength(1000);
            
            entity.Property(s => s.Grade)
                .IsRequired(false);

            entity.Property(s => s.SubmittedAt)
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            entity.HasOne(s => s.LabWork)
                .WithMany()
                .HasForeignKey(s => s.LabWorkId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(s => s.Student)
                .WithMany()
                .HasForeignKey(s => s.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(s => s.Teacher)
                .WithMany()
                .HasForeignKey(s => s.TeacherId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // --- НАСТРОЙКА TEACHER ---
        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasKey(t => t.Id);
            
            entity.Property(t => t.Email)
                .IsRequired()
                .HasMaxLength(150);
                
            entity.HasIndex(t => t.Email)
                .IsUnique();

            entity.Property(t => t.FirstName).IsRequired().HasMaxLength(50);
            entity.Property(t => t.LastName).IsRequired().HasMaxLength(50);
            entity.Property(t => t.PasswordHash).IsRequired().HasMaxLength(255);
            
            entity.Property(t => t.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .IsRequired();
        });
    }
}