using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UndergroundBank.AccountService.Domain.Entities;
using UndergroundBank.Common.Base.Interfaces;

namespace UndergroundBank.AccountService.Application.Interfaces
{
    public interface IUserRepository : IBaseRepository<User> { }
}
