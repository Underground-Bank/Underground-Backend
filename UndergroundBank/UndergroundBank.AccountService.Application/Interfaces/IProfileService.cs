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

        /// <summary>
        /// Method for change user password
        /// </summary>
        /// <param name="changePassword">Creds for password from corresponding user</param>
        /// <param name="userId">Id of correspoding user</param>
        public Task ChangePassword(ChangePasswordDto changePassword, string userId);

        /// <summary>
        /// Method for add bundle of Firebase and user
        /// </summary>
        /// <param name="userId">Id of correspoding user</param>
        /// <param name="firebaseToken">Token from firebase</param>
        public Task AddFirebaseToken(string userId, string firebaseToken);
    }
}
