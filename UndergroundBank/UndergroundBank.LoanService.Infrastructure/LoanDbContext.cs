using Microsoft.EntityFrameworkCore;
using UndergroundBank.Common.Data.Models;
using UndergroundBank.LoanService.Domain.Entities;

namespace UndergroundBank.LoanService.Infrastructure
{
    public class LoanDbContext : DbContext
    {
        public LoanDbContext(DbContextOptions<LoanDbContext> options)
            : base(options) { }

        public DbSet<Loan> Loans { get; set; }
        public DbSet<Tariff> Tariffs { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<AutoTopUpJob> TopUpJobs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Loan>()
                .HasOne(c => c.Tariff)
                .WithMany()
                .HasForeignKey(c => c.TariffId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AutoTopUpJob>()
                .HasKey(j => new { j.BankAccountNumber, j.LoanId, j.UserId });
        }
    }
}
