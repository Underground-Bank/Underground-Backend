using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using UndergroundBank.Common.Data.Constants;
using UndergroundBank.Common.Dto.Transaction;
using UndergroundBank.Common.DTO.Transaction;
using UndergroundBank.HistoryService.Application.DTO;
using UndergroundBank.HistoryService.Application.Interfaces;

namespace UndergroundBank.HistoryService.Infrastructure.Services.HistoryQueue;

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

        bus.Rpc.Respond<Guid, List<GetOverduePaymentDto>>(
        async request =>
        {
            return await historyService.GetOverduedPayments(loanId: null, userId: request);
        },
        x => x.WithQueueName(Queues.GET_OVERDUE_PAYMENTS)
        );

        bus.PubSub.Subscribe<OverduePaymentDto>(
            Queues.ADD_OVERDUE_PAYMENT,
            data => historyService.AddOverduePayment(data)
        );
    }
}

