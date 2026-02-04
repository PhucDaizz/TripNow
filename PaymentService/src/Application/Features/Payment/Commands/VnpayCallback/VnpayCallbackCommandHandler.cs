using Domain.Common.Response;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using PaymentService.Application.Contracts;

namespace PaymentService.Application.Features.Payment.Commands.VnpayCallback
{
    public class VnpayCallbackCommandHandler: IRequestHandler<VnpayCallbackCommand, Result>
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<VnpayCallbackCommandHandler> _logger;

        public VnpayCallbackCommandHandler(IPaymentService paymentService, ILogger<VnpayCallbackCommandHandler> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        public async Task<Result> Handle(
            VnpayCallbackCommand request,
            CancellationToken cancellationToken)
        {
            var result = _paymentService.HandleCallback(request.Parameters);

            if (!result.IsSuccess)
            {
                _logger.LogError("Vnpay callback failed: {Reason}", result.FailureReason);
                return Result.Failure(new Error("" ,result.FailureReason));
            }

            _logger.LogInformation("Vnpay callback succeeded for transaction {TransactionId}", result.TransactionId);
            return Result.Success();
        }
    }
}
