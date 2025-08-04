using System;
using Microsoft.EntityFrameworkCore;
using TaskHistoryService.Enitities;

namespace TaskHistoryService.Data;

public class TaskHistoryAPIDbContext : DbContext
{
    public TaskHistoryAPIDbContext(DbContextOptions<TaskHistoryAPIDbContext> options) : base(options)
    {
        
    }
    public DbSet<TicketHistory> TicketHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TicketHistory>().Property(d => d.CreateDate).HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<TicketHistory>().Property(d => d.ModifyDate).HasDefaultValueSql("GETUTCDATE()");
    }
    
    
}

    