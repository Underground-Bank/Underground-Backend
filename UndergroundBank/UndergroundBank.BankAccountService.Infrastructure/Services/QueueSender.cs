using EasyNetQ;
using UndergroundBank.Common.Data.Constants;
using UndergroundBank.Common.Dto.Transaction;

namespace UndergroundBank.BankAccountService.Infrastructure.Services
{
    public class QueueSender
    {
        private IBus _bus;

        public QueueSender()
        {
            _bus = RabbitHutch.CreateBus("host=localhost");
        }

        public async Task SendMessage<T>(T message, string topik)
        {
            await _bus.PubSub.PublishAsync(message, topik);
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
