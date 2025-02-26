using AutoMapper;
using UndergroundBank.AccountService.Application.Dto;
using UndergroundBank.AccountService.Domain.Entities;
using UndergroundBank.Common.Dto.AccountService;

namespace UndergroundBank.AccountService.Application.Helpers.AutoMapper
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            CreateMap<RegisterInfoDto, User>();
            CreateMap<User, ProfileDto>();
        }
    }
}
