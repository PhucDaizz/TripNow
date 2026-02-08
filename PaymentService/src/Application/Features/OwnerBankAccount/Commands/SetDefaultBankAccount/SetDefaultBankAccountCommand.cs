using Domain.Common.Response;
using MediatR;

namespace PaymentService.Application.Features.OwnerBankAccount.Commands.SetDefaultBankAccount
{
    public class SetDefaultBankAccountCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
    }
}
