using Domain.Common.Response;
using MediatR;

namespace PaymentService.Application.Features.OwnerBankAccount.Commands.DeleteBankAccount
{
    public class DeleteBankAccountCommand: IRequest<Result>
    {
        public Guid Id { get; set; }
    }
}
