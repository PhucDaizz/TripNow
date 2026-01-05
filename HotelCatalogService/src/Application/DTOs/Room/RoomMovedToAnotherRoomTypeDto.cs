namespace HotelCatalogService.Application.DTOs.Room
{
    public record RoomMovedToAnotherRoomTypeDto
    {
        public Guid OldRoomType { get; init; }
        public Guid NewRoomType { get; init; }
    }
}
