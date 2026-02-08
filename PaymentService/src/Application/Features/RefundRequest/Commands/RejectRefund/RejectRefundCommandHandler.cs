using Domain.Common.Response;
using MediatR;
using PaymentService.Application.Common.Interfaces;

namespace PaymentService.Application.Features.RefundRequest.Commands.RejectRefund
{
    public class RejectRefundCommandHandler : IRequestHandler<RejectRefundCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RejectRefundCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(RejectRefundCommand request, CancellationToken cancellationToken)
        {
            var refundReq = await _unitOfWork.RefundRequests.GetByIdAsync(request.RefundRequestId);
            if (refundReq == null) return Result.Failure(new Error("Refund.NotFound", "Refund request is not existing"));

            refundReq.Reject(request.Reason);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
