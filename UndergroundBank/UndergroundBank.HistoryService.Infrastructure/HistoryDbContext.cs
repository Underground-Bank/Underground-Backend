using Microsoft.EntityFrameworkCore;
using UndergroundBank.LoanService.Domain.Entities;

namespace UndergroundBank.HistoryService.Infrastructure
{
    public class HistoryDbContext : DbContext
    {
        public HistoryDbContext(DbContextOptions<HistoryDbContext> options)
            : base(options) { }

        public DbSet<OperationsHistoryElement> OperationsHistory { get; set; }
    }
}
