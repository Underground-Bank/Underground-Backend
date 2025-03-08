using AutoMapper;
using EasyNetQ;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.EntityFrameworkCore;
using UndergroundBank.BankAccountService.Application.Interfaces;
using UndergroundBank.BankAccountService.Domain.Entities;
using UndergroundBank.Common.Data.Enums;
using UndergroundBank.Common.Dto.AccountService;
using UndergroundBank.Common.Dto.BankAccountService;
using UndergroundBank.Common.Dto.Transaction;
using UndergroundBank.Common.Middlewares;

namespace UndergroundBank.BankAccountService.Infrastructure.Services
{
    public class BankService : IBankService
    {
        private readonly BankAccountDbContext _dbContext;
        private readonly QueueSender _queueSender;
        private readonly IBus _bus;
        private readonly IMapper _mapper;

        public BankService(BankAccountDbContext dBContext, IMapper mapper, QueueSender queueSender)
        {
            _dbContext = dBContext;
            _mapper = mapper;
            _bus = RabbitHutch.CreateBus("host=localhost");
            _queueSender = queueSender;
        }

        public async Task BlockAccountNumber(string accountNumber)
        {
            var bankAccount = await _dbContext.BankAccounts.FirstOrDefaultAsync(b =>
                b.AccountNumber == accountNumber
            );

            if (bankAccount == null)
            {
                throw new NotFoundException("Данного счета не существует!");
            }

            if (bankAccount.IsLocked)
            {
                throw new NotFoundException("Счет уже заблокирован!");
            }
            bankAccount.IsLocked = true;
            await _dbContext.SaveChangesAsync();
        }

        public async Task UnblockAccountNumber(string accountNumber)
        {
            var bankAccount = await _dbContext.BankAccounts.FirstOrDefaultAsync(b =>
                b.AccountNumber == accountNumber
            );
            if (bankAccount == null)
            {
                throw new NotFoundException("Данного счета не существует!");
            }
            if (!bankAccount.IsLocked)
            {
                throw new NotFoundException("Счет не заблокирован!");
            }

            bankAccount.IsLocked = false;
            await _dbContext.SaveChangesAsync();
        }

        public async Task CreateAccountNumber(Guid userId)
        {
            var user = await _bus.Rpc.RequestAsync<Guid, ProfileDto>(
                userId,
                x => x.WithQueueName("bank_UserProfileResponse")
            );
            if (user.Roles.Contains(Role.Employee)) { }
            if (user == null)
            {
                throw new NotFoundException("Данного пользователя не существует!");
            }

            var account = _mapper.Map<BankAccount>(user);
            var accountNumber = GenerateAccountNumber();
            account.UserId = user.Id;
            account.AccountNumber = accountNumber;
            account.CreatedDate = DateTime.UtcNow;
            account.IsLocked = false;
            account.Balance = 0;
            _dbContext.Add(account);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAccountNumber(string accountNumber)
        {
            var bankAccount = await _dbContext.BankAccounts.FirstOrDefaultAsync(b =>
                b.AccountNumber == accountNumber
            );
            if (bankAccount == null)
            {
                throw new NotFoundException("Данного счета не существует!");
            }
            _dbContext.Remove(bankAccount);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<BankAccountDto>> GetAllAccountNumbers()
        {
            var bankAccount = await _dbContext
                .BankAccounts.Where(b => b.AccountNumber != null)
                .ToListAsync();
            return _mapper.Map<List<BankAccountDto>>(bankAccount);
        }

        public async Task<List<BankAccountDto>> GetMyAccountNumbers(Guid userId)
        {
            var bankAccount = await _dbContext
                .BankAccounts.Where(b => b.UserId == userId)
                .ToListAsync();
            return _mapper.Map<List<BankAccountDto>>(bankAccount);
        }

        public async Task<BankAccountDto> GetMyCorrespondingAccountNumber(
            string accountNumber,
            Guid userId,
            List<Role> userRoles
        )
        {
            var bankAccount = await _dbContext.BankAccounts.FirstOrDefaultAsync(b =>
                b.AccountNumber == accountNumber
            );
            if (bankAccount == null)
            {
                throw new NotFoundException("Данного счета не существует!");
            }

            if (
                bankAccount.UserId != userId
                && !(userRoles.Contains(Role.Employee) || userRoles.Contains(Role.Admin))
            )
            {
                throw new BadRequestException("Вы не смотреть чужой кошелек");
            }

            return _mapper.Map<BankAccountDto>(bankAccount);
        }

        public async Task TopUpAccountNumber(string accountNumber, decimal moneyCount)
        {
            if (moneyCount <= 0)
            {
                throw new BadRequestException("Вы не можете пополнить меньше чем на 1 деньгу!");
            }

            var bankAccount = await _dbContext.BankAccounts.FirstOrDefaultAsync(b =>
                b.AccountNumber == accountNumber
            );

            if (bankAccount == null)
            {
                throw new NotFoundException("Данного счета не существует!");
            }

            if (bankAccount.IsLocked)
            {
                throw new NotFoundException("Счет заблокирован!");
            }

            bankAccount.Balance += moneyCount;
            await _dbContext.SaveChangesAsync();
        }

        public async Task WithdrawAccountNumber(
            string accountNumber,
            decimal moneyCount,
            Guid userId
        )
        {
            if (moneyCount <= 0)
            {
                throw new BadRequestException("Вы не можете снять меньше чем 1 деньгу!");
            }

            var bankAccount = await _dbContext.BankAccounts.FirstOrDefaultAsync(b =>
                b.AccountNumber == accountNumber
            );

            if (bankAccount == null)
            {
                throw new NotFoundException("Данного счета не существует!");
            }

            if (bankAccount.UserId != userId)
            {
                throw new BadRequestException("Вы не можете снять деньги с чужого счета!");
            }

            if (bankAccount.IsLocked)
            {
                throw new NotFoundException("Счет заблокирован!");
            }

            if (bankAccount.Balance - moneyCount < 0)
            {
                throw new BadRequestException("Деняг на счету не хватает!");
            }

            bankAccount.Balance -= moneyCount;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<BankAccountDto>> GetAccountNumbersWithUserId(Guid userId)
        {
            var bankAccounts = await _dbContext
                .BankAccounts.Where(bc => bc.UserId == userId)
                .ToListAsync();

            if (!bankAccounts.Any())
            {
                throw new BadRequestException("У этого пользователя нет счетов!");
            }

            return _mapper.Map<List<BankAccountDto>>(bankAccounts);
        }

        public async Task WithdrawMoneyForLoan(TransactionRequestDto transactionCreds)
        {
            var bankAccount = await _dbContext.BankAccounts.FirstOrDefaultAsync(bc =>
                bc.AccountNumber == transactionCreds.AccountNumber
            );

            if (bankAccount == null || bankAccount.Balance < transactionCreds.MoneyCount)
            {
                transactionCreds.Status = Status.Rejected;
            }
            else
            {
                transactionCreds.Status = Status.Approved;
                bankAccount.Balance -= transactionCreds.MoneyCount;
                await _dbContext.SaveChangesAsync();
            }
            var trans = new TransactionResponseDto()
            {
                TransactionId = transactionCreds.TransactionId,
                Status = transactionCreds.Status,
                AccountNumber = transactionCreds.AccountNumber,
                LoanId = transactionCreds.LoanId,
                MoneyCount = transactionCreds.MoneyCount,
            };

            await _queueSender.SendTransaction(trans);
        }

        private string GenerateAccountNumber()
        {
            Random random = new Random();
            char[] accountNumber = new char[12];

            for (int i = 0; i < 12; i++)
            {
                accountNumber[i] = (char)('0' + random.Next(0, 10));
            }

            return new string(accountNumber);
        }
    }
}
