using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UndergroundBank.AccountService.Application.Dto;
using UndergroundBank.Common.Dto.AccountService;

namespace UndergroundBank.AccountService.Application.Interfaces
{
    public interface IProfileService
    {
        /// <summary>
        /// Method for getting user profile
        /// </summary>
        /// <param name="userId">Id of corresponding user</param>
        public Task<ProfileDto> GetUserProfile(string userId);

        /// <summary>
        /// Method for edit user profile
        /// </summary>
        /// <param name="editCreds">Parametrs which user can edit</param>
        public Task EditProfile(EditProfileInfoDto editCreds, string userId);
    }
}
