using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UndergroundBank.Common.Dto.BankAccountService
{
    public class BankAccountDto
    {
        public string AccountNumber { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public decimal Balance { get; set; }
        public bool IsLocked { get; set; }
        public bool IsHidden { get; set; }
    }
}
