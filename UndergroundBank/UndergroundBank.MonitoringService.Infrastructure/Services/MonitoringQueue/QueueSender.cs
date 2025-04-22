//using EasyNetQ;

//namespace UndergroundBank.HistoryService.Infrastructure.Services.HistoryQueue
//{
//    public class QueueSender
//    {
//        private IBus _bus;
//        public QueueSender()
//        {
//            _bus = RabbitHutch.CreateBus("host=localhost");
//        }

//        public async Task SendMessage<T>(T message, string topik)
//        {
//            await _bus.PubSub.PublishAsync(message, topik);
//        }
//    }
//}
