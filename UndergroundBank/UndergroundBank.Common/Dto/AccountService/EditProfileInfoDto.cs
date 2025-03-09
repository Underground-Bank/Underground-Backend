using UndergroundBank.Common.Data.Enums;

namespace UndergroundBank.Common.Dto.AccountService
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
