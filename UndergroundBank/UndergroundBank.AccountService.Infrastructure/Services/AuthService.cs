using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using UndergroundBank.AccountService.Application.Dto;
using UndergroundBank.AccountService.Application.Interfaces;
using UndergroundBank.AccountService.Domain.Entities;
using UndergroundBank.AccountService.Infrastructure.Helpers.TokenHerlpers;

namespace UndergroundBank.AccountService.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IMapper _mapper;
        private readonly TokenHelper _tokenHelper;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration,
            TokenHelper tokenHelper,
            IMapper mapper,
            IUserRepository userRepository
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _tokenHelper = tokenHelper;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task<AuthResponseDto> Register(RegisterInfoDto registerCreds)
        {
            var user = _mapper.Map<User>(registerCreds);
            user.UserName = user.Email;
            var result = await _userManager.CreateAsync(user, registerCreds.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Client");
                return await Login(
                    new LoginInfoDto()
                    {
                        Email = registerCreds.Email,
                        Password = registerCreds.Password,
                    }
                );
            }
            else
            {
                throw new Exception("User registration failed");
            }
        }

        public async Task<AuthResponseDto> Login(LoginInfoDto loginCreds)
        {
            var veryfiedUser = await CheckBasedUserInformation(
                loginCreds.Email,
                loginCreds.Password
            );
            if (veryfiedUser == null)
            {
                Console.WriteLine("NotFound");
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(u =>
                u.Email == loginCreds.Email
            );
            if (user == null)
            {
                Console.WriteLine("NotFound");
            }

            var roles = await _userManager.GetRolesAsync(user);

            var jwt = _tokenHelper.GenerateJwtToken(user, roles);

            var tokenRefresh = _tokenHelper.GenerateRefreshToken();
            user.RefreshToken = tokenRefresh;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(
                _configuration.GetSection("Jwt").GetValue<int>("RefreshTokenLifetimeInDays")
            );

            await _userRepository.SaveChangeAsync();

            return new AuthResponseDto { AccessToken = jwt, RefreshToken = tokenRefresh };
        }

        private async Task<ClaimsIdentity> CheckBasedUserInformation(string email, string password)
        {
            Console.WriteLine(email);
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return null;
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            if (!result.Succeeded)
            {
                return null;
            }

            var claims = new List<Claim> { new Claim(ClaimTypes.Email, user.Id.ToString()) };

            return new ClaimsIdentity(claims, "Token", ClaimTypes.Email, ClaimTypes.Role);
        }
    }
}
