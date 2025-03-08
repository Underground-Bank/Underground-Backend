using System.Transactions;
using UndergroundBank.Common.Data.Enums;
using UndergroundBank.Common.Dto;
using UndergroundBank.Common.Dto.BankAccountService;

namespace UndergroundBank.BankAccountService.Application.Interfaces
{
    public interface IBankService
    {
        public Task CreateAccountNumber(Guid userId);

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

        public Task TopUpAccountNumber(string accountNumber, decimal moneyCount);

        public Task WithdrawAccountNumber(string accountNumber, decimal moneyCount, Guid userId);
        public Task WithdrawMoneyForLoan(TransactionDto transactionDtoCreds);
    }
}
