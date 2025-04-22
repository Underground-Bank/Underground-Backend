using AutoMapper;
using EasyNetQ;
using Microsoft.EntityFrameworkCore;
using UndergroundBank.BankAccountService.Application.Dto;
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
        private readonly AdditionalCurrencyService _additionalCurrencyService;

        public BankService(
            BankAccountDbContext dBContext,
            IMapper mapper,
            QueueSender queueSender,
            AdditionalCurrencyService additionalCurrencyService
        )
        {
            _dbContext = dBContext;
            _mapper = mapper;
            _bus = RabbitHutch.CreateBus("host=localhost");
            _queueSender = queueSender;
            _additionalCurrencyService = additionalCurrencyService;
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

        public async Task ChangeVisibilityOfBankAccount(string accountNumber)
        {
            var bankAccount = await _dbContext.BankAccounts.FirstOrDefaultAsync(b =>
                b.AccountNumber == accountNumber
            );
            if (bankAccount == null)
            {
                throw new NotFoundException("Данного счета не существует!");
            }
            bankAccount.IsHidden = !bankAccount.IsHidden;
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

        public async Task CreateAccountNumber(Guid userId, Currency currency)
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
            account.IsHidden = false;
            account.Currency = currency;
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

        public async Task TransferMoneyToAccountNumber(MoneyTransferDto moneyTransfer)
        {
            if (moneyTransfer.MoneyCount <= 0)
            {
                throw new BadRequestException("Вы не можете перевести меньше чем на 1 деньгу!");
            }

            var bankAccount = await _dbContext.BankAccounts.FirstOrDefaultAsync(b =>
                b.AccountNumber == moneyTransfer.AccountNumberSender
            );

            if (bankAccount == null)
            {
                throw new NotFoundException("Счет с которого вы хотите отправить не существует!");
            }

            if (bankAccount.IsLocked)
            {
                throw new NotFoundException("Счет заблокирован!");
            }
            var bankAccountConsumer = await _dbContext.BankAccounts.FirstOrDefaultAsync(b =>
                b.AccountNumber == moneyTransfer.AccountNumberConsumer
            );

            if (bankAccountConsumer == null)
            {
                throw new NotFoundException("Счет на который вы хотите отправить не существует!");
            }

            if (bankAccountConsumer.IsLocked)
            {
                throw new NotFoundException("Счет заблокирован!");
            }

            if (bankAccount.Balance - moneyTransfer.MoneyCount < 0)
            {
                throw new BadRequestException("Вы не можете отправить такое количество деняг");
            }

            var convertedAmount = await _additionalCurrencyService.CalculateMoneyCount(
                bankAccount.Currency,
                bankAccountConsumer.Currency,
                moneyTransfer.MoneyCount
            );

            bankAccount.Balance -= moneyTransfer.MoneyCount;
            bankAccountConsumer.Balance += convertedAmount;

            await _dbContext.SaveChangesAsync();
            var operation = new OperationHistoryDto()
            {
                AccountNumber = bankAccount.AccountNumber,
                CreatedAt = DateTime.UtcNow,
                MoneyCount = moneyTransfer.MoneyCount,
                Status = Status.Approved,
                TransactionId = Guid.NewGuid(),
                UserId = bankAccount.UserId,
                TransactionType = TransactionType.TransferMoney,
                DestinationAccountNumber = bankAccountConsumer.AccountNumber,
                DestinationType = AccountType.Account,
            };

            await _queueSender.SendOperationInfo(operation);
        }

        public async Task TopUpAccountNumber(
            string accountNumber,
            decimal moneyCount,
            Currency currency
        )
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

            var convertedAmount = await _additionalCurrencyService.CalculateMoneyCount(
                currency,
                bankAccount.Currency,
                moneyCount
            );

            bankAccount.Balance += convertedAmount;

            await _dbContext.SaveChangesAsync();
            var operation = new OperationHistoryDto()
            {
                AccountNumber = accountNumber,
                CreatedAt = DateTime.UtcNow,
                MoneyCount = moneyCount,
                Status = Status.Approved,
                TransactionId = Guid.NewGuid(),
                UserId = bankAccount.UserId,
                TransactionType = TransactionType.TopUp,
            };
            await _queueSender.SendOperationInfo(operation);
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
            var operation = new OperationHistoryDto()
            {
                AccountNumber = accountNumber,
                CreatedAt = DateTime.UtcNow,
                MoneyCount = moneyCount,
                Status = Status.Approved,
                TransactionId = Guid.NewGuid(),
                UserId = bankAccount.UserId,
                TransactionType = TransactionType.Withdraw,
            };
            await _queueSender.SendOperationInfo(operation);
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
                bc.AccountNumber == transactionCreds.To.AccountNumber
            );

            if (bankAccount == null || bankAccount.Balance < transactionCreds.MoneyCount)
            {
                transactionCreds.Status = Status.Rejected;
            }
            else
            {
                transactionCreds.Status = Status.Approved;
                var convertedAmount = await _additionalCurrencyService.CalculateMoneyCount(
                    transactionCreds.Currency,
                    bankAccount.Currency,
                    transactionCreds.MoneyCount
                );

                bankAccount.Balance -= convertedAmount;
                await _dbContext.SaveChangesAsync();
            }
            var trans = new TransactionResponseDto()
            {
                TransactionId = transactionCreds.TransactionId,
                Status = transactionCreds.Status,
                To = new TransferEndpoint
                {
                    LoanId = transactionCreds.From.GetLoanId(),
                    Type = AccountType.Loan,
                },
                UserId = bankAccount.UserId,
                From = new TransferEndpoint
                {
                    AccountNumber = transactionCreds.To.GetAccountNumber(),
                    Type = AccountType.Account,
                },
                Currency = transactionCreds.Currency,
                MoneyCount = transactionCreds.MoneyCount,
            };

            await _queueSender.SendTransaction(trans);
            var operation = _mapper.Map<OperationHistoryDto>(trans);
            operation.AccountNumber = trans.From.GetAccountNumber();
            operation.DestinationLoanId = trans.To.GetLoanId();
            operation.CreatedAt = DateTime.UtcNow;
            operation.TransactionType = TransactionType.LoanPayment;
            await _queueSender.SendOperationInfo(operation);
        }

        public async Task<TransactionResponseDto> WithdrawMoneyFromMasterAccount(
            TransactionRequestDto transactionCreds
        )
        {
            if (true)
            {
                throw new InternalServerErrorException("500");
            }
            var bankAccount = await _dbContext.BankAccounts.FirstOrDefaultAsync(bc =>
                bc.AccountNumber == transactionCreds.To.AccountNumber
            );

            if (bankAccount == null || bankAccount.Balance < transactionCreds.MoneyCount)
            {
                transactionCreds.Status = Status.Rejected;
            }
            else
            {
                transactionCreds.Status = Status.Approved;
                var convertedAmount = await _additionalCurrencyService.CalculateMoneyCount(
                    transactionCreds.Currency,
                    bankAccount.Currency,
                    transactionCreds.MoneyCount
                );

                bankAccount.Balance -= convertedAmount;
                await _dbContext.SaveChangesAsync();
            }
            var trans = new TransactionResponseDto()
            {
                TransactionId = transactionCreds.TransactionId,
                Status = transactionCreds.Status,
                To = new TransferEndpoint
                {
                    LoanId = transactionCreds.From.GetLoanId(),
                    Type = AccountType.Loan,
                },
                UserId = bankAccount.UserId,
                From = new TransferEndpoint
                {
                    AccountNumber = transactionCreds.To.GetAccountNumber(),
                    Type = AccountType.Account,
                },
                MoneyCount = transactionCreds.MoneyCount,
                Currency = transactionCreds.Currency,
            };

            var operation = _mapper.Map<OperationHistoryDto>(trans);
            operation.AccountNumber = trans.From.GetAccountNumber();
            operation.DestinationLoanId = trans.To.GetLoanId();
            operation.CreatedAt = DateTime.UtcNow;
            operation.TransactionType = TransactionType.LoanPayment;
            await _queueSender.SendOperationInfo(operation);

            return trans;
        }

        public async Task<CheckBankAccountAccessResponse> CheckAccountNumberExists(
            CheckBankAccountAccessRequest request
        )
        {
            var bankAccount = await _dbContext.BankAccounts.FirstOrDefaultAsync(b =>
                b.AccountNumber == request.BankAccountNumber
            );

            if (bankAccount == null)
            {
                return new CheckBankAccountAccessResponse
                {
                    IsBankAccountExists = false,
                    HasUserAccess = false,
                };
            }

            bool hasUserAccess = bankAccount.UserId == request.UserId;

            return new CheckBankAccountAccessResponse
            {
                IsBankAccountExists = true,
                HasUserAccess = hasUserAccess,
            };
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
