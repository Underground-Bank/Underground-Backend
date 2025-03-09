using AutoMapper;
using UndergroundBank.BankAccountService.Domain.Entities;
using UndergroundBank.Common.Dto.AccountService;
using UndergroundBank.Common.Dto.BankAccountService;

namespace UndergroundBank.BankAccountService.Application.Helpers.AutoMapper
{
    public class BankAccountMapper : Profile
    {
        public BankAccountMapper()
        {
            CreateMap<ProfileDto, BankAccount>();
            CreateMap<BankAccount, BankAccountDto>();
            CreateMap<BankAccount, List<BankAccountDto>>();
        }
    }
}
