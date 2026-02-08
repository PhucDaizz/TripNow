using MediatR;
using Microsoft.Extensions.Logging;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Application.Contracts;
using PaymentService.Application.DTOs.Payment.Event;
using PaymentService.Domain.Entities;

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
            var escrow = await _unitOfWork.EscrowAccounts.GetByBookingIdAsync(notification.BookingId);
            if (escrow == null) return;

            escrow.Refund(notification.RefundAmount);

            var settlement = escrow.CalculateSettlement(_serviceFeeSettings.Percentage);

            if (settlement.NetAmount > 0)
            {
                var hotel = await _hotelCatalogService.GetHotelSummary(notification.HotelId);
                if (hotel != null)
                {
                    var ownerWallet = await _unitOfWork.OwnerWallets.GetByOwnerIdAsync(hotel.OwnerId);
                    if (ownerWallet == null)
                    {
                        ownerWallet = new Domain.Entities.OwnerWallet(hotel.OwnerId);
                        await _unitOfWork.OwnerWallets.AddAsync(ownerWallet);
                    }

                    ownerWallet.ReceiveRevenue(
                        bookingId: notification.BookingId,
                        amount: settlement.NetAmount,
                        transactionGrossAmount: settlement.GrossAmount,
                        transactionFee: settlement.Fee,
                        "Booking cancellation refund"
                    );
                }
            }
            escrow.Release();

            var originalTxn = await _unitOfWork.PaymentTransactions
                .GetSuccessTransactionByBookingIdAsync(notification.BookingId);

            if (originalTxn == null) return;

            var refundRequest = new Domain.Entities.RefundRequest(
                notification.BookingId,
                notification.UserId,
                notification.RefundAmount,
                originalTxn.Id,
                originalTxn.ProviderTxnId,
                "Booking cancellation refund"
            );

            await _unitOfWork.RefundRequests.AddAsync(refundRequest);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
