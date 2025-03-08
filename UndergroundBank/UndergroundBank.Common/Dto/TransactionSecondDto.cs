using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UndergroundBank.Common.Data.Enums;

namespace UndergroundBank.Common.Dto
{
    public class TransactionSecondDto
    {
        public Guid TransactionId { get; set; }
        public Guid LoanId { get; set; }
        public string AccountNumber { get; set; }
        public decimal MoneyCount { get; set; }
        public Status Status { get; set; }
    }
}
