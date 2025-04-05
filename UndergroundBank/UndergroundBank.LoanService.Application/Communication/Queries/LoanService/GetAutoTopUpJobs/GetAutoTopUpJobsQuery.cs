using MediatR;
using UndergroundBank.LoanService.Application.Dto.Loan;

namespace UndergroundBank.LoanService.Application.Communication.Queries.LoanService.GetAutoTopUpJobs;

public record GetAutoTopUpJobsQuery(Guid userId) : IRequest<GetAutoTopUpLoanJobsListDto>;

