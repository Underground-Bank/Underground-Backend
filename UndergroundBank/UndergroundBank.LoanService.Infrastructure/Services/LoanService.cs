using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Quartz;
using UndergroundBank.Common.Data.Constants;
using UndergroundBank.Common.Data.Enums;
using UndergroundBank.Common.Data.Models;
using UndergroundBank.Common.Dto;
using UndergroundBank.Common.Middlewares;
using UndergroundBank.LoanService.Application.Dto.Loan;
using UndergroundBank.LoanService.Application.Interfaces;
using UndergroundBank.LoanService.Domain.Entities;
using UndergroundBank.LoanService.Infrastructure.BackgroundJob;
using UndergroundBank.LoanService.Infrastructure.Services.LoanQueue;

namespace UndergroundBank.LoanService.Infrastructure.Services
{
    public class LoansService : ILoanService
    {
        private readonly IMapper _mapper;
        private readonly LoanDbContext _dbContext;
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly QueueSender _queueSender;

        public LoansService(LoanDbContext dbContext, IMapper mapper, ISchedulerFactory schedulerFactory, QueueSender queueSender)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            _mapper = mapper;
            _schedulerFactory = schedulerFactory;
            _queueSender = queueSender;
        }

        public async Task<GetLoansDto> GetAllLoans(Guid? userId)
        {
            _dbContext.ignoreUserFilter = true;
            return await GetLoansLogic(userId);
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
            return await GetLoansLogic(null);
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

        public async Task StartTopUpLoan(decimal payment, string BankAccountNumber, Guid loanId)
        {
            var loan = _dbContext.Loans.Where(l => l.Id == loanId).Include(u => u.Tariff).FirstOrDefault();
            if (loan == null) { throw new NotFoundException("Кредита с таким id не существует"); }
            var transaction = new TransactionDto
            {
                AccountNumber = BankAccountNumber,
                LoanId = loanId,
                MoneyCount = payment,
                Status = Status.InProgress,
            };
            await _queueSender.SendMessage<TransactionDto>(transaction, Queues.TRANSACTION_QUEUE_REQUEST);
            await WriteTransaction(transaction);
        }

        public async Task AutoTopUpLoan(Guid loanId, string accountNumber)
        {
            var loan = _dbContext.Loans.Where(l => l.Id == loanId).Include(u => u.Tariff).FirstOrDefault();
            if (loan == null) { throw new NotFoundException("Кредита с таким id не существует"); }
            var monthPayment = CalculateMonthPayment(loan.Tariff, loan.LoanDurationInMonth, loan.Amount);
            var transaction = new TransactionDto
            {
                AccountNumber = accountNumber,
                LoanId = loanId,
                MoneyCount = monthPayment,
                Status = Status.InProgress,
            };
            await _queueSender.SendMessage<TransactionDto>(transaction, Queues.TRANSACTION_QUEUE_REQUEST);
            await WriteTransaction(transaction);
        }

        private async Task TopUpLoanLogic(Guid loanId, decimal payment)
        {
            var loan = await _dbContext.Loans.Where(l => l.Id == loanId).FirstOrDefaultAsync();
            if (loan == null)
            {
                throw new NotFoundException("Кредита с таким ID не существует");
            }

            if (loan.RemainingPayment - payment < 0)
            {
                throw new BadRequestException("Сумма пополнения кредита не может превышать оставшийся платёж по кредиту");
            }

            loan.RemainingPayment -= payment;
            if (loan.RemainingPayment == 0)
            {
                loan.Status = LoanStatus.Closed;
            };

            _dbContext.Update(loan);

            await _dbContext.SaveChangesAsync();
        }

        private async Task<GetLoansDto> GetLoansLogic(Guid? userId)
        {
            var loans = await _dbContext.Loans
                .Where(l => userId == null || l.UserId == userId)
                .ToListAsync();

            return new GetLoansDto
            {
                loans = _mapper.Map<List<GetLoanDto>>(loans)
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
                .UsingJobData("BankAccountNumber", bankAccountId.ToString())
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

        public async Task EndTopUpLoanTransaction(TransactionDto transactionDto)
        {
            var transaction = _dbContext.Transactions.Where(t => t.Id == transactionDto.TransactionId).FirstOrDefault();
            if (transaction == null)
            {
                throw new NotFoundException("Транзакции с таким id не существует");
            }

            if (transaction.Status == Status.InProgress && transactionDto.Status == Status.Approved)
            {
                await TopUpLoanLogic(transactionDto.LoanId, transactionDto.MoneyCount);
            }
            transaction.Status = Status.Finished;
            transaction.EndAt = DateTime.UtcNow;
            _dbContext.Update(transaction);
            await _dbContext.SaveChangesAsync();

        }
        private async Task WriteTransaction(TransactionDto transactionDto)
        {
            var transaction = new Transaction
            {
                AccountNumber = transactionDto.AccountNumber,
                StartAt = DateTime.UtcNow,
                Status = Status.InProgress,
                Id = transactionDto.TransactionId,
                Description = "",
                Name = ""
            };
            await _dbContext.AddAsync(transaction);
        }
    }
}
