using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UndergroundBank.BankAccountService.Domain.Entities
{
    public static class BankDefaults
    {
        public const string MasterAccountNumber = "0";
        public static readonly Guid MasterUserId = Guid.NewGuid();
    }
}
