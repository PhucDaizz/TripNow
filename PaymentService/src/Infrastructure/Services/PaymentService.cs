using Microsoft.AspNetCore.Http;
using PaymentService.Application.Contracts;
using PaymentService.Application.DTOs.Payment;
using VNPAY;
using VNPAY.Models;
using VNPAY.Models.Enums;

namespace PaymentService.Infrastructure.Services
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

        public Task<PaymentURLVNPayDetail> CreateVNPaymentLink(double moneyToPay, string description)
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

            return Task.FromResult(new PaymentURLVNPayDetail
            {
                PaymentUrl = paymentUrl,
                MerchantRefId = paymentUrlInfor.PaymentId.ToString()
            });

        }

        public PaymentCallbackResult HandleCallback(
            IReadOnlyDictionary<string, string> parameters)
        {
            var query = _httpContextAccessor.HttpContext!.Request.Query;

            var result = _vnpayClient.GetPaymentResult(query);

            var vnpResponseCode = parameters.GetValueOrDefault("vnp_ResponseCode");
            var vnpTxnRef = parameters.GetValueOrDefault("vnp_TxnRef");       
            var vnpTransactionNo = parameters.GetValueOrDefault("vnp_TransactionNo"); 
            var vnpAmount = parameters.GetValueOrDefault("vnp_Amount");

            bool isSuccess = vnpResponseCode == "00";

            return new PaymentCallbackResult
            {
                IsSuccess = isSuccess,
                MerchantRef = vnpTxnRef, 
                ProviderTxnId = vnpTransactionNo,
                RawResponse = System.Text.Json.JsonSerializer.Serialize(parameters), 
                FailureReason = isSuccess ? null : $"VnPay Error: {vnpResponseCode}",
                Amount = Convert.ToDecimal(vnpAmount) / 100
            };
        }
    }
}
