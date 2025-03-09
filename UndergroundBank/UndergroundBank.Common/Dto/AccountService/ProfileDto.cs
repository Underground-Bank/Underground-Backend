using UndergroundBank.Common.Data.Enums;

namespace UndergroundBank.Common.Dto.AccountService
{
    public class ProfileDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public Gender Gender { get; set; }
        public bool IsLocked { get; set; }
        public List<Role> Roles { get; set; }
    }
}
