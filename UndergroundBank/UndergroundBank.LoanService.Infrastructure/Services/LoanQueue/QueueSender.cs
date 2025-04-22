using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EasyNetQ;
using Polly;
using Polly.Wrap;
using UndergroundBank.Common.Data.Constants;
using UndergroundBank.Common.Dto.Transaction;
using UndergroundBank.Common.DTO.Transaction;
using UndergroundBank.Common.Helpers.MessageBroker;
using UndergroundBank.Common.Middlewares;

namespace UndergroundBank.LoanService.Infrastructure.Services.LoanQueue
{
    public class QueueSender : ResilientQueueSender
    {
        public async Task SendMessage<T>(T message, string topic)
        {
            await ExecuteWithPolicies(() => _bus.PubSub.PublishAsync(message, topic));
        }

        public Task<CheckBankAccountAccessResponse> CheckBankAccountAccess(
            CheckBankAccountAccessRequest dto
        )
        {
            return ExecuteWithPolicies(
                () =>
                    _bus.Rpc.RequestAsync<
                        CheckBankAccountAccessRequest,
                        CheckBankAccountAccessResponse
                    >(dto, x => x.WithQueueName(Queues.CHECK_BANK_ACCOUNT_ACCESS))
            );
        }

        public Task<TransactionResponseDto> RequestMasterBankAccount(TransactionRequestDto dto)
        {
            return ExecuteWithPolicies(
                () =>
                    _bus.Rpc.RequestAsync<TransactionRequestDto, TransactionResponseDto>(
                        dto,
                        x => x.WithQueueName(Queues.WITHDRAW_MONEY_FROM_MASTER)
                    )
            );
        }

        public Task<List<GetOverduePaymentDto>> GetOverduePayments(Guid userId)
        {
            return ExecuteWithPolicies(
                () =>
                    _bus.Rpc.RequestAsync<Guid, List<GetOverduePaymentDto>>(
                        userId,
                        x => x.WithQueueName(Queues.GET_OVERDUE_PAYMENTS)
                    )
            );
        }
    }
}
