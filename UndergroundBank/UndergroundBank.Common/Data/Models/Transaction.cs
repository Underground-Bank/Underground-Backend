using UndergroundBank.Common.Data.Enums;

namespace UndergroundBank.Common.Data.Models
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public Status Status { get; set; }
    }
}
