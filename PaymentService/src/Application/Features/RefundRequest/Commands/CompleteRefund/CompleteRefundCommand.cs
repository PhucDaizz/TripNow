using Domain.Common.Response;
using MediatR;

namespace PaymentService.Application.Features.RefundRequest.Commands.CompleteRefund
{
    public class CompleteRefundCommand : IRequest<Result>
    {
        public Guid RefundRequestId { get; set; }
        public string RefundGatewayTransactionRef { get; set; } 
        public string? Note { get; set; } // "Đã chuyển khoản xong qua VCB"
    }
}
