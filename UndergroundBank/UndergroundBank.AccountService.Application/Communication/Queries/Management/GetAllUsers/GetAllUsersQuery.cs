using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using UndergroundBank.Common.Data.Enums;
using UndergroundBank.Common.Dto.AccountService;

namespace UndergroundBank.AccountService.Application.Communication.Queries.Management.GetAllUsers
{
    public record GetAllUsersQuery(Role? role) : IRequest<List<ProfileDto>>;
}
