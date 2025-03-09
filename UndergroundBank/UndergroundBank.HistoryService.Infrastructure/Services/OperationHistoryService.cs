using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UndergroundBank.Common.Dto.Transaction;
using UndergroundBank.Common.Middlewares;
using UndergroundBank.HistoryService.Application.DTO;
using UndergroundBank.HistoryService.Application.Interfaces;
using UndergroundBank.LoanService.Domain.Entities;

namespace UndergroundBank.HistoryService.Infrastructure.Services
{
    public class OperationHistoryService : IHistoryService
    {
        private readonly IMapper _mapper;
        private readonly HistoryDbContext _dbContext;

        public OperationHistoryService(
            HistoryDbContext dbContext,
            IMapper mapper
        )
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<GetOpeationsHistoryDto> GetOperationsHistory(string bankAccountNumber, Guid? userId)
        {
            var operationsHistory = await _dbContext.OperationsHistory
                .Where(x => x.AccountNumber == bankAccountNumber)
                .ToListAsync();

            if (!operationsHistory.Any())
            {
                return new GetOpeationsHistoryDto { operationsHistory = new List<OperationsHistoryDto>() };
            }

            if (userId.HasValue && operationsHistory.Any(a => a.UserId != userId.Value))
            {
                throw new ForbiddenException("У вас нет доступа к историям этого счета");
            }

            return new GetOpeationsHistoryDto
            {
                operationsHistory = _mapper.Map<List<OperationsHistoryDto>>(operationsHistory)
            };
        }

        public async Task AddToHistory(OperationResultDto operationHistoryDto)
        {
            var historyElement = _mapper.Map<OperationsHistoryElement>(operationHistoryDto);
            await _dbContext.AddAsync(historyElement);
            _dbContext.SaveChanges();
        }
    }
}
