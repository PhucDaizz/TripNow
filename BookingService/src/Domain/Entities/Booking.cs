using BookingService.Domain.Common;
using BookingService.Domain.Enum;
using BookingService.Domain.Events.Booking;
using BookingService.Domain.Exceptions;

namespace BookingService.Domain.Entities
{
    public class Booking: BaseEntity, AggregateRoot
    {
        public Guid UserId { get; private set; }
        public string CustomerName { get; private set; }
        public string CustomerEmail { get; private set; }
        public Guid HotelId { get; private set; }
        public string HotelName { get; private set; }
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

        public Booking(Guid userId, string customerName, string customerEmail, Guid hotelId, string hotelName, DateOnly checkIn, DateOnly checkOut,
                       BookingSource source, Guid createdBy)
        {
            if (checkOut <= checkIn)
                throw new DomainException("Ngày Check-out phải sau ngày Check-in.");

            UserId = userId;
            CustomerName = customerName;
            CustomerEmail = customerEmail;
            HotelId = hotelId;
            HotelName = hotelName;
            CheckInDate = checkIn;
            CheckOutDate = checkOut;
            Source = source;
            CreatedBy = createdBy.ToString();

            Status = BookingStatus.Pending;
            PaymentStatus = PaymentStatus.Unpaid;

            AddDomainEvent(new BookingCreatedDomainEvent(this));
        }

        public BookingItem AddItem(Guid roomTypeId, int quantity, decimal unitPrice)
        {
            if (Status != BookingStatus.Pending)
                throw new DomainException("Chỉ được sửa phòng khi đơn đang chờ.");

            var item = new BookingItem(this.Id, roomTypeId, quantity, unitPrice);
            _items.Add(item);

            RecalculatePriceStructure();
            return item;
        }


        public void ApplyPromotion(Guid promotionId, string code, decimal discountAmount)
        {
            if (Status != BookingStatus.Pending)
                throw new DomainException("Chỉ được áp dụng mã khi đơn đang chờ.");

            PromotionId = promotionId;
            PromotionCode = code;
            DiscountAmount = discountAmount;

            RecalculatePriceStructure();
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

            if (PaymentStatus == PaymentStatus.Paid)
            {
                if (refundAmount > 0)
                {
                    PaymentStatus = PaymentStatus.RefundPending;
                }
                else
                {
                    PaymentStatus = PaymentStatus.Paid;
                }
            }
            else if (PaymentStatus == PaymentStatus.Unpaid)
            {
                PaymentStatus = PaymentStatus.Cancelled;

                AddDomainEvent(new BookingCancelledWithOutPayDomainEvent
                {
                    BookingId = this.Id,
                    Reason = reason
                });
            }

            Cancellation = new BookingCancellation(this.Id, who, reason, policy, refundAmount);

            AddDomainEvent(new BookingCancelledDomainEvent
            {
                Fromdate = this.CheckInDate,
                ToDate = this.CheckOutDate,
                Items = this.Items.ToList()
            });

            if (refundAmount > 0)
            {
                AddDomainEvent(new BookingRefundInitiatedDomainEvent
                {
                    BookingId = this.Id,
                    Amount = refundAmount,
                    UserId = this.UserId
                });
            }
        }

        public void PaymentSusscess()
        {
            PaymentStatus = PaymentStatus.Paid;
            Status = BookingStatus.Confirmed;
        }

        public void AddPriceDetail(PriceType type, decimal amount, string description)
        {
            var detail = new BookingPriceDetail(this.Id, type, amount, description);
            _priceDetails.Add(detail);

            RecalculatePriceStructure();
        }

        private void RecalculatePriceStructure()
        {
            _priceDetails.Clear();

            // Tính số đêm lưu trú
            int nights = CheckOutDate.DayNumber - CheckInDate.DayNumber;
            if (nights <= 0) nights = 1;

            decimal totalRoomCharge = _items.Sum(i => i.Quantity * i.Price * nights);

            _priceDetails.Add(new BookingPriceDetail(
                Id,
                PriceType.RoomCharge,
                totalRoomCharge,
                $"Room charge ({nights} nights))"
            ));

            decimal actualDiscount = (DiscountAmount.HasValue && DiscountAmount.Value > totalRoomCharge)
                ? totalRoomCharge
                : (DiscountAmount ?? 0m);

            if (actualDiscount > 0)
            {
                _priceDetails.Add(new BookingPriceDetail(
                    Id,
                    PriceType.Promotion,
                    -actualDiscount, 
                    $"Discount voucher: {PromotionCode}"
                ));
            }

            decimal taxableAmount = totalRoomCharge - actualDiscount;

            decimal vatRate = 0.10m; 
            decimal vatAmount = taxableAmount * vatRate;


            _priceDetails.Add(new BookingPriceDetail(
                Id,
                PriceType.VAT,
                vatAmount,
                "VAT (10%)"
            ));

            TotalAmount = taxableAmount + vatAmount;
        }

        public decimal CalculateTotalRefundAmount(DateTime cancelTime)
        {
            decimal totalRefund = 0;
            foreach (var item in _items)
            {
                totalRefund += item.CalculateRefund(cancelTime, this.CheckInDate.ToDateTime(new TimeOnly(14, 0)));
            }
            return totalRefund;
        }

        public void CheckOutRoom(Guid roomId)
        {
            var assignment = Items.SelectMany(i => i.Assignments)
                                  .FirstOrDefault(a => a.RoomId == roomId);

            if (assignment == null) throw new DomainException("Phòng không thuộc đơn này.");

            assignment.CheckOut();

            var allAssignments = Items.SelectMany(i => i.Assignments);
            if (allAssignments.All(a => !a.IsCheckedIn))
            {
                Status = BookingStatus.Completed;

                AddDomainEvent(new BookingCompletedDomainEvent(this.Id, this.HotelId, this.TotalAmount));
            }
        }

        public void AddSurcharge(decimal amount, string description)
        {
            if (Status != BookingStatus.Pending) throw new DomainException("Cannot add surcharge confirmed booking");

            _priceDetails.Add(new BookingPriceDetail(this.Id, PriceType.ServiceFee, amount, description));

            // Update lại TotalAmount (Không gọi RecalculatePriceStructure vì sẽ bị clear mất)
            TotalAmount = _priceDetails.Sum(x => x.Amount);
        }

    }
}
