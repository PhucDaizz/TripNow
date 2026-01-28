using Domain.Common.Response;

namespace HotelCatalogService.Domain.Errors
{
    public static class RoomErrors
    {
        public static readonly Error NotFound = new("Room.NotFound", "Không tìm thấy phòng.");
        public static readonly Error NotAvailable = new("Room.NotAvailable", "Phòng không sẵn sàng để check-in.");
        public static readonly Error Maintenance = new("Room.Maintenance", "Phòng đang bảo trì.");
    }
}
