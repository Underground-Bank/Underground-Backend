using UndergroundBank.AccountService.Domain.Enums;

namespace UndergroundBank.Common.Dto.AccountService
{
    public class ProfileDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public Gender Gender { get; set; }
        public List<Role> Roles { get; set; }
    }
}
