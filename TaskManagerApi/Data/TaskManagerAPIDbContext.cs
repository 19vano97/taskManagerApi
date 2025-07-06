using System;
using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Enitities.Ai;
using TaskManagerApi.Enitities.Organization;
using TaskManagerApi.Enitities.Project;
using TaskManagerApi.Enitities.Task;

namespace TaskManagerApi.Data;

public class TaskManagerAPIDbContext : DbContext
{
    public TaskManagerAPIDbContext(DbContextOptions<TaskManagerAPIDbContext> options) : base (options)
    {    }

    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<TicketStatus> TicketStatuses { get; set; }
    public DbSet<TicketType> TicketTypes { get; set; }
    public DbSet<TicketStatusType> TicketStatusType { get; set; }
    public DbSet<ProjectItem> ProjectItems { get; set; }
    public DbSet<ProjectTaskStatusMapping> ProjectTaskStatusMapping { get; set; }
    public DbSet<ProjectAccount> ProjectAccounts { get; set; }
    public DbSet<OrganizationItem> OrganizationItem { get; set; }
    public DbSet<OrganizationAccount> OrganizationAccount { get; set; }
    public DbSet<AiThreads> AiThreads { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ticket>().Property(d => d.CreateDate).HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<Ticket>().Property(d => d.ModifyDate).HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<TicketStatus>().Property(d => d.CreateDate).HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<TicketStatus>().Property(d => d.ModifyDate).HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<TicketType>().Property(d => d.CreateDate).HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<TicketType>().Property(d => d.ModifyDate).HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<TicketStatusType>().Property(d => d.CreateDate).HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<TicketStatusType>().Property(d => d.ModifyDate).HasDefaultValueSql("GETUTCDATE()");
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
        modelBuilder.Entity<AiThreads>().Property(d => d.CreateDate).HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<AiThreads>().Property(d => d.ModifyDate).HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<Ticket>().HasIndex(p => p.ProjectId).IsUnique(false);
        modelBuilder.Entity<Ticket>().HasIndex(s => s.StatusId).IsUnique(false);
    }
}
