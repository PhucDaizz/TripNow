using Domain.Common.Response;
using MediatR;
using PaymentService.Application.DTOs.EscrowAccount;

namespace PaymentService.Application.Features.EscrowAccount.Queries.GetEscrowByBookingId
{
    public record GetEscrowByBookingIdQuery(Guid BookingId) : IRequest<Result<EscrowAccountDto>>;
}
