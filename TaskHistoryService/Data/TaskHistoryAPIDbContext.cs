using System;
using Microsoft.EntityFrameworkCore;
using TaskHistoryService.Enitities;

namespace TaskHistoryService.Data;

public class TaskHistoryAPIDbContext : DbContext
{
    public TaskHistoryAPIDbContext(DbContextOptions<TaskHistoryAPIDbContext> options) : base(options)
    {
        
    }
    public DbSet<TaskHistory> TaskHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskHistory>().Property(d => d.CreateDate).HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<TaskHistory>().Property(d => d.ModifyDate).HasDefaultValueSql("GETUTCDATE()");
    }
    
    
}

    