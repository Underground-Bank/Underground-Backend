using Microsoft.AspNetCore.Identity;
using UndergroundBank.Common.Data.Enums;

namespace UndergroundBank.AccountService.Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        /// <summary>
        /// Name of corresponding user
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Surname of corresponding user
        /// </summary>
        public string Surname { get; set; }

        /// <summary>
        /// Phone of corresponding user
        /// </summary>
        public override string? PhoneNumber { get; set; }

        /// <summary>
        /// Gender of corresponding user
        /// </summary>
        public Gender Gender { get; set; }

        /// <summary>
        /// Birthdate of corresponding user
        /// </summary>
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// Refresh token of corresponding user
        /// </summary>
        public string? RefreshToken { get; set; }

        /// <summary>
        /// Time for refresh token expiry
        /// </summary>
        public DateTime RefreshTokenExpiry { get; set; }

        /// <summary>
        /// Block user param
        /// </summary>
        public bool IsLocked { get; set; }
    }
}
