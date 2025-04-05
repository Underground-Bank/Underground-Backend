using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UndergroundBank.AccountService.Application.Dto;
using UndergroundBank.Common.Data.Enums;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace UndergroundBank.AccountService.Application.Interfaces
{
    public interface IUserSettingsService
    {
        public Task<UserSettingsDto> GetUserSettings(Guid userId);
        public Task EditUserSettings(Guid userId, EditUserSettingsDto settings);
    }
}
