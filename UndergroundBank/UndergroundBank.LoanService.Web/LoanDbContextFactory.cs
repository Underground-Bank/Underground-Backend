using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace UndergroundBank.LoanService.Infrastructure
{
    public class LoanDbContextFactory : IDesignTimeDbContextFactory<LoanDbContext>
    {
        public LoanDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<LoanDbContext>();
            optionsBuilder.UseNpgsql(configuration.GetConnectionString("LoanDatabasePostgres"));

            return new LoanDbContext(optionsBuilder.Options, new FakeUserContext());
        }
    }

    public class FakeUserContext : IUserContext
    {
        public Guid UserId => Guid.Empty;
    }
}