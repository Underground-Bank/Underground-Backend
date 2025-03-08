using Microsoft.EntityFrameworkCore;
using UndergroundBank.Common.Data.Models;
using UndergroundBank.LoanService.Domain.Entities;

namespace UndergroundBank.LoanService.Infrastructure
{
    public class LoanDbContext : DbContext
    {
        private readonly IUserContext _userContext;
        public bool ignoreUserFilter { get; set; } = false;
        public LoanDbContext(DbContextOptions<LoanDbContext> options, IUserContext userContext)
            : base(options) { _userContext = userContext; }

        public DbSet<Loan> Loans { get; set; }
        public DbSet<Tariff> Tariffs { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Loan>()
                .HasQueryFilter(c => !ignoreUserFilter && c.UserId == _userContext.UserId)
                .HasOne(c => c.Tariff)
                .WithMany()
                .HasForeignKey(c => c.TariffId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
