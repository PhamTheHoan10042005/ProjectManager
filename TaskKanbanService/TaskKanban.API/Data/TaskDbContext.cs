using Microsoft.EntityFrameworkCore;
using TaskKanban.API.Models;

namespace TaskKanban.API.Data;

public class TaskDbContext(DbContextOptions<TaskDbContext> options) : DbContext(options)
{
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<SubTask> SubTasks => Set<SubTask>();
    public DbSet<TimeLog> TimeLogs => Set<TimeLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.ToTable("Tasks");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Title).HasMaxLength(200).IsRequired();
            entity.Property(t => t.Description).HasMaxLength(4000);
            entity.Property(t => t.LabelColor).HasMaxLength(20);
            entity.Property(t => t.Priority).HasConversion<int>();
            entity.Property(t => t.Status).HasConversion<int>();
            entity.HasIndex(t => t.ProjectId);
            entity.HasIndex(t => new { t.ProjectId, t.Status, t.OrderIndex });
        });

        modelBuilder.Entity<SubTask>(entity =>
        {
            entity.ToTable("SubTasks");
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Title).HasMaxLength(200).IsRequired();
            entity.HasOne(s => s.Task)
                .WithMany(t => t.SubTasks)
                .HasForeignKey(s => s.TaskId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TimeLog>(entity =>
        {
            entity.ToTable("TimeLogs");
            entity.HasKey(l => l.Id);
            entity.Property(l => l.Hours).HasPrecision(5, 2);
            entity.Property(l => l.Description).HasMaxLength(500);
            entity.HasOne(l => l.Task)
                .WithMany(t => t.TimeLogs)
                .HasForeignKey(l => l.TaskId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(l => l.TaskId);
            entity.HasIndex(l => l.UserId);
        });
    }
}
