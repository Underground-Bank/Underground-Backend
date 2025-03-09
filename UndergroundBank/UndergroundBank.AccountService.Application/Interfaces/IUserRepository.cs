using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;
using UndergroundBank.AccountService.Domain.Entities;
using UndergroundBank.Common.Base.Interfaces;

namespace UndergroundBank.AccountService.Application.Interfaces
{
    public interface IUserRepository : IBaseRepository<User>
    {
        public Task<List<User>> GetAllUsers();
    }
}
