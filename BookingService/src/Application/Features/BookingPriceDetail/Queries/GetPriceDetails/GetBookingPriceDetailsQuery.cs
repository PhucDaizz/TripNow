using BookingService.Application.DTOs.BookingPriceDetail;
using Domain.Common.Response;
using MediatR;

namespace BookingService.Application.Features.BookingPriceDetail.Queries.GetPriceDetails
{
    public record GetBookingPriceDetailsQuery(Guid BookingId) : IRequest<Result<List<BookingPriceDetailDto>>>;
}
