using PaymentService.Domain.Common;
using PaymentService.Domain.Enum;
using PaymentService.Domain.Exceptions;

namespace PaymentService.Domain.Entities
{
    public class SettlementItem: BaseEntity
    {
        public Guid SettlementId { get; private set; }
        public Guid BookingId { get; private set; }
        public decimal GrossAmount { get; private set; } // doanh thu gốc
        public decimal CommissionAmount { get; private set; } // phí hoa hồng
        public decimal NetAmount { get; private set; } // Số tiền thực nhận (Net = Gross - Commission). doanh thu sau phí
        public SettlementItemType Type { get; private set; }

        private SettlementItem() { }

        internal SettlementItem(Guid settlementId, Guid bookingId, decimal grossAmount, decimal commissionAmount, SettlementItemType type)
        {
            if (settlementId == Guid.Empty) throw new DomainException("SettlementId không được để trống.");
            SettlementId = settlementId;
            BookingId = bookingId;
            GrossAmount = grossAmount;
            CommissionAmount = commissionAmount;
            Type = type;
            NetAmount = grossAmount - commissionAmount;
        }
    }
}
