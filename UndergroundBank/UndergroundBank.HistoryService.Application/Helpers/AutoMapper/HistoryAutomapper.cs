using AutoMapper;
using UndergroundBank.Common.Dto.Transaction;
using UndergroundBank.LoanService.Domain.Entities;


namespace UndergroundBank.BankAccountService.Application.Helpers.AutoMapper
{
    public class HistoryAutomapper : Profile
    {
        public HistoryAutomapper()
        {
            CreateMap<OperationResultDto, OperationsHistoryElement>().ReverseMap();
        }
    }
}
