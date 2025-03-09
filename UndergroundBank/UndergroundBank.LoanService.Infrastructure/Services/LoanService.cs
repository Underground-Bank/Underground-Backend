using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Quartz.Spi;
using UndergroundBank.Common.Data.Constants;
using UndergroundBank.Common.Data.Enums;
using UndergroundBank.Common.Data.Models;
using UndergroundBank.Common.Dto.Transaction;
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
        private readonly IJobFactory _jobFactory;
        private Quartz.IScheduler _scheduler;
        private readonly QueueSender _queueSender;

        public LoansService(
            LoanDbContext dbContext,
            IMapper mapper,
            ISchedulerFactory schedulerFactory,
            QueueSender queueSender,
            IJobFactory jobFactory
        )
        {
            _mapper = mapper;
            _dbContext = dbContext;
            _mapper = mapper;
            _schedulerFactory = schedulerFactory;
            _queueSender = queueSender;
            _jobFactory = jobFactory;
        }

        public async Task<GetLoansDto> GetAllLoans(Guid? userId)
        {
            return await GetLoansLogic(userId);
        }

        public async Task<GetLoanDto> GetLoan(Guid loanId)
        {
            var loan = await _dbContext.Loans.Where(l => l.Id == loanId).FirstOrDefaultAsync();
            if (loan == null)
            {
                throw new NotFoundException("Кредита с таким id не существует");
            }
            var loanDto = _mapper.Map<GetLoanDto>(loan);
            return loanDto;
        }

        public async Task<GetLoansDto> GetMyLoans(Guid userId)
        {
            return await GetLoansLogic(userId);
        }

        public async Task TakeLoan(TakeLoanDto takeLoanDto, Guid userId)
        {
            var tariff = await _dbContext
                .Tariffs.Where(t => t.Id == takeLoanDto.TariffId)
                .FirstOrDefaultAsync();

            if (tariff == null)
            {
                throw new NotFoundException("Тарифа с таким id не существует");
            }

            await CeckBankAccountAccession(takeLoanDto.BankAccountNumber, userId);

            var loanId = Guid.NewGuid();

            var newLoan = new Loan()
            {
                Id = loanId,
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

            var topUpBankAccountTransaction = new TopUpBankAccountTransaction
            {
                AccountNumber = takeLoanDto.BankAccountNumber,
                LoanId = loanId,
                MoneyCount = takeLoanDto.LoanAmount,
                UserId = userId,
                TransactionId = Guid.NewGuid(),
                Status = Status.InProgress,
            };
            await _queueSender.SendMessage(topUpBankAccountTransaction, Queues.TOP_UP_BANK_ACCOUNT_FROM_LOAN);
        }

        public async Task StartTopUpLoan(decimal payment, string BankAccountNumber, Guid loanId, Guid userId)
        {
            var loan = _dbContext
                .Loans.Where(l => l.Id == loanId)
                .Include(u => u.Tariff)
                .FirstOrDefault();
            if (loan == null)
            {
                throw new NotFoundException("Кредита с таким id не существует");
            }

            var transaction = new TransactionRequestDto
            {
                TransactionId = Guid.NewGuid(),
                AccountNumber = BankAccountNumber,
                LoanId = loanId,
                MoneyCount = payment,
                UserId = userId,
                Status = Status.InProgress,
            };
            await _queueSender.SendMessage<TransactionRequestDto>(
                transaction,
                Queues.TRANSACTION_QUEUE_REQUEST
            );
            await WriteLoanTopUTransaction(transaction);
        }


        //TODO: добавить валидацию для accountNumber
        public async Task AutoTopUpLoan(Guid loanId, string accountNumber, Guid userId)
        {
            var loan = _dbContext
                .Loans.Where(l => l.Id == loanId)
                .Include(u => u.Tariff)
                .FirstOrDefault();
            if (loan == null)
            {
                throw new NotFoundException("Кредита с таким id не существует");
            }
            var monthPayment = CalculateMonthPayment(
                loan.Tariff,
                loan.LoanDurationInMonth,
                loan.Amount
            );
            var transaction = new TransactionRequestDto
            {
                TransactionId = Guid.NewGuid(),
                AccountNumber = accountNumber,
                LoanId = loanId,
                UserId = userId,
                MoneyCount = monthPayment,
                Status = Status.InProgress,
            };
            await _queueSender.SendMessage<TransactionRequestDto>(
                transaction,
                Queues.TRANSACTION_QUEUE_REQUEST
            );
            await WriteLoanTopUTransaction(transaction);
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
                throw new BadRequestException(
                    "Сумма пополнения кредита не может превышать оставшийся платёж по кредиту"
                );
            }

            if (loan.Status == LoanStatus.Closed)
            {
                throw new BadRequestException("Кредит уже закрыт");
            }

            loan.RemainingPayment -= payment;
            if (loan.RemainingPayment == 0)
            {
                loan.Status = LoanStatus.Closed;
            }
            ;

            _dbContext.Update(loan);

            await _dbContext.SaveChangesAsync();
        }

        private async Task<GetLoansDto> GetLoansLogic(Guid? userId)
        {
            var loans = await _dbContext
                .Loans.Where(l => userId == null || l.UserId == userId)
                .ToListAsync();

            return new GetLoansDto { loans = _mapper.Map<List<GetLoanDto>>(loans) };
        }

        private decimal CalculateMonthPayment(
            Tariff tariff,
            int loanDurationInMonth,
            decimal loanAmount
        )
        {
            var monthPayment = (loanAmount * (tariff.InterestRate)) / (loanDurationInMonth * 100);
            return monthPayment;
        }

        public async Task CreateAutoTopUp(string bankAccountId, Guid loanId, Guid userId)
        {
            await CeckBankAccountAccession(bankAccountId, userId);

            _scheduler = await _schedulerFactory.GetScheduler();
            _scheduler.JobFactory = _jobFactory;

            var loan = _dbContext.Loans.Where(l => l.Id == loanId).FirstOrDefault();
            if (loan == null)
            {
                throw new NotFoundException("Кредита с таким ID не существует");
            }

            var jobKey = new JobKey($"{loanId}-{bankAccountId}-{userId}", "CreditRepaymentJobs");

            var job = JobBuilder
                .Create<TopUpLoanJob>()
                .WithIdentity(jobKey)
                .UsingJobData("LoanId", loanId.ToString())
                .UsingJobData("BankAccountNumber", bankAccountId.ToString())
                .UsingJobData("UserId", userId.ToString())
                .Build();

            var trigger = TriggerBuilder
                .Create()
                .WithIdentity($"{loanId}-{bankAccountId}-{userId}Trigger", "CreditRepaymentJobs")
                .StartNow()
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(5).RepeatForever())
                .Build();

            await _scheduler.ScheduleJob(job, trigger);
            await _scheduler.Start();
        }

        public async Task EndTopUpLoanTransaction(TransactionResponseDto transactionDto)
        {
            var transaction = _dbContext
                .Transactions.Where(t => t.Id == transactionDto.TransactionId)
                .FirstOrDefault();
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

        private async Task WriteLoanTopUTransaction(TransactionRequestDto transactionDto)
        {
            var transaction = new Transaction
            {
                AccountNumber = transactionDto.AccountNumber,
                StartAt = DateTime.UtcNow,
                Status = Status.InProgress,
                UserId = transactionDto.UserId,
                Id = transactionDto.TransactionId,
                Description = "Loan top up transaction",
                Name = "LoanTopUp",
            };
            await _dbContext.AddAsync(transaction);
            await _dbContext.SaveChangesAsync();
        }
        private async Task CeckBankAccountAccession(string bankAccountNumber, Guid userId)
        {
            var accessionInfoRequest = new CheckBankAccountAccessRequest
            {
                BankAccountNumber = bankAccountNumber,
                UserId = userId
            };

            var accessionInfo = await _queueSender.CheckBankAccountAccess(accessionInfoRequest);

            if (!accessionInfo.IsBankAccountExists)
            {
                throw new NotFoundException("Такого счета не существует");
            }

            if (!accessionInfo.HasUserAccess)
            {
                throw new ForbiddenException("У вас нет доступа к этому счету");
            }
        }
    }
}
