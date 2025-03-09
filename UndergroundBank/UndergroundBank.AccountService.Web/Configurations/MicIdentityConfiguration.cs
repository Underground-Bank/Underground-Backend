using Microsoft.AspNetCore.Identity;
using UndergroundBank.AccountService.Domain.Entities;
using UndergroundBank.AccountService.Infrastructure;
using UndergroundBank.Common.Data.Enums;

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

        public static async Task ConfigureAdminAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<
                RoleManager<IdentityRole<Guid>>
            >();

            string adminEmail = "admin@undergroundbank.com";
            string adminPassword = "Admin@123";

            if (!await roleManager.RoleExistsAsync(Role.Admin.ToString()))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(Role.Admin.ToString()));
            }

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    Name = "Admin",
                    Surname = "User",
                    EmailConfirmed = true,
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, Role.Admin.ToString());
                }
            }
        }
    }
}
