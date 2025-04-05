using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UndergroundBank.Common.Data.Enums;
using UndergroundBank.Common.Middlewares;

namespace UndergroundBank.BankAccountService.Infrastructure.Services
{
    public class AdditionalCurrencyService
    {
        private readonly BankAccountDbContext _bankAccountContext;

        public AdditionalCurrencyService(BankAccountDbContext bankAccountContext)
        {
            _bankAccountContext = bankAccountContext;
        }

        public async Task<decimal> CalculateMoneyCount(
            Currency fromCurrency,
            Currency toCurrency,
            decimal moneyCount
        )
        {
            if (fromCurrency == toCurrency)
                return moneyCount;

            if (fromCurrency == Currency.RUB)
            {
                return await FromRubles(toCurrency, moneyCount);
            }

            if (toCurrency == Currency.RUB)
            {
                return await ToRubles(fromCurrency, moneyCount);
            }

            var moneyInRubles = await ToRubles(fromCurrency, moneyCount);
            return await FromRubles(toCurrency, moneyInRubles);
        }

        private async Task<decimal> ToRubles(Currency fromCurrency, decimal amount)
        {
            var rate = await _bankAccountContext.Currencies.FirstOrDefaultAsync(c =>
                c.Currency == fromCurrency
            );

            if (rate == null || rate.Value == 0)
                throw new NotFoundException($"Не удалось найти курс для {fromCurrency}");

            return amount / rate.Value;
        }

        private async Task<decimal> FromRubles(Currency toCurrency, decimal amount)
        {
            var rate = await _bankAccountContext.Currencies.FirstOrDefaultAsync(c =>
                c.Currency == toCurrency
            );

            if (rate == null || rate.Value == 0)
                throw new NotFoundException($"Не удалось найти курс для {toCurrency}");

            return amount * rate.Value;
        }
    }
}
