using System.Transactions;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using UndergroundBank.BankAccountService.Application.Interfaces;
using UndergroundBank.Common.Data.Constants;
using UndergroundBank.Common.Dto.AccountService;
using UndergroundBank.Common.Dto.Transaction;

namespace UndergroundBank.BankAccountService.Infrastructure.MessageBroker
{
    public static class QueueListener
    {
        public static void QueueSubscribe(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var bus = RabbitHutch.CreateBus("host=localhost");
            var bankAccountService = serviceProvider.GetRequiredService<IBankService>();

            bus.PubSub.Subscribe<TopUpBankAccountTransaction>(
                Queues.TOP_UP_BANK_ACCOUNT_FROM_LOAN,
                async data =>
                {
                    await bankAccountService.TopUpAccountNumber(
                        data.AccountNumber,
                        data.MoneyCount
                    );
                },
                x => x.WithAutoDelete()
            );

            bus.PubSub.Subscribe<TransactionRequestDto>(
                Queues.TRANSACTION_QUEUE_REQUEST,
                async data =>
                {
                    await bankAccountService.WithdrawMoneyForLoan(data);
                },
                x => x.WithAutoDelete()
            );

            bus.Rpc.Respond<CheckBankAccountAccessRequest, CheckBankAccountAccessResponse>(
                async request =>
                {
                    return await bankAccountService.CheckAccountNumberExists(request);
                },
                x => x.WithQueueName(Queues.CHECK_BANK_ACCOUNT_ACCESS)
            );
        }
    }
}
