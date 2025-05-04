using System.ComponentModel.DataAnnotations;

namespace UndergroundBank.AccountService.Domain.Entities
{
    public class UserFirebase
    {
        [Key]
        public Guid FirebaseUserId { get; set; }
        public Guid UserId { get; set; }
        public string FirebaseId { get; set; }
    }
}
