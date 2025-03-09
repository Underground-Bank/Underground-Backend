using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using UndergroundBank.Common.Dto.AccountService;

namespace UndergroundBank.AccountService.Application.Communication.Commands.Management.CreateNewEmployee
{
    public record CreateNewEmployeeCommand(ManagerDto managerDto) : IRequest<InputManagerDataDto>;
}
