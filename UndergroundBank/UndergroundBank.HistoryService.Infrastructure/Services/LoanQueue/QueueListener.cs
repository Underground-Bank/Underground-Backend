using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using UndergroundBank.Common.Data.Constants;
using UndergroundBank.Common.Dto.Transaction;
using UndergroundBank.HistoryService.Application.Interfaces;

namespace UndergroundBank.LoanService.Infrastructure.Services.LoanQueue
{
    public static class QueueListener
    {
        public static void AddListeners(this IServiceCollection services)
        {
            IBus bus = RabbitHutch.CreateBus("host=localhost");
            var serviceProvider = services.BuildServiceProvider();
            var historyService = serviceProvider.GetRequiredService<IHistoryService>();

            bus.PubSub.Subscribe<OperationHistoryDto>(
                Queues.ADD_TO_HISTORY,
                data => historyService.AddToHistory(data)
            );
        }
    }
}
