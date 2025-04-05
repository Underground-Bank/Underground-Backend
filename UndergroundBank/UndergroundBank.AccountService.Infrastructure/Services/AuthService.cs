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
using UndergroundBank.Common.Data;
using UndergroundBank.Common.Middlewares;

namespace UndergroundBank.AccountService.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly RedisDbContext _redisDBContext;

        public AuthService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration,
            IMapper mapper,
            IUserRepository userRepository,
            RedisDbContext redisDBContext
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _mapper = mapper;
            _userRepository = userRepository;
            _redisDBContext = redisDBContext;
        }

        /// <inheritdoc/>
        public async Task Register(RegisterInfoDto registerCreds)
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
            }
            else
            {
                throw new NotFoundException(
                    "Ошибка регистрации! Проверьте данные или попробуйте позже!"
                );
            }
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
    }
}
