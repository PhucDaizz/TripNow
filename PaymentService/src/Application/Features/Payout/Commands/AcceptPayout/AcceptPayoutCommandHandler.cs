using Domain.Common.Response;
using MediatR;
using PaymentService.Application.Common.Interfaces;

namespace PaymentService.Application.Features.Payout.Commands.AcceptPayout
{
    public class AcceptPayoutCommandHandler : IRequestHandler<AcceptPayoutCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AcceptPayoutCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(AcceptPayoutCommand request, CancellationToken cancellationToken)
        {
            var payout = await _unitOfWork.Payouts.GetByIdAsync(request.PayoutId);
            if (payout is null)
            {
                return Result.Failure(new Error("NOTFOUND.PAYOUT","Payout not found."));
            }

            payout.MarkAsCompleted(request.TransactionReference);

            await _unitOfWork.Payouts.UpdateAsync(payout);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
