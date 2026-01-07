using BookingService.Domain.Common;
using BookingService.Domain.Enum;
using BookingService.Domain.Exceptions;

namespace BookingService.Domain.Entities
{
    public class Booking: BaseEntity, AggregateRoot
    {
        public Guid UserId { get; private set; }
        public Guid HotelId { get; private set; }
        public DateOnly CheckInDate { get; private set; }
        public DateOnly CheckOutDate { get; private set; }
        public BookingStatus Status { get; private set; }
        public PaymentStatus PaymentStatus { get; private set; }
        public BookingSource Source { get; private set; }

        public Guid? PromotionId { get; private set; }
        public string? PromotionCode { get; private set; }
        public decimal? DiscountAmount { get; private set; }

        public decimal TotalAmount { get; private set; }


        private readonly List<BookingItem> _items = new();
        public IReadOnlyCollection<BookingItem> Items => _items.AsReadOnly();

        private readonly List<BookingPriceDetail> _priceDetails = new();
        public IReadOnlyCollection<BookingPriceDetail> PriceDetails => _priceDetails.AsReadOnly();


        public virtual BookingPriceSnapshot? PriceSnapshot { get; private set; }
        public virtual BookingCancellation? Cancellation { get; private set; }


        private Booking() {}

        public Booking(Guid userId, Guid hotelId, DateOnly checkIn, DateOnly checkOut,
                       BookingSource source, Guid createdBy)
        {
            if (checkOut <= checkIn)
                throw new DomainException("Ngày Check-out phải sau ngày Check-in.");

            UserId = userId;
            HotelId = hotelId;
            CheckInDate = checkIn;
            CheckOutDate = checkOut;
            Source = source;
            CreatedBy = createdBy.ToString();

            Status = BookingStatus.Pending;
            PaymentStatus = PaymentStatus.Unpaid;
        }

        public void AddItem(Guid roomTypeId, int quantity, decimal unitPrice)
        {
            if (Status != BookingStatus.Pending)
                throw new DomainException("Chỉ được sửa phòng khi đơn đang chờ.");

            var item = new BookingItem(this.Id, roomTypeId, quantity, unitPrice);
            _items.Add(item);

            RecalculateTotal();
        }


        public void ApplyPromotion(Guid promotionId, string code, decimal discountAmount)
        {
            PromotionId = promotionId;
            PromotionCode = code;
            DiscountAmount = discountAmount;
            RecalculateTotal();
        }

        public void SetPriceSnapshot(BookingPriceSnapshot snapshot)
        {
            PriceSnapshot = snapshot;
            // Map snapshot details vào list PriceDetails để hiển thị
            // Logic map...
        }

        public void ConfirmPayment()
        {
            if (Status == BookingStatus.Cancelled)
                throw new DomainException("Không thể thanh toán đơn đã hủy.");

            PaymentStatus = PaymentStatus.Paid;
            Status = BookingStatus.Confirmed;

            // AddDomainEvent(new BookingPaidEvent(this.Id));
        }

        public void CompleteStay()
        {
            if (Status != BookingStatus.Confirmed) return;
            Status = BookingStatus.Completed;
            // AddDomainEvent(new BookingCompletedEvent(this.Id));
        }

        public void Cancel(CancelledBy who, string reason, RefundPolicy policy, decimal refundAmount)
        {
            if (Status == BookingStatus.Completed)
                throw new DomainException("Không thể hủy đơn đã hoàn tất.");

            Status = BookingStatus.Cancelled;

            // Nếu đã thanh toán thì cập nhật trạng thái tiền để Payment Service xử lý hoàn tiền
            if (PaymentStatus == PaymentStatus.Paid)
            {
                PaymentStatus = PaymentStatus.Refunded; // Hoặc PartialRefunded
            }

            Cancellation = new BookingCancellation(this.Id, who, reason, policy, refundAmount);
        }

        private void RecalculateTotal()
        {
            var itemsTotal = _items.Sum(x => x.Price * x.Quantity);
            if (!DiscountAmount.HasValue)
            {
                TotalAmount = itemsTotal - DiscountAmount.Value;
            }
            else
            {
                TotalAmount = itemsTotal;
            }
            if (TotalAmount < 0) TotalAmount = 0;
        }

    }
}
