using Domain.Common.Response;
using MediatR;
using PaymentService.Application.DTOs.Payment;

namespace PaymentService.Application.Features.Payment.Queries.GetTransactionById
{
    public record GetTransactionByIdQuery(Guid Id) : IRequest<Result<PaymentTransactionDto>>;
}
