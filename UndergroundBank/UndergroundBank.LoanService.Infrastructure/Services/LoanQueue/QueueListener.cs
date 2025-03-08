using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using UndergroundBank.Common.Data.Constants;
using UndergroundBank.Common.Dto.Transaction;
using UndergroundBank.LoanService.Application.Interfaces;

namespace UndergroundBank.LoanService.Infrastructure.Services.LoanQueue
{
    public static class QueueListener
    {
        public static void AddListeners(this IServiceCollection services)
        {
            IBus bus = RabbitHutch.CreateBus("host=localhost");
            var serviceProvider = services.BuildServiceProvider();
            var loanService = serviceProvider.GetRequiredService<ILoanService>();

            bus.PubSub.Subscribe<TransactionRequestDto>(
                Queues.TRANSACTION_QUEUE_RESPONSE,
                data => loanService.EndTopUpLoanTransaction(data)
            );
        }
    }
}
