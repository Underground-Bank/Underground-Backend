using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
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
    }
}
