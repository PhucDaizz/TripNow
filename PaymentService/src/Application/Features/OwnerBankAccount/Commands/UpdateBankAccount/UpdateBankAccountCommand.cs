using Domain.Common.Response;
using MediatR;

namespace PaymentService.Application.Features.OwnerBankAccount.Commands.UpdateBankAccount
{
    public class UpdateBankAccountCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
        public string BankName { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankAccountHolder { get; set; }
    }
}
