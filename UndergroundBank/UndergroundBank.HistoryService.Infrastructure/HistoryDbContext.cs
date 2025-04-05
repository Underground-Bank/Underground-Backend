using Microsoft.EntityFrameworkCore;
using UndergroundBank.HistoryService.Domain.Entities;

namespace UndergroundBank.HistoryService.Infrastructure
{
    public class HistoryDbContext : DbContext
    {
        public HistoryDbContext(DbContextOptions<HistoryDbContext> options)
            : base(options) { }

        public DbSet<OperationsHistoryElement> OperationsHistory { get; set; }
        public DbSet<OverduePayment> OverduePayments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OverduePayment>().HasKey(o => new { o.TransactionId, o.LoanId });
            modelBuilder.Entity<OverduePayment>()
                .HasOne(op => op.Transaction)
                .WithMany()
                .HasForeignKey(op => op.TransactionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
