using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UndergroundBank.BankAccountService.Application.Interfaces
{
    public interface ICurrencyService
    {
        public Task GetCurrency();
    }
}
