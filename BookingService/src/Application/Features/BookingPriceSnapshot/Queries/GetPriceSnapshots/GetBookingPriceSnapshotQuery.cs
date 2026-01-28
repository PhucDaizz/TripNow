using BookingService.Application.DTOs.BookingPriceSnapshot;
using Domain.Common.Response;
using MediatR;

namespace BookingService.Application.Features.BookingPriceSnapshot.Queries.GetPriceSnapshots
{
    public record GetBookingPriceSnapshotQuery(Guid BookingId) : IRequest<Result<BookingPriceSnapshotDto>>;
}
