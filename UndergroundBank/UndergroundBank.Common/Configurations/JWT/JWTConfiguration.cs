using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using OpenIddict.Server.AspNetCore;
using UndergroundBank.Common.Configurations.JWT;

namespace UndergroundBank.Common.Configurations.OpenIddict
{
    public static class OpenIddictJwtConfiguration
    {
        public static IServiceCollection AddOpenIddictValidation(
            this IServiceCollection services,
            string authority
        )
        {
            services.AddAuthorization();

            return services;
        }

        public static IServiceCollection AddSwaggerWithOAuth(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

                c.AddSecurityDefinition(
                    "oauth2",
                    new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.OAuth2,
                        Flows = new OpenApiOAuthFlows
                        {
                            AuthorizationCode = new OpenApiOAuthFlow
                            {
                                AuthorizationUrl = new Uri(
                                    "https://localhost:5026/connect/authorize"
                                ),
                                TokenUrl = new Uri("https://localhost:5026/connect/token"),
                                Scopes = new Dictionary<string, string>
                                {
                                    { "openid", "OpenID" },
                                    { "profile", "User profile" },
                                    { "email", "User email" },
                                    { "api", "Access to API" },
                                },
                            },
                        },
                    }
                );

                c.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "oauth2",
                                },
                            },
                            new[] { "openid", "profile", "email", "api" }
                        },
                    }
                );
            });

            return services;
        }
    }
}
