using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UndergroundBank.Common.Data.Enums;

namespace UndergroundBank.BankAccountService.Domain.Entities
{
    public class CurrencyModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Currency Currency { get; set; }
        public decimal Value { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
