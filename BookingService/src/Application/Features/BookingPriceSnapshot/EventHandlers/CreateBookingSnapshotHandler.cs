using BookingService.Application.Common.Interfaces;
using BookingService.Domain.Enum;
using BookingService.Domain.Events.Booking;
using MediatR;

namespace BookingService.Application.Features.BookingPriceSnapshot.EventHandlers
{
    public class CreateBookingSnapshotHandler : INotificationHandler<BookingCreatedDomainEvent>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateBookingSnapshotHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(BookingCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            var booking = notification.Booking;

            var details = booking.PriceDetails;

            decimal gross = details.FirstOrDefault(x => x.Type == PriceType.RoomCharge)?.Amount ?? 0;

            decimal promo = Math.Abs(details.FirstOrDefault(x => x.Type == PriceType.Promotion)?.Amount ?? 0);

            decimal vat = details.FirstOrDefault(x => x.Type == PriceType.VAT)?.Amount ?? 0;

            decimal serviceFee = details.FirstOrDefault(x => x.Type == PriceType.ServiceFee)?.Amount ?? 0;

            var snapshot = new Domain.Entities.BookingPriceSnapshot(
                booking.Id,
                booking.HotelId,
                gross,
                promo,
                vat,
                serviceFee
            );

            await _unitOfWork.BookingPriceSnapshot.AddAsync(snapshot, cancellationToken);
        }
    }
}
