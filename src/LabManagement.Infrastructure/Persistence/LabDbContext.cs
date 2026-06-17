using Microsoft.EntityFrameworkCore;
using LabManagement.App.Common;
using LabManagement.App.Domain.Entities;

namespace LabManagement.Infrastructure.Persistence; 

public class LabDbContext(DbContextOptions<LabDbContext> options) : DbContext(options), ILabDbContext
{
    public DbSet<Submission> Submissions { get; set; }

    public async Task<Submission?> GetSubmissionByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await Submissions.FirstOrDefaultAsync(sub => sub.Id == id, cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

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

            entity.Property(s => s.TeacherId)
                .IsRequired(false);
        });
    }
}