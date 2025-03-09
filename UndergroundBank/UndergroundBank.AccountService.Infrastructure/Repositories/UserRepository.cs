using Microsoft.EntityFrameworkCore;
using UndergroundBank.AccountService.Application.Interfaces;
using UndergroundBank.AccountService.Domain.Entities;
using UndergroundBank.Common.Base;
using UndergroundBank.Common.Base.Interfaces;

namespace UndergroundBank.AccountService.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User, AccountDbContext>, IUserRepository
    {
        private readonly AccountDbContext _accountContext;

        public UserRepository(AccountDbContext dbContext)
            : base(dbContext)
        {
            _accountContext = dbContext;
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _accountContext.Users.Where(u => u.Id != null).AsQueryable().ToListAsync();
        }
    }
}
