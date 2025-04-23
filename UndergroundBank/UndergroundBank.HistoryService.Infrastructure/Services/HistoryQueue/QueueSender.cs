using EasyNetQ;
using UndergroundBank.Common.Data.Constants;
using UndergroundBank.Common.Dto.AccountService;
using UndergroundBank.Common.Dto.Notification;
using UndergroundBank.Common.Dto.Transaction;
using UndergroundBank.Common.Helpers.MessageBroker;

namespace UndergroundBank.HistoryService.Infrastructure.Services.HistoryQueue
{
    public class QueueSender : ResilientQueueSender
    {
        public async Task SendMessage<T>(T message, string topic)
        {
            await ExecuteWithPolicies(() => _bus.PubSub.PublishAsync(message, topic));
        }

        public async Task SendMessageInfo(NotificationDto dto)
        {
            await SendMessage(dto, Queues.NOTIFICATION_SEND);
        }
    }
}
