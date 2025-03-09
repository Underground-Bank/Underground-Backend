using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using UndergroundBank.AccountService.Application.Dto;
using UndergroundBank.AccountService.Application.Helpers.Validations;
using UndergroundBank.AccountService.Application.Interfaces;
using UndergroundBank.AccountService.Domain.Entities;
using UndergroundBank.AccountService.Infrastructure.Helpers.TokenHerlpers;
using UndergroundBank.Common.Data;
using UndergroundBank.Common.Middlewares;

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
        private readonly RedisDbContext _redisDBContext;

        public AuthService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration,
            TokenHelper tokenHelper,
            IMapper mapper,
            IUserRepository userRepository,
            RedisDbContext redisDBContext
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _tokenHelper = tokenHelper;
            _mapper = mapper;
            _userRepository = userRepository;
            _redisDBContext = redisDBContext;
        }

        /// <inheritdoc/>
        public async Task<AuthResponseDto> Register(RegisterInfoDto registerCreds)
        {
            var validateUserData = UserValidations.ValidateUserData(
                registerCreds.Name,
                registerCreds.Surname,
                registerCreds.Password,
                registerCreds.Email,
                registerCreds.BirthDate,
                registerCreds.PhoneNumber,
                registerCreds.Gender
            );

            if (validateUserData != string.Empty)
            {
                throw new BadRequestException(validateUserData);
            }

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
                throw new NotFoundException(
                    "Ошибка регистрации! Проверьте данные или попробуйте позже!"
                );
            }
        }

        /// <inheritdoc/>
        public async Task<AuthResponseDto> Login(LoginInfoDto loginCreds)
        {
            var veryfiedUser = await CheckBasedUserInformation(
                loginCreds.Email,
                loginCreds.Password
            );
            if (veryfiedUser == null)
            {
                throw new BadRequestException("Неверный Email или пароль!");
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(u =>
                u.Email == loginCreds.Email
            );
            if (user == null)
            {
                throw new NotFoundException("Данного пользователя не существует!");
            }
            if (user.IsLocked == true)
            {
                throw new BadRequestException("Данный пользователь заблокирован!");
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

        public async Task<AuthResponseDto> RefreshToken(RefreshTokenRequestDto refreshTokenRequest)
        {
            var principal = _tokenHelper.GetUserIdFromToken(refreshTokenRequest.AccessToken);

            if (principal == null)
            {
                throw new ForbiddenException("Пользователь не авторизован!");
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(u =>
                u.Id.ToString() == principal
            );

            if (user == null)
            {
                throw new NotFoundException("Данного пользователя не существует!");
            }

            var roles = await _userManager.GetRolesAsync(user);

            var jwt = _tokenHelper.GenerateJwtToken(user, roles);

            var tokenRefresh = _tokenHelper.GenerateRefreshToken();

            user.RefreshToken = tokenRefresh;
            var refreshTokenLifetimeInDays = _configuration
                .GetSection("Jwt")
                .GetValue<int>("RefreshTokenLifetimeInDays");
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(refreshTokenLifetimeInDays);
            user.RefreshTokenExpiry = refreshTokenExpiry;

            await _userRepository.SaveChangeAsync();

            return new AuthResponseDto { AccessToken = jwt, RefreshToken = tokenRefresh };
        }

        /// <inheritdoc/>
        public async Task Logout(string token, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("Данного пользователя не существует!");
            }
            user.RefreshToken = null;
            await _userManager.UpdateAsync(user);
            await _redisDBContext.AddToken(token);
        }

        private async Task<ClaimsIdentity> CheckBasedUserInformation(string email, string password)
        {
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
