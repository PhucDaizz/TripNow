using PaymentService.Domain.Common;
using PaymentService.Domain.Enum;
using PaymentService.Domain.Exceptions;

namespace PaymentService.Domain.Entities
{
    public class RefundRequest : BaseEntity, AggregateRoot
    {
        public Guid BookingId { get; private set; }
        public Guid UserId { get; private set; } // Người yêu cầu (Khách hàng)

        public decimal Amount { get; private set; }

        public Guid OriginalPaymentTransactionId { get; private set; } // FK tới PaymentTransaction
        public string OriginalGatewayTransactionRef { get; private set; } // Mã GD Ngân hàng/VNPAY (để Admin tra soát)

        public Guid? RefundPaymentTransactionId { get; private set; } // FK tới PaymentTransaction (Giao dịch hoàn tiền mới tạo)
        public string? RefundGatewayTransactionRef { get; private set; } // Mã GD hoàn tiền từ Ngân hàng cấp

        public RefundStatus Status { get; private set; }
        public string Reason { get; private set; } // Lý do hoàn tiền (VD: Khách hủy phòng)
        public string? AdminNote { get; private set; } // Ghi chú của Admin (VD: Đã chuyển khoản qua VCB)
        public DateTime? ProcessedAt { get; private set; } // Thời điểm xử lý xong

        private RefundRequest() { }

        public RefundRequest(
            Guid bookingId,
            Guid userId,
            decimal amount,
            Guid originalPaymentTransactionId,
            string originalGatewayRef,
            string reason)
        {
            if (amount <= 0) throw new DomainException("Số tiền hoàn phải lớn hơn 0.");
            if (bookingId == Guid.Empty) throw new DomainException("BookingId không hợp lệ.");
            if (originalPaymentTransactionId == Guid.Empty) throw new DomainException("Phải có giao dịch gốc.");

            BookingId = bookingId;
            UserId = userId;
            Amount = amount;
            OriginalPaymentTransactionId = originalPaymentTransactionId;
            OriginalGatewayTransactionRef = originalGatewayRef;
            Reason = reason;
            Status = RefundStatus.Pending;

            // Bắn event: Đã có yêu cầu hoàn tiền mới (để báo Admin)
            // AddDomainEvent(new RefundRequestCreatedEvent(this.Id)); 
        }

        // Hàm 1: Xác nhận hoàn thành (Admin đã chuyển tiền thủ công/API)
        public void MarkAsCompleted(Guid refundPaymentTransactionId, string refundGatewayRef, string? note)
        {
            if (Status != RefundStatus.Pending)
                throw new DomainException("Chỉ có thể xử lý yêu cầu đang chờ (Pending).");

            Status = RefundStatus.Completed;
            RefundPaymentTransactionId = refundPaymentTransactionId; // Link tới Transaction mới tạo
            RefundGatewayTransactionRef = refundGatewayRef;          // Mã ngân hàng cấp
            AdminNote = note;
            ProcessedAt = DateTime.UtcNow;

            // Bắn event: Hoàn tiền thành công (để gửi mail cho khách)
            // AddDomainEvent(new RefundRequestCompletedEvent(this.Id, this.UserId, this.Amount));
        }

        // Hàm 2: Từ chối hoàn tiền (Nếu phát hiện gian lận hoặc sai sót)
        public void Reject(string reason)
        {
            if (Status != RefundStatus.Pending)
                throw new DomainException("Chỉ có thể từ chối yêu cầu đang chờ (Pending).");

            Status = RefundStatus.Rejected;
            AdminNote = reason;
            ProcessedAt = DateTime.UtcNow;

            // Bắn event: Hoàn tiền bị từ chối
            // AddDomainEvent(new RefundRequestRejectedEvent(this.Id, this.UserId));
        }
    }
}