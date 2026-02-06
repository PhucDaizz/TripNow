using FluentValidation;

namespace PaymentService.Application.Features.Payout.Commands.RejectPayout
{
    public class RejectPayoutCommandValidator : AbstractValidator<RejectPayoutCommand>
    {
        public RejectPayoutCommandValidator()
        {
            RuleFor(x => x.PayoutId)
                .NotEmpty().WithMessage("Payout ID is required.");
            RuleFor(x => x.RejectionReason)
                .NotEmpty().WithMessage("Rejection reason is required.")
                .MaximumLength(500).WithMessage("Rejection reason cannot exceed 500 characters.");
        }
    }
}
