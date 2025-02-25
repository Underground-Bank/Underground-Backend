using AutoMapper;
using UndergroundBank.AccountService.Application.Dto;
using UndergroundBank.AccountService.Domain.Entities;

namespace UndergroundBank.AccountService.Application.Helpers.AutoMapper
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            CreateMap<RegisterInfoDto, User>();
        }
    }
}
