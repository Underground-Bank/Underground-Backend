using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UndergroundBank.AccountService.Domain.Entities;

namespace UndergroundBank.AccountService.Infrastructure
{
    public class AccountDbContext
        : IdentityDbContext<
            User,
            IdentityRole<Guid>,
            Guid,
            IdentityUserClaim<Guid>,
            IdentityUserRole<Guid>,
            IdentityUserLogin<Guid>,
            IdentityRoleClaim<Guid>,
            IdentityUserToken<Guid>
        >
    {
        public AccountDbContext(DbContextOptions<AccountDbContext> options)
            : base(options) { }
    }
}
