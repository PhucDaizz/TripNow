using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Application.Contracts;
using PaymentService.Application.DTOs.Payment.Event;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Enum;
using PaymentService.Domain.Exceptions;

namespace PaymentService.Application.Features.Payment.EventHandlers
{
    public class BookingRefundEventHandler : INotificationHandler<BookingRefund>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceFeeSettings _serviceFeeSettings;
        private readonly IHotelCatalogService _hotelCatalogService;
        private readonly ILogger<BookingRefundEventHandler> _logger;

        public BookingRefundEventHandler(IUnitOfWork unitOfWork, IServiceFeeSettings serviceFeeSettings, IHotelCatalogService hotelCatalogService, ILogger<BookingRefundEventHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _serviceFeeSettings = serviceFeeSettings;
            _hotelCatalogService = hotelCatalogService;
            _logger = logger;
        }

        public async Task Handle(BookingRefund notification, CancellationToken cancellationToken)
        {
            bool isAlreadyRefunded = await _unitOfWork.RefundRequests.ExistsByBookingIdAsync(notification.BookingId, cancellationToken);
            if (isAlreadyRefunded)
            {
                _logger.LogInformation("Booking {BookingId} has already had a refund order created previously. Ignore the event.", notification.BookingId);
                return;
            }

            var escrow = await _unitOfWork.EscrowAccounts.GetByBookingIdAsync(notification.BookingId);
            if (escrow == null) throw new DomainException($"ERROR: Escrow not found for Booking {notification.BookingId}");

            var originalTxn = await _unitOfWork.PaymentTransactions.GetSuccessTransactionByBookingIdAsync(notification.BookingId);
            if (originalTxn == null) throw new DomainException($"ERROR: Original Transaction not found to refund the Booking {notification.BookingId}");

            if (escrow.Status == EscrowStatus.Refunded || escrow.Status == EscrowStatus.Released)
            {
                return;
            }

            try
            {
                escrow.Refund(notification.RefundAmount);

                var settlement = escrow.CalculateSettlement(_serviceFeeSettings.Percentage);

                if (settlement.NetAmount > 0)
                {
                    var hotel = await _hotelCatalogService.GetHotelSummary(notification.HotelId);
                    if (hotel == null) throw new DomainException($"Hotel not found {notification.HotelId}");

                    var ownerWallet = await _unitOfWork.OwnerWallets.GetByOwnerIdAsync(hotel.OwnerId, cancellationToken);
                    if (ownerWallet == null) throw new DomainException($"SERIOUS ERROR: Wallet not found for Owner {hotel.OwnerId}");

                    ownerWallet.ReceiveRevenue(
                        bookingId: notification.BookingId,
                        amount: settlement.NetAmount,
                        transactionGrossAmount: settlement.GrossAmount,
                        transactionFee: settlement.Fee,
                        description: "Receive revenue from guest room cancellation penalty fees"
                    );

                    escrow.Release();
                }

                var refundRequest = new Domain.Entities.RefundRequest(
                    notification.BookingId,
                    notification.UserId,
                    notification.RefundAmount,
                    originalTxn.Id,
                    originalTxn.ProviderTxnId,
                    "Refund due to customer canceling the booking"
                );

                await _unitOfWork.RefundRequests.AddAsync(refundRequest);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning(ex, "Data conflict when processing Refund for Booking {BookingId}. Throw an error so the Broker retries with a new DbContext.", notification.BookingId);
                throw;
            }
        }
    }
}
