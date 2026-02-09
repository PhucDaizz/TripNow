using Domain.Common.Response;
using MediatR;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Application.DTOs.Wallet;

namespace PaymentService.Application.Features.OwnerWallet.Queries.GetMyWallet
{
    public class GetMyWalletQueryHandler : IRequestHandler<GetMyWalletQuery, Result<OwnerWalletSummaryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetMyWalletQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<OwnerWalletSummaryDto>> Handle(GetMyWalletQuery request, CancellationToken token)
        {
            var ownerId = Guid.Parse(_currentUserService.UserId);

            var wallet = await _unitOfWork.OwnerWallets.GetByOwnerIdAsync(ownerId);

            if (wallet == null)
            {
                return Result.Success(new OwnerWalletSummaryDto
                {
                    AvailableBalance = 0,
                    PendingBalance = 0
                });
            }

            return Result.Success(new OwnerWalletSummaryDto
            {
                Id = wallet.Id,
                AvailableBalance = wallet.AvailableBalance,
                PendingBalance = wallet.PendingBalance
            });
        }
    }
}
