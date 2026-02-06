using FluentValidation;

namespace PaymentService.Application.Features.Payout.Commands.AcceptPayout
{
    public class AcceptPayoutCommandValidator : AbstractValidator<AcceptPayoutCommand>
    {
        public AcceptPayoutCommandValidator()
        {
            RuleFor(x => x.PayoutId)
                .NotEmpty().WithMessage("Payout ID is required.");

            RuleFor(x => x.TransactionReference)
                .NotEmpty().WithMessage("Transaction reference is required.");
        }
    }
}
