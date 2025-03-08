using System.Transactions;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using UndergroundBank.BankAccountService.Application.Interfaces;
using UndergroundBank.Common.Data.Constants;
using UndergroundBank.Common.Dto;
using UndergroundBank.Common.Dto.AccountService;

namespace UndergroundBank.BankAccountService.Infrastructure.MessageBroker
{
    public static class QueueListener
    {
        public static void QueueSubscribe(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var bus = RabbitHutch.CreateBus("host=localhost");
            var bankAccountService = serviceProvider.GetRequiredService<IBankService>();

            bus.PubSub.Subscribe<TransactionDto>(
                Queues.TRANSACTION_QUEUE_REQUEST,
                async data =>
                {
                    await bankAccountService.WithdrawMoneyForLoan(data);
                },
                x => x.WithAutoDelete()
            );
        }
    }
}
