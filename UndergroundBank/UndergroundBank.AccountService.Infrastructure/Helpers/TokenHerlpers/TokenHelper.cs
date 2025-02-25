using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using UndergroundBank.AccountService.Application.Dto;
using UndergroundBank.AccountService.Domain.Entities;
using UndergroundBank.AccountService.Domain.Enums;

namespace UndergroundBank.AccountService.Infrastructure.Helpers.TokenHerlpers
{
    public class TokenHelper
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly AccountDbContext _accountDBContext;
        private readonly IConfiguration _configuration;

        public TokenHelper(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            AccountDbContext accountDbContext,
            IConfiguration configuration
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _accountDBContext = accountDbContext;
            _configuration = configuration;
        }

        public async Task AddRoleToUser(Role role, Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (!Enum.IsDefined(typeof(Role), role))
            {
                throw new BadHttpRequestException($"Role '{role}' is not defined.");
            }

            var roleName = Enum.GetName(typeof(Role), role);

            var result = await _userManager.AddToRoleAsync(user, roleName);
        }

        public string GenerateJwtToken(User user, IList<string> roles)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Email, user.Id.ToString()) };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var jwt = new JwtSecurityToken(
                issuer: _configuration.GetSection("Jwt")["Issuer"],
                audience: _configuration.GetSection("Jwt")["Audience"],
                notBefore: DateTime.UtcNow,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    _configuration.GetSection("Jwt").GetValue<int>("AccessTokenLifetimeInMinutes")
                ),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(
                            _configuration.GetSection("Jwt")["Secret"] ?? string.Empty
                        )
                    ),
                    SecurityAlgorithms.HmacSha256
                )
            );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        public string GenerateRefreshToken()
        {
            var randomValues = new byte[128];
            using (var numberGenerator = RandomNumberGenerator.Create())
            {
                numberGenerator.GetBytes(randomValues);
            }
            return Convert.ToBase64String(randomValues);
        }

        public async Task<AuthResponseDto> RefreshToken(
            RefreshTokenRequestDto refreshTokenRequestDTO
        )
        {
            var principal = GetUserIdFromToken(refreshTokenRequestDTO.AccessToken);

            if (principal == null)
            {
                Console.WriteLine("principal null");
            }
            var user = await _userManager.Users.FirstOrDefaultAsync(u =>
                u.Id.ToString() == principal
            );
            Console.WriteLine(principal);
            if (user == null)
            {
                Console.WriteLine("user null");
            }
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim> { new Claim(ClaimTypes.Email, user.Id.ToString()) };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var jwt = new JwtSecurityToken(
                issuer: _configuration.GetSection("Jwt")["Issuer"],
                audience: _configuration.GetSection("Jwt")["Audience"],
                notBefore: DateTime.UtcNow,
                claims: claims,
                expires: DateTime.UtcNow.Add(
                    TimeSpan.FromMinutes(
                        _configuration
                            .GetSection("Jwt")
                            .GetValue<int>("AccessTokenLifetimeInMinutes")
                    )
                ),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(
                            _configuration.GetSection("Jwt")["Secret"] ?? string.Empty
                        )
                    ),
                    SecurityAlgorithms.HmacSha256
                )
            );

            var tokenRefresh = GenerateRefreshToken();

            user.RefreshToken = tokenRefresh;
            var refreshTokenLifetimeInDays = _configuration
                .GetSection("Jwt")
                .GetValue<int>("RefreshTokenLifetimeInDays");
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(refreshTokenLifetimeInDays);
            user.RefreshTokenExpiry = refreshTokenExpiry;

            await _accountDBContext.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(jwt),
                RefreshToken = tokenRefresh,
            };
        }

        public string? GetUserIdFromToken(string token)
        {
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Secret").Value)
            );

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = securityKey,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
            };

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var claimsPrincipal = handler.ValidateToken(
                    token,
                    validationParameters,
                    out var validatedToken
                );

                if (
                    claimsPrincipal is null
                    || !(validatedToken is JwtSecurityToken jwtSecurityToken)
                )
                    return null;

                var emailClaim = claimsPrincipal.FindFirst(ClaimTypes.Email);

                return emailClaim?.Value;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
