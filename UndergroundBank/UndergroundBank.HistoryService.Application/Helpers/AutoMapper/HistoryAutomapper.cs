using AutoMapper;
using UndergroundBank.Common.Dto.Transaction;
using UndergroundBank.Common.DTO.Transaction;
using UndergroundBank.HistoryService.Application.DTO;
using UndergroundBank.HistoryService.Domain.Entities;

namespace UndergroundBank.BankAccountService.Application.Helpers.AutoMapper
{
    public class HistoryAutomapper : Profile
    {
        public HistoryAutomapper()
        {
            CreateMap<OperationsHistoryElement, OperationsHistoryDto>();
            CreateMap<OperationHistoryDto, OperationsHistoryElement>();
            CreateMap<GetOverduePaymentDto, OverduePayment>();
        }
    }
}
