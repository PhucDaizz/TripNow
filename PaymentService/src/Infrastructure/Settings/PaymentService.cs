using Microsoft.AspNetCore.Http;
using PaymentService.Application.Contracts;
using PaymentService.Application.DTOs.Payment;
using VNPAY;
using VNPAY.Models;
using VNPAY.Models.Enums;

namespace PaymentService.Infrastructure.Settings
{
    public class PaymentService : IPaymentService
    {
        private readonly IVnpayClient _vnpayClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PaymentService(IVnpayClient vnpayClient, IHttpContextAccessor httpContextAccessor)
        {
            _vnpayClient = vnpayClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<string> CreateVNPaymentLink(double moneyToPay, string description)
        {
            var request = new VnpayPaymentRequest
            {
                Money = moneyToPay,
                Description = description,
                BankCode = BankCode.ANY,
                Language = DisplayLanguage.Vietnamese
            };

            var paymentUrlInfor = _vnpayClient.CreatePaymentUrl(request);
            var paymentUrl = paymentUrlInfor.Url;

            return Task.FromResult(paymentUrl);
        }

        public PaymentCallbackResult HandleCallback(
            IReadOnlyDictionary<string, string> parameters)
        {
            var query = _httpContextAccessor.HttpContext!.Request.Query;

            var result = _vnpayClient.GetPaymentResult(query);

            return new PaymentCallbackResult
            {
                IsSuccess = true,
                TransactionId = result.VnpayTransactionId.ToString(),
                FailureReason = result.Description
            };
        }
    }
}
