using Domain.Common.Response;
using MediatR;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Application.Contracts;

namespace PaymentService.Application.Features.SettlementPeriod.Commands.RetrySettlement
{
    public class RetrySettlementCommandHandler : IRequestHandler<RetrySettlementCommand, Result>
    {
        private readonly ISettlementService _settlementService;
        private readonly IUnitOfWork _unitOfWork; 

        public RetrySettlementCommandHandler(ISettlementService settlementService, IUnitOfWork unitOfWork)
        {
            _settlementService = settlementService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(RetrySettlementCommand request, CancellationToken token)
        {
            var userWallet = await _unitOfWork.OwnerWallets.GetByOwnerIdAsync(request.OwnerId, token);

            if (userWallet == null)
            {
                return Result.Failure(new Error("Settlement.UserWalletNotFound", $"No wallet found for owner with ID {request.OwnerId}."));
            }

            var isSuccess = await _settlementService.ProcessSettlementForOwnerAsync(request.OwnerId, token);

            if (!isSuccess)
            {
                return Result.Failure(new Error("Settlement.RetryFailed", "Processing failed or there is no data to reconcile."));
            }

            return Result.Success();
        }
    }
}
