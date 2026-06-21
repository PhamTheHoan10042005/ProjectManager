using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Entities;
using ProjectEntity = Project.Infrastructure.Entities.Project;

namespace Project.Infrastructure.Data;

public partial class ProjectDbContext : DbContext
{
    public ProjectDbContext(DbContextOptions<ProjectDbContext> options)
        : base(options)
    {
    }

    // 1. Quản lý danh sách bảng dữ liệu
    public virtual DbSet<ProjectEntity> Projects { get; set; }

    public virtual DbSet<ProjectMember> ProjectMembers { get; set; }

    public virtual DbSet<Sprint> Sprints { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 2. Cấu hình thực thể bảng Projects
        modelBuilder.Entity<ProjectEntity>(entity => {
            entity.HasKey(e => e.Id).HasName("PK__Projects__3213E83F8C5C4B5A");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Color)
                .HasMaxLength(7)
                .IsUnicode(false)
                .HasDefaultValue("#6366f1")
                .HasColumnName("color");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("end_date");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("start_date");
        });

        // 3. Cấu hình thực thể bảng Project_Members (Thành viên)
        modelBuilder.Entity<ProjectMember>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Project___3213E83F495D26A0");

            entity.ToTable("Project_Members");

            entity.HasIndex(e => new { e.ProjectId, e.UserId }, "UNIQUE_Project_User").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.JoinedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("joined_at");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.Role)
                .HasDefaultValue(2)
                .HasColumnName("role");
            entity.Property(e => e.UserId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("user_id");

            // ĐÃ CHUẨN HÓA: Chỉ định rõ kiểu dữ liệu liên kết dẫn tới lớp Project
            entity.HasOne<ProjectEntity>()
                .WithMany(p => p.ProjectMembers)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK__Project_M__proje__3C69FB99")
                .OnDelete(DeleteBehavior.Cascade);
        });

        // 4. Cấu hình thực thể bảng Sprints (Chu kỳ công việc)
        modelBuilder.Entity<Sprint>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Sprints__3213E83FBD3BA5D8");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("end_date");
            entity.Property(e => e.Goal).HasColumnName("goal");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("start_date");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("PLANNING")
                .HasColumnName("status");

            // ĐÃ CHUẨN HÓA: Chỉ định rõ kiểu dữ liệu liên kết dẫn tới lớp Project
            entity.HasOne<ProjectEntity>()
                .WithMany(p => p.Sprints)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK__Sprints__project__403A8C7D")
                .OnDelete(DeleteBehavior.Cascade);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
