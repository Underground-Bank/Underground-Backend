using Microsoft.EntityFrameworkCore;
using UndergroundBank.MonitoringService.Domain.Entities;

namespace UndergroundBank.MonitoringService.Infrastructure
{
    public class MonitoringDbContext : DbContext
    {
        public MonitoringDbContext(DbContextOptions<MonitoringDbContext> options)
            : base(options) { }

        public DbSet<LogEntry> Logs { get; set; } = default!;
        public DbSet<TraceSpan> Traces { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LogEntry>().HasIndex(l => l.TraceId);
            modelBuilder.Entity<LogEntry>().HasIndex(l => l.Timestamp);
            modelBuilder.Entity<TraceSpan>().HasIndex(t => t.TraceId);
            modelBuilder.Entity<TraceSpan>().HasIndex(t => t.StartTime);
        }
    }
}
