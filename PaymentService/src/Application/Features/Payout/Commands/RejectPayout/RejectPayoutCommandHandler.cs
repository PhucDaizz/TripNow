using Domain.Common.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
            int maxRetries = 3;

            for (int retryCount = 0; retryCount < maxRetries; retryCount++)
            {
                try
                {
                    var payout = await _unitOfWork.Payouts.GetByIdAsync(request.PayoutId, cancellationToken);
                    if (payout is null)
                    {
                        return Result.Failure(new Error("Payout.NotFound", "Withdrawal order not found."));
                    }

                    if (payout.Status != Domain.Enum.PayoutStatus.Pending && payout.Status != Domain.Enum.PayoutStatus.Processing)
                    {
                        return Result.Failure(new Error("Payout.InvalidState", "You can only reject withdrawal requests that are in Pending or Processing status."));
                    }

                    var wallet = await _unitOfWork.OwnerWallets.GetByIdAsync(payout.OwnerWalletId, cancellationToken);
                    if (wallet == null)
                    {
                        return Result.Failure(new Error("Wallet.NotFound", "Could not find the hotel owner's wallet."));
                    }


                    payout.MarkAsFailed(request.RejectionReason);

                    wallet.RefundFailedPayout(
                        payout.Id,
                        payout.Amount,
                        payout.Amount,
                        0
                    );

                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    return Result.Success();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (retryCount == maxRetries - 1)
                    {
                        return Result.Failure(new Error("System.Busy", "The wallet is processing another transaction, please try again later."));
                    }

                    foreach (var entry in ex.Entries) 
                    {
                        await entry.ReloadAsync();
                    }

                    await Task.Delay(Random.Shared.Next(50, 150), cancellationToken);
                }
            }

            return Result.Failure(new Error("System.UnknownError", "Unknown error while processing refund."));
        }
    }
}
