using MediatR;
using UndergroundBank.LoanService.Application.Dto.Loan;

namespace UndergroundBank.LoanService.Application.Communication.Queries.LoanService.GetCreditRating;

public record GetCreditRatingQuery(Guid userId) : IRequest<CreditRatingDto>;


