using Microsoft.AspNetCore.Identity;
using UndergroundBank.AccountService.Domain.Entities;
using UndergroundBank.AccountService.Infrastructure;

namespace UndergroundBank.AccountService.Web.Configurations
{
    public static class IdentityDependenciesConfiguration
    {
        public static IServiceCollection AddMicIdentityConfiguration(
            this IServiceCollection services
        )
        {
            services
                .AddIdentity<User, IdentityRole<Guid>>(
                    (
                        options =>
                        {
                            options.SignIn.RequireConfirmedEmail = false;
                            options.User.RequireUniqueEmail = true;
                        }
                    )
                )
                .AddEntityFrameworkStores<AccountDbContext>()
                .AddDefaultTokenProviders()
                .AddSignInManager<SignInManager<User>>()
                .AddUserManager<UserManager<User>>()
                .AddRoleManager<RoleManager<IdentityRole<Guid>>>();

            return services;
        }
    }
}
