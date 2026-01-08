using BookingService.Domain.Common;
using BookingService.Domain.Exceptions;

namespace BookingService.Domain.Entities
{
    public class Inventory : BaseEntity, AggregateRoot
    {

        public Guid RoomTypeId { get; private set; }
        public DateOnly Date { get; private set; }
        public int TotalStock { get; private set; }
        public int SoldStock { get; private set; }
        public byte[] RowVersion { get; private set; }
        public int AvailableStock => TotalStock - SoldStock;
        private Inventory() { }

        private Inventory(Guid roomTypeId, DateOnly date, int totalStock)
        {
            RoomTypeId = roomTypeId;
            Date = date;
            TotalStock = totalStock;
            SoldStock = 0;
            RowVersion = new byte[8];
        }

        public static Inventory Create(Guid roomTypeId, DateOnly date, int totalStock)
        {
            if (totalStock <= 0)
                throw new DomainException("TotalStock phải lớn hơn 0.");
            return new Inventory(roomTypeId, date, totalStock);
        }

        /// <summary>
        /// Giữ phòng (Tăng SoldStock)
        /// </summary>
        public void ReserveStock(int quantity)
        {
            if (quantity <= 0)
                throw new DomainException("Số lượng đặt phải lớn hơn 0.");

            if (AvailableStock < quantity)
                throw new DomainException($"Hết phòng cho ngày {Date}. Chỉ còn {AvailableStock}.");

            SoldStock += quantity;
        }

        /// <summary>
        /// Nhả phòng (Giảm SoldStock - Khi hủy đơn)
        /// </summary>
        public void ReleaseStock(int quantity)
        {
            if (quantity <= 0) return;

            SoldStock -= quantity;

            // Safety check: Không bao giờ để sold bị âm
            if (SoldStock < 0) SoldStock = 0;
        }

        /// <summary>
        /// Điều chỉnh tổng quỹ phòng (VD: Khách sạn xây thêm phòng hoặc sửa chữa)
        /// </summary>
        public void AdjustTotalStock(int newTotal)
        {
            if (newTotal < SoldStock)
                throw new DomainException("Không thể giảm tổng quỹ phòng thấp hơn số lượng đã bán.");

            TotalStock = newTotal;
        }

    }
}
