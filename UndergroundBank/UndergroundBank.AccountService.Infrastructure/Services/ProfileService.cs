using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using UndergroundBank.AccountService.Application.Dto;
using UndergroundBank.AccountService.Application.Helpers.Validations;
using UndergroundBank.AccountService.Application.Interfaces;
using UndergroundBank.AccountService.Domain.Entities;
using UndergroundBank.AccountService.Domain.Enums;
using UndergroundBank.Common.Dto.AccountService;
using UndergroundBank.Common.Middlewares;

namespace UndergroundBank.AccountService.Infrastructure.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public ProfileService(
            IMapper mapper,
            IUserRepository userRepository,
            UserManager<User> userManager,
            IConfiguration configuration
        )
        {
            _configuration = configuration;
            _mapper = mapper;
            _userRepository = userRepository;
            _userManager = userManager;
        }

        public async Task<ProfileDto> GetUserProfile(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var userRoles = await _userManager.GetRolesAsync(user);

            if (user == null)
            {
                throw new NotFoundException("Данного пользователя не существует!");
            }

            var userProfile = _mapper.Map<ProfileDto>(user);
            userProfile.Roles = userRoles.Select(r => Enum.Parse<Role>(r)).ToList();

            return userProfile;
        }

        public async Task EditProfile(EditProfileInfoDto editCreds, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new NotFoundException("Такого пользователя не существует");
            }
            var validateUserData = UserValidations.ValidateUserData(
                editCreds.Name,
                editCreds.Surname,
                null,
                editCreds.Email,
                editCreds.Birthdate,
                editCreds.Phone,
                editCreds.Gender
            );

            if (validateUserData != string.Empty)
            {
                throw new BadRequestException(validateUserData);
            }

            user.Name = string.IsNullOrWhiteSpace(editCreds.Name) ? user.Name : editCreds.Name;
            user.Surname = string.IsNullOrWhiteSpace(editCreds.Surname)
                ? user.Surname
                : editCreds.Surname;
            user.PhoneNumber = string.IsNullOrWhiteSpace(editCreds.Phone)
                ? user.PhoneNumber
                : editCreds.Phone;
            user.Email = string.IsNullOrWhiteSpace(editCreds.Email) ? user.Email : editCreds.Email;
            user.BirthDate = editCreds.Birthdate;
            user.Gender = editCreds.Gender;

            await _userManager.UpdateAsync(user);
        }

        public async Task ChangePassword(ChangePasswordDto changePasswordCreds, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new NotFoundException("Пользователь не найден!");
            }
            if (changePasswordCreds.Password != changePasswordCreds.ConfirmPassword)
            {
                throw new BadRequestException("Пароли должны совпадать!");
            }
            var result = await _userManager.ChangePasswordAsync(
                user,
                changePasswordCreds.OldPassword,
                changePasswordCreds.Password
            );
            if (!result.Succeeded)
            {
                throw new BadRequestException(
                    string.Join(", ", result.Errors.Select(x => x.Description))
                );
            }
        }
    }
}
