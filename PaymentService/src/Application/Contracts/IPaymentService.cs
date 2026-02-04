using PaymentService.Application.DTOs.Payment;

namespace PaymentService.Application.Contracts
{
    public interface IPaymentService
    {
        Task<string> CreateVNPaymentLink(double moneyToPay, string description);
        PaymentCallbackResult HandleCallback(IReadOnlyDictionary<string, string> parameters);
    }
}
