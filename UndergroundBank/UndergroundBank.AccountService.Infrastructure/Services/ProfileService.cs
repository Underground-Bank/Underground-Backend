using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using UndergroundBank.AccountService.Application.Helpers.Validations;
using UndergroundBank.AccountService.Application.Interfaces;
using UndergroundBank.AccountService.Domain.Entities;
using UndergroundBank.Common.Data.Enums;
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
        private readonly AccountDbContext _context;

        public ProfileService(
            IMapper mapper,
            IUserRepository userRepository,
            UserManager<User> userManager,
            IConfiguration configuration,
            AccountDbContext context
        )
        {
            _configuration = configuration;
            _mapper = mapper;
            _userRepository = userRepository;
            _userManager = userManager;
            _context = context;
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

        public async Task AddFirebaseToken(string userId, string firebaseToken)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("Пользователь не найден!");
            }

            var userGuid = Guid.Parse(userId);

            var existingToken = await _context.UsersFirebase.FirstOrDefaultAsync(u =>
                u.UserId == userGuid && u.FirebaseId == firebaseToken
            );

            if (existingToken != null)
            {
                return;
            }

            var tokenInOtherUser = await _context.UsersFirebase.FirstOrDefaultAsync(u =>
                u.FirebaseId == firebaseToken && u.UserId != userGuid
            );

            if (tokenInOtherUser != null)
            {
                _context.UsersFirebase.Remove(tokenInOtherUser);
            }

            var newToken = new UserFirebase
            {
                FirebaseUserId = Guid.NewGuid(),
                UserId = userGuid,
                FirebaseId = firebaseToken,
            };

            _context.UsersFirebase.Add(newToken);
            await _context.SaveChangesAsync();
        }
    }
}
