using Domain.Common.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Application.Contracts;
using PaymentService.Application.DTOs.Payment.Event;
using PaymentService.Domain.Enum;
using PaymentService.Domain.Exceptions;

namespace PaymentService.Application.Features.Payment.EventHandlers
{
    public class BookingCompletedEventHandler : INotificationHandler<BookingCompleted>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHotelCatalogService _hotelCatalogService;
        private readonly IServiceFeeSettings _serviceFeeSettings;
        private readonly ILogger<BookingCompletedEventHandler> _logger;

        public BookingCompletedEventHandler(IUnitOfWork unitOfWork, IHotelCatalogService hotelCatalogService, IServiceFeeSettings serviceFeeSettings, ILogger<BookingCompletedEventHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _hotelCatalogService = hotelCatalogService;
            _serviceFeeSettings = serviceFeeSettings;
            _logger = logger;
        }

        public async Task Handle(BookingCompleted notification, CancellationToken cancellationToken)
        {
            var hotel = await _hotelCatalogService.GetHotelSummary(notification.HotelId);
            if (hotel == null) return;

            var escrow = await _unitOfWork.EscrowAccounts.GetByBookingIdAsync(notification.BookingId);
            if (escrow == null)
            {
                _logger.LogWarning("Escrow not found for BookingId: {BookingId}", notification.BookingId);
                return;
            }

            if (escrow.Status == EscrowStatus.Refunded || escrow.Status == EscrowStatus.Released)
            {
                return;
            }

            var ownerWallet = await _unitOfWork.OwnerWallets.GetByOwnerIdAsync(hotel.OwnerId, cancellationToken);

            if (ownerWallet == null)
            {
                throw new DomainException($"CRITICAL ERROR: OwnerWallet not found for OwnerId {hotel.OwnerId}.");
            }

            bool isAlreadyPaid = await _unitOfWork.OwnerWallets.HasTransactionAsync(
                hotel.OwnerId,
                notification.BookingId,
                LedgerReferenceType.Booking,
                cancellationToken);

            if (isAlreadyPaid)
            {
                _logger.LogInformation("Booking {BookingId} has already been credited previously. Ignore the event.", notification.BookingId);
                return;
            }

            decimal commissionRate = _serviceFeeSettings.Percentage;
            decimal commissionAmount = notification.Amount * commissionRate / 100;
            decimal netAmount = notification.Amount - commissionAmount;

            try
            {
                escrow.Release();

                ownerWallet.ReceiveRevenue(
                    amount: netAmount,
                    transactionGrossAmount: notification.Amount,
                    transactionFee: commissionAmount,
                    bookingId: notification.BookingId,
                    description: $"Revenue from booking {notification.BookingId}"
                );

                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning(ex, "Data collision when adding money for Owner {OwnerId}. Throw an error for Broker Retry.", hotel.OwnerId);
                throw;
            }
        }
    }
}
