using AutoMapper;
using UndergroundBank.LoanService.Application.Dto.Tariff;
using UndergroundBank.LoanService.Domain.Entities;

namespace UndergroundBank.LoanService.Application.Helpers.Automapper
{
    public class LoanServiceMapper : Profile
    {
        public LoanServiceMapper()
        {
            CreateMap<CreateTariffDto, Tariff>().ReverseMap();
            CreateMap<List<Tariff>, List<GetTariffDto>>().ReverseMap();
        }
    }
}
