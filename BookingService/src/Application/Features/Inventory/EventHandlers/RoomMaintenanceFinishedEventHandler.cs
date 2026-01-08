using BookingService.Application.Common.Interfaces;
using BookingService.Application.DTOs.Inventory;
using MediatR;

namespace BookingService.Application.Features.Inventory.EventHandlers
{
    public class RoomMaintenanceFinishedEventHandler : INotificationHandler<RoomMaintenanceFinishedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoomMaintenanceFinishedEventHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(RoomMaintenanceFinishedEvent notification, CancellationToken cancellationToken)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            // 1. Xác định ngày bắt đầu cần mở khóa (Release)
            // Nếu lịch bảo trì nằm hoàn toàn trong tương lai (VD: Lên lịch tuần sau nhưng nay hủy):
            // -> Bắt đầu mở từ ngày OriginalStart.
            // Nếu đang bảo trì dở dang (VD: Lịch đến ngày 15, nay 12 đã xong):
            // -> Bắt đầu mở từ ngày Today (12).

            DateOnly releaseFromDate = notification.FromDate > today
                ? notification.FromDate
                : today;

            // 2. Kiểm tra tính hợp lệ
            // Nếu ngày bắt đầu mở lại nằm SAU ngày kết thúc dự kiến
            // => Nghĩa là đã hết hạn bảo trì từ lâu, hoặc xong đúng hạn -> Không có ngày dư để mở.
            if (releaseFromDate > notification.ToDate)
            {
                return;
            }

            // 3. Tạo danh sách các ngày cần Update
            var datesToRelease = new List<DateOnly>();

            // Loop từ ngày bắt đầu mở -> đến ngày kết thúc dự kiến cũ
            for (var date = releaseFromDate; date <= notification.ToDate; date = date.AddDays(1))
            {
                datesToRelease.Add(date);
            }

            // 4. Gọi Repository để trừ BlockedStock
            if (datesToRelease.Any())
            {
                // Gọi hàm Bulk Update (Đã viết ở các bước trước)
                // Tham số -1: Nghĩa là giảm BlockedStock đi 1 đơn vị (Nhả phòng)
                await _unitOfWork.Inventory.UpdateBlockedStockBulkAsync(
                    notification.RoomTypeId,
                    datesToRelease,
                    -1,
                    cancellationToken
                );
            }
        }
    }
}
