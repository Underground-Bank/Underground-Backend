using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UndergroundBank.Common.Dto.AccountService
{
    public class UserFirebaseDto
    {
        public Guid FirebaseUserId { get; set; }
        public Guid UserId { get; set; }
        public string FirebaseId { get; set; }
    }
}
