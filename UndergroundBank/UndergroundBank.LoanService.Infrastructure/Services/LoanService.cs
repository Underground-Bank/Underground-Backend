using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UndergroundBank.AccountService.Application.Interfaces;
using UndergroundBank.AccountService.Infrastructure;
using UndergroundBank.Common.Middlewares;
using UndergroundBank.LoanService.Application.Dto.Loan;
using UndergroundBank.LoanService.Application.Interfaces;
using UndergroundBank.LoanService.Domain.Entities;

namespace UndergroundBank.LoanService.Infrastructure.Services
{
    public class LoansService : ILoanService
    {
        private readonly IMapper _mapper;
        private readonly ILoanRepository _loanRepository;
        private readonly LoanDbContext _dbContext;

        public LoansService(ILoanRepository loanRepository, LoanDbContext dbContext, IMapper mapper)
        {
            _mapper = mapper;
            _loanRepository = loanRepository;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public Task GetAllLoans()
        {
            throw new NotImplementedException();
        }

        public Task GetLoan()
        {
            throw new NotImplementedException();
        }

        public Task GetMyLoans()
        {
            throw new NotImplementedException();
        }

        public async Task TakeLoan(string userId) { }
        public async Task TakeLoan(TakeLoanDto takeLoanDto, Guid userId)
        {
            ValidateStartDate(takeLoanDto.StartDate);

            var tariff = await _dbContext
                .Tariffs.Where(t => t.Id == takeLoanDto.TariffId)
                .FirstOrDefaultAsync();
            var loanStatus =
                takeLoanDto.StartDate > DateOnly.FromDateTime(DateTime.Today)
                    ? LoanStatus.NotStarted
                    : LoanStatus.Active;

            var newLoan = new Loan()
            {
                Amount = takeLoanDto.LoanAmount,
                LoanDurationInMonth = takeLoanDto.LoanDurationInMonths,
                StartDate = takeLoanDto.StartDate,
                Status = loanStatus,
                RemainingPayment = takeLoanDto.LoanAmount,
                TariffId = takeLoanDto.TariffId,
                UserId = userId,
            };

            await _dbContext.AddAsync(newLoan);
            await _dbContext.SaveChangesAsync();
        }

        private void ValidateStartDate(DateOnly startDate)
        {
            if (startDate < DateOnly.FromDateTime(DateTime.Today))
            {
                throw new BadRequestException("Нельзя взять кредит в прошлое");
            }
        }
    }
}
