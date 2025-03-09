using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UndergroundBank.Common.Data.Enums;
using UndergroundBank.Common.Dto.AccountService;

namespace UndergroundBank.AccountService.Application.Interfaces
{
    public interface IManagementService
    {
        public Task BlockUser(Guid userId, Guid currentUserId);
        public Task UnblockUser(Guid userId, Guid currentUserId);
        public Task<List<ProfileDto>> GetAllUsers(Role? role);
        public Task<InputManagerDataDto> CreateNewEmployee(ManagerDto manager);
        public Task DeleteUser(Guid userId, Guid currentUserId);
    }
}
