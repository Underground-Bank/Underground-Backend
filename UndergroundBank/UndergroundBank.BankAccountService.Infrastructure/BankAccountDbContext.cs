using Microsoft.EntityFrameworkCore;
using UndergroundBank.BankAccountService.Domain.Entities;

namespace UndergroundBank.BankAccountService.Infrastructure
{
    public class BankAccountDbContext : DbContext
    {
        public BankAccountDbContext(DbContextOptions<BankAccountDbContext> options)
            : base(options) { }

        public DbSet<BankAccount> BankAccounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BankAccount>().HasKey(x => x.AccountNumber);
        }
    }
}
