using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using UndergroundBank.AccountService.Application.Helpers.Validations;
using UndergroundBank.AccountService.Application.Interfaces;
using UndergroundBank.AccountService.Domain.Entities;
using UndergroundBank.AccountService.Infrastructure.Helpers.TokenHerlpers;
using UndergroundBank.Common.Data;
using UndergroundBank.Common.Data.Enums;
using UndergroundBank.Common.Dto.AccountService;
using UndergroundBank.Common.Middlewares;

namespace UndergroundBank.AccountService.Infrastructure.Services
{
    public class ManagementService : IManagementService
    {
        private readonly IMapper _mapper;
        private readonly TokenHelper _tokenHelper;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly RedisDbContext _redisDBContext;

        public ManagementService(
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

        public async Task BlockUser(Guid userId, Guid currentUserId)
        {
            if (userId == currentUserId)
            {
                throw new BadRequestException("Вы не можете заблокировать себя!");
            }
            var user = await _userManager.Users.FirstOrDefaultAsync(u =>
                u.Id.ToString() == userId.ToString()
            );
            if (user == null)
            {
                throw new NotFoundException("Данного пользователя не существует!");
            }

            user.IsLocked = true;
            user.RefreshToken = null;
            await _userManager.UpdateAsync(user);
            await _userManager.UpdateSecurityStampAsync(user);
            await _userRepository.SaveChangeAsync();
        }

        public async Task UnblockUser(Guid userId, Guid currentUserId)
        {
            if (userId == currentUserId)
            {
                throw new BadRequestException("Вы не можете разблокировать себя!");
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(u =>
                u.Id.ToString() == userId.ToString()
            );
            if (user == null)
            {
                throw new NotFoundException("Данного пользователя не существует!");
            }

            user.IsLocked = false;
            await _userManager.UpdateAsync(user);
            await _userRepository.SaveChangeAsync();
        }

        public async Task<InputManagerDataDto> CreateNewEmployee(ManagerDto managerCreds)
        {
            var validateUserData = UserValidations.ValidateUserData(
                managerCreds.Name,
                managerCreds.Surname,
                managerCreds.Password,
                managerCreds.Email,
                managerCreds.BirthDate,
                managerCreds.PhoneNumber,
                managerCreds.Gender
            );

            if (validateUserData != string.Empty)
            {
                throw new BadRequestException(validateUserData);
            }

            var user = _mapper.Map<User>(managerCreds);
            user.UserName = user.Email;
            var result = await _userManager.CreateAsync(user, managerCreds.Password);

            if (!result.Succeeded)
            {
                throw new Exception("Ошибка при создании пользователя.");
            }

            var addRoleResult = await _userManager.AddToRoleAsync(user, nameof(Role.Employee));

            if (!addRoleResult.Succeeded)
            {
                throw new Exception("Ошибка при назначении роли.");
            }

            await _userRepository.SaveChangeAsync();

            return _mapper.Map<InputManagerDataDto>(managerCreds);
        }

        public async Task DeleteUser(Guid userId, Guid currentUserId)
        {
            if (userId == currentUserId)
            {
                throw new BadRequestException("Вы не можете удалить себя!");
            }
            var user = await _userManager.Users.FirstOrDefaultAsync(u =>
                u.Id.ToString() == userId.ToString()
            );
            if (user == null)
            {
                throw new NotFoundException("Данного пользователя не существует!");
            }
            await _userRepository.DeleteAsync(user);
            await _userManager.UpdateSecurityStampAsync(user);
            await _userRepository.SaveChangeAsync();
        }

        public async Task<List<ProfileDto>> GetAllUsers(Role? role)
        {
            List<User> users =
                role != null
                    ? (await _userManager.GetUsersInRoleAsync(role.ToString())).ToList()
                    : await _userRepository.GetAllUsers();

            var profileDtos = _mapper.Map<List<ProfileDto>>(users);

            var result = new List<ProfileDto>();

            foreach (var profileDto in profileDtos)
            {
                List<Role> roles;

                if (role != null)
                {
                    roles = new List<Role> { (Role)Enum.Parse(typeof(Role), role.ToString()) };
                }
                else
                {
                    var userRoles = await _userManager.GetRolesAsync(
                        users.First(u => u.Id == profileDto.Id)
                    );
                    roles = userRoles.Select(roleString => Enum.Parse<Role>(roleString)).ToList();
                }

                profileDto.Roles = roles;
                result.Add(profileDto);
            }

            return result;
        }
    }
}
