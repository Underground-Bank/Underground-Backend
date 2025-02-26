using UndergroundBank.AccountService.Domain.Enums;

namespace UndergroundBank.AccountService.Application.Dto
{
    public class EditProfileInfoDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public Gender Gender { get; set; }
        public DateTime Birthdate { get; set; }
    }
}
