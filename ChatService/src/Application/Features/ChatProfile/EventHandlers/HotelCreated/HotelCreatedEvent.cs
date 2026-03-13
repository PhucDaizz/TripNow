using MediatR;

namespace ChatService.Application.Features.ChatProfile.EventHandlers.HotelCreated
{
    public class HotelCreatedEvent : INotification
    {
        public Guid HotelId { get; init; }
        public required string Name { get; init; }
    }
}
