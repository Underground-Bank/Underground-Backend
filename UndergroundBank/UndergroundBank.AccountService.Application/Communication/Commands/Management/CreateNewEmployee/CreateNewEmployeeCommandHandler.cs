using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using UndergroundBank.AccountService.Application.Communication.Commands.Management.BlockUser;
using UndergroundBank.AccountService.Application.Interfaces;
using UndergroundBank.Common.Dto.AccountService;

namespace UndergroundBank.AccountService.Application.Communication.Commands.Management.CreateNewEmployee
{
    public class CreateNewEmployeeCommandHandler
        : IRequestHandler<CreateNewEmployeeCommand, InputManagerDataDto>
    {
        private readonly IManagementService _managementService;

        public CreateNewEmployeeCommandHandler(IManagementService managementService)
        {
            _managementService = managementService;
        }

        public async Task<InputManagerDataDto> Handle(
            CreateNewEmployeeCommand request,
            CancellationToken cancellationToken
        )
        {
            return await _managementService.CreateNewEmployee(request.managerDto);
        }
    }
}
