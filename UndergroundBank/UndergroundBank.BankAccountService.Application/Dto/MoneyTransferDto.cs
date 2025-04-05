using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace UndergroundBank.BankAccountService.Application.Dto
{
    public class MoneyTransferDto
    {
        public string AccountNumberSender { get; set; }
        public string AccountNumberConsumer { get; set; }

        public decimal MoneyCount { get; set; }
    }
}
