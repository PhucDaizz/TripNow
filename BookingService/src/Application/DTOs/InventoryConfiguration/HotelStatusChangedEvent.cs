using MediatR;

namespace BookingService.Application.DTOs.InventoryConfiguration
{
    public class HotelStatusChangedEvent: INotification
    {
        public Guid HotelId { get; set; }
        public bool IsClosed { get; set; } 
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
    }
}
