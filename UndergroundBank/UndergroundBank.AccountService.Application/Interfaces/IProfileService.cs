using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UndergroundBank.Common.Dto.AccountService;

namespace UndergroundBank.AccountService.Application.Interfaces
{
    public interface IProfileService
    {
        public Task<ProfileDto> GetUserProfile(string userId);
    }
}
