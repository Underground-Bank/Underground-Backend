namespace UndergroundBank.Common.Data.Constants
{
    public class Queues
    {
        public const string TRANSACTION_QUEUE_REQUEST = "loan_transaction_request";
        public const string TRANSACTION_QUEUE_RESPONSE = "bankaccount_transaction_response";
        public const string ADD_TO_HISTORY = "add_to_history";
        public const string CHECK_BANK_ACCOUNT_ACCESS = "check_bank_account_access";
        public const string TOP_UP_BANK_ACCOUNT_FROM_LOAN = "top_up_bank_account_from_loan";
    }
}
