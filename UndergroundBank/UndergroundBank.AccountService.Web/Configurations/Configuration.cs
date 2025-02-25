using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace UndergroundBank.AccountService.Web.Configurations
{
    public static class Configuration
    {
        public static void ConfigurePresentationLayer(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.ConfigureSwaggerAuth();
            services.ConfigureJWTAuth(configuration);
        }

        public static void ConfigureSwaggerAuth(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        Description = "JWT Authorization. Enter 'Bearer [space] your token'",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Scheme = "Bearer",
                        Type = SecuritySchemeType.Http,
                        BearerFormat = "JWT",
                    }
                );
                options.AddSecurityRequirement(
                    new OpenApiSecurityRequirement()
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer",
                                },
                            },
                            new List<string>()
                        },
                    }
                );
            });
        }

        private static void ConfigureJWTAuth(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            var key = configuration.GetValue<string>("ApiSettings:SecretKey");

            var issuer = configuration.GetValue<string>("ApiSettings:Issuer");

            var audience = configuration.GetValue<string>("ApiSettings:Audience");

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidIssuer = issuer,
                        ValidateAudience = false,
                        ValidAudience = audience,
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.ASCII.GetBytes(key ?? string.Empty)
                        ),
                        ValidateIssuerSigningKey = true,
                    };
                });
        }
    }
}
