using System.Transactions;
using UndergroundBank.BankAccountService.Application.Dto;
using UndergroundBank.Common.Data.Enums;
using UndergroundBank.Common.Dto.BankAccountService;
using UndergroundBank.Common.Dto.Transaction;

namespace UndergroundBank.BankAccountService.Application.Interfaces
{
    public interface IBankService
    {
        public Task CreateAccountNumber(Guid userId, Currency currency);

        public Task ChangeVisibilityOfBankAccount(string accountNumber);

        public Task<BankAccountDto> GetMyCorrespondingAccountNumber(
            string accountNumber,
            Guid userId,
            List<Role> userRoles
        );
        public Task<List<BankAccountDto>> GetMyAccountNumbers(Guid userId);

        public Task DeleteAccountNumber(string accountNumber);

        public Task BlockAccountNumber(string accountNumber);

        public Task UnblockAccountNumber(string accountNumber);

        public Task<List<BankAccountDto>> GetAllAccountNumbers();

        public Task<List<BankAccountDto>> GetAccountNumbersWithUserId(Guid userId);

        public Task TopUpAccountNumber(string accountNumber, decimal moneyCount, Currency currency);
        public Task TransferMoneyToAccountNumber(MoneyTransferDto moneyTransfer);
        public Task<TransactionResponseDto> WithdrawMoneyFromMasterAccount(
            TransactionRequestDto transactionCreds
        );

        public Task WithdrawAccountNumber(string accountNumber, decimal moneyCount, Guid userId);
        public Task WithdrawMoneyForLoan(TransactionRequestDto transactionDtoCreds);

        public Task<CheckBankAccountAccessResponse> CheckAccountNumberExists(
            CheckBankAccountAccessRequest request
        );
    }
}
