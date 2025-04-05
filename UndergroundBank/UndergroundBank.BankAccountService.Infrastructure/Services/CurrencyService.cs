using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UndergroundBank.BankAccountService.Application.Dto;
using UndergroundBank.BankAccountService.Application.Interfaces;
using UndergroundBank.BankAccountService.Domain.Entities;
using UndergroundBank.Common.Data.Enums;

namespace UndergroundBank.BankAccountService.Infrastructure.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly HttpClient _httpClient;
        private readonly BankAccountDbContext _dbContext;

        private const string BaseUrl = "https://www.cbr-xml-daily.ru/latest.js";

        public CurrencyService(HttpClient httpClient, BankAccountDbContext dbContext)
        {
            _httpClient = httpClient;
            _dbContext = dbContext;
        }

        public async Task GetCurrency()
        {
            var response = await _httpClient.GetStringAsync(BaseUrl);
            var result = JsonSerializer.Deserialize<ExchangeRateResponse>(
                response,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            var supportedCurrencies = new Dictionary<string, Currency>
            {
                { "USD", Currency.USD },
                { "CNY", Currency.CNY },
                { "KZT", Currency.KZT },
            };

            var today = DateTime.UtcNow.Date;

            foreach (var item in result.Rates)
            {
                if (!supportedCurrencies.TryGetValue(item.Key, out var enumCurrency))
                    continue;

                var existing = _dbContext.Currencies.FirstOrDefault(x =>
                    x.Currency == enumCurrency && x.UpdatedAt.Date == today
                );

                if (existing != null)
                {
                    existing.Value = item.Value;
                    existing.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    var currency = new CurrencyModel
                    {
                        Currency = enumCurrency,
                        Value = item.Value,
                        UpdatedAt = DateTime.UtcNow,
                    };

                    _dbContext.Currencies.Add(currency);
                }
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
