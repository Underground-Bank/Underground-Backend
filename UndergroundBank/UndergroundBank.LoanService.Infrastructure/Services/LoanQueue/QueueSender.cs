using EasyNetQ;
using UndergroundBank.Common.Data.Constants;
using UndergroundBank.Common.Dto.Transaction;
using UndergroundBank.Common.DTO.Transaction;
using UndergroundBank.Common.Middlewares;

namespace UndergroundBank.LoanService.Infrastructure.Services.LoanQueue
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

        public async Task<CheckBankAccountAccessResponse> CheckBankAccountAccess(
            CheckBankAccountAccessRequest checkAccessDto
        )
        {
            try
            {
                var accessionInfo = await _bus.Rpc.RequestAsync<
                    CheckBankAccountAccessRequest,
                    CheckBankAccountAccessResponse
                >(checkAccessDto, x => x.WithQueueName(Queues.CHECK_BANK_ACCOUNT_ACCESS));
                return accessionInfo;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                throw new BadRequestException(ex.Message);
            }
        }
        public async Task<List<GetOverduePaymentDto>> GetOverduePayments(
        Guid userId
        )
        {
            try
            {
                var overduePayments = await _bus.Rpc.RequestAsync<
                    Guid,
                    List<GetOverduePaymentDto>
                >(userId, x => x.WithQueueName(Queues.GET_OVERDUE_PAYMENTS));
                return overduePayments;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                throw new BadRequestException(ex.Message);
            }
        }
    }
}
