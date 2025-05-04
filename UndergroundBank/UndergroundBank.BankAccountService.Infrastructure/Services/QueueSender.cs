using EasyNetQ;
using UndergroundBank.Common.Data.Constants;
using UndergroundBank.Common.Dto.Transaction;
using UndergroundBank.Common.Helpers.MessageBroker;

namespace UndergroundBank.BankAccountService.Infrastructure.Services
{
    public class QueueSender : ResilientQueueSender
    {
        public async Task SendMessage<T>(T message, string topic)
        {
            await ExecuteWithPolicies(() => _bus.PubSub.PublishAsync(message, topic));
        }

        public async Task SendTransaction(TransactionResponseDto transaction)
        {
            await SendMessage(transaction, Queues.TRANSACTION_QUEUE_RESPONSE);
        }

        public async Task SendOperationInfo(OperationHistoryDto operation)
        {
            await SendMessage(operation, Queues.ADD_TO_HISTORY);
        }
    }
}
