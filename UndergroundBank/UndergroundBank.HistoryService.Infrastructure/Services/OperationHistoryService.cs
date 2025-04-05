using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using UndergroundBank.Common.Data.Constants;
using UndergroundBank.Common.Dto.Transaction;
using UndergroundBank.Common.DTO.Transaction;
using UndergroundBank.Common.Middlewares;
using UndergroundBank.HistoryService.Application.DTO;
using UndergroundBank.HistoryService.Application.Interfaces;
using UndergroundBank.HistoryService.Domain.Entities;

namespace UndergroundBank.HistoryService.Infrastructure.Services
{
    public class OperationHistoryService : IHistoryService
    {
        private readonly IMapper _mapper;
        private readonly HistoryDbContext _dbContext;
        private readonly IHubContext<OperationHistoryHub> _historyHub;

        public OperationHistoryService(
            HistoryDbContext dbContext,
            IMapper mapper,
            IHubContext<OperationHistoryHub> operationHistoryHub
        )
        {
            _mapper = mapper;
            _dbContext = dbContext;
            _historyHub = operationHistoryHub;
        }

        public async Task<GetOpeationsHistoryDto> GetOperationsHistory(
            string bankAccountNumber,
            Guid? userId
        )
        {
            var operationsHistory = await _dbContext
                .OperationsHistory.Where(x => x.AccountNumber == bankAccountNumber)
                .ToListAsync();

            if (!operationsHistory.Any())
            {
                return new GetOpeationsHistoryDto
                {
                    operationsHistory = new List<OperationsHistoryDto>(),
                };
            }

            if (userId.HasValue && operationsHistory.Any(a => a.UserId != userId.Value))
            {
                throw new ForbiddenException("У вас нет доступа к историям этого счета");
            }

            return new GetOpeationsHistoryDto
            {
                operationsHistory = _mapper.Map<List<OperationsHistoryDto>>(operationsHistory),
            };
        }

        public async Task AddToHistory(OperationHistoryDto operationHistoryDto)
        {
            var historyElement = _mapper.Map<OperationsHistoryElement>(operationHistoryDto);
            await _dbContext.AddAsync(historyElement);
            await _dbContext.SaveChangesAsync();
            var operationsHistoryDto = _mapper.Map<OperationsHistoryDto>(historyElement);

            await _historyHub.Clients.All.SendAsync(
                WebSockets.TRANSACTION_UPDATED,
                operationsHistoryDto
            );
        }

        public async Task AddOverduePayment(OverduePaymentDto overduePaymentDto)
        {
            var historyElement = _mapper.Map<OperationsHistoryElement>(
                overduePaymentDto.OperationHistoryDto
            );
            await _dbContext.AddAsync(historyElement);
            var overduePayment = new OverduePayment
            {
                LoanId = overduePaymentDto.LoanId,
                TransactionId = historyElement.TransactionId,
            };
            await _dbContext.AddAsync(overduePayment);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<GetOverduePaymentDto>> GetOverduedPayments(Guid? loanId, Guid userId)
        {
            var overduedPayments = _dbContext.OverduePayments.Where(p =>
                p.Transaction.UserId == userId
            );
            if (loanId != null)
            {
                overduedPayments = overduedPayments.Where(p => p.LoanId == loanId);
            }
            var overduePaymentsDto = _mapper.Map<List<GetOverduePaymentDto>>(
                await overduedPayments.ToListAsync()
            );
            return overduePaymentsDto;
        }
    }
}
