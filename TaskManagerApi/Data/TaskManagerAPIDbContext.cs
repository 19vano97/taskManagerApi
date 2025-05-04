using System;
using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Enitities;

namespace TaskManagerApi.Data;

public class TaskManagerAPIDbContext : DbContext
{
    public TaskManagerAPIDbContext(DbContextOptions<TaskManagerAPIDbContext> options) : base (options)
    {
        
    }

    public DbSet<TaskItem> TaskItems { get; set; }
    public DbSet<TaskItemStatus> TaskItemStatuses { get; set; }
    public DbSet<TaskItemStatusType> TaskItemStatusType { get; set; }
    public DbSet<ProjectItem> ProjectItems { get; set; }
    public DbSet<ProjectTaskStatusMapping> ProjectTaskStatusMapping { get; set; }
    public DbSet<ProjectAccount> ProjectAccounts { get; set; }
    public DbSet<OrganizationItem> OrganizationItem { get; set; }
    public DbSet<OrganizationAccount> OrganizationAccount { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskItem>().Property(d => d.CreateDate).HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<TaskItem>().Property(d => d.ModifyDate).HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<TaskItemStatus>().Property(d => d.CreateDate).HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<TaskItemStatus>().Property(d => d.ModifyDate).HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<TaskItemStatusType>().Property(d => d.CreateDate).HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<TaskItemStatusType>().Property(d => d.ModifyDate).HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<ProjectItem>().Property(d => d.CreateDate).HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<ProjectItem>().Property(d => d.ModifyDate).HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<ProjectTaskStatusMapping>().Property(d => d.CreateDate).HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<ProjectTaskStatusMapping>().Property(d => d.ModifyDate).HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<ProjectAccount>().Property(d => d.CreateDate).HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<ProjectAccount>().Property(d => d.ModifyDate).HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<OrganizationItem>().Property(d => d.CreateDate).HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<OrganizationItem>().Property(d => d.ModifyDate).HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<OrganizationAccount>().Property(d => d.CreateDate).HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<OrganizationAccount>().Property(d => d.ModifyDate).HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<TaskItem>().HasIndex(p => p.ProjectId).IsUnique(false);
        modelBuilder.Entity<TaskItem>().HasIndex(s => s.StatusId).IsUnique(false);
    }
}
