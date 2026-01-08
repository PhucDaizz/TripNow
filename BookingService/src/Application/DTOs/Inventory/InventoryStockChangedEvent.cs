using MediatR;

namespace BookingService.Application.DTOs.Inventory
{
    public class InventoryStockChangedEvent : INotification
    {
        public Guid RoomTypeId { get; set; }
        public int QuantityChange { get; set; }
    }
}
