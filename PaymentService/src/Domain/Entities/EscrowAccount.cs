using PaymentService.Domain.Common;
using PaymentService.Domain.Enum;
using PaymentService.Domain.Events.EscrowAccount;
using PaymentService.Domain.Exceptions;

namespace PaymentService.Domain.Entities
{
    public class EscrowAccount: BaseEntity, AggregateRoot
    {
        public Guid BookingId { get; private set; }
        public decimal Amount { get; private set; }
        public EscrowStatus Status { get; private set; }
        public decimal ProviderFee { get; private set; }
        public decimal RefundedAmount { get; private set; }

        private EscrowAccount() { }

        public EscrowAccount(Guid bookingId, decimal amount, decimal providerFee)
        {
            if (bookingId == Guid.Empty)
                throw new DomainException("BookingId không được để trống khi tạo tài khoản giữ tiền.");

            if (amount <= 0)
                throw new DomainException("Số tiền giữ (Escrow Amount) phải lớn hơn 0.");

            if (providerFee < 0)
                throw new DomainException("Phí giao dịch (Provider Fee) không được là số âm.");

            if (providerFee >= amount)
                throw new DomainException("Phí giao dịch không được lớn hơn hoặc bằng tổng số tiền.");

            BookingId = bookingId;
            Amount = amount;
            ProviderFee = providerFee;
            Status = EscrowStatus.Holding;
            RefundedAmount = 0;

            AddDomainEvent(new EscrowCreatedEvent(this.BookingId));
        }

        public void Release()
        {
            if (Status == EscrowStatus.Refunded) return; 
            if (Status == EscrowStatus.Released) return;

            if (Status != EscrowStatus.Holding)
                throw new DomainException("Chỉ có thể giải phóng tiền (Release) khi đang ở trạng thái Giữ (Holding).");

            Status = EscrowStatus.Released;
        }

        public void Refund(decimal refundAmount)
        {
            if (Status != EscrowStatus.Holding)
                throw new DomainException("Chỉ có thể hoàn tiền khi đang ở trạng thái Giữ (Holding).");

            if (refundAmount <= 0)
                throw new DomainException("Số tiền hoàn phải lớn hơn 0.");

            if (refundAmount > Amount)
                throw new DomainException("Không thể hoàn số tiền lớn hơn số tiền gốc.");

            RefundedAmount = refundAmount;

            decimal remainingRevenue = Amount - RefundedAmount;

            if (remainingRevenue == 0)
            {
                ProviderFee = 0;
                Status = EscrowStatus.Refunded;
            }
            else
            {
                decimal remainingRatio = remainingRevenue / Amount;
                ProviderFee = Math.Round(ProviderFee * remainingRatio, 2); 

                Status = EscrowStatus.PartiallyRefunded;
            }
        }

        public SettlementCalculationResult CalculateSettlement(decimal commissionRate)
        {
            decimal actualRevenue = Amount - RefundedAmount;

            if (actualRevenue <= 0)
                return new SettlementCalculationResult(0, 0, 0);

            decimal actualFee = actualRevenue * (commissionRate / 100);
            decimal netAmount = actualRevenue - actualFee;

            return new SettlementCalculationResult(netAmount, actualRevenue, actualFee);
        }

        public record SettlementCalculationResult(decimal NetAmount, decimal GrossAmount, decimal Fee);
    }
}
