using Domain.Common.Response;
using MediatR;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Application.DTOs.Payment;

namespace PaymentService.Application.Features.Payment.Queries.GetTransactionById
{
    public class GetTransactionByIdQueryHandler : IRequestHandler<GetTransactionByIdQuery, Result<PaymentTransactionDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetTransactionByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PaymentTransactionDto>> Handle(GetTransactionByIdQuery request, CancellationToken token)
        {
            var txn = await _unitOfWork.PaymentTransactions.GetByIdAsync(request.Id);
            if (txn == null) return Result.Failure<PaymentTransactionDto>(new Error("Txn.NotFound", "Cannot found this transaction"));

            var dto = new PaymentTransactionDto
            {
                Id = txn.Id,
                BookingId = txn.BookingId,
                Amount = txn.Amount,
                Provider = txn.Provider.ToString(),
                Type = txn.Type.ToString(),
                TransactionStatus = txn.TransactionStatus.ToString(),
                MerchantRef = txn.MerchantRef,
                ProviderTxnId = txn.ProviderTxnId,
                PaymentDate = txn.PaymentDate,
                FailureReason = txn.FailureReason, 
                Note = txn.Note,
            };

            return Result.Success(dto);
        }
    }
}
