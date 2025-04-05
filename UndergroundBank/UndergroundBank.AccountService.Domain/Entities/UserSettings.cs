using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UndergroundBank.Common.Data.Enums;

namespace UndergroundBank.AccountService.Domain.Entities
{
    public class UserSettings
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Theme Theme { get; set; }
    }
}
