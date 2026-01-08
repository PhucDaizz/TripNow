namespace HotelCatalogService.Application.DTOs.Room
{
    public record InventoryStockChangedEvent
    {
        public Guid RoomTypeId { get; init; }
        public int QuantityChange { get; init; }

    }
}
