using Domain.Common.Response;
using MediatR;

namespace PaymentService.Application.Features.Payment.Commands.VnpayCallback
{
    public record VnpayCallbackCommand(IReadOnlyDictionary<string, string> Parameters) : IRequest<Result>;
}
