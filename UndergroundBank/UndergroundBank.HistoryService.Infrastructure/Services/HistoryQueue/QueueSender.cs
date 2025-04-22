using EasyNetQ;
using UndergroundBank.Common.Helpers.MessageBroker;

namespace UndergroundBank.HistoryService.Infrastructure.Services.HistoryQueue
{
    public class QueueSender : ResilientQueueSender
    {
        public async Task SendMessage<T>(T message, string topic)
        {
            await ExecuteWithPolicies(() => _bus.PubSub.PublishAsync(message, topic));
        }
    }
}
