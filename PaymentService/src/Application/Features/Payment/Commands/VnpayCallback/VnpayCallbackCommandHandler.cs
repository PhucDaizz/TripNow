using Domain.Common.Response;
using MediatR;
using Microsoft.Extensions.Logging;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Application.Contracts;
using PaymentService.Domain.Exceptions;

namespace PaymentService.Application.Features.Payment.Commands.VnpayCallback
{
    public class VnpayCallbackCommandHandler: IRequestHandler<VnpayCallbackCommand, Result>
    {
        private readonly IPaymentService _paymentService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<VnpayCallbackCommandHandler> _logger;
        private readonly IServiceFeeSettings _serviceFeeSettings;

        public VnpayCallbackCommandHandler(IPaymentService paymentService, 
            IUnitOfWork unitOfWork,
            ILogger<VnpayCallbackCommandHandler> logger,
            IServiceFeeSettings serviceFeeSettings)
        {
            _paymentService = paymentService;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _serviceFeeSettings = serviceFeeSettings;
        }

        public async Task<Result> Handle(
            VnpayCallbackCommand request,
            CancellationToken cancellationToken)
        {
            var callbackResult = _paymentService.HandleCallback(request.Parameters);
            var transaction = await _unitOfWork.PaymentTransactions.GetByMerchantRefAsync(callbackResult.MerchantRef, cancellationToken);
            if (transaction == null)
            {
                _logger.LogError("Transaction not found with Ref: {Ref}", callbackResult.MerchantRef);
                return Result.Failure(new Error("Payment.NotFound", "Transaction not found"));
            }
            if (callbackResult.IsSuccess)
            {
                try
                {
                    transaction.ProcessCallback(
                        callbackResult.ProviderTxnId,
                        callbackResult.RawResponse,
                        callbackResult.Amount * _serviceFeeSettings.Percentage / 100
                    );
                }
                catch (DomainException ex)
                {
                    _logger.LogWarning(ex.Message);
                    return Result.Success(); 
                }
            }
            else
            {
                transaction.MarkAsFailed(callbackResult.RawResponse, callbackResult.FailureReason);
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully updated status for TransactionId: {Id}", transaction.Id);
            return Result.Success();
        }
    }
}
