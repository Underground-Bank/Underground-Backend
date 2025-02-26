using UndergroundBank.AccountService.Domain.Enums;

namespace UndergroundBank.AccountService.Application.Dto
{
    public class RegisterInfoDto
    {
        /// <summary>
        /// Parametr Name for register
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Parametr Surname for register
        /// </summary>
        public string Surname { get; set; }

        /// <summary>
        /// Parametr Gender for register
        /// </summary>
        public Gender Gender { get; set; }

        /// <summary>
        /// Parametr Email for register
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Parametr Password for register
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Parametr PhoneNumber for register
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Parametr BirthDate for register
        /// </summary>
        public DateTime BirthDate { get; set; }
    }
}
