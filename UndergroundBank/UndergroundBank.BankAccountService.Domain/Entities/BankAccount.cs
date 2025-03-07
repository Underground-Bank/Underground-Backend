using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace UndergroundBank.BankAccountService.Domain.Entities
{
    public class BankAccount
    {
        [Key]
        public string AccountNumber { get; set; }
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedDate { get; set; }
        public Double Balance { get; set; }
        public Boolean IsLocked { get; set; }
    }
}
