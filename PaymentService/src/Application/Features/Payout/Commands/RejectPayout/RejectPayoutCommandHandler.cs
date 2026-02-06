using Domain.Common.Response;
using MediatR;
using PaymentService.Application.Common.Interfaces;

namespace PaymentService.Application.Features.Payout.Commands.RejectPayout
{
    public class RejectPayoutCommandHandler : IRequestHandler<RejectPayoutCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RejectPayoutCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(RejectPayoutCommand request, CancellationToken cancellationToken)
        {
            var payout = await _unitOfWork.Payouts.GetByIdAsync(request.PayoutId, cancellationToken);

            if (payout is null)
            {
                return Result.Failure(new Error("Payout.NotFound", "The specified payout was not found."));
            }

            payout.MarkAsFailed(request.RejectionReason);

            var wallet = await _unitOfWork.OwnerWallets.GetByIdAsync(payout.OwnerWalletId);

            if (wallet == null)
                return Result.Failure(new Error("Wallet.NotFound", "Wallet not found for refund."));

            var settlement = await _unitOfWork.SettlementPeriods.GetByIdAsync(payout.SettlementId.Value);

            wallet.RefundFailedPayout(
                payout.Id,
                payout.Amount,  
                payout.Amount,  
                0
            );

            await _unitOfWork.Payouts.UpdateAsync(payout);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
