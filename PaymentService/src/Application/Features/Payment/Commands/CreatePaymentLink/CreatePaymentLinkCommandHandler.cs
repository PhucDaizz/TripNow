using Domain.Common.Response;
using MediatR;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Application.Contracts;
using PaymentService.Application.DTOs.Payment;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Enum;

namespace PaymentService.Application.Features.Payment.Commands.CreatePaymentLink
{
    public class CreatePaymentLinkCommandHandler : IRequestHandler<CreatePaymentLinkCommand, Result<string>>
    {
        private readonly IPaymentService _paymentService;
        private readonly IUnitOfWork _unitOfWork;

        public CreatePaymentLinkCommandHandler(IPaymentService paymentService, IUnitOfWork unitOfWork)
        {
            _paymentService = paymentService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> Handle(CreatePaymentLinkCommand request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(request.BookingId, out var bookingGuid))
            {
                return Result.Failure<string>(new Error("BookingId.Invalid", "Invalid BookingId."));
            }

            var description = $"Thanh toan Booking {request.BookingId}";

            var paymentLink = new PaymentURLVNPayDetail();

            var existingTxn = await _unitOfWork.PaymentTransactions.GetByIdWithStatusAsync(Guid.Parse(request.BookingId), PaymentTransactionStatus.Pending, cancellationToken);

            if (existingTxn != null)
            {
                if (existingTxn.CreatedAt.AddMinutes(15) > DateTime.UtcNow)
                {
                    var newLinkResult = await GetPaymentUrlAsync(request, description);

                    existingTxn.SetMerchantRef(newLinkResult.MerchantRefId);
                    existingTxn.UpdatePaymentDate();

                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    return Result.Success<string>(newLinkResult.PaymentUrl); 
                }
                else
                {
                    existingTxn.MarkAsFailed("Expired", "User requested new link");
                }
            }

            var linkResult = await GetPaymentUrlAsync(request, description);
            var newTxn = new PaymentTransaction(
                Guid.Parse(request.BookingId),
                (decimal)request.MoneyToPay,
                request.providerBank,
                linkResult.MerchantRefId,
                request.PayerUserId
            );

            await _unitOfWork.PaymentTransactions.AddAsync(newTxn);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success<string>(linkResult.PaymentUrl);
        }


        private async Task<PaymentURLVNPayDetail> GetPaymentUrlAsync(CreatePaymentLinkCommand request, string description)
        {
            var paymentLink = new PaymentURLVNPayDetail();
            switch (request.providerBank)
            {
                case PaymentProvider.VNPay:
                    paymentLink = await _paymentService.CreateVNPaymentLink(request.MoneyToPay, description);
                    break;
                default:
                    paymentLink = await _paymentService.CreateVNPaymentLink(request.MoneyToPay, description);
                    break;
            }
            return paymentLink;
        }
    }
}
