using UndergroundBank.Common.Data.Models;

namespace UndergroundBank.HistoryService.Domain.Entities
{
    public class OverduePayment
    {
        public Guid LoanId { get; set; }

        /// <summary>
        /// FK for Transaction
        /// </summary>
        public Guid TransactionId { get; set; }
        public Transaction Transaction { get; set; }
    }
}
