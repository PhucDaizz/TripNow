using Domain.Common.Response;
using MediatR;
using PaymentService.Application.DTOs.OwnerBankAccount;

namespace PaymentService.Application.Features.OwnerBankAccount.Queries.GetOwnerBankAccounts
{
    public class GetOwnerBankAccountsQuery: IRequest<Result<List<OwnerBankAccountDto?>>>
    {
        public Guid? OwnerId { get; set; }
    }
}
