using AutoMapper;
using UndergroundBank.Common.Dto.Transaction;
using UndergroundBank.HistoryService.Application.DTO;
using UndergroundBank.LoanService.Domain.Entities;

namespace UndergroundBank.BankAccountService.Application.Helpers.AutoMapper
{
    public class HistoryAutomapper : Profile
    {
        public HistoryAutomapper()
        {
            CreateMap<OperationsHistoryElement, OperationsHistoryDto>();
            CreateMap<OperationHistoryDto, OperationsHistoryElement>();
        }
    }
}
