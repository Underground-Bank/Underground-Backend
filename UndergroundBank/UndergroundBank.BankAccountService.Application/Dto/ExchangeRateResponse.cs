using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UndergroundBank.BankAccountService.Application.Dto
{
    public class ExchangeRateResponse
    {
        public string Disclaimer { get; set; }
        public string Base { get; set; }
        public string Date { get; set; }
        public long Timestamp { get; set; }
        public Dictionary<string, decimal> Rates { get; set; }
    }
}
