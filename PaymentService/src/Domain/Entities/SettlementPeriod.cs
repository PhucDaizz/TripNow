using PaymentService.Domain.Common;
using PaymentService.Domain.Enum;
using PaymentService.Domain.Exceptions;

namespace PaymentService.Domain.Entities
{
    public class SettlementPeriod: BaseEntity, AggregateRoot
    {
        public Guid OwnerId { get; private set; }
        public decimal TotalGross { get; private set; }
        public decimal TotalCommission { get; private set; }
        public decimal TotalNetPayable { get; private set; }
        public PeriodStatus Status { get; private set; }
        public DateTime PeriodFrom { get; private set; }
        public DateTime PeriodTo { get; private set; }

        private readonly List<SettlementItem> _settlementItems = new();
        public IReadOnlyCollection<SettlementItem> SettlementItems => _settlementItems.AsReadOnly();

        private SettlementPeriod() { }

        public SettlementPeriod(Guid ownerId, DateTime from, DateTime to)
        {
            if (from >= to) throw new DomainException("Ngày bắt đầu kỳ đối soát phải nhỏ hơn ngày kết thúc.");

            OwnerId = ownerId;
            PeriodFrom = from;
            PeriodTo = to;
            TotalGross = 0;
            TotalCommission = 0;
            TotalNetPayable = 0;
            Status = PeriodStatus.Processing; 
        }

        public void AddSettlementItem(Guid bookingId, decimal grossAmount, decimal commissionAmount, SettlementItemType type)
        {
            if (Status != PeriodStatus.Processing)
            {
                throw new DomainException("Không thể thêm dữ liệu vào kỳ đối soát đã chốt hoặc đã thanh toán.");
            }

            // Tạo Item con
            // Lưu ý: SettlementItem nên tự tính NetAmount trong constructor của nó
            var item = new SettlementItem(this.Id, bookingId, grossAmount, commissionAmount, type);

            _settlementItems.Add(item);

            TotalGross += item.GrossAmount;
            TotalCommission += item.CommissionAmount;
            TotalNetPayable += item.NetAmount;
        }

        public void FinalizePeriod()
        {
            if (Status != PeriodStatus.Processing) return;

            if (_settlementItems.Count == 0)
            {
                Status = PeriodStatus.Paid;
                return;
            }

            Status = PeriodStatus.Open; 
        }
        public void MarkAsOpen()
        {
            Status = PeriodStatus.Open;
        }

        /*public void RequestPayout(string bankInfo)
        {
            if (Status != PeriodStatus.Open)
            {
                throw new DomainException("Chỉ có thể rút tiền cho các kỳ đối soát đang ở trạng thái Open.");
            }

            if (TotalNetPayable <= 0)
            {
                throw new DomainException("Số tiền thực nhận <= 0, không thể tạo lệnh rút tiền.");
            }

            var payout = new Payout(this.Id, this.TotalNetPayable, bankInfo);

            _payouts.Add(payout);

            Status = PeriodStatus.Locked;
        }

        public void HandlePayoutResult(Guid payoutId, bool isSuccess)
        {
            var payout = _payouts.FirstOrDefault(p => p.Id == payoutId);
            if (payout == null) return;

            if (isSuccess)
            {
                Status = PeriodStatus.Paid;
            }
            else
            {
                Status = PeriodStatus.Open;
            }
        }*/
    }
}
