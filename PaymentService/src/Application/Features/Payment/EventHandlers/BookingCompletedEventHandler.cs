using Domain.Common.Response;
using MediatR;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Application.Contracts;
using PaymentService.Application.DTOs.Payment.Event;

namespace PaymentService.Application.Features.Payment.EventHandlers
{
    public class BookingCompletedEventHandler : INotificationHandler<BookingCompleted>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHotelCatalogService _hotelCatalogService;
        private readonly IServiceFeeSettings _serviceFeeSettings;

        public BookingCompletedEventHandler(IUnitOfWork unitOfWork, IHotelCatalogService hotelCatalogService, IServiceFeeSettings serviceFeeSettings)
        {
            _unitOfWork = unitOfWork;
            _hotelCatalogService = hotelCatalogService;
            _serviceFeeSettings = serviceFeeSettings;
        }

        public async Task Handle(BookingCompleted notification, CancellationToken cancellationToken)
        {
            var hotel = await _hotelCatalogService.GetHotelSummary(notification.HotelId);

            if (hotel != null)
            {
                var escrow = await _unitOfWork.EscrowAccounts.GetByBookingIdAsync(notification.BookingId);

                if (escrow != null) 
                    escrow.Release();

                decimal commissionRate = _serviceFeeSettings.Percentage;
                decimal commissionAmount = notification.Amount * commissionRate / 100;
                decimal netAmount = notification.Amount - commissionAmount;
                
                var ownerWallet = await _unitOfWork.OwnerWallets.GetByOwerIdAsync(hotel.OwnerId);

                if (ownerWallet == null)
                {
                    ownerWallet = new Domain.Entities.OwnerWallet(hotel.OwnerId);
                    await _unitOfWork.OwnerWallets.AddAsync(ownerWallet);
                }

                ownerWallet.ReceiveRevenue(
                    amount: netAmount,
                    bookingId: notification.BookingId,
                    description: $"Revenue from booking {notification.BookingId}"
                );

                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
