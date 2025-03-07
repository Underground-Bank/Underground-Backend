using UndergroundBank.Common.Dto.BankAccountService;

namespace UndergroundBank.BankAccountService.Application.Interfaces
{
    public interface IBankService
    {
        public Task CreateAccountNumber(Guid userId);

        public Task<BankAccountDto> GetMyCorrespondingAccountNumber(
            string accountNumber,
            Guid userId
        );
        public Task<List<BankAccountDto>> GetMyAccountNumbers(Guid userId);

        public Task DeleteAccountNumber(string accountNumber);

        public Task BlockAccountNumber(string accountNumber);

        public Task UnblockAccountNumber(string accountNumber);

        public Task<List<BankAccountDto>> GetAllAccountNumbers();

        public Task<List<BankAccountDto>> GetAccountNumbersWithUserId(Guid userId);

        public Task TopUpAccountNumber(string accountNumber, int moneyCount);

        public Task WithdrawAccountNumber(string accountNumber, int moneyCount);
    }
}
