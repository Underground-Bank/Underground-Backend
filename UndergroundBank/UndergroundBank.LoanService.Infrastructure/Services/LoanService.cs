using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Quartz;
using UndergroundBank.Common.Middlewares;
using UndergroundBank.LoanService.Application.Dto.Loan;
using UndergroundBank.LoanService.Application.Interfaces;
using UndergroundBank.LoanService.Domain.Entities;
using UndergroundBank.LoanService.Infrastructure.BackgroundJob;

namespace UndergroundBank.LoanService.Infrastructure.Services
{
    public class LoansService : ILoanService
    {
        private readonly IMapper _mapper;
        private readonly LoanDbContext _dbContext;
        private readonly ISchedulerFactory _schedulerFactory;

        public LoansService(LoanDbContext dbContext, IMapper mapper, ISchedulerFactory schedulerFactory)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            _mapper = mapper;
            _schedulerFactory = schedulerFactory;
        }

        public async Task<GetLoansDto> GetAllLoans()
        {
            _dbContext.ignoreUserFilter = true;
            return await GetLoansLogic();
        }

        public async Task<GetLoanDto> GetLoan(Guid loanId)
        {
            _dbContext.ignoreUserFilter = true;
            var loan = await _dbContext.Loans.Where(l => l.Id == loanId).FirstOrDefaultAsync();
            if (loan == null) { throw new NotFoundException("Кредита с таким id не существует"); }
            var loanDto = _mapper.Map<GetLoanDto>(loan);
            return loanDto;
        }

        public async Task<GetLoansDto> GetMyLoans()
        {
            return await GetLoansLogic();
        }

        public async Task TakeLoan(TakeLoanDto takeLoanDto, Guid userId)
        {
            var tariff = await _dbContext
                .Tariffs.Where(t => t.Id == takeLoanDto.TariffId)
                .FirstOrDefaultAsync();

            if (tariff == null) { throw new NotFoundException("Тарифа с таким id не существует"); }

            var newLoan = new Loan()
            {
                Amount = takeLoanDto.LoanAmount,
                LoanDurationInMonth = takeLoanDto.LoanDurationInMonths,
                StartDate = DateOnly.FromDateTime(DateTime.Today),
                Status = LoanStatus.Active,
                RemainingPayment = takeLoanDto.LoanAmount,
                TariffId = takeLoanDto.TariffId,
                UserId = userId,
            };

            await _dbContext.AddAsync(newLoan);
            await _dbContext.SaveChangesAsync();
        }

        //TODO add remainig month calculation
        public async Task TopUpLoan(decimal payment, Guid loanId)
        {
            var loan = _dbContext.Loans.Where(l => l.Id == loanId).Include(u => u.Tariff).FirstOrDefault();
            if (loan == null) { throw new NotFoundException("Кредита с таким id не существует"); }
            await TopUpLoanLogic(loan, payment);
        }

        public async Task AutoTopUpLoan(Guid loanId)
        {
            var loan = _dbContext.Loans.Where(l => l.Id == loanId).Include(u => u.Tariff).FirstOrDefault();
            if (loan == null) { throw new NotFoundException("Кредита с таким id не существует"); }
            var monthPayment = CalculateMonthPayment(loan.Tariff, loan.LoanDurationInMonth, loan.Amount);
            await TopUpLoanLogic(loan, monthPayment);
        }

        private async Task TopUpLoanLogic(Loan loan, decimal payment)
        {
            loan.RemainingPayment -= payment;
            if (loan.RemainingPayment <= 0)
            {
                loan.Status = LoanStatus.Closed;
            };

            _dbContext.Update(loan);

            await _dbContext.SaveChangesAsync();
        }

        private async Task<GetLoansDto> GetLoansLogic()
        {
            var loans = await _dbContext.Loans.ToListAsync();
            var loansListDto = _mapper.Map<List<GetLoanDto>>(loans);
            return new GetLoansDto()
            {
                loans = loansListDto
            };
        }

        private decimal CalculateMonthPayment(Tariff tariff, int loanDurationInMonth, decimal loanAmount)
        {
            var monthPayment = (loanAmount * (tariff.InterestRate)) / (loanDurationInMonth * 100);
            return monthPayment;
        }

        public async Task CreateAutoTopUp(Guid bankAccountId, Guid loanId)
        {
            var scheduler = await _schedulerFactory.GetScheduler();

            var jobKey = new JobKey($"{loanId}-{bankAccountId}", "CreditRepaymentJobs");

            var job = JobBuilder.Create<TopUpLoanJob>()
                .WithIdentity(jobKey)
                .UsingJobData("LoanId", loanId.ToString())
                .UsingJobData("BankAccountId", bankAccountId.ToString())
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity($"{loanId}-{bankAccountId}-trigger", "CreditRepaymentTriggers")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(20)
                    .RepeatForever())
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }
    }
}
