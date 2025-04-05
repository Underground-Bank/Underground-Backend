using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UndergroundBank.AccountService.Application.Dto;
using UndergroundBank.AccountService.Application.Interfaces;
using UndergroundBank.AccountService.Domain.Entities;
using UndergroundBank.Common.Data.Enums;
using UndergroundBank.Common.Dto.BankAccountService;
using UndergroundBank.Common.Middlewares;

namespace UndergroundBank.AccountService.Infrastructure.Services
{
    public class UserSettingsService : IUserSettingsService
    {
        private readonly IMapper _mapper;
        private readonly AccountDbContext _dbContext;

        public UserSettingsService(IMapper mapper, AccountDbContext dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task EditUserSettings(Guid userId, EditUserSettingsDto settings)
        {
            var userSettings = await _dbContext.UserSettings.FirstOrDefaultAsync(us =>
                us.UserId == userId
            );
            if (userSettings != null)
            {
                userSettings.Theme = settings.Theme;
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                var newUserSettings = new UserSettings()
                {
                    UserId = userId,
                    Theme = settings.Theme,
                };
                _dbContext.UserSettings.Add(newUserSettings);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<UserSettingsDto> GetUserSettings(Guid userId)
        {
            var userSettings = await _dbContext.UserSettings.FirstOrDefaultAsync(us =>
                us.UserId == userId
            );
            if (userSettings == null)
            {
                throw new BadRequestException(
                    "Добавьте настройки, а только потом уже запрашивайте!"
                );
            }
            return _mapper.Map<UserSettingsDto>(userSettings);
        }
    }
}
