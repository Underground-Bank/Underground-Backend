using EasyNetQ;
using UndergroundBank.Common.Data.Constants;
using UndergroundBank.Common.Dto;

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

        public async Task SendTransaction(TransactionSecondDto transaction)
        {
            await SendMessage(transaction, Queues.TRANSACTION_QUEUE_RESPONSE);
        }
    }
}
