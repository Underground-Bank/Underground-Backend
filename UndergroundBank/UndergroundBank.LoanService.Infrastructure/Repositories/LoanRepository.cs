using UndergroundBank.AccountService.Application.Interfaces;
using UndergroundBank.Common.Base;
using UndergroundBank.LoanService.Domain.Entities;

namespace UndergroundBank.LoanService.Infrastructure.Repositories
{
    public class LoanRepository : BaseRepository<Loan, LoanDbContext>, ILoanRepository
    {
        private readonly LoanDbContext _loanContext;

        public LoanRepository(LoanDbContext dbContext)
            : base(dbContext)
        {
            _loanContext = dbContext;
        }
    }
}
