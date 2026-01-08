namespace HotelCatalogService.Application.DTOs.Room
{
    public class RoomMaintenanceFinishedDto
    {
        public Guid RoomTypeId { get; set; }
        public DateOnly FromDate { get; set; }
        public DateOnly ToDate { get; set; }
    }
}
