using Domain.Common.Response;
using MediatR;

namespace PaymentService.Application.Features.OwnerBankAccount.Commands.CreateBankAccount
{
    public class CreateBankAccountCommand : IRequest<Result<Guid>>
    {
        public string BankName { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankAccountHolder { get; set; }
        public bool IsDefault { get; set; }
    }
}
