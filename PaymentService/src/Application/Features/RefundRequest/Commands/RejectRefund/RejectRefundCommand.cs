using Domain.Common.Response;
using MediatR;

namespace PaymentService.Application.Features.RefundRequest.Commands.RejectRefund
{
    public class RejectRefundCommand : IRequest<Result>
    {
        public Guid RefundRequestId { get; set; }
        public string Reason { get; set; }
    }
}
