
using Domain.Common.Response;
using MediatR;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Enum;

namespace PaymentService.Application.Features.RefundRequest.Commands.CompleteRefund
{
    public class CompleteRefundCommandHandler : IRequestHandler<CompleteRefundCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompleteRefundCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(CompleteRefundCommand request, CancellationToken token)
        {
            var refundReq = await _unitOfWork.RefundRequests.GetByIdAsync(request.RefundRequestId);
            if (refundReq == null) return Result.Failure(new Error("Refund.NotFound", "Refund request is not existing"));

            var refundTxn = new PaymentTransaction(
                refundReq.BookingId,
                refundReq.Amount,
                PaymentProvider.VNPay, 
                TransactionType.Refund, 
                request.RefundGatewayTransactionRef,
                $"Refund for request {refundReq.Id} - {request.Note}"
            );

            refundTxn.MarkAsManualSuccess(request.RefundGatewayTransactionRef);

            await _unitOfWork.PaymentTransactions.AddAsync(refundTxn);

            refundReq.MarkAsCompleted(refundTxn.Id, request.RefundGatewayTransactionRef, request.Note);

            await _unitOfWork.SaveChangesAsync(token);
            return Result.Success();
        }
    }
}
