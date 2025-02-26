using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UndergroundBank.AccountService.Application.Dto;

namespace UndergroundBank.AccountService.Application.Interfaces
{
    public interface IAuthService
    {
        /// <summary>
        /// Method to authorize user in system
        /// </summary>
        /// <param name="loginCreds">Login creds</param>
        public Task<AuthResponseDto> Login(LoginInfoDto loginCreds);

        /// <summary>
        /// Method for add new user to system
        /// </summary>
        /// <param name="registerCreds">Register creds</param>
        /// <returns></returns>
        public Task<AuthResponseDto> Register(RegisterInfoDto registerCreds);

        /// <summary>
        /// Method for logout from system
        /// </summary>
        public Task Logout(string token, string userId);
    }
}
