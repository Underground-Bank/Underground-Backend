using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UndergroundBank.AccountService.Application.Interfaces;
using UndergroundBank.AccountService.Infrastructure;
using UndergroundBank.AccountService.Infrastructure.Repositories;
using UndergroundBank.AccountService.Infrastructure.Services;
using UndergroundBank.Common.Data;
using UndergroundBank.Common.Helpers;

namespace UndergroundBank.AccountService.Web.Configurations
{
    public static class ServiceDependencyExtension
    {
        public static IServiceCollection AddAccountBlServiceDependencies(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.AddDbContext<AccountDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("AuthDatabasePostgres"));
                options.UseOpenIddict();
            });
            services.AddSingleton<RedisDbContext>(provider =>
            {
                var connectionString = configuration.GetConnectionString("RedisDBContext");
                return new RedisDbContext(connectionString);
            });

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IManagementService, ManagementService>();
            services.AddScoped<IUserSettingsService, UserSettingsService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<AdditionalTokenHelper>();
            services
                .AddOpenIddict()
                .AddCore(options =>
                {
                    options.UseEntityFrameworkCore().UseDbContext<AccountDbContext>();
                })
                .AddServer(options =>
                {
                    options.DisableAccessTokenEncryption();
                    options
                        .SetAuthorizationEndpointUris("/connect/authorize")
                        .SetTokenEndpointUris("/connect/token");

                    options.AllowAuthorizationCodeFlow().RequireProofKeyForCodeExchange();

                    options.AcceptAnonymousClients();

                    // Убедитесь, что клиент может отправить client_secret
                    options.RegisterScopes("openid", "profile", "email", "roles", "api");
                    options.AllowClientCredentialsFlow();

                    options
                        .UseAspNetCore()
                        .EnableAuthorizationEndpointPassthrough()
                        .EnableTokenEndpointPassthrough()
                        .EnableStatusCodePagesIntegration();

                    options
                        .AddDevelopmentEncryptionCertificate()
                        .AddDevelopmentSigningCertificate();
                })
                .AddValidation(options =>
                {
                    options.SetIssuer("https://localhost:5026/");
                    options.UseSystemNetHttp();
                    options.UseAspNetCore();
                });

            return services;
        }
    }
}
