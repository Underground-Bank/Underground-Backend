using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UndergroundBank.Common.Data.Enums;

namespace UndergroundBank.AccountService.Application.Dto
{
    public class UserSettingsDto
    {
        public Guid UserId { get; set; }
        public Theme Theme { get; set; }
    }
}
