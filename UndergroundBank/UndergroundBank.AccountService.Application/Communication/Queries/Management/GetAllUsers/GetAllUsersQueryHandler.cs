using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using UndergroundBank.AccountService.Application.Communication.Commands.Management.DeleteUser;
using UndergroundBank.AccountService.Application.Interfaces;
using UndergroundBank.Common.Dto.AccountService;

namespace UndergroundBank.AccountService.Application.Communication.Queries.Management.GetAllUsers
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, List<ProfileDto>>
    {
        private readonly IManagementService _managementService;

        public GetAllUsersQueryHandler(IManagementService managementService)
        {
            _managementService = managementService;
        }

        public async Task<List<ProfileDto>> Handle(
            GetAllUsersQuery request,
            CancellationToken cancellationToken
        )
        {
            return await _managementService.GetAllUsers(request.role);
        }
    }
}
