using MediatR;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Application.DTOs.OwnerWallet.Event;

namespace PaymentService.Application.Features.OwnerWallet.EventHandlers
{
    public class RegisterHotelOwerEventHandler : INotificationHandler<RegisterHotelOwer>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RegisterHotelOwerEventHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(RegisterHotelOwer notification, CancellationToken cancellationToken)
        {
            Guid.TryParse(notification.UserId, out Guid userId);

            var isOwnerWalletExist = await _unitOfWork.OwnerWallets.GetByOwnerIdAsync(userId, cancellationToken);

            if (isOwnerWalletExist == null)
            {
                var newOwnerWallet = new Domain.Entities.OwnerWallet(userId);

                await _unitOfWork.OwnerWallets.AddAsync(newOwnerWallet, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
